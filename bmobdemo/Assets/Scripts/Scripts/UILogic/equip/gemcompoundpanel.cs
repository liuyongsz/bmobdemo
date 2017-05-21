using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

public class gemcompoundpanel : BasePanel
{
    public Transform backBtn;
    public Transform compoundBtn;
    public Transform info;
    public Transform next;
    public UIGrid chooseGrid;
    public UIGrid costGrid;
}
public class CompoundInfo
{
    public Item m_gem = null;
    public int m_select;
}


public class GemCompoundMediator : UIMediator<gemcompoundpanel>
{

    public static GemCompoundMediator gemCompoundMediator;

    private Item select_gem;
    private GemItem mCurGem;
    //选中的装备
    private long select_equip_id;
    //球员ＩＤ
    private int player_id;
    //第几个孔
    private int m_index;
    private List<Item> bagList = new List<Item>();
    private List<object> compound_list = new List<object>();
    private gemcompoundpanel panel
    {
        get
        {
            return m_Panel as gemcompoundpanel;
        }
    }
    public GemCompoundMediator() : base("gemcompoundpanel")
    {
        m_isprop = true;
    
        RegistPanelCall(NotificationID.GemCompound_Show, OpenPanel);
        RegistPanelCall(NotificationID.GemCompound_Hide, ClosePanel);

        RegistPanelCall(NotificationID.BagRefresh, GemRefresh);

    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.compoundBtn.gameObject).onClick = OnClick;

    }
    /// <summary>
    /// 刷新可能合成晶体
    /// </summary>
    /// <param name="notification"></param>
    private void GemRefresh(INotification notification)
    {
        if(GUIManager.HasView("gemcompoundpanel"))
        {

            int type = GameConvert.IntConvert((notification.Body as List<object>)[0]);
            Item data = (notification.Body as List<object>)[1] as Item;
            Item item = GetListItemData(data);
            if (item == null) return;

            BagChangeType change_type = (BagChangeType)Enum.Parse(typeof(Equip_Pos), (type).ToString());

            ItemInfo info = ItemManager.GetItemInfo(item.itemID);
            if (info.tabType != (int)ItemType.Gem)
                return;
            if (item.amount < 3 || change_type == BagChangeType.Remove)
                panel.chooseGrid.DeleteCustomData(item, true);
            else if (change_type == BagChangeType.Update && item.amount > 2)
                panel.chooseGrid.UpdateCustomData(item);
            else if (change_type == BagChangeType.Add)
            {
                panel.chooseGrid.AddCustomData(item);
                panel.chooseGrid.UpdateCustomView();
            }
        }
    }

    private Item GetListItemData(Item item)
    {
        for(int i=0; i < panel.chooseGrid.mDataSource.Count; i++)
        {
            Item data = panel.chooseGrid.mDataSource[i] as Item;
            if (data.uuid == item.uuid)
            {
                data.amount = item.amount;
                return data;
            }
        }
        return null;

    }
    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (gemCompoundMediator == null)
        {
            gemCompoundMediator = Facade.RetrieveMediator("GemCompoundMediator") as GemCompoundMediator;
        }
      
        panel.chooseGrid.enabled = true;
        panel.chooseGrid.BindCustomCallBack(ChooseGrid_UpdateItem);
        panel.chooseGrid.StartCustom();

        panel.costGrid.enabled = true;
        panel.costGrid.BindCustomCallBack(CostUpdataItem);
        panel.costGrid.StartCustom();

        panel.costGrid.AddCustomDataList(CostListGrid());

        SetInfo();
        SetGem();
    }
    /// <summary>
    /// 设置一信息
    /// </summary>
    public void SetInfo()
    {
        if (null == m_Panel) return;
        GetGemList();
        compound_list = AddListGrid(bagList);
        panel.chooseGrid.AddCustomDataList(compound_list);
        
    }
    private void SetGem()
    {
        GemItem gem = select_gem == null ? null : GemItemConfig.GetGemItem(int.Parse(select_gem.itemID));
        GemCompound compound = select_gem == null ? null : GemCompoundConfig.GetGemCompound(int.Parse(select_gem.itemID));
        GemItem compoundGem = compound==null?null: GemItemConfig.GetGemItem(compound.compoundId);


        string name = select_gem == null ? TextManager.GetUIString("UI2065") : TextManager.GetItemString(select_gem.itemID);
        string prop_name = gem == null ? " " : TextManager.GetUIString(gem.propType);
        string prop_value = gem == null ? " " : gem.propValue.ToString();
        string prop_value1 = compoundGem == null ? " " : compoundGem.propValue.ToString();

        panel.next.gameObject.SetActive(gem!=null&&gem.level < 9);
        panel.compoundBtn.gameObject.SetActive(gem != null && gem.level < 9);

        panel.info.FindChild("gem_name").GetComponent<UILabel>().text = name;
        panel.info.FindChild("prop_name").GetComponent<UILabel>().text = prop_name;
        panel.next.FindChild("prop_name1").GetComponent<UILabel>().text = prop_name;
        panel.next.FindChild("prop_name2").GetComponent<UILabel>().text = prop_name;

        panel.info.FindChild("prop_value").GetComponent<UILabel>().text = prop_value;
        panel.next.FindChild("prop_value1").GetComponent<UILabel>().text = prop_value;
        panel.next.FindChild("prop_value2").GetComponent<UILabel>().text = prop_value1;

        panel.info.FindChild("descTxt3").gameObject.SetActive(compound_list.Count == 0);
        panel.info.FindChild("descTxt4").gameObject.SetActive(gem!=null&&gem.level == 9);

    }
    private void ChooseGrid_UpdateItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        Item info = item.oData as Item;
        item.onClick = ClickGemItem;

        UILabel name = item.mScripts[0] as UILabel;
        UITexture icon = item.mScripts[1] as UITexture;
        UILabel level = item.mScripts[2] as UILabel;
        UISprite color = item.mScripts[3] as UISprite;

        ItemInfo item_info = ItemManager.GetItemInfo(info.itemID);
        GemItem gem = GemItemConfig.GetGemItem(int.Parse(info.itemID));
        if (item_info == null)
            return;
        color.spriteName = "color" + item_info.color;

        name.text = TextManager.GetItemString(info.itemID);
        level.text = info.amount.ToString();
        LoadSprite.LoaderItem(icon, item_info.itemID, false);
        
    }


    private void CostUpdataItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        CompoundInfo info = item.oData as CompoundInfo;

        UISprite color = item.mScripts[0] as UISprite;
        UITexture icon = item.mScripts[1] as UITexture;
        UILabel num = item.mScripts[2] as UILabel;
        UISprite add = item.mScripts[3] as UISprite;
        color.spriteName = "color" + 1;
        add.transform.gameObject.SetActive(info.m_select == 0);
        item.transform.FindChild("contain").gameObject.SetActive(info.m_select == 1);

        if(info.m_gem!=null)
        {
            GemItem gem = GemItemConfig.GetGemItem(int.Parse(info.m_gem.itemID));

            ItemInfo item_info = ItemManager.GetItemInfo(info.m_gem.itemID);
            num.text = "1";

            color.spriteName = "color" + item_info.color;
            LoadSprite.LoaderItem(icon, item_info.itemID, false);

        }



    }

    private void OnClick(GameObject go)
    {
        if (select_gem == null)
        {
            GUIManager.SetPromptInfo(TextManager.GetUIString("UI2066"), null);
            return;
        }
        GemCompound gem = GemCompoundConfig.GetGemCompound(int.Parse(select_gem.itemID));
        if(gem == null)
            return;
            
        ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_COMPOUND_GEM, GameConvert.LongConvert(select_gem.uuid));
        
    }
    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickGemItem(UIGridItem data)
    {
        Item gem = data.oData as Item;

        if (select_gem != null && gem.uuid == select_gem.uuid)
            return;
        else
            select_gem = gem;

        if (select_gem == null)
            panel.chooseGrid.OldSelectedItem.Selected = false;

        mCurGem = select_gem == null ? null : GemItemConfig.GetGemItem(int.Parse(select_gem.itemID));

        panel.costGrid.AddCustomDataList(CostListGrid());
        SetGem();
    }


    private List<object> AddListGrid(List<Item> list)
    {
        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; ++i)
        {
            ItemInfo info = ItemManager.GetItemInfo(list[i].itemID);
           
            list[i].bagID = 1;
            if (info.tabType == (int)ItemType.Gem&& list[i].amount>2)
            {
                GemCompound compound = GemCompoundConfig.GetGemCompound(int.Parse(list[i].itemID));
                if (compound != null)
                    listObj.Add(list[i]);
            }
        }
        return listObj;
    }

    private List<object> CostListGrid()
    {
        List<object> listObj = new List<object>();
        CompoundInfo info = null;
        int amount = 0;
        if (mCurGem != null)
        {
            amount = mCurGem.level < 9 ? 3 : 0;
        }
        for(int i=0; i< amount; i++)
        {
            info = new CompoundInfo();
            info.m_gem = select_gem;

            info.m_select = select_gem == null ? 0 : 1;

            listObj.Add(info);
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
    public int CompareItem(object x, object y)
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
                ItemInfo a = ItemManager.GetItemInfo((x as Item).itemID);
                ItemInfo b = ItemManager.GetItemInfo((y as Item).itemID);
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
        select_gem = null;
    }

}