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
    private int currentPlayer = 0; // 0 = player 1, 1 = player 2

    public int CurrentPlayer => currentPlayer;

    public bool IsPlayerTurn(int playerId) => playerId == currentPlayer;

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
        currentPlayer = (currentPlayer + 1) % 2;
        UpdateTurnText();
    }

    public void PassTurn()
    {
        EndTurn();
    }

    private void UpdateTurnText()
    {
        if (turnText != null)
            turnText.text = currentPlayer == 0 ? "Player 1" : "Player 2";
    }
}
