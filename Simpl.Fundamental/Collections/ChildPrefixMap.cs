using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Collections
{
    ///<summary>
    /// Contains Dictionary<String, PrefixPhrase>
    ///
    /// However, for put, checks to see if String key is WILDCARD. 
    /// If so, sets special wildcardMatch slot instead of adding to HashMap.
    /// <p/>
    /// Likewise, for get(), if a specific match does not work, chekcs to see if there is a wildcardMatch. 
    /// If there is one, it will be returned.
    /// 
    /// @author andruid
    ///</summary>
    public class ChildPrefixMap<O> : Dictionary<String, PrefixPhrase<O>>
    {
	    private PrefixPhrase<O> wildcardMatch;
	
	    public static readonly String WILDCARD = "*";

	    public ChildPrefixMap()
	    {   
	    }

	    public ChildPrefixMap(int initialCapacity) : base(initialCapacity)
	    {
	    }

	    public ChildPrefixMap(Dictionary< String, PrefixPhrase<O>> m) : base(m)
	    {
	    }

	    ///<summary>
        ///For put, checks to see if String key is WILDCARD. 
        ///If so, sets special wildcardMatch slot instead of adding to HashMap.
	    ///</summary>    
        public new PrefixPhrase<O> Add(String key, PrefixPhrase<O> value)
	    {
	        PrefixPhrase<O> result;

	        if (WILDCARD.Equals(key))
	        {
		        result	= wildcardMatch;
		        wildcardMatch = value;
		        return result;
	        }

	        TryGetValue(key, out result);
            if (result == null)
            {
                base.Add(key, value);
                result = value;

            }
	        else
            {
                base.Remove(key);
                base.Add(key, value);
            }

	        return result;
        }

        ///<summary>
	    /// For get(), if a specific match does not work, check to see if there is a wildcardMatch. 
	    ///    * If there is one, it will be returned.	 
	    ///</summary>
        public PrefixPhrase<O> Get(String key)
        {
            PrefixPhrase<O> result;
            
            TryGetValue(key, out result);
	        return result ?? wildcardMatch;
        }
	
        public PrefixPhrase<O> WildcardMatch
        {
	        get { return wildcardMatch; }
            set { wildcardMatch = value; }
        }

        public int Size()
        {
	        int result = base.Count;
	        if (wildcardMatch != null)
		        result++;
	        return result;
        }
    }
}
