using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.collections;
using System.Reflection;

namespace ecologylabFundamental.ecologylab.xml
{
    public class TranslationScope
    {
        private String name;
        private TranslationScope[] inheritedTranslationScopes;

        private Scope<ClassDescriptor> entriesByClassSimpleName = new Scope<ClassDescriptor>();

        private Scope<ClassDescriptor> entriesByClassName = new Scope<ClassDescriptor>();
        private Scope<ClassDescriptor> entriesByTag = new Scope<ClassDescriptor>();

        private Scope<Type> nameSpaceClassesByURN = new Scope<Type>();

        private static Dictionary<String, TranslationScope> allTranslationScopes = new Dictionary<String, TranslationScope>();
        private TranslationScope[] translationScope;
        private Type[] translations;
        private List<ClassDescriptor> classDescriptors;

        public const String STATE = "State";

        /// <summary>
        /// 
        /// </summary>
        public TranslationScope()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private TranslationScope(String name)
        {
            this.name = name;
            allTranslationScopes.Add(name, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedTranslationScope"></param>
        private TranslationScope(String name, TranslationScope inheritedTranslationScope)
            : this(name)
        {
            AddTranslations(inheritedTranslationScope);
            TranslationScope[] inheritedTranslationScopes = new TranslationScope[1];
            inheritedTranslationScopes[0] = inheritedTranslationScope;
            this.inheritedTranslationScopes = inheritedTranslationScopes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedTranslationScopes"></param>
        private TranslationScope(String name, params TranslationScope[] inheritedTranslationScopes)
            : this(name)
        {

            if (inheritedTranslationScopes != null)
            {
                this.inheritedTranslationScopes = inheritedTranslationScopes;
                int n = inheritedTranslationScopes.Length;
                for (int i = 0; i < n; i++)
                    AddTranslations(inheritedTranslationScopes[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baseTranslationsSet"></param>
        private TranslationScope(String name, List<TranslationScope> baseTranslationsSet)
            : this(name)
        {
            foreach (TranslationScope thatTranslationScope in baseTranslationsSet)
                AddTranslations(thatTranslationScope);
             inheritedTranslationScopes		= (TranslationScope[]) baseTranslationsSet.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="translations"></param>
        private TranslationScope(String name, params Type[] translations)
            : this(name, (TranslationScope[])null, translations)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedTranslationScopes"></param>
        /// <param name="translations"></param>
        private TranslationScope(String name, TranslationScope[] inheritedTranslationScopes, Type[] translations) : this(name, inheritedTranslationScopes)
        {   
	         AddTranslations(translations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedTranslationsSet"></param>
        /// <param name="translations"></param>
        private TranslationScope(String name, List<TranslationScope> inheritedTranslationsSet, Type[] translations)
            : this(name, inheritedTranslationsSet)
        {

            AddTranslations(translations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedTranslationScope"></param>
        /// <param name="translation"></param>
        private TranslationScope(String name, TranslationScope inheritedTranslationScope, Type translation)
            : this(name, inheritedTranslationScope)
        {
            AddTranslation(translation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inheritedTranslationScope"></param>
        /// <param name="translations"></param>
        private TranslationScope(String name, TranslationScope inheritedTranslationScope, Type[] translations)
            : this(name, inheritedTranslationScope)
        {

            AddTranslations(translations);
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
        /// <param name="inheritedTranslationScope"></param>
        private void AddTranslations(TranslationScope inheritedTranslationScope)
        {
            if (inheritedTranslationScope != null)
            {
                UpdateMapWithValues(inheritedTranslationScope.entriesByClassSimpleName, entriesByClassSimpleName, "classSimpleName");
                UpdateMapWithValues(inheritedTranslationScope.entriesByClassName, entriesByClassName, "className");
                UpdateMapWithValues(inheritedTranslationScope.entriesByTag, entriesByTag, "tagName");

                Dictionary<String, Type> inheritedNameSpaceClassesByURN = inheritedTranslationScope.nameSpaceClassesByURN;
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
                    System.Console.WriteLine("Overriding " + warn + " " + key + " with " + translationEntry);

                newMap.Add(key, translationEntry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thatClass"></param>
        private void AddTranslation(Type thatClass)
        {
            ClassDescriptor entry = ClassDescriptor.GetClassDescriptor(thatClass);
            entriesByTag.Add(entry.TagName, entry);
            entriesByClassSimpleName.Add(entry.DescribedClassSimpleName, entry);
            entriesByClassName.Add(thatClass.Name, entry);

            String[] otherTags = XMLTools.OtherTags(entry.DescribedClass);
            if(otherTags != null)
                foreach (String otherTag in otherTags)
                {
                    if (otherTag != null && otherTag.Length > 0)
                    {
                        entriesByTag.Add(otherTag, entry);
                    }
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        public static TranslationScope Get(string name, params Type[] translations)
        {
            TranslationScope result = null;
            if (!allTranslationScopes.TryGetValue(name, out result))
            {
                result = new TranslationScope(name, translations);
            }
            return result;
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
                    result = entriesByClassSimpleName.Values.ToList();
                    this.classDescriptors = result;
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        internal ClassDescriptor GetClassDescriptorByTag(string tagName)
        {
            return entriesByTag[tagName];
        }
    }   
}
