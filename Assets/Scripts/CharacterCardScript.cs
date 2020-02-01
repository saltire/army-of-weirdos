using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterCardScript : MonoBehaviour {
    public CharacterScriptableObject character;
    public TextMeshPro nameHeader;
    public SpriteRenderer portrait;

    public void SetCharacter(CharacterScriptableObject charObject) {
        character = charObject;
        nameHeader.text = charObject.name;
        portrait.sprite = charObject.portrait;
    }
}
