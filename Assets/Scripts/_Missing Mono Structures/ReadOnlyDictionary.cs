using System.Collections.Generic;

// Implimentation of a ReadOnlyDictionary, as one is not included in Unity's system.collections.generic

public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	private IDictionary<TKey, TValue> _dict;

	public ReadOnlyDictionary(IDictionary<TKey, TValue> dict)
	{
		if (dict == null)
            throw new System.ArgumentNullException("dict");
		_dict = dict;
	}

	public TValue this[TKey key]
	{
		get
		{
			return _dict[key];
		}
		set
		{
			throw new System.NotSupportedException();
		}
	}

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
        get
        {
            return true;
        }
    }

    public void Add(TKey key, TValue value)
    {
        throw new System.NotSupportedException();
    }
    public void Add(KeyValuePair<TKey,TValue> kvp)
    {
        throw new System.NotSupportedException();
    }
    public bool Remove(TKey key)
    {
        throw new System.NotSupportedException();
    }
    public bool Remove(System.Collections.Generic.KeyValuePair<TKey,TValue> kvp)
    {
        throw new System.NotSupportedException();
    }

    public bool ContainsKey(TKey key)
    {
        return _dict.ContainsKey(key);
    }
    public bool Contains(KeyValuePair <TKey, TValue> keyPair)
    {
        return _dict.Contains(keyPair);
    }

    public void Clear()
    {
        throw new System.NotSupportedException();
    }


/*
Assets/Scripts/ReadOnlyDictionary.cs(3,14): error CS0738: `ReadOnlyDictionary<TKey,TValue>' does not implement interface member `System.Collections.Generic.IDictionary<TKey,TValue>.TryGetValue(TKey, out TValue)' and the best implementing candidate `ReadOnlyDictionary<TKey,TValue>.Add(TKey, TValue)' return type `void' does not match interface member return type `bool'
*/

    public bool TryGetValue(TKey key, out TValue oVar)
    {
        return _dict.TryGetValue(key, out oVar);
    }

    
// Assets/Scripts/ReadOnlyDictionary.cs(3,14): error CS0738: `ReadOnlyDictionary<TKey,TValue>' does not implement interface member `System.Collections.Generic.IDictionary<TKey,TValue>.Keys.get' and the best implementing candidate `ReadOnlyDictionary<TKey,TValue>.Add(TKey, TValue)' return type `void' does not match interface member return type `System.Collections.Generic.ICollection<TKey>'
	public System.Collections.Generic.ICollection<TKey> Keys
    {
        get
        {
            return _dict.Keys;
        }
    }

    public System.Collections.Generic.ICollection<TValue> Values
    {
        get
        {
            return _dict.Values;
        }
    }

    public int Count
    {
        get
        {
            return _dict.Count;
        }
    }

    public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator ()
    {
        return _dict.GetEnumerator();
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public void CopyTo(System.Collections.Generic.KeyValuePair<TKey,TValue>[] target, int index)
    {
        _dict.CopyTo(target, index);
    }
}