using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public List<GameObject> Inbox;
    public Image background;
    public Image forground;

    public GameObject massagecountholder;
    public Text massagecount;

    float timar = 5;

    public GameObject chatpannel;

    public void ChatInput(InputAction.CallbackContext context)
    {
        //edward
        if (FindObjectOfType<MapManager>() != null && FindObjectOfType<MapManager>().bMap)
        {
            return;
        }
        //

        ActiveChat();
    }


    public void RegisterInput()
    {
        //edward
        if (FindObjectOfType<MapManager>() != null && FindObjectOfType<MapManager>().bMap)
        {
            return;
        }
        //

        GameInputHandler.instance.gameInput.Menu.ChatEnter.performed += ctx => ChatInput(ctx);
        GameInputHandler.instance.gameInput.Chat.ChatExit.performed += ctx => ChatInput(ctx);
    }

    // Start is called before the first frame update
    void Start()
    {
        RegisterInput();
        chatpannel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (chatpannel.activeSelf)
            GameManager.instance.massagecount = 0;
        massagecount.text = GameManager.instance.massagecount.ToString();
        if (GameManager.instance.massagecount != 0)
            massagecountholder.SetActive(true);  
        else
            massagecountholder.SetActive(false);

        /*if(chatpannel.activeSelf!=isctrl && isctrl)
        {
            chatpannel.SetActive(isctrl);
            Inbox[0].GetComponent<InputField>().Select();
            Inbox[0].GetComponent<InputField>().ActivateInputField();

        }
        else
        {
            chatpannel.SetActive(isctrl);
        }*/
        
        timar = 0;
        timar -= Time.deltaTime;
        if(Inbox.Count>1)
            Inbox[1].SetActive(false);

        if (Inbox.Count > 2)
            Inbox[2].SetActive(false);

        background.color = new Color(21 / 255.0f, 21 / 255.0f, 21 / 255.0f, 0.1f);
        forground.color = new Color(12 / 255.0f, 12 / 255.0f, 12 / 255.0f, 0.1f);

        if (timar < 2.5f)
        {
            
        }
        else if (timar < 0)
        {
            timar = 0;

            foreach(GameObject obj in Inbox)
            {
                obj.SetActive(false);
            }
        }

        /*if(Inbox[0].GetComponent<InputField>().isFocused)
        {
            timar = 5.0f;
            foreach (GameObject obj in Inbox)
            {
                obj.SetActive(true);
            }
        }*/

    }
    public void ActiveChat()
    {
        chatpannel.SetActive(!chatpannel.activeSelf);
        GameInputHandler.instance.ActivateChat(chatpannel.activeSelf);
        if (chatpannel.activeSelf)
        {
            Inbox[0].GetComponent<InputField>().Select();
            Inbox[0].GetComponent<InputField>().ActivateInputField();
        }
    }
}
