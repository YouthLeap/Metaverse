using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicoptorEnabler : MonoBehaviour
{
    public GameObject[] helicopters;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag== "Player")
        {
            helicopters[0].gameObject.SetActive(true);
            helicopters[1].gameObject.SetActive(false);
            helicopters[2].gameObject.SetActive(false);
        }
    }
}
