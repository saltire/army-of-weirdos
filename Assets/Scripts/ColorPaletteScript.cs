using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorPaletteScript : MonoBehaviour {
    public Color[] colors;
    public GameObject swatchPrefab;
    public float readyInterval = 1.5f;

    Vector3 startPos;
    int[] playerColorIndices = new int[] { 0, 1 };
    public ColorSelectScript[] selectors = new ColorSelectScript[2];

    public AudioSource sfxSource;
    public AudioClip[] blips = new AudioClip[2];

    void Start() {
        startPos = transform.position + Vector3.left * swatchPrefab.transform.localScale.x * (colors.Length / 2.0f - 0.5f);

        for (int c = 0; c < colors.Length; c++) {
            GameObject swatch = Instantiate<GameObject>(swatchPrefab, GetSwatchPos(c), Quaternion.identity);
            swatch.GetComponent<MeshRenderer>().material.SetColor("_Color", colors[c]);
        }

        for (int p = 0; p < 2; p++) {
            SetPlayerColor(p, playerColorIndices[p]);
        }
    }

    public Vector3 GetSwatchPos(int c) {
        return startPos + Vector3.right * swatchPrefab.transform.localScale.x * c;
    }

    public void UpdatePlayerColor(int p, int change) {
        int c = (playerColorIndices[p] + change + colors.Length) % colors.Length;
        playerColorIndices[p] = c;
        if (c == playerColorIndices[1 - p]) {
            UpdatePlayerColor(p, change);
        }
        else {
            SetPlayerColor(p, c);
            sfxSource.clip = blips[p];
            sfxSource.Play();
        }
    }

    void SetPlayerColor(int p, int c) {
        selectors[p].marker.transform.position = GetSwatchPos(c);
        selectors[p].readyText.color = colors[c];
        Options.playerColors[p] = colors[c];
    }

    public void OnStart() {
        if (selectors.All(selector => selector.readyText.gameObject.activeSelf)) {
            StartCoroutine(ReadyTimer());
        }
    }

    IEnumerator ReadyTimer() {
        yield return new WaitForSeconds(readyInterval);
        SceneManager.LoadScene("game");
    }
}
