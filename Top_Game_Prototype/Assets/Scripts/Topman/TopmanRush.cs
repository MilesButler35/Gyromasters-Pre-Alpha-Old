using UnityEngine;
using UnityEngine.UI;


public class TopmanRush : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify the different players.
	public float m_RushCooldown;

	private string m_RushButton;				// The input axis that is used for launching the spinner forward.
	private float NextRush;						// 


	private void OnEnable()
	{

	}


	private void Start ()
	{

		// The fire axis is based on the player number.
		m_RushButton = "Rush" + m_PlayerNumber;

	}


	private void Update ()
	{

		if (Input.GetButton (m_RushButton) && Time.time > NextRush) {
			//If the player used the skill, reset the timer to a new point in the future
			NextRush = Time.time + m_RushCooldown;

			//Skill logic
			Rush ();
		}
	}


	private void Rush ()
	{

	}
}

