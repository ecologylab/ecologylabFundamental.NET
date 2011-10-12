using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Simpl.Serialization.Context;

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
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        public override void Serialize(object obj, TextWriter textWriter, TranslationContext translationContext)
        {
            translationContext.ResolveGraph(obj);
            ClassDescriptor rootObjectClassDescriptor = ClassDescriptor.GetClassDescriptor(obj.GetType());
            try
            {
                Serialize(obj, rootObjectClassDescriptor.PseudoFieldDescriptor, textWriter, translationContext);
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
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private void Serialize(object obj, FieldDescriptor rootObjectFieldDescriptor, TextWriter textWriter, TranslationContext translationContext)
        {
            if (obj == null)
                return;

            if (AlreadySerialized(obj, translationContext))
            {
                WriteSimplRef(obj, rootObjectFieldDescriptor, textWriter);
                return;
            }

            translationContext.MapObject(obj);

            SerializationPreHook(obj, translationContext);

            WriteObjectStart(rootObjectFieldDescriptor, textWriter);

            ClassDescriptor rootObjectClassDescriptor = GetClassDescriptor(obj);
            SerializedAttributes(obj, textWriter, translationContext, rootObjectClassDescriptor);

            List<FieldDescriptor> elementFieldDescriptors = rootObjectClassDescriptor.ElementFieldDescriptors;

            Boolean hasXmlText = rootObjectClassDescriptor.HasScalarTextField;
            Boolean hasElements = elementFieldDescriptors.Count > 0;

            if (!hasElements && !hasXmlText)
            {
                WriteCompleteClose(textWriter);
            }
            else
            {
                WriteClose(textWriter);

                if (hasXmlText)
                {
                    WriteValueAsText(obj, rootObjectClassDescriptor.ScalarTextFD, textWriter);
                }

                SerializeFields(obj, textWriter, translationContext, elementFieldDescriptors);

                WriteObjectClose(rootObjectFieldDescriptor, textWriter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        private void WriteObjectClose(FieldDescriptor fd, TextWriter textWriter)
        {
            textWriter.Write('<');
            textWriter.Write('/');
            textWriter.Write(fd.ElementStart);
            textWriter.Write('>');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="elementFieldDescriptors"></param>
        private void SerializeFields(object obj, TextWriter textWriter, TranslationContext translationContext, IEnumerable<FieldDescriptor> elementFieldDescriptors)
        {
            foreach (FieldDescriptor fd in elementFieldDescriptors)
            {
                switch (fd.FdType)
                {
                    case FieldTypes.Scalar:
                        WriteValueAsLeaf(obj, fd, textWriter, translationContext);
                        break;
                    case FieldTypes.CompositeElement:
                        Object compositeObject = fd.GetObject(obj);
                        if (compositeObject != null)
                        {
                            FieldDescriptor compositeObjectFieldDescriptor = fd.IsPolymorphic ? GetClassDescriptor(
                                    compositeObject).PseudoFieldDescriptor
                                    : fd;
                            WriteWrap(fd, textWriter, false);
                            Serialize(compositeObject, compositeObjectFieldDescriptor, textWriter, translationContext);
                            WriteWrap(fd, textWriter, true);
                        }
                        break;
                    case FieldTypes.CollectionScalar:
                    case FieldTypes.MapScalar:
                        Object scalarCollectionObject = fd.GetObject(obj);
                        ICollection scalarCollection = XmlTools.GetCollection(scalarCollectionObject);
                        if (scalarCollection != null && scalarCollection.Count > 0)
                        {
                            WriteWrap(fd, textWriter, false);

                            foreach (Object collectionScalar in scalarCollection)
                            {
                                WriteScalarCollectionLeaf(collectionScalar, fd, textWriter, translationContext);
                            }
                            WriteWrap(fd, textWriter, true);
                        }
                        break;
                    case FieldTypes.CollectionElement:
                    case FieldTypes.MapElement:
                        Object compositeCollectionObject = fd.GetObject(obj);
                        ICollection compositeCollection = XmlTools.GetCollection(compositeCollectionObject);
                        if (compositeCollection != null && compositeCollection.Count > 0)
                        {
                            WriteWrap(fd, textWriter, false);
                            foreach (Object collectionComposite in compositeCollection)
                            {
                                FieldDescriptor collectionObjectFieldDescriptor = fd.IsPolymorphic ? GetClassDescriptor(
                                        collectionComposite).PseudoFieldDescriptor
                                        : fd;
                                Serialize(collectionComposite, collectionObjectFieldDescriptor, textWriter,
                                        translationContext);
                            }
                            WriteWrap(fd, textWriter, true);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private void WriteScalarCollectionLeaf(object obj, FieldDescriptor fd, TextWriter textWriter, TranslationContext translationContext)
        {
            if (!fd.IsDefaultValue(obj.ToString()))
            {
                textWriter.Write('<');
                textWriter.Write(fd.ElementStart);
                textWriter.Write('>');
                fd.AppendCollectionScalarValue(textWriter, obj, translationContext, Format.Xml);
                textWriter.Write('<');
                textWriter.Write('/');
                textWriter.Write(fd.ElementStart);
                textWriter.Write('>');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="close"></param>
        private void WriteWrap(FieldDescriptor fd, TextWriter textWriter, bool close)
        {
            if (fd.IsWrapped)
            {
                textWriter.Write('<');
                if (close)
                    textWriter.Write('/');
                textWriter.Write(fd.TagName);
                textWriter.Write('>');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private void WriteValueAsLeaf(object obj, FieldDescriptor fd, TextWriter textWriter, TranslationContext translationContext)
        {
            if (!fd.IsDefaultValueFromContext(obj))
            {
                textWriter.Write('<');
                textWriter.Write(fd.ElementStart);
                textWriter.Write('>');
                fd.AppendValue(textWriter, obj, translationContext, Format.Xml);
                textWriter.Write('<');
                textWriter.Write('/');
                textWriter.Write(fd.ElementStart);
                textWriter.Write('>');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        private void WriteValueAsText(object obj, FieldDescriptor fd, TextWriter textWriter)
        {
            if (!fd.IsDefaultValueFromContext(obj))
            {
                if (fd.IsCdata)
                    textWriter.Write(StartCdata);
                fd.AppendValue(textWriter, obj, null, Format.Xml);
                if (fd.IsCdata)
                    textWriter.Write(EndCdata);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        private void WriteClose(TextWriter textWriter)
        {
            textWriter.Write('>');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="rootObjectClassDescrpitor"></param>
        private void SerializedAttributes(object obj, TextWriter textWriter, TranslationContext translationContext, ClassDescriptor rootObjectClassDescrpitor)
        {
            List<FieldDescriptor> attributeFieldDescriptors = rootObjectClassDescrpitor
                .AttributeFieldDescriptors;

            foreach (FieldDescriptor childFd in attributeFieldDescriptors)
            {
                try
                {
                    WriteValueAsAtrribute(obj, childFd, textWriter, translationContext);
                }
                catch (Exception ex)
                {
                    throw new Exception("serialize for attribute " + obj, ex);
                }
            }

            if (SimplTypesScope.graphSwitch == SimplTypesScope.GRAPH_SWITCH.ON)
            {
                if (translationContext.NeedsHashCode(obj))
                {
                    WriteSimplIdAttribute(obj, textWriter);
                }

                if (_isRoot && translationContext.IsGraph)
                {
                    WriteSimplNameSpace(textWriter);
                    _isRoot = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        private static void WriteSimplNameSpace(TextWriter textWriter)
        {
            textWriter.Write(TranslationContext.SimplNamespace);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        private static void WriteSimplIdAttribute(object obj, TextWriter textWriter)
        {
            textWriter.Write(' ');
            textWriter.Write(TranslationContext.SimplId);
            textWriter.Write('=');
            textWriter.Write('"');
            textWriter.Write(obj.GetHashCode().ToString());
            textWriter.Write('"');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private static void WriteValueAsAtrribute(object obj, FieldDescriptor fd, TextWriter textWriter, TranslationContext translationContext)
        {
            if (obj != null)
            {
                if (!fd.IsDefaultValueFromContext(obj))
                {
                    textWriter.Write(' ');
                    textWriter.Write(fd.TagName);
                    textWriter.Write('=');
                    textWriter.Write('"');

                    fd.AppendValue(textWriter, obj, translationContext, Format.Xml);

                    textWriter.Write('"');
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        private static void WriteSimplRef(object obj, FieldDescriptor fd, TextWriter textWriter)
        {
            WriteObjectStart(fd, textWriter);
            WriteSimpRefAttribute(obj, textWriter);
            WriteCompleteClose(textWriter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        private static void WriteCompleteClose(TextWriter textWriter)
        {
            textWriter.Write('/');
            textWriter.Write('>');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        private static void WriteSimpRefAttribute(object obj, TextWriter textWriter)
        {
            textWriter.Write(' ');
            textWriter.Write(TranslationContext.SimplRef);
            textWriter.Write('=');
            textWriter.Write('"');
            textWriter.Write(obj.GetHashCode().ToString());
            textWriter.Write('"');
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        private static void WriteObjectStart(FieldDescriptor fd, TextWriter textWriter)
        {
            textWriter.Write('<');
            textWriter.Write(fd.ElementStart);
        }
    }
}
