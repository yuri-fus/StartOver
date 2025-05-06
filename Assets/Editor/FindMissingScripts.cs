using UnityEngine;
using UnityEditor;

public class FindMissingScripts : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    static void FindMissingScriptsInScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int missingCount = 0;

        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    Debug.LogWarning($"Найден объект с отсутствующим скриптом: {obj.name}", obj);
                    missingCount++;
                }
            }
        }

        if (missingCount == 0)
        {
            Debug.Log("В сцене нет объектов с отсутствующими скриптами.");
        }
    }
}
