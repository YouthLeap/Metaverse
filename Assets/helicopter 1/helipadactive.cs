using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helipadactive : MonoBehaviour
{
    public Canvas Selector;
    public GameObject Helicopter;

    private void Awake()
    {
        Selector.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {

        Helicopter.GetComponentInChildren<Camera>().enabled = false;
        Helicopter.GetComponentInChildren<TMPro.TMP_Text>().enabled = false;

        Selector.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        //Player = GameObject.FindGameObjectWithTag("Player");
    }


    //public void SetIsFlying()
    //{
    //    FindObjectOfType<waypointToCarRace>().ISFlying = true;
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            other.gameObject.SetActive(false);
            Helicopter.GetComponentsInChildren<Camera>()[0].enabled = true;
            Helicopter.GetComponentsInChildren<Camera>()[1].enabled = true;
            Selector.enabled = true;
           

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {


            Helicopter.GetComponentsInChildren<Camera>()[0].enabled = false;
            Helicopter.GetComponentsInChildren<Camera>()[1].enabled = false;
            Helicopter.GetComponentInChildren<TMPro.TMP_Text> ().enabled = true;
            


        }
    }


    public void BuildingToCarRace()
    {
        FindObjectOfType<waypointToCarRace>().BuildingtoCarrace = true;
        // SetIsFlying();
        Selector.enabled = false;
    }
    public void BuildingToFightArena()
    {
        FindObjectOfType<waypointToCarRace>().BuildingtoArena = true;
        // SetIsFlying();
        Selector.enabled = false;
    }
    public void CarRaceToBuilding()
    {
        FindObjectOfType<waypointToCarRace>().CarRacetobuilding = true; Selector.enabled = false;
        // SetIsFlying();
    }
    public void FightArenaToBuilding()
    {
        FindObjectOfType<waypointToCarRace>().ArenatoBuilding = true;
        // SetIsFlying();
        Selector.enabled = false;
    }
    public void FightArenaToCarRace()
    {
        FindObjectOfType<waypointToCarRace>().ArenatoCarRace = true; Selector.enabled = false;
        // SetIsFlying();
    }
    public void CarRaceToFightArena()
    {
        FindObjectOfType<waypointToCarRace>().CarRacetoarena = true; Selector.enabled = false;
        //  SetIsFlying();
    }
    public void diabled()
    {
        Selector.enabled = false;
    }
}
