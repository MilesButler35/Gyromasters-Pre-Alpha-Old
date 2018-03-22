using UnityEngine;
using UnityEngine.UI;


public class TopmanDive : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify the different players.
	public float m_DiveCooldown;
	public float m_TimeBeforeLanding = 1.5f;        // 
	public LayerMask m_TankMask;
	public ParticleSystem m_ExplosionParticles;       
	public AudioSource m_ExplosionAudio;              
	public float m_MaxDamage = 100f;
    public float m_ChargeVelocitySlowdownRate = 0.90f;
    public float m_ExplosionForce = 1000f;
    public GameObject m_DiveTarget;
    public float m_TargetSpeed = 20f;
    public float m_HitStun = 2f;
    public float m_ExplosionRadius = 5f;
    public GameObject m_HitBox;

    private Rigidbody rb;
    private bool landed;
	private string m_DiveButton;				// The input axis that is used for dive attack.
	private float nextDive;
	private float resetStateTimer;
	private TopmanPlayerController playerController;

    private void OnEnable()
	{

	}
	
	private void Start ()
	{

		// The fire axis is based on the player number.
		m_DiveButton = "Dive" + m_PlayerNumber;
		playerController = gameObject.GetComponent<TopmanPlayerController> ();
		resetStateTimer = m_TimeBeforeLanding;
		rb = gameObject.GetComponent<Rigidbody> ();

	}

	private void Update ()
	{
        if (playerController.currentState != TopmanPlayerController.StateMachine.DIVE && m_DiveTarget.activeSelf)
        {
            //Destroy(target);
            m_DiveTarget.SetActive(false);
        }
        if (playerController.currentState == TopmanPlayerController.StateMachine.DIVE)
        {
            resetStateTimer -= Time.deltaTime;
            Vector3 targetPosition = m_DiveTarget.transform.position;
            if (resetStateTimer <= 0)
            {               
                landed = true;

                //Skill logic
                CreateHitBox();
                m_DiveTarget.SetActive(false);

                //m_ExplosionParticles.transform.parent = null;

                m_ExplosionParticles.Play();

                m_ExplosionAudio.Play();

                //Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

            }
        }
        if (Input.GetButton(m_DiveButton) && Time.time > nextDive && playerController.currentState == TopmanPlayerController.StateMachine.MOVE)
        {
            //If the player used the skill, reset the timer to a new point in the future
            nextDive = Time.time + m_DiveCooldown;

            playerController.currentState = TopmanPlayerController.StateMachine.DIVE;

            playerController.slowdownRate = m_ChargeVelocitySlowdownRate;
            m_DiveTarget.SetActive(true);
            m_DiveTarget.transform.position = gameObject.transform.position;
            m_DiveTarget.GetComponent<DiveTargetController>().m_PlayerNumber = m_PlayerNumber;
            m_DiveTarget.GetComponent<DiveTargetController>().speed = m_TargetSpeed;
        }
	}

    private void FixedUpdate()
    {
        if (landed)
        {
            rb.position = m_DiveTarget.transform.position;
            playerController.currentState = TopmanPlayerController.StateMachine.MOVE;
            resetStateTimer = m_TimeBeforeLanding;

            landed = false;
        }      
    }

    private void CreateHitBox()
    {
        // Instatiate hitbox with stats from this script
        GameObject instance = Instantiate(m_HitBox);
        // Set position of hitbox to the position of the target
        instance.transform.position = m_DiveTarget.transform.position;
        HitboxCollision hcol = instance.GetComponent<HitboxCollision>();
        hcol.currentType = HitboxCollision.HitboxType.DIVE;
        hcol.m_PlayerNumber = m_PlayerNumber;              // Used to identify the different players.
        hcol.m_TankMask = m_TankMask;
        hcol.m_MaxDamage = m_MaxDamage;
        hcol.m_HitStun = m_HitStun;
        hcol.m_ExplosionForce = m_ExplosionForce;
        //bcol.m_MaxLifeTime = m_MaxLifeTime;
        hcol.m_ExplosionRadius = m_ExplosionRadius;
        hcol.m_OwnerRigidbody = gameObject.GetComponent<Rigidbody>();
    }
}
