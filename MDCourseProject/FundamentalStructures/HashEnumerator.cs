using System.Collections;
using System.Collections.Generic;

namespace FundamentalStructures;

/// <summary>
/// Осуществляет перебор индексов на основе значений первичной и вторичной хэш-функций некоторого ключа указанное число раз.
/// </summary>
public class HashEnumerator : IEnumerator<int>, IEnumerable<int>
{
    private int _index;

    private int _capacity;
    private int _firstHFResult;
    private int _secondHFResult;

    public HashEnumerator() { _capacity = 0; }
    
    public HashEnumerator(int maxAttempts, int firstHfResult, int secondHfResult)
    {
        SetForNewHash(maxAttempts, firstHfResult, secondHfResult);
    }

    public void SetForNewHash(int maxAttempts, int firstHfResult, int secondHfResult)
    {
        _index = 0;
        _capacity = maxAttempts;
        _firstHFResult = firstHfResult;
        _secondHFResult = secondHfResult;
    }
            
    public bool MoveNext()
    {
        if (_index < 2 || Current != _firstHFResult)
        {
            Current = (_firstHFResult + _index * _secondHFResult) % _capacity;
            _index++;
                    
            return true;
        }
            
        return false;
    }

    public void Reset() { _index = 0; }
    public void Dispose() { }
    public int Current { get; private set; }
    object IEnumerator.Current => Current;
    public IEnumerator<int> GetEnumerator() { return this; }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}