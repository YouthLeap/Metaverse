using AdvancedPeopleSystem;
#if !UNITY_WEBGL  
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Linq;
#else
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Examples.Utils;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;


public class FirebaseData : MonoBehaviour
{
#if !UNITY_WEBGL
    public FirebaseAuth auth;
    public FirebaseDatabase databasemain;
    public DatabaseReference firebasemain;
    public DatabaseReference friendlistmain;
    public DatabaseReference caractercustomlistmain;
    public DatabaseReference userNamelistmain;
    public DatabaseReference userNamelist2main;
    public DatabaseReference userReportmain;
    public DatabaseReference userBlockmain;
    public DatabaseReference lastLoginmain;
    public FirebaseUser usermain;
    public DatabaseReference RacingLeaderBoardmain;
    public DatabaseReference FightingLeaderBoardmain;
#else
    public string useremail = "";
    public string firebase= "UserData";
    public string friendlist= "FriendList";
    public string caractercustomlist= "CaracterCustomList";
    public string userNamelist= "UserData";
    public string userNamelist2= "UserData";
    public string userReport = "ReportData";
    public string userBlock = "BlockData";
    public string lastLogin= "UserData";
    public string RacingLeaderBoard = "RacingLeaderBoardData";
    public string FightingLeaderBoard= "FightingLeaderBoardData";
#endif


    public static FirebaseData instance;

#if !UNITY_WEBGL
    public Dictionary<string, object> caractercustomdict = new Dictionary<string, object>();
#else
    public Dictionary<string, string> caractercustomdict = new Dictionary<string, string>();
#endif

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

#if !UNITY_WEBGL
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus dependencystatus = task.Result;
            if (dependencystatus == DependencyStatus.Available)
            {
                Initialize();
            }
        });
#else
            Initialize();
#endif
    }
    private void Initialize()
    {
#if !UNITY_WEBGL
        FirebaseApp app = FirebaseApp.DefaultInstance;
        auth = Firebase.Auth.FirebaseAuth.GetAuth(app);
        databasemain = FirebaseDatabase.GetInstance(app);
        firebasemain = databasemain.RootReference;
        friendlistmain = firebasemain.Child("FriendList");
        caractercustomlistmain = firebasemain.Child("CaracterCustomList");
        userNamelistmain = firebasemain.Child("UserData");
        userNamelist2main = firebasemain.Child("UserData");
        lastLoginmain = firebasemain.Child("UserData");
        userReportmain= firebasemain.Child("ReportData");
        userBlockmain= firebasemain.Child("BlockData");
        FightingLeaderBoardmain= firebasemain.Child("FightingLeaderBoardData");
        RacingLeaderBoardmain = firebasemain.Child("RacingLeaderBoardData");
        firebasemain = firebasemain.Child("UserData");
        //Debug.Log("Here");
        
#else

#endif
    }

    public IEnumerator LoginWithEmail(string email, string password)
    {
        PopUpHelper.instance.StartLoading();
#if !UNITY_WEBGL

        var LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        Debug.Log("first enter");
        
        
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        Debug.Log("Entered");
        Debug.Log(LoginTask.Exception);

        if (LoginTask.Exception != null)
        {
            PopUpHelper.instance.StopLoading();
            PopUpHelper.instance.SetMassage("Invalid Username or Password or\nAccount Doesnt Exist", Color.red);

        }
        else
        {
            usermain = LoginTask.Result;

            if (usermain != null)
            {
                if(!usermain.IsEmailVerified)
                {
                    PopUpHelper.instance.StopLoading();
                    LoginGuiManager.instance.ErrorMasage("Please verify your mail.",Color.red);
                    usermain.SendEmailVerificationAsync();
                }
                else
                {
                    
                    GetLoginUpdate(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email));
                    
                 
                }
                
            }
            else
            {

            }
        }
#else
        useremail = email;
        FirebaseAuth.SignInWithEmailAndPassword(email,password,gameObject.name, "WebLoginCallBack", "WebLoginFallBack");
        yield return null;
