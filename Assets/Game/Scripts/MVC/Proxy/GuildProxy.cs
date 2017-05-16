using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;
/// <summary>
/// 副本
/// </summary>
public class GuildProxy: Proxy<GuildProxy>
{
    //创建公会 name, introduce
    public static string OnClientCreateGuild = "onClientCreateGuild";

    //客户端请求公会列表
    public static string OnClientGuildList = "onClientGuildList";
    
    //客户端申请加入公会 id
    public static string OnClientApplyJoinGuild = "onClientApplyJoinGuild";

    //客户端请求自己公会信息
    public static string OnClientGetGuildInfo = "onClientGetGuildInfo";

    //客户端请求建筑list
    public static string OnClientBuildList = "onClientBuildList";

    //客户端请求公会成员列表
    public static string OnClientGetGuildMember = "onClientGetGuildMember";

    //客户端请求申请加入列表
    public static string OnClientGetGuildApply = "onClientGetGuildApply";

    //客户端取消申请加入公会 id
    public static string OnClientCancelApply = "onClientCancelApply";

    //客户端申请公会副会长及简介 id
    public static string OnClientGetViceIntroduce = "onClientGetViceIntroduce";

    //客户端申请更改简介和公告 
    // 参数4个  是否修改简介  简介内容 是否修改公告 公告内容
    public static string OnClientChangeNotice = "onClientChangeNotice";

    //客户端申请修改公会名字
    //参数1个 公会名字
    public static string OnClientChangeName = "onClientChangeName";

    //客户端申请捐赠
    //参数1个 欧元
    public static string OnClientDonate = "onClientDonate";

    //客户端请求建筑升级
    //参数1个 建筑ID
    public static string OnClientBuildUpgrade = "onClientBuildUpgrade";

    //客户端建筑升级加速
    //参数2个 建筑ID  加速小时
    public static string OnClientBuildSpeed = "onClientBuildSpeed";

    //客户端任命官职
    //参数2个 成员ID 职位
    public static string OnClientAppointPower = "onClientAppointPower";

    //客户端公会转让
    //参数1个 成员ID 
    public static string OnClientGuildTransfer = "onClientGuildTransfer";

    //客户端拒绝申请加入
    //参数1个 applyID
    public static string OnClientRejectApply = "onClientRejectApply";

    //客户端接受申请加入
    //参数1个 applyID
    public static string OnClientAgreeJoin = "onClientAgreeJoin";

    //客户端弹劾会长
    public static string OnClientImpeach = "onClientImpeach";
    
    //客户端开除
    public static string OnClientKickOut = "onClientKickOut";
    
    //客户端请求解散公会
    public static string OnClientDismissGuild = "onClientDismissGuild";

    //客户端请求取消解散公会
    public static string OnClientCancelDismiss = "onClientCancelDismiss";

    //客户端退出公会
    public static string OnClientQuitGuild = "onClientQuitGuild";
    //公会上诉曝光
    //参数3三个  playerID guildID  appealID
    public static string OnClientAppealExposure = "onClientAppealExposure";

    //客户端购买公会保护时间
    public static string OnClientBuyGuildProtect = "onClientBuyGuildProtect";

    //客户端请求讨好挑拨顾问
    //顾问ID id
    public static string OnClientAdviserFriend = "onClientAdviserFriend";

    //客户端顾问公会好友度
    //顾问ID id
    public static string OnClientAdviserGuildList = "onClientAdviserGuildList";

    //客户端请求顾问信息 
    public static string OnClientAdviserList = "onClientAdviserList";

    //客户端请求公会顾问友好度
    //公会ID
    public static string OnClientGuildAdviser = "onClientGuildAdviser";

    //客户端请求公会顾问友好度
    //顾问ID id  拉拢ID （1，2）
    public static string OnClientAdviserRope = "onClientAdviserRope";
    


