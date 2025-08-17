using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject, 3f);
    }
}
