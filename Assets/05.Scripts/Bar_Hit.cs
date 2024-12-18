using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar_Hit : MonoBehaviour
{
    [SerializeField] Collider2D bar_collider;
    [SerializeField] float rayLength;
    // private LayerMask targetLayer;

    // Start is called before the first frame update
    void Start()
    {

    }

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
        Vector3 rayDirection = Vector3.back;
        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            Debug.Log("Hit : " + hit.collider.name);
        }
        else
        {
            Debug.Log("No Hit");
        }
    }
}
