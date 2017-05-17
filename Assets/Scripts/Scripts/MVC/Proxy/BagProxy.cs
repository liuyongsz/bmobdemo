using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

/// <summary>
/// 背包
/// </summary>
public class BagProxy : Proxy<BagProxy>
{

    public BagProxy()
        : base(ProxyID.Bag)
    {
        KBEngine.Event.registerOut(this, "set_bagSize");
        KBEngine.Event.registerOut(this, "onGetItemList");
        KBEngine.Event.registerOut(this, "onGetItemInfo");
    }
    public void set_bagSize(KBEngine.Entity entity, object obj)
    {
        obj = entity.getDefinedProperty("bagSize");
        if (BagMediator.bagMediator != null)
            BagMediator.bagMediator.AddBagSizeSucess(int.Parse(obj.ToString()));
        else
            BagMediator.bagSize = int.Parse(obj.ToString());
    }

    public void onGetItemList(List<object> list)
    {
        BagMediator.ItemList.Clear();
        Item info = null;
        for (int i = 0; i < list.Count; ++i)
        {
            info = new Item();
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            info.uuid = Info["UUID"].ToString();
            info.itemID = Info["itemID"].ToString();
            info.amount = int.Parse(Info["amount"].ToString());
            if (BagMediator.ItemList.ContainsKey(info.uuid))
                BagMediator.ItemList[info.uuid] = info;
            else
                BagMediator.ItemList.Add(info.uuid, info);
        }
    }
    public void onGetItemInfo(Dictionary<string,object> list)
    {
        Item info = new Item();      
        info.uuid = list["UUID"].ToString();
        info.itemID = list["itemID"].ToString();
        info.amount = int.Parse(list["amount"].ToString());
        BagChangeType type = BagChangeType.Null;
           
        if (BagMediator.ItemList.ContainsKey(info.uuid))
        {
            if (info.amount == 0)
            {
                BagMediator.ItemList.Remove(info.uuid);
                type = BagChangeType.Remove;
            }
            else
            {
                BagMediator.ItemList[info.uuid] = info;
                type = BagChangeType.Update;

            }
        }
        else
        {
            BagMediator.ItemList.Add(info.uuid, info);
            type = BagChangeType.Add;

        }
        if (BagMediator.bagMediator != null)
            BagMediator.bagMediator.UpdateBagSize();

        //刷新背包装备数据
        ItemInfo item = ItemManager.GetItemInfo(info.itemID);
        if (item.tabType == (int)ItemType.Equip)
            EquipProxy.Instance.GetquipList(0);
        
        List<object> bag_data = new List<object>();
        bag_data.Add(type);
        bag_data.Add(info);

        Facade.SendNotification(NotificationID.BagRefresh, bag_data);
    }
}
