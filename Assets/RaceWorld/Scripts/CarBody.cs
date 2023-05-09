using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBody : MonoBehaviour
{
    public Transform tirefront1;
    public Transform tirefront2;
    public Transform tireback1;
    public Transform tireback2;
    public Transform exaust;

    public float stearing;
    public float speed;
    private void Awake()
    {
        tirefront1 = transform.Find("A1_TLF");
        tirefront2 = transform.Find("A1_TRF");
        tireback1 = transform.Find("A1_TLR");
        tireback2 = transform.Find("A1_TRR");
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Stearing(float ammount,bool sound)
    {
        stearing = ammount;

        if (sound&&speed > 10 && (stearing>0.5f|| stearing < -0.5f))
        {
            int ran = Random.Range(0, 3);
            if(ran==0)
                AudioManager.instance.Playcontinue("TIRESCREECH1",new string[] { "TIRESCREECH1", "TIRESCREECH2", "TIRESCREECH3" });
            else if(ran==1)
                AudioManager.instance.Playcontinue("TIRESCREECH2", new string[] { "TIRESCREECH1", "TIRESCREECH2", "TIRESCREECH3" });
            else
                AudioManager.instance.Playcontinue("TIRESCREECH3", new string[] { "TIRESCREECH1", "TIRESCREECH2", "TIRESCREECH3" });
        }
        

        if (tirefront1!=null)
            tirefront1.localRotation = Quaternion.Euler(tirefront1.localRotation.eulerAngles.x, 30 * ammount, tirefront1.localRotation.eulerAngles.z);
        if (tirefront2 != null)
            tirefront2.localRotation = Quaternion.Euler(tirefront1.localRotation.eulerAngles.x, 30 * ammount, tirefront1.localRotation.eulerAngles.z);
    }

    public void Speed(float ammount, bool sound)
    {
        speed = ammount;

        if (sound&&speed > 5)
            AudioManager.instance.Playcontinue("RACETHROTTLE");
        else
            AudioManager.instance.Stop("RACETHROTTLE");

        if (tirefront1 != null)
            tirefront1.localRotation *= Quaternion.Euler(ammount, 0, 0);
        if (tirefront2 != null)
            tirefront2.localRotation *= Quaternion.Euler(ammount, 0, 0);
        if (tireback1 != null)
            tireback1.localRotation *= Quaternion.Euler(ammount, 0, 0);
        if (tireback2 != null)
            tireback2.localRotation *= Quaternion.Euler(ammount, 0, 0);
    }
}
