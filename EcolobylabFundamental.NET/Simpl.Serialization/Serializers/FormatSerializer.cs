using System;
using System.IO;
using Simpl.Serialization.Context;
using Simpl.Serialization.Serializers.BinaryFormats;
using Simpl.Serialization.Serializers.StringFormats;
using ecologylab.serialization;


namespace Simpl.Serialization.Serializers
{
    /// <summary>
    /// FormatSerializer. an abstract base class from where format-specific serializers derive. Its main
    /// use is for exposing the API for serialization methods. It contains helper functions and wrapper
    /// serialization functions, allowing software developers to use different types of objects for
    /// serialization, such as System.out, File, StringBuilder, or return serialized data as
    /// StringBuilder
    /// </summary>
    public abstract class FormatSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        public void Serialize(Object obj, StreamWriter streamWriter)
        {
            Serialize(obj, streamWriter, new TranslationContext());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="streamWriter"></param>
        /// <param name="translationContext"></param>
        public abstract void Serialize(Object obj, StreamWriter streamWriter,
                                       TranslationContext translationContext);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileStream"></param>
        public void Serialize(Object obj, FileStream fileStream)
        {
            Serialize(obj, new StreamWriter(fileStream), new TranslationContext());
        }

        public abstract void Serialize(Object obj, FileStream fileStream,
                                       TranslationContext translationContext);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ClassDescriptor GetClassDescriptor(Object obj)
        {
            return ClassDescriptor.GetClassDescriptor(obj.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="translationContext"></param>
        protected void SerializationPostHook(Object obj, TranslationContext translationContext)
        {
            if (obj is ISimplSerializationPost)
            {
                ((ISimplSerializationPost) obj).SerializationPostHook(translationContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="translationContext"></param>
        protected void SerializationPreHook(Object obj, TranslationContext translationContext)
        {
            if (obj is ISimplSerializationPre)
            {
                ((ISimplSerializationPre) obj).SerializationPreHook(translationContext);
            }
        }


        protected Boolean AlreadySerialized(Object obj, TranslationContext translationContext)
        {
            return TranslationScope.graphSwitch == TranslationScope.GRAPH_SWITCH.ON
                   && translationContext.AlreadyMarshalled(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static FormatSerializer GetSerializer(Format format)
        {
            switch (format)
            {
                case Format.Xml:
                    return new XmlSerializer();
                case Format.Json:
                    return new JsonSerializer();
                case Format.Tlv:
                    return new TlvSerializer();
                case Format.Bibtex:
                    return new BibtexSerializer();
                default:
                    throw new Exception(format + " format not supported");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static StringSerializer GetStringSerializer(StringFormat format)
        {
            switch (format)
            {
                case StringFormat.Xml:
                    return new XmlSerializer();
                case StringFormat.Json:
                    return new JsonSerializer();
                case StringFormat.Bibtex:
                    return new BibtexSerializer();
                default:
                    throw new Exception(format + " format not supported");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static FormatSerializer GetBinarySerializer(BinaryFormat format)
        {
            switch (format)
            {
                case BinaryFormat.Tlv:
                    return new TlvSerializer();
                default:
                    throw new Exception(format + " format not supported");
            }
        }
    }
}
        