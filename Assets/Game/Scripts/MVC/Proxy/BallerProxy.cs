using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 球队
/// </summary>
public class BallerProxy : Proxy<BallerProxy>
{

    public BallerProxy()
        : base(ProxyID.Baller)
    {
        KBEngine.Event.registerOut(this, "onGetAllCardInfo");
        KBEngine.Event.registerOut(this, "onGetAllPieces");
        KBEngine.Event.registerOut(this, "onBallerCallBack");
        KBEngine.Event.registerOut(this, "onMentalityUP");
        KBEngine.Event.registerOut(this, "onMentalityTenUP");
        KBEngine.Event.registerOut(this, "onInheritSucess");
        KBEngine.Event.registerOut(this, "onCombineCardInfo");
        KBEngine.Event.registerOut(this, "onCardFightValueChange");
        KBEngine.Event.registerOut(this, "onUpdateCardInfo");
        KBEngine.Event.registerOut(this, "onLevelMax");
    }

    public void onCardFightValueChange(object cardId,object fightValue)
    {
        int id = UtilTools.IntParse(cardId.ToString());
        int Value = UtilTools.IntParse(fightValue.ToString());
        if (TeamMediator.teamMediator != null && TeamMediator.currentTeamBaller != null && TeamMediator.currentTeamBaller.id == id)
        {
            TeamMediator.ballerInfoList["fightValue"].text = Value.ToString();
            TeamMediator.currentTeamBaller.fightValue = Value;
            TeamMediator.teamList[TeamMediator.currentTeamBaller.configId] = TeamMediator.currentTeamBaller;
        }
    }

    /// <summary>
    /// 合成球员成功
    /// </summary>
    public void onCombineCardInfo(Dictionary<string, object> list)
    {
        TeamBaller player = new TeamBaller();
        player = AddBallerInfo(list);
        if (TeamMediator.teamMediator != null)
        {
            TeamMediator.teamMediator.CommbineSucess(player);         
        }
    }


    /// <summary>
    /// 球员属性刷新
    /// </summary>
    /// <param name="list"></param>
    public void onUpdateCardInfo(Dictionary<string, object> list)
    {
        TeamBaller player = new TeamBaller();
        player = AddBallerInfo(list);
        if (TeamMediator.teamMediator != null && player.id == TeamMediator.currentTeamBaller.id)
        {
            TeamMediator.currentTeamBaller = player;
            TeamMediator.teamMediator.ShowBallerInfo();
        }
        TeamMediator.teamList[player.configId] = player;
    }

