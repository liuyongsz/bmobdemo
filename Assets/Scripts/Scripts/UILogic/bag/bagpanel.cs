using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;


public class bagpanel : BasePanel
{
    public UIButton backBtn;
    public UIToggle equipBtn;
    public UIToggle consueBtn;
    public UIToggle materialBtn;
    public UIToggle gemBtn;
    public UIButton addBagBtn;
    public UIButton batchSellBtn;
    public UIButton oKSellBtn;
    public UIButton cancelSellBtn;
    public UIGrid BagGrid;
    public Transform batchSell;
    public Transform addBagSize;
    public UIButton yesSellBtn;
    public UIButton noSellBtn;
    public UIButton minBtn;
    public UIButton maxBtn;
    public UIButton addBtn;
    public UIButton subtractBtn;
    public UIButton noAddSize;
    public UIButton yesAddSize;
    public UILabel changeNum;
    public UILabel needDiamond;
    public UILabel Price;
    public UILabel bagSize;
    public UIGrid SellGrid;
}

public class BagMediator : UIMediator<bagpanel>
{
    private bagpanel panel
    {
        get
        {
            return m_Panel as bagpanel;
        }
    }

    public Item SeverInfo;
    public ItemInfo  clientInfo;

    public static Dictionary<string, Item> ItemList = new Dictionary<string, Item>();
    private List<Item> bagList = new List<Item>();
    public List<Item> sellList = new List<Item>();

    public static BagMediator bagMediator;

    public static ItemType currentType;

    private bool batchSell = false;

    public static int bagSize = 0;
    private BagSize bagSizeInfo;

