using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour {
    public Color playerColor;

    public SpriteRenderer playerSprite;
    public Transform cardPlaceholder;
    public Transform deckPlaceholder;
    public GameObject readyText;

    public float cardFlipDuration = 0.3f;

    public bool showAttackValues = false;
    public bool waitingForAttack = false;

    public Queue<GameObject> playerCards { get; } = new Queue<GameObject>();
    public Attack? selectedAttack { get; private set; }

    CharacterCardScript currentCard;

    void Start() {
        if (playerColor != null) {
            playerSprite.material.SetColor("_DestColor", playerColor);
        }
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

        playerSprite.enabled = true;
        waitingForAttack = true;
        selectedAttack = null;
        readyText.SetActive(false);
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
        if (waitingForAttack && context.performed) {
            Attack? attack = currentCard.GetButtonAttack(context.control.displayName);
            if (attack != null) {
                selectedAttack = attack;
                readyText.SetActive(true);
            }
        }
    }
}
