using UnityEngine;
using UnityEngine.UI;

/*TODO: Check for any differences in code between this script 
 *      and the player version. Edit this script to match the
 *      player implentation but with AI control.
*/

public class AIRush : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify the different players.
    public Slider m_AimSlider;
    public AudioSource m_RushAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_RushCooldown;
    public Slider m_CooldownSlider;
    public float m_ChargeVelocitySlowdownRate = 0.90f; // How quickly player slows down while charging skill 
                                                       // 0 = complete stop | 1 = unchanged
    public float m_MinLaunchForce = 1000;
    public float m_MaxLaunchForce = 3000;
    public float m_TimeInState;                 // Amount of time (in seconds) before finishing skill
    public float m_MaxChargeTime = 0.75f;       // How long (in seconds) the skill can charge for before it is fired at max force.
    public int attackChance;

    private Rigidbody rb;
    private float moveHorizontal;
    private float moveVertical;
    private float ResetStateTimer;
    private string m_RushButton;                // The input axis that is used for launching the spinner forward.
    private float nextRush;

    private AIManager playerController;
    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
    private bool pressed;
    private float rand;


    private void OnEnable()
    {

    }


    private void Start()
    {

        // The fire axis is based on the player number.

        rb = GetComponent<Rigidbody>();
        playerController = gameObject.GetComponent<AIManager>();
        ResetStateTimer = m_TimeInState;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        m_AimSlider.value = 0f; //m_MinLaunchForce
        m_CooldownSlider.maxValue = m_RushCooldown;
        m_CurrentLaunchForce = m_MinLaunchForce;
        pressed = false;
    }


    private void Update()
    {
        if (playerController.dist < 14 && playerController.dist > 8)
        {
            rand = Random.Range(1, 100);
        }
        m_AimSlider.value = 0f; //m_MinLaunchForce
        moveHorizontal = playerController.playerPos.x; //Mathf.Round(Input.GetAxis (h_MovementAxisName)*4f)/4f;
        moveVertical = playerController.playerPos.y; //Mathf.Round(Input.GetAxis (v_MovementAxisName)*4f)/4f;

        if (playerController.dist <= 14 && playerController.dist > 8 && rand <= attackChance && playerController.currentState == AIManager.StateMachine.MOVE && !m_Fired)
        {
            pressed = true;
        }
            //If interrupted by an attack start cooldown timer
            if (playerController.currentState == AIManager.StateMachine.STUN && m_CurrentLaunchForce > m_MinLaunchForce)
            {

                m_CurrentLaunchForce = m_MinLaunchForce;
                nextRush = Time.time + m_RushCooldown;
                playerController.slowdownRate = 1f;
                m_Fired = true;
            }
            // If the max force has been exceeded and the shell hasn't yet been launched...
            else if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                nextRush = Time.time + m_RushCooldown;
                playerController.slowdownRate = 1f;
                // ... use the max force and launch the shell.
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Rush();
                // Cut the charging clip and change it to firing.
                m_RushAudio.Stop();
                m_RushAudio.clip = m_FireClip;
                m_RushAudio.Play();
            }
            // Otherwise, if the fire button has been released and the shell has been launched...
            else if (playerController.currentState == AIManager.StateMachine.RUSH && m_Fired)
            {
                // Decrement timer to reset state back to neutral
                ResetStateTimer -= Time.deltaTime;
                playerController.slowdownRate = 0.90f;
                if (ResetStateTimer <= 0)
                {
                    playerController.currentState = AIManager.StateMachine.MOVE;
                    ResetStateTimer = m_TimeInState;
                }
            }

            // Otherwise, if the fire button has just started being pressed...
            else if (pressed && Time.time > nextRush && (moveHorizontal != 0 || moveVertical != 0) && playerController.currentState == AIManager.StateMachine.MOVE)
            {
                // ... reset the fired flag and reset the launch force.
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;
                playerController.slowdownRate = m_ChargeVelocitySlowdownRate;
                playerController.currentState = AIManager.StateMachine.RUSH;
                // Change the clip to the charging clip and start it playing.
               
                m_RushAudio.clip = m_ChargingClip;
                m_RushAudio.Play();
            }

            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
            else if (pressed && !m_Fired)
            {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce / m_MaxLaunchForce;
        }

            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
            else if (pressed && !m_Fired)
            {
                nextRush = Time.time + m_RushCooldown;
                playerController.slowdownRate = 1f;
                // ... launch the shell.
                Rush();

                // If the charging clip hasn't ended, cut it.
                m_RushAudio.Stop();
                // Change the clip to firing and play it.
                m_RushAudio.clip = m_FireClip;
                m_RushAudio.Play();
            
            }
        if (nextRush - Time.time >= 0)
        {
            SetCooldownUI();
        }



    }



    private void Rush()
    {
        m_Fired = true;

        m_CooldownSlider.interactable = false;

        rb.velocity = new Vector3(0, 0, 0);

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(rb.transform.forward * m_CurrentLaunchForce);

        pressed = false;
    }

    private void SetCooldownUI()
    {
        // Adjust the value and colour of the slider.
        float cooldownTime = nextRush - Time.time;
        if (cooldownTime < 0.02f)
        {
            cooldownTime = 0f;
           m_CooldownSlider.interactable = true;
        }
        m_CooldownSlider.value = Mathf.Clamp(cooldownTime, 0f, cooldownTime);
    }
}

