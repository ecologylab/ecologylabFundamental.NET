using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Simpl.Fundamental.Net;
using Simpl.Serialization.Graph;

namespace Simpl.Serialization.Context
{
    /// <summary>
    /// Representing the graph context
    /// </summary>
    public class TranslationContext : FieldTypes
    {
        public const String SimplNamespace = " xmlns:simpl=\"http://ecologylab.net/research/simplGuide/serialization/index.html\"";

        public const String SimplId = "simpl:id";
        public const String SimplRef = "simpl:ref";
        public const String JsonSimplRef = "simpl.ref";
        public const String JsonSimplId = "simpl.id";

        private readonly MultiMap<Int32> _marshalledObjects = new MultiMap<Int32>();
        private readonly MultiMap<Int32> _needsAttributeHashCode = new MultiMap<Int32>();
        private readonly Dictionary<String, Object> _unmarshalledObjects = new Dictionary<String, Object>();
        private readonly MultiMap<Int32> _visitedElements = new MultiMap<Int32>();

        private ParsedUri _baseDirPurl;
        private String _delimiter = ",";


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

                List<FieldDescriptor> elementFieldDescriptors =
                    ClassDescriptor.GetClassDescriptor(obj).ElementFieldDescriptors;

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
                            var compositeElement = next;
                            if (AlreadyVisited(compositeElement))
                            {
                                _needsAttributeHashCode.Add(compositeElement.GetHashCode(), compositeElement);
                            }
                            else
                            {
                                ResolveGraph(compositeElement);
                            }
                        }
                    }
                    else
                    {
                        var compositeElement = thatReferenceObject;
                        if (AlreadyVisited(compositeElement))
                        {
                            _needsAttributeHashCode.Add(compositeElement.GetHashCode(), compositeElement);
                        }
                        else
                        {
                            ResolveGraph(compositeElement);
                        }
                    }
                }
            }
        }

        private bool AlreadyVisited(Object obj)
        {
            return _visitedElements.Contains(obj.GetHashCode(), obj);
        }

        /// <summary>
        /// Adding to the marshalledObjects
        /// </summary>
        /// <param name="obj"></param>
        public void MapObject(Object obj)
        {
            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
            {
                _marshalledObjects.Add(obj.GetHashCode(), obj);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AlreadyMarshalled(Object obj)
        {
            return _marshalledObjects.Contains(obj.GetHashCode(), obj);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool NeedsHashCode(Object obj)
        {
            return _needsAttributeHashCode.Contains(obj.GetHashCode(), obj);
        }
    }
}