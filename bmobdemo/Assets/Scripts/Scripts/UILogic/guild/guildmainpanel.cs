using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;
using System.Collections.Generic;
using Holoville.HOTween;

public class guildmainpanel: BasePanel
{
    public Transform buildinfo;              //建筑信息0
    public Transform dismiss;
    public UIButton return_btn;              
    public UIButton fund_btn;                //资金0
    public UIButton gongxian_btn;            //贡献0
    public UILabel notice_label;             //公告滚动条0
    public UIButton change_btn;              //更改0
    public Transform build;                  //建筑0
    public UIButton cancel_btn;
    public UIButton incident_btn;
    public UIButton member_btn;              
    public UIButton whipround_btn;
    public UISlider reputation_slider;
    public UIButton into_btn;                //进入0
    public UISprite emblem;
    public UILabel guildname;
    public UILabel guildlevel;
    public UILabel member_label;
    public UILabel fund_label;
    public UILabel gongxian_label;
    public UISprite spriteBg;
}

public class PowerEnmu
{
    public int leader = 5; // 领袖
    public int secondLeader = 4; // 副统领
    public int director = 3;// 理事
    public int deacon = 2; // 执事
    public int elite = 1; // 精英
    public int member = 0; // 成员
}
public class GuildMainMediator : UIMediator<guildmainpanel>
{
    public guildmainpanel panel
    {
        get
        {
            return m_Panel as guildmainpanel;
        }
    }

    private string timerDismissKey = "GuildDismissTimerKey";

    public static MyGuildInfo mMyGuild = new MyGuildInfo() ;
    public List<GuildBuild> mBuildList = new List<GuildBuild>();
    public GuildBase mGuildBaseInfo = null;

    public static GuildMainMediator guildmainMediator;

    private Transform mBuildContent;
    public Dictionary<Transform, GuildBuildInfo> mTransBuildDict = new Dictionary<Transform, GuildBuildInfo>();
    private List<string> mTimerKeyList = new List<string>();
    public GuildMainMediator() : base("guildmainpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildMain_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildMain_Hide, ClosePanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (guildmainMediator == null)
        {
            guildmainMediator = Facade.RetrieveMediator("GuildMainMediator") as GuildMainMediator;
        }

        Facade.SendNotification(NotificationID.Gold_Hide);
        mGuildBaseInfo = GuildBaseConfig.GetGuildBase(1);
        mTimerKeyList.Clear();
        SetBuild();

    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.return_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.fund_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.gongxian_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.change_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.cancel_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.incident_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.member_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.whipround_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.spriteBg.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.into_btn.gameObject).onClick = OnClick;

