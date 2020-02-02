using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconScript : MonoBehaviour {
    public Sprite brokenSprite;

    SpriteRenderer spriter;
    Sprite initialSprite;

    void Start() {
        initialSprite = GetComponent<SpriteRenderer>().sprite;
    }
    
    public void ToggleBroken(bool broken) {
        GetComponent<SpriteRenderer>().sprite = broken ? brokenSprite : initialSprite;
    }
}
