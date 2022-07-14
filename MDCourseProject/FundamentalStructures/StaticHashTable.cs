using System;

namespace FundamentalStructures;

public class StaticHashTable<TKey, TValue> where TKey : IComparable<TKey> where TValue : IComparable<TValue>
{
    private const int STATUS_EMPTY = 0;
    private const int STATUS_PLACED = 1;
    private const int STATUS_REMOVED = 2;
    
    private readonly int _capacity;
    
    private KeyValuePair<TKey, TValue>[] _valuesTable;
    private byte[] _statusesTable;
    
    private readonly HashEnumerator _hashEnumerator;

    #region DEFAULT_HASH_FUNCTIONS
    
    private uint FirstHashFunction(TKey key)
    {
        var numericKey = Math.Abs(key.GetHashCode());
        
        // Получает 10^(кол-во цифр в _capacity - 1)
        int mod = (int) Math.Pow(10, (int) Math.Log10(_capacity));
        
        int temp = 0;
        while (numericKey != 0)
        {
            temp += numericKey % mod;
            numericKey /= mod;
        }

        return (uint) (temp % _capacity);
    }
        
    private uint SecondHashFunction(TKey key)
    {
        var numericKey = Math.Abs(key.GetHashCode());
        return (uint) (numericKey % (_capacity - 1) + 1);
    }
    
    #endregion

    public StaticHashTable(uint capacity)
    {
        _capacity = (int) capacity;
        Count = 0;

        _valuesTable = new KeyValuePair<TKey, TValue>[_capacity];
        _statusesTable = new byte[_capacity];
        _secondHFValues = new string[_capacity];

        _hashEnumerator = new HashEnumerator();
    }

    public void Add(TKey key, TValue value)
    {
        int possibleIndex = -1;
        _hashEnumerator.SetForNewHash(_capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
        foreach (int i in _hashEnumerator)
        {
            if (_statusesTable[i] == STATUS_EMPTY)
            {
                if (possibleIndex == -1)
                {
                    possibleIndex = i;
                }
                break;
            }
            
            if (_statusesTable[i] == STATUS_REMOVED && possibleIndex == -1)
            {
                possibleIndex = i;
            }

            if (_statusesTable[i] == STATUS_PLACED 
                && _valuesTable[i].Key.CompareTo(key) == 0 
                && _valuesTable[i].Value.CompareTo(value) == 0
            )
            {
                throw new Exception($"The value by key {key} is already exist!");
            }
        }

        if (possibleIndex == -1)
        {
            throw new Exception("No place in the table!");
        }
        
        _valuesTable[possibleIndex] = new KeyValuePair<TKey, TValue>(key, value);
        _statusesTable[possibleIndex] = STATUS_PLACED;
        _secondHFValues[possibleIndex] = GetSecondHashValues(key);

        Count += 1;
    }

    public void Remove(TKey key, TValue value)
    {
        _hashEnumerator.SetForNewHash(_capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
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
        _hashEnumerator.SetForNewHash(_capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
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
        _hashEnumerator.SetForNewHash(_capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
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
        return TryGetValue(key, out value, out _);
    }
    
    public bool TryGetValue(TKey key, out TValue value, out int stepsToFind)
    {
        stepsToFind = 0;
        
        _hashEnumerator.SetForNewHash(_capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
        foreach (int i in _hashEnumerator)
        {
            stepsToFind += 1;
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

        for (int i = 0; i < _valuesTable.Length; i++)
        {
            if (_statusesTable[i] == STATUS_PLACED)
            {
                output += $"{index}] Key:{_valuesTable[i].Key}; Value: {_valuesTable[i].Value}\n";
                index += 1;
            }
        }

        return output;
    }

    private string[] _secondHFValues;
    private string GetSecondHashValues(TKey key)
    {
        var output = string.Empty;

        var firstHFResult = (int) FirstHashFunction(key);
        
        _hashEnumerator.SetForNewHash(_capacity, firstHFResult, (int) SecondHashFunction(key));
        foreach (var index in _hashEnumerator)
        {
            if (_statusesTable[index] == STATUS_EMPTY)
            {
                if (index != firstHFResult)
                {
                    output += index.ToString();
                }
                break;
            }
            output += index + ", ";
        }
        
        if (string.IsNullOrWhiteSpace(output))
        {
            output = "–";
        }
        return output;
    }

    public string ToStringWithStatuses()
    {
        string output = "";

        for (int i = 0; i < _capacity; i++)
        {
            if (_statusesTable[i] != 0) //Модифицировал, чтобы не выводило миллион записей
                output += $"\t{i}) Key: {_valuesTable[i].Key};"
                          + $"  Value: {_valuesTable[i].Value};"
                          + $"  HashCode: {_valuesTable[i].Key.GetHashCode()};"
                          + $"  Status: {_statusesTable[i]};"
                          + $"  FirstHF: {FirstHashFunction(_valuesTable[i].Key)};"
                          + $"  PureSecondHF({SecondHashFunction(_valuesTable[i].Key)});"
                          + $"  SecondHF: {_secondHFValues[i]}\n";
        }

        return output;
    }
    public int Count { get; private set; }
}