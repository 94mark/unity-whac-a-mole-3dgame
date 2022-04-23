# unity-whac-a-mole-3dgame
두더지 잡기 게임 제작

https://user-images.githubusercontent.com/90877724/160127927-a1aa3a07-db2c-42d7-a422-496f01520453.mp4

## 1. 프로젝트 개요
### 1-1. 개발 인원/기간 및 포지션
- 1인, 총 3일 소요
### 1-2. 개발 환경
- Unity 2020.3.16f
- 언어 : C#
- OS : Window 10

## 2. 핵심 구현 내용
### 2-1. 타겟(두더지) 행동 제어
- 코루틴을 이용한 FSM 구현(상태별 행동)
- 열거형 변수를 ToString() 메소드를 이용해 문자열로 반환하면 열거형 요소 이름 반환
- IEnumerator UnderGround() / OnGround() / MoveUp() / MoveDown()
```c#
  public void ChangeState(MoleState newState)
    {               
        StopCoroutine(MoleState.ToString());
        MoleState = newState;
        StartCoroutine(MoleState.ToString());
    }
```
- spawnTime 시간마다 9개의 Hole 중 랜덤으로 두더지 상태를 MoveUp으로 변경하는 코루틴 실행

### 2-2. 타겟 공격
- 망치 오브젝트 생성(공중에 위치하면 Game View에 그림자가 생기므로 MeshRenderer.CastShadows를 Off로 설정
- 마우스 클릭 시 Raycast에 저장된 이벤트 메소드 실행(OnHit)
- Mole 태그를 가진 타겟 정보가 호출되면 MoleFSM 상태가 Underground로 전환, 망치는 최초의 위치로 전환되는 MoveUp 코루틴 실행
```c#
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
            main.startColor = mole.GetComponent<MeshRenderer>().material.color;

            StartCoroutine("MoveUp");
        }
    }
```
- OnHit 함수 호출 시 카메라가 흔들리는 효과를 위해 Random.insideUnitSphere 메서드 사용
```c#
 private IEnumerator ShakeByPosition()
    {
        Vector3 startPosition = transform.position;

        while (shakeTime > 0.0f)
        {
            transform.position = startPosition + Random.insideUnitSphere * shakeIntensity;
            shakeTime -= Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
    }
```
- 피격 효과 파티클 생성, ParticleSystem은 main으로 캐싱해야 
### 2-3. 점수 출력 및 스테이지 설정
### 2-4. 등장 확률에 따른 타겟 추가
### 2-5. 콤보 시스템 구현
### 2-6. 씬 추가 및 정보 저장

## 3. 문제 해결 내용
### 3-1. MoleSpawner 호출 시 null 에러 발생
- Mole.Awake()에서 Movement3D 컴포넌트 정보를 얻어오기 전에 MoleSpawner의 SpanwMole()이 호출되서 최초 null 에러 발생
- MoleSpawner에서 Awake()가 아닌 Start() 함수를 사용해 우선순위 지정


```
```
