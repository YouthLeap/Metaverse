using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectManager : MonoBehaviour
{

    public Transform target;

    public GameObject BackButton;
    public GameObject MaleUI;
    public GameObject FemaleUI;
    

    int count = 0;

    public int itemtoselect;

    void Start()
    {
        //PlayerPrefs.SetInt("PlayerSelect", PlayerPrefs.GetInt("PlayerSelect", 3));
        //FirebaseData.instance.UpdateCustomCaracterList("PlayerSelect", 3.ToString());
        count = PlayerPrefs.GetInt("PlayerSelect", -1);
        if (count != int.Parse(ServerManager.instance.character.PlayerSelect))
        {
            BackButton?.SetActive(false);
        }

        count = int.Parse(ServerManager.instance.character.PlayerSelect);
        //;

        if (count < 3)
            count = 3;

        
    }

    // Update is called once per frame
    void Update()
    {
        target.position = new Vector3(count<3? -2+count*2:100 * count, target.position.y, target.position.z);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (MaleUI!=null && FemaleUI!=null)
        {
            if (count % 2 == 1)
            {
                MaleUI.SetActive(true);
                FemaleUI.SetActive(false);
                
            }
            else if (count % 2 == 0)
            {
                FemaleUI.SetActive(true);
                MaleUI.SetActive(false);
                
            }
            else
            {
                FemaleUI.SetActive(false);
                MaleUI.SetActive(false);
                
            }
        }
    }

    public void PrevBtn()
    {

        count++;
        if (count > 4)
            count = 3;


    }
    public void NextBtn()
    {
        count--;

        if (count < 3)
            count = 4;

    }
    public void SelectBtn()
    {
        PlayerPrefs.SetInt("PlayerSelect",count);
        ServerManager.instance.character.UpdateCharacterPlayer(count.ToString());
        //TODO- POOH
        SceneManager.LoadScene("LoadingScene");
    }

    public void BackBtn()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void VisitWebSite()
    {
        SceneManager.LoadScene("WebView");
    }
}
