using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class equipstrongpanel : BasePanel
{
    public Transform cur_equip;
    public UILabel goldtext;
    public Transform prop_info;
    public Transform strongBtn;
    public Transform strongOneKeyBtn;

}
public class EquipStrongMediator : UIMediator<equipstrongpanel>
{

    public static EquipStrongMediator equipStrongMediator;
    private EquipItemInfo cur_equip_data;
    private EquipInfo cur_equip_info;
    private EquipStrong strong_info;
    public int cur_strong_lv;
    public int cur_star;
    public int next_strong_lv;
    private long select_equip_id;
    private equipstrongpanel panel
    {
        get
        {
            return m_Panel as equipstrongpanel;
        }
    }
    public EquipStrongMediator() : base("equipstrongpanel")
    {
        m_isprop = true;

        RegistPanelCall(NotificationID.EquipStrong_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipStrong_Hide, ClosePanel);
    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.strongBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.strongOneKeyBtn.gameObject).onClick = OnClick;

    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipStrongMediator == null)
        {
            equipStrongMediator = Facade.RetrieveMediator("EquipStrongMediator") as EquipStrongMediator;
        }
        SetEquip();
    }
    /// <summary>
    /// 设置升星装备
    /// </summary>
    public void SetEquip()
    {
        cur_equip_data = EquipMediator.cur_equip;
        if (int.Parse(cur_equip_data.itemID) == 0)
            return;
        cur_equip_info = EquipConfig.GetEquipInfo(int.Parse(cur_equip_data.itemID));

        select_equip_id = EquipMediator.cur_select_player_id == 0 ? GameConvert.LongConvert(cur_equip_data.uuid) : GameConvert.LongConvert(cur_equip_data.itemID);

        ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_STAR, EquipMediator.cur_select_player_id, select_equip_id);


    }
    public void SetInfo(int star = 0, int stronglv = 0)
    {
        cur_equip_data = EquipMediator.cur_equip;
        cur_star = star;
        cur_strong_lv = stronglv;
        next_strong_lv = cur_strong_lv >= cur_equip_info.maxStrongLevel ? cur_equip_info.maxStrongLevel : cur_strong_lv + 1;
        SetEquipInfo();
        SetStrongLvInfo();
        SetPropInfo();
    }
    /// <summary>
    /// 设置装备信息
    /// </summary>
    private void SetEquipInfo()
    {
        UISprite[] cur_star_obj = UtilTools.GetChilds<UISprite>(panel.cur_equip, "star");
        UtilTools.SetStar(cur_star, cur_star_obj);

        UISprite cur_color = UtilTools.GetChild<UISprite>(panel.cur_equip, "color");
        cur_color.spriteName = "color" + cur_star;

         UITexture icon = UtilTools.GetChild<UITexture>(panel.cur_equip, "icon");
        LoadSprite.LoaderItem(icon, cur_equip_data.itemID, false);

    }
    public void SetStrongLvInfo()
    {
        UILabel cur_strong_txt = UtilTools.GetChild<UILabel>(panel.prop_info, "cur_strong_lv");
        cur_strong_txt.text = "Lv." + cur_strong_lv.ToString();

        UILabel next_strong_txt = UtilTools.GetChild<UILabel>(panel.prop_info, "next_strong_lv");
        next_strong_txt.text = "Lv."+next_strong_lv.ToString();

    }
    /// <summary>
    /// 设置属性信息
    /// </summary>
    public void SetPropInfo()
    {
         strong_info = EquipStrongConfig.GetEquipStrongInfo(cur_equip_info.star, cur_strong_lv);
        if (strong_info == null)
            return;
            
        panel.goldtext.text = strong_info.cost.ToString();

        List<EquipAddInfo> prop_list = EquipConfig.GetPropAddDataListByID(1,cur_equip_info.id, cur_star, cur_strong_lv);
        for (int i = 0; i < prop_list.Count; i++)
        {
            EquipAddInfo info = prop_list[i];

            string prop_name_label = string.Format("prop_name_{0}", i.ToString());
            UILabel prop_name = UtilTools.GetChild<UILabel>(panel.prop_info, prop_name_label);
            prop_name.text = TextManager.GetUIString(info.prop_name);

            string cur_prop_txt_label = string.Format("cur_prop_value_{0}", i.ToString());
            UILabel cur_prop_txt = UtilTools.GetChild<UILabel>(panel.prop_info, cur_prop_txt_label);
            cur_prop_txt.text = (info.prop_base_value + info.prop_strong_value + info.prop_star_value).ToString();

            string next_prop_txt_label = string.Format("next_prop_value_{0}", i.ToString());
            UILabel next_prop_txt = UtilTools.GetChild<UILabel>(panel.prop_info, next_prop_txt_label);
            next_prop_txt.text = (info.prop_base_value + info.prop_next_strong_value + info.prop_next_star_value).ToString();

        }

    }




    private void OnClick(GameObject go)
    {
        if (select_equip_id == 0)
            return;
        if (strong_info == null || PlayerMediator.playerInfo.euro < strong_info.cost)
        {
            GUIManager.SetPromptInfo(TextManager.GetUIString("equip_error_6"), null);
            return;
        }
        if ( PlayerMediator.playerInfo.level <= cur_strong_lv)
        {
            GUIManager.SetPromptInfo(TextManager.GetUIString("equip_error_7"), null);
            return;
        }
        if (PlayerMediator.playerInfo.level >= cur_equip_info.maxStrongLevel)
        {
            GUIManager.SetPromptInfo(TextManager.GetUIString("equip_error_8"), null);
            return;
        }
        switch (go.transform.name)
        {

            case "strongBtn":
                ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_UP_STRONG, EquipMediator.cur_select_player_id, select_equip_id);
                break;
            case "strongOneKeyBtn":
                ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_ONEKEY_STRONG, EquipMediator.cur_select_player_id, select_equip_id);

                break;
        }
    }

  

    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

}