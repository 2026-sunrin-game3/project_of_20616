using UnityEngine;
using System.Collections.Generic;
public enum MathType
{
    Increase,
    Decrease,
    Add,
    Remove
}
public class EntityStat : MonoBehaviour
{
    Dictionary<string, float> baseValue = new();
    Dictionary<string, float> resultValue = new();
    public List<Buf> bufs = new();
    public struct Buf
    {
        public string Key;
        public MathType mathType;
        public float Value;
    }
    struct StatValue
    {
        public string Key;
        public float Value;
    }
[SerializeField]
List<StatValue> defaultStat = new()
    {
        new StatValue{Key = "attackDAmage", Value = 3},
        new StatValue{Key = "defense", Value = 0},
        new StatValue{Key = "increaseDamage",Value=0 },
        new StatValue{Key = "critPer",Value = 0},
        new StatValue{Key = "criltMul",Value = 0},
        new StatValue{Key = "hurtDamage",Value = 0},
        new StatValue{Key = "atkSpeed",Value=0},
        new StatValue{Key="moveSpeed",Value=0},
    };
    public float attackDamage, defense, increaseDamage;
    // 공격력 방어력 가하는 피해 증가 치명타 확률 / 피해, 받는 피해 증가,공격 속도, 이동 속도
    public float critPer, critMul, hurtDamage, atkSpeed, moveSpeed;
    void Start()
    {
        foreach(StatValue val in defaultStat)
        {
            baseValue[val.Key] = val.Value;
            Calc(val.Key);
        }
    }
    public float GetResultValue(string Key)
    {
        return resultValue[Key];
    }
    public float Calc(string key)
    {
        float value = baseValue[key];
        float increase = 100;
        foreach (Buf buf in bufs)
        {
            switch (buf.mathType)
            {
                case MathType.Increase:
                    increase += buf.Value;
                    break;
                case MathType.Decrease:
                    increase -= buf.Value;
                    break;
                case MathType.Add:
                    value += buf.Value;
                    break;
                case MathType.Remove:
                    value -= buf.Value;
                    break;
            }
        }
        return resultValue[key] = value * increase * 0.01f;
    }
}
