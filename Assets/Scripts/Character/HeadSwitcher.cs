using UnityEngine;

public class HeadSwitcher : MonoBehaviour
{
    public ModularBoneAnimator animator;

    // Этот метод можно вызывать из анимационного ивента с передачей строки
    public void OnHeadSwitchEvent(string headCategoryName)
    {
        if (animator == null)
        {
            Debug.LogWarning("ModularBoneAnimator не назначен.");
            return;
        }

        animator.SwitchHeadCategory(headCategoryName);
    }
}
