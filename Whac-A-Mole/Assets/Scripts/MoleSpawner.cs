using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleSpawner : MonoBehaviour
{
    [SerializeField]
    private MoleFSM[] moles;
    [SerializeField]
    private float spawnTime;

    //카운트다운 완료 이후 두더지 생성
    public void Setup()
    {
        StartCoroutine("SpawnMole");
    }

    private IEnumerator SpawnMole()
    {
        while( true )
        {
            int index = Random.Range(0, moles.Length);
            moles[index].ChangeState(MoleState.MoveUp);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
