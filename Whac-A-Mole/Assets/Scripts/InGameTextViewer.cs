using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameTextViewer : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private TextMeshProUGUI textScore;
    [SerializeField]
    private TextMeshProUGUI textPlayTime;
    [SerializeField]
    private Slider sliderPlayTime;

    private void Update()
    {
        textScore.text = "Score " + gameController.Score;

        textPlayTime.text = gameController.CurrentTime.ToString("F1");
        sliderPlayTime.value = gameController.CurrentTime / gameController.MaxTime;
    }
}
