using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintButton : MonoBehaviour
{
    static BlueprintButton self;
    public static BlueprintButton GetBlueprintButton
    {
        get { return self; }
    }

    List<UnitButton> all_unit_buttons;   
    [HideInInspector]
    public int start_index = 0;
    [HideInInspector]
    public int end_index = 0;
    [HideInInspector]
    public bool load_user_data_over = false;

    [HideInInspector]
    public Sprite[] sp;
    [HideInInspector]
    public Image self_image;
    [HideInInspector]
    public bool pressed = false;

    #region 重要外部引用
    UnitBlueprint unit_blueprint;
    Transform UIunit_group;
    ButtonController BC;

    RectTransform UI_canvas_base;//大层，包含滚动零件菜单
    RectTransform unit_interface_top;
    RectTransform unit_interface_ground;//滚动单位菜单的父级
    RectTransform unit_interface;//滚动单位菜单
    RectTransform content;//所有 UnitButton 被本脚本创建后需要挂在这个物体下面
    RectTransform unit_interface_middle;//单位菜单遮挡用外框
    #endregion

    void Start ()
    {
        self = this;
        sp = Resources.LoadAll<Sprite>("Image/LayerButtonImage");
        self_image = GetComponent<Image>();
        
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        UIunit_group = GameObject.Find("UnitPart").transform;
        BC = GameObject.Find("ButtonController").GetComponent<ButtonController>();
        content = GameObject.Find("UICanvasBase/UnitInterfaceGround/UnitInterfaceBackGround/UnitInterface/Viewport/UnitInterfaceContent").transform as RectTransform;
        UI_canvas_base = GameObject.Find("UICanvasBase").transform as RectTransform;
        unit_interface_top = GameObject.Find("UICanvasTop/UnitInterface").transform as RectTransform;
        unit_interface_ground = GameObject.Find("UICanvasBase/UnitInterfaceGround").transform as RectTransform;
        unit_interface = GameObject.Find("UICanvasBase/UnitInterfaceGround/UnitInterfaceBackGround/UnitInterface").transform as RectTransform;
        
        unit_interface_middle = GameObject.Find("UICanvasMiddle/UnitInterfaceMiddle").transform as RectTransform;

        
        //调用协程加载所有用户数据:
        LoadUserDataAtStart();

        self_image.sprite = sp[0];

    }
	

	void Update ()
    {
        
    }
    #region 单位设计器场景 初始化方法
    public void LoadUserDataAtStart()
    {
        StartCoroutine(CreatUnitPartByBatch());
    }
    IEnumerator CreatUnitPartByBatch()
    {
        while (UserDataController.GetSingleton == null)
        {
            yield return null;
        }

        Debug.LogError("玩家数据里一共储存了多少个蓝图 = " + UserDataController.GetSingleton.UD.user_blueprints.Count);

        int max_index = UserDataController.GetSingleton.UD.user_blueprints.Count - 1;
        Debug.LogError(Time.realtimeSinceStartup);
        do
        {
            start_index = end_index;

            if (start_index + 8 >= max_index)
            {
                end_index = max_index;
            }
            else
            {
                end_index = start_index + 8;
            }

            CreatUnitPart(start_index, end_index);

            yield return null;
        }
        while (end_index != max_index);

        load_user_data_over = true;

        all_unit_buttons = new List<UnitButton>();
        foreach (var temp in unit_interface.GetComponentsInChildren<UnitButton>())
        {
            all_unit_buttons.Add(temp);
        }

        ToolCloseBlueprintInterface();
        Debug.LogError(Time.realtimeSinceStartup);
    }
    #endregion

    public void ChickBlueprintButton()
    {
        if (pressed)
        {
            //点击蓝图时，蓝图处于按下状态，则关闭全部面板
            self_image.sprite = sp[0];
            pressed = false;
            BC.ClickClosePartInterfaceButton();
            ToolCloseBlueprintInterface();
            unit_blueprint.menu_open = false;
            return;
        }
        //点击蓝图时，蓝图没有处于按下状态，则打开蓝图并且将除了蓝图外的所有按钮image置为normal
        self_image.sprite = sp[2];
        pressed = true;
        BC.ClickClosePartInterfaceButton();
        ToolOpenBlueprintInterface();
        unit_blueprint.menu_open = true;
    }
    public void ClickCloseUnitInterfaceButton()
    {
        self_image.sprite = sp[0];
        pressed = false;
        ToolCloseBlueprintInterface();
        unit_blueprint.menu_open = false;
    }
    public bool CreatUnitPartLastOne()
    {
        //这个方法会读取用户存档中最后一个蓝图实例化出来
        int max_index = UserDataController.GetSingleton.UD.user_blueprints.Count - 1;
        CreatUnitPart(max_index, max_index);
        ToolCloseAllUIunit();
        return true;
    }
    bool CreatUnitPart(int start, int end)
    {
        //这个方法会读取用户存档中从指定开始下标到指定结束下标数据，将其实例化为 UnitPart 物体
        for(int i = start; i <= end; i++)
        {
            UnitStruct us = UserDataController.GetSingleton.UD.user_blueprints[i];
            //创建一个空物体并以当前读取的unit的名字命名：
            GameObject this_UIunit = new GameObject("Empty");
            this_UIunit.name = us.unit_name;

            //根据当前unit结构体里的信息，先把需要用到的零件都实例化出来并挂在 this_unit 下：           
            foreach (var pss in us.all_part_serialize_struct)
            {
                switch (pss.part_type_int)
                {
                    case 0:
                        GameObject power_part_prefab = Instantiate(Resources.Load<GameObject>("Part/PowerPart/" + pss.part_prefab_name));
                        power_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                        power_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                        Destroy(power_part_prefab.GetComponent<PowerPart>());
                        Destroy(power_part_prefab.GetComponent<BoxCollider>());
                        Destroy(power_part_prefab.GetComponent<Rigidbody>());

                        power_part_prefab.transform.SetParent(this_UIunit.transform);

                        break;
                    case 1:
                        GameObject leg_part_prefab = Instantiate(Resources.Load<GameObject>("Part/MovementPart/LegPart/" + pss.part_prefab_name));
                        leg_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                        leg_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                        Destroy(leg_part_prefab.GetComponent<PowerPart>());
                        Destroy(leg_part_prefab.GetComponent<BoxCollider>());
                        Destroy(leg_part_prefab.GetComponent<Rigidbody>());

                        leg_part_prefab.transform.SetParent(this_UIunit.transform);

                        break;
                    case 2:
                        GameObject ctrl_part_prefab = Instantiate(Resources.Load<GameObject>("Part/ControlPart/" + pss.part_prefab_name));
                        ctrl_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                        ctrl_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                        Destroy(ctrl_part_prefab.GetComponent<PowerPart>());
                        Destroy(ctrl_part_prefab.GetComponent<BoxCollider>());
                        Destroy(ctrl_part_prefab.GetComponent<Rigidbody>());

                        ctrl_part_prefab.transform.SetParent(this_UIunit.transform);

                        break;
                    case 3:
                        GameObject E_part_prefab = Instantiate(Resources.Load<GameObject>("Part/EquipmentPart/" + pss.part_prefab_name));
                        E_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                        E_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                        Destroy(E_part_prefab.GetComponent<PowerPart>());
                        Destroy(E_part_prefab.GetComponent<BoxCollider>());
                        Destroy(E_part_prefab.GetComponent<Rigidbody>());

                        E_part_prefab.transform.SetParent(this_UIunit.transform);
                        break;
                }
            }

            this_UIunit.tag = "Untagged";
            this_UIunit.layer = 13;
            //Debug.LogError("length: " + this_UIunit.GetComponentsInChildren<Transform>().Length);
            foreach (var child in this_UIunit.GetComponentsInChildren<Transform>())
            {
                child.tag = "Untagged";
                child.gameObject.layer = 13;
            }

            this_UIunit.transform.SetParent(UIunit_group);
            //创建一个 UnitButton ：
            CreatUnitButton(this_UIunit, us);
        }
        return true;
    }
    bool CreatUnitButton(GameObject _UIunit, UnitStruct _us)
    {
        //从资源池里实例化一个 UnitButtonDefault 并挂在 content 下：
        GameObject this_unit_button = Instantiate(Resources.Load<GameObject>("UI/UnitButton/UnitButtonDefault"), content);
        //获取这个 UnitButtonDefault 上的 UnitButton 脚本：
        UnitButton ub = this_unit_button.GetComponent<UnitButton>();
        //初始化这个脚本：
        ub.Init(_UIunit);
        ub.self_unit_struct = _us;
        return true;
    }
    #region 工具性方法
    public void ToolCloseBlueprintInterface()
    {
        unit_interface_top.gameObject.SetActive(false);
        unit_interface_ground.gameObject.SetActive(false);
        unit_interface.gameObject.SetActive(false);
        unit_interface_middle.gameObject.SetActive(false);
        ToolCloseAllUIunit();

        unit_blueprint.menu_open = false;
    }
    public void ToolOpenBlueprintInterface()
    {
        unit_interface_top.gameObject.SetActive(true);
        unit_interface_ground.gameObject.SetActive(true);
        unit_interface.gameObject.SetActive(true);
        unit_interface_middle.gameObject.SetActive(true);
        ToolShowAllUIunit();

        unit_blueprint.menu_open = true;
    }

    void ToolCloseAllUIunit()
    {
        foreach(Transform u in UIunit_group)
        {
            u.gameObject.SetActive(false);
        }
    }
    void ToolShowAllUIunit()
    {
        foreach (Transform u in UIunit_group)
        {
            u.gameObject.SetActive(true);
        }
    }

    #endregion
    public void OnPointerEnter()
    {
        if (!pressed) self_image.sprite = sp[1];
    }
    public void OnPointerExit()
    {
        if (!pressed) self_image.sprite = sp[0];
    }
}
