using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Serializers.StringFormats
{
    public class BibtexSerializer : StringSerializer
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
        private void Serialize(object obj, FieldDescriptor rootObjectFieldDescriptor, TextWriter textWriter,
                               TranslationContext translationContext)
        {
            SerializationPreHook(obj, translationContext);

            WriteObjectStart(rootObjectFieldDescriptor, textWriter);

            IEnumerable<FieldDescriptor> allFieldDescriptors = GetClassDescriptor(obj).AllFieldDescriptors;

            SerializeFields(obj, textWriter, translationContext, allFieldDescriptors.ToList());

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
        private void SerializeFields(object obj, TextWriter textWriter, TranslationContext translationContext,
                                     IList<FieldDescriptor> allFieldDescriptors)
        {
            int numOfFields = 0;

            foreach (FieldDescriptor childFd in allFieldDescriptors)
            {
                switch (childFd.FdType)
                {
                    case FieldTypes.Scalar:
                        SerializeScalar(obj, childFd, textWriter, translationContext);
                        break;
                    case FieldTypes.CompositeElement:
                        SerializeComposite(obj, textWriter, translationContext, childFd);
                        break;
                    case FieldTypes.CollectionScalar:
                    case FieldTypes.MapScalar:
                        SerializeScalarCollection(obj, textWriter, translationContext, childFd);
                        break;
                    case FieldTypes.CollectionElement:
                    case FieldTypes.MapElement:
                        if (!childFd.IsPolymorphic)
                            SerializeCompositeCollection(obj, textWriter, translationContext, childFd);
                        break;
                }

                if (++numOfFields < allFieldDescriptors.Count)
                    textWriter.Write(',');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="fd"></param>
        private void SerializeCompositeCollection(object obj, TextWriter textWriter,
                                                  TranslationContext translationContext, FieldDescriptor fd)
        {
            Object scalarCollectionObject = fd.GetObject(obj);
            ICollection scalarCollection = XmlTools.GetCollection(scalarCollectionObject);

            String delim = "author".Equals(fd.BibtexTagName) ? " and " : translationContext.Delimiter;

            if (scalarCollection.Count > 0)
            {
                int numberOfItems = 0;

                WriteCollectionStart(fd, textWriter);
                foreach (Object collectionObject in scalarCollection)
                {
                    FieldDescriptor compositeAsScalarFd = GetClassDescriptor(collectionObject).ScalarValueFieldDescripotor;

                    if (compositeAsScalarFd != null)
                    {
                        WriteScalarBibtexAttribute(collectionObject, compositeAsScalarFd, textWriter, translationContext);
                    }

                    if (++numberOfItems < scalarCollection.Count)
                        textWriter.Write(delim);
                }
                WriteCollectionEnd(textWriter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private void WriteScalarBibtexAttribute(object obj, FieldDescriptor fd, TextWriter textWriter,
                                                TranslationContext translationContext)
        {
            if (!fd.IsDefaultValueFromContext(obj))
            {
                fd.AppendValue(textWriter, obj, translationContext, Format.Bibtex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="fd"></param>
        private void SerializeScalarCollection(object obj, TextWriter textWriter, TranslationContext translationContext,
                                               FieldDescriptor fd)
        {
            Object scalarCollectionObject = fd.GetObject(obj);
            ICollection scalarCollection = XmlTools.GetCollection(scalarCollectionObject);

            String delim = "author".Equals(fd.BibtexTagName) ? " and " : translationContext.Delimiter;

            if (scalarCollection.Count > 0)
            {
                int numberOfItems = 0;

                WriteCollectionStart(fd, textWriter);
                foreach (Object collectionObject in scalarCollection)
                {
                    WriteCollectionScalar(collectionObject, fd, textWriter, translationContext);
                    if (++numberOfItems < scalarCollection.Count)
                        textWriter.Write(delim);
                }
                WriteCollectionEnd(textWriter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionObject"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private void WriteCollectionScalar(object collectionObject, FieldDescriptor fd, TextWriter textWriter,
                                           TranslationContext translationContext)
        {
            fd.AppendCollectionScalarValue(textWriter, collectionObject, translationContext, Format.Bibtex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        private void WriteCollectionEnd(TextWriter textWriter)
        {
            textWriter.Write("}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        private void WriteCollectionStart(FieldDescriptor fd, TextWriter textWriter)
        {
            textWriter.Write(fd.TagName);
            textWriter.Write('=');
            textWriter.Write("{");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="fd"></param>
        private void SerializeComposite(object obj, TextWriter textWriter, TranslationContext translationContext,
                                        FieldDescriptor fd)
        {
            Object compositeObject = fd.GetObject(obj);
            FieldDescriptor compositeAsScalarFd = GetClassDescriptor(compositeObject).ScalarValueFieldDescripotor;

            if (compositeAsScalarFd != null)
            {
                WriteBibtexAttribute(compositeObject, fd, textWriter, translationContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private void SerializeScalar(object obj, FieldDescriptor fd, TextWriter textWriter,
                                     TranslationContext translationContext)
        {
            WriteBibtexAttribute(obj, fd, textWriter, translationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        private void WriteBibtexAttribute(object obj, FieldDescriptor fd, TextWriter textWriter,
                                          TranslationContext translationContext)
        {
            if (!fd.IsDefaultValueFromContext(obj))
            {
                if (!fd.IsBibtexKey)
                {
                    textWriter.Write(fd.BibtexTagName);
                    textWriter.Write('=');
                    textWriter.Write('{');
                }
            }

            fd.AppendValue(textWriter, obj, translationContext, Format.Bibtex);

            if (!fd.IsBibtexKey)
                textWriter.Write('}');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="textWriter"></param>
        private void WriteObjectStart(FieldDescriptor fd, TextWriter textWriter)
        {
            textWriter.Write('@');
            textWriter.Write(fd.BibtexTagName);
            textWriter.Write('{');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        private void WriteClose(TextWriter textWriter)
        {
            textWriter.Write('}');
        }
    }
}
