using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class equipmakepanel : BasePanel
{
    public Transform cur_equip;
    public UILabel goldtext;
    public UIGrid equipGrid;
    public UIGrid costGrid;


    public Transform equipItemInfo;
    public Transform costItem;


    public Transform headBtn;
    public Transform clothBtn;
    public Transform trousersBtn;
    public Transform legguardBtn;
    public Transform shoesBtn;

    public Transform makeEquipBtn;
    public UILabel gold_num;

}

public enum EQUIP_POS_DEF
{
    None = 0,       //默认值无位置
    Hat = 1,    //头部
    Cloth = 2,    //球衣
    Legguard = 3,    //护腿
    Trousers = 4,    //裤子
    Shoe = 5,    //鞋子
};

public class EquipMakeMediator : UIMediator<equipmakepanel>
{

    public static EquipMakeMediator equipMakeMediator;
    private EquipInfo cur_equip_info;
    private ItemInfo item_equip_info;
    private EquipMakeInfo cur_make_info;
    private int m_currEquipPosDef = 0;    //当前选择位置                 
    private List<object> m_EquipDataList = new List<object>();                                          //当前选择装备数据列表
    private equipmakepanel panel
    {
        get
        {
            return m_Panel as equipmakepanel;
        }
    }
    public EquipMakeMediator() : base("equipmakepanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.EquipMake_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipMake_Hide, ClosePanel);

        RegistPanelCall(NotificationID.BagRefresh, RefreshBag);

    }
    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipMakeMediator == null)
        {
            equipMakeMediator = Facade.RetrieveMediator("EquipMakeMediator") as EquipMakeMediator;
        }
        InitGride();

        ChooseEquipPos(EQUIP_POS_DEF.Hat);
    }
    /// <summary>
    /// 初始化 uigride
    /// </summary>
    private void InitGride()
    {
        panel.costGrid.enabled = true;
        panel.costGrid.BindCustomCallBack(OnUpdateCost);
        panel.costGrid.StartCustom();

        panel.equipGrid.enabled = true;
        panel.equipGrid.BindCustomCallBack(OnUpdateEquip);
        panel.equipGrid.StartCustom();

    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.makeEquipBtn.gameObject).onClick = OnClick;


        UIEventListener.Get(panel.headBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.clothBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.trousersBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.legguardBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.shoesBtn.gameObject).onClick = OnClick;
        
    }
    /// <summary>
    /// 刷新按钮状态
    /// </summary>
    private void RefreshBtnStates()
    {
        panel.headBtn.GetComponent<UIToggle>().value = (int)EQUIP_POS_DEF.Hat == m_currEquipPosDef;
        panel.clothBtn.GetComponent<UIToggle>().value = (int)EQUIP_POS_DEF.Cloth == m_currEquipPosDef;
        panel.trousersBtn.GetComponent<UIToggle>().value = (int)EQUIP_POS_DEF.Trousers == m_currEquipPosDef;
        panel.legguardBtn.GetComponent<UIToggle>().value = (int)EQUIP_POS_DEF.Legguard == m_currEquipPosDef;
        panel.shoesBtn.GetComponent<UIToggle>().value = (int)EQUIP_POS_DEF.Shoe == m_currEquipPosDef;

    }

    //选择装备类型
    public void ChooseEquipPos(EQUIP_POS_DEF pos)
    {
        ChooseEquipPos((int)pos);
        RefreshBtnStates();
    }

    public void ChooseEquipPos(int pos)
    {
        if (m_currEquipPosDef == pos) return;
        m_currEquipPosDef = pos;
        m_EquipDataList.Clear();

        InitEquipListData(m_currEquipPosDef);
        SetEquip();
    }
    private void InitEquipListData(int equippos = 0)
    {
        m_EquipDataList = EquipMakeConfig.GetEquipListByPos(equippos);

        cur_make_info = m_EquipDataList[0] as EquipMakeInfo;
        cur_equip_info = EquipConfig.GetEquipInfo(cur_make_info.ID);
        item_equip_info = ItemManager.GetItemInfo(cur_equip_info.id.ToString());

        panel.equipGrid.AddCustomDataList(m_EquipDataList);
        panel.equipGrid.SetSelect(0);
    }
    private void OnUpdateEquip(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        item.onClick = ClickEquipItem;
        item.Selected = false;
        UITexture star = item.mScripts[0] as UITexture;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture icon = item.mScripts[3] as UITexture;
        UILabel name = item.mScripts[2] as UILabel;
        UISprite[] equip_star = UtilTools.GetChilds<UISprite>(item.transform, "star");

        EquipMakeInfo info = item.oData as EquipMakeInfo;
        EquipInfo equip_info = EquipConfig.GetEquipInfo(info.ID);
        
        name.text = TextManager.GetItemString(info.ID.ToString());
        color.spriteName = "color" + equip_info.star;
        LoadSprite.LoaderItem(icon, info.ID.ToString(), false);

        UtilTools.SetTextColor(name, equip_info.star);
        UtilTools.SetStar(equip_info.star, equip_star);

    }

    private void OnUpdateCost(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        EquipCostInfo info = item.oData as EquipCostInfo;

        UISprite color = item.mScripts[0] as UISprite;
        UITexture icon = item.mScripts[1] as UITexture;
        UILabel num = item.mScripts[2] as UILabel;

        ItemInfo item_info = ItemManager.GetItemInfo(info.item_id.ToString());
        color.spriteName = "color" + item_info.color;

        LoadSprite.LoaderItem(icon, info.item_id.ToString(), false);


        int total_num = ItemManager.GetBagItemCount(info.item_id.ToString());
        num.text = string.Format("{0}/{1}",  total_num, info.need_num.ToString());
        num.color = total_num >= info.need_num ? Color.white : Color.red;
    }

    private void OnClick(GameObject go)
    {
        switch (go.transform.name)
        {
            
            case "makeEquipBtn":

                if (PlayerMediator.playerInfo.euro < cur_make_info.money)
                {
                    GUIManager.SetPromptInfo(TextManager.GetUIString("equip_error_6"), null);
                    return;
                }
                if(!IsCanMakeEquip())
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
                    return;
                }
                ServerCustom.instance.SendClientMethods(EquipProxy.CLINET_MAKE_EQUIP, cur_make_info.ID);
                break;

            case "headBtn":
                ChooseEquipPos(EQUIP_POS_DEF.Hat);
                break;

            case "clothBtn":
                ChooseEquipPos(EQUIP_POS_DEF.Cloth);
                break;

            case "trousersBtn":
                ChooseEquipPos(EQUIP_POS_DEF.Trousers);
                break;

            case "legguardBtn":
                ChooseEquipPos(EQUIP_POS_DEF.Legguard);
                break;

            case "shoesBtn":
                ChooseEquipPos(EQUIP_POS_DEF.Shoe);
                break;
          
        }
    }
    /// <summary>
    /// 点击装备
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickEquipItem(UIGridItem item)
    {

        //go.GetComponent<MyUIToggle>().Value = true;

        cur_make_info = item.oData as EquipMakeInfo;
        cur_equip_info = EquipConfig.GetEquipInfo(cur_make_info.ID);
        item_equip_info = ItemManager.GetItemInfo(cur_equip_info.id.ToString());
        SetEquip();
    }

    /// <summary>
    /// 设置升星装备
    /// </summary>
    public void SetEquip()
    {
        SetEquipInfo();
        SetPropInfo();
        SetCostInfo();
    }
    /// <summary>
    /// 设置装备信息
    /// </summary>
    private void SetEquipInfo()
    {
        UISprite[] cur_star = UtilTools.GetChilds<UISprite>(panel.cur_equip, "star");
        UtilTools.SetStar(cur_equip_info.star, cur_star);

        UISprite cur_color = UtilTools.GetChild<UISprite>(panel.cur_equip, "color");
        cur_color.spriteName = "color" + cur_equip_info.star;

        UITexture icon = UtilTools.GetChild<UITexture>(panel.cur_equip, "icon");
        LoadSprite.LoaderItem(icon, cur_equip_info.id.ToString(), false);

        UILabel name = UtilTools.GetChild<UILabel>(panel.cur_equip, "name");
        name.text = TextManager.GetItemString(cur_equip_info.id.ToString());
        UtilTools.SetTextColor(name, cur_equip_info.star);

        panel.gold_num.text = cur_make_info.money.ToString();

    }

    /// <summary>
    /// 设置属性信息
    /// </summary>
    private void SetPropInfo()
    {
        float fight = 0;
        List<EquipAddInfo> prop_list = GetPropAddList();
        for (int i = 0; i < prop_list.Count; i++)
        {
            EquipAddInfo info = prop_list[i];

            string prop_name_label = string.Format("prop_name_{0}", i.ToString());
            UILabel prop_name = UtilTools.GetChild<UILabel>(panel.cur_equip, prop_name_label);
            prop_name.text = TextManager.GetUIString(info.prop_name);

            string prop_txt_label = string.Format("prop_value_{0}", i.ToString());
            UILabel cur_prop_txt = UtilTools.GetChild<UILabel>(panel.cur_equip, prop_txt_label);
            cur_prop_txt.text = (info.prop_base_value).ToString();

            fight += PropChangeFightConfig.GetPropForFightValue(info.prop_name, GameConvert.FloatConvert(info.prop_base_value));
        }

        UILabel fight_txt = UtilTools.GetChild<UILabel>(panel.cur_equip, "fight_value");
        fight_txt.text = GameConvert.IntConvert(fight).ToString();
    }
    /// <summary>
    /// 制作成功
    /// </summary>
    public void MakeSucc()
    {
        SetCostInfo();
        GUIManager.SetPromptInfo("制作成功", null);
    }

    /// <summary>
    /// 刷新材料
    /// </summary>
    /// <param name="notification"></param>
    private void RefreshBag(INotification notification)
    {
        if (GUIManager.HasView("equipmakepanel"))
        {
            int type = GameConvert.IntConvert((notification.Body as List<object>)[0]);
            if (type == (int)BagChangeType.Update||type == (int)BagChangeType.Remove)
                SetCostInfo();
        }
    }
    /// <summary>
    /// 消耗信息
    /// </summary>
    public void SetCostInfo()
    {

        string[] need_str_arr = cur_make_info.Cost.Split(',');
        // 默认打开球队卡牌界面
        List<object> listObj = new List<object>();
        for (int i = 0; i < need_str_arr.Length; i++)
        {
            EquipCostInfo info = new EquipCostInfo();
            string[] info_arr = need_str_arr[i].Split(':');
            info.item_id = GameConvert.IntConvert(info_arr[0]);
            info.need_num = GameConvert.IntConvert(info_arr[1]);
            listObj.Add(info);
        }
        panel.costGrid.AddCustomDataList(listObj);
        
    }



    /// <summary>
    /// 获取属性增加数据
    /// </summary>
    /// <returns></returns>

    private List<EquipAddInfo> GetPropAddList()
    {
        EquipAddInfo info = null;
        string[] prop_arr = Define.EquipPropStr.Split(',');
        List<EquipAddInfo> list = new List<EquipAddInfo>();

        for (int i = 0; i < prop_arr.Length; i++)
        {
            string prop_str = prop_arr[i];
            int prop_value = GameConvert.IntConvert(cur_equip_info.GetType().GetField(prop_str).GetValue(cur_equip_info));

            if (prop_value == 0)
                continue;

            info = new EquipAddInfo();
            
            info.prop_name = prop_str;
            info.prop_base_value = prop_value;
      
            list.Add(info);
        }
        return list;
    }
    /// <summary>
    /// 是否可以制作
    /// </summary>
    /// <returns></returns>
    private bool IsCanMakeEquip()
    {
        string[] need_str_arr = cur_make_info.Cost.Split(',');
        List<object> listObj = new List<object>();
        for (int i = 0; i < need_str_arr.Length; i++)
        {
            string[] info_arr = need_str_arr[i].Split(':');
            int item_id = GameConvert.IntConvert(info_arr[0]);
            int need_num = GameConvert.IntConvert(info_arr[1]);
            int total_num = ItemManager.GetBagItemCount(item_id.ToString());
            if (need_num > total_num)
                return false;
        }
        return true;
    }
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_currEquipPosDef = 0;
        cur_equip_info = null;
    }

}