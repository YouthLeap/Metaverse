using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePlayer : MonoBehaviour
{
    public Text playerName;
    public Image playerImage;
    public Text addremovefriend;
    public Text blockUnblock;

    public int index=0;

    public bool isfriend=false;
    public bool isblocked = false;
    public bool isotherblocked = false;
    public GameObject playerkick;
    public FriendData data = new FriendData();

    void Start()
    {
        if(ServerManager.instance.playerData.isAdmin)
        {
            playerkick.SetActive(true);
        }
    }
    float timarcleanar = 0.1f;
    // Update is called once per frame
    void Update()
    {
        if (index<OnlinePLayerManager.instance.playerList.Count)
        {
            playerName.text = OnlinePLayerManager.instance.playerList[index].playerName;
            playerImage.gameObject.SetActive(OnlinePLayerManager.instance.playerList[index].Muted);
            isblocked= OnlinePLayerManager.instance.playerList[index].isblocked|| OnlinePLayerManager.instance.playerList[index].isotherblocked;
            isotherblocked = OnlinePLayerManager.instance.playerList[index].isblocked;
        }
        timarcleanar -= Time.deltaTime;

        if(timarcleanar<0)
        {
            timarcleanar = 0.1f;
            

        }


        addremovefriend.text = data.isAproved ? "Unfriend" :(data.receiver == OnlinePLayerManager.instance.playerList[index].playerEmail ? "Accept": (data.sender != "" ? "Pending" : "AddFriend"));
        blockUnblock.text = isotherblocked ? "Unblock":"Block";
    }

    public void MuteUnmute()
    {
        AudioManager.instance.PlayButtonClick();
        if (index < OnlinePLayerManager.instance.playerList.Count)
        {
            OnlinePLayerManager.instance.playerList[index].Muted = !OnlinePLayerManager.instance.playerList[index].Muted;
            Debug.Log("MuteUpmute index");
        }
    }

    public void MuteUnmute(bool mu)
    {
        AudioManager.instance.PlayButtonClick();
        if (index < OnlinePLayerManager.instance.playerList.Count)
        {
            OnlinePLayerManager.instance.playerList[index].Muted = mu;
        }
    }

    public void FriendUnfriend()
    {
        AudioManager.instance.PlayButtonClick();
        isfriend = !isfriend;

        if(!data.isAproved && data.sender == "" && data.receiver == "")
        {
            data.sender = ServerManager.instance.playerData.email;
            data.receiver = OnlinePLayerManager.instance.playerList[index].playerEmail;
            data.isAproved = false;
            ServerManager.instance.RequsetFriend(data);
        }
        else if(!data.isAproved && data.receiver == ServerManager.instance.playerData.email)
        {
            AcceptRejectFriend.instance.SetData(data, OnlinePLayerManager.instance.playerList[index].playerName);
        }
        else if (data.isAproved)
        {
            ServerManager.instance.AcceptFriend(data._id, false);
        }
    }

    public void Kick()
    {

        Debug.Log("Kicked PLayer");
        if (index < OnlinePLayerManager.instance.playerList.Count)
        {
            OnlinePLayerManager.instance.playerList[index].Kick();
        }
    }

    public void Block()
    {
        if (!isotherblocked)
        {
            ServerManager.instance.blocklist.Add(OnlinePLayerManager.instance.playerList[index].playerEmail);
            Debug.Log("NotIsBlocked"+ ServerManager.instance.blocklist.Count);
        }
        else
        {
            ServerManager.instance.blocklist.Remove(OnlinePLayerManager.instance.playerList[index].playerEmail);
            Debug.Log("IsBlocked" + ServerManager.instance.blocklist.Count);
        }

        ServerManager.instance.UpdateBlockList();

    }

    public void Report()
    {

        Debug.Log("Kicked PLayer");
        if (index < OnlinePLayerManager.instance.playerList.Count)
        {
            Debug.Log("Kicked PLayer2");
            ReportManager.instance.ShowReport(OnlinePLayerManager.instance.playerList[index]);
        }
    }
}
