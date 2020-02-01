using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackScript : MonoBehaviour {
    public SpriteRenderer button;
    public TextMeshPro buttonLabel;
    public TextMeshPro attackName;
    public Transform iconContainer;

    public Color[] buttonColors;
    public string[] buttonLabels;

    public GameObject rockPrefab;
    public GameObject scissorsPrefab;
    public GameObject paperPrefab;
    public Vector3 iconPosition;
    public float iconSpacing;
    public float iconMaxWidth;

    public void SetAttack(int buttonIndex, Attack attack) {
        button.material.SetColor("_DestColor", buttonColors[buttonIndex]);
        buttonLabel.text = buttonLabels[buttonIndex];

        attackName.text = attack.name;

        int total = attack.rock + attack.scissors + attack.paper;
        float spacing = Mathf.Min(iconSpacing, iconMaxWidth / Mathf.Max(total, 1));

        for (int j = 0; j < attack.rock; j++) {
            GameObject icon = Instantiate<GameObject>(rockPrefab);
            icon.transform.parent = iconContainer;
            icon.transform.localPosition = iconPosition + Vector3.right * spacing * (iconContainer.childCount - 1);
            icon.transform.localRotation = Quaternion.identity;
        }
        for (int j = 0; j < attack.scissors; j++) {
            GameObject icon = Instantiate<GameObject>(scissorsPrefab);
            icon.transform.parent = iconContainer;
            icon.transform.localPosition = iconPosition + Vector3.right * spacing * (iconContainer.childCount - 1);
            icon.transform.localRotation = Quaternion.identity;
        }
        for (int j = 0; j < attack.paper; j++) {
            GameObject icon = Instantiate<GameObject>(paperPrefab);
            icon.transform.parent = iconContainer;
            icon.transform.localPosition = iconPosition + Vector3.right * spacing * (iconContainer.childCount - 1);
            icon.transform.localRotation = Quaternion.identity;
        }

        ToggleAttackIcons(false);
    }

    public void ToggleAttackIcons(bool show) {
        iconContainer.gameObject.SetActive(show);
        attackName.gameObject.SetActive(!show);
    }
}
