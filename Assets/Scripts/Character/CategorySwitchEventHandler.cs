using System.Collections.Generic;
using UnityEngine;

public class CategorySwitchEventHandler : MonoBehaviour
{
    public ModularBoneAnimator animator;

    // ����� ���������� �� ������������� �������
    public void OnSwitchCategoryFromEvent(string data)
    {
        // ������ ������: "����:����_�����_�������"
        var parts = data.Split(':');
        if (parts.Length != 2)
        {
            Debug.LogWarning("�������� ������ ������ �������. ��������� 'group:category'");
            return;
        }

        string group = parts[0];
        string category = parts[1];

        animator.SwitchCategoryInGroup(group, category);
    }

    // ����� �� Animation Event � �������������� ������: "������:���������;������2:���������2"
    public void OnMultiGroupSwitchEvent(string data)
    {
        if (animator == null || string.IsNullOrEmpty(data))
        {
            Debug.LogWarning("��� ������ ��� ������������ ���������.");
            return;
        }

        Dictionary<string, string> map = new Dictionary<string, string>();

        var entries = data.Split(';');
        foreach (var entry in entries)
        {
            var parts = entry.Split(':');
            if (parts.Length != 2)
            {
                Debug.LogWarning($"�������� ������ ����: {entry}. ��������� '������:���������'");
                continue;
            }

            string group = parts[0].Trim();
            string category = parts[1].Trim();
            map[group] = category;
        }

        animator.SwitchMultipleGroups(map);
    }

}
