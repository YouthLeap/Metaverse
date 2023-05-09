using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcceptRejectFriend : MonoBehaviour
{
    public Text maintext;
    public FriendData data;
    public string friendName;

    public static AcceptRejectFriend instance;

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
        maintext.text = "Do you want to accept friend request from \n" + friendName + "";
    }

    public void SetData(FriendData fdata,string fname)
    {
        data = fdata;
        friendName = fname;
        gameObject.SetActive(true);
    }

    public void Accept()
    {
        ServerManager.instance.AcceptFriend(data._id, true);
        gameObject.SetActive(false);
    }

    public void Reject()
    {
        ServerManager.instance.AcceptFriend(data._id, false);
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
