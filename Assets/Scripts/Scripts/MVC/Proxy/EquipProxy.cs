using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;
using KBEngine;
/// <summary>
/// 装备
/// </summary>
public class EquipProxy : Proxy<EquipProxy>
{
    //装备制作成功
    public static string SERVER_MAKE_EQUIP_SUCC = "makeEquipSucc";
    //穿戴装备成功
    public static string SERVER_PUT_ON_EQUIP_SUCCC = "putOnEquipSucc";
    //客户端请求制作
    public static  string CLINET_MAKE_EQUIP = "onClientMakeEquip";
    /// <summary>
    /// 客户端请求装备星级和强化等级
    /// </summary>
    public static string CLIENT_EQUIP_STAR = "onClientGetEquipStar";
    /// <summary>
    /// 客户端请求装备列表
    /// </summary>
    public static string CLIENT_Get_EQUIP_LIST = "onClientGetEquipList";
    /// <summary>
    ///客户端请求装备升星
    /// </summary>
    public static string CLIENT_BAG_EQUIP_UP_STAR = "onClientEquipUpStar";
    /// <summary>
    /// 客户端请求装备强化
    ///  装备升级 player_id == 0背包装备 equip_id 是uuid
    /// player_id > 0球员装备 equip_id 是 itemID
    /// </summary>
    public static string CLIENT_EQUIP_UP_STRONG = "onClientEquipUpStrongLv";
    /// <summary>
    /// 客户端请求装备一键升级
    ///  装备升级 player_id == 0背包装备 equip_id 是uuid
    /// player_id > 0球员装备 equip_id 是 itemID
    /// </summary>
    public static string CLIENT_EQUIP_ONEKEY_STRONG = "onClientEquipOneKeyUPStrong";
    /// <summary>
    /// 客户端请求穿戴装备
    ///  装备升级 player_id == 0背包装备 equip_id 是uuid
    /// player_id > 0球员装备 equip_id 是 itemID
    /// </summary>
    public static string CLIENT_EQUIP_PUT_ON = "onClientPutOnEquip";
    /// <summary>
    /// 客户端请求脱掉装备
      /// player_id > 0球员装备 equip_id 是 itemID
    /// </summary>
    public static string CLIENT_EQUIP_TAKE_OFF = "onClientTakeOffEquip";

    //客户端请求宝石镶嵌
    //index, player_id, equip_id, gem_id  
    public static string CLIENT_GEM_INSET = "onClientInsetGem";

    //客户端请求卸下宝石
    //index, player_id, equip_id  
    public static string CLIENT_GEM_TAKEOFF = "onClentTakeOffGem";

    //客户端请求开槽
    public static string CLIENT_GEM_OPEN = "onClientGemOpen";

    //客户端请求继承
    public static string CLIENT_EQUIP_INHERIT = "onClientEqupInherit";

    //客户端请求宝石合成
    public static string CLIENT_COMPOUND_GEM = "onClientGemCompound";

    public EquipProxy(): 
        base(ProxyID.Equip)
    {
        
        KBEngine.Event.registerOut(this, SERVER_MAKE_EQUIP_SUCC);
        KBEngine.Event.registerOut(this, "getEquipList");
        KBEngine.Event.registerOut(this, "starUpEquipSucc");
        KBEngine.Event.registerOut(this, "getStarAndStrongLv");
        KBEngine.Event.registerOut(this, "strongUpEquipSucc");
        KBEngine.Event.registerOut(this, "getOneKeyUpStrongResult");
        KBEngine.Event.registerOut(this, SERVER_PUT_ON_EQUIP_SUCCC);
        KBEngine.Event.registerOut(this, "takeOffEquipSucc");
        KBEngine.Event.registerOut(this, "returnGemResult");
        KBEngine.Event.registerOut(this, "returnInheritResult");
        KBEngine.Event.registerOut(this, "gemCompoundSucc");
        
    }
    //装备制作成功
    public void makeEquipSucc(object obj)
    {
        EquipMakeMediator.equipMakeMediator.MakeSucc();
        GetquipList(0);
    }


    //宝石合成成功
    public void gemCompoundSucc(object obj)
    {
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_22"), null);

    }

