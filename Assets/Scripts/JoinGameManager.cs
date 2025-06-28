using UnityEngine;
using TMPro;

public class JoinGameManager : MonoBehaviour
{
    public TMP_InputField roomCodeInput;
    public CreateGameManager createGameManager;

    public void TryJoinRoom()
    {
        if (roomCodeInput == null)
        {
            Debug.LogError("roomCodeInput boş! Doğru objeye bağlı değil.");
            return;
        }
        else
        {
            Debug.Log("InputField bağlı → Mevcut değer: " + roomCodeInput.text);
        }
        
        if (roomCodeInput == null || createGameManager == null)
        {
            Debug.LogError("Referanslardan biri eksik!");
            return;
        }

        string enteredCode = roomCodeInput.text.Trim().ToUpper();
        string createdCode = createGameManager.GetCurrentRoomCode();

        Debug.Log("Entered Code: " + enteredCode);
        Debug.Log("Created Code: " + createdCode);

        if (string.IsNullOrEmpty(createdCode))
        {
            Debug.LogWarning("createdCode boş. CreateRoom() hiç çağrılmamış olabilir.");
        }

        if (enteredCode == createdCode)
        {
            Debug.Log("Join successful! Room Code matched: " + enteredCode);
        }
        else
        {
            Debug.LogWarning("Join failed. Entered code does not match any room.");
        }
    }

}
