using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;
using System.Collections.Generic;

public class guildofficepanel : BasePanel
{
    public UIButton off_btn;
    public UILabel state_label;
    public UIButton mail_btn;
    public UIButton dissolve_btn;
    public UIButton quit_btn;
    public UIToggle member_toggle;
    public UIToggle apply_toggle;
    public UIToggle post_toggle;
    public UIScrollView ScrollView;
    public UIToggle online_Toggle;
    public UIToggle week_Toggle;
    public UIToggle offline_Toggle;

    public UIToggle applyTimeBtn;
    public UIToggle levelBtn;
    public UIToggle onlineBtn;
    public UIGrid membergrid;
    public UIGrid applygrid;
    public UIGrid postgrid;
    public Transform rankway;
    public Transform menmber;
    public Transform apply;

    public UILabel online_label;
}
public class GuildOfficeMediator : UIMediator<guildofficepanel>
{
    private enum Func_type
    {
        Member =1,
        Apply =2,
        Official = 3
    }
    private enum CompareType
    {
        OnLine=1,
        WeekDonate=2,
        OutLineTime=3,
        ApplyTime =4,
        Level = 5,
    }
 
    private int mMaxGuildMemberNum;
    private guildofficepanel panel
    {
        get
        {
            return m_Panel as guildofficepanel;
        }
    }
    private Func_type mCurType = Func_type.Member;

    private CompareType mMemberCompare = CompareType.OnLine;
    private CompareType mApplyCompare = CompareType.ApplyTime;

    public static GuildOfficeMediator guildOfficeMediator;
    public GuildOfficeMediator() : base("guildofficepanel")
    {
        m_isprop = true;
        setDepth = 4;
        RegistPanelCall(NotificationID.GuildOffice_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildOffice_Hide, ClosePanel);
    }
   
    GuildApplyInfo applyinfo;
    
    GuildPower powerinfo;
    protected override void OnShow(INotification notification)
    {
        if (guildOfficeMediator == null)
        {
            guildOfficeMediator = Facade.RetrieveMediator("GuildOfficeMediator") as GuildOfficeMediator;
        }
        SetFuncs();
        InitGrid();

        mMaxGuildMemberNum = GuildMainMediator.guildmainMediator.GetGuildMaxMember();
        
        OnClientGetGuildApply();
        OnClientGetGuildMember();
        
    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.mail_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.dissolve_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.quit_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.member_toggle.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.apply_toggle.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.post_toggle.gameObject).onClick = OnClick;


        UIEventListener.Get(panel.online_Toggle.gameObject).onClick = OnCompareClick;
        UIEventListener.Get(panel.week_Toggle.gameObject).onClick = OnCompareClick;
        UIEventListener.Get(panel.offline_Toggle.gameObject).onClick = OnCompareClick;

