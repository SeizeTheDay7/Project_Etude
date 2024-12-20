using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar_Judge_Movement : MonoBehaviour
{
    Dictionary<string, int> noteDirections;
    bool isInBox = false;
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

    [SerializeField] float bpm;
    float sec_per_quarter;
    float speed;
    [SerializeField] Vector3 goingDirection = Vector3.right; // Initial direction

    void Start()
    {
        float boundSize = transform.GetComponent<SpriteRenderer>().bounds.size.x;
        localFrontOffset = new Vector3(boundSize / 2, 0, 0);
        localBackOffset = new Vector3(-boundSize / 2, 0, 0);
        rayDirection = Vector3.back;
        sec_per_quarter = 60f / bpm;
        speed = 1f / sec_per_quarter;
    }

    void Update()
    {

        RaycastFrontAndBack();
        CheckInbox();
        if (isInBox) Judge();
        // transform.Translate(goingDirection * speed * Time.deltaTime, Space.World);
        transform.Translate(goingDirection * speed * Time.deltaTime);
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
                Debug.Log("InBox");
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
                Debug.Log("Game Over");
            }
        }
    }

    private void Judge()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) keyInput |= KeyType.Up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) keyInput |= KeyType.Down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) keyInput |= KeyType.Left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) keyInput |= KeyType.Right;

        // 잘못된 키를 눌렀다면 게임 오버
        if ((keyInput | missionKeyType) != missionKeyType)
        {
            Debug.Log("Game Over");
        }
        // 필요한 키 다 눌렀으니 판정 성공
        else if (keyInput == missionKeyType)
        {
            Debug.Log("Success");
            missionBlockScript.RemoveBlock();
            missionBlockIndex++;
            TurnMovement();
            isInBox = false;
        }
    }

    private void TurnMovement()
    {
        Vector3 nextDirection = Vector3.zero;

        if ((missionKeyType & KeyType.Up) != 0) nextDirection += Vector3.up;
        if ((missionKeyType & KeyType.Down) != 0) nextDirection += Vector3.down;
        if ((missionKeyType & KeyType.Left) != 0) nextDirection += Vector3.left;
        if ((missionKeyType & KeyType.Right) != 0) nextDirection += Vector3.right;

        float angle = Mathf.Atan2(nextDirection.y, nextDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
