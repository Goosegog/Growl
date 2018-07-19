using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/* 
 * 单位蓝图用于储存所有用户的设计信息，挂在设计平台的子节点Unit上，所有零件一旦进入平台范围并合法安置后都会包含在在Unit下
 * 这是一个被动的类，不会自动检测Unit下右多少东西，都是靠其他类调用AddPart2Unit和RemovePartFromUnit来更新设计列表中的物体
 * 
 * 
*/
public class UnitBlueprint : MonoBehaviour
{
    #region 功能性全局变量
    [HideInInspector]
    public bool mouse_picking_a_part = false;
    [HideInInspector]
    public bool menu_open = false;
    [HideInInspector]
    public bool clean_all_part_complete = false;//当执行 CleanAllPart() 方法后会开启一个协程来监控 all_parts 列表的元素数量，当元素为0时说明清除完毕，这个开关被置为true
    #endregion

    public bool have_power_part = false;
    public bool have_R_movement_part = false;
    public bool have_L_movement_part = false;
    public bool have_ctrl_part = false;

    ButtonController button_controller;

    //[HideInInspector]
    public List<Part> all_parts;//当前设计平台上的全部零件gameobject列表(不论是否合法)，在设计过程中直接用这个列表储存具体的指向明确的零部件
    [HideInInspector]
    public List<Part> all_legal_parts;//当前全部合法零件 part 脚本列表
    List<Part> remove_those_from_all_legal_parts;//当检测到一个零件因为合法性改变需要从多个列表中移除时，先用这个列表缓存想要移除的零件以避免在遍历过程中对列表进行删改
    [HideInInspector]
    public List<Part> all_power_parts;//当前全部合法动力零件脚本列表
    [HideInInspector]
    public List<Part> all_leg_parts;//当前全部合法 leg 零件脚本列表
    [HideInInspector]
    public List<Part> all_ctrl_parts;//当前全部合法 控制 零件脚本列表
    [HideInInspector]
    public List<Part> all_equip_parts;//当前全部合法 装备 零件脚本列表

    GameObject clean_parts_warning;


    List<Part> all_energySupply_parts;
    List<int> part_ID;//零件ID列表，当设计完成时填充这个列表便于将数据输出保存

    #region 最大设计区域
    public float max_size_X;
    public float max_size_minusX;
    public float max_size_Y;
    public float max_size_minusY;
    public float max_size_Z;
    public float max_size_minusZ;
    #endregion


    private void Awake()
    {
        Cursor.visible = true;
    }
    void Start()
    {
        button_controller = GameObject.Find("ButtonController").GetComponent<ButtonController>();
        part_ID = new List<int>();
        all_parts = new List<Part>();
        all_legal_parts = new List<Part>();
        remove_those_from_all_legal_parts = new List<Part>();
        all_power_parts = new List<Part>();
        all_leg_parts = new List<Part>();
        all_ctrl_parts = new List<Part>();
        all_equip_parts = new List<Part>();
        all_energySupply_parts = new List<Part>();
        clean_parts_warning = GameObject.Find("UICanvasTop/CleanPartsWarning");

        clean_parts_warning.SetActive(false);

    }

    void Update()
    {
        //Debug.LogError("腿部合法零件的个数" + all_leg_parts.Count);
        //Debug.Log("动力组件合法零件的个数" + all_power_parts.Count);
        CheckAllPartEnergySupply();
    }
    private void FixedUpdate()
    {
        CheckLegal();
        //Debug.Log("合法零件列表中有多少个零件？" + all_legal_parts.Count);
        //if (all_legal_parts.Count > 0) Debug.Log("第一个合法零件的合法性为" + all_legal_parts[0].name + all_legal_parts[0].legal);
        UpdataUnitInfo();
        //Debug.Log("have_ctrl_part = " + have_ctrl_part);
       
         

        
        
    }

