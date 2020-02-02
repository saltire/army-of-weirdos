using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorSelectScript : MonoBehaviour {
    public int playerIndex;
    public ColorPaletteScript palette;
    public TextMeshPro marker;
    public TextMeshPro readyText;

    public void Initialize(ColorPaletteScript palette, int p) {
        this.palette = palette;
        playerIndex = p;
        GetComponent<TextMeshPro>().text = $"{p + 1}";
    }

    public void OnNavigate(InputAction.CallbackContext context) {
        if (context.performed) {
            if (context.control.name.Equals("left")) {
                palette.UpdatePlayerColor(playerIndex, -1);
            }
            else if (context.control.name.Equals("right")) {
                palette.UpdatePlayerColor(playerIndex, 1);
            }
        }
    }

    public void OnStart(InputAction.CallbackContext context) {
        if (context.performed) {
            readyText.gameObject.SetActive(true);
            palette.OnStart();
        }
    }
}
