using System;
using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

// This script shows a simple example of how
// swipe controls can be handled.
public class TouchPadExample : MonoBehaviour
{
	[SerializeField] private VRInput m_VRInput;                                        
	[SerializeField] private Rigidbody m_Rigidbody;
	[SerializeField] private float m_StepInterval;
	[SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
	[SerializeField] private float m_StickToGroundForce;

	private Camera m_Camera;
	private bool m_Jump;
	private float m_YRotation;
	private Vector2 m_Input;
	private Vector3 m_MoveDir = Vector3.zero;
	private CharacterController m_CharacterController;
	private CollisionFlags m_CollisionFlags;
	private bool m_PreviouslyGrounded;
	private Vector3 m_OriginalCameraPosition;
	private float m_StepCycle;
	private float m_NextStep;
	private bool m_Jumping;
	private AudioSource m_AudioSource;

	private void OnEnable()
	{
		m_CharacterController = GetComponent<CharacterController>();
		m_Camera = Camera.main;
		m_OriginalCameraPosition = m_Camera.transform.localPosition;
		m_StepCycle = 0f;
		m_NextStep = m_StepCycle/2f;
		m_Jumping = false;
		m_AudioSource = GetComponent<AudioSource>();

//		m_VRInput.OnSwipe += HandleSwipe;
		m_VRInput.OnClick += HandleOnClick;
	}


	private void OnDisable()
	{
//		m_VRInput.OnSwipe -= HandleSwipe;
		m_VRInput.OnClick -= HandleOnClick;

	}

	// Update is called once per frame
	private void Update () { 
		
	}

	private void FixedUpdate()
	{
		
	}


	private void HandleSwipe(VRInput.SwipeDirection swipeDirection)
	{
		switch (swipeDirection)
		{
		case VRInput.SwipeDirection.NONE:
			break;
		case VRInput.SwipeDirection.UP:
			Debug.Log("HandleSwipe -- UP - START");

			break;
		case VRInput.SwipeDirection.DOWN:
			Debug.Log("HandleSwipe -- DOWN - START");

			break;
		case VRInput.SwipeDirection.LEFT:
			Debug.Log("HandleSwipe -- LEFT - START");

			break;
		case VRInput.SwipeDirection.RIGHT:
			Debug.Log("HandleSwipe -- RIGHT - START");

			break;
		}
	}

	private void HandleOnClick()
	{
		Debug.Log ("HandleOnClick -- START");

		moveCharacter(true);
	}



	private void moveCharacter(Boolean fromStopToStart){
		Debug.Log ("moveCharacter -- START -- " + fromStopToStart);
		m_Input = fromStopToStart ? new Vector2(0.0f, 1.0f) : new Vector2(0.0f, 0.0f);
		float speed = 20.0f;		//running or walking

		Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

		// get a normal for the surface that is being touched to move along it
		RaycastHit hitInfo;
		Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
			m_CharacterController.height/2f, ~0, QueryTriggerInteraction.Ignore);
		desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

		m_MoveDir.x = desiredMove.x*speed;
		m_MoveDir.z = desiredMove.z*speed;

		if (m_CharacterController.isGrounded)
		{
			Debug.Log ("m_CharacterController -- isGrounded");

			m_MoveDir.y = -m_StickToGroundForce;
		}

		m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

		ProgressStepCycle(speed);
	}

	private void ProgressStepCycle(float speed)
	{
		Debug.Log ("ProgressStepCycle -- START -- " + m_StepCycle + " -- " + m_NextStep);

		if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
		{
			m_StepCycle += (m_CharacterController.velocity.magnitude + speed*Time.fixedDeltaTime);
		}

//		if (!(m_StepCycle > m_NextStep))
//		{
//			return;
//		}

		m_NextStep = m_StepCycle + m_StepInterval;

		PlayFootStepAudio();
	}

	private void PlayFootStepAudio()
	{
		Debug.Log ("PlayFootStepAudio -- START");

		if (!m_CharacterController.isGrounded)
		{
			return;
		}
		// pick & play a random footstep sound from the array,
		// excluding sound at index 0
		int n = Random.Range(1, m_FootstepSounds.Length);
		m_AudioSource.clip = m_FootstepSounds[n];
		m_AudioSource.PlayOneShot(m_AudioSource.clip);
		// move picked sound to index 0 so it's not picked next time
		m_FootstepSounds[n] = m_FootstepSounds[0];
		m_FootstepSounds[0] = m_AudioSource.clip;
	}
}

