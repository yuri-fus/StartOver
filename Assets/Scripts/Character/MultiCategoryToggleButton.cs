using System.Collections.Generic;
using UnityEngine;

public class MultiCategorySwitchButton : MonoBehaviour
{
    public ModularBoneAnimator animator;

    [Header("���������, ������� ����� ��������")]
    public List<string> categoriesToEnable = new List<string>();

    [Header("���������, ������� ����� ���������")]
    public List<string> categoriesToDisable = new List<string>();

    public void SwitchCategories()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator not assigned.");
            return;
        }

        foreach (string name in categoriesToDisable)
        {
            animator.DisableCategory(name);
        }

        foreach (string name in categoriesToEnable)
        {
            animator.EnableCategory(name);
        }
    }
}
