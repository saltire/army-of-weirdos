using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterCardScript : MonoBehaviour {
    public CharacterScriptableObject character;
    public TextMeshPro nameHeader;
    public SpriteRenderer portrait;

    public AttackScript attackPrefab;
    public Vector3 attackPosition;
    public float attackSpacing;
    public Color[] buttonColors;
    public string[] buttonLabels;

    public void SetCharacter(CharacterScriptableObject charObject) {
        character = charObject;
        nameHeader.text = charObject.name;
        portrait.sprite = charObject.portrait;

        for (int i = 0; i < charObject.attacks.Length; i++) {
            AttackScript attack = Instantiate<AttackScript>(attackPrefab);
            attack.transform.parent = transform;
            attack.transform.localPosition = attackPosition + Vector3.down * attackSpacing * i;
            attack.transform.localRotation = Quaternion.identity;
            attack.button.material.SetColor("_DestColor", buttonColors[i]);
            attack.buttonLabel.text = buttonLabels[i];
            attack.attackName.text = charObject.attacks[i].name;
        }
    }
}
