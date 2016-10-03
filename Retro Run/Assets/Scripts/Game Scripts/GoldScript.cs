using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldScript : MonoBehaviour{

    public AudioClip goldSound;

    public delegate void CollectAction(GameObject go);
    public static event CollectAction OnGoldCollect;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            //AudioSource.PlayClipAtPoint(goldSound, transform.position);
            OnGoldCollect(other.gameObject);
        }
        Destroy(gameObject);
    }
}
