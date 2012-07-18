using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Simpl.Fundamental.Generic;
using Simpl.Fundamental.Net;
using Simpl.Serialization.Graph;
using System.IO;

namespace Simpl.Serialization.Context
{
    /// <summary>
    /// Representing the graph context
    /// </summary>
    public class TranslationContext : IScalarUnmarshallingContext
    {
        public const String SimplNamespace = "xmlns:simpl";

        public const String SimplNamespaceAttribute =
            " xmlns:simpl=\"http://ecologylab.net/research/simplGuide/serialization/index.html\"";

        public const String SimplId = "simpl:id";
        public const String SimplRef = "simpl:ref";
        public const String JsonSimplRef = "simpl.ref";
        public const String JsonSimplId = "simpl.id";

        private MultiMap<Int32> _marshalledObjects = new MultiMap<Int32>();
        private MultiMap<Int32> _needsAttributeHashCode = new MultiMap<Int32>();
        private Dictionary<String, Object> _unmarshalledObjects = new Dictionary<String, Object>();
        private MultiMap<Int32> _visitedElements = new MultiMap<Int32>();

        private ParsedUri   _baseDirPurl;
        private FileInfo    _baseDirFile;
        private String      _delimiter = ",";


        public TranslationContext()
        {

        }

        public TranslationContext(FileInfo fileDirContext)
        {
            if (fileDirContext != null)
                this.BaseDirFile = fileDirContext;
        }

        public FileInfo BaseDirFile
        {
            get { return _baseDirFile; }
            set
            {
                this._baseDirFile = value;
                this._baseDirPurl = new ParsedUri(value.FullName);
            }
               
        }

        /// <summary>
        /// Return whether it is a graph
        /// </summary>
        public bool IsGraph
        {
            get { return _needsAttributeHashCode.Count > 0; }
        }

        public string Delimiter
        {
            get { return _delimiter; }
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
            if (SimplTypesScope.graphSwitch == SimplTypesScope.GRAPH_SWITCH.ON)
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
            if (SimplTypesScope.graphSwitch == SimplTypesScope.GRAPH_SWITCH.ON)
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
                        Debug.WriteLine("WARNING re-trying access! " + e.StackTrace);

                        try
                        {
                            thatReferenceObject = childField.GetValue(obj);
                        }
                        catch (FieldAccessException e1)
                        {
                            Debug.WriteLine("Can't access " + childField.Name);
                            Debug.WriteLine(e1.StackTrace);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("error" + e);
                    }

                    if (thatReferenceObject == null)
                        continue;

                    int childFdType = elementFieldDescriptor.FdType;

                    ICollection thatCollection;
                    switch (childFdType)
                    {
                        case FieldTypes.CollectionElement:
                        case FieldTypes.CollectionScalar:
                        case FieldTypes.MapElement:
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
            if(_unmarshalledObjects == null)
                InitializeMultiMaps();

            return _visitedElements.Contains(obj.GetHashCode(), obj) != -1;
        }

        /// <summary>
        /// Adding to the marshalledObjects
        /// </summary>
        /// <param name="obj"></param>
        public void MapObject(Object obj)
        {
            if (SimplTypesScope.graphSwitch == SimplTypesScope.GRAPH_SWITCH.ON)
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
            if (obj == null)
                return false;

            return _marshalledObjects.Contains(obj.GetHashCode(), obj) != -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool NeedsHashCode(Object obj)
        {
            return _needsAttributeHashCode.Contains(obj.GetHashCode(), obj) != -1;
        }

        public ParsedUri UriContext()
        {
            return _baseDirPurl;
        }

        public void SetUriContext(ParsedUri purl)
        {
            _baseDirPurl = purl;
        }

        public object GetFromMap(string key)
        {
//            if (_unmarshalledObjects.ContainsKey(key))
                return _unmarshalledObjects[key];
//            else return null;
        }

        public void MarkAsUnmarshalled(String value, Object elementState)
        {
            if (_unmarshalledObjects == null)
                InitializeMultiMaps();
            _unmarshalledObjects.Put(value, elementState);
        }

        public void InitializeMultiMaps()
        {
            _marshalledObjects = new MultiMap<Int32>();
            _visitedElements = new MultiMap<Int32>();
            _needsAttributeHashCode = new MultiMap<Int32>();
            _unmarshalledObjects = new Dictionary<string, object>();
        }

        public String GetSimplId(Object obj)
        {
            int objectHashCode = ((int) obj.GetHashCode());
            int orderedIndex = _marshalledObjects.Contains(objectHashCode, obj);

            if (orderedIndex > 0)
                return objectHashCode.ToString() + "," + orderedIndex.ToString();

            return objectHashCode.ToString();
        }
    }
}