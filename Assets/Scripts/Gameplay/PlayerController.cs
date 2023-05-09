using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using Photon.Chat.Demo;
using AdvancedPeopleSystem;
using System.Threading.Tasks;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Equipping;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IPunObservable
{
    public enum playerState
    {
        idle = 0,
        walking = 1,
        running = 2,
        riding = 3,
        chatting = 4,
        fighting = 5,
        racing = 6,
        navigating = 7,
        undefine = 8,
    }
    public playerState currentState;
    AnimatorController animcontroller;
    public ThirdPersonController thirdPersonController;
    public Animator animator;
    public Text NameText;
    public Image NameTextImage;

    public Text HealthText;

    public TMPro.TextMeshPro rtext;

    public Transform HealthBack;
    public Image HealthFront;

    public Transform nametarget;
    public Transform cameraTarget;
    public Transform cameraTarget2;

    public bool isdummy = false;

    public IUseable useable;

    public string playerName = "";
    public string playerEmail = "";

    public string playerMassage = "";
    public string playerlastMassage = "";

    public PhotonView view;


    public Transform nameplate;
    public Transform nameposition;

    Transform camtarget;

    public bool AudioOn = false;

    public bool AdminAudioOn = false;

    public bool Muted = false;

    public Transform audiosource;

    public AudioSource watersource;

    public AudioSource jumpstartsource;
    public AudioSource jumpendsource;
    public AudioSource jumpwatersource;

    public bool isKicked = false;

    public int life = 0;

    public int blendShapeIndex = 0;

    public CharacterCustomization characterCustomization;

    public float scalemodifier;

    public GameObject handObject;
    public bool isblocked = false;
    public bool isotherblocked = false;
    public bool ispreviousblocked = false;

    PrivateZone zone;

    public Transform handone;
    public Transform handtwo;
    public Transform legone;
    public Transform legtwo;
    public CarCaracterController carcont;

    public GameObject HandPower;
    public GameObject LegPower;
    public GameObject MagicPower;

    public float punchtime = 0.5f;
    public float kicktime = 0.75f;
    public float magictime = 1.75f;

    public GameObject racecamminmap;

    public bool isalive = true;

    public InwaterIndicator topindicator;
    public InwaterIndicator bottomindicator;

    //check attacking mode

    public bool isAttackingMode = false;
    Vector2 moveBuffer;

    //PlayerNormalInput
    public void MoveInput(InputAction.CallbackContext context)
    {
        moveBuffer = context.ReadValue<Vector2>();
        if (!isAttackingMode)
        {

            thirdPersonController.move = context.ReadValue<Vector2>();

#if UNITY_ANDROID || UNITY_IOS
            if(thirdPersonController.move.magnitude>0.75f)
            {
                thirdPersonController.sprint = true;
            }
            else
            {
                thirdPersonController.sprint = false;
            }
#endif

        }
        else
        {
            thirdPersonController.move = new Vector2(0, 0);

        }


    }
    public void LookInput(InputAction.CallbackContext context)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        Vector2 vec= context.ReadValue<Vector2>();
        
        thirdPersonController.look = new Vector2(Mathf.Abs(vec.x) > 0.01f?(vec.x/Mathf.Abs(vec.x)*50):0, Mathf.Abs(vec.y) > 0.01f ? (vec.y / Mathf.Abs(vec.y) * 50) : 0);
#else
        thirdPersonController.look= context.ReadValue<Vector2>();
#endif
    }

    bool ismovezone;
    public void LookTouchInput(InputAction.CallbackContext context)
    {

        if(Vector2.Distance(Vector2.zero, context.ReadValue<Touch>().position)<300)
        {
            ismovezone = true;
        }
        else
        {
            ismovezone = false;
        }
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        thirdPersonController.jump = context.ReadValueAsButton();
    }

    public void ClimbInput(InputAction.CallbackContext context)
    {
        bool ispressed = context.ReadValueAsButton();
        Debug.Log("Is pressed"+ ispressed);
        if (ispressed && inwall)
        {
            Debug.Log("Is pressed");
            if (animcontroller.ChangeAnimationStateWithLock("Base Layer.Climb"))
            {
                
                animator.SetBool("Climb", true);
                animator.applyRootMotion = true;

                AnimationFunction animfunction = new AnimationFunction();
                animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
                animfunction.animtime = AimeTime.INFIRSTBETWEEN;
                animfunction.time = 0.25f;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.onComplete = () =>
                {

                    animator.applyRootMotion = false;
                };
                animfunction.animtime = AimeTime.END;
                animcontroller.AddAction(animfunction);
            }
        }
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        thirdPersonController.sprint = context.ReadValueAsButton();
    }

    public void NormalKickInput(InputAction.CallbackContext context)
    {
        bool kick = context.ReadValueAsButton();
        animator.SetBool("KICK", kick);
        if (kick)
            useable?.Use(transform);
    }
    public void SwitchCameraInput(InputAction.CallbackContext context)
    {
        if (vcamobj.activeSelf)
        {
            vcamobj.SetActive(false);
            vcamfobj.SetActive(true);
        }
        else
        {
            vcamobj.SetActive(true);
            vcamfobj.SetActive(false);
        }

    }
    //PlayerArenaInput
    public void PunchInput(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            //SetAnimation("BoxingPunch");
            if (animcontroller.ChangeAnimationStateWithLock("Base Layer.BoxingPunch"))
            {

                isAttackingMode = true;
                animator.SetBool("BoxingPunch", true);
                animator.applyRootMotion = true;

                AnimationFunction animfunction = new AnimationFunction();
                animfunction.action = () => { animator.SetBool("BoxingPunch", false); return DamegeOtherPlayer(0, 0.5f, 20, handone, HandPower); };
                animfunction.animtime = AimeTime.INFIRSTBETWEEN;
                animfunction.time = 0.25f;
                Debug.Log("Attacking Mode INFIRSTBETWEEEN: " + isAttackingMode);

                animcontroller.AddAction(animfunction);
                animfunction = new AnimationFunction();
                animfunction.action = () => { DamegeOtherPlayerMissed(); return false; };
                animfunction.onComplete = () =>
                {
                    isAttackingMode = false;
                    animator.SetBool("BoxingPunch", false);

                };
                animfunction.animtime = AimeTime.MIDDLE;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
                animfunction.animtime = AimeTime.INBETWEEN;
                animfunction.time = 0;
                animfunction.endtime = 0.25f;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.onComplete = () =>
                {

                    animator.applyRootMotion = false;
                };
                animfunction.animtime = AimeTime.END;
                animcontroller.AddAction(animfunction);


            }

            //Debug.Log(length);
        }
    }
    public void PunchInput1(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            LongHookPunch();
        }
    }
    public void KickInput(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            //SetAnimation("BoxingKick");
            if (animcontroller.ChangeAnimationStateWithLock("Base Layer.BoxingKick"))
            {
                isAttackingMode = true;
                animator.SetBool("BoxingKick", true);
                animator.applyRootMotion = true;

                AnimationFunction animfunction = new AnimationFunction();
                animfunction.action = () => { animator.SetBool("BoxingKick", false); return DamegeOtherPlayer(0, 0.5f, 40, legone, LegPower); };
                animfunction.animtime = AimeTime.INBETWEEN;
                animfunction.time = 0.25f;
                animfunction.endsubtracttime = 0.25f;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.action = () => { DamegeOtherPlayerMissed(); return false; };
                animfunction.onComplete = () =>
                {

                    animator.SetBool("BoxingKick", false);
                    isAttackingMode = false;
                };
                animfunction.animtime = AimeTime.MIDDLE;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
                animfunction.animtime = AimeTime.INBETWEEN;
                animfunction.time = 0;
                animfunction.endtime = 0.25f;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.onComplete = () =>
                {

                    animator.applyRootMotion = false;
                };
                animfunction.animtime = AimeTime.END;
                animcontroller.AddAction(animfunction);

            }
        }
    }

    public void KickInput1(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            SideKick();
        }
    }

    public void KickInput2(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            SpinningBackKick();
        }
    }

    public void PowerAttackInput(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            //SetAnimation("BoxingMagic");
            if (animcontroller.ChangeAnimationStateWithLock("Base Layer.BoxingMagic"))
            {
                isAttackingMode = true;
                animator.SetBool("BoxingMagic", true);
                animator.applyRootMotion = true;

                AnimationFunction animfunction = new AnimationFunction();
                animfunction.action = () => { animator.SetBool("BoxingMagic", false); return DamegeOtherPlayer(0, 4, 90, transform, MagicPower); };
                animfunction.animtime = AimeTime.MIDDLE;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.action = () => { DamegeOtherPlayerMissed(); return false; };
                animfunction.onComplete = () =>
                {

                    animator.SetBool("BoxingMagic", false);
                    isAttackingMode = false;

                };
                animfunction.animtime = AimeTime.MIDDLE;
                animcontroller.AddAction(animfunction);

                animfunction = new AnimationFunction();
                animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
                animfunction.animtime = AimeTime.INBETWEEN;
                animfunction.time = 0;
                animfunction.endtime = 0.25f;
                animcontroller.AddAction(animfunction);


                animfunction = new AnimationFunction();
                animfunction.onComplete = () =>
                {

                    animator.applyRootMotion = false;
                };
                animfunction.animtime = AimeTime.END;
                animcontroller.AddAction(animfunction);


            }
        }

    }

    public void ComboAttackInput(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            ComboSpecialSkill();
        }

    }

    //test new input methods
    public void LongHookPunch()
    {
        if (animcontroller.ChangeAnimationStateWithLock("Base Layer.LongHookPunch"))
        {
            isAttackingMode = true;
            animator.applyRootMotion = true;
            animator.SetBool("LongHookPunch", true);
            AnimationFunction animfunction = new AnimationFunction();
            animfunction.action = () => { animator.SetBool("LongHookPunch", false); return DamegeOtherPlayer(0, 0.5f, 25, handtwo, HandPower); };
            animfunction.animtime = AimeTime.INBETWEEN;
            animfunction.time = 0.75f;
            animfunction.endsubtracttime = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { DamegeOtherPlayerMissed(); return false; };
            animfunction.onComplete = () =>
            {

                animator.SetBool("LongHookPunch", false);
                isAttackingMode = false;
            };
            animfunction.animtime = AimeTime.MIDDLE;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
            animfunction.animtime = AimeTime.INBETWEEN;
            animfunction.time = 0;
            animfunction.endtime = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.onComplete = () =>
            {

                animator.applyRootMotion = false;
            };
            animfunction.animtime = AimeTime.END;
            animcontroller.AddAction(animfunction);

        }
    }
    public void SideKick()
    {
        if (animcontroller.ChangeAnimationStateWithLock("Base Layer.SideKick"))
        {
            isAttackingMode = true;
            animator.SetBool("SideKick", true);
            animator.applyRootMotion = true;

            AnimationFunction animfunction = new AnimationFunction();
            animfunction.action = () => { animator.SetBool("SideKick", false); return DamegeOtherPlayer(0, 0.5f, 40, legone, LegPower); };
            animfunction.animtime = AimeTime.INBETWEEN;
            animfunction.time = 0.5f;
            animfunction.endsubtracttime = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { DamegeOtherPlayerMissed(); return false; };
            animfunction.onComplete = () =>
            {

                animator.SetBool("SideKick", false);
                isAttackingMode = false;
            };
            animfunction.animtime = AimeTime.MIDDLE;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
            animfunction.animtime = AimeTime.INBETWEEN;
            animfunction.time = 0;
            animfunction.endtime = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.onComplete = () =>
            {

                animator.applyRootMotion = false;
            };
            animfunction.animtime = AimeTime.END;
            animcontroller.AddAction(animfunction);

        }
    }
    public void SpinningBackKick()
    {
        if (animcontroller.ChangeAnimationStateWithLock("Base Layer. SpinningBackKick "))
        {
            isAttackingMode = true;
            animator.SetBool(" SpinningBackKick ", true);

            animator.applyRootMotion = true;

            AnimationFunction animfunction = new AnimationFunction();
            animfunction.action = () => { animator.SetBool(" SpinningBackKick ", false); return DamegeOtherPlayer(0, 0.5f, 50, legtwo, LegPower); };
            animfunction.animtime = AimeTime.INBETWEEN;
            animfunction.time = 0.25f;
            animfunction.endsubtracttime = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { DamegeOtherPlayerMissed(); return false; };
            animfunction.onComplete = () =>
            {

                animator.SetBool(" SpinningBackKick ", false);
                isAttackingMode = false;
            };
            animfunction.animtime = AimeTime.MIDDLE;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
            animfunction.animtime = AimeTime.INBETWEEN;
            animfunction.time = 0;
            animfunction.endtime = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
           
            animfunction.onComplete = () =>
            {

                animator.applyRootMotion = false;

            };
            animfunction.animtime = AimeTime.END;
            animcontroller.AddAction(animfunction);

        }
    }
    public void ComboSpecialSkill()
    {
        if (animcontroller.ChangeAnimationStateWithLock("Base Layer.ComboSpecialSkill"))
        {
            isAttackingMode = true;
            animator.SetBool("ComboSpecialSkill", true);
            animator.applyRootMotion = true;
            AnimationFunction animfunction = new AnimationFunction();
            animfunction.action = () => { animator.SetBool("ComboSpecialSkill", false); return DamegeOtherPlayer(0, 0.5f, 40, handone, HandPower); };
            animfunction.animtime = AimeTime.INFIRSTBETWEEN;
            animfunction.time = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { DamegeOtherPlayerMissed(); return false; };
            animfunction.onComplete = () =>
            {

                animator.SetBool("ComboSpecialSkill", false);
                isAttackingMode = false;
            };
            animfunction.animtime = AimeTime.MIDDLE;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.action = () => { thirdPersonController.move = new Vector2(0, 1); return false; };
            animfunction.animtime = AimeTime.INBETWEEN;
            animfunction.time = 0;
            animfunction.endtime = 0.25f;
            animcontroller.AddAction(animfunction);

            animfunction = new AnimationFunction();
            animfunction.onComplete = () =>
            {

                animator.applyRootMotion = false;
            };
            animfunction.animtime = AimeTime.END;
            animcontroller.AddAction(animfunction);

        }
    }

   

    public bool isBlocked;
    public void PowerBlockInput(InputAction.CallbackContext context)
    {
        isBlocked = context.ReadValueAsButton();

    }

    public void RegisterInput()
    {
        RegisterNormalInput();
        RegisterArenaInput();
        RegisterSwimingInput();
    }

    public void UnregisterInput()
    {
        UnregisterNormalInput();
        UnregisterArenaInput();
        UnregisterSwimingInput();
    }
    public void RegisterNormalInput()
    {
        GameInputHandler.instance.gameInput.Player.Move.performed += ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Player.Move.canceled += ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Player.Look.performed += ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Player.Look.canceled += ctx => LookInput(ctx);

        

        GameInputHandler.instance.gameInput.Player.Jump.performed += ctx => JumpInput(ctx);
        GameInputHandler.instance.gameInput.Player.Jump.canceled += ctx => JumpInput(ctx);

        GameInputHandler.instance.gameInput.Player.Sprint.performed += ctx => SprintInput(ctx);
        GameInputHandler.instance.gameInput.Player.Sprint.canceled += ctx => SprintInput(ctx);

        GameInputHandler.instance.gameInput.Player.Kick.performed += ctx => NormalKickInput(ctx);
        GameInputHandler.instance.gameInput.Player.Kick.canceled += ctx => NormalKickInput(ctx);

        GameInputHandler.instance.gameInput.Player.SwitchCamera.performed += ctx => SwitchCameraInput(ctx);
    }

    public void UnregisterNormalInput()
    {
        GameInputHandler.instance.gameInput.Player.Move.performed -= ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Player.Move.canceled -= ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Player.Look.performed -= ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Player.Look.canceled -= ctx => LookInput(ctx);



        GameInputHandler.instance.gameInput.Player.Jump.performed -= ctx => JumpInput(ctx);
        GameInputHandler.instance.gameInput.Player.Jump.canceled -= ctx => JumpInput(ctx);

        GameInputHandler.instance.gameInput.Player.Sprint.performed -= ctx => SprintInput(ctx);
        GameInputHandler.instance.gameInput.Player.Sprint.canceled -= ctx => SprintInput(ctx);

        GameInputHandler.instance.gameInput.Player.Kick.performed -= ctx => NormalKickInput(ctx);
        GameInputHandler.instance.gameInput.Player.Kick.canceled -= ctx => NormalKickInput(ctx);

        GameInputHandler.instance.gameInput.Player.SwitchCamera.performed -= ctx => SwitchCameraInput(ctx);
    }

    public void RegisterArenaInput()
    {
        GameInputHandler.instance.gameInput.Fighter.Move.performed += ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Move.canceled += ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Look.performed += ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Look.canceled += ctx => LookInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Jump.performed += ctx => JumpInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Jump.canceled += ctx => JumpInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Sprint.performed += ctx => SprintInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Sprint.canceled += ctx => SprintInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Punch.performed += ctx => PunchInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Punch.canceled += ctx => PunchInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Punch1.performed += ctx => PunchInput1(ctx);
        GameInputHandler.instance.gameInput.Fighter.Punch1.canceled += ctx => PunchInput1(ctx);

        GameInputHandler.instance.gameInput.Fighter.Kick.performed += ctx => KickInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Kick.canceled += ctx => KickInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Kick1.performed += ctx => KickInput1(ctx);
        GameInputHandler.instance.gameInput.Fighter.Kick1.canceled += ctx => KickInput1(ctx);

        GameInputHandler.instance.gameInput.Fighter.Kick2.performed += ctx => KickInput2(ctx);
        GameInputHandler.instance.gameInput.Fighter.Kick2.canceled += ctx => KickInput2(ctx);

        GameInputHandler.instance.gameInput.Fighter.Block.performed += ctx => PowerBlockInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Block.canceled += ctx => PowerBlockInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Power.performed += ctx => PowerAttackInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Power.canceled += ctx => PowerAttackInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Combo.performed += ctx => ComboAttackInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Combo.canceled += ctx => ComboAttackInput(ctx);

    }

    public void UnregisterArenaInput()
    {
        GameInputHandler.instance.gameInput.Fighter.Move.performed -= ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Move.canceled -= ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Look.performed -= ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Look.canceled -= ctx => LookInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Jump.performed -= ctx => JumpInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Jump.canceled -= ctx => JumpInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Sprint.performed -= ctx => SprintInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Sprint.canceled -= ctx => SprintInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Punch.performed -= ctx => PunchInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Punch.canceled -= ctx => PunchInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Punch1.performed -= ctx => PunchInput1(ctx);
        GameInputHandler.instance.gameInput.Fighter.Punch1.canceled -= ctx => PunchInput1(ctx);

        GameInputHandler.instance.gameInput.Fighter.Kick.performed -= ctx => KickInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Kick.canceled -= ctx => KickInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Kick1.performed -= ctx => KickInput1(ctx);
        GameInputHandler.instance.gameInput.Fighter.Kick1.canceled -= ctx => KickInput1(ctx);

        GameInputHandler.instance.gameInput.Fighter.Kick2.performed -= ctx => KickInput2(ctx);
        GameInputHandler.instance.gameInput.Fighter.Kick2.canceled -= ctx => KickInput2(ctx);

        GameInputHandler.instance.gameInput.Fighter.Block.performed -= ctx => PowerBlockInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Block.canceled -= ctx => PowerBlockInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Power.performed -= ctx => PowerAttackInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Power.canceled -= ctx => PowerAttackInput(ctx);

        GameInputHandler.instance.gameInput.Fighter.Combo.performed -= ctx => ComboAttackInput(ctx);
        GameInputHandler.instance.gameInput.Fighter.Combo.canceled -= ctx => ComboAttackInput(ctx);

    }

    public void RegisterSwimingInput()
    {
        GameInputHandler.instance.gameInput.Swimmer.Move.performed += ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Swimmer.Move.canceled += ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Swimmer.Look.performed += ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Swimmer.Look.canceled += ctx => LookInput(ctx);

        GameInputHandler.instance.gameInput.Swimmer.Jump.performed += ctx => ClimbInput(ctx);
        GameInputHandler.instance.gameInput.Swimmer.Jump.canceled += ctx => ClimbInput(ctx);
    }

    public void UnregisterSwimingInput()
    {
        GameInputHandler.instance.gameInput.Swimmer.Move.performed -= ctx => MoveInput(ctx);
        GameInputHandler.instance.gameInput.Swimmer.Move.canceled -= ctx => MoveInput(ctx);

        GameInputHandler.instance.gameInput.Swimmer.Look.performed -= ctx => LookInput(ctx);
        GameInputHandler.instance.gameInput.Swimmer.Look.canceled -= ctx => LookInput(ctx);

        GameInputHandler.instance.gameInput.Swimmer.Jump.performed -= ctx => ClimbInput(ctx);
        GameInputHandler.instance.gameInput.Swimmer.Jump.canceled -= ctx => ClimbInput(ctx);
    }


    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animcontroller = GetComponent<AnimatorController>();
        zone = GetComponent<PrivateZone>();
        NameTextImage = NameText.transform.parent.GetComponent<Image>();

        if (!GameManager.IngameScene())
            NameTextImage.enabled = false;

        camtarget = GameObject.FindWithTag("MainCamera").transform;
        if (handObject != null)
            handObject.GetComponent<SphereCollider>().enabled = false;
        if (view != null && view.IsMine && !view.IsRoomView)
        {
            //Debug.Log("asasdasdasdasdasd");
            playerName = ServerManager.instance.username;
            playerEmail = ServerManager.instance.email;
            GetComponent<SphereCollider>().enabled = false;
            //ServerManager.instance.UpdateFightingData(50);
            //ServerManager.instance.UpdateRacingData(50000);
            rtext.color = Color.blue;
            RegisterInput();
            if (RaceManager.isstartrace)
            {
                StartCoroutine(latestartrace());
            }
        }
        else if (view != null && !view.IsMine)
        {
            rtext.color = Color.red;
            GetComponent<SphereCollider>().enabled = true;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<ThirdPersonController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<StarterAssetsInputs>().enabled = false;
            OnlinePLayerManager.instance?.AddPlayer(this);
            racecamminmap.SetActive(false); 
        }
        if(InventorySystemManager.GetDisplayPanelManager() != null)
            InventorySystemManager.GetDisplayPanelManager().Initialize(false);

    }

    IEnumerator latestartrace()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.racemanager.StartRace();
    }

    public static Vector3 mainplayerposition = Vector3.zero;
    public void Kick()
    {
        view.RPC("NetKick", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void NetKick()
    {
        isKicked = true;
    }

    public void OtherBlocK(bool block)
    {
        view.RPC("NetOtherBlocK", RpcTarget.AllBufferedViaServer, ServerManager.instance.playerData.email, block);
    }

    [PunRPC]
    public void NetOtherBlocK(string str, bool block)
    {
        if (block)
            ServerManager.instance.otherblocklist.Add(str);
        else
            ServerManager.instance.otherblocklist.Remove(str);
    }

    public bool ispunched = false;
    public void Punched(int damage)
    {
        if (isblocked || isotherblocked)
            return;
        view.RPC("NetPunched", RpcTarget.AllBufferedViaServer, damage);
    }

    public void PlayAudio(string str, Vector3 position)
    {
        view.RPC("NetPlayAudio", RpcTarget.AllBufferedViaServer, str, position);
    }


    [PunRPC]
    public void NetPlayAudio(string str, Vector3 position)
    {
        if (inArena && Vector3.Distance(position, mainplayerposition) < 25)
            AudioManager.instance.Play(str);
    }
    public bool MatchOver = false;
    [PunRPC]
    public void NetPunched(int damage)
    {
        ispunched = true;
        AudioManager.instance.Play("PUNCH_" + Random.Range(1, 8));
        if(isBlocked)
        {
            /*if (animcontroller.ChangeAnimationState("Base Layer.BoxingBlock"))
            {
                animator.SetBool("BoxingBlock", true);
                AnimationFunction animfunction = new AnimationFunction();
                animfunction.action = () => {
                    animator.SetBool("BoxingBlock", false);
                    return false;
                };
                animfunction.animtime = AimeTime.END;
                animcontroller.AddAction(animfunction);
            }*/
            damage /= 2;
        }
        else
        {
            if (!animcontroller.IsCurrentState("Base Layer.BoxingDeath") && animcontroller.ChangeAnimationState("Base Layer.BoxingPunched"))
            {
                isAttackingMode = false;

                animator.SetBool("BoxingPunched", true);
                AnimationFunction animfunction = new AnimationFunction();
                animfunction.action = () =>
                {
                    animator.SetBool("BoxingPunched", false);
                    return false;
                };
             
                animfunction.animtime = AimeTime.END;
                animcontroller.AddAction(animfunction);
            }
        }
        life -= damage;
        if(life<=0)
        {
            if(isalive)
            {
                AudioManager.instance.Stop("Health_25");
                AudioManager.instance.Stop("Health_10");
                AudioManager.instance.Play("KNOCKOUT");
                animator.SetBool("BoxingDeath", true);
                if (animcontroller.ChangeAnimationState("Base Layer.BoxingDeath"))
                {
                    AnimationFunction animfunction = new AnimationFunction();
                    animfunction.action = () => {

                        GameManager.othermapposition = new Vector3(-192.992676f, 18.6999989f, -100.036316f);
                        NetworkManager.instance.ChangeScene("GameScene");
                        return false;
                    };
                    animfunction.onComplete = () =>
                    {
                        Debug.Log("onComplete lambda called");
                        animator.SetBool("BoxingDeath", false);

                        //isalive = true;
                    };
                    
                    animfunction.animtime = AimeTime.END;
                    animcontroller.AddAction(animfunction);

                }
                isalive = false;
            }
            MatchOver = true;
        }
        else if(life<=500*0.10f)
        {
            AudioManager.instance.Stop("Health_25");
            AudioManager.instance.Playcontinue("Health_10");
        }
        else if(life<=500 * 0.25f)
        {
            AudioManager.instance.Playcontinue("Health_25");
        }
    }

    float maxheight = 0;
    // Update is called once per frame
    void Update()
    {
        if(nameplate!=null && nameposition!=null)
        {
            nameplate.localScale = new Vector3(-1,1,1);

            if(maxheight< nameposition.position.y- transform.position.y)
            {
                maxheight = nameposition.position.y - transform.position.y;
            }

            nameplate.position = new Vector3(transform.position.x, transform.position.y+ maxheight+0.2f, transform.position.z);
        }
        if (maxheight > 0)
            maxheight -= Time.deltaTime;
        NameText.text = playerName;

        GetComponent<CapsuleCollider>().isTrigger = ((!inArena) || (isblocked || isotherblocked));// (||);
        if (life > 0)
        {
            HealthBack.gameObject.SetActive(true);
            HealthText.text = life + "/" + 500;
            HealthFront.fillAmount = (life/ 500.0f);
        }
        else
        {
            HealthBack.gameObject.SetActive(false);
            HealthText.text = "";
        }
        if (characterCustomization!=null)
            NameTextImage.rectTransform.anchoredPosition = new Vector2(0,characterCustomization.headSizeValue/4f-characterCustomization.headOffset/1000);
        
        if(carcont.isactive)
        {
            NameTextImage.rectTransform.anchoredPosition = new Vector2(0, 0.35f);
        }
        
        if (characterCustomization != null)
            cameraTarget.transform.localPosition = new Vector3(0, 1.4f+(1.85f - 1.4f) * ((characterCustomization.heightValue + 0.1f) / 0.2f), 0);
        if (watersource!=null)
        {
            if (inwater)
                watersource.volume = 0.1f;
            else
                watersource.volume = 0.0f;
        }

        /*if (audiosource!=null && (AudioOn||AdminAudioOn))
            audiosource.gameObject.SetActive(true);
        else if(audiosource != null)
            audiosource.gameObject.SetActive(false);*/

        if (camtarget != null)
            nametarget.LookAt(camtarget);
        DummyInput();
        
       
                
        if(view!=null && !view.IsMine)
        {
            if(ispreviousblocked!=isblocked && isblocked)
            {
                OtherBlocK(true);
                ispreviousblocked = isblocked;
            }
            else if(ispreviousblocked != isblocked && !isblocked)
            {
                OtherBlocK(false);
                ispreviousblocked = isblocked;
            }
        }

    }

    GameObject vcamobj;
    GameObject vcamfobj;
    GameObject vcamcobj;

    public void AddCamera(CinemachineVirtualCamera vcam, CinemachineVirtualCamera vcamf, CinemachineVirtualCamera vcamc)
    {
        vcam.Follow = cameraTarget;
        vcamobj = vcam.gameObject;
        vcamf.Follow = cameraTarget;
        vcamfobj = vcamf.gameObject;
        vcamc.Follow = cameraTarget;
        vcamcobj = vcamc.gameObject;

        vcamobj.SetActive(true);
        vcamfobj.SetActive(false);
        vcamcobj.SetActive(false);
    }

    public void SwitchRaceCam(bool racetrack)
    {
        if(racetrack)
        {
            vcamobj.SetActive(false);
            vcamfobj.SetActive(false);
            vcamcobj.SetActive(true);
        }
        else
        {
            vcamobj.SetActive(true);
            vcamfobj.SetActive(false);
            vcamcobj.SetActive(false);
        }
    }

    float lookheight = 0;

    bool lastFrameGrounded = true;
    ThirdPersonController tpc;
    
    public float punchcounter = 0;
    public float punchbuffercounter = 0;

    public bool InsideLeaderBoard = false;
    public void DummyInput()
    {
        

        if (view==null || !view.IsMine)
            return;
        mainplayerposition = transform.position;

        if (isKicked)
        {
            NetworkManager.instance.LeaveRoom(0);
            isKicked = false;
        }
        if (tpc==null)
        {
            tpc = GetComponent<ThirdPersonController>();
        }
        if (tpc.Grounded && !lastFrameGrounded && inwater)
        {
            jumpwatersource.Play();
        }
        else if(tpc.Grounded && !lastFrameGrounded)
        {
            jumpendsource.Play();
        }
        else if(!tpc.Grounded && lastFrameGrounded)
        {
            jumpstartsource.Play();
        }
        if(bottomindicator!=null)
            underwater = bottomindicator.isSubmarged;
        if(topindicator!=null)
            underbelowwater = topindicator.isSubmarged;

        lastFrameGrounded = tpc.Grounded;

        if (underwater)
            inwater = true;

        animator.SetBool("INWATER", inwater);
        
        ispunched = false;
        if (isAttackingMode)
        {
            thirdPersonController.move = new Vector2(0, 0);
        }
        else
        {
            thirdPersonController.move = moveBuffer;
        }

    }

    private void OnDestroy()
    {
        OnlinePLayerManager.instance?.RemovePlayer(this);
        UnregisterInput();
    }

    public bool isattacked=false;
    public bool DamegeOtherPlayer(float timeduration,float damagearea,int damage,Transform trans,GameObject effect)
    {
        //yield return new WaitForSeconds(timeduration);
        //bool donedamage = false;
        for(int i=0;i< OnlinePLayerManager.instance.playerList.Count;i++)
        {
            Vector3 one = trans.position;
            one.y = 0;
            Vector3 two = OnlinePLayerManager.instance.playerList[i].transform.position;
            two.y = 0;
            if (Vector3.Distance(one, two) < damagearea)
            {
                OnlinePLayerManager.instance.playerList[i].Punched(damage);
                if (trans == transform)
                    trans = OnlinePLayerManager.instance.playerList[i].transform;
                //Instantiate(effect, trans);
                //donedamage = true;
                isattacked = true;
                AudioManager.instance.Play("PUNCH_"+Random.Range(1,8));
                NetworkManager.instance.InstansiateObject(effect.name, trans.position);
                ServerManager.instance.UpdateFightingData(damage);
                return true;
            }
        }

        for (int i = 0; i < GameManager.instance.puncables.Count; i++)
        {
            Vector3 one = trans.position;
            one.y = 0;
            Vector3 two = GameManager.instance.puncables[i].transform.position;
            two.y = 0;
            if (Vector3.Distance(one, two) < damagearea)
            {
                //OnlinePLayerManager.instance.playerList[i].Punched(damage);
                /*if (trans == transform)
                    trans = OnlinePLayerManager.instance.playerList[i].transform;*/
                //Instantiate(effect, trans);
                //donedamage = true;
                isattacked = true;
                AudioManager.instance.Play("PUNCH_" + Random.Range(1, 8));
                NetworkManager.instance.InstansiateObject(effect.name, trans.position);
                ServerManager.instance.UpdateFightingData(damage);
                return true;
            }
        }

        Debug.Log("Damaging");
        return false;
    }

    public bool DamegeOtherPlayerMissed()
    {
        if (!isattacked)
        {
            AudioManager.instance.Play("PUNCH_MISS_" + Random.Range(1, 6));
        }
        Debug.Log("DamagingSound");
        isattacked = false;
        return false;
    }

    /*public void SetAnimation(string param)
    {
        animator.SetBool(param, false);
        AnimatorStateInfo clipinfo=animator.GetCurrentAnimatorStateInfo(0);

        if(clipinfo.IsName(param))
        {
            return;
        }

        string[] namelist = { "BoxingMagic", "BoxingIdle", "BoxingKick", "BoxingPunched", "BoxingPunch" };
        
        foreach(string str in namelist)
        {
            if (clipinfo.IsName(str))
            {
                Debug.Log(str);
                return;
            }       
        }

        animator.SetBool(param,true);

        if(param== "BoxingPunch")
        {
            StartCoroutine(DamegeOtherPlayer(punchtime,1, 20,handone,HandPower));
        }
        if (param == "BoxingKick")
        {
            StartCoroutine(DamegeOtherPlayer(kicktime,2, 40, legone,LegPower));
        }
        if (param == "BoxingMagic")
        {
            StartCoroutine(DamegeOtherPlayer(magictime, 4, 40, transform, MagicPower));
        }

    }*/

    public bool PotentialTarget;
    public bool HandTouched;
    public bool checkEnterArena = false;

    private void OnTriggerEnter(Collider other)
    {

        /*if (view != null && !view.IsMine && other.gameObject != handObject && other.tag == "HAND")
        {
            Debug.Log("Touched By Player 2  " + name + "  " + other.tag);
            Punched();
        }*/

        if (view == null || !view.IsMine)
            return;
        useable = other.GetComponent<IUseable>();
        if (other.tag == "ARENA")
        {
            GameInputHandler.instance.ActivateFighter();
            inArena = true;
            if (checkEnterArena == false)
            {
                life = 500;
                checkEnterArena = true;
                //isalive = true;
                Debug.Log("Life Added");
                GetComponent<CharacterController>().stepOffset = 0.1f;
            }
            AudioManager.instance.Play("RUFFYRUMBLE");
        }
        if(other.tag== "LEADERBOARD")
        {
            //InsideLeaderBoard = true;
            GameInputHandler.instance.ActivateNewtral(true);
            GameManager.instance.racemanager.PopUp.SetActive(true);
        }
        if (other.tag == "Water")
        {
            inwater = true;
        }
    }

    /*PUNCH_1-7
     * Health_25-10-5
     * PUNCH_MISS_1-5
     * KNOCKOUTCHEER
     * KNOCKOUT
     * RUFFYRUMBLE
     */


    public bool inwater = false;
    public bool inwall = false;
    public bool underwater = false;
    public bool underbelowwater = false;
    public bool inArena = false;

    private void OnTriggerStay(Collider other)
    {
        
        if (view != null && !view.IsMine && other.tag == "HAND")
        {
           // Debug.Log("Touched By Player 3  " + name + "  " + other.tag);
        }
       // Debug.Log("Touched By Player 4  " + name + "  " + other.tag);
        if (view==null || !view.IsMine)
            return;

        if(ispunched)
        {
            if(other.gameObject.GetComponent<PlayerController>())
            {
                other.gameObject.GetComponent<PlayerController>().PotentialTarget = true;
            }
        }

        if (other.tag=="Water")
        {
            inwater = true;
        }

        if (other.tag == "WALL")
        {
            inwall = true;
        }

        if (other.tag == "EXITARENA")
        {
            inArena = false;
            checkEnterArena = false;
            isalive = true;
            GameInputHandler.instance.ActivatePlayer();
            life = 0;
            AudioManager.instance.Stop("RUFFYRUMBLE");
            AudioManager.instance.Stop("KNOCKOUTCHEER");
            AudioManager.instance.Stop("Health_25");
            AudioManager.instance.Stop("Health_10");
            AudioManager.instance.Stop("KNOCKOUT");
            GetComponent<CharacterController>().stepOffset = 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (view == null || !view.IsMine)
            return;

        if (useable == other.GetComponent<IUseable>())
            useable = null;

        if (other.tag == "Water")
        {
            inwater = false;
        }

        if (other.tag == "Wall")
        {
            inwall = false;
        }

        if (other.tag == "LEADERBOARD")
        {
            //InsideLeaderBoard = false;
            GameInputHandler.instance.ActivateNewtral(false);
            GameManager.instance.racemanager.PopUp.SetActive(false);
           
        }
    }

    private void LateUpdate()
    {
        if (!view.IsMine)
            return;

        if (carcont.pathCreator != null)
        {
            Vector3 pointpos = carcont.pathCreator.path.GetClosestPointOnPath(transform.position);
            float currentdistance = Vector3.Distance(pointpos, transform.position);

            //Debug.Log(currentdistance);
            //Debug.Log(pointpos);

            if (carcont.isactive)
            {
                if (currentdistance > carcont.MaxDistance)
                {
                    transform.position = pointpos + (transform.position - pointpos).normalized * carcont.MaxDistance;
                }
            }
            else
            {
                if (currentdistance < (carcont.MaxDistance + 5))
                {
                    transform.position = pointpos + (transform.position - pointpos).normalized * (carcont.MaxDistance + 5);
                }
            }
            
        }

        ///2022/08/29 by pooh for test
        if (!carcont.isactive)
        {
            if (currentState != playerState.chatting && Input.GetKeyDown(KeyCode.P))
            {

                carcont.TakeCar(!carcont.isOnCar);
            }

        }


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //edward
            //stream.SendNext(carcont.car.stearing);
            //stream.SendNext(carcont.car.speed);
            //
            stream.SendNext(carcont.isactive);
            stream.SendNext(carcont.isOnCar);
            stream.SendNext(playerName);
            stream.SendNext(playerEmail);
            stream.SendNext(AudioOn);
            stream.SendNext(AdminAudioOn);
            stream.SendNext(inwater);
            stream.SendNext(underwater);
            stream.SendNext(blendShapeIndex);
            stream.SendNext(life);
            stream.SendNext(inArena);
            stream.SendNext(MatchOver);

            bool isinroom = false;
            if (zone != null)
                isinroom = zone.isinroom;
            stream.SendNext(isinroom);
            float ismultiplier = 0;
            if (zone != null)
                ismultiplier = zone.multiplier;
            stream.SendNext(ismultiplier);
            if (MatchOver)
                MatchOver = false;
            stream.SendNext(playerMassage);
            
            blendShapeIndex = -1;

            stream.SendNext(carcont.carindex);
            stream.SendNext(carcont.nitro);
            //edward
            Quaternion[] qt = new Quaternion[4] { Quaternion.identity, Quaternion.identity, Quaternion.identity, Quaternion.identity};
            for (int i = 0; i < 4; i++)
            {
                if(carcont.carmaincontroller.enabled)
                {
                    stream.SendNext(carcont.carmaincontroller.m_WheelMeshes[i].transform.rotation);
                }
                else
                {
                    stream.SendNext(qt[i]);
                }
            }
            //
        }
        else if (stream.IsReading)
        {
            //edward
            //carcont.car.Stearing((float)stream.ReceiveNext(),false);
            //carcont.car.Speed((float)stream.ReceiveNext(),false);
            //
            var bActive = (bool)stream.ReceiveNext();
            var bOnCar = (bool)stream.ReceiveNext();
            if (bActive)
            {
                carcont.Active(bActive);
            }
            else
            {
                carcont.TakeCar(bOnCar);
            }

            playerName = (string)stream.ReceiveNext();
            playerEmail = (string)stream.ReceiveNext();
            AudioOn = (bool)stream.ReceiveNext();
            AdminAudioOn= (bool)stream.ReceiveNext();
            inwater = (bool)stream.ReceiveNext();
            underwater = (bool)stream.ReceiveNext();
            blendShapeIndex = (int)stream.ReceiveNext();
            int templife = life;
            life= (int)stream.ReceiveNext();
            inArena= (bool)stream.ReceiveNext();
            MatchOver= (bool)stream.ReceiveNext();
            if(MatchOver)
            {
                if(GameManager.instance.mycontroller.inArena)
                    AudioManager.instance.Play("KNOCKOUTCHEER");
            }
            bool isinroom= (bool)stream.ReceiveNext();
            if (zone != null)
                zone.isinroom = isinroom;
            float multiplier= (float)stream.ReceiveNext();
            if (zone != null)
                zone.multiplier = multiplier;
            playerMassage = (string)stream.ReceiveNext();
            if (playerMassage!="" && playerMassage != playerlastMassage && view!=null && !view.IsMine && !isblocked && !isotherblocked)
            {
                Debug.Log(view.ViewID);
                Debug.Log(playerMassage);
                AudioManager.instance.PlayMessageReceived();
                ChatGui.allmassage += playerMassage;
                playerlastMassage = playerMassage;
                ChatGui.instance.ShowChannel();
                GameManager.instance.massagecount++;
            }
            if(blendShapeIndex!=-1)
            {
                var anim = characterCustomization.Settings.characterAnimationPresets[blendShapeIndex];
                if (anim != null)
                    characterCustomization.PlayBlendshapeAnimation(anim.name, 2f);
            }

            carcont.carindex= (int)stream.ReceiveNext();
            carcont.nitro= (bool)stream.ReceiveNext();

            //edward
            Quaternion[] qt = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                qt[i] = (Quaternion)stream.ReceiveNext();
            }
            for (int i = 0; i < 4; i++)
            {
                if(carcont.carmaincontroller.enabled)
                {
                    carcont.carmaincontroller.m_WheelMeshes[i].transform.rotation = qt[i];
                }
            }
            //
        }
    }

    ///2022/08/27 by POOH///
    public void OnTakeCar(bool bFlag)
    {
        
        if (carcont != null)
        {
            carcont.TakeCar(bFlag);
        }
    }
}
