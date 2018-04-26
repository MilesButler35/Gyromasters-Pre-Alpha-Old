using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class TopmanStats : MonoBehaviour
{
	public float m_PlayerNumber = 1f;
    public float m_StartingHealth = 100f;
	public float m_BaseAttack = 10f;  
	public float m_AttackMod = 1f;				// Percentage of speed added to attack
	public float m_BaseDefense = 5f;  
	public float m_DefenseMod = 1f;				// Percentage of speed added to defense  

	public Rigidbody m_Rigidbody;
    public Slider m_Slider;
    public Text m_HealthText;
    public Image m_FillImage;
    public GameObject m_DamageText;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    public Transform spawnPoint;

    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;
    private TopmanPlayerController playerController;
    private float m_CurrentHealth;  
	private float m_LastVelocity;
    private float m_StunTimer;
    private bool m_Dead;
    private bool m_Stunned = false;
    public int KillCount = 2;

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);

    }

    private void Start()
    {
        InvokeRepeating("LoseHealth", 1.0f, 2f);
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
		m_Rigidbody = GetComponent<Rigidbody> ();
        playerController = gameObject.GetComponent<TopmanPlayerController>();
        SetHealthUI();
        m_Slider.maxValue = m_StartingHealth;
        //Lose Health every 2 seconds
        //InvokeRepeating("LoseHealth", 1.0f, 2f);
    }

    void Update()
    {       
        if (m_Stunned)
        {
            // Reset state back to neutral after m_StunTimer amount of time
            Invoke("ResetState", m_StunTimer);
            m_Stunned = false;           
        }
        if(m_CurrentHealth <= 0)
        {
            Invoke("RpcRespawn", 3f);
            OnDeath();
        }
    }

    void FixedUpdate ()
    {
		m_LastVelocity = m_Rigidbody.velocity.magnitude;
        if (m_Dead)
        {
            RpcRespawn();
            m_Dead = false;
        }
    }

	private void OnCollisionEnter(Collision col)
    {
        // Get object collided with's health
		TopmanStats targetHealth = col.gameObject.GetComponent <TopmanStats>();


        if (targetHealth != null) 
		{
			Rigidbody targetRigidbody = col.rigidbody;

			float damage = CalculateDamage (m_LastVelocity, targetHealth.m_LastVelocity, targetHealth);

			//string s = m_PlayerNumber.ToString() + " " + damage.ToString() + " " + m_Rigidbody.velocity.magnitude.ToString();
			//print (s);

            targetHealth.TakeDamage(damage, 0.1f);
		}
        // Reset state to neutral if player collides with ANY object while stunned or in the middle of a rush
        else if (playerController.currentState == TopmanPlayerController.StateMachine.STUN || playerController.currentState == TopmanPlayerController.StateMachine.RUSH)
        {
            ResetState();
        }
	}
		

	private float CalculateDamage(float currentVelocity, float targetVelocity, TopmanStats targetHealth)
    {
		// Calculate damage as (our speed + attack) - (target's speed + defense).
		float damage = ((currentVelocity * m_AttackMod) + m_BaseAttack) - ((targetVelocity * targetHealth.m_DefenseMod) + targetHealth.m_BaseDefense);

		// Make sure that the minimum damage is always 0.
		damage = Mathf.Max (0f, damage);

		return damage;
	}

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, 0f);
    }

    public void TakeDamage(float amount, float stunTime)
    {
		// Adjust the topman's current health, update the UI based on the new health and check whether or not the topman is dead.
		m_CurrentHealth -= amount;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0f, m_StartingHealth);
        SetHealthUI();

        // Put player in hitstun for given amount of stunTime
        playerController.currentState = TopmanPlayerController.StateMachine.STUN;
        m_Stunned = true;
        m_StunTimer = stunTime;

        m_DamageText.GetComponent<FloatingTextControl>().CreateText(m_PlayerNumber, amount, transform.position);

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            if (KillCount == 0)
            {
                OnDeath();
            }
            //else
                //m_Dead = true;//RpcRespawn();

        }
        
    }

    private void LoseHealth()
    {
        m_CurrentHealth -= 1;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0f, m_StartingHealth);
        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            if (KillCount == 0)
            {
                OnDeath();
            }
            //else
                //m_Dead = true;//RpcRespawn();
        }
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0f, m_StartingHealth);
        m_Slider.value = m_CurrentHealth;

        //FIX later
        if (m_HealthText != null)
            m_HealthText.text = "P" + m_PlayerNumber + "| " + Mathf.Round(m_CurrentHealth);

        m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth/m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        //m_Dead = true;
        m_CurrentHealth = m_StartingHealth;
        m_ExplosionParticles.transform.position = transform.position;
		m_ExplosionParticles.gameObject.SetActive (true);

		m_ExplosionParticles.Play ();

		m_ExplosionAudio.Play ();

        gameObject.SetActive(false);

    }

    private void ResetState()
    {
        playerController.currentState = TopmanPlayerController.StateMachine.MOVE;
    }

    private void RpcRespawn()
    {

        gameObject.SetActive(true);
        // move back to zero location
        //transform.position = Vector3.zero;
        Vector3 spawnPos = spawnPoint.transform.position;
        m_Rigidbody.MovePosition(spawnPos);
        m_Rigidbody.velocity = Vector3.zero;
        ResetState();
        KillCount--;
        System.Console.WriteLine(KillCount);
        if (KillCount == 0)
        {
            EditorSceneManager.LoadScene(4);
        }
    }
}