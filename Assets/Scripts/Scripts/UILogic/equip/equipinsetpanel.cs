using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class equipinsetpanel : BasePanel
{
    public Transform cur_equip;

    public Transform gemInfo;

}
public class EquipInsetMediator : UIMediator<equipinsetpanel>
{

    public static EquipInsetMediator equipInsetMediator;
    private EquipItemInfo cur_equip_data;
    private EquipInfo cur_equip_info;

    public int cur_star;
    ///宝石数据
    public string gem_data_info;

    public long select_equip_id;
    private int m_max_gem_num = 3;
    private List<int> m_gem_state_list = new List<int>();
    public List<HoldInfo> m_holdInfo_list = new List<HoldInfo>();
    private HoldInfo cur_info;
    private equipinsetpanel panel
    {
        get
        {
            return m_Panel as equipinsetpanel;
        }
    }
    public EquipInsetMediator() : base("equipinsetpanel")
    {
        m_isprop = true;

        RegistPanelCall(NotificationID.EquipInset_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipInset_Hide, ClosePanel);
    }

    protected override void AddComponentEvents()
    {
        for (int i = 0; i < m_max_gem_num; i++)
        {
            GameObject child_gem = panel.gemInfo.GetChild(i).gameObject;
            UIEventListener.Get(child_gem.transform.FindChild("takeBtn").gameObject).onClick = OnClick;
            UIEventListener.Get(child_gem.transform.FindChild("openBtn").gameObject).onClick = OnClick;
            UIEventListener.Get(child_gem.transform.FindChild("insetBtn").gameObject).onClick = OnClick;

        }
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipInsetMediator == null)
        {
            equipInsetMediator = Facade.RetrieveMediator("EquipInsetMediator") as EquipInsetMediator;
        }
        SetEquip();
    }
    /// <summary>
    /// 设置升星装备
    /// </summary>
    public void SetEquip()
    {

        cur_equip_info = EquipConfig.GetEquipInfo(int.Parse(EquipMediator.cur_equip.itemID));
        select_equip_id = EquipMediator.cur_select_player_id == 0 ? GameConvert.LongConvert(EquipMediator.cur_equip.uuid) : GameConvert.LongConvert(EquipMediator.cur_equip.itemID);
        SetInfo();
    }

    public void SetInfo()
    {

        cur_equip_data = EquipMediator.cur_equip;
        cur_star = cur_equip_data.star;
        gem_data_info = cur_equip_data.gem1 + ";" + cur_equip_data.gem2 + ";" + cur_equip_data.gem3; ;
        m_gem_state_list.Clear();

        SetEquipInfo();
        SetGemInfo();
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
    //设置装备宝石信息
    private void SetGemInfo()
    {
        m_holdInfo_list.Clear();
        GameObject child_gem;
        int gem_state=-1;
        string[] gem_data_arr = gem_data_info.Split(';');
        string[] need_hold_arr = cur_equip_info.needOpenHold.Split(';');
        for (int i=0; i<m_max_gem_num; i++)
        {
            child_gem = panel.gemInfo.GetChild(i).gameObject;
            gem_state = GameConvert.IntConvert(gem_data_arr[i]);
            string[] hold_need_data = need_hold_arr[i].Split(':');
            HoldInfo hold_info = new HoldInfo();
            hold_info.m_gem_id = gem_state;
            hold_info.m_index = i + 1;

            hold_info.m_info = gem_state > 0 ? GemItemConfig.GetGemItem(gem_state) : null;

            if (gem_state > 0)
                hold_info.m_type = Hold_STATE.EQUIP;
            else if (gem_state == 0)
                hold_info.m_type = Hold_STATE.EMPTY;
            else
            {
                int need_num = GameConvert.IntConvert(hold_need_data[1]);
                int has_num = ItemManager.GetBagItemCount(hold_need_data[0]);
                HoldInfo previous_info = null;
                if (i - 1 >= 0)
                    previous_info = m_holdInfo_list[i - 1];

                if(previous_info==null||previous_info.m_type == Hold_STATE.EQUIP ||previous_info.m_type == Hold_STATE.EMPTY)
                    hold_info.m_type = has_num > need_num ? Hold_STATE.CAN_OPEN : Hold_STATE.MATERIAL_LOCK;
                else
                    hold_info.m_type = Hold_STATE.NOT_OPEN_LOCK;//上一个孔没开启
            }

            SetGemItemInfo(child_gem, hold_info);

            m_holdInfo_list.Add(hold_info);


        }

    }
    /// <summary>
    /// 设置宝石组件信息
    /// </summary>
    private void SetGemItemInfo(GameObject child,HoldInfo info)
    {

        GameObject suo = child.transform.FindChild("suo").gameObject;
        GameObject takeBtn = child.transform.FindChild("takeBtn").gameObject;
        GameObject openBtn = child.transform.FindChild("openBtn").gameObject;
        GameObject insetBtn = child.transform.FindChild("insetBtn").gameObject;

        UITexture icon = child.transform.FindChild("icon").GetComponent<UITexture>();
        UILabel descTxt = child.transform.FindChild("descTxt").GetComponent<UILabel>();
        UISprite color = child.transform.FindChild("color").GetComponent<UISprite>();

        icon.gameObject.SetActive(info.m_gem_id > 0);
        color.gameObject.SetActive(info.m_gem_id > 0);
        if (info.m_gem_id > 0)
        {
            ItemInfo item_info = ItemManager.GetItemInfo(info.m_gem_id.ToString());
            color.spriteName = "color" + item_info.color;
            LoadSprite.LoaderItem(icon, info.m_gem_id.ToString(), false);
        }
        suo.SetActive(info.m_type == Hold_STATE.MATERIAL_LOCK|| info.m_type == Hold_STATE.NOT_OPEN_LOCK|| info.m_type == Hold_STATE.CAN_OPEN);
        takeBtn.SetActive(info.m_type == Hold_STATE.EQUIP);
        openBtn.SetActive(info.m_type == Hold_STATE.CAN_OPEN);
        insetBtn.SetActive(info.m_type == Hold_STATE.EMPTY);

        string des = GetGemDesc(info);
 
        descTxt.text = des;

    }


    private void OnClick(GameObject go)
    {
        if (select_equip_id == 0)
            return;

        int index =  go.transform.parent.GetSiblingIndex();
        cur_info = m_holdInfo_list[index];

        switch (go.transform.name)
        {
            case "takeBtn":
                ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_GEM_TAKEOFF, cur_info.m_index, EquipMediator.cur_select_player_id, select_equip_id);
                break;
            case "openBtn":
                int material_id = 0;
                bool is_material = IsOpenMaterial(cur_info, out material_id);
                if(!is_material)
                {
                    GUIManager.SetPromptInfo(TextManager.GetUIString("UI2055"), null);
                    return;
                }

                //ShowItemInfo showItemInfo = new ShowItemInfo();
                //ItemMediator.panelType = PanelType.Use;
                //showItemInfo.useOne = UseItem;
                //Item item_data = ItemManager.GetBagItemInfo(material_id.ToString());

                //List<object> list_item = new List<object>();
                //list_item.Add(showItemInfo);
                //list_item.Add(item_data);
                //Facade.SendNotification(NotificationID.ItemInfo_Show, list_item);

                ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_GEM_OPEN, cur_info.m_index, EquipMediator.cur_select_player_id, select_equip_id);


                break;
            case "insetBtn":

                List<object> list = new List<object>();

                list.Add(EquipMediator.cur_select_player_id);
                list.Add(select_equip_id);
                list.Add(cur_info.m_index);
                Facade.SendNotification(NotificationID.GemChoose_Show, list);

                break;
        }
    }
    void UseItem(Item info)
    {
        ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_GEM_OPEN, cur_info.m_index, EquipMediator.cur_select_player_id, select_equip_id);
        Facade.SendNotification(NotificationID.ItemInfo_Hide);
        
    }
    /// <summary>
    /// 获取孔描述
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private string GetGemDesc(HoldInfo info)
    {
        string des = "";
        switch (info.m_type)
        {
            case Hold_STATE.EQUIP:
                PropFightInfo prop_fight = PropChangeFightConfig.GetEquipInfo(info.m_info.propType);

                string name = TextManager.GetItemString(info.m_gem_id.ToString());
                string value = prop_fight.IsPercent == 0 ? info.m_info.propValue.ToString() : string.Format("{0}%", info.m_info.propValue * 100);
                string prop_name = TextManager.GetUIString(info.m_info.propType);
                des += name + "\n" + prop_name + "+" + value.ToString();

                break;
            case Hold_STATE.EMPTY:
                des += TextManager.GetUIString("UI2057");
                break;
            case Hold_STATE.CAN_OPEN:
                des += TextManager.GetUIString("UI2056");
                break;
            case Hold_STATE.MATERIAL_LOCK:
                des += TextManager.GetUIString("UI2055");
                break;
            case Hold_STATE.NOT_OPEN_LOCK:
                des += TextManager.GetUIString("UI2058"); 
                break;
        }
        return des;
    }

    ///开槽材料是否满足
    private bool IsOpenMaterial(HoldInfo info,out int material_id)
    {
        string[] materal_arr = cur_equip_info.needOpenHold.Split(';');
        string material = materal_arr[info.m_index - 1];

        int id = GameConvert.IntConvert(material.Split(':')[0]);
        int need_num  = GameConvert.IntConvert(material.Split(':')[1]);
        material_id = id;
        int has_num = ItemManager.GetBagItemCount(id.ToString());
        if (id == 0 || need_num <= has_num)
            return true;

        return false;
    }

    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

}
public class HoldInfo
{
    //宝石id
    public int m_gem_id;
    //宝石顺序
    public int m_index;
    //装备类型
    public Hold_STATE m_type;
    //宝石id
    public GemItem m_info;
}
//孔状态信息类型
public enum Hold_STATE
{
    EQUIP,//已装备
    EMPTY,//未装备(缺少)
    CAN_OPEN, //可开孔
    MATERIAL_LOCK,//不可开孔(缺少材料)
    NOT_OPEN_LOCK,//不可开孔(上个没有打孔)
}