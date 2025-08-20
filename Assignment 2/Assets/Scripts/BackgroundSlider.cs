using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSlider : MonoBehaviour
{
    public float scrollSpeed = 1f;
    public float backgroundWidth; // Set this in the Inspector based on your background image width

    void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x < -backgroundWidth)
        {
            // Reposition to the right to create the loop
            transform.position = new Vector3(transform.position.x + (backgroundWidth * 2.515f), transform.position.y, transform.position.z);
        }
    }
}

