using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public AudioClip menuMusic;
    public AudioClip combatMusic;
    public GameObject player;
    public GameObject enemy;
    public GameObject turno;
    public Canvas mainMenu;
    public Canvas gameplay;
    public Canvas story;
    public Canvas buttons;
    public bool nextTurn;
    public int turn;
    private Coroutine blinkCoroutine;
    public bool gameStart = false;
    public Camera mainCamera;
    public Transform cameraGameplay;
    public static GameManager instance;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        turn = 1;
        nextTurn = false;
        GetComponent<AudioSource>().clip = menuMusic;
        GetComponent<AudioSource>().Play();
        blinkCoroutine = StartCoroutine(Blink());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.GetComponent<HeroKnight>().death && gameStart) {
            if (!nextTurn) {
                if (turn < 3) {
                    StartCoroutine(storyTurn());
                } else {
                    StartCoroutine(newTurn());
                }
                nextTurn = true;
            }
        }

        if (Input.anyKeyDown && !gameStart && !Story.instance.storyOn) {
            StopCoroutine(blinkCoroutine);
            StartCoroutine(TransToStory());
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

    public IEnumerator storyTurn() {
        GetComponent<AudioSource>().Pause();
        if (player.GetComponent<EvilWizard>().death) {
            player.GetComponent<EvilWizard>().m_body2d.velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(1.5f);
        } else {
            player.GetComponent<EvilWizard>().death = true;
            player.GetComponent<EvilWizard>().m_animator.SetInteger("AnimState", 0);
            player.GetComponent<EvilWizard>().m_body2d.velocity = new Vector2(0, 0);
            if (player.GetComponent<EvilWizard>().lightningExists) {
                yield return new WaitForSeconds(0.7f);
                Destroy(player.GetComponent<EvilWizard>().lightning);
                yield return new WaitForSeconds(0.8f);
            } else {
                yield return new WaitForSeconds(1.5f);
            }
        }
        Story.instance.storyOn = true;
        gameplay.transform.gameObject.SetActive(false);
        story.transform.gameObject.SetActive(true);
        story.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
        story.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
        player.GetComponent<EvilWizard>().fire.SetActive(false);
        player.GetComponent<EvilWizard>().fireRev.SetActive(false);
        turn++;
        enemy.GetComponent<HeroKnight>().m_speed += 0.5f;
        enemy.GetComponent<HeroKnight>().defense += 10;
        enemy.GetComponent<HeroKnight>().damage += 3;
        enemy.GetComponent<HeroKnight>().Sword.SetActive(false);
        enemy.GetComponent<HeroKnight>().revSword.SetActive(false);
        yield return null;
    }

    private IEnumerator Blink() {

        TextMesh press = mainMenu.transform.GetChild(0).transform.GetComponent<TextMesh>();
        
        while(true) {
            Color color = press.color;
            color.a = PressBlink(Time.time);
            press.color = color;
            yield return null;
        }
    }

    private IEnumerator TransToStory() {
        mainCamera.transform.gameObject.GetComponent<CameraMovement>().transition = true;
        mainMenu.transform.gameObject.SetActive(false);
        yield return new WaitForSeconds(7.5f);
        GetComponent<AudioSource>().Stop();
        story.transform.gameObject.SetActive(true);
        Story.instance.storyOn = true;
    }

    public void TransToGameplay() {
        story.transform.gameObject.SetActive(false);
        if (GetComponent<AudioSource>().clip != combatMusic) {
            GetComponent<AudioSource>().clip = combatMusic;
        }
        GetComponent<AudioSource>().Play();
        gameplay.transform.gameObject.SetActive(true);
        gameStart = true;
        player.GetComponent<EvilWizard>().death = false;
        enemy.GetComponent<HeroKnight>().death = false;
        nextTurn = false;
    }

    public void CreditScene() {
        story.transform.gameObject.SetActive(false);
        GetComponent<AudioSource>().clip = menuMusic;
        GetComponent<AudioSource>().Play();
        CreditsMovement.instance.transition = true;
    }

    public void ShareScore() {
        Application.OpenURL("https://twitter.com/intent/tweet?text=I%20survived%20for%20" + (turn - 1) + "%20rounds%20on%20The%20Hero%20NEVER%20Dies!%0A%0ACan%20you%20do%20it%20better%3F%20%23TheHeroNEVERDies%0A%0APlay%20it%20now%20for%20free%20on%20https%3A%2F%2Fmickael-vavrinec.itch.io%2Fthe-hero-never-dies");
    }

    public void Replay() {
        SceneManager.LoadScene("replay");
    }

    private static float PressBlink(float t) {
        float frequency = 0.75f;  //Frecuencia de la señal
        float slope = 10f; //Pendiente de los flancos de subida y bajada
        float activationRatio = 0.5f; //Proporcion del periodo en el que la señal está a 1
        return SquareWave(t, frequency, slope, activationRatio);
    }

    private static float SquareWave(float t, float frequency, float slope, float activationRatio) {        
        // La función devuelve una señal cuadrada.
        float levelAdjust = slope * (1-activationRatio) - 0.5f; //Valor a restar para ajustar la señal al activationRatio deseado
        return Mathf.Clamp(PingPong(t*frequency) * slope - levelAdjust, 0f, 1f);
    }

    private static float PingPong(float t) {
        return Mathf.PingPong(t*2f, 1f);
    }
}
