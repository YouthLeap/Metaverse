using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;
using AdvancedPeopleSystem;
using System.Collections.Generic;

public class ServerManager : MonoBehaviour
{
    private string baseURL = "http://157.230.188.64/";

    public string username = "";
    public string email = "";
    public string password = "";

    ////
    public UserData playerData = new UserData();
    public CharacterData character = new CharacterData();
    public FightingLeaderBoardData playerFightingData = new FightingLeaderBoardData();
    public RacingLeaderBoardData playerRacingData = new RacingLeaderBoardData();

    public List<FriendData> friendlist = new List<FriendData>();
    public List<FightingLeaderBoardData> dailyfightingLeaderBoardList = new List<FightingLeaderBoardData>();
    public List<FightingLeaderBoardData> weeklyfightingLeaderBoardList = new List<FightingLeaderBoardData>();
    public List<FightingLeaderBoardData> monthlyfightingLeaderBoardList = new List<FightingLeaderBoardData>();
    public List<FightingLeaderBoardData> yearlyfightingLeaderBoardList = new List<FightingLeaderBoardData>();
    public List<RacingLeaderBoardData> dailyracingLeaderBoardList = new List<RacingLeaderBoardData>();
    public List<RacingLeaderBoardData> weeklyracingLeaderBoardList = new List<RacingLeaderBoardData>();
    public List<RacingLeaderBoardData> montlyracingLeaderBoardList = new List<RacingLeaderBoardData>();
    public List<RacingLeaderBoardData> yearlyracingLeaderBoardList = new List<RacingLeaderBoardData>();
    public List<string> blocklist = new List<string>();
    public List<string> otherblocklist = new List<string>();

    public static ServerManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    public void GetLoginData()
    {
        SignIn(this.email, this.password, (status, errMsg) =>
        {
            //if (!usermain.IsEmailVerified)
            //{
            //    LoginGuiManager.instance.ErrorMasage("Please verify your mail.", Color.red);
            //    usermain.SendEmailVerificationAsync();
            //}
            //else
            {
                if (status == 200)
                {
                    GetLoginUpdate();
                }
                else if (status == 400)
                {
                    SignUp("", this.email, this.password);
                }
                else
                {
                    LoginGuiManager.instance.ErrorMasage(errMsg, Color.red);
                }

            }
        });
    }

    public void SignIn(string email, string password, System.Action<long, string> callback = null)
    {
        this.email = email;
        this.password = password;
        StartCoroutine(ESignIn(email, password, (status, errMsg) =>
        {
            callback(status, errMsg);
        }));
    }

    public void SignUp(string username, string email, string password, System.Action<long, string> callback = null)
    {
        StartCoroutine(ESignUp(username, email, password, (status, errMsg) =>
        {
            if (status == 200)
            {
                GetLoginUpdate();
            }
            callback(status, errMsg);
        }));
    }

    public void ChangePassword(string email, string oldpassword, string newpassword, System.Action<long, string> callback = null)
    {
        StartCoroutine(EChangePassword(email, oldpassword, newpassword, callback));
    }

    public void ResetPassword(string email, System.Action<long, string> callback = null)
    {
        StartCoroutine(EResetPassword(email, callback));
    }
    public void SignIn(System.Action<long, string> callback = null)
    {
        StartCoroutine(ESignIn(this.email, this.password, callback));
    }

