using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilWizard : MonoBehaviour
{
    [SerializeField] float      m_speed = 5.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
	
	private int health = 100;
	//public Slider healthSlider;
	private int mana = 100;
	//public Slider manaSlider;
	private float timeRecoverMana = 0;
	private int damage;
	
	public GameObject lightningPre;
	public GameObject lightning;
	public Transform lightningPos;
	public Transform lightningPosRev;
	public bool lightningExists = false;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_EvilWizard   m_groundSensor;
    private bool                m_grounded = false;
    private float               m_timeSinceAttack = 0.0f;
    private int                 m_facingDirection = 1;
    private float               m_delayToIdle = 0.0f;
	
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_EvilWizard>();
    }

    // Update is called once per frame
    void Update()
    {

        m_timeSinceAttack += Time.deltaTime;
		timeRecoverMana += Time.deltaTime;
		
		if (timeRecoverMana >= 1.0f) {
			mana += 2;
			timeRecoverMana = 0;
		}
		
		if (m_timeSinceAttack > 0.6f && lightningExists) {
			Destroy(lightning);
			lightningExists = false;
		}

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
			if (lightningExists) {
				lightning.transform.position = lightningPos.position;
			}
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
			if (lightningExists) {
				lightning.transform.position = lightningPosRev.position;
			}
        }

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

        // Move
        m_body2d.velocity = new Vector2(-inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Death
        if (Input.GetKeyDown("e"))
        {
            m_animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q"))
        {
            m_animator.SetTrigger("Hurt");
        }   

        //Attack
        else if(Input.GetKeyDown("1") && m_timeSinceAttack > 0.75f && mana >= 10)
        {
            m_animator.SetTrigger("Attack1");
			damage = 110
			mana -= 10;
            m_timeSinceAttack = 0;
        }

        else if(Input.GetKeyDown("2") && m_timeSinceAttack > 0.6f && mana >= 20)
        {
            m_animator.SetTrigger("Attack2");
			if (GetComponent<SpriteRenderer>().flipX) {
				lightning = Instantiate(lightningPre, lightningPos.position, Quaternion.identity);
			} else {
				lightning = Instantiate(lightningPre, lightningPosRev.position, Quaternion.identity);
			}
			lightning.transform.parent = this.transform;
			lightningExists = true;
			damage = 120;
			mana -= 20;
            m_timeSinceAttack = 0;
        }
            

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded)
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
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
                if(m_delayToIdle < 0) {
                    m_animator.SetInteger("AnimState", 0);
                }
        }
    }
}
