using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool kick;
		public bool switchcamera;
		public bool mainmenu;
		public bool talk;
		public bool onlineplayer;
		public bool enter;
		public bool freemove;
		public bool admintalk;
		public bool punch;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnKick(InputValue value)
		{
			KickInput(value.isPressed);
		}

		public void OnPunch(InputValue value)
		{
			PunchInput(value.isPressed);
		}

		public void OnSwitchCamera(InputValue value)
		{
			SwitchCameraInput(value.isPressed);
		}
		public void OnMainMenu(InputValue value)
		{
			MainMenuInput(value.isPressed);
		}
		public void OnTalk(InputValue value)
		{
			TalkInput(value.isPressed);
		}

		public void OnAdminTalk(InputValue value)
		{
			AdminTalkInput(value.isPressed);
		}
		public void OnOnlinePlayer(InputValue value)
		{
			OnlinePlayerInput(value.isPressed);
		}

		public void OnEnter(InputValue value)
		{
			EnterInput(value.isPressed);
		}

		public void OnFreeMove(InputValue value)
		{
			FreeMoveInput(value.isPressed);
		}
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			
            {
				move = newMoveDirection;

#if PLATFORM_ANDROID || PLATFORM_IOS || UNITY_ANDROID || UNITY_IOS
				if (move.magnitude > 0.75)
					sprint = true;
				else
					sprint = false;
#endif

			}
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			
				look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			
				jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			
				sprint = newSprintState;
		}

		public void KickInput(bool newKickState)
		{
			
				kick = newKickState;
		}

		public void PunchInput(bool newKickState)
		{
			
				punch = newKickState;
		}

		public void SwitchCameraInput(bool newSwitchState)
		{
			
				switchcamera = newSwitchState;
		}

		public void MainMenuInput(bool newMainMenuState)
		{
			if(!GameManager.instance.isreporting)
			mainmenu = newMainMenuState;
		}

		public void TalkInput(bool newTalkState)
		{
			
				talk = newTalkState;
		}
		public void AdminTalkInput(bool newTalkState)
		{
			
				admintalk = newTalkState;
		}

		public void OnlinePlayerInput(bool newOnlinePlayerState)
		{
			if (!GameManager.instance.isreporting)
				onlineplayer = newOnlinePlayerState;
		}

		public void EnterInput(bool newEnterState)
		{
			enter = newEnterState;
			mainmenu = false;
			onlineplayer = false;
		}

		public void FreeMoveInput(bool newEnterState)
		{
			freemove = newEnterState;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.Confined;
		}

#endif

	}
	
}