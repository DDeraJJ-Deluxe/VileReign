using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    public float hoverDistance = 0.1f; // The distance the image should move up and down
    public float hoverSpeed = 1.0f; // The speed at which the image should move

    private Vector3 startPos; // The starting position of the image

    void Start () {
        startPos = transform.position;
    }

    void Update () {
        float y = startPos.y + (Mathf.Sin(Time.time * hoverSpeed) * hoverDistance);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
