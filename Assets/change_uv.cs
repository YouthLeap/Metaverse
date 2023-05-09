using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_uv : MonoBehaviour
{
    public float scrol_x = 0.5f;
    public float duration = 3f;
    public float target;
    public float start_time;

    // Start is called before the first frame update
    void Start()
    {
        start_time = Time.time;
        target  = scrol_x + 0.2f;
    }
    // Update is called once per frame
    void Update()
    {
            float t = (Time.time - start_time)/ duration;
            Vector2 m = new Vector2(Mathf.SmoothStep(scrol_x,target,t), 0 );
            GetComponent<Renderer>().material.mainTextureOffset = m;

            if(m.x == target)
            {
                StartCoroutine(ExampleCoroutine(m.x));
            }         
    }  

    IEnumerator ExampleCoroutine(float value)
    {
        yield return new WaitForSeconds(3);
        if(value == 1)
        {
            scrol_x = 0;
            target =  0.2f;
        }
        else
        {
            scrol_x = value;
            target = value + 0.2f;
        }    
        start_time = Time.time;
    }


}