    public void AddPart2Unit(Part add_this_part)
    {
        if (all_parts.Contains(add_this_part))
        {
            Debug.LogError("严重错误：重复添加同一个零件到蓝图中！");
        }

        //挂到Unit节点下
        add_this_part.transform.parent = transform;
        //挂到父节点下
        //if (add_this_part.parent_part != null)
        //{
        //    add_this_part.transform.SetParent(add_this_part.parent_part.transform);
        //}
        //加到当前全部零件列表里

        all_parts.Add(add_this_part);

        //调用分配函数将传入的零件正确分配到各个列表中：
        AllotList(add_this_part.GetComponent<Part>());
        //进行合法性检测
        CheckLegal();

    }
    public void RemovePartFromUnit(Part remove_this_part)
    {
        //从当前全部零件列表里移除
        if (all_parts.Contains(remove_this_part))
        {
            all_parts.Remove(remove_this_part);
            //调用列表移除函数将传入的零件正确从分配到各个列表中移除
            RemoveFromList(remove_this_part.GetComponent<Part>());
           
        }
        else
        {
            Debug.LogError("严重错误 ：all_parts 列表中没有要删除的 gameobject.name = " + remove_this_part.name);
        }        
        
    }
    void AllotList(Part p)
    {
        //这个函数不管零件当前是否合法，直接加到相应的合法的零件列表中，然后再通过每帧的合法性检测移除那些不合法的零件；
        if (all_legal_parts.Contains(p))
        {
            Debug.LogError("严重错误 ：AllotList 列表中已经存在这个 Part 的实例");
        }
        else
        {
            all_legal_parts.Add(p);
            Debug.LogError("all_legal_parts = " + all_legal_parts.Count);
        }
        
        if (p.part_type == PartType.PowerPart)
        {
            //have_power_part = true;
            //button_controller.BanPowerPartButton();
            //all_power_parts.Add(p.GetComponent<PowerPart>());
            Debug.LogError("111111111111111111111111111all_power_parts = " + all_power_parts.Count);
        }
        else if (p.part_type == PartType.LegPart)
        {
            LegPart temp = p.GetComponent<LegPart>();
            if (temp.legal)
            {
                //如果是合法的
            }

            switch (temp.self_side)
            {
                case -1:
                    have_L_movement_part = true;
                    break;
                case 1:
                    have_R_movement_part = true;
                    break;
            }

            all_leg_parts.Add(temp);

            //执行移动组件检测重新确定左右是否都有合法的移动组件
            CheckMovementSide();
        }
        else if(p.part_type == PartType.ControlPart)
        {
            ControlPart temp = p.GetComponent<ControlPart>();
            if (temp.legal)
            {
                //如果是合法的
            }
            have_ctrl_part = true;            
            all_ctrl_parts.Add(temp);
        }
        else if (p.part_type == PartType.Equipment)
        {
            EquipmentPart temp = p.GetComponent<EquipmentPart>();
            if (temp.legal)
            {
                //如果是合法的
            }
  
            all_equip_parts.Add(temp);
        }
    }
    void RemoveFromList(Part p)
    {
        //移除的话不用管零件当前是否合法，如果能从列表里找到就移除，找不到说明零件不合法根本就没在列表里
        //但是这里的判断条件用了两个，实际上只要一个为true另一个理论上也应该为true，这么写是为了验证代码逻辑有没有漏洞
        if (all_legal_parts.Contains(p) && p.legal)
        {
            all_legal_parts.Remove(p);
            Debug.Log("零件已从合法零件列表中删除");
        }
        
        
        if (p.part_type == PartType.PowerPart)
        {
            //如果移除的是动力组件，还要重新打开所有动力组件的按钮并且从动力零件列表中删除
            PowerPart temp = p.GetComponent<PowerPart>();
            have_power_part = false;
            button_controller.ReleasePowerPartButton();
            if (all_power_parts.Contains(temp) && temp.legal)
            {
                all_power_parts.Remove(temp);
                Debug.Log("动力组件已经移除！");
            }
            else
            {
                Debug.Log("动力组件不合法已经直接移除！");
            }
            
        }      
        else if (p.part_type == PartType.LegPart)
        {
            LegPart temp = p.GetComponent<LegPart>();
            switch (temp.self_side)
            {
                case -1:
                    have_L_movement_part = false;
                    break;
                case 1:
                    have_R_movement_part = false;
                    break;
            }

            if (all_leg_parts.Contains(temp) && temp.legal)
            {
                all_leg_parts.Remove(temp);
                Debug.Log("一个合法的腿部组件已经移除！");
               
            }
            else
            {
                Debug.Log("腿部组件不合法已经直接移除！");
            }

            //执行移动组件检测重新确定左右是否都有合法的移动组件
            CheckMovementSide();
        }
        else if(p.part_type == PartType.ControlPart)
        {
            ControlPart temp = p.GetComponent<ControlPart>();
            if (all_ctrl_parts.Contains(temp) && temp.legal)
            {
                all_ctrl_parts.Remove(temp);
                Debug.Log("一个合法的控制组件已经移除！");
                
                
            }
            else
            {
                Debug.Log("控制组件不合法已经直接移除！");
            }
        }
        else if(p.part_type == PartType.Equipment)
        {
            EquipmentPart temp = p.GetComponent<EquipmentPart>();
            if (all_equip_parts.Contains(temp) && temp.legal)
            {
                all_equip_parts.Remove(temp);
                Debug.Log("一个合法的控制组件已经移除！");
            }
            else
            {
                Debug.Log("控制组件不合法已经直接移除！");
            }
        }
    }

