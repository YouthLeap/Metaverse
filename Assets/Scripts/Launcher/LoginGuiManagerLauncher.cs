using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Runtime.InteropServices;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class LoginGuiManagerLauncher : MonoBehaviour
{
    public InputField loginEmail;
    public InputField loginPass;

    public InputField registerName;
    public InputField registerEmail;
    public InputField registerPass;

    public InputField recoveryEmail;

    public GameObject loginPage;
    public GameObject registerPage;
    public GameObject recovaryPage;

    public Text errormassage;
    public static LoginGuiManagerLauncher instance;

    public Toggle termsaccept;
    public Button login;
    public Button register;
    public Button forgot;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void GoFullscreen();
 
    public void ActivateFullscreen()
    {
        GoFullscreen();
    }
#else
    public void ActivateFullscreen()
    { }
#endif

#if PLATFORM_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenURLInExternalWindow(string str);
    
    [DllImport("__Internal")]
    private static extern void closewindow();
#endif

    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        loginEmail.text = PlayerPrefs.GetString("Email", "");
        loginPass.text = PlayerPrefs.GetString("Pass", "");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (loginPage.activeSelf)
                Login();
            else if (registerPage.activeSelf)
                Register();
            else if (recovaryPage.activeSelf)
                PasswordRecover();
        }

        loginEmail.enabled=!PlayManager.instance.isbusy;
        loginPass.enabled = !PlayManager.instance.isbusy;
        login.enabled = !PlayManager.instance.isbusy;
        register.enabled = !PlayManager.instance.isbusy;
        forgot.enabled = !PlayManager.instance.isbusy;


    }
    public void TermsAndCOndition()
    {
#if PLATFORM_WEBGL
        OpenURLInExternalWindow("https://metaruffy.io/privacy-policy");
#else
        Application.OpenURL("https://metaruffy.io/privacy-policy");
#endif
    }
    public void OnLoginSuccess()
    {
        PlayerPrefs.SetString("Email", loginEmail.text);
        PlayerPrefs.SetString("Pass", loginPass.text);
        UnityEngine.Debug.Log("asdasdasd");
        int count = PlayerPrefs.GetInt("PlayerSelect", -1);
        PlayManager.instance.PlayGame();
    }

    public void ErrorMasage(string massage, Color color)
    {
        PopUpHelper.instance.SetMassage(massage,color);
        PopUpHelper.instance.StopLoading();
    }


    public void LoginPage()
    {
        loginPage.SetActive(true);
        registerPage.SetActive(false);
        recovaryPage.SetActive(false);
    }

    public void RegisterPage()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(true);
        recovaryPage.SetActive(false);
    }

    public void RecovaryPage()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(false);
        recovaryPage.SetActive(true);
    }

    public void Login()
    {
        if (!loginEmail.text.Contains("@"))
        {
            ErrorMasage("Provide a valid E-mail", Color.red);
            return;
        }

        bool hasnumber = false;
        bool hasspecial = false;

        for (int i = 0; i < loginPass.text.Length; i++)
        {
            if (loginPass.text[i] >= '0' && loginPass.text[i] <= '9')
            {
                hasnumber = true;
                break;
            }
        }

        for (int i = 0; i < loginPass.text.Length; i++)
        {
            if (!(loginPass.text[i] >= '0' && loginPass.text[i] <= '9') && !(loginPass.text[i] >= 'a' && loginPass.text[i] <= 'z') && !(loginPass.text[i] >= 'A' && loginPass.text[i] <= 'Z'))
            {
                hasspecial = true;
                break;
            }
        }


        if (loginPass.text.Length < 6)
        {
            ErrorMasage("Password must be Minimum 6 character", Color.red);
            return;
        }
        ServerManager.instance.email = loginEmail.text;
        ServerManager.instance.password = loginPass.text;
        //FireBaseLogin.instance.LoginWithEmail();
        PopUpHelper.instance.StartLoading();
    }

    public void Register()
    {

        

        UnityEngine.Debug.Log("registering");
        if (registerName.text.Length < 4)
        {
            ErrorMasage("Name must be atleast 4 caracters long.", Color.red);
            return;
        }

        if (!registerEmail.text.Contains("@"))
        {
            ErrorMasage("Provide a valid E-mail", Color.red);
            return;
        }

        bool hasnumber = false;
        bool hasspecial = false;

        for (int i=0;i< registerPass.text.Length;i++)
        {
            if(registerPass.text[i]>='0'&& registerPass.text[i] <= '9')
            {
                hasnumber = true;
                break;
            }
        }

        for (int i = 0; i < registerPass.text.Length; i++)
        {
            if (!(registerPass.text[i] >= '0' && registerPass.text[i] <= '9')&& !(registerPass.text[i] >= 'a' && registerPass.text[i] <= 'z')&& !(registerPass.text[i] >= 'A' && registerPass.text[i] <= 'Z'))
            {
                hasspecial = true;
                break;
            }
        }


        if (registerPass.text.Length<6)
        {
            ErrorMasage("Password must be Minimum 6 character", Color.red);
            return;
        }

        if (!termsaccept.isOn)
        {
            ErrorMasage("Accept the terms and conditions.", Color.red);
            return;
        }

        ServerManager.instance.username = registerName.text;
        ServerManager.instance.email = registerEmail.text;
        ServerManager.instance.password = registerPass.text;
        //PlayFabLogin.instance.RegisterWithEmailLauncher();
        ServerManager.instance.SignUp(registerName.text, registerEmail.text, registerPass.text, (status, errMsg) =>
        {
            if (status == 200)
            {
                ErrorMasage("Your Account is Registered.\nPlease verify your email.", Color.green);
            }
            else
            {
                PopUpHelper.instance.SetMassage(errMsg, Color.red);
            }
        });
    }

    public void PasswordRecover()
    {
        ServerManager.instance.email = recoveryEmail.text;
        //FireBaseLogin.instance.RecoverWithEmailLauncher();
    }
}