#endif
    }

#if UNITY_WEBGL
    public void WebLoginCallBack(string output)
    {
        Debug.Log("Login success"+ output);
        if(output.Contains("false") || output.Contains("False"))
        {
        LoginGuiManager.instance.ErrorMasage("Please verify your mail.",Color.red);
        }
        else
        {
        GetLoginUpdate(RemoveSpecialCharacters(useremail));
        }
        
    }
    
    public void WebLoginFallBack(string output)
    {
        Debug.Log("Login failed" + output);
        LoginGuiManager.instance.ErrorMasage("Invalid Username or Password or\nAccount Doesnt Exist", Color.green);
        //StartCoroutine(RegisterWithEmail("", useremail, "123456"));
    }
#endif
#if !UNITY_WEBGL
    public IEnumerator RecoverWithEmail(string email)
    {
        //var request = new SendAccountRecoveryEmailRequest { Email = email, TitleId = "D1431" };
        //PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverSuccess, OnRecoverFailure);
        
        FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(email);
        LoginGuiManager.instance.ErrorMasage("We Sent you password reset email\nPlease check your email", Color.green);
        yield return null;
    }
#endif
    public IEnumerator RegisterWithEmail(string username, string email, string password)
    {
#if !UNITY_WEBGL
        //FirebaseApp app = FirebaseApp.DefaultInstance;
        //auth = Firebase.Auth.FirebaseAuth.GetAuth(app);
        var LoginTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        Debug.Log(password);
        
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.Log(LoginTask.Exception);
        }
        else
        {
            usermain = LoginTask.Result;
            if (usermain != null)
            {
                //SetLoginData();
                LoginGuiManager.instance.ErrorMasage("Your Account is Registered.\nPlease verify your mail.", Color.green);
                UserData userData = new UserData();
                userData.email = email;
                userData.name = username;
                userData.isAdmin = false;
                userData.isBanned = false;
                userData.isApproved = true;
                lastLoginmain.Child(RemoveSpecialCharacters(email)).SetRawJsonValueAsync(JsonUtility.ToJson(userData));
                
                usermain.SendEmailVerificationAsync();
            }
            else
            {
                
            }
        }
#else
        
        FirebaseAuth.CreateUserWithEmailAndPassword(email, password,gameObject.name, "WebRegisterCallBack", "WebRegisterFallBack");
        yield return null;
#endif
    }
#if UNITY_WEBGL
    public void WebRegisterCallBack(string output)
    {
        FirebaseDatabase.PostJSON(firebase + "/" + RemoveSpecialCharacters(useremail), StringSerializationAPI.Serialize(typeof(UserData), FireBaseLogin.instance.playerData), gameObject.name, "WebSetDataCallBack", "WebSetDataFallBack");
    }
    public void WebRegisterFallBack(string output)
    {
        Debug.Log("Register failed" + output);
    }
#endif

    public static string RemoveSpecialCharacters(string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
            {
                sb.Append(c);
            }
        }
        //Debug.Log(sb.ToString());
        return sb.ToString();
    }

#if UNITY_WEBGL
    public void WebSetDataCallBack(string output)
    {
        Debug.Log("Register success"+output);
        LoginGuiManager.instance.ErrorMasage("Register Completed.\nPlease verify your mail.",Color.green);
        //GetLoginUpdate(RemoveSpecialCharacters(useremail));
    }
    public void WebSetDataFallBack(string output)
    {
        Debug.Log("Register failed" + output);
    }
#endif


    public void UpdateLoginData()
    {
        //UpdateLoginUpdate(RemoveSpecialCharacters(useremail), FireBaseLogin.instance.playerData.loginvalue);
#if !UNITY_WEBGL
        UpdateLoginUpdate(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email));
#else
        UpdateLoginUpdate(RemoveSpecialCharacters(useremail));
