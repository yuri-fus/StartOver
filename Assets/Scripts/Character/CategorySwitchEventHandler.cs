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
}
