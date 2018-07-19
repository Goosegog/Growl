using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPoitController : MonoBehaviour
{

    public float sensitivityX = 15;
    public float sensitivityY = 15;
    public float min_rotation_x = -7;
    public float max_rotation_x = 85;

    float rotationX;

    

    void Start ()
    {
        //Debug.Log((Quaternion.Euler(new Vector3(-7,0,0))).eulerAngles);
        
    }
	

	void Update ()
    {
        Mouse1();

    }

    void Mouse1()
    {
        //如果持续按住鼠标右键：
        if (Input.GetButton("Fire2"))
        {
            float rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;//鼠标横向滑动是沿世界y轴旋转，这种旋转不用限制角度
            rotationX += Input.GetAxis("Mouse Y") * sensitivityY;//鼠标横向滑动是沿世界x轴旋转(即俯仰)，这种旋转需要限制角度
            //Debug.Log(rotationX);
            rotationX = Mathf.Clamp(rotationX, min_rotation_x, max_rotation_x);
            transform.localEulerAngles = new Vector3(-rotationX, rotationY, 0);
        }


    }

}
