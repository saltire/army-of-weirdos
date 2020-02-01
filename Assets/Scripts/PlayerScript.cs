using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour {
    public Color playerColor;

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
}