#endif
    }

    public void GetLoginUpdate(string dest)
    {
#if !UNITY_WEBGL
        Debug.Log("Check for login");
        firebasemain.Child(dest).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Check for login"+ task.IsFaulted);
                
            }
            else if (task.IsCompleted)
            {

                Debug.Log("Check for login" + task.IsCompleted);
                DataSnapshot snapshot = task.Result;
                Debug.Log("Check for login" + snapshot);
                Debug.Log(snapshot.Value.GetType().ToString());
                var json = JsonConvert.SerializeObject(snapshot.Value);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                //foreach (KeyValuePair<string, object> kvp in dictionary)
                //{
                //    //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                //    Debug.Log("Key = "+kvp.Key+", Value = "+kvp.Value);
                //}
                UserData data = new UserData(dictionary);
                FireBaseLogin.instance.username = dictionary["name"].ToString();
                Debug.Log(FireBaseLogin.instance.username);
                Debug.Log("here");
                Debug.Log("Check for login------------" + data);
                //Debug.Log(data);
                if (data != null)
                {

                if (!data.isApproved)
                {
                    LoginGuiManager.instance.ErrorMasage("Your Account is pending aproval.\nPlease wait patiently.",Color.red);
                    return;
                }
                    
                    FireBaseLogin.instance.playerData = data;
                    FireBaseLogin.instance.playerData.loginvalue = DateTime.Now.Ticks;
                    UpdateLoginData();
                    firebasemain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).ChildChanged+= (object sender, ChildChangedEventArgs arg) => {
                        string key = arg.Snapshot.Key;
                        if (key== "loginvalue")
                        {
                            long data = Convert.ToInt64(arg.Snapshot.Value);
                            if (data > FireBaseLogin.instance.playerData.loginvalue)
                            {
                                NetworkManager.instance.Disconnect();
                            }
                        }
                        else if (key == "isBanned")
                        {

                        if(Boolean.TryParse(arg.Snapshot.Value as string,out bool banned))
                            {
                                if(banned)
                                    NetworkManager.instance.Disconnect();
                             }
                            /*bool banned= Convert.ToBoolean(arg.Snapshot.Value as string);
                            if (banned)
                                NetworkManager.instance.Disconnect();
                            Debug.Log(banned);*/
                        }
                    };
                    GetCustomCaracterList();
                }
                else
                {

                    //FireBaseLogin.instance.playerData.loginvalue = DateTime.Now.Ticks;
                    UpdateLoginData();
                    GetCustomCaracterList();
                }
            }
        });

#else
        Debug.Log("Get login Data");
        FirebaseDatabase.GetJSON(firebase + "/" + dest, gameObject.name, "WebGetLoginUpdateCallBack", "WebGetLoginUpdateFallBack");
#endif
    }
#if UNITY_WEBGL
    public void WebGetLoginUpdateCallBack(string output)
    {
        Debug.Log("Check for login success------------" + output);
        UserData data = StringSerializationAPI.Deserialize(typeof(UserData),output) as UserData;
        Debug.Log("Check for login------------" + data);

        

        if (data != null)
        {
            if (!data.isApproved)
            {
                LoginGuiManager.instance.ErrorMasage("Your Account is pending aproval.\nPlease wait patiently.",Color.red);
                return;
            }
            FireBaseLogin.instance.playerData = data;
            FireBaseLogin.instance.username = data.name;
            FireBaseLogin.instance.playerData.loginvalue = DateTime.Now.Ticks;
            Debug.Log("Check for login------------1" + data.isAdmin);
            UpdateLoginData();
            FirebaseDatabase.ListenForChildChanged(firebase + "/" + RemoveSpecialCharacters(useremail), gameObject.name, "WebChildChangedUser", "");
            Debug.Log("Check for login------------2" + data.isAdmin);
            GetCustomCaracterList();
            Debug.Log("Check for login------------3" + data.isAdmin);
        }
        else
        {
            FireBaseLogin.instance.playerData.loginvalue = DateTime.Now.Ticks;
            UpdateLoginData();
            FirebaseDatabase.ListenForChildChanged(firebase + "/" + RemoveSpecialCharacters(useremail), gameObject.name, "WebChildChangedUser", "");
            GetCustomCaracterList();
        }
    }
    public void WebGetLoginUpdateFallBack(string output)
    {
        Debug.Log("Check for login failed------------" + output);
    }

    public void WebChildChangedUser(string output)
    {
        Debug.Log("Check for datareset 0"+output);
        try
        {
            long data = Convert.ToInt64(output);
            Debug.Log("Check for datareset 1");
            Debug.Log("Check for datareset " + data + " " + FireBaseLogin.instance.playerData.loginvalue);
            Debug.Log("Check for datareset 2");
            if (data > FireBaseLogin.instance.playerData.loginvalue)
            {
                NetworkManager.instance.Disconnect();
            }
        }
        catch(Exception ex)
        {
            if(Boolean.TryParse(output,out bool banned))
            {
                if(banned)
                    NetworkManager.instance.Disconnect();
            }
        }
        
    }

