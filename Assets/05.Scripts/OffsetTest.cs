using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OffsetTest : MonoBehaviour
{
    private float offset_avg = 0.0f;
    private int offset_count = 0;
    [SerializeField] TextMeshProUGUI offsetText;
    [SerializeField] GameObject player;
    [SerializeField] StartDirector StartDriector;

    public void GetOneOffset(float offset)
    {
        offsetText.text = "Offset: " + offset * 60 / 85 + "s";
        offset_avg += offset;
        offset_count++;
        if (offset_count == 10)
        {
            offset_avg /= 10;
            offsetText.text = "";
            Debug.Log("Offset Average: " + offset_avg);
            ES3.Save<float>("InputOffset", offset_avg); // 오프셋 저장해두기
            player.GetComponent<Bar_Judge_Movement>().enabled = false;
            StartCoroutine(StartDriector.StartGame());
        }
    }
}
