using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using StarterAssets;
using PathCreation;
using UnityEngine.Rendering;
using Opsive.UltimateInventorySystem.Core;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public List<GameObject> playerPrefabs;
    public CinemachineVirtualCamera cameravirtual;
    public CinemachineVirtualCamera cameravirtualF;
    public CinemachineVirtualCamera cameravirtualC;
    

    public static GameManager instance;

    public GameObject CurvedMenuCamera;
    public GameObject ChatBox;
    public GameObject ChatBoxMobile;
    public GameObject ChatBoxPC;
    public GameObject CurvedMenu;
    public GameObject CurvedMenuTab;

    public GameObject areyousureUI;

    public Transform spawnPoint;

    public static Vector3 othermapposition;

    public PlayerController mycontroller;
    
    public Text pingText;
    public Text playerCountText;

    public Text pingTextPC;
    public Text playerCountTextPC;

    public Slider sensitivity;
    public Slider MasterAudio;
    public Slider VoiceAudio;
    public Slider EffectAudio;
    public Toggle LogginToggle;

    public long laptime;

    public GameObject interior;

    public int massagecount = 0;

    public UIControllerDEMO uiController;
    public GameObject uiObject;
    public GameObject battlearina;
    public List<GameObject> fightingbtns;
    public List<GameObject> drivingbtns;
    public List<GameObject> normalbtns;

    public List<GameObject> puncables;

    public bool isreporting = false;

    public PathCreator raceTrack;
    public RaceManager racemanager;
    public RaceManager racemanagerPc;
    public RaceManager racemanagerMobile;

    public Transform racestarttrans;
    public Camera mainCam;

    public Volume rvolume;

    public GameObject movestickmobile;
    public GameObject carstickmobile;
    //2022/6/14 by cai
    public GameObject walletPrefab;
    public GameObject inventoryCanvas;

    public int minimumPing = 500;
    public GameObject ConnectingUI;
    bool isTryingToConnect = false;
    Coroutine TimeoutCheck;

#if PLATFORM_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenURLInExternalWindow(string str);
    
    [DllImport("__Internal")]
    private static extern void closewindow();