#endif

    public void PostLoginUpdate(string dest, long log)
    {
#if !UNITY_WEBGL
        firebasemain.Child(dest).SetValueAsync(FireBaseLogin.instance.playerData.Deserialize());
#else
        
        FirebaseDatabase.PostJSON(firebase + "/" + dest, StringSerializationAPI.Serialize(typeof(UserData), FireBaseLogin.instance.playerData), gameObject.name, "", "");
#endif
    }

    public void UpdateLoginUpdate(string dest, long log)
    {
#if !UNITY_WEBGL
        firebasemain.Child(dest).UpdateChildrenAsync(FireBaseLogin.instance.playerData.Deserialize());
#else
        FirebaseDatabase.UpdateJSON(firebase + "/" + dest, StringSerializationAPI.Serialize(typeof(UserData), FireBaseLogin.instance.playerData), gameObject.name, "", "");
#endif
    }
    public void UpdateLoginUpdate(string dest)
    {
#if !UNITY_WEBGL
        firebasemain.Child(dest).UpdateChildrenAsync(FireBaseLogin.instance.playerData.Deserialize());
#else
        FirebaseDatabase.UpdateJSON(firebase + "/" + dest, StringSerializationAPI.Serialize(typeof(UserData), FireBaseLogin.instance.playerData), gameObject.name, "", "");
#endif
    }
    public void GetUserNameList()
    {
#if !UNITY_WEBGL
        userNamelistmain.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Dictionary<string, UserData> dict = snapshot.Value as Dictionary<string, UserData>;
                if (dict != null)
                    foreach (string key in dict.Keys)
                    {
                        UserData data = dict[key];
                        if (data!=null)
                            if (data.name == FireBaseLogin.instance.username)
                            {
                                LoginGuiManager.instance.ErrorMasage("This username already exist.", Color.red);
                                return;
                            }
                    }
                FirebaseData.instance.RegisterWithEmail(FireBaseLogin.instance.username, FireBaseLogin.instance.email, FireBaseLogin.instance.password);
            }
        });
#else
        FirebaseDatabase.GetJSON(userNamelist,gameObject.name, "WebGetUserNameListCallBack", "WebGetUserNameListFallBack");
#endif
    }
#if UNITY_WEBGL
    public void WebGetUserNameListCallBack(string output)
    {
        Dictionary<string, UserData> dict = StringSerializationAPI.Deserialize(typeof(Dictionary<string, UserData>), output) as Dictionary<string, UserData>;

        if (dict != null)
            foreach (string key in dict.Keys)
            {
                UserData data = dict[key];
                if (data != null)
                    if (data.name == FireBaseLogin.instance.username)
                    {
                        LoginGuiManager.instance.ErrorMasage("This username already exist.", Color.red);
                        return;
                    }
            }

        //FireBaseLogin.instance.RegisterWithEmail();
    }
    public void WebGetUserNameListFallBack(string output)
    {

    }
