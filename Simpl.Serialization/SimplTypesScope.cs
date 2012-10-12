using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Simpl.Fundamental.Net;
using Simpl.Fundamental.PlatformSpecifics;
using Simpl.Serialization.Context;
using Simpl.Serialization.Deserializers.PullHandlers;
using Simpl.Serialization.Deserializers.PullHandlers.StringFormats;
using Simpl.Serialization.PlatformSpecifics;
using Simpl.Serialization.Serializers;
using Simpl.Serialization.Serializers.StringFormats;
using Ecologylab.Collections;
using System.IO;

namespace Simpl.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class SimplTypesScope
    {
        /// <summary>
        /// 
        /// </summary>
        public const String STATE = "State";

        private static Dictionary<string, SimplTypesScope> allTranslationScopes = new Dictionary<string, SimplTypesScope>();

        private String name = null;
        private SimplTypesScope[] _inheritedSimplTypesScopes = null;
        private Scope<ClassDescriptor> entriesByClassSimpleName = new Scope<ClassDescriptor>();
        private Scope<ClassDescriptor>  entriesByClassName = new Scope<ClassDescriptor>();
        private Scope<ClassDescriptor> entriesByTag = new Scope<ClassDescriptor>();
        private Scope<Type> nameSpaceClassesByURN = new Scope<Type>();

        private List<ClassDescriptor> classDescriptors;

        /// <summary>
        /// Graph Switch enum
        /// </summary>
        public enum GRAPH_SWITCH
        {
            ON, OFF
        }

        /// <summary>
        /// Initialising graph switch to be off by default
        /// </summary>
        public static GRAPH_SWITCH graphSwitch = GRAPH_SWITCH.OFF;

        /// <summary>
        /// 
        /// </summary>
        public SimplTypesScope()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public SimplTypesScope(String name)
        {
            this.name = name;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedSimplTypesScope"></param>
        public SimplTypesScope(String name, SimplTypesScope inheritedSimplTypesScope)
            : this(name)
        {
            AddTranslations(inheritedSimplTypesScope);
            SimplTypesScope[] _inheritedSimplTypesScopes = new SimplTypesScope[1];
            _inheritedSimplTypesScopes[0] = inheritedSimplTypesScope;
            this._inheritedSimplTypesScopes = _inheritedSimplTypesScopes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_inheritedSimplTypesScopes"></param>
        public SimplTypesScope(String name, params SimplTypesScope[] _inheritedSimplTypesScopes)
            : this(name)
        {

            if (_inheritedSimplTypesScopes != null)
            {
                this._inheritedSimplTypesScopes = _inheritedSimplTypesScopes;
                int n = _inheritedSimplTypesScopes.Length;
                for (int i = 0; i < n; i++)
                    AddTranslations(_inheritedSimplTypesScopes[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baseTranslationsSet"></param>
        public SimplTypesScope(String name, List<SimplTypesScope> baseTranslationsSet)
            : this(name)
        {
            foreach (SimplTypesScope thatTranslationScope in baseTranslationsSet)
                AddTranslations(thatTranslationScope);
             _inheritedSimplTypesScopes		= (SimplTypesScope[]) baseTranslationsSet.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="translations"></param>
        public SimplTypesScope(String name, params Type[] translations)
            : this(name, (SimplTypesScope[])null, translations)
        {
            AddTranslationScope(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_inheritedSimplTypesScopes"></param>
        /// <param name="translations"></param>
        public SimplTypesScope(String name, SimplTypesScope[] _inheritedSimplTypesScopes, Type[] translations)
            : this(name, _inheritedSimplTypesScopes)
        {   
	         AddTranslations(translations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedTranslationsSet"></param>
        /// <param name="translations"></param>
        public SimplTypesScope(String name, List<SimplTypesScope> inheritedTranslationsSet, Type[] translations)
            : this(name, inheritedTranslationsSet)
        {

            AddTranslations(translations);

            AddTranslationScope(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedSimplTypesScope"></param>
        /// <param name="translation"></param>
        public SimplTypesScope(String name, SimplTypesScope inheritedSimplTypesScope, Type translation)
            : this(name, inheritedSimplTypesScope)
        {
            AddTranslation(translation);
            AddTranslationScope(name);
        }

        private void AddTranslationScope(string name)
        {
            if (allTranslationScopes.ContainsKey(name))
            {
                allTranslationScopes.Remove(name);
            }
            allTranslationScopes.Add(name, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedSimplTypesScope"></param>
        /// <param name="translations"></param>
        private SimplTypesScope(String name, SimplTypesScope inheritedSimplTypesScope, Type[] translations)
            : this(name, inheritedSimplTypesScope)
        {

            AddTranslations(translations);
            AddTranslationScope(name);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classes"></param>
        private void AddTranslations(Type[] classes)
        {
            if (classes != null)
            {
                int numClasses = classes.Length;
                for (int i = 0; i < numClasses; i++)
                {
                    Type thatClass = classes[i];
                    AddTranslation(thatClass);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inheritedSimplTypesScope"></param>
        public void AddTranslations(SimplTypesScope inheritedSimplTypesScope)
        {
            if (inheritedSimplTypesScope != null)
            {
                UpdateMapWithValues(inheritedSimplTypesScope.EntriesByClassSimpleName, EntriesByClassSimpleName, "classSimpleName");
                UpdateMapWithValues(inheritedSimplTypesScope.EntriesByClassName, EntriesByClassName, "className");
                UpdateMapWithValues(inheritedSimplTypesScope.EntriesByTag, EntriesByTag, "tagName");

                Dictionary<string, Type> inheritedNameSpaceClassesByURN = inheritedSimplTypesScope.nameSpaceClassesByURN;
                if (inheritedNameSpaceClassesByURN != null)
                {
                    foreach (String urn in inheritedNameSpaceClassesByURN.Keys)
                    {
                        Type valueToAdd = null;

                        if (inheritedNameSpaceClassesByURN.TryGetValue(urn, out valueToAdd))
                            nameSpaceClassesByURN.Add(urn, valueToAdd);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inheritedMap"></param>
        /// <param name="newMap"></param>
        /// <param name="warn"></param>
        private void UpdateMapWithValues(Scope<ClassDescriptor> inheritedMap, Scope<ClassDescriptor> newMap, string warn)
        {
            foreach (String key in inheritedMap.Keys)
            {
                ClassDescriptor translationEntry = null;
                if(inheritedMap.TryGetValue(key, out translationEntry))
                    UpdateMapWithEntry(newMap, key, translationEntry, warn);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newMap"></param>
        /// <param name="key"></param>
        /// <param name="translationEntry"></param>
        /// <param name="warn"></param>
        private void UpdateMapWithEntry(Scope<ClassDescriptor> newMap, string key, ClassDescriptor translationEntry, string warn)
        {
            ClassDescriptor existingEntry = null;

            Boolean entryExists = newMap.TryGetValue(key, out existingEntry);
            Boolean newEntry = !entryExists ? true : existingEntry.DescribedClass != translationEntry.DescribedClass;

            if (newEntry)
            {
                if (entryExists)
                    Debug.WriteLine("Overriding " + warn + " " + key + " with " + translationEntry);

                newMap.Add(key, translationEntry);
            }
        }


        public void AddTranslation(ClassDescriptor entry)
        {
            if (!EntriesByTag.ContainsKey(entry.TagName)) 
                EntriesByTag.Add(entry.TagName, entry);
            if (!EntriesByClassSimpleName.ContainsKey(entry.DescribedClassSimpleName))
                EntriesByClassSimpleName.Add(entry.DescribedClassSimpleName, entry);
            if (!EntriesByClassName.ContainsKey(entry.DescribedClassSimpleName))
                EntriesByClassName.Add(entry.DescribedClassSimpleName, entry);

            String[] otherTags = XmlTools.OtherTags(entry.DescribedClass);
            if(otherTags != null)
                foreach (string otherTag in otherTags.Where(otherTag => !string.IsNullOrEmpty(otherTag)))
                {
                    EntriesByTag.Add(otherTag, entry);
                }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        public void AddTranslation(Type thatClass)
        {
            ClassDescriptor entry = ClassDescriptor.GetClassDescriptor(thatClass);
            AddTranslation(entry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        public static SimplTypesScope Get(string name, params Type[] translations)
        {
            SimplTypesScope result = null;
            if (!allTranslationScopes.TryGetValue(name, out result))
            {
                result = new SimplTypesScope(name, translations);
            }
            return result;
        }

        public static SimplTypesScope Get(string name, SimplTypesScope inheritedScope, params Type[] translations)
        {
            SimplTypesScope result = null;
            if (!allTranslationScopes.TryGetValue(name, out result))
            {
                result = new SimplTypesScope(name, inheritedScope, translations);
            }
            return result;
        }

        public static SimplTypesScope Get(string name, SimplTypesScope[] inheritedScopes, params Type[] translations)
        {
            SimplTypesScope result = null;
            if (!allTranslationScopes.TryGetValue(name, out result))
            {
                result = new SimplTypesScope(name, inheritedScopes, translations);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        public static SimplTypesScope Get(string name)
        {
            return Lookup(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<ClassDescriptor> ClassDescriptors
        {
            get
            {
                List<ClassDescriptor> result = classDescriptors;
                if (result == null)
                {
                    result = EntriesByClassSimpleName.Values.ToList();
                    this.classDescriptors = result;
                }
                return result;
            }
        }

        public Scope<ClassDescriptor> EntriesByClassName
        {
            get { return entriesByClassName; }
            set { entriesByClassName = value; }
        }

        public Scope<ClassDescriptor> EntriesByClassSimpleName
        {
            get { return entriesByClassSimpleName; }
            set { entriesByClassSimpleName = value; }
        }

        public Scope<ClassDescriptor> EntriesByTag
        {
            get { return entriesByTag; }
            private set { entriesByTag = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public ClassDescriptor GetClassDescriptorByTag(string tagName)
        {
            ClassDescriptor result = null;
            EntriesByTag.TryGetValue(tagName, out result);
            return result;
        }

        /// <summary>
        /// Gets the Type of the tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Type GetClassByTag(String tag)
        {
            ClassDescriptor entry = GetClassDescriptorByTag(tag);
            return entry == null ? null : entry.DescribedClass;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ClassDescriptor> GetClassDescriptors()
        {
            List<ClassDescriptor> result = classDescriptors;
            if (result == null)
            {
                // result = entriesByClassSimpleName.values();
                result = EntriesByTag.Values.ToList(); // we use entriesByTag so that overriding works well.
                
                this.classDescriptors = result;
            }
            return result;
        }

        public SimplTypesScope GetAssignableSubset(string newName, Type superClassCriterion)
        {
            SimplTypesScope result = Lookup(newName);
            if (result == null)
            {
                result = Lookup(newName);
                if (result == null)
                {
                    result = new SimplTypesScope(newName);
                    AddTranslationScope(newName);
                    foreach (ClassDescriptor classDescriptor in EntriesByClassName.Values)
                    {
                        Type thatClass = classDescriptor.DescribedClass;
                        if (superClassCriterion.GetTypeInfo().IsAssignableFrom(thatClass.GetTypeInfo()))
                            result.AddTranslation(thatClass);
                    }
                }
            }
            return result;
        }

        public SimplTypesScope GetSubtractedSubset(string newName, Type superClassCriterion)
        {
            SimplTypesScope result = Lookup(newName);
            if (result == null)
            {
                result = Lookup(newName);
                if (result == null)
                {
                    result = new SimplTypesScope(newName);
                    AddTranslationScope(newName);
                    foreach (ClassDescriptor classDescriptor in EntriesByClassName.Values)
                    {
                        Type thatClass = classDescriptor.DescribedClass;
                        if (!superClassCriterion.GetTypeInfo().IsAssignableFrom(thatClass.GetTypeInfo()))
                            result.AddTranslation(thatClass);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static SimplTypesScope Lookup(String name)
        {
            SimplTypesScope result = null;
            allTranslationScopes.TryGetValue(name, out result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void EnableGraphSerialization()
        {
            graphSwitch = GRAPH_SWITCH.ON;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void DisableGraphSerialization()
        {
            graphSwitch = GRAPH_SWITCH.OFF;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task<object> DeserializeFile(string filename, Format format, Encoding encoding = null)
        {
            Task<object> fileTask = FundamentalPlatformSpecifics.Get().CreateFile(filename);
            object file = await fileTask;
            return Deserialize(file, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="format"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public object Deserialize(object file, Format format, Encoding encoding = null)
        {
            return Deserialize(file, new TranslationContext(file), null, format, encoding);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="translationContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        /// <param name="format"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task<object> Deserialize(object file, TranslationContext translationContext, IDeserializationHookStrategy deserializationHookStrategy, Format format, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            Stream readStream = await FundamentalPlatformSpecifics.Get().OpenFileReadStream(file, encoding);
            return Deserialize(readStream, translationContext, deserializationHookStrategy, format);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public Object Deserialize(Stream inputStream, Format format)
        {
            PullDeserializer pullDeserializer = PullDeserializer.GetPullDeserializer(this, new TranslationContext(), 
                                                                                     null, format);
            return pullDeserializer.Parse(inputStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="translationContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public object Deserialize(Stream inputStream, TranslationContext translationContext, IDeserializationHookStrategy deserializationHookStrategy, Format format)
        {
            PullDeserializer pullDeserializer = PullDeserializer.GetPullDeserializer(this, translationContext,
                                                                                     deserializationHookStrategy, format);
            return pullDeserializer.Parse(inputStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public Object Deserialize(String inputString, StringFormat format)
        {
            return Deserialize(inputString, new TranslationContext(), null, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="translationContext"></param>
        /// <param name="deserializationHookStrategy"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public Object Deserialize(String inputString, TranslationContext translationContext, IDeserializationHookStrategy deserializationHookStrategy, StringFormat format)
        {
            StringPullDeserializer pullDeserializer = PullDeserializer.GetStringDeserializer(this, translationContext,
                                                                                             deserializationHookStrategy,
                                                                                             format);

            return pullDeserializer.Parse(inputString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="file"></param>
        /// <param name="format"></param>
        public static void Serialize(object obj, object file, Format format)
        {
            Serialize(obj, file, new TranslationContext(), format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="file"></param>
        /// <param name="translationContext"></param>
        /// <param name="format"></param>
        public async static void Serialize(object obj, object file, TranslationContext translationContext, Format format)
        {
            Stream writeStream = await FundamentalPlatformSpecifics.Get().OpenFileWriteStream(file);
            Serialize(obj, writeStream, translationContext, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stringBuilder"></param>
        /// <param name="format"></param>
        public static void Serialize(object obj, StringBuilder stringBuilder, StringFormat format)
        {
            Serialize(obj, stringBuilder, new TranslationContext(), format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stringBuilder"></param>
        /// <param name="translationContext"></param>
        /// <param name="format"></param>
        public static void Serialize(object obj, StringBuilder stringBuilder, TranslationContext translationContext, StringFormat format)
        {
            StringSerializer stringSerializer = FormatSerializer.GetStringSerializer(format);
            stringSerializer.Serialize(obj, stringBuilder, translationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static String Serialize(object obj, StringFormat format)
        {
            return Serialize(obj, new TranslationContext(), format);
        }

        public static String Serialize(object obj, TranslationContext translationContext, StringFormat format)
        {
            StringSerializer stringSerializer = FormatSerializer.GetStringSerializer(format);
            return stringSerializer.Serialize(obj, translationContext);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="translationContext"></param>
        /// <param name="format"></param>
        public static void Serialize(object obj, Stream stream, TranslationContext translationContext, Format format)
        {
            FormatSerializer serializer = FormatSerializer.GetSerializer(format);
            serializer.Serialize(obj, stream, translationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="translationContext"></param>
        /// <param name="format"></param>
        public static void Serialize(object obj, TextWriter textWriter, TranslationContext translationContext, StringFormat format)
        {
            StringSerializer serializer = FormatSerializer.GetStringSerializer(format);
            serializer.Serialize(obj, textWriter, translationContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        public static void Serialize(object obj, Stream stream, Format format)
        {
           Serialize(obj, stream, new TranslationContext(), format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        /// <param name="format"></param>
        public static void Serialize(object obj, TextWriter textWriter, StringFormat format)
        {
           Serialize(obj, textWriter, new TranslationContext(), format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockType"></param>
        /// <returns></returns>
        public ClassDescriptor GetClassDescriptorByTlvId(int blockType)
        {
            throw new NotImplementedException();
        }
    }   
}
