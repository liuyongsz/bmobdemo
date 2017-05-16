using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

public class PlayerProxy : Proxy<PlayerProxy>
{
    public PlayerProxy()
        : base(ProxyID.Player)
    {
        installEvents();
    }

    void installEvents()
    {
        KBEngine.Event.registerOut(this, "set_name");
        KBEngine.Event.registerOut(this, "set_diamond");
        KBEngine.Event.registerOut(this, "set_euro");
        KBEngine.Event.registerOut(this, "set_bodyPower");
        KBEngine.Event.registerOut(this, "set_bodyPowerBuyTimes");
        KBEngine.Event.registerOut(this, "set_euroBuyTimes");
        KBEngine.Event.registerOut(this, "onEnterScene");
        KBEngine.Event.registerOut(this, "set_vipLevel");
        KBEngine.Event.registerOut(this, "set_rmb");
        KBEngine.Event.registerOut(this, "set_roleId");
        KBEngine.Event.registerOut(this, "set_photoIndex");
        KBEngine.Event.registerOut(this, "set_level");
        KBEngine.Event.registerOut(this, "set_exp");
        KBEngine.Event.registerOut(this, "set_slogan");
        KBEngine.Event.registerOut(this, "set_club");
        KBEngine.Event.registerOut(this, "set_camp");
        KBEngine.Event.registerOut(this, "set_fightValue");
        KBEngine.Event.registerOut(this, "set_job");
        KBEngine.Event.registerOut(this, "set_formation");
        KBEngine.Event.registerOut(this, "set_benchSize");
        KBEngine.Event.registerOut(this, "set_guildDBID");
        KBEngine.Event.registerOut(this, "set_guildPower");
        KBEngine.Event.registerOut(this, "set_guildDonate");
        KBEngine.Event.registerOut(this, "set_myRank"); 
        KBEngine.Event.registerOut(this, "set_blackMoney");
        KBEngine.Event.registerOut(this, "set_arenaTimes");

    }

    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    public void onEnterScene()
    {
        GameProxy.Instance.GotoMainCity();
    }
 public void set_arenaTimes(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("arenaTimes");
        PlayerMediator.playerInfo.arenaTimes = int.Parse(v.ToString());
        if (ArenaMediator.arenaMediator != null)
            ArenaMediator.arenaMediator.BuyArenaTimesSucess();
    }
    public void set_blackMoney(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("blackMoney");
        PlayerMediator.playerInfo.blackMoney = int.Parse(v.ToString());
        if (GameShopMediator.gameShopMediator != null)
        {
            GameShopMediator.gameShopMediator.panel.moneyLabel.text = PlayerMediator.playerInfo.blackMoney.ToString();
        }
    }
    public void set_myRank(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("myRank");
        PlayerMediator.playerInfo.myRank = int.Parse(v.ToString());
    }
    public void set_name(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("name");
        PlayerMediator.playerInfo.name = v.ToString();
    }
    public void set_roleId(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("roleId");
        PlayerMediator.playerInfo.roleId = int.Parse(v.ToString());
    }
    public void set_photoIndex(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("photoIndex");
        PlayerMediator.playerInfo.photoIndex = v.ToString();
    }
    public void set_level(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("level");
        PlayerMediator.playerInfo.level = int.Parse(v.ToString());
        if (MainMediator.mainMediator == null)
            return;
        Facade.SendNotification(NotificationID.Level_Change);
    }
    public void set_rmb(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("rmb");
        PlayerMediator.playerInfo.rmb = int.Parse(v.ToString());
    }
    public void set_euro(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("euro");
        PlayerMediator.playerInfo.euro = int.Parse(v.ToString());
        if (GoldMediator.goldMediator != null)
        {
            GoldMediator.goldMediator.GoldChangeCall("euro");
        }
        if (GameShopMediator.gameShopMediator != null)
        {
            GameShopMediator.gameShopMediator.panel.moneyLabel.text = PlayerMediator.playerInfo.euro.ToString();
        }
        if (GuildDonationMediator.guilddonationMediator != null)
        {
            GuildDonationMediator.guilddonationMediator.panel.myEuro_label.text = PlayerMediator.playerInfo.euro.ToString();
        }
    }
    public void set_exp(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("exp");
        PlayerMediator.playerInfo.exp = int.Parse(v.ToString());
    }
    public void set_slogan(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("slogan");
        PlayerMediator.playerInfo.slogan = v.ToString();
        if (GUIManager.HasView("playerpanel"))
        {
            PlayerMediator.playerMediator.panel.slogan.text = PlayerMediator.playerInfo.slogan;
        }
    }
    public void set_club(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("club");
        PlayerMediator.playerInfo.club = v.ToString();
    }
    public void set_camp(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("camp");
        PlayerMediator.playerInfo.camp = int.Parse(v.ToString());
    }
    public void set_fightValue(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("fightValue");
        PlayerMediator.playerInfo.fightValue = int.Parse(v.ToString());
        if (MainMediator.mainMediator == null)
            return;
        Facade.SendNotification(NotificationID.Fight_Change);
    }
    public void set_vipLevel(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("vipLevel");
        PlayerMediator.playerInfo.vipLevel = int.Parse(v.ToString());
        int coachID = 0;
        if (PlayerMediator.playerInfo.vipLevel >= CommonConfig.GetCommonInfo(10).value)
        {
            if (SkillPanel.coachInfoList[3].isLock != 1)
                coachID = 4;
            SkillPanel.coachInfoList[3].isLock = 1;
        }
        if (PlayerMediator.playerInfo.vipLevel >= CommonConfig.GetCommonInfo(11).value)
        {
            if (SkillPanel.coachInfoList[4].isLock != 1)
                coachID = 5;
            SkillPanel.coachInfoList[4].isLock = 1;
        }
        if (coachID != 0)
        {
            if (TeamMediator.skillPanel != null)
                TeamMediator.skillPanel.UnLockCoachSucess(coachID);
        }
        if (MainMediator.mainMediator == null)
            return;
        Facade.SendNotification(NotificationID.Vip_Change);
    }
    public void set_job(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("job");
        PlayerMediator.playerInfo.job = int.Parse(v.ToString());
    }
    public void set_diamond(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("diamond");
        PlayerMediator.playerInfo.diamond = int.Parse(v.ToString());
        if (GoldMediator.goldMediator != null)
        {
            GoldMediator.goldMediator.GoldChangeCall("diamond");
        }
        if (GameShopMediator.gameShopMediator != null)
        {
            GameShopMediator.gameShopMediator.panel.moneyLabel.text = PlayerMediator.playerInfo.diamond.ToString();
        }
    }
    public void set_bodyPower(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("bodyPower");
        PlayerMediator.playerInfo.bodyPower = int.Parse(v.ToString());
        if (GoldMediator.goldMediator != null)
        {
            GoldMediator.goldMediator.GoldChangeCall("bodyPower");
        }
    }
    public void set_bodyPowerBuyTimes(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("bodyPowerBuyTimes");
        PlayerMediator.playerInfo.PowerTimes = int.Parse(v.ToString());
        if (PowerMediator.powerMediator != null)
        {
            PowerMediator.powerMediator.UpdateTimes();
        }
    }
    public void set_euroBuyTimes(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("euroBuyTimes");
        PlayerMediator.playerInfo.euroBuyTimes = int.Parse(v.ToString());
        if (PowerMediator.powerMediator != null)
        {
            PowerMediator.powerMediator.UpdateTimes();
        }
    }
    public void set_formation(KBEngine.Entity entity, object v)
    {

        v = entity.getDefinedProperty("formation");
        PlayerMediator.playerInfo.formation = int.Parse(v.ToString());
      
        Facade.SendNotification(NotificationID.Formation_Change);
    }
    public void set_benchSize(KBEngine.Entity entity, object v)
    {

        v = entity.getDefinedProperty("benchSize");
        PlayerMediator.playerInfo.benchSize = int.Parse(v.ToString());

    }
    public void set_guildDBID(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("guildDBID");
        PlayerMediator.playerInfo.guildDBID = int.Parse(v.ToString());
    }