#endif

    public void GetUserNameListLauncher()
    {
#if !UNITY_WEBGL
        userNamelist2main.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {

            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Dictionary<string, UserData> dict = snapshot.Value as Dictionary<string, UserData>;

                if (dict != null)
                    foreach (string key in dict.Keys)
                    {
                        UserData data = dict[key];
                        if (data != null)
                            if (data.name == FireBaseLogin.instance.username)
                            {
                                LoginGuiManager.instance.ErrorMasage("This username already exist.", Color.red);
                                return;
                            }
                    }

                FirebaseData.instance.RegisterWithEmail(FireBaseLogin.instance.username, FireBaseLogin.instance.email, FireBaseLogin.instance.password);
            }
        });
#else
        FirebaseDatabase.GetJSON(userNamelist2, gameObject.name, "WebGetUserNameList2CallBack", "WebGetUserNameList2FallBack");
#endif
    }
#if UNITY_WEBGL
    public void WebGetUserNameList2CallBack(string output)
    {
        Dictionary<string, UserData> dict = StringSerializationAPI.Deserialize(typeof(Dictionary<string, UserData>), output) as Dictionary<string, UserData>;

        if (dict != null)
            foreach (string key in dict.Keys)
            {
                UserData data = dict[key];
                if (data != null)
                    if (data.name == FireBaseLogin.instance.username)
                    {
                        LoginGuiManager.instance.ErrorMasage("This username already exist.", Color.red);
                        return;
                    }
            }

        //FireBaseLogin.instance.RegisterWithEmailLauncher();
    }
    public void WebGetUserNameList2FallBack(string output)
    {

    }
#endif

    public void UpdateFightingData(int damage)
    {
#if !UNITY_WEBGL
        FireBaseLogin.instance.playerFightingData.email = FirebaseData.instance.usermain.Email;
        FireBaseLogin.instance.playerFightingData.name= FireBaseLogin.instance.playerData.name;
#else
        FireBaseLogin.instance.playerFightingData.email = useremail;
        FireBaseLogin.instance.playerFightingData.name = FireBaseLogin.instance.playerData.name;
#endif
        FireBaseLogin.instance.playerFightingData.ValidateDamage();
        FireBaseLogin.instance.playerFightingData.UpdateDamage(damage);
#if !UNITY_WEBGL
        FightingLeaderBoardmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).SetValueAsync(FireBaseLogin.instance.playerFightingData.Deserialize());
#else
        FirebaseDatabase.PostJSON(FightingLeaderBoard + "/" + RemoveSpecialCharacters(useremail), StringSerializationAPI.Serialize(typeof(FightingLeaderBoardData), FireBaseLogin.instance.playerFightingData), gameObject.name, "", "");
#endif
    }

#if UNITY_WEBGL
    public void WebFightingLeaderBoardAddedCallBack(string output)
    {
        FightingLeaderBoardData data = StringSerializationAPI.Deserialize(typeof(FightingLeaderBoardData), output) as FightingLeaderBoardData;
        string key = RemoveSpecialCharacters(data.email);
        data.AddToLeaderBoard();
    }

    public void WebFightingLeaderBoardChangedCallBack(string output)
    {
        FightingLeaderBoardData data = StringSerializationAPI.Deserialize(typeof(FightingLeaderBoardData), output) as FightingLeaderBoardData;
        string key = RemoveSpecialCharacters(data.email);
        data.UpdateToLeaderBoard(key);
    }
#endif

    public void UpdateRacingData(long laptime)
    {

        FireBaseLogin.instance.playerRacingData.email = FireBaseLogin.instance.playerData.email;
        
        FireBaseLogin.instance.playerRacingData.name = FireBaseLogin.instance.playerData.name;
        FireBaseLogin.instance.playerRacingData.ValidateLapTime();
        FireBaseLogin.instance.playerRacingData.UpdateLapTime(laptime);
#if !UNITY_WEBGL
        
        RacingLeaderBoardmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).SetValueAsync(FireBaseLogin.instance.playerRacingData.Deserialize());
#else
        FirebaseDatabase.PostJSON(RacingLeaderBoard + "/" + RemoveSpecialCharacters(useremail), StringSerializationAPI.Serialize(typeof(RacingLeaderBoardData), FireBaseLogin.instance.playerRacingData), gameObject.name, "", "");
