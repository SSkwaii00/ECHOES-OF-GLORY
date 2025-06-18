using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/New Enemy", order = 2)]
public class EnemyData : ScriptableObject
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
    public string Element = "None";
    public string Path = "Default";
    public int ArtPoints = 0;

    // Thêm Prefab để liên kết với GameObject trong Scene (nếu cần)
    public GameObject Prefab;
}