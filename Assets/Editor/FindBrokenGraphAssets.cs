using UnityEngine;
using UnityEditor;
using System.IO;

public class FindBrokenGraphAssets : MonoBehaviour
{
    [MenuItem("Tools/Find Broken Graph Assets")]
    static void FindBrokenAssets()
    {
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        int brokenFilesCount = 0;

        foreach (string path in assetPaths)
        {
            if (path.EndsWith(".controller") || path.EndsWith(".anim") || path.EndsWith(".shadergraph") || path.EndsWith(".vfx") || path.EndsWith(".playable") || path.EndsWith(".prefab"))
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

                if (asset == null)
                {
                    Debug.LogError($"Ошибка: Файл {path} повреждён или отсутствует!");
                    brokenFilesCount++;
                    continue;
                }

                try
                {
                    // Пробуем получить зависимости ассета
                    Object[] dependencies = EditorUtility.CollectDependencies(new Object[] { asset });

                    foreach (Object dep in dependencies)
                    {
                        if (dep == null)
                        {
                            Debug.LogError($"Файл {path} содержит повреждённые ссылки!", asset);
                            brokenFilesCount++;
                            break;
                        }
                    }
                }
                catch
                {
                    Debug.LogError($"Ошибка при обработке {path}. Возможно, он повреждён!", asset);
                    brokenFilesCount++;
                }
            }
        }

        if (brokenFilesCount == 0)
        {
            Debug.Log("Все проверенные ассеты в порядке!");
        }
    }
}
