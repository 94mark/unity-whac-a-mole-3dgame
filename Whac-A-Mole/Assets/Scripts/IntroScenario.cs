using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScenario : MonoBehaviour
{
    [SerializeField]
    private Movement3D[] movementMoles;
    [SerializeField]
    private GameObject[] textMoles;
    [SerializeField]
    private GameObject textPressAnyKey;
    [SerializeField]
    private float maxY = 2.5f;
    private int currentIndex = 0;

    private void Awake()
    {
        StartCoroutine("Scenario");
    }

    private IEnumerator Scenario()
    {
        while (currentIndex < movementMoles.Length)
        {
            yield return StartCoroutine("MoveMole");
        }

        textPressAnyKey.SetActive(true);

        while(true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("Main");
            }

            yield return null;
        }
    }

    private IEnumerator MoveMole()
    {
        movementMoles[currentIndex].MoveTo(Vector3.up);

        while(true)
        {
            if(movementMoles[currentIndex].transform.position.y >= maxY)
            {
                movementMoles[currentIndex].MoveTo(Vector3.zero);
                break;
            }

            yield return null;
        }

        textMoles[currentIndex].SetActive(true);
        currentIndex++;
    }
}

