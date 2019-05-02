using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*TODO: Check for any differences in code between this script 
 *      and the player version. Edit this script to match the
 *      player implentation but with AI control.
*/

public class AIBarrier : MonoBehaviour
{
    public int m_PlayerNumber = 2;              // Used to identify the different players.
    public float m_TopSpeed;
    public float m_BarrierCooldown;
    public AudioSource m_BarrierAudio;
    public AudioClip m_ActivationClip;
    public Slider m_CooldownSlider;
    public float m_TimeInState = 1.5f;
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public float m_MaxDamage = 100f;
    public float m_ChargeVelocitySlowdownRate = 0.90f;
    public float m_ExplosionForce = 1000f;
    public float m_HitStun = 2f;
    public float m_ExplosionRadius = 5f;
    public GameObject m_HitBox;
    public float defenseChance;

    private float nextBarrier;
    private float resetStateTimer;
    private AIManager playerController;
    private GameObject hitbox;
    private float rand;
    
    private void OnEnable()
    {

    }


    private void Start()
    {

        // The fire axis is based on the player number.
        playerController = gameObject.GetComponent<AIManager>();
        resetStateTimer = m_TimeInState;
        m_CooldownSlider.maxValue = m_BarrierCooldown;

    }


    private void Update()
    {
        if (playerController.dist < 15)
        {
            rand = Random.Range(1, 100);
        }
       
        Debug.Log("Random Number = " + rand);
        if (playerController.currentState != AIManager.StateMachine.BARRIER && hitbox != null)
        {
            Destroy(hitbox);
        }
        if (playerController.currentState == AIManager.StateMachine.BARRIER)
        {
            resetStateTimer -= Time.deltaTime;
            if (resetStateTimer < 0)
            {
                playerController.currentState = AIManager.StateMachine.MOVE;
                resetStateTimer = m_TimeInState;
            }
        }

        /*TODO: Input.GetButton() activates this skill
         *      Switch Input.GetButton() with some other boolean flag
         *      This can be done inside this script or in the AI manager             
        */      
        if ( playerController.dist <= 8 && Time.time > nextBarrier && rand <= defenseChance && playerController.currentState == AIManager.StateMachine.MOVE )
        {
            //If the player used the skill, reset the timer to a new point in the future
            nextBarrier = Time.time + m_BarrierCooldown;

            m_CooldownSlider.interactable = false;

            playerController.currentState = AIManager.StateMachine.BARRIER;

            //Skill logic
            playerController.slowdownRate = m_ChargeVelocitySlowdownRate;
            playerController.skillTopSpeed = m_TopSpeed;
            CreateHitBox();

            //Particles and Audio
            m_BarrierAudio.clip = m_ActivationClip;
            m_BarrierAudio.Play();

            //m_ExplosionParticles.transform.parent = null;

            m_ExplosionParticles.Play();


            //Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
           
        }

        if (nextBarrier - Time.time >= 0)
        {
            SetCooldownUI();
        }

        rand = Random.Range(0, 100);
    }

    private void CreateHitBox()
    {
        // Instatiate hitbox with stats from this script
        GameObject instance = Instantiate(m_HitBox);
        hitbox = instance;
        // Set hitbox as child of player
        instance.transform.SetParent(GameObject.Find("Player" + m_PlayerNumber).transform, false);
        HitboxCollision hcol = instance.GetComponent<HitboxCollision>();
        hcol.currentType = HitboxCollision.HitboxType.BARRIER;
        hcol.m_PlayerNumber = m_PlayerNumber;              // Used to identify the different players.
        hcol.m_TankMask = m_TankMask;
        hcol.m_MaxDamage = m_MaxDamage;
        hcol.m_HitStun = m_HitStun;
        hcol.m_ExplosionForce = m_ExplosionForce;
        hcol.m_MaxLifeTime = m_TimeInState;
        hcol.m_ExplosionRadius = m_ExplosionRadius;
        hcol.m_OwnerRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void SetCooldownUI()
    {
        float cooldownTime = nextBarrier - Time.time;
        if (cooldownTime < 0.02f)
        {
            cooldownTime = 0f;
           m_CooldownSlider.interactable = true;
        }
        // Adjust the value and colour of the slider.
        m_CooldownSlider.value = cooldownTime;
    }
}
