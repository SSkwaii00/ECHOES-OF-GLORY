using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string SkillName;
    public float DamageMultiplier = 1f;
    public bool IsAoE;
    public float SlowChance;
    public GameObject EffectPrefab;
    public AudioClip SoundClip;
    public string AnimationTrigger;
    public StatusEffect StatusEffect;
    public float StatusEffectChance;
    public int StatusEffectDuration;
}