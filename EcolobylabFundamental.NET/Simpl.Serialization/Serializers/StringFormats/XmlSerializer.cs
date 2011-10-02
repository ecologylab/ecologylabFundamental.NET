using System;
using System.Collections;
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

        private const String StartCdata = "<![CDATA[";
        private const String EndCdata = "]]>";

        private Boolean _isRoot = true;

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
        private void SerializeFields(object obj, StreamWriter streamWriter, TranslationContext translationContext, IEnumerable<FieldDescriptor> elementFieldDescriptors)
        {
           foreach (FieldDescriptor fd in elementFieldDescriptors)
		{
			switch (fd.FdType)
			{
			case FieldTypes.Scalar:
				WriteValueAsLeaf(obj, fd, streamWriter, translationContext);
				break;
            case FieldTypes.CompositeElement:
				Object compositeObject = fd.GetObject(obj);
				if (compositeObject != null)
				{
					FieldDescriptor compositeObjectFieldDescriptor = fd.IsPolymorphic ? GetClassDescriptor(
							compositeObject).PseudoFieldDescriptor
							: fd;
					WriteWrap(fd, streamWriter, false);
					Serialize(compositeObjectFieldDescriptor, streamWriter, translationContext);
					WriteWrap(fd, streamWriter, true);
				}
				break;
            case FieldTypes.CollectionScalar:
            case FieldTypes.MapScalar:
				Object scalarCollectionObject = fd.GetObject(obj);
				ICollection scalarCollection = XmlTools.GetCollection(scalarCollectionObject);
				if (scalarCollection != null && scalarCollection.Count > 0)
				{
					WriteWrap(fd, streamWriter, false);

					foreach (Object collectionScalar in scalarCollection)
					{
						WriteScalarCollectionLeaf(collectionScalar, fd, streamWriter, translationContext);
					}
					WriteWrap(fd, streamWriter, true);
				}
				break;
            case FieldTypes.CollectionElement:
            case FieldTypes.MapElement:
				Object compositeCollectionObject = fd.GetObject(obj);
				ICollection compositeCollection = XmlTools.GetCollection(compositeCollectionObject);
                if (compositeCollection != null && compositeCollection.Count > 0)
				{
					WriteWrap(fd, streamWriter, false);
					foreach (Object collectionComposite in compositeCollection)
					{
						FieldDescriptor collectionObjectFieldDescriptor = fd.IsPolymorphic ? GetClassDescriptor(
								collectionComposite).PseudoFieldDescriptor
								: fd;
						Serialize(collectionComposite, collectionObjectFieldDescriptor, streamWriter,
								translationContext);
					}
					WriteWrap(fd, streamWriter, true);
				}
				break;
			}
		}
        }

        private void WriteScalarCollectionLeaf(object obj, FieldDescriptor fd, StreamWriter streamWriter, TranslationContext translationContext)
        {
            if (!fd.IsDefaultValue(obj.ToString()))
            {
                streamWriter.Write('<');
                streamWriter.Write(fd.ElementStart);
                streamWriter.Write('>');
                fd.AppendCollectionScalarValue(streamWriter, obj, translationContext, Format.Xml);
                streamWriter.Write('<');
                streamWriter.Write('/');
                streamWriter.Write(fd.ElementStart);
                streamWriter.Write('>');
            }
        }

        private void WriteWrap(FieldDescriptor fd, StreamWriter streamWriter, bool close)
        {
            if (fd.IsWrapped)
            {
                streamWriter.Write('<');
                if (close)
                    streamWriter.Write('/');
                streamWriter.Write(fd.TagName);
                streamWriter.Write('>');
            }
        }

        private void WriteValueAsLeaf(object obj, FieldDescriptor fd, StreamWriter streamWriter, TranslationContext translationContext)
        {
            if (!fd.IsDefaultValueFromContext(obj))
            {
                streamWriter.Write('<');
                streamWriter.Write(fd.ElementStart);
                streamWriter.Write('>');
                fd.AppendValue(streamWriter, obj, translationContext, Format.Xml);
                streamWriter.Write('<');
                streamWriter.Write('/');
                streamWriter.Write(fd.ElementStart);
                streamWriter.Write('>');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="streamWriter"></param>
        private void WriteValueAsText(object obj, FieldDescriptor fd, StreamWriter streamWriter)
        {
            if (!fd.IsDefaultValueFromContext(obj))
            {
                if (fd.IsCdata)
                    streamWriter.Write(StartCdata);
                fd.AppendValue(streamWriter, obj, null, Format.Xml);
                if (fd.IsCdata)
                    streamWriter.Write(EndCdata);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamWriter"></param>
        private void WriteClose(StreamWriter streamWriter)
        {
            streamWriter.Write('>');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="rootObjectClassDescrpitor"></param>
        private void SerializedAttributes(object obj, StreamWriter streamWriter, TranslationContext translationContext, ClassDescriptor rootObjectClassDescrpitor)
        {
            List<FieldDescriptor> attributeFieldDescriptors = rootObjectClassDescrpitor
                .AttributeFieldDescriptors;

            foreach (FieldDescriptor childFd in attributeFieldDescriptors)
            {
                try
                {
                    WriteValueAsAtrribute(obj, childFd, streamWriter, translationContext);
                }
                catch (Exception ex)
                {
                    throw new Exception("serialize for attribute " + obj, ex);
                }
            }

            if (TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON)
            {
                if (translationContext.NeedsHashCode(obj))
                {
                    WriteSimplIdAttribute(obj, streamWriter);
                }

                if (_isRoot && translationContext.IsGraph)
                {
                    WriteSimplNameSpace(streamWriter);
                    _isRoot = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamWriter"></param>
        private static void WriteSimplNameSpace(StreamWriter streamWriter)
        {
            streamWriter.Write(TranslationContext.SimplNamespace);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        private static void WriteSimplIdAttribute(object obj, StreamWriter streamWriter)
        {
            streamWriter.Write(' ');
            streamWriter.Write(TranslationContext.SimplId);
            streamWriter.Write('=');
            streamWriter.Write('"');
            streamWriter.Write(obj.GetHashCode().ToString());
            streamWriter.Write('"');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="streamWriter"></param>
        /// <param name="translationContext"></param>
        private static void WriteValueAsAtrribute(object obj, FieldDescriptor fd, StreamWriter streamWriter, TranslationContext translationContext)
        {
            if (obj != null)
            {
                if (!fd.IsDefaultValueFromContext(obj))
                {
                    streamWriter.Write(' ');
                    streamWriter.Write(fd.TagName);
                    streamWriter.Write('=');
                    streamWriter.Write('"');

                    fd.AppendValue(streamWriter, obj, translationContext, Format.Xml);

                    streamWriter.Write('"');
                }
            }
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