    public GuildProxy()
        : base(ProxyID.Clone)
    {

        KBEngine.Event.registerOut(this, "onFindCamp");//返回公会列表
        KBEngine.Event.registerOut(this, "onResponse");//返回申请结果
        KBEngine.Event.registerOut(this, "onGetGuildInfo");//公会信息
        KBEngine.Event.registerOut(this, "onGuildApplyList");//公会申请信息
        KBEngine.Event.registerOut(this, "onApplyGuildIDList");//已经申请过的 公会ID
        KBEngine.Event.registerOut(this, "onGuildMemberList");//返回公会成员列表
        KBEngine.Event.registerOut(this, "onGuildViceIntroduce");//获取公会副会长及简介
        KBEngine.Event.registerOut(this, "onClientGuildBuildInfo");//返回公会建筑信息
        KBEngine.Event.registerOut(this, "onGuildError");//公会返回错误信息 
        KBEngine.Event.registerOut(this, "onGuildUpgradeSucc");//公会升级成功
        KBEngine.Event.registerOut(this, "onChangeNameSucc");//公会返回修改公会名字
        KBEngine.Event.registerOut(this, "onChangeNoticeSucc");//公会返回修改公告
        KBEngine.Event.registerOut(this, "onGuildReadyDismiss");//公会准备解散
        KBEngine.Event.registerOut(this, "onGuildDonateSucc");//公会捐钱成功
        KBEngine.Event.registerOut(this, "onGuilRefreshMemeber");//公会刷新成员信息
        KBEngine.Event.registerOut(this, "onClientGuildFunds");//公会刷新公会资金
        KBEngine.Event.registerOut(this, "onKictOutSucc");//公会开除
        KBEngine.Event.registerOut(this, "onBuyGuildProtectSucc");//购买保护时间成功
        
        KBEngine.Event.registerOut(this, "onAdviserError");//顾问处理结果
        KBEngine.Event.registerOut(this, "onAdviserGuildRank");//顾问公会
        KBEngine.Event.registerOut(this, "onAdviserList");//顾问信息
        KBEngine.Event.registerOut(this, "onGuildAdviserList");//公会顾问友好度
        KBEngine.Event.registerOut(this, "onUpdataAdviserGuild");//更新顾问公会友好度
        KBEngine.Event.registerOut(this, "onUpdateGuildAdviser");//更新公会顾问友好度




    }

    //获取公会副会长及简介
    public void onGuildViceIntroduce(List<object> list,object val)
    {

        string introduce = GameConvert.StringConvert(val);
        List<GuildMemberInfo> listMember = new List<GuildMemberInfo>();
        GuildMemberInfo info = null;
        for (int i=0; i<list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;
            info = ChangeMemberData(data);
            listMember.Add(info);
        }

        GuildInfoMediator.guildinfoMediator.SetGuildInfo(introduce, listMember);
    }

    //公会申请信息
    public void onGuildApplyList(List<object> list)
    {
        MyGuildInfo info = GuildMainMediator.mMyGuild;
        info.applyList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;
            GuildApplyInfo apply = ChangeApplyData(data);
            info.applyList.Add(apply);
        }

