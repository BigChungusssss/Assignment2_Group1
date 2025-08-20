using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBlink : MonoBehaviour
{
    public GameObject alienBlink;
    public int count = 0;

    // Update is called once per frame
    void Update()
    {
        count +=1;

        if (count == 500)
        {
            alienBlink.SetActive(true);
            Debug.Log("blinked");

        }

        if (count == 540)
        {
            alienBlink.SetActive(false);
            count = 0;
        }


    }
}
