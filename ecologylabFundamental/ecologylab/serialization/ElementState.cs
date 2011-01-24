using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;

namespace ecologylab.serialization
{
    public enum Format
    {
        XML, JSON
    }

    /// <summary>
    ///     This class is the heart of the <code>ecologylab.xml</code>
    ///     translation framework.
    ///
    ///     <p/>
    ///     To use the framework, the programmer must define a tree of objects derived
    ///     from this class. The public fields in each of these derived objects 
    ///     correspond to the XML DOM. The declarations of attribute fields  must 
    ///     preceed thos for nested XML elements. Attributes are built directly from
    ///     Strings, using classes derived from <code>ScalarType</code>    
    ///
    ///     <p/>
    ///     The framework proceeds automatically through the application of rules.
    ///     In the standard case, the rules are based on the automatic mapping of
    ///     XML element names (aka tags), to ElementState class names.
    ///     An mechanism for supplying additional translations may also be provided.
    ///
    ///     <p/>
    ///     <code>ElementState</code> is based on 2 methods, each of which employs 
    ///     .NET reflection and recursive descent.
    ///
    ///     <li><code>translateToXML(...)</code> translates a tree of these 
    ///     <code>ElementState</code> objects into XML.</li>
    ///
    ///    <li><code>translateFromXML(...)</code> translates an XML DOM into a tree of these
    ///    <code>ElementState</code> objects</li>
    /// </summary>
    public class ElementState : FieldTypes
    {
        #region Private & Public Fields


        /// <summary>
        /// 
        /// </summary>
        public static int CDATA = 1;

        /// <summary>
        /// 
        /// </summary>
        public static int NORMAL = 0;        

        /// <summary>
        /// 
        /// </summary>
        private ClassDescriptor elementClassDescriptor = null;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<String, ElementState> elementById;

        /// <summary>
        /// 
        /// </summary>
        private ElementState parent;

        public ElementState Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        #endregion

        #region Translation To functions

        public void serialize(StringBuilder output, Format format)
        {
            switch (format)
            {
                case Format.XML  : 
                    serializeToXML(output);
                    break;
                case Format.JSON : serializeToJSON(output);
                    break;
            }
        }

        /// <summary>
        ///     Translates to XML file representation of the object. 
        ///     Uses <c>ClassDescriptors</c> and <c>FieldDescriptors</c> to 
        ///     marshall the object to its XML representation. 
        /// </summary>
        /// <param name="output">
        ///     The output buffer which contains the marshalled representation of the
        ///     run-time object.
        /// </param>
        public void serializeToXML(StringBuilder output)
        {
            if (output == null) throw new Exception("null : output object");
            else serializeToXML(this.ElementClassDescriptor.PseudoFieldDescriptor, output);
        }

