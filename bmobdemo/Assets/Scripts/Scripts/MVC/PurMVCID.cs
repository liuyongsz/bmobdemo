using System;
using System.Collections.Generic;
using System.Text;
using XLua;

[LuaCallCSharp]
public enum ProxyID
{
    NONE,
    Baby,
    Game,
    Login,
    Player,
    Chat,
    Mail,
    Store,
    World,
    Card,
    Friend,
    Power,
    Bag,
    Baller,
    Clone, //副本
    Equip,
    Formation,
    Skill,
    Chapter,
    GameShop,
    Rank,
    Arena,
    WorldBoss,
}
/// <summary>
/// 所有通知ID注册类
/// </summary>
[LuaCallCSharp]
public enum NotificationID
{
    START_UP,
    #region //框架
    START_GAME,
    START_UP_PUREMVC,
    CHANGE_SCENE,
    UPDATE_SCENE_MEDIATOR,
    SCENE_CHANGE_FINISH, //场景切换完成
    PLAYERMAKER_EVENT_MSG,//playermaker 事件消息

    UPDATE_MESSAGE,        //更新消息
    UPDATE_EXTRACT,        //更新解包
    UPDATE_DOWNLOAD,       //更新下载
    UPDATE_PROGRESS,       //更新进度
#endregion

#region
MATCH_OVER, //比赛结束
    #endregion

    #region 测试界面
    test_msg,
    open_test_panel,
    close_test_panel,
    open_mytest_panel,
    open_panel_tt,
    #endregion


    #region 界面操作 
    NULL,   
    Login_Show,
    Login_Hide,
    LOGIN_RECSERVERLIST,//接受到服务列表
    LOGIN_SERVERNOCHANGE,//选择的服务列表发送改变
    Show_MessageBox,
    Hide_MessageBox,
    Show_Main,
    Hide_Main,
    Fight_Change,
    BallerFight_Change,
    Vip_Change,
    Level_Change,
    Formation_Change,
    Chat_RecChatInfo,


    Role_Show,
    Role_Hide,
    Mail_Show,
    Mail_Hide,
    SystemText_Show,
    SystemText_Hide,
    Prompt_Show,
    Prompt_Hide,
    Store_Show,
    Store_Hide,
    Gold_Show,
    Gold_Hide,
    Create_Show,
    Create_Hide,
    Card_Show,
    Card_Hide,
    Friend_Hide,
    Friend_Show,
    Power_Show,
    Power_Hide,
    Chat_Show,
    Chat_Hide,
    Bag_Show,
    Bag_Hide,
    Baby_Show,
    Baby_Hide,
    ItemInfo_Show,
    ItemInfo_Hide,
    Team_Show,
    Team_Hide,
    Sever_Show,
    Sever_Hide,
    EquipInset_Show,
    EquipInset_Hide,
    EquipInherit_Show,
    EquipInherit_Hide,
    EquipStar_Show,
    EquipStar_Hide,
    EquipStrong_Show,
    EquipStrong_Hide,
    EquipStrongResult_Show,
    EquipStrongResult_Hide,
    EquipMake_Show,
    EquipMake_Hide,
    EquipMain_Show,
    EquipMain_Hide,
    EquipChoose_Show,
    EquipChoose_Hide,
    GemChoose_Show,
    GemChoose_Hide,
    GemCompound_Show,
    GemCompound_Hide,
    EquipInformation_Show,
    EquipInformation_Hide,
    BallerLevel_Show,
    BallerLevel_Hide,

    CheckPoint_Show,
    CheckPoint_Hide,
    Clone_Inflo,

    BattleEnd_Show,
    BattleEnd_Hide,

    TeamFormine_Show,
    TeamFormine_Hide,
    Bench_Show,
    Bench_Hide,
    BenchChoose_Show,
    BenchChoose_Hide,
    FormationSysActive_Show,
    FormationSysActive_Hide,
    GameShop_Show,
    GameShop_Hide,
    BuyItem_Show,
    BuyItem_Hide,
    Rank_Show,
    Rank_Hide,
    GuildList_Show,
    GuildList_Hide,
    GuildCreat_Show,
    GuildCreat_Hide,
    GuildInfo_Show,
    GuildInfo_Hide,
    GuildMain_Show,
    GuildMain_Hide,
    GuildDonation_Show,
    GuildDonation_Hide,
    GuildOffice_Show,
    GuildOffice_Hide,
    GuildLVUp_Show,
    GuildLVUp_Hide,
    GuildSpeed_Show,
    GuildSpeed_Hide,
    GuildAlterName_Show,
    GuildAlterName_Hide,
    GuildAlterNotice_Show,
    GuildAlterNotice_Hide,
    GuildAlterInfo_Show,
    GuildAlterInfo_Hide,
    GuildInteract_Show,
    GuildInteract_Hide,
    GuildTactic_Show,
    GuildTactic_Hide,
    GuildCounselor_Show,
    GuildCounselor_Hide,


    Arena_Show,
    Arena_Hide,
    WorldBoss_Show,
    WorldBoss_Hide,
    RankReward_Show,
    RankReward_Hide,
    #endregion

    #region 注册页面
    REGISTER_CLICKITEM,
    #endregion

    #region 球操作界面

    BALLOPER_OPEN,
    BALLOPER_CLOSE,
    #endregion

    #region 球操作界面

    OPERTEST_OPEN,
    OPERTEST_CLOSE,
    #endregion

    #region 比赛信息界面
    MatchInfo_Open,
    MatchInfo_Close,
    MatchInfo_Score, //比分信息
    MatchInfo_Timer, //时间
    MatchInfo_ShowJudge,
    MatchInfo_AddBuff,    //增加buff
    MatchInfo_DelBuff,    //减少buff
    MatchInfo_CardList,   //技能列表
    MatchInfo_SkillCall,  //技能呐喊
    #endregion
    #region 沙盘
    SandTable_Show,
    SandTable_Hide,
    #endregion

    #region 比赛信息
    Match_WinSide,  //获取胜利的方
    #endregion

    #region 背包刷新
   BagRefresh,  //背包刷新
    #endregion
    #region 资源更新UI
    UpdateResources_Open,
    UpdateResources_Close,
    #endregion
}
