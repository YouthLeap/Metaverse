using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FortuneWheelManager : MonoBehaviour
{
    public float reducer;
    public float multiplier = 1;
    bool round1 = false;
    public bool isStoped = false;
 
 
    void Start()
    {
        reducer = Random.Range(0.01f, 0.5f);
    }
 
    // Update is called once per frameQ
    void FixedUpdate()
    {
 
        if (Input.GetKey(KeyCode.Space))
        {
            Reset();
        }
 
        if (multiplier > 0)
        {
            transform.Rotate(Vector3.forward, 1 * multiplier);
        }
        else
        {
            isStoped = true;
        }
 
        if (multiplier < 20 && !round1)
        {
            multiplier += 0.1f;
        }
        else
        {
            round1 = true;
        }
 
        if (round1 && multiplier > 0)
        {
            multiplier -= reducer;
        }
    }
 
 
    void Reset()
    {
        multiplier = 1;
        reducer = Random.Range(0.01f, 0.5f);
        round1 = false;
        isStoped = false;
    }

}
