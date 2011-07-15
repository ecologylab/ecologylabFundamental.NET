using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylabFundamental.ecologylab.collections;
using ecologylab.net;
using System.IO;
using System.Reflection;
using System.Collections;
using ecologylab.serialization.sax;

namespace ecologylabFundamental.ecologylab.serialization
{
    /// <summary>
    /// Representing the graph context
    /// </summary>
    public class TranslationContext : FieldTypes
    {
        private const String SIMPL_NAMESPACE	= " xmlns:simpl=\"http://ecologylab.net/research/simplGuide/serialization/index.html\"";

	    private const String SIMPL_ID = "simpl:id";

	    private const String	SIMPL_REF	= "simpl:ref";

        private MultiMap<Int32, ElementState> marshalledObjects = new MultiMap<Int32, ElementState>();

        private MultiMap<Int32, ElementState> visitedElements = new MultiMap<Int32, ElementState>();

        private MultiMap<Int32, ElementState> needsAttributeHashCode = new MultiMap<Int32, ElementState>();

        private Dictionary<String, ElementState> unmarshalledObjects = new Dictionary<String, ElementState>();

        /// <summary>
        /// constructor
        /// </summary>
        public TranslationContext()
        {
        }
        
        /// <summary>
        /// Handle simpl Ids associated with the given element state object
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <param name="elementState"></param>
        /// <returns></returns>
        public bool HandleSimplIds(String tag, String value, ElementState elementState)
	    {
            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
		    {
			    if (tag.Equals(TranslationContext.SIMPL_ID))
			    {
				    this.unmarshalledObjects.Add(value, elementState);
				    return true;
			    }
			    else
			    {
				    if (tag.Equals(TranslationContext.SIMPL_REF))
				    {
					    return true;
				    }
			    }
		    }
		    return false;
	    }

        /// <summary>
        /// resolving the graph based on the value of the graph switch
        /// </summary>
        /// <param name="elementState"></param>
        public void ResolveGraph(ElementState elementState)
	    {
		    if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
		    {
			    this.visitedElements.Add(elementState.GetHashCode(), elementState);

			    List<FieldDescriptor> elementFieldDescriptors = elementState.ClassDescriptor.ElementFieldDescriptors;
				
			    foreach (FieldDescriptor elementFieldDescriptor in elementFieldDescriptors)
			    {
				    Object thatReferenceObject = null;
				    FieldInfo childField = elementFieldDescriptor.Field;
				    try
				    {
                        thatReferenceObject = childField.GetValue(elementState);					    
				    }
				    catch (FieldAccessException e)
				    {
					    Console.WriteLine("WARNING re-trying access! " + e.StackTrace);
                        
                        try
					    {
						    thatReferenceObject = childField.GetValue(elementState);
					    }
					    catch (FieldAccessException e1)
					    {
						    Console.WriteLine("Can't access " + childField.Name);
                            Console.WriteLine(e1.StackTrace);						    
					    }
				    }
				    catch (Exception e)
				    {
                        Console.WriteLine("error");					   
				    }
				    // ignore null reference objects
				    if (thatReferenceObject == null)
					    continue;

				    int childFdType = elementFieldDescriptor.Type;

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
					    foreach (Object next in thatCollection)
					    {
                            if(next is ElementState)
						    {
							    ElementState compositeElement = (ElementState) next;
							    if (this.AlreadyVisited(compositeElement))
							    {
								    //this.needsAttributeHashCode.put(System.identityHashCode(compositeElement),
								    //		compositeElement);
								    this.needsAttributeHashCode.Add(compositeElement.GetHashCode(),
										    compositeElement);
							    }
							    else
							    {
								    this.ResolveGraph(compositeElement);
							    }
						    }
					    }
				    }
				    else if (thatReferenceObject is ElementState)
				    {
					    ElementState compositeElement = (ElementState) thatReferenceObject;

					    if (this.AlreadyVisited(compositeElement))
					    {
						    //this.needsAttributeHashCode.put(System.identityHashCode(compositeElement),
						    //		compositeElement);
						    this.needsAttributeHashCode.Add(compositeElement.GetHashCode(),
								    compositeElement);
					    }
					    else
					    {
						    ResolveGraph(compositeElement);
					    }
				    }
			    }
		    }
	    }

