using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrivateZone : MonoBehaviour
{
    public GameObject visual;
    public GameObject visual2;
    public PhotonView view;
    public List<AudioSource> sources;

    public AudioSource scenario;
    float basevolume = 0.03f;
    public bool isinroom;
    public bool isblocked;
    public bool isotherblocked;
    PlayerController mycontroller;
    CarCaracterController carcontroller;
    string identifier = "";
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        mycontroller = GetComponent<PlayerController>();
        carcontroller = GetComponent<CarCaracterController>();
        identifier = mycontroller.playerEmail;

        if (GameManager.IngameScene())
        {
            if(view.IsMine)
                AudioManager.instance?.PlayLogin(1* PlayerPrefs.GetFloat("MasterAudio", 1.0f));
            else
                AudioManager.instance?.PlayLogin(0.25f* PlayerPrefs.GetInt("LogginToggle", 1)* PlayerPrefs.GetFloat("MasterAudio", 1.0f));
        }
        
        
    }
    public float multiplier = 1;
    // Update is called once per frame
    void Update()
    {

        if (view != null && !view.IsMine && visual.activeSelf)
            GetComponent<CapsuleCollider>().enabled = true;
        else
            GetComponent<CapsuleCollider>().enabled = false;

        

        isblocked = false;
        isotherblocked = false;
        for (int i=0;i< ServerManager.instance.blocklist.Count;i++)
        {
            if(ServerManager.instance.blocklist[i]==identifier)
            {
                isblocked = true;
            }
        }

        for (int i = 0; i < ServerManager.instance.otherblocklist.Count; i++)
        {
            if (ServerManager.instance.otherblocklist[i] == identifier)
            {
                isotherblocked = true;
            }
        }

        mycontroller.isblocked = isblocked;
        mycontroller.isotherblocked = isotherblocked;
        if (isinroom || isblocked || isotherblocked)
        {
            if (view != null && !view.IsMine)
            {
                visual?.SetActive(false);
                visual2?.SetActive(false);
            }
        }
        else
        {
            if (view != null && !view.IsMine)
            {
                if(!carcontroller.isactive)
                    visual?.SetActive(true);
                else
                    visual?.SetActive(false);
                visual2?.SetActive(true);
            }
        }

        if (view != null && view.IsMine)
        {
            if (GameManager.instance != null && GameManager.instance.cameravirtualF.gameObject.activeSelf)
                visual.SetActive(false);
            else
                visual.SetActive(true&&!carcontroller.isactive&&!carcontroller.isOnCar);
        }

        if (!visual2.activeSelf)
            return;

        if (mycontroller.inwater|| mycontroller.underwater)
            return;

        foreach (AudioSource source in sources)
            source.volume = basevolume * multiplier*PlayerPrefs.GetFloat("EffectAudio", 1.0f)* PlayerPrefs.GetFloat("MasterAudio", 1.0f);

        if (GetComponent<Animator>().GetFloat("Speed") > 2)
        {
            bool isplaying = false;

            foreach (AudioSource source in sources)
            {
                if (source.isPlaying)
                    isplaying = true;
            }
            if(!isplaying)
            {
                int rand = Random.Range(0, sources.Count);
                AudioSource source = sources[rand];
                if(!source.isPlaying)
                    source.Play();
                source.pitch = GetComponent<Animator>().GetFloat("Speed") / 2.5f;
            }
        }
        else if(GetComponent<Animator>().GetFloat("Speed")>1)
        {
            bool isplaying = false;

            foreach (AudioSource source in sources)
            {
                if (source.isPlaying)
                    isplaying = true;
            }
            if (!isplaying)
            {
                int rand = Random.Range(0, sources.Count);
                AudioSource source = sources[rand];
                if (!source.isPlaying)
                    source.Play();
                source.pitch = GetComponent<Animator>().GetFloat("Speed")*1.25f;
            }

        }
        else
        {
            foreach (AudioSource source in sources)
            {
                if (source.isPlaying)
                    source?.Stop();
            }
            
        }

        if(!GetComponent<Animator>().GetBool("Grounded"))
        {
            foreach (AudioSource source in sources)
            {
                if (source.isPlaying)
                    source?.Stop();
            }
        }
        
        
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {

        if (view != null && !view.IsMine)
            return;

        if (other.tag == "PrivateZone")
        {
            multiplier = 2;
            isinroom = true;
            if (view != null && view.IsMine)
            {
                GameManager.instance.interior.SetActive(true);
                scenario.volume = 0.0f;
            }
        }
        else if (other.tag == "PublicZone")
        {
            isinroom = false;
            if (view != null && view.IsMine)
            {
                GameManager.instance.interior.SetActive(false);
                scenario.volume = 0.0f;
            }
                
            multiplier = 1;
        }
    }
}
