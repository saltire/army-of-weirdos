using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedMusicScript : MonoBehaviour {
    public float delayTime = 2.0f;

    void Start() {
        GetComponent<AudioSource>().PlayDelayed(delayTime);
    }
}
