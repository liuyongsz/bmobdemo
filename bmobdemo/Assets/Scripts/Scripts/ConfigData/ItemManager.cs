using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyBinaryXml;


    #region  道具总表
public class ItemManager
{
    /// <summary>
    /// 道具字典表
    /// </summary>
    public static Dictionary<string, ItemInfo> itemList = new Dictionary<string, ItemInfo>();


    /// <summary>
    /// 消耗品字典表
    /// </summary>
    private static Dictionary<string, ConsumeItemInfo> consumemList = new Dictionary<string, ConsumeItemInfo>();


    /// <summary>
    /// 材料字典表
    /// </summary>
    public static Dictionary<string, MaterialItemInfo> materialList = new Dictionary<string, MaterialItemInfo>();


    /// <summary>
    /// 游戏商城道具字典表
    /// </summary>
    public static Dictionary<string, ShopItemInfo> shopList = new Dictionary<string, ShopItemInfo>();

    public static void Init()
    {
        ResourceManager.Instance.LoadBytes("Item", AssetBundles.EResType.E_BYTE, LoadItemConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("ConSueItem", AssetBundles.EResType.E_BYTE, LoadConsuleConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("MaterialItem", AssetBundles.EResType.E_BYTE, LoadMaterialConfig, UtilTools.errorload);
        ResourceManager.Instance.LoadBytes("gameShopConfig", AssetBundles.EResType.E_BYTE, LoadShopItemConfig, UtilTools.errorload);
    }

    /// <summary>
    /// 加载道具配置
    /// </summary>
    public static void LoadItemConfig(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;

        if (asset == null)
            return;
        
        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }
        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Object/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return;
        }
        ItemInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ItemInfo();
            info.itemID = node.GetStringValue("ItemId");
            info.itemType = node.GetIntValue("ItemType");
            info.color = node.GetIntValue("Color");
            info.itemName = node.GetStringValue("ItemName");
            info.itemPrice = node.GetIntValue("ItemPrice");
            info.tabType = node.GetIntValue("TabType");
            info.isTogether = node.GetIntValue("isTogethe");
            info.qualityOrder = node.GetIntValue("worthValue");
            if (itemList.ContainsKey(info.itemID))
            {
                itemList[info.itemID] = info;
            }
            else
            {
                itemList.Add(info.itemID, info);
            }
        }
        asset = null;      
    }

    /// <summary>
    /// 加载消耗品配置
    /// </summary>
    public static void LoadConsuleConfig(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;

        if (asset == null)
            return;

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }
        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Object/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return;
        }
        ConsumeItemInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ConsumeItemInfo();
            info.itemID = node.GetStringValue("id");
            info.propValue = node.GetIntValue("addValue");
            if (consumemList.ContainsKey(info.itemID))
            {
                consumemList[info.itemID] = info;
            }
            else
            {
                consumemList.Add(info.itemID, info);
            }
        }
        asset = null;
    }

    /// <summary>
    /// 加载材料配置
    /// </summary>
    public static void LoadMaterialConfig(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;

        if (asset == null)
            return;

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }
        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Object/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return;
        }
        MaterialItemInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new MaterialItemInfo();
            info.itemID = node.GetStringValue("ID");
            info.needAmount = node.GetIntValue("combineCount");
            info.materialId = node.GetIntValue("materialID");
            if (materialList.ContainsKey(info.itemID))
            {
                materialList[info.itemID] = info;
            }
            else
            {
                materialList.Add(info.itemID, info);
            }
        }
        asset = null;
    }

    /// <summary>
    /// 加载商城配置
    /// </summary>
    public static void LoadShopItemConfig(AssetBundles.NormalRes data)
    {
        byte[] asset = (data as AssetBundles.BytesRes).m_bytes;

        if (asset == null)
            return;

        TbXmlNode docNode = TbXml.Load(asset).docNode;
        if (docNode == null)
        {
            return;
        }
        List<TbXmlNode> xmlNodeList = docNode.GetNodes("Object/Property");
        int xmlNodeListLength = xmlNodeList.Count;
        if (xmlNodeListLength < 1)
        {
            return;
        }
        ShopItemInfo info;
        for (int i = 0; i < xmlNodeList.Count; ++i)
        {
            TbXmlNode node = xmlNodeList[i] as TbXmlNode;
            info = new ShopItemInfo();
            info.itemID = node.GetStringValue("shopItemID");
            info.itemType = node.GetIntValue("ItemType");
            info.tabType = node.GetIntValue("TabType");
            info.limitTime = node.GetIntValue("limitTimes");
            info.itemPrice = node.GetIntValue("price");
            info.moneyType = node.GetIntValue("moneyType");
            info.disCount = node.GetIntValue("disCount");
            info.recommend = node.GetIntValue("recommend");
            info.isLimit = node.GetIntValue("isLimit");
            if (shopList.ContainsKey(info.itemID))
            {
                shopList[info.itemID] = info;
            }
            else
            {
                shopList.Add(info.itemID, info);
            }
        }
        asset = null;
    }
    /// <summary>
    /// 获取背包中道具个数
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static int GetBagItemCount(string itemId)
    {
        int count = 0;
        foreach (Item item in BagMediator.ItemList.Values)
        {
            if (item.itemID == itemId)
            {
                count += item.amount;
            }
        }
        return count;
    }
   
    /// <summary>
    /// 获取背包道具信息
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static Item GetBagItemInfo (string itemId)
    {
        foreach (Item item in BagMediator.ItemList.Values)
        {
            if (item.itemID == itemId)
            {
                return item;
            }
        }      
        return null;
    }

    /// <summary>
    /// 通过UUID 获取背包道具信息
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public static Item GetBagItemInfoByUUID(string uuid)
    {

        foreach (Item item in BagMediator.ItemList.Values)
        {
            if (item.uuid == uuid)
            {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取道具信息
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static ItemInfo GetItemInfo(string itemId)
    {
        if (itemList.ContainsKey(itemId))
        {
            return itemList[itemId];
        }
        return null;
    }

    /// <summary>
    /// 获取道具信息
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static ConsumeItemInfo GetConsueItemInfo(string itemId)
    {
        if (consumemList.ContainsKey(itemId))
        {
            return consumemList[itemId];
        }
        return null;
    }

    /// <summary>
    /// 获取材料道具信息
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static MaterialItemInfo GetMaterialInfo(string itemId)
    {
        if (materialList.ContainsKey(itemId))
        {
            return materialList[itemId];
        }
        return null;
    }

    /// <summary>
    /// 获取道具ID
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static string GetMaterialID(string materialID)
    {
        foreach (MaterialItemInfo item in materialList.Values)
        {
            if (item.materialId.ToString() == materialID)
            {
                return item.itemID;
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 获取商城道具
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static ShopItemInfo GetShopItemInfo(string itemID)
    {
        if (shopList.ContainsKey(itemID))
            return shopList[itemID];
        return null;
    }

}
#endregion


    #region 相关类
    public enum ItemType
        {
            Equip = 1,   // 装备
            ConSue = 2,  // 消耗
            Material = 3,// 材料
            Gem = 4,     // 宝石
        }

//背包数据改变类型
public enum BagChangeType
{
    Null = 0,
    Add = 1,
    Update = 2,
    Remove = 3,
}
/// <summary>
/// 道具基类
/// </summary>
public class Item
    {
        public int bagID;
        public string itemID = string.Empty;
        public string uuid;
        public int itemType;
        public int tabType; //页签type
        public int amount;
        public int qualityOrder;
        public int color;
        public int moneyType;
        public int itemPrice;
    }

    /// <summary>
    /// 客户端道具总表
    /// </summary>
    public class ItemInfo : Item
    {
        public string itemName;
        public int isTogether; //是否可以叠加
       
    }

    /// <summary>
    /// 装备类
    /// </summary>
    public class EquipItemInfo : Item
    {
        public int position;    // 部位
        public int shoot;       // 射门
        public int pass;        // 带盘
        public int reel;        // 射门
        public int defend;      // 防守
        public int trick;       // 拦截
        public int steal;       // 抢断
        public int control;     // 控球
        public int keep;        // 守门
        public int suit;        // 套装编号
        public int star;           //x星级
        public int strongLevel;//强化等级
        public int gem1;
        public int gem2;
        public int gem3;

}

    /// <summary>
    /// 装备位置
    /// </summary>
    public enum Equip_Pos
    {
        Null = 0,
        Head = 1,
        Cloth = 2,
        Trousers = 3,
        Legguard=4,
        Shoe = 5,
    }
    //装备筛选类型 
    public enum Equip_Select_Type
    {
        Pos,//位置
        Level,//强化等级
    }
    /// <summary>
    /// 材料类
    /// </summary>
    public class MaterialItemInfo : Item
    {
        public int needAmount;
        public int materialId;
    }

    /// <summary>
    /// 消耗品类
    /// </summary>
    public class ConsumeItemInfo : Item
    {
        public int propValue;   // 加成数值
    }

    /// <summary>
    /// 宝石类
    /// </summary>
    public class GemItemInfo : Item
    {
    public int propType;    // 加成类型
    public int propValue;   // 加成数值
    }

    /// <summary>
    /// 消耗品类
    /// </summary>
    public class ShopItemInfo : Item
    {
        public int limitTime;   // 限制次数
        public int disCount;    // 折扣
        public int recommend;   // 推荐  
        public int isLimit;     // 是否限购 
    }
#endregion