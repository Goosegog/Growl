using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundOBJBehaviour : MonoBehaviour
{
    float rotation_speed = 2f;

	void Start ()
    {
		
	}
	

	void Update ()
    {
        SelfRotation();

    }
    void SelfRotation()
    {
        transform.Rotate(Vector3.forward, rotation_speed * Time.deltaTime, Space.World);
    }
}
