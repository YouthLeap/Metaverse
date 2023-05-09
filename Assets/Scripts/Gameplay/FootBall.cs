using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBall : MonoBehaviour,IUseable
{

    Rigidbody rb;
    PhotonView view;

    public AudioSource kicksound;

    public void Use(Transform tf)
    {
        view.RPC("Kick", RpcTarget.AllBuffered, tf.position);
        kicksound.Play();
    }

    [PunRPC]
    void Kick(Vector3 pos)
    {
        rb.velocity = (rb.position - pos + new Vector3(0, 0.25f, 0)).normalized * 5;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
