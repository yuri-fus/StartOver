using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFrameAnimator : MonoBehaviour
{
    public float frameDuration = 0.1f; // ������������ ������ �����
    private List<GameObject> spriteFrames = new List<GameObject>();
    private int currentFrame = 0;
    private float timer;

    void Start()
    {
        // ������� ��� �������� ������� � ���������
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                spriteFrames.Add(child.gameObject);
                child.gameObject.SetActive(false); // ������� ��� ���������
            }
        }

        if (spriteFrames.Count > 0)
        {
            spriteFrames[0].SetActive(true); // �������� ������ ����
        }
    }

    void Update()
    {
        if (spriteFrames.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameDuration)
        {
            timer = 0f;

            // ��������� ������� ����
            spriteFrames[currentFrame].SetActive(false);

            // ��������� � ���������� �����
            currentFrame = (currentFrame + 1) % spriteFrames.Count;

            // �������� ����� ����
            spriteFrames[currentFrame].SetActive(true);
        }
    }
}
