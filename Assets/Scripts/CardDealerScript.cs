using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardDealerScript : MonoBehaviour {
    public PlayerScript[] players;

    public GameObject cardPrefab;
    public Transform dealerPlaceholder;
    public GameObject selectAttackText;
    public GameObject winText;
    public Vector3 cardSpacing = new Vector3(0, 1f, -0.2f);
    public float cardStackInterval = 0.1f;
    public float cardDealInterval = 0.1f;
    public float cardDealDuration = 0.3f;
    public float cardReturnDuration = 0.5f;
    public float postInterval = 0.2f;
    public float readyInterval = 0.5f;
    public float finishInterval = 2.0f;
    public float winInterval = 2.0f;
    public string nextScene;
    public AudioSource audioSource;
    public AudioClip winSong;

    System.Random rnd;
    List<CharacterScriptableObject> characters;
    Stack<GameObject> dealerCards = new Stack<GameObject>();
    Queue<GameObject> tiedCards = new Queue<GameObject>();

    void Start() {
        rnd = new System.Random();
        characters = new List<CharacterScriptableObject>(Resources.LoadAll<CharacterScriptableObject>("Characters"));

        StartCoroutine("StackCards");
    }

    IEnumerator StackCards() {
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

            PlayerScript winningPlayer;
            PlayerScript losingPlayer;
            Queue<GameObject> targetQueue;
            Transform targetTransform;
            if (players[0].finalDamage == players[1].finalDamage) {
                int rndIndex = rnd.Next(2);
                winningPlayer = players[rndIndex];
                losingPlayer = players[1 - rndIndex];
                targetQueue = tiedCards;
                targetTransform = dealerPlaceholder;
                foreach (PlayerScript player in players) {
                    player.ShowTie();
                }

                // TODO: Currently tying on a player's last card causes them to lose the game.
                // Should let them keep that card and go another round.
            }
            else {
                int winIndex = players[0].finalDamage > players[1].finalDamage ? 0 : 1;
                winningPlayer = players[winIndex];
                losingPlayer = players[1 - winIndex];
                targetQueue = winningPlayer.playerCards;
                targetTransform = winningPlayer.deckPlaceholder;
                winningPlayer.ShowWinner();

                // Move all tied cards into the target queue.
                while (tiedCards.Count > 0) {
                    targetQueue.Enqueue(tiedCards.Dequeue());
                }
            }
            // Move the front card from each player's queue into the end of the target queue, losing card first.
            targetQueue.Enqueue(losingPlayer.playerCards.Dequeue());
            targetQueue.Enqueue(winningPlayer.playerCards.Dequeue());

            yield return new WaitForSeconds(finishInterval);
            foreach (PlayerScript player in players) {
                player.FinishRound();
            }

            // Animate all the cards in the winning player's queue to their new position.
            List<Vector3> cardPosStarts = new List<Vector3>();
            List<Quaternion> cardRotStarts = new List<Quaternion>();
            List<Vector3> cardPosTargets = new List<Vector3>();
            for (int i = 0; i < targetQueue.Count; i++) {
                GameObject card = targetQueue.ElementAt(i);
                cardPosStarts.Add(card.transform.position);
                cardRotStarts.Add(card.transform.rotation);
                cardPosTargets.Add(targetTransform.position + cardSpacing * (targetQueue.Count - i - 1));
            }
            float startTime = Time.time;
            while (Time.time < startTime + cardReturnDuration) {
                float step = Mathf.SmoothStep(0, 1, (Time.time - startTime) / cardReturnDuration);
                for (int i = 0; i < targetQueue.Count; i++) {
                    GameObject card = targetQueue.ElementAt(i);
                    card.transform.position = Vector3.Lerp(cardPosStarts[i], cardPosTargets[i], step);
                    card.transform.rotation = Quaternion.Lerp(cardRotStarts[i], targetTransform.rotation, step);
                }
                yield return null;
            }

            yield return new WaitForSeconds(readyInterval);
        }

        int winnerIndex = players[0].playerCards.Count > 0 ? 0 : 1;
        TextMeshPro winTextMesh = winText.GetComponent<TextMeshPro>();
        winTextMesh.text = $"PLAYER {winnerIndex + 1} WINS!";
        winTextMesh.color = Options.playerColors[winnerIndex];
        winText.SetActive(true);

        audioSource.clip = winSong;
        audioSource.loop = false;
        audioSource.Play();

        yield return new WaitForSeconds(winInterval);

        SceneManager.LoadScene(nextScene);
    }
}
