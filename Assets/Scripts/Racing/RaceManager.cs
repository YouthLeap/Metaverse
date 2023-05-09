using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public Image start1;
    public Image start2;
    public Image start3;

    public Image pstart1;
    public Image pstart2;
    public Image pstart3;

    public Image minimap;

    public Text gotext;
    public Text timartext;
    public Button buttonexit;
    public Button buttonoff;
    public Text onofftext;

    float counter = -1;
    public float countinterval;
    public long currentlaptime;
    public long currentTotallaptime;
    CarCaracterController carcontrol;
    public GameObject PopUp;
    public Text speedmeter;

    public string congratulationtext;
    public GameObject fakeobject;
    public GameObject fakeobject2;

    //edward
    public GameObject WaitingPanel;
    public bool isStart = false;
    public GameObject RestartPanel;

    public AudioSource engineAudioSource;
    public List<Transform> m_SpawnList;
    public long m_currenttime;
    //
    public Vector3 othermapposition;


    // Start is called before the first frame update
    void Start()
    {
        pstart1.gameObject.SetActive(false);
        pstart2.gameObject.SetActive(false);
        pstart3.gameObject.SetActive(false);
        gotext.gameObject.SetActive(false);
        buttonexit.gameObject.SetActive(false);
        buttonoff.gameObject.SetActive(false);
        minimap.gameObject.SetActive(false);

        PopUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(PopUp!=null && PopUp.gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        float speed = 0;
        if (GameManager.instance.mycontroller.carcont.rb != null)
        {
            speed = GameManager.instance.mycontroller.carcont.rb.velocity.magnitude;
        }

        if (kmph)
        {
            speedmeter.text = (speed * 3.6).ToString("0.0") + " KMPH";
        }
        else
        {
            speedmeter.text = (speed * 3.6 / 1.62).ToString("0.0") + " MPH";
        }

        //edward
        if (!isstartrace) return;

        //if (GameManager.instance.m_totalplayers < 2)
        //{
        //    if (!isStart)
        //    {
        //        if (WaitingPanel != null)
        //        {
        //            WaitingPanel.SetActive(true);
        //        }
        //    }
        //    return;
        //}
        //else
        //{
        //    if (WaitingPanel != null)
        //    {
        //        WaitingPanel.SetActive(false);
        //    }
        //}


        EngineSound();
        //

        if (counter < 0)
            return;

        counter += Time.deltaTime;

        if(counter > countinterval * 4.2f)
        {
            start1.transform.parent.gameObject.SetActive(false);
            start2.transform.parent.gameObject.SetActive(false);
            start3.transform.parent.gameObject.SetActive(false);
            if (carcontrol != null)
                carcontrol.raceready = true;
            if (GameManager.instance.laptime == 0)
                GameManager.instance.laptime = DateTime.UtcNow.Ticks;
            currentlaptime = DateTime.UtcNow.Ticks;
            currentTotallaptime = DateTime.UtcNow.Ticks - GameManager.instance.laptime;
            
            if(isStart)
            {
                gotext.text = carcontrol.wrongway;
                m_currenttime = DateTime.UtcNow.Ticks - GameManager.instance.laptime;
                timartext.text = "   Best Time: " + GetTime(ServerManager.instance.playerRacingData.yearlylaptime) + "\n\n" +
                                 "Current Time: " + GetTime(m_currenttime) + "\n" +
                    PlayerPrefs.GetString("LastRace1", "") + "\n" +
                    PlayerPrefs.GetString("LastRace2", "") + "\n" +
                    PlayerPrefs.GetString("LastRace3", "") + "\n" +
                    PlayerPrefs.GetString("LastRace4", "") + "\n" +
                    PlayerPrefs.GetString("LastRace5", "");
            }
            else
            {
                GameManager.instance.mycontroller.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (counter > countinterval * 4)
        {
            //edward
            GameManager.instance.FreeCursor(false);
            isStart = true;
            //
        }
        else if (counter > countinterval * 3.5)
        {
            GameManager.instance.mycontroller.gameObject.GetComponent<Rigidbody>().mass = 5000;
            //GameManager.instance.mycontroller.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        }
        else if (counter > countinterval * 3)
        {
            start3.color = Color.green;
            gotext.text = "1";
            GameManager.instance.laptime = 0;
        }
        else if (counter > countinterval * 2)
        {
            start2.color = Color.green;
            gotext.text = "2";
            GameManager.instance.laptime = 0;
        }
        else if (counter > countinterval)
        {
            start1.color = Color.green;
            gotext.text = "3";
            AudioManager.instance.Playcontinue("RACECOUNTDOWN");
            GameManager.instance.laptime = 0;
            SetupEngineAudio();
        }
    }

    //edward
    public void SetupEngineAudio()
    {
        if (engineAudioSource)
        {
            //engineAudioSource.clip = engineSound;
            engineAudioSource.loop = true;
            //engineAudioSource.spatialBlend = 1.0f;
            engineAudioSource.minDistance = 3.0f;
            engineAudioSource.maxDistance = 1000f;
            engineAudioSource.Play();
        }
    }

    public void StopEngineAudio()
    {
        if (engineAudioSource)
        {
            engineAudioSource.Stop();
        }
    }

    public void EngineSound()
    {
        if(carcontrol != null)
        {
            engineAudioSource.pitch = Mathf.MoveTowards(engineAudioSource.pitch, carcontrol.carmaincontroller.CurrentSpeed / carcontrol.carmaincontroller.MaxSpeed + 0.5f, 0.05f);

            engineAudioSource.volume = Mathf.MoveTowards(engineAudioSource.volume, 0.75f + Mathf.Abs(carcontrol.move.y), 0.05f);
        }
    }
    //

    public static bool isstartrace = false;

    public void StartOtherMapRace()
    {
        NetworkManager.instance.ChangeRoom("RaceTrackScene");
        isstartrace = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //edward
        SetupEngineAudio();
        //
    }

    public void StartRace()
    {
        carcontrol = GameManager.instance.mycontroller.GetComponent<CarCaracterController>();
        carcontrol.Active(true);
        PopUp.SetActive(false);
        pstart1.gameObject.SetActive(true);
        pstart2.gameObject.SetActive(true);
        pstart3.gameObject.SetActive(true);
        gotext.gameObject.SetActive(true);
        buttonexit.gameObject.SetActive(true);
        buttonoff.gameObject.SetActive(true);
        minimap.gameObject.SetActive(true);
        timartext.gameObject.SetActive(true);
        gotext.text = "";
        timartext.text = "";
        counter = 0;
        start1.color = Color.red;
        start2.color = Color.red;
        start3.color = Color.red;
        start1.transform.parent.gameObject.SetActive(true);
        start2.transform.parent.gameObject.SetActive(true);
        start3.transform.parent.gameObject.SetActive(true);

        AudioManager.instance.Playcontinue("RACEENGINESTART");
        AudioManager.instance.Playcontinue("RACEDRILL");

        AudioManager.instance.Playcontinue("RACINGTHEMESONG");
        GameManager.instance.mycontroller.gameObject.layer = 11;
        
        //GameManager.instance.mainCam.cullingMask = LayerMask.GetMask(new string[] { "Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "Player" });
        GameInputHandler.instance.ActivateNewtral(false);
        //fakeobject.SetActive(false);
        //fakeobject2.SetActive(false);
    }

    public void SecondStartRace()
    {
        carcontrol = GameManager.instance.mycontroller.GetComponent<CarCaracterController>();
        carcontrol.Active(true);
        PopUp.SetActive(false);
        counter = countinterval * 3.5f;
        gotext.text = "Completed race in "+ GetTime(DateTime.UtcNow.Ticks - GameManager.instance.laptime);
        PlayerPrefs.SetString("LastRace5", PlayerPrefs.GetString("LastRace4", ""));
        PlayerPrefs.SetString("LastRace4", PlayerPrefs.GetString("LastRace3", ""));
        PlayerPrefs.SetString("LastRace3", PlayerPrefs.GetString("LastRace2", ""));
        PlayerPrefs.SetString("LastRace2", PlayerPrefs.GetString("LastRace1", ""));
        PlayerPrefs.SetString("LastRace1", GetTime(DateTime.UtcNow.Ticks - GameManager.instance.laptime));
        GameManager.instance.laptime = 0;
        GameManager.instance.mycontroller.carcont.maxdistancecovered = 5;
    }

    public void ShowResult()
    {
        gotext.text = "Completed race in " + GetTime(m_currenttime);
        PlayerPrefs.SetString("LastRace5", PlayerPrefs.GetString("LastRace4", ""));
        PlayerPrefs.SetString("LastRace4", PlayerPrefs.GetString("LastRace3", ""));
        PlayerPrefs.SetString("LastRace3", PlayerPrefs.GetString("LastRace2", ""));
        PlayerPrefs.SetString("LastRace2", PlayerPrefs.GetString("LastRace1", ""));
        PlayerPrefs.SetString("LastRace1", GetTime(m_currenttime));
        GameManager.instance.laptime = 0;
        GameManager.instance.mycontroller.carcont.maxdistancecovered = 5;
    }

    public void RestartRace()
    {
        isStart = true;
        counter = countinterval * 3.5f;
        gotext.text = "Completed race in " + GetTime(DateTime.UtcNow.Ticks - GameManager.instance.laptime);
        PlayerPrefs.SetString("LastRace5", PlayerPrefs.GetString("LastRace4", ""));
        PlayerPrefs.SetString("LastRace4", PlayerPrefs.GetString("LastRace3", ""));
        PlayerPrefs.SetString("LastRace3", PlayerPrefs.GetString("LastRace2", ""));
        PlayerPrefs.SetString("LastRace2", PlayerPrefs.GetString("LastRace1", ""));
        PlayerPrefs.SetString("LastRace1", GetTime(DateTime.UtcNow.Ticks - GameManager.instance.laptime));
        GameManager.instance.laptime = 0;
        GameManager.instance.mycontroller.carcont.maxdistancecovered = 5;
    }

    public void CancelRace()
    {
        GameInputHandler.instance.ActivateNewtral(false);
        PopUp.SetActive(false);
        AudioManager.instance.Stop("RACINGTHEMESONG");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool isfreecamOn=false;
    public void SwitchFreeCam()
    {
        isfreecamOn = !isfreecamOn;

        onofftext.text = "Free cam\n" + (isfreecamOn ? "On" : "Off");

        if (isfreecamOn)
        {
            GameManager.instance.cameravirtualC.Follow = GameManager.instance.mycontroller.cameraTarget;
        }
        else
        {
            GameManager.instance.cameravirtualC.Follow = GameManager.instance.mycontroller.cameraTarget2;
        }
    }

    //edward
    public void BacktoHomeButtonClick()
    {
        EndRace();
    }
    public void YesButtonClick()
    {
        RestartPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //RestartRace();
    }
    public void NoButtonClick()
    {
        EndRace();
    }
    //
    public void EndRace()
    {
        //edward
        StopEngineAudio();
        //
        pstart1.gameObject.SetActive(false);
        pstart2.gameObject.SetActive(false);
        pstart3.gameObject.SetActive(false);
        gotext.gameObject.SetActive(false);
        buttonexit.gameObject.SetActive(false);
        buttonoff.gameObject.SetActive(false);
        minimap.gameObject.SetActive(false);
        timartext.gameObject.SetActive(false);
        CarCaracterController car = GameManager.instance.mycontroller.GetComponent<CarCaracterController>();
        {
            GameManager.othermapposition = othermapposition;
            NetworkManager.instance.ChangeRoom("GameScene");
            isstartrace = false;
        }
        car.Active(false);
        counter = -1;
        GameManager.instance.mycontroller.gameObject.layer= 6;
        //fakeobject.SetActive(true);
        //fakeobject2.SetActive(true);
        //GameManager.instance.mainCam.cullingMask = LayerMask.GetMask(new string[] { "Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "Player", "FAKEGROUND", });

    }

    string GetTime(long tics)
    {

        tics /= 10000;


        long milis = tics % 1000;
        tics /= 1000;
        long seconds = tics % 60;
        tics /= 60;
        long minits = tics % 60;
        tics /= 60;
        long hours = tics % 24;
        tics /= 24;
        string str = "";
        
        if(hours>0)
        {
            if (hours <= 9)
                str += "0" + hours + ":";
            else
                str += hours + ":";
        }

        if(minits>9)
        {
            str += minits + ":";
        }
        else
        {
            str += "0" + minits + ":";
        }

        if(seconds>9)
        {
            str += seconds + ":";
        }
        else
        {
            str += "0" + seconds + ":";
        }

        if (milis > 99)
        {
            str += milis;
        }
        else if (milis>9)
        {
            str += "0" + milis;
        }
        else
        {
            str += "00" + milis;
        }


        return str;
    }
    bool kmph=false;
    public void SwitchMeter()
    {
        kmph = !kmph;
    }
    public Slider slider;
    public void NitroMeter(float nitro)
    {
        slider.value = nitro;
    }
}
