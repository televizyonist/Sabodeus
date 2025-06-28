using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;

public class JoinGameManager : MonoBehaviour
{
    public TMP_InputField roomCodeInput;
    public RelayManager relayManager;
    public TMP_Text feedbackText;

    public void TryJoinRoom()
    {
        if (roomCodeInput == null || relayManager == null)
        {
            Debug.LogError("[JoinGameManager] Gerekli referanslar eksik!");
            return;
        }

        string enteredCode = roomCodeInput.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(enteredCode))
        {
            Debug.LogWarning("[JoinGameManager] Kod boş!");
            SetFeedback("Please enter a code.");
            return;
        }

        SetFeedback("Joining...");

        relayManager.JoinRelayWithCallback(enteredCode, () =>
        {
            Debug.Log("[JoinGameManager] Join successful with code: " + enteredCode);
            SetFeedback("Joined room " + enteredCode + "!");
        });

        StartCoroutine(CheckJoinResult());
    }

    private System.Collections.IEnumerator CheckJoinResult()
    {
        yield return new WaitForSeconds(2f);

        if (!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogWarning("[JoinGameManager] Join failed.");
            SetFeedback("Join failed. Check the code and try again.");
        }
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

}
