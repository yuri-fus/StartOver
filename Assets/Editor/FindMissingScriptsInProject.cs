using UnityEngine;
using UnityEditor;
using System.IO;

public class FindMissingScriptsInProject : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts in Project")]
    static void FindScriptsInProject()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int missingCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Component[] components = prefab.GetComponentsInChildren<Component>(true);

            foreach (Component component in components)
            {
                if (component == null)
                {
                    Debug.LogWarning($"Найден Missing Script в Prefab: {path}", prefab);
                    missingCount++;
                    break;
                }
            }
        }

        if (missingCount == 0)
        {
            Debug.Log("В проекте нет объектов с отсутствующими скриптами.");
        }
    }
}