        AddBuildBtnEvent();
    }
    private void AddBuildBtnEvent()
    {
        for (int i = 0; i < panel.build.childCount; i++)
        {
            Transform child = panel.build.GetChild(i);
            UIEventListener.Get(child.gameObject).onClick = OnGuildBuildClick;

        }
    }
    //设置公会基本信息
    public void SetGuildInfo()
    {
        int totalNum = GetGuildMaxMember();
        panel.member_label.text = mMyGuild.memberNum + "/" + totalNum;

        panel.gongxian_label.text = PlayerMediator.playerInfo.guildDonate.ToString();

        bool isNotice = IsPowerEnough("8");
        panel.change_btn.isEnabled = isNotice;

        SetGuildFunds();
        SetGuildNotice();
        SetGuildName();
        SetDismissTime();
        SetGuildReputation();
    }
    public void SetGuildFunds()
    {
        panel.fund_label.text = mMyGuild.guildFunds.ToString();

    }
    public void SetGuildReputation()
    {
        panel.guildlevel.text = mMyGuild.level.ToString();
        int level = mMyGuild.level;
        int maxLevel = mGuildBaseInfo.maxLevel;
        int nextReputation = GuildUpdateConfig.GetGuildUpdate(level).needReputation;

        panel.reputation_slider.value = (float)mMyGuild.reputation / (float)nextReputation;
        UILabel silderLabel = panel.reputation_slider.transform.FindChild("EXP").GetComponent<UILabel>();

        string content = "";
        if (level >= maxLevel)
            content = mMyGuild.reputation+ TextManager.GetUIString("UILevelMax");
        else
            content = mMyGuild.reputation + "/" + nextReputation;

        silderLabel.text = content;
    }
    public void SetGuildName()
    {
        panel.guildname.text = mMyGuild.name;

    }
    public void SetGuildNotice()
    {

        if (string.IsNullOrEmpty(mMyGuild.notice) || panel == null || panel.notice_label == null)
            return;
        panel.notice_label.text = mMyGuild.notice;
        float pox = 300 + panel.notice_label.width / 2;

        float poy = panel.notice_label.transform.localPosition.y;

        panel.notice_label.transform.localPosition = new Vector3(pox, poy, 0);
        Vector3 endPos = new Vector3(-pox,poy,0);
        HOTween.Restart(panel.notice_label);
        TweenParms parms = new TweenParms();
        parms.Prop("localPosition", endPos);
        parms.OnComplete(OnFinishNoticeTween);
        HOTween.To(panel.notice_label.transform, 10f, parms);

    }
    private void OnFinishNoticeTween()
    {
        SetGuildNotice();
    }
    public void SetBuild()
    {
        var enumor = GuildBuildConfig.mGuildBuildDict.Values.GetEnumerator();
        while(enumor.MoveNext())
        {
            SetBuildInfo(enumor.Current);
        }
    }
    public void SetBuildInfo(GuildBuildInfo item)
    {

        if (item.id == 1)
            SetGuildInfo();

        Transform child = null;

        int index = item.id;
        child = panel.build.FindChild("build" + index);

        //bool isPower = IsPowerEnough("7");
        //if (!isPower)
        //    return;

        Transform content = child.FindChild("content");
        content.gameObject.SetActive(false);

        UILabel level = child.FindChild("level").GetComponent<UILabel>();
        level.text ="Lv" + item.level.ToString();

        Transform speed_btn = content.FindChild("speed_btn");
        Transform upgrade_btn = content.FindChild("upgrade_btn");
        Transform into_btn = content.FindChild("into_btn");

        speed_btn.gameObject.SetActive(item.state == 1);
        upgrade_btn.gameObject.SetActive(item.state == 0);

        UIEventListener.Get(speed_btn.gameObject).onClick = OnBuildBtnClick;
        UIEventListener.Get(upgrade_btn.gameObject).onClick = OnBuildBtnClick;
        UIEventListener.Get(into_btn.gameObject).onClick = OnBuildBtnClick;

        Transform upgrade = content.FindChild("upgrade");
        upgrade.gameObject.SetActive(item.state == 1);

        if (item.state == 1)
        {
            UISlider slider = upgrade.FindChild("Slider").GetComponent<UISlider>();
            UILabel timeLabel = upgrade.FindChild("time").GetComponent<UILabel>();

            TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1));
            long time = (long)timeSpan.TotalSeconds - 8 * 60 * 60;

            long leftTime = item.endTimes - time;
            int totalTime = GetNeedUpgradeTime(item.id, item.level)*60*60;

            string timerKey = "UpdateGuildBuild" + item.id;

            
            TimerManager.Destroy(timerKey);
            TimerManager.AddTimerRepeat(timerKey, 1, delegate (object[] obj)
            {
                if (leftTime <= 0)
                {
                    leftTime = 0;
                    TimerManager.Destroy(timerKey);
                    timeLabel.text = "";
                    slider.value = 1;
                }
                else
                {
                    leftTime--;
                    int sildertime = totalTime -GameConvert.IntConvert(leftTime);
                    slider.value = (float)sildertime / (float)totalTime;
                    timeLabel.text= UtilTools.formatDuring(leftTime);
                }

            });

            if (!mTimerKeyList.Contains(timerKey))
                mTimerKeyList.Add(timerKey);
        }
        if (mTransBuildDict.ContainsKey(child))
            mTransBuildDict[child] = item;
        else
            mTransBuildDict.Add(child, item);

        
    }
    //公告0
    public void Setnotice(string intro, List<string> nameList)
    {
        if (!GUIManager.HasView("guildmainpanel"))
            return;
        panel.notice_label.text = intro;
    }

    //设置公会解散时间
    public void SetDismissTime()
    {

        UILabel timeLabel = panel.dismiss.FindChild("countdown").GetComponent<UILabel>();   
        panel.dismiss.gameObject.SetActive(mMyGuild.dismissTime>0);
        if (mMyGuild.dismissTime <= 0)
        {
            TimerManager.Destroy(timerDismissKey);
            timeLabel.text = "";
            return;
        }

        panel.cancel_btn.gameObject.SetActive(PlayerMediator.playerInfo.guildPower == 5);

        TimeSpan timeSpan = (DateTime.Now - new DateTime(1970, 1, 1));
        long time = (long)timeSpan.TotalSeconds;
        long leftTime = mMyGuild.dismissTime - time;
        TimerManager.Destroy(timerDismissKey);
        TimerManager.AddTimerRepeat(timerDismissKey, 1, delegate (object[] obj)
        {
            if (leftTime <= 0)
            {
                TimerManager.Destroy(timerDismissKey);
                timeLabel.text = "";
            }
            else
            {
                leftTime--;
                timeLabel.text = UtilTools.formatDuring(leftTime);
            }
        });

    }

    private void OnClick(GameObject go)
    {
        bool isPower = false;
        switch (go.transform.name)
        {
            case "return_btn":
                Facade.SendNotification(NotificationID.GuildMain_Hide);
                break;
            case "spriteBg":
                if (PlayerMediator.playerInfo.guildPower == 5)
                    Facade.SendNotification(NotificationID.GuildAlterInfo_Show);
                else
                    Facade.SendNotification(NotificationID.GuildInfo_Show, null);

                break;

            case "fund_btn":
                Facade.SendNotification(NotificationID.GuildDonation_Show);
                break;
            case "gongxian_btn":

                break;
            case "change_btn":
                 isPower = IsPowerEnough("8");
                if (!isPower)
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_guild_err11"), null);
                    return;
                }
                Facade.SendNotification(NotificationID.GuildAlterNotice_Show);
                break;
            case "cancel_btn":
                if (mMyGuild.dismissTime > 0)
                    ServerCustom.instance.SendClientMethods(GuildProxy.OnClientCancelDismiss);
                break;
            case "incident_btn":
                break;
            case "member_btn":
                Facade.SendNotification(NotificationID.GuildOffice_Show);
                break;
            case "whipround_btn":
                Facade.SendNotification(NotificationID.GuildDonation_Show);

                break;
            case "into_btn":
                break;
        }
  
    }
    private void OnGuildBuildClick(GameObject go)
    {
        if (mBuildContent != null)
            mBuildContent.gameObject.SetActive(false);

        if (!mTransBuildDict.ContainsKey(go.transform))
            return;
        
        GuildBuildInfo info = mTransBuildDict[go.transform];
        bool isPower = IsPowerEnough("7");
        Transform content = go.transform.FindChild("content");
        content.gameObject.SetActive(isPower);
        mBuildContent = content;
        if (!isPower)
            OpenBuildFunc(info.id);
    }
    //打开界面功能
    private void  OpenBuildFunc(int id)
    {
        switch (id)
        {
            case 1:
                Facade.SendNotification(NotificationID.GuildTactic_Show);

                break;
            case 2:
                break;
            case 3:
                Facade.SendNotification(NotificationID.GuildCounselor_Show);

                break;
            case 4:
                break;
            case 5:
                break;
        }
    }
    private void OnBuildBtnClick(GameObject go)
    {
        Transform parent = go.transform.parent.parent;
        if (!mTransBuildDict.ContainsKey(parent))
            return;

        GuildBuildInfo info = mTransBuildDict[parent];
        switch (go.name)
        {
            case "upgrade_btn":
                int needFunds = GetNeedFunds(info.id,info.level);
       
                if(info.level>=mMyGuild.level)
                {
                    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err16"));
                    return;
                }
                Facade.SendNotification(NotificationID.GuildLVUp_Show,info);

                break;
            case "speed_btn":
                Facade.SendNotification(NotificationID.GuildSpeed_Show,info);
                break;
            case "into_btn":
                if (info.id==1)
                {
                    Facade.SendNotification(NotificationID.GuildTactic_Show);
                }
                else if (info.id == 3)
                {
                    Facade.SendNotification(NotificationID.GuildCounselor_Show);
                }
                break;

        }
    }
    //获取公会最大人数上限
    public int GetGuildMaxMember()
    {
        int num = GuildBaseConfig.GetGuildBase(1).maxMemberNum;
        if(GuildBuildConfig.mGuildBuildDict.ContainsKey(1))
        {
            GuildBuildInfo info = GuildBuildConfig.mGuildBuildDict[1];

            GuildUpHall hallInfo = GuildUpHallConfig.GetGuildUpHall(info.level);

            num += hallInfo.addNum;

        }
        return num;
    }

    //判断是否拥有改权限
    public bool IsPowerEnough(string power)
    {
        GuildOfficial guildOfficial = GuildOfficialConfig.GetGuildOfficial(PlayerMediator.playerInfo.guildPower);
        if (guildOfficial == null|| string.IsNullOrEmpty(guildOfficial.powerOpen))
            return false;
        string powerStr = guildOfficial.powerOpen;
        string[] powerStrArr = powerStr.Split(',');

        List<string> listPower = new List<string>(powerStrArr);
        int indexPower = listPower.IndexOf(power);
        if (indexPower == -1)
            return false;
        return true;
    }
    //升级需要资金
    public int GetNeedFunds(int buildID,int level)
    {
        int money = 0;

        if(buildID == 1)
            money = GuildUpHallConfig.GetGuildUpHall(level).needFunds;
        else if(buildID ==2)
            money = GuildUpShopConfig.GetGuildUpShop(level).needFunds;
        else if (buildID == 3)
            money = GuildUpCounselorConfig.GetGuildUpCounselor(level).needFunds;
        else if (buildID == 4)
            money = GuildUpTaskConfig.GetGuildUpTask(level).needFunds;

        return money;
    }
    //获取升级需要时间
    public int GetNeedUpgradeTime(int buildID, int level)
    {
        int needTime = 0;

        if (buildID == 1)
            needTime = GuildUpHallConfig.GetGuildUpHall(level).needTime;
        else if (buildID == 2)
            needTime = GuildUpShopConfig.GetGuildUpShop(level).needTime;
        else if (buildID == 3)
            needTime = GuildUpCounselorConfig.GetGuildUpCounselor(level).needTime;
        else if (buildID == 4)
            needTime = GuildUpTaskConfig.GetGuildUpTask(level).needTime;

        return needTime;
    }
        
    protected override void OnDestroy()
    {
        Facade.SendNotification(NotificationID.Gold_Show);
        TimerManager.Destroy(timerDismissKey);

        for (int i=0; i<mTimerKeyList.Count; i++)
            TimerManager.Destroy(mTimerKeyList[i]);
        base.OnDestroy();
    }
}
