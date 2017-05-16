using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ProxyInstance;

/// <summary>曲线运动信息</summary>
public struct CureMoveInfo
{
    public Vector3 EndPoint;
    public Vector3 LinePoint;
    public Vector3 StartPoint;
    public float TotalTimer;
    public float CurrentTimer;
}

public class MatchPlayerItem
{
    /// <summary>是否是左半场球队</summary>
    public bool IsLeft = true;
    public int Did;

    public string name = "";
    public string owerName;
    /// <summary>阵法ID</summary>
    public int matchId;
    /// <summary>球员Body</summary>
    public GameObject playerBody;
    /// <summary>球员</summary>
    public TD_Player player;
    /// <summary>npc 怪物配置项目</summary>
    public TD_Monster monsterCfg;
    /// <summary>职位</summary>
    public TD_PlayerPosition position;
    /// <summary>单元格位置</summary>
    public int cellPosition;
    /// <summary>进攻时候位置</summary>
    public Vector3 atkPositon = Define.NullPos;
    /// <summary>发起进攻值</summary>
    public float ToAtkVal;
    /// <summary>位置点的属性</summary>
    public TD_PositionAttribute positonAttribute;
    /// <summary>防守目标</summary>
    public float Speed;

    /// <summary>踩单车 开始点</summary>
    public Vector3 ChaDanCheStartPoint;

    /// <summary>曲线于东信息</summary>
    public CureMoveInfo Cure_MoveInfo;

    /// <summary>移动离目标距离</summary>
    public Dictionary<MoveType, float> MoveOffsetDistance = new Dictionary<MoveType, float>();

    public MatchPlayerItem defTarget
    {
        get
        {
            int step = MatchManager.m_doStep;
            MatchPlayerItem tm;
            if (m_defTarget.TryGetValue(step,out tm))
            {
                return tm;
            }

            return null;
        }
    }

    public void SetDefTarget(MatchPlayerItem newTarget,int step)
    {
        MatchPlayerItem defItm;

        if(m_defTarget.TryGetValue(step, out defItm))
            defItm.oponents.Remove(this);

        if(newTarget == null)
        {
            Debug.LogError("m_defTarget is Null");
            return;
        }

        newTarget.GetOponents(step).Add(this);

        m_defTarget[step] = newTarget;
    }

    private Dictionary<int, MatchPlayerItem> m_defTarget = new Dictionary<int, MatchPlayerItem>();

    public void AddDefTarget(int step, MatchPlayerItem val)
    { 
        m_defTarget[step] = val;
    }

    public List<MatchPlayerItem> GetOponents(int step)
    {
        List<MatchPlayerItem> tmp;

        if (oponentsData.TryGetValue(step, out tmp))
            return tmp;

        oponentsData[step] = new List<MatchPlayerItem>();

        return oponentsData[step];
    }

    /// <summary>对手 这里指的是防御者</summary>
    public List<MatchPlayerItem> oponents
    {
       get
        {
            int step = MatchManager.m_doStep;
            return GetOponents(step);
        }
    }
    /// <summary>对手 这里指的是防御者</summary>
    public Dictionary<int, List<MatchPlayerItem>> oponentsData = new Dictionary<int, List<MatchPlayerItem>>();

    /// <summary>是否已经参与过点球大战</summary>
    public bool JoinedShootOut = false;

    public KBEngine.Card Card;
    public KBEngine.Monster Monster;

    /// <summary>移动结束位置</summary>
    public Vector3 MoveEndPosition;
    /// <summary>移动开始位置</summary>
    public Vector3 MoveStartPosition;
    //FB_BallInPlayerView 可以重置
    public bool MoveUtilCanReset;
    /// <summary>开启视野</summary>
    public bool OpenField;

    /// <summary>旋转操作点 弹出操作界面</summary>
    public List<Vector3> OperPostion = new List<Vector3>();
    /// <summary>旋转操作点 和上一个攻击者的同步点</summary>
    public List<Vector3> SynPostion = new List<Vector3>();
    /// <summary>旋转操作点 和上一个攻击者的同步时间</summary>
    public List<float> SynTimer = new List<float>();
    /// <summary>如果是边界点的移动到的点</summary>
    public Vector3 BoundToPosion = Define.NullPos;

    public bool ChangeSkin = false;
    public bool arriveOperPostion = false;

    private Player m_playerControl;

    public Vector3 MoveToPosition;
    public List<Vector3> MoveToPositonList = new List<Vector3>();

    /// <summary>参加防守次数</summary>
    public int DefCount = 0;
    /// <summary>参加进攻次数</summary>
    public int AtkCount = 0;

    public List<Vector3> InitPostion = new List<Vector3>();

    /// <summary>每一轮的位置</summary>
    public List<Vector3> AtkPostion = new List<Vector3>();
    /// <summary>每一轮的位置</summary>
    public List<Vector3> AtkOrgPostion = new List<Vector3>();

    /// <summary>射门或者传球结束</summary>
    public bool ShootOrPassOver = false;

    public Dictionary<string, bool> StateYetDic = new Dictionary<string, bool>();

    public float NoWadeBallTotaleTimer = 0;
    public float NoWadeBallCutDownTimer = 0;

