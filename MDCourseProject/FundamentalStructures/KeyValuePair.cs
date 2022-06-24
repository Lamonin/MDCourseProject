using System;

namespace FundamentalStructures
{
    public readonly struct KeyValuePair<TKey, TValue>:IComparable where TKey:IComparable<TKey> where TValue:IComparable<TValue>
    {
        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        
        public int CompareTo(object obj)
        {
            if (obj is KeyValuePair<TKey, TValue> pair)
            {
                var res = Key.CompareTo(pair.Key);
                if (res == 0) res = Value.CompareTo(pair.Value);
                return res;
            }
            
            throw new Exception($"Try to compare KeyValuePair type with {obj.GetType()} type!");
        }

        public override string ToString()
        {
            return $"Key: {Key.ToString()}; Value: {Value.ToString()}";
        }

        public TKey Key { get; }
        public TValue Value { get; }
    }
}