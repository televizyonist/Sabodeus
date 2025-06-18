using UnityEngine;
using TMPro;

public class CreateGameManager : MonoBehaviour
{
    public TMP_Text roomCodeText;

    private string currentRoomCode;

    public void CreateRoom()
    {
        currentRoomCode = GenerateRandomCode(4);
        roomCodeText.text = "Room Code: " + currentRoomCode;
        Debug.Log("Room created with code: " + currentRoomCode);
    }

    public string GetCurrentRoomCode()
    {
        return currentRoomCode;
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        for (int i = 0; i < length; i++)
        {
            int index = Random.Range(0, chars.Length);
            code += chars[index];
        }
        return code;
    }
}
