using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadWordChecker : MonoBehaviour
{
    public static BadWordChecker instance;

    public string bigstring;
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public List<string> wordstocheck;
    // Start is called before the first frame update
    void Start()
    {
        string[] strlist= bigstring.Split(new char[] { '(' });

        foreach (string str in strlist)
        {
            wordstocheck.Add(str);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string CheckWord(string str)
    {

        for(int i=0;i<wordstocheck.Count;i++)
        {
            string checknow = wordstocheck[i];
            int startindex = str.IndexOf(checknow, 0, System.StringComparison.InvariantCultureIgnoreCase);
            Debug.Log(startindex);
            if(startindex>=0)
                str = str.Substring(0, startindex) + ((startindex + checknow.Length) < (str.Length - 1)? str.Substring(startindex + checknow.Length, str.Length-1):"");
        }


        return str;
    }
}
