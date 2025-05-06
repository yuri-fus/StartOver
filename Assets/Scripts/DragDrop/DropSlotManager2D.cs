using UnityEngine;

public class DropSlotManager2D : MonoBehaviour
{
    private DropSlot2D[] slots;
    private SlotsHandler2D handler;

    private void Start()
    {
        slots = FindObjectsOfType<DropSlot2D>();
        handler = FindObjectOfType<SlotsHandler2D>();
    }

    public void CheckAllSlots()
    {
        foreach (var slot in slots)
        {
            if (!slot.HasCorrectChild())
                return;
        }

        handler?.AllSettled();
    }
}