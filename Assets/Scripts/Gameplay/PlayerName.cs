using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerName : MonoBehaviour
{
    public InputField namefield;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmBtn()
    {
        if(namefield.text!="")
            PlayerPrefs.SetString("PlayerName",namefield.text);
        SceneManager.LoadScene("PlayerSelectionScene");
    }

}
