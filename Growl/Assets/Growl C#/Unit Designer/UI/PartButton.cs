using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PartType part_type;
    public string part_name;//实例化零件时用的名字
    public string UIpart_name;//实例化UI零件时用的名字
    RectTransform canvas;
    Camera UIpart_camera;
    Camera UI_camera;
    Vector3 self_screen_pos;
    Vector3 self_world_pos;
    Transform UIpart_group;
    Vector3 part_pos_default;

    
    [HideInInspector]
    public GameObject UIpart_prefab;
    #region UI相关
    GameObject temp_part_prefab;//为了获得零件数据，临时实例化一个零件
    Vector3 UIpart_offset;
    Text name_text;
    Image ban_image;
    iIcon self_i_icon;
    [HideInInspector]
    public Image hover_image;
    [HideInInspector]
    public GameObject part_particulars_UI;

    #endregion

    #region 重要外部引用
    protected UnitBlueprint unit_blueprint;
    RectTransform UI_canvas_top;
    #endregion
    void Start()
    {

    }
    public void Init()
    {               
        part_pos_default = new Vector3(-2.5f, 1.3f, -1f);
        UIpart_camera = GameObject.Find("UIPartCamera").GetComponent<Camera>();
        UI_camera = GameObject.Find("UICamera").GetComponent<Camera>();
        canvas = GameObject.Find("UICanvasBase").transform as RectTransform;
        UIpart_group = GameObject.Find("UIPart").transform;
        UIpart_offset = new Vector3(0, 0, -0.2f);
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        UI_canvas_top = GameObject.Find("UICanvasTop").transform as RectTransform;
        self_screen_pos = Camera.main.WorldToScreenPoint(transform.position);

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == "HoverImage")
            {
                hover_image = t.GetComponent<Image>();

            }
            else if (t.name == "BanImage")
            {
                ban_image = t.GetComponent<Image>();
            }
            else if (t.name == "PartName")
            {
                name_text = t.GetComponent<Text>();             
            }
            else if(t.name== "iIcon")
            {
                self_i_icon = t.GetComponent<iIcon>();
                self_i_icon.parent_button = this;              
            }
        }
        switch ((int)part_type)
        {
            case 0:
                UIpart_prefab = Instantiate(Resources.Load<GameObject>("UIPart/PowerUIPart/" + UIpart_name), UIpart_group);
                part_particulars_UI = Instantiate(Resources.Load<GameObject>("UI/PartParticulars/PowerPartParticulars"), UI_canvas_top);               
                temp_part_prefab = Instantiate(Resources.Load<GameObject>("Part/PowerPart/" + part_name));
                PowerPart temp_powerpart = temp_part_prefab.GetComponent<PowerPart>();
                name_text.text = temp_powerpart.self_name;
                foreach (RectTransform rt in part_particulars_UI.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Name")
                    {
                        rt.GetComponent<Text>().text = temp_powerpart.self_name;                   
                    }
                    else if (rt.name == "Power")
                    {
                        rt.GetComponent<Text>().text = "能量  +" + temp_powerpart.power_value.ToString();
                    }
                    else if (rt.name == "Weight")
                    {
                        rt.GetComponent<Text>().text = "重量  " + temp_powerpart.weight_value.ToString();
                    }
                    else if (rt.name == "Durability")
                    {
                        rt.GetComponent<Text>().text = "耐久  " + temp_powerpart.durability_value.ToString();
                    }
                    else if(rt.name == "Specialty")
                    {                      
                        //rt.GetComponent<Text>().text = "特性  " + temp_powerpart.durability_value.ToString();
                    }
                    else if (rt.name == "NormalDEF")
                    {
                        rt.GetComponent<Text>().text = temp_powerpart.normalDEF_value.ToString();
                    }
                    else if (rt.name == "BlastDEF")
                    {
                        rt.GetComponent<Text>().text = temp_powerpart.blastDEF_value.ToString();
                    }
                    else if (rt.name == "EnergyDEF")
                    {
                        rt.GetComponent<Text>().text =  temp_powerpart.energyDEF_value.ToString();
                    }
                    else if (rt.name == "Cost")
                    {
                        rt.GetComponent<Text>().text = temp_powerpart.cost.ToString();
                    }
                }
                break;
            case 1:
                UIpart_prefab = Instantiate(Resources.Load<GameObject>("UIPart/MovementUIPart/" + UIpart_name), UIpart_group);
                part_particulars_UI = Instantiate(Resources.Load<GameObject>("UI/PartParticulars/LegPartParticulars"), UI_canvas_top);
                temp_part_prefab = Instantiate(Resources.Load<GameObject>("Part/MovementPart/LegPart/" + part_name));
                LegPart temp_legpart = temp_part_prefab.GetComponent<LegPart>();
                name_text.text = temp_legpart.self_name;
                foreach (RectTransform rt in part_particulars_UI.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Name")
                    {
                        rt.GetComponent<Text>().text = temp_legpart.self_name;
                    }
                    else if (rt.name == "UsePower")
                    {
                        rt.GetComponent<Text>().text = "持续耗能    " + temp_legpart.usepower_value.ToString();
                    }
                    else if (rt.name == "Load")
                    {
                        rt.GetComponent<Text>().text = "最大载重    " + temp_legpart.load_value.ToString();
                    }
                    else if (rt.name == "Move")
                    {
                        rt.GetComponent<Text>().text = "移动力       " + temp_legpart.move_value.ToString();
                    }
                    else if(rt.name == "Dodge")
                    {
                        rt.GetComponent<Text>().text = "灵活性       " + temp_legpart.dodge_value.ToString();
                    }
                    else if (rt.name == "Durability")
                    {
                        rt.GetComponent<Text>().text = "耐久         " + temp_legpart.durability_value.ToString();
                    }
                    else if (rt.name == "Specialty")
                    {
                        if (temp_legpart.specialty.Count == 0)
                        {

                            rt.GetComponent<Text>().text = "";
                        }
                        else
                        {
                            string spec = "";
                            foreach (int sp in temp_legpart.specialty)
                            {
                                switch (sp)
                                {
                                    case 0:
                                        spec += "闪避叠加  ";
                                        break;
                                    case 1:
                                        spec += "反应装甲  ";
                                        break;

                                }

                            }
                            rt.GetComponent<Text>().text = "特性         " + spec;
                        }
 
                    }
                    else if (rt.name == "NormalDEF")
                    {
                        rt.GetComponent<Text>().text = temp_legpart.normalDEF_value.ToString();
                    }
                    else if (rt.name == "BlastDEF")
                    {
                        rt.GetComponent<Text>().text = temp_legpart.blastDEF_value.ToString();
                    }
                    else if (rt.name == "EnergyDEF")
                    {
                        rt.GetComponent<Text>().text = temp_legpart.energyDEF_value.ToString();
                    }
                    else if (rt.name == "Cost")
                    {
                        rt.GetComponent<Text>().text = temp_legpart.cost.ToString();
                    }
                }
                break;
            case 2:
                UIpart_prefab = Instantiate(Resources.Load<GameObject>("UIPart/ControlUIPart/" + UIpart_name), UIpart_group);
                part_particulars_UI = Instantiate(Resources.Load<GameObject>("UI/PartParticulars/ControlPartParticulars"), UI_canvas_top);
                temp_part_prefab = Instantiate(Resources.Load<GameObject>("Part/ControlPart/" + part_name));
                ControlPart temp_controlpart = temp_part_prefab.GetComponent<ControlPart>();
                name_text.text = temp_controlpart.self_name;
                foreach (RectTransform rt in part_particulars_UI.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Name")
                    {
                        rt.GetComponent<Text>().text = temp_controlpart.self_name;
                    }
                    else if (rt.name == "UsePower")
                    {
                        rt.GetComponent<Text>().text = "持续耗能    " + temp_controlpart.usepower_value.ToString();
                    }
                    else if (rt.name == "Weight")
                    {
                        rt.GetComponent<Text>().text = "重量    " + temp_controlpart.weight_value.ToString();
                    }
                    else if (rt.name == "Modules")
                    {
                        rt.GetComponent<Text>().text = "模块    " + temp_controlpart.modules_value.ToString();
                    }
                    else if (rt.name == "Durability")
                    {
                        rt.GetComponent<Text>().text = "耐久    " + temp_controlpart.durability_value.ToString();
                    }
                    else if (rt.name == "NormalDEF")
                    {
                        rt.GetComponent<Text>().text = temp_controlpart.normalDEF_value.ToString();
                    }
                    else if (rt.name == "BlastDEF")
                    {
                        rt.GetComponent<Text>().text = temp_controlpart.blastDEF_value.ToString();
                    }
                    else if (rt.name == "EnergyDEF")
                    {
                        rt.GetComponent<Text>().text = temp_controlpart.energyDEF_value.ToString();
                    }
                    else if (rt.name == "Cost")
                    {
                        rt.GetComponent<Text>().text = temp_controlpart.cost.ToString();
                    }
                }
                break;
            case 3:
                UIpart_prefab = Instantiate(Resources.Load<GameObject>("UIPart/EquipmentUIPart/" + UIpart_name), UIpart_group);
                part_particulars_UI = Instantiate(Resources.Load<GameObject>("UI/PartParticulars/EquipmentPartParticulars"), UI_canvas_top);
                temp_part_prefab = Instantiate(Resources.Load<GameObject>("Part/EquipmentPart/" + part_name));
                EquipmentPart temp_equippart = temp_part_prefab.GetComponent<EquipmentPart>();
                name_text.text = temp_equippart.self_name;
                foreach (RectTransform rt in part_particulars_UI.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Name")
                    {
                        rt.GetComponent<Text>().text = temp_equippart.self_name;
                    }
                    else if(rt.name == "PartType")
                    {
                        string s1 = "";
                        string s2 = "";

                       
                        switch ((int)temp_equippart.damage_type)
                        {
                            case 0:
                                //normal                          
                                s1 = "常规武器";
                                break;
                            case 1:
                                //blast
                                s1 = "爆炸武器";
                                break;
                            case 2:
                                //energy
                                s1 = "能量武器";
                                break;
                        }
                        switch ((int)temp_equippart.atk_pattern)
                        {
                            case 0:
                                s2 = "射击";
                                break;
                            case 1:
                                s2 = "飞弹";
                                break;
                        }

                        rt.GetComponent<Text>().text = "装备·" + s1 + "·" + s2;

                    }
                    else if (rt.name == "UsePower")
                    {
                        rt.GetComponent<Text>().text = "持续耗能    " + temp_equippart.usepower_value.ToString();
                    }
                    else if (rt.name == "EachUsePower")
                    {
                        rt.GetComponent<Text>().text = "使用耗能    " + temp_equippart.eachusepower_value.ToString();
                    }
                    else if (rt.name == "Damage")
                    {
                        rt.GetComponent<Text>().text = "伤害             " + temp_equippart.damage.ToString() + " * " + temp_equippart.attack_number.ToString();
                    }
                    else if(rt.name == "DamageTypeIcon")
                    {
                        switch ((int)temp_equippart.damage_type)
                        {
                            case 0:
                                //normal                          
                                rt.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Icon/DamageTypeIcons")[0];
                                break;
                            case 1:
                                //blast
                                rt.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Icon/DamageTypeIcons")[1];
                                break;
                            case 2:
                                //energy
                                rt.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Icon/DamageTypeIcons")[2];
                                break;
                        }                       
                    }
                    else if(rt.name == "Range")
                    {
                        rt.GetComponent<Text>().text = "射程          " + temp_equippart.range_value.ToString();
                    }
                    else if (rt.name == "Ammo")
                    {
                        string one;
                        string two;
                        if (temp_equippart.ammo_value == -999 )
                        {
                            one = "∞";
                        }
                        else
                        {
                            one = temp_equippart.ammo_value.ToString();
                        }
                        if(temp_equippart.magazine_value == -999)
                        {
                            two = "∞";
                        }
                        else
                        {
                            two = temp_equippart.magazine_value.ToString();
                        }
                        rt.GetComponent<Text>().text = "弹药          " + one + "/" + two;
                    }
                    else if (rt.name == "Specialty")
                    {
                        if (temp_equippart.specialty.Count == 0)
                        {
                            
                            rt.GetComponent<Text>().text = "";                           
                        }
                        else
                        {
                            string spec = "";
                            foreach (int sp in temp_equippart.specialty)
                            {
                                switch (sp)
                                {
                                    case 0:
                                        spec += "连射  ";
                                        break;
                                    case 1:
                                        spec += "  ";
                                        break;

                                }

                            }
                            rt.GetComponent<Text>().text = "特性         " + spec;
                        }
                      
                    }                    
                    else if (rt.name == "Weight")
                    {
                        rt.GetComponent<Text>().text = "重量          " + temp_equippart.weight_value.ToString();
                    }
                    else if (rt.name == "Durability")
                    {
                        rt.GetComponent<Text>().text = "耐久          " + temp_equippart.durability_value.ToString();
                    }
                    else if(rt.name == "Tag")
                    {
                        if (temp_equippart.symmetry)
                        {
                            rt.GetComponentInChildren<Text>().text = "对称";
                        }
                        else
                        {
                            rt.GetComponentInChildren<Text>().text = "多方向";
                        }
                        
                    }
                    else if (rt.name == "NormalDEF")
                    {
                        rt.GetComponent<Text>().text = temp_equippart.normalDEF_value.ToString();
                    }
                    else if (rt.name == "BlastDEF")
                    {
                        rt.GetComponent<Text>().text = temp_equippart.blastDEF_value.ToString();
                    }
                    else if (rt.name == "EnergyDEF")
                    {
                        rt.GetComponent<Text>().text = temp_equippart.energyDEF_value.ToString();
                    }
                    else if(rt.name == "Cost")
                    {
                        rt.GetComponent<Text>().text = temp_equippart.cost.ToString();
                    }
                }
                break;

        }

        Destroy(temp_part_prefab);//销毁临时零件
        part_particulars_UI.GetComponent<PartParticulars>().parent_button = this;       
        part_particulars_UI.SetActive(false);//关闭零件详情页

    }

    void Update()
    {
        SetUIPartPostition();
        //Debug.Log(name + transform.position);
    }

    void SetUIPartPostition()
    {
        if (UIpart_prefab != null)
        {
            UIpart_prefab.transform.position = transform.position + UIpart_offset;
        }
    }
    public void CloseUIPart()
    {
        if (UIpart_prefab == null)
        {

        }
        else
        {
            UIpart_prefab.gameObject.SetActive(false);
        }

    }
    public void ShowUIPart()
    {
        if (UIpart_prefab == null)
        {

        }
        else
        {
            UIpart_prefab.gameObject.SetActive(true);
        }

    }

    public void BanSelf()
    {
        ban_image.GetComponent<Image>().enabled = true;
    }
    public void ReleaseSelf()
    {
        ban_image.GetComponent<Image>().enabled = false;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        hover_image.enabled = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hover_image.enabled = false;
    }



}
