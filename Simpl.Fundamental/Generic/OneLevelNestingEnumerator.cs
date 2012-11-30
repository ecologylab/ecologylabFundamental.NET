using System.Collections.Generic;
using System.Collections;

namespace Simpl.Fundamental.Generic
{
    ///<summary>
    /// An enumerator that iterates through nested elements and collections that are children of the object
    ///</summary>
    ///<typeparam name="I"></typeparam>
    ///<typeparam name="O"></typeparam>
    public class OneLevelNestingEnumerator<I, O> : IEnumerator<I> where O : IEnumerable<I>
    {
	    protected IEnumerator<I>    _firstIterator;
	
	    protected IEnumerator<O>    _collection;
	
	    protected O			        _currentObject;
	
	    protected IEnumerator<I>	_currentIterator;

        private O _firstObject;

        private I _current;
	
	    ///<summary>
	    ///</summary>
	    ///<param name="firstObject"></param>
	    ///<param name="iterableCollection"></param>
	    public OneLevelNestingEnumerator(O firstObject, IEnumerator<O> iterableCollection)
	    {
            this._firstIterator = firstObject.GetEnumerator();
	        this._firstObject   = firstObject;
            this._currentObject = firstObject;
            this._collection    = iterableCollection;
	    }
	
	    ///<summary>
	    ///</summary>
	    ///<param name="firstObject"></param>
	    ///<param name="iterableCollection"></param>
	    public OneLevelNestingEnumerator(O firstObject, IEnumerable<O> iterableCollection)
	    {
            this._firstIterator = firstObject.GetEnumerator();
	        this._firstObject   = firstObject;
            this._currentObject = firstObject;
            this._collection    = (iterableCollection != null) ? iterableCollection.GetEnumerator() : null;
	    }

        private bool CollectionHasNext()
        {
            return _collection != null && _collection.MoveNext();
        }

        private bool CurrentHasNext()
        {
            return _currentIterator != null && _currentIterator.MoveNext();
        }

        public virtual bool MoveNext() 
	    {
		    if (_firstIterator.MoveNext())
            {
                I firstNext = _firstIterator.Current;
			    // avoid returning the collection, itself, when it is a field in the firstIterator
			    if (!firstNext.Equals(_collection))
                {
                    _current = firstNext;
                    return true;
                }
                else
                    MoveNext();
		    }
		    // else
            if (CurrentHasNext())
            {
                _current = _currentIterator.Current;
                return true;
            }
		    // else
            if (CollectionHasNext())
		    {
			    _currentObject		= _collection.Current;
			    _currentIterator    = _currentObject.GetEnumerator();
			    _currentIterator.MoveNext();
                _current = _currentIterator.Current;
                return true;
		    }

		    return false;
	    }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public I Current
        {
            get { return _current; }
            protected set { _current = value; }
        }

        public void Reset()
        {
            _firstIterator.Reset();
            _currentObject = _firstObject;
            if (_collection != null)
                _collection.Reset();
        }

        public void Dispose()
        {
            _firstIterator.Dispose();
            _currentIterator.Dispose();
            _collection.Dispose();
        }

        public O CurrentObject()
        {
            return _currentObject;
        }
    }
}