    public void onGetAllCardInfo(List<object> list)
    {
        TeamMediator.teamList.Clear();

        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            TeamBaller baller = AddBallerInfo(Info);
            if (TeamMediator.teamList.ContainsKey(baller.configId))
                TeamMediator.teamList[baller.configId] = baller;
            else
                TeamMediator.teamList.Add(baller.configId, baller);

            EquipProxy.Instance.GetquipList(baller.id);
        }
        EquipProxy.Instance.GetquipList(0);
    }

    TeamBaller AddBallerInfo(Dictionary<string, object> Info)
    {
        TeamBaller info = new TeamBaller();
        info.id = int.Parse(Info["id"].ToString());
        info.configId = Info["configID"].ToString();
        info.level = int.Parse(Info["level"].ToString());
        info.exp = int.Parse(Info["exp"].ToString());
        info.star = int.Parse(Info["star"].ToString());
        info.pos = int.Parse(Info["pos"].ToString());
        info.inTeam = int.Parse(Info["inTeam"].ToString());
        info.bench = int.Parse(Info["bench"].ToString());
        info.isSelf = int.Parse(Info["isSelf"].ToString());
        if (info.isSelf == 1)
            PlayerMediator.playerInfo.cardId = info.configId;
        info.brokenLayer = int.Parse(Info["brokenLayer"].ToString());
        info.fightValue = int.Parse(Info["fightValue"].ToString());
        info.shoot = int.Parse(Info["shoot"].ToString());
        info.shootM = int.Parse(Info["shootM"].ToString());
        info.shootExp = int.Parse(Info["shootExp"].ToString());
        info.passBall = int.Parse(Info["pass"].ToString());
        info.passBallM = int.Parse(Info["passBallM"].ToString());
        info.passBallExp = int.Parse(Info["passBallExp"].ToString());
        info.defend = int.Parse(Info["defend"].ToString());
        info.defendM = int.Parse(Info["defendM"].ToString());
        info.defendExp = int.Parse(Info["defendExp"].ToString());
        info.trick = int.Parse(Info["trick"].ToString());
        info.trickM = int.Parse(Info["trickM"].ToString());
        info.trickExp = int.Parse(Info["trickExp"].ToString());
        info.reel = int.Parse(Info["reel"].ToString());
        info.reelM = int.Parse(Info["reelM"].ToString());
        info.reelExp = int.Parse(Info["reelExp"].ToString());
        info.steal = int.Parse(Info["steal"].ToString());
        info.stealM = int.Parse(Info["stealM"].ToString());
        info.stealExp = int.Parse(Info["stealExp"].ToString());
        info.controll = int.Parse(Info["controll"].ToString());
        info.controllM = int.Parse(Info["controllM"].ToString());
        info.controllExp = int.Parse(Info["controllExp"].ToString());
        info.keep = int.Parse(Info["keep"].ToString());
        info.keepM = int.Parse(Info["keepM"].ToString());
        info.keepExp = int.Parse(Info["keepExp"].ToString());
        info.tech = float.Parse(Info["tech"].ToString());
        info.health = float.Parse(Info["health"].ToString());
        info.strikeNeedCost = int.Parse(Info["strikeNeedCost"].ToString());
        info.keepPercent = float.Parse(Info["keepPercent"].ToString());
        info.defendPercent = float.Parse(Info["defendPercent"].ToString());
        info.controllPercent = float.Parse(Info["controllPercent"].ToString());
        info.shootPercent = float.Parse(Info["shootPercent"].ToString());
        int level = int.Parse(Info["skill1"].ToString());
        if (info.isSelf == 1)
            PlayerManager.playerList[int.Parse(info.configId)].skill1 = (level / 100).ToString();
        info.skill1Lv = level - level / 100 * 100;
        level = int.Parse(Info["skill2"].ToString());
        info.skill2Lv = level - level / 100 * 100;
        level = int.Parse(Info["skill3"].ToString());
        info.skill3Lv = level - level / 100 * 100;
        level = int.Parse(Info["skill4"].ToString());
        info.skill4Lv = level - level / 100 * 100;
        level = int.Parse(Info["skill11"].ToString());
        info.skill11Lv = level - level / 100 * 100;
        level = int.Parse(Info["skill12"].ToString());
        info.skill12Lv = level - level / 100 * 100;
        level = int.Parse(Info["skill13"].ToString());
        info.skill13Lv = level - level / 100 * 100;
        return info;
    }
    public void onInheritSucess(Dictionary<string, object> Info)
    {
        onUpdateCardInfo(Info);
        TeamMediator.inheritPanel.InHeriSucess();
    }
    public void onGetAllPieces(List<object> list)
    {
        TeamMediator.clipList.Clear();
        BallerPiece info;
        foreach (MaterialItemInfo item in ItemManager.materialList.Values)
        {
            info = new BallerPiece();
            info.uuid = string.Empty;
            info.configID = item.materialId.ToString();
            info.itemID = item.itemID;
            info.isHave = 0;
            info.amount = 0;
            TeamMediator.clipList.Add(info.itemID, info);
        }
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            if (TeamMediator.clipList.ContainsKey(Info["itemID"].ToString()))
            {
                TeamMediator.clipList[Info["itemID"].ToString()].uuid = Info["UUID"].ToString();
                TeamMediator.clipList[Info["itemID"].ToString()].amount = int.Parse(Info["amount"].ToString());
            }
        }
    }

    public void onMentalityUP(List<object> list)
    {
        AddInfoList(list);
        TeamMediator.ballerRateyInfo.CheckUpMentality();
    }
    public void onMentalityTenUP(List<object> list)
    {
        AddInfoList(list);
        ItemMediator.panelType = PanelType.UpMentality;
        Facade.SendNotification(NotificationID.ItemInfo_Show);
    }


    void AddInfoList(List<object> list)
    {
        MentalityUpItem item;
        MentalityPanel.mentalityUpList.Clear();
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> Info = list[i] as Dictionary<string, object>;
            item = new MentalityUpItem();
            item.Name = Info["name"].ToString();
            item.Value = UtilTools.IntParse(Info["number"].ToString());
            if (MentalityPanel.mentalityUpList.ContainsKey(item.Name))
            {
                MentalityPanel.mentalityUpList[item.Name].Value += item.Value;
            }
            else
            {
                MentalityPanel.mentalityUpList.Add(item.Name, item);
            }
        }
    }
    public void onLevelMax(object obj)
    {
        BallerLevelMediator.ballerLevelMediator.LevelLimit(UtilTools.IntParse(obj.ToString()));
    }
    public void onBallerCallBack(object obj)
    {
        int index = UtilTools.IntParse(obj.ToString());       
        switch(index)
        {
            case 10:
                TeamMediator.slevelPanel.SlevelSucess();
                break;
            case 8:
                TeamMediator.strikePanel.SwtichSucessCallBack();
                break;
            case 6:
                TeamMediator.strikePanel.StrikeSucessCallBack();
                break;
            case 16:
                TeamMediator.abilityPanel.AbilitySucess();
                break;
            case 17:
                BallerLevelMediator.ballerLevelMediator.LevelSucess();
                return;
            case 18:
                TeamMediator.ballerRateyInfo.MentalitySuceess();
                return;
        }
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_" + (index + 2).ToString()), null);
    }
}


