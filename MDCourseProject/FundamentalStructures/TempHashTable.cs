using System;
using System.Collections;
using System.Collections.Generic;

namespace FundamentalStructures
{
    /// <summary>
    /// Осуществляет перебор индексов на основе значений первичной и вторичной хэш-функций ключа для указанного размера таблицы.
    /// </summary>
    internal class HashEnumerator : IEnumerator<int>, IEnumerable<int>
    {
        private int _i;

        private int _capacity;
        private int _firstHFResult;
        private int _secondHFResult;

        public HashEnumerator(int capacity, int firstHfResult, int secondHfResult)
        {
            SetForNewKey(capacity, firstHfResult, secondHfResult);
        }

        public void SetForNewKey(int capacity, int firstHfResult, int secondHfResult)
        {
            _i = 0;
            _capacity = capacity;
            _firstHFResult = firstHfResult;
            _secondHFResult = secondHfResult;
        }
            
        public bool MoveNext()
        {
            if (_i < _capacity * 2)
            {
                Current = (_firstHFResult + _i * _secondHFResult) % _capacity;
                _i++;
                return true;
            }
            
            return false;
        }

        public void Reset() { _i = 0; }
        public void Dispose() { }

        public int Current { get; private set; }
        object IEnumerator.Current => Current;
        public IEnumerator<int> GetEnumerator() { return this; }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
    
    public class TempHashTable<TKey, TValue> : IHashTable<TKey, TValue> where TKey : IComparable where TValue : IComparable
    {
        private const int INITIAL_CAPACITY = 16;

        private KeyValuePair<TKey, TValue>[] _table;
        private byte[] _tableStatuses;
        
        private int _capacity;
        private int _maxCapacity;
        private int _minCapacity;
        
        private readonly HashEnumerator _hashEnumerator;

        private int FirstHashFunction(int key)
        {
            key = Math.Abs(key);
            int mod = 10;
            while (mod < _capacity) mod *= 10;

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

            var tempTable = _table;
            var tempStatuses = _tableStatuses;
            _table = new KeyValuePair<TKey, TValue>[_capacity];
            _tableStatuses = new byte[_capacity];

            for (int i = 0; i < tempStatuses.Length; i++)
            {
                //Если ячейка свободна, или её значение удалено, то игнорируем её
                if (tempStatuses[i] != 1) continue;  
                
                //Вставляем значение по новому хэшу
                Add(tempTable[i].Key, tempTable[i].Value);
            }
        }

