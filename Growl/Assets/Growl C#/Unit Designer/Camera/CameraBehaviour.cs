using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    UnitBlueprint unit_blueprint;

    public Transform rotation_point;
    public float scroll_speed;
    float mouse_scrollwheel = 0;
    float fov_start;
    float min_fov = 10f;
    float max_fov = 35f;
    Camera main_camera;

    Vector3 offset2point_default;
    float offset2point_default_distance_sqr;
    float offset2point_default_distance;

    void Start ()
    {
        main_camera = GetComponent<Camera>();
        fov_start = main_camera.fieldOfView;
        offset2point_default = transform.position - rotation_point.position;
        offset2point_default_distance_sqr = offset2point_default.sqrMagnitude;
        offset2point_default_distance = offset2point_default.magnitude;
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        //Debug.Log(offset2point_default_distance_sqr);
    }
	

	void Update ()
    {
        transform.LookAt(rotation_point.position);   
        ScrollView();
        CrossWall();

    }
    void ScrollView()
    {
        if (unit_blueprint.menu_open) return;
        float v = 0f;
        mouse_scrollwheel += Input.GetAxis("Mouse ScrollWheel") * scroll_speed;
        mouse_scrollwheel = Mathf.Clamp(mouse_scrollwheel, min_fov - fov_start, max_fov - fov_start);
        //Debug.Log("mouse_scrollwheel = " + mouse_scrollwheel);
        float target_fov = fov_start + mouse_scrollwheel;

        //Debug.Log("target_fov = " + target_fov);
        float fov_smoothdamp = Mathf.SmoothDamp(main_camera.fieldOfView, target_fov, ref v, 0.1f);
        main_camera.fieldOfView = fov_smoothdamp;
    }

    void CrossWall()
    {
        //这个方法处理视线跨墙的情况
        RaycastHit hitwhat;
        if (Physics.Linecast(rotation_point.position, transform.position, out hitwhat, 1 << 9))
        {
            //从旋转点往相机打射线，打中了墙就把相机移动到命中点并往旋转点方向稍微移动一段距离
            //Debug.Log(hitwhat.transform.name);
            if (hitwhat.transform.tag == "Wall")
            {                
                transform.position = hitwhat.point + transform.forward * 0.1f;
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward * -1f, out hitwhat, 1000f, 1 << 9))
        {
            //从相机往后打射线，如果能打到墙，说明现在距离刚好
        }
        else
        {
            
            //如果从相机往前后打射线都打不到墙，说明可以把距离平滑拉回默认距离           
            Vector3 offset2point = transform.position - rotation_point.position;
            transform.position = rotation_point.position + Vector3.Lerp(offset2point, offset2point.normalized * offset2point_default_distance, 2f * Time.deltaTime);
        }
        //Debug.Log(Vector3.Distance(transform.position, rotation_point.position));
    }
}
