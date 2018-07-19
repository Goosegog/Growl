using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPart : Part
{

    [HideInEditorMode]
    public bool overlap = false;
    [HideInEditorMode]
    public bool inair = false;//零件是否没有与任何挂点连接而悬空出现在设计平台上
    [HideInEditorMode]
    public bool overstep = false;//零件位置是否超出了最大设计区域

    #region 面板属性
    public int eachusepower_value;//每次使用装备额外消耗的能量
    public AttackPattern atk_pattern;
    public DamageType damage_type;
    public int damage;
    public int attack_number;
    //public int weight_value;
    public int ammo_value;//弹夹容量
    public int magazine_value;//弹夹
    public int range_value;
    public int durability_value;
    public List<WeaponSpecialty> specialty = new List<WeaponSpecialty>();
    public int normalDEF_value;
    public int blastDEF_value;
    public int energyDEF_value;

    #endregion
    #region 重要外部引用

    #endregion
    #region EquipmentPart的独特字段
    public bool symmetry = true;//是对称就是两方向转换，不是对称就是四方向转换
    [ShowIf("symmetry")]
    public int self_side = 0;//-1是左，1是右
    [ShowIf("symmetry")]
    public string brother_part_name;
    public float movespeedX;
    public float movespeedY;
    public float movespeedZ;
    float auto_link_threshold_scale = 0.02f;//自动吸附挂点的阈值 = 屏幕跨度的像素值乘以这个值，如果两个挂点之间的虚拟平面投影点距离小于阈值就会被吸附
    #endregion
    #region Design Area
    float max_size_X;
    float max_size_minusX;
    float max_size_Y;
    float max_size_minusY;
    float max_size_Z;
    float max_size_minusZ;
    #endregion
    #region UI
    

    #endregion
    #region Material
    public Material normal_mat;
    public Material error_mat;
    public Material hover_mat;
    public Material be_picked_mat;
    List<Renderer> renders;
    #endregion

    #region 挂点相关
    List<Vector3> project_allMP;//自动吸附时，自身的所有节点的投影，顺序一致
    #endregion

    void Start ()
    {
		
	}
    public override void Init()
    {
        base.Init();
        #region 重要外部引用初始化
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        rotation_point = GameObject.Find("Rotation point").transform;
        virtual_plane = GameObject.Find("Virtual Plane").transform;
        virtual_plane_normal = virtual_plane.transform.up;
        #endregion
        #region 自身属性初始化

        parent_part = null;
        child_part = new List<Part>();
        project_allMP = new List<Vector3>();
        overlaped_parts = new List<Part>();
        mount_points = new List<MountPoint>();
        part_type = PartType.Equipment;
        #region 挂点初始化
        MountPoint[] temps = GetComponentsInChildren<MountPoint>();
        foreach (var m in temps)
        {
            m.Init();//手动初始化，因为Start执行顺序有问题
            mount_points.Add(m);
        }

        #endregion
        #region 渲染组件初始化
        renders = new List<Renderer>();
        //找到子节点的所有渲染组件
        Renderer[] temps2 = GetComponentsInChildren<Renderer>();
        foreach (var r in temps2)
        {
            //但是只有tag是"Part Child"的才被添加进renders列表
            if (r.tag == "Part Child")
            {
                renders.Add(r);
            }
        }
        #endregion
        #endregion
        #region 设计区域最大范围初始化
        max_size_X = unit_blueprint.max_size_X;
        max_size_minusX = unit_blueprint.max_size_minusX;
        max_size_Y = unit_blueprint.max_size_Y;
        max_size_minusY = unit_blueprint.max_size_minusY;
        max_size_Z = unit_blueprint.max_size_Z;
        max_size_minusZ = unit_blueprint.max_size_minusZ;
        #endregion     
        #region UI初始化
        UIcamera = GameObject.Find("UICamera").GetComponent<Camera>();
        canvas = GameObject.Find("UICanvasBase").transform as RectTransform;
        all_UI = new List<Image>();

        overstep_UI = Instantiate(Resources.Load<RectTransform>("UI/" + overstep_UI_name).GetComponent<Image>(), small_icon_canvas);
        overstep_text = overstep_UI.GetComponentInChildren<Text>();
        all_UI.Add(overstep_UI);
        //overstep_UI.name = overstep_UI.name + "_" + name;
        overlap_UI = Instantiate(Resources.Load<RectTransform>("UI/" + overlap_UI_name).GetComponent<Image>(), small_icon_canvas);
        overlap_text = overlap_UI.GetComponentInChildren<Text>();
        all_UI.Add(overlap_UI);
        inair_UI = Instantiate(Resources.Load<RectTransform>("UI/" + inair_UI_name).GetComponent<Image>(), small_icon_canvas);
        inair_text = inair_UI.GetComponentInChildren<Text>();
        all_UI.Add(inair_UI);



        UI_pos_in_worldspace = transform.TransformPoint(GetComponent<BoxCollider>().center);

        overstep_UI.enabled = false;
        overstep_text.enabled = false;
        overlap_UI.enabled = false;
        overlap_text.enabled = false;
        inair_UI.enabled = false;
        inair_text.enabled = false;
        #endregion


    }

    protected override void Update()
    {
        if (deserialize_over || be_creat_from_button)
        {
            base.Update();
            //if (!be_creat_from_button) return;
            CheckMouseHover();
            BePickedAndUnPicked();
            ChangeSelfOrientation();
            DeleteSelf();
            BePickedMovement();
        }
        //Debug.LogError(name + legal);
        //Debug.LogError(transform.rotation.w + "/" + transform.rotation.x + "/" + transform.rotation.y + "/" + transform.rotation.z);

    }
    private void FixedUpdate()
    {
        if (deserialize_over || be_creat_from_button)
        {
            //if (!be_creat_from_button) return;
            AutoLink2MountPoint();
            ChangeMaterial();
            UI();
            //CheckLega的实现需要基于物理系统所以放在这里
            CheckLegal();
        }
           
    }

    void CheckMouseHover()
    {
        if (unit_blueprint.menu_open) return;

        if (unit_blueprint.mouse_picking_a_part)
        {
            //Debug.Log("鼠标正拾取一个零件" + hover);
            hover = false;
            return;
        }
        RaycastHit hitwhat;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitwhat, 2000f, 1 << 12))
        {
            if (hitwhat.transform == transform)
            {
                //Debug.Log("悬停在这个零件");
                hover = true;
            }
            else if (hover)
            {
                hover = false;
            }
        }
        else if (hover)
        {
            hover = false;
        }

    }
    void BePickedAndUnPicked()
    {
        //if (CheckGuiRaycastObjects()) return;
        if (unit_blueprint.menu_open) return;

        if (Input.GetButtonDown("Fire1") && !be_picked && hover)
        {
            RaycastHit hitwhat;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitwhat, 2000f, 1 << 12))
            {
                if (hitwhat.transform == transform)
                {
                    Debug.Log("选中了");
                    be_picked = true;
                    unit_blueprint.mouse_picking_a_part = true;
                    //一旦一个零件被拾取，重置这个零件的所有节点信息、清空父级零件信息                   
                    foreach (var MP in mount_points)
                    {
                        MP.UnLink2MountPoint();
                    }
                    //////一旦一个零件被拾取，重置所有零件的 energy_supply 变量为false;
                    ////unit_blueprint.SetAllPartEnergySupplyFalse();

                    //Cursor.visible = false;
                }
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire1")) && be_picked)
        {
            Debug.Log("取消选中了");
            be_picked = false;
            unit_blueprint.mouse_picking_a_part = false;
        }
    }
    void BePickedMovement()
    {
        if (unit_blueprint.menu_open) return;

        if (!be_picked)
        {
            return;
        }

        //被拾取状态下根据鼠标拖移来移动

        RaycastHit hitwhat;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitwhat, 2000f, 1 << 10))
        {
            Vector3 real_pos = hitwhat.point;
            //Debug.Log(hitwhat.point + hitwhat.transform.name);
            //Debug.DrawLine(Camera.main.transform.position, real_pos, Color.red, 0.1f);
            transform.position = new Vector3
                (
                    Mathf.Clamp(real_pos.x, max_size_minusX, max_size_X),
                    Mathf.Clamp(real_pos.y, max_size_minusY, max_size_Y),
                    Mathf.Clamp(real_pos.z, max_size_minusZ, max_size_Z)
                );

            if ((transform.position - real_pos).sqrMagnitude > 0.005f)
            {
                //如果当前零件位置和命中点之间距离过大，说明零件因为超出范围被强制设置了位置，需要更改材质
                overstep = true;
            }
            else
            {
                overstep = false;
            }
        }
        else
        {
            Debug.LogError("虚拟平面丢失！！");
        }

    }
    void ChangeSelfOrientation()
    {
        if (unit_blueprint.menu_open) return;
        if (Input.GetKeyDown(KeyCode.C) && be_picked && !symmetry)
        {
            //transform.Rotate(Vector3.forward, 90);
            
            transform.localRotation *= Quaternion.Euler(0, 0, 90);
            return;
        }
        if (Input.GetKeyDown(KeyCode.C) && be_picked && symmetry)
        {
            EquipmentPart temp_E_part = (Instantiate(Resources.Load<GameObject>("Part/EquipmentPart/" + brother_part_name))).GetComponent<EquipmentPart>();
            temp_E_part.enabled = true;
            temp_E_part.Init();//手动初始化，因为Start执行顺序有问题
            temp_E_part.CreatFromButton();
            temp_E_part.be_creat_from_button = true;

            mandatory_delete = true;
            DeleteSelf();
        }
    }
   
    public void CreatFromButton()
    {
        //通过点击按钮创建一个本零件的实例时，会由按钮脚本呼叫本函数

        be_picked = true;
        unit_blueprint.mouse_picking_a_part = true;
        //不管零件是以什么形式入场的，都要添加进蓝图
        unit_blueprint.AddPart2Unit(this);
    }

    void AutoLink2MountPoint()
    {
        //动力组件会自动吸附能被看到的任何节点，而不是只吸附合法零件上的节点

        //对于动力组件来说父节点永远为空，如果这个组件没有被拾取，不需要开启自动吸附
        if (!be_picked) return;

        //每帧清空投影坐标列表：
        project_allMP.Clear();
        //更新吸附的距离阈值
        float auto_link_threshold = Screen.width * auto_link_threshold_scale;
        Debug.Log("auto_link_threshold = " + auto_link_threshold);

        for (int i = 0; i < mount_points.Count; i++)
        {
            MountPoint MP_now = mount_points[i];

            if (MP_now.link_info[0] != -999)
            {
                Debug.LogError("严重错误：一个组件正在运行自动吸附，但是它的这个节点的信息没有被清空！" + MP_now.name);
            }
            else
            {
                Vector3 project_thisMP = Camera.main.WorldToScreenPoint(MP_now.transform.position);
                project_thisMP.z = 0;
                project_allMP.Add(project_thisMP);
            }
        }

        //节点的投影到另一个节点的投影的距离
        float dis_self2mp = -1000f;
        //把所有位于设计平台上的零件的所有节点（不论是否合法）投影到虚拟平面上      
        foreach (var GB in unit_blueprint.all_parts)
        {
            Part part = GB.GetComponent<Part>();

            if (part.transform == transform)
            {
                //如果找到的零件是自身，就跳过
                continue;
            }
            //如果找到的零件不是自身，就开始投影--判断吸附的逻辑
            foreach (var mp in part.mount_points)
            {
                //如果不是一个空节点，就跳过这个节点接着分析下一个节点
                if (mp.link_info[0] != -999) continue;

                Vector3 this_mp_screen_pos = Camera.main.WorldToScreenPoint(mp.transform.position);
                #region 看不见的节点应该被屏蔽掉，还没写
                //Ray ray = Camera.main.ScreenPointToRay(this_mp_screen_pos);
                //if (Physics.Raycast(ray,))
                #endregion
                this_mp_screen_pos.z = 0;

                //对于自身每一个节点，都需要分别执行一次距离计算：
                for (int i = 0; i < project_allMP.Count; i++)
                {
                    MountPoint self_MP_now = mount_points[i];
                    Vector3 project_thisMP = project_allMP[i];

                    float dis_self2mp_now = Vector3.Distance(project_thisMP, this_mp_screen_pos);

                    if (dis_self2mp < 0)
                    {
                        dis_self2mp = dis_self2mp_now;
                    }

                    if (dis_self2mp_now <= auto_link_threshold)
                    {
                        //判断法线方向是否相符：
                        if (mp.transform.forward == -1 * self_MP_now.transform.forward)
                        {
                            //法线方向吻合的话直接把两个零件互挂
                            bool b = self_MP_now.Link2MountPoint(mp);
                            //正确的挂完了之后自动解除被拾取状态  
                            if (b)
                            {
                                be_picked = false;
                                unit_blueprint.mouse_picking_a_part = false;
                            
                                return;
                            }

                        }
                    }

                }
            }
        }
    }

    void CheckLegal()
    {
        //整个个方法的作用是每帧进行零件的合法性检测

        #region 连接性检测（是否没有和任何节点连接而成为一个悬空物体）
        CheckInAir();
        #endregion
        #region 能量供应检测
        //零件是否直接或间接的与动力组件相连
        //CheckEnergySupply();
        #endregion
        #region 接地与左右高度对称性检测
        //这个方法的作用是检测腿是否接地，如果这是唯一的腿，就以这条腿为基准将整个单位接地，如果这不是唯一的腿，则检测左右腿长是否相等
        #endregion
        #region 零件重叠检测 
        //零件重叠检测由 OnTrigger 系列函数完成碰撞列表的更新，由 CheckOverlap() 方法具体实现判断逻辑
        CheckOverlap();
        #endregion
        #region 超出设计区域检测
        //超出设计区域检测由 BePickedMovement() 完成
        CheckOverStep();
        #endregion
        //只有所有不合法行为都被排除，才能设置为合法
        if (!overlap && !overstep && !inair && energy_supply)
        {
            bool legal_now = true;
            if (legal != legal_now)
            {
                //如果合法性状态有所改变，可以在这里处理一些事件
                Debug.LogError("零件上一帧的合法性为" + legal + "零件这一帧的合法性为" + legal_now);
                legal = legal_now;

            }

            legal = legal_now;
        }
        else
        {
            bool legal_now = false;
            if (legal != legal_now)
            {
                //如果合法性状态有所改变，可以在这里处理一些事件
                Debug.LogError("零件上一帧的合法性为" + legal + "零件这一帧的合法性为" + legal_now);
                legal = legal_now;

            }
        }

    }
    void CheckInAir()
    {
        foreach (var mp in mount_points)
        {
            if (mp.link_info[0] != -999)
            {
                //只要有一个连接，就能证明不是 inair
                inair = false;
                return;
            }
        }
        //全都是-999，说明是 inair
        inair = true;
    }
    void CheckOverStep()
    {
        if (transform.position.x <= max_size_minusX || transform.position.x >= max_size_X)
        {
            overstep = true;
        }
        else if (transform.position.y <= max_size_minusY || transform.position.y >= max_size_Y)
        {
            overstep = true;
        }
        else if (transform.position.z <= max_size_minusZ || transform.position.z >= max_size_Z)
        {
            overstep = true;
        }
        else
        {
            overstep = false;
        }
    }
    //void CheckEnergySupply()
    //{
    //    int energy_supply_count_thisframe = 0;
    //    foreach (var mp in mount_points)
    //    {
    //        if (mp.link_part != null)
    //        {
    //            if (mp.link_part.energy_supply)
    //            {
    //                energy_supply_count_thisframe++;
    //            }
    //        }
    //    }

    //    if (energy_supply_count_thisframe == 0)
    //    {
    //        energy_supply_count = 0;
    //        energy_supply = false;
    //    }

    //    if (energy_supply_count_thisframe < energy_supply_count)
    //    {
    //        energy_supply_count = energy_supply_count_thisframe;
    //        energy_supply = false;
    //    }
    //    else
    //    {
    //        energy_supply_count = energy_supply_count_thisframe;
    //        energy_supply = true;
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        //通过这个Stay函数，可以保证发生碰撞或持续停留碰撞时【双方都会】在每一次 FixUpdata 运行一次
        //Debug.Log("OnTriggerStay" + other.transform.position + " 碰撞列表的大小" + overlaped_parts.Count);
        if (other.transform != transform)
        {
            if (other.gameObject.layer == 11)
            {
                //如果只是与节点交叉是合法的
                //Debug.Log("合法碰撞" + other.name + other.transform.tag);

            }
            else if (other.gameObject.layer == 12)
            {
                //如果只是与零件交叉是不合法的
                //Debug.Log("非法碰撞" + other.name + other.transform.tag);                
                if (!overlaped_parts.Contains(other.GetComponent<Part>()))
                {
                    //如果碰撞列表里没有这个物体，就添加进去
                    overlaped_parts.Add(other.GetComponent<Part>());
                }

            }
        }
        else
        {
            Debug.Log("和未定义的物体发生碰撞，撞到了==" + other.name + other.transform.tag);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit运行了一次" + other.transform.position);
        if (other.gameObject.layer == 11)
        {
            //节点从节点中退出碰撞，对零件碰撞的合法性是没有影响的
            return;
        }
        else if (other.gameObject.layer == 12)
        {
            if (!overlaped_parts.Contains(other.GetComponent<Part>()))
            {
                //如果退出了碰撞，碰撞列表里却没有这个物体，说明发生了错误
                Debug.LogError("严重错误：物体退出了碰撞但碰撞列表里却没有相应物体");

            }
            else
            {
                overlaped_parts.Remove(other.GetComponent<Part>());
            }
        }
        else
        {
            Debug.Log("从未定义的物体退出碰撞" + other.name + other.transform.tag);


        }

    }
    void CheckOverlap()
    {
        //OnTrigger系列函数只负责更新 overlaped_parts 列表，具体的判断由本方法完成

        if (overlaped_parts.Count > 0)
        {
            //只要碰撞列表里有任何零件，就代表发生了部件重叠
            overlap = true;
        }
        else
        {
            overlap = false;
        }

    }

    void ChangeMaterial()
    {
        if (hover)
        {
            foreach (var r in renders)
            {
                if (r.material != hover_mat) { r.material = hover_mat; }
            }
        }
        else if (overlap)
        {
            foreach (var r in renders)
            {
                if (r.material != error_mat) r.material = error_mat;
            }
        }
        else if (overstep)
        {
            foreach (var r in renders)
            {
                if (r.material != error_mat) r.material = error_mat;
            }
        }
        else if (be_picked)
        {
            foreach (var r in renders)
            {
                if (r.material != be_picked_mat) { r.material = be_picked_mat; }
            }
        }
        else if (inair)
        {
            foreach (var r in renders)
            {
                if (r.material != error_mat) r.material = error_mat;
            }
        }
        else if (!energy_supply)
        {
            foreach (var r in renders)
            {
                if (r.material != error_mat) r.material = error_mat;
            }
        }
        else
        {
            foreach (var r in renders)
            {
                if (r.material != normal_mat) r.material = normal_mat;
            }
        }

    }
    void UI()
    {
        Tool_CloaseAllUI();

        if (overstep)
        {
            overstep_UI.enabled = true;
            overstep_text.enabled = true;
            UI_pos_in_worldspace = transform.TransformPoint(GetComponent<BoxCollider>().center);
            Vector2 UI_screen_pos = Camera.main.WorldToScreenPoint(UI_pos_in_worldspace);
            Vector2 pos_in_canvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, UI_screen_pos, UIcamera, out pos_in_canvas);
            (overstep_UI.transform as RectTransform).anchoredPosition = pos_in_canvas;

        }
        else if (overlap)
        {
            overlap_UI.enabled = true;
            overlap_text.enabled = true;

            UI_pos_in_worldspace = transform.TransformPoint(GetComponent<BoxCollider>().center);
            Vector2 UI_screen_pos = Camera.main.WorldToScreenPoint(UI_pos_in_worldspace);
            Vector2 pos_in_canvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, UI_screen_pos, UIcamera, out pos_in_canvas);
            (overlap_UI.transform as RectTransform).anchoredPosition = pos_in_canvas;
        }
        else if (inair)
        {
            //如果部件正被拾取或者悬停高亮，即使缺少连接也显示UI
            if (be_picked || hover) return;

            inair_UI.enabled = true;
            inair_text.enabled = true;

            UI_pos_in_worldspace = transform.TransformPoint(GetComponent<BoxCollider>().center);
            Vector2 UI_screen_pos = Camera.main.WorldToScreenPoint(UI_pos_in_worldspace);
            Vector2 pos_in_canvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, UI_screen_pos, UIcamera, out pos_in_canvas);
            (inair_UI.transform as RectTransform).anchoredPosition = pos_in_canvas;
        }
        else if (!energy_supply)
        {
            //如果部件正被拾取或者悬停高亮，即使缺少连接也不要显示 energy_supply UI
            if (be_picked || hover) return;

            energy_supply_UI.enabled = true;
            energy_supply_text.enabled = true;

            UI_pos_in_worldspace = transform.TransformPoint(GetComponent<BoxCollider>().center);
            Vector2 UI_screen_pos = Camera.main.WorldToScreenPoint(UI_pos_in_worldspace);
            Vector2 pos_in_canvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, UI_screen_pos, UIcamera, out pos_in_canvas);
            (energy_supply_UI.transform as RectTransform).anchoredPosition = pos_in_canvas;
        }


    }

    #region 工具性函数
    

    #endregion
}
