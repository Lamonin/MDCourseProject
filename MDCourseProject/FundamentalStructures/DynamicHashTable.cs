using System;
using System.Collections;
using System.Collections.Generic;

namespace FundamentalStructures;

public class DynamicHashTable<TKey, TValue> : IHashTable<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey> where TValue : IComparable<TValue>
{
    //HASH_TABLE_ENUMERATOR
    private struct HashTableEnumerator: IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private int _index = 0;
        private readonly DynamicHashTable<TKey, TValue> _hashTable;

        public HashTableEnumerator(DynamicHashTable<TKey, TValue> hashTable) => _hashTable = hashTable;

        public bool MoveNext()
        {
            do
            {
                _index++;    
            } while (_index < _hashTable._statusesTable.Length && _hashTable._statusesTable[_index] != STATUS_PLACED);

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
    
    private const int INITIAL_CAPACITY = 16;
    
    private int _capacity;
    private int _minCapacity;
    private int _maxCapacity;
    
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

    private void ResizeToBigger()
    {
        _capacity *= 2;
        RehashTable();
    }

    private void ResizeToSmaller()
    {
        if (_capacity <= INITIAL_CAPACITY) return;
        _capacity /= 2;
        RehashTable();
    }
    
    private void RehashTable()
    {
        Count = 0;
        _maxCapacity = _capacity * 75 / 100;
        _minCapacity = _capacity * 25 / 100;
        
        var tempValuesTable = _valuesTable;
        var tempStatusesTable = _statusesTable;
        
        _valuesTable = new KeyValuePair<TKey, TValue>[_capacity];
        _statusesTable = new byte[_capacity];

        for (int i = 0; i < tempStatusesTable.Length; i++)
        {
            //Вставляем значение по новому хэшу
            if (tempStatusesTable[i] == STATUS_PLACED)
                Add(tempValuesTable[i].Key, tempValuesTable[i].Value);
        }
    }
    
    public DynamicHashTable()
    {
        _capacity = INITIAL_CAPACITY;
        _maxCapacity = _capacity * 75 / 100;
        _minCapacity = _capacity * 25 / 100;
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
        
        int hashCode = key.GetHashCode();
            
        _hashEnumerator.SetForNewHash(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
        foreach (int i in _hashEnumerator)
        {
            if (_statusesTable[i] != STATUS_PLACED)
            {
                _valuesTable[i] = new KeyValuePair<TKey, TValue>(key, value);
                _statusesTable[i] = STATUS_PLACED;
                break;
            }
        }

        Count += 1;
        if (Count > _maxCapacity) ResizeToBigger();
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
        
        if (Count < _minCapacity) ResizeToSmaller();
    }

    public void Clear()
    {
        _capacity = INITIAL_CAPACITY;
        _maxCapacity = _capacity * 75 / 100;
        _minCapacity = _capacity * 25 / 100;
        
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
            
            if (_statusesTable[i] == STATUS_PLACED && _valuesTable[i].Key.CompareTo(key) == 0 && _valuesTable[i].Value.CompareTo(value) == 0)
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
            if (_statusesTable[i] == 0)
                output += $"{i+1}] EMPTY\n";
            else
                output += $"{i+1}] Key:{_valuesTable[i].Key}; Value: {_valuesTable[i].Value}; Status: {_statusesTable[i]}\n";
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
                        Add(key, value); //Значения нет в таблице, добавляем новое
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