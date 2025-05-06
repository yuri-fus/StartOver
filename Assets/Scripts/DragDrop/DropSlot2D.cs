using UnityEngine;

public class DropSlot2D : MonoBehaviour
{
    public string requiredTag = "Triangle";
    public Vector3 insertedOffset = Vector3.zero;
    public Vector3 insertedRotation = Vector3.zero;
    public AudioClip insertSound;
    public DropSlot2D[] requiredSlots; // —лоты, которые должны быть заполнены перед этим

    private bool isOccupied = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public bool TryInsert(DraggableObject2D obj)
    {
        if (isOccupied || obj.matchingTag != requiredTag || !IsAvailableForInsert())
            return false;

        obj.transform.SetParent(transform);
        obj.transform.localPosition = insertedOffset;
        obj.transform.localRotation = Quaternion.Euler(insertedRotation);

        if (insertSound != null) audioSource.PlayOneShot(insertSound);
        obj.Highlight(false);

        isOccupied = true;
        return true;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public bool HasCorrectChild()
    {
        foreach (Transform child in transform)
        {
            DraggableObject2D drag = child.GetComponent<DraggableObject2D>();
            if (drag != null && drag.matchingTag == requiredTag)
                return true;
        }
        return false;
    }

    public bool IsAvailableForInsert()
    {
        if (requiredSlots == null || requiredSlots.Length == 0)
            return true;

        foreach (var slot in requiredSlots)
        {
            if (slot == null || !slot.IsOccupied())
                return false;
        }
        return true;
    }
}