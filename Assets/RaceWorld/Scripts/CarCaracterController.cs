using PathCreation;
using Photon.Pun;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;

public class CarCaracterController : MonoBehaviour
{
    public Rigidbody rb;

    public float moveForce = 0;
    public Vector3 rotaionVector = Vector3.zero;
    public float MaxSpeed;
    public float MaxDistance;
    public CarBody car;
    public GameObject carparts;
    public GameObject carColliders;
    public CarController carmaincontroller;
    public Transform nameplate;
    public Transform nameposition;
    public List<CarBody> carbodys;

    public int carindex = 0;

    public PathCreator pathCreator;

    public Transform looktarget;

    public List<GameObject> carlist;
    public GameObject mybody;

    public PhotonView view;

    public bool isactive = false;
    public bool raceready = false;

    public Vector2 move = Vector2.zero;
    public Vector2 look = Vector2.zero;
    public bool nitro = false;

    public float NitroBar = 100;
    public float NitroConsumtionrate = 100;
    public float NitroRefilrate = 100;

    public GameObject Booster1;
    public GameObject Booster2;

    public Light pointlight;
    public Light headlightL;
    public Light headlightR;

    //edward
    //public AudioSource engineAudioSource;
    //public AudioClip engineSound;
    //


    /// <summary>
    /// Tuorism Car 2022/08/27 by POOH
    /// </summary>
    [Space]
    [Header("--------Tuorism--------")]
    [HideInInspector] public bool isOnCar = false;
    public CarBody tourismCar;
    public GameObject tourismCarObj;

    public void MoveInput(InputAction.CallbackContext context)
    {
        //edward
        if (FindObjectOfType<MapManager>() != null && FindObjectOfType<MapManager>().bMap)
        {
            return;
        }
        //

        move = context.ReadValue<Vector2>();
    }
    public void LookInput(InputAction.CallbackContext context)
    {
        //edward
        if (FindObjectOfType<MapManager>() != null && FindObjectOfType<MapManager>().bMap)
        {
            return;
        }
        //

        look = context.ReadValue<Vector2>();
    }

    public void NitroInput(InputAction.CallbackContext context)
    {
        //edward
        if (FindObjectOfType<MapManager>() != null && FindObjectOfType<MapManager>().bMap)
        {
            return;
        }
        //

        if (NitroBar > 10)
            nitro = context.ReadValueAsButton();
    }

    public void RegisterInput()
    {
        GameInputHandler.instance.gameInput.Racer.Move.performed += ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Racer.Move.canceled += ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Racer.Look.performed += ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Racer.Look.canceled += ctx => LookInput(ctx);

        GameInputHandler.instance.gameInput.Racer.Nitro.performed += ctx => NitroInput(ctx);
        GameInputHandler.instance.gameInput.Racer.Nitro.canceled += ctx => NitroInput(ctx);
    }

    public void UnregisterInput()
    {
        GameInputHandler.instance.gameInput.Racer.Move.performed -= ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Racer.Move.canceled -= ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Racer.Look.performed -= ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Racer.Look.canceled -= ctx => LookInput(ctx);

        GameInputHandler.instance.gameInput.Racer.Nitro.performed -= ctx => NitroInput(ctx);
        GameInputHandler.instance.gameInput.Racer.Nitro.canceled -= ctx => NitroInput(ctx);
    }



    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        Active(false);
        if (GameManager.instance != null && GameManager.instance.raceTrack != null)
            pathCreator = GameManager.instance.raceTrack;