    //获取装备列表
    public void getEquipList(object obj,List<object> list)
    {
        EquipItemInfo info = null;
        //id == 0 背包装备  id>0玩家装备
        int card_id = GameConvert.IntConvert(obj);

        if (EquipConfig.m_player_eqiup.ContainsKey(card_id))
        {
            EquipConfig.m_player_eqiup[card_id].Clear();
        }
        
        for (int i = 0; i < list.Count; ++i)
        {
            info = new EquipItemInfo();
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            info.uuid = Info["UUID"].ToString();
            info.itemID = Info["itemID"].ToString();
            info.star = GameConvert.IntConvert(Info["star"]);
            info.strongLevel = GameConvert.IntConvert(Info["strongLevel"]);
            info.gem1 = GameConvert.IntConvert(Info["gem1"]);
            info.gem2= GameConvert.IntConvert(Info["gem2"]);
            info.gem3 = GameConvert.IntConvert(Info["gem3"]);

            if (GameConvert.IntConvert(info.itemID) <= 0)
                continue;


            if (!EquipConfig.m_player_eqiup.ContainsKey(card_id))
            {
                EquipConfig.m_player_eqiup[card_id] = new List<EquipItemInfo>();
            }
            var entry = EquipConfig.m_player_eqiup[card_id];
            entry.Add(info);
        }

        if(EquipMediator.equipMediator != null && GUIManager.HasView("equipmainpanel"))
        {
            EquipMediator.equipMediator.SetEquipGridInfo(card_id);
            EquipMediator.equipMediator.OpenFunction();
        }
        
        GameEventManager.RaiseEvent(GameEventTypes.EquipRefresh, card_id);
        
    }

    //宝石镶嵌开槽卸下结果返回
    public void returnGemResult(object obj, List<object> list)
    {
        EquipItemInfo info = null;
        //id == 0 背包装备  id>0玩家装备
        int card_id = GameConvert.IntConvert(obj);
        for (int i = 0; i < list.Count; ++i)
        {
            info = new EquipItemInfo();
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            info.uuid = Info["UUID"].ToString();
            info.itemID = Info["itemID"].ToString();
            info.star = GameConvert.IntConvert(Info["star"]);
            info.strongLevel = GameConvert.IntConvert(Info["strongLevel"]);
            info.gem1 = GameConvert.IntConvert(Info["gem1"]);
            info.gem2 = GameConvert.IntConvert(Info["gem2"]);
            info.gem3 = GameConvert.IntConvert(Info["gem3"]);
        }
        
        EquipConfig.RefreshEquipData(card_id, info);
        EquipMediator.cur_equip = info;

        EquipMediator.equipMediator.RefreshOpenFunc(info.star, info.strongLevel);

        Facade.SendNotification(NotificationID.GemChoose_Hide);

    }

    /// <summary>
    /// 装备继承返回
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="list"></param>
    public void returnInheritResult(object obj, List<object> list)
    {
        EquipItemInfo info = null;
        //id == 0 背包装备  id>0玩家装备
        int card_id = GameConvert.IntConvert(obj);
        int play_id = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            info = new EquipItemInfo();
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            info.uuid = Info["UUID"].ToString();
            info.itemID = Info["itemID"].ToString();
            info.star = GameConvert.IntConvert(Info["star"]);
            info.strongLevel = GameConvert.IntConvert(Info["strongLevel"]);
            info.gem1 = GameConvert.IntConvert(Info["gem1"]);
            info.gem2 = GameConvert.IntConvert(Info["gem2"]);
            info.gem3 = GameConvert.IntConvert(Info["gem3"]);
            play_id = i == 0 ? card_id : 0;
            EquipConfig.RefreshEquipData(play_id, info);
            if (i == 0)
                EquipMediator.cur_equip = info;

            EquipMediator.equipMediator.SetEquipGridInfo(play_id);

        }

