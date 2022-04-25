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
- 피격 효과 파티클 생성, ParticleSystem은 main으로 캐싱
### 2-3. 점수 출력 및 스테이지 설정

![20220423_171715](https://user-images.githubusercontent.com/90877724/164886491-dfe95923-76f7-431f-9c23-10f3eca7cede.png)


- OnHit() 함수 호출 시 Score UI text 출력(점수 증가)
- 스테이지 시작 전 CountDown 이벤트 함수 구현, MoleSpawn 호출 전 완료하기 위해 GameController의 SetUp() 메소드 사용
```c#
private IEnumerator OnCountDown(UnityAction action, int start, int end)
    {
        endOfCountDown.AddListener(action);

        while( start > end -1 )
        {
            audioSource.Play();
            textCountDown.text = start.ToString();
            yield return StartCoroutine("OnFontAnimation");
            start--;
        }

        endOfCountDown.Invoke();
        endOfCountDown.RemoveListener(action);
        gameObject.SetActive(false);
    }
```
- 폰트 크기가 시간에 따라 변화되는 애니메이션 효과 구현, Lerp() 함수로 자연스러운 전환
```c#
 private IEnumerator OnFontAnimation()
    {
        float percent = 0;

        while ( percent < 1 )
        {
            percent += Time.deltaTime;

            textCountDown.fontSize = Mathf.Lerp(maxFontSize, minFontSize, percent);

            yield return null;
        }
    }
```
- 슬라이더 bar를 사용하여 남은 시간 count
```c#
sliderPlayTime.value = gameController.CurrentTime / gameController.MaxTime;
```
### 2-4. 등장 확률에 따른 타겟 추가
- 색상별로 85%, 10% ,5% 확률로 타겟 생성, 타겟 종류가 늘어나도 사용할 수 있는 cumulative 변수를 사용한 메서드 작성
```c#
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
```
- 타격된 두더지 타겟 별로 다른 점수 텍스트 출력, Score Text UI 애니메이션 설정
- 타격 시 점수 텍스트 출력(Vector2.up 방향, 알파 값을 1 -> 0으로 감소시켜 투명도 변환)
```c#
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
```
### 2-5. 콤보 시스템 구현
- 기본 x 1 에 10콤보 당 0.5씩 더해지는 콤보 계산
![20220425_110748](https://user-images.githubusercontent.com/90877724/165009176-c7f4f74b-859c-4dfc-9528-1b260f318d0b.png)
```c#
float scoreMultiple = 1 + gameController.Combo / 10 * 0.5f;
            int getScore = (int)(scoreMultiple * 50);
```

- 10콤보 이상 시 두더지 타겟 2마리 생성
```c#
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
```
```c#
 if( combo <= 70 )
     {
     moleSpawner.MaxSpawnMole = 1 + (combo + 10) / 20;
     }
```

### 2-6. 씬 추가 및 정보 저장

## 3. 문제 해결 내용
### 3-1. MoleSpawner 호출 시 null 에러 발생
- Mole.Awake()에서 Movement3D 컴포넌트 정보를 얻어오기 전에 MoleSpawner의 SpanwMole()이 호출되서 최초 null 에러 발생
- MoleSpawner에서 Awake()가 아닌 Start() 함수를 사용해 우선순위 지정
### 3-2. 두더지 타겟의  생성을 위한 난수 생성 메서드 구현
- 0 ~ maxCount까지 숫자 중 n개의 겹치지 않는 난수 생성
- maxCount 개수만큼의 방을 가지는 배열 생성(int[] defaults)
- 배열에 0부터 maxCount - 1까지 순서대로 숫자 저장 후 n 크기만큼 반복문 실행

![20220425_112851](https://user-images.githubusercontent.com/90877724/165010850-0fe7f072-1631-42c7-9efc-17588d6a2e76.png)

```c#
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
```


