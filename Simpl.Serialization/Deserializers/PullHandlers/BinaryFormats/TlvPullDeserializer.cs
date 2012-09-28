using System;
using System.Collections;
using System.IO;
using System.Text;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types.Element;

namespace Simpl.Serialization.Deserializers.PullHandlers.BinaryFormats
{
    /// <summary>
    /// 
    /// </summary>
    public class TlvPullDeserializer : BinaryPullDeserializer
    {
        private const int HeaderSize = 8;

        /// <summary>
        /// 
        /// </summary>
        private BinaryReader _binaryReader;

        /// <summary>
        /// 
        /// </summary>
        private int _blockType;

        /// <summary>
        /// 
        /// </summary>
        private int _blockLength;

        /// <summary>
        /// 
        /// </summary>
        private bool _isEos;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        public TlvPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext,
                                   IDeserializationHookStrategy deserializationHookStrategy)
            : base(inputSimplTypesScope, inputContext, deserializationHookStrategy)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputSimplTypesScope"></param>
        /// <param name="inputContext"></param>
        public TlvPullDeserializer(SimplTypesScope inputSimplTypesScope, TranslationContext inputContext)
            : base(inputSimplTypesScope, inputContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override object Parse(Stream stream)
        {
            _binaryReader = new BinaryReader(stream, Encoding.UTF8);
            return Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public override object Parse(BinaryReader binaryReader)
        {
            _binaryReader = binaryReader;
            return Parse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Object Parse()
        {
            NextHeader();

            ClassDescriptor rootClassDescriptor = simplTypesScope.GetClassDescriptorByTlvId(_blockType);

            if (rootClassDescriptor == null)
            {
                throw new SimplTranslationException("cannot find the class descriptor for root element; make sure if translation scope is correct.");
            }

            object root = rootClassDescriptor.GetInstance();

            DeserializationPreHook(root, translationContext);
            if (deserializationHookStrategy != null)
                deserializationHookStrategy.DeserializationPreHook(root, null);

            return CreateObjectModel(root, rootClassDescriptor, _blockLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="rootClassDescriptor"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private Object CreateObjectModel(Object root, ClassDescriptor rootClassDescriptor, int length)
        {
            int bytesRead = 0;

            while (!_isEos && bytesRead < length)
            {
                bytesRead += NextHeader();

                if (_blockType == TranslationContext.SimplId.GetTlvId())
                {
                    int simplId = _binaryReader.ReadInt32();
                    translationContext.MarkAsUnmarshalled(simplId.ToString(), root);
                    bytesRead += 4;
                    continue;
                }

                if (_blockType == TranslationContext.SimplRef.GetTlvId())
                {
                    int simplRef = _binaryReader.ReadInt32();
                    return translationContext.GetFromMap(simplRef.ToString());
                }

                FieldDescriptor currentFieldDescriptor = rootClassDescriptor.GetFieldDescriptorByTlvId(_blockType);

                int fieldType = currentFieldDescriptor.FdType;

                switch (fieldType)
                {
                    case FieldTypes.Scalar:
                        bytesRead += DeserializeScalar(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CollectionScalar:
                        bytesRead += DeserializeScalarCollectionElement(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CompositeElement:
                        bytesRead += DeserializeComposite(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.CollectionElement:
                        bytesRead += DeserializeCompositeCollectionElement(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.MapElement:
                        bytesRead += DeserializeCompositeMapElement(root, currentFieldDescriptor);
                        break;
                    case FieldTypes.Wrapper:
                        currentFieldDescriptor = currentFieldDescriptor.WrappedFd;
                        switch (currentFieldDescriptor.FdType)
                        {
                            case FieldTypes.CollectionScalar:
                                bytesRead += DeserializeScalarCollection(root, currentFieldDescriptor);
                                break;
                            case FieldTypes.CollectionElement:
                                bytesRead += DeserializeCompositeCollection(root, currentFieldDescriptor);
                                break;
                            case FieldTypes.MapElement:
                                bytesRead += DeserializeCompositeMap(root, currentFieldDescriptor);
                                break;
                            case FieldTypes.CompositeElement:
                                //TODO: wrapped composites in tlv?
                                break;
                        }
                        break;
                }
            }

            DeserializationPostHook(root, translationContext);
		    if (deserializationHookStrategy != null)
			    deserializationHookStrategy.DeserializationPostHook(root, null);

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        private int DeserializeCompositeMap(Object root, FieldDescriptor fd)
        {
            int bytesRead = 0;
            int length = _blockLength;
            do
            {
                bytesRead += NextHeader();
                bytesRead += DeserializeCompositeMapElement(root, fd);
            } while (!_isEos && bytesRead < length);
            return bytesRead;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        private int DeserializeCompositeCollection(Object root, FieldDescriptor fd)
        {
            int bytesRead = 0;
            int length = _blockLength;
            do
            {
                bytesRead += NextHeader();
                bytesRead += DeserializeCompositeCollectionElement(root, fd);
            } while (!_isEos && bytesRead < length);
            return bytesRead;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        private int DeserializeScalarCollection(Object root, FieldDescriptor fd)
        {
            int bytesRead = 0;
            int length = _blockLength;
            do
            {
                bytesRead += NextHeader();
                bytesRead += DeserializeScalarCollectionElement(root, fd);
            } while (!_isEos && bytesRead < length);
            return bytesRead;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        private int DeserializeCompositeMapElement(Object root, FieldDescriptor fd)
        {
            int length = _blockLength;

            object subRoot = GetSubRoot(fd, root);
            if (subRoot is IMappable<Object>)
            {
                Object key = ((IMappable<Object>) subRoot).Key();
                IDictionary dictionary = (IDictionary) fd.AutomaticLazyGetCollectionOrMap(root);
                if (dictionary.Contains(key))
                {
                    //Note: overriding a key in map, duplicate data?. 
                    dictionary[key] = subRoot;
                }
                else
                {
                    dictionary.Add(key, subRoot);
                }
            }

            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        private int DeserializeCompositeCollectionElement(Object root, FieldDescriptor fd)
        {
            int length = _blockLength;

            object subRoot = GetSubRoot(fd, root);
            if (subRoot != null)
            {
                IList collection = (IList) fd.AutomaticLazyGetCollectionOrMap(root);
                collection.Add(subRoot);
            }
            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        /// <returns></returns>
        private int DeserializeComposite(Object root, FieldDescriptor currentFieldDescriptor)
        {
            int length = _blockLength;
            Object subRoot = GetSubRoot(currentFieldDescriptor, root);
            currentFieldDescriptor.SetFieldToComposite(root, subRoot);
            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentFieldDescriptor"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private Object GetSubRoot(FieldDescriptor currentFieldDescriptor, Object root)
        {
            ClassDescriptor subRootClassDescriptor = currentFieldDescriptor.GetChildClassDescriptor(_blockType);

            object subRoot = subRootClassDescriptor.GetInstance();
            DeserializationPreHook(subRoot, translationContext);
            if (deserializationHookStrategy != null)
                deserializationHookStrategy.DeserializationPreHook(subRoot, currentFieldDescriptor);

            if (subRoot != null)
            {
                if (subRoot is ElementState && root is ElementState)
                {
                    ((ElementState) subRoot).SetupInParent((ElementState) root);
                }
            }

            return CreateObjectModel(subRoot, subRootClassDescriptor, _blockLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        private int DeserializeScalarCollectionElement(Object root, FieldDescriptor fd)
        {
            byte[] value = _binaryReader.ReadBytes(_blockLength);
            String stringValue = System.Text.Encoding.UTF8.GetString(value,0,value.Length);
            fd.AddLeafNodeToCollection(root, stringValue, translationContext);
            return _blockLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="currentFieldDescriptor"></param>
        /// <returns></returns>
        private int DeserializeScalar(Object root, FieldDescriptor currentFieldDescriptor)
        {
            byte[] value = _binaryReader.ReadBytes(_blockLength);
            String stringValue = System.Text.Encoding.UTF8.GetString(value,0,value.Length);
            currentFieldDescriptor.SetFieldToScalar(root, stringValue, translationContext);
            return _blockLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int NextHeader()
        {
            try
            {
                _blockType = _binaryReader.ReadInt32();
                _blockLength = _binaryReader.ReadInt32();
                return HeaderSize;
            }
            catch (Exception)
            {
                _isEos = true;
                return 0;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public int BlockType
        {
            get { return _blockType; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BlockLength
        {
            get { return _blockLength; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEos
        {
            get { return _isEos; }
        }
    }
}