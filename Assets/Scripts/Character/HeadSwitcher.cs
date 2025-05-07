using UnityEngine;

public class HeadSwitcher : MonoBehaviour
{
    public ModularBoneAnimator animator;

    // ���� ����� ����� �������� �� ������������� ������ � ��������� ������
    public void OnHeadSwitchEvent(string headCategoryName)
    {
        if (animator == null)
        {
            Debug.LogWarning("ModularBoneAnimator �� ��������.");
            return;
        }

        animator.SwitchHeadCategory(headCategoryName);
    }
}