    public void UpdateCharacter(string param, string value, System.Action<long, string, CharacterData> callback = null)
    {
        StartCoroutine(EUpdateCharacter(param, value, (status, errMsg, character) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EUpdateCharacter(param, value, callback));
                        }
                        else
                        {
                            callback(status, errMsg, character);
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(status, errMsg, character);
                }

            }
        }));
    }

    public void GetData()
    {
        GetFriends();
        GetFightLeaderBoards();
        GetRaceLeaderBoards();
        //GetBlockList();
    }

    public void UpdateReport(string dest, string report)
    {
//#if !UNITY_WEBGL
//        IDictionary<string, object> datalist = new Dictionary<string, object>();
//#else
//        IDictionary<string, string> datalist = new Dictionary<string, string>();
//#endif
//        datalist.Add("Email", FireBaseLogin.instance.playerData.email);
//        datalist.Add("Report", report);
//#if !UNITY_WEBGL
//        userReportmain.Child(dest).Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).SetValueAsync(datalist);
//#else
//        Debug.Log("userReport "+ dest + report);
//        FirebaseDatabase.PostJSON(userReport + "/" + dest+"/"+ RemoveSpecialCharacters(useremail), StringSerializationAPI.Serialize(typeof(Dictionary<string, string>), datalist), gameObject.name, "", "");
//#endif
//        Debug.Log("Check for report " + dest + " " + report);
    }

    public void UpdateBlockList()
    {
//#if !UNITY_WEBGL
//        IDictionary<string, object> datalist = new Dictionary<string, object>();
//#else
//        IDictionary<string, string> datalist = new Dictionary<string, string>();
//#endif
//        for (int i = 0; i < FireBaseLogin.instance.blocklist.Count; i++)
//        {
//            datalist.Add(RemoveSpecialCharacters(FireBaseLogin.instance.blocklist[i]), FireBaseLogin.instance.blocklist[i]);
//        }
//#if !UNITY_WEBGL

//        userBlockmain.Child(RemoveSpecialCharacters(usermain.Email)).SetValueAsync(datalist);
//#else
//        FirebaseDatabase.PostJSON(userBlock + "/" + RemoveSpecialCharacters(useremail), StringSerializationAPI.Serialize(typeof(Dictionary<string, string>), datalist), gameObject.name, "", "");
//#endif
    }

    public void GetFriends(System.Action<long, string> callback = null)
    {
        StartCoroutine(EGetFriends((status, errMsg) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EGetFriends(callback));
                        }
                        else
                        {
                            callback(status, errMsg);
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(status, errMsg);
                }

            }
        }));
    }

    public void RequsetFriend(FriendData data, System.Action<long, string> callback = null)
    {
        StartCoroutine(ERequestFriend(data, (status, errMsg) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(ERequestFriend(data, callback));
                        }
                        else
                        {
                            callback(status, errMsg);
                        }
                    });
                }
            }
            if (callback != null)
            {
                callback(status, errMsg);
            }
        }));
    }

    public void AcceptFriend(long id, bool bAccept, System.Action<long, string> callback = null)
    {
        StartCoroutine(EAcceptFriend(id, bAccept, (status, errMsg) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EAcceptFriend(id, bAccept, callback));
                        }
                        else
                        {
                            callback(status, errMsg);
                        }
                    });
                }
            }
            if (callback != null)
            {
                callback(status, errMsg);
            }
        }));
    }

    public void GetLoginUpdate()
    {
        if (playerData != null && playerData.email != "")
        {
            if (!playerData.isVerified)
            {
                LoginGuiManager.instance.ErrorMasage("You should verify your account.\nPlease check your E-mail inbox to verify.", Color.red);
                return;
            }

            if (!playerData.isApproved)
            {
                LoginGuiManager.instance.ErrorMasage("Your Account is pending aproval.\nPlease wait patiently.", Color.red);
                return;
            }

            if (playerData.isBanned)
            {
                NetworkManager.instance.Disconnect();
            }
            this.username = playerData.name;
            GetCustomCaracterList();
        }
        else
        {
            LoginGuiManager.instance.ErrorMasage("Login Failed. Try again", Color.red);
            return;
        }
    }

    public void GetCustomCaracterList()
    {
        StartCoroutine(EGetCharacter((status, errMsg, character) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EGetCharacter());
                        }
                        else
                        {
                            LoginGuiManager.instance.ErrorMasage("Login Failed. Try Again.", Color.red);
                        }
                    });
                }
                else
                {
                    LoginGuiManager.instance.ErrorMasage("Login Failed. Try Again.", Color.red);
                }
            }
            else
            {
                if (character != null)
                {
                    this.character = character;
                    Debug.LogError(character.PlayerSelect);
                    LoginGuiManager.instance.OnLoginSuccess();
                }
                else
                {
                    LoginGuiManager.instance.ErrorMasage("Failed to get character data.", Color.red);
                }

            }
        }));
    }

    public void GetFightLeaderBoards(System.Action<long, string> callback = null)
    {
        StartCoroutine(EGetFightLeaderBoard((status, errMsg) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EGetFightLeaderBoard(callback));
                        }
                        else
                        {
                            callback(status, errMsg);
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(status, errMsg);
                }

            }
        }));
    }
    public void UpdateFightingData(int damage, System.Action<long, string> callback = null)
    {
        playerFightingData.email = playerData.email;
        playerFightingData.name = playerData.name;
        playerFightingData.ValidateDamage();
        playerFightingData.UpdateDamage(damage);
        StartCoroutine(EUpdateFightLeaderBoard((status, errMsg) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EUpdateFightLeaderBoard(callback));
                        }
                        else
                        {
                            callback(status, errMsg);
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(status, errMsg);
                }

            }
        }));
    }

    public void GetRaceLeaderBoards(System.Action<long, string> callback = null)
    {
        StartCoroutine(EGetRaceLeaderBoard((status, errMsg) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EGetRaceLeaderBoard(callback));
                        }
                        else
                        {
                            callback(status, errMsg);
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(status, errMsg);
                }

            }
        }));
    }

    public void UpdateRacingData(long laptime, System.Action<long, string> callback = null)
    {
        playerRacingData.email = playerData.email;
        playerRacingData.name = playerData.name;
        playerRacingData.ValidateLapTime();
        playerRacingData.UpdateLapTime(laptime);

        StartCoroutine(EUpdateRaceLeaderBoard((status, errMsg) =>
        {
            if (status != 200)
            {
                if (status == 403) //invalid token
                {
                    SignIn((status, errMsg) =>
                    {
                        if (status == 200)
                        {
                            StartCoroutine(EUpdateRaceLeaderBoard(callback));
                        }
                        else
                        {
                            callback(status, errMsg);
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(status, errMsg);
                }

            }
        }));
    }

    private IEnumerator ESignIn(string email, string password, System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "signin";

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        PopUpHelper.instance.StartLoading();
        using (UnityWebRequest www = UnityWebRequest.Post(strUrl, form))
        {
            yield return www.SendWebRequest();

            PopUpHelper.instance.StopLoading();
            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.downloadHandler.text);
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (response.message == "" || response.message.ToLower().Trim() == "ok")
                {
                    playerData = DBHandler.ReadFromJSON<UserData>(response.result);
                }

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator ESignUp(string username, string email, string password, System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "signup";

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);

        PopUpHelper.instance.StartLoading();
        using (UnityWebRequest www = UnityWebRequest.Post(strUrl, form))
        {
            yield return www.SendWebRequest();

            PopUpHelper.instance.StopLoading();
            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.downloadHandler.text);
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (response.message == "" || response.message.ToLower().Trim() == "ok")
                {
                    playerData = DBHandler.ReadFromJSON<UserData>(response.result);
                }

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EChangePassword(string email, string oldPwd, string newPwd, System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "changePassword";

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("oldPassword", oldPwd);
        form.AddField("newPassword", newPwd);

        PopUpHelper.instance.StartLoading();
        using (UnityWebRequest www = UnityWebRequest.Post(strUrl, form))
        {
            yield return www.SendWebRequest();

            PopUpHelper.instance.StopLoading();
            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.downloadHandler.text);
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);
                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EResetPassword(string email, System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "forgotPassword";

        WWWForm form = new WWWForm();
        form.AddField("email", email);

        PopUpHelper.instance.StartLoading();
        using (UnityWebRequest www = UnityWebRequest.Post(strUrl, form))
        {
            yield return www.SendWebRequest();

            PopUpHelper.instance.StopLoading();
            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.downloadHandler.text);
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);
                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EGetCharacter(System.Action<long, string, CharacterData> callback = null)
    {
        string strUrl = baseURL + "users/getcharacter";
        CharacterData character = new CharacterData();

        WWWForm form = new WWWForm();
        form.AddField("email", playerData.email);

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);
        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.downloadHandler.text, character);
                }

            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (response.message == "" || response.message.ToLower().Trim() == "ok")
                {
                    character = DBHandler.ReadFromJSON<CharacterData>(response.result);
                    Debug.LogError(character.name);
                }

                if (callback != null)
                {
                    callback(www.responseCode, response.message, character);
                }
            }
        }
    }

    private IEnumerator EUpdateCharacter(string param, string value, System.Action<long, string, CharacterData> callback = null)
    {
        string strUrl = baseURL + "users/setcharacter";
        CharacterData character = new CharacterData();

        WWWForm form = new WWWForm();
        form.AddField("email", playerData.email);
        form.AddField("param", param);
        form.AddField("value", value);

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);
        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.downloadHandler.text, character);
                }

            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (response.message == "" || response.message.ToLower().Trim() == "ok")
                {
                    character = DBHandler.ReadFromJSON<CharacterData>(response.result);
                }

                if (callback != null)
                {
                    callback(www.responseCode, response.message, character);
                }
            }
        }
    }

    private IEnumerator EGetFriends(System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "friends/";

        WWWForm form = new WWWForm();
        form.AddField("email", playerData.email);

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);
        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.downloadHandler.text);
                }

            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (response.message == "" || response.message.ToLower().Trim() == "ok")
                {
                    this.friendlist = DBHandler.ReadListFromJSON<FriendData>(response.result);
                    Debug.LogError(this.friendlist.Count);
                }

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator ERequestFriend(FriendData data, System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "friends/request/";

        WWWForm form = new WWWForm();
        form.AddField("sender", data.sender);
        form.AddField("receiver", data.receiver);

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);

        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.error.ToString());
                }

            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EAcceptFriend(long id, bool bAccept, System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "friends/approve/";

        WWWForm form = new WWWForm();
        form.AddField("id", id.ToString());
        form.AddField("accept", bAccept.ToString());

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);

        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.error.ToString());
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EGetFightLeaderBoard(System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "data/lb/fight/";

        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);

        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.error.ToString());
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (response.message == "" || response.message.ToLower().Trim() == "ok")
                {
                    List<FightingLeaderBoardData> fData = DBHandler.ReadListFromJSON<FightingLeaderBoardData>(response.result);
                }

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EUpdateFightLeaderBoard(System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "data/lb/fight/update/";

        WWWForm form = new WWWForm();
        form.AddField("email", playerFightingData.email);
        form.AddField("name", playerFightingData.name);
        form.AddField("damage", playerFightingData.damage);
        form.AddField("ddamage", playerFightingData.dailydamage);
        form.AddField("wdamage", playerFightingData.weeklydamage);
        form.AddField("mdamage", playerFightingData.monthlydamage);
        form.AddField("ydamage", playerFightingData.yearlydamage);
        form.AddField("updateTime", playerFightingData.updateTime.ToString());
        form.AddField("deathTime", playerFightingData.deathTime.ToString());

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);

        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.error.ToString());
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EGetRaceLeaderBoard(System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "data/lb/race/";

        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);

        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.error.ToString());
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (response.message == "" || response.message.ToLower().Trim() == "ok")
                {
                    List<RacingLeaderBoardData> fData = DBHandler.ReadListFromJSON<RacingLeaderBoardData>(response.result);
                    Debug.LogError("RaceLeaderBoardData:>>>" + fData.Count);
                }

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }

    private IEnumerator EUpdateRaceLeaderBoard(System.Action<long, string> callback = null)
    {
        string strUrl = baseURL + "data/lb/race/update/";

        WWWForm form = new WWWForm();
        form.AddField("email", playerRacingData.email);
        form.AddField("name", playerRacingData.name);
        form.AddField("laptime", playerRacingData.laptime.ToString());
        form.AddField("dlaptime", playerRacingData.dailylaptime.ToString());
        form.AddField("wlaptime", playerRacingData.weeklylaptime.ToString());
        form.AddField("mlaptime", playerRacingData.monthlylaptime.ToString());
        form.AddField("ylaptime", playerRacingData.yearlylaptime.ToString());
        form.AddField("updateTime", playerRacingData.updateTime.ToString());

        UnityWebRequest www = UnityWebRequest.Post(strUrl, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer " + playerData.token);

        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    callback(www.responseCode, www.error.ToString());
                }
            }
            else
            {
                WWWResponse response = new WWWResponse(www.downloadHandler.text);

                if (callback != null)
                {
                    callback(www.responseCode, response.message);
                }
            }
        }
    }
}

