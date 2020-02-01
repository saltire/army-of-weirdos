using UnityEngine;

[System.Serializable]
public struct Attack {
    public string name;
    public int rock;
    public int scissors;
    public int paper;
}

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character", order = 1)]
public class CharacterScriptableObject : ScriptableObject {
    public Sprite portrait;
    public Attack[] attacks;
}