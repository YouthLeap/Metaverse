﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//Statistics.cs keeps track of the racer's rank, name, vehicle name, lap, race times, race state, saving best times, wrong way detecion etc.

namespace RGSK
{
    public class Statistics : MonoBehaviour
    {
        [System.Serializable]
        public class RacerDetails
        {
            public string racerName;
            public string vehicleName;
        }

        public RacerDetails racerDetails;

        //Int
        public int rank;//current rank
        public int lap; //current lap
        public int checkpoint; //current checkpoint(Checkpoint Race)

        //Strings
        public string currentLapTime; //current lap time string displayed by RaceUI.cs
        public string prevLapTime; //Previous lap time string displayed by RaceUI.cs
        public string totalRaceTime; //Total lap time string displayed by RaceUI.cs
        public string bestLapTime; //Best lap time string for current session;

        //Floats
        private float lapTimeCounter; // keeps track of our current Lap time counter
        private float totalTimeCounter; //keeps track of our total race time
        private float prevLapCounter;
        private float bestLapCounter; //keeps track of the current session's best lap time
        private float dotProduct; //used for wrong way detection
        private float registerDistance = 10.0f; //distance to register a passed node
        private float reviveTimer;
        private float wrongwayTimer; //delay timer

        //Hidden Vars
        [HideInInspector]
        public Transform lastPassedNode; //last node to passed - used when respawning.
        [HideInInspector]
        public Transform target; //progress tracker target
        [HideInInspector]
        public int currentNodeNumber; //next node index in the "path" list
        [HideInInspector]
        public List<Transform> path = new List<Transform>();
        [HideInInspector]
        public List<bool> passednodes = new List<bool>();
        [HideInInspector]
        public List<Transform> checkpoints = new List<Transform>();
        [HideInInspector]
        public List<bool> passedcheckpoints = new List<bool>();
        [HideInInspector]
        public bool finishedRace;
        [HideInInspector]
        public bool knockedOut;
        [HideInInspector]
        public bool goingWrongway;
        [HideInInspector]
        public bool passedAllNodes;
        [HideInInspector]
        public bool infiniteLaps;
        [HideInInspector]
        public float speedRecord;//speed trap top speed

        void Start()
        {
            if (!EddyRaceManager.instance) { enabled = false; return; }

            FindPath();

            FindCheckpoints();

            Initialize();
        }

        void Initialize()
        {
            lap = 1;

            //Set the start timer for a checkpoint race
            if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.Checkpoints)
            {
                lapTimeCounter = EddyRaceManager.instance.initialCheckpointTime;
            }

            //Set the elimination timer for a eimination race
            if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.Elimination)
            {
                lapTimeCounter = EddyRaceManager.instance.eliminationTime;
            }

