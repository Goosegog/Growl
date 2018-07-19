using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerPartButton : MonoBehaviour
{
    public string power_part_name;
    GameObject power_part;
    bool PowerPartExisted = false;
    public RectTransform ban_image;
    Vector3 power_part_pos_default;
    UnitBlueprint unit_blueprint;

    void Start ()
    {
        power_part_pos_default = GameObject.Find("Rotation point").transform.position;
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
    }
	

	void Update ()
    {
		
	}

    public void ClickButton()
    {
        if (PowerPartExisted)
        {
            Debug.LogError("设计平台上已经存在一个动力组件！");
            return;
        }

        power_part = Instantiate(Resources.Load<GameObject>("Part/PowerPart/" + power_part_name));
        power_part.GetComponent<PowerPart>().Init();//手动初始化
        power_part.GetComponent<PowerPart>().CreatFromButton();//呼叫“从按钮创建动力组件实例”函数
       
    }

    public void BanSelf()
    {
        ban_image.GetComponent<Image>().enabled = true;
        PowerPartExisted = true;
    }
    public void ReleaseSelf()
    {
        ban_image.GetComponent<Image>().enabled = false;
        PowerPartExisted = false;
    }

}