public class CharacterData
{
    public string name = "";
    public string FemaleSettingsAccessory = "0";
    public string FemaleSettingsAngry = "0";
    public string FemaleSettingsBackpackOffset = "0";
    public string FemaleSettingsBeard = "0";
    public string FemaleSettingsBreastSize = "0";
    public string FemaleSettingsBrow_Height = "0";
    public string FemaleSettingsBrow_Length = "0";
    public string FemaleSettingsBrow_Shape = "0";
    public string FemaleSettingsBrow_Thickness = "0";
    public string FemaleSettingsCheek_Size = "0";
    public string FemaleSettingsChin_Form = "0";
    public string FemaleSettingsChin_Offset = "0";
    public string FemaleSettingsChin_Width = "0";
    public string FemaleSettingsEar_Angle = "0";
    public string FemaleSettingsEar_Size = "0";
    public string FemaleSettingsEye_Close = "0";
    public string FemaleSettingsEye_Corner = "0";
    public string FemaleSettingsEye_Form = "0";
    public string FemaleSettingsEye_Height = "0";
    public string FemaleSettingsEye_InnerCorner = "0";
    public string FemaleSettingsEye_Offset = "0";
    public string FemaleSettingsEye_OffsetH = "0";
    public string FemaleSettingsEye_Rotation = "0";
    public string FemaleSettingsEye_ScaleX = "0";
    public string FemaleSettingsEye_ScaleY = "0";
    public string FemaleSettingsEye_Size = "0";
    public string FemaleSettingsEye_Width = "0";
    public string FemaleSettingsEyeb = "-1";
    public string FemaleSettingsEyeg = "-1";
    public string FemaleSettingsEyer = "-1";
    public string FemaleSettingsFace_Form = "0";
    public string FemaleSettingsFat = "0";
    public string FemaleSettingsHair = "0";
    public string FemaleSettingsHairb = "-1";
    public string FemaleSettingsHairg = "-1";
    public string FemaleSettingsHairr = "-1";
    public string FemaleSettingsHat = "0";
    public string FemaleSettingsHead_Offset = "0";
    public string FemaleSettingsItem1 = "0";
    public string FemaleSettingsJaw_Offset = "0";
    public string FemaleSettingsJaw_Shift = "0";
    public string FemaleSettingsJaw_Width = "0";
    public string FemaleSettingsLipsCorners_Offset = "0";
    public string FemaleSettingsMouth_Bulging = "0";
    public string FemaleSettingsMouth_Offset = "0";
    public string FemaleSettingsMouth_Open = "0";
    public string FemaleSettingsMouth_Size = "0";
    public string FemaleSettingsMouth_Width = "0";
    public string FemaleSettingsNeck_Width = "0";
    public string FemaleSettingsNose_Angle = "0";
    public string FemaleSettingsNose_Bridge = "0";
    public string FemaleSettingsNose_Hump = "0";
    public string FemaleSettingsNose_Length = "0";
    public string FemaleSettingsNose_Offset = "0";
    public string FemaleSettingsNose_Size = "0";
    public string FemaleSettingsOralCavityb = "-1";
    public string FemaleSettingsOralCavityg = "-1";
    public string FemaleSettingsOralCavityr = "-1";
    public string FemaleSettingsPants = "0";
    public string FemaleSettingsSadness = "0";
    public string FemaleSettingsShirt = "0";
    public string FemaleSettingsShoes = "0";
    public string FemaleSettingsSkinb = "-1";
    public string FemaleSettingsSking = "-1";
    public string FemaleSettingsSkinr = "-1";
    public string FemaleSettingsSlimness = "0";
    public string FemaleSettingsSmile = "0";
    public string FemaleSettingsSurprise = "0";
    public string FemaleSettingsTeethb = "-1";
    public string FemaleSettingsTeethg = "-1";
    public string FemaleSettingsTeethr = "-1";
    public string FemaleSettingsThoughtful = "0";
    public string FemaleSettingsUnderpantsb = "-1";
    public string FemaleSettingsUnderpantsg = "-1";
    public string FemaleSettingsUnderpantsr = "-1";
    public string FemaleSettingsheadSizeValue = "0";
    public string FemaleSettingsheightValue = "0";
    public string MaleSettingsAccessory = "0";
    public string MaleSettingsAngry = "0";
    public string MaleSettingsBackpackOffset = "0";
    public string MaleSettingsBeard = "0";
    public string MaleSettingsBrow_Height = "0";
    public string MaleSettingsBrow_Length = "0";
    public string MaleSettingsBrow_Shape = "0";
    public string MaleSettingsBrow_Thickness = "0";
    public string MaleSettingsCheek_Size = "0";
    public string MaleSettingsChin_Form = "0";
    public string MaleSettingsChin_Offset = "0";
    public string MaleSettingsChin_Width = "0";
    public string MaleSettingsEar_Angle = "0";
    public string MaleSettingsEar_Size = "0";
    public string MaleSettingsEye_Close = "0";
    public string MaleSettingsEye_Corner = "0";
    public string MaleSettingsEye_Form = "0";
    public string MaleSettingsEye_Height = "0";
    public string MaleSettingsEye_InnerCorner = "0";
    public string MaleSettingsEye_Offset = "0";
    public string MaleSettingsEye_OffsetH = "0";
    public string MaleSettingsEye_Rotation = "0";
    public string MaleSettingsEye_ScaleX = "0";
    public string MaleSettingsEye_ScaleY = "0";
    public string MaleSettingsEye_Size = "0";
    public string MaleSettingsEye_Width = "0";
    public string MaleSettingsEyeb = "-1";
    public string MaleSettingsEyeg = "-1";
    public string MaleSettingsEyer = "-1";
    public string MaleSettingsFace_Form = "0";
    public string MaleSettingsFat = "0";
    public string MaleSettingsHair = "0";
    public string MaleSettingsHairb = "-1";
    public string MaleSettingsHairg = "-1";
    public string MaleSettingsHairr = "-1";
    public string MaleSettingsHat = "0";
    public string MaleSettingsHead_Offset = "0";
    public string MaleSettingsItem1 = "0";
    public string MaleSettingsJaw_Offset = "0";
    public string MaleSettingsJaw_Shift = "0";
    public string MaleSettingsJaw_Width = "0";
    public string MaleSettingsLipsCorners_Offset = "0";
    public string MaleSettingsMouth_Bulging = "0";
    public string MaleSettingsMouth_Offset = "0";
    public string MaleSettingsMouth_Open = "0";
    public string MaleSettingsMouth_Size = "0";
    public string MaleSettingsMouth_Width = "0";
    public string MaleSettingsMuscles = "0";
    public string MaleSettingsNeck_Width = "0";
    public string MaleSettingsNose_Angle = "0";
    public string MaleSettingsNose_Bridge = "0";
    public string MaleSettingsNose_Hump = "0";
    public string MaleSettingsNose_Length = "0";
    public string MaleSettingsNose_Offset = "0";
    public string MaleSettingsNose_Size = "0";
    public string MaleSettingsOralCavityb = "-1";
    public string MaleSettingsOralCavityg = "-1";
    public string MaleSettingsOralCavityr = "-1";
    public string MaleSettingsPants = "0";
    public string MaleSettingsSadness = "0";
    public string MaleSettingsShirt = "0";
    public string MaleSettingsShoes = "0";
    public string MaleSettingsSkinb = "-1";
    public string MaleSettingsSking = "-1";
    public string MaleSettingsSkinr = "-1";
    public string MaleSettingsSmile = "0";
    public string MaleSettingsSurprise = "0";
    public string MaleSettingsTeethb = "-1";
    public string MaleSettingsTeethg = "-1";
    public string MaleSettingsTeethr = "-1";
    public string MaleSettingsThin = "0";
    public string MaleSettingsThoughtful = "0";
    public string MaleSettingsUnderpantsb = "-1";
    public string MaleSettingsUnderpantsg = "-1";
    public string MaleSettingsUnderpantsr = "-1";
    public string MaleSettingsheadSizeValue = "0";
    public string MaleSettingsheightValue = "0";
    public string PlayerSelect = "0";

