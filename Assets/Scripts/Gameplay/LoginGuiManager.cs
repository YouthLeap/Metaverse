using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System;
using Firebase.Database;

public class LoginGuiManager : MonoBehaviour
{

    public InputField loginEmail;
    public InputField loginPass;

    public InputField registerName;
    public InputField registerEmail;
    public InputField registerPass;

    public InputField recoveryEmail;

    public InputField changeEmail;
    public InputField oldPassword;
    public InputField newPassword;

    public GameObject loginPage;
    public GameObject registerPage;
    public GameObject recovaryPage;
    public GameObject changepwdPage;

    public Text errormassage;

    public static LoginGuiManager instance;

    public Toggle termsaccept;

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
        loginEmail.text = PlayerPrefs.GetString("Email","");
        loginPass.text = PlayerPrefs.GetString("Pass", "");
        NetworkManager.instance.coonectedmaster = false;
        NetworkManager.instance.ConnectToServerServer();
        registerName.onValueChanged.AddListener(delegate { RegisterNameValueChangeCheck(); });
    }
    public int attemtcount;

    public void RegisterNameValueChangeCheck()
    {
        registerName.text = BadWordChecker.instance.CheckWord(registerName.text);
    }
    // Update is called once per frame
    void Update()
    {
        DateTime lastAttempt = new DateTime(Convert.ToInt64(PlayerPrefs.GetString("AttemptLast", "0")));
        //Debug.Log(lastAttempt.Ticks);
        attemtcount = PlayerPrefs.GetInt("AttemptCount", 5);
        if ((DateTime.UtcNow - lastAttempt).Ticks > 300000000)
        {
            PlayerPrefs.SetInt("AttemptCount", 5);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (loginPage.activeSelf)
                Login();
            else if (registerPage.activeSelf)
                Register();
            else if (recovaryPage.activeSelf)
                PasswordRecover();
        }
    }

    public void OnLoginSuccess()
    {
        //if (PlayFabLogin.instance.playerData.isBanned)
        //{
        //    ErrorMasage("Your account has been banned", Color.red);
        //    return;
        //}
        //if (FirebaseData.instance.usermain.)
        //{
        //    ErrorMasage("Your account has been banned", Color.red);
        //    return;
        //}
        PlayerPrefs.SetString("AttemptLast", "0");
        PlayerPrefs.SetInt("AttemptCount", 5);

        PlayerPrefs.SetString("Email", loginEmail.text);
        PlayerPrefs.SetString("Pass", loginPass.text);
        int count = PlayerPrefs.GetInt("PlayerSelect", -1);

        NetworkManager.instance.ConnectToAudioServer();
        //TODO-POOH
        ServerManager.instance.GetData();
        //FirebaseData.instance.WatchFriendList();
        //FirebaseData.instance.GetBlockList();
        //FirebaseData.instance.WatchFightingLeaderBoard();
        //FirebaseData.instance.WatchRacingLeaderBoard();
        //2022/6/21 by tiger for testing...
        //NetworkManager.instance.ChangeScene("PlayerSelectionScene");

        if (count != int.Parse(ServerManager.instance.character.PlayerSelect))
            NetworkManager.instance.ChangeScene("PlayerSelectionScene");
        else
            NetworkManager.instance.ChangeScene("LoadingScene");
    }

    public void ErrorMasage(string massage,Color color)
    {
        PopUpHelper.instance.SetMassage(massage,color);
        PopUpHelper.instance.StopLoading();
    }


    public void LoginPage()
    {
        loginPage.SetActive(true);
        registerPage.SetActive(false);
        recovaryPage.SetActive(false);
        changepwdPage.SetActive(false);
        //AudioManager.instance.PlayButtonClick();
    }

    public void RegisterPage()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(true);
        recovaryPage.SetActive(false);
        changepwdPage.SetActive(false);
        //AudioManager.instance.PlayButtonClick();
    }

    public void RecovaryPage()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(false);
        recovaryPage.SetActive(true);
        changepwdPage.SetActive(false);
        //AudioManager.instance.PlayButtonClick();
    }

    public void ChangePasswordPage()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(false);
        recovaryPage.SetActive(false);
        changepwdPage.SetActive(true);
        //AudioManager.instance.PlayButtonClick();
    }

    public void Login()
    {
        //AudioManager.instance.PlayButtonClick();
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
            ErrorMasage("Password must be Minimum 6", Color.red);
            return;
        }

        PlayerPrefs.SetString("AttemptLast", DateTime.UtcNow.Ticks.ToString());
        PlayerPrefs.SetInt("AttemptCount", PlayerPrefs.GetInt("AttemptCount", 5) - 1);
        if (PlayerPrefs.GetInt("AttemptCount", 5)<0)
        {
            ErrorMasage("Try again a few minute later.", Color.red);
            return;
        }

        //FirebaseData.instance.usermain.UpdateEmailAsync(loginEmail.text);
        //FirebaseData.instance.usermain.UpdatePasswordAsync(loginPass.text);

        //PlayFabLogin.instance.email = loginEmail.text;
        //PlayFabLogin.instance.password = loginPass.text;

        ServerManager.instance.SignIn(loginEmail.text, loginPass.text, (status, errMsg) =>
        {
            if (status == 200)
            {
                ServerManager.instance.GetLoginUpdate();
                
            }else
            {
                PopUpHelper.instance.SetMassage(errMsg, Color.red);
            }
        });
        //StartCoroutine(FirebaseData.instance.LoginWithEmail(loginEmail.text, loginPass.text));

        //FirebaseData.instance.FightingLeaderBoardmain.ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
        //{
        //    if (e2.DatabaseError != null)
        //    {
        //        Debug.LogError(e2.DatabaseError.Message);
        //    }


        //    if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
        //    {

        //        foreach (var childSnapshot in e2.Snapshot.Children)
        //        {
        //            //Debug.Log(childSnapshot.Child("email").Value.ToString()+"====got here");
        //            if (childSnapshot.Child("email").Value.ToString() == loginEmail.text)
        //            {
        //                var name = childSnapshot.Child("name").Value.ToString();
        //                Debug.Log("==============================================================================================================" + name.ToString());
        //            }
        //            //text.text = name.ToString();
        //            //text.text = childSnapshot.ToString();
        //        }

        //    }
        //};
        //Debug.LogError(FirebaseData.instance.FightingLeaderBoardmain.Child(loginEmail.text));
        //PlayFabLogin.instance.LoginWithEmail();

    }
    public void TermsAndCOndition()
    {
#if PLATFORM_WEBGL
        OpenURLInExternalWindow("https://metaruffy.io/privacy-policy");
#else
        Application.OpenURL("https://metaruffy.io/privacy-policy");
#endif

    }
    public void Register()
    {
        //AudioManager.instance.PlayButtonClick();
        Debug.Log("registering");
        if (registerName.text.Length<4)
        {
            ErrorMasage("Name must be atleast 4 caracters long.", Color.red);
            return;
        }

        if (!registerEmail.text.Contains("@"))
        {
            ErrorMasage("Provide a valid E-mail", Color.red);
            return;
        }

        if (!CheckPasswordStrength(registerPass.text))
        {
            ErrorMasage("Password must be Minimum 8 character long\n and must contain a Capital letter a small letter a number\n and a special caracter", Color.red);
            return;
        }
        
        if (!termsaccept.isOn)
        {
            ErrorMasage("Accept the terms and conditions.", Color.red);
            return;
        }

        DateTime lastAttempt = new DateTime(Convert.ToInt64(PlayerPrefs.GetString("AttemptLastTime", "0")));

        //if ((DateTime.UtcNow - lastAttempt).Ticks < 3600000000)
        //{
        //    ErrorMasage("You can't create another account within 1 hour of the last register.", Color.red);
        //    return;
        //}
        PlayerPrefs.SetString("AttemptLastTime", DateTime.UtcNow.Ticks.ToString());

        Debug.LogError("here");


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
    
    public void ChangePassword()
    {
        if (oldPassword.text == newPassword.text)
        {
            ErrorMasage("Please don't use same password as before.", Color.red);
            return;
        }
        if (!CheckPasswordStrength(newPassword.text))
        {
            ErrorMasage("Password must be Minimum 8 character long\n and must contain a Capital letter a small letter a number\n and a special caracter", Color.red);
            return;
        }

        ServerManager.instance.ChangePassword(changeEmail.text, oldPassword.text, newPassword.text, (status, errMsg) =>
        {
            if (status == 200)
            {
                PopUpHelper.instance.SetMassage("Changed password successfully!", Color.green);
            }
            else
            {
                PopUpHelper.instance.SetMassage(errMsg, Color.red);
            }
        });
    }
    public void PasswordRecover()
    {
        ServerManager.instance.ResetPassword(recoveryEmail.text, (status, errMsg) =>
        {
            if (status == 200)
            {
                PopUpHelper.instance.SetMassage("Please check your mail for the recovery password.", Color.green);
            }
            else
            {
                PopUpHelper.instance.SetMassage(errMsg, Color.red);
            }
        });
    }

    private bool CheckPasswordStrength(string strPwd)
    {
        bool hasnumber = false;
        bool hascapital = false;
        bool hassmaller = false;
        bool hasspecial = false;

        for (int i = 0; i < strPwd.Length; i++)
        {
            if (strPwd[i] >= '0' && strPwd[i] <= '9')
            {
                hasnumber = true;
                break;
            }
        }

        for (int i = 0; i < strPwd.Length; i++)
        {
            if (strPwd[i] >= 'A' && strPwd[i] <= 'Z')
            {
                hascapital = true;
                break;
            }
        }

        for (int i = 0; i < strPwd.Length; i++)
        {
            if (strPwd[i] >= 'a' && strPwd[i] <= 'z')
            {
                hassmaller = true;
                break;
            }
        }

        for (int i = 0; i < strPwd.Length; i++)
        {
            if (!(strPwd[i] >= '0' && strPwd[i] <= '9') && !(strPwd[i] >= 'a' && strPwd[i] <= 'z') && !(strPwd[i] >= 'A' && strPwd[i] <= 'Z'))
            {
                hasspecial = true;
                break;
            }
        }


        if (strPwd.Length < 8 || !hascapital || !hasnumber || !hassmaller || !hasspecial)
        {
            return false;
        }

        return true;
    }

}
