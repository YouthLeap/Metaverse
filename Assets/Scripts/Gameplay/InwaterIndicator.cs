using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InwaterIndicator : MonoBehaviour
{

    public bool isSubmarged;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
            isSubmarged = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Water")
            isSubmarged = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
            isSubmarged = false;
    }
}
