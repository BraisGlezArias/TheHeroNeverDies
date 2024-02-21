using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool transition;
    private float speed = 300.0f;
    private float t = 0f;
    public Transform cameraGameplay;
    // Start is called before the first frame update
    void Start()
    {
        transition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transition) {
            t += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(transform.localPosition, cameraGameplay.localPosition, t / speed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, cameraGameplay.localRotation, t / speed);

            if (transform.localPosition.x == cameraGameplay.localPosition.x) {
                transition = false;
            }
        }
    }
}
