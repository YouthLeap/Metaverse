using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryEffect : MonoBehaviour
{
    Vector3 localstart;
    Vector3 localEnd;
    // Start is called before the first frame update
    void Start()
    {
        localstart = transform.position;
        localEnd = transform.position +new  Vector3(500,0,0);

        transform.position = localEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if(nextposstart)
        {
            transform.position -= new Vector3(500 * Time.deltaTime, 0, 0);
            if (transform.position.x <= localstart.x)
                transform.position = localstart;
        }
        else
        {
            transform.position += new Vector3(500 * Time.deltaTime, 0, 0);
            if (transform.position.x >= localEnd.x)
                transform.position = localEnd;
        }


    }

    bool nextposstart = false;

    public void Open()
    {
        nextposstart = true;
    }
    public void Close()
    {
        nextposstart = false;
    }


}
