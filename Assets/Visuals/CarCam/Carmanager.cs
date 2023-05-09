using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carmanager : MonoBehaviour
{

    public List<Transform> carlist;

    public int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        currentIndex = 0;
        GameManager.instance.mycontroller.carcont.carindex = currentIndex;
        PlayerPrefs.SetInt("carindex", currentIndex);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<carlist.Count;i++)
        {
            carlist[i].localPosition = new Vector3(((i- currentIndex)>=0? (i - currentIndex): (carlist.Count+ (i - currentIndex)))*10, -1,5);
            carlist[i].localRotation *= Quaternion.Euler(0,1,0);
        }

    }

    public void Next()
    {
        currentIndex++;
        if (currentIndex >= carlist.Count)
        {
            currentIndex = 0;
        }
        GameManager.instance.mycontroller.carcont.carindex = currentIndex;
        PlayerPrefs.SetInt("carindex", currentIndex);
    }

    public void Previous()
    {
        currentIndex--;
        if (currentIndex <0)
        {
            currentIndex = carlist.Count - 1;
        }
        GameManager.instance.mycontroller.carcont.carindex = currentIndex;
        PlayerPrefs.SetInt("carindex", currentIndex);
    }
}
