using UnityEngine;

public class MultiplayerBoardInitializer : MonoBehaviour
{
    public GameObject cityAreaPrefab;
    public Vector3 playerOnePosition = new Vector3(-3f, 0f, 0f);
    public Vector3 playerTwoPosition = new Vector3(3f, 0f, 0f);

    private void Start()
    {
        if (cityAreaPrefab == null)
            return;

        // Player 1 city
        GameObject c1 = Instantiate(cityAreaPrefab, playerOnePosition, Quaternion.identity);
        var m1 = c1.GetComponent<CityAreaManager>();
        if (m1 != null)
            m1.playerId = 0;

        // Player 2 city
        GameObject c2 = Instantiate(cityAreaPrefab, playerTwoPosition, Quaternion.identity);
        var m2 = c2.GetComponent<CityAreaManager>();
        if (m2 != null)
            m2.playerId = 1;
    }
}


