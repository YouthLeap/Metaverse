using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public bool Fighting;

    public Text dailyfooter;
    public Text weeklyfooter;
    public Text monthlyfooter;
    public Text yearlyfooter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Fighting)
        {
            dailyfooter.text = "";
            for (int i=0;i<ServerManager.instance.dailyfightingLeaderBoardList.Count;i++)
            {
                FightingLeaderBoardData data = ServerManager.instance.dailyfightingLeaderBoardList[i];
                dailyfooter.text +="#"+ (i + 1).ToString()+" "+ data.name+" "+ data.dailydamage+ "\n";
            }
            weeklyfooter.text = "";
            for (int i = 0; i < ServerManager.instance.weeklyfightingLeaderBoardList.Count; i++)
            {
                FightingLeaderBoardData data = ServerManager.instance.weeklyfightingLeaderBoardList[i];
                weeklyfooter.text += "#" + (i + 1).ToString() + " " + data.name + " " + data.dailydamage + "\n";
            }
            monthlyfooter.text = "";
            for (int i = 0; i < ServerManager.instance.monthlyfightingLeaderBoardList.Count; i++)
            {
                FightingLeaderBoardData data = ServerManager.instance.monthlyfightingLeaderBoardList[i];
                monthlyfooter.text += "#" + (i + 1).ToString() + " " + data.name + " " + data.monthlydamage + "\n";
            }
            yearlyfooter.text = "";
            for (int i = 0; i < ServerManager.instance.yearlyfightingLeaderBoardList.Count; i++)
            {
                FightingLeaderBoardData data = ServerManager.instance.yearlyfightingLeaderBoardList[i];
                yearlyfooter.text += "#" + (i+1).ToString() + " " + data.name + " " + data.yearlydamage + "\n";
            }
        }
        else
        {
            dailyfooter.text = "";
            for (int i = 0; i < ServerManager.instance.dailyracingLeaderBoardList.Count; i++)
            {
                RacingLeaderBoardData data = ServerManager.instance.dailyracingLeaderBoardList[i];
                dailyfooter.text += "#" + (i + 1).ToString() + " " + data.name + " " + GetTime(data.dailylaptime) + "\n";
            }
            weeklyfooter.text = "";
            for (int i = 0; i < ServerManager.instance.weeklyracingLeaderBoardList.Count; i++)
            {
                RacingLeaderBoardData data = ServerManager.instance.weeklyracingLeaderBoardList[i];
                weeklyfooter.text += "#" + (i + 1).ToString() + " " + data.name + " " + GetTime(data.dailylaptime) + "\n";
            }
            monthlyfooter.text = "";
            for (int i = 0; i < ServerManager.instance.montlyracingLeaderBoardList.Count; i++)
            {
                RacingLeaderBoardData data = ServerManager.instance.montlyracingLeaderBoardList[i];
                monthlyfooter.text += "#" + (i + 1).ToString() + " " + data.name + " " + GetTime(data.monthlylaptime) + "\n";
            }
            yearlyfooter.text = "";
            for (int i = 0; i < ServerManager.instance.yearlyracingLeaderBoardList.Count; i++)
            {
                RacingLeaderBoardData data = ServerManager.instance.yearlyracingLeaderBoardList[i];
                yearlyfooter.text += "#" + (i + 1).ToString() + " " + data.name + " " + GetTime(data.yearlylaptime) + "\n";
            }
        }
    }


    string GetTime(long tics)
    {

        tics /= 10000;


        long milis = tics % 1000;
        tics /= 1000;
        long seconds = tics % 60;
        tics /= 60;
        long minits = tics % 60;
        tics /= 60;
        long hours = tics % 24;
        tics /= 24;
        string str = "";

        if (hours > 0)
        {
            if (hours <= 9)
                str += "0" + hours + ":";
            else
                str += hours + ":";
        }

        if (minits > 9)
        {
            str += minits + ":";
        }
        else
        {
            str += "0" + minits + ":";
        }

        if (seconds > 9)
        {
            str += seconds + ":";
        }
        else
        {
            str += "0" + seconds + ":";
        }

        if (milis > 99)
        {
            str += milis;
        }
        else if (milis > 9)
        {
            str += "0" + milis;
        }
        else
        {
            str += "00" + milis;
        }


        return str;
    }

}
