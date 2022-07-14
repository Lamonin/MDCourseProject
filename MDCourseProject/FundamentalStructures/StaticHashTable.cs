using System;

namespace FundamentalStructures;

public class StaticHashTable<TKey, TValue> where TKey : IComparable<TKey> where TValue : IComparable<TValue>
{
    private const int STATUS_EMPTY = 0;
    private const int STATUS_PLACED = 1;
    private const int STATUS_REMOVED = 2;
    
    public int Capacity { get; }
    
    private KeyValuePair<TKey, TValue>[] valuesTable;
    private byte[] statusesTable;
    
    private readonly HashEnumerator hashEnumerator;

    #region DEFAULT_HASH_FUNCTIONS
    
    private uint FirstHashFunction(TKey key)
    {
        var numericKey = Math.Abs(key.GetHashCode());
        
        // Получает 10^(кол-во цифр в _capacity - 1)
        int mod = (int) Math.Pow(10, (int) Math.Log10(Capacity));
        
        int temp = 0;
        while (numericKey != 0)
        {
            temp += numericKey % mod;
            numericKey /= mod;
        }

        return (uint) (temp % Capacity);
    }
        
    private uint SecondHashFunction(TKey key)
    {
        var numericKey = Math.Abs(key.GetHashCode());
        return (uint) (numericKey % (Capacity - 1) + 1);
    }
    
    #endregion

    public StaticHashTable(uint capacity)
    {
        Capacity = (int) capacity;
        Count = 0;

        valuesTable = new KeyValuePair<TKey, TValue>[Capacity];
        statusesTable = new byte[Capacity];
        _secondHFValues = new string[Capacity];

        hashEnumerator = new HashEnumerator();
    }

    public void Add(TKey key, TValue value)
    {
        int possibleIndex = -1;
        hashEnumerator.SetForNewHash(Capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
        foreach (int i in hashEnumerator)
        {
            if (statusesTable[i] == STATUS_EMPTY)
            {
                if (possibleIndex == -1)
                {
                    possibleIndex = i;
                }
                break;
            }
            
            if (statusesTable[i] == STATUS_REMOVED && possibleIndex == -1)
            {
                possibleIndex = i;
            }

            if (statusesTable[i] == STATUS_PLACED 
                && valuesTable[i].Key.CompareTo(key) == 0 
                && valuesTable[i].Value.CompareTo(value) == 0
            )
            {
                throw new Exception($"The value by key {key} is already exist!");
            }
        }

        if (possibleIndex == -1)
        {
            throw new Exception("No place in the table!");
        }
        
        valuesTable[possibleIndex] = new KeyValuePair<TKey, TValue>(key, value);
        statusesTable[possibleIndex] = STATUS_PLACED;
        _secondHFValues[possibleIndex] = GetSecondHashValues(key);

        Count += 1;
    }

    public void Remove(TKey key, TValue value)
    {
        hashEnumerator.SetForNewHash(Capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
        foreach (int i in hashEnumerator)
        {
            if (statusesTable[i] == STATUS_EMPTY) return; 
            
            if (statusesTable[i] == STATUS_PLACED && valuesTable[i].Key.CompareTo(key) == 0 && valuesTable[i].Value.CompareTo(value) == 0)
            {
                statusesTable[i] = STATUS_REMOVED;
                Count -= 1;
                break;
            }
        }
    }

    public void Clear()
    {
        valuesTable = new KeyValuePair<TKey, TValue>[Capacity];
        statusesTable = new byte[Capacity];
        Count = 0;
    }

    public bool Contains(TKey key, TValue value)
    {
        hashEnumerator.SetForNewHash(Capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
        foreach (int i in hashEnumerator)
        {
            if (statusesTable[i] == STATUS_EMPTY) break; 
            
            if (statusesTable[i] == STATUS_PLACED &&valuesTable[i].Key.CompareTo(key) == 0 && valuesTable[i].Value.CompareTo(value) == 0)
            {
                return true;
            }
        }
        
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        hashEnumerator.SetForNewHash(Capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
        foreach (int i in hashEnumerator)
        {
            if (statusesTable[i] == STATUS_EMPTY) break; 
            
            if (statusesTable[i] == STATUS_PLACED && valuesTable[i].Key.CompareTo(key) == 0)
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
        
        hashEnumerator.SetForNewHash(Capacity, (int) FirstHashFunction(key), (int) SecondHashFunction(key));
        foreach (int i in hashEnumerator)
        {
            stepsToFind += 1;
            if (statusesTable[i] == STATUS_EMPTY) break;
            
            if (statusesTable[i] == STATUS_PLACED && valuesTable[i].Key.CompareTo(key) == 0)
            {
                value = valuesTable[i].Value;
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

        for (int i = 0; i < valuesTable.Length; i++)
        {
            if (statusesTable[i] == STATUS_PLACED)
            {
                output += $"{index}] Key:{valuesTable[i].Key}; Value: {valuesTable[i].Value}\n";
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
        
        hashEnumerator.SetForNewHash(Capacity, firstHFResult, (int) SecondHashFunction(key));
        foreach (var index in hashEnumerator)
        {
            if (statusesTable[index] == STATUS_EMPTY)
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

        for (int i = 0; i < Capacity; i++)
        {
            if (statusesTable[i] != 0) //Модифицировал, чтобы не выводило миллион записей
                output += $"\t{i}) Key: {valuesTable[i].Key};"
                          + $"  Value: {valuesTable[i].Value};"
                          + $"  HashCode: {valuesTable[i].Key.GetHashCode()};"
                          + $"  Status: {statusesTable[i]};"
                          + $"  FirstHF: {FirstHashFunction(valuesTable[i].Key)};"
                          + $"  PureSecondHF({SecondHashFunction(valuesTable[i].Key)});"
                          + $"  SecondHF: {_secondHFValues[i]}\n";
        }

        return output;
    }
    public int Count { get; private set; }
}