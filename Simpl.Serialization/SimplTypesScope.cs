using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Net;
using Simpl.Serialization.Context;
using Simpl.Serialization.Deserializers.PullHandlers;
using Simpl.Serialization.Deserializers.PullHandlers.StringFormats;
using Simpl.Serialization.Serializers;
using Simpl.Serialization.Serializers.StringFormats;
using ecologylab.collections;
using System.IO;
using ecologylab.serialization;

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

        public static void EnableGraphSerialization()
        {
            graphSwitch = GRAPH_SWITCH.ON;
        }

        public static void DisableGraphSerialization()
        {
            graphSwitch = GRAPH_SWITCH.OFF;
        }

        public Object DeserializeFile(string filename, StringFormat stringFormat)
        {
            FileStream f = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader r = new StreamReader(f);
            return Deserialize(r.ReadToEnd(), stringFormat);
        }

        public Object Deserialize(String inputString, StringFormat format)
        {
            return Deserialize(inputString, new TranslationContext(), null, format);
        }

        public Object Deserialize(String inputString, TranslationContext translationContext, IDeserializationHookStrategy deserializationHookStrategy, StringFormat format)
        {
            StringPullDeserializer pullDeserializer = PullDeserializer.GetStringDeserializer(this, translationContext,
                                                                                             deserializationHookStrategy,
                                                                                             format);

            return pullDeserializer.Parse(inputString);
        }

        public static void Serialize(Object obj, Format format, Stream stream)
        {
            FormatSerializer serializer = FormatSerializer.GetSerializer(format);
            serializer.Serialize(obj, stream);
        }

        public static void Serialize(Object obj, StringFormat format, TextWriter textWriter)
        {
            StringSerializer serializer = FormatSerializer.GetStringSerializer(format);
            serializer.Serialize(obj, textWriter);
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
					foreach(ClassDescriptor classDescriptor in EntriesByClassName.Values)
					{
					    Type thatClass = classDescriptor.DescribedClass;
						if (superClassCriterion.IsAssignableFrom(thatClass))
							result.AddTranslation(thatClass);
					}
			    }
		    }
		    return result;
        }

        
    }   
}