    public CharacterData()
    {

    }

    public void UpdateCharacterWithHeight(CharacterSettings setting, string value)
    {
        if (setting.name == "MaleSettings")
        {
            MaleSettingsheightValue = value;
            
        }
        else
        {
            FemaleSettingsheightValue = value;
        }
        ServerManager.instance.UpdateCharacter(setting.name + "heightValue", value);
    }

    public void UpdateCharacterWithHeadSize(CharacterSettings setting, string value)
    {
        if (setting.name == "MaleSettings")
        {
            MaleSettingsheadSizeValue = value;
        }
        else
        {
            FemaleSettingsheadSizeValue = value;
        }
        ServerManager.instance.UpdateCharacter(setting.name + "headSizeValue", value);
    }

    public void UpdateCharacterPlayer(string value)
    {
        PlayerSelect = value;
        ServerManager.instance.UpdateCharacter("PlayerSelect", value);
    }
    public void UpdateCharacter(CharacterSettings setting, BodyColorPart part, Color color)
    {
        if (setting.name == "MaleSettings")
        {
            switch (part)
            {
                case BodyColorPart.Skin:
                    MaleSettingsSkinr = color.r.ToString();
                    MaleSettingsSking = color.g.ToString();
                    MaleSettingsSkinb = color.b.ToString();
                    break;
                case BodyColorPart.Eye:
                    MaleSettingsEyer = color.r.ToString();
                    MaleSettingsEyeg = color.g.ToString();
                    MaleSettingsEyeb = color.b.ToString();
                    break;
                case BodyColorPart.Hair:
                    MaleSettingsHairr = color.r.ToString();
                    MaleSettingsHairg = color.g.ToString();
                    MaleSettingsHairb = color.b.ToString();
                    break;
                case BodyColorPart.Underpants:
                    MaleSettingsUnderpantsr = color.r.ToString();
                    MaleSettingsUnderpantsg = color.g.ToString();
                    MaleSettingsUnderpantsb = color.b.ToString();
                    break;
                case BodyColorPart.OralCavity:
                    MaleSettingsOralCavityr = color.r.ToString();
                    MaleSettingsOralCavityg = color.g.ToString();
                    MaleSettingsOralCavityb = color.b.ToString();
                    break;
                case BodyColorPart.Teeth:
                    MaleSettingsTeethr = color.r.ToString();
                    MaleSettingsTeethg = color.g.ToString();
                    MaleSettingsTeethb = color.b.ToString();
                    break;
            }
        }
        else
        {
            switch (part)
            {
                case BodyColorPart.Skin:
                    FemaleSettingsSkinr = color.r.ToString();
                    FemaleSettingsSking = color.g.ToString();
                    FemaleSettingsSkinb = color.b.ToString();
                    break;
                case BodyColorPart.Eye:
                    FemaleSettingsEyer = color.r.ToString();
                    FemaleSettingsEyeg = color.g.ToString();
                    FemaleSettingsEyeb = color.b.ToString();
                    break;
                case BodyColorPart.Hair:
                    FemaleSettingsHairr = color.r.ToString();
                    FemaleSettingsHairg = color.g.ToString();
                    FemaleSettingsHairb = color.b.ToString();
                    break;
                case BodyColorPart.Underpants:
                    FemaleSettingsUnderpantsr = color.r.ToString();
                    FemaleSettingsUnderpantsg = color.g.ToString();
                    FemaleSettingsUnderpantsb = color.b.ToString();
                    break;
                case BodyColorPart.OralCavity:
                    FemaleSettingsOralCavityr = color.r.ToString();
                    FemaleSettingsOralCavityg = color.g.ToString();
                    FemaleSettingsOralCavityb = color.b.ToString();
                    break;
                case BodyColorPart.Teeth:
                    FemaleSettingsTeethr = color.r.ToString();
                    FemaleSettingsTeethg = color.g.ToString();
                    FemaleSettingsTeethb = color.b.ToString();
                    break;
            }
        }

        ServerManager.instance.UpdateCharacter(setting.name + part.ToString() + "r", color.r.ToString());
        ServerManager.instance.UpdateCharacter(setting.name + part.ToString() + "g", color.g.ToString());
        ServerManager.instance.UpdateCharacter(setting.name + part.ToString() + "b", color.b.ToString());
    }

