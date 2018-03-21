using UnityEngine;
using UnityEngine.UI;


public class TopmanDive : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify the different players.
	public float m_DiveCooldown;
	public float m_TimeInState = 1.5f;        // 
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

    Rigidbody rb;
	private string m_DiveButton;				// The input axis that is used for dive attack.
	private float nextDive;
	private float resetStateTimer;
	private TopmanPlayerController playerController;
    private GameObject target;



    private void OnEnable()
	{

	}
	
	private void Start ()
	{

		// The fire axis is based on the player number.
		m_DiveButton = "Dive" + m_PlayerNumber;
		playerController = gameObject.GetComponent<TopmanPlayerController> ();
		resetStateTimer = m_TimeInState;
		rb = GetComponent<Rigidbody> ();

	}

	private void Update ()
	{
        if (playerController.currentState != TopmanPlayerController.StateMachine.DIVE && target != null)
        {
            Destroy(target);
        }
        if (playerController.currentState == TopmanPlayerController.StateMachine.DIVE)
        {
            resetStateTimer -= Time.deltaTime;
            if (resetStateTimer <= 0)
            {
                rb.MovePosition(target.transform.position);
                playerController.currentState = TopmanPlayerController.StateMachine.MOVE;
                resetStateTimer = m_TimeInState;

                //Skill logic
                CreateHitBox();
                Destroy(target);
                target = null;

                //m_ExplosionParticles.transform.parent = null;

                m_ExplosionParticles.Play();

                m_ExplosionAudio.Play();

                //Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

            }
        }
        if (Input.GetButton (m_DiveButton) && Time.time > nextDive && playerController.currentState == TopmanPlayerController.StateMachine.MOVE) {
			//If the player used the skill, reset the timer to a new point in the future
			nextDive = Time.time + m_DiveCooldown;

			playerController.currentState = TopmanPlayerController.StateMachine.DIVE;

            //gameObject.transform.localScale = new Vector3(0, 0, 0);
            //rb.velocity= new Vector3 (0f,0f,0f);
            playerController.slowdownRate = m_ChargeVelocitySlowdownRate;
            GameObject instance = Instantiate(m_DiveTarget);
            instance.transform.position = gameObject.transform.position;
            //instance.transform.SetParent(GameObject.Find("Player" + m_PlayerNumber).transform, false);
            instance.GetComponent<DiveTargetController>().m_Owner = gameObject;
            instance.GetComponent<DiveTargetController>().speed = m_TargetSpeed;
            target = instance;
        }
	}

    private void CreateHitBox()
    {
        // Instatiate hitbox with stats from this script
        GameObject instance = Instantiate(m_HitBox);
        // Set position of hitbox to the position of the target
        instance.transform.position = target.transform.position;
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
