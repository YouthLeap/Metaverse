using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public int point = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CarCaracterController controller = other.GetComponentInParent<CarCaracterController>();
        if (controller != null && controller.view != null && controller.view.IsMine && controller.isactive)
        {
            //if (controller.rb.velocity.x > 0)
            //{
            //    point++;
            //}
            //else
            //{
            //    point--;
            //}

            //if (point == 1)
            //{
            //    FirebaseData.instance.UpdateRacingData(DateTime.UtcNow.Ticks - GameManager.instance.laptime);
            //    GameManager.instance.racemanager.RestartRace();
            //    point = 0;
            //}

            //edward
            if(GameManager.instance.racemanager.isStart)
            {
                ServerManager.instance.UpdateRacingData(GameManager.instance.racemanager.m_currenttime);
                GameManager.instance.racemanager.isStart = false;
                GameManager.instance.racemanager.RestartPanel.SetActive(true);
                GameManager.instance.racemanager.ShowResult();
                GameManager.instance.FreeCursor(true);
            }
            //
        }
    }

}