    /// <summary>补射球的 补点</summary>
    public Vector3 FollowPoint;
    /// <summary>协防补位 补点</summary>
    public Vector3 XieFangPoint;
    /// <summary>协防补位 补点</summary>
    public Vector3 YueWeiPoint = Define.NullPos;

    /// <summary>接应球员</summary>
    public MatchPlayerItem SupplyPlayer;
    /// <summary>穿回球员 目前只和 SupplyPlayer配合使用</summary>
    public MatchPlayerItem PassToPlayer;

    public bool DOper;

    /// <summary>卧草   成功/失败</summary>
    public bool WoCaoSuccess = false;

    /// <summary>离防守目标球员最小距离开始追赶</summary>
    public float CatchUpMinDis = 0;

    /// <summary>球员控制器</summary>
    public Player playerControl
    {
        get
        {
            if (null == m_playerControl && null != playerBody)
                m_playerControl = playerBody.GetComponent<Player>();

            return m_playerControl;
        }
    }

    public SkillControl Skill_Control
    {
        get
        {
           return playerControl.Skill_Control;
        }
    }

    /// <summary>到达目的点后没有继续跑</summary>
    public bool GoMove; 

   
    public void LoadObject()
    {
        string loadName = "player001";

        playerBody = GameObject.Instantiate(GameManager.Instance.ObjDic[loadName]);
        playerBody.SetActive(true);

        if (null == playerBody)
            ResourceManager.Instance.LoadPrefab(loadName, OnLoad_PlayerComplete, OnLoad_PlayerError);
        else
            OnInitPlayer(loadName, playerBody);
    }

    private void OnLoad_PlayerComplete(string name, GameObject val)
    {
        OnInitPlayer( name, val);
    }

    private void OnInitPlayer(string name, GameObject val)
    {
        MatchManager.m_initPlayerCount--;
        if (null == MatchManager.m_matchPlayerDic)
            MatchManager.m_matchPlayerDic = new Dictionary<GameObject, MatchPlayerItem>();

        playerBody = val; 
        
        string key = !IsLeft ? "Monster" + "_" + Did : "Card" + "_" + Did;
        playerBody.name = key;
        name = key;

        MatchManager.m_matchPlayerDic[playerBody] = this;
        CoreGame.Instance.players.Add(playerBody);

        if (IsLeft && null != Card)
        {
            player = InstanceProxy.Get<PlayerManager>().GetItem(Card.configID_B);
            cellPosition = Card.coordinate;
            name = key;
        }
        else if(null != Monster)
        {
            monsterCfg = InstanceProxy.Get<MonsterConfig>().GetItem(Monster.configID_B);
            cellPosition = Monster.coordinate;
        }
    }

    private void OnLoad_PlayerError(string error)
    {
       
    }

    public bool IsYetDo(string nm)
    {
        bool b = false;

        StateYetDic.TryGetValue(nm,out b);

        return b;
    }

    private float m_totalTm = 0;
    private string m_countAniName;
    private UnityEngine.Events.UnityAction<MatchPlayerItem> m_complete;
    public void SetAniComplete(float cnt,string aniname, UnityEngine.Events.UnityAction<MatchPlayerItem> complete)
    {
        m_complete = complete;
        m_countAniName = aniname;
        m_totalTm = playerControl.Ani[aniname].length * cnt;
    }

    public void Update()
    {
        if (m_totalTm > 0 && playerControl.Ani.IsPlaying(m_countAniName))
        {
            m_totalTm -= Time.deltaTime;
            if(m_totalTm <= 0)
            {
                m_complete(this);
                m_totalTm = 0;
                m_countAniName = null;
            }
        }
           
    }

    /// <summary>是否是自己的卡牌</summary>
    public bool IsMyCard
    {
        get
        {
            if (null == MatchManager.MyCards)
                return false;

            return MatchManager.MyCards.IndexOf(Did) != -1;
        }
    }

    public void ResetData()
    {
        MoveOffsetDistance = new Dictionary<MoveType, float>();
        m_defTarget = new Dictionary<int, MatchPlayerItem>();
        StateYetDic = new Dictionary<string, bool>();
        NoWadeBallCutDownTimer = 0f;
        AtkOrgPostion = new List<Vector3>();
        InitPostion = new List<Vector3>();
        OperPostion = new List<Vector3>();
        AtkPostion = new List<Vector3>();
        SynPostion = new List<Vector3>();
        BoundToPosion = Define.NullPos;
        YueWeiPoint = Define.NullPos;
        FollowPoint = Define.NullPos;
        atkPositon = Define.NullPos;
        SynTimer = new List<float>();
        SupplyPlayer = null;
        PassToPlayer = null;
        WoCaoSuccess = false;
        CatchUpMinDis = 0;
        DOper = false;

        oponentsData[1] = new List<MatchPlayerItem>();
        oponentsData[2] = new List<MatchPlayerItem>();
        oponentsData[3] = new List<MatchPlayerItem>();

        if (playerBody)
            playerControl.MatchReset();

        ShootOrPassOver = false;
        GoMove = false;

        MoveOffsetDistance[MoveType.Cure] = 0f;
        MoveOffsetDistance[MoveType.Normal] = 0f;
        MoveOffsetDistance[MoveType.OnlyX] = 0f;
        MoveOffsetDistance[MoveType.OnlyZ] = 0f;
    }
}