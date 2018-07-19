using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MountPointType
{
    PowerOutput,//动力输出挂点
    PowerInput,//动力输入挂点
    Normal,//普通零件挂点
}
public enum PartType
{
    PowerPart,
    LegPart,
    ControlPart,
    Equipment,
}
public enum DamageType
{
    Normal,
    Blast,
    Energy,
}
public enum AttackPattern
{
    Shooting,
    Missile,
}
public enum WeaponSpecialty
{
    Dartle,


}
public enum MovementPartSpecialty
{
    DodgeAddition,//闪避值可叠加
    RA,//反应装甲
}

