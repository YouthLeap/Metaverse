using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WebViewGuiManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.instance.LeaveRoom(2);
        NetworkManager.instance.coonectedmaster = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.instance.coonectedmaster && check)
        {
            NetworkManager.instance.CreateRoom(NetworkManager.instance.RoomName);
            check = false;
        }
    }
    bool check = false;
    bool clicked = true;
    public void GoToSelection()
    {
        if(clicked)
        {
            clicked = false;
            check = true;
        }
            
    }
}