#endif
    private void Awake()
    {
        instance = this;
        InvokeRepeating("PingCheck", 1, 1);
    }

    void PingCheck()
    {
        //Debug.Log("Ping check");
        if (NetworkManager.instance.Ping() < minimumPing || !PhotonNetwork.IsConnected || Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (TimeoutCheck == null)
            {

                ConnectingUI.SetActive(true);
                TimeoutCheck = StartCoroutine(LogoutIn(10));
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 0;
            }
        }
        else
        {
            if (TimeoutCheck != null)
            {

                ConnectingUI.SetActive(false);
                StopCoroutine(TimeoutCheck);
                TimeoutCheck = null;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 1;
            }
        }
        //yield return null;
    }

    public static class CoroutineUtilities
    {
        public static IEnumerator WaitForRealTime(float delay)
        {
            while (true)
            {
                float pauseEndTime = Time.realtimeSinceStartup + delay;
                while (Time.realtimeSinceStartup < pauseEndTime)
                {
                    yield return 0;
                }
                break;
            }
        }
    }


    IEnumerator LogoutIn(int seconds)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(seconds));
        // yield return new WaitForSeconds(seconds);
        LogOff();
    }
    public void MainMenuInput(InputAction.CallbackContext context)
    {
        CurvedMenu.SetActive(!CurvedMenu.activeSelf);
        CurvedMenuTab.SetActive(false);
        ChatBox.SetActive(!CurvedMenu.activeSelf);
        GameInputHandler.instance.ActivateNewtral(CurvedMenuTab.activeSelf || CurvedMenu.activeSelf);
    }

    public void OnlinePlayerInput(InputAction.CallbackContext context)
    {
        CurvedMenuTab.SetActive(!CurvedMenuTab.activeSelf);
        CurvedMenu.SetActive(false);
        ChatBox.SetActive(!CurvedMenuTab.activeSelf);
        GameInputHandler.instance.ActivateNewtral(CurvedMenuTab.activeSelf || CurvedMenu.activeSelf);

    }

    public void EmojiInput(InputAction.CallbackContext context)
    {
        uiObject.SetActive(!uiObject.activeSelf);
        FreeCursor(uiObject.activeSelf);
    }

    public void AudioInput(InputAction.CallbackContext context)
    {
        mycontroller.AudioOn = context.ReadValueAsButton();
        if (mycontroller.audiosource != null && (mycontroller.AudioOn || mycontroller.AdminAudioOn))
            mycontroller.audiosource.gameObject.SetActive(true);
        else if (mycontroller.audiosource != null)
            mycontroller.audiosource.gameObject.SetActive(false);
    }

    public void AdminAudioInput(InputAction.CallbackContext context)
    {
        if (ServerManager.instance.playerData.isAdmin)
            mycontroller.AdminAudioOn = context.ReadValueAsButton();
        if (mycontroller.audiosource != null && (mycontroller.AudioOn || mycontroller.AdminAudioOn))
            mycontroller.audiosource.gameObject.SetActive(true);
        else if (mycontroller.audiosource != null)
            mycontroller.audiosource.gameObject.SetActive(false);
    }

    public void RegisterInput()
    {
        GameInputHandler.instance.gameInput.Menu.MainMenu.performed += ctx => MainMenuInput(ctx);

        GameInputHandler.instance.gameInput.Menu.OnlinePlayer.performed += ctx => OnlinePlayerInput(ctx);

        GameInputHandler.instance.gameInput.Menu.Emoji.performed += ctx => EmojiInput(ctx);

        GameInputHandler.instance.gameInput.Menu.Talk.performed += ctx => AudioInput(ctx);
        GameInputHandler.instance.gameInput.Menu.Talk.canceled += ctx => AudioInput(ctx);

        GameInputHandler.instance.gameInput.Menu.AdminTalk.performed += ctx => AdminAudioInput(ctx);
        GameInputHandler.instance.gameInput.Menu.AdminTalk.canceled += ctx => AdminAudioInput(ctx);
    }
    IEnumerator latestartrace()
    {
        yield return new WaitForSeconds(0.25f);
        LoadingScreen.instance.LoadingStop();
    }
    void Start()
    {
        StartCoroutine(latestartrace());
        rvolume.enabled = false;
        sensitivity.value = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        MasterAudio.value = PlayerPrefs.GetFloat("MasterAudio", 1.0f);
        EffectAudio.value = PlayerPrefs.GetFloat("EffectAudio", 1.0f);
        VoiceAudio.value = PlayerPrefs.GetFloat("VoiceAudio", 1.0f);
        LogginToggle.isOn = PlayerPrefs.GetInt("LogginToggle", 1) == 0 ? false:true;
#if UNITY_ANDROID || UNITY_IOS
        ChatBox =ChatBoxMobile;
        ChatBoxPC.SetActive(false);
        ChatBoxMobile.SetActive(true);
        racemanager = racemanagerMobile;
#else
        Cursor.visible = false;
        ChatBox = ChatBoxPC;
        ChatBoxPC.SetActive(true);
        ChatBoxMobile.SetActive(false);
        racemanager = racemanagerPc;
#endif

        int count = int.Parse(ServerManager.instance.character.PlayerSelect);
        Transform trns= NetworkManager.instance.InstansiateObject(playerPrefabs[count].name, othermapposition!=Vector3.zero?(othermapposition) :spawnPoint.position, spawnPoint.rotation);
        mycontroller = trns.GetComponent<PlayerController>();
        mycontroller.AddCamera(cameravirtual, cameravirtualF,cameravirtualC);
        uiController.CharacterCustomization = mycontroller.characterCustomization;
        othermapposition = Vector3.zero;
        RegisterInput();
        
    }

    public void Update()
    {
        //Debug.Log($"Ping is ={NetworkManager.instance.Ping()} and Photon is {PhotonNetwork.IsConnected} and internet is {Application.internetReachability}");

        if (NetworkManager.instance.Ping() < minimumPing || !PhotonNetwork.IsConnected || Application.internetReachability == NetworkReachability.NotReachable)
        {
            
        }
        PingCheck();
        PlayerPrefs.SetInt("LogginToggle", LogginToggle.isOn ? 1 : 0);

        if(mycontroller!=null)
        {
            foreach(GameObject obj in fightingbtns)
            {
                obj.SetActive(mycontroller.inArena);
            }

            foreach (GameObject obj in normalbtns)
            {
                obj.SetActive(!mycontroller.inArena && !mycontroller.carcont.enabled);
            }

            //foreach (GameObject obj in drivingbtns)
            //{
            //    obj.SetActive(false);//TEMPOR 2022/9/19 CAI
            //}

        }
        
        if (pingText!=null && NetworkManager.instance!=null)
            pingText.text = "Ping: " +NetworkManager.instance.Ping()+"ms";
        if (playerCountText != null && NetworkManager.instance != null)
            playerCountText.text = "Total Players: " +NetworkManager.instance.TotalPlayer();
        if (pingTextPC != null && NetworkManager.instance != null)
            pingTextPC.text = "Ping: " + NetworkManager.instance.Ping() + "ms";
        if (playerCountTextPC != null && NetworkManager.instance != null)
            playerCountTextPC.text = "Total Players: " + NetworkManager.instance.TotalPlayer();
    }

    public void FreeCursor(bool free)
    {
#if !(UNITY_ANDROID || UNITY_IOS)
 if (free)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
#endif
    }

    /*    public void IsBusy(bool bsy)
        {
            busy = bsy;
            if(!busy)
            {
                _input.admintalk = false;
                _input.mainmenu = false;
                _input.onlineplayer = false;
            }
        }*/

    public void OnsesitivityChange()
    {
        PlayerPrefs.SetFloat("Sensitivity",sensitivity.value);
    }
    public void OnMasterAudioChange()
    {
        PlayerPrefs.SetFloat("MasterAudio", MasterAudio.value);
    }
    public void OnEffectAudioChange()
    {
        PlayerPrefs.SetFloat("EffectAudio", EffectAudio.value);
    }
    public void OnVoiceAudioChange()
    {
        PlayerPrefs.SetFloat("VoiceAudio", VoiceAudio.value);
    }

    public void Punch()
    {
        if(mycontroller!=null)
        {
            mycontroller.punchcounter = 0.25f;
        }
    }

    public void AreYouSure(bool status)
    {
        areyousureUI.SetActive(status);
    }
    public void LogOff()
    {

        if (NetworkManager.instance.Ping() < minimumPing || !PhotonNetwork.IsConnected || Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (TimeoutCheck == null)
            {

                NetworkManager.instance.Disconnect();
                SceneManager.LoadScene("LoginScene");
            }
        }
        else
        {
            if (TimeoutCheck != null)
            {

                ConnectingUI.SetActive(false);
                StopCoroutine(TimeoutCheck);
                TimeoutCheck = null;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 1;
            }
        }

    }

    public void Quit()
    {

#if PLATFORM_WEBGL

        closewindow();
#else
       Application.Quit(0);
#endif

    }

    public void Browser()
    {

#if PLATFORM_WEBGL

        OpenURLInExternalWindow("https://www.google.com");
#else
        NetworkManager.instance.ChangeScene("Browser");
#endif
    }

    public void Website()
    {

#if PLATFORM_WEBGL
        OpenURLInExternalWindow("https://www.ruffycoin.io");
#else
        NetworkManager.instance.ChangeScene("WebView");
#endif
    }

    public void CaracterCreation()
    {
        NetworkManager.instance.LeaveRoom(0);
    }

    public GameObject customizer;
    public void Settings()
    {
        customizer.SetActive(!customizer.activeSelf);
    }

    public void Back()
    {
        CurvedMenu.SetActive(!CurvedMenu.activeSelf);
        CurvedMenuTab.SetActive(false);
        ChatBox.SetActive(!CurvedMenu.activeSelf);
        GameInputHandler.instance.ActivateNewtral(CurvedMenuTab.activeSelf || CurvedMenu.activeSelf);
    }
    public void BackTab()
    {
        CurvedMenuTab.SetActive(!CurvedMenuTab.activeSelf);
        CurvedMenu.SetActive(false);
        ChatBox.SetActive(!CurvedMenuTab.activeSelf);
        GameInputHandler.instance.ActivateNewtral(CurvedMenuTab.activeSelf || CurvedMenu.activeSelf);
    }

    private void OnDestroy()
    {
        instance = null;
    }

    //2022/6/14 by cai
    public void onWalletConnectButtonClicked()
    {
        if (!walletPrefab.activeSelf)
            walletPrefab.SetActive(true);
    }

    public void onInventoryButtonClicked(bool flag)
    {
        inventoryCanvas.SetActive(flag);
    }

    public void onWalletCancelButtonClicked()
    {
        if (walletPrefab.activeSelf)
            walletPrefab.SetActive(false);
    }

    static string[] GameScenes= { "GameScene", "CastleScene", "FightArenaScene", "RaceTrackScene" };
    public static bool IngameScene()
    {

        for (int i = 0; i < GameScenes.Length; i++)
        {
            if (SceneManager.GetActiveScene().name == GameScenes[i])
                return true;
        }

        return false;
    }
    public void OnTakeCar(bool bFlag)
    {
        if (mycontroller != null)
        {
            mycontroller.OnTakeCar(bFlag);
        }
    }
}
