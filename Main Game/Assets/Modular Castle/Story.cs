using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story : MonoBehaviour
{
    public GameObject story;
    public String[] texto1 = {
            "So... You are the new Hero, aren't you?",
            "The King's new plaything...",
            "Listen, pal, I'm gonna be honest with you.",
            "I was a Hero once too.",
            "I gave my life for this same Kingdom.",
            "And trust me, it didn't end well.",
            "So, I'm okay with letting you live.",
            "...",
            "Not a chance, huh?",
            "Fine.",
            "What is a Hero, but a pile of dissapointments.",
            "Let's dance with the Death."
    };
    public String[] texto2 = {
            "Dead so soon?",
            "I guess heroes are not what they used to be.",
            "What a shame.",
            "Oh? So you are still alive after all.",
            "Good! I wouldn't want it any other way."
    };
    public String[] texto3 = {
            "I'm pretty sure I killed you this time.",
            "No? So that's how it is...",
            "Then I will kill you as many times as I need."
    };
    public string[] endText = {
            "So... This is how it ends...",
            "I guess it's true what they say...",
            "The Hero Never Dies...",
            "...",
            "Indeed...",
            "The Hero NEVER Dies"
    };
    public int index = 0;
    public int turn = 1;
    public bool storyOn = false;
    public bool ending = false;
    public static Story instance;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        story.transform.GetChild(3).GetComponent<TextMesh>().text = texto1[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && storyOn) {
            if (!ending) {
                if (turn == 1 && index < (texto1.Length - 1)) {
                    if (index == 6) {
                        story.transform.GetChild(0).gameObject.SetActive(false);
                        story.transform.GetChild(1).gameObject.SetActive(true);
                    } else {
                        story.transform.GetChild(0).gameObject.SetActive(true);
                        story.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    index++;
                    story.transform.GetChild(3).GetComponent<TextMesh>().text = texto1[index];
                } else if (turn == 2 && index < (texto2.Length - 1)) {
                    if (index == 2) {
                        GameManager.instance.turno.transform.GetComponent<TextMesh>().text = "Round " + turn; 

                        GameManager.instance.enemy.GetComponent<HeroKnight>().health = 100;
                        GameManager.instance.enemy.GetComponent<HeroKnight>().energy = 100;
                        GameManager.instance.player.GetComponent<EvilWizard>().health = 100;
                        GameManager.instance.player.GetComponent<EvilWizard>().mana = 100;

                        GameManager.instance.enemy.GetComponent<HeroKnight>().m_animator.SetBool("Death", false);
                    }
                    index++;
                    story.transform.GetChild(3).GetComponent<TextMesh>().text = texto2[index];
                } else if (turn == 3 && index < (texto3.Length - 1)) {
                    if (index == 0) {
                        GameManager.instance.turno.transform.GetComponent<TextMesh>().text = "Round " + turn; 

                        GameManager.instance.enemy.GetComponent<HeroKnight>().health = 100;
                        GameManager.instance.enemy.GetComponent<HeroKnight>().energy = 100;
                        GameManager.instance.player.GetComponent<EvilWizard>().health = 100;
                        GameManager.instance.player.GetComponent<EvilWizard>().mana = 100;

                        GameManager.instance.enemy.GetComponent<HeroKnight>().m_animator.SetBool("Death", false);
                    }
                    index++;
                    story.transform.GetChild(3).GetComponent<TextMesh>().text = texto3[index];
                } else {
                    storyOn = false;
                    GameManager.instance.TransToGameplay();
                    index = 0;
                    turn++;
                    if (turn == 2) {
                        story.transform.GetChild(3).GetComponent<TextMesh>().text = texto2[index];
                    } else if (turn == 3) {
                        story.transform.GetChild(3).GetComponent<TextMesh>().text = texto3[index];
                    }
                }
            } else {
                if (index < (endText.Length - 1)) {
                    if (index == 2) {
                        story.transform.GetChild(0).gameObject.SetActive(false);
                        story.transform.GetChild(1).gameObject.SetActive(true);
                        story.transform.GetChild(3).localPosition = new Vector3(0, 2, -2);
                    } else if (index == 3) {
                        story.transform.GetChild(1).gameObject.SetActive(false);
                        story.transform.GetChild(2).gameObject.SetActive(true);
                    }
                    index++;
                    story.transform.GetChild(3).GetComponent<TextMesh>().text = endText[index];
                } else {
                    GameManager.instance.CreditScene();
                }
            }
        }
    }
}
