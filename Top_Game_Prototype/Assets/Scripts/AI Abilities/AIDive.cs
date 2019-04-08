using UnityEngine;
using UnityEngine.UI;

/*TODO: Check for any differences in code between this script 
 *      and the player version. Edit this script to match the
 *      player implentation but with AI control.
*/

public class AIDive : MonoBehaviour
{
    public int m_PlayerNumber = 2;              // Used to identify the different players.
    public TopmanAnim m_DiveAnim;
    public Slider m_CooldownSlider;
    public float m_DiveCooldown;
    public float m_TimeBeforeLanding = 1.5f;        // 
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public float m_MaxDamage = 100f;
    public float m_ChargeVelocitySlowdownRate = 0.90f;
    public float m_ExplosionForce = 1000f;
    public float m_TargetSpeed = 20f;
    public float m_HitStun = 2f;
    public float m_ExplosionRadius = 5f;
    public GameObject m_HitBox;

    private GameObject m_DiveTarget;
    private Rigidbody rb;
    private bool landed;               
    private float nextDive;
    private float resetStateTimer;
    private AIManager playerController;
    private bool pressed;

    private void OnEnable()
    {

    }

    private void Start()
    {

        // The fire axis is based on the player number.
        pressed = true;
        playerController = gameObject.GetComponent<AIManager>();
        resetStateTimer = m_TimeBeforeLanding;
        rb = gameObject.GetComponent<Rigidbody>();
        m_CooldownSlider.maxValue = m_DiveCooldown;

    }

    private void Update()
    {


        if (playerController.currentState != AIManager.StateMachine.DIVE && m_DiveTarget != null)
        {
            //Reset back to idle
            Destroy(m_DiveTarget);
            m_DiveAnim.Jump(false);
            m_DiveAnim.IsChargingDive(false);

        }
        if (playerController.currentState == AIManager.StateMachine.DIVE && m_DiveTarget != null)
        {
            resetStateTimer -= Time.deltaTime;

            Vector3 targetPosition = playerController.moveDirection;
            if (!pressed)
            {
                //Stop movement of target and start jump animation
                m_DiveTarget.GetComponent<Rigidbody>().velocity = Vector3.zero;
                m_DiveTarget.GetComponent<DiveTargetController>().speed = 0f;
                m_DiveAnim.IsChargingDive(false);
                m_DiveAnim.Jump(true);
                rb.detectCollisions = false;
            }
            if (resetStateTimer <= 0)
            {
                m_DiveTarget.GetComponent<Rigidbody>().velocity = Vector3.zero;
                m_DiveTarget.GetComponent<DiveTargetController>().speed = 0f;
                m_DiveAnim.IsChargingDive(false);
                m_DiveAnim.Jump(true);
                rb.detectCollisions = false;
            }
        }
        if (pressed && Time.time > nextDive && playerController.currentState == AIManager.StateMachine.MOVE)
        {
            //If the player used the skill, reset the timer to a new point in the future
            nextDive = Time.time + m_DiveCooldown;

            m_CooldownSlider.interactable = false;

            playerController.currentState = AIManager.StateMachine.DIVE;

            // Start Charging animation
            m_DiveAnim.IsChargingDive(true);

            playerController.slowdownRate = m_ChargeVelocitySlowdownRate;
            m_DiveTarget = (GameObject)Instantiate(Resources.Load("DiveTarget"));
            m_DiveTarget.transform.position = gameObject.transform.position;
            m_DiveTarget.GetComponent<DiveTargetController>().m_PlayerNumber = m_PlayerNumber;
            m_DiveTarget.GetComponent<DiveTargetController>().speed = m_TargetSpeed;
        }

        if (nextDive - Time.time >= 0)
        {
            SetCooldownUI();
        }
    }

    private void FixedUpdate()
    {
        if (landed)
        {
            //rb.position = m_DiveTarget.transform.position;
            rb.detectCollisions = true;
            playerController.currentState = AIManager.StateMachine.MOVE;
            resetStateTimer = m_TimeBeforeLanding;

            landed = false;
        }
    }

    public void CreateHitBox()
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
        hcol.m_ExplosionRadius = m_ExplosionRadius;
        hcol.m_OwnerRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public void MoveToTarget()
    {
        rb.position = m_DiveTarget.transform.position;
    }

    private void SetCooldownUI()
    {
        float cooldownTime = nextDive - Time.time;
        if (cooldownTime < 0.02f)
        {
            cooldownTime = 0f;
            m_CooldownSlider.interactable = true;
        }
        // Adjust the value and colour of the slider.
        m_CooldownSlider.value = cooldownTime;
    }

    public void OnGround()
    {
        landed = true;
        m_DiveAnim.IsChargingDive(false);
        m_DiveAnim.Jump(false);

        rb.detectCollisions = true;
        //Skill logic
        CreateHitBox();
        Destroy(m_DiveTarget);

        //m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        //Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
    }
}
