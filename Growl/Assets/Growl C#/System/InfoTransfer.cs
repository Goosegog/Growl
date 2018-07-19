using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTransfer
{
    static InfoTransfer self;
    public static InfoTransfer GetInfoTransfer
    {
        get
        {
            if (self == null)
            {
                self = new InfoTransfer();
                return self;
            }
            else
            {
                return self;
            }
        }
    }

    public string want_loadingscene_name;
    public float loading_value_now;
    public Quaternion loading_anim_rotation_state;

}
