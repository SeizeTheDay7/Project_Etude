using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class Bar_Judge_Movement : MonoBehaviour
{
    [SerializeField] bool OffsetTestMode;
    [SerializeField] OffsetTest offsetTest;
    [SerializeField] TextMeshProUGUI JudgeDebugText;
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] bool mapEditMode;
    [SerializeField] GameObject[] hitSoundObjects;
    bool isInBox = false;
    bool game_ongoing = false;
    float rayLength = 1f;
    Vector3 localFrontOffset;
    Vector3 localBackOffset;
    Vector3 rayDirection;
    RaycastHit hit1, hit2;
    int missionBlockIndex;
    Collider missionBlockCollider;
    NoteBlock missionBlockScript;
    KeyType missionKeyType;
    KeyType keyInput;
    float distanceToCenter;

    [SerializeField] AudioSource MainMusic;
    [SerializeField] HitSound hitSound;
    [SerializeField] AudioSource myAudioSource;

    [SerializeField] float bpm;
    [SerializeField] float smoothIntensity;

    // 플레이어는 기본적으로 오른쪽으로 전진만 한다. 회전이 방향 전환을 맡음.
    [SerializeField] GameObject show_player;
    [SerializeField] GameObject future_player;
    [SerializeField] float future_offset; // 가상의 미래의 플레이어 위치
    [SerializeField] Vector3 goingDirection = Vector3.right;
    Vector3 initPosition; // 맨 처음 위치
    Quaternion initRotate; // 맨 처음 각도
    double lastBlockStartTime; // 마지막 블럭의 이론적 시작 시간 (dspTime)
    double delta_dspTime; // 이전 dspTime과의 차이
    Vector3 offsetCenter; // 앞뒤 레이캐스트의 중심
    Vector3 nowBlockPos; // 기준 블럭 (현재)
    Vector3 nextBlockPos; // 기준 블럭 (다음)
    float storedNoteDuration; // 현재 노트의 노트 길이
    bool game_paused = false;
    double pauseStartTime; // 멈추기 시작한 시간
    double pausedDuration = 0; // 멈춘 시간


    void Start()
    {
        MainMusic.Play();
        MainMusic.Pause();
        storedNoteDuration = 0;

        future_player.transform.position = transform.position + goingDirection * future_offset;

        float InputOffset = ES3.Load<float>("InputOffset", defaultValue: 0f);
        if (OffsetTestMode) InputOffset = 0f;
        float boundSize = transform.GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        localFrontOffset = new Vector3(boundSize / 2 - InputOffset, 0, 0);
        localBackOffset = new Vector3(-boundSize / 2 - InputOffset, 0, 0);
        rayDirection = Vector3.back;

        initPosition = transform.position;
        initRotate = transform.rotation;

        hitSound = hitSoundObjects[PlayerPrefs.GetInt("HitSound", 0)].GetComponent<HitSound>();
    }

    void Update()
    {
        if (game_paused) return;
        RaycastFrontAndBack();
        CheckInbox();
        if (isInBox) KeyPressJudge();
        CheckMusic();
        if (game_ongoing)
        {
            player_move(); // dspTime 기반 플레이어 이동
            follow_show_player(); // 보여지는 플레이어
        }
        else
        {
            show_player.transform.position = transform.position;
            show_player.transform.rotation = transform.rotation;
        }
        DebugText();
    }

    /// <summary>
    /// 오브젝트의 로컬 좌표 기준 앞뒤에서 raycast
    /// </summary>
    private void RaycastFrontAndBack()
    {
        Vector3 frontPos = transform.TransformPoint(localFrontOffset);
        Vector3 backPos = transform.TransformPoint(localBackOffset);
        offsetCenter = (frontPos + backPos) / 2;
        Ray ray1 = new Ray(frontPos, rayDirection);
        Ray ray2 = new Ray(backPos, rayDirection);
        Physics.Raycast(ray1, out hit1, rayLength);
        Physics.Raycast(ray2, out hit2, rayLength);
        Debug.DrawRay(frontPos, rayDirection * rayLength, Color.red);
        Debug.DrawRay(backPos, rayDirection * rayLength, Color.blue);
        // if (hit1.collider != null) Debug.Log("Hit1 : " + hit1.collider);
        // if (hit2.collider != null) Debug.Log("Hit2 : " + hit2.collider);
    }

    /// <summary>
    /// 지금 쳐야할 노트에 들어왔는지, 나갔는지 판단
    /// </summary>
    private void CheckInbox()
    {
        int hit1Index = hit1.collider?.GetComponentInParent<NoteBlock>()?.noteBlockIndex ?? -1;
        int hit2Index = hit2.collider?.GetComponentInParent<NoteBlock>()?.noteBlockIndex ?? -1;
        // Debug.Log("Hit1 : " + hit1Index + ", Hit2 : " + hit2Index);


        // 이번에 쳐야할 블럭에 진입
        if (hit1Index == missionBlockIndex || hit2Index == missionBlockIndex)
        {
            if (!isInBox)
            {
                if (hit1Index == missionBlockIndex) missionBlockCollider = hit1.collider;
                else missionBlockCollider = hit2.collider;
                missionBlockScript = missionBlockCollider.GetComponentInParent<NoteBlock>();
                missionKeyType = missionBlockScript.requiredKeys;
                isInBox = true;
                keyInput = 0;
                EndBlockCheck();
            }
        }
        // hit 전에 블럭을 빠져나감
        else
        {
            if (isInBox)
            {
                GameOver();
            }
        }
    }

    /// <summary>
    /// 게임 시작하면 음악 재생. 음악 재생중이라면 이전과의 간격 계산.
    /// </summary>
    private void CheckMusic()
    {
        if (game_ongoing)
        {
            if (!MainMusic.isPlaying)
            {
                MainMusic.Play();
                lastBlockStartTime = AudioSettings.dspTime;
            }
            else
            {
                delta_dspTime = AudioSettings.dspTime - lastBlockStartTime - pausedDuration;
                // DebugText.text = "delta_dspTime : " + delta_dspTime;
            }
        }
    }

    /// <summary>
    /// 판정용 플레이어 이동
    /// </summary>
    private void player_move()
    {
        // nowBlockPos와 nextBlockPos를 포함하는 1차 함수를 만들어서, delta_dspTime에 따른 플레이어 위치를 계산
        float newX = (float)(nowBlockPos.x + (nextBlockPos.x - nowBlockPos.x) * delta_dspTime / storedNoteDuration);
        float newY = (float)(nowBlockPos.y + (nextBlockPos.y - nowBlockPos.y) * delta_dspTime / storedNoteDuration);
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    /// <summary>
    /// 보여지는 플레이어 이동
    /// </summary>
    private void follow_show_player()
    {
        show_player.transform.position = Vector3.Lerp(show_player.transform.position, future_player.transform.position, Time.deltaTime * smoothIntensity);
        show_player.transform.rotation = transform.rotation;
    }

    private void DebugText()
    {
        if (debugText == null) return;
        debugText.text =
            "bpm : " + bpm + "\n" +
            "lastBlockStartTime : " + lastBlockStartTime + "\n" +
            "delta_dspTime : " + delta_dspTime + "\n" +
            "nowBlockPos : " + nowBlockPos + "\n" +
            "nextBlockPos : " + nextBlockPos + "\n" +
            "transform.position : " + transform.position + "\n" +
            "missionBlockIndex : " + missionBlockIndex + "\n" +
            "isInBox : " + isInBox + "\n" +
            "game_ongoing : " + game_ongoing + "\n" +
            "missionKeyType : " + missionKeyType + "\n" +
            "keyInput : " + keyInput + "\n" +
            "hit1 : " + hit1.collider + "\n" +
            "hit2 : " + hit2.collider + "\n" +
            "missionBlockCollider : " + missionBlockCollider + "\n" +
            "missionBlockScript : " + missionBlockScript + "\n" +
            "offsetCenter : " + offsetCenter + "\n" +
            "goingDirection : " + goingDirection + "\n" +
            "localFrontOffset : " + localFrontOffset + "\n" +
            "localBackOffset : " + localBackOffset + "\n" +
            "future_offset : " + future_offset + "\n" +
            "smoothIntensity : " + smoothIntensity + "\n";
    }

    /// <summary>
    /// 잘못된 키를 눌렀는지, 필요한 키를 다 눌렀는지 판정
    /// </summary>
    private void KeyPressJudge()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) keyInput |= KeyType.Up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) keyInput |= KeyType.Down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) keyInput |= KeyType.Left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) keyInput |= KeyType.Right;

        // 잘못된 키를 눌렀다면 게임 오버
        if ((keyInput | missionKeyType) != missionKeyType)
        {
            GameOver();
        }
        // 필요한 키 다 눌렀으니 판정 성공
        else if (keyInput == missionKeyType)
        {
            HitSuccess();
        }
    }

    /// <summary>
    /// 최종 블럭인지 확인
    /// </summary>
    private void EndBlockCheck()
    {
        if (!OffsetTestMode && !mapEditMode && missionBlockScript.nextNoteBlock == null)
        {
            Debug.Log("Game Clear");
            // GameOver();

            // MainMusic.Stop();
            GameManager.Instance.LoadNextMap();
            game_ongoing = false;
            this.enabled = false;
        }
    }

    public void GameOver()
    {
        WakeUpAllBlocks();
        OffsetTest(0);
        game_ongoing = false;
        keyInput = 0;
        missionBlockIndex = 0;
        isInBox = false;
        transform.position = initPosition;
        transform.rotation = initRotate;
        delta_dspTime = 0;
        pausedDuration = 0;
        MainMusic.Stop();
        Debug.Log("Game Over");
    }

    private void HitSuccess()
    {
        UpdateLineBasis();

        if (missionBlockScript != null) missionBlockScript.DisableCollider();

        CheckMusic();
        if (OffsetTestMode && game_ongoing) OffsetTest(1); // 오프셋 계산할때만 작동, 게임 맨 처음 시작할 땐 작동 안 함.
        TurnPlayer();

        // 변수 초기화
        keyInput = 0;
        game_ongoing = true;
        isInBox = false;
        missionBlockIndex++;

        // 이펙트 재생
        hitSoundPlay();
        // DistanceJudge();
        Debug.Log("Success");
    }

    /// <summary>
    /// 판정 성공했을 때 경로선의 기준들을 업데이트한다.
    /// </summary>
    private void UpdateLineBasis()
    {
        // 경로의 기준 블럭 위치들을 갱신한다
        nowBlockPos = missionBlockCollider.transform.position;
        if (missionBlockScript.nextNoteBlock != null)
            nextBlockPos = missionBlockScript.nextNoteBlock.transform.position;

        // 경로의 계산 기준점을 노트 길이만큼 이동시킨다
        lastBlockStartTime += storedNoteDuration;
        storedNoteDuration = missionBlockScript.noteDuration * (60 / bpm);
    }

    private void TurnPlayer()
    {
        Vector2 nextDirection = Vector3.zero;
        if ((missionKeyType & KeyType.Up) != 0) nextDirection += Vector2.up;
        if ((missionKeyType & KeyType.Down) != 0) nextDirection += Vector2.down;
        if ((missionKeyType & KeyType.Left) != 0) nextDirection += Vector2.left;
        if ((missionKeyType & KeyType.Right) != 0) nextDirection += Vector2.right;
        nextDirection = nextDirection.normalized;

        float angle = Mathf.Atan2(nextDirection.y, nextDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void hitSoundPlay()
    {
        myAudioSource.clip = hitSound.RandomHitSound();
        myAudioSource.Play();
    }

    private void OffsetTest(int code)
    {
        if (missionBlockIndex == 0)
        {
            missionBlockCollider.GetComponentInParent<NoteBlock>().nextNoteBlock.GetComponent<NoteBlock>().EnableCollider();
            missionBlockIndex = 0;
        }
        else if (missionBlockIndex == 1)
        {
            missionBlockCollider.GetComponentInParent<NoteBlock>().prevNoteBlock.GetComponent<NoteBlock>().EnableCollider();
            missionBlockIndex = -1;
        }

        Debug.Log("OffsetTest :: delta_dspTime : " + delta_dspTime);

        if (code == 1)
            offsetTest.GetOneOffset((float)delta_dspTime);
    }

    /// <summary>
    /// 거리에 따라 판정
    /// </summary>
    private void DistanceJudge()
    {
        float distance = Vector2.Distance(missionBlockCollider.bounds.center, offsetCenter);
        // Debug.Log("Distance : " + distance);
        switch (distance)
        {
            case float d when d < 0.02f:
                // Debug.Log("Perfect");
                if (JudgeDebugText != null) JudgeDebugText.text = "Perfect";
                break;
            case float d when d < 0.06f:
                // Debug.Log("Nice");
                if (JudgeDebugText != null) JudgeDebugText.text = "Nice";
                break;
            case float d when d < 0.1f:
                // Debug.Log("Good");
                if (JudgeDebugText != null) JudgeDebugText.text = "Good";
                break;
            default:
                // Debug.Log("Bad");
                if (JudgeDebugText != null) JudgeDebugText.text = "Bad";
                break;
        }
    }

    private void WakeUpAllBlocks()
    {
        if (missionBlockCollider == null) return;
        NoteBlock turnOnBlock = missionBlockCollider.GetComponentInParent<NoteBlock>();
        while (turnOnBlock != null)
        {
            turnOnBlock.EnableCollider();
            turnOnBlock = (turnOnBlock.prevNoteBlock != null)
            ? turnOnBlock.prevNoteBlock.GetComponent<NoteBlock>() : null;
        }
    }

    /// <summary>
    /// Dropdown의 인덱스를 받아서 해당하는 효과음으로 바꾸기
    /// </summary>
    public void ChangeHitSound(int value)
    {
        hitSound = hitSoundObjects[value].GetComponent<HitSound>();
        PlayerPrefs.SetInt("HitSound", value);
    }


    /// <summary>
    /// esc를 누르면 호출되며 게임 일시정지
    /// </summary>
    public void PauseGame()
    {
        game_paused = true;
        MainMusic.Pause();
        pauseStartTime = AudioSettings.dspTime;
    }

    /// <summary>
    /// esc를 다시 누르거나 resume 버튼 누르면 호출되며 게임 재개
    /// </summary>
    public void ResumeGame()
    {
        game_paused = false;
        if (game_ongoing)
        {
            MainMusic.UnPause();
            lastBlockStartTime += AudioSettings.dspTime - pauseStartTime;
        }
    }

}
