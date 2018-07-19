using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MountPoint : MonoBehaviour
{
    public int ID;
    public MountPointType MP_type;
    Vector3 offset;//挂点在模型空间下相对于零件的偏移量
    public Vector3 normal;//挂点法线方向////注意：这个字段已经废弃，现在挂点的法线直接用transform.forward表示以简化旋转操作带来的手动法线更新
    [HideInEditorMode]
    public Part self_root_part;

    [HideInEditorMode]
    public int[] link_info;//【S】连接的零件ID和连接的节点的ID
    [HideInEditorMode]
    public Part link_part;//这个节点与哪个零件实例向连接，这个字段不需要【S】，因为可以通过 link_info 字段自己找回来
    [HideInEditorMode]
    public MountPoint link_mount_point;//指向与本节点连接的节点的实例，这个字段不需要【S】，因为可以通过 link_info 字段自己找回来
    MountPoint link_mount_point_last;//储存这个节点上一个连接的节点实例是谁，为了避免一个零件被拾取后立即被吸附到原位置的同一个节点上，
                                     //这个字段会保留短暂的时间，如果在这段时间内准备挂的节点与这个节点是同一个则不会连接
    float interval_time = 0.75f;//上面所提到的时间间隔是多少

    UnitBlueprint unit_blueprint;
    #region 序列化反序列化相关
    MountPointSerializeStruct MPSS;
    #endregion
    public MountPointSerializeStruct SerializeMPSS()
    {
        MPSS = new MountPointSerializeStruct();

        if (link_info[0] == -999)
        {
            MPSS.link_info = new int[3] { -999, -999, -999};
        }
        else
        {
            MPSS.link_info = new int[3] { link_info[0], link_part.orderID, link_info[1] };
        }

        return MPSS;
    }

    void Start ()
    {
        //Debug.Log(link_info[0]);     
    }
	public void Init()
    {
        link_info = new int[2] { -999, -999 };//连接的零件ID和连接的节点的ID
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        //找到自己归属于哪个零件：
        Transform search_parent = transform;//开始搜寻时将搜寻起点设置为自己
        while (search_parent.tag != "Part")
        {
            search_parent = search_parent.transform.parent;
        }
        self_root_part = search_parent.GetComponent<Part>();
    }


    void Update ()
    {
        //if (self_root_part.part_type == PartType.PowerPart && ID == 0)
        //{
        //    //Debug.Log(link_info[0]);
        //}
    }

    public bool Link2MountPoint(MountPoint target_MP)
    {
        //这个方法将传入的参数所属的零件作为本节点所属的零件的父级，对本零件进行连接与位移

        if(link_mount_point_last == target_MP)
        {
            //Debug.LogError("操作容差时间");
            return false;
        }

        if (link_info[0] != -999)
        {
            Debug.LogError("调用了 Link2MountPoint ，但这个节点已经挂在另一个节点上了！" + name + "挂载目标ID = " + link_info[0]);
            return false;
        }
      
        Vector3 offset2target_pos = target_MP.transform.position - transform.position;
        //找到目标零件，(已废弃：注意，此时直接 transform.root 获取的是设计平台)
        Part target_part = target_MP.self_root_part;
        self_root_part.transform.position += offset2target_pos;//把自己整个零件移过去

        //Debug.Log(link_info[0]);
        //Debug.Log(target_MP);
        //Debug.Log("++" + target_MP.transform.root.name);
        //Debug.Log(target_MP.transform.root.GetComponent<Part>());
        //Debug.Log(target_MP.transform.root.GetComponent<Part>().ID);

        //自身的节点信息更新：
        link_mount_point = target_MP;
        link_part = target_part;
        link_info[0] = target_part.ID;      
        link_info[1] = target_MP.ID;       
        //自身零件的父零件信息更新：
        //////self_root_part.parent_part = target_part;
        //把自身零件挂在场景中挂到父零件下面：
        //self_root_part.transform.SetParent(target_part.transform);
        //Debug.Log(link_info[0] + "#" + link_info[1]);
        //目标零件（本零件目前的父零件）的相应节点的节点信息更新：
        target_MP.link_mount_point = this;
        target_MP.link_part = self_root_part;
        target_MP.link_info[0] = self_root_part.ID;
        target_MP.link_info[1] = ID;
        Debug.Log(target_MP.link_info[0] + "#" + target_MP.link_info[1]);
        //目标零件（本零件目前的父零件）的子零件列表信息更新：
        ////target_part.child_part.Add(self_root_part);
        self_root_part.link_audio.Play();

        return true;
    }

    public void UnLink2MountPoint()
    {
        //与这个节点相连的节点实例的节点信息被清理：
        if (link_mount_point != null)
        {
            Debug.Log("即将清理的外部节点的名字是" + link_mount_point.name);
            link_mount_point.link_info[0] = -999;
            link_mount_point.link_info[1] = -999;
            link_mount_point.link_mount_point = null;
            link_mount_point.link_part = null;
            StartCoroutine(link_mount_point.CleanLastMP());
        }
        //这个零件的所有子零件不再以这个零件为父零件，并且直接挂到 Unit 下：
        //foreach (Part child_part in self_root_part.child_part)
        //{
        //    child_part.parent_part = null;
        //    //child_part.transform.SetParent(unit_blueprint.transform);
        //}
        //开启容差时间的协程：
        if (link_mount_point != null)
        {
            link_mount_point_last = link_mount_point;
            StartCoroutine(CleanLastMP());
        }

        //这个节点自身的信息被清理：
        link_mount_point = null;
        link_info[0] = -999;
        link_info[1] = -999;
        //自身节点的连接零件引用被置空：
        link_part = null;
        //自身零件的父级零件的子零件列表中移除自身：
        
        //////////if (self_root_part.parent_part != null && !self_root_part.parent_part.child_part.Contains(self_root_part))
        //////////{
        //////////    //如果自身的父零件不为null，但是父零件的子零件列表中找不到自身，报出严重错误
        //////////    Debug.Log("严重错误：要移除的零件的 父级零件的子集零件列表 中没有当前零件！");
        //////////}
        //////////else if(self_root_part.parent_part != null)
        //////////{
        //////////    self_root_part.parent_part.child_part.Remove(self_root_part);
        //////////}
        //自身零件的父级零件被置空：
        self_root_part.parent_part = null;
        //把自身零件挂在设计平台的 Unit 下
        self_root_part.transform.SetParent(unit_blueprint.transform);
        Debug.LogError("归属于" + self_root_part.name + "的节点" + ID + "已经断开连接");

        self_root_part.link_audio.Play();

    }

    IEnumerator CleanLastMP()
    {
        yield return new WaitForSeconds(interval_time);
        link_mount_point_last = null;
    }
}