        if (view != null && view.IsMine)
        {
            RegisterInput();
        }
        //edward
        //SetupEngineAudio();
        //
    }

    void ActivateBooster(bool active)
    {
        Booster1.SetActive(active);
        Booster2.SetActive(active);
    }

    bool firstTime = true;
    public void Active(bool active)
    {
        //car = carbodys[carindex];
        //edward
        car = carbodys[PlayerPrefs.GetInt("carindex")];
        carmaincontroller.raceCarWheelMeshes[0] = car.tirefront1.gameObject;
        carmaincontroller.raceCarWheelMeshes[1] = car.tireback1.gameObject;
        carmaincontroller.raceCarWheelMeshes[2] = car.tirefront2.gameObject;
        carmaincontroller.raceCarWheelMeshes[3] = car.tireback2.gameObject;
        //

        //carmaincontroller.m_WheelMeshes[0] = car.tirefront1.gameObject;
        //carmaincontroller.m_WheelMeshes[1] = car.tireback1.gameObject;
        //carmaincontroller.m_WheelMeshes[2] = car.tirefront2.gameObject;
        //carmaincontroller.m_WheelMeshes[3] = car.tireback2.gameObject;


        if (view.IsMine)
        {
            if (active)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.carstickmobile.SetActive(true);
                    GameManager.instance.movestickmobile.SetActive(false);
                }
                GameManager.instance.mycontroller.thirdPersonController.enabled = false;


                Vector3 pos = transform.position;

                if (pathCreator != null)
                {
                    pos = pathCreator.path.GetPointAtDistance(40);

                    for (int i = 40; i < pathCreator.path.length; i++)
                    {
                        Vector3 poscheck = pathCreator.path.GetPointAtDistance(i);
                        poscheck.y = 0;
                        bool check = false;
                        for (int j = 0; j < OnlinePLayerManager.instance.playerList.Count; j++)
                        {
                            if (Vector3.Distance(OnlinePLayerManager.instance.playerList[j].transform.position, poscheck) < 10)
                            {
                                check = true;
                                break;
                            }
                        }

                        if (!check)
                        {
                            pos = poscheck;
                            break;
                        }

                    }
                    pos.y = 0;
                }


                //edward
                int index = Random.Range(0, GameManager.instance.racemanager.m_SpawnList.Count);
                //pos = GameManager.instance.racemanager.m_SpawnList[index].position;
                pos = pos + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                //
                transform.position = pos;// GameManager.instance.racemanager.m_SpawnList[index].position;
                transform.LookAt(pos + new Vector3(1, 0, 0));

                if (GameManager.instance != null)
                {
                    GameManager.instance.mycontroller.SwitchRaceCam(true);
                }
                maxdistancecovered = 40;

                GameInputHandler.instance.ActivateRacer();
                GameManager.instance.cameravirtualC.Follow = GameManager.instance.mycontroller.cameraTarget2;
                GameManager.instance.racemanager.isfreecamOn = false;
            }
            else
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.movestickmobile.SetActive(true);
                    GameManager.instance.carstickmobile.SetActive(false);
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    else
                    {
                        if (GameManager.instance.mycontroller != null)
                            GameManager.instance.mycontroller.thirdPersonController.enabled = true;
                    }

                }

                raceready = false;
                if (GameManager.instance != null)
                {
                    GameManager.instance.mycontroller.SwitchRaceCam(false);

                }
                AudioManager.instance.Stop("RACETHROTTLE");
                AudioManager.instance.Stop("TIRESCREECH1");
                AudioManager.instance.Stop("RACECOUNTDOWN");
                AudioManager.instance.Stop("RACEENGINESTART");
                AudioManager.instance.Stop("RACINGTHEMESONG");

                GameInputHandler.instance.ActivatePlayer();
                if (GameManager.instance != null)
                    GameManager.instance.FreeCursor(false);
            }
            GetComponent<CharacterController>().enabled = !active;
            rb.useGravity = active;
            carparts.SetActive(active);
            carColliders.SetActive(active);

        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            car.GetComponent<MeshCollider>().enabled = false;
            carColliders.SetActive(active);
        }


        car.gameObject.SetActive(active);

        for (int i = 0; i < carbodys.Count; i++)
        {
            if (car != carbodys[i])
                carbodys[i].gameObject.SetActive(false);
        }


        this.enabled = active;
        if (mybody != null)
        {
            mybody.SetActive(!active);
        }

        isactive = active;
        carmaincontroller.SetRaceCarMode(isactive);
    }
    /// <summary>
    /// 2022/08/27 by POOH
    /// </summary>
    /// <param name="active"></param>
    public void TakeCar(bool active)
    {
        if (PhotonNetwork.CurrentRoom.Name != "GameScene")
            return;
        car = tourismCar;

        carmaincontroller.m_WheelMeshes[0] = car.tirefront1.gameObject;
        carmaincontroller.m_WheelMeshes[1] = car.tireback1.gameObject;
        carmaincontroller.m_WheelMeshes[2] = car.tirefront2.gameObject;
        carmaincontroller.m_WheelMeshes[3] = car.tireback2.gameObject;


        if (view.IsMine)
        {
            if (active)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.carstickmobile.SetActive(true);
                    GameManager.instance.movestickmobile.SetActive(false);
                    GameManager.instance.mycontroller.SwitchRaceCam(true);
                    if (GameManager.instance.mycontroller != null)
                    {
                        GameManager.instance.mycontroller.thirdPersonController.enabled = false;
                    }
                    //edward
                    GameManager.instance.racemanager.SetupEngineAudio();
                    //
                }

                GameInputHandler.instance.ActivateRacer();
                GameManager.instance.cameravirtualC.Follow = GameManager.instance.mycontroller.cameraTarget2;
                //GameManager.instance.racemanager.isfreecamOn = false;
            }
            else
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.movestickmobile.SetActive(true);
                    GameManager.instance.carstickmobile.SetActive(false);
                    GameManager.instance.mycontroller.SwitchRaceCam(false);
                    if (GameManager.instance.mycontroller != null)
                    {
                        GameManager.instance.mycontroller.thirdPersonController.enabled = true;
                    }
                    //edward
                    GameManager.instance.racemanager.StopEngineAudio();
                    //
                }
                GameInputHandler.instance.ActivatePlayer();
                if (GameManager.instance != null)
                    GameManager.instance.FreeCursor(false);
            }
            GetComponent<CharacterController>().enabled = !active;
            rb.useGravity = active;
            carparts.SetActive(active);
            carColliders.SetActive(active);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            carColliders.SetActive(false);
        }

        carmaincontroller.SetTourCarMode(active);
        car.gameObject.SetActive(active);
        tourismCarObj.SetActive(active);

        for (int i = 0; i < carbodys.Count; i++)
        {
            if (car != carbodys[i])
                carbodys[i].gameObject.SetActive(false);
        }


        this.enabled = active;
        if (mybody != null)
            mybody.SetActive(!active);
        isOnCar = active;

        //isactive = active;
    }

    //edward
    public void EngineSound()
    {
        GameManager.instance.racemanager.engineAudioSource.pitch = Mathf.MoveTowards(GameManager.instance.racemanager.engineAudioSource.pitch, carmaincontroller.CurrentSpeed / carmaincontroller.MaxSpeed / 3f + 0.3f, 0.05f);

        GameManager.instance.racemanager.engineAudioSource.volume = Mathf.MoveTowards(GameManager.instance.racemanager.engineAudioSource.volume, 0.7f + Mathf.Abs(move.y) / 2, 0.05f);

        //Debug.Log(carmaincontroller.CurrentSpeed.ToString() + ", " + carmaincontroller.MaxSpeed.ToString());
    }
    //

    private void FixedUpdate()
    {
        if (!view.IsMine)
            return;

        //edward
        if (tourismCarObj.activeSelf)
        {
            EngineSound();
        }
        //

        if (!raceready)
            return;

        //edward
        if (RaceManager.isstartrace)
        {
            if(!GameManager.instance.racemanager.isStart)
            {
                return;
            }
        }
        //


            //Debug.Log("Angle " + Mathf.Acos(Vector3.Dot(Vector3.up, transform.up) / (Vector3.up.magnitude * transform.up.magnitude)));
        if (Mathf.Acos(Vector3.Dot(Vector3.up, transform.up) / (Vector3.up.magnitude * transform.up.magnitude)) > 45 / 180.0 * Mathf.PI)
        {
            Vector3 right = transform.up;
            right.y = 0;
            right = right.normalized;
            transform.up = ((Vector3.up - right) / 2).normalized;
        }


        /*if (rb.velocity.magnitude >= MaxSpeed*(nitro?1.25f:1))
        {
            if(Vector3.Dot(rb.velocity, transform.forward)>0)
                rb.velocity = MaxSpeed * (nitro ? 1.25f : 1) * transform.forward;
            else
                rb.velocity = MaxSpeed * (nitro ? 1.25f : 1) * -transform.forward;
        }
        else
        {
            rb.AddForce(moveForce * direction * transform.forward+ (nitro ? 2 : 0)*moveForce * transform.forward, ForceMode.Force);   
        }

        if(direction>=0)
        {
            Vector3 newdirect = Vector3.Lerp(transform.forward, rb.velocity, Time.fixedDeltaTime*5);
            rb.velocity = newdirect.normalized * rb.velocity.magnitude;
        }
        

        if(rb.velocity.magnitude>1)
            transform.rotation *= Quaternion.Euler(0, stearing*45*Time.fixedDeltaTime, 0);
        car.Speed(rb.velocity.magnitude,true);
        car.Stearing(stearing, true);*/
    }
    float stearing;
    public float direction = 1;
    private void OnEnable()
    {
        pointlight.gameObject.SetActive(true);
        headlightL.gameObject.SetActive(true);
        headlightR.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        pointlight.gameObject.SetActive(false);
        headlightL.gameObject.SetActive(false);
        headlightR.gameObject.SetActive(false);
    }
    // Update is called once per frame
    float lastwheelturn;
    void Update()
    {
        //edward
        if (RaceManager.isstartrace)
        {
            if (!GameManager.instance.racemanager.isStart)
            {
                return;
            }
        }
        //

        if (RenderSettings.ambientIntensity < 0.25f)
        {
            pointlight.gameObject.SetActive(true);
            headlightL.gameObject.SetActive(true);
            headlightR.gameObject.SetActive(true);
        }
        else
        {
            pointlight.gameObject.SetActive(false);
            headlightL.gameObject.SetActive(false);
            headlightR.gameObject.SetActive(false);
        }

        if (nameplate != null && nameposition != null)
        {
            if (isOnCar)
            {
                nameplate.localScale = new Vector3(-5.0f, 5.0f, 1);
                nameplate.position = new Vector3(transform.position.x, nameposition.position.y + 2.0f, transform.position.z);
            }
            else
            {
                nameplate.localScale = new Vector3(-5.0f, 5.0f, 1);
                nameplate.position = new Vector3(transform.position.x, nameposition.position.y, transform.position.z);
            }

        }
        ActivateBooster(nitro);
        if (!view.IsMine)
            return;

        if (nitro && NitroBar > 0)
        {
            if (GameManager.instance != null)
                GameManager.instance.rvolume.enabled = true;
            NitroBar -= NitroConsumtionrate * Time.deltaTime;
            rb.drag = 0;
            carmaincontroller.m_Topspeed = 200;
        }
        else if (nitro && NitroBar < 0)
        {
            nitro = false;
            rb.drag = 0.05f;
            carmaincontroller.m_Topspeed = 125;
        }
        else
        {
            if (GameManager.instance != null)
                GameManager.instance.rvolume.enabled = false;
            NitroBar += NitroRefilrate * Time.deltaTime;
            if (NitroBar > 100)
                NitroBar = 100;
            rb.drag = 0.05f;
            carmaincontroller.m_Topspeed = 125;
        }

        if (isOnCar)
        {
            if (carmaincontroller.CurrentSpeed < 1f)
            {
                carmaincontroller.m_Topspeed = 125;
            }
            else
            {
                carmaincontroller.m_Topspeed = 70;
            }
        }
        if (GameManager.instance != null)
            GameManager.instance.racemanager.NitroMeter(NitroBar);
        if (GameManager.instance != null)
            GameManager.instance.mycontroller.thirdPersonController.look = look;
        //if (GameManager.instance != null)
        //    GameManager.instance.FreeCursor(false);//2022/09/04 cai
        looktarget.LookAt(looktarget.position + transform.forward - transform.up / 10);
        moveForce = 0;

        if (lastwheelturn != 0 && lastwheelturn == move.x * -1)
        {
            carmaincontroller.m_SteerHelper = 0.4f;
        }


        if (carmaincontroller.CurrentSpeed > 10f)
        {
            if (move.x != 0)
                carmaincontroller.m_SteerHelper += Time.deltaTime * .5f;
            else
                carmaincontroller.m_SteerHelper -= Time.deltaTime * .5f;

        }

        carmaincontroller.m_SteerHelper = Mathf.Clamp(carmaincontroller.m_SteerHelper, 0.4f, 1.0f);
        carmaincontroller.Move(move.x, move.y, move.y, 0f);

        if (move.y != 0)
        {
            //edward
            direction = move.y;
            //
            moveForce = 25 - (move.y > 0 ? 0 : 10);
        }

        if (move.x != 0)
        {
            //edward
            stearing += move.x * Time.deltaTime / 2;
            //
        }
        else
        {
            if (stearing > 0)
                stearing -= 1.0f * Time.deltaTime;
            else
                stearing += 1.0f * Time.deltaTime;
        }

        if (stearing > 1.0f)
            stearing = 1.0f;
        if (stearing < -1.0f)
            stearing = -1.0f;
    }

    public float maxdistancecovered = 0;

    public string wrongway = "";

    public float flimtimar = 2;
    private void LateUpdate()
    {
        if (!view.IsMine)
            return;

        //edward
        if (RaceManager.isstartrace)
        {
            if (!GameManager.instance.racemanager.isStart)
            {
                return;
            }
        }
        //

        if (pathCreator != null)
        {
            if (isactive)
            {
                Vector3 pointpos = pathCreator.path.GetClosestPointOnPath(transform.position);
                float currentdistance = Vector3.Distance(pointpos, transform.position);
                float maxdist = pathCreator.path.GetClosestDistanceAlongPath(pointpos);

                Debug.Log(maxdistancecovered.ToString() + " " + maxdist.ToString());

                if (maxdistancecovered < maxdist)
                    maxdistancecovered = maxdist;

                if(maxdistancecovered > (maxdist + 1000))
                {
                    maxdistancecovered = maxdist;
                }
                    

                if (maxdistancecovered - 10 > maxdist)
                {
                    wrongway = "Wrong way.";

                }
                else
                {
                    wrongway = "";

                }
                //if (currentdistance > maxdist)
                //{
                //    wrongway = "Wrong way.";

                //}
                //else
                //{
                //    wrongway = "";

                //}

                //edward
                //if (goingWrongway)
                //{
                //    wrongway = "Wrong way.";

                //}
                //else
                //{
                //    wrongway = "";

                //}                
                //

                if (Mathf.Acos(Vector3.Dot(Vector3.up, transform.up) / (Vector3.up.magnitude * transform.up.magnitude)) > 45 / 180.0 * Mathf.PI)
                {
                    flimtimar -= Time.deltaTime;
                    if (flimtimar < 0)
                    {
                        flimtimar = 2;

                        Vector3 pos = pathCreator.path.GetPointAtDistance(40);

                        for (int i = (int)currentdistance; i > 40; i--)
                        {
                            Vector3 poscheck = pathCreator.path.GetPointAtDistance(i);
                            poscheck.y = 1;
                            bool check = false;
                            for (int j = 0; j < OnlinePLayerManager.instance.playerList.Count; j++)
                            {
                                if (Vector3.Distance(OnlinePLayerManager.instance.playerList[j].transform.position, poscheck) < 10)
                                {
                                    check = true;
                                    break;
                                }
                            }

                            if (!check)
                            {
                                pos = poscheck;
                                break;
                            }

                        }
                        pos.y = 1;

                        transform.position = pos;
                        transform.LookAt(pos + new Vector3(1, 0, 0));

                    }
                }

                if (isactive)
                {
                    if (currentdistance > MaxDistance)
                    {
                        transform.position = pointpos + (transform.position - pointpos).normalized * MaxDistance;
                    }
                }
                else
                {
                    if (currentdistance < (MaxDistance + 5))
                    {
                        transform.position = pointpos + (transform.position - pointpos).normalized * (MaxDistance + 5);
                    }
                }

            }




        }
    }

    //edward
    public bool goingWrongway = false;
    public float wrongwayTimer = 0;
    public void CheckForWrongway()
    {
        Vector3 pointpos = pathCreator.path.GetClosestPointOnPath(transform.position);
        //float currentdistance = Vector3.Distance(pointpos, transform.position);
        //float maxdist = pathCreator.path.GetClosestDistanceAlongPath(pointpos);

        float nodeAngle = pointpos.y;

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
    //
    private void OnDestroy()
    {
        UnregisterInput();
    }

}
