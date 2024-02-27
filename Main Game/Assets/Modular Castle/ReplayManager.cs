using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayManager : MonoBehaviour {

    public AudioClip menuMusic;
    public AudioClip combatMusic;
    public GameObject player;
    public GameObject enemy;
    public GameObject turno;
    public Canvas gameplay;
    public Canvas buttons;
    public bool nextTurn;
    public int turn;
    public static ReplayManager instance;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        turn = 1;
        nextTurn = false;
        GetComponent<AudioSource>().clip = combatMusic;
        GetComponent<AudioSource>().Play();
        player.GetComponent<EvilWizard>().replay = true;
        player.GetComponent<EvilWizard>().death = false;
        enemy.GetComponent<HeroKnight>().death = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.GetComponent<HeroKnight>().death && !nextTurn) {
            StartCoroutine(newTurn());
            nextTurn = true;
        }
    } 

    private IEnumerator newTurn() {
        turn++;
        if (turn <= 10) {
            enemy.GetComponent<HeroKnight>().m_speed += 0.5f;
            enemy.GetComponent<HeroKnight>().defense += 10;
            enemy.GetComponent<HeroKnight>().damage += 3;
        }

        yield return new WaitForSeconds(3f);

        turno.transform.GetComponent<TextMesh>().text = "Round " + turn; 

        enemy.GetComponent<HeroKnight>().health = 100;
        enemy.GetComponent<HeroKnight>().energy = 100;
        player.GetComponent<EvilWizard>().health = 100;
        player.GetComponent<EvilWizard>().mana = 100;

        enemy.GetComponent<HeroKnight>().death = false;
        enemy.GetComponent<HeroKnight>().m_animator.SetBool("Death", enemy.GetComponent<HeroKnight>().death);
        nextTurn = false;
    }

    public void ShareScore() {
        Application.OpenURL("https://twitter.com/intent/tweet?text=I%20survived%20for%20" + (turn) + "%20rounds%20on%20The%20Hero%20NEVER%20Dies!%0A%0ACan%20you%20do%20it%20better%3F%20%23TheHeroNEVERDies%0A%0APlay%20it%20now%20for%20free%20on%20https%3A%2F%2Fmickael-vavrinec.itch.io%2Fthe-hero-never-dies");
    }

    public void Replay() {
        SceneManager.LoadScene("replay");
    }

    public IEnumerator ending() {
        player.GetComponent<EvilWizard>().m_body2d.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(1.5f);
        gameplay.gameObject.SetActive(false);
        buttons.gameObject.SetActive(true);
        buttons.transform.GetChild(1).GetChild(1).GetComponent<TextMesh>().text = "YOU SURVIVED " + (turn) + " ROUNDS";
        GetComponent<AudioSource>().clip = menuMusic;
        GetComponent<AudioSource>().Play();
    }
}
