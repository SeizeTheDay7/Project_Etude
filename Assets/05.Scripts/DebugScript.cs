using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float boundSize = transform.GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x;
        Debug.Log("새롭게 만든 오브젝트의 boundsize : " + boundSize);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
