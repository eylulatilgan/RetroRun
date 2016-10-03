using UnityEngine;
using System.Collections;

public class CustomSerializable : MonoBehaviour {

    private Vector3 realPosition = Vector3.zero;
    private Vector3 sendPosition;

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            sendPosition = this.transform.position;
            stream.Serialize(ref sendPosition);
        }
        else
        {
            stream.Serialize(ref realPosition);
        }
    }

    
    public void Update()
    {
        if (!GetComponent<NetworkView>().isMine)
        {            
            transform.position = Vector3.Lerp(this.transform.position, realPosition, Time.deltaTime * 15);
        }
    }

}
	

