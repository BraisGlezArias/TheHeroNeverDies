using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 1.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] GameObject m_slideDust;
	
	private int health = 100;
	//public Slider healthSlider;
	private int energy = 100;
	//public Slider energySlider;
	private float timeRecoverEnergy = 0f;
	private float defense = 10f;
	private float damage = 10f;
	private float waitToAct = 0f;
	private float timeToAct = 5.0f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private bool                m_grounded = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;


    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
		timeRecoverEnergy += Time.deltaTime;
		
		if (timeRecoverEnergy >= 1.0f) {
			mana += 2;
			timeRecoverEnergy = 0;
		}
        /* // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;
		waitToAct += Time.deltaTime;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
		int movementX = Math.Random(0,1);
		int movementY = Math.Random(0,1);
		
		if (movementX == 0) {
			float inputX = 0f;
		} else {
			float inputX = Math.Random(-1.0f, 1.0f);
		}

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }
		
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
		
		//Jump
        if (movementY == 1 && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Death
        if (Input.GetKeyDown("e"))
        {
            m_animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");
		
		if (waitToAct >= timeToAct) {
		
        //Attack
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1))
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        } */
    }
	
	void TakeDamage {
			if (health > 0) {
				
			}
	}
}
