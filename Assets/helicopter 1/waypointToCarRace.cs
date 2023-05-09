using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class waypointToCarRace : MonoBehaviour
{


    public AudioSource HelicopterSound;
    public Vector3 CurrecntPos;
    public Quaternion Currecntrot;
   

    public GameObject[] Buildingwaypointstoarena;
    public GameObject[] arenatoBuildingWaypoint;



    public GameObject[] Buildingwaypointstocastle;
    public GameObject[] castletobuildingwaypoint;


    public GameObject[] Buildingwaypointstocarrace;
    public GameObject[] carraceWaypointTobuilding;


    public GameObject[] waypointcarracetoarena;
    public GameObject[] AreantoCarraceWaypoint;
    public GameObject player;
    public Transform ARENAPlayerPos;
    public Transform cARrACEPlayerPos;
    public Transform bUILDINGPlayerPos;
    public int current = 0;
    public float speed = 20;
    public float WPradius = 5;
    /// <summary>
    /// ///////////////////
    /// </summary>
    public bool BuildingtoArena;
    public bool BuildingtoCarrace;
    public bool BuildingtoCastle;

    /// <summary>
    /// ///////////////////
    /// </summary>
    public bool ArenatoBuilding;
    public bool ArenatoCastle;
    public bool ArenatoCarRace;
    /// <summary>
    /// ////////////////
    /// </summary>
    public bool CarRacetobuilding;
    public bool CarRacetoarena;
    public bool CarRacetocastle;
    /// <summary>
    /// ///////////
    /// </summary>
    public bool castleTobuilding;
    public bool castleToarena;
    public bool castleTcarrRace;
    public static GameObject FindInChildrenIncludingInactive(GameObject go, string name)
    {

        for (int i = 0; i < go.transform.childCount; i++)
        {
            if (go.transform.GetChild(i).gameObject.tag == name) return go.transform.GetChild(i).gameObject;
            GameObject found = FindInChildrenIncludingInactive(go.transform.GetChild(i).gameObject, name);
            if (found != null) return found;
        }

        return null;  //couldn't find crap
    }
    public static GameObject FindIncludingInactive(string name)
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            //no scene loaded
            return null;
        }

        var game_objects = new List<GameObject>();
        scene.GetRootGameObjects(game_objects);

        foreach (GameObject obj in game_objects)
        {
            if (obj.transform.tag == name) return obj;

            GameObject found = FindInChildrenIncludingInactive(obj, name);
            if (found) return found;
        }

        return null;
    }
    private void Awake()
    {
        this.GetComponentInChildren<Camera>().enabled = false;
        CurrecntPos = this.transform.position;
        Currecntrot = this.transform.rotation;
    }

    private GameObject[] heli;
    private void Start()
    {
        {
            player = FindIncludingInactive("Player");

            BuildingtoArena = false; BuildingtoCarrace = false; BuildingtoCastle = false;
        }
    }
    void Update()
    {
        //if (ISFlying)
        //{

        //    heli = GameObject.FindGameObjectsWithTag("Helicopter");

        //    foreach (GameObject Helicopters in heli)
        //    {
        //        Helicopters.SetActive(false);

        //    }
        //    this.gameObject.SetActive(true);
        //}
        if (BuildingtoArena)
        {
            HeliTransfer(Buildingwaypointstoarena, ref BuildingtoArena, ARENAPlayerPos);
        }


        if (BuildingtoCarrace)
        {
            HeliTransfer(Buildingwaypointstocarrace, ref BuildingtoCarrace, cARrACEPlayerPos);
        }
        if (ArenatoBuilding)
        {
            HeliTransfer(arenatoBuildingWaypoint, ref ArenatoBuilding, bUILDINGPlayerPos);
        }
        if (CarRacetobuilding)
        {
            HeliTransfer(carraceWaypointTobuilding, ref CarRacetobuilding, bUILDINGPlayerPos);
        }

        if (CarRacetoarena)
        {
            HeliTransfer(AreantoCarraceWaypoint, ref CarRacetoarena, ARENAPlayerPos);

        }

        if (ArenatoCarRace)
        {
            HeliTransfer(AreantoCarraceWaypoint, ref ArenatoCarRace, cARrACEPlayerPos);
        }




    }

    void HeliTransfer(GameObject[] Waypoints, ref bool ThisFunctionname, Transform PlayerPositionForActivate)
    {


        HelicopterSound.Play();
       
        helicoptorEnabler[] Helicopterss2 = FindObjectsOfType<helicoptorEnabler>();
        foreach (helicoptorEnabler helicopterss1 in Helicopterss2)
        {
            
            helicopterss1.helicopters[0].gameObject.GetComponentInChildren<Camera>().enabled = true;
            helicopterss1.helicopters[1].gameObject.GetComponentInChildren<Camera>().enabled = true;
            helicopterss1.helicopters[2].gameObject.GetComponentInChildren<Camera>().enabled = true;
            helicopterss1.helicopters[0].gameObject.GetComponentInChildren<TMPro.TMP_Text>().enabled = true;
            helicopterss1.helicopters[1].gameObject.GetComponentInChildren<TMPro.TMP_Text>().enabled = true;
            helicopterss1.helicopters[2].gameObject.GetComponentInChildren<TMPro.TMP_Text>().enabled = true;
        }
        this.gameObject.SetActive(true);
        this.GetComponentInChildren<Camera>().enabled = true;
     
        FindObjectOfType<helipadactive>().Selector.enabled = false;
        if (Vector3.Distance(Waypoints[current].transform.position, transform.position) < WPradius)
        {
            current++;
            speed = 20;
            if (current >= Waypoints.Length)
            {
                ThisFunctionname = false;
                speed = 0;
                current = 0;
                this.transform.position = CurrecntPos;
                this.transform.rotation = Currecntrot;
                helicoptorEnabler[] Helicopterss = FindObjectsOfType<helicoptorEnabler>();
                foreach(helicoptorEnabler helicopterss1 in Helicopterss)
                {
                    helicopterss1.helicopters[0].gameObject.SetActive(true);
                    helicopterss1.helicopters[1].gameObject.SetActive(true);
                    helicopterss1.helicopters[2].gameObject.SetActive(true);
                    helicopterss1.helicopters[0].gameObject.GetComponentInChildren<Camera>().enabled = false;
                    helicopterss1.helicopters[1].gameObject.GetComponentInChildren<Camera>().enabled = false;
                    helicopterss1.helicopters[2].gameObject.GetComponentInChildren<Camera>().enabled = false;
                    helicopterss1.helicopters[0].gameObject.GetComponentInChildren<TMPro.TMP_Text>().enabled = false;
                    helicopterss1.helicopters[1].gameObject.GetComponentInChildren<TMPro.TMP_Text>().enabled = false;
                    helicopterss1.helicopters[2].gameObject.GetComponentInChildren<TMPro.TMP_Text>().enabled = false;

                }
                //helipadactive[] HelipadsCam = FindObjectsOfType<helipadactive>();
                //foreach (helipadactive Camera in HelipadsCam)
                //{
                //   Camera.gameObject.GetComponentInChildren<Camera>().enabled =false; 
                //}
                //this.gameObject.SetActive(false);

                //if (this.transform.position == CurrecntPos)
                //{
                //    this.gameObject.SetActive(true);
                //}
                this.GetComponentInChildren<Camera>().enabled = false;
                player.transform.position = PlayerPositionForActivate.transform.position;
                player.gameObject.SetActive(true);

                HelicopterSound.Stop();



            }
        }
        transform.position = Vector3.MoveTowards(transform.position, Waypoints[current].transform.position, Time.deltaTime * speed);
        var targetRotation = Quaternion.LookRotation(Waypoints[current].transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);


    }


}