#endif
    }

#if UNITY_WEBGL
    public void WebRacingLeaderBoardAddedCallBack(string output)
    {
        RacingLeaderBoardData data = StringSerializationAPI.Deserialize(typeof(RacingLeaderBoardData), output) as RacingLeaderBoardData;
        string key = RemoveSpecialCharacters(data.email);
        data.AddToLeaderBoard();
    }
    
    public void WebRacingLeaderBoardChangedCallBack(string output)
    {
        RacingLeaderBoardData data = StringSerializationAPI.Deserialize(typeof(RacingLeaderBoardData), output) as RacingLeaderBoardData;
        string key = RemoveSpecialCharacters(data.email);
        data.UpdateToLeaderBoard(key);
    }
#endif

    public void WatchFriendList()
    {
#if !UNITY_WEBGL
        friendlistmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).ChildAdded += (object sender, ChildChangedEventArgs arg) => {
            string key = arg.Snapshot.Key;
            FriendData data = new FriendData();
            data.Serialize((arg.Snapshot.Value as Dictionary<string, object>));
            FireBaseLogin.instance.friendlist.Add(key, data);
            
        };
        friendlistmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).ChildChanged += (object sender, ChildChangedEventArgs arg) => {
            string key = arg.Snapshot.Key;
            FriendData data = new FriendData();
            data.Serialize((arg.Snapshot.Value as Dictionary<string, object>));
            FireBaseLogin.instance.friendlist.Remove(key);
            FireBaseLogin.instance.friendlist.Add(key, data);
        };
        friendlistmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).ChildRemoved += (object sender, ChildChangedEventArgs arg) => {
            string key = arg.Snapshot.Key;
            FireBaseLogin.instance.friendlist.Remove(key);
        };
#else
        FirebaseDatabase.ListenForChildAdded(friendlist + "/" + RemoveSpecialCharacters(useremail), gameObject.name, "WebFriendChildAddedCallBack", "");
        FirebaseDatabase.ListenForChildRemoved(friendlist + "/" + RemoveSpecialCharacters(useremail), gameObject.name, "WebFriendChildRemovedCallBack", "");
        FirebaseDatabase.ListenForChildChanged(friendlist + "/" + RemoveSpecialCharacters(useremail), gameObject.name, "WebFriendChildChangedCallBack", "");
#endif

    }
#if UNITY_WEBGL
    public void WebFriendChildAddedCallBack(string output)
    {
        Debug.Log("Friend Child Added " + output);
        FriendData data = StringSerializationAPI.Deserialize(typeof(FriendData), output) as FriendData;
        string key = RemoveSpecialCharacters(data.email);
        FireBaseLogin.instance.friendlist.Add(key, data);
    }
    public void WebFriendChildRemovedCallBack(string output)
    {
        Debug.Log("Friend Child Removed " + output);
        FriendData data = StringSerializationAPI.Deserialize(typeof(FriendData), output) as FriendData;
        string key = RemoveSpecialCharacters(data.email);
        FireBaseLogin.instance.friendlist.Remove(key);
        
    }
    public void WebFriendChildChangedCallBack(string output)
    {
        Debug.Log("Friend Child Changed " + output);
        FriendData data = StringSerializationAPI.Deserialize(typeof(FriendData), output) as FriendData;
        string key = RemoveSpecialCharacters(data.email);
        FireBaseLogin.instance.friendlist.Remove(key);
        FireBaseLogin.instance.friendlist.Add(key, data);
    }
    public void WebGetFriendListFallBack(string output)
    {

    }
#endif

    public void GetBlockList()
    {
#if !UNITY_WEBGL
        userBlockmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {

            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                IDictionary<string, object> dict = snapshot.Value as IDictionary<string, object>;
                FireBaseLogin.instance.blocklist.Clear();
                if (dict != null)
                    foreach (string key in dict.Keys)
                    {
                            FireBaseLogin.instance.blocklist.Add((string)dict[key]);
                    }
            }
        });
#else
        FirebaseDatabase.GetJSON(userBlock + "/" + RemoveSpecialCharacters(useremail), gameObject.name, "WebGetBlockListCallBack", "WebGetBlockListFallBack");
#endif

    }
#if UNITY_WEBGL
    public void WebGetBlockListCallBack(string output)
    {
        IDictionary<string, string> dict =StringSerializationAPI.Deserialize(typeof(Dictionary<string, string>), output) as Dictionary<string, string>;
        FireBaseLogin.instance.blocklist.Clear();
        if (dict != null)
            foreach (string key in dict.Keys)
            {
                FireBaseLogin.instance.blocklist.Add(dict[key]);
            }

        Debug.Log("BlockList "+ FireBaseLogin.instance.blocklist.Count);
    }
    public void WebGetBlockListFallBack(string output)
    {

    }
#endif


    public void UpdateBlockList()
    {
#if !UNITY_WEBGL
        IDictionary<string, object> datalist = new Dictionary<string, object>();
#else
        IDictionary<string, string> datalist = new Dictionary<string, string>();
#endif
        for (int i = 0; i < FireBaseLogin.instance.blocklist.Count; i++)
        {
            datalist.Add(RemoveSpecialCharacters(FireBaseLogin.instance.blocklist[i]), FireBaseLogin.instance.blocklist[i]);
        }
#if !UNITY_WEBGL
        
        userBlockmain.Child(RemoveSpecialCharacters(usermain.Email)).SetValueAsync(datalist);    
#else
        FirebaseDatabase.PostJSON(userBlock + "/" + RemoveSpecialCharacters(useremail), StringSerializationAPI.Serialize(typeof(Dictionary<string, string>), datalist), gameObject.name, "", "");
#endif
    }

    public void UpdateReport(string dest,string report)
    {
#if !UNITY_WEBGL
        IDictionary<string, object> datalist = new Dictionary<string, object>();
#else
        IDictionary<string, string> datalist = new Dictionary<string, string>();
#endif
        datalist.Add("Email", FireBaseLogin.instance.playerData.email);
        datalist.Add("Report", report);
#if !UNITY_WEBGL
        userReportmain.Child(dest).Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).SetValueAsync(datalist);
#else
        Debug.Log("userReport "+ dest + report);
        FirebaseDatabase.PostJSON(userReport + "/" + dest+"/"+ RemoveSpecialCharacters(useremail), StringSerializationAPI.Serialize(typeof(Dictionary<string, string>), datalist), gameObject.name, "", "");
#endif
        Debug.Log("Check for report "+dest+" "+report);
    }

    public void GetCustomCaracterList()
    {


#if !UNITY_WEBGL
        caractercustomlistmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                LoginGuiManager.instance.ErrorMasage("Login Failed. Try Again.", Color.red);
            }
            else if (task.IsCompleted)
            {
                
                DataSnapshot snapshot = task.Result;
                caractercustomdict = snapshot.Value as Dictionary<string, object>;

                if (caractercustomdict == null)
                {
                    caractercustomdict = new Dictionary<string, object>();
                }
                
                LoginGuiManager.instance.OnLoginSuccess();
            }
        });
#else
        FirebaseDatabase.GetJSON(caractercustomlist+"/"+ RemoveSpecialCharacters(useremail),gameObject.name, "WebGetCustomCaracterListCallBack", "WebGetCustomCaracterListFallBack");
#endif
    }
