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

    public void SetCharacter(CharacterScriptableObject charObject) {
        character = charObject;
        nameHeader.text = charObject.name;
        portrait.sprite = charObject.portrait;

        for (int i = 0; i < charObject.attacks.Length; i++) {
            AttackScript attackLabel = Instantiate<AttackScript>(attackPrefab);
            attackLabel.transform.parent = transform;
            attackLabel.transform.localPosition = attackPosition + Vector3.down * attackSpacing * i;
            attackLabel.transform.localRotation = Quaternion.identity;

            attackLabel.SetAttack(i, charObject.attacks[i]);
        }
    }

    public void ToggleAttackIcons(bool show) {
        foreach (AttackScript attackLabel in GetComponentsInChildren<AttackScript>()) {
            attackLabel.ToggleAttackIcons(show);
        }
    }
}
