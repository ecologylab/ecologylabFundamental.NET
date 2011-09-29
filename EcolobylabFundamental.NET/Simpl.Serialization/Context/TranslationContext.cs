using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Simpl.Serialization.Graph;
using ecologylab.serialization;

namespace Simpl.Serialization.Context
{
    /// <summary>
    /// Representing the graph context
    /// </summary>
    public class TranslationContext : FieldTypes
    {
        public const String SimplNamespace =
            " xmlns:simpl=\"http://ecologylab.net/research/simplGuide/serialization/index.html\"";

        public const String SimplId = "simpl:id";

        public const String SimplRef = "simpl:ref";

        private readonly MultiMap<Int32, ElementState> _marshalledObjects = new MultiMap<Int32, ElementState>();

        private readonly MultiMap<Int32, ElementState> _needsAttributeHashCode = new MultiMap<Int32, ElementState>();

        private readonly Dictionary<String, ElementState> _unmarshalledObjects = new Dictionary<String, ElementState>();
        private readonly MultiMap<Int32, ElementState> _visitedElements = new MultiMap<Int32, ElementState>();

        /// <summary>
        /// Return whether it is a graph
        /// </summary>
        public bool IsGraph
        {
            get { return _needsAttributeHashCode.Count > 0; }
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
                if (tag.Equals(SimplId))
                {
                    _unmarshalledObjects.Add(value, elementState);
                    return true;
                }
                if (tag.Equals(SimplRef))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// resolving the graph based on the value of the graph switch
        /// </summary>
        /// <param name="obj"></param>
        public void ResolveGraph(object obj)
        {
            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
            {
                _visitedElements.Add(obj.GetHashCode(), obj);

                List<FieldDescriptor> elementFieldDescriptors = obj.ClassDescriptor.ElementFieldDescriptors;

                foreach (FieldDescriptor elementFieldDescriptor in elementFieldDescriptors)
                {
                    Object thatReferenceObject = null;
                    FieldInfo childField = elementFieldDescriptor.Field;
                    try
                    {
                        thatReferenceObject = childField.GetValue(obj);
                    }
                    catch (FieldAccessException e)
                    {
                        Console.WriteLine("WARNING re-trying access! " + e.StackTrace);

                        try
                        {
                            thatReferenceObject = childField.GetValue(obj);
                        }
                        catch (FieldAccessException e1)
                        {
                            Console.WriteLine("Can't access " + childField.Name);
                            Console.WriteLine(e1.StackTrace);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("error" + e);
                    }
                    // ignore null reference objects
                    if (thatReferenceObject == null)
                        continue;

                    int childFdType = elementFieldDescriptor.Type;

                    ICollection thatCollection;
                    switch (childFdType)
                    {
                        case CollectionElement:
                        case CollectionScalar:
                        case MapElement:
                            thatCollection = XmlTools.GetCollection(thatReferenceObject);
                            break;
                        default:
                            thatCollection = null;
                            break;
                    }

                    if (thatCollection != null && (thatCollection.Count > 0))
                    {
                        foreach (Object next in thatCollection)
                        {
                            if (next is ElementState)
                            {
                                var compositeElement = (ElementState) next;
                                if (AlreadyVisited(compositeElement))
                                {
                                    //this.needsAttributeHashCode.put(System.identityHashCode(compositeElement),
                                    //		compositeElement);
                                    _needsAttributeHashCode.Add(compositeElement.GetHashCode(),
                                                                compositeElement);
                                }
                                else
                                {
                                    ResolveGraph(compositeElement);
                                }
                            }
                        }
                    }
                    else if (thatReferenceObject is ElementState)
                    {
                        var compositeElement = (ElementState) thatReferenceObject;

                        if (AlreadyVisited(compositeElement))
                        {
                            //this.needsAttributeHashCode.put(System.identityHashCode(compositeElement),
                            //		compositeElement);
                            _needsAttributeHashCode.Add(compositeElement.GetHashCode(),
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
            return _visitedElements.Contains(elementState.GetHashCode(), elementState);
        }

        /// <summary>
        /// Adding to the marshalledObjects
        /// </summary>
        /// <param name="elementState"></param>
        public void MapElementState(ElementState elementState)
        {
            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
            {
                _marshalledObjects.Add(elementState.GetHashCode(), elementState);
            }
        }

        /// <summary>
        /// Append the simpl id
        /// </summary>
        /// <param name="appendable"></param>
        /// <param name="elementState"></param>
        public void AppendSimplIdIfRequired(StringBuilder appendable, ElementState elementState)
        {
            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON && NeedsHashCode(elementState))
            {
                AppendSimplIdAttribute(appendable, elementState);
            }
        }

        /// <summary>
        /// decides whether the give elementstate is already marshalled
        /// </summary>
        /// <param name="compositeElementState"></param>
        /// <returns></returns>
        public bool AlreadyMarshalled(ElementState compositeElementState)
        {
            return _marshalledObjects.Contains(compositeElementState.GetHashCode(), compositeElementState);
        }

        /// <summary>
        /// Append the simpl namespace
        /// </summary>
        /// <param name="appendable"></param>
        public void AppendSimplNameSpace(StringBuilder appendable)
        {
            appendable.Append(SimplNamespace);
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
            AppendSimplIdAttributeWithTagName(appendable, SimplRef, elementState);
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
            appendable.Append((elementState.GetHashCode()).ToString());
            appendable.Append('"');
        }

        /// <summary>
        /// Append simpl id attribute
        /// </summary>
        /// <param name="appendable"></param>
        /// <param name="elementState"></param>
        public void AppendSimplIdAttribute(StringBuilder appendable, ElementState elementState)
        {
            AppendSimplIdAttributeWithTagName(appendable, SimplId, elementState);
        }

        /// <summary>
        /// returns whether the hashcode is needed
        /// </summary>
        /// <param name="elementState"></param>
        /// <returns></returns>
        public bool NeedsHashCode(ElementState elementState)
        {
            return _needsAttributeHashCode.Contains(elementState.GetHashCode(), elementState);
        }

        internal bool AlreadyMarshalled(object obj)
        {
            throw new NotImplementedException();
        }

        public void MapObject(object o)
        {
            throw new NotImplementedException();
        }

        public bool NeedsHashCode(object elementState)
        {
            throw new NotImplementedException();
        }

        public ElementState GetFromMap(ecologylab.serialization.sax.Attributes attributes)
        {
            throw new NotImplementedException();
        }
    }
}