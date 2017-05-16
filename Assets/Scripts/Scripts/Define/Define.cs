using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 定义
/// </summary>
public class Define{

    /// <summary>启动增量更新模式</summary>
    public static bool UpdateMode = false;                       //更新模式-默认关闭 
    public static bool UseLoacalRes = true;                    //是否使用本地资源
    public static bool WaiWangDebug = false;                    //外网测试
    public const string WaiWangURL = "http://16k7u48531.imwork.net:27072/ClientRes/";

    public const bool DebugMode = false;                       //调试模式-用于内部测试
    public const int GameFrameRate = 26;                        //游戏帧频
    public const string AppName = "football";               //应用程序名称
    public const string AssetDir = "StreamingAssets";           //素材目录 
    public const string AppPrefix = AppName + "_";              //应用程序前缀
    public const bool LuaBundleMode = true;                     //Lua代码AssetBundle模式
   
    public static string FrameworkRoot
    {
        get
        {
            return Application.dataPath + "/" + AppName;
        }
    }

    public static Vector3 NullPos = new Vector3(0, -100f, 0);

    /// <summary>球tag</summary>
    public const string Tag_Ball = "Ball";
    /// <summary>己方球员Tag</summary>
    public const string Tag_PlayerTeam = "PlayerTeam1";
    /// <summary>对手球员Tag</summary>
    public const string Tag_OponentTeam = "OponentTeam";
    /// <summary>场景Tag</summary>
    public const string Tag_Scenary = "Scenary";
    /// <summary>球员的头tag</summary>
    public const string Tag_Head = "Bip001 Head";
    /// <summary>球员的手tag</summary>
    public const string Tag_Hand = "Bip001 R Hand";
    /// <summary>守门员tag</summary>
    public const string Tag_GoalKeeper_Right = "GoalKeeper";
    /// <summary>防守守门员tag</summary>
    public const string Tag_GoalKeeper_Left = "GoalKeeper_Opponent";
    /// <summary>防守守门员tag</summary>
    public const string Tag_GoalKeeper = "GoalKeeper_Opponent";
    /// <summary>防守守门员tag</summary>
    public const string Tag_GoalKeeper_Opponent = "GoalKeeper"; 

    /// <summary>地面tag</summary>
    public const string Tag_Ground = "ground";

    /// <summary>动作名字 休息</summary>
    public const string AniName_Rest = "stand";
    /// <summary>动作名字 休息</summary>
    public const string AniName_KeeperRest = "standshoumen";
    /// <summary>动作名字 跑路</summary>
    public const string AniName_Running = "wuqiupaodong";
    /// <summary>动作名字 跑路</summary>
    public const string AniName_RunZuo = "runzuo";
    /// <summary>动作名字 跑路</summary>
    public const string AniName_RunYou = "runyou";
    /// <summary>动作名字 跑路</summary>
    public const string AniName_RunHou = "runhou";
    /// <summary>动作名字 跑路</summary>
    public const string AniName_RunHeng = "runheng";
    /// <summary>动作名字 带球跑动</summary>
    public const string AniName_RunningWithBall = "daiqiupaodong";
    /// <summary>动作名字 传球</summary>
    public const string AniName_Pass = "duanchuan";
    /// <summary>动作名字 铲球</summary>
    public const string AniName_Tackle = "changqiu";
    /// <summary>动作名字 铲球被动</summary>
    public const string AniName_ChangQiuBeidong = "changqiu_beidong";
    /// <summary>动作名字 警惕</summary>
    public const string AniName_Alert = "stand";
    /// <summary>动作名字 走路</summary>
    public const string AniName_Walk = "chouse";
    /// <summary>动作名字 无求跑动</summary>
    public const string AniName_WuQiuPaoDong = "wuqiupaodong";
    /// <summary>动作名字 倒地扑球</summary>
    public const string AniName_DaodiPuQiu = "daodipuqiu";
    /// <summary>动作名字 飞身扑球</summary>
    public const string AniName_FeiShenPuQiu = "feishenpuqiu";
    /// <summary>动作名字 转向1</summary>
    public const string AniName_ZhuanXiang1 = "zhuanxiang1";
    /// <summary>动作名字 无球跑动加速</summary>
    public const string AniName_WuqiupaodongJiasu = "wuqiupaodong_jiasu";
    /// <summary>动作名字 带球跑动加速</summary>
    public const string AniName_DaiqiupaodongJiasu = "daiqiupaodong_jiasu";
    /// <summary>动作名字 接球</summary>
    public const string AniName_JieQiu = "jieqiu";
    /// <summary>动作名字 拦截求</summary>
    public const string AniName_LanJieQiu = "lanjieqiu";
    /// <summary>动作名字 过人</summary>
    public const string AniName_GuoRen1 = "guoren1";
    /// <summary>动作名字 过人被动</summary>
    public const string AniName_GuoRenBeiDong = "guoren_beidong";
    /// <summary>动作名字 推射</summary>
    public const string AniName_TuiShe = "tuishe";
    /// <summary>动作名字 射门</summary>
    public const string AniName_Chouse = "chouse";
    /// <summary>动作名字 踩单车</summary>
    public const string AniName_CaiDanChe = "caidanche";
    /// <summary>动作名字 踩单车被动</summary>
    public const string AniName_CaiDanCheBeiDong = "caidanche_beidong";
    /// <summary>动作名字 右切被动</summary>
    public const string AniName_QieQiuYou = "qieqiuyou";
    /// <summary>动作名字 左切被动</summary>
    public const string AniName_QieQiuZuo = "qieqiuzuo";
    /// <summary>动作名字 后转</summary>
    public const string AniName_HouZhuan = "zhuanhou";
    /// <summary>动作名字 彩虹过人</summary>
    public const string AniName_CaiHongGuoRen = "caihongguoren";
    /// <summary>动作名字 超车</summary>
    public const string AniName_ChaoChe = "chaoche";
    /// <summary>动作名字 超车被动</summary>
    public const string AniName_ChaoCheBeiDong = "chaoche_beidong";
    /// <summary>动作名字 卧草战术</summary>
    public const string AniName_WoCao = "wocao";
    /// <summary>动作名字 卧草战术</summary>
    public const string AniName_BaoLiChangQiuBeiDong = "baolichangqiu_beidong";
    /// <summary>动作名字 守门员出击</summary>
    public const string AniName_ShouMenYuanChuJi = "shoumenyuanchuji";
    /// <summary>动作名字 肘击</summary>
    public const string AniName_ZhouJi = "zhouji";
    /// <summary>动作名字 肘击被动</summary>
    public const string AniName_ZhouJiBeiDong = "zhouji_beidong";
    /// <summary>动作名字 兰花指突破</summary>
    public const string AniName_LanHuZhiTuPo = "lanhuazhitupo";

