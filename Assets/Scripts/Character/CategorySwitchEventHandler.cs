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
}
