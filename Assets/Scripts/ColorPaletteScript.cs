using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPaletteScript : MonoBehaviour {
    public Color[] colors;
    public GameObject swatchPrefab;
    public ColorSelectScript markerPrefab;

    Vector3 startPos;
    int[] playerColorIndices = new int[] { 0, 1 };
    ColorSelectScript[] playerMarkers = new ColorSelectScript[2];

    void Start() {
        startPos = transform.position + Vector3.left * swatchPrefab.transform.localScale.x * (colors.Length / 2.0f - 0.5f);
        for (int c = 0; c < colors.Length; c++) {
            GameObject swatch = Instantiate<GameObject>(swatchPrefab, GetSwatchPos(c), Quaternion.identity);
            swatch.GetComponent<MeshRenderer>().material.SetColor("_Color", colors[c]);
        }
        for (int p = 0; p < 2; p++) {
            Options.playerColors[p] = colors[playerColorIndices[p]];
            playerMarkers[p] = Instantiate<ColorSelectScript>(markerPrefab, GetSwatchPos(playerColorIndices[p]), Quaternion.identity);
            playerMarkers[p].Initialize(this, p);
        }
    }

    Vector3 GetSwatchPos(int c) {
        return startPos + Vector3.right * swatchPrefab.transform.localScale.x * c;
    }

    public void UpdatePlayerColor(int p, int change) {
        int c = (playerColorIndices[p] + change + colors.Length) % colors.Length;
        playerColorIndices[p] = c;
        if (c == playerColorIndices[1 - p]) {
            UpdatePlayerColor(p, change);
        }
        else {
            playerMarkers[p].transform.position = GetSwatchPos(c);
            Options.playerColors[p] = colors[c];
        }
    }
}
