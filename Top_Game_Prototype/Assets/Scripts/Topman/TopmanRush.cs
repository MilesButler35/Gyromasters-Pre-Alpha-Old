using UnityEngine;
using UnityEngine.UI;


public class TopmanRush : MonoBehaviour
{
	public int m_PlayerNumber = 1;              // Used to identify the different players.
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
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

    private Rigidbody rb;
	private float moveHorizontal;
	private float moveVertical;
    private float ResetStateTimer;
	private string h_MovementAxisName;          
	private string v_MovementAxisName;  
	private string m_RushButton;				// The input axis that is used for launching the spinner forward.
	private float nextRush;						
	private TopmanPlayerController playerController;
    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.


    private void OnEnable()
	{

	}


	private void Start ()
	{

		// The fire axis is based on the player number.
		m_RushButton = "Rush" + m_PlayerNumber;
		h_MovementAxisName = "Horizontal" + m_PlayerNumber;
		v_MovementAxisName = "Vertical" + m_PlayerNumber;
		rb = GetComponent<Rigidbody>();
		playerController = gameObject.GetComponent<TopmanPlayerController> ();
        ResetStateTimer = m_TimeInState;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        m_AimSlider.value = 0f; //m_MinLaunchForce
        m_CooldownSlider.maxValue = m_RushCooldown;
    }


    private void Update()
    {
        m_AimSlider.value = 0f; //m_MinLaunchForce
        SetCooldownUI();
        moveHorizontal = Input.GetAxis(h_MovementAxisName); //Mathf.Round(Input.GetAxis (h_MovementAxisName)*4f)/4f;
        moveVertical = Input.GetAxis(v_MovementAxisName); //Mathf.Round(Input.GetAxis (v_MovementAxisName)*4f)/4f;

        //If interrupted by an attack start cooldown timer
        if (playerController.currentState == TopmanPlayerController.StateMachine.STUN && m_CurrentLaunchForce != m_MinLaunchForce)
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
        }
        // Otherwise, if the fire button has been released and the shell has been launched...
        else if (playerController.currentState == TopmanPlayerController.StateMachine.RUSH && m_Fired)
        {
            // Decrement timer to reset state back to neutral
            ResetStateTimer -= Time.deltaTime;
            playerController.slowdownRate = 0.90f;
            if (ResetStateTimer <= 0)
            {
                playerController.currentState = TopmanPlayerController.StateMachine.MOVE;
                ResetStateTimer = m_TimeInState;
            }
        }

        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown(m_RushButton) && Time.time > nextRush && (moveHorizontal != 0 || moveVertical != 0) && playerController.currentState == TopmanPlayerController.StateMachine.MOVE)
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            playerController.slowdownRate = m_ChargeVelocitySlowdownRate;
            playerController.currentState = TopmanPlayerController.StateMachine.RUSH;
            // Change the clip to the charging clip and start it playing.
            //m_ShootingAudio.clip = m_ChargingClip;
            //m_ShootingAudio.Play();
        }

        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetButton(m_RushButton) && !m_Fired)
        {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce / m_MaxLaunchForce;
        }

        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetButtonUp(m_RushButton) && !m_Fired)
        {
            nextRush = Time.time + m_RushCooldown;
            playerController.slowdownRate = 1f;
            // ... launch the shell.
            Rush();
        }
        

        
    }


	private void Rush ()
	{
        m_Fired = true;

        rb.velocity = new Vector3 (0,0,0);

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rb.AddForce (movement * m_CurrentLaunchForce);
	}

    private void SetCooldownUI()
    {
        // Adjust the value and colour of the slider.
        m_CooldownSlider.value = nextRush - Time.time;
    }
}

