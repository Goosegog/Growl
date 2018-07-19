using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartBehaviour : MonoBehaviour
{
    float rotation_speed = 65f;
	
	void Update ()
    {
        SelfRotation();
    }

    void SelfRotation()
    {
        //transform.Rotate(Vector3.up, rotation_speed);
        //transform.RotateAround(transform.position, Vector3.up, rotation_speed);
        transform.Rotate(0, rotation_speed * Time.deltaTime, 0);
    }
}