    public void UpdateCharacter(CharacterSettings setting, CharacterElementType type, string value)
    {
        if (setting.name == "MaleSettings")
        {
            switch (type)
            {
                case CharacterElementType.Hat:
                    MaleSettingsHat = value;
                    break;
                case CharacterElementType.Shirt:
                    MaleSettingsShirt = value;
                    break;
                case CharacterElementType.Pants:
                    MaleSettingsPants = value;
                    break;
                case CharacterElementType.Shoes:
                    MaleSettingsShoes = value;
                    break;
                case CharacterElementType.Accessory:
                    MaleSettingsAccessory = value;
                    break;
                case CharacterElementType.Hair:
                    MaleSettingsHair = value;
                    break;
                case CharacterElementType.Beard:
                    MaleSettingsBeard = value;
                    break;
                case CharacterElementType.Item1:
                    MaleSettingsItem1 = value;
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case CharacterElementType.Hat:
                    FemaleSettingsHat = value;
                    break;
                case CharacterElementType.Shirt:
                    FemaleSettingsShirt = value;
                    break;
                case CharacterElementType.Pants:
                    FemaleSettingsPants = value;
                    break;
                case CharacterElementType.Shoes:
                    FemaleSettingsShoes = value;
                    break;
                case CharacterElementType.Accessory:
                    FemaleSettingsAccessory = value;
                    break;
                case CharacterElementType.Hair:
                    FemaleSettingsHair = value;
                    break;
                case CharacterElementType.Beard:
                    FemaleSettingsBeard = value;
                    break;
                case CharacterElementType.Item1:
                    FemaleSettingsItem1 = value;
                    break;
            }
        }
        ServerManager.instance.UpdateCharacter(setting.name + type.ToString(), value);
    }
    public void UpdateCharacter(CharacterSettings setting, CharacterBlendShapeType type, string value)
    {
        if (setting.name == "MaleSettings")
        {
            switch (type)
            {
                case CharacterBlendShapeType.Fat:
                    MaleSettingsFat = value;
                    break;
                case CharacterBlendShapeType.Muscles:
                    MaleSettingsMuscles = value;
                    break;
                case CharacterBlendShapeType.Thin:
                    MaleSettingsThin = value;
                    break;
                case CharacterBlendShapeType.Neck_Width:
                    MaleSettingsNeck_Width = value;
                    break;
                case CharacterBlendShapeType.Ear_Size:
                    MaleSettingsEar_Size = value;
                    break;
                case CharacterBlendShapeType.Ear_Angle:
                    MaleSettingsEar_Angle = value;
                    break;
                case CharacterBlendShapeType.Jaw_Width:
                    MaleSettingsJaw_Width = value;
                    break;
                case CharacterBlendShapeType.Jaw_Offset:
                    MaleSettingsJaw_Offset = value;
                    break;
                case CharacterBlendShapeType.Jaw_Shift:
                    MaleSettingsJaw_Shift = value;
                    break;
                case CharacterBlendShapeType.Cheek_Size:
                    MaleSettingsCheek_Size = value;
                    break;
                case CharacterBlendShapeType.Chin_Offset:
                    MaleSettingsChin_Offset = value;
                    break;
                case CharacterBlendShapeType.Eye_Width:
                    MaleSettingsEye_Width = value;
                    break;
                case CharacterBlendShapeType.Eye_Form:
                    MaleSettingsEye_Form = value;
                    break;
                case CharacterBlendShapeType.Eye_InnerCorner:
                    MaleSettingsEye_InnerCorner = value;
                    break;
                case CharacterBlendShapeType.Eye_Corner:
                    MaleSettingsEye_Corner = value;
                    break;
                case CharacterBlendShapeType.Eye_Rotation:
                    MaleSettingsEye_Rotation = value;
                    break;
                case CharacterBlendShapeType.Eye_Offset:
                    MaleSettingsEye_Offset = value;
                    break;
                case CharacterBlendShapeType.Eye_OffsetH:
                    MaleSettingsEye_OffsetH = value;
                    break;
                case CharacterBlendShapeType.Eye_ScaleX:
                    MaleSettingsEye_ScaleX = value;
                    break;
                case CharacterBlendShapeType.Eye_ScaleY:
                    MaleSettingsEye_ScaleY = value;
                    break;
                case CharacterBlendShapeType.Eye_Size:
                    MaleSettingsEye_Size = value;
                    break;
                case CharacterBlendShapeType.Eye_Close:
                    MaleSettingsEye_Close = value;
                    break;
                case CharacterBlendShapeType.Eye_Height:
                    MaleSettingsEye_Height = value;
                    break;
                case CharacterBlendShapeType.Brow_Height:
                    MaleSettingsBrow_Height = value;
                    break;
                case CharacterBlendShapeType.Brow_Shape:
                    MaleSettingsBrow_Shape = value;
                    break;
                case CharacterBlendShapeType.Brow_Thickness:
                    MaleSettingsBrow_Thickness = value;
                    break;
                case CharacterBlendShapeType.Brow_Length:
                    MaleSettingsBrow_Length = value;
                    break;
                case CharacterBlendShapeType.Nose_Length:
                    MaleSettingsNose_Length = value;
                    break;
                case CharacterBlendShapeType.Nose_Size:
                    MaleSettingsNose_Size = value;
                    break;
                case CharacterBlendShapeType.Nose_Angle:
                    MaleSettingsNose_Angle = value;
                    break;
                case CharacterBlendShapeType.Nose_Offset:
                    MaleSettingsNose_Offset = value;
                    break;
                case CharacterBlendShapeType.Nose_Bridge:
                    MaleSettingsNose_Bridge = value;
                    break;
                case CharacterBlendShapeType.Nose_Hump:
                    MaleSettingsNose_Hump = value;
                    break;
                case CharacterBlendShapeType.Mouth_Offset:
                    MaleSettingsMouth_Offset = value;
                    break;
                case CharacterBlendShapeType.Mouth_Width:
                    MaleSettingsMouth_Width = value;
                    break;
                case CharacterBlendShapeType.Mouth_Size:
                    MaleSettingsMouth_Size = value;
                    break;
                case CharacterBlendShapeType.Mouth_Open:
                    MaleSettingsMouth_Open = value;
                    break;
                case CharacterBlendShapeType.Mouth_Bulging:
                    MaleSettingsMouth_Bulging = value;
                    break;
                case CharacterBlendShapeType.LipsCorners_Offset:
                    MaleSettingsLipsCorners_Offset = value;
                    break;
                case CharacterBlendShapeType.Face_Form:
                    MaleSettingsFace_Form = value;
                    break;
                case CharacterBlendShapeType.Chin_Width:
                    MaleSettingsChin_Width = value;
                    break;
                case CharacterBlendShapeType.Chin_Form:
                    MaleSettingsChin_Form = value;
                    break;
                case CharacterBlendShapeType.Head_Offset:
                    MaleSettingsHead_Offset = value;
                    break;
                case CharacterBlendShapeType.Smile:
                    MaleSettingsSmile = value;
                    break;
                case CharacterBlendShapeType.Sadness:
                    MaleSettingsSadness = value;
                    break;
                case CharacterBlendShapeType.Surprise:
                    MaleSettingsSurprise = value;
                    break;
                case CharacterBlendShapeType.Thoughtful:
                    MaleSettingsThoughtful = value;
                    break;
                case CharacterBlendShapeType.Angry:
                    MaleSettingsAngry = value;
                    break;
                case CharacterBlendShapeType.BackpackOffset:
                    MaleSettingsBackpackOffset = value;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case CharacterBlendShapeType.Fat:
                    FemaleSettingsFat = value;
                    break;
                case CharacterBlendShapeType.Slimness:
                    FemaleSettingsSlimness = value;
                    break;
                case CharacterBlendShapeType.BreastSize:
                    FemaleSettingsBreastSize = value;
                    break;
                case CharacterBlendShapeType.Neck_Width:
                    FemaleSettingsNeck_Width = value;
                    break;
                case CharacterBlendShapeType.Ear_Size:
                    FemaleSettingsEar_Size = value;
                    break;
                case CharacterBlendShapeType.Ear_Angle:
                    FemaleSettingsEar_Angle = value;
                    break;
                case CharacterBlendShapeType.Jaw_Width:
                    FemaleSettingsJaw_Width = value;
                    break;
                case CharacterBlendShapeType.Jaw_Offset:
                    FemaleSettingsJaw_Offset = value;
                    break;
                case CharacterBlendShapeType.Jaw_Shift:
                    FemaleSettingsJaw_Shift = value;
                    break;
                case CharacterBlendShapeType.Cheek_Size:
                    FemaleSettingsCheek_Size = value;
                    break;
                case CharacterBlendShapeType.Chin_Offset:
                    FemaleSettingsChin_Offset = value;
                    break;
                case CharacterBlendShapeType.Eye_Width:
                    FemaleSettingsEye_Width = value;
                    break;
                case CharacterBlendShapeType.Eye_Form:
                    FemaleSettingsEye_Form = value;
                    break;
                case CharacterBlendShapeType.Eye_InnerCorner:
                    FemaleSettingsEye_InnerCorner = value;
                    break;
                case CharacterBlendShapeType.Eye_Corner:
                    FemaleSettingsEye_Corner = value;
                    break;
                case CharacterBlendShapeType.Eye_Rotation:
                    FemaleSettingsEye_Rotation = value;
                    break;
                case CharacterBlendShapeType.Eye_Offset:
                    FemaleSettingsEye_Offset = value;
                    break;
                case CharacterBlendShapeType.Eye_OffsetH:
                    FemaleSettingsEye_OffsetH = value;
                    break;
                case CharacterBlendShapeType.Eye_ScaleX:
                    FemaleSettingsEye_ScaleX = value;
                    break;
                case CharacterBlendShapeType.Eye_ScaleY:
                    FemaleSettingsEye_ScaleY = value;
                    break;
                case CharacterBlendShapeType.Eye_Size:
                    FemaleSettingsEye_Size = value;
                    break;
                case CharacterBlendShapeType.Eye_Close:
                    FemaleSettingsEye_Close = value;
                    break;
                case CharacterBlendShapeType.Eye_Height:
                    FemaleSettingsEye_Height = value;
                    break;
                case CharacterBlendShapeType.Brow_Height:
                    FemaleSettingsBrow_Height = value;
                    break;
                case CharacterBlendShapeType.Brow_Shape:
                    FemaleSettingsBrow_Shape = value;
                    break;
                case CharacterBlendShapeType.Brow_Thickness:
                    FemaleSettingsBrow_Thickness = value;
                    break;
                case CharacterBlendShapeType.Brow_Length:
                    FemaleSettingsBrow_Length = value;
                    break;
                case CharacterBlendShapeType.Nose_Length:
                    FemaleSettingsNose_Length = value;
                    break;
                case CharacterBlendShapeType.Nose_Size:
                    FemaleSettingsNose_Size = value;
                    break;
                case CharacterBlendShapeType.Nose_Angle:
                    FemaleSettingsNose_Angle = value;
                    break;
                case CharacterBlendShapeType.Nose_Offset:
                    FemaleSettingsNose_Offset = value;
                    break;
                case CharacterBlendShapeType.Nose_Bridge:
                    FemaleSettingsNose_Bridge = value;
                    break;
                case CharacterBlendShapeType.Nose_Hump:
                    FemaleSettingsNose_Hump = value;
                    break;
                case CharacterBlendShapeType.Mouth_Offset:
                    FemaleSettingsMouth_Offset = value;
                    break;
                case CharacterBlendShapeType.Mouth_Width:
                    FemaleSettingsMouth_Width = value;
                    break;
                case CharacterBlendShapeType.Mouth_Size:
                    FemaleSettingsMouth_Size = value;
                    break;
                case CharacterBlendShapeType.Mouth_Open:
                    FemaleSettingsMouth_Open = value;
                    break;
                case CharacterBlendShapeType.Mouth_Bulging:
                    FemaleSettingsMouth_Bulging = value;
                    break;
                case CharacterBlendShapeType.LipsCorners_Offset:
                    FemaleSettingsLipsCorners_Offset = value;
                    break;
                case CharacterBlendShapeType.Face_Form:
                    FemaleSettingsFace_Form = value;
                    break;
                case CharacterBlendShapeType.Chin_Width:
                    FemaleSettingsChin_Width = value;
                    break;
                case CharacterBlendShapeType.Chin_Form:
                    FemaleSettingsChin_Form = value;
                    break;
                case CharacterBlendShapeType.Head_Offset:
                    FemaleSettingsHead_Offset = value;
                    break;
                case CharacterBlendShapeType.Smile:
                    FemaleSettingsSmile = value;
                    break;
                case CharacterBlendShapeType.Sadness:
                    FemaleSettingsSadness = value;
                    break;
                case CharacterBlendShapeType.Surprise:
                    FemaleSettingsSurprise = value;
                    break;
                case CharacterBlendShapeType.Thoughtful:
                    FemaleSettingsThoughtful = value;
                    break;
                case CharacterBlendShapeType.Angry:
                    FemaleSettingsAngry = value;
                    break;
                case CharacterBlendShapeType.BackpackOffset:
                    FemaleSettingsBackpackOffset = value;
                    break;
                default:
                    break;
            }
        }
        ServerManager.instance.UpdateCharacter(setting.name + type.ToString(), value);
    }
}

