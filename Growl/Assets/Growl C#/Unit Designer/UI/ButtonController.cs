using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    List<PartButton> all_part_buttons;
    List<LayerButton> all_layer_button;
    List<PartButton> power_part_buttons;
    List<PartButton> leg_part_buttons;
    List<PartButton> control_part_buttons;
    List<PartButton> equip_part_buttons;
    List<PartButton> module_part_buttons;
    #region 重要外部引用
    UnitBlueprint unit_blueprint;
    BlueprintButton BB;
    AudioSource creat_part_audio;

    RectTransform UI_canvas_base;//大层，包含滚动零件菜单
    RectTransform part_interface_top;
    RectTransform part_interface_ground;//滚动零件菜单的父级
    RectTransform part_interface;//滚动零件菜单
    RectTransform part_interface_middle;//零件菜单遮挡用外框
    #endregion
    GameObject part_prefab;
    AudioSource self_AS;
    Dictionary<string,AudioClip> all_ACs;

    void Start()
    {
        self_AS = GetComponent<AudioSource>();
        all_ACs = new Dictionary<string, AudioClip>();
        all_ACs.Add("Error", Resources.Load<AudioClip>("AudioClip/Error"));


        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        BB = GameObject.Find("UICanvasTop/BlueprintButton").GetComponent<BlueprintButton>();
        creat_part_audio = GameObject.Find("AudioClickButton01").GetComponent<AudioSource>();

        UI_canvas_base = GameObject.Find("UICanvasBase").transform as RectTransform;
        part_interface_top = GameObject.Find("UICanvasTop/PartInterface").transform as RectTransform;
        part_interface_ground = GameObject.Find("UICanvasBase/PartInterfaceGround").transform as RectTransform;
        part_interface = GameObject.Find("UICanvasBase/PartInterfaceGround/PartInterfaceBackGround/PartInterface").transform as RectTransform;       
        part_interface_middle = GameObject.Find("UICanvasMiddle/PartInterfaceMiddle").transform as RectTransform;

        all_part_buttons = new List<PartButton>();
        all_layer_button = new List<LayerButton>();
        power_part_buttons = new List<PartButton>();
        leg_part_buttons = new List<PartButton>();
        control_part_buttons = new List<PartButton>();
        equip_part_buttons = new List<PartButton>();
        module_part_buttons = new List<PartButton>();
        Transform LB_temp = GameObject.Find("UICanvasTop/PartInterfaceBottomBar/PartsLayerToogleGroup").transform;
        foreach(var lb in LB_temp.GetComponentsInChildren<LayerButton>())
        {
            all_layer_button.Add(lb);

        }
        foreach (var temp in part_interface.GetComponentsInChildren<PartButton>())
        {
            all_part_buttons.Add(temp);
            temp.Init();//手动初始化

            switch ((int)temp.part_type)
            {
                case 0:
                    power_part_buttons.Add(temp);
                    break;
                case 1:
                    leg_part_buttons.Add(temp);
                    break;
                case 2:
                    control_part_buttons.Add(temp);
                    break;
                case 3:
                    equip_part_buttons.Add(temp);
                    break;
            }

        }
                
        ToolClosePartInterface();
    }
	void Update ()
    {
        //Debug.Log(leg_part_buttons.Count);
	}

    void CreatButton()
    {

    }

    #region ClickLayersButton

    public void ClickPowerPartLayerButton(LayerButton click_whichLB)
    {
        if (click_whichLB.pressed)
        {             
            //点击动力LB时，动力LB处于按下状态，则关闭全部面板
            ToolClosePartInterface();
            click_whichLB.self_image.sprite = click_whichLB.sp[2];
            click_whichLB.pressed = false;
            return;
        }

        ////点击动力LB时，动力LB没有处于按下状态
        foreach (var lb in all_layer_button)
        {
            if (lb != click_whichLB)
            {
                lb.self_image.sprite = lb.sp[0];
                lb.pressed = false;
            }
            else
            {
                lb.self_image.sprite = lb.sp[2];
                lb.pressed = true;
            }
        }

        BB.ClickCloseUnitInterfaceButton();
        ToolOpenPartInterface();
        foreach (var this_PPB in power_part_buttons)
        {
            this_PPB.gameObject.SetActive(true);
            this_PPB.ShowUIPart();
        }
        foreach(var this_LPB in leg_part_buttons)
        {
            this_LPB.gameObject.SetActive(false);
            this_LPB.CloseUIPart();
        }
        foreach (var this_CPB in control_part_buttons)
        {
            this_CPB.gameObject.SetActive(false);
            this_CPB.CloseUIPart();
        }
        foreach(var this_EPB in equip_part_buttons)
        {
            this_EPB.gameObject.SetActive(false);
            this_EPB.CloseUIPart();
        }

        unit_blueprint.menu_open = true;
    }
    public void ClickMovetLayerButton(LayerButton click_whichLB)
    {
        if (click_whichLB.pressed)
        {
            ToolClosePartInterface();
            click_whichLB.self_image.sprite = click_whichLB.sp[2];
            click_whichLB.pressed = false;
            return;
        }

        foreach (var lb in all_layer_button)
        {
            if (lb != click_whichLB)
            {
                lb.self_image.sprite = lb.sp[0];
                lb.pressed = false;
            }
            else
            {
                lb.self_image.sprite = lb.sp[2];
                lb.pressed = true;
            }
        }

        BB.ClickCloseUnitInterfaceButton();
        ToolOpenPartInterface();
        foreach (var this_PPB in power_part_buttons)
        {
            this_PPB.gameObject.SetActive(false);
            this_PPB.CloseUIPart();
        }
        foreach (var this_LPB in leg_part_buttons)
        {
            this_LPB.gameObject.SetActive(true);
            this_LPB.ShowUIPart();
        }
        foreach (var this_CPB in control_part_buttons)
        {
            this_CPB.gameObject.SetActive(false);
            this_CPB.CloseUIPart();
        }
        foreach (var this_EPB in equip_part_buttons)
        {
            this_EPB.gameObject.SetActive(false);
            this_EPB.CloseUIPart();
        }

        unit_blueprint.menu_open = true;
    }
    public void ClickControlLayerButton(LayerButton click_whichLB)
    {
        if (click_whichLB.pressed)
        {
            ToolClosePartInterface();
            click_whichLB.self_image.sprite = click_whichLB.sp[2];
            click_whichLB.pressed = false;
            return;
        }

        foreach (var lb in all_layer_button)
        {
            if (lb != click_whichLB)
            {
                lb.self_image.sprite = lb.sp[0];
                lb.pressed = false;
            }
            else
            {
                lb.self_image.sprite = lb.sp[2];
                lb.pressed = true;
            }
        }
        BB.ClickCloseUnitInterfaceButton();
        ToolOpenPartInterface();
        foreach (var this_PPB in power_part_buttons)
        {
            this_PPB.gameObject.SetActive(false);
            this_PPB.CloseUIPart();
        }
        foreach (var this_LPB in leg_part_buttons)
        {
            this_LPB.gameObject.SetActive(false);
            this_LPB.CloseUIPart();
        }
        foreach (var this_CPB in control_part_buttons)
        {
            this_CPB.gameObject.SetActive(true);
            this_CPB.ShowUIPart();
        }
        foreach (var this_EPB in equip_part_buttons)
        {
            this_EPB.gameObject.SetActive(false);
            this_EPB.CloseUIPart();
        }

        unit_blueprint.menu_open = true;
    }
    public void ClickEquipmentLayerButton(LayerButton click_whichLB)
    {
        if (click_whichLB.pressed)
        {
            ToolClosePartInterface();
            click_whichLB.self_image.sprite = click_whichLB.sp[2];
            click_whichLB.pressed = false;
            return;
        }

        foreach (var lb in all_layer_button)
        {
            if (lb != click_whichLB)
            {
                lb.self_image.sprite = lb.sp[0];
                lb.pressed = false;
            }
            else
            {
                lb.self_image.sprite = lb.sp[2];
                lb.pressed = true;
            }
        }
        BB.ClickCloseUnitInterfaceButton();
        ToolOpenPartInterface();
        foreach (var this_PPB in power_part_buttons)
        {
            this_PPB.gameObject.SetActive(false);
            this_PPB.CloseUIPart();
        }
        foreach (var this_LPB in leg_part_buttons)
        {
            this_LPB.gameObject.SetActive(false);
            this_LPB.CloseUIPart();
        }
        foreach (var this_CPB in control_part_buttons)
        {
            this_CPB.gameObject.SetActive(false);
            this_CPB.CloseUIPart();
        }
        foreach (var this_EPB in equip_part_buttons)
        {
            this_EPB.gameObject.SetActive(true);
            this_EPB.ShowUIPart();
        }

        unit_blueprint.menu_open = true;
    }

    #endregion
    #region ClickPartButton
    public void ClickPartButton(PartButton click_this_button)
    {
        //计算零件出生位置：
        ////Vector3 start_pos = SetPosWhenPartCreat();

        switch ((int)click_this_button.part_type)
        {
            case 0:

                if (unit_blueprint.have_power_part)
                {
                    Debug.LogError("设计平台上已经存在一个动力组件！");
                    self_AS.clip = all_ACs["Error"];
                    self_AS.Play();
                    return;
                }
              
                part_prefab = Instantiate(Resources.Load<GameObject>("Part/PowerPart/" + click_this_button.part_name));
                PowerPart temp_power_part = part_prefab.GetComponent<PowerPart>();
                temp_power_part.enabled = true;
                temp_power_part.Init();//手动初始化，因为Start执行顺序有问题
                temp_power_part.CreatFromButton();
                temp_power_part.be_creat_from_button = true;
                break;

            case 1:

                GameObject temp_go = Resources.Load<GameObject>("Part/MovementPart/LegPart/" + click_this_button.part_name);
                part_prefab = Instantiate(temp_go);
                //part_prefab.transform.position = start_pos;
                LegPart temp_leg_part = part_prefab.GetComponent<LegPart>();
                temp_leg_part.enabled = true;
                temp_leg_part.Init();//手动初始化，因为Start执行顺序有问题
                temp_leg_part.CreatFromButton();
                temp_leg_part.be_creat_from_button = true;
                break;
            case 2:
                if (unit_blueprint.have_ctrl_part)
                {
                    Debug.Log("注意：设计平台上已经存在一个控制组件！");
                    //break;
                }

                part_prefab = Instantiate(Resources.Load<GameObject>("Part/ControlPart/" + click_this_button.part_name));
                ControlPart temp_control_part= part_prefab.GetComponent<ControlPart>();
                temp_control_part.enabled = true;
                temp_control_part.Init();//手动初始化，因为Start执行顺序有问题
                temp_control_part.CreatFromButton();
                temp_control_part.be_creat_from_button = true;
                break;
            case 3:
                part_prefab = Instantiate(Resources.Load<GameObject>("Part/EquipmentPart/" + click_this_button.part_name));
                EquipmentPart temp_equip_part = part_prefab.GetComponent<EquipmentPart>();
                temp_equip_part.enabled = true;
                temp_equip_part.Init();//手动初始化，因为Start执行顺序有问题
                temp_equip_part.CreatFromButton();
                temp_equip_part.be_creat_from_button = true;
                break;
        }

        //播放创建零件的音效：
        creat_part_audio.Play();
        //关闭悬停高亮Ui：
        click_this_button.hover_image.enabled = false;

        foreach (var lb in all_layer_button)
        {
            lb.self_image.sprite = lb.sp[0];
            lb.pressed = false;
        }

        ToolClosePartInterface();
        unit_blueprint.menu_open = false;
    }

    Vector3 SetPosWhenPartCreat()
    {
        RaycastHit hitwhat;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitwhat, 2000f, 1 << 10))
        {
            Vector3 real_pos = hitwhat.point;
            Vector3 start_pos = new Vector3
                (
                    Mathf.Clamp(real_pos.x, unit_blueprint.max_size_minusX, unit_blueprint.max_size_X),
                    Mathf.Clamp(real_pos.y, unit_blueprint.max_size_minusY, unit_blueprint.max_size_Y),
                    Mathf.Clamp(real_pos.z, unit_blueprint.max_size_minusZ, unit_blueprint.max_size_Z)
                );
            return start_pos;
        }
        else
        {
            Debug.LogError("虚拟平面丢失！！");
            return new Vector3(0, 0, 0);
        }
    }
    #endregion
    public void ClickClosePartInterfaceButton()
    {
        foreach (var lb in all_layer_button)
        {
            lb.self_image.sprite = lb.sp[0];
            lb.pressed = false;
        }
        creat_part_audio.Play();
        ToolClosePartInterface();       
    }

    #region 工具性函数
    public void ToolClosePartInterface()
    {
        part_interface_top.gameObject.SetActive(false);
        part_interface_ground.gameObject.SetActive(false);
        part_interface.gameObject.SetActive(false);
        part_interface_middle.gameObject.SetActive(false);
        ToolCloseAllUIPart();

        unit_blueprint.menu_open = false;
    }
    public void ToolOpenPartInterface()
    {
        part_interface_top.gameObject.SetActive(true);
        part_interface_ground.gameObject.SetActive(true);
        part_interface.gameObject.SetActive(true);
        part_interface_middle.gameObject.SetActive(true);

        unit_blueprint.menu_open = true;
    }
    void ToolCloseAllUIPart()
    {
        foreach (var pb in all_part_buttons)
        {
            pb.UIpart_prefab.SetActive(false);
        }
    }

    #endregion

    #region PowerPartButton

    public void BanPowerPartButton()
    {
        foreach(var this_PPB in power_part_buttons)
        {
            this_PPB.BanSelf();
        }
    }
    public void ReleasePowerPartButton()
    {
        foreach (var this_PPB in power_part_buttons)
        {
            this_PPB.ReleaseSelf();
        }
    }
    #endregion
    #region LegPartButton

    #endregion

}
