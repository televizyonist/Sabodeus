using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class StartGameHandler : MonoBehaviour
{
    public void StartGame()
    {
        const string gameplaySceneName = "GamePlay";

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            // Use Netcode scene manager so that every connected client changes
            // scenes together when the host starts the game.
            NetworkManager.Singleton.SceneManager.LoadScene(
                gameplaySceneName,
                LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("StartGame called without host authority. Loading locally.");
            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}
