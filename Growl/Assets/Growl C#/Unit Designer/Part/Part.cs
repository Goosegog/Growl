
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Part : MonoBehaviour
{
    public string self_prefab_name;
    public int ID;//ID是零件在整个游戏数值策划层面的ID
    public int orderID; //【S】orderID 用以区分同名零件
    public string self_name;
    public int cost;//造价


    [HideInEditorMode]
    public bool be_picked = false;
    [HideInEditorMode]
    public bool hover = false;
    [HideInEditorMode]
    public bool be_creat_from_button = false;//表示这个零件是否是由按钮创建的
    [HideInEditorMode]
    public bool deserialize_over = false;//表示这个零件是否反序列化并且初始化完成
    [HideInEditorMode]
    public int energy_supply_count = 0;//表示与这个零件直接相连的零件是 energy_supply = true 的个数
    [HideInEditorMode]
    public bool becheckedES = false;//表示与这个零件在当前帧已经被检测过动力连接性了
    [HideInEditorMode]
    public bool energy_supply = false;//表示这个零件是否直接或间接的与动力组件相连
    [HideInEditorMode]
    public bool mandatory_delete = false;//自毁开关：自毁开关一旦开启，会在下一帧的时候执行本零件的 DeleteSelf() 方法，或者也可以在开启开关后后立即手动执行
    [HideInEditorMode]
    public PartType part_type;
    #region 面板属性
    public int usepower_value;
    public int weight_value;
    #endregion

    #region 自身属性

    [HideInEditorMode]
    public List<MountPoint> mount_points;
    [HideInEditorMode]
    public bool legal = false;//默认这个零件是不合法的，然后每帧都要检测任何非法行为
    
    [HideInEditorMode]
    public Part parent_part;//【S】父级零件是谁，对于动力组件来说这个值是null；
    [HideInEditorMode]
    public List<Part> child_part;//【S】子级零件都有谁
    [HideInEditorMode]
    public List<Part> overlaped_parts;//这个零件现在正与哪些零件相重叠，由 OnTrigger 函数实时更新
    [HideInEditorMode]
    public Animator animator;

    #endregion
    #region 重要外部引用

    EventSystem eventsystem;
    GraphicRaycaster canvas_base;

    protected Transform rotation_point;
    protected UnitBlueprint unit_blueprint;
    [HideInEditorMode]
    public Transform virtual_plane;
    [HideInEditorMode]
    public Vector3 virtual_plane_normal;
    [HideInEditorMode]
    public Transform small_icon_canvas;
    [HideInEditorMode]
    public AudioSource link_audio;
    #endregion
    #region UI相关
    [HideInEditorMode]
    public Camera UIcamera;
    [HideInEditorMode]
    public RectTransform canvas;
    [HideInEditorMode]
    public List<Image> all_UI;

    [HideInEditorMode]
    public string overstep_UI_name;
    [HideInEditorMode]
    public Image overstep_UI;
    [HideInEditorMode]
    public Text overstep_text;
    [HideInEditorMode]
    public string overlap_UI_name;
    [HideInEditorMode]
    public Image overlap_UI;
    [HideInEditorMode]
    public Text overlap_text;
    [HideInEditorMode]
    public string inair_UI_name;
    [HideInEditorMode]
    public Image inair_UI;
    [HideInEditorMode]
    public Text inair_text;
    [HideInEditorMode]
    public string energy_supply_UI_name;
    [HideInEditorMode]
    public Image energy_supply_UI;
    [HideInEditorMode]
    public Text energy_supply_text;

    [HideInEditorMode]
    public Vector3 UI_pos_in_worldspace;

    #endregion
    #region 序列化反序列化相关
    PartSerializeStruct PSS;
    #endregion
    public PartSerializeStruct SerializePSS()
    {
        PSS = new PartSerializeStruct();
        PSS.part_prefab_name = self_prefab_name;
        PSS.part_type_int = (int)part_type;
        PSS.pos = new float[3] { transform.position.x, transform.position.y, transform.position.z };
        Vector3 temp_eulerAngles = transform.rotation.eulerAngles;
        PSS.rotation = new float[3] { temp_eulerAngles.x, temp_eulerAngles.y, temp_eulerAngles.z };
        PSS.orderID = orderID;
        if (parent_part == null)
        {
            PSS.parent_part = new int[2] { -999, -999 };
        }
        else
        {
            PSS.parent_part = new int[2] { parent_part.ID, parent_part.orderID };
        }      
        PSS.child_part = new List<int[]>();
        foreach(var p in child_part)
        {
            PSS.child_part.Add(new int[2] { p.ID, p.orderID });
        }

        PSS.MP_struct_dict = new Dictionary<int, MountPointSerializeStruct>();
        foreach(var mp in mount_points)
        {
            PSS.MP_struct_dict.Add(mp.ID, mp.SerializeMPSS());
        }

        return PSS;
    }


    void Start ()
    {
        
    }	
    public virtual void Init()
    {
        animator = GetComponent<Animator>();
        canvas_base = GameObject.Find("UICanvasBase").GetComponent<GraphicRaycaster>();
        small_icon_canvas = GameObject.Find("SmallIconCanvas").transform;
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        link_audio = unit_blueprint.GetComponent<AudioSource>();
        #region UI初始化
        overstep_UI_name = "Warning";
        overlap_UI_name = "Overlap";
        inair_UI_name = "Inair";
        energy_supply_UI_name = "NoPower";

        energy_supply_UI = Instantiate(Resources.Load<RectTransform>("UI/" + energy_supply_UI_name).GetComponent<Image>(), small_icon_canvas);
        energy_supply_text = energy_supply_UI.GetComponentInChildren<Text>();
        all_UI.Add(energy_supply_UI);

        energy_supply_UI.enabled = false;
        energy_supply_text.enabled = false;
        #endregion

    }


    protected virtual void Update ()
    {
        //Debug.LogError(name + "  " + energy_supply);
        //Debug.LogError(name + "  " + legal);
    }

    #region 工具性函数
   

    public virtual void Tool_CloaseAllUI()
    {
        if (overstep_UI.enabled) overstep_UI.enabled = false;
        if (overstep_text.enabled) overstep_text.enabled = false;
        if (overlap_UI.enabled) overlap_UI.enabled = false;
        if (overlap_text.enabled) overlap_text.enabled = false;
        if (inair_UI.enabled) inair_UI.enabled = false;
        if (inair_text.enabled) inair_text.enabled = false;
        if (energy_supply_UI.enabled) energy_supply_UI.enabled = false;
        if (energy_supply_text.enabled) energy_supply_text.enabled = false;
    }
    protected bool CheckGuiRaycastObjects()
    {
        PointerEventData eventData = new PointerEventData(eventsystem);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        canvas_base.Raycast(eventData, list);
        return list.Count > 0;
    }
    #endregion

    public virtual void DeleteSelf()
    {
        if (mandatory_delete)
        {

        }
        else if (unit_blueprint.menu_open)
        {
            return;
        }
        

        if ((Input.GetKeyDown(KeyCode.Delete) && (be_picked || hover)) || mandatory_delete)
        {
            be_picked = false;
            hover = false;

            //清理蓝图的信息：
            unit_blueprint.mouse_picking_a_part = false;
            unit_blueprint.RemovePartFromUnit(this);
            //整理相关的零件的碰撞信息列表：
            foreach (Part OP in overlaped_parts)
            {
                //从所有与本零件重叠的零件的碰撞列表中移除本零件
                Debug.Log("*****" + OP.overlaped_parts.Contains(this));
                OP.overlaped_parts.Remove(this);
            }
            //重置与这个零件连接的所有节点信息、清空父级零件信息
            foreach (var MP in mount_points)
            {
                MP.UnLink2MountPoint();
            }
            //销毁自身的UI组件
            foreach (var UI in all_UI)
            {
                Destroy(UI.gameObject);
            }
            //Debug.Log("即将销毁自身");
            //销毁自身
            Destroy(gameObject);
        }
    }

}
