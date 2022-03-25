using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private CountDown countDown;
    [SerializeField]
    private MoleSpawner moleSpawner;
    private int score;
    private int combo;
    private float currentTime;

    public int Score
    {
        set => score = Mathf.Max(0, value);
        get => score;
    }

    public int Combo
    {
        //set => combo = Mathf.Max(0, value);
        set
        {
            combo = Mathf.Max(0, value);
            if( combo <= 70 )
            {
                moleSpawner.MaxSpawnMole = 1 + (combo + 10) / 20;
            }
        }
        get => combo;
    }

    //자동 구현 프로퍼티를 인스펙터 뷰에서 보이게 할 때 사용
    [field:SerializeField]
    public float MaxTime { private set; get; }
    //public float CurrentTime { private set; get; }

    public float CurrentTime
    {
        set => currentTime = Mathf.Clamp(value, 0, MaxTime);
        get => currentTime;
    }

    private void Start()
    {
        countDown.StartCountDown(GameStart);
    }

    private void GameStart()
    {
        moleSpawner.Setup();

        StartCoroutine("OnTimeCount");
    }

    private IEnumerator OnTimeCount()
    {
        CurrentTime = MaxTime;

        while ( CurrentTime > 0 )
        {
            CurrentTime -= Time.deltaTime;

            yield return null;
        }
    }
}