    public BagMediator() : base("bagpanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.Bag_Show, OpenPanel);
        RegistPanelCall(NotificationID.Bag_Hide, ClosePanel);
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (bagMediator == null)
        {
            bagMediator = Facade.RetrieveMediator("BagMediator") as BagMediator;
        }
        foreach (Item item in ItemList.Values)
        {
            bagList.Add(item);
        }
        UpdateBagSize();
        bagList.Sort(CompareItem);
        currentType = ItemType.Equip;
        panel.BagGrid.enabled = true;
        bagSizeInfo = BagSizeConfig.GetBagSize(1);
        panel.BagGrid.BindCustomCallBack(UpdateBagGridItem);
        panel.BagGrid.StartCustom();
        panel.BagGrid.AddCustomDataList(AddListGrid(bagList));

        panel.SellGrid.enabled = true;
        panel.SellGrid.BindCustomCallBack(UpdateBatchSellGrid);
        panel.SellGrid.StartCustom();
    }
    public void UpdateBagSize()
    {
        panel.bagSize.text = UtilTools.StringBuilder(ItemList.Count, "/", bagSize);
    }
    /// <summary>
    /// 释放
    /// </summary>
    protected override void OnDestroy()
    {
        batchSell = false;
        bagList.Clear();
        panel.backBtn = null;
        panel.equipBtn = null;
        panel.consueBtn = null;
        panel.materialBtn = null;
        panel.gemBtn = null;
        panel.addBagBtn = null;
        panel.batchSellBtn = null;
        panel.oKSellBtn = null;
        panel.cancelSellBtn = null;
        panel.BagGrid = null;
        panel.batchSell = null;
        panel.addBagSize = null;
        panel.yesSellBtn = null;
        panel.noSellBtn = null;
        panel.minBtn = null;
        panel.maxBtn = null;
        panel.addBtn = null;
        panel.subtractBtn = null;
        panel.noAddSize = null;
        panel.yesAddSize = null;
        panel.changeNum = null;
        panel.needDiamond = null;
        panel.Price = null;
        panel.bagSize = null;
        panel.SellGrid = null;
        bagMediator = null;
        base.OnDestroy();
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.equipBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.consueBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.materialBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.gemBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.addBagBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.batchSellBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.oKSellBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.cancelSellBtn.gameObject).onClick = OnClick;    
        UIEventListener.Get(panel.yesSellBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.noSellBtn.gameObject).onClick = OnClick;
        LongClickEvent.Get(panel.addBtn.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.addBtn.gameObject).duration = 2;
        LongClickEvent.Get(panel.subtractBtn.gameObject).onPress = OnPress;
        LongClickEvent.Get(panel.subtractBtn.gameObject).duration = 2;
        UIEventListener.Get(panel.minBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.maxBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.yesAddSize.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.noAddSize.gameObject).onClick = OnClick;
    }
    private void OnClick(GameObject go)
    {
        if (go == panel.equipBtn.gameObject)
        {
            if (currentType != ItemType.Equip)
            {
                batchSell = false;
                SellBtn(batchSell);
                sellList.Clear();
                currentType = ItemType.Equip;
                panel.BagGrid.AddCustomDataList(AddListGrid(bagList));
            }
        }
        else if (go == panel.consueBtn.gameObject)
        {
            if (currentType != ItemType.ConSue)
            {
                batchSell = false;
                SellBtn(batchSell);
                sellList.Clear();
                currentType = ItemType.ConSue;
                panel.BagGrid.AddCustomDataList(AddListGrid(bagList));
            }
        }
        else if (go == panel.materialBtn.gameObject)
        {
            if (currentType != ItemType.Material)
            {
                batchSell = false;
                SellBtn(batchSell);
                sellList.Clear();
                currentType = ItemType.Material;
                panel.BagGrid.AddCustomDataList(AddListGrid(bagList));
            }
        }
        else if (go == panel.gemBtn.gameObject)
        {
            if (currentType != ItemType.Gem)
            {
                batchSell = false;
                SellBtn(batchSell);
                sellList.Clear();
                currentType = ItemType.Gem;
                panel.BagGrid.AddCustomDataList(AddListGrid(bagList));
            }
        }
        else if (go == panel.batchSellBtn.gameObject)
        {
            if (bagList.Count < 1)
                GUIManager.SetPromptInfo(TextManager.GetUIString("UIBag"), null);
            else
            {
                batchSell = true;
                sellList.Clear();
                SellBtn(batchSell);
                panel.BagGrid.UpdateCustomDataList(AddListGrid(bagList));
            }
        }
        else if (go == panel.oKSellBtn.gameObject)
        {
            if (sellList.Count <= 0)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UIBag1"), SetBtn);
                return;
            }
            batchSell = false;
            SellBtn(batchSell);
            panel.batchSell.gameObject.SetActive(true);
            List<object> listObj = new List<object>(); ;
            int Price = 0;
            for (int i = 0; i < sellList.Count; i++)
            {
                listObj.Add(sellList[i] as object);
                ItemInfo item = ItemManager.GetItemInfo((sellList[i] as Item).itemID);
                Price += item.itemPrice * (sellList[i] as Item).amount;
            }
            panel.Price.text = string.Format(TextManager.GetUIString("UIBatchPrice"), Price);
            panel.SellGrid.AddCustomDataList(listObj);
        }
        else if (go == panel.yesSellBtn.gameObject)
        {
            List<object> baguuid = new List<object>();
            for (int i = 0; i < sellList.Count; ++i)
            {
                baguuid.Add(UInt64.Parse((sellList[i] as Item).uuid));
                for (int j = bagList.Count - 1; j >= 0; j--)
                {
                    if (bagList[j].uuid == (sellList[i] as Item).uuid)
                    {
                        bagList.Remove(bagList[j]);
                    }
                }
            }
            sellList.Clear();
            panel.BagGrid.AddCustomDataList(AddListGrid(bagList));
            ServerCustom.instance.SendClientMethods("onClientSellBatch", baguuid);
            panel.batchSell.gameObject.SetActive(false);
        }
        else if (go == panel.noSellBtn.gameObject)
        {
            batchSell = false;
            SellBtn(batchSell);
            panel.BagGrid.AddCustomDataList(AddListGrid(bagList));
            panel.batchSell.gameObject.SetActive(false);
        }
        else if (go == panel.cancelSellBtn.gameObject)
        {
            batchSell = false;
            SellBtn(batchSell);
            sellList.Clear();
            panel.BagGrid.AddCustomDataList(AddListGrid(bagList));
        }
        else if (go == panel.backBtn.gameObject)
        {
            Facade.SendNotification(NotificationID.Bag_Hide);
        }
        else if (go == panel.addBagBtn.gameObject)
        {
            panel.addBagSize.gameObject.SetActive(true);
            panel.changeNum.text = "1";
            panel.needDiamond.text = string.Format(TextManager.GetUIString("UIBasize1"), bagSizeInfo.needDimaond);
        }
        else if (go == panel.minBtn.gameObject)
        {
            panel.changeNum.text = "1";
            panel.needDiamond.text = string.Format(TextManager.GetUIString("UIBasize1"), bagSizeInfo.needDimaond);
        }
        else if (go == panel.maxBtn.gameObject)
        {
            panel.changeNum.text = bagSizeInfo.maxSize.ToString();
            panel.needDiamond.text = string.Format(TextManager.GetUIString("UIBasize1"), bagSizeInfo.maxSize * bagSizeInfo.needDimaond).ToString();
        }
        else if (go == panel.yesAddSize.gameObject)
        {
            // 背包扩容
            panel.addBagSize.gameObject.SetActive(false);
            ServerCustom.instance.SendClientMethods("onClientBuyBagSize", UtilTools.IntParse(panel.changeNum.text));
        }
        else if (go == panel.noAddSize.gameObject)
        {
            panel.addBagSize.gameObject.SetActive(false);
        }
    }
    void OnPress(GameObject go, bool pressed)
    {
        if (go == panel.addBtn.gameObject)
        {
            LongClickEvent.Get(panel.addBtn.gameObject).time = 0;
            if (panel.changeNum.text == "999")
                return;
            panel.changeNum.text = (UtilTools.IntParse(panel.changeNum.text) + 1).ToString();           
        }
        else
        {
            LongClickEvent.Get(panel.subtractBtn.gameObject).time = 0;
            if (panel.changeNum.text == "1")
                return;
            panel.changeNum.text = (UtilTools.IntParse(panel.changeNum.text) - 1).ToString();
        }
        panel.needDiamond.text = string.Format(TextManager.GetUIString("UIBasize1"), UtilTools.IntParse(panel.changeNum.text) * bagSizeInfo.needDimaond);
    }
    void UpdateBagGridItem(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        Item info = item.oData as Item;
        item.onClick = OnClickItem;
        UISprite color = item.mScripts[0] as UISprite;
        UITexture itemIcon = item.mScripts[1] as UITexture;
        UILabel Name = item.mScripts[2] as UILabel;
        UILabel num = item.mScripts[3] as UILabel;
        UISprite gou = item.mScripts[4] as UISprite;
        UISprite sprite = item.mScripts[5] as UISprite;
        sprite.gameObject.SetActive(false);
        Name.gameObject.SetActive(info.itemID != string.Empty);
        num.gameObject.SetActive(info.itemID != string.Empty);
        if (info.itemID != string.Empty)
        {
            gou.gameObject.SetActive(batchSell);
            color.gameObject.SetActive(true);
            Name.gameObject.SetActive(true);
            num.gameObject.SetActive(true);
            itemIcon.gameObject.SetActive(true);
            ItemInfo data = ItemManager.GetItemInfo(info.itemID);
            int m_color = data.color;
            if(currentType == ItemType.Equip)
            {
                EquipItemInfo equip = EquipConfig.GetEquipDataByUUID(info.uuid);
                if (equip != null)
                    m_color = equip.star;
            }
            color.spriteName = UtilTools.StringBuilder("color", m_color);
            Name.text = TextManager.GetItemString(info.itemID);
            num.text = info.amount.ToString();
            LoadSprite.LoaderItem(itemIcon,info.itemID);
        }
        else
        {
            gou.gameObject.SetActive(false);
            color.gameObject.SetActive(false);
            Name.gameObject.SetActive(false);
            num.gameObject.SetActive(false);
            itemIcon.gameObject.SetActive(false);
        }
    }

    void UpdateBatchSellGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        Item info = item.oData as Item;
        UISprite color = item.mScripts[0] as UISprite;
        UITexture itemIcon = item.mScripts[1] as UITexture;
        UILabel Name = item.mScripts[2] as UILabel;
        UILabel num = item.mScripts[3] as UILabel;
        UISprite gou = item.mScripts[4] as UISprite;
        gou.gameObject.SetActive(false);
        ItemInfo data = ItemManager.GetItemInfo(info.itemID);
        color.spriteName = UtilTools.StringBuilder("color", info.color);
        LoadSprite.LoaderItem(itemIcon, info.itemID);
        Name.text = TextManager.GetItemString(info.itemID);
        num.text = info.amount.ToString();
    }
    /// <summary>
    /// 点击背包中的道具
    /// </summary>
    void OnClickItem(UIGridItem item)
    {
        Item info = item.oData as Item;
        if (info.itemID == string.Empty)
            return;
        if (batchSell)
        {
            GameObject itemObj = item.mScripts[5].gameObject;
            itemObj.SetActive(!itemObj.activeSelf);
            if (itemObj.activeSelf)
                sellList.Add(info);
            else
                sellList.Remove(info);
            itemObj = null;
            return;
        }
        GUIManager.ShowItemInfoPanel(PanelType.Info, info.itemID, info.uuid, SellItem, UseItem);
    }

    /// <summary>
    /// 出售
    /// </summary>
    void SellItem(Item info)
    {
        SeverInfo = info;
        if (info.amount == 1)
        {
            SellOneItemOK(info.amount);
            Facade.SendNotification(NotificationID.ItemInfo_Hide);
            return;
        }          
        ShowItemInfo showItemInfo = new ShowItemInfo();
        ItemMediator.panelType = PanelType.Sell;
        showItemInfo.sellOne = SellOneItemOK;
        List<object> list = new List<object>();
        list.Add(showItemInfo);
        list.Add(info);
        if (info == null)
        {
            return;
        }
        Facade.SendNotification(NotificationID.ItemInfo_Show, list);     
    }

    /// <summary>
    /// 确认出售
    /// </summary>
    void SellOneItemOK(int count)
    {
        ServerCustom.instance.SendClientMethods("onClientSellOne",Int64.Parse(SeverInfo.uuid), Int32.Parse(count.ToString()));
        for (int j = bagList.Count - 1; j >= 0; j--)
        {
            if (bagList[j].uuid == SeverInfo.uuid)
            {
                if (Int32.Parse(count.ToString()) < SeverInfo.amount)
                {
                    bagList[j].amount -= Int32.Parse(count.ToString());
                    panel.BagGrid.UpdateCustomData(bagList[j]);
                }
                else
                {
                    panel.BagGrid.DeleteCustomData(bagList[j], true);
                    bagList.Remove(bagList[j]);
                                  
                }
            }
        }
    }

    /// <summary>
    /// 使用
    /// </summary>
    void UseItem(Item info)
    {
        SeverInfo = info;
        ShowItemInfo showItemInfo = new ShowItemInfo();
        ItemMediator.panelType = PanelType.Use;
        showItemInfo.useOne = UseOneItem;
        List<object> list = new List<object>();
        list.Add(showItemInfo);
        list.Add(info);
        if (info == null)
        {
            return;
        }
        Facade.SendNotification(NotificationID.ItemInfo_Show, list);
    }

    /// <summary>
    /// 使用
    /// </summary>
    void UseOneItem(Item info)
    {
       
    }
    void SetBtn()
    {
        batchSell = false;
        SellBtn(batchSell);
    }
    void SellBtn(bool isSell)
    {
        panel.oKSellBtn.gameObject.SetActive(isSell);
        panel.cancelSellBtn.gameObject.SetActive(isSell);
        panel.addBagBtn.gameObject.SetActive(!isSell);
        panel.batchSellBtn.gameObject.SetActive(!isSell);
    }
    public void AddBagSizeSucess(int size)
    {
        for (int i = 0; i < size - bagSize; i++)
        {
            Item item = new Item();
            item.bagID = bagSize + 1 + i;
            item.itemID = string.Empty;
            panel.BagGrid.AddCustomData(item);
            panel.BagGrid.UpdateCustomData(item);
        }
        bagSize = size;
        UpdateBagSize();
    }
    List<object> AddListGrid(List<Item> list)
    {
        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; ++i)
        {
            ItemInfo info = ItemManager.GetItemInfo(list[i].itemID);
            list[i].bagID = 1;
            if (info.tabType == (int)currentType)
            {
                listObj.Add(list[i]);
            }
        }
        for (int i = listObj.Count; i < bagSize; ++i)
        {
            Item item = new Item();
            item.bagID = i;
            item.itemID = string.Empty;
            listObj.Add(item);
        }
        return listObj;
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
}
