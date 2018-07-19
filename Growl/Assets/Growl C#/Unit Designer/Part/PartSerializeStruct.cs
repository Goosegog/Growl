using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PartSerializeStruct
{
    public string part_prefab_name;//储存这个零件的预制体的名字
    ////public string UIpart_prefab_name;//储存这个零件的UIpart预制体的名字
    public int part_type_int;//储存这个零件的种类的枚举
    public float[] pos;//储存零件在世界空间下的位置坐标
    public float[] rotation;//储存零件在世界空间下的旋转四元数
    public int orderID;
    public int[] parent_part;
    public List<int[]> child_part;
    public Dictionary<int, MountPointSerializeStruct> MP_struct_dict;

}
