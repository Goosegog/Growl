using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleButtonInUnitInfoInterface : MonoBehaviour
{
    [HideInInspector]
    public AudioSource installed_AS;
    [HideInInspector]
    public bool installed = false;
    [HideInInspector]
    public Text click_text;
    [HideInInspector]
    public Image icon;
    [HideInInspector]
    public int self_iconID_in_sprite;
    [HideInInspector]
    public Text name_text;


    public ModuleSaveData SerializePSS()
    {
        ModuleSaveData MSD = new ModuleSaveData();
        MSD.name = name_text.text;
        MSD.self_iconID_in_sprite = self_iconID_in_sprite;
        return MSD;
    }
    void Start()
    {
        installed_AS = GetComponentInParent<AudioSource>();
    }


    void Update()
    {

    }

    public void Init()
    {
        foreach (var t in GetComponentsInChildren<Transform>())
        {
            if (t.name == "ClickText")
            {
                click_text = t.GetComponent<Text>();
            }
            else if (t.name == "Icon")
            {
                icon = t.GetComponent<Image>();
            }
            else if(t.name == "NameText")
            {
                name_text = t.GetComponent<Text>();
            }
        }

        icon.gameObject.SetActive(false);
        name_text.gameObject.SetActive(false);
    }

    public void Install(Sprite _icon, int _self_iconID_in_sprite, string _name)
    {
        click_text.gameObject.SetActive(false);
        icon.sprite = _icon;
        self_iconID_in_sprite = _self_iconID_in_sprite;
        name_text.text = _name;
        icon.gameObject.SetActive(true);
        name_text.gameObject.SetActive(true);
        installed_AS.Play();
        installed = true;
    }

    public void Uninstall()
    {
        icon.sprite = null;
        name_text.text = null;

        icon.gameObject.SetActive(false);
        name_text.gameObject.SetActive(false);
        click_text.gameObject.SetActive(true);

        installed_AS.Play();
        installed = false;
    }


}
