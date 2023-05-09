using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportManager : MonoBehaviour
{
    public Text MainText;
    public InputField reson;

    public PlayerController controller;

    public static ReportManager instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        MainText.text = "You want to submit a report against "+controller.playerName+ " Please provide us the reason below.";
    }

    public void Report()
    {
        if(controller!=null && controller.playerEmail!="")
            FirebaseData.instance.UpdateReport(FirebaseData.RemoveSpecialCharacters(controller.playerEmail), reson.text);
        gameObject.SetActive(false);
        GameManager.instance.isreporting = false;
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
        GameManager.instance.isreporting = false;
    }

    public void ShowReport(PlayerController ctr)
    {
        controller = ctr;
        gameObject.SetActive(true);
        GameManager.instance.isreporting=true;
    }
}