        EquipMediator.equipMediator.RefreshOpenFunc(info.star, info.strongLevel);

    }

    /// <summary>
    /// 客户端请求球员装备列表
    /// </summary>
    /// <param name="cardId"></param>
    public void GetquipList(int cardId)
    {
        ServerCustom.instance.SendClientMethods(EquipProxy.CLIENT_Get_EQUIP_LIST, cardId);
    }
    //强化成功
    public void strongUpEquipSucc(object val, Dictionary<string, object> list)
    {
        int card_id = GameConvert.IntConvert(val);

        EquipItemInfo info = new EquipItemInfo();
        info.uuid = list["UUID"].ToString();
        info.itemID = list["itemID"].ToString();
        info.star = GameConvert.IntConvert(list["star"]);
        info.strongLevel = GameConvert.IntConvert(list["strongLevel"]);
        info.gem1 = GameConvert.IntConvert(list["gem1"]);
        info.gem2 = GameConvert.IntConvert(list["gem2"]);
        info.gem3 = GameConvert.IntConvert(list["gem3"]);

        EquipMediator.cur_equip = info;

        EquipConfig.RefreshEquipData(card_id, info);
        
        EquipStrongMediator.equipStrongMediator.SetInfo(info.star, info.strongLevel);

        GUIManager.SetPromptInfo(TextManager.GetUIString("UI2024"), null);

        //GetquipList(card_id);
        EquipMediator.equipMediator.RefreshOpenFunc(info.star, info.strongLevel);
        EquipMediator.equipMediator.SetEquipGridInfo(card_id);

    }
    //升星成功
    public void starUpEquipSucc(object obj, Dictionary<string, object> list)
    {
        int card_id = GameConvert.IntConvert(obj);

        EquipItemInfo info = new EquipItemInfo();
        info.uuid = list["UUID"].ToString();
        info.itemID = list["itemID"].ToString();
        info.star = GameConvert.IntConvert(list["star"]);
        info.strongLevel = GameConvert.IntConvert(list["strongLevel"]);
        info.gem1 = GameConvert.IntConvert(list["gem1"]);
        info.gem2 = GameConvert.IntConvert(list["gem2"]);
        info.gem3 = GameConvert.IntConvert(list["gem3"]);


        EquipMediator.cur_equip = info;
        
        EquipConfig.RefreshEquipData(card_id, info);
        
        GUIManager.SetPromptInfo(TextManager.GetUIString("UI2023"), null);
        EquipMediator.equipMediator.RefreshOpenFunc(info.star, info.strongLevel);
        EquipMediator.equipMediator.SetEquipGridInfo(card_id);

    }
    /// <summary>
    /// 穿戴装备成功
    /// </summary>
    /// <param name="obj"></param>
    public void putOnEquipSucc(object obj)
    {
        int card_id = GameConvert.IntConvert(obj);
        //GUIManager.SetPromptInfo(TextManager.GetUIString("UI2049"), null);

        GetquipList(0);
        GetquipList(card_id);
        Facade.SendNotification(NotificationID.EquipInformation_Hide);

    }

    /// <summary>
    /// 脱掉装备成功
    /// </summary>
    /// <param name="obj"></param>
    public void takeOffEquipSucc(object obj)
    {
        int card_id = GameConvert.IntConvert(obj);
        //GUIManager.SetPromptInfo(TextManager.GetUIString("UI2050"), null);
        Facade.SendNotification(NotificationID.EquipInformation_Hide);

        GetquipList(0);
        GetquipList(card_id);

    }
    //获取当前装备星级和强化等级
    public void getStarAndStrongLv(object obj, object data)
    {
        int star = GameConvert.IntConvert(obj);
        int level = GameConvert.IntConvert(data);

        EquipMediator.equipMediator.RefreshOpenFunc(star,level);
    }
    /// <summary>
    /// 服务器返回装备一键强化结果
    /// </summary>
    /// <param name="list"></param>
    public void getOneKeyUpStrongResult(object obj, Dictionary<string, object> equip, List<object> list)
    {
        int card_id = GameConvert.IntConvert(obj);
        
        EquipItemInfo item = new EquipItemInfo();
        item.uuid = equip["UUID"].ToString();
        item.itemID = equip["itemID"].ToString();
        item.star = GameConvert.IntConvert(equip["star"]);
        item.strongLevel = GameConvert.IntConvert(equip["strongLevel"]);
        item.gem1 = GameConvert.IntConvert(equip["gem1"]);
        item.gem2 = GameConvert.IntConvert(equip["gem2"]);
        item.gem3 = GameConvert.IntConvert(equip["gem3"]);

        EquipMediator.cur_equip = item;
        EquipConfig.RefreshEquipData(card_id, item);
        EquipMediator.equipMediator.RefreshOpenFunc(item.star, item.strongLevel);


        StrongResultData info = null;
        List<StrongResultData> list_strong = new List<StrongResultData>();
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;
            info = new StrongResultData();
            info.preStrong = GameConvert.IntConvert(data["preStrong"]);
            info.nextStrong = GameConvert.IntConvert(data["nextStrong"]);
            info.cost = GameConvert.IntConvert(data["cost"]);
            list_strong.Add(info);
        }
        EquipStrongResultMediator.info_list = list_strong;
        Facade.SendNotification(NotificationID.EquipStrongResult_Show);
        //GUIManager.SetPromptInfo(TextManager.GetUIString("UI2022"), null);
        EquipMediator.equipMediator.SetEquipGridInfo(card_id);

    }
}

