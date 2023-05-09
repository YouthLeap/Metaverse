using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class Teleportar : MonoBehaviour
{
    public Vector3 teleportLocation;
    public Transform teleporttransform;

    public Transform rotatetarget;

    public GameObject outside;
    public GameObject inside;
    public string stopsound;
    public string startsound;

    public bool IsInsideBuilding;

    public delegate void TeleportFunction(Transform trans);

    public TeleportFunction teleportFunction;

    public string SceneName;

    void Start()
    {
        teleportFunction = Teleported;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Teleported(Transform trans)
    {
        if(SceneName!="")
        {
            GameManager.othermapposition = teleportLocation;
            NetworkManager.instance.ChangeRoom(SceneName);
            //trans.position = teleportLocation;
        }
        else if(trans==GameManager.instance.mycontroller.transform)
        {
            AudioManager.instance.Play("TELEPORT");
            if (outside != null)
                outside.SetActive(false);
            if (inside != null)
                inside.SetActive(true);

            if (IsInsideBuilding)
                SeasonManager.instance.LightOn(true);
            else
                SeasonManager.instance.LightOn(false);

            AudioManager.instance.Stop(stopsound);
            AudioManager.instance.Play(startsound);

            if (teleporttransform == null)
            {
                trans.position = teleportLocation;

            }
            else
            {
                trans.position = teleporttransform.position;
                trans.rotation = teleporttransform.rotation;
                if (trans.GetComponent<ThirdPersonController>())
                {
                    trans.GetComponent<ThirdPersonController>()._cinemachineTargetPitch = teleporttransform.rotation.eulerAngles.x;
                    trans.GetComponent<ThirdPersonController>()._cinemachineTargetYaw = teleporttransform.rotation.eulerAngles.y;
                }
                trans.GetComponent<CharacterController>().SimpleMove(trans.forward * 2);
                //Debug.Log(teleporttransform.rotation);
            }
        }
        

        

        Debug.Log("Teleported");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CharacterController>()!=null)
        {
            other.GetComponent<CharacterController>().enabled = false;
            teleportFunction(other.transform);
            other.GetComponent<CharacterController>().enabled = true;
        }
            
    }


}
