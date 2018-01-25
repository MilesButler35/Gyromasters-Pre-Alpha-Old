using UnityEngine;
using UnityEngine.UI;


public class TopmanRush : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify the different players.
	public float m_RushCooldown;
	public float RushForce = 1000;

	private Rigidbody rb;
	private float moveHorizontal;
	private float moveVertical;
	private string h_MovementAxisName;          
	private string v_MovementAxisName;  
	private string m_RushButton;				// The input axis that is used for launching the spinner forward.
	private float NextRush;						// 
	private TopmanPlayerController playerController;

	private void OnEnable()
	{

	}


	private void Start ()
	{

		// The fire axis is based on the player number.
		m_RushButton = "Rush" + m_PlayerNumber;
		h_MovementAxisName = "Horizontal" + m_PlayerNumber;
		v_MovementAxisName = "Vertical" + m_PlayerNumber;
		rb = GetComponent<Rigidbody>();
		playerController = gameObject.GetComponent<TopmanPlayerController> ();

	}


	private void Update ()
	{
		moveHorizontal = Input.GetAxis (h_MovementAxisName); //Mathf.Round(Input.GetAxis (h_MovementAxisName)*4f)/4f;
		moveVertical = Input.GetAxis (v_MovementAxisName); //Mathf.Round(Input.GetAxis (v_MovementAxisName)*4f)/4f;

		if (Input.GetButton (m_RushButton) && Time.time > NextRush && (moveHorizontal != 0 || moveVertical != 0) && playerController.currentState == TopmanPlayerController.StateMachine.MOVE ) {
			//If the player used the skill, reset the timer to a new point in the future
			NextRush = Time.time + m_RushCooldown;



			//Skill logic
			Rush ();
		}
	}


	private void Rush ()
	{
		rb.velocity = new Vector3 (0,0,0);

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rb.AddForce (movement * RushForce);
	}
}

