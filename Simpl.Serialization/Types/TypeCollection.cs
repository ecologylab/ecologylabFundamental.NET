using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.Types
{
    /// <summary>
    /// A class that represents a collection of different SimplTypes, searchable by their corresponding type name in a target language.
    /// </summary>
    public class TypeCollection<SomeType> where SomeType :SimplType
    {
        /// A list of name type collection indexers that index by name. 
        /// <summary>
        /// </summary>
        private List<TypeCollectionIndexer<string, SomeType>> nameindexers;
     
        /// <summary>
        /// A default lambda to create a new type entry in this TypeCollection.
        /// </summary>
        private Func<Type, SomeType> createNewSomeType = (t) => { throw new InvalidOperationException(string.Format("Type {0} missing! Addition of new SimplTypes is not a valid operation for {1}",t.Name,typeof(SomeType).Name)); };

        /// <summary>
        /// Initializes an instance of <see cref="TypeCollection"/> with a given creation predicate
        /// </summary>
        /// <param name="creationPredicate">A predicate that produces the SimplType for a given Type</param>
        public TypeCollection(Func<Type, SomeType> creationPredicate) : this()
        {
            this.createNewSomeType = creationPredicate;
        }

        /// <summary>
        /// Initializes an instance of <see cref="TypeCollection"/> with the default creation predicate (which throws an error on type creation.)
        /// </summary>
        public TypeCollection()
        {
            this.nameindexers = new List<TypeCollectionIndexer<string, SomeType>>();

            this.JavaName = new TypeCollectionIndexer<string, SomeType>((t) => t.JavaTypeName);
            this.SimpleName = new TypeCollectionIndexer<string, SomeType>((t) => t.SimplName);
            this.CSharpName = new TypeCollectionIndexer<string, SomeType>((t) => t.CSharpTypeName);
            this.ObjectiveCName = new TypeCollectionIndexer<string, SomeType>((t) => t.ObjectiveCTypeName);
            this.DBName = new TypeCollectionIndexer<string, SomeType>((t) => t.DbTypeName);
            this.CSharpType = new TypeCollectionIndexer<Type, SomeType>((t) => t.CSharpType);
            this.CrossPlatformName = new TypeCollectionIndexer<string, SomeType>((t) => SimplType.DeriveCrossPlatformName(t.CSharpType, t is ScalarType));

            nameindexers.AddRange(new[] { CSharpName, JavaName, SimpleName, CrossPlatformName, ObjectiveCName, DBName });
        }


        // TODO: Possible perf benifits can be had here if we allow some sort of caching.

        /// <summary>
        /// Indexes through the indexers in this collection... if one of the indexers has the given type name, returns that type.
        /// </summary>
        /// <param name="theName">The name to obtain a colletion type for</param>
        /// <returns>The given type. Or throws an exception if the operation doesn't </returns>
        public SomeType this[string theName]
        {
            get
            {
                foreach (var indexer in nameindexers)
                {
                    if (indexer.Contains(theName))
                    {
                        return indexer[theName];
                    }
                }

                throw new InvalidOperationException(String.Format("Key name {0} not in type collection.",theName));
            }
        }

        /// <summary>
        /// Obtains or Adds the corresponding SimplType for a given Type
        /// Calls the creation predicate for this type collection
        /// </summary>
        /// <param name="theType">The type to obtain a simplType for</param>
        /// <returns>The simplType or an exception if the operation is not currently supported</returns>
        public SomeType GetOrAdd(Type theType)
        {
            if (this.CSharpType.Contains(theType))
            {
                return this.CSharpType[theType];
            }
            else
            {
                // Create an instance of the type
                var newInstance = createNewSomeType(theType);

                // Add it to the collection
                this.TryAdd(newInstance);

                // Return the created instance
                return newInstance;
            }
        }

        /// <summary>
        /// Indexes the current TypeCollection; does not attempt to create the requested type.
        /// </summary>
        /// <param name="theType">The type to attempt to obtain a SimplType for </param>
        /// <returns>The type, or an exception if the type does not exist</returns>
        public SomeType this[Type theType]
        {
            get
            {
                if (this.CSharpType.Contains(theType))
                {
                    return this.CSharpType[theType];
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Type {0} does not exist in the registry.", theType.Name));
                }
            }
        }

        /// <summary>
        /// An indexer that indexes this type collection by JavaName
        /// </summary>
        public TypeCollectionIndexer<string, SomeType> JavaName
        {
            get;
            private set;
        }

        /// <summary>
        /// An indexer that indexes this type collection by CrossPlatformName
        /// </summary>
        public TypeCollectionIndexer<string, SomeType> CrossPlatformName
        {
            get;
            private set;
        }

        /// <summary>
        /// An indexer that indexes this gtype collection by CSharpName
        /// </summary>
        public TypeCollectionIndexer<string, SomeType> CSharpName
        {
            get;
            private set;
        }

        /// <summary>
        /// An indexer that indexes this type collection by Simpl Name
        /// </summary>
        public TypeCollectionIndexer<string, SomeType> SimpleName
        {
            get;
            private set;
        }

        /// <summary>
        /// An indexer that indexes this type collection by the ObjectiveC Name
        /// </summary>
        public TypeCollectionIndexer<string, SomeType> ObjectiveCName
        {
            get;
            private set;
        }

        /// <summary>
        /// An inexer that indexes this type collection by the DB name
        /// </summary>
        public TypeCollectionIndexer<string, SomeType> DBName
        {
            get;
            private set;
        }

        /// <summary>
        /// An indexer that indexes this type collection by the CSharp Type
        /// </summary>
        public TypeCollectionIndexer<Type, SomeType> CSharpType
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Attempts to add a given object of SomeType to the TypeCollection
        /// </summary>
        /// <param name="theType">The instance of a given type to add to the system</param>
        /// <returns>True if added to any of the indexers</returns>
        public bool TryAdd(object theType)
        {
            bool result = false;

            if(theType is SomeType)
            {
                foreach (var indexer in nameindexers)
                {
                    result |= indexer.TryAdd(theType as SomeType);
                }

                this.CSharpType.TryAdd(theType as SomeType);
            }
             
            return result;
        }

        /// <summary>
        /// Determines if the type collection contains theName in any of its type indexers
        /// </summary>
        /// <param name="theName">The name, of any type of name</param>
        /// <returns>True if it's in there</returns>
        public bool Contains(string theName)
        {
            foreach (var indexer in this.nameindexers)
            {
                if(indexer.Contains(theName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the type collection contains theType in its type indexer
        /// </summary>
        /// <param name="theType">The type</param>
        /// <returns>True if it's in there</returns>
        public bool Contains(Type theType)
        {
            return this.CSharpType.Contains(theType);
        }
    }

    /// <summary>
    /// Indexes a type collection cleanly
    /// </summary>
    public class TypeCollectionIndexer<Key,SomeType>
    {
        private IDictionary<Key, SomeType> thisDictionary;
        private Func<SomeType, bool> addPredicate;
        private Func<SomeType, Key> keyPred;

        /// <summary>
        /// Curries in the core requirements to a predicate; core requires are that it is not null and that the dictionary does not contain the key
        /// </summary>
        /// <param name="predicate">The predicate to analyze if a type should be added to this indexer</param>
        /// <returns>A function with the core requirements curried in</returns>
        private Func<SomeType, bool> CurryCoreRequirements(Func<SomeType, bool> predicate)
        {
            return (t) => t != null && predicate(t) && !this.thisDictionary.ContainsKey(keyPred(t));
        }

        /// <summary>
        /// Creates an instance of the TypeCollectionIndexer with a keyPredicate. If the keyPredicate is not null, a given type will be added to the indexer
        /// </summary>
        /// <param name="keyPredicate">Predicate to obtain the key for a given SomeType</param>
        public TypeCollectionIndexer(Func<SomeType, Key> keyPredicate)
        {
            this.thisDictionary = new Dictionary<Key, SomeType>();
            this.keyPred = keyPredicate;
            this.addPredicate = CurryCoreRequirements((t) => this.keyPred(t) != null);
        }

        /// <summary>
        /// Creates an instance of the TypeCollectionIndexer with a given key and insert pedicate. 
        /// If the insert predicate returns true, the sometype will be added with the key obtained by the key predicate
        /// </summary>
        /// <param name="insertPredicate">Returns true if the SomeType should be added</param>
        /// <param name="keyPredicate">Returns the key to use for the type indexer</param>
        public TypeCollectionIndexer(Func<SomeType, bool> insertPredicate, Func<SomeType, Key> keyPredicate)
        {
            this.thisDictionary = new Dictionary<Key, SomeType>();
            this.addPredicate = CurryCoreRequirements(insertPredicate);
            this.keyPred = keyPredicate;
        }

        /// <summary>
        /// Attempts to add someType to the type indexer
        /// </summary>
        /// <param name="someType">SomeType to index</param>
        /// <returns>True if added to the index</returns>
        public bool TryAdd(SomeType someType)
        {
            if (addPredicate(someType))
            {
                this.thisDictionary.Add(keyPred(someType), someType);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Indexes through the collection by the Key name
        /// </summary>
        /// <param name="Name">Key of some sort to index</param>
        /// <returns>The SomeType in the dictionary. </returns>
        public SomeType this[Key Name]
        {
            get
            {
                return thisDictionary[Name];
            }
        }

        /// <summary>
        /// Determines if the keyValue is contained in the indexer
        /// </summary>
        /// <param name="keyValue">The keyValue to search for</param>
        /// <returns>True if the keyValue is contained</returns>
        public bool Contains(Key keyValue)
        {
            return this.thisDictionary.ContainsKey(keyValue);
        }

        /// <summary>
        /// Obtains the number of keys indexed by this indexer
        /// </summary>
        public int Count
        {
            get
            {
                return this.thisDictionary.Keys.Count();
            }
        }
    }
}
