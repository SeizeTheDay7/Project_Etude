using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bar_movement : MonoBehaviour
{
    int going = 2;
    [SerializeField] float bpm;
    float sec_per_quarter;
    float speed;
    private Vector3 goingDirection = Vector3.right; // Initial direction

    void Start()
    {
        sec_per_quarter = 60f / bpm;
        speed = 1f / sec_per_quarter;
    }

    void Update()
    {
        // RotateAndMove();
        DebugMove();
    }

    void DebugMove()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    void RotateAndMove()
    {
        Vector3 inputDirection = Vector3.zero;

        // Check each key independently and add their directions
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputDirection += Vector3.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            inputDirection += Vector3.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            inputDirection += Vector3.down;
        }

        // If any key is pressed, update the goingDirection and rotation
        if (inputDirection != Vector3.zero)
        {
            goingDirection = inputDirection.normalized; // Normalize to prevent faster diagonal movement
            float angle = Mathf.Atan2(goingDirection.y, goingDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Move the object in the current direction
        transform.Translate(goingDirection * speed * Time.deltaTime, Space.World);
    }
}
