using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class StartGameHandler : MonoBehaviour
{
    public void StartGame()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                "SampleScene",
                LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("StartGame called without host authority. Loading locally.");
            SceneManager.LoadScene("SampleScene");
        }
    }
}
