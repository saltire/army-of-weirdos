using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDealerScript : MonoBehaviour {
    public PlayerScript[] players;

    public GameObject cardPrefab;
    public Transform dealerPlaceholder;
    public GameObject selectAttackText;
    public GameObject continueText;
    public Vector3 cardSpacing = new Vector3(0, 1f, -0.2f);
    public float cardStackInterval = 0.1f;
    public float cardDealInterval = 0.1f;
    public float cardDealDuration = 0.3f;
    public float postInterval = 0.2f;
    public float readyInterval = 0.5f;

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
        
        StartCoroutine(DealCards());
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
        }

        StartCoroutine(Game());
    }

    IEnumerator Game() {
        while (players.All(player => player.playerCards.Count > 0)) {
            // Flip up the top cards.
            foreach (PlayerScript player in players) {
                player.StartCoroutine("FlipTopCard");
            }

            // Wait for animations to finish.
            while (players.Any(player => !player.waitingForAttack)) {
                yield return null;
            }
            selectAttackText.SetActive(true);

            // Wait for both players to select an attack.
            while (players.Any(player => player.selectedAttack == null)) {
                yield return null;
            }
            foreach (PlayerScript player in players) {
                player.DisableAttackSelect();
            }
            selectAttackText.SetActive(false);
            yield return new WaitForSeconds(readyInterval);

            // Display the attack animations and calculate damage.
            players[0].StartAttack(players[0].selectedAttack.Value, players[1].selectedAttack.Value);
            players[1].StartAttack(players[1].selectedAttack.Value, players[0].selectedAttack.Value);
            // Wait for both players to calculate their final damage.
            while (players.Any(player => player.finalDamage == null)) {
                yield return null;
            }

            // Show the winner text on the winning player.
            ((players[0].finalDamage > players[1].finalDamage) ? players[0] : players[1]).ShowWinner();
            continueText.SetActive(true);

            // Wait for both players to hit a button to finish.
            while (players.Any(player => player.waitingForFinish)) {
                yield return null;
            }
            continueText.SetActive(false);
            yield return new WaitForSeconds(readyInterval);
            foreach (PlayerScript player in players) {
                player.FinishRound();
            }

            while (true) {
                yield return null;
            }
        }
    }
}
