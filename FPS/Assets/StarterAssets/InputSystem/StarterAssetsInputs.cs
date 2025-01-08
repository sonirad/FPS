using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
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

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Tooltip("�÷��̾ ����ٴϴ� ī�޶�")]
		private CinemachineVirtualCamera followCamera;
		[Tooltip("�� �Է��� ���� �� ����Ǵ� ��������Ʈ(t : Ȯ��� ��, f : ���󺹱� �� ��")]
		public Action<bool> onZoom;

		[Tooltip("�÷��̾�")]
		private Player player;

        private void Awake()
        {
			player = GetComponent<Player>();
        }

        private void Start()
        {
			followCamera = GameManager.Instance.FollowCamera;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
			MoveInput(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (cursorInputForLook)
            {
                LookInput(context.ReadValue<Vector2>());
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpInput(context.performed);
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            SprintInput(context.ReadValue<float>() > 0.1f);
        }

		public void OnZoom(InputAction.CallbackContext context)
		{
			bool isPress = !context.canceled;

            StopAllCoroutines();
			StartCoroutine(Zoom(isPress));
			onZoom?.Invoke(isPress);
        }

		/// <summary>
		/// ZoomIn / ZoomOut�� ó���ϴ� �ڷ�ƾ
		/// </summary>
		/// <param name="zoomIn">t : Ȯ��, f : �ϻ󺹱�</param>
		/// <returns></returns>
        private IEnumerator Zoom(bool zoomIn)
        {
            const float zoomFOV = 20.0f;
            const float normalFOV = 40.0f;
            const float zoomTime = 0.25f;

            float speed = (normalFOV - zoomFOV) / zoomTime;
            float fov = followCamera.m_Lens.FieldOfView;

			if (zoomIn)
			{
                while (fov > zoomFOV)
                {
                    fov -= Time.deltaTime * speed;
                    followCamera.m_Lens.FieldOfView = fov;

                    yield return null;
                }

				followCamera.m_Lens.FieldOfView = zoomFOV;
            }
			else
			{
                while (fov > normalFOV)
                {
                    fov += Time.deltaTime * speed;
                    followCamera.m_Lens.FieldOfView = fov;

                    yield return null;
                }
            }

            followCamera.m_Lens.FieldOfView = normalFOV;
        }

        public void OnFire(InputAction.CallbackContext context)
        {
			player.GunFire(!context.canceled);
        }

        public void OnReload(InputAction.CallbackContext context)
        {
			if (context.performed)
			{
				player.RevolverReload();
			}
        }

#if ENABLE_INPUT_SYSTEM
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
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
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
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}