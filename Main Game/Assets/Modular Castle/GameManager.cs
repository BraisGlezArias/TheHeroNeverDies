using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public AudioClip combatMusic;
    public int turn;
    public GameObject player;
    public GameObject enemy;
    public bool nextTurn;

    // Start is called before the first frame update
    void Start()
    {
        turn = 1;
        nextTurn = false;
        GetComponent<AudioSource>().clip = combatMusic;
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.GetComponent<HeroKnight>().death) {
            if (!nextTurn) {
                StartCoroutine(newTurn());
                nextTurn = true;
            }
        }
    } 

    IEnumerator newTurn() {
        turn++;
        if (turn <= 10) {
            enemy.GetComponent<HeroKnight>().m_speed += 0.5f;
            enemy.GetComponent<HeroKnight>().defense += 10;
            enemy.GetComponent<HeroKnight>().damage += 3;
        }

        yield return new WaitForSeconds(3f);

        enemy.GetComponent<HeroKnight>().health = 100;
        enemy.GetComponent<HeroKnight>().energy = 100;
        player.GetComponent<EvilWizard>().health = 100;
        player.GetComponent<EvilWizard>().mana = 100;

        enemy.GetComponent<HeroKnight>().death = false;
        enemy.GetComponent<HeroKnight>().m_animator.SetBool("Death", enemy.GetComponent<HeroKnight>().death);
        nextTurn = false;
    }
}