    public void CheckLegal()
    {
        //Debug.LogError("111111111111111111all_power_parts.Count =  " + all_power_parts.Count);
        //这个函数每帧检测零件的合法性变化情况及时更改各个列表中储存的零件

        //清空缓存列表：
        if (remove_those_from_all_legal_parts.Count != 0) remove_those_from_all_legal_parts.Clear();

        foreach (var p in all_parts)
        {
            if (p.legal)
            {
                //如果检测零件合法但是合法零件列表中没有这个零件,就添加
                if (!all_legal_parts.Contains(p))
                {
                    all_legal_parts.Add(p);

                    //然后再向分类的合法零件列表中添加
                    if (p.part_type == PartType.PowerPart && !all_power_parts.Contains(p))
                    {
                        Debug.LogError("这里添加" + all_power_parts.Count);
                        //如果是动力组件并且分类列表中没有这个组件才添加
                        PowerPart temp = p.GetComponent<PowerPart>();                        
                        button_controller.BanPowerPartButton();
                        all_power_parts.Add(temp);
                        have_power_part = true;
                        Debug.LogError("这里添加" + all_power_parts.Count);
                    }
                    else if (p.part_type == PartType.LegPart && !all_leg_parts.Contains(p))
                    {
                        LegPart temp = p.GetComponent<LegPart>();
                        all_leg_parts.Add(temp);
                        //执行移动组件检测重新确定左右是否都有合法的移动组件
                        CheckMovementSide();
                    }
                    else if (p.part_type == PartType.ControlPart && !all_ctrl_parts.Contains(p))
                    {
                        ControlPart temp = p.GetComponent<ControlPart>();
                        all_ctrl_parts.Add(temp);
                    }
                    else if (p.part_type == PartType.Equipment && !all_equip_parts.Contains(p))
                    {
                        EquipmentPart temp = p.GetComponent<EquipmentPart>();
                        all_equip_parts.Add(temp);
                    }

                }

            }
            else
            {
                //如果检测零件不合法，先添加到移除的缓存列表中
                //一个零件由合法转为不合法的时，此时 all_legal_parts 里存在这个零件，但 p.legal = false;
                if (all_legal_parts.Contains(p) && !p.legal)
                {
                    remove_those_from_all_legal_parts.Add(p);
                    //Debug.LogError("零件已经进入移除缓存区");

                    //再处理其他合法零件列表
                    if (p.part_type == PartType.PowerPart && all_power_parts.Contains(p))
                    {
                        PowerPart temp = p.GetComponent<PowerPart>();
                        button_controller.ReleasePowerPartButton();
                        all_power_parts.Remove(temp);
                        have_power_part = false;
                        //Debug.LogError(all_power_parts.Contains(p));
                        //Debug.LogError("这里" + all_power_parts.Count);
                    }
                    else if (p.part_type == PartType.LegPart && all_leg_parts.Contains(p))
                    {
                        LegPart temp = p.GetComponent<LegPart>();
                        all_leg_parts.Remove(temp);
                        //执行移动组件检测重新确定左右是否都有合法的移动组件
                        CheckMovementSide();
                    }
                    else if (p.part_type == PartType.ControlPart && all_ctrl_parts.Contains(p))
                    {
                        ControlPart temp = p.GetComponent<ControlPart>();
                        all_ctrl_parts.Remove(temp);
                    }
                    else if (p.part_type == PartType.Equipment && all_equip_parts.Contains(p))
                    {
                        EquipmentPart temp = p.GetComponent<EquipmentPart>();
                        all_equip_parts.Remove(temp);
                    }
                }
             
            }

        }
        //Debug.LogError("all_legal_parts = " + all_legal_parts.Count);
        foreach (var p in remove_those_from_all_legal_parts)
        {
            all_legal_parts.Remove(p);
            Debug.LogError(p.name + "零件已从合法零件列表中移除");
        }

       
    }

