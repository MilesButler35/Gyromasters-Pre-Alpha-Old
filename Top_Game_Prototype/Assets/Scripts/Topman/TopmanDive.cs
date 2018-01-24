using UnityEngine;
using UnityEngine.UI;


public class TopmanDive : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify the different players.
	public float m_DiveCooldown;
	public float m_TimeStopMovement = 1.5f;
	public LayerMask m_TankMask;
	public ParticleSystem m_ExplosionParticles;       
	public AudioSource m_ExplosionAudio;              
	public float m_MaxDamage = 100f;                  
	public float m_ExplosionForce = 1000f;            
	//public float m_MaxLifeTime = 2f;                  
	public float m_ExplosionRadius = 5f; 

	Rigidbody rb;
	private string m_DiveButton;				// The input axis that is used for dive attack.
	private float NextDive;
	private float ResetStateTimer;
	private TopmanPlayerController playerController;


	private void OnEnable()
	{

	}
	
	private void Start ()
	{

		// The fire axis is based on the player number.
		m_DiveButton = "Dive" + m_PlayerNumber;
		playerController = gameObject.GetComponent<TopmanPlayerController> ();
		ResetStateTimer = m_TimeStopMovement;
		rb = GetComponent<Rigidbody> ();

	}

	private void Update ()
	{
		if (playerController.currentState == TopmanPlayerController.StateMachine.DIVE) {
			ResetStateTimer -= Time.deltaTime;
			if(ResetStateTimer < 0)
			{
				
				gameObject.transform.localScale = new Vector3(1, 1, 1);
				rb.detectCollisions = true;
				playerController.currentState = TopmanPlayerController.StateMachine.MOVE;
				ResetStateTimer = m_TimeStopMovement;
				//Skill logic
				Dive ();
			}
		}
		if (Input.GetButton (m_DiveButton) && Time.time > NextDive && playerController.currentState != TopmanPlayerController.StateMachine.DIVE) {
			//If the player used the skill, reset the timer to a new point in the future
			NextDive = Time.time + m_DiveCooldown;

			playerController.currentState = TopmanPlayerController.StateMachine.DIVE;

			gameObject.transform.localScale = new Vector3(0, 0, 0);
			rb.velocity= new Vector3 (0f,0f,0f);
			rb.detectCollisions = false;
		}
	}
	
	private void Dive ()
	{
		// Find all the tanks in an area around the shell and damage them.
		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

		for (int i = 0; i < colliders.Length; i++) 
		{
			Rigidbody targetRigidbody = colliders [i].GetComponent<Rigidbody> ();

			if (!targetRigidbody || targetRigidbody == GetComponent<Rigidbody>())
				continue;
			targetRigidbody.velocity= new Vector3 (0f,0f,0f);

			targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

			TopmanStats targetHealth = targetRigidbody.GetComponent<TopmanStats> ();

			if (!targetHealth)
				continue;

			float damage = CalculateDamage (targetRigidbody.position);

			targetHealth.TakeDamage (damage);
		}

		//m_ExplosionParticles.transform.parent = null;

		m_ExplosionParticles.Play ();

		m_ExplosionAudio.Play ();

		//Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
	}

	private float CalculateDamage(Vector3 targetPosition)
	{
		// Calculate the amount of damage a target should take based on it's position.
		Vector3 explosionToTarget = targetPosition - transform.position;

		float explosionDistance = explosionToTarget.magnitude;

		// Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

		// Calculate damage as this proportion of the maximum possible damage.
		float damage = relativeDistance * m_MaxDamage;

		// Make sure that the minimum damage is always 0.
		damage = Mathf.Max (0f, damage);

		return damage;
	}
}
