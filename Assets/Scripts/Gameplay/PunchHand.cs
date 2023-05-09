using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchHand : MonoBehaviour
{
    public GameObject parentObject;
    public bool checkbool;
    SphereCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkbool && !punched && checkbool!= collider.enabled)
        {
            parentObject.GetComponent<PlayerController>().PlayAudio("PUNCH_MISS_" + Random.Range(1, 6), transform.position);
        }

        checkbool = collider.enabled;

        if (!collider.enabled)
        {
            punched = false;
        }
    }
    public bool punched = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != parentObject && other.tag == "Player")
        {
            parentObject.GetComponent<PlayerController>().PlayAudio("PUNCH_" + Random.Range(1, 8),transform.position);
            punched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        punched = false;
    }
}
