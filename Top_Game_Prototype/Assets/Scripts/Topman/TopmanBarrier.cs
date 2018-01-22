using UnityEngine;
using UnityEngine.UI;


public class TopmanBarrier : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify the different players.
	public float m_BarrierCooldown;

	private string m_BarrierButton;             // The input axis that is used for launching shells.
	private float NextBarrier;


    private void OnEnable()
    {
		
    }


    private void Start ()
    {

        // The fire axis is based on the player number.
        m_BarrierButton = "Barrier" + m_PlayerNumber;

    }


    private void Update ()
    {

		if (Input.GetButton (m_BarrierButton) && Time.time > NextBarrier) {
			//If the player used the skill, reset the timer to a new point in the future
			NextBarrier = Time.time + m_BarrierCooldown;

			//Skill logic
			Barrier ();
		}
			
    }


    private void Barrier ()
    {

    }
		
}