    void UpdataUnitInfo()
    {
        //本帧合法性检测完成后，再根据各类零件的子列表的种类更新单位信息
        //Debug.LogError(all_ctrl_parts.Count);

        if (all_ctrl_parts.Count > 0)
        {
            have_ctrl_part = true;
        }
        else
        {
            have_ctrl_part = false;
        }
    }
    void CheckMovementSide()
    {
        //这个函数用来检测平台上是否至少有一个左/右的腿部组件
        //由于这个函数需要get组件，尽量不要每帧调用，只在可能发生改变的时候调用
        have_L_movement_part = false;
        have_R_movement_part = false;

        if (all_leg_parts.Count > 0)
        {
            foreach (var lp in all_leg_parts)
            {
                LegPart llpp = lp.GetComponent<LegPart>();

                if (llpp.self_side == -1)
                {
                    have_L_movement_part = true;
                }
                else
                {
                    have_R_movement_part = true;
                }
            }
        }
        else
        {
            have_L_movement_part = false;
            have_R_movement_part = false;
        }
    }

    public void SetPartOrderID()
    {
        Dictionary<int, List<Part>> temp = new Dictionary<int, List<Part>>();

        foreach (var p in all_legal_parts)
        {
            if (temp.ContainsKey(p.ID))
            {
                temp[p.ID].Add(p);
            }
            else
            {
                temp.Add(p.ID, new List<Part>());
                temp[p.ID].Add(p);
            }
        }

        foreach(List<Part> LP in temp.Values)
        {
            int orderID = 0;
            foreach(Part p in LP)
            {
                p.orderID = orderID;
                orderID++;
            }
        }
    }
    #region 清理所有零件按钮
    public void ClickCleanPartButton()
    {
        menu_open = true;
        clean_parts_warning.SetActive(true);
    }
    public void ClickYesInCleanPartsWarning()
    {
        CleanAllPart();
        clean_parts_warning.SetActive(false);
        menu_open = false;
    }
    public void ClickNoInCleanPartsWarning()
    {
        clean_parts_warning.SetActive(false);
        menu_open = false;
    }