        UIEventListener.Get(panel.applyTimeBtn.gameObject).onClick = OnApplyCompareClick;
        UIEventListener.Get(panel.levelBtn.gameObject).onClick = OnApplyCompareClick;
        UIEventListener.Get(panel.onlineBtn.gameObject).onClick = OnApplyCompareClick;
    }
    private void InitGrid()
    {

        bool isPower = GuildMainMediator.guildmainMediator.IsPowerEnough("2");
        panel.dissolve_btn.gameObject.SetActive(isPower);
        panel.quit_btn.gameObject.SetActive(!isPower);

        panel.membergrid.enabled = true;
        panel.membergrid.BindCustomCallBack(UpdateMemberGrid);
        panel.membergrid.StartCustom();

        panel.applygrid.enabled = true;
        panel.applygrid.BindCustomCallBack(UpdateApplyGrid);
        panel.applygrid.StartCustom();

        panel.postgrid.enabled = true;
        panel.postgrid.BindCustomCallBack(UpdatePostGrid);
        panel.postgrid.StartCustom();
    }
    //设置公会成员数据
    public void SetMemberGridData()
    {
      
        //if (GuildMainMediator.mMyGuild.memberList.Count <= 0)
        //    return;
        List<object> listObj = new List<object>();
        List<GuildMemberInfo> memberList = GuildMainMediator.mMyGuild.memberList;

        switch (mMemberCompare)
        {
            case CompareType.OnLine:
                memberList.Sort(CompareOnLine);
                break;
            case CompareType.WeekDonate:
                memberList.Sort(CompareWeekDonate);
                break;
            case CompareType.OutLineTime:
                memberList.Sort(CompareOutLineTime);
                break;

        }

        for ( int i=0; i<memberList.Count; i++)
        {
            listObj.Add(memberList[i]);
        }
        
        panel.membergrid.AddCustomDataList(listObj);       
        int onLineNum = GetOnLineNum();
        panel.online_label.text = onLineNum + "/" + mMaxGuildMemberNum;

        SetPowerGridData();

    }
    //设置公会申请数据
    public void SetApplyGridData()
    {
        //if (GuildMainMediator.mMyGuild.applygrid.Count <= 0)
        //    return;
        List<object> listObj = new List<object>();
        List<GuildApplyInfo> applyList  = GuildMainMediator.mMyGuild.applyList;

        switch (mApplyCompare)
        {
            case CompareType.ApplyTime:
                applyList.Sort(CompareApplyTime);
                break;
            case CompareType.Level:
                applyList.Sort(CompareApplyLevel);
                break;
            case CompareType.OutLineTime:
                break;
        }

        for (int i = 0; i < applyList.Count; i++)
        {
            listObj.Add(applyList[i]);
        }
        panel.applygrid.AddCustomDataList(listObj);

    }

    //设置位置信息
    private void SetPowerGridData()
    {
        List<GuildOfficial> list = GuildOfficialConfig.GetGuildOfficalList();
        List<object> listObj = new List<object>();

        for (int i = list.Count-1; i>=0; i--)
        {
            listObj.Add(list[i]);
        }

        panel.postgrid.AddCustomDataList(listObj);
    }
    private void SetFuncs()
    {
        string uistring = "UI_Guildoffice" + GameConvert.IntConvert(mCurType);
        panel.state_label.text = TextManager.GetUIString(uistring);
        panel.applygrid.gameObject.SetActive(mCurType == Func_type.Apply);
        panel.membergrid.gameObject.SetActive(mCurType == Func_type.Member);
        panel.postgrid.gameObject.SetActive(mCurType == Func_type.Official);
        panel.menmber.gameObject.SetActive(mCurType == Func_type.Member);
        panel.apply.gameObject.SetActive(mCurType == Func_type.Apply);

    }


    //刷新成员列表
    private void UpdateMemberGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null) 
            return;
        GuildMemberInfo memberinfo = item.oData as GuildMemberInfo;
        UITexture portrait_texture = item.mScripts[0] as UITexture;
        UILabel post_label = item.mScripts[1] as UILabel;
        UILabel name_label = item.mScripts[2] as UILabel;
        UILabel G_post_label = item.mScripts[3] as UILabel;
        UILabel level_label = item.mScripts[4] as UILabel;
        UILabel weekgongxian_label = item.mScripts[5] as UILabel;
        UILabel allgongxian_label = item.mScripts[6] as UILabel;
        UISprite interaction_btn = item.mScripts[7] as UISprite;
        UILabel state_label = item.mScripts[8] as UILabel;
        name_label.text = memberinfo.playerName;
        post_label.text = TextManager.GetUIString("UIGuildPower"+ memberinfo.power);
        G_post_label.text = TextManager.GetUIString("UIOffical" + memberinfo.offical);
        level_label.text = memberinfo.level.ToString();
        weekgongxian_label.text = memberinfo.weekDonate.ToString();
        allgongxian_label.text = memberinfo.sumDonate.ToString();

        interaction_btn.transform.GetComponent<UIButton>().isEnabled = PlayerMediator.playerInfo.roleId != memberinfo.id;
        if (memberinfo.onlineState>1)
        {
            TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            int cstName = (int)timeSpan.TotalSeconds;
            state_label.color = Color.white;
            state_label.text = string.Format(TextManager.GetUIString("UIFriend2"), UtilTools.GetMaxTimeFomat(cstName - memberinfo.onlineState));
        }
        else
        {
            state_label.color = Color.green;
            state_label.text = TextManager.GetUIString("UIFriend1");
        }

        UIEventListener.Get(interaction_btn.transform.gameObject).onClick = OnInteractClick;

    }
    //刷新批准申请
    private void UpdateApplyGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        applyinfo = item.oData as GuildApplyInfo;
        UITexture portrait_texture = item.mScripts[0] as UITexture;
        UILabel name_label = item.mScripts[1] as UILabel;
        UILabel post_label = item.mScripts[2] as UILabel;
        UILabel level_label = item.mScripts[3] as UILabel;
        UILabel applytime_label = item.mScripts[4] as UILabel;
        UILabel state_time = item.mScripts[5] as UILabel;
        UISprite agree_btn = item.mScripts[6] as UISprite;
        UISprite reject_btn = item.mScripts[7] as UISprite;

        post_label.text = TextManager.GetUIString("UIOffical" + applyinfo.offical);
        name_label.text = applyinfo.playerName;
        level_label.text = applyinfo.level.ToString();


        DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(applyinfo.applyTime);
        //转成本地时间  
        DateTime localdt = utcdt.ToLocalTime();
        string timeformat = localdt.ToString("yyyy-MM-dd HH:mm:ss");

        applytime_label.text = timeformat;
        state_time.text ="";

        UIEventListener.Get(agree_btn.gameObject).onClick = OnApplyClick;
        UIEventListener.Get(reject_btn.gameObject).onClick = OnApplyClick;

    }
    //刷新职位权限
    private void UpdatePostGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        GuildOfficial info = item.oData as GuildOfficial;
        UILabel post_label = item.mScripts[0] as UILabel;
        UILabel count_label = item.mScripts[1] as UILabel;
        UILabel powerinfo_label = item.mScripts[2] as UILabel;
        UIButton accuse_btn = item.mScripts[3] as UIButton;

        bool isImpeach = GetIsImpeach();
        accuse_btn.isEnabled = isImpeach;
        accuse_btn.gameObject.SetActive(info.id == 5);
        post_label.text = TextManager.GetUIString("UIGuildPower" + info.id);
        int nowcount = GetGuildPowerNum(info.id);

        count_label.gameObject.SetActive(info.num > 0);
        count_label.text = nowcount + "/" + info.num;

        string content = string.Empty;

        string[] powerArr = info.powerOpen.Split(',');
        for(int i=0; i<powerArr.Length; i++)
        {
            if (GameConvert.IntConvert(powerArr[i]) > 0)
            {
                content += TextManager.GetUIString("UI_GuildPower" + powerArr[i]);
                content += i < powerArr.Length - 1 ? "," : " ";
            }
        }
        UIEventListener.Get(accuse_btn.gameObject).onClick = OnImpeachClick;

        powerinfo_label.text = content;

    }
    private void OnImpeachClick(GameObject go)
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientImpeach);
    }
    private void OnApplyClick(GameObject go)
    {
        UIGridItem item = go.transform.parent.GetComponent<UIGridItem>();
        GuildApplyInfo data = item.oData as GuildApplyInfo;


        bool isPower = GuildMainMediator.guildmainMediator.IsPowerEnough("10");
        if(!isPower)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_guild_err11"),null);
            return;
        }
        switch(go.transform.name)
        {
            case "agree_btn":
                ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAgreeJoin, data.id);
                break;
            case "reject_btn":
                ServerCustom.instance.SendClientMethods(GuildProxy.OnClientRejectApply,data.id);
                break;
        }
    }
    private void OnInteractClick(GameObject go)
    {
        
        UIGridItem item = go.transform.parent.GetComponent<UIGridItem>();
        GuildMemberInfo info = item.oData as GuildMemberInfo;
        if (info.id == PlayerMediator.playerInfo.roleId)
            return;
        Facade.SendNotification(NotificationID.GuildInteract_Show, info);
    }
    private void OnClick(GameObject go)
    {
        switch (go.transform.name)
        {
            case "off_btn":
                Facade.SendNotification(NotificationID.GuildOffice_Hide);
                break;
            case "mail_btn":

                break;
            case "dissolve_btn":
                if (PlayerMediator.playerInfo.guildPower != 5)
                {
                    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err11"));
                    return;
                }
                string title1 = TextManager.GetUIString("UI_GuildPower2");
                string content1 = TextManager.GetUIString("UI_Guildoffice26");
                GUIManager.SetPromptInfoChoose(title1, content1, SureDissolve);
                break;
            case "quit_btn":
                if (PlayerMediator.playerInfo.guildPower == 5)
                    return;

                string title = TextManager.GetUIString("UIGuild32");
                string content= TextManager.GetUIString("UI_Guildoffice25");
                GUIManager.SetPromptInfoChoose(title, content, SureQuit);
                break;
            case "member_toggle":

                mCurType = Func_type.Member;
                SetFuncs();

                break;
            case "apply_toggle":
             
                mCurType = Func_type.Apply;
                SetFuncs();

                break;
            case "post_toggle":

                 mCurType = Func_type.Official;
                SetFuncs();

                break;
        }
    }
    //解散公会
    private void SureDissolve()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientDismissGuild);

    }
    //退出公会
    private void SureQuit()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientQuitGuild);

    }
    private void OnCompareClick(GameObject go)
    {
        CompareType type = CompareType.OnLine;
        switch (go.transform.name)
        {
            case "online_Toggle":
                type = CompareType.OnLine;
                break;
            case "week_Toggle":
                type = CompareType.WeekDonate;

                break;
            case "offline_Toggle":
                type = CompareType.OutLineTime;
                break;
         
        }
        if (type == mMemberCompare)
            return;
        mMemberCompare = type;
        panel.ScrollView.ResetPosition();
        SetMemberGridData();
    }

    private void OnApplyCompareClick(GameObject go)
    {
        CompareType type = CompareType.OnLine;

        switch (go.transform.name)
        {
      
            case "applyTimeBtn":
                type = CompareType.ApplyTime;
                break;
            case "levelBtn":
                type = CompareType.Level;
                break;
            case "onlineBtn":
                type = CompareType.OutLineTime;
                break;
        }
        if (type == mApplyCompare)
            return;
        mApplyCompare = type;
        SetApplyGridData();
    }

  
    /// <summary>
    /// 获取在线成员数量
    /// </summary>
    /// <returns></returns>
    private int GetOnLineNum()
    {
        int num=0;

        for(int i =0; i<GuildMainMediator.mMyGuild.memberList.Count; i++)
        {
            if (GuildMainMediator.mMyGuild.memberList[i].onlineState == 1)
                num++;
        }

        return num;
    }

    //获取公会职务数量
    private int GetGuildPowerNum(int power)
    {
        int num = 0;
        for (int i = 0; i < GuildMainMediator.mMyGuild.memberList.Count; i++)
        {
            if (GuildMainMediator.mMyGuild.memberList[i].power == power)
                num++;
        }

        return num;
    }
    //能否弹劾 领袖
    private bool GetIsImpeach()
    {
        bool isPower = GuildMainMediator.guildmainMediator.IsPowerEnough("13");
        if (!isPower)
            return false;
        int needTime = GuildBaseConfig.GetGuildBase(1).impeachTime;
        for (int i = 0; i < GuildMainMediator.mMyGuild.memberList.Count; i++)
        {
            GuildMemberInfo info = GuildMainMediator.mMyGuild.memberList[i];
            if (info.power == 5&& info.onlineState>1)
            {
                TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                int cstName = (int)timeSpan.TotalSeconds;
                int leftTime = cstName - info.onlineState;
                if (leftTime >= needTime * 24 * 60 * 60)
                    return true;
            }
        }
        return false;
    }

    //在线优先
    private int CompareOnLine(GuildMemberInfo info1,GuildMemberInfo info2)
    {
        if(info1.onlineState ==1 || info2.onlineState==1)
        {
            if (info1.onlineState < info2.onlineState)
                return -1;
            else if (info1.onlineState > info2.onlineState)
                return 1;
            else if (info1.power > info2.power)
                return -1;
            else if (info1.power < info2.power)
                return 1;
        }
        else
        {
            if (info1.onlineState > info2.onlineState)
                return -1;
            else if (info1.onlineState < info2.onlineState)
                return 1;
            else if (info1.power > info2.power)
                return -1;
            else if (info1.power < info2.power)
                return 1;
        }

        return 0;

    }
    //离线时间
    private int CompareOutLineTime(GuildMemberInfo info1, GuildMemberInfo info2)
    {
        if (info1.onlineState == 1 || info2.onlineState == 1)
        {
            if (info1.onlineState < info2.onlineState)
                return 1;
            else if (info1.onlineState > info2.onlineState)
                return -1;
            else if (info1.power > info2.power)
                return -1;
            else if (info1.power < info2.power)
                return 1;
        }
        else
        {
            if (info1.onlineState > info2.onlineState)
                return 1;
            else if (info1.onlineState < info2.onlineState)
                return -1;
            else if (info1.power > info2.power)
                return -1;
            else if (info1.power < info2.power)
                return 1;
        }

        return 0;
    }
    //周贡献
    private int CompareWeekDonate(GuildMemberInfo info1, GuildMemberInfo info2)
    {
        if (info1.weekDonate > info2.weekDonate)
            return -1;
        else if (info1.weekDonate < info2.weekDonate)
            return 1;
        else if (info1.onlineState < info2.onlineState)
            return -1;
        else if (info1.onlineState > info2.onlineState)
            return 1;
        else if (info1.power > info2.power)
            return -1;
        else if (info1.power < info2.power)
            return 1;
        else
            return 0;
    }
    //申请时间
    private int CompareApplyTime(GuildApplyInfo info1, GuildApplyInfo info2)
    {
        if (info1.applyTime > info2.applyTime)
            return -1;
        else if (info1.applyTime < info2.applyTime)
            return 1;
        else
            return 0;
    }
    //玩家等级
    private int CompareApplyLevel(GuildApplyInfo info1, GuildApplyInfo info2)
    {
        if (info1.level > info2.level)
            return -1;
        else if (info1.level < info2.level)
            return 1;
        else
            return 0;
    }
    //客户端请求自己公会成员列表
    private void OnClientGetGuildMember()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientGetGuildMember);
    }

    //客户端请求自己公会成员列表
    private void OnClientGetGuildApply()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientGetGuildApply);
    }
}
