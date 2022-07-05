using System;
using System.Collections;
using System.Collections.Generic;

namespace FundamentalStructures;

public class StaticHashTable<TKey, TValue> : IHashTable<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey> where TValue : IComparable<TValue>
{
    //HASH_TABLE_ENUMERATOR
    private struct HashTableEnumerator: IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private int _index = 0;
        private readonly StaticHashTable<TKey, TValue> _hashTable;

        public HashTableEnumerator(StaticHashTable<TKey, TValue> hashTable) => _hashTable = hashTable;

        public bool MoveNext()
        {
            _index++;
            while (_index < _hashTable._statusesTable.Length && _hashTable._statusesTable[_index] != STATUS_PLACED)
                _index++;

            if (_index == _hashTable._statusesTable.Length)
                return false;
                
            return true;
        }

        public void Dispose() { }
        public void Reset() => _index = 0;
        public KeyValuePair<TKey, TValue> Current => _hashTable._valuesTable[_index];

        object IEnumerator.Current => Current;
    }
    
    private const int STATUS_EMPTY = 0;
    private const int STATUS_PLACED = 1;
    private const int STATUS_REMOVED = 2;
    
    private readonly int _capacity;
    
    private KeyValuePair<TKey, TValue>[] _valuesTable;
    private byte[] _statusesTable;
    
    private readonly HashEnumerator _hashEnumerator;

    #region DEFAULT_HASH_FUNCTIONS
    
    private int FirstHashFunction(int key)
    {
        key = Math.Abs(key);
        
        int mod = 10;
        while (mod*10 < _capacity)
        {
            mod *= 10;
        }

        int temp = 0;
        while (key != 0)
        {
            temp += key % mod;
            key /= mod;
        }

        return temp % _capacity;
    }
        
    private int SecondHashFunction(int key)
    {
        key = Math.Abs(key);
        return 13 - key % 13;
    }
    
    #endregion

    public StaticHashTable(int maxCapacity)
    {
        _capacity = maxCapacity;
        Count = 0;

        _valuesTable = new KeyValuePair<TKey, TValue>[_capacity];
        _statusesTable = new byte[_capacity];

        FirstHashFunc = FirstHashFunction;
        SecondHashFunc = SecondHashFunction;
        
        _hashEnumerator = new HashEnumerator();
        _hashTableEnumerator = new HashTableEnumerator(this);
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            throw new Exception($"The value by key {key} is already exist!");
        }
        
        bool isAdded = false;
        int hashCode = key.GetHashCode();
            
        _hashEnumerator.SetForNewHash(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
        foreach (int i in _hashEnumerator)
        {
            if (_statusesTable[i] != STATUS_PLACED)
            {
                _valuesTable[i] = new KeyValuePair<TKey, TValue>(key, value);
                _statusesTable[i] = STATUS_PLACED;
                
                isAdded = true;
                break;
            }
        }

        if (!isAdded)
        {
            throw new Exception("No place in the table!");
        }

        Count += 1;
    }

    public void Remove(TKey key, TValue value)
    {
        int hashCode = key.GetHashCode();
            
        _hashEnumerator.SetForNewHash(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
        foreach (int i in _hashEnumerator)
        {
            if (_statusesTable[i] == STATUS_EMPTY) return; 
            
            if (_statusesTable[i] == STATUS_PLACED && _valuesTable[i].Key.CompareTo(key) == 0 && _valuesTable[i].Value.CompareTo(value) == 0)
            {
                _statusesTable[i] = STATUS_REMOVED;
                Count -= 1;
                break;
            }
        }
    }

    public void Clear()
    {
        _valuesTable = new KeyValuePair<TKey, TValue>[_capacity];
        _statusesTable = new byte[_capacity];
        Count = 0;
    }

    public bool Contains(TKey key, TValue value)
    {
        int hashCode = key.GetHashCode();
            
        _hashEnumerator.SetForNewHash(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
        foreach (int i in _hashEnumerator)
        {
            if (_statusesTable[i] == STATUS_EMPTY) break; 
            
            if (_statusesTable[i] == STATUS_PLACED &&_valuesTable[i].Key.CompareTo(key) == 0 && _valuesTable[i].Value.CompareTo(value) == 0)
            {
                return true;
            }
        }
        
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        int hashCode = key.GetHashCode();
            
        _hashEnumerator.SetForNewHash(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
        foreach (int i in _hashEnumerator)
        {
            if (_statusesTable[i] == STATUS_EMPTY) break; 
            
            if (_statusesTable[i] == STATUS_PLACED && _valuesTable[i].Key.CompareTo(key) == 0)
            {
                return true;
            }
        }
        
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int hashCode = key.GetHashCode();
            
        _hashEnumerator.SetForNewHash(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
        foreach (int i in _hashEnumerator)
        {
            if (_statusesTable[i] == STATUS_EMPTY) break; 
            
            if (_statusesTable[i] == STATUS_PLACED && _valuesTable[i].Key.CompareTo(key) == 0)
            {
                value = _valuesTable[i].Value;
                return true;
            }
        }
        
        value = default;
        return false;
    }

    public override string ToString()
    {
        string output = "";
        int index = 1;
        
        foreach (var pair in this)
        {
            output += $"{index}] Key:{pair.Key}; Value: {pair.Value}\n";
            index += 1;
        }
        
        return output;
    }

    public string ToStringWithStatuses()
    {
        string output = "";

        for (int i = 0; i < _capacity; i++)
        {
            if (_statusesTable[i] != 0) //Модифицировал, чтобы не выводило миллион записей
                output += $"{i+1}) Key: {_valuesTable[i].Key}; Value: {_valuesTable[i].Value}; Status: {_statusesTable[i]}; FirstHF: {FirstHashFunc(_valuesTable[i].Key.GetHashCode())}; SecondHF: {SecondHashFunc(_valuesTable[i].Key.GetHashCode())}\n";
        }

        return output;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (key is null)
                throw new Exception("Try to get value by null key!");
            
            if (!TryGetValue(key, out var value))
                throw new Exception($"The element by key {key} doesn't exist!");

            return value;
        }
        set
        {
            if (key is null)
                throw new Exception("Try to set value by null key!");
            
            int hashCode = key.GetHashCode();
            _hashEnumerator.SetForNewHash(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));

            if (ContainsKey(key))
            {
                foreach (int i in _hashEnumerator)
                {
                    if (_statusesTable[i] == STATUS_PLACED && _valuesTable[i].Key.CompareTo(key) == 0)
                    {
                        _valuesTable[i] = new KeyValuePair<TKey, TValue>(key, value);
                        break;
                    }
                }
            }
            else
            {
                foreach (int i in _hashEnumerator)
                {
                    if (_statusesTable[i] != STATUS_PLACED)
                    {
                        //Значения нет в таблице, добавляем новое
                        Add(key, value);
                        break;
                    }
                }
            }
        }
    }

    private readonly HashTableEnumerator _hashTableEnumerator;
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return _hashTableEnumerator; }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    public int Count { get; private set; }

    private Func<int, int> _firstHashFunc;
    public Func<int, int> FirstHashFunc
    {
        get => _firstHashFunc;
        set => _firstHashFunc = value ?? throw new Exception("Unable to set first hash function to null!");
    }

    private Func<int, int> _secondHashFunc;
    public Func<int, int> SecondHashFunc
    {
        get => _secondHashFunc;
        set => _secondHashFunc = value ?? throw new Exception("Unable to set second hash function to null!");
    }
}