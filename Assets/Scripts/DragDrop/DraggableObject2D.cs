using UnityEngine;

public class DraggableObject2D : MonoBehaviour
{
    public string matchingTag = "Triangle";
    public Vector3 insertOffset = Vector3.zero;
    public AudioClip failDropSound;
    public Color highlightColor = Color.green;

    private Vector3 initialPosition;
    private bool isDragging = false;
    private bool isInserted = false;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private DropSlot2D currentHoveredSlot = null;
    private DropSlotManager2D slotManager;

    private Camera cam;

    private void Start()
    {
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        cam = Camera.main;
        slotManager = FindObjectOfType<DropSlotManager2D>();
    }

    void OnMouseDown()
    {
        if (isInserted) return;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!isDragging || isInserted) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;

        // Проверка на слоты
        DropSlot2D[] allSlots = FindObjectsOfType<DropSlot2D>();
        DropSlot2D closestValid = null;
        float minDist = float.MaxValue;

        foreach (var slot in allSlots)
        {
            if (slot.requiredTag != matchingTag || slot.IsOccupied() || !slot.IsAvailableForInsert()) continue;

            float dist = Vector2.Distance(transform.position, slot.transform.position);
            if (dist < 1.0f && dist < minDist)
            {
                minDist = dist;
                closestValid = slot;
            }
        }

        if (closestValid != null && closestValid != currentHoveredSlot)
        {
            currentHoveredSlot = closestValid;
            Highlight(true);
        }
        else if (closestValid == null)
        {
            Highlight(false);
            currentHoveredSlot = null;
        }
    }

    void OnMouseUp()
    {
        if (!isDragging || isInserted) return;
        isDragging = false;
        Highlight(false);

        if (currentHoveredSlot != null && currentHoveredSlot.TryInsert(this))
        {
            isInserted = true;
            currentHoveredSlot = null;

            slotManager?.CheckAllSlots();
            return;
        }

        // ❌ Отмена
        transform.position = initialPosition;
        if (failDropSound != null) audioSource.PlayOneShot(failDropSound);
    }

    public void Highlight(bool on)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = on ? highlightColor : originalColor;
    }
}