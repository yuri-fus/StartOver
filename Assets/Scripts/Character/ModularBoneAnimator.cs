using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteCategory
{
    public string name;
    public bool isActive = true; // включена ли категория
    public List<GameObject> frames = new List<GameObject>();

    [HideInInspector] public int currentFrame = 0;
    [HideInInspector] public float timer = 0f;
}

public class ModularBoneAnimator : MonoBehaviour
{
    public float frameDuration = 0.1f;
    public List<SpriteCategory> categories = new List<SpriteCategory>();

    void Start()
    {
        // Отключаем все спрайты в начале
        foreach (var category in categories)
        {
            foreach (var frame in category.frames)
            {
                if (frame != null)
                    frame.SetActive(false);
            }

            if (category.isActive && category.frames.Count > 0)
            {
                category.frames[0].SetActive(true);
            }
        }
    }

    void Update()
    {
        foreach (var category in categories)
        {
            if (!category.isActive || category.frames.Count == 0)
                continue;

            category.timer += Time.deltaTime;
            if (category.timer >= frameDuration)
            {
                category.frames[category.currentFrame].SetActive(false);
                category.currentFrame = (category.currentFrame + 1) % category.frames.Count;
                category.frames[category.currentFrame].SetActive(true);
                category.timer = 0f;
            }
        }
    }
    
    // Включить категорию по имени
    public void EnableCategory(string categoryName)
    {
        var category = categories.Find(c => c.name == categoryName);
        if (category != null && !category.isActive)
        {
            category.isActive = true;

            // Включаем текущий кадр
            if (category.frames.Count > 0)
            {
                foreach (var frame in category.frames)
                    frame.SetActive(false);

                category.currentFrame = 0;
                category.timer = 0f;
                category.frames[0].SetActive(true);
            }
        }
    }
    

    // Выключить категорию по имени
    public void DisableCategory(string categoryName)
    {
        var category = categories.Find(c => c.name == categoryName);
        if (category != null && category.isActive)
        {
            category.isActive = false;

            // Отключаем все кадры
            foreach (var frame in category.frames)
            {
                if (frame != null)
                    frame.SetActive(false);
            }
        }
    }


    public void SwitchHeadCategory(string categoryToEnable)
    {
        // Считаем все категории с именем, содержащим "голова"
        foreach (var category in categories)
        {
            if (category.name.ToLower().Contains("head"))
            {
                // Отключаем все категории головы
                if (category.isActive)
                {
                    DisableCategory(category.name);
                }
            }
        }

        // Включаем указанную
        EnableCategory(categoryToEnable);
    }

    // Универсальный метод для переключения категории в заданной группе
    public void SwitchCategoryInGroup(string groupName, string categoryToEnable)
    {
        if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(categoryToEnable))
            return;

        string groupKey = groupName.ToLower();

        foreach (var category in categories)
        {
            if (category.name.ToLower().Contains(groupKey))
            {
                DisableCategory(category.name);
            }
        }

        EnableCategory(categoryToEnable);
    }
}
