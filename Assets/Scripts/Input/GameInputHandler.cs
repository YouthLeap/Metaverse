using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputHandler : MonoBehaviour
{
    public static GameInputHandler instance;
    [SerializeField]
    public GameInput gameInput;
    public PlayerInputActions actions;
    public float test;
    public PlayerInput input;

    InputActionMap previousActionMap;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            gameInput = new GameInput();
        }
        else
        {
            Destroy(gameObject);
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        ActivatePlayer();
        //ActivateFighter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        gameInput.Enable();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }

    private void OnDestroy()
    {
        gameInput.Dispose();
    }

    public void ActivatePlayer()
    {
        gameInput.Fighter.Disable();
        gameInput.Racer.Disable();
        gameInput.Player.Enable();
        gameInput.Menu.Enable();
        gameInput.Swimmer.Disable();
        previousActionMap = gameInput.Player;
    }
    public void ActivateRacer()
    {
        previousActionMap.Disable();
        gameInput.Racer.Enable();
        previousActionMap = gameInput.Racer;
    }
    public void ActivateFighter()
    {
        previousActionMap.Disable();
        gameInput.Fighter.Enable();
        previousActionMap = gameInput.Fighter;
    }
    public void ActivateSwimmer()
    {
        previousActionMap.Disable();
        gameInput.Swimmer.Enable();
        previousActionMap = gameInput.Swimmer;
    }
    public void ActivateNewtral(bool newtral)
    {
        Debug.Log("CallpreviousActionMap ");
        if (newtral)
        { 
            previousActionMap.Disable();
        }
        else
        {
            try
            {
                previousActionMap.Enable();
            }
            catch (Exception e)
            {
                Debug.Log("previousActionMap " + e.Message);
            }
        }
            

        GameManager.instance.FreeCursor(newtral);
    }

    public void ActivateChat(bool newtral)
    {

        Debug.Log("Chant enabled");
        if (newtral)
        {
            previousActionMap.Disable();
            gameInput.Menu.Disable();
            GameManager.instance.mycontroller.currentState = PlayerController.playerState.chatting;
            //gameInput.Chat.Enable();
        }
        else
        {
            previousActionMap.Enable();
            gameInput.Menu.Enable();
            GameManager.instance.mycontroller.currentState = PlayerController.playerState.idle;
            //gameInput.Chat.Disable();
        }
        GameManager.instance.FreeCursor(newtral);
    }
}
