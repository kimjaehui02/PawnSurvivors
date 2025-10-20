using System;
using System.Collections.Generic;

public class EnumValueMap<TEnum, TValue> where TEnum : Enum
{
    private readonly Dictionary<TEnum, TValue> _map = new();

    // 값 추가 또는 갱신
    public void Set(TEnum key, TValue value)
    {
        _map[key] = value;
    }

    // 값 조회
    public bool TryGet(TEnum key, out TValue value)
    {
        return _map.TryGetValue(key, out value);
    }

    // 값 삭제
    public bool Remove(TEnum key)
    {
        return _map.Remove(key);
    }

    // 모든 키-값 반환
    public Dictionary<TEnum, TValue> GetAll()
    {
        return new Dictionary<TEnum, TValue>(_map);
    }

    // 존재 여부
    public bool ContainsKey(TEnum key)
    {
        return _map.ContainsKey(key);
    }

    // 초기화
    public void Clear()
    {
        _map.Clear();
    }
}
