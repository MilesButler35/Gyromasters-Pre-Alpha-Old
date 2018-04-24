using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopmanPlayerController : MonoBehaviour {
	public int m_PlayerNumber = 1;
	public float speed;
    public float topSpeed;
    [HideInInspector] public float slowdownRate; //Rate at which player slows down when using a skill
    [HideInInspector] public float hitStunTime; //Amount of time player is in stun state
    [HideInInspector] public float skillTopSpeed; //Amount of time player is in stun state

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

	void Start ()
    {
		h_MovementAxisName = "Horizontal" + m_PlayerNumber;
		v_MovementAxisName = "Vertical" + m_PlayerNumber;
	}

	void Update ()
    {
		switch (currentState)
        {
			case StateMachine.MOVE:
                RotateDirectionVelocity();
                break;
			case StateMachine.STUN:
                break;
			case StateMachine.BARRIER:
                RotateDirectionVelocity();
                break;
			case StateMachine.DIVE:
				break;
			case StateMachine.RUSH:
                RotateDirectionVelocity();
                break;
		}
	}
	void FixedUpdate ()
    {
		switch (currentState)
        {
			case StateMachine.MOVE:
				Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
                if (rb.velocity.magnitude > topSpeed)
                {
                    slowdownRate = 0.95f;
                    SlowDownVelocity();
                }
                else
                {
                    rb.AddForce(movement * speed * rb.mass * Time.deltaTime);          
                }
                    break;
			case StateMachine.STUN:
				break;
			case StateMachine.BARRIER:
                Vector3 Barriermovement = new Vector3(moveHorizontal, 0.0f, moveVertical);
                if (rb.velocity.magnitude > skillTopSpeed)
                {
                    SlowDownVelocity();
                }
                else
                {
                    rb.AddForce(Barriermovement * speed * rb.mass * Time.deltaTime);
                }
                break;
			case StateMachine.DIVE:
                SlowDownVelocity();
                break;
			case StateMachine.RUSH:
                SlowDownVelocity();
                break;
		}
	}

    private void RotateDirectionVelocity ()
    {
        moveHorizontal = Input.GetAxis(h_MovementAxisName);
        moveVertical = Input.GetAxis(v_MovementAxisName);

        // Rotate the player character in the direction they are moving
        Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
        if (moveDirection != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, newRotation, Time.deltaTime * 8);
        }
    }

    private void SlowDownVelocity ()
    {
        // Gradually lower velocity at a rate of slowdownRate
        rb.velocity = new Vector3(rb.velocity.x * slowdownRate, rb.velocity.y * slowdownRate, rb.velocity.z * slowdownRate);
    }

}

