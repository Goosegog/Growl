using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraBehavior : MonoBehaviour
{
    Camera self;
	void Start ()
    {
        self = GetComponent<Camera>();
	}
	
	void Update ()
    {
        self.fieldOfView = Camera.main.fieldOfView;
	}
}