        /// <summary>
        ///     Internal recursive function to marshall the <c>ElementState</c> object to its
        ///     XML representation.
        /// </summary>
        /// <param name="fieldDescriptor"></param>
        /// <param name="output"></param>
        private void serializeToXML(FieldDescriptor fieldDescriptor, StringBuilder output)
        {
            this.preTranslationProcessingHook();

            fieldDescriptor.WriteElementStart(output);

            List<FieldDescriptor> attributeFieldDescriptors = this.ElementClassDescriptor.AttributeFieldDescriptors;
            int numAttributes = attributeFieldDescriptors.Count;

            if (numAttributes > 0)
            {
                try
                {
                    for (int i = 0; i < numAttributes; i++)
                    {
                        FieldDescriptor childFD = attributeFieldDescriptors[i];
                        childFD.AppendValueAsAttribute(output, this);
                    }
                }
                catch (Exception e)
                {
                    //TODO : implement exception class
                    throw new Exception("TranslateToXML for attribute " + this, e);
                }
            }

            List<FieldDescriptor> elementFieldDescriptors = ElementClassDescriptor.ElementFieldDescriptors;
            int numElements = elementFieldDescriptors.Count;

            //FIXME -- get rid of old textNode stuff. it doesnt even work
            StringBuilder textNode = this.textNodeBuffy;
            Boolean hasXmlText = fieldDescriptor.HasXmlText;
            if ((numElements == 0) && (textNode == null) && !hasXmlText)
            {
                output.Append('/').Append('>');
            }
            else
            {
                if (!fieldDescriptor.IsXmlNsDecl)
                    output.Append('>');


                if (hasXmlText)
                {
                    try
                    {
                        fieldDescriptor.AppendXmlText(output, this);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("TranslateToXML for @xml_field " + this, e);
                    }
                }

                for (int i = 0; i < numElements; i++)
                {
                    FieldDescriptor childFD = elementFieldDescriptors[i];
                    int childFdType = childFD.GetTypeId();
                    if (childFdType == SCALAR)
                    {
                        try
                        {
                            childFD.AppendLeaf(output, this);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("TranslateToXML for leaf node " + this, e);
                        }
                    }
                    else
                    {
                        Object thatReferenceObject = null;
                        FieldInfo childField = childFD.Field;
                        try
                        {
                            thatReferenceObject = childField.GetValue(this);
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine("WARNING re-trying access! " + e.ToString());

                            try
                            {
                                thatReferenceObject = childField.GetValue(this);
                            }
                            catch (Exception e1)
                            {
                                Console.WriteLine(e1.ToString());
                                System.Console.WriteLine("Can't access " + childField.Name);
                            }
                        }

                        if (thatReferenceObject == null)
                            continue;

                        Boolean isScalar = (childFdType == COLLECTION_SCALAR) || (childFdType == MAP_SCALAR);

                        ICollection thatCollection;
                        switch (childFdType)
                        {
                            case COLLECTION_ELEMENT:
                            case COLLECTION_SCALAR:
                            case MAP_ELEMENT:
                            case MAP_SCALAR:
                                thatCollection = XMLTools.GetCollection(thatReferenceObject);
                                break;
                            default:
                                thatCollection = null;
                                break;
                        }

                        if (thatCollection != null && (thatCollection.Count > 0))
                        {
                            if (childFD.IsWrapped)
                                childFD.WriteWrap(output, false);
                            foreach (Object next in thatCollection)
                            {
                                if (isScalar)
                                {
                                    try
                                    {
                                        childFD.AppendCollectionLeaf(output, next);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception("TranslateToXML for collection leaf " + this, e);
                                    }
                                }
                                else if (next is ElementState)
                                {
                                    ElementState collectionSubElementState = (ElementState)next;
                                    // Type collectionElementClass = collectionSubElementState.GetClass();

                                    FieldDescriptor collectionElementFD = childFD.IsPolymorphic ?
                                            collectionSubElementState.ElementClassDescriptor.PseudoFieldDescriptor :
                                            childFD;

                                    collectionSubElementState.serializeToXML(collectionElementFD, output);
                                }
                                else
                                    throw new Exception("thrown");
                            }
                            if (childFD.IsWrapped)
                                childFD.WriteWrap(output, true);
                        }
                        else if (thatReferenceObject is ElementState)
                        {
                            ElementState nestedES = (ElementState)thatReferenceObject;
                            FieldDescriptor nestedF2XO = childFD.IsPolymorphic ?
                                    nestedES.ElementClassDescriptor.PseudoFieldDescriptor : childFD;

                            nestedES.serializeToXML(nestedF2XO, output);
                        }
                    }
                }

                fieldDescriptor.WriteCloseTag(output);
            }
        }


