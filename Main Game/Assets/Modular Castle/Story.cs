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
            "So, I'm okay with letting you escape.",
            "...",
            "Not a chance, huh?",
            "Fine.",
            "What is a Hero, but a pile of dissapointments.",
            "Let's dance with the Death."
    };
    public String[] texto2 = {
            "Dead so soon?",
            "I guess heroes are not what they used to be.",
            "What a shame"
    };
    public int index = 0;
    public int turn = 1;
    public bool storyOn = false;
    public static Story instance;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        story.GetComponent<TextMesh>().text = texto1[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && storyOn) {
            if (turn == 1 && index < (texto1.Length - 1)) {
                index++;
                story.GetComponent<TextMesh>().text = texto1[index];
            } else {
                storyOn = false;
                GameManager.instance.TransToGameplay();
            }
        }
    }
}
