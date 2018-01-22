using UnityEngine;
using UnityEngine.UI;


public class TopmanDive : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify the different players.
	public float m_DiveCooldown;

	private string m_DiveButton;				// The input axis that is used for dive attack.
	private float NextDive;


	private void OnEnable()
	{

	}
	
	private void Start ()
	{

		// The fire axis is based on the player number.
		m_DiveButton = "Dive" + m_PlayerNumber;

	}

	private void Update ()
	{
		if (Input.GetButton (m_DiveButton) && Time.time > NextDive) {
			//If the player used the skill, reset the timer to a new point in the future
			NextDive = Time.time + m_DiveCooldown;

			//Skill logic
			Dive ();
		}
	}
	
	private void Dive ()
	{

	}
}
