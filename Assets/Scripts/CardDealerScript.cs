using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealerScript : MonoBehaviour {
    public PlayerScript[] players;

    public GameObject cardPrefab;
    public Transform dealerPlaceholder;
    public Vector3 cardSpacing = new Vector3(0, 1f, -0.2f);
    public float cardStackInterval = 0.1f;
    public float cardDealInterval = 0.1f;
    public float cardDealDuration = 0.3f;
    public float postInterval = 0.2f;

    List<CharacterScriptableObject> characters;
    Stack<GameObject> dealerCards = new Stack<GameObject>();

    void Start() {
        characters = new List<CharacterScriptableObject>(Resources.LoadAll<CharacterScriptableObject>("Characters"));

        StartCoroutine("StackCards");
    }

    IEnumerator StackCards() {
        System.Random rnd = new System.Random();

        int cardCount = characters.Count;
        for (int i = 0; i < cardCount; i++) {
            yield return new WaitForSeconds(cardStackInterval);

            GameObject card = Instantiate<GameObject>(cardPrefab, dealerPlaceholder.position + cardSpacing * dealerCards.Count, dealerPlaceholder.rotation);
            card.transform.parent = transform;
            dealerCards.Push(card);
            
            CharacterScriptableObject character = characters[rnd.Next(characters.Count)];
            card.GetComponent<CharacterCardScript>().SetCharacter(character);
            characters.Remove(character);
        }
        yield return new WaitForSeconds(postInterval);
        
        StartCoroutine("DealCards");
    }

    IEnumerator DealCards() {
        Stack<GameObject>[] playerCards = new Stack<GameObject>[] {
            new Stack<GameObject>(),
            new Stack<GameObject>(),
        };

        int playerIndex = 0;
        while (dealerCards.Count > 0) {
            yield return new WaitForSeconds(cardDealInterval);

            PlayerScript player = players[playerIndex];

            GameObject card = dealerCards.Pop();
            Vector3 startPos = card.transform.position;
            Vector3 targetPos = player.deckPlaceholder.position + cardSpacing * playerCards[playerIndex].Count;
            float startTime = Time.time;
            while (Time.time < startTime + cardDealDuration) {
                card.transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, (Time.time - startTime) / cardDealDuration));
                yield return null;
            }

            playerCards[playerIndex].Push(card);
            playerIndex = 1 - playerIndex;
        }
        yield return new WaitForSeconds(postInterval);

        // Move cards from each player's stack into a queue.
        for (int p = 0; p < playerCards.Length; p++) {
            while (playerCards[p].Count > 0) {
                players[p].playerCards.Enqueue(playerCards[p].Pop());
            }

            players[p].NextCard();
        }
    }
}
