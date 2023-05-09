using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpHelper : MonoBehaviour
{
    public static PopUpHelper instance;
    public Text textmassage;
    public GameObject board;
    public GameObject loadingback;
    public Image loading;

    float timarlimit = 2.0f;
    float timarcount = 0;

    private void Awake()
    {
        instance = this;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        board.SetActive(false);
        loadingback.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timarcount += Time.deltaTime;

        if (timarcount > timarlimit)
            timarcount = 0;
        loading.fillAmount = (timarcount / timarlimit);
    }

    public void BtnCross()
    {
        //AudioManager.instance.PlayButtonClick();
        board.SetActive(false);
    }

    public void SetMassage(string massage,Color color)
    {
        board.SetActive(true);
        textmassage.text = massage;
        textmassage.color = color;
    }

    public void StartLoading()
    {
        timarcount = 0;
        loadingback.SetActive(true);
    }
    public void StopLoading()
    {
        loadingback.SetActive(false);
    }
}
