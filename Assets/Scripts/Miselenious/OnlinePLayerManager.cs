using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePLayerManager : MonoBehaviour
{

    public RectTransform containerRect;
    public RectTransform containerRect2;

    public List<PlayerController> playerList;

    public List<OnlinePlayer> onlinePlayerList;
    public List<OnlinePlayer> onlinePlayerList2=new List<OnlinePlayer>();

    public GameObject onlinePlayerPrefab;

    public static OnlinePLayerManager instance;

    public bool mute;
    public Image playerImage;
    public Image playerImage2;

    public InputField SearchMenu;
    public InputField SearchTab;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float timarcleanar = 0.1f;

    // Update is called once per frame
    void Update()
    {
        mute = false;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playerList[i].Muted)
            {
                mute = true;
            }
        }

        playerImage.gameObject.SetActive(!mute);
        playerImage2.gameObject.SetActive(!mute);

        timarcleanar -= Time.deltaTime;
        if(timarcleanar<0)
        {
            timarcleanar = 0.1f;
            int count = 0;
            int count2 = 0;
            int count3 = 0;
            onlinePlayerList2.Clear();
            for (int i = 0; i < onlinePlayerList.Count; i++)
            {

                onlinePlayerList[i].isfriend = false;
                onlinePlayerList[i].data = new FriendData();

                //TODO-POOH
                FriendData data = ServerManager.instance.friendlist.Find(it => it.sender == OnlinePLayerManager.instance.playerList[onlinePlayerList[i].index].playerEmail && it.receiver == OnlinePLayerManager.instance.playerList[onlinePlayerList[i].index].playerEmail);
                if (data != null)
                {
                    onlinePlayerList[i].data = data;
                    if (data.isAproved)
                        onlinePlayerList[i].isfriend = true;
                }

                if (onlinePlayerList[i].isfriend)
                {
                    count++;
                    if(SearchMenu!=null && SearchMenu.text!="")
                    {
                        if(FindMatch(onlinePlayerList[i].playerName.text, SearchMenu.text))
                        {
                            count2++;
                            onlinePlayerList[i].gameObject.SetActive(true);
                            onlinePlayerList[i].transform.parent = containerRect.transform;
                            onlinePlayerList[i].transform.localScale = new Vector3(1, 1, 1);
                        }
                        else
                        {
                            onlinePlayerList[i].gameObject.SetActive(false);
                            onlinePlayerList[i].transform.parent = transform;
                            onlinePlayerList[i].transform.localScale = new Vector3(1, 1, 1);
                        }
                    }
                    else
                    {
                        count2++;
                        onlinePlayerList[i].gameObject.SetActive(true);
                        onlinePlayerList[i].transform.parent = containerRect.transform;
                        onlinePlayerList[i].transform.localScale = new Vector3(1, 1, 1);
                    }
                    
                }
                else
                {
                    onlinePlayerList[i].transform.parent = transform;
                    onlinePlayerList[i].transform.localScale = new Vector3(1, 1, 1);
                    onlinePlayerList2.Add(onlinePlayerList[i]);
                }
            }



            for(int i=0;i< onlinePlayerList2.Count;i++)
            {
                for(int j=i+1;j< onlinePlayerList2.Count;j++)
                {
                    if((GameManager.instance.mycontroller.transform.position- playerList[onlinePlayerList2[j].index].transform.position).magnitude> (GameManager.instance.mycontroller.transform.position - playerList[onlinePlayerList2[i].index].transform.position).magnitude)
                    {
                        OnlinePlayer TEMP = onlinePlayerList2[i];
                        onlinePlayerList2[i] = onlinePlayerList2[j];
                        onlinePlayerList2[j] = TEMP;
                    }
                }
            }

            for (int i = 0; i < onlinePlayerList2.Count; i++)
            {

                if (SearchTab != null && SearchTab.text != "")
                {
                    if (FindMatch(onlinePlayerList2[i].playerName.text, SearchTab.text))
                    {
                        count3++;
                        onlinePlayerList2[i].gameObject.SetActive(true);
                        onlinePlayerList2[i].transform.parent = containerRect2.transform;
                        onlinePlayerList2[i].transform.localScale = new Vector3(1, 1, 1);
                    }
                    else
                    {
                        onlinePlayerList2[i].gameObject.SetActive(false);
                        onlinePlayerList2[i].transform.parent = transform;
                        onlinePlayerList2[i].transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                else
                {
                    count3++;
                    onlinePlayerList2[i].gameObject.SetActive(true);
                    onlinePlayerList2[i].transform.parent = containerRect2.transform;
                    onlinePlayerList2[i].transform.localScale = new Vector3(1, 1, 1);
                }

                
            }

            containerRect.sizeDelta = new Vector2(100, 170 * count2);
            containerRect2.sizeDelta = new Vector2(100, 170 * count3);
        }
        

    }

    public bool FindMatch(string main,string match)
    {
        for(int i=0;i<main.Length;i++)
        {
            for(int j=0;j<match.Length && j+i<main.Length;j++)
            {
                if (main[i+j] != match[j])
                    break;
                if (j == match.Length - 1)
                    return true;
            }
        }

        return false;
    }

    public void AddPlayer(PlayerController player)
    {
        OnlinePlayer onlinePlayer= Instantiate(onlinePlayerPrefab, containerRect).GetComponent<OnlinePlayer>();
        onlinePlayer.index = playerList.Count;
        playerList.Add(player);
        onlinePlayerList.Add(onlinePlayer);
    }


    public void RemovePlayer(PlayerController player)
    {
        playerList.Remove(player);

        while(onlinePlayerList.Count> playerList.Count)
        {
            OnlinePlayer onlinePlayer = onlinePlayerList[onlinePlayerList.Count - 1];
            onlinePlayerList.Remove(onlinePlayer);
            if(onlinePlayer)
            Destroy(onlinePlayer.gameObject);
        }

        for (int i=0;i< onlinePlayerList.Count;i++)
        {
            onlinePlayerList[i].index = i;
        }

        
    }



    public void MuteAll()
    {
        bool mu = false;
        Debug.Log("MuteUpmute ALL");
        for (int i = 0; i < playerList.Count; i++)
        {
            if(!playerList[i].Muted)
            {
                mu = true;
            }
        }

        for (int i = 0; i < onlinePlayerList.Count; i++)
        {
            onlinePlayerList[i].MuteUnmute(mu);
        }

    }

    private void OnDestroy()
    {
        instance = null;
    }
}