        /// <summary>
        ///     Translates to XML file representation of the object. 
        ///     Uses <c>ClassDescriptors</c> and <c>FieldDescriptors</c> to 
        ///     marshall the object to its XML representation. 
        /// </summary>
        /// <param name="output">
        ///     The output buffer which contains the marshalled representation of the
        ///     run-time object.
        /// </param>
        public void serializeToJSON(StringBuilder output)
        {
            if (output == null) throw new Exception("null : output object");
            else serializeToJSON(this.ElementClassDescriptor.PseudoFieldDescriptor, output);
        }
        private void serializeToJSON(FieldDescriptor fieldDescriptor, StringBuilder output)
        {
            output.Append('{');
            serializeToJSONRecursive(fieldDescriptor, output, true);
            output.Append('}');
        }
        private void serializeToJSONRecursive(FieldDescriptor fieldDescriptor, StringBuilder output, bool withTag)
        {
            fieldDescriptor.WriteJSONElementStart(output, withTag);

            List<FieldDescriptor> elementFieldDescriptors = ElementClassDescriptor.ElementFieldDescriptors;
            List<FieldDescriptor> attributeFieldDescriptors = ElementClassDescriptor.AttributeFieldDescriptors;

            int numAttributes = attributeFieldDescriptors.Count;
            int numElements = elementFieldDescriptors.Count;

            bool attributesSerialized = false;

            if (numAttributes > 0)
            {
                try
                {
                    for (int i = 0; i < numAttributes; i++)
                    {
                        // iterate through fields
                        FieldDescriptor childFD = attributeFieldDescriptors[i];
                        bool isDefaultValue = childFD.IsDefaultValue(this);
                        if (!isDefaultValue)
                        {
                            childFD.AppendValueAsJSONAttribute(output, this, !attributesSerialized);
                            if (!attributesSerialized)
                            {
                                attributesSerialized = true;
                            }
                        }

                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw (ex);
                }
            }

            bool elementsSerialized = false;
		for (int i = 0; i < numElements; i++)
		{
			FieldDescriptor childFD = elementFieldDescriptors[i];
			int childFdType = childFD.Type;
			if (childFdType == SCALAR)
			{
				try
				{
					bool isDefaultValue = childFD.IsDefaultValue(this);
					if (!isDefaultValue)
					{
						childFD.AppendValueAsJSONAttribute(output, this, !elementsSerialized);
						if (!elementsSerialized)
						{
							elementsSerialized = true;
						}
					}
				}
				catch (Exception e)
				{
                    Console.WriteLine(e.ToString());
					throw e;
				}
			}
			else
			{
				// if (attributesSerialized || i > 0)
				// appendable.append(", ");

				Object thatReferenceObject = null;
				FieldInfo childField = childFD.Field;
                    
				try
				{
					thatReferenceObject = childField.GetValue(this);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
				// ignore null reference objects
				if (thatReferenceObject == null)
					continue;

				bool isScalar = (childFdType == COLLECTION_SCALAR) || (childFdType == MAP_SCALAR);
				// gets Collection object directly or through Map.values()
				ICollection thatCollection;
				switch (childFdType)
				{
				case COLLECTION_ELEMENT:
				case COLLECTION_SCALAR:
				case MAP_ELEMENT:
				case MAP_SCALAR:
					thatCollection = XMLTools.GetCollection(thatReferenceObject);
					break;
				default:
					thatCollection = null;
					break;
				}

				if (thatCollection != null && (thatCollection.Count > 0))
				{
					if (attributesSerialized || elementsSerialized)
						output.Append(", ");

					elementsSerialized = true;

					if (!childFD.IsPolymorphic)
					{
						if (childFD.IsWrapped)
							childFD.WriteJSONWrap(output, false);

						childFD.WriteJSONCollectionStart(output);

                        int j = 0;
                        foreach(Object next in thatCollection)
                        {
                            if (isScalar) // leaf node!
                            {
                                try
                                {
                                    childFD.AppendJSONCollectionAttribute(output, next, j == 0);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                    throw e;
                                }
                            }
                            else if (next is ElementState && !childFD.IsPolymorphic)
                            {
                                if (j != 0)
                                    output.Append(',');

                                ElementState collectionSubElementState = (ElementState)next;
                                collectionSubElementState.serializeToJSONRecursive(childFD, output, false);
                            }
                            j++;
                        }

						childFD.WriteJSONCollectionClose(output);

						if (childFD.IsWrapped)
							childFD.WriteJSONWrap(output, true);
					}
					else
					{
						childFD.WriteJSONPolymorphicCollectionStart(output);

                        int j = 0;
                        foreach (Object next in thatCollection)
                        {
                            if (j != 0)
                                output.Append(',');

                            ElementState collectionSubElementState = (ElementState)next;

                            FieldDescriptor collectionElementFD = collectionSubElementState.ElementClassDescriptor
                                    .PseudoFieldDescriptor;

                            output.Append('{');
                            collectionSubElementState.serializeToJSONRecursive(collectionElementFD, output,
                                    true);
                            output.Append('}');

                            j++;
                        }

						childFD.WriteJSONCollectionClose(output);
					}

				}
				else if (thatReferenceObject is ElementState)
				{
					if (attributesSerialized || elementsSerialized)
						output.Append(", ");

					elementsSerialized = true;

					ElementState nestedES = (ElementState) thatReferenceObject;
					FieldDescriptor nestedFD = childFD.IsPolymorphic ? nestedES.ElementClassDescriptor.PseudoFieldDescriptor : childFD;

                    nestedES.serializeToJSONRecursive(nestedFD, output, true);

				}
			}
		}

		fieldDescriptor.WriteJSONCloseTag(output);
        }
        
        #endregion

        #region Hook Methods

        /// <summary>
        ///     
        /// </summary>
        public void preTranslationProcessingHook()
        {
            //Inheriting class can override to execute custom functionality.
        }


        #endregion

        #region Public Utility Functions

        /// <summary>
        /// 
        /// </summary>
        public StringBuilder textNodeBuffy
        {
            get
            {
                //TODO: figure out why this is so.
                return new StringBuilder("dummy");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetupRoot()
        {
            this.elementById = new Dictionary<string, ElementState>();
        }

        /// <summary>
        ///     Translates fields to attributes in XML
        /// </summary>
        /// <param name="atts"></param>
        public void TranslateAttributes(sax.Attributes atts)
        {
            int numAttributes = atts.attArray.Count;

            for (int i = 0; i < numAttributes; i++)
            {
                String tag = atts.getQName(i);
                String value = atts.getValue(i);

                if (value != null)
                {
                    ClassDescriptor classDescriptor = this.ElementClassDescriptor;
                    FieldDescriptor fieldDescriptor = classDescriptor.GetFieldDescriptorByTag(tag);

                    if (fieldDescriptor == null)
                    {
                        // TODO: give warning;
                    }
                    else
                    {
                        try
                        {
                            fieldDescriptor.SetFieldToScalar(this, value, null);
                            if ("id".Equals(fieldDescriptor.TagName))
                                this.elementById.Add(value, this);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("processing FieldDescriptor for tag " + tag);
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newChildElementState"></param>
        public void SetupChildElementState(ElementState newChildElementState)
        {
            newChildElementState.elementById = elementById;
            newChildElementState.parent = this;
            ClassDescriptor parentOptimizations = ElementClassDescriptor;
            newChildElementState.elementClassDescriptor = ClassDescriptor.GetClassDescriptor(newChildElementState);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ClassDescriptor ElementClassDescriptor
        {
            get
            {
                ClassDescriptor result = elementClassDescriptor;
                if (result == null)
                {
                    result = ClassDescriptor.GetClassDescriptor(this.GetType());
                    this.elementClassDescriptor = result;
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasScalarTextField 
        {
            get
            {
                return ElementClassDescriptor.HasScalarTextField;
            }
        }

        #endregion


        List<FieldEntry> cachedEnumerableFields;

        /// <summary>
        /// Enumerates over all the field descriptors,
        /// first the attributeFieldDescriptors, then the elementFieldDescriptors
        /// 
        /// </summary>
        public List<FieldEntry> EnumerableFields
        {
            get
            {
                List<FieldEntry> result = cachedEnumerableFields;

                if (result != null)
                    return result;

                result = new List<FieldEntry>();
                foreach (FieldDescriptor fd in elementClassDescriptor.AllFieldDescriptors)
                {

                    FieldEntry entry = new FieldEntry();

                    if (fd.WrappedFieldDescriptor != null)
                    {
                        entry.FD = fd.WrappedFieldDescriptor;
                        entry.Value = fd.WrappedFieldDescriptor.Field.GetValue(this);
                    }
                    else
                    {
                        entry.FD = fd;
                        entry.Value = fd.Field.GetValue(this);
                    }
                    if (entry.Value != null)
                        result.Add(entry);
                }
                cachedEnumerableFields = result;
                return result;
            }
        }

        /// <summary>
        /// Number of levels to the furthest child node.
        /// FIXME: Handle cycles.
        /// </summary>
        public int TreeDepth
        {
            get
            {
                return GetTreeDepth();
            }
            

        }

        public int GetTreeDepth()
        {
            List<int> depths = new List<int>();
            foreach (FieldEntry fieldEntry in EnumerableFields)
            {
                if (fieldEntry.Value == null)
                    continue;

                if (fieldEntry.Value is ElementState)
                {
                    int childDepth = (fieldEntry.Value as ElementState).TreeDepth;
                    depths.Add(childDepth);
                }
                if (fieldEntry.FD.IsCollection)
                {
                    IList collection = (IList)fieldEntry.Value;
                    if (collection.Count == 0)
                        continue;

                    if (typeof(ElementState).IsAssignableFrom(fieldEntry.FD.Field.FieldType.GetGenericArguments()[0]))
                    {
                        Console.WriteLine("Encountered collection of ElementState");
                        foreach (ElementState item in collection)
                        {
                            depths.Add(item.TreeDepth + 1);
                        }
                    }
                  
                    collection.OfType<ElementState>();
                }
            }

            return depths.Count == 0 ? 1 : depths.Max() + 1;
        }
    }

    public struct FieldEntry
    {
        FieldDescriptor fd;
        Object value;

        public FieldDescriptor FD
        {
            get { return fd; }
            set { fd = value; }
        }

        public Object Value
        {
            get { return value; }
            set {this.value = value; }
        }
    }
}