    #endregion
    #region 工具类方法
    public void CheckAllPartEnergySupply()
    {
        if (!have_power_part)
        {
            foreach (var p in all_parts)
            {
                if (p.part_type == PartType.PowerPart)
                {
                    p.energy_supply = true;
                }
                else if (p.energy_supply)
                {
                    p.energy_supply = false;
                }
            }
        }
        else
        {
            all_energySupply_parts.Clear();          
            foreach (MountPoint mp in all_power_parts[0].mount_points)
            {
                if (mp.link_part == null) continue;
                mp.self_root_part.energy_supply = true;
                mp.link_part.energy_supply = true;
                mp.link_part.becheckedES = true;
                if (!all_energySupply_parts.Contains(mp.link_part)) all_energySupply_parts.Add(mp.link_part);
                SetESTrue(mp.link_part, all_power_parts[0]);
            }

            foreach (var p in all_parts)
            {
                if(p.part_type == PartType.PowerPart)
                {
                    p.energy_supply = true;
                }
                else if (!all_energySupply_parts.Contains(p) && p.part_type != PartType.PowerPart)
                {
                    p.energy_supply = false;
                }
            }
        }
    }
    void SetESTrue(Part p, Part caller)
    {
        if (p.part_type == PartType.PowerPart)
        {
            p.energy_supply = true;
            return;
        }

        foreach (MountPoint mp in p.mount_points)
        {
            if (mp.link_part == null) continue;

            mp.link_part.energy_supply = true;
            if (!all_energySupply_parts.Contains(mp.link_part)) all_energySupply_parts.Add(mp.link_part);
            if (mp.link_part.mount_points.Count == 1)
            {
                continue;
            }
            else if(mp.link_part == caller)
            {
                continue;
            }
            else
            {
                SetESTrue(mp.link_part, p);
            }
        }
    }

    public void SetAllPartEnergySupplyFalse()
    {
        foreach(Part p in all_parts)
        {
            if(p.part_type != PartType.PowerPart) p.energy_supply = false;
        }
        
    }

    public void CleanAllPart()
    {       
        clean_all_part_complete = false;
        Debug.LogError("即将打开所有零件的自毁开关！");
        Debug.LogError("当前设计平台上一共有零件 all_parts.Count = " + all_parts.Count);
        foreach (Part p in all_parts)
        {
            p.mandatory_delete = true;
        }
        StartCoroutine(WaitForCleanComplete());        
    }
    IEnumerator WaitForCleanComplete()
    {
        while (all_parts.Count != 0)
        {
            Debug.LogError("没有清理完成，协程 WaitForCleanComplete() 还在等待");
            yield return new WaitForEndOfFrame();
        }
        clean_all_part_complete = true;
        clean_parts_warning.SetActive(false);
        menu_open = false;
    }
    #endregion
    #region 序列化与反序列化相关方法：
    public void SaveUnitToUserDataControllerUD()
    {
        //先为当前设计新建一个 UnitStruct：
        UnitStruct save_this = new UnitStruct();
        //给这个 UnitStruct 的字段赋值：
        save_this.unit_name = UnitInfo.GetUnitInfo.unit_name;
        save_this.creat_time = System.DateTime.Now.ToString();
        save_this.all_part_serialize_struct = new List<PartSerializeStruct>();
        save_this.all_modules_savedata = new List<ModuleSaveData>();
        //在给具体的 PSS 赋值之前先分配每个合法零件的 次序ID：
        SetPartOrderID();
        //序列化零件信息结构体并添加进列表：
        foreach (var p in all_legal_parts)
        {
            save_this.all_part_serialize_struct.Add(p.SerializePSS());
        }
        //序列化模块信息结构体并添加进列表：
        foreach (var m in UnitInfo.GetUnitInfo.modules_script)
        {
            if (m.installed)
            {
                save_this.all_modules_savedata.Add(m.SerializePSS());
            }
            
        }
        //创建并赋值完成后，将这个结构体添加进 UserDataController 的 UD 的 user_blueprints 列表中：
        Debug.LogError(UserDataController.GetSingleton);
        Debug.LogError(UserDataController.GetSingleton.UD);
        Debug.LogError(UserDataController.GetSingleton.UD.user_blueprints);
        UserDataController.GetSingleton.UD.user_blueprints.Add(save_this);
        Debug.LogError("当前设计已经添加进 UserData 的 user_blueprints 列表");
    }
    //public void LoadAndCreatUnitToDesignPlatform(string data_path)
    //{
    //    //给定一个文件路径，把文件读出来再反序列化为一个 UnitStruct 结构体：
    //    UnitStruct US = new UnitStruct();
    //    try
    //    {
    //        FileStream stream = new FileStream(@data_path, FileMode.Open);
    //        BinaryFormatter bFormat = new BinaryFormatter();            
    //        US = (UnitStruct)bFormat.Deserialize(stream);//反序列化得到的是一个object对象.必须做下类型转换
    //        stream.Close();
    //    }
    //    catch
    //    {