public class WWWResponse
{
    public string message = "";
    public string result = "";

    public WWWResponse(string str)
    {
        var response = DBHandler.ReadFromJSON<dynamic>(str);
        message = (string)(response["message"]);
        result = string.Format("{0}", response["result"]);
    }
}

public class DBHandler
{
    public static T ReadFromJSON<T>(string strJSON)
    {
        if (string.IsNullOrEmpty(strJSON))
        {
            return default(T);
        }

        T obj = JsonConvert.DeserializeObject<T>(strJSON);
        return obj;
    }

    public static List<T> ReadListFromJSON<T>(string strJSON)
    {
        List<T> list = new List<T>();

        if (string.IsNullOrEmpty(strJSON))
        {
            return list;
        }

        var resultDynamics = JsonConvert.DeserializeObject<dynamic>(strJSON);

        foreach (var item in resultDynamics)
        {
            T obj = ReadFromJSON<T>(string.Format("{0}", item));
            if (obj != null)
            {
                list.Add(obj);
            }
        }

        return list;
    }

    public static T[] FromJSON<T>(string strJSON)
    {
        Wrapper<T> wraper = JsonConvert.DeserializeObject<Wrapper<T>>(strJSON);
        return wraper.items;
    }

    public class Wrapper<T>
    {
        public T[] items;
    }
}


