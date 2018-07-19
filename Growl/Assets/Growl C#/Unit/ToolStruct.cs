using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct UnitStruct
{
    public string unit_name;
    public string creat_time;//这个结构体被创建的时候的系统时间
    public List<PartSerializeStruct> all_part_serialize_struct;//这个设计所有的合法零件中需要被序列化的结构体都在这里
    public List<ModuleSaveData> all_modules_savedata;

}

[Serializable]
public class ModuleSaveData
{
    public string name;
    public int self_iconID_in_sprite;
}

[Serializable]
public class UserData
{
    public List<UnitStruct> user_blueprints;//这个列表用来储存所有用户自己设计的单位

    public UserData()
    {
        user_blueprints = new List<UnitStruct>();
    }
}


