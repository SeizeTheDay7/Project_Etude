using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar_Hit : MonoBehaviour
{
    [SerializeField] float rayLength;
    // private LayerMask targetLayer;

    // Update is called once per frame
    void Update()
    {
        // CheckHit();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckHit();
        }
    }

    void CheckHit()
    {
        Vector3 rayOrigin = transform.position;
        Debug.Log("Ray Origin : " + rayOrigin);
        Vector3 rayDirection = Vector3.back;
        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            Debug.Log("Hit : " + hit.collider.name);
            Judge(hit);
        }
        else
        {
            Debug.Log("No Hit");
        }
    }

    /// <summary>
    /// 노트 블럭 중심과의 거리를 계산하여 판정
    /// </summary>
    void Judge(RaycastHit hit)
    {
        float distance = Vector2.Distance(hit.collider.bounds.center, hit.point);
        Debug.Log("Distance : " + distance);
        switch (distance)
        {
            case float d when d < 0.02f:
                Debug.Log("Perfect");
                break;
            case float d when d < 0.06f:
                Debug.Log("Nice");
                break;
            case float d when d < 0.1f:
                Debug.Log("Good");
                break;
            default:
                Debug.Log("Bad");
                break;
        }
    }
}
