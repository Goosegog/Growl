using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour
{
    // UnitInfo 单例挂在单位设计场景中的单位属性面板上，负责统计当前设计的全局属性，需要注意这个单例和设计蓝图脚本的区别

    static UnitInfo _unit_info;
    public static UnitInfo GetUnitInfo
    {
        get { return _unit_info; }
        
    }


    AudioSource self_AS;
    #region 重要外部引用
    UnitBlueprint unit_blueprint;
    //RectTransform unit_info_interface;
    Transform rotation_point;
    Dictionary<string,AudioClip> all_audioclip;
    #endregion
    #region 合法性相关
    [HideInInspector]
    public bool legal;//当前设计是否合法
    bool name_legal;
    [HideInInspector]
    public bool power_legal;
    bool weight_legal;
    bool movement_legal;
    bool ctrl_legal;


    #endregion
    [HideInInspector]
    public string unit_name;//玩家给当前设计起的名字

    bool initover = false;
    int need_power = 0;
    int power_max = 0;
    int need_weight = 0;
    int load_max = 0;

    int movement_now = 999;
    int dodge_now = 999;
    int cost_now = 0;

    int modules_count = 0;

    #region UI相关
    Sprite[] WarningColor;
    Image power_part_UI;
    Text power_part_UItext;
    Image ctrl_part_UI;
    Text ctrl_part_UItext;
    Image movement_part_UI;
    Text movement_part_UItext;
    GameObject module_error;
    [HideInInspector]
    public Text unit_name_text;
    Image input_warning_UI;

    Text power;
    Text power_text;
    Text movement;
    Text movement_text;
    Text weight;
    Text weight_text;
    Text dodge;
    Text dodge_text;
    Text cost_text;
    List<int> compare4dodge;

    [HideInInspector]
    public List<RectTransform> modules;
    [HideInInspector]
    public List<ModuleButtonInUnitInfoInterface> modules_script;
    bool have_unoccupied_module = true;
    #endregion

    void Start ()
    {
        _unit_info = this;
        self_AS = GetComponent<AudioSource>();
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        rotation_point = GameObject.Find("Rotation point").transform;
        all_audioclip = new Dictionary<string, AudioClip>();
        all_audioclip.Add("PowerDown", Resources.Load<AudioClip>("AudioClip/PowerDown"));
        all_audioclip.Add("PowerUp", Resources.Load<AudioClip>("AudioClip/PowerUp"));
        WarningColor = Resources.LoadAll<Sprite>("Icon/WarningColor");
        modules = new List<RectTransform>();
        modules_script = new List<ModuleButtonInUnitInfoInterface>();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if(t.name== "UnitName")
            {
                unit_name_text = t.GetComponent<Text>();
                foreach (Transform tt in unit_name_text.GetComponentsInChildren<Transform>())
                {
                    if (tt.name == "InputWarningUI")
                    {
                        input_warning_UI = tt.GetComponent<Image>();
                    }
                }
            }
            else if (t.name == "PowerPartError")
            {
                power_part_UI = t.GetComponent<Image>();
                power_part_UItext = power_part_UI.GetComponentInChildren<Text>();
            }
            else if(t.name == "CtrlPartError")
            {
                ctrl_part_UI = t.GetComponent<Image>();
                ctrl_part_UItext = ctrl_part_UI.GetComponentInChildren<Text>();
            }
            else if(t.name == "MovementPartError")
            {
                movement_part_UI = t.GetComponent<Image>();
                movement_part_UItext = movement_part_UI.GetComponentInChildren<Text>();
            }
            else if(t.name == "ModuleError")
            {
                module_error = t.gameObject;
            }
            else if(t.name == "Power")
            {
                power = t.GetComponent<Text>();
                foreach (Transform tt in power.GetComponentsInChildren<Transform>())
                {
                    if(tt.name== "PowerValue")
                    {
                        power_text = tt.GetComponent<Text>();
                    }
                    
                }                   
            }
            else if(t.name == "Movement")
            {
                movement = t.GetComponent<Text>();
                foreach (Transform tt in movement.GetComponentsInChildren<Transform>())
                {
                    if (tt.name == "MovementValue")
                    {
                        movement_text = tt.GetComponent<Text>();
                    }

                }
            }
            else if(t.name == "Weight")
            {
                weight = t.GetComponent<Text>();
                foreach (Transform tt in weight.GetComponentsInChildren<Transform>())
                {
                    if (tt.name == "WeightValue")
                    {
                        weight_text = tt.GetComponent<Text>();
                    }

                }              
            }
            else if(t.name == "Dodge")
            {
                dodge = t.GetComponent<Text>();
                foreach (Transform tt in dodge.GetComponentsInChildren<Transform>())
                {
                    if (tt.name == "DodgeValue")
                    {
                        dodge_text = tt.GetComponent<Text>();
                    }
                }
            }
            else if(t.name == "Cost")
            {
                foreach(Transform tt in t.GetComponentsInChildren<Transform>())
                {
                    if (tt.name == "CostValue")
                    {
                        cost_text = tt.GetComponent<Text>();
                    }
                }
                    
            }
            else if(t.name== "ModulesGroup")
            {
                foreach (Transform tt in t.GetComponentsInChildren<Transform>())
                {
                    if (tt.tag == "ModuleButton")
                    {
                        modules.Add(tt as RectTransform);
                        tt.GetComponent<ModuleButtonInUnitInfoInterface>().Init();
                        modules_script.Add(tt.GetComponent<ModuleButtonInUnitInfoInterface>());
                    }
                }
            }

        }

        compare4dodge = new List<int>();

        CountPowerValue();
        SetPowerValueUI();
        CountWeightValue();
        SetWeightValueUI();
        CountMovementValue();
        SetMovementValueUI();        
        CountDodgeValue();
        SetDodgeValueUI();
        CountCostValue();
        SetCostValue();
        CountModulesCount();
        SetModulesCount();
        CountModuleNumber();
        initover = true;
    }
	
	void Update ()
    {
        SetPositionByRotation();
        HavePowerPart();
        HaveCtrlPart();
        HaveMovementPart();
        CountPowerValue();
        CountWeightValue();
        CountMovementValue();
        CountDodgeValue();
        CountCostValue();
        CountModulesCount();
        CountCostValue();
        CountModuleNumber();
        CheckLegal();
    }
    void SetPositionByRotation()
    {
        if (rotation_point.rotation.y > 0)
        {

        }
    }

    public void SetUnitName()
    {
        unit_name = unit_name_text.text.ToString();
        if (unit_name == "")
        {
            input_warning_UI.enabled = true;
        }
    }
    public void SetUnitNameUI()
    {
        input_warning_UI.enabled = false;
    }

    void CountModulesCount()
    {
        int _count_now = 0;

        if (!unit_blueprint.have_ctrl_part)
        {
            _count_now = 0;
        }
        else
        {
            foreach(var p in unit_blueprint.all_ctrl_parts)
            {
                _count_now += p.GetComponent<ControlPart>().modules_value;
                if (_count_now > 8)
                {
                    _count_now = 8;
                    return;
                }
            }
        }

        if (_count_now != modules_count)
        {
            modules_count = _count_now;
            SetModulesCount();
        }
    }

    void SetModulesCount()
    {      
        for(int i = 0; i <= modules_count - 1; i++)
        {
            modules[i].gameObject.SetActive(true);
        }
        for(int i = modules_count; i < 8; i++)
        {
            modules[i].gameObject.SetActive(false);
        }
    }

    void CountPowerValue()
    {
        int power_max_now = 0;
        int need_power_now = 0;

        if (unit_blueprint.have_power_part)
        {          
            foreach (var pp in unit_blueprint.all_power_parts)
            {               
                power_max_now += pp.GetComponent<PowerPart>().power_value;
            }
        }
        else
        {
            power_max_now = 0;
        }
               
        foreach(var p in unit_blueprint.all_legal_parts)
        {
            if (p.usepower_value == -999)
            {
                continue;
            }
            else
            {
                need_power_now += p.usepower_value;
            }
            
        }

        if (need_power_now != need_power || power_max_now != power_max)
        {
            if (initover && power_max < need_power && need_power_now < power_max_now)
            {
                self_AS.clip = all_audioclip["PowerUp"];
                self_AS.Play();
            }
            need_power = need_power_now;
            power_max = power_max_now;           
            SetPowerValueUI();
        }
    }
    void SetPowerValueUI()
    {
        if (power_max == 0)
        {
            power.text = "<color=#FF0003>能量</color>";
            power_text.text = "<color=#FF0003>0</color>" + "<color=#FF0003> /</color>" + "<color=#FF0003>0</color>";
            self_AS.clip = all_audioclip["PowerDown"];
            power_legal = false;

        }
        else
        {
            // int power_max = unit_blueprint.all_power_parts[0].GetComponent<PowerPart>().power_value;
            int surplus = power_max - need_power;
            Debug.LogError("surplus/power_max = " + ((float)surplus) / ((float)power_max));
            float ratio = ((float)surplus) / ((float)power_max);
            if (ratio >= 0.3)
            {
                power.text = "<color=#00FAFF>能量</color>";
                power_text.text = "<color=#00FAFF>" + surplus.ToString() + "</color>" + "<color=#00FAFF> /</color>" + "<color=#00FAFF>" + power_max.ToString() + "</color>";

                power_legal = true;
            }
            else if(ratio > 0)
            {
                power.text = "<color=#FFE000>能量</color>";
                power_text.text = "<color=#FFE000>" + surplus.ToString() + "</color>" + "<color=#FFE000> /</color>" + "<color=#FFE000>" + power_max.ToString() + "</color>";

                power_legal = true;

            }
            else
            {
                power.text = "<color=#FF0003>能量</color>";
                power_text.text = "<color=#FF0003>" + surplus.ToString() + "</color>" + "<color=#FF0003> /</color>" + "<color=#FF0003>" + power_max.ToString() + "</color>";
                self_AS.clip = all_audioclip["PowerDown"];
                self_AS.Play();
                power_legal = false;
            }
            
            Debug.Log("surplus = " + surplus);
        }
    }

    void CountWeightValue()
    {
        int load_max_now = 0;
        int need_weight_now = 0;

        if (unit_blueprint.all_leg_parts.Count == 0)
        {
            load_max_now = 0;
        }
        else
        {
            foreach(var p in unit_blueprint.all_leg_parts)
            {
                var LP = p.GetComponent<LegPart>();
                load_max_now += LP.load_value;
            }
            
        }

        foreach (var p in unit_blueprint.all_legal_parts)
        {
            if (p.weight_value == -999)
            {
                continue;
            }
            else
            {
                need_weight_now += p.weight_value;
            }
        }
        //Debug.LogError("总重量 " + need_weight_now);
        if(load_max_now != load_max || need_weight_now != need_weight)
        {
            load_max = load_max_now;
            need_weight = need_weight_now;
            SetWeightValueUI();
        }
    }
    void SetWeightValueUI()
    {
        if (need_weight == 0)
        {
            weight.text = "<color=#FF0003>重量</color>";
            weight_text.text = "<color=#FF0003>0</color>" + "<color=#FF0003> /</color>" + "<color=#FF0003>0</color>";

            weight_legal = false;
        }
        else
        {                                  
            float ratio = ((float)need_weight) / ((float)load_max);
            if (ratio <= 0.75f)
            {
                weight.text = "<color=#00FAFF>重量</color>";
                weight_text.text = "<color=#00FAFF>" + need_weight.ToString() + "</color>" + "<color=#00FAFF> /</color>" + "<color=#00FAFF>" + load_max.ToString() + "</color>";
                //power.color = new Color(0, 250, 255, 255);
                //power_text.color = new Color(0, 250, 255, 255);

                weight_legal = true;
            }
            else if (ratio <= 1f)
            {
                weight.text = "<color=#FFE000>重量</color>";
                weight_text.text = "<color=#FFE000>" + need_weight.ToString() + "</color>" + "<color=#FFE000> /</color>" + "<color=#FFE000>" + load_max.ToString() + "</color>";

                weight_legal = true;
            }
            else
            {
                weight.text = "<color=#FF0003>重量</color>";
                weight_text.text = "<color=#FF0003>" + need_weight.ToString() + "</color>" + "<color=#FF0003> /</color>" + "<color=#FF0003>" + load_max.ToString() + "</color>";

                weight_legal = false;
            }

        }
    }
    void CountMovementValue()
    {
        int temp_movement = 999;

        if (unit_blueprint.all_leg_parts.Count == 0)
        {
            temp_movement = 999;
        }
        else
        {
            foreach (var p in unit_blueprint.all_leg_parts)
            {
                int p_move = p.GetComponent<LegPart>().move_value;
                if (temp_movement == 999)
                {
                    temp_movement = p_move;
                }
                else if (temp_movement > p_move)
                {
                    temp_movement = p_move;
                }
            }
        }

        if (temp_movement != movement_now)
        {
            Debug.LogError("移动力改变");
            movement_now = temp_movement;
            SetMovementValueUI();
        }
    }
    void SetMovementValueUI()
    {
        if(movement_now == 999)
        {
            movement.text = "<color=#FF0003>移动力</color>";
            movement_text.text = "<color=#FF0003>每点能量 0 格</color>";
        }
        else
        {
            movement.text = "<color=#00FAFF>移动力</color>";
            movement_text.text = "<color=#00FAFF>每点能量 </color>" + "<color=#00FAFF>" + movement_now.ToString() + "</color>" + "<color=#00FAFF> 格</color>";
        }
        
    }
    void CountDodgeValue()
    {
        compare4dodge.Clear();// compare4dodge 列表用来储存没有“灵敏叠加”特性的组件的灵活性
        int temp_dodge_add = 999;//这个变量单独储存有“灵敏叠加”特性的组件的叠加后灵活性
        int temp_dodge_finaly = 999;//上面两项比较后小的那个会赋值给这个变量
               
        if (unit_blueprint.all_leg_parts.Count == 0)
        {
            temp_dodge_finaly = 999;
        }
        else
        {
            foreach (var p in unit_blueprint.all_leg_parts)
            {
                var LP = p.GetComponent<LegPart>();
                int p_dodge = LP.dodge_value;

                if (LP.specialty.Contains(MovementPartSpecialty.DodgeAddition))
                {
                    if (temp_dodge_add == 999)
                    {
                        temp_dodge_add = p_dodge;
                    }
                    else
                    {
                        temp_dodge_add += p_dodge;
                    }                  
                }
                else
                {
                    compare4dodge.Add(p_dodge);                   
                }               
            }

            //把叠加后的灵活性数值也放入列表，再进行升序排序，则 compare4dodge[0] 就是实际的灵活性数值
            compare4dodge.Add(temp_dodge_add);
            
            compare4dodge.Sort();
            temp_dodge_finaly = compare4dodge[0];
        }

        if (temp_dodge_finaly != dodge_now)
        {
            Debug.LogError("灵活性改变");
            dodge_now = temp_dodge_finaly;
            SetDodgeValueUI();
        }
    }
    void SetDodgeValueUI()
    {
        if (dodge_now == 999)
        {
            dodge.text = "<color=#FF0003>灵活性</color>";
            dodge_text.text = "<color=#FF0003>0</color>";
        }
        else
        {
            dodge.text = "<color=#00FAFF>灵活性</color>";
            dodge_text.text = "<color=#00FAFF>" + dodge_now.ToString() + "</color>";
        }
    }
    void CountCostValue()
    {
        int cost_temp = 0;
        foreach (var p in unit_blueprint.all_legal_parts)
        {
            cost_temp += p.cost;
        }

        if (cost_now != cost_temp)
        {
            cost_now = cost_temp;
            SetCostValue();
        }
    }
    void SetCostValue()
    {
        cost_text.text = cost_now.ToString();
    }
    void CountModuleNumber()
    {
        
        if (!ctrl_legal)
        {
            have_unoccupied_module = false;
        }

        for (int i = 0; i < modules_count; i++)
        {
            if (!modules_script[i].installed)
            {
                have_unoccupied_module = true;
                break;
            }
            else
            {
                have_unoccupied_module = false;
            }
        }

        SetModuleError();

    }
    void SetModuleError()
    {
        if(have_unoccupied_module && !module_error.activeSelf) module_error.SetActive(true);
        if(!have_unoccupied_module && module_error.activeSelf) module_error.SetActive(false);
    }

    void CheckLegal()
    {
        name_legal = (unit_name != "");
        movement_legal = unit_blueprint.have_L_movement_part || unit_blueprint.have_R_movement_part;
        ctrl_legal = unit_blueprint.have_ctrl_part;

        if(name_legal && power_legal && weight_legal && movement_legal && ctrl_legal)
        {
            legal = true;
        }
        else
        {
            legal = false;
        }
    }

    void HavePowerPart()
    {
        if (!unit_blueprint.have_power_part)
        {
            //说明没有动力组件
            power_part_UI.gameObject.SetActive(true);
            power_part_UI.sprite = WarningColor[0];
            power_part_UItext.text = "<color=#FF0000>缺少 动力组件</color>";
            power_part_UItext.color = new Color(255, 0, 0, 255);
        }
        else
        {
            power_part_UI.gameObject.SetActive(false);
        }
    }
    void HaveCtrlPart()
    {
        if (!unit_blueprint.have_ctrl_part)
        {
            //说明没有 控制组件
            ctrl_legal = false;
            ctrl_part_UI.gameObject.SetActive(true);
            ctrl_part_UI.sprite = WarningColor[0];
            ctrl_part_UItext.text = "<color=#FF0000>缺少 控制组件</color>";
        }
        else if(unit_blueprint.all_ctrl_parts.Count > 1)
        {
            //有多个控制组件
            ctrl_legal = true;
            ctrl_part_UI.gameObject.SetActive(true);
            ctrl_part_UI.sprite = WarningColor[1];
            ctrl_part_UItext.text = "<color=#F7D308>使用多个控制组件可能会消耗大量能量</color>";
        }
        else
        {
            ctrl_legal = true;
            ctrl_part_UI.gameObject.SetActive(false);
        }
    }
    void HaveMovementPart()
    {
        if (!unit_blueprint.have_L_movement_part && !unit_blueprint.have_R_movement_part)
        {
            //说明一个移动组件都没有
            movement_part_UI.gameObject.SetActive(true);
            movement_part_UI.sprite = WarningColor[0];
            movement_part_UItext.text = "<color=#FF0000>缺少 移动组件</color>";
            movement_part_UItext.color = new Color(255, 0, 0, 255);
        }
        else
        {
            movement_part_UI.gameObject.SetActive(false);
        }
       
    }

    
}
