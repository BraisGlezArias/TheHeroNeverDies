using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public AudioClip menuMusic;
    public AudioClip combatMusic;
    public GameObject player;
    public GameObject enemy;
    public GameObject turno;
    public Canvas mainMenu;
    public Canvas gameplay;
    public Canvas story;
    public bool nextTurn;
    public int turn;
    private Coroutine blinkCoroutine;
    private bool gameStart = false;
    public Camera mainCamera;
    public Transform cameraGameplay;

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
                StartCoroutine(newTurn());
                nextTurn = true;
            }
        }

        if (Input.anyKeyDown && !gameStart) {
            StopCoroutine(blinkCoroutine);
            StartCoroutine(TransToStory());
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

        turno.transform.GetChild(0).transform.GetComponent<TextMesh>().text = "Round " + turn; 

        enemy.GetComponent<HeroKnight>().health = 100;
        enemy.GetComponent<HeroKnight>().energy = 100;
        player.GetComponent<EvilWizard>().health = 100;
        player.GetComponent<EvilWizard>().mana = 100;

        enemy.GetComponent<HeroKnight>().death = false;
        enemy.GetComponent<HeroKnight>().m_animator.SetBool("Death", enemy.GetComponent<HeroKnight>().death);
        nextTurn = false;
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
        yield return new WaitForSeconds(6f);
        GetComponent<AudioSource>().Stop();
        story.transform.gameObject.SetActive(true);
    }

    private void TransToGameplay() {
        story.transform.gameObject.SetActive(false);
        GetComponent<AudioSource>().clip = combatMusic;
        GetComponent<AudioSource>().Play();
        gameplay.transform.gameObject.SetActive(true);
        gameStart = true;
        player.GetComponent<EvilWizard>().death = false;
        enemy.GetComponent<HeroKnight>().death = false;
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