        public TempHashTable()
        {
            Count = 0;
            FirstHashFunc = FirstHashFunction;
            SecondHashFunc = SecondHashFunction;

            _table = new KeyValuePair<TKey, TValue>[INITIAL_CAPACITY];
            _tableStatuses = new byte[INITIAL_CAPACITY];
            _capacity = INITIAL_CAPACITY;
            _maxCapacity = _capacity * 75 / 100;
            _minCapacity = _capacity * 25 / 100;

            _hashEnumerator = new HashEnumerator(_capacity, 0, 1);
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new Exception($"The value by key {key} is already exist!");
            
            int hashCode = key.GetHashCode();
            
            _hashEnumerator.SetForNewKey(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
            foreach (var index in _hashEnumerator)
            {
                if (_tableStatuses[index] != 1)
                {
                    _table[index] = new KeyValuePair<TKey, TValue>(key, value);
                    _tableStatuses[index] = 1;
                    break;
                }
            }

            Count++;

            if (Count > _maxCapacity) ResizeToBigger();
        }

        public void Remove(TKey key, TValue value)
        {
            int hashCode = key.GetHashCode();
            
            _hashEnumerator.SetForNewKey(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
            foreach (var index in _hashEnumerator)
            {
                if (_tableStatuses[index] == 0) return; //Такого элемента в таблице не оказалось

                //Если есть ключ с таким значением в таблице, то помечаем его удаленным
                if (_tableStatuses[index] == 1 && _table[index].Key.CompareTo(key) == 0 && _table[index].Value.CompareTo(value) == 0)
                {
                    _tableStatuses[index] = 2;
                    break;
                }
            }

            Count--;
            
            if (Count < _minCapacity) ResizeToSmaller();
        }

        public void Clear()
        {
            Count = 0;
            
            _table = new KeyValuePair<TKey, TValue>[INITIAL_CAPACITY];
            _tableStatuses = new byte[INITIAL_CAPACITY];
            _capacity = INITIAL_CAPACITY;
            _maxCapacity = _capacity * 75 / 100;
            _minCapacity = _capacity * 25 / 100;
        }

        public bool Contains(TKey key, TValue value)
        {
            int hashCode = key.GetHashCode();
            
            _hashEnumerator.SetForNewKey(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
            foreach (var index in _hashEnumerator)
            {
                if (_tableStatuses[index] == 0) break;
                
                if (_tableStatuses[index] == 1 && _table[index].Key.CompareTo(key) == 0 && _table[index].Value.CompareTo(value) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            int hashCode = key.GetHashCode();
            
            _hashEnumerator.SetForNewKey(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
            foreach (var index in _hashEnumerator)
            {
                if (_tableStatuses[index] == 0) break;
                
                if (_tableStatuses[index] == 1 && _table[index].Key.CompareTo(key) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            int hashCode = key.GetHashCode();
            
            _hashEnumerator.SetForNewKey(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
            foreach (var index in _hashEnumerator)
            {
                if (_tableStatuses[index] == 0) break;
                
                if (_tableStatuses[index] == 1 && _table[index].Key.CompareTo(key) == 0)
                {
                    value = _table[index].Value;
                    return true;
                }
            }

            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key is null)
                    throw new Exception("Try to get value by null key!");
                
                int hashCode = key.GetHashCode();
                
                _hashEnumerator.SetForNewKey(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
                foreach (var index in _hashEnumerator)
                {
                    if (_tableStatuses[index] == 0) break;
                
                    if (_tableStatuses[index] == 1 && _table[index].Key.CompareTo(key) == 0)
                    {
                        return _table[index].Value;
                    }
                }

                throw new Exception($"The element by key {key} doesn't exist!");
            }
            
            set
            {
                if (key is null)
                    throw new Exception("Try to set value by null key!");

                int hashCode = key.GetHashCode();
                
                _hashEnumerator.SetForNewKey(_capacity, FirstHashFunc(hashCode), SecondHashFunc(hashCode));
                foreach (var index in _hashEnumerator)
                {
                    if (_tableStatuses[index] == 0)
                    {
                        Add(key, value);
                        break;
                    }
                    
                    if (_tableStatuses[index] == 1 && _table[index].Key.CompareTo(key) == 0)
                    {
                        //Заменяем значение по ключу key
                        _table[index] = new KeyValuePair<TKey, TValue>(key, value);
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            string output = "";

            foreach (var pair in _table)
                output += pair + "\n";

            return output;
        }
        
        public string ToStringWithStatus()
        {
            string output = "";

            for (var i = 0; i < _table.Length; i++)
            {
                if (_tableStatuses[i] == 0)
                    output += $"{i}] Status: 0\n";
                else
                    output += $"{i}] {_table[i]}; Status: {i}\n";
            }

            return output;
        }

        public int Count { get; private set; }

        private Func<int, int> _firstHashFunc;
        public Func<int, int> FirstHashFunc
        {
            get => _firstHashFunc;
            set => _firstHashFunc = value ?? throw new Exception("Сan't set first hash function to null!");
        }

        private Func<int, int> _secondHashFunc;
        public Func<int, int> SecondHashFunc
        {
            get => _secondHashFunc;
            set => _secondHashFunc = value ?? throw new Exception("Сan't set second hash function to null!");
        }
    }
}