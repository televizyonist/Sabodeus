using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class AssetScanner
{
    [MenuItem("Tools/Export Asset Structure")]
    public static void ExportAssets()
    {
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        List<string> projectAssets = new List<string>();

        foreach (string path in assetPaths)
        {
            if (path.StartsWith("Assets/") && !path.EndsWith(".meta"))
            {
                projectAssets.Add(path);
            }
        }

        string outputPath = Application.dataPath + "/asset_list.json";
        File.WriteAllText(outputPath, JsonUtility.ToJson(new AssetWrapper { assets = projectAssets.ToArray() }, true));
        Debug.Log("Asset list exported to: " + outputPath);
    }

    [System.Serializable]
    private class AssetWrapper
    {
        public string[] assets;
    }
}
