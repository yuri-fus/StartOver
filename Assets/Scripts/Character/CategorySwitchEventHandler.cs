using System.Collections.Generic;
using UnityEngine;

public class CategorySwitchEventHandler : MonoBehaviour
{
    public ModularBoneAnimator animator;

    // Метод вызывается из анимационного события
    public void OnSwitchCategoryFromEvent(string data)
    {
        // Пример строки: "рука:рука_левая_согнута"
        var parts = data.Split(':');
        if (parts.Length != 2)
        {
            Debug.LogWarning("Неверный формат данных события. Ожидается 'group:category'");
            return;
        }

        string group = parts[0];
        string category = parts[1];

        animator.SwitchCategoryInGroup(group, category);
    }

    // Вызов из Animation Event с множественными парами: "группа:категория;группа2:категория2"
    public void OnMultiGroupSwitchEvent(string data)
    {
        if (animator == null || string.IsNullOrEmpty(data))
        {
            Debug.LogWarning("Нет данных для переключения категорий.");
            return;
        }

        Dictionary<string, string> map = new Dictionary<string, string>();

        var entries = data.Split(';');
        foreach (var entry in entries)
        {
            var parts = entry.Split(':');
            if (parts.Length != 2)
            {
                Debug.LogWarning($"Неверный формат пары: {entry}. Ожидается 'группа:категория'");
                continue;
            }

            string group = parts[0].Trim();
            string category = parts[1].Trim();
            map[group] = category;
        }

        animator.SwitchMultipleGroups(map);
    }

}
