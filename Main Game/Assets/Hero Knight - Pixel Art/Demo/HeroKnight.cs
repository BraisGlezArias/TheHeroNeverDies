using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroKnight : MonoBehaviour {

    public float m_speed = 1f;
    [SerializeField] float m_jumpForce = 7.5f;

    public GameObject player;
	
	public int health = 100;
	public Slider healthSlider;
	public int energy = 100;
	public Slider energySlider;
	private float timeRecoverEnergy = 0f;
	public int defense = 10;
	public int damage = 5;

    public Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private bool m_grounded = false;
    public float m_facingDirection = 0f;
	public GameObject Sword;
	public GameObject revSword;

	public float changeStateTolerance = 3;
	public float normalRate = 1;
	float nrmTimer;
	public float closeRate = 0.5f;
	float clTimer;
	public float blockingRate;
	float blTimer;
	public float aiStateLife = 1;
	float aiTimer;
	
	bool initiateAI;
	public bool attacking = false;
	bool closeCombat;
	bool gotRandom;
	float storeRandom;
	public bool blocking;
	bool randomizeAttacks;
	int numberOfAttacks;
	float timeRecoverHit;
	public float recover;
	public bool beingHitted;
	public bool death = false;
	
	public float JumpRate = 1;
	float jRate;
	bool jump;
	float jtimer;
	
	public enum AIState {
		closeState,
		normalState,
		resetAI
	}
	
	public AIState aiState;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
		m_facingDirection = 0f;
		m_animator.SetBool("Grounded", true);
		death = true;
    }

    // Update is called once per frame
    void Update ()
    {

    	if (!death) {

			if (!player.GetComponent<EvilWizard>().death) {
		
				CheckDistance();
				States();
				AIAgent();
					
				timeRecoverEnergy += Time.deltaTime;
				if (timeRecoverEnergy >= 1.0f && energy < 100) {
					energy += 2;
					timeRecoverEnergy = 0;
				}

				if (blocking || beingHitted) {
					m_facingDirection = 0;
				} 

				//Death
				if (health <= 0)
				{
					Dead();
				}

				healthSlider.value = health * 0.01f;
				energySlider.value = energy * 0.01f;
			} else {
				m_facingDirection = 0;
			}

			
			if (beingHitted) {
				timeRecoverHit += Time.deltaTime;

				if (timeRecoverHit >= recover) {
					m_animator.SetBool("Hurt", false);
					beingHitted = false;
					timeRecoverHit = 0;
				}
			}

			if (blocking) {
				blTimer += Time.deltaTime;

				if (blTimer > blockingRate) {
					m_animator.SetBool("IdleBlock", false);
					blTimer = 0;
					blocking = false;
				}
			} else {
				m_animator.SetBool("IdleBlock", false);
				blTimer = 0;
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
	    }
		m_body2d.velocity = new Vector2(m_facingDirection * m_speed, m_body2d.velocity.y);
		m_animator.SetInteger("AnimState", (int)m_facingDirection);
		m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
    }
	
	void States() {
		switch(aiState) {
			case AIState.closeState:
				CloseState();
				break;
			case AIState.normalState:
				NormalState();
				break;
			case AIState.resetAI:
				ResetAI();
				break;
		}
		
		if (!beingHitted && !blocking) {
			Blocking();
			Jumping();
		}
	}
	
	void AIAgent() {
		if (initiateAI) {
			aiState = AIState.resetAI;
			float multiplier = 0;
			
			if (!gotRandom) {
				storeRandom = ReturnRandom();
				gotRandom = true;
			}
			
			if (!closeCombat) {
				multiplier += 30;
			} else {
				multiplier -= 30;
			}
			
			if (storeRandom + multiplier < 50) {
				if (!blocking && !beingHitted) {
					Attack();
				}
			} 

			if (storeRandom > 50) {
				if (!blocking && !beingHitted) {
					Movement();
				}
			} else {
				m_facingDirection = 0f;
			}
		}
	}
	
	void Attack() {
		if (!gotRandom) {
			storeRandom = ReturnRandom();
			gotRandom = true;
		}
		
		if (!randomizeAttacks) {
			numberOfAttacks = Random.Range(1,4);
			randomizeAttacks = true;
		}
			
		if (energy >= 10 && !attacking) {
			attacking = true;
			StartCoroutine(OpenAttack(1, numberOfAttacks));
		}
	}

	void Movement() {
		if (!gotRandom) {
			storeRandom = ReturnRandom();
			gotRandom = true;
		}

		if (storeRandom < 90) {
			if (this.transform.position.x > player.transform.position.x) {
				GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = -1f;
			} else {
                GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1f;
			} 
		} else {
			if (this.transform.position.x > player.transform.position.x) {
				GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1f;
			} else {
				GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = -1f;
			} 
		}
	}

	void ResetAI() {
		aiTimer += Time.deltaTime;

		if (aiTimer > aiStateLife) {
			initiateAI = false;
			m_facingDirection = 0f;
			aiTimer = 0;

			gotRandom = false;

			storeRandom = ReturnRandom();

			if (storeRandom < 50) {
				aiState = AIState.normalState;
			} else {
				aiState = AIState.closeState;
			}

			randomizeAttacks = false;
		}
	}

	void CheckDistance() {
		float distance = Vector3.Distance(transform.position, player.transform.position);

		if (distance < changeStateTolerance) {
			if (aiState != AIState.resetAI) {
				aiState = AIState.closeState;
			}

			closeCombat = true;
		} else {
			if (aiState != AIState.resetAI) {
				aiState = AIState.normalState;
			}

			if (closeCombat) {
				if (!gotRandom) {
					storeRandom = ReturnRandom();
					gotRandom = true;
				}

				if (storeRandom < 60) {
					Movement();
				}
			}
		}
	}

	void Blocking() {

		if (Input.GetKeyDown("1") || Input.GetKeyDown("2")) {
			if (Input.GetKeyDown("1")) {
				blockingRate = 0.75f;
			} else {
				blockingRate = 0.6f;
			}

			if (!gotRandom) {
				storeRandom = ReturnRandom();
				gotRandom = true;
			}

			if (storeRandom < 50 && energy >= 20 && closeCombat) {
				if ((player.transform.position.x > transform.position.x && player.GetComponent<SpriteRenderer>().flipX == true && GetComponent<SpriteRenderer>().flipX == false) || (player.transform.position.x < transform.position.x && player.GetComponent<SpriteRenderer>().flipX == false && GetComponent<SpriteRenderer>().flipX == true)) {
					blocking = true;
					m_animator.SetTrigger("Block");
            		m_animator.SetBool("IdleBlock", true);
					energy -= 20;
					m_facingDirection = 0f;
				}
			}
		}
	}

	void NormalState() {
		nrmTimer += Time.deltaTime;

		if (nrmTimer > normalRate) {
			initiateAI = true;
			nrmTimer = 0;
		}
	}

	void CloseState() {
		clTimer += Time.deltaTime;

		if (clTimer > closeRate) {
			clTimer = 0;
			initiateAI = true;
		}
	}

	void Jumping() {
		if (!blocking) {

			if (Input.GetKeyDown("space")) {
				if (!gotRandom) {
					storeRandom = ReturnRandom();
					gotRandom = true;
				}

				if (storeRandom < 50 && m_grounded && player.GetComponent<SpriteRenderer>().flipX != GetComponent<SpriteRenderer>().flipX) {
					jump = true;
				}
			}


			if (jump) {
				m_animator.SetTrigger("Jump");
				m_grounded = false;
				m_animator.SetBool("Grounded", m_grounded);
				m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
				m_groundSensor.Disable(0.2f);
				jump = false;
			} else {
				m_grounded = true;
				m_animator.SetBool("Grounded", m_grounded);
			}

			jtimer += Time.deltaTime;

			if (jtimer > JumpRate*10) {
				m_body2d.velocity = new Vector2(m_body2d.velocity.x, 0);
				jRate = ReturnRandom();

				if (jRate < 50) {
					jump = true;
				} else {
					jump = false;
				}

				jtimer = 0;
			}
		}
	}

	void Dead() {
		m_facingDirection = 0f;
		m_animator.SetBool("Hurt", false);
		m_animator.SetBool("IdleBlock", false);
		beingHitted = false;
		timeRecoverHit = 0;
		blocking = false;
		blTimer = 0;
		attacking = false;
		death = true;
		m_animator.SetBool("Death", death);
		m_body2d.velocity = new Vector2(0, 0);
	}
	
	float ReturnRandom() {
		float retVal = Random.Range(0,101);
		return retVal;
	}
	
	IEnumerator OpenAttack(int i, int a) {
		m_animator.SetTrigger("Attack" + i);
		if (!GetComponent<SpriteRenderer>().flipX) {
			Sword.SetActive(true);
		} else {
			revSword.SetActive(true);
		}
		energy -= 10;
		yield return new WaitForSeconds(0.25f);
		Sword.SetActive(false);
		revSword.SetActive(false);
		if (i == a) {
			yield return new WaitForSeconds(0.75f);
		} 

		if (i < a) {
			i++;
			if (energy >= 10 && !blocking && !beingHitted) {
				StartCoroutine(OpenAttack(i, a));
			} else {
				attacking = false;
			}
		} else {
			attacking = false;
		}
	}
}
