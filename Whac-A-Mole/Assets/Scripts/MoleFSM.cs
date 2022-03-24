using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoleState { UnderGround = 0, OnGround, MoveUp, MoveDown }

public enum MoleType { Normal = 0, Red, Blue }

public class MoleFSM : MonoBehaviour
{
    [SerializeField]
    private float waitTimeOnGround;
    [SerializeField]
    private float limitMinY;
    [SerializeField]
    private float limitMaxY;

    private Movement3D movement3D;
    private MeshRenderer meshRenderer;

    private MoleType moleType;
    private Color defaultColor;

    public MoleState MoleState { private set; get; }

    public MoleType MoleType
    {
        set
        {
            moleType = value;

            switch ( moleType )
            {
                case MoleType.Normal:
                    meshRenderer.material.color = defaultColor;
                    break;
                case MoleType.Red:
                    meshRenderer.material.color = Color.red;
                    break;
                case MoleType.Blue:
                    meshRenderer.material.color = Color.blue;
                    break;
            }
        }
        get => moleType;
    }

    [field:SerializeField]
    public int MoleIndex { private set; get; }

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        meshRenderer = GetComponent<MeshRenderer>();

        defaultColor = meshRenderer.material.color;

        ChangeState(MoleState.UnderGround);
    }

    public void ChangeState(MoleState newState)
    {
        //ToString() 메소드를 이용해 열거형 요소 이름 반환
                
        StopCoroutine(MoleState.ToString());
        MoleState = newState;
        StartCoroutine(MoleState.ToString());
    }

    private IEnumerator UnderGround()
    {
        movement3D.MoveTo(Vector3.zero);
        transform.position = new Vector3(transform.position.x, limitMinY, transform.position.z);
        yield return null;
    }

    private IEnumerator OnGround()
    {
        movement3D.MoveTo(Vector3.zero);
        transform.position = new Vector3(transform.position.x, limitMaxY, transform.position.z);
        yield return new WaitForSeconds(waitTimeOnGround);
        ChangeState(MoleState.MoveDown);
    }

    private IEnumerator MoveUp()
    {
        movement3D.MoveTo(Vector3.up);

        while( true )
        {
            if( transform.position.y >= limitMaxY )
            {
                ChangeState(MoleState.OnGround);
            }

            yield return null;
        }
    }

    private IEnumerator MoveDown()
    {
        movement3D.MoveTo(Vector3.down);

        while( true )
        {
            if( transform.position.y <= limitMinY )
            {
                ChangeState(MoleState.UnderGround);
            }

            yield return null;
        }
    }
}
