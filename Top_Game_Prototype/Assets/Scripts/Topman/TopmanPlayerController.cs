using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopmanPlayerController : MonoBehaviour {
	public int m_PlayerNumber = 1;
	public float speed;

	private Rigidbody rb;
	private float moveHorizontal;
	private float moveVertical;
	private string h_MovementAxisName;          
	private string v_MovementAxisName;  

	public enum StateMachine {MOVE, STUN, BARRIER, DIVE, RUSH}

	public StateMachine currentState = StateMachine.MOVE;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}


	private void OnEnable ()
	{
		rb.isKinematic = false;
		moveHorizontal = 0f;
		moveVertical = 0f;
	}


	private void OnDisable ()
	{
		rb.isKinematic = true;
	}

	// Use this for initialization
	void Start () {
		h_MovementAxisName = "Horizontal" + m_PlayerNumber;
		v_MovementAxisName = "Vertical" + m_PlayerNumber;
	}

	void Update () {
		switch (currentState) {
			case StateMachine.MOVE:
				moveHorizontal = Input.GetAxis (h_MovementAxisName);
				moveVertical = Input.GetAxis (v_MovementAxisName);
				break;
			case StateMachine.STUN:
				break;
			case StateMachine.BARRIER:
				rb.velocity = new Vector3 (0f,0f,0f);
				break;
			case StateMachine.DIVE:
				moveHorizontal = Input.GetAxis (h_MovementAxisName);
				moveVertical = Input.GetAxis (v_MovementAxisName);
				break;
			case StateMachine.RUSH:
				break;
			default:
				moveHorizontal = Input.GetAxis (h_MovementAxisName);
				moveVertical = Input.GetAxis (v_MovementAxisName);
				break;
		}
	}

	void FixedUpdate () {
		switch (currentState) {
			case StateMachine.MOVE:
				Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

				rb.AddForce (movement * speed);
				break;
			case StateMachine.STUN:
				break;
			case StateMachine.BARRIER:
				break;
			case StateMachine.DIVE:
				// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
				movement = transform.forward * moveVertical * 5f * Time.deltaTime + transform.right * moveHorizontal * 5f * Time.deltaTime;

				// Apply this movement to the rigidbody's position.
				rb.MovePosition(rb.position + movement);
				break;
			case StateMachine.RUSH:
				break;
			default:
				movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

				rb.AddForce (movement * speed);
				break;
		}
	}
}
