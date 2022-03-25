using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleSpawner : MonoBehaviour
{
    [SerializeField]
    private MoleFSM[] moles;
    [SerializeField]
    private float spawnTime;

    private int[] spawnPercents = new int[3] { 85, 10, 5 };

    public int MaxSpawnMole { set; get; } = 1;

    //카운트다운 완료 이후 두더지 생성
    public void Setup()
    {
        StartCoroutine("SpawnMole");
    }

    private IEnumerator SpawnMole()
    {
        while (true)
        {
            //int index = Random.Range(0, moles.Length);
            //moles[index].MoleType = SpawnMoleType();
            //moles[index].ChangeState(MoleState.MoveUp);
            StartCoroutine("SpanwMultiMoles");
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private MoleType SpawnMoleType()
    {
        int percent = Random.Range(0, 100);
        float cumulative = 0;

        for(int i = 0; i < spawnPercents.Length; ++i)
        {
            cumulative += spawnPercents[i];

            if(percent < cumulative)
            {
                return (MoleType)i;
            }
        }

        return MoleType.Normal;
    }

    private IEnumerator SpanwMultiMoles()
    {
        int[] indexs = RandomNumerics(moles.Length, moles.Length);
        int currentSpawnMole = 0;
        int curretIndex = 0;

        while (curretIndex < indexs.Length)
        {
            if (moles[indexs[curretIndex]].MoleState == MoleState.UnderGround)
            {
                moles[indexs[curretIndex]].MoleType = SpawnMoleType();
                moles[indexs[curretIndex]].ChangeState(MoleState.MoveUp);
                currentSpawnMole++;

                yield return new WaitForSeconds(0.1f);
            }

            if (currentSpawnMole == MaxSpawnMole)
            {
                break;
            }

            curretIndex++;
            yield return null;
        }
    }
    
    //0 ~ maxCount까지의 숫자 중 겹치지 않는 n개의 난수생성 메서드
    private int[] RandomNumerics(int maxCount, int n)
    {
        int[] defaults = new int[maxCount];
        int[] results = new int[n];

        for( int i = 0; i < maxCount; i++ )
        {
            defaults[i] = i;
        }

        for( int i = 0; i < n; i++ )
        {
            int index = Random.Range(0, maxCount);

            results[i] = defaults[index];
            defaults[index] = defaults[maxCount - 1];

            maxCount--;
        }

        return results;
    }
}
