using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LegPart : Part
{
   
    [HideInEditorMode]
    public bool overlap = false;
    [HideInEditorMode]
    public bool inair = false;//零件是否没有与任何挂点连接而悬空出现在设计平台上
    [HideInEditorMode]
    public bool overstep = false;//零件位置是否超出了最大设计区域


    #region 面板属性

    public int load_value;
    public int move_value;
    public int dodge_value;
    public int durability_value;
    public List<MovementPartSpecialty> specialty = new List<MovementPartSpecialty>();
    public int normalDEF_value;
    public int blastDEF_value;
    public int energyDEF_value;
    #endregion
    #region 重要外部引用

    #endregion
    #region LegPart的独特字段   
    public bool symmetry = true;
    [ShowIf("symmetry")]
    public int self_side = 0;//-1是左，1是右
    [ShowIf("symmetry")]
    public string brother_part_name;
    public float movespeedX;
    public float movespeedY;
    public float movespeedZ;
    float auto_link_threshold_scale = 0.02f;//自动吸附挂点的阈值 = 屏幕跨度的像素值乘以这个值，如果两个挂点之间的虚拟平面投影点距离小于阈值就会被吸附
    float anim_interval_time = 1f;//一个零件合法后间隔多长时间开始播放动画
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
    
    MountPoint self_powerinput_MP;//自身的动力输入节点
    #endregion

    void Start()
    {    
        //Cursor.visible = false;         
    }
    public override void Init()
    {
        base.Init();
        #region 重要外部引用初始化
        
        rotation_point = GameObject.Find("Rotation point").transform;
        virtual_plane = GameObject.Find("Virtual Plane").transform;
        virtual_plane_normal = virtual_plane.transform.up;
        #endregion
        #region 自身属性初始化

        parent_part = null;
        child_part = new List<Part>();
        overlaped_parts = new List<Part>();    
        mount_points = new List<MountPoint>();
        part_type = PartType.LegPart;
        
            #region 挂点初始化
        MountPoint[] temps = GetComponentsInChildren<MountPoint>();
        foreach (var m in temps)
        {
            m.Init();//手动初始化，因为Start执行顺序有问题
            mount_points.Add(m);
        }
        foreach (var self_MP in mount_points)
        {
            if (self_MP.MP_type == MountPointType.PowerInput)
            {
                self_powerinput_MP = self_MP;
            }
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
        if(deserialize_over || be_creat_from_button)
        {
            base.Update();           
            CheckMouseHover();
            BePickedAndUnPicked();
            DeleteSelf();
            BePickedMovement();
            ChangeSelfOrientation();
        }
        
        
    }
    private void FixedUpdate()
    {
        if (deserialize_over || be_creat_from_button)
        {
            AutoLink2MountPoint();
            ChangeMaterial();
            UI();
            //CheckLega的实现需要基于物理系统所以放在这里
            CheckLegal();
            //Debug.LogError(name + legal);
            //动画状态的改变基于 CheckLegal() 所以放在这里
            Anim();
        }
           
       
    }
    private void LateUpdate()
    {
        
    }
  
    void CheckMouseHover()
    {
        if (unit_blueprint.mouse_picking_a_part)
        {
            //Debug.Log("鼠标正拾取一个零件" + hover);
            hover = false;
            return;
        }

        if (unit_blueprint.menu_open) return;

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
        if (unit_blueprint.menu_open) return;

        if (Input.GetButtonDown("Fire1") && !be_picked && hover)
        {
            RaycastHit hitwhat;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitwhat,2000f, 1<<12))
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
        else if (   (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire1"))    && be_picked)
        {
            Debug.Log("取消选中了");
            be_picked = false;
            unit_blueprint.mouse_picking_a_part = false;
        }
    }
    public void BePickedMovement()
    {
        if (unit_blueprint.menu_open) return;

        if (!be_picked)
        {
            return;
        }

        //被拾取状态下根据鼠标拖移来移动

        RaycastHit hitwhat;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitwhat, 2000f, 1<<10))
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

            if((transform.position - real_pos).sqrMagnitude > 0.005f)
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
    //public void DeleteSelf()
    //{
    //    if (unit_blueprint.menu_open) return;

    //    if ((Input.GetKeyDown(KeyCode.Delete) && (be_picked || hover)) || mandatory_delete)
    //    {
    //        be_picked = false;
    //        hover = false;
    //        energy_supply = false;

    //        //清理蓝图的信息：
    //        unit_blueprint.mouse_picking_a_part = false;
    //        unit_blueprint.RemovePartFromUnit(this);
    //        //整理相关的零件的碰撞信息列表：
    //        foreach (Part OP in overlaped_parts)
    //        {
    //            //从所有与本零件重叠的零件的碰撞列表中移除本零件
    //            Debug.Log("*****" + OP.overlaped_parts.Contains(this));
    //            OP.overlaped_parts.Remove(this);
    //        }
    //        //重置与这个零件连接的所有节点信息、清空父级零件信息
    //        foreach (var MP in mount_points)
    //        {
    //            MP.UnLink2MountPoint();
    //        }
    //        //销毁自身的UI组件
    //        foreach (var UI in all_UI)
    //        {
    //            Destroy(UI.gameObject);
    //        }
    //        //销毁自身
    //        Destroy(gameObject);
    //    }    
    //}
    void ChangeSelfOrientation()
    {
        if (unit_blueprint.menu_open) return;
        if (Input.GetKeyDown(KeyCode.C) && be_picked && symmetry)
        {

            LegPart temp_leg_part = (Instantiate(Resources.Load<GameObject>("Part/MovementPart/LegPart/" + brother_part_name))).GetComponent<LegPart>();
            temp_leg_part.enabled = true;
            temp_leg_part.Init();//手动初始化，因为Start执行顺序有问题
            temp_leg_part.CreatFromButton();
            temp_leg_part.be_creat_from_button = true;

            mandatory_delete = true;
            DeleteSelf();
        }
    }

    public void CreatFromButton()
    {       
        //通过点击按钮创建一个本零件的实例时，会由按钮脚本呼叫本函数

        if (!unit_blueprint.have_power_part)
        {
            //如果蓝图里没有任何动力组件，就设置为被拾取状态自由移动
            be_picked = true;
            unit_blueprint.mouse_picking_a_part = true;
        }
        else
        {
            //如果有动力组件先逐个取出动力组件
            foreach (var t in unit_blueprint.all_power_parts)
            {
                //检测动力输出挂点是否被占满了
                foreach (var MP in t.GetComponent<PowerPart>().self_poweroutput_MP)
                {
                    if (MP.link_info[0] == -999)
                    {
                        //如果发现了一个空节点,看节点法线方向是否吻合                          
                        if (MP.normal == -1 * self_powerinput_MP.transform.forward)
                        {
                            Debug.Log("从按钮新实例化出来的腿零件找到了一个空的且法线吻合的动力输入节作为预制挂点");
                            //吻合的话直接把两个零件互挂
                            bool b = self_powerinput_MP.Link2MountPoint(MP);
                            //正确挂上了之后自动解除被拾取状态
                            if (b)
                            {
                                be_picked = false;
                                unit_blueprint.mouse_picking_a_part = false;
                                //UnitInfo.GetUnitInfo.CountNeedPower(usepower_value);
                            }                         
                            //最后不管零件是以什么形式入场的，都要添加进蓝图
                            unit_blueprint.AddPart2Unit(this);
                            return;
                        }
                        else
                        {
                            //不吻合的话接着找
                            continue;
                        }
                    }
                }
            }
            //如果上面的遍历跑完一遍后本零件没有被挂在任何节点上（link_info[0]还是-999），就设置为被拾取状态
            if (self_powerinput_MP.link_info[0] == -999)
            {
                be_picked = true;
                unit_blueprint.mouse_picking_a_part = true;
            }
        
        }
        //最后不管零件是以什么形式入场的，都要添加进蓝图
        unit_blueprint.AddPart2Unit(this);
    }

    void AutoLink2MountPoint()
    {
        //如果这个零件的父节点不为空或者没有被拾取，不需要自动吸附
        if (parent_part != null || !be_picked) return;

        //更新吸附的距离阈值
        float auto_link_threshold = Screen.width * auto_link_threshold_scale;
        Debug.Log("auto_link_threshold = " + auto_link_threshold);
        //更新虚拟平面的法线
        virtual_plane_normal = virtual_plane.transform.up;
        //腿上的动力输入节点的投影       
        Vector3 project_SPMP = Camera.main.WorldToScreenPoint(self_powerinput_MP.transform.position);
        project_SPMP.z = 0;
        //腿上的动力输入节点的投影到另一个节点的投影的距离
        float dis_self2mp = -1000f;
        //把所有零件上的所有节点投影到虚拟平面上      
        foreach (var part in unit_blueprint.all_parts)
        {
            if (part.transform == transform)
            {
                //如果找到的零件是自身，就跳过
                continue;
            }
            //如果找到的零件不是自身，就开始投影--判断吸附的逻辑
            foreach (var mp in part.mount_points)
            {

                ////////如果这个节点不是动力输出节点，不能自动吸附，跳过这个节点接着分析下一个节点
                //////if (mp.MP_type != MountPointType.PowerOutput) continue;

                //如果不是一个空节点，就跳过这个节点接着分析下一个节点
                if (mp.link_info[0] != -999) continue;
               
                Vector3 this_mp_screen_pos = Camera.main.WorldToScreenPoint(mp.transform.position);
                #region 看不见的节点应该被屏蔽掉，还没写
                //Ray ray = Camera.main.ScreenPointToRay(this_mp_screen_pos);
                //if (Physics.Raycast(ray,))
                #endregion
                this_mp_screen_pos.z = 0;
                //float dis_self2mp_now = Vector3.Distance(Camera.main.ScreenToWorldPoint(project_SPMP), Camera.main.ScreenToWorldPoint(this_mp_screen_pos));
                float dis_self2mp_now = Vector3.Distance(project_SPMP, this_mp_screen_pos);
                //Debug.Log("腿节点的投影坐标" + project_SPMP + "挂点的投影坐标" + project_TMP);                
                //Debug.Log("腿节点的投影坐标" + Camera.main.ScreenToWorldPoint(project_SPMP) + "挂点的投影坐标" + Camera.main.ScreenToWorldPoint(this_mp_screen_pos)); 
                Debug.Log(dis_self2mp_now + mp.name);
                if (dis_self2mp < 0)
                {
                    //如果这是第一次判断，就先把当前的距离值赋为距离，再判断
                    dis_self2mp = dis_self2mp_now;
                }

                if (dis_self2mp_now <= dis_self2mp)
                {
                    dis_self2mp = dis_self2mp_now;

                    if (dis_self2mp < auto_link_threshold)
                    {

                        //如果距离足够近                      
                        if (mp.transform.forward == -1 * self_powerinput_MP.transform.forward)
                        {
                            //法线方向吻合的话直接把两个零件互挂
                            self_powerinput_MP.Link2MountPoint(mp);

                            //挂完了之后自动解除被拾取状态                                                  
                            be_picked = false;
                            unit_blueprint.mouse_picking_a_part = false;

                            if (transform.position.x <= max_size_minusX || transform.position.x >= max_size_X)
                            {
                                overlap = true;
                            }
                            else if (transform.position.y <= max_size_minusY || transform.position.y >= max_size_Y)
                            {
                                overlap = true;
                            }
                            else if (transform.position.z <= max_size_minusZ || transform.position.z >= max_size_Z)
                            {
                                overlap = true;
                            }

                            return;
                        }
                    }
                }
                Debug.Log("***" + dis_self2mp + mp.name);
            }
        }
    }
    void AutoAdjustPosition()
    {
        //这个方法用于当腿部组件和动力组件连接后自动找平到平台平面
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hitInfo, 2000f, 1 << 9))
        {
            if(hitInfo.transform.tag == "DesignPlatformform")
            {
                Vector3 offest = hitInfo.point - transform.position;
                transform.position += offest;
            }
        }
    }

    void CtrlCursor()
    {
        Cursor.visible = false;
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
            if(legal != legal_now)
            {
                //如果合法性状态由不合法变为了合法，可以在这里处理一些事件
                Debug.LogError("零件上一帧的合法性为" + legal + "零件这一帧的合法性为" + legal_now);
                legal = legal_now;

                if (usepower_value > 0)
                {
                    //UnitInfo.GetUnitInfo.CountNeedPower(usepower_value);
                }
                
            }

            legal = legal_now;
        }
        else
        {
            bool legal_now = false;
            if (legal != legal_now)
            {
                //如果合法性状态由合法变为了不合法，可以在这里处理一些事件
                Debug.LogError("零件上一帧的合法性为" + legal + "零件这一帧的合法性为" + legal_now);
                legal = legal_now;

                if (usepower_value > 0)
                {
                    //UnitInfo.GetUnitInfo.CountNeedPower(-1 * usepower_value);
                }
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
    //        energy_supply_count = energy_supply_count_thisframe;
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (other.transform != transform)
    //    //{
    //    //    if (other.gameObject.layer == 11 || other.gameObject.layer == 9)
    //    //    {
    //    //        //Debug.Log("合法碰撞" + other.name + other.transform.tag);
    //    //        //如果只是与节点交叉或是与环境碰撞是合法的
    //    //        overlap = false;

    //    //    }
    //    //    else
    //    //    {
    //    //        //Debug.Log("非法碰撞" + other.name + other.transform.tag);
    //    //        overlap = true;
    //    //    }


    //    //}
    //    //else
    //    //{
    //    //    //Debug.Log("else碰撞" + other.name + other.transform.tag);
    //    //    overlap = false;
    //    //}

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
            else if(other.gameObject.layer == 12)
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
                if(r.material != error_mat) r.material = error_mat;
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
                if (r.material != be_picked_mat) {r.material = be_picked_mat;}            
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
            Vector2 UI_screen_pos  = Camera.main.WorldToScreenPoint(UI_pos_in_worldspace);          
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
            //如果部件正被拾取或者悬停高亮，即使缺少连接也不要显示 inair UI
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
            //如果部件正被拾取或者悬停高亮，即使缺少供能也不要显示 energy_supply UI
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

    void Anim()
    {
        if (unit_blueprint.mouse_picking_a_part || !legal || !UnitInfo.GetUnitInfo.power_legal)
        {
            animator.SetBool("idle", false);
            return;
        }
        else
        {
            animator.SetBool("idle", true);
        }
        
    }
   

    #region 工具性函数
    

    #endregion




}
