using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour {
    public Color playerColor;

    public Transform cardPlaceholder;
    public Transform deckPlaceholder;

    public float cardFlipDuration = 0.3f;

    public Queue<GameObject> playerCards = new Queue<GameObject>();

    void Start() {
        if (playerColor != null) {
            GetComponent<SpriteRenderer>().material.SetColor("_DestColor", playerColor);
        }
    }

    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed) {
            GetComponent<SpriteRenderer>().material.SetColor("_DestColor", Color.magenta);
        }
        else if (context.canceled) {
            GetComponent<SpriteRenderer>().material.SetColor("_DestColor", playerColor);
        }
    }

    public void NextCard() {
        StartCoroutine("FlipTopCard");
    }

    IEnumerator FlipTopCard() {
        GameObject card = playerCards.Peek();

        card.GetComponent<CharacterCardScript>().portrait.material.SetColor("_DestColor", playerColor);

        Vector3 startPos = card.transform.position;
        Quaternion startRot = card.transform.rotation;
        float startTime = Time.time;
        while (Time.time < startTime + cardFlipDuration) {
            float step = Mathf.SmoothStep(0, 1, (Time.time - startTime) / cardFlipDuration);
            card.transform.position = Vector3.Lerp(startPos, cardPlaceholder.position, step);
            card.transform.rotation = Quaternion.Lerp(startRot, cardPlaceholder.rotation, step);
            yield return null;
        }
    }
}