public class UserData
{
    public long loginvalue = DateTime.Now.Ticks;
    public string email = "";
    public string name = "";
    public bool isAdmin = false;
    public bool isBanned = false;
    public bool isApproved = false;
    public bool isVerified = false;
    public string token = "";

    public UserData()
    {

    }

    public UserData(IDictionary<string, object> dict)
    {

        Serialize(dict);

    }
    public IDictionary<string, object> Deserialize()
    {
        IDictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("loginvalue", loginvalue);
        dict.Add("email", email);
        dict.Add("name", name);
        dict.Add("isAdmin", isAdmin);
        dict.Add("isBanned", isBanned);
        dict.Add("isApproved", isApproved);
        dict.Add("isVerified", isVerified);
        dict.Add("token", token);
        return dict;
    }

    public void Serialize(IDictionary<string, object> dict)
    {

        if (dict.TryGetValue("loginvalue", out object objloginvalue))
        {
            loginvalue = (long)objloginvalue;
        }
        if (dict["email"] != null)
        {
            email = dict["email"].ToString();
        }
        if (dict.TryGetValue("name", out object objname))
        {
            name = (string)objname;
        }
        if (dict.TryGetValue("isAdmin", out object objisAdmin))
        {
            isAdmin = (bool)objisAdmin;
        }
        if (dict.TryGetValue("isBanned", out object objisBanned))
        {
            isBanned = (bool)objisBanned;
        }
        if (dict.TryGetValue("isApproved", out object objisApproved))
        {
            isApproved = (bool)objisApproved;
        }
        if (dict.TryGetValue("isVerified", out object objisVerified))
        {
            isVerified = (bool)objisVerified;
        }
        if (dict.TryGetValue("token", out object objtoken))
        {
            token = (string)objtoken;
        }
        Debug.Log("HERE3");
    }
}


public class FriendData
{
    public long _id = 0;
    public string sender = "";
    public string receiver = "";
    public bool isAproved = false;
    public FriendData()
    {

    }

    public FriendData(IDictionary<string, object> dict)
    {
        Serialize(dict);
    }
    public IDictionary<string, object> Deserialize()
    {
        IDictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("sender", sender);
        dict.Add("receiver", receiver);
        dict.Add("isAproved", isAproved);
        dict.Add("_id", _id);
        return dict;
    }

    public void Serialize(IDictionary<string, object> dict)
    {
        if (dict.TryGetValue("_id", out object objId))
        {
            _id = (long)objId;
        }
        if (dict.TryGetValue("sender", out object objsender))
        {
            sender = (string)objsender;
        }
        if (dict.TryGetValue("receiver", out object objreceiver))
        {
            receiver = (string)objreceiver;
        }
        if (dict.TryGetValue("isAproved", out object objisAproved))
        {
            isAproved = (bool)objisAproved;
        }
    }
}