        if (GUIManager.HasView("guildofficepanel"))
        {
            GuildOfficeMediator.guildOfficeMediator.SetApplyGridData();
        }
    }

    //公会信息
    public void onGetGuildInfo(object val, object val1, object val2, object val3, object val4, object val5,object val6, object val7, object val8, object val9, object val10, List<object> list)
    {

        Facade.SendNotification(NotificationID.GuildList_Hide);

        MyGuildInfo info = GuildMainMediator.mMyGuild;
        info.level = GameConvert.IntConvert(val);
        info.name = GameConvert.StringConvert(val1);
        info.memberNum = GameConvert.IntConvert(val2);
        info.guildFunds = GameConvert.IntConvert(val3);
        info.reputation = GameConvert.IntConvert(val4);
        info.notice = GameConvert.StringConvert(val5);
        info.dismissTime = GameConvert.LongConvert(val6);
        info.id = GameConvert.IntConvert(val7);
        info.introduction = GameConvert.StringConvert(val8);
        info.protectTime = GameConvert.IntConvert(val9);
        info.ropleTime = GameConvert.IntConvert(val10);


        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;
            int id = GameConvert.IntConvert(data["id"]);
            GuildBuildInfo build = GuildBuildConfig.GetGuildBuildInfo(id);
            build.id = id;
            build.level = GameConvert.IntConvert(data["level"]);
            build.state = GameConvert.IntConvert(data["state"]);
            build.endTimes = GameConvert.LongConvert(data["endTime"]);
        }
        Facade.SendNotification(NotificationID.GuildMain_Show);

        //if (GUIManager.HasView("guildmainpanel"))
        //{
        //    GuildMainMediator.guildmainMediator.SetBuild();
        //}
    }

    //返回公会建筑信息
    public void onClientGuildBuildInfo(Dictionary<string, object> data)
    {
        int id = GameConvert.IntConvert(data["id"]);
        GuildBuildInfo info = GuildBuildConfig.GetGuildBuildInfo(id);
        info.id = id;
        info.level = GameConvert.IntConvert(data["level"]);
        info.state = GameConvert.IntConvert(data["state"]);
        info.endTimes = GameConvert.LongConvert(data["endTime"]);

        if (GUIManager.HasView("guildmainpanel"))
        {
            GuildMainMediator.guildmainMediator.SetBuildInfo(info);
        }
    }
    //公会成员列表信息
    public void onGuildMemberList(List<object> list)
    {
        MyGuildInfo guild = GuildMainMediator.mMyGuild;
        guild.memberList.Clear();

       GuildMemberInfo info = null;
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;
            info = ChangeMemberData(data);
            guild.memberList.Add(info);
        }

        if (GUIManager.HasView("guildofficepanel"))
        {
            GuildOfficeMediator.guildOfficeMediator.SetMemberGridData();
        }
        if (GUIManager.HasView("guilddonationpanel"))
        {
            GuildDonationMediator.guilddonationMediator.SetDayDonateData();
        }
    }
    //已经申请过的 公会ID
    public void onApplyGuildIDList(List<object> list)
    {
        GuildBaseConfig.mApplyGuildIdList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            int guildId = GameConvert.IntConvert(list[i]);
            GuildBaseConfig.mApplyGuildIdList.Add(guildId);
        }
        GuildListMediator.guildlistMediator.RefreshApplyItem();
    }
    //返回 # 1 创建公会成功  2 已经是成员 3已经申请过了 4申请成功！等待公会管理回应 5  # 修改公告成功 6  # 离开公会 7 # 弹劾成功 8 捐赠成功

    public void onResponse(object val)
    {
        int result = GameConvert.IntConvert(val);

        if (result == 13 && PlayerMediator.playerInfo.guildPower != 5)
        {//被开除公会
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild20"));
            CloseAllGuildPanel();
        }
        else if (result == 14 && PlayerMediator.playerInfo.guildPower == 5)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild1"));
            Facade.SendNotification(NotificationID.GuildCreat_Hide);
        }
        else if(result ==16)//解散
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild" + result));
        }
        else if (result == 11)//转让
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild" + result));
            Facade.SendNotification(NotificationID.GuildInteract_Hide);
        } 
        else if(result == 15)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild" + result));
            ServerCustom.instance.SendClientMethods("onClientGetGuildValueRank", 0);
        }
        else if(result == 17)
        {
            GuildMainMediator.mMyGuild.dismissTime = 0;
            if (GUIManager.HasView("guildmainpanel"))
                GuildMainMediator.guildmainMediator.SetDismissTime();
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild" + result));

        }
        else if(result == 18)
        {
            CloseAllGuildPanel();
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild" + result));

        }
        else           
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild" + result));

    }

    //返回公会错误信息
    // 1  公会名字不符合规则 ,2 公会简介不符合规则, 3 钻石不足, 4重复的名字 , 5已经加入公会, 6不存在的公会 ,
    //7不在公会中, 8已经申请了, 9 公会简介不符合规则,10  还没加入公会,11没有权利 , 12公会人员已满，请腾出空间再试！,13领袖离线时间不到7天 14欧元不足
    //15 该职位人员已满，请腾出空间再试！

    public void onGuildError(object val)
    { 
        int errorID = GameConvert.IntConvert(val);
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err" + errorID));
    }
    //返回公会列表
    public void onFindCamp(object val,List<object> list)
    {
        int amount = GameConvert.IntConvert(val);
        GuildBaseConfig.mGuildList.Clear();

        GuildInfo info;
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;
            info = ChangeGuildInfo(data);
            GuildBaseConfig.mGuildList.Add(info);
        }

        GuildListMediator.guildlistMediator.SetData();
    }

    //公会升级成功
    public void onGuildUpgradeSucc(object val,object val1)
    {
        int level = GameConvert.IntConvert(val);
        GuildMainMediator.mMyGuild.level = level;
        GuildMainMediator.mMyGuild.reputation = GameConvert.IntConvert(val1); ;

        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild10"));
        if (GUIManager.HasView("guildmainpanel"))
        {
            GuildMainMediator.guildmainMediator.SetGuildReputation();

        }
    }

    //返回公会公告
    public void onChangeNoticeSucc(object val ,object val1)
    {
        GuildMainMediator.mMyGuild.notice = GameConvert.StringConvert(val);
        GuildMainMediator.mMyGuild.introduction = GameConvert.StringConvert(val1);

        if (GUIManager.HasView("guildmainpanel"))
        {
            GuildMainMediator.guildmainMediator.SetGuildNotice();

        }

    }

    //返回公会名字
    public void onChangeNameSucc(object val)
    {
        GuildMainMediator.mMyGuild.name = GameConvert.StringConvert(val);
        if (GUIManager.HasView("guildmainpanel"))
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild12"));
            GuildMainMediator.guildmainMediator.SetGuildName();
        }

    }

    //捐钱成功
    public void onGuildDonateSucc(object val, Dictionary<string, object> data)
    {
        GuildMainMediator.mMyGuild.guildFunds = GameConvert.IntConvert(val);
        GuildMemberInfo info = ChangeMemberData(data);
        for(int i = 0; i<GuildMainMediator.mMyGuild.memberList.Count; i++)
        {
            GuildMemberInfo item = GuildMainMediator.mMyGuild.memberList[i];
            if (item.id == info.id)
            {
                GuildMainMediator.mMyGuild.memberList.Remove(item);
                GuildMainMediator.mMyGuild.memberList.Add(info);
            }
        }
        if (GUIManager.HasView("guildmainpanel"))
        {
            GuildMainMediator.guildmainMediator.SetGuildFunds();
        }

        if (GUIManager.HasView("guilddonationpanel"))
        {
            GuildDonationMediator.guilddonationMediator.SetDayDonateData();
            GuildDonationMediator.guilddonationMediator.SetDonationInfo();
        }
    }
    //公会刷新成员信息
    public void onGuilRefreshMemeber(Dictionary<string, object> data)
    {
        GuildMemberInfo info = ChangeMemberData(data);
        for (int i = 0; i < GuildMainMediator.mMyGuild.memberList.Count; i++)
        {
            GuildMemberInfo item = GuildMainMediator.mMyGuild.memberList[i];
            if (item.id == info.id)
            {
                GuildMainMediator.mMyGuild.memberList.Remove(item);
                GuildMainMediator.mMyGuild.memberList.Add(info);
            }
        }
        
        if (GUIManager.HasView("guildofficepanel"))
        {
            GuildOfficeMediator.guildOfficeMediator.SetMemberGridData();
        }
        if (GUIManager.HasView("guilddonationpanel"))
        {
            GuildDonationMediator.guilddonationMediator.SetDayDonateData();
            GuildDonationMediator.guilddonationMediator.SetDonationInfo();
        }
        if (GUIManager.HasView("guildinteractpanel"))
        {
            if (GuildInteractMediator.guildinteractMediator.mInfo.id != info.id)
                return;
            GuildInteractMediator.guildinteractMediator.mInfo = info;
            GuildInteractMediator.guildinteractMediator.SetPowerGridDate();
        }
    }


    //公会刷新公会资金
    public void onClientGuildFunds(object val)
    {
        int  funds = GameConvert.IntConvert(val);
        GuildMainMediator.mMyGuild.guildFunds = funds;
        if (GUIManager.HasView("guildmainpanel"))
        {
            GuildMainMediator.guildmainMediator.SetGuildFunds();
        }
    }
    //公会开除
    public void onKictOutSucc(object val)
    {
        int roleId = GameConvert.IntConvert(val);
        for(int i =0; i<GuildMainMediator.mMyGuild.memberList.Count; i++)
        {
            GuildMemberInfo info = GuildMainMediator.mMyGuild.memberList[i];
            if (info.id == roleId)
                GuildMainMediator.mMyGuild.memberList.Remove(info);
        }

        if (GUIManager.HasView("guildofficepanel"))
        {
            GuildOfficeMediator.guildOfficeMediator.SetMemberGridData();
        }
        if (GUIManager.HasView("guilddonationpanel"))
        {
            GuildDonationMediator.guilddonationMediator.SetDayDonateData();
            GuildDonationMediator.guilddonationMediator.SetDonationInfo();
        }
        if (GUIManager.HasView("guildinteractpanel"))
        {
            GuildInteractMediator.guildinteractMediator.SetPowerGridDate();
        }

    }
    //公会解散
    public void onGuildReadyDismiss(object val)
    {
        GuildMainMediator.mMyGuild.dismissTime = GameConvert.LongConvert(val);
        if (GUIManager.HasView("guildmainpanel"))
        {
            GuildMainMediator.guildmainMediator.SetDismissTime();
        }
    }
    //公会保护购买成功
    public void onBuyGuildProtectSucc(object val)
    {
        GuildMainMediator.mMyGuild.protectTime = GameConvert.IntConvert(val);
        if(GuildTacticMediator.guildtacticMediator!=null)
        {
            GuildTacticMediator.guildtacticMediator.SetCutOffTime();
        }
    }

    //顾问处理失败
    public void onAdviserError(object val)
    {
        int id = GameConvert.IntConvert(val);
    }
    //顾问信息
    public void onAdviserList(List<object> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;

            AdviserInfo info = changeAdviserData(data);

            if (!GuildCounselorConfig.mAdviserDict.ContainsKey(info.dbid))
                GuildCounselorConfig.mAdviserDict.Add(info.dbid, info);
            else
                GuildCounselorConfig.mAdviserDict[info.dbid] = info;
            
        }

        if (GuildCounselorMediator.guildcounselorMediator != null)
        {
            ServerCustom.instance.SendClientMethods(OnClientGuildAdviser,PlayerMediator.playerInfo.guildDBID);
        }

    }
    //顾问公会好友度列表
    public void onAdviserGuildRank(List<object> list)
    {

        GuildCounselorConfig.mGuildFriendsList.Clear();
        for(int i=0; i<list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;

            AdviserGuildInfo info = ChangeAdviserGuildData(data);
            GuildCounselorConfig.mGuildFriendsList.Add(info);
        }
        if (GuildCounselorMediator.guildcounselorMediator != null)
        {
            GuildCounselorMediator.guildcounselorMediator.SetAdviserGuildList();
        }

    }
    //更新顾问公会好友度
    public void onUpdataAdviserGuild(Dictionary<string, object> data)
    {
        AdviserGuildInfo adviserGuild = ChangeAdviserGuildData(data);

        for(int i=0; i < GuildCounselorConfig.mGuildFriendsList.Count; i++)
        {
            AdviserGuildInfo info = GuildCounselorConfig.mGuildFriendsList[i];

            if(info.guildID == adviserGuild.guildID)
                GuildCounselorConfig.mGuildFriendsList.Remove(info);
        }

        GuildCounselorConfig.mGuildFriendsList.Add(adviserGuild);

        if (GuildCounselorMediator.guildcounselorMediator != null)
        {
            GuildCounselorMediator.guildcounselorMediator.SetAdviserGuildList();
        }

    }
    //公会顾问友好度
    public void onGuildAdviserList(List<object> list)
    {
        GuildMainMediator.mMyGuild.adviserList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;

            GuildAdviser info = ChangeGuildAdviserData(data);
            GuildMainMediator.mMyGuild.adviserList.Add(info);
        }
        if(GuildCounselorMediator.guildcounselorMediator!=null)
        {
            GuildCounselorMediator.guildcounselorMediator.SetAdviserList();
        }
    }

    //更新公会顾问好友度
    public void onUpdateGuildAdviser(Dictionary<string, object> data)
    {
        GuildAdviser info = ChangeGuildAdviserData(data);
        for(int i=0; i < GuildMainMediator.mMyGuild.adviserList.Count; i++)
        {
            GuildAdviser item = GuildMainMediator.mMyGuild.adviserList[i];
            if (item.id == info.id)
                item.friendliness = info.friendliness;
        }
        
        if (GuildCounselorMediator.guildcounselorMediator != null)
        {
            GuildCounselorMediator.guildcounselorMediator.SetAdviserList();
        }
    }
    //公会数据转换
    private GuildInfo ChangeGuildInfo(Dictionary<string, object> data)
    {
        GuildInfo info = new GuildInfo();

        info.id = GameConvert.IntConvert(data["dbid"]); 
        info.guildName = GameConvert.StringConvert(data["guildName"]);
        info.camp = GameConvert.IntConvert(data["camp"]); 
        info.level = GameConvert.IntConvert(data["level"]); 
        info.count = GameConvert.IntConvert(data["count"]); 
        info.leader = GameConvert.StringConvert(data["leader"]); 
        return info;

    }

    //成员数据转换
    private GuildMemberInfo ChangeMemberData(Dictionary<string, object> data)
    {
        GuildMemberInfo info = new GuildMemberInfo();

        info.id = GameConvert.IntConvert(data["dbid"]); 
        info.playerName = GameConvert.StringConvert(data["playerName"]);
        info.power = GameConvert.IntConvert(data["power"]); 
        info.level = GameConvert.IntConvert(data["level"]); 
        info.offical = GameConvert.IntConvert(data["offical"]); 
        info.dayDonate = GameConvert.IntConvert(data["dayDonate"]); 
        info.weekDonate = GameConvert.IntConvert(data["weekDonate"]); 
        info.sumDonate = GameConvert.IntConvert(data["sumDonate"]); 
        info.onlineState = GameConvert.IntConvert(data["onlineState"]);         
        return info;
    }
    //申请数据转换
    private GuildApplyInfo ChangeApplyData(Dictionary<string, object> data)
    {
        GuildApplyInfo apply = new GuildApplyInfo();
        apply.id = GameConvert.IntConvert(data["dbid"]);
        apply.playerName = GameConvert.StringConvert(data["playerName"]);
        apply.offical = GameConvert.IntConvert(data["offical"]);
        apply.level = GameConvert.IntConvert(data["level"]);
        apply.applyTime = GameConvert.DoubleConvert(data["applyTime"]);
        return apply;
    }
    //顾问数据转换
    private AdviserInfo changeAdviserData(Dictionary<string, object> data)
    {
        AdviserInfo info = new AdviserInfo();

        info.dbid = GameConvert.IntConvert(data["dbid"]);
        info.guildDBID = GameConvert.IntConvert(data["guidDBID"]);
        info.configID = GameConvert.IntConvert(data["configID"]);
        info.guilidName = GameConvert.StringConvert(data["guildName"]);
        info.friendliness = GameConvert.IntConvert(data["friendliness"]);

        return info;
    }
    //顾问公会好友度转换
    private AdviserGuildInfo ChangeAdviserGuildData(Dictionary<string, object> data)
    {
        AdviserGuildInfo info = new AdviserGuildInfo();

        info.guildID = GameConvert.IntConvert(data["guildID"]);
        info.guildName = GameConvert.StringConvert(data["guildName"]);
        info.friendliness = GameConvert.IntConvert(data["friendliness"]);
        
        return info;
    }

    //公会顾问友好度
    private GuildAdviser ChangeGuildAdviserData(Dictionary<string, object> data)
    {
        GuildAdviser info = new GuildAdviser();
        
        info.id = GameConvert.IntConvert(data["dbid"]);
        info.friendliness = GameConvert.IntConvert(data["friendliness"]);
        return info;

    }


    //关闭公会界面
    private void CloseAllGuildPanel()
    {
        Facade.SendNotification(NotificationID.GuildMain_Hide);
        Facade.SendNotification(NotificationID.GuildAlterName_Hide);
        Facade.SendNotification(NotificationID.GuildAlterNotice_Hide);
        Facade.SendNotification(NotificationID.GuildOffice_Hide);
        Facade.SendNotification(NotificationID.GuildSpeed_Hide);
        Facade.SendNotification(NotificationID.GuildTactic_Hide);
        Facade.SendNotification(NotificationID.GuildLVUp_Hide);
        Facade.SendNotification(NotificationID.GuildInteract_Hide);
        Facade.SendNotification(NotificationID.GuildCounselor_Hide);

    }

}


