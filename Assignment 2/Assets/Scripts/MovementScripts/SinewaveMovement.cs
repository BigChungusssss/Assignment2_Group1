using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinewaveMovement : MonoBehaviour
{
    // Start is called before the first frame update
    float posy;
    void Start()
    {
        posy = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FixedUpdate()
    {
        Vector2 pos = transform.position;
        float sin = Mathf.Sin(pos.x);
        pos.y = posy + sin;
        transform.position = pos;
    }
}
