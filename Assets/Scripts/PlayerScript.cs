using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour {
    public Color playerColor;

    public SpriteRenderer playerSprite;
    public Transform cardPlaceholder;
    public Transform deckPlaceholder;

    public float cardFlipDuration = 0.3f;

    public Queue<GameObject> playerCards = new Queue<GameObject>();

    public bool showAttackValues = false;

    CharacterCardScript currentCard;

    void Start() {
        if (playerColor != null) {
            playerSprite.material.SetColor("_DestColor", playerColor);
        }
    }

    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed) {
            playerSprite.material.SetColor("_DestColor", Color.magenta);
        }
        else if (context.canceled) {
            playerSprite.material.SetColor("_DestColor", playerColor);
        }
    }

    public void ToggleAttackIcons(InputAction.CallbackContext context) {
        if (currentCard != null) {
            if (context.performed) {
                currentCard.ToggleAttackIcons(true);
            }
            else if (context.canceled) {
                currentCard.ToggleAttackIcons(false);
            }
        }
    }

    public void NextCard() {
        StartCoroutine("FlipTopCard");
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
    }
}
