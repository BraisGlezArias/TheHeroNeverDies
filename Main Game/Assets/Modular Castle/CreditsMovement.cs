using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMovement : MonoBehaviour
{
    public bool transition;
    private float speed = 10000.0f;
    private float t = 0f;
    public Canvas credits;
    public static CreditsMovement instance;
    void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        transition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transition) {
            credits.gameObject.SetActive(true);
            t += Time.deltaTime;

            credits.transform.GetChild(0).GetComponent<RectTransform>().offsetMax = Vector2.Lerp(credits.transform.GetChild(0).GetComponent<RectTransform>().offsetMax, new Vector2(0, 0), t / speed);

            if (credits.transform.GetChild(0).GetComponent<RectTransform>().offsetMax.y > -25) {
                transition = false;
                GameManager.instance.buttons.gameObject.SetActive(true);
                GameManager.instance.buttons.transform.GetChild(1).GetChild(1).GetComponent<TextMesh>().text = "YOU SURVIVED " + (GameManager.instance.turn - 1) + " ROUNDS";
            }
        }
    }
}
