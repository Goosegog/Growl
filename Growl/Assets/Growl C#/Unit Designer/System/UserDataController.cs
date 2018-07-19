using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class UserDataController : MonoBehaviour
{
    static UserDataController _user_data_controller;
    public static UserDataController GetSingleton
    {
        get { return _user_data_controller; }

    }

    [HideInInspector]
    public UserData UD;

    string data_path;

    void Start()
    {
        _user_data_controller = this;
        UD = new UserData();
        data_path = @"" + Application.persistentDataPath + @"\UserData.save";
       
        LoadUserDataAtDesignStart();
        Debug.LogError(data_path);
    }
	
	void Update ()
    {
        //Debug.LogError(name + "////" + GetComponentsInChildren<Transform>().Length);
    }
    
    void LoadUserDataAtDesignStart()
    {
        
        if (System.IO.File.Exists(data_path))
        {
            Debug.LogError("已近存在存档文件，无需新建");
        }
        else
        {
            //没有存档，新建存档：            
            SaveUserData();
            Debug.LogError("不存在存档文件，已近新建存档");

        }
        //打开存档文件：
        FileStream stream = new FileStream(@data_path, FileMode.Open);
        BinaryFormatter bFormat = new BinaryFormatter();
        UD = (UserData)bFormat.Deserialize(stream);//反序列化得到的是一个object对象.必须做下类型转换
        Debug.LogError("用户数据已经反序列化为 UserDataController 单例 的 UD 字段");
        stream.Close();
    }

    public void SaveUserData()
    {
        //将这个字段序列化到指定路径中：
        Debug.LogError(Application.persistentDataPath);
        FileStream stream = new FileStream(@data_path, FileMode.Create);
        BinaryFormatter bFormat = new BinaryFormatter();
        // 注意： Serialize 序列化要求字段中包含的字段也必须被标记为 [Serializable]
        bFormat.Serialize(stream, UD);
        stream.Close();
        Debug.LogError("当前设计已经以二进制流保存到外部的指定路径下");
    }
}
