using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;
using Simpl.Serialization.Types;

namespace Simpl.Serialization.Serializers.StringFormats
{
    public class JsonSerializer : StringSerializer
    {
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
                WriteStart(textWriter);

                Serialize(obj, rootObjectClassDescriptor.PseudoFieldDescriptor, textWriter, translationContext, true);

                WriteClose(textWriter);
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
        /// <param name="withTag"></param>
        private void Serialize(object obj, FieldDescriptor rootObjectFieldDescriptor, TextWriter textWriter, TranslationContext translationContext, bool withTag)
        {
            if (obj == null)
                return;

            if (AlreadySerialized(obj, translationContext))
            {
                WriteSimplRef(obj, rootObjectFieldDescriptor, textWriter, withTag, translationContext);
                return;
            }

            translationContext.MapObject(obj);

            SerializationPreHook(obj, translationContext);

            WriteObjectStart(rootObjectFieldDescriptor, textWriter, withTag);
           
            ClassDescriptor cd = GetClassDescriptor(obj);

            SerializeFields(obj, textWriter, translationContext, cd);

            WriteClose(textWriter);

            SerializationPostHook(obj, translationContext);
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="allFieldDescriptors"></param>
        private void SerializeFields(object obj, TextWriter textWriter, TranslationContext translationContext, ClassDescriptor cd)
        {
            List<FieldDescriptor> allFieldDescriptors = cd.AllFieldDescriptors.ToList();

            if(SimplTypesScope.graphSwitch == SimplTypesScope.GRAPH_SWITCH.ON)
            {
                if(translationContext.NeedsHashCode(obj))
                {
                    WriteSimplIdAttribute(obj, textWriter, allFieldDescriptors.Count <= 0, translationContext);
                }
            }

            List<FieldDescriptor> attributeFieldDescriptors = cd.AttributeFieldDescriptors as List<FieldDescriptor>;
		    int numOfFields = SerializeFieldsHelper(textWriter, obj, translationContext, attributeFieldDescriptors, 0);
		    List<FieldDescriptor> elementFieldDescriptors = cd.ElementFieldDescriptors;
		    SerializeFieldsHelper(textWriter, obj, translationContext, elementFieldDescriptors,numOfFields);
        }

        private int SerializeFieldsHelper(TextWriter textWriter, object obj, TranslationContext translationContext,
			List<FieldDescriptor> fieldDescriptorList, int numOfFields) 
        {
            foreach (FieldDescriptor fd in fieldDescriptorList)
            {
                if(IsSerializable(fd, obj))
                {
                    if (numOfFields++ > 0)
                        textWriter.Write(',');

                    switch (fd.FdType)
                    {
                        case FieldTypes.Scalar:
                            SerializeScalar(obj, fd, textWriter, translationContext);
                            break;
                        case FieldTypes.CompositeElement:
                            SerializeComposite(obj, textWriter, translationContext, fd);
                            break;
                        case FieldTypes.CollectionScalar:
                        case FieldTypes.MapScalar:
                            SerializeScalarCollection(obj, textWriter, translationContext, fd);
                            break;
                        case FieldTypes.CollectionElement:
                        case FieldTypes.MapElement:
                            if (fd.IsPolymorphic)
                                SerializePolymorphicCollection(obj, textWriter, translationContext, fd);
                            else
                                SerializeCompositeCollection(obj, textWriter, translationContext, fd);
                            break;
                        default:
                            if (numOfFields > 0)
                                numOfFields = numOfFields;
                            break;

                    }
                }
            }

            return numOfFields;
        }

        private void SerializeCompositeCollection(object obj, TextWriter textWriter, TranslationContext translationContext, FieldDescriptor fd)
        {
            Object collectionObject = fd.GetObject(obj);
            ICollection compositeCollection = XmlTools.GetCollection(collectionObject);
            int numberOfItems = 0;

            //WriteWrap(fd, textWriter, false);
            var tagName = fd.IsWrapped ? fd.TagName : fd.CollectionOrMapTagName;
            WriteCollectionStart(tagName, textWriter);
            foreach (Object collectionComposite in compositeCollection)
            {
                FieldDescriptor collectionObjectFieldDescriptor = fd.IsPolymorphic
                                                                      ? GetClassDescriptor(
                                                                          collectionComposite).PseudoFieldDescriptor
                                                                      : fd;

                Serialize(collectionComposite, collectionObjectFieldDescriptor, textWriter,
                          translationContext, false);

                if (++numberOfItems < compositeCollection.Count)
                    textWriter.Write(',');
            }
            WriteCollectionEnd(textWriter);
            //WriteWrap(fd, textWriter, true);
        }

        private void WriteCollectionEnd(TextWriter textWriter)
        {
            textWriter.Write(']');
        }

        private void WriteCollectionStart(String collectionFieldTag, TextWriter textWriter)
        {
            textWriter.Write('"');
            textWriter.Write(collectionFieldTag);
            textWriter.Write('"');
            textWriter.Write(':');
            textWriter.Write('[');
        }

        private void SerializePolymorphicCollection(object obj, TextWriter textWriter, TranslationContext translationContext, FieldDescriptor fd)
        {
            Object collectionObject = fd.GetObject(obj);
            ICollection compositeCollection = XmlTools.GetCollection(collectionObject);
            int numberOfItems = 0;

            WriteCollectionStart(fd.TagName, textWriter);
            foreach (Object collectionComposite in compositeCollection)
            {
                FieldDescriptor collectionObjectFieldDescriptor = fd.IsPolymorphic
                                                                      ? GetClassDescriptor(
                                                                          collectionComposite).PseudoFieldDescriptor
                                                                      : fd;

                WriteStart(textWriter);
                Serialize(collectionComposite, collectionObjectFieldDescriptor, textWriter,
                          translationContext, true);
                WriteClose(textWriter);

                if (++numberOfItems < compositeCollection.Count)
                    textWriter.Write(',');
            }
            WriteCollectionEnd(textWriter);
        }

        private void WritePolymorphicCollectionStart(FieldDescriptor fd, TextWriter textWriter)
        {
            textWriter.Write('"');
            textWriter.Write(fd.TagName);
            textWriter.Write('"');
            textWriter.Write(':');
            textWriter.Write('[');
        }

        private void SerializeScalarCollection(object obj, TextWriter textWriter, TranslationContext translationContext, FieldDescriptor fd)
        {
            Object scalarCollectionObject = fd.GetObject(obj);
            ICollection scalarCollection = XmlTools.GetCollection(scalarCollectionObject);
            int numberOfItems = 0;

            //WriteWrap(fd, textWriter, false);
            WriteCollectionStart(fd.TagName, textWriter);
            foreach (Object collectionObject in scalarCollection)
            {
                WriteCollectionScalar(collectionObject, fd, textWriter, translationContext);
                if (++numberOfItems < scalarCollection.Count)
                    textWriter.Write(',');
            }
            WriteCollectionEnd(textWriter);
            //WriteWrap(fd, textWriter, true);
        }

        private void WriteCollectionScalar(object obj, FieldDescriptor fd, TextWriter textWriter, TranslationContext translationContext)
        {
            if (fd.ScalarType.NeedsJsonQuotationWrap())
                textWriter.Write('"');
            fd.AppendCollectionScalarValue(textWriter, obj, translationContext, Format.Json);
            if (fd.ScalarType.NeedsJsonQuotationWrap())
                textWriter.Write('"');
        }

        private void SerializeComposite(object obj, TextWriter textWriter, TranslationContext translationContext, FieldDescriptor fd)
        {
            Object compositeObject = fd.GetValue(obj);
            FieldDescriptor compositeObjectFieldDescriptor = fd.IsPolymorphic
                                                                 ? GetClassDescriptor(
                                                                     compositeObject).PseudoFieldDescriptor
                                                                 : fd;
            WriteWrap(fd, textWriter, false);
            Serialize(compositeObject, compositeObjectFieldDescriptor, textWriter, translationContext, true);
            WriteWrap(fd, textWriter, true);
        }

        private static void SerializeScalar(object obj, FieldDescriptor fd, TextWriter textWriter, TranslationContext translationContext)
        {
            // check wether we need quotation marks to surround the value.
            bool needQuotationMarks = true;
            ScalarType st = fd.ScalarType;
            if (st != null)
            {
                needQuotationMarks = st.NeedsJsonQuotationWrap();
            }

            textWriter.Write('"');
            textWriter.Write(fd.TagName);
            textWriter.Write('"');
            textWriter.Write(':');
            if (needQuotationMarks)
                textWriter.Write('"');
            fd.AppendValue(textWriter, obj, translationContext, Format.Json);
            if (needQuotationMarks)
                textWriter.Write('"');
        }

        private static bool IsSerializable(FieldDescriptor fd, object obj)
        {
            switch (fd.FdType)
            {
                case FieldTypes.Scalar:
                    if (fd.IsDefaultValueFromContext(obj))
                        return false;
                    break;
                case FieldTypes.CompositeElement:
                case FieldTypes.CollectionElement:
                case FieldTypes.MapElement:
                    Object tempObj = fd.GetObject(obj);
                    if (tempObj == null)
                        return false;
                    break;
                case FieldTypes.CollectionScalar:
                case FieldTypes.MapScalar:
                    Object scalarCollectionObject = fd.GetObject(obj);
                    if (scalarCollectionObject == null)
                        return false;
                    ICollection scalarCollection = XmlTools.GetCollection(scalarCollectionObject);
                    if (scalarCollection == null || scalarCollection.Count <= 0)
                        return false;
                    break;
                case FieldTypes.IgnoredElement:
                case FieldTypes.IgnoredAttribute:
                    return false;
                default:
                    break;
            }

            return true;
        }

        private static void WriteWrap(FieldDescriptor fd, TextWriter textWriter, bool close)
        {
            if (fd.IsWrapped)
            {
                if (!close)
                {
                    textWriter.Write('"');
                    textWriter.Write(fd.TagName);
                    textWriter.Write('"');
                    textWriter.Write(':');
                    textWriter.Write('{');
                }
                else
                {
                    textWriter.Write('}');
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        private static void WriteClose(TextWriter textWriter)
        {
            textWriter.Write('}');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        private static void WriteStart(TextWriter textWriter)
        {
            textWriter.Write('{');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        private static void WriteSimplIdAttribute(object obj, TextWriter textWriter, Boolean last, TranslationContext translationContext)
        {
            textWriter.Write('"');
            textWriter.Write(TranslationContext.JsonSimplId);
            textWriter.Write('"');
            textWriter.Write(':');
            //textWriter.Write('"');
            textWriter.Write(translationContext.GetSimplId(obj));

            if (!last)
            {
                //textWriter.Write('"');
                textWriter.Write(',');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="withTag"></param>
        /// <param name="translationContext"></param>
        private static void WriteSimplRef(object obj, FieldDescriptor fd, TextWriter textWriter, Boolean withTag, TranslationContext translationContext)
        {
            WriteObjectStart(fd, textWriter, withTag);
            WriteSimpRefAttribute(obj, textWriter, translationContext);
            WriteClose(textWriter);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private static void WriteSimpRefAttribute(object obj, TextWriter textWriter, TranslationContext translationContext)
        {
            textWriter.Write('"');
            textWriter.Write(TranslationContext.JsonSimplRef);
            textWriter.Write('"');
            textWriter.Write(':');
            //textWriter.Write('"');
            textWriter.Write(translationContext.GetSimplId(obj));
            //textWriter.Write('"');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="withTag"></param>
        private static void WriteObjectStart(FieldDescriptor fd, TextWriter textWriter, Boolean withTag)
        {
            if (withTag)
            {
                textWriter.Write('"');
                textWriter.Write(fd.ElementStart);
                textWriter.Write('"');
                textWriter.Write(':');
            }
            textWriter.Write('{');
        }
    }
}