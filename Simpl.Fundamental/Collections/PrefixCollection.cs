using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Net;

namespace Simpl.Fundamental.Collections
{
    ///<summary>
    /// An optimized data structure for managing a hierarchical collection of prefixes, automatically 
    /// merging and removing entries, and providing a fast matching function.
    /// 
    /// @author andruid
    ///</summary>
    public class PrefixCollection<O> : PrefixPhrase<O> 
    {
	    private readonly char separator;
	    
        ///<summary>
	    /// true means use file portion of the path when creating entries.
	    /// false means use host and directory portions of the path only. 
	    ///</summary>
	    private bool usePathFile;

        ///<summary>
        /// Construct a PrefixCollection in which each prefix can be parsed into PrefixPhrases,
        /// using the separator to split the phrases.
        ///</summary>
	    public PrefixCollection(char separator, bool usePathFile) : base(null, null)
	    {	    
		    this.separator				= separator;
		    this.usePathFile			= usePathFile;
	    }

	    public PrefixCollection(char separator) : this(separator, false)
	    {
	    }

	    ///<summary>
	    /// Construct a PrefixCollection in which each prefix can be parsed into PrefixPhrases,
	    /// using the separator to split the phrases.
	    ///</summary>
	    public PrefixCollection() : this(false) 
	    {
	    }

	    ///<summary>
	    /// Construct a PrefixCollection in which each prefix can be parsed into PrefixPhrases,
	    /// using '/' as the separator to split the phrases.
	    ///</summary> 
	    public PrefixCollection(bool usePathFile) : this('/', usePathFile)
	    {
	    }

        public char Separator
        {
            get { return separator; }
        }

        public PrefixPhrase<O> Add(ParsedUri purl)
	    {
		    String host	= purl.Host;//edit		
		    // domainPrefix is a child of this, the root (with no parent)
		    PrefixPhrase<O> hostPrefix	= GetPrefix(null, host);
		
		    // children of hostPrefix
		    String pathStringToParse = usePathFile ? purl.ToString() : purl.LocalPath;//.pathDirectoryString;
		    return (hostPrefix != null) ? hostPrefix.Add(pathStringToParse, separator) : LookupChild(host);
	    }
	
	    public PrefixPhrase<O> getMatchingPrefix(ParsedUri purl)
	    {
		    String host	= purl.Host;		
		    // domainPrefix is a child of this, the root (with no parent)
		    PrefixPhrase<O> hostPrefix	= LookupChild(host);
		
		    // children of hostPrefix
		    String path = purl.AbsolutePath;
		    return (hostPrefix == null) ? null : hostPrefix.GetMatchingPrefix(path, 1, separator);	// skip over starting '/'
	    }
	
	    public bool Match(ParsedUri purl)
	    {
		    String host	= purl.Host;		
		    // domainPrefix is a child of this, the root (with no parent)
		    PrefixPhrase<O> hostPrefix	= LookupChild(host);
		
		    // children of hostPrefix
		    return (hostPrefix == null) ? false : hostPrefix.Match(purl.LocalPath, separator);//edit
	    }
	
	    public List<String> Values()
	    {
		    return Values(separator);
	    }
    }
}
