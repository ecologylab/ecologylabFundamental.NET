using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ecologylabFundamental.ecologylab.xml
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementState : FieldTypes
    {
        private ClassDescriptor elementClassDescriptor;
        private Dictionary<String, ElementState> elementById;
        
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
        public ElementState parent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        public void translateToXMLStringBuilder(StringBuilder output)
        {
            if (output == null) throw new Exception("null : output object");
            else translateToXMLStringBuilder(this.ElementClassDescriptor.PseudoFieldDescriptor, output);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="translationScope"></param>
        /// <returns></returns>
        public static ElementState translateFromXML(String filePath, TranslationScope translationScope)
        {
            ElementStateSAXHandler saxHandler = new ElementStateSAXHandler(filePath, translationScope);
            return saxHandler.Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldDescriptor"></param>
        /// <param name="output"></param>
        public void translateToXMLStringBuilder(FieldDescriptor fieldDescriptor, StringBuilder output)
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

                //FIXME get rid of this block, because this method of dealing with text nodes is obsolete
                if (textNode != null)
                {
                    XMLTools.EscapeXML(output, textNode);
                }

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
                    int childOptimizationsType = childFD.GetTypeId();
                    if (childOptimizationsType == LEAF)
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
                                System.Console.WriteLine("Can't access " + childField.Name);
                            }
                        }

                        if (thatReferenceObject == null)
                            continue;

                        Boolean isScalar = (childOptimizationsType == COLLECTION_SCALAR) || (childOptimizationsType == MAP_SCALAR);

                        ICollection thatCollection;
                        switch (childOptimizationsType)
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

                                    collectionSubElementState.translateToXMLStringBuilder(collectionElementFD, output);
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

                            nestedES.translateToXMLStringBuilder(nestedF2XO, output);
                        }
                    }
                }

                fieldDescriptor.WriteCloseTag(output);
            }
        }

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
        private void preTranslationProcessingHook()
        {
            //Inheriting class can override to execute custom functionality.
        }

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
        /// 
        /// </summary>
        /// <param name="translationScope"></param>
        /// <param name="atts"></param>
        /// <param name="marshallingContext"></param>
        /// <param name="context"></param>
        public void TranslateAttributes(TranslationScope translationScope, sax.Attributes atts, ElementStateSAXHandler marshallingContext, ElementState context)
        {
            int numAttributes = atts.attArray.Count;

            for (int i = 0; i < numAttributes; i++)
            {
                String tag = atts.getQName(i);
                String value = atts.getValue(i);

                if (value != null)
                {
                    ClassDescriptor classDescriptor = this.ElementClassDescriptor;
                    FieldDescriptor fieldDescriptor = classDescriptor.GetFieldDescriptorByTag(tag, translationScope, context);

                    if (fieldDescriptor == null)
                    {
                        // TODO: give warning;
                    }
                    else
                    {
                        switch (fieldDescriptor.Type)
                        {
                            case ATTRIBUTE:
                                fieldDescriptor.SetFieldToScalar(this, value, marshallingContext);
                                if ("id".Equals(fieldDescriptor.TagName))
                                    this.elementById.Add(value, this);
                                break;
                            case XMLNS_ATTRIBUTE:
                                //don't know why it's here but its here :) 
                                break;
                            default:
                                break;
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
    }
}
