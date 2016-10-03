using UnityEngine;
using System.Collections;
using Photon;

public class DestroyMissile : PunBehaviour {

	// Use this for initialization
	void Start () {
	    Invoke("DestroyGameObject", 3.5f);        
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void DestroyGameObject()
    {
        PhotonNetwork.Destroy(gameObject);        
    }
    
}
