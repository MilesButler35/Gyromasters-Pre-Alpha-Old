using UnityEngine;
using UnityEngine.UI;

public class TopmanStats : MonoBehaviour
{
	public float m_PlayerNumber = 1f;
    public float m_StartingHealth = 100f;
	public float m_BaseAttack = 10f;  
	public float m_AttackMod = 1f;				// Percentage of speed added to attack
	public float m_BaseDefense = 5f;  
	public float m_DefenseMod = 1f;				// Percentage of speed added to defense
	public float m_SpiralAttack = 10f;			// Stat for skill damage    
	public float m_SpiralDefense = 5f;			// Stat for resistance against skills   

	public Rigidbody m_Rigidbody;
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
	private float m_LastVelocity; 
    private bool m_Dead;            

    private void Awake() {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable() {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
		m_Rigidbody = GetComponent<Rigidbody> ();

        SetHealthUI();
    }

	void Update () {
		//Reduce health every frame (about 90 second lifespan)
		TakeDamage(0.03f);
	}

	void FixedUpdate () {
		m_LastVelocity = m_Rigidbody.velocity.magnitude;
	}

	private void OnCollisionEnter(Collision col) {
		TopmanStats targetHealth = col.gameObject.GetComponent <TopmanStats>(); 

		if (targetHealth != null) 
		{
			Rigidbody targetRigidbody = col.rigidbody;

			float damage = CalculateDamage (m_LastVelocity, targetHealth.m_LastVelocity, targetHealth);

			string s = m_PlayerNumber.ToString() + " " + damage.ToString() + " " + m_Rigidbody.velocity.magnitude.ToString();
			print (s);

			targetHealth.TakeDamage (damage);
		}
	}
		

	private float CalculateDamage(float currentVelocity, float targetVelocity, TopmanStats targetHealth) {
		// Calculate damage as (our speed + attack) - (target's speed + defense).
		float damage = ((currentVelocity * m_AttackMod) + m_BaseAttack) - ((targetVelocity * targetHealth.m_DefenseMod) + targetHealth.m_BaseDefense);

		// Make sure that the minimum damage is always 0.
		damage = Mathf.Max (0f, damage);

		return damage;
	}


    public void TakeDamage(float amount) {
		// Adjust the topman's current health, update the UI based on the new health and check whether or not the topman is dead.
		m_CurrentHealth -= amount;

		SetHealthUI();

		if (m_CurrentHealth <= 0f && !m_Dead) {
			OnDeath();
		}
    }


    private void SetHealthUI() {
        // Adjust the value and colour of the slider.
		m_Slider.value = m_CurrentHealth;

		m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth/m_StartingHealth);
    }


    private void OnDeath() {
        // Play the effects for the death of the tank and deactivate it.
		m_Dead = true;

		m_ExplosionParticles.transform.position = transform.position;
		m_ExplosionParticles.gameObject.SetActive (true);

		m_ExplosionParticles.Play ();

		m_ExplosionAudio.Play ();

		gameObject.SetActive (false);
    }
}