using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitGameHandler : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Çıkış yapılıyor...");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
