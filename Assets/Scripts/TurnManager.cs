using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [Tooltip("UI text to display which player's turn it is")]
    public TMP_Text turnText;
    [Tooltip("How many cards can be played per turn")]
    public int cardsPerTurn = 2;

    private int cardsPlayedThisTurn = 0;
    private bool isPlayerOneTurn = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateTurnText();
    }

    public void CardPlayed()
    {
        cardsPlayedThisTurn++;
        if (cardsPlayedThisTurn >= cardsPerTurn)
            EndTurn();
    }

    public void EndTurn()
    {
        cardsPlayedThisTurn = 0;
        isPlayerOneTurn = !isPlayerOneTurn;
        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        if (turnText != null)
            turnText.text = isPlayerOneTurn ? "Player 1" : "Player 2";
    }
}
