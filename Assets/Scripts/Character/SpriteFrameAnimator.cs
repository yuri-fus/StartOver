using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFrameAnimator : MonoBehaviour
{
    public float frameDuration = 0.1f; // Длительность одного кадра
    private List<GameObject> spriteFrames = new List<GameObject>();
    private int currentFrame = 0;
    private float timer;

    void Start()
    {
        // Находим все дочерние объекты с рендерами
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                spriteFrames.Add(child.gameObject);
                child.gameObject.SetActive(false); // Сначала все отключаем
            }
        }

        if (spriteFrames.Count > 0)
        {
            spriteFrames[0].SetActive(true); // Включаем первый кадр
        }
    }

    void Update()
    {
        if (spriteFrames.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameDuration)
        {
            timer = 0f;

            // Выключаем текущий кадр
            spriteFrames[currentFrame].SetActive(false);

            // Переходим к следующему кадру
            currentFrame = (currentFrame + 1) % spriteFrames.Count;

            // Включаем новый кадр
            spriteFrames[currentFrame].SetActive(true);
        }
    }
}