[Serializable]
public class FightingLeaderBoardData
{
    public string email = "";
    public string name = "";
    public long updateTime = 0;
    public long deathTime = 0;
    public int dailydamage = 0;
    public int weeklydamage = 0;
    public int monthlydamage = 0;
    public int yearlydamage = 0;
    public int damage = 0;
    public Dictionary<string, object> allFightingData = new Dictionary<string, object>();
    public void ValidateDamage()
    {
        DateTime lastUpdate = new DateTime(updateTime);

        if (lastUpdate.Year != DateTime.UtcNow.Year)
        {
            dailydamage = 0;
            monthlydamage = 0;
            yearlydamage = 0;
        }

        if (lastUpdate.Month != DateTime.UtcNow.Month)
        {
            dailydamage = 0;
            monthlydamage = 0;
        }

        if (lastUpdate.Day != DateTime.UtcNow.Day)
        {
            dailydamage = 0;
        }

        if ((DateTime.UtcNow - lastUpdate).Days >= 7)
        {
            weeklydamage = 0;
        }
        else if (DateTime.UtcNow.DayOfWeek > DateTime.UtcNow.DayOfWeek)
        {
            weeklydamage = 0;
        }

    }
    public void UpdateDamage(int damage)
    {
        yearlydamage += damage;
        monthlydamage += damage;
        dailydamage += damage;
        weeklydamage += damage;
        updateTime = DateTime.UtcNow.Ticks;
    }
    public IDictionary<string, object> Deserialize()
    {
        IDictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("email", email);
        dict.Add("name", name);
        dict.Add("updateTime", updateTime);
        dict.Add("deathTime", deathTime);
        dict.Add("dailydamage", dailydamage);
        dict.Add("weeklydamage", weeklydamage);
        dict.Add("monthlydamage", monthlydamage);
        dict.Add("yearlydamage", yearlydamage);
        dict.Add("damage", damage);
        return dict;
    }

    public void Serialize(IDictionary<string, object> dict)
    {
        if (dict.TryGetValue("deathTime", out object objdeathTime))
        {
            deathTime = Convert.ToInt64(objdeathTime);
        }
        if (dict.TryGetValue("updateTime", out object objupdateTime))
        {
            updateTime = Convert.ToInt64(objupdateTime);
        }
        if (dict.TryGetValue("email", out object objemail))
        {
            email = (string)objemail;
        }
        if (dict.TryGetValue("name", out object objname))
        {
            name = (string)objname;
        }
        if (dict.TryGetValue("dailydamage", out object objdailydamage))
        {
            dailydamage = Convert.ToInt32(objdailydamage);
        }
        if (dict.TryGetValue("weeklydamage", out object objweeklydamage))
        {
            weeklydamage = Convert.ToInt32(objweeklydamage);
        }
        if (dict.TryGetValue("monthlydamage", out object objmonthlydamage))
        {
            monthlydamage = Convert.ToInt32(objmonthlydamage);
        }
        if (dict.TryGetValue("yearlydamage", out object objyearlydamage))
        {
            yearlydamage = Convert.ToInt32(objyearlydamage);
        }
        if (dict.TryGetValue("damage", out object objdamage))
        {
            damage = Convert.ToInt32(objdamage);
        }
    }
}

[Serializable]
public class RacingLeaderBoardData
{
    public static long longmaxvalue = 9223372036854775000;
    public string email = "";
    public string name = "";
    public long updateTime = 0;
    public long dailylaptime = longmaxvalue;
    public long weeklylaptime = longmaxvalue;
    public long monthlylaptime = longmaxvalue;
    public long yearlylaptime = longmaxvalue;
    public long laptime = longmaxvalue;



    public void ValidateLapTime()
    {
        DateTime lastUpdate = new DateTime(updateTime);

        if (lastUpdate.Year != DateTime.UtcNow.Year)
        {
            dailylaptime = longmaxvalue;
            monthlylaptime = longmaxvalue;
            yearlylaptime = longmaxvalue;
        }

        if (lastUpdate.Month != DateTime.UtcNow.Month)
        {
            dailylaptime = longmaxvalue;
            monthlylaptime = longmaxvalue;
        }

        if (lastUpdate.Day != DateTime.UtcNow.Day)
        {
            dailylaptime = longmaxvalue;
        }

        if ((DateTime.UtcNow - lastUpdate).Days >= 7)
        {
            weeklylaptime = longmaxvalue;
        }
        else if (DateTime.UtcNow.DayOfWeek > DateTime.UtcNow.DayOfWeek)
        {
            weeklylaptime = longmaxvalue;
        }
    }

    public void UpdateLapTime(long laptime)
    {
        if (laptime <= yearlylaptime)
        {
            yearlylaptime = laptime;
            updateTime = DateTime.UtcNow.Ticks;
        }
        if (laptime <= weeklylaptime)
        {
            weeklylaptime = laptime;
            updateTime = DateTime.UtcNow.Ticks;
        }
        if (laptime <= monthlylaptime)
        {
            monthlylaptime = laptime;
            updateTime = DateTime.UtcNow.Ticks;
        }
        if (laptime <= dailylaptime)
        {
            dailylaptime = laptime;
            updateTime = DateTime.UtcNow.Ticks;
        }
    }
    public IDictionary<string, object> Deserialize()
    {
        IDictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add("email", email);
        dict.Add("name", name);
        dict.Add("updateTime", updateTime);
        dict.Add("dailylaptime", dailylaptime);
        dict.Add("weeklylaptime", weeklylaptime);
        dict.Add("monthlylaptime", monthlylaptime);
        dict.Add("yearlylaptime", yearlylaptime);
        dict.Add("laptime", laptime);
        return dict;
    }

    public void Serialize(IDictionary<string, object> dict)
    {
        if (dict.TryGetValue("updateTime", out object objupdateTime))
        {
            updateTime = Convert.ToInt64(objupdateTime);
        }
        if (dict.TryGetValue("email", out object objemail))
        {
            email = (string)objemail;
        }
        if (dict.TryGetValue("name", out object objname))
        {
            name = (string)objname;
        }
        if (dict.TryGetValue("dailylaptime", out object objdailylaptime))
        {
            dailylaptime = Convert.ToInt64(objdailylaptime);
        }
        if (dict.TryGetValue("weeklylaptime", out object objweeklylaptime))
        {
            weeklylaptime = Convert.ToInt64(objweeklylaptime);
        }
        if (dict.TryGetValue("monthlylaptime", out object objmonthlylaptime))
        {
            monthlylaptime = Convert.ToInt64(objmonthlylaptime);
        }
        if (dict.TryGetValue("yearlylaptime", out object objyearlylaptime))
        {
            yearlylaptime = Convert.ToInt64(objyearlylaptime);
        }
        //if (dict["laptime"]!=null)
        //{
        //    laptime = Convert.ToInt64(dict["laptime"]);
        //}
    }
}

