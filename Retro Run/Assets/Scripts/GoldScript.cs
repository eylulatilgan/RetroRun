using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldScript : MonoBehaviour{

    public AudioClip goldSound;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(goldSound, transform.position);
        }
        Destroy(gameObject);
    }
}
