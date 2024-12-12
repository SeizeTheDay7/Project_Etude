using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bar_movement : MonoBehaviour
{
    int going = 2;
    [SerializeField] float bpm;
    float sec_per_quarter;
    float speed;

    void Start()
    {
        sec_per_quarter = 60f / bpm;
        speed = 1f / sec_per_quarter;
    }

    void Update()
    {
        RotateAndMove();
    }

    void RotateAndMove()
    {
        float moveSpeed = speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            going = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            going = 2;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            going = 3;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
            going = 4;
        }

        switch (going)
        {
            case 1:
                transform.Translate(Vector3.right * moveSpeed);
                break;
            case 2:
                transform.Translate(Vector3.right * moveSpeed);
                break;
            case 3:
                transform.Translate(Vector3.right * moveSpeed);
                break;
            case 4:
                transform.Translate(Vector3.right * moveSpeed);
                break;
        }
    }
}