#if UNITY_WEBGL
    public void WebGetCustomCaracterListCallBack(string output)
    {
        try
        {
            output = output.Substring(1,output.Length-2);

            Debug.Log(output);

            string[] outputlist = output.Split(',');
            caractercustomdict = new Dictionary<string, string>();
            foreach (string str in outputlist)
            {
                string[] templist = str.Split(':');

                try
                {
                    caractercustomdict.Add(templist[0].Replace("\"", ""), templist[1].Replace("\"", ""));
                }
                catch
                {

                }

            }

            //Dictionary<string, double> templist= StringSerializationAPI.Deserialize(typeof(Dictionary<string, double>), output) as Dictionary<string, double>;

        }
        catch
        {

        }
        Debug.Log("Check for login------------4");
        

        if (caractercustomdict == null)
        {
            caractercustomdict = new Dictionary<string, string>();
        }
        Debug.Log("Check for login------------5");
        LoginGuiManager.instance.OnLoginSuccess();
        Debug.Log("Check for login------------6");
    }
    public void WebGetCustomCaracterListFallBack(string output)
    {

    }
#endif
    public void UpdateCustomCaracterList(string dest, string property)
    {
        Debug.Log("Update "+dest+ property);
#if !UNITY_WEBGL
        caractercustomlistmain.Child(RemoveSpecialCharacters(FirebaseData.instance.usermain.Email)).Child(dest).SetValueAsync(property);
        caractercustomdict[dest] = property;
#else
        FirebaseDatabase.PostJSON(caractercustomlist+"/"+ RemoveSpecialCharacters(useremail)+"/"+dest, property,gameObject.name,"","");
        caractercustomdict[dest] = property;
#endif
    }

    public static bool NumberParse(string num,out int number)
    {
        number = 0;

        string tempnum = (string)num.Clone();

        foreach(char c in tempnum)
        {
            if ((c < '0' && c > '9'))
                return false;
        }

        foreach (char c in tempnum)
        {
            number = number * 10 + c - '0';
        }
        return true;
    }

    public static bool NumberParse(string num, out float number)
    {
        number = 0;

        string tempnum = (string)num.Clone();

        Debug.Log(tempnum +" "+ tempnum.Length+" "+num+" "+num.Length);

        foreach (char c in tempnum)
        {
            if ((c < '0' && c > '9')||c!='.')
                return false;
        }
        float count = 10;
        bool countcheck = false;
        foreach (char c in tempnum)
        {
            if(c=='.')
            {
                countcheck = true;
                continue;
            }
            if(!countcheck)
            {
                number = number * 10 + c - '0';
            }
            else
            {
                number = number + (c - '0') / count;
                count *= 10;
            }
        }
        return true;
    }

    public string GetCaracterList(string dest)
    {
        
        if (caractercustomdict.ContainsKey(dest))
        {
            /*Debug.Log(dest);
            
            Debug.Log(caractercustomdict[dest]);*/
            return caractercustomdict[dest].ToString();

        }
        else
        {
            
            if (dest.Contains(BodyColorPart.Eye.ToString() + "r") || dest.Contains(BodyColorPart.Eye.ToString() + "g") || dest.Contains(BodyColorPart.Eye.ToString() + "b"))
                return "-1";
            else if (dest.Contains(BodyColorPart.Hair.ToString() + "r") || dest.Contains(BodyColorPart.Hair.ToString() + "g") || dest.Contains(BodyColorPart.Hair.ToString() + "b"))
                return "-1";
            else if (dest.Contains(BodyColorPart.OralCavity.ToString() + "r") || dest.Contains(BodyColorPart.OralCavity.ToString() + "g") || dest.Contains(BodyColorPart.OralCavity.ToString() + "b"))
                return "-1";
            else if (dest.Contains(BodyColorPart.Skin.ToString() + "r") || dest.Contains(BodyColorPart.Skin.ToString() + "g") || dest.Contains(BodyColorPart.Skin.ToString() + "b"))
                return "-1";
            else if (dest.Contains(BodyColorPart.Teeth.ToString() + "r") || dest.Contains(BodyColorPart.Teeth.ToString() + "g") || dest.Contains(BodyColorPart.Teeth.ToString() + "b"))
                return "-1";
            else if (dest.Contains(BodyColorPart.Underpants.ToString() + "r") || dest.Contains(BodyColorPart.Underpants.ToString() + "g") || dest.Contains(BodyColorPart.Underpants.ToString() + "b"))
                return "-1";
            return "0";
        }
        
    }

}
