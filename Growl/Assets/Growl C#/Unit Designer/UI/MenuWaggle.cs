using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWaggle : MonoBehaviour
{
    Transform rotation_point;

    Quaternion self_r_default;
    Quaternion self_r_waggle;
    public Vector3 Euler_self_r_waggle;
    Quaternion rotation_point_r_lastframe;

    void Start ()
    {
        rotation_point = GameObject.Find("Rotation point").transform;
        self_r_default = transform.rotation;
        self_r_waggle = Quaternion.Euler(Euler_self_r_waggle);
        rotation_point_r_lastframe = rotation_point.rotation;
    }
	
	void Update ()
    {
        //Debug.Log(self_r_default.eulerAngles);
        if (!(rotation_point.rotation.Equals(rotation_point_r_lastframe)))
        {
            //Debug.Log("旋转了视角");

            (transform as RectTransform).rotation = Quaternion.Lerp((transform as RectTransform).rotation, self_r_waggle, 10f * Time.deltaTime);
        }
        else
        {
            //Debug.Log("归位中");
            if (!((transform as RectTransform).rotation.Equals(self_r_default)))
            {
                (transform as RectTransform).rotation = Quaternion.Lerp((transform as RectTransform).rotation, self_r_default, 10f * Time.deltaTime);
            }
            
        }

        rotation_point_r_lastframe = rotation_point.rotation;
    }
}
