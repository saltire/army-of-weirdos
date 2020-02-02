using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour {
    public int playerIndex;
    public Color playerColor;

    public SpriteRenderer playerSprite;
    public Transform cardPlaceholder;
    public Transform deckPlaceholder;
    public Transform iconContainer;
    public GameObject readyText;
    public GameObject winnerText;
    public GameObject tieText;

    public float cardFlipDuration = 0.3f;

    public Queue<GameObject> playerCards { get; } = new Queue<GameObject>();
    public bool waitingForAttack { get; private set; } = false;
    public Attack? selectedAttack { get; private set; }
    public int? finalDamage { get; private set; }
    // public bool waitingForFinish { get; private set; } = false;

    public IconScript rockPrefab;
    public IconScript scissorsPrefab;
    public IconScript paperPrefab;
    public float iconSpacing;
    public float iconScale = 1;
    public float iconMoveDistance = 2;
    public float iconMoveDuration = 0.2f;

    CharacterCardScript currentCard;

    void Start() {
        if (Options.playerColors[playerIndex] != null) {
            playerColor = Options.playerColors[playerIndex];
        }
        playerSprite.material.SetColor("_DestColor", playerColor);
    }

    IEnumerator FlipTopCard() {
        currentCard = playerCards.Peek().GetComponent<CharacterCardScript>();

        currentCard.portrait.material.SetColor("_DestColor", playerColor);
        currentCard.ToggleAttackIcons(false);

        Vector3 startPos = currentCard.transform.position;
        Quaternion startRot = currentCard.transform.rotation;
        float startTime = Time.time;
        while (Time.time < startTime + cardFlipDuration) {
            float step = Mathf.SmoothStep(0, 1, (Time.time - startTime) / cardFlipDuration);
            currentCard.transform.position = Vector3.Lerp(startPos, cardPlaceholder.position, step);
            currentCard.transform.rotation = Quaternion.Lerp(startRot, cardPlaceholder.rotation, step);
            yield return null;
        }

        playerSprite.sprite = currentCard.character.portrait;
        playerSprite.enabled = true;
        waitingForAttack = true;
        selectedAttack = null;
        readyText.SetActive(false);
        finalDamage = null;
    }

    public void ToggleAttackIcons(InputAction.CallbackContext context) {
        if (currentCard != null && waitingForAttack) {
            if (context.performed) {
                currentCard.ToggleAttackIcons(true);
            }
            else if (context.canceled) {
                currentCard.ToggleAttackIcons(false);
            }
        }
    }

    public void OnAttackSelect(InputAction.CallbackContext context) {
        if (context.performed) {
            if (waitingForAttack) {
                Attack? attack = currentCard.GetButtonAttack(context.control.displayName);
                if (attack != null) {
                    selectedAttack = attack;
                    readyText.SetActive(true);
                }
            }
            // else if (waitingForFinish) {
            //     waitingForFinish = false;
            //     readyText.SetActive(true);
            // }
        }
    }

    public void DisableAttackSelect() {
        waitingForAttack = false;
    }

    public void StartAttack(Attack attack, Attack counterattack) {
        readyText.SetActive(false);

        StartCoroutine(ExecuteAttack(attack, counterattack));
    }

    IEnumerator ExecuteAttack(Attack attack, Attack counterattack) {
        Vector3 xSpacing = (playerIndex == 1 ? Vector3.right : Vector3.left) * iconSpacing;
        Vector3 ySpacing = Vector3.down * iconSpacing;

        List<IconScript> rockIcons = new List<IconScript>();
        List<IconScript> scissorsIcons = new List<IconScript>();
        List<IconScript> paperIcons = new List<IconScript>();

        for (int j = 0; j < attack.rock; j++) {
            IconScript icon = Instantiate<IconScript>(rockPrefab);
            icon.transform.parent = iconContainer;
            icon.transform.localPosition = xSpacing * j;
            icon.transform.localScale = Vector3.one * iconScale;
            rockIcons.Add(icon);
        }
        for (int j = 0; j < attack.scissors; j++) {
            IconScript icon = Instantiate<IconScript>(scissorsPrefab);
            icon.transform.parent = iconContainer;
            icon.transform.localPosition = xSpacing * j + ySpacing;
            icon.transform.localScale = Vector3.one * iconScale;
            scissorsIcons.Add(icon);
        }
        for (int j = 0; j < attack.paper; j++) {
            IconScript icon = Instantiate<IconScript>(paperPrefab);
            icon.transform.parent = iconContainer;
            icon.transform.localPosition = xSpacing * j + ySpacing * 2;
            icon.transform.localScale = Vector3.one * iconScale;
            paperIcons.Add(icon);
        }

        Vector3 startPos = iconContainer.position;
        Vector3 targetPos = iconContainer.position + (playerIndex == 1 ? Vector3.left : Vector3.right) * iconMoveDistance;
        float startTime = Time.time;
        while (Time.time < startTime + iconMoveDuration) {
            iconContainer.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, (Time.time - startTime) / iconMoveDuration));
            yield return null;
        }

        rockIcons.GetRange(0, Mathf.Min(rockIcons.Count, counterattack.paper)).ForEach(icon => icon.ToggleBroken(true));
        scissorsIcons.GetRange(0, Mathf.Min(scissorsIcons.Count, counterattack.rock)).ForEach(icon => icon.ToggleBroken(true));
        paperIcons.GetRange(0, Mathf.Min(paperIcons.Count, counterattack.scissors)).ForEach(icon => icon.ToggleBroken(true));

        startTime = Time.time;
        while (Time.time < startTime + iconMoveDuration) {
            iconContainer.position = Vector3.Lerp(targetPos, startPos, Mathf.SmoothStep(0, 1, (Time.time - startTime) / iconMoveDuration));
            yield return null;
        }
        
        // waitingForFinish = true;
        finalDamage = Mathf.Max(attack.rock - counterattack.paper, 0) + 
            Mathf.Max(attack.scissors - counterattack.rock, 0) + 
            Mathf.Max(attack.paper - counterattack.scissors, 0);
    }

    public void ShowWinner() {
        winnerText.SetActive(true);
    }

    public void ShowTie() {
        tieText.SetActive(true);
    }

    public void FinishRound() {
        readyText.SetActive(false);
        winnerText.SetActive(false);
        tieText.SetActive(false);
        playerSprite.enabled = false;
        
        foreach (Transform icon in iconContainer.transform) {
            Destroy(icon.gameObject);
        }
    }
}
