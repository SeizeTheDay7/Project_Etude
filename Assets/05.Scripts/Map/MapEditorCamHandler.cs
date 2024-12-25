using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraHandler : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Cinemachine.CinemachineVirtualCamera virtualCamera;
    [SerializeField] GameObject player;
    bool isFollowingPlayer = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFollowingPlayer = !isFollowingPlayer;
        }

        if (isFollowingPlayer && virtualCamera.Follow == gameObject.transform)
        {
            player.GetComponent<Bar_Judge_Movement>().enabled = true;
            virtualCamera.Follow = player.transform;
        }
        else if (!isFollowingPlayer && virtualCamera.Follow == player.transform)
        {
            player.GetComponent<Bar_Judge_Movement>().enabled = false;
            // gameObject.transform.position = player.transform.position;
            virtualCamera.Follow = gameObject.transform;
        }

        if (!isFollowingPlayer)
        {
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, y, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
