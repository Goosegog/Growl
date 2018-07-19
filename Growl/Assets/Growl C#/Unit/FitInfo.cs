using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitInfo
{
    
    int partID;
    int part_orderID;
    int MPID;
    int link_partID;
    int link_part_orderID;
    int link_MPID;

    public FitInfo(int _partID, int _part_orderID, int _MPID, int _link_partID, int _link_part_orderID, int _link_MPID)
    {
        partID = _partID;
        part_orderID = _part_orderID;
        MPID = _MPID;
        link_partID = _link_partID;
        link_part_orderID = _link_partID;
        link_MPID = _link_MPID;
    }
}