public class GuildInfo
{
    public int rank;
    public int id;                    //公会ID
    public string guildName;      // 公会名字
    public string playerName;      // 公会名字
    public int camp;       // 阵营
    public int ranking;     // 排名
    public int reputation;  // 声望
    public int level;        // 等级 
    public int count;          // 人数
    public string leader;    // 名字
    public int guildFunds;    // 名字
}

//公会申请成员信息
public class GuildApplyInfo
{
    public int id;                    //玩家DBID
    public string playerName;      // 玩家名字
    public int offical;       // 官职
    public int level;        // 等级 
    public double applyTime; //申请时间

}

//自己公会信息
public class MyGuildInfo
{
    public int id;
    public string name;//公会名字
    public int level;        // 等级 
    public int memberNum;    // 公会人数
    public int guildFunds;        // 公会资金
    public int reputation;        // 公会声望
    public string notice=string.Empty;        // 公会公告
    public long dismissTime;  // 公会解散时间
    public string introduction = string.Empty;// 公会简介
    public int protectTime;  // 公会保护时间
    public int ropleTime;  // 拉拢次数

    public List<GuildApplyInfo> applyList = new List<GuildApplyInfo>(); //申请列表
    public List<GuildMemberInfo> memberList = new List<GuildMemberInfo>(); //公会成员列表
    public List<GuildAdviser> adviserList = new List<GuildAdviser>(); //顾问列表
}


//公会成员信息
public class GuildMemberInfo
{

    public int id;                    //玩家DBID
    public string playerName;      // 玩家名字
    public int power;                    //公会职务
    public int offical;       // 官职
    public int level;        // 等级
    public int dayDonate;     //每日贡献
    public int weekDonate;        // 本周贡献 
    public int sumDonate;        // 累积贡献
    public int onlineState;        // 状态，离开，在帮会

}

//公会建筑
public class GuildBuildInfo
{
    public int id;
    public int level;        // 等级 
    public int state;    //状态
    public long endTimes;//升级结束时间
}

//顾问信息
public class AdviserInfo
{
    public int dbid;   //dbid
    public int configID; 
    public int guildDBID;
    public string guilidName;
    public int friendliness;

}


//公会顾问信息
public class GuildAdviser
{
    public int id;//顾问id
    public int friendliness;//友好度
}

//顾问公会信息
public class AdviserGuildInfo
{
    public int guildID;
    public int rank;
    public string guildName;
    public int friendliness;
}

