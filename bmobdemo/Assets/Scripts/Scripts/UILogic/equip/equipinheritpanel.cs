using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class equipinheritpanel : BasePanel
{
    public Transform cur_equip;
    public Transform select_equip;
    public UILabel goldtext;
    public Transform prop_info;
    public Transform inheritBtn;
    public Transform selectbg;
    public Transform strongOneKeyBtn;

}
public class EquipInheritMediator : UIMediator<equipinheritpanel>
{

    public static EquipInheritMediator equipInheritMediator;
    private EquipItemInfo cur_equip_data;
    private EquipItemInfo select_equip_data;

    private EquipInfo cur_equip_info;
    private EquipStrong strong_info;
    public int cur_strong_lv;
    public int cur_star;
    public int next_strong_lv;
    private long select_equip_id;
    

    private equipinheritpanel panel
    {
        get
        {
            return m_Panel as equipinheritpanel;
        }
    }
    public EquipInheritMediator() : base("equipinheritpanel")
    {
        m_isprop = true;

        RegistPanelCall(NotificationID.EquipInherit_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipInherit_Hide, ClosePanel);
    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.inheritBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.selectbg.gameObject).onClick = OnClick;

    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipInheritMediator == null)
        {
            equipInheritMediator = Facade.RetrieveMediator("EquipInheritMediator") as EquipInheritMediator;
        }
        SetEquip();
    }
    /// <summary>
    /// 设置升星装备
    /// </summary>
    public void SetEquip()
    {
        select_equip_data = null;

        cur_equip_data = EquipMediator.cur_equip;
        cur_equip_info = EquipConfig.GetEquipInfo(int.Parse(cur_equip_data.itemID));

        select_equip_id = EquipMediator.cur_select_player_id == 0 ? GameConvert.LongConvert(cur_equip_data.uuid) : GameConvert.LongConvert(cur_equip_data.itemID);

        SetInfo();
    }
    public void SetInfo()
    {
        cur_star = cur_equip_data.star;
        cur_strong_lv = cur_equip_data.strongLevel;

        SetShow(false);
        SetEquipInfo();
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

        UILabel cur_strong_txt = UtilTools.GetChild<UILabel>(panel.prop_info, "cur_strong_lv");
        cur_strong_txt.text = "Lv." + cur_strong_lv.ToString();

    }
    /// <summary>
    /// 设置属性信息
    /// </summary>
    public void SetPropInfo()
    {
         strong_info = EquipStrongConfig.GetEquipStrongInfo(cur_equip_info.star, cur_strong_lv);
        if (strong_info == null)
            return;
        

        List<EquipAddInfo> prop_list = EquipConfig.GetPropAddDataListByID(-1,cur_equip_info.id, cur_star, cur_strong_lv);
        for (int i = 0; i < prop_list.Count; i++)
        {
            EquipAddInfo info = prop_list[i];

            string prop_name_label = string.Format("prop_name_{0}", i.ToString());
            UILabel prop_name = UtilTools.GetChild<UILabel>(panel.prop_info, prop_name_label);
            prop_name.text = TextManager.GetUIString(info.prop_name);

            string cur_prop_txt_label = string.Format("cur_prop_value_{0}", i.ToString());
            UILabel cur_prop_txt = UtilTools.GetChild<UILabel>(panel.prop_info, cur_prop_txt_label);
            cur_prop_txt.text = (info.prop_base_value + info.prop_strong_value + info.prop_star_value).ToString();
            
        }

    }

    private void SetInherit()
    {

        panel.goldtext.text = GetNeedEuro() + "";

        UISprite[] star_obj = UtilTools.GetChilds<UISprite>(panel.select_equip, "star");
        UtilTools.SetStar(select_equip_data.star, star_obj);

        UISprite color = UtilTools.GetChild<UISprite>(panel.select_equip, "color");
        color.spriteName = "color" + select_equip_data.star;

        UITexture icon = UtilTools.GetChild<UITexture>(panel.select_equip, "icon");
        LoadSprite.LoaderItem(icon, select_equip_data.itemID, false);


        next_strong_lv = select_equip_data.strongLevel - 10;
        strong_info = EquipStrongConfig.GetEquipStrongInfo(cur_equip_info.star, next_strong_lv);
        if (strong_info == null)
            return;




        UILabel next_strong_txt = UtilTools.GetChild<UILabel>(panel.prop_info, "next_strong_lv");
        next_strong_txt.text = "Lv." + next_strong_lv.ToString();

        List<EquipAddInfo> prop_list = EquipConfig.GetPropAddDataListByID(-1, int.Parse(select_equip_data.itemID), cur_star, next_strong_lv);
        for (int i = 0; i < prop_list.Count; i++)
        {
            EquipAddInfo info = prop_list[i];
            
            string next_prop_txt_label = string.Format("next_prop_value_{0}", i.ToString());
            UILabel next_prop_txt = UtilTools.GetChild<UILabel>(panel.prop_info, next_prop_txt_label);
            next_prop_txt.text = (info.prop_base_value + info.prop_next_strong_value + info.prop_next_star_value).ToString();

        }

    }




    private void OnClick(GameObject go)
    {
       
        switch (go.transform.name)
        {

            case "inheritBtn":
                if (select_equip_data == null)
                    return;

                long inherit_id = GameConvert.LongConvert(select_equip_data.uuid);

                int need_euro = GetNeedEuro();
                if(PlayerMediator.playerInfo.euro< need_euro)
                {
                    GUIManager.SetPromptInfo(TextManager.GetUIString("equip_error_6"), null);

                    return;
                }

                ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_INHERIT, EquipMediator.cur_select_player_id, select_equip_id, inherit_id);
                break;
            case "selectbg":

                List<EquipItemInfo> equip_list = EquipConfig.GetEquipDataListByLv(cur_strong_lv + 10);
                if(equip_list.Count<=0)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_24"), null);
                    return;
                }
                EquipChooseMediator.SelectItem = new EquipChooseMediator.SelectItemHandle(SelectEquip);

                List<object> list = new List<object>();

                EquipChooseData data = new EquipChooseData(EquipMediator.cur_select_player_id, Equip_Select_Type.Level, Equip_Pos.Null, cur_strong_lv+10);
                list.Add(data);

                Facade.SendNotification(NotificationID.EquipChoose_Show, list);

                break;
        }
    }

    private void SelectEquip(EquipItemInfo data)
    {
        select_equip_data = data;
        SetShow(true);

        SetInherit();

    }

    public void SetShow(bool show=false)
    {
        panel.goldtext.text = "0";
        bool is_show = show;
        panel.select_equip.transform.FindChild("add").gameObject.SetActive(!is_show);
        panel.select_equip.transform.FindChild("color").gameObject.SetActive(is_show);
        panel.select_equip.transform.FindChild("star").gameObject.SetActive(is_show);
        panel.select_equip.transform.FindChild("icon").gameObject.SetActive(is_show);

        panel.prop_info.transform.FindChild("next_strong_lv").gameObject.SetActive(is_show);
        panel.prop_info.transform.FindChild("next_prop_value_0").gameObject.SetActive(is_show);
        panel.prop_info.transform.FindChild("next_prop_value_1").gameObject.SetActive(is_show);


    }

    ///需要消耗欧元
    private int GetNeedEuro()
    {


        int inherit_lv = select_equip_data.strongLevel - 10;
        int euro = 100 * (inherit_lv - cur_strong_lv);

        return euro;
    }
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

}