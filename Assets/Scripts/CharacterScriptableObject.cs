using UnityEngine;

public enum AttackType {
    Rock,
    Scissors,
    Paper,
}

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character", order = 1)]
public class CharacterScriptableObject : ScriptableObject {
    public int powerLevel;
    public Sprite portrait;
    public AttackType attackType;
}