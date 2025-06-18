using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SceneExporter : MonoBehaviour
{
    [MenuItem("Tools/Export Scene To JSON")]
    public static void ExportSceneToJson()
    {
        List<SceneObjectData> allObjects = new List<SceneObjectData>();

        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject rootObj in rootObjects)
        {
            TraverseHierarchy(rootObj, null, allObjects);
        }

        string json = JsonHelper.ToJson(allObjects.ToArray(), true);
        string path = Application.dataPath + "/scene_export.json";
        File.WriteAllText(path, json);
        Debug.Log("Scene exported to: " + path);
    }

    private static void TraverseHierarchy(GameObject obj, string parentName, List<SceneObjectData> allObjects)
    {
        SceneObjectData data = new SceneObjectData
        {
            name = obj.name,
            tag = obj.tag,
            layer = LayerMask.LayerToName(obj.layer),
            parent = parentName,
            position = obj.transform.position,
            rotation = obj.transform.eulerAngles,
            scale = obj.transform.localScale,
            components = new List<string>()
        };

        Component[] comps = obj.GetComponents<Component>();
        foreach (Component c in comps)
        {
            if (c != null)
                data.components.Add(c.GetType().Name);
        }

        allObjects.Add(data);

        foreach (Transform child in obj.transform)
        {
            TraverseHierarchy(child.gameObject, obj.name, allObjects);
        }
    }

    [System.Serializable]
    public class SceneObjectData
    {
        public string name;
        public string tag;
        public string layer;
        public string parent;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public List<string> components;
    }

    public static class JsonHelper
    {
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T> { Items = array };
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