/// <summary>
/// 球队球员类
/// </summary>
public  class TeamBaller
{
    public int id;            // 服务端ID
    public string configId;   // 材料ID
    public int level;         // 等级
    public int exp;           // 经验
    public int star;          // 星级
    public int inTeam;        // 上阵
    public int bench;         //替补
    public int isSelf;        // 自身
    public int pos;          //阵型中的位置
    public int brokenLayer;   // 突破等级
    public int fightValue;    // 战斗力
    public int shoot;       // 射门
    public int shootM;      // 意识射门
    public int shootExp;    // 能力射门
    public int passBall;    // 传球
    public int passBallM;   // 意识传球
    public int passBallExp; // 能力传球
    public int reel;        // 带盘
    public int reelM;       // 意识带盘
    public int reelExp;     // 能力带盘  
    public int controll;    // 控球
    public int controllM;   // 意识控球
    public int controllExp; // 能力控球
    public int defend;      // 防守
    public int defendM;     // 意识防守
    public int defendExp;   // 能力防守
    public int trick;       // 拦截
    public int trickM;      // 意识拦截
    public int trickExp;    // 能力拦截
    public int steal;       // 抢断
    public int stealM;      // 意识抢断
    public int stealExp;    // 能力抢断
    public int keep;        // 守门
    public int keepM;       // 意识守门
    public int keepExp;     // 能力守门
    public float health;      // 身体
    public float tech;        // 技术
    public int needExp;       // 所需经验
    public int strikeNeedCost;// 突破消耗碎片数量
    public float keepPercent;     // 守门百分比加成
    public float controllPercent; // 控球百分比加成
    public float shootPercent;    // 射门百分比加成
    public float defendPercent;   // 防守百分比加成
    public string material;     // 消耗材料
    public int skill1Lv;     // 技能1
    public int skill2Lv;     // 技能2
    public int skill3Lv;     // 技能3
    public int skill4Lv;     // 技能4
    public int skill11Lv;     // 技能11
    public int skill12Lv;     // 技能12
    public int skill13Lv;     // 技能13
}

/// <summary>
/// 球队员碎片类
/// </summary>
public class BallerPiece
{
    public string uuid;
    public string itemID;
    public string configID;
    public int amount;
    public int isHave;
}
