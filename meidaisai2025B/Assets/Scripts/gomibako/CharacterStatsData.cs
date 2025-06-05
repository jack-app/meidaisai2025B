using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsData", menuName = "ScriptableObjects/CharacterStatsData")]
public class CharacterStatsData : ScriptableObject
{
    public string characterName = "New Character";
    
    [Header("基本ステータス")]
    public int maxHealth = 100;
    public int attackPower = 10;
    public float moveSpeed = 5f;
}
