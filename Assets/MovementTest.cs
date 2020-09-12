using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float radius = 5;
    public float speed = 10;

    private float angle = 0;

    // Update is called once per frame
    void Update()
    {
        angle += Time.deltaTime * speed;
        transform.position = new Vector3(radius * Mathf.Sin(angle), 0, radius * Mathf.Cos(angle));
    }
}