            //Set the time limit timer for a drift race
            if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.Drift && EddyRaceManager.instance.timeLimit)
            {
                lapTimeCounter = EddyRaceManager.instance.driftTimeLimit;
            }

            infiniteLaps = EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.TimeTrial;
        }


        void FindPath()
        {
            if (!EddyRaceManager.instance.pathContainer)
                return;

            Transform[] nodes = EddyRaceManager.instance.pathContainer.GetComponentsInChildren<Transform>();

            foreach (Transform p in nodes)
            {

                if (p != EddyRaceManager.instance.pathContainer)
                {
                    path.Add(p);
                }
            }

            passednodes = new List<bool>(new bool[path.Count]);

            lastPassedNode = path[0];
        }


        void FindCheckpoints()
        {
            if (!EddyRaceManager.instance.checkpointContainer)
                return;

            Checkpoint[] _checkpoint = EddyRaceManager.instance.checkpointContainer.GetComponentsInChildren<Checkpoint>();

            foreach (Checkpoint c in _checkpoint)
            {
                //Find SpeedTrap Checkpoints
                if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.SpeedTrap)
                {
                    if (c.checkpointType == Checkpoint.CheckpointType.Speedtrap)
                    {
                        checkpoints.Add(c.transform);
                    }
                }

                //Find Time Checkpoints
                if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.Checkpoints)
                {
                    if (c.checkpointType == Checkpoint.CheckpointType.TimeCheckpoint)
                    {
                        checkpoints.Add(c.transform);
                    }
                }
            }

            passedcheckpoints = new List<bool>(new bool[checkpoints.Count]);
        }


        void Update()
        {
            GetPath();

            CalculateRaceTimes();

            CheckForWrongway();

            CheckForRespawn();
        }


        void GetPath()
        {
            int n = currentNodeNumber;

            Transform node = path[n] as Transform;

            Vector3 nodeVector = target.InverseTransformPoint(node.position);

            //register that we have passed this node
            if (nodeVector.magnitude <= registerDistance)
            {
                currentNodeNumber++;

                passednodes[n] = true;

                //set our last passed node
                if (n != 0)
                    lastPassedNode = path[n - 1];
                else
                    lastPassedNode = path[path.Count - 1];
            }

            //Check if all nodes have been passed
            foreach (bool pass in passednodes)
            {
                if (pass == true)
                {
                    passedAllNodes = true;
                }
                else
                {
                    passedAllNodes = false;
                }
            }

            //Reset the currentNodeNumber after passing all the nodes
            if (currentNodeNumber >= path.Count)
            {
                currentNodeNumber = 0;
            }
        }


        // Race time calculations
        void CalculateRaceTimes()
        {

            if (EddyRaceManager.instance.raceStarted && !knockedOut && !finishedRace)
            {

                if (EddyRaceManager.instance.timerType == EddyRaceManager.TimerType.CountUp)
                {
                    lapTimeCounter += Time.deltaTime;
                }

                if (EddyRaceManager.instance.timerType == EddyRaceManager.TimerType.CountDown)
                {
                    lapTimeCounter -= Time.deltaTime;

                    if (lapTimeCounter <= 0)
                    {
                        if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.Checkpoints)
                        {
                            knockedOut = true;

                            EddyRaceManager.instance.KnockoutRacer(this);
                        }

                        if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.Drift)
                        {
                            if (!knockedOut && !finishedRace)
                                FinishRace();
                        }
                    }
                }

                totalTimeCounter += Time.deltaTime;
            }

            //Format the time strings
            currentLapTime = EddyRaceManager.instance.FormatTime1(lapTimeCounter);

            totalRaceTime = EddyRaceManager.instance.FormatTime1(totalTimeCounter);
        }


        public void NewLap()
        {
            if (finishedRace || knockedOut) return;

            prevLapTime = currentLapTime;
            prevLapCounter = lapTimeCounter;

            CheckForBestTime();

            //Reset passed nodes
            for (int i = 0; i < passednodes.Count; i++)
            {
                passednodes[i] = false;
            }

            //Reset passed checkpoints
            for (int i = 0; i < passedcheckpoints.Count; i++)
            {
                passedcheckpoints[i] = false;
            }

            //Lap increment logic
            lap++;

            if (!infiniteLaps)
            {
                //Show final lap indication on the final lap
                if (lap == EddyRaceManager.instance.totalLaps && EddyRaceManager.instance.showRaceInfoMessages && gameObject.tag == "Player")
                    RaceUI.instance.ShowRaceInfo("Final Lap!", 2.0f, Color.white);


                if (lap > EddyRaceManager.instance.totalLaps)
                {
                    if (!knockedOut && !finishedRace)
                        FinishRace();
                }
            }


            //Reset the lap timer based on the Timer Type
            if (EddyRaceManager.instance.timerType != EddyRaceManager.TimerType.CountDown)
            {
                lapTimeCounter = 0.0f;
            }

            //Check for knockout
            if (EddyRaceManager.instance._raceType == EddyRaceManager.RaceType.LapKnockout)
            {
                if (this.rank == RankManager.instance.currentRacers - 1)
                    EddyRaceManager.instance.KnockoutRacer(EddyRaceManager.instance.GetLastPlace());
            }
        }

        public void FinishRace()
        {
            if (finishedRace) return;

            finishedRace = true;

            //Player finish
            if (gameObject.tag == "Player")
            {
                EddyRaceManager.instance.EndRace(rank);
            }

            //Continue after finishing
            if (EddyRaceManager.instance.continueAfterFinish)
            {
                AIMode();
            }
            else
            {
                EddyRaceManager.instance.DisableRacerInput(gameObject);
            }           
        }


        // Switches a player car to an AI controlled car
        public void AIMode()
        {
            if (GetComponent<PlayerControl>())
            {

                GetComponent<PlayerControl>().enabled = false;

                if (GetComponent<OpponentControl>())
                {
                    GetComponent<OpponentControl>().enabled = true;
                }
                else
                {
                    gameObject.AddComponent<OpponentControl>();
                }
            }
        }


        // Switches a AI car to a human controlled car
        public void PlayerMode()
        {
            if (GetComponent<OpponentControl>())
            {

                GetComponent<OpponentControl>().enabled = false;

                if (GetComponent<PlayerControl>())
                {
                    GetComponent<PlayerControl>().enabled = true;
                }
                else
                {
                    gameObject.AddComponent<PlayerControl>();
                }
            }
        }


        void RegisterCheckpoint(Checkpoint.CheckpointType type, float timeAdd)
        {
            if (finishedRace || knockedOut) return;

            switch (type)
            {

                case Checkpoint.CheckpointType.Speedtrap:
                    if (EddyRaceManager.instance._raceType != EddyRaceManager.RaceType.SpeedTrap)
                        return;

                    //add to the racers total speed
                    float speed = 0;

                    if (GetComponent<Car_Controller>())
                        speed = GetComponent<Car_Controller>().currentSpeed;

                    if (GetComponent<Motorbike_Controller>())
                        speed = GetComponent<Motorbike_Controller>().currentSpeed;

                    speedRecord += speed;

                    //play a sound and show info
                    if (gameObject.tag == "Player")
                    {
                        try { SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.speedTrapSound); } catch { }

                        if (EddyRaceManager.instance.showRaceInfoMessages)
                            RaceUI.instance.ShowRaceInfo("+ " + speed + " mph", 1.0f, Color.white);
                    }

                    break;

                case Checkpoint.CheckpointType.TimeCheckpoint:
                    //add our chekpoint
                    checkpoint++;

                    //add to the timer
                    lapTimeCounter += timeAdd;

                    //play a sound and show info
                    if (gameObject.tag == "Player")
                    {
                        try { SoundManager.instance.PlayDefaultSound(SoundManager.instance.defaultSounds.checkpointSound); } catch { }

                        if (EddyRaceManager.instance.showRaceInfoMessages)
                            RaceUI.instance.ShowRaceInfo("+ " + EddyRaceManager.instance.FormatTime(timeAdd), 1.0f, Color.white);
                    }
                    break;
            }
        }

        void GhostVehicleLogic(bool firstLap, bool beatLastLap)
        {
            if (!EddyRaceManager.instance.enableGhostVehicle || !gameObject.GetComponent<GhostVehicle>()) return;

            //Always create a ghost and cache values after the first lap
            if (firstLap)
            {
                GetComponent<GhostVehicle>().CacheValues();
                EddyRaceManager.instance.CreateGhostVehicle(gameObject);
            }

            //Create a ghost & cache the values if we beat the last ghost
            if (beatLastLap)
            {
                GetComponent<GhostVehicle>().CacheValues();
                EddyRaceManager.instance.CreateGhostVehicle(gameObject);
            }
            else
            {
                //Use the cached values if we dont beat the last lap
                if (!firstLap)
                {
                    GetComponent<GhostVehicle>().UseCachedValues();
                    EddyRaceManager.instance.CreateGhostVehicle(gameObject);
                }
            }

            //Reset the recorded values
            GetComponent<GhostVehicle>().ClearValues();
        }

        void CheckForBestTime()
        {
            //Best Lap Time
            if (bestLapCounter == 0)
            {
                bestLapCounter = lapTimeCounter;
                bestLapTime = EddyRaceManager.instance.FormatTime(bestLapCounter);
                GhostVehicleLogic(true, false);
            }

            else if (prevLapCounter < bestLapCounter)
            {
                bestLapCounter = prevLapCounter;
                bestLapTime = EddyRaceManager.instance.FormatTime(bestLapCounter);
                GhostVehicleLogic(false, true);
            }

            else if (prevLapCounter > bestLapCounter)
            {
                GhostVehicleLogic(false, false);
            }

            //Save Best Track Lap Time
            if (gameObject.tag == "Player")
            {
                //Set new best
                if (!PlayerPrefs.HasKey("BestTimeFloat" + SceneManager.GetActiveScene().name))
                {
                    PlayerPrefs.SetString("BestTime" + SceneManager.GetActiveScene().name, EddyRaceManager.instance.FormatTime(lapTimeCounter));

                    PlayerPrefs.SetFloat("BestTimeFloat" + SceneManager.GetActiveScene().name, Mathf.Abs(lapTimeCounter));

                    if (EddyRaceManager.instance.showRaceInfoMessages)
                    {
                        RaceUI.instance.ShowRaceInfo("New best time!", 2.0f, Color.white);
                    }
                }

                //Beat our best
                if (PlayerPrefs.GetFloat("BestTimeFloat" + SceneManager.GetActiveScene().name) > lapTimeCounter)
                {
                    PlayerPrefs.SetString("BestTime" + SceneManager.GetActiveScene().name, EddyRaceManager.instance.FormatTime(lapTimeCounter));

                    PlayerPrefs.SetFloat("BestTimeFloat" + SceneManager.GetActiveScene().name, lapTimeCounter);

                    if (EddyRaceManager.instance.showRaceInfoMessages)
                    {
                        RaceUI.instance.ShowRaceInfo("New best time!", 2.0f, Color.white);
                    }
                }
            }
        }

        void CheckForWrongway()
        {
            float nodeAngle = target.transform.eulerAngles.y;

            float transformAngle = transform.eulerAngles.y;

            float angleDifference = nodeAngle - transformAngle;


            //Set wrong way to true after a dealy of 1.0 seconds
            goingWrongway = (wrongwayTimer >= 1.0f);

            if (Mathf.Abs(angleDifference) <= 230f && Mathf.Abs(angleDifference) >= 120)
            {
                //Add/reset the timer
                if (GetComponent<Rigidbody>().velocity.magnitude >= 5.0f)
                {
                    wrongwayTimer += Time.deltaTime;
                }
                else
                {
                    wrongwayTimer = 0.0f;
                }
            }
            else
            {
                wrongwayTimer = 0.0f;
            }
        }

        void CheckForRespawn()
        {
            if (finishedRace && knockedOut)
                return;

            //incase the car flips over or going wrong way then respawn
            if (transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280 || EddyRaceManager.instance.forceWrongwayRespawn && goingWrongway)
            {
                reviveTimer += Time.deltaTime;
            }
            else
            {
                reviveTimer = 0.0f;
            }

            if (reviveTimer >= 5.0f)
            {
                EddyRaceManager.instance.RespawnRacer(transform, lastPassedNode, 3.0f);

                reviveTimer = 0.0f;
            }
        }

        void OnTriggerEnter(Collider other)
        {

            //Finish line
            if (other.tag == "FinishLine" || other.tag == "Finish")
            {
                if (passedAllNodes)
                    NewLap();
            }

            //Checkpoint
            if (other.GetComponent<Checkpoint>())
            {
                for (int i = 0; i < checkpoints.Count; i++)
                {
                    if (checkpoints[i] == other.transform && !passedcheckpoints[i])
                    {
                        passedcheckpoints[i] = true;

                        RegisterCheckpoint(checkpoints[i].GetComponent<Checkpoint>().checkpointType, checkpoints[i].GetComponent<Checkpoint>().timeToAdd);
                    }
                }
            }
        }
    }
}
