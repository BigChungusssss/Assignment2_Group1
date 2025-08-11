using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 5;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FixedUpdate()
    {
        Vector2 pos = transform.position;
        pos.x -= speed * Time.fixedDeltaTime;
        if (pos.x < -9.9)
        {
            Destroy(gameObject);
        }
        transform.position = pos;
    }
}
