using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    public int m_PlayerNumber = 2;
    public float speed = 20;
    [HideInInspector] public float slowdownRate; //Rate at which AI slows down when using a skill
    [HideInInspector] public float hitStunTime; //Amount of time AI is in stun state
    public GameObject Player1;
    public GameObject self;
    public Vector3 playerPos;
    public Vector3 selfPos;
    private Rigidbody rb;
    private float moveHorizontal;
    private float moveVertical;
    private string h_MovementAxisName;
    private string v_MovementAxisName;
    private float choice;

    public enum StateMachine { MOVE, STUN, BARRIER} //DIVE, RUSH }

    public StateMachine currentState = StateMachine.MOVE;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        rb.isKinematic = false;
        moveHorizontal = 0f;
        moveVertical = 0f;
    }

    private void OnDisable()
    {
        rb.isKinematic = true;
    }
    void Start()
    {
        h_MovementAxisName = "Horizontal" + m_PlayerNumber;
        v_MovementAxisName = "Vertical" + m_PlayerNumber;
    }

	// Update is called once per frame
	void Update ()
    {
        choice = Random.Range(0, 100);
        if(choice <= 5 && self.GetComponent<TopmanBarrier>().m_BarrierCooldown ==0)
        {
            currentState = StateMachine.BARRIER;
        }
        playerPos = Player1.transform.position;
        selfPos = self.transform.position;
        switch (currentState)
        {
            case StateMachine.MOVE:
                RotateDirectionVelocity();
                break;
            case StateMachine.STUN:
                break;
            case StateMachine.BARRIER:
                break;
           /* case StateMachine.DIVE:
                break;
            case StateMachine.RUSH:
                RotateDirectionVelocity();
                break;
          */
        }
    }
    void FixedUpdate()
    {
        switch (currentState)
        {
            case StateMachine.MOVE:
                Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
                rb.AddForce(movement * speed);
                break;
            case StateMachine.STUN:
                break;
            case StateMachine.BARRIER:
                SlowDownVelocity();
                break;
           /* case StateMachine.DIVE:
                SlowDownVelocity();
                break;
            case StateMachine.RUSH:
                SlowDownVelocity();
                break;
            */
        }
    }
    private void RotateDirectionVelocity()
    {
        float xSpeed = 0;
        float zSpeed = 0;
        if (playerPos.x - selfPos.x < 0)
            xSpeed = -1;
        else if (playerPos.x - selfPos.x > 0)
            xSpeed = 1;
        if (playerPos.z - selfPos.z < 0)
            zSpeed = -1;
        else if (playerPos.z - selfPos.z > 0)
            zSpeed = 1;
        moveHorizontal = xSpeed;
        moveVertical = zSpeed;

        // Rotate the player character in the direction they are moving
        Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
        if (moveDirection != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, newRotation, Time.deltaTime * 8);
        }
    }

    private void SlowDownVelocity()
    {
        // Gradually lower velocity at a rate of slowdownRate
        rb.velocity = new Vector3(rb.velocity.x * slowdownRate, rb.velocity.y * slowdownRate, rb.velocity.z * slowdownRate);
    }
}
