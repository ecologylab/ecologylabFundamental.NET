using System;
using System.Collections.Generic;
using System.IO;
using Simpl.Serialization.Context;
using ecologylab.serialization;

namespace Simpl.Serialization.Serializers.StringFormats
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlSerializer : StringSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        /// <param name="translationContext"></param>
        public override void Serialize(Object obj, StreamWriter streamWriter, TranslationContext translationContext)
        {
            translationContext.ResolveGraph(obj);
            ClassDescriptor rootObjectClassDescriptor = ClassDescriptor.GetClassDescriptor(obj.GetType());
            try
            {
                Serialize(obj, rootObjectClassDescriptor.PseudoFieldDescriptor, streamWriter, translationContext);
            }
            catch (Exception ex)
            {
                throw new Exception("IO exception occured", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootObjectFieldDescriptor"></param>
        /// <param name="streamWriter"></param>
        /// <param name="translationContext"></param>
        private void Serialize(object obj, FieldDescriptor rootObjectFieldDescriptor, StreamWriter streamWriter, TranslationContext translationContext)
        {
            if (obj == null)
                return;

            if (AlreadySerialized(obj, translationContext))
            {
                WriteSimplRef(obj, rootObjectFieldDescriptor, streamWriter);
                return;
            }

            translationContext.MapObject(obj);

            SerializationPreHook(obj, translationContext);

            WriteObjectStart(rootObjectFieldDescriptor, streamWriter);

            ClassDescriptor rootObjectClassDescriptor = GetClassDescriptor(obj);
            SerializedAttributes(obj, streamWriter, translationContext, rootObjectClassDescriptor);

            List<FieldDescriptor> elementFieldDescriptors = rootObjectClassDescriptor.ElementFieldDescriptors;

            Boolean hasXmlText = rootObjectClassDescriptor.HasScalarTextField;
            Boolean hasElements = elementFieldDescriptors.Count > 0;

            if (!hasElements && !hasXmlText)
            {
                WriteCompleteClose(streamWriter);
            }
            else
            {
                WriteClose(streamWriter);

                if (hasXmlText)
                {
                    WriteValueAsText(obj, rootObjectClassDescriptor.ScalarTextFD, streamWriter);
                }

                SerializeFields(obj, streamWriter, translationContext, elementFieldDescriptors);

                WriteObjectClose(rootObjectFieldDescriptor, streamWriter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="streamWriter"></param>
        private void WriteObjectClose(FieldDescriptor fd, StreamWriter streamWriter)
        {
            streamWriter.Write('<');
            streamWriter.Write('/');
            streamWriter.Write(fd.ElementStart);
            streamWriter.Write('>');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="elementFieldDescriptors"></param>
        private void SerializeFields(object obj, StreamWriter streamWriter, TranslationContext translationContext, List<FieldDescriptor> elementFieldDescriptors)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="scalarTextFD"></param>
        /// <param name="streamWriter"></param>
        private void WriteValueAsText(object o, FieldDescriptor scalarTextFD, StreamWriter streamWriter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamWriter"></param>
        private void WriteClose(StreamWriter streamWriter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="rootObjectClassDescritor"></param>
        private void SerializedAttributes(object obj, StreamWriter streamWriter, TranslationContext translationContext, ClassDescriptor rootObjectClassDescritor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="streamWriter"></param>
        private static void WriteSimplRef(object obj, FieldDescriptor fd, StreamWriter streamWriter)
        {
            WriteObjectStart(fd, streamWriter);
            WriteSimpRefAttribute(obj, streamWriter);
            WriteCompleteClose(streamWriter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamWriter"></param>
        private static void WriteCompleteClose(StreamWriter streamWriter)
        {
            streamWriter.Write('/');
            streamWriter.Write('>');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        private static void WriteSimpRefAttribute(object obj, StreamWriter streamWriter)
        {
            streamWriter.Write(' ');
            streamWriter.Write(TranslationContext.SimplRef);
            streamWriter.Write('=');
            streamWriter.Write('"');
            streamWriter.Write(obj.GetHashCode().ToString());
            streamWriter.Write('"');
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="streamWriter"></param>
        private static void WriteObjectStart(FieldDescriptor fd, StreamWriter streamWriter)
        {
            streamWriter.Write('<');
            streamWriter.Write(fd.ElementStart);
        }
    }
}
