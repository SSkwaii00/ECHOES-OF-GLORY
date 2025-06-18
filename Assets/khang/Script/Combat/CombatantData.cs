using UnityEngine;

[CreateAssetMenu(fileName = "New Combatant", menuName = "Combatant/New Combatant", order = 1)]
public class CombatantData : ScriptableObject
{
    public string Name;
    public int Level = 1;
    public int HP = 100;
    public int Attack = 10;
    public int Defense = 5;
    public int Agility = 50;
    public float CritRate = 5.0f;
    public float CritDamage = 50.0f;
    public Sprite AvatarSprite;
    public string Element = "None"; // Thay ElementType bằng string, có thể mở rộng thành enum sau
    public string Path = "Default";
    public int ArtPoints = 0;
}