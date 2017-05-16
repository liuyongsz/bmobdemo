using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class equipinformationpanel : BasePanel
{
    public Transform equipChooseItem;
    public Transform equipProp;
    public Transform equipInfo;
    public Transform closeBtn;
    public Transform takeOffBtn;
    public Transform changeBtn;
    public Transform strongBtn;
    public Transform gemInfo;
    public UILabel suit_txt;
    public UIGrid suitGrid;
}



public class EquipInformationMediator : UIMediator<equipinformationpanel>
{

    public static EquipInformationMediator equipInformationMediator;
    //选中的装备
    public  EquipItemInfo select_equip;
    //装备配置数据
    public EquipInfo cur_equip_info;
 
    //球员ＩＤ
    private int player_id;
    //当前装备ID 
    private int equip_id;
    private int suit_num = 0;
    private equipinformationpanel panel
    {
        get
        {
            return m_Panel as equipinformationpanel;
        }
    }
    public EquipInformationMediator() : base("equipinformationpanel")
    {
        m_isprop = true;

        RegistPanelCall(NotificationID.EquipInformation_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipInformation_Hide, ClosePanel);

    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;

        UIEventListener.Get(panel.takeOffBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.changeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.strongBtn.gameObject).onClick = OnClick;

    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {

        if (equipInformationMediator == null)
        {
            equipInformationMediator = Facade.RetrieveMediator("EquipInformationMediator") as EquipInformationMediator;
        }

        player_id = GameConvert.IntConvert((notification.Body as List<object>)[0]);
        select_equip = (notification.Body as List<object>)[1] as EquipItemInfo;
        cur_equip_info = EquipConfig.GetEquipInfo(int.Parse(select_equip.itemID));

        panel.suitGrid.enabled = true;
        panel.suitGrid.BindCustomCallBack(SuitGrid_UpdateItem);
        panel.suitGrid.StartCustom();


        SetEquipInfo();
        SetPropInfo();
        SetGemInfo();
        SetSuitInfo();
    }
    /// <summary>
    /// 设置装备信息
    /// </summary>
    private void SetEquipInfo()
    {
        UILabel pos_txt = UtilTools.GetChild<UILabel>(panel.equipProp, "pos_info");
        pos_txt.text = TextManager.GetUIString("position_" + cur_equip_info.position);

        UILabel des_txt = UtilTools.GetChild<UILabel>(panel.equipInfo, "des_txt");
        des_txt.text = TextManager.GetPropsString("description_" + cur_equip_info.id);

        UILabel name_txt = UtilTools.GetChild<UILabel>(panel.equipInfo, "name");
        name_txt.text = TextManager.GetItemString(select_equip.itemID);

        UILabel level_txt = UtilTools.GetChild<UILabel>(panel.equipInfo, "level");
        level_txt.text = select_equip.strongLevel.ToString();


        UISprite[] cur_star_obj = UtilTools.GetChilds<UISprite>(panel.equipInfo, "star");
        UtilTools.SetStar(select_equip.star, cur_star_obj);

        UISprite cur_color = UtilTools.GetChild<UISprite>(panel.equipInfo, "color");
        cur_color.spriteName = "color" + select_equip.star;
        
        UITexture icon = UtilTools.GetChild<UITexture>(panel.equipInfo, "icon");
        LoadSprite.LoaderItem(icon, select_equip.itemID, false);

    }

    /// <summary>
    /// 设置属性信息
    /// </summary>
    private void SetPropInfo()
    {
        float fight = 0;
        List<EquipAddInfo> prop_list = EquipConfig.GetPropAddDataListByID(-1,int.Parse( select_equip.itemID),select_equip.star,select_equip.strongLevel);
        for (int i = 0; i < prop_list.Count; i++)
        {
            EquipAddInfo info = prop_list[i];

            PropFightInfo prop_fight = PropChangeFightConfig.GetEquipInfo(info.prop_name);

            string prop_name_label = string.Format("prop_name_{0}", i.ToString());
            UILabel prop_name = UtilTools.GetChild<UILabel>(panel.equipProp, prop_name_label);
            prop_name.text = TextManager.GetUIString(info.prop_name);

            string prop_txt_label = string.Format("prop_value_{0}", i.ToString());
            UILabel cur_prop_txt = UtilTools.GetChild<UILabel>(panel.equipProp, prop_txt_label);
            float prop_value = GameConvert.FloatConvert((info.prop_base_value + info.prop_star_value + info.prop_strong_value));

            //cur_prop_txt.text = prop_fight.IsPercent==0?prop_value.ToString():string.Format("{0}%", prop_value*100);
            cur_prop_txt.text = prop_value.ToString();
            fight += PropChangeFightConfig.GetPropForFightValue(info.prop_name, prop_value);
        }

        UILabel fight_txt = UtilTools.GetChild<UILabel>(panel.equipProp, "fight_value");
        fight_txt.text = GameConvert.IntConvert(fight).ToString();
    }
    /// <summary>
    /// 设置信息
    /// </summary>
    public void SetSuitInfo()
    {
         suit_num = 0;
        if (cur_equip_info.suit <= 0)
            return;
        List<EquipItemInfo> equipList = EquipConfig.GetEquipDataListByPlayerID(player_id);
        for (int i = 0; i < equipList.Count; i++)
        {
            EquipInfo info = EquipConfig.GetEquipInfo(int.Parse(equipList[i].itemID));
            if (info.suit == cur_equip_info.suit)
                suit_num++;
        }
        List<object> listObj = new List<object>();
        SuitInfo suit_infp = SuitConfig.GetSuitInfoByID(cur_equip_info.suit);
        if (suit_infp == null)
            return;
        for (int j=0; j<4; j++)
        {
            SuitItemInfo suit = new SuitItemInfo();
            string descText = string.Format(TextManager.GetPropsString(UtilTools.StringBuilder("suit", suit_infp.id, j + 2)), suit_infp.suitAdd.Split(',')[j]);
            suit.content = descText;

            int active = suit_num > (j +1) ? 1 : 0;
            suit.active = active;
            listObj.Add(suit);
        }
        panel.suitGrid.AddCustomDataList(listObj);


    }

    private void SetGemInfo()
    {
        GameObject child_gem;
        int gem_state;

        string gem_data_info = select_equip.gem1 + ";" + select_equip.gem2 + ";" + select_equip.gem3; ;
        string[] gem_data_arr = gem_data_info.Split(';');

        for (int i = 0; i < 3; i++)
        {
            child_gem = panel.gemInfo.GetChild(i).gameObject;
            gem_state = GameConvert.IntConvert(gem_data_arr[i]);

            GameObject suo = child_gem.transform.FindChild("suo").gameObject;
            UITexture icon = child_gem.transform.FindChild("icon").GetComponent<UITexture>();

            suo.SetActive(gem_state==-1);

            icon.transform.gameObject.SetActive(gem_state > 0);
            if (gem_state > 0)
                LoadSprite.LoaderItem(icon, gem_state.ToString(), false);
        }
    }
    private void SuitGrid_UpdateItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;
        SuitItemInfo info = item.oData as SuitItemInfo;
        UILabel content = item.mScripts[0] as UILabel;
        content.text = info.content;
        content.color = info.active > 0 ? Color.green : Color.white;
    }
    private void OnClick(GameObject go)
    {
        switch (go.transform.name)
        {
            case "closeBtn":
                Facade.SendNotification(NotificationID.EquipInformation_Hide);
                break;
            case "takeOffBtn":
                ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_TAKE_OFF, player_id, cur_equip_info.id);
                break;
            case "changeBtn":
                List<object> list = new List<object>();

                Equip_Pos equip_pos = (Equip_Pos)Enum.Parse(typeof(Equip_Pos), (cur_equip_info.position).ToString());
                EquipChooseData data = new EquipChooseData(player_id, Equip_Select_Type.Pos, equip_pos, 0);
                list.Add(data);

                Facade.SendNotification(NotificationID.EquipChoose_Show, list); break;
            case "strongBtn":
                Facade.SendNotification(NotificationID.EquipInformation_Hide);
                Facade.SendNotification(NotificationID.EquipMain_Show);
                break;
        }
    }
    /// <summary>
    /// 点击装备
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickEquipItem(object data, GameObject go)
    {

        //MyUIToggle toggle = go.GetComponent<MyUIToggle>();
        //if (toggle.Value == true)
        //    return;
        select_equip = data as EquipItemInfo;
        ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_PUT_ON, player_id, GameConvert.LongConvert(select_equip.uuid));


    }

    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        select_equip = null;

    }

   
}
public class SuitItemInfo
{
    public int active;
    public string content;
}