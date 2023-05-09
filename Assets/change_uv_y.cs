using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_uv_y : MonoBehaviour
{
    public float scrol_y = 0.5f;
    public float duration = 3f;
    public float target;
    public float start_time;

    // Start is called before the first frame update
    void Start()
    {
        start_time = Time.time;
        target  = scrol_y + 0.3333f;
    }
    // Update is called once per frame
    void Update()
    {
            float t = (Time.time - start_time)/ duration;
            Vector2 m = new Vector2(0, Mathf.SmoothStep(scrol_y,target,t));
            GetComponent<Renderer>().material.mainTextureOffset = m;

            if(m.y == target)
            {
                StartCoroutine(ExampleCoroutine(m.y));
            }         
    }  

    IEnumerator ExampleCoroutine(float value)
    {
        yield return new WaitForSeconds(3);
        if(value == 1)
        {
            scrol_y = 0;
            target =  0.333333f;
        }
        else
        {
            scrol_y = value;
            target = value + 0.333333f;
        }    
        start_time = Time.time;
    }


}
