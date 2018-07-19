using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundOBJBehaviourPingPang : MonoBehaviour
{

	void Update ()
    {
        transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time * 4, 20), transform.position.z);
    }
}