    public void set_guildPower(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("guildPower");
        PlayerMediator.playerInfo.guildPower = int.Parse(v.ToString());

    }
    public void set_guildDonate(KBEngine.Entity entity, object v)
    {
        v = entity.getDefinedProperty("guildDonate");
        int hasDonate = GameConvert.IntConvert(v) - PlayerMediator.playerInfo.guildDonate;
        PlayerMediator.playerInfo.guildDonate = int.Parse(v.ToString());
        if (GameShopMediator.gameShopMediator != null)
        {
            GameShopMediator.gameShopMediator.panel.moneyLabel.text = PlayerMediator.playerInfo.guildDonate.ToString();
        }

        if (GuildDonationMediator.guilddonationMediator != null)
        {
            string content = string.Format(TextManager.GetSystemString("ui_system_guild21"), hasDonate);
            GUIManager.SetJumpText(content);
            GuildDonationMediator.guilddonationMediator.panel.gongxian_label.text = PlayerMediator.playerInfo.guildDonate.ToString();
        }

        if (GuildMainMediator.guildmainMediator != null)
        {
            GuildMainMediator.guildmainMediator.panel.gongxian_label.text = PlayerMediator.playerInfo.guildDonate.ToString();
        }
    }
}
public class PlayerInfo
{
    public int roleId;
    public int ranking;      // 名次 
    public string name;      // 玩家名 
    public int myRank;       // 玩家ID    public int roleId;       // 玩家ID
    public string cardId;    // 主角卡牌ID
    public string photoIndex;// 玩家头像 
    public int level;        // 玩家等级 
    public int exp;          // 玩家经验值 
    public string slogan;    // 玩家个性签名 
    public string club;         //玩家俱乐部 
    public string guildName;    //公会名 
    public int camp;       //玩家势力 
    public int officalPosition;  //玩家官职
    public int achievements;     //玩家政绩 
    public int fightValue;   //玩家战斗力 
    public int vipLevel;     //玩家VIP等级 
    public int job;         //玩家位置 
    public int diamond;      //玩家钻石 
    public int bodyPower;    //玩家体力 
    public int PowerTimes;   //玩家体力购买次数
    public int euroBuyTimes;  //玩家欧元购买次数
    public int rmb;          //玩家RMB（充值 主要用来计算vip）
    public int euro;          //欧元
    public int blackMoney;    //黑市币    public int formation;  //阵型
    public int benchSize; //替补开启数量
    public int arenaTimes;
    public string masterName;
    public int guildDBID; // 玩家公会ID
    public int guildPower; // 玩家公会职位
    public int guildDonate; // 玩家公会贡献点
    public int formation;
}