        /// <summary>
        /// Returns whether the object is already visited
        /// </summary>
        /// <param name="elementState"></param>
        /// <returns></returns>
        public bool AlreadyVisited(ElementState elementState)
        {
            return this.visitedElements.Contains(elementState.GetHashCode(), elementState);
        }

        /// <summary>
        /// Adding to the marshalledObjects
        /// </summary>
        /// <param name="elementState"></param>
        public void MapElementState(ElementState elementState)
        {
            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
            {
                this.marshalledObjects.Add(elementState.GetHashCode(), elementState);
            }
        }

        /// <summary>
        /// Append the simpl id
        /// </summary>
        /// <param name="appendable"></param>
        /// <param name="elementState"></param>
        public void AppendSimplIdIfRequired(StringBuilder appendable, ElementState elementState)			
	    {
		    if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON && this.NeedsHashCode(elementState))
		    {
			    this.AppendSimplIdAttribute(appendable, elementState);
		    }
	    }

        /// <summary>
        /// decides whether the give elementstate is already marshalled
        /// </summary>
        /// <param name="compositeElementState"></param>
        /// <returns></returns>
        public bool AlreadyMarshalled(ElementState compositeElementState)
	    {
		    return this.marshalledObjects.Contains(compositeElementState.GetHashCode(), compositeElementState);
	    }
	
        /// <summary>
        /// Append the simpl namespace
        /// </summary>
        /// <param name="appendable"></param>
	    public void AppendSimplNameSpace(StringBuilder appendable) 
	    {
            appendable.Append(SIMPL_NAMESPACE);
	    }

        /// <summary>
        /// Append Simpl ref id
        /// </summary>
        /// <param name="appendable"></param>
        /// <param name="elementState"></param>
        /// <param name="compositeElementFD"></param>
	    public void AppendSimplRefId(StringBuilder appendable, ElementState elementState,
			    FieldDescriptor compositeElementFD) 
	    {
		    compositeElementFD.WriteElementStart(appendable);
		    AppendSimplIdAttributeWithTagName(appendable, SIMPL_REF, elementState);
		    appendable.Append("/>");
	    }

        /// <summary>
        /// Append simpl id with the tag name
        /// </summary>
        /// <param name="appendable"></param>
        /// <param name="tagName"></param>
        /// <param name="elementState"></param>
	    public void AppendSimplIdAttributeWithTagName(StringBuilder appendable, String tagName,
			    ElementState elementState)
	    {
		    appendable.Append(' ');
		    appendable.Append(tagName);
		    appendable.Append('=');
		    appendable.Append('"');
		    appendable.Append(((Int32) elementState.GetHashCode()).ToString());
		    appendable.Append('"');
	    }

        /// <summary>
        /// Append simpl id attribute
        /// </summary>
        /// <param name="appendable"></param>
        /// <param name="elementState"></param>
	    public void AppendSimplIdAttribute(StringBuilder appendable, ElementState elementState)
	    {
		    AppendSimplIdAttributeWithTagName(appendable, SIMPL_ID, elementState);
	    }

        /// <summary>
        /// returns whether the hashcode is needed
        /// </summary>
        /// <param name="elementState"></param>
        /// <returns></returns>
        public bool NeedsHashCode(ElementState elementState)
        {
            return this.needsAttributeHashCode.Contains(elementState.GetHashCode(), elementState);
        }

        /// <summary>
        /// Return whether it is a graph
        /// </summary>
        public bool IsGraph
        {
            get
            {
                return this.needsAttributeHashCode.Count > 0;
            }
        }

        /// <summary>
        /// Getting the attributes from the unmarshalled objects
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public ElementState GetFromMap(Attributes attributes)
	    {
		    ElementState unMarshalledObject = null;

		    int numAttributes = attributes.getLength();
		    for (int i = 0; i < numAttributes; i++)
		    {
			    String tag = attributes.getQName(i);
			    String value = attributes.getValue(i);

			    if (tag.Equals(TranslationContext.SIMPL_REF))
			    {
                    unMarshalledObject = this.unmarshalledObjects[value];
			    }
		    }
		    return unMarshalledObject;
	    }
    }
}
