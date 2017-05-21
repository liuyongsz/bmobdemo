using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class gemchoosepanel : BasePanel
{
    public Transform equipChooseItem;
    public Transform closeBtn;
    public UIGrid chooseGrid;
}



public class GemChooseMediator : UIMediator<gemchoosepanel>
{

    public static GemChooseMediator gemChooseMediator;
    //选中的装备
    private long select_equip_id;
    //球员ＩＤ
    private int player_id;
    //第几个孔
    private int m_index;
    private List<Item> bagList = new List<Item>();

    private gemchoosepanel panel
    {
        get
        {
            return m_Panel as gemchoosepanel;
        }
    }
    public GemChooseMediator() : base("gemchoosepanel")
    {
        m_isprop = true;
    
        RegistPanelCall(NotificationID.GemChoose_Show, OpenPanel);
        RegistPanelCall(NotificationID.GemChoose_Hide, ClosePanel);
        RegistPanelCall(NotificationID.BagRefresh, GemRefresh);

    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;
        
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (gemChooseMediator == null)
        {
            gemChooseMediator = Facade.RetrieveMediator("GemChooseMediator") as GemChooseMediator;
        }
        player_id = GameConvert.IntConvert((notification.Body as List<object>)[0]);
        select_equip_id = GameConvert.LongConvert((notification.Body as List<object>)[1]);
        m_index = GameConvert.IntConvert((notification.Body as List<object>)[2]);

        panel.chooseGrid.enabled = true;
        panel.chooseGrid.BindCustomCallBack(ChooseGrid_UpdateItem);
        panel.chooseGrid.StartCustom();

        SetInfo();
    }
    /// <summary>
    /// 设置一信息
    /// </summary>
    public void SetInfo()
    {
        if (null == m_Panel) return;
        GetGemList();
        panel.chooseGrid.AddCustomDataList(AddListGrid(bagList));
        
    }

    private void ChooseGrid_UpdateItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        Item info = item.oData as Item;
        item.onClick = ClickGemItem;

        UILabel name = item.mScripts[0] as UILabel;
        UITexture icon = item.mScripts[1] as UITexture;
        UILabel num = item.mScripts[2] as UILabel;

        name.text = TextManager.GetItemString(info.itemID);
        num.text = info.amount.ToString();

        ItemInfo item_info = ItemManager.GetItemInfo(info.itemID.ToString());
        if (item_info == null)
            return;

    
        LoadSprite.LoaderItem(icon, item_info.itemID, false);



    }


    private void OnClick(GameObject go)
    {
        Facade.SendNotification(NotificationID.GemChoose_Hide);
    }
    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickGemItem(UIGridItem data)
    {
        Item info = data.oData as Item;
        bool is_has= IsHasEquiped(int.Parse(info.itemID));
        if(is_has)
        {
            GUIManager.SetPromptInfo(TextManager.GetUIString("UI2060"), null);
            return;
        }
        ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_GEM_INSET, m_index, player_id, select_equip_id,int.Parse(info.itemID));

    }
    private void  GemRefresh(INotification notification)
    {
        if (GUIManager.HasView("gemchoosepanel"))
        {
            SetInfo();
        }
    }

    List<object> AddListGrid(List<Item> list)
    {

        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; ++i)
        {
            ItemInfo info = ItemManager.GetItemInfo(list[i].itemID);
            list[i].bagID = 1;
            if (info.tabType == (int)ItemType.Gem)
            {
                bool isHas = IsHasEquiped(int.Parse(info.itemID));
                if (!isHas)
                    listObj.Add(list[i]);
            }
        }
        return listObj;
    }
    private void GetGemList()
    {
        bagList.Clear();
        foreach (Item item in BagMediator.ItemList.Values)
        {
            bagList.Add(item);
        }
        bagList.Sort(CompareItem);
    }
    /// <summary>
    /// 排序
    /// </summary>
    public int CompareItem(Item x, Item y)
    {
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                ItemInfo a = ItemManager.GetItemInfo(x.itemID);
                ItemInfo b = ItemManager.GetItemInfo(y.itemID);
                return a.qualityOrder.CompareTo(b.qualityOrder);
            }
        }
    }
    /// <summary>
    /// 是否已经安装
    /// </summary>
    /// <param name="gem_id"></param>
    /// <returns></returns>
    private bool IsHasEquiped(int gem_id)
    {
        List<HoldInfo> list = EquipInsetMediator.equipInsetMediator.m_holdInfo_list;
        GemItem gem_info = GemItemConfig.GetGemItem(gem_id);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].m_gem_id <= 0)
                continue;
            GemItem info = GemItemConfig.GetGemItem(list[i].m_gem_id);
            if (info .propType== gem_info.propType)
                return true;
        }
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