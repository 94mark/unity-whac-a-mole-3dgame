using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoleHitTextViewer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 30.0f;
    private Vector2 defaultPosition;
    private TextMeshProUGUI textHit;
    private RectTransform rectHit;

    private void Awake()
    {
        textHit = GetComponent<TextMeshProUGUI>();
        rectHit = GetComponent<RectTransform>();
        defaultPosition = rectHit.anchoredPosition;

        gameObject.SetActive(false);
    }

    public void OnHit(string hitData, Color color)
    {
        gameObject.SetActive(true);
        textHit.text = hitData;

        StopCoroutine("OnAnimation");
        StartCoroutine("OnAnimation", color);
    }

    private IEnumerator OnAnimation(Color color)
    {
        rectHit.anchoredPosition = defaultPosition;

        while ( color.a > 0)
        {
            rectHit.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
            color.a -= Time.deltaTime;
            textHit.color = color;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
