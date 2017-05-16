using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;

/// <summary>
/// 实例化控件
/// </summary>
public class equipstarpanel : BasePanel
{

    public Transform cur_equip;
    public Transform next_equip;
    public Transform prop_info;
    public Transform starUpBtn;
    public Transform costItem;

    public UIGrid costGrid;


}

public class EquipAddInfo
{
    public string prop_name;
    public int prop_base_value;

    public int prop_star_value;
    public int prop_next_star_value;

    public int prop_strong_value;
    public int prop_next_strong_value;

}

public class EquipCostInfo
{
    public int item_id;
    public int need_num;

}

public class EquipStarMediator : UIMediator<equipstarpanel>
{

    public static EquipStarMediator equipStarMediator;
    private  EquipItemInfo cur_equip_data;
    private EquipInfo cur_equip_info;
    public int cur_star_num;
    public int cur_strong_lv;

    public int next_star_num;
    private long select_equip_id;
    private bool m_openSetInfo;
    private equipstarpanel panel
    {
        get
        {
            return m_Panel as equipstarpanel;
        }
    }
    public EquipStarMediator() : base("equipstarpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.EquipStar_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipStar_Hide, ClosePanel);

        RegistPanelCall(NotificationID.BagRefresh, RefreshBag);

    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipStarMediator == null)
        {
            equipStarMediator = Facade.RetrieveMediator("EquipStarMediator") as EquipStarMediator;
        }

        m_Panel.transform.GetComponent<UIPanel>().depth = 6;
        InitGride();
        SetEquip();
        if (m_openSetInfo)
            SetInfo();
    }
    private void InitGride()
    {
        panel.costGrid.enabled = true;
        panel.costGrid.BindCustomCallBack(OnUpdateCost);
        panel.costGrid.StartCustom();
    }
    /// <summary>
    /// 设置升星装备
    /// </summary>
    public  void SetEquip()
    {
        cur_equip_data = EquipMediator.cur_equip;
        cur_equip_info = EquipConfig.GetEquipInfo(int.Parse(cur_equip_data.itemID));

        select_equip_id = EquipMediator.cur_select_player_id == 0 ? GameConvert.LongConvert(cur_equip_data.uuid) : GameConvert.LongConvert(cur_equip_data.itemID);


        ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_STAR, EquipMediator.cur_select_player_id, select_equip_id);



    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="star"></param>
    /// <param name="stronglv"></param>
    public void SetInfo(int star=0,int stronglv=0)
    {
        if (null == m_Panel)
        {
            m_openSetInfo = true;
            return;
        }
        cur_equip_data = EquipMediator.cur_equip;
        cur_star_num = star;
        cur_strong_lv = stronglv;
        next_star_num = cur_star_num == cur_equip_info.maxStar ? cur_equip_info.maxStar : cur_star_num + 1;
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
        UISprite[] next_star = UtilTools.GetChilds<UISprite>(panel.next_equip, "star");

        UtilTools.SetStar(cur_star_num, cur_star);
        UtilTools.SetStar(next_star_num, next_star);

        UITexture icon = UtilTools.GetChild<UITexture>(panel.cur_equip, "icon");
        LoadSprite.LoaderItem(icon, cur_equip_data.itemID, false);

        UITexture next_icon = UtilTools.GetChild<UITexture>(panel.next_equip, "icon");
        LoadSprite.LoaderItem(next_icon, cur_equip_data.itemID, false);

        UISprite cur_color = UtilTools.GetChild<UISprite>(panel.cur_equip, "color");
        cur_color.spriteName = "color" + cur_star_num;

        UISprite next_color = UtilTools.GetChild<UISprite>(panel.next_equip, "color");
        next_color.spriteName = "color" + next_star_num;

    }
    /// <summary>
    /// 设置属性信息
    /// </summary>
    private void SetPropInfo()
    {
        UISprite[] cur_star = UtilTools.GetChilds<UISprite>(panel.prop_info, "star");
        UISprite[] next_star = UtilTools.GetChilds<UISprite>(panel.prop_info, "next_star");

        UtilTools.SetStar(cur_star_num, cur_star);
        UtilTools.SetStar(next_star_num, next_star);

        List<EquipAddInfo> prop_list = EquipConfig.GetPropAddDataListByID(0,cur_equip_info.id,cur_star_num, cur_strong_lv);
        for (int i=0; i<prop_list.Count; i++)
        {
            EquipAddInfo info = prop_list[i];

            string prop_name_label = string.Format("prop_name_{0}",i.ToString());
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

    /// <summary>
    /// 消耗信息
    /// </summary>
    private void SetCostInfo()
    {
        EquipStar star_info = EquipStarConfig.GetEquipStarInfo(cur_equip_info.id, cur_star_num);
        if (star_info == null ||string.IsNullOrEmpty(star_info.cost))
            return;
        string[] need_str_arr = star_info.cost.Split(';');
        // 默认打开球队卡牌界面
        List<object> listObj = new List<object>();
        for (int i = 0; i < need_str_arr.Length; i++)
        {
            EquipCostInfo info = new EquipCostInfo();
            string[] info_arr = need_str_arr[i].Split(':');
            info.item_id = GameConvert.IntConvert(info_arr[0]);
            info.need_num = GameConvert.IntConvert(info_arr[1]);
            if (info.item_id == 0)
                continue;
            listObj.Add(info);
        }
    
        panel.costGrid.AddCustomDataList(listObj);

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
        if (item_info == null)
            return;
        color.spriteName = "color" + item_info.color;

        LoadSprite.LoaderItem(icon, info.item_id.ToString(), false);


        int total_num = ItemManager.GetBagItemCount(info.item_id.ToString());
        num.text = string.Format("{0}/{1}", total_num, info.need_num.ToString());
        num.color = total_num >= info.need_num ? Color.white : Color.red;

    }

    protected override void AddComponentEvents()
    {
        if (null == m_Panel.starUpBtn) return;

        UIEventListener.Get(panel.starUpBtn.gameObject).onClick = OnClick;
    }
    
    private void OnClick(GameObject go)
    {
        switch (go.transform.name)
        {
          
            case "starUpBtn":
                if(cur_equip_data.star>=cur_equip_info.maxStar)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_25"), null);

                    return;
                }
                bool is_materials = IsMaterials();
                if (!is_materials)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
                    return;
                }

                ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_BAG_EQUIP_UP_STAR, EquipMediator.cur_select_player_id, select_equip_id);
                break;
         
        }
    }
    /// <summary>
    /// 刷新材料
    /// </summary>
    /// <param name="notification"></param>
    private void RefreshBag(INotification notification)
    {
        if (GUIManager.HasView("equipstarpanel"))
        {
            SetCostInfo();
        }
    }
    /// <summary>
    /// 是否材料足够
    /// </summary>
    private bool IsMaterials()
    {
        EquipStar star_info = EquipStarConfig.GetEquipStarInfo(cur_equip_info.id, cur_star_num);
        if (star_info == null|| string.IsNullOrEmpty(star_info.cost))
            return false;
       
        string[] need_str_arr = star_info.cost.Split(';');
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
    }
}
