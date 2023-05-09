//using PlayFab;
//using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class FireBaseLogin : MonoBehaviour
{

    public static FireBaseLogin instance;

    public string username="";
    public string email;
    public string password;

    public UserData playerData=new UserData();
    public FightingLeaderBoardData playerFightingData = new FightingLeaderBoardData();
    public RacingLeaderBoardData playerRacingData = new RacingLeaderBoardData();

    public IDictionary<string,FriendData> friendlist = new Dictionary<string, FriendData>();
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
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }



}