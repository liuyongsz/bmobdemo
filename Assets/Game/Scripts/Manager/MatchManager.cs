using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProxyInstance;
using PureMVC.Patterns;
using UnityEngine.SceneManagement;

/// <summary>
/// 赛场管理器
/// </summary>
public class MatchManager : MonoBehaviour
{
    public static CloneGMFlag CloneGMType = CloneGMFlag.no_steal;

    public static bool GameOver = false;
    public static bool RoundEnd = false;
    public static bool CanSwitch = false;
    private static bool m_Stop = false;
    private static List<MatchPlayerItem> m_timeStopOutList;
    public static List<MatchPlayerItem> GoGetBallPlayerFrontList;

    public static Goal LeftGoal;
    public static Goal RightGoal;
    ///<summary>使用补球射们 once</summary>
    public static bool UseFollowShoot;

    /// <summary>本轮开始</summary>
    private static bool m_startRound;

    private static List<object> m_atkIDs;
    private static List<object> i_ids;
    public static List<int> i_poses;
    private static List<int> i_fstList;
    private static List<int> i_secList;
    private static List<int> i_thrList;
    private static object i_keeperId;
    public static List<int> MyCards;
    public static int m_initPlayerCount = 0;
    public static int AttackIndex = 1;
    public static int AtkTimer;

    /// <summary>焦点球员</summary>
    public static MatchPlayerItem FocusPlayer;
    /// <summary>接应的球员</summary>
    public static MatchPlayerItem SupplyPlayer;
    /// <summary>补射球员</summary>
    public static MatchPlayerItem FollowShootPlayer;
    /// <summary>补射结果</summary>
    public static E_StepResult FollowShootResult = E_StepResult.Null;
    /// <summary>技能效果类型</summary>
    public static Skill_EffectType SkillEffectType = Skill_EffectType.Null;

    /// <summary>协防补位球员</summary>
    public static MatchPlayerItem XieFangPlayer;

    private static bool m_canResetScore;

    private static Transform m_leftGoalMesh;

    private static bool m_needCheckLeft;


    private static string Timer_StepOneKey = "Step_OneCountDown";
    [HideInInspector]
    public static Transform LeftGoalMesh
    {
        get
        {
            if(null == m_leftGoalMesh)  m_leftGoalMesh = GameObject.Find("goalmesh_left").transform;

            return m_leftGoalMesh;
        }
    }

    private static Transform m_rightGoalMesh;
    [HideInInspector]
    public static Transform RightGoalMesh
    {
        get
        {
            if (null == m_rightGoalMesh)    m_rightGoalMesh = GameObject.Find("goalmesh_right").transform;

            return m_rightGoalMesh;
        }
    }

    /// <summary>左半场守门员</summary>
    private Transform m_leftShootOutDot;
    /// <summary>右半场守门员</summary>
    private Transform m_rightShootOutDot;

    [HideInInspector]
    /// <summary>左边点球大战 球的放置点</summary>
    public Transform LeftShootOutDot { get { if (null == m_leftShootOutDot) m_leftShootOutDot = GameObject.Find("LeftShootOutDot").transform; return m_leftShootOutDot; } }
    [HideInInspector]
    /// <summary>右边点球大战 球的放置点</summary>
    public Transform RightShootOutDot { get { if (null == m_rightShootOutDot) m_rightShootOutDot = GameObject.Find("RightShootOutDot").transform; return m_rightShootOutDot; } }

    /// <summary>攻击方守门员</summary>
    public static MatchPlayerItem m_leftKeepGoal;
    /// <summary>防守方守门员</summary>
    public static MatchPlayerItem m_rightKeepGoal;

    /// <summary>进攻者</summary>
    public static Dictionary<string,MatchPlayerItem> m_leftMatchData;
    /// <summary>防御者</summary>
    public static Dictionary<string, MatchPlayerItem> m_rightMatchData;
    /// <summary>比赛的球员</summary>
    public static Dictionary<GameObject, MatchPlayerItem> m_matchPlayerDic;

    /// <summary>长</summary>
    public static int Height = 105;
    /// <summary>宽</summary>
    public static int Width = 68;

    /// <summary>回合信息</summary>
	public static List<MatchRoundInfo> m_roundInfo;

    /// <summary>进攻者</summary>
    public static MatchPlayerItem AtkPlayer;
    /// <summary>中间者</summary>
    public static MatchPlayerItem MidPlayer;
    /// <summary>终结者</summary>
    public static MatchPlayerItem EndPlayer;

    [HideInInspector]
    public static MatchManager Instace;

    /// <summary>网格单位长度</summary>
    private static float m_cellX;
    /// <summary>网格单位宽带</summary>
    private static float m_cellZ;
    /// <summary>网格长</summary>
    private static float m_cellLength;
    /// <summary>左半场 布点开始计算点</summary>
    private static Vector3 m_leftCalCornerPoint;
    /// <summary>右班车 布点开始计算点</summary>
    private static Vector3 m_rigthCalCornetPoint;

    /// <summary>第1轮给下一轮的接球者的加成</summary>
    public static E_AtartNodeOper FstChooseOper;
    /// <summary>第2轮给下一轮的接球者的加成</summary>
    public static E_AtartNodeOper SecChooseOper;
    /// <summary>第3轮给下一轮的接球者的加成</summary>
    public static E_AtartNodeOper ThrChooseOper;

    /// <summary>比赛是否结束</summary>
    public static bool IsOver;

    /// <summary>攻向左边</summary>
    public static bool IsAtkLeft = true;
    /// <summary>左边阵型id</summary>
    public int LeftTeamMatchId = 1;
    /// <summary>右边阵型id</summary>
    public int RightTeamMatchId = 2;
    /// <summary>进攻方队伍tag</summary>
    public string AttackTeamTag = "PlayerTeam1";
    /// <summary>防守方队伍tag</summary>
    public string DefendTeamTag = "OponentTeam";

    /// <summary>计算的射门结果</summary>
    public static bool ShootCalResult = false;


    private float AgainTimer = 5f;

    /// <summary>寻路</summary>
    public static MatchAStar m_aStar;

    /// <summary>是否是最后一次进攻</summary>
    public bool IsLastAtk = false;

    /// <summary>左边进球数量</summary>
    public static int LeftGoalNum = 0;
    /// <summary>右边进球数量</summary>
    public static int RightGoalNum = 0;

    /// <summary>当前比赛轮次信息</summary>
    public static MatchRoundInfo m_currentRoundInfo;

    [HideInInspector]
    /// <summary>在点球大战中</summary>
    public static bool InShootOut = false;

    private static string m_timerKey = "MatchOverCountDown";

    /// <summary>当前比赛类型</summary>
    public E_MatchType CurrentType;

    /// <summary>射门飞翔状态</summary>
    public static bool InShootFly;

	private static int m_totalRound = 0;

    public static Dictionary<int, E_StepResult> StepOperResult = new Dictionary<int, E_StepResult>();
    private static Dictionary<int, E_StepResult> NewStepOperResult = new Dictionary<int, E_StepResult>();
    public static E_StepResult SerOperResult {
        get
        {
            E_StepResult res;
            StepOperResult.TryGetValue(m_doStep, out res);

            return res;
        }
    }

    /// <summary>是否初始化过了</summary>
    public static bool NeedInited = true;

    /// <summary>副本ID</summary>
    public int CloneId;

    public static int m_doStep = 0;
    public void Start()
    {
        if(null == Instace)
        {
            Instace = this;
            AllRef.coreGame = CoreGame.Instance;
        }

        CurrentType = E_MatchType.PVE;
    }

    public void SetCloneInfo(int id)
    {
        CloneId = id;
    }

    public void OnLoaded_Complete()
    {
        InitPositon();
        SetCloneInfo(1);
    }

    public void ResetMatchItem()
    {
        if (null != m_leftMatchData)
            foreach (var cItm in m_leftMatchData)
                cItm.Value.ResetData();

        if (null != m_rightMatchData)
            foreach (var cItm in m_rightMatchData)
                cItm.Value.ResetData();
    }

    /// <summary>重置比赛</summary>
    /// atkLeft 是否攻击的方向是左边
    public void ResetMatch(bool atkLeft = true,bool isShootOut = false)
    {
        if (null == AllRef.Ball) return;

        if (null != m_leftMatchData)
            foreach (var cItm in m_leftMatchData)
                cItm.Value.ResetData();

        if (null != m_rightMatchData)
            foreach (var cItm in m_rightMatchData)
                cItm.Value.ResetData();

        AllRef.DoShoot = false;
        TimerManager.Destroy(m_timerKey);
        TimerManager.Destroy(Timer_StepOneKey);

        Facade.Instance.SendNotification(NotificationID.BALLOPER_CLOSE);

        InShootOut = isShootOut;
        if (AllRef.RotateSelector)
            AllRef.RotateSelector.ResetMatch();

        if (m_canResetScore)
        {
            m_canResetScore = false; 
        }

        m_doStep = 0;
        IsOver = false;
        gui_go = false;
        AtkPlayer = null;
        MidPlayer = null;
        EndPlayer = null;
        CanSwitch = false;
        InShootFly = false;
        XieFangPlayer = null;
        FocusPlayer = null;
        SupplyPlayer = null;
        FollowShootPlayer = null;
        m_currentRoundInfo = null;
        GoGetBallPlayerFrontList = null;
        FollowShootResult = E_StepResult.Null;
        SkillEffectType = Skill_EffectType.Null;

        ShootCalResult = false;
        AllRef.HaveTarck = false;
        AllRef.InShootGoal = false;
        SecChooseOper = E_AtartNodeOper.Null;

        if(null != m_aStar)
             m_aStar.Reset();

        MatchManager.Stop = false;
        MyCamera.Instance.MatchReset();
        AllRef.Ball.MatchReset();
        IsAtkLeft = atkLeft;

        CoreGame.Instance.SetState(CoreGame.InGameState.GOAL);
        CoreGame.Instance.goalKeeper_left.MatchReset();
        CoreGame.Instance.goalkeeper_right.MatchReset();

        AttackTeamTag = atkLeft ? Define.Tag_PlayerTeam : Define.Tag_OponentTeam;

        //处于点球大战中
        if (InShootOut)
            SetShootOutPlayer(IsAtkLeft);
        else
            if (null != m_roundInfo && m_roundInfo.Count > 0)
            {
                m_currentRoundInfo = m_roundInfo[0];
                Facade.Instance.SendNotification(NotificationID.MatchInfo_Timer);

                m_roundInfo.RemoveAt(0);
                atkLeft = m_currentRoundInfo.isLeft;
            }
    }

    private void OnTimer_DoStepOne()
    {
        if (m_initPlayerCount > 0 || null == AllRef.Ball)
            return;

        GameOver = false;
        RoundEnd = false;

        StepOperResult = NewStepOperResult;
        m_doStep = 1;

        AllRef.Ball.owner = AtkPlayer.playerBody;
        CommonFun.BallToPlayerFoot(AllRef.Ball.owner.transform);
        TimerManager.Destroy(Timer_StepOneKey);

        Step_One();

        CommonFun.Debug("第一轮开始");
    }

    public void FixedUpdate()
    {
        if(null != Main.Ins)
            Main.Ins.Camera3D.transform.position = Camera.main.transform.position;
    }

    public void Update()
    {
        if (NeedInited
            && MyCards != null
            && AllRef.Ball != null
            && FollowShootResult == E_StepResult.Null
            && CoreGame.Instance.goalKeeper_left 
            && CoreGame.Instance.goalkeeper_right 
            && i_ids != null)
        {
            NeedInited = false;
            MatchManager.Instace.OnLoaded_Complete();
            ResetMatch();
            InitPlayer();
            m_startRound = false;
        }

        if (!NeedInited
            && null != AtkPlayer
            && null != AtkPlayer.playerBody
            && null != MidPlayer
            && null != MidPlayer.playerBody
            && null != EndPlayer
            && null != EndPlayer.playerBody
            && !CanSwitch
            && !m_startRound)
        {
            m_startRound = true;
            StartRound();
        }

        if (CanSwitch)
        {
            if (GameOver)
            {
                CanSwitch = false;

                if(null == DefineManager.Ins && DefineManager.Ins.TestStep)
                {
                    CloneProxy.Instance.onClientLeaveClone();
                    CloneProxy.Instance.Send_EnterClone(1001);
                }
                //else
                //    CloneProxy.Instance.GameOver();

                bool bwin = GetWinSide() == E_MatchWinSide.Left;
                Facade.Instance.SendNotification(NotificationID.BattleEnd_Show, bwin);
            }
            else if (RoundEnd)
            {
                CanSwitch = false;
                RoundEnd = false;
                //RoundMatch();

             Facade.Instance.SendNotification(NotificationID.SandTable_Show);
            }
        }

        //比赛结束
        if (IsOver)
        {

        }
    }

    public static void DirectRoundMatch()
    {
        OnTimer_RoundEndComplete();
    }

    public static void RoundMatch()
    {
        TimerManager.AddTimer("onRoundEnd", 4f, OnTimer_RoundEndComplete);
    }

    private static void OnTimer_RoundEndComplete()
    {
        TimerManager.Destroy("onRoundEnd");
        NeedInited = true;
    }

    /// <summary>本轮比赛开始</summary>
    public void StartRound()
    {
        float delay = m_doStep == 1 ? 1f : 1f;
        TimerManager.AddTimerRepeat(Timer_StepOneKey, delay, OnTimer_DoStepOne);
    }

    public static void InitPlayer()
    {
        CoreGame.Instance.players = new List<GameObject>();
        CoreGame.Instance.oponents = new List<GameObject>();

        if (null == m_leftMatchData)
            m_leftMatchData = new Dictionary<string, MatchPlayerItem>();

        if (null == m_rightMatchData)
            m_rightMatchData = new Dictionary<string, MatchPlayerItem>();

        if (null == m_matchPlayerDic)
            m_matchPlayerDic = new Dictionary<GameObject, MatchPlayerItem>();

        #region
        //==============================================================================
        int kepId = int.Parse(i_keeperId.ToString());

        KBEngine.Entity entity = KBEngine.KBEngineApp.app.GetEntity(kepId);
        KBEngine.Monster monster = entity as KBEngine.Monster;
        KBEngine.Card card = entity as KBEngine.Card;

        if (null == m_leftKeepGoal)
            m_leftKeepGoal = new MatchPlayerItem();
        m_leftKeepGoal.IsLeft = true;
        m_leftKeepGoal.playerBody = CoreGame.Instance.goalKeeper_left.gameObject;

        if (null == m_rightKeepGoal)
            m_rightKeepGoal = new MatchPlayerItem();
        m_rightKeepGoal.IsLeft = false;
        m_rightKeepGoal.playerBody = CoreGame.Instance.goalkeeper_right.gameObject;

        if (null != monster && monster.configID_B > 0)
            m_rightKeepGoal.monsterCfg = InstanceProxy.Get<MonsterConfig>().GetItem(monster.configID_B);

        else if (null != card && card.configID_B > 0)
            m_leftKeepGoal.player = InstanceProxy.Get<PlayerManager>().GetItem(card.configID_B);

        m_matchPlayerDic[m_leftKeepGoal.playerBody] = m_leftKeepGoal;
        m_matchPlayerDic[m_rightKeepGoal.playerBody] = m_rightKeepGoal;
        //=============================================================================

        #endregion
        if (null != card)
            m_leftKeepGoal.Card = card;
        else
            m_rightKeepGoal.Monster = monster;

        m_initPlayerCount = i_ids.Count + i_fstList.Count + i_secList.Count + i_thrList.Count;
        MatchPlayerItem atkItm;
        int cnt = i_ids.Count;
        m_needCheckLeft = true;
        for (int i = 0; i < cnt; i++)
        {
            atkItm = CreateSngPlayer(int.Parse(i_ids[i].ToString()),i_poses[i]);

            if (null == AtkPlayer) AtkPlayer = atkItm;
            else if (null == MidPlayer) MidPlayer = atkItm;
            else if (null == EndPlayer) EndPlayer = atkItm;
        }

        for(int i = 0; i < cnt; i++)
        {
            if (i == 0) CreateSngDef(AtkPlayer, i_fstList, 1);
            else if (i == 1) CreateSngDef(MidPlayer, i_secList, 2);
            else if (i == 2) CreateSngDef(EndPlayer, i_thrList, 3);
        }

        m_atkIDs = i_ids;
        i_fstList = null;
        i_ids = null;
        i_keeperId = 0;
        i_secList = null;
        i_thrList = null;
    }

    /// <summary>是否是攻击方</summary>
    public static bool IsAtkSide(MatchPlayerItem val)
    {
        if (null != val.Card)
            return m_atkIDs.IndexOf(val.Card.id) != -1;
        else
            return m_atkIDs.IndexOf(val.monsterCfg.id) != -1;
    }

    /// <summary>计算进攻次数</summary>
	public static void CalAtkNum(int val)
    {
        m_totalRound = val;
        CalTimerArrange();
    }

    /// <summary>计算进攻次数</summary>
	private static void CalTimerArrange()
    {
        m_roundInfo = new List<MatchRoundInfo>();

        MatchRoundInfo item;

        int maxNum = 180; //进攻间隔最短30秒
        List<int> timerList = new List<int>();
        for(int i = 1; i < maxNum; i++)
        {
            timerList.Add(i);
        }

        for(int i = 0; i < m_totalRound; i++)
        {
            int rnd = UnityEngine.Random.Range(0,timerList.Count);
            int timer = timerList[rnd] *30;

            timerList.RemoveAt(rnd);

            item = new MatchRoundInfo();
            m_roundInfo.Add(item);
        }
    }

    /// <summary>设置球场的大小</summary>
    public void SetWidthAndHeight(int s_width,int s_height)
    {
        Width = s_width;
        Height = s_height;
        InitPositon();
    }

    /// <summary>初始化坐标 不包含球员位置</summary>
    public static void InitPositon()
    {
        int xnum = 10;
        int znum = 7;
        float width = Width;   //x
        float height = Height / 2; //z

        float halfWidht = width / 2;
        m_leftCalCornerPoint = new Vector3(halfWidht, 0,height);
        m_rigthCalCornetPoint = new Vector3(-halfWidht, 0, -height);

        m_cellX = width / xnum;
        m_cellZ = height / znum;

        m_aStar = new MatchAStar();
        m_aStar.CellX = m_cellX / 2;
        m_aStar.CellZ = m_cellZ / 2;

        Vector3 maxPos = new Vector3(Mathf.FloatToHalf(halfWidht / m_cellX), 0, Mathf.FloatToHalf(halfWidht / m_cellZ));
        m_aStar.MaxPosition = maxPos;
        m_aStar.MinPosition = new Vector3(-maxPos.x,0,-maxPos.z);

        m_cellLength = Mathf.Sqrt(m_cellX * m_cellX + m_cellZ * m_cellZ);
    }

    /// <summary>
    /// 获取左半场排列点位置
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 GetLeftDotPostioin(string pos)
    {
        Vector3 v3 = Vector3.zero;
        char[] _pos = pos.ToString().ToCharArray();

        if (_pos.Length < 1 && _pos.Length > 2)
        {
			CommonFun.Debug("no this pos");
            return v3;
        }

        int posX = int.Parse(_pos[1].ToString());
        int posZ = int.Parse(_pos[0].ToString());

        v3.x = m_leftCalCornerPoint.x - posX * m_cellX;
        v3.z = m_leftCalCornerPoint.z - posZ * m_cellZ;

        return v3;
    }

    /// <summary>
    /// 获取右半场排列点位置
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 GetRightDotPostioin(string pos)
    {
        Vector3 v3 = Vector3.zero;
        char[] _pos = pos.ToString().ToCharArray();

        if (_pos.Length < 1 && _pos.Length > 2)
        {
			CommonFun.Debug("no this pos");
            return v3;
        }

        int posX = int.Parse(_pos[1].ToString());
        int posZ = int.Parse(_pos[0].ToString());

        v3.x = m_rigthCalCornetPoint.x + posX * m_cellX;
        v3.z = m_rigthCalCornetPoint.z + posZ * m_cellZ;

        return v3;
    }

    /// <summary>
    /// 回合结束
    /// </summary>
    private void OnTimer_RoundGo()
    {
        E_MatchWinSide winSide = GetWinSide();

        if (CurrentType == E_MatchType.Cup || CurrentType == E_MatchType.PVP)
        {
            if (m_roundInfo.Count == 0 && winSide == E_MatchWinSide.Null)
            {
                InShootOut = true;
                ResetMatch(!IsAtkLeft, true);
                Debug.Log("进入点球大战");
				CommonFun.Debug("进入点球大战");
            }
            else if (m_roundInfo.Count == 0 && winSide != E_MatchWinSide.Null)
            {
                if (InShootOut)
                {
                    InShootOut = false;
					CommonFun.Debug("点球大战分出胜负了！" + winSide);
                }
                else
                {
                    //Inited = false;
                    CommonFun.Debug("进入下一个回合");
                }
            }
            else
            {
                //Inited = false;
                CommonFun.Debug("进入下一个回合");
            }
        }
        else
        {
            //InShootOut = true;
            //ResetMatch(IsAtkLeft,InShootOut);
			//CommonFun.Debug("进入下一个回合");
        }
    }

    /// <summary>获取胜利方</summary>
    /// <returns></returns>
    public E_MatchWinSide GetWinSide()
    {
        bool hover = null != m_roundInfo && m_roundInfo.Count == 0;

        if (LeftGoalNum == 0 && RightGoalNum == 0) return E_MatchWinSide.Null;
        else if(LeftGoalNum == RightGoalNum && LeftGoalNum > 0 && RightGoalNum > 0) return E_MatchWinSide.Equally;

        E_MatchWinSide winSide = LeftGoalNum > RightGoalNum ? E_MatchWinSide.Left : E_MatchWinSide.Right;
        CommonFun.Debug("WinSide : " + winSide);

        return winSide;
    }

    /// <summary>获取球员</summary>
    public static MatchPlayerItem GetPlayerItem(GameObject target)
    {
        if (null == target || null == m_matchPlayerDic) return null;

        MatchPlayerItem item;

        m_matchPlayerDic.TryGetValue(target,out item);

        return item;
    }

    /// <summary>获取球员</summary>
    public static MatchPlayerItem GetPlayerItem(Transform target)
    {
        MatchPlayerItem item;

        if (null == m_matchPlayerDic || null == target) return null;

        m_matchPlayerDic.TryGetValue(target.gameObject, out item);

        return item;
    }

    private static E_AtartNodeOper GetWillState(E_StepResult res)
    {
        E_AtartNodeOper op = E_AtartNodeOper.Null;
        if (res == E_StepResult.con_result_pass_succ || res == E_StepResult.con_result_perfect_pass)
        {
            op = E_AtartNodeOper.Pass;
        }
        else if (res == E_StepResult.con_result_shoot_fail || res == E_StepResult.con_result_shoot_succ)
        {
            op = E_AtartNodeOper.Shoot;
        }
        else if (res == E_StepResult.con_result_be_steal)
        {
            op = E_AtartNodeOper.Tackle;
        }

        return op;
    }

    /// <summary>自动</summary>
    /// <param name="target">目标</param>
    public static void AutoDoStep(MatchPlayerItem playerItem,Transform target)
    {
        //攻击者离目标有一定距离 方便奔跑活跃起来

        if(MatchManager.SupplyPlayer != null)
        {

        }
        else
        {
            MatchPlayerItem nxtItm = GetPlayerItem(target);
            AllRef.Ball.WaitToSelectedPlayer = nxtItm;
        }

        E_StepResult res;
        StepOperResult.TryGetValue(m_doStep, out res);

        E_AtartNodeOper oper = GetWillState(res);
        // MoveToAtkPositon(playerItem,oper);

        if (oper == E_AtartNodeOper.Tackle)
        {
            Do_Stole(playerItem);
        } 
        else
        {
            playerItem.playerControl.Set_AstarWillState(oper);
            if (playerItem.BoundToPosion != Define.NullPos && playerItem == EndPlayer && m_doStep == 3)
            {
                playerItem.atkPositon = playerItem.BoundToPosion;
                playerItem.MoveToPosition = playerItem.BoundToPosion;
            }
            else
                playerItem.MoveToPosition = playerItem.atkPositon;
        }
    }

    /// <summary>防守者移动攻击点</summary>
    /// <param name="target">目标</param>
    /// <param name="selfMove">自己是否移动</param>
    public static void MoveToOperPositon(MatchPlayerItem playerItem, bool selfMove = false, int step = -1)
    {
        if (null != playerItem)
        {
            MatchPlayerItem item;
            Vector3 dir = playerItem.playerControl.transform.position - playerItem.AtkPostion[0];
            float len = dir.magnitude;
            dir = dir.normalized;

            Vector3 toPos = playerItem.OperPostion[0];

            playerItem.OperPostion[0] = toPos;

            List<MatchPlayerItem> oponents;
            if (step == -1)
                oponents = playerItem.oponents;
            else
                oponents = playerItem.GetOponents(step);

            //防守者跑向攻击点
            if (null != oponents && oponents.Count > 0)
            {
                int cout = oponents.Count;

                if (playerItem.playerControl.Skill_Control.UseStraightBehind)
                {
                    toPos = playerItem.playerControl.transform.position;
                    for (int i = 0; i < cout; i++)
                    {
                        item = oponents[i];
                        Vector3 dir2 = (playerItem.OperPostion[0] - item.playerBody.transform.position).normalized;
                        item.playerControl.PlayRun(toPos + dir2 * 0.4f);
                    }
                }
                else
                {
                    if (cout == 1)
                    {
                        item = oponents[0];

                        //离目标点1米距离
                        Vector3 dir2 = (playerItem.OperPostion[0] - playerItem.AtkOrgPostion[0]).normalized;

                        float betwwendis = Define.KeeperBetwwenDis;
                        item.playerControl.PlayRun(toPos - dir2 * betwwendis);
                    }
                    else
                    {

                        for (int i = 0; i < cout; i++)
                        {
                            item = oponents[i];

                            //离目标点1米距离
                            Vector3 dir2 = (playerItem.OperPostion[0] - item.playerBody.transform.position).normalized;

                            float betwwendis = i == 0 ? Define.KeeperBetwwenDis : Define.KeeperBetwwenDis2;
                            item.playerControl.PlayRun(toPos - dir2 * betwwendis);
                        }
                    }
                }
            }
            if (selfMove)
            {
                playerItem.playerControl.PlayRun(toPos);
            }
        }
    }

    /// <summary>
    /// 防守者移动攻击点
    /// </summary>
    /// <param name="target">目标</param>
    /// /// <param name="selfMove">自己是否移动</param>
    public static void MoveToAtkPositon(MatchPlayerItem playerItem, E_AtartNodeOper willOper, E_AtartNodeOper helpOper = E_AtartNodeOper.Null)
    {
        if (null != playerItem)
        {
            playerItem.playerControl.PlayerSpeed = Player.PlayerNormalSpeed;

            if (willOper == E_AtartNodeOper.Tackle || helpOper == E_AtartNodeOper.Tackle)
            {
                Do_Stole(playerItem);
            }
            else
            {
                playerItem.playerControl.Set_AstarWillState(willOper);
                if (playerItem.BoundToPosion != Define.NullPos && playerItem == EndPlayer && m_doStep == 3)
                {
                    playerItem.atkPositon = playerItem.BoundToPosion;
                    playerItem.MoveToPosition = playerItem.BoundToPosion;
                }
                //else if (playerItem.Skill_Control.UseGuoRenCure)
                    playerItem.MoveToPosition = playerItem.AtkOrgPostion[0];
                //else
                //    playerItem.MoveToPosition = playerItem.AtkPostion[0];

                playerItem.playerControl.PlayRun();

                if (playerItem == AtkPlayer || playerItem == MidPlayer || playerItem == EndPlayer)
                //DisplayUtil.Play(playerItem.playerControl.Ani, Define.AniName_GuoRen1);
                playerItem.playerControl.PlayRun();

                if (playerItem.Skill_Control.UseCollisionsHelp)
                {
                    playerItem.Skill_Control.UseCollisionsHelp = false;
                    MatchManager.CreateCollisionPlayer();
                }
                else
                {

                }

                if (null != playerItem.oponents && playerItem.oponents.Count > 0 && !playerItem.Skill_Control.UseStraightBehind)
                {
                    MatchPlayerItem item;
                    int cout = playerItem.oponents.Count;
                    //随机一个做过人被动动作
                    int bid = UnityEngine.Random.Range(0, cout - 1);
                    for (int i = 0; i < cout; i++)
                    {
                        item = playerItem.oponents[i];

                        if (item.Skill_Control.UseZhouJI)
                            continue;

                        Vector3 dir = playerItem.MoveToPosition - item.playerBody.transform.position;
                        dir = dir.normalized;
                        item.CatchUpMinDis = 1.8f;

                        if (playerItem.BoundToPosion != Define.NullPos && playerItem == MatchManager.EndPlayer && MatchManager.m_doStep == 3)
                        {
                            item.playerControl.Move_Type = MoveType.OnlyX;
                            item.playerControl.PlayRun(playerItem.BoundToPosion);
                        }
                        else
                        {
                            item.playerControl.Move_Type = MoveType.OnlyZ;
                            item.playerControl.PlayRun(playerItem.atkPositon);
                        }

                        if (i == 0)
                        {
                            item.playerControl.CanPlayRun = false;
                            DisplayUtil.Play(item.playerControl.Ani, Define.AniName_RunHou);
                            item.SetAniComplete(2, Define.AniName_RunHou, OnComplete_RunHouAniCnt);
                        }
                    }
                }
            }
        }
    }

    /// <summary>动画次数 播放向后奔跑完成</summary>
    private static void OnComplete_RunHouAniCnt(MatchPlayerItem val)
    {
        if(null == val.defTarget)
        {
            Debug.LogError("No Deftarget!");
            return;
        }

        val.playerControl.CanPlayRun = true;
        val.playerControl.PlayRun(val.defTarget.MoveToPosition);

        val.playerControl.PlayerSpeed = Player.PlayerChaDanCheBeiDongSpeed;

        string beidongName = Define.AniName_CaiDanCheBeiDong;

        WrapMode wmode = WrapMode.Loop;
        MatchPlayerItem defItem = val.defTarget;
        if (defItem.playerControl.Skill_Control.UseCaiHongGuoRen)
        {
            DisplayUtil.Play(defItem.playerControl.Ani, Define.AniName_CaiHongGuoRen);
            defItem.Cure_MoveInfo.TotalTimer = defItem.playerControl.Ani[Define.AniName_CaiHongGuoRen].length;

            defItem.playerControl.Skill_Control.UseCaiHongGuoRen = false;
        }
        else if (defItem.playerControl.Skill_Control.UseChaoChe)
        {
            DisplayUtil.Play(defItem.playerControl.Ani, Define.AniName_ChaoChe);
            defItem.Cure_MoveInfo.TotalTimer = defItem.playerControl.Ani[Define.AniName_ChaoChe].length;

            wmode = WrapMode.Once;
            beidongName = Define.AniName_ChaoCheBeiDong;
        }
        else if (defItem.playerControl.Skill_Control.UseLanHuaZhiTuPo)
        {
            DisplayUtil.Play(defItem.playerControl.Ani, Define.AniName_LanHuZhiTuPo);
            defItem.Cure_MoveInfo.TotalTimer = defItem.playerControl.Ani[Define.AniName_LanHuZhiTuPo].length;

            wmode = WrapMode.Once;
        }
        else if (defItem.playerControl.Skill_Control.UseCollisionsHelp)
        {
            
        }
        else if (val.playerControl.Skill_Control.UseGrassTactical)
        {
            beidongName = Define.AniName_WoCao;

            if (val.WoCaoSuccess)
            {
                AllRef.HaveTarck = true;
                AllRef.Ball.ResetYetState();
                AllRef.Ball.isPassing = true;
                MatchManager.CanSwitch = true;
                val.MoveToPosition = Define.NullPos;
            }
        }
        else if (defItem.playerControl.Skill_Control.UseZhouJI
            || defItem.playerControl.Skill_Control.UseCollisionsHelp
            || MatchManager.SupplyPlayer != null)
        {
           
        }
        else
        {
            DisplayUtil.Play(val.defTarget.playerControl.Ani, Define.AniName_CaiDanChe);
            val.defTarget.Cure_MoveInfo.TotalTimer = val.defTarget.playerControl.Ani[Define.AniName_CaiDanChe].length;
        }

        if (defItem.playerControl.Skill_Control.UseChaoChe)
        {
            defItem.playerControl.Skill_Control.UseChaoChe = false;
            //val.SetAniComplete(1, beidongName,OnComplete_AniOverToRest);
        }
        else if (defItem.playerControl.Skill_Control.UseLanHuaZhiTuPo)
        {
            defItem.playerControl.Skill_Control.UseLanHuaZhiTuPo = false;
        }
        else if (val.playerControl.Skill_Control.UseGrassTactical)
        {
            MatchManager.FocusPlayer = val;
            val.playerControl.Skill_Control.UseGrassTactical = false;
        }
        else
        {
            val.SetAniComplete(1, beidongName, OnComplete_AniCaiDanCheCnt);
        }

        DisplayUtil.Play(val.playerControl.Ani, beidongName, wmode);

        val.defTarget.Cure_MoveInfo.CurrentTimer = 0;

        val.defTarget.ChaDanCheStartPoint = val.defTarget.playerControl.transform.position;

       //if(val.defTarget.Skill_Control.UseGuoRenCure)
            val.defTarget.playerControl.Move_Type = MoveType.Cure;

        val.defTarget.Cure_MoveInfo.LinePoint = val.defTarget.playerControl.transform.position;
        val.defTarget.Cure_MoveInfo.StartPoint = val.defTarget.playerControl.transform.position;

        val.defTarget.Cure_MoveInfo.EndPoint = val.defTarget.MoveToPosition;
        val.defTarget.Cure_MoveInfo.EndPoint.x = val.defTarget.Cure_MoveInfo.StartPoint.x;
    }

    /// <summary>动画次数 踩单车完成</summary>
    private static void OnComplete_AniCaiDanCheCnt(MatchPlayerItem val)
    {
        val.MoveToPosition = val.AtkOrgPostion[0];
        val.playerControl.Move_Type = MoveType.Normal;
        val.playerControl.PlayRun(val.defTarget.atkPositon);
        val.playerControl.PlayerSpeed = Player.PlayerNormalSpeed;
    }

    /// <summary>动画次数 踩单车完成</summary>
    private static void OnComplete_AniOverToRest(MatchPlayerItem val)
    {
        val.MoveToPosition = val.AtkOrgPostion[0];
        val.playerControl.Move_Type = MoveType.Normal;
        val.playerControl.PlayRest();
        val.playerControl.PlayerSpeed = Player.PlayerNormalSpeed;
    }

    public static void Do_Stole(MatchPlayerItem playerItem)
    {
        if (null != playerItem)
        {
            MatchPlayerItem item;
            if (null != playerItem.oponents && playerItem.oponents.Count > 0)
            {
                int cout = playerItem.oponents.Count;
                for (int i = 0; i < cout; i++)
                {
                    item = playerItem.oponents[i];
                    item.playerControl.CanTackle = true;
                    item.playerControl.SetState(Player_State.STOLE_BALL);
                }
            }
        }
    }

    private bool gui_go = false;
    public void OnGUI()
    {
        //     if (GUI.Button(new Rect(0, 80, 100, 40), "ShootOut"))
        //     {
        //         InShootOut = true;
        //         OnTimer_RoundGo();
        //CommonFun.Debug("进入点球大战");
        //     }

        if (GUI.Button(new Rect(0, 0, 100, 40), "屏蔽抢断"))
        {
            SkillProxy.Instance.onCloneGM(CloneGMType);
        }

        if (GUI.Button(new Rect(0, 41, 100, 40), "协防"))
        {
            CreateXieFangPlayer(AtkPlayer);

            //MatchPlayerItem ownItm = GetPlayerItem(AllRef.Ball.owner);
            //ChangeDefTarget(ownItm,MidPlayer);
        }

        if(GUI.Button(new Rect(0, 82, 100, 40), "裁判"))
        {
            Facade.Instance.SendNotification(NotificationID.MatchInfo_ShowJudge, 0);
            Facade.Instance.SendNotification(NotificationID.MatchInfo_ShowJudge, 1);
        }
    }

    /// <summary></summary>
    public static void ChangeDefTarget(GameObject oldTarget, GameObject toTarget)
    {
        ChangeDefTarget(GetPlayerItem(oldTarget),GetPlayerItem(toTarget));
    }

    /// <summary></summary>
    public static void ChangeDefTarget(MatchPlayerItem oldTarget, MatchPlayerItem toTarget)
    {
        if(oldTarget.oponents == null)
        {
            CommonFun.Debug("ChangeDefTarget:oponents=NULL");
            return;
        }

        MatchPlayerItem opm;
        int cnt = oldTarget.oponents.Count;

        for (int i = 0; i < cnt; i++)
        {
            opm = oldTarget.oponents[i];
            opm.SetDefTarget(toTarget, MatchManager.m_doStep);
            opm.MoveToPosition = toTarget.OperPostion[0];
            opm.atkPositon = opm.AtkPostion[0];
        }
    }

    public static void CreateCollisionPlayer()
    {
        Vector3 outPos = GetOutScreenPostion();
        MatchPlayerItem supplyItm = CreateSngPlayer(100, 1002, outPos, true);
        MatchPlayerItem ballOwnItm = MatchManager.GetPlayerItem(AllRef.Ball.owner);
        ballOwnItm.SupplyPlayer = supplyItm;
        supplyItm.PassToPlayer = ballOwnItm;

        MatchManager.SupplyPlayer = supplyItm;

        if (supplyItm.playerBody != null)
            supplyItm.playerControl.PlayerSpeed = Player.PlayerSupplySpeed;

        supplyItm.playerControl.Skill_Control.PassNearTimer = Define.SuppyPassTimer;
        ballOwnItm.playerControl.Skill_Control.PassNearTimer = Define.SuppyPassTimer;

        int faceZ = IsAtkLeft ? 1 : -1;
        Vector3 faceV = new Vector3(0, 0, faceZ);
        float tts = Player.PlayerSupplySpeed * (Define.SuppyPassTimer + Define.PassAniTimer);
        float rayLen = 15f;
        
        int cnt = ballOwnItm.oponents.Count;
        for (int i = cnt - 1; i >= 0; i--)
        {
            supplyItm.oponents.Add(ballOwnItm.oponents[i]);
        }

        supplyItm.OperPostion.Add(outPos + faceV * tts);
        supplyItm.AtkPostion.Add(outPos + faceV * rayLen);
        supplyItm.AtkOrgPostion.Add(outPos + faceV * rayLen);
        supplyItm.atkPositon = supplyItm.AtkPostion[0];
        supplyItm.ChangeSkin = true;
        MoveToOperPositon(supplyItm, true);

        ballOwnItm.playerControl.AstarWillToPlayer = supplyItm;
        AllRef.Ball.WaitToSelectedPlayer = supplyItm;
        ballOwnItm.playerControl.SupplyOperPostion = ballOwnItm.OperPostion[0];
        ballOwnItm.playerControl.Do_Pass();

        Facade.Instance.SendNotification(NotificationID.BALLOPER_CLOSE);
    }

    /// <summary> 创建补射球员</summary>
    public static void CreateFollowPlayer()
    {
        Vector3 outPos = GetOutScreenPostion();
        FollowShootPlayer= CreateSngPlayer(101, 1002, outPos, true);

        if (null != FollowShootPlayer.playerBody)
            FollowShootPlayer.playerControl.PlayerSpeed = Player.PlayerFollowSpeed;

        AddTimeStopOutList(FollowShootPlayer);
    }

    /// <summary> 创建协防补位球员</summary>
    public static void CreateXieFangPlayer(MatchPlayerItem item)
    {
        Vector3 outPos = GetOutScreenPostion();
        XieFangPlayer = CreateSngPlayer(102, 1002, outPos, true);

        Vector3 offset = new Vector3(1, 0, 0);
        Vector3 endPos = item.playerControl.transform.position + offset * 2f;
        XieFangPlayer.SetDefTarget(item.defTarget, m_doStep);
        XieFangPlayer.XieFangPoint = endPos;

        if (null != XieFangPlayer.playerBody)
        {
            XieFangPlayer.playerBody.transform.position = Define.NullPos;
            XieFangPlayer.playerControl.PlayerSpeed = Player.PlayerFollowSpeed;
            XieFangPlayer.playerControl.PlayRun(endPos);
            XieFangPlayer.atkPositon = endPos;
        }
    }

    /// <summary>随机一个门柱位置</summary>
    public Vector3 GetShootToGoalPost(TD_PassOrShootItem item)
    {
        Vector3 pos = Vector3.one;

        return pos;
    }

    /// <summary>随机一个进门位置</summary>
    public static Vector3 RandomInToGoalPos()
    {
        //string[] indexs = item.TargetRndIndex.Split(',');
        //int rndInd = UnityEngine.Random.Range(0,indexs.Length);
        //int val = int.Parse(indexs[rndInd]);
        //float sx = -3f;
        //float sy = 2f;
        //float cx = sx / 5;
        //float cy = sy / 3;
        //float px = cx / 5;
        //float py = cx % 5;

        //float lx = sx + px * cx;
        //float ly = sy - py * cy;
        //float rndX = UnityEngine.Random.Range(lx, lx + cx);
        //float rndY = UnityEngine.Random.Range(ly, ly - cy);

        Vector3 cankaoPos = Define.ShootSuccessRndPos;
        float rndY = UnityEngine.Random.Range(-0.8f, 0.7f);
        //守门员身后n米
        float ofz = 1.2f;
        Vector3 facePs = IsAtkLeft ? m_leftCalCornerPoint : m_rigthCalCornetPoint;
        float rndZ = facePs.z > 0 ? facePs.z + ofz : facePs.z - ofz;
        int faceX = UnityEngine.Random.Range(0, 1f) > 0.5f ? 1 : -1;
        Vector3 tps = new Vector3(cankaoPos.x * faceX * 1f, cankaoPos.y + rndY, rndZ);
        tps.y = 0.24f;
        return tps;
    }

    /// <summary>获取进攻方对手的守门员</summary>
    public static GoalKeeper OponetKeeper
    {
       get {
            return   IsAtkLeft? CoreGame.Instance.goalKeeper_left : CoreGame.Instance.goalkeeper_right;
        }
    }

     public MatchPlayerItem GetOponetKeeperItem()
    {
        return IsAtkLeft ? m_leftKeepGoal : m_rightKeepGoal;
    }

    #region
    /// <summary>
    /// 第一步
    /// </summary>
    public static void Step_One()
    {
        m_step = 1;
        m_doStep = 1;
        MatchManager.Stop = false;
        AtkPlayer.atkPositon = AtkPlayer.AtkPostion[0];
        MidPlayer.atkPositon = MidPlayer.AtkPostion[0];

        MoveToOperPositon(AtkPlayer, true);
        SetAllDefenderTrigger(true);
    }

    /// <summary>第二步</summary>
    public void Step_Second()
    {
        m_step = 2;
        MidPlayer.atkPositon = MidPlayer.AtkPostion[0];

        MoveToOperPositon(MidPlayer, true);
        SetAllDefenderTrigger(true);
    }

    /// <summary>第三步</summary>
    public static void Step_Third()
    {
        m_step = 3;
        EndPlayer.atkPositon = EndPlayer.AtkPostion[0];

        EndPlayer.playerControl.DefenderTrigger(false);
        SetAllDefenderTrigger(true);

        MoveToOperPositon(EndPlayer, true);
    }

    #endregion
    #region 点球大战
    
    /// <summary>设置点球大战球员</summary>
    public bool SetShootOutPlayer(bool left)
    {
        Transform shootOutDot= left ? LeftShootOutDot : RightShootOutDot;
        Vector3 offz = left ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1);
        GoalKeeper keeper = OponetKeeper;
        bool leftComplete = true;
        bool rightComplete = true;

        IsAtkLeft = left;
        AttackTeamTag = IsAtkLeft ? Define.Tag_PlayerTeam : Define.Tag_OponentTeam;

        Dictionary<string, MatchPlayerItem> dic = left ? m_leftMatchData : m_rightMatchData;
        Dictionary<string, MatchPlayerItem> dic2 = !left ? m_leftMatchData : m_rightMatchData;

        foreach (var item in dic2)
        {
            item.Value.playerControl.Visible = false;
        }

        MatchPlayerItem maxItem = null;
        PlayerItem maxLvItem = null;
        foreach (var item in dic)
        {
            //没有参加点球大战
            if (!item.Value.JoinedShootOut)
            {
                KBEngine.Card card = item.Value.Card;
                KBEngine.Monster monster = item.Value.Monster;

                PlayerItem thisLvItem = GetPlayerLevelItem(item.Value);

                if (null == maxItem)
                {
                    maxItem = item.Value;
                    maxLvItem = maxItem.player.GetLevelItem(1);
                }
                else if (maxLvItem.shoot < thisLvItem.shoot)
                {
                    maxItem = item.Value;
                }
            }
            item.Value.playerControl.Visible = false;

            if (left && !item.Value.JoinedShootOut) leftComplete = false;
            else if(!left && !item.Value.JoinedShootOut) rightComplete = false;
        }
        
        if (null != maxItem)
        {
            AllRef.RotateSelector.player = maxItem.playerControl;
            maxItem.playerControl.Visible = true;
            maxItem.atkPositon = shootOutDot.position;
            maxItem.atkPositon.y = 0;
            maxItem.playerBody.transform.position = maxItem.atkPositon + offz * 5f;
            maxItem.JoinedShootOut = true;

            MyCamera.Instance.MatchReset();
            AllRef.Ball.MatchReset();
            AllRef.Ball.SetPosition(maxItem.atkPositon);

            AtkPlayer = null;
            MidPlayer = null;
            EndPlayer = maxItem;

            maxItem.playerControl.Set_AstarWillState(E_AtartNodeOper.Shoot); //向前跑一段踢球
            //CalThirdPK(maxItem);

            MoveToAtkPositon(maxItem, E_AtartNodeOper.Shoot);
        }
        
        if (leftComplete && rightComplete)
        {
			CommonFun.Debug("点球大战本轮结束");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 防守者移动攻击点
    /// </summary>
    /// <param name="target">目标</param>
    /// /// <param name="selfMove">自己是否移动</param>
    public static void MoveToSynPositon(MatchPlayerItem playerItem)
    {
        if (null != playerItem)
        {
            //playerItem.playerControl.PlayRun(playerItem.SynPostion[0]);
            playerItem.playerControl.PlayRun(playerItem.OperPostion[0]);

            //float tods = (playerItem.playerControl.transform.position - playerItem.OperPostion[0]).magnitude;
            //float totm = playerItem.playerControl.Ani[Define.AniName_GuoRen1].length + 0.5f + playerItem.SynTimer[0];
            //playerItem.playerControl.PlayerSpeed = tods / totm;
        }
    }
    #endregion

    /// <summary>设置所有的防御者触发状态</summary>
    /// <param name="tigger"></param>
    public static void SetAllDefenderTrigger(bool tigger)
    {
        MidPlayer.playerControl.DefenderTrigger(tigger);
        AtkPlayer.playerControl.DefenderTrigger(tigger);
        EndPlayer.playerControl.DefenderTrigger(tigger);
    }


    /// <summary>获取等级数据</summary>
    public PlayerItem GetPlayerLevelItem(MatchPlayerItem target)
    {
        PlayerItem lvItem;
        if (!target.IsLeft && CurrentType == E_MatchType.PVE)
            lvItem = target.monsterCfg as PlayerItem;
        else
            lvItem = target.player.GetLevelItem(1);

        return lvItem;
    }

    /// <summary>创建球员和防御者</summary>
    public static void CreatePlayerWithDefender(int attackTimer,List<object> ids, List<int> pos, List<int> fstList, List<int> secList, List<int> thrList, object keeperId)
    {
        AtkTimer = attackTimer;

        i_ids = ids;
        i_poses = pos;
        i_fstList = fstList;
        i_secList = secList;
        i_thrList = thrList;
        i_keeperId = keeperId;

        if (DefineManager.Ins.OneEndSameDebug)
        {
            //pos[0] = 77;
            //pos[1] = 32;
            //pos[2] = 24;

            pos[0] = 73;
            pos[1] = 28;
            pos[2] = 37;

            //pos[0] = 75;
            //pos[1] = 28;
            //pos[2] = 34;
            i_ids[2] = i_ids[0];
        }

        List<int> tp = DefineManager.Ins.TestPoint;
        if (null != tp && tp.Count > 0 && tp[0] > 0 && tp[1] > 0 && tp[2] > 0)
        {
            //pos[0] = 67;
            //pos[1] = 57;
            //pos[2] = 33;
            //pos[0] = 73;
            //pos[1] = 28;
            //pos[2] = 33;

            //pos[0] = 65;
            //pos[1] = 36;
            //pos[2] = 24;

            //pos[0] = 32;
            //pos[1] = 12;
            //pos[2] = 37;

            //pos[0] = 79;
            //pos[1] = 13;
            //pos[2] = 15;

            pos[0] = tp[0];
            pos[1] = tp[1];
            pos[2] = tp[2];
        }

        int cnt = pos.Count;
        for (int i = 0; i < cnt;i++)
        {
            CommonFun.Debug("InitPlayer Poses : " + pos[i].ToString(), "#6699CC");
        }
    }

    public static int m_step = 1;
    /// <summary>设置 当前轮的PK结果</summary>
    public static void Set_ServerOperResult(int step,E_StepResult res)
    {
        if (step == 1)
        {
            m_canResetScore = true;
        }

        if (step == 1 && !m_startRound)
        {
            NewStepOperResult = new Dictionary<int, E_StepResult>();
            NewStepOperResult[step] = res;
        }
        else
        {
            StepOperResult[step] = res;
        }
        
        if (res == E_StepResult.con_result_shoot_succ)
        {
            ShootCalResult = true;
        }

        string str = string.Format("<color={0}>{1}</color>", "#00AACC", "StepResult : " + res + " Step:" + step);
        UnityEngine.Debug.Log(str);
    }

    /// <summary>防御者</summary>
    private static void CreateSngDef(MatchPlayerItem atkItm ,List<int> defs,int step)
    {
        int cnt = defs.Count;
        List<MatchPlayerItem> opLst = new List<MatchPlayerItem>();
        for (int i = 0; i < cnt; i++)
        {
            MatchPlayerItem opItem = CreateSngPlayer(defs[i],0);
            opItem.AddDefTarget(step,atkItm);
            opItem.DefCount++;
            opLst.Add(opItem);

            List<MatchPlayerItem> tp;
            if (!atkItm.oponentsData.TryGetValue(step,out tp))
                atkItm.oponentsData[step] = new List<MatchPlayerItem>();

            atkItm.oponentsData[step].Add(opItem);
        }

        float rndDis2 = m_cellLength * 0.5f;
        GoalKeeper keeper = OponetKeeper;
        atkItm.atkPositon = atkItm.AtkPostion[0];
        Vector3 dirK = (keeper.transform.position - atkItm.atkPositon).normalized;

        float ofd;
        float operd = 10f;
        float disnxtpt = 0f;
        float fstDis = 5f;
        float playerSP = Player.PlayerNormalSpeed;
        float optm = operd / playerSP;
        float faceZ = dirK.z > 0 ? -1f : 1f;

        Vector3 atkPos;
        if (AtkPlayer == EndPlayer && step == 3)
            atkPos = atkItm.AtkOrgPostion[1];
        else
            atkPos = atkItm.AtkOrgPostion[0];

        Vector3 sPos;
        Vector3 operPos;
        //UI操作点
        sPos = atkPos.Clone();
        sPos.z += faceZ * operd;
        operPos = sPos;
        atkItm.OperPostion.Add(sPos);
        //if (atkItm == EndPlayer && step == 3)
        //operd = 5f;

        Vector3 tpos;
        bool preInFont = false;
        if (atkItm == MidPlayer)
        {
            tpos = MidPlayer.AtkOrgPostion[0];
            tpos.z += faceZ * operd;
            disnxtpt = (tpos - AtkPlayer.AtkPostion[0]).magnitude;

            preInFont = GeomUtil.Match_InFront(AtkPlayer.AtkPostion[0], MidPlayer.OperPostion[0]);
        }
        else if (atkItm == EndPlayer && EndPlayer != AtkPlayer && step != 1)
        {
            tpos = EndPlayer.AtkOrgPostion[0];
            tpos.z += faceZ * operd;
            disnxtpt = (tpos - MidPlayer.AtkPostion[0]).magnitude;

            preInFont = GeomUtil.Match_InFront(MidPlayer.AtkPostion[0], EndPlayer.OperPostion[0]);
        }
        else if (atkItm == EndPlayer && EndPlayer == AtkPlayer && step != 1)
        {
            tpos = EndPlayer.AtkOrgPostion[1];
            tpos.z += faceZ * operd;
            disnxtpt = (tpos - MidPlayer.AtkPostion[0]).magnitude;

            preInFont = GeomUtil.Match_InFront(MidPlayer.AtkPostion[0], AtkPlayer.OperPostion[1]);
        }

        PlayerManager pfg = ProxyInstance.InstanceProxy.Get<PlayerManager>();
        
        TD_PassDisTimer disItm = null;
        if (step != 1)
        {
            //计算转向花费的时间
            //float angleTm = anglet / 300f;
           
            disItm = pfg.GetPassDisInfo(disnxtpt);

            //oftimer > 0延迟时间到 <0提前时间到
            float oftm = 0;
            float houzhuanTm = 0; //向后转 动画的时间
            float chuanqiuPreTm = 0.2f; //传球在 传之前的动画时间
            if (DefineManager.Ins.UseOffsetTimer)
                oftm = disItm.oftimer;

            if (preInFont)
                houzhuanTm = 0.6f; //在后面加上向后转的时间


            rndDis2 = (disItm.timer + pfg.AngleTurnTimer + oftm + houzhuanTm + chuanqiuPreTm) * playerSP;
            // pfg.AngleTurnTimer 待优化点
            ofd = operd + (disItm.timer + pfg.AngleTurnTimer + oftm + houzhuanTm + chuanqiuPreTm) * playerSP + operd;
        }
        else
            ofd = operd + fstDis;

        //if(AtkPlayer == EndPlayer)
        //    ofd += 5f;

        //初始化点
        sPos = atkPos.Clone();

        sPos.z += faceZ * ofd;
        atkItm.InitPostion.Add(sPos);

        //同步点
        if(step != 1)
        {
            float needtm = disItm.timer + pfg.AngleTurnTimer + optm;

            atkItm.SynTimer.Add(needtm);
            float synLen = ofd - fstDis;
            sPos = atkPos.Clone();
            sPos.z += faceZ * synLen;
            atkItm.SynPostion.Add(sPos);
        }

        sPos = atkPos.Clone();
        for (int i = 0; i < cnt; i++)
        {
            MatchPlayerItem opItem = opLst[i];
            if(cnt == 1)
            {
                dirK = IsAtkLeft ? new Vector3(0,0,1) : new Vector3(0,0,-1);
            }
            else
            {
                int angle = i == 0 ? -30 : 30;
                dirK = GeomUtil.RotationMatrix(dirK, angle);
            }

            Vector3 newPos = rndDis2 * dirK + operPos;
            opItem.InitPostion.Add(newPos);
        }
    }

    /// <summary>创建球员</summary>
    private static MatchPlayerItem CreateSngPlayer(int cardDid,int posId)
    {
        KBEngine.Entity entity = KBEngine.KBEngineApp.app.GetEntity(cardDid);
        KBEngine.Monster monster = entity as KBEngine.Monster;
        KBEngine.Card card = entity as KBEngine.Card;

        if (m_needCheckLeft && null == ShootTestManager.Instace)
        {
            IsAtkLeft = card != null ? true : false;
            m_needCheckLeft = false;
        }

        if (null != DefineManager.Ins && DefineManager.Ins.RoundType != MatchHalfType.Both)
            MatchManager.IsAtkLeft = DefineManager.Ins.RoundType == MatchHalfType.Left ? true : false;

        string key = null == card ? "Monster" + "_" + cardDid : "Card" + "_" + cardDid;
        int cellPos = posId;

        bool have;
        MatchPlayerItem matchItem;
        if (null != card)
            have = m_leftMatchData.TryGetValue(key, out matchItem);
        else
            have = m_rightMatchData.TryGetValue(key,out matchItem);

        if (!have)
        {
            matchItem = new MatchPlayerItem();
            matchItem.ResetData();
        }

        PlayerManager pm = InstanceProxy.Get<PlayerManager>();

        if (null != monster && monster.configID_B > 0)
            matchItem.monsterCfg = InstanceProxy.Get<MonsterConfig>().GetItem(monster.configID_B);
        else if (null != card && card.configID_B > 0)
            matchItem.player = pm.GetItem(card.configID_B);

        matchItem.Did = cardDid;
        matchItem.AtkCount++;

        matchItem.IsLeft = card != null ? true : false;
        matchItem.MoveToPosition = Define.NullPos;
        matchItem.ChangeSkin = true;
        matchItem.Monster = monster;
        matchItem.Card = card;

        if (null == m_leftMatchData) m_leftMatchData = new Dictionary<string, MatchPlayerItem>();
        if (null == m_rightMatchData) m_rightMatchData = new Dictionary<string, MatchPlayerItem>();

        if (null != card)
            m_leftMatchData[key] = matchItem;
        else
            m_rightMatchData[key] = matchItem;

        if(cellPos != 0)
        {
            Vector3 gpos = IsAtkLeft ? GetLeftDotPostioin(cellPos.ToString()) : GetRightDotPostioin(cellPos.ToString());
            matchItem.AtkOrgPostion.Add(gpos);
            gpos += pm.AtkPointOffset;
            matchItem.AtkPostion.Add(gpos);

            TD_PlayerPosition posCfg = ProxyInstance.InstanceProxy.Get<PlayerPositionConfig>().GetItem(cellPos);
            matchItem.position = posCfg;
            if (null != posCfg && posCfg.atkEnable)
            {
                TD_PositionAttribute posAtt = ProxyInstance.InstanceProxy.Get<PositionAttributeConfig>().GetItem(cellPos);
                matchItem.positonAttribute = posAtt;
                if (posAtt.isBound)
                {
                    float df = Define.FstDistance;
                    int faceZ = gpos.z > 0 ? -1 : 1;
                    int faceX = gpos.x > 0 ? -1 : 1;

                    gpos.z += faceZ * df;
                    gpos.x += faceX * df;
                    matchItem.BoundToPosion = gpos;
                }
            }
        }

        if (!have)
        {
            matchItem.LoadObject();
        }
        else
        {
            m_initPlayerCount--;
            if(null != matchItem.playerBody)
                matchItem.playerBody.SetActive(true);
        }

        return matchItem;
    }


    /// <summary>创建球员</summary>
    private static MatchPlayerItem CreateSngPlayer(int cardDid,int cfgId, Vector3 initPos,bool isCard = true)
    {
        if (null == m_leftMatchData) m_leftMatchData = new Dictionary<string, MatchPlayerItem>();
        if (null == m_rightMatchData) m_rightMatchData = new Dictionary<string, MatchPlayerItem>();

        string key = !isCard ? "Monster" + "_" + cardDid : "Card" + "_" + cardDid;

        bool have;
        MatchPlayerItem matchItem;
        if (isCard)
            have = m_leftMatchData.TryGetValue(key, out matchItem);
        else
            have = m_rightMatchData.TryGetValue(key, out matchItem);

        if (!have)
        {
            matchItem = new MatchPlayerItem();
            matchItem.ResetData();
        }

        PlayerManager pm = InstanceProxy.Get<PlayerManager>();

        if (!isCard && cfgId > 0)
            matchItem.monsterCfg = InstanceProxy.Get<MonsterConfig>().GetItem(cfgId);
        else if (isCard && cfgId > 0)
            matchItem.player = pm.GetItem(cfgId);

        matchItem.Did = cardDid;
        matchItem.AtkCount++;

        matchItem.IsLeft = isCard ? true : false;
        matchItem.MoveToPosition = Define.NullPos;
        matchItem.ChangeSkin = true;
        matchItem.InitPostion.Add(initPos);

        if (null == m_leftMatchData) m_leftMatchData = new Dictionary<string, MatchPlayerItem>();
        if (null == m_rightMatchData) m_rightMatchData = new Dictionary<string, MatchPlayerItem>();

        if (isCard)
            m_leftMatchData[key] = matchItem;
        else
            m_rightMatchData[key] = matchItem;

        if (!have)
            matchItem.LoadObject();
        else if (null != matchItem.playerBody)
        {
            matchItem.playerBody.SetActive(true);
            m_matchPlayerDic[matchItem.playerBody] = matchItem;
        }
               

        return matchItem;
    }

    /// <summary>计算球将要落地的位置</summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public Vector3 GetBallWillToPostion(Player player)
    {
        float tm = GeomUtil.CalGravityAirTimer(AllRef.Ball.BallRigidbody.velocity.y);
        Vector3 vec = AllRef.Ball.BallRigidbody.velocity;
        Vector3 startPoint = Vector3.zero;

        Vector3 suiPinPos = AllRef.Ball.BallRigidbody.position.IgnoreY();
        Vector3 dir = suiPinPos - startPoint.IgnoreY();

        Vector3 toPos = dir.normalized * player.PlayerSpeed * tm + suiPinPos;

        return toPos;
    }

    /// <summary>获取屏幕外一点</summary>
    /// <returns></returns>
    public static Vector3 GetOutScreenPostion(float type = 1,Transform orgTarget = null)
    {
        Vector3 ballPos;

        if (null == orgTarget)
            ballPos = AllRef.Ball.transform.position;
        else
            ballPos = orgTarget.position;

       //float offZ = 6f; 
        float divX = 5f;
        Vector3 xp;

        bool left = IsAtkLeft;

        if (type != 1)
            left = !IsAtkLeft;

        if (ballPos.x > 0)
            ballPos.x -= divX;
        else
            ballPos.x += divX;

        Vector3 screenPt = Camera.main.WorldToScreenPoint(ballPos);
        if (left)
            xp = Camera.main.ScreenToWorldPoint(new Vector3(0, screenPt.y, Camera.main.transform.position.z));
        else
            xp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, screenPt.y, Camera.main.transform.position.z));

        Ray ray = Camera.main.ScreenPointToRay(xp);//射线

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))//发射射线(射线，射线碰撞信息，射线长度，射线会检测的层级)
        {
            ballPos.z = hit.point.z;

            //if (left)
            //    ballPos.z -= offZ;
            //else
            //    ballPos.z += offZ;
        }

        CommonFun.Debug("out screen point : " + ballPos.ToString());

        return ballPos;
    }

    public static bool Stop
    {
        get
        {
            return m_Stop;
        }
        set
        {
            m_Stop = value;
        }
    }

    /// <summary>获取目标</summary>
    public static MatchPlayerItem GetToTarget(MatchPlayerItem val)
    {
        Transform tran = GetToTarget(val, E_AtartNodeOper.Null);

        return GetPlayerItem(tran.gameObject);
    }

    /// <summary>获取目标</summary>
    public static Transform GetToTarget(MatchPlayerItem val,E_AtartNodeOper waitAngleInRangleDo)
    {
        Transform toTarget = null;
        MatchManager ment = MatchManager.Instace;

        Player player = val.playerControl;

        if (waitAngleInRangleDo == E_AtartNodeOper.Shoot)
        {
            toTarget = MatchManager.OponetKeeper.transform;
        }
        else if (null != MatchManager.AtkPlayer && val == MatchManager.AtkPlayer)
        {
            toTarget = MatchManager.MidPlayer.playerBody.transform;
        }
        else if (null != MatchManager.MidPlayer && val == MatchManager.MidPlayer)
        {
            toTarget = MatchManager.EndPlayer.playerBody.transform;
        }
        else if (null != MatchManager.EndPlayer && val == MatchManager.EndPlayer)
        {
            toTarget = MatchManager.OponetKeeper.transform;
        }

        return toTarget;
    }

    /// <summary>添加 时间停止列表</summary>
    /// <param name="val"></param>
    public static void AddTimeStopOutList(MatchPlayerItem val)
    {
        if (null == m_timeStopOutList)
            m_timeStopOutList = new List<MatchPlayerItem>();

        if (m_timeStopOutList.IndexOf(val) == -1)
            m_timeStopOutList.Add(val);
    }

    /// <summary>删除 时间停止列表</summary>
    /// <param name="val"></param>
    public static void RemoveTimeStopOutList(MatchPlayerItem val)
    {
        if (null == m_timeStopOutList)
            return;

        if (m_timeStopOutList.IndexOf(val) != -1)
            m_timeStopOutList.Remove(val);

        if (m_timeStopOutList.Count == 0)
            m_timeStopOutList = null;
    }

    /// <summary>添加 时间停止列表</summary>
    /// <param name="val"></param>
    public static void AddGoGetBallPlayerFront(MatchPlayerItem val)
    {
        if (null == GoGetBallPlayerFrontList)
            GoGetBallPlayerFrontList = new List<MatchPlayerItem>();

        if (null == val.oponents)
            return;

        int step = 1;
        if (val == MidPlayer)
            step = 2;
        else if (val == EndPlayer)
            step = 3;

        List<MatchPlayerItem> oponents;
        oponents = val.GetOponents(step);

       int cnt = oponents.Count;

        for(int i = 0; i < cnt;i++)
            if (GoGetBallPlayerFrontList.IndexOf(oponents[i]) == -1)
            {
                GoGetBallPlayerFrontList.Add(oponents[i]);
                AddTimeStopOutList(oponents[i]);
            }
    }

    /// <summary>删除 时间停止列表</summary>
    /// <param name="val"></param>
    public static void RemoveGoGetBallPlayerFront(MatchPlayerItem val)
    {
        if (null == GoGetBallPlayerFrontList)
            return;

        Stop = false;

        if (GoGetBallPlayerFrontList.IndexOf(val) != -1)
        {
            GoGetBallPlayerFrontList.Remove(val);
            RemoveTimeStopOutList(val);
            val.YueWeiPoint = Define.NullPos;
        }

        if (GoGetBallPlayerFrontList.Count == 0)
        {
            MatchManager.Stop = false;
            GoGetBallPlayerFrontList = null;

            if (SkillEffectType == Skill_EffectType.effect_end_round)
                CanSwitch = true;
        }
    }

    public static bool InGoGetBallPlayerFront(MatchPlayerItem val)
    {
        if (null == GoGetBallPlayerFrontList)
            return false;

        return GoGetBallPlayerFrontList.IndexOf(val) != -1;
    }


    /// <summary>是否在时间停止内还可以活动</summary>
    public static bool IsTimeStopItem(MatchPlayerItem val)
    {
        if (null == m_timeStopOutList)
            return false;

        return m_timeStopOutList.IndexOf(val) > -1 ? true : false;
    }

    /// <summary>通过Did获取Item</summary>
    public static MatchPlayerItem GetItemByDid(int did)
    {
        foreach (var child in m_matchPlayerDic)
            if (null != child.Value.Card && child.Value.Card.id == did)
                return child.Value;
            else if (null != child.Value.Monster && child.Value.Monster.id == did)
                return child.Value;

        return null;
    }

    public static void DestroyData()
    {
        Stop = false;
        m_totalRound = 0;
        NeedInited = true;
		m_roundInfo = null;
        m_leftMatchData = null;
        m_rightMatchData = null;
        m_matchPlayerDic = null;
        m_timeStopOutList = null;
        GameManager.m_openPVEUI = false;
        GameManager.Instance.ObjDic = new Dictionary<string, GameObject>();

        if (AllRef.RotateSelector)
            AllRef.RotateSelector.ResetMatch();

        LeftGoalNum = 0;
        RightGoalNum = 0;
        m_startRound = false;
        AllRef.DoShoot = false;
        UseFollowShoot = false;
        AllRef.RotateSelector = null;
        AllRef.Ball.m_openOperUI = false;

        LeftGoal = null;
        RightGoal = null;

        m_step = 1;
        m_doStep = 0;
        IsOver = false;
        AtkPlayer = null;
        MidPlayer = null;
        EndPlayer = null;
        CanSwitch = false;
        InShootOut = false;
        m_leftGoalMesh = null;
        m_rightGoalMesh = null;
        NewStepOperResult = null;
        GoGetBallPlayerFrontList = null;
        SkillEffectType = Skill_EffectType.Null;
        StepOperResult = new Dictionary<int, E_StepResult>();

        FstChooseOper = E_AtartNodeOper.Null;
        SecChooseOper = E_AtartNodeOper.Null;
        ThrChooseOper = E_AtartNodeOper.Null;

        FocusPlayer = null;
        SupplyPlayer = null;
        XieFangPlayer = null;
        FollowShootPlayer = null;

        MyCards = null;
        m_atkIDs = null;
        i_ids = null;
        i_fstList = null;
        i_secList = null;
        i_thrList = null;

        TimerManager.Destroy(Timer_StepOneKey);
        TimerManager.Destroy(m_timerKey);
        TimerManager.Destroy("ExitClone");
        TimerManager.Destroy("onRoundEnd");

        Facade.Instance.SendNotification(NotificationID.MatchInfo_Close);
        Facade.Instance.SendNotification(NotificationID.BALLOPER_CLOSE);
    }

    public void OnDrawGizmos()
    {
        int xnum = 11;
        int znum = 15;

        Vector3 v1;
        Vector3 v2;
        float halfH = Height / 2;
        float halfW = Width / 2;
        for (int i = 0; i < xnum; i++)
        {
            v1 = new Vector3(i * m_cellX - halfW, 0.01f, halfH);
            v2 = new Vector3(i * m_cellX - halfW, 0.01f, -halfH);
            Debug.DrawLine(v1, v2, Color.red);
        }

        for (int j = 0; j < znum; j++)
        {
            v1 = new Vector3(halfW, 0.01f, j * m_cellZ - halfH);
            v2 = new Vector3(-halfW, 0.01f, j * m_cellZ - halfH);

            Debug.DrawLine(v1, v2, Color.red);
        }
    }
}
