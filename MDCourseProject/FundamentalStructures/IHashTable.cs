using System;

namespace FundamentalStructures
{
    public interface IHashTable<TKey, TValue> where TKey:IComparable<TKey> where TValue:IComparable<TValue>
    { 
        /// <summary> Добавляет значение value по ключу key в таблицу </summary>
        void Add(TKey key, TValue value);

        /// <summary> Удаляет значение value по ключу key в таблице </summary>
        void Remove(TKey key, TValue value);

        /// <summary> Очищает таблицу </summary>
        void Clear();

        /// <summary> Содержит ли таблица значение value по ключу key </summary>
        bool Contains(TKey key, TValue value);
        
        /// <summary> Содержит ли таблица ключ key </summary>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Возвращает true, если таблица содержит ключ key и переменной value присваивается значение, соответствующее ключу
        /// </summary>
        bool TryGetValue(TKey key, out TValue value);

        TValue this[TKey key] { get; set; }

        /// <summary> Количество элементов в таблице </summary>
        int Count { get; }
        
        /// <summary> Первичная хэш-функция </summary>
        Func<TKey, uint> FirstHashFunc { get; set; }
        
        /// <summary> Вторичная хэш-функция </summary>
        Func<TKey, uint> SecondHashFunc { get; set; }
    }
}