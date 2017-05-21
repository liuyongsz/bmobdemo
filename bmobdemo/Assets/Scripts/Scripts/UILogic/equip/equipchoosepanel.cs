using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;

//装备筛选数据
public class EquipChooseData
{

    public Equip_Pos pos = Equip_Pos.Null;
    public Equip_Select_Type type = Equip_Select_Type.Pos;
    public int select_level = 0;
    public int player_id = 0;

    public  EquipChooseData(int id, Equip_Select_Type type, Equip_Pos pos, int level)
    {
        player_id = id;
        this.type = type;
        this.pos = pos;
        this.select_level = level;
    }
}

public class equipchoosepanel : BasePanel
{
    public Transform equipChooseItem;
    public Transform closeBtn;
    public UIGrid chooseGrid;
}



public class EquipChooseMediator : UIMediator<equipchoosepanel>
{
    public delegate void SelectItemHandle(EquipItemInfo info);

    public static SelectItemHandle SelectItem;

    public static EquipChooseMediator equipChooseMediator;
    //选中的装备
    public  EquipItemInfo select_equip;
    //球员ＩＤ
    private int player_id;
    private int select_level;
    //位置
    private Equip_Pos select_pos = Equip_Pos.Null;
    private Equip_Select_Type select_type = Equip_Select_Type.Pos;
    private EquipChooseData select_data = null;
    private equipchoosepanel panel
    {
        get
        {
            return m_Panel as equipchoosepanel;
        }
    }
    public EquipChooseMediator() : base("equipchoosepanel")
    {
        m_isprop = true;
    

        RegistPanelCall(NotificationID.EquipChoose_Show, OpenPanel);
        RegistPanelCall(NotificationID.EquipChoose_Hide, ClosePanel);
    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;
        GameEventManager.RegisterEvent(GameEventTypes.EquipRefresh, OnRefreshEquip);
        
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (equipChooseMediator == null)
        {
            equipChooseMediator = Facade.RetrieveMediator("EquipChooseMediator") as EquipChooseMediator;
        }
        select_data = (notification.Body as List<object>)[0] as EquipChooseData;
        player_id = select_data.player_id;
        select_pos = select_data.pos;
        select_type = select_data.type;
        select_level = select_data.select_level;

        panel.chooseGrid.enabled = true;
        panel.chooseGrid.BindCustomCallBack(ChooseGrid_UpdateItem);
        panel.chooseGrid.StartCustom();

        SetInfo();
    }
    /// <summary>
    /// 设置一键强化信息
    /// </summary>
    public void SetInfo()
    {
        if (null == m_Panel) return;

        List<object> listObj = new List<object>();
        if (!EquipConfig.m_player_eqiup.ContainsKey(0))
            return;
        List<EquipItemInfo> equipList = null;


        equipList = GetEquipList();

        equipList.Sort(CompareEquip);
        for (int i = 0; i < equipList.Count; i++)
        {
            listObj.Add(equipList[i]);
        }
       
      
        panel.chooseGrid.AddCustomDataList(listObj);
    }

    private void ChooseGrid_UpdateItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        EquipItemInfo info = item.oData as EquipItemInfo;
        item.onClick = ClickEquipItem;

        UILabel name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture icon = item.mScripts[2] as UITexture;

        UILabel level = item.mScripts[7] as UILabel;
        UITexture star = item.mScripts[8] as UITexture;
        UISprite[] equip_star = UtilTools.GetChilds<UISprite>(item.transform, "star");

        ItemInfo item_info = ItemManager.GetItemInfo(info.itemID.ToString());
        if (item_info == null)
            return;

        level.text = info.strongLevel.ToString();
        name.text = TextManager.GetItemString(info.itemID);
        color.spriteName = "color" + info.star;
        LoadSprite.LoaderItem(icon, item_info.itemID, false);

        UtilTools.SetStar(info.star, equip_star);

    }


    private void OnClick(GameObject go)
    {
        Facade.SendNotification(NotificationID.EquipChoose_Hide);
    }
    /// <summary>
    /// 点击装备
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickEquipItem(UIGridItem data)
    {


        select_equip = data.oData as EquipItemInfo;
        if (SelectItem != null)
            SelectItem(select_equip);
        else
            ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_EQUIP_PUT_ON, player_id, GameConvert.LongConvert(select_equip.uuid));
        
        ClosePanel(null);
    }
    private void OnRefreshEquip(GameEventTypes eventType, object[] args)
    {
        if (GameConvert.IntConvert(args[0]) == 0)
            SetInfo();
    }

    private List<EquipItemInfo> GetEquipList()
    {
        if(select_type == Equip_Select_Type.Pos)
            return EquipConfig.GetEquipDataListByPos((int)select_pos);
        else if(select_type == Equip_Select_Type.Level)
            return EquipConfig.GetEquipDataListByLv(select_level);

        return null;
    }
    //装备按照位置排序
    private int CompareEquip(EquipItemInfo info1, EquipItemInfo info2)
    {
      
        ItemInfo equip1 = ItemManager.GetItemInfo(info1.itemID);
        ItemInfo equip2 = ItemManager.GetItemInfo(info2.itemID);

        if (equip1.qualityOrder > equip2.qualityOrder)
            return 1;
        else if (equip1.qualityOrder < equip2.qualityOrder)
            return -1;
        else if (info1.star < info2.star)
            return 1;
        else if (info1.star > info2.star)
            return -1;
        else if (info1.strongLevel < info2.strongLevel)
            return 1;
        else if (info1.strongLevel > info2.strongLevel)
            return -1;
        else
            return 0;

    }
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        select_equip = null;
        SelectItem = null;
        GameEventManager.RegisterEvent(GameEventTypes.EquipRefresh, OnRefreshEquip);
        select_pos = Equip_Pos.Null;
    }

}