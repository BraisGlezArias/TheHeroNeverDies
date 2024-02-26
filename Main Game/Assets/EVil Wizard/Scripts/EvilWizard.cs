using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvilWizard : MonoBehaviour
{
    private int m_speed = 5;
    [SerializeField] float m_jumpForce = 7.5f;
	
	public int health = 100;
	public Slider healthSlider;
	public int mana = 100;
	public Slider manaSlider;
	private float timeRecoverMana = 0;
	public int damage;

    public bool replay = false;

    public GameObject enemy;
	
	public GameObject lightningPre;
	public GameObject lightning;
    public GameObject fire;
    public GameObject fireRev;
	public Transform lightningPos;
	public Transform lightningPosRev;
	public bool lightningExists = false;
    public bool fireExists = false;

    public Animator m_animator;
    public Rigidbody2D m_body2d;
    private Sensor_EvilWizard m_groundSensor;
    private bool m_grounded = false;
    public float m_timeSinceAttack = 0.0f;
    private int m_facingDirection = 1;
    private float m_delayToIdle = 0.0f;
    public bool death;
	
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_EvilWizard>();
        death = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (!death) {

            m_timeSinceAttack += Time.deltaTime;
            timeRecoverMana += Time.deltaTime;
            
            if (timeRecoverMana >= 1.0f && mana < 100) {
                mana += 2;
                timeRecoverMana = 0;
            }
            
            if (m_timeSinceAttack > 0.6f && lightningExists) {
                Destroy(lightning);
                lightningExists = false;
            }

            if (m_timeSinceAttack > 0.75f && fireExists) {
                fire.SetActive(false);
                fireRev.SetActive(false);
                fireExists = false;
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
                if (fireExists) {
                    fire.SetActive(true);
                    fireRev.SetActive(false);
                }
            }
                
            else if (inputX < 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1;
                if (lightningExists) {
                    lightning.transform.position = lightningPosRev.position;
                }
                if (fireExists) {
                    fire.SetActive(false);
                    fireRev.SetActive(true);
                }
            } else {
                m_facingDirection = 0;
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
            m_body2d.velocity = new Vector2(m_facingDirection * m_speed, m_body2d.velocity.y);

            //Attack
            if(Input.GetKeyDown("1") && m_timeSinceAttack > 0.75f && mana >= 10 && !enemy.GetComponent<HeroKnight>().death) 
            {
                m_animator.SetTrigger("Attack1");
                if (GetComponent<SpriteRenderer>().flipX) {
                    fire.SetActive(true);
                } else {
                    fireRev.SetActive(true);
                }
                damage = 110;
                mana -= 10;
                m_timeSinceAttack = 0;
                fireExists = true;
            }

            else if(Input.GetKeyDown("2") && m_timeSinceAttack > 0.6f && mana >= 20 && !enemy.GetComponent<HeroKnight>().death)
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

            healthSlider.value = health * 0.01f;
            manaSlider.value = mana * 0.01f;

            if (health <= 0) {
                Dead();
            }
        }
    }

    void Dead() {
		m_facingDirection = 0;
		m_animator.SetBool("Death", true);
		death = true;
        m_body2d.velocity = new Vector2(0, 0);
        if (lightningExists) {
            Destroy(lightning);
        }
        if (!replay) {
            Story.instance.ending = true;
            Story.instance.story.transform.GetChild(3).GetComponent<TextMesh>().text =  Story.instance.endText[0];
            StartCoroutine(GameManager.instance.storyTurn());
        } else {
            StartCoroutine(ReplayManager.instance.ending());
        }
	}
}
