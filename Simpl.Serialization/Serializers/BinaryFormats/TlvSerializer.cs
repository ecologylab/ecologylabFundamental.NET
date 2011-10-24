using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Generic;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Serializers.BinaryFormats
{
    /// <summary>
    /// 
    /// </summary>
    public class TlvSerializer : BinarySerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="binaryWriter"></param>
        /// <param name="translationContext"></param>
        public override void Serialize(object obj, BinaryWriter binaryWriter, TranslationContext translationContext)
        {
            translationContext.ResolveGraph(obj);
            ClassDescriptor rootObjectClassDescriptor = ClassDescriptor.GetClassDescriptor(obj.GetType());
            try
            {
                Serialize(obj, rootObjectClassDescriptor.PseudoFieldDescriptor, binaryWriter, translationContext);
            }
            catch (Exception ex)
            {
                throw new SimplTranslationException("IO exception occured: ", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootObjectFieldDescriptor"></param>
        /// <param name="binaryWriter"></param>
        /// <param name="translationContext"></param>
        private void Serialize(object obj, FieldDescriptor rootObjectFieldDescriptor, BinaryWriter binaryWriter,
                               TranslationContext translationContext)
        {
            if (AlreadySerialized(obj, translationContext))
            {
                WriteSimplRef(obj, rootObjectFieldDescriptor, binaryWriter);
                return;
            }

            translationContext.MapObject(obj);

            SerializationPreHook(obj, translationContext);
            ClassDescriptor rootObjectClassDescriptor = GetClassDescriptor(obj);

            MemoryStream bufferMemoryStream = new MemoryStream();
            BinaryWriter outputBuffer = new BinaryWriter(bufferMemoryStream);

            IEnumerable<FieldDescriptor> allFieldDescriptors = GetClassDescriptor(obj).AllFieldDescriptors;

            SerializeFields(obj, outputBuffer, translationContext, allFieldDescriptors.ToList());
            WriteHeader(binaryWriter, bufferMemoryStream, rootObjectFieldDescriptor.TlvId);
            SerializationPostHook(obj, translationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="outputBuffer"></param>
        /// <param name="translationContext"></param>
        /// <param name="allFieldDescriptors"></param>
        private void SerializeFields(object obj, BinaryWriter outputBuffer, TranslationContext translationContext, IList<FieldDescriptor> allFieldDescriptors)
        {
            if (SimplTypesScope.graphSwitch == SimplTypesScope.GRAPH_SWITCH.ON)
            {
                if (translationContext.NeedsHashCode(obj))
                {
                    WriteSimplIdAttribute(obj, outputBuffer);
                }
            }

            foreach (FieldDescriptor childFd in allFieldDescriptors)
            {
                MemoryStream memoryStreamCollection = new MemoryStream();
                BinaryWriter collectionBuffer = new BinaryWriter(memoryStreamCollection);

                switch (childFd.FdType)
                {
                    case FieldTypes.Scalar:
                        WriteValue(obj, childFd, outputBuffer, translationContext);
                        break;
                    case FieldTypes.CompositeElement:
                        Object compositeObject = childFd.GetObject(obj);
                        FieldDescriptor compositeObjectFieldDescriptor = childFd.IsPolymorphic
                                                                             ? GetClassDescriptor(
                                                                                 compositeObject).PseudoFieldDescriptor
                                                                             : childFd;

                        WriteWrap(childFd, outputBuffer, memoryStreamCollection);
                        Serialize(compositeObject, compositeObjectFieldDescriptor, outputBuffer, translationContext);
                        WriteWrap(childFd, outputBuffer, memoryStreamCollection);
                        break;
                    case FieldTypes.CollectionScalar:
                    case FieldTypes.MapScalar:
                        Object scalarCollectionObject = childFd.GetObject(obj);
                        ICollection scalarCollection = XmlTools.GetCollection(scalarCollectionObject);
                        foreach (Object collectionObject in scalarCollection)
                        {
                            WriteScalarCollectionLeaf(collectionObject, childFd, collectionBuffer, translationContext);
                        }
                        WriteWrap(childFd, outputBuffer, memoryStreamCollection);
                        break;
                    case FieldTypes.CollectionElement:
                    case FieldTypes.MapElement:
                        Object compositeCollectionObject = childFd.GetObject(obj);
                        ICollection compositeCollection = XmlTools.GetCollection(compositeCollectionObject);
                        foreach (Object collectionComposite in compositeCollection)
                        {
                            FieldDescriptor collectionObjectFieldDescriptor = childFd.IsPolymorphic
                                                                                  ? GetClassDescriptor(
                                                                                      collectionComposite).
                                                                                        PseudoFieldDescriptor
                                                                                  : childFd;
                            Serialize(collectionComposite, collectionObjectFieldDescriptor, collectionBuffer,
                                      translationContext);
                        }
                        WriteWrap(childFd, outputBuffer, memoryStreamCollection);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="outputBuffer"></param>
        /// <param name="translationContext"></param>
        private void WriteScalarCollectionLeaf(object obj, FieldDescriptor fd, BinaryWriter outputBuffer, TranslationContext translationContext)
        {
            if (!fd.IsDefaultValue(obj.ToString()))
            {
                outputBuffer.Write(fd.TlvId);

                StringBuilder value = new StringBuilder();
                StringWriter valueWriter = new StringWriter(value);

                fd.AppendValue(valueWriter, obj, translationContext, Format.Tlv);

                MemoryStream temp = new MemoryStream();
                BinaryWriter tempStream = new BinaryWriter(temp);
                tempStream.Write(value.ToString());

                outputBuffer.Write(temp.Length);
                temp.WriteTo(outputBuffer.BaseStream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="outputBuffer"></param>
        /// <param name="collectionBuffy"></param>
        private void WriteWrap(FieldDescriptor fd, BinaryWriter outputBuffer, MemoryStream collectionBuffy)
        {
            if (fd.IsWrapped)
            {
                outputBuffer.Write(fd.WrappedTLVId);
                outputBuffer.Write(collectionBuffy.Length);
                collectionBuffy.WriteTo(outputBuffer.BaseStream);
            }
            else
                collectionBuffy.WriteTo(outputBuffer.BaseStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="outputBuffer"></param>
        /// <param name="translationContext"></param>
        private void WriteValue(object obj, FieldDescriptor fd, BinaryWriter outputBuffer, TranslationContext translationContext)
        {
            if (!fd.IsDefaultValueFromContext(obj))
            {
                outputBuffer.Write(fd.TlvId);

                StringBuilder value = new StringBuilder();
                StringWriter valueWriter = new StringWriter(value);

                fd.AppendValue(valueWriter, obj, translationContext, Format.Tlv);

                MemoryStream temp = new MemoryStream();
                BinaryWriter tempStream = new BinaryWriter(temp);
                tempStream.Write(value.ToString());

                outputBuffer.Write(temp.Length);
                temp.WriteTo(outputBuffer.BaseStream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="outputBuffer"></param>
        private void WriteSimplIdAttribute(object obj, BinaryWriter outputBuffer)
        {
            outputBuffer.Write(TranslationContext.SimplId.GetTlvId());
            outputBuffer.Write(4);
            outputBuffer.Write(obj.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <param name="buffer"></param>
        /// <param name="tlvId"></param>
        private void WriteHeader(BinaryWriter binaryWriter, MemoryStream buffer, int tlvId)
        {
            binaryWriter.Write(tlvId);
            binaryWriter.Write(buffer.Length);
            buffer.WriteTo(binaryWriter.BaseStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="binaryWriter"></param>
        private void WriteSimplRef(object obj, FieldDescriptor fd, BinaryWriter binaryWriter)
        {
            MemoryStream simplRefData = new MemoryStream();
            BinaryWriter outputBuffer = new BinaryWriter(simplRefData);
            binaryWriter.Write(TranslationContext.SimplRef.GetTlvId());
            binaryWriter.Write(4);
            binaryWriter.Write(obj.GetHashCode().ToString());
            WriteHeader(binaryWriter, simplRefData, fd.TlvId);
        }
    }
}
