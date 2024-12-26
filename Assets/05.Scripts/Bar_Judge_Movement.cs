using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Bar_Judge_Movement : MonoBehaviour
{
    [SerializeField] bool OffsetTestMode;
    [SerializeField] OffsetTest offsetTest;
    Dictionary<string, int> noteDirections;
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

    [SerializeField] float bpm;
    float sec_per_quarter;
    float speed;

    // 플레이어는 기본적으로 오른쪽으로 전진만 한다. 회전이 방향 전환을 맡음.
    [SerializeField] GameObject show_player;
    [SerializeField] Vector3 goingDirection = Vector3.right;
    Vector2 nowDirection; // 현재 진행 방향 벡터
    Vector3 initPosition; // 맨 처음 위치
    Quaternion initRotate; // 맨 처음 각도
    double lastdspTime; // 음악 시작 시간
    double dspTimeGap; // 이전 dspTime과의 차이


    void Start()
    {
        MainMusic.Play();
        MainMusic.Pause();
        float InputOffset = ES3.Load<float>("InputOffset", defaultValue: 0f);
        if (OffsetTestMode) InputOffset = 0f;
        float boundSize = transform.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        localFrontOffset = new Vector3(boundSize / 2 - InputOffset, 0, 0);
        localBackOffset = new Vector3(-boundSize / 2 - InputOffset, 0, 0);
        rayDirection = Vector3.back;

        sec_per_quarter = 60f / bpm;
        speed = 1f / sec_per_quarter;

        initPosition = transform.position;
        initRotate = transform.rotation;

        nowDirection = transform.rotation.eulerAngles;
        nowDirection = new Vector2(Mathf.Cos(initRotate.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(initRotate.eulerAngles.z * Mathf.Deg2Rad)).normalized;
    }

    void Update()
    {
        RaycastFrontAndBack();
        CheckInbox();
        if (isInBox) Judge();
        CheckMusic();
        if (game_ongoing)
            transform.Translate(goingDirection * speed * (float)dspTimeGap);
        follow_show_player();
    }

    /// <summary>
    /// 오브젝트의 로컬 좌표 기준 앞뒤에서 raycast
    /// </summary>
    private void RaycastFrontAndBack()
    {
        Vector3 frontPos = transform.TransformPoint(localFrontOffset);
        Vector3 backPos = transform.TransformPoint(localBackOffset);
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
        // Debug.Log("missionBlockIndex : " + missionBlockIndex);

        // 이번에 쳐야할 블럭에 진입
        if (hit1Index == missionBlockIndex || hit2Index == missionBlockIndex)
        {
            if (!isInBox)
            {
                // Debug.Log("InBox");
                if (hit1Index == missionBlockIndex) missionBlockCollider = hit1.collider;
                else missionBlockCollider = hit2.collider;
                missionBlockScript = missionBlockCollider.GetComponentInParent<NoteBlock>();
                missionKeyType = missionBlockScript.requiredKeys;
                isInBox = true;
                keyInput = 0;
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
                lastdspTime = AudioSettings.dspTime;
            }
            else
            {
                dspTimeGap = AudioSettings.dspTime - lastdspTime;
                lastdspTime = AudioSettings.dspTime;
            }
        }
    }

    private void follow_show_player()
    {
        show_player.transform.position = Vector3.Lerp(show_player.transform.position, transform.position, Time.deltaTime * 50);
        show_player.transform.rotation = transform.rotation;
    }

    /// <summary>
    /// 잘못된 키를 눌렀는지, 필요한 키를 다 눌렀는지 판정
    /// </summary>
    private void Judge()
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

    public void GameOver()
    {
        WakeUpAllBlocks();
        game_ongoing = false;
        keyInput = 0;
        missionBlockIndex = 0;
        isInBox = false;
        transform.position = initPosition;
        transform.rotation = initRotate;
        MainMusic.Stop();
        Debug.Log("Game Over");
    }

    private void HitSuccess()
    {
        // Debug.Log("HitSuccess : missionBlockIndex = " + missionBlockIndex);
        if (missionBlockScript != null) missionBlockScript.DisableCollider();
        missionBlockIndex++;
        CheckMusic();
        TurnWithNewPos();
        keyInput = 0;
        isInBox = false;
        if (OffsetTestMode && game_ongoing) OffsetTest(); // 오프셋 계산할때만 작동, 게임 맨 처음 시작할 땐 작동 안 함.
        game_ongoing = true;
        Debug.Log("Success");
    }

    private void OffsetTest()
    {
        {
            if (missionBlockIndex == 2)
            {
                WakeUpAllBlocks();
                missionBlockIndex = 0;
            }

            offsetTest.GetOneOffset(distanceToCenter);
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

    private void TurnWithNewPos()
    {
        // 다음 방향을 계산만 해놓고, 위치 이동 후 방향 반영
        Vector2 nextDirection = Vector3.zero;
        if ((missionKeyType & KeyType.Up) != 0) nextDirection += Vector2.up;
        if ((missionKeyType & KeyType.Down) != 0) nextDirection += Vector2.down;
        if ((missionKeyType & KeyType.Left) != 0) nextDirection += Vector2.left;
        if ((missionKeyType & KeyType.Right) != 0) nextDirection += Vector2.right;
        nextDirection = nextDirection.normalized;

        float angle = Mathf.Atan2(nextDirection.y, nextDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 중앙과의 거리를 계산한 뒤 어긋난 거리만큼 플레이어 위치 조정
        // 이전 방향 벡터로 이전 위치에서 중심까지의 변위를 나눈 것은,
        // 회전한 방향 벡터로 새로운 위치에서 중심까지의 변위를 나눈 것과 같다.
        Vector3 centerPos = missionBlockCollider.bounds.center;
        Vector2 displacement = centerPos - transform.position;
        Vector2 nextPos;

        if (displacement.normalized == nowDirection)
        {
            distanceToCenter = -displacement.magnitude; // 오프셋 테스트용
            nextPos = (Vector2)centerPos + displacement.magnitude * -nextDirection;
        }
        else
        {
            distanceToCenter = displacement.magnitude; // 오프셋 테스트용
            nextPos = (Vector2)centerPos + displacement.magnitude * nextDirection;
        }

        transform.position = new Vector3(nextPos.x, nextPos.y, rayLength / 10);
        nowDirection = nextDirection;
    }


}