    /// <summary>角色移动速度</summary>
    public const float speedForAnimation = 6.0f;
    /// <summary>最大行走速度</summary>
    public const float speedWalkMax = 0.5f;
    /// <summary>球员离球可偷取范围</summary>
    public const float ballStoleDis = 0.8f;
    /// <summary>球员过人的时候 球离右叫的距离</summary>
    public const float ballGuoRenDis = 0.6f;

    /// <summary>回合基数</summary>
    public const float match_roundBase = 12;
    /// <summary>球的最小高度</summary>
    public const float ballMinY = 2.0f;
    /// <summary>球员带球的时候和球的最小距离</summary>
    public const float ballForwardDis = 0.8f;
    /// <summary>球员带球判断跳跃最小距离</summary>
    public const float goalKeeperDecideJumpDis = 6f;
    /// <summary>球员带球判断跳跃X最小距离 0.4old</summary>
    public const float goalKeeperDecideJumpDisX = 0f;
    /// <summary>球员带球判断跳跃y最小距离</summary>
    public const float goalKeeperDecideJumpDisY = 1.1f;
    /// <summary>球员之间的距离</summary>
    public const float KeeperBetwwenDis = 2.5f;
    /// <summary>球员之间的距离</summary>
    public const float KeeperBetwwenDis2 = 1.6f;

    /// <summary>点最小范围</summary>
    public const float minDotNearDis = 2.0f;
    /// <summary>初始化球员的时候 球员之间最小的距离</summary>
    public const float minPlayerDis = 12f;

    /// <summary>抽射的距离</summary>
    public const float ChouseInRange = 10f;
    /// <summary>决定跳跃时间比</summary>
    public const float JumpTimerNormalized = 0.45f;
    /// <summary>倒地扑球速度</summary>
    public const float DaodiPuQiuSpeed = 1f;
    /// <summary>飞身扑球速度</summary>
    public const float FeiShenPuQiuSpeed = 1f;
    /// <summary>
    /// 装备所有属性
    /// </summary>
    public const string EquipPropStr = "shoot,pass,reel,defend,trick,steal,control,keep";

    public static float DaodiPuqiuGetNor = 0.17f;
    public static float FeishenPuqiuGetNor = 0.166f;
    public static float JieQiuBTimer = 0.285f;
    public static float ChanqiuBeidongSpeed = 2.5f;
    public static float AvatarScale = 1.5f;
    public static float FstDistance = 10f;
    public static float SecDistance = 8f;
    public static float ThrDistance = 5f;
    public static float JumpDelay = 0.2f;
    public static float TuiSheTimer = 0.28125f;
    public static float TuiSheStopPer = 0.375f;
    public static float CouSheTimer = 0.2f;
    public static float CouSheStopPer = 0.375f;
    public static float ChaDanCheGuoRenPer = 0.25f;
    public static float ChanQiuStopPer = 0.35f;
    public static float PassAniTimer = 0.3f;
    public static float HouZhouStopTimer = 0.65f;
    public static float SuppyPassTimer = 0.8f; //接应球的时候传球的时间
    public static List<int> TuiSheCellPostion = new List<int> { 14, 14, 15, 16, 17, 23, 24, 25, 26, 27};
    //public static List<int> TuiSheCellPostion = new List<int>();
    //public static List<int> TuiSheCellPostion = new List<int> { 14,14,15,16,17,23,24,25,26,27}; 
    /// <summary>球门死角 参考位置</summary>
    public static Vector3 GoalACorner = new Vector3(3.5f,1.7f,0);
    public static Vector3 ShootSuccessRndPos = new Vector3(2.5f, 1f, 0);
    public static Vector3 GoalWideCorner = new Vector3(5f, 4f, 0); //射偏了
}
