using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegPartButton : MonoBehaviour
{
    public string leg_part_name;
    GameObject leg_part;

    public RectTransform ban_image;
    Vector3 leg_part_pos_default;


    void Start ()
    {
        leg_part_pos_default = new Vector3(-2.5f, 1.3f, -1f);
    }
	

	void Update ()
    {
		
	}

    public void ClickButton()
    {
        leg_part = Instantiate(Resources.Load<GameObject>("Part/MovementPart/LegPart/" + leg_part_name));
        leg_part.GetComponent<LegPart>().Init();//手动初始化，因为Start执行顺序有问题
        leg_part.GetComponent<LegPart>().CreatFromButton();
        
    }

}
