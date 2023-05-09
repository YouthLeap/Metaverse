using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingscenegui : MonoBehaviour
{

    public Image loader;

    float percent = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool check = true;
    // Update is called once per frame
    void Update()
    {

        if(NetworkManager.instance.coonectedmaster && check)
        {
            //TestRoom101
            NetworkManager.instance.CreateRoom("GameScene");
            check = false;
        }

        percent += Time.deltaTime * 1;

        if (percent > 1)
            percent = 0;

        loader.fillAmount = percent;
    }
}