    //    }

    //    //反序列化完成后，先清理掉整个设计平台：
    //    CleanAllPart();
    //    //然后开启协程 CreatUnitToDesignPlatform 等待清除完成
    //    StartCoroutine(CreatUnitToDesignPlatform(US));       
    //}

    //IEnumerator CreatUnitToDesignPlatform(UnitStruct us)
    //{
    //    while (all_parts.Count != 0)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    //根据这个结构体里的信息，先把需要用到的零件的预制体都实例化出来：

    //    List<GameObject> all_parts_GB = new List<GameObject>();
    //    foreach(var pss in us.all_part_serialize_struct)
    //    {
    //        switch (pss.part_type_int)
    //        {
    //            case 0:
    //                GameObject power_part_prefab = Instantiate(Resources.Load<GameObject>("Part/PowerPart/" + pss.part_prefab_name));
    //                power_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
    //                power_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);
    //                PowerPart temp_power_part = power_part_prefab.GetComponent<PowerPart>();
    //                temp_power_part.enabled = true;
    //                temp_power_part.orderID = pss.orderID;
    //                all_parts_GB.Add(power_part_prefab);
    //                //temp_power_part.Init();//手动初始化，因为Start执行顺序有问题                    
    //                //temp_power_part.deserialize_over = true;
    //                break;
    //            case 1:
    //                GameObject leg_part_prefab = Instantiate(Resources.Load<GameObject>("Part/MovementPart/LegPart/" + pss.part_prefab_name));
    //                leg_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
    //                leg_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);
    //                LegPart temp_leg_part = leg_part_prefab.GetComponent<LegPart>();
    //                temp_leg_part.enabled = true;
    //                temp_leg_part.orderID = pss.orderID;
    //                all_parts_GB.Add(leg_part_prefab);
    //                break;
    //            case 2:
    //                GameObject ctrl_part_prefab = Instantiate(Resources.Load<GameObject>("Part/ControlPart/" + pss.part_prefab_name));
    //                ctrl_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
    //                ctrl_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);
    //                ControlPart temp_ctrl_part = ctrl_part_prefab.GetComponent<ControlPart>();
    //                temp_ctrl_part.enabled = true;
    //                temp_ctrl_part.orderID = pss.orderID;
    //                all_parts_GB.Add(ctrl_part_prefab);
    //                break;
    //            case 3:
    //                GameObject E_part_prefab = Instantiate(Resources.Load<GameObject>("Part/EquipmentPart/" + pss.part_prefab_name));
    //                E_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
    //                E_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);
    //                EquipmentPart temp_E_part = E_part_prefab.GetComponent<EquipmentPart>();
    //                temp_E_part.enabled = true;
    //                temp_E_part.orderID = pss.orderID;
    //                all_parts_GB.Add(E_part_prefab);
    //                break;

    //        }
          
           
    //    }
        
    //}
    #endregion
}
