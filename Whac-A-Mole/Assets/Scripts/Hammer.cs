using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField]
    private float maxY;
    [SerializeField]
    private float minY;
    [SerializeField]
    private GameObject moleHitEffectPrefab;
    [SerializeField]
    private AudioClip[] audioClips;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private ObjectDetector objectDetector;
    private Movement3D movement3D;
    private AudioSource audioSource;

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        objectDetector.raycastEvent.AddListener(OnHit);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnHit(Transform target)
    {
        if( target.CompareTag("Mole"))
        {
            MoleFSM mole = target.GetComponent<MoleFSM>();

            if (mole.MoleState == MoleState.UnderGround) return;

            transform.position = new Vector3(target.position.x, minY, target.position.z);

            mole.ChangeState(MoleState.UnderGround);

            ShakeCamera.Instance.OnShakeCamera(0.1f, 0.1f);

            GameObject clone = Instantiate(moleHitEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = clone.GetComponent<ParticleSystem>().main;
            //파티클 시스템의 main은 변수로 가져와야 캐싱이 가능
            main.startColor = mole.GetComponent<MeshRenderer>().material.color;

            //gameController.Score += 50;
            MoleHitProcess(mole);

            StartCoroutine("MoveUp");
        }
    }
    
    private IEnumerator MoveUp()
    {
        movement3D.MoveTo(Vector3.up);

        while (true)
        { 
            if( transform.position.y >= maxY )
            {
                movement3D.MoveTo(Vector3.zero);

                break;
            }

            yield return null;
        }
    }

    private void MoleHitProcess(MoleFSM mole)
    {
        if(mole.MoleType == MoleType.Normal)
        {
            gameController.Score += 50;
        }
        else if(mole.MoleType == MoleType.Red)
        {
            gameController.Score -= 300;
        }
        else if(mole.MoleType == MoleType.Blue)
        {
            gameController.CurrentTime += 3;
        }
        PlaySound((int)mole.MoleType);
    }

    private void PlaySound(int index)
    {
        audioSource.Stop();
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
}
