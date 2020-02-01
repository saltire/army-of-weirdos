using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManagerScript : MonoBehaviour {
    public PlayerScript[] players;

    public GameObject cardPrefab;
    public Transform dealerPlaceholder;
    public Transform deck1Placeholder;
    public Transform deck2Placeholder;
    public Vector3 cardSpacing = new Vector3(0, 1f, -0.2f);

    void Start() {
        List<CharacterScriptableObject> characters = new List<CharacterScriptableObject>(Resources.LoadAll<CharacterScriptableObject>("Characters"));

        Queue<GameObject> dealerCards = new Queue<GameObject>();
        foreach (CharacterScriptableObject character in characters) {
            GameObject card = Instantiate<GameObject>(cardPrefab, dealerPlaceholder.position + cardSpacing * dealerCards.Count, dealerPlaceholder.rotation);
            card.transform.parent = transform;
            dealerCards.Enqueue(card);
        }

        System.Random rnd = new System.Random();
        int playerIndex = 0;
        while (characters.Count > 0) {
            CharacterScriptableObject character = characters[rnd.Next(characters.Count)];
            characters.Remove(character);

            GameObject card = dealerCards.Dequeue();
            card.GetComponent<CharacterCardScript>().character = character;
            players[playerIndex].playerCards.Enqueue(card);

            playerIndex = 1 - playerIndex;
        }
    }
}
