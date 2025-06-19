using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
