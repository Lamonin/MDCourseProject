using System;

namespace FundamentalStructures
{
    public class KeyValuePair<TKey, TValue> where TKey:IComparable where TValue:IComparable
    {
        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        
        public int CompareTo(KeyValuePair<TKey, TValue> pair)
        {
            var res = Key.CompareTo(pair.Key);
            if (res == 0) res = Value.CompareTo(pair.Value);
            return res;
        }
        public TKey Key { get; }
        public TValue Value { get; }
    }
}