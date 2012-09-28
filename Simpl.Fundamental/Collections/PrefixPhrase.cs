using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Collections
{

    ///<summary>
    /// Recursive unit (bucket) for prefix pattern matching.
    ///
    /// @author andruid
    ///</summary>
    public class PrefixPhrase<O>
    {
	    private readonly String phrase;
	
	    private PrefixPhrase<O> parent;
	
	    ChildPrefixMap<O> childPhraseMap = new ChildPrefixMap<O>();
	
	    private	O mappedObject;

	    public O MappedObject
	    {
		    get { return mappedObject; }
            set { mappedObject = value; }
	    }
        
        public PrefixPhrase(PrefixPhrase<O> parent, String phrase)
	    {
		    this.parent	= parent;
		    this.phrase	= phrase;
	    }

	    public PrefixPhrase<O> Add(String str, char separator)
	    {
		    return Add(str, 0, separator);
	    }
	
	    protected PrefixPhrase<O> Add(String str, int start, char separator)
	    {
		    int end	= str.Length;
		    bool terminate = false;
		
		    if (start == end)
			    terminate = true;
		    else
		    {		
			    if (str[start] == separator)
				    start++;
			    if (start == end)
				    terminate = true;
		    }
		    if (terminate)
		    {
			    Clear();
			    return this;
		    }
		    int nextSeparator	= str.IndexOf(separator, start);
		    if (nextSeparator == -1)
			    nextSeparator	= end;
		
		    if (nextSeparator > -1)
		    {
			    String phraseString	= str.Substring(start, nextSeparator - start);
			    // extra round of messing with synch, because we need to know if we
			    // are creating a new Phrase
			    PrefixPhrase<O> nextPrefixPhrase = GetPrefix(this, phraseString);
			    if (nextPrefixPhrase != null)
			    {
				    return nextPrefixPhrase.Add(str, nextSeparator, separator);
			    }
			    else
			    {
				    // done!
				    PrefixPhrase<O> newTerminal = LookupChild(phraseString);
				    //newTerminal.clear();
				    return newTerminal;
    //				synchronized (this)
    //				{
    //					nextPrefixPhrase	= getPrefix(this, phraseString);
    //					if (nextPrefixPhrase == null)
    //					{
    //						nextPrefixPhrase	= childPhraseMap.getOrCreateAndPutIfNew(phraseString, this);
    //						result				= nextPrefixPhrase;
    //					}
    //				}
			    }
		    }
		    else
		    {
			    Debug.WriteLine("help! wrong block!!!");
			    // last segment
			    return null;
		    }
	    }
	
	    public bool Match(String str, char separator)
	    {
		    return Match(str, 0, separator);
	    }
	    protected bool Match(String str, int start, char separator)
	    {
		    if (IsTerminal())
			    return true;
		
		    int end				= str.Length;
		    bool terminate	= false;
		
		    if (start == end)
			    terminate		= true;
		    else
		    {		
			    if (str[start] == separator)
				    start++;
			    if (start == end)
				    terminate = true;
		    }
		    if (terminate)
		    {
			    return false;
		    }
		
		    int nextSeparator = str.IndexOf(separator, start);
		    if (nextSeparator == -1)
			    nextSeparator	= end;
		
    //		String phraseString	= string.substring(start, nextSeparator);
    //		PrefixPhrase nextPrefixPhrase	= lookupChild(phraseString);
		    PrefixPhrase<O> nextPrefixPhrase	= MatchChild(str, start, nextSeparator);
		    return (nextPrefixPhrase != null) && nextPrefixPhrase.Match(str, nextSeparator, separator);
	    }
	
	    /**
	     * Match child prefix by iterating, instead of using HashMap, to avoid allocating substring keys.
	     * 
	     * @param source	String to get key from.
	     * @param start		start of substring for key in string
	     * @param end		end of substring for key in string
	     * 
	     * @return			Matching PrefixPhrase for the substring key from source, or null if there is no match.
	     */
	    private PrefixPhrase<O> MatchChild(String source, int start, int end)
	    {
		    // FIXME concurrent modification exception :-(
		    PrefixPhrase<O> wildcardPhrase = childPhraseMap.WildcardMatch;
		    if (wildcardPhrase != null)
			    return wildcardPhrase;
		
		    foreach (String thatPhrase in childPhraseMap.Keys)
		    {
			    if (Match(thatPhrase, source, start, end))
				    return childPhraseMap.Get(thatPhrase);
		    }
		    return null;
	    }

        ///<summary>
	    /// @return	true if the substring of source running from start to end is the same String as target.
	    ///</summary>
	    private static bool Match(String target, String source, int start, int end)
	    {
		    int	targetLength	= target.Length;
		    int sourceLength	= end - start;
		    if (targetLength != sourceLength)
			    return false;
		    for (int i=0; i<sourceLength; i++)
		    {
			    if (source[start++] != target[i])
				    return false;
		    }
		    return true;
	    }
	    /**
	     * Seek the PrefixPhrase corresponding to the argument.
	     * If it does not exist, return it.
	     * <p/>
	     * If it does exist, does it have 0 children?
	     * 		If so, return null. No need to insert for the argument's phrase.
	     * 		If not, return it.
	     * 
	     * @param prefixPhrase
	     * @return
	     */
	    protected PrefixPhrase<O> GetPrefix(PrefixPhrase<O> parent, String prefixPhrase)
	    {
		    PrefixPhrase<O> preexistingPrefix = childPhraseMap.Get(prefixPhrase);	// will match wildcard specially, if this is called for
		    bool createNew = false;
		
		    if (preexistingPrefix == null)
		    {
				preexistingPrefix	= new PrefixPhrase<O>(parent, prefixPhrase);
				childPhraseMap.Add(prefixPhrase, preexistingPrefix);
				createNew = true;
		    }
		    if (!createNew && preexistingPrefix.IsTerminal())
			    return null;
		
		    return preexistingPrefix;
	    }

	    protected PrefixPhrase<O> LookupChild(String prefix)
	    {
		    return childPhraseMap.Get(prefix);
	    }
	
	    protected void Clear()
	    {
		    childPhraseMap.Clear();
	    }

	    public void ToStringBuilder(StringBuilder buffy, char separator)
	    {
		    if (parent != null)
		    {
			    parent.ToStringBuilder(buffy, separator);
			    buffy.Append(separator);
		    }
		    buffy.Append(phrase);
	    }
	
	    ///<summary>
	    /// From this root, find each the terminal children.
        ///</summary>
	    void FindTerminals(List<PrefixPhrase<O>> phraseSet)
	    {
		    if (phrase == null)
			    return;
		    if (IsTerminal())
		    {
			    phraseSet.Add(this);
		    }
		    else
		    {
			    foreach (PrefixPhrase<O> childPhrase in childPhraseMap.Values)
				    childPhrase.FindTerminals(phraseSet);
		    }
	    }
	
	    public int NumChildren()
	    {
		    return childPhraseMap.Size();
	    }
	
	    ///<summary>
	    /// Is the end of a prefix.
        ///</summary>
	    public bool IsTerminal()
	    {
		    return NumChildren() == 0;
	    }
	
	    public List<String> Values(char separator)
	    {
		    if (phrase == null)
			    return new List<String>(0);
		
		    List<PrefixPhrase<O>> terminalPrefixPhrases	= new List<PrefixPhrase<O>>();
		    FindTerminals(terminalPrefixPhrases);
		
		    List<String> result	= new List<String>(terminalPrefixPhrases.Count);
		    StringBuilder buffy	= new StringBuilder();
            //TODO StringBuilderUtils.acquire(buffy)
		
		    foreach (PrefixPhrase<O> thatPhrase in terminalPrefixPhrases)
		    {
			    buffy.Clear();
			    thatPhrase.ToStringBuilder(buffy, separator);
			    result.Add(buffy.ToString().Substring(0, buffy.Length));
		    }
	        
            //TODO StringBuilderUtils.release(buffy)
		    return result;
	    }
	    public String GetMatchingPhrase(String purl, char seperator)
	    {
		    StringBuilder buffy	= new StringBuilder();
            //TODO StringBuilderUtils.acquire(buffy)
		    
            getMatchingPhrase(buffy, purl, seperator);
		    String result = buffy.ToString();
		    buffy.Clear();
		    //TODO StringBuilderUtils.release(buffy)
		    return result;
	    }

	    ///<summary>
	    /// This function returns the whole matching path which you have
	    /// followed to reach the PrefixPhrase.
        ///</summary>
        public void getMatchingPhrase(StringBuilder buffy, String purl,char seperator)
	    {
		    String returnValue="";
		    int seperatorIndex	= purl.IndexOf(seperator);
		    if(seperatorIndex>0)
		    {
			    String key 					= purl.Substring(0, seperatorIndex);
                String phrase = purl.Substring(seperatorIndex + 1, purl.Length - (seperatorIndex + 1));
			    PrefixPhrase<O> childPrefixPhrase = childPhraseMap.Get(key);
			
                // now handled inside ChildPrefixMap
                //			if(childPrefixPhrase==null)
                //			{
                //				// try getting it using wildcard as key
                //				childPrefixPhrase = childPhraseMap.get("*");
                //				key="*";
                //			}

                if(childPrefixPhrase!=null)
			    {
				    buffy.Append(returnValue).Append(key).Append(seperator);
				    buffy.Append(childPrefixPhrase.GetMatchingPhrase(phrase, seperator));
			    }
		    }
	    }
	
	    public PrefixPhrase<O> GetMatchingPrefix(String input, int start, char seperator)
	    {
		    if (IsTerminal())
			    return this;
		    int seperatorIndex	= input.IndexOf(seperator, start);
		    if(seperatorIndex>0)
		    {
			    String key 				= input.Substring(start, seperatorIndex - start);
			    PrefixPhrase<O> childPrefixPhrase	= childPhraseMap.Get(key);
			    if (childPrefixPhrase!=null)
			    {
				    return (seperatorIndex < input.Length) ? childPrefixPhrase.GetMatchingPrefix(input, seperatorIndex+1, seperator) : this;
			    }
		    }
		    return null;
	    }
	
	    public void RemovePrefix(String prefix)
	    {
		    childPhraseMap.Remove(prefix);		
	    }
	
	    public override String ToString()
	    {
		    String result = this.phrase;
		    if (parent != null)
			    result			+= " < " + parent;
		    return result;
	    }
    }
}
