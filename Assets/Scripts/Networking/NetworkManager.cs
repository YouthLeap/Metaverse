using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using agora_gaming_rtc;
using System;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    //28868fe87af54d0d9ffe09b21fe9ecbb
    //4a50153e2aa348519815d4b837a9932b
    string appID = "28868fe87af54d0d9ffe09b21fe9ecbb";

    public IRtcEngine rtcEngine;

    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        QualitySettings.vSyncCount = 0; // VSync must be disabled.
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
#if PLATFORM_STANDALONE
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
#endif

#if PLATFORM_ANDROID
        Permission.RequestUserPermission(Permission.Microphone);
#endif
    }

    private void Update()
    {
#if PLATFORM_STANDALONE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
#endif
    }
    private void OnError(int error, string msg)
    {
        Debug.Log("Error : "+msg + error);
    }

    public void OnLeaveChannel(RtcStats stats)
    {
        Debug.Log("Leave Channel : " + stats.duration);
    }
    public Hashtable hash = new Hashtable();
    public void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        //Debug.Log("Joined Channel : " + channelName);
        hash = new Hashtable();
        hash.Add("agoraID", uid.ToString());
        PhotonNetwork.SetPlayerCustomProperties(hash);
        //Debug.Log("Gain hassed2" + uid.ToString());
    }

    public void onJoinChannelSuccess(string channelName)
    {
        string chanel = channelName.Substring(0,channelName.IndexOf('|'));
        string uid= channelName.Substring(channelName.IndexOf('|')+1, channelName.Length- channelName.IndexOf('|')-1);
        //Debug.Log("Joined Channel : " + channelName);
        hash = new Hashtable();
        hash.Add("agoraID", uid.ToString());
        PhotonNetwork.SetPlayerCustomProperties(hash);
        Debug.Log("Gain hassed3"+ uid.ToString());
    }



    //NetworkFunctions

    public void ConnectToServerServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void ConnectToAudioServer()
    {
        rtcEngine = IRtcEngine.getEngine(appID);
        rtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        rtcEngine.OnLeaveChannel = OnLeaveChannel;
        rtcEngine.OnError = OnError;

        //rtcEngine.EnableSoundPositionIndication(true);

    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }


    public string RoomName;
    public string ChangeRoomName;
    public void CreateRoom(string roomName)
    {
        RoomName = roomName;
        PhotonNetwork.CreateRoom(roomName);
        
    }

    public void ChangeRoom(string roomName)
    {
        ChangeRoomName = roomName;
        LeaveRoom(1);
        LoadingScreen.instance.LoadingStart();
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    int isleaving;
    public void LeaveRoom(int leaving)
    {
        if (!coonectedmaster)
            return;
        isleaving = leaving;
        PhotonNetwork.LeaveRoom();
    }


    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void Disconnect()
    {
        Debug.Log("DisconnectOne");
        PhotonNetwork.Disconnect();
    }

    public Transform InstansiateObject(string name,Vector3 position= default, Quaternion rotation=default)
    {
        return PhotonNetwork.Instantiate(name, position, rotation).transform;
    }

    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public bool coonectedmaster = false;
    //Callbacks
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        
        JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        coonectedmaster = true;

        if(ChangeRoomName!="")
        {
            CreateRoom(ChangeRoomName);
            ChangeRoomName = "";
        }

    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Room Created"+PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        rtcEngine.JoinChannel(PhotonNetwork.CurrentRoom.Name);
        ChangeScene(RoomName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        JoinRoom(RoomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        rtcEngine.LeaveChannel();
        Debug.Log("room left");
        if(isleaving==0)
            ChangeScene("PlayerSelectionScene");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        coonectedmaster = false;
        Debug.Log("disconnected");
        IRtcEngine.Destroy();
        SceneManager.LoadScene("LoginScene");
        //ChangeScene("NameSelection");
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        Debug.Log("leave lobby");
        coonectedmaster = false;
        PhotonNetwork.Disconnect();

    }

    private void OnDestroy()
    {
        IRtcEngine.Destroy();
    }

    public int Ping()
    {
        return PhotonNetwork.GetPing();
    }

    public int TotalPlayer()
    {
        return PhotonNetwork.CurrentRoom==null?0:PhotonNetwork.CurrentRoom.PlayerCount;
    }

}
