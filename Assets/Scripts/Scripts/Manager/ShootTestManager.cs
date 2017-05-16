using UnityEngine;
using ProxyInstance;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public class ShootTestManager : MonoBehaviour
{
    public static bool InTest;

    public Player player;
    public Player AttackPlayer;
    public GoalKeeper Keeper;
    public Vector3 EndPostion;
    public Transform EndObject;
    public Transform LeftMesh;
    public Transform RightMesh;
    public E_PassOrShootType m_shootType = E_PassOrShootType.Ground;

    public float force;
    public float Yforce;
    public float airDrag;
    public float groundDrag;

    public static float LastTimer = 0f;
    public static float LastDistance = 0f;
    public int ShootPoint = 34;

    public TD_PassOrShootItem PSItem;

    private Vector3 m_oldEndPostion;

    [HideInInspector]
    private TD_PassOrShootItem m_curItm;

    [HideInInspector]
    public static ShootTestManager Instace;

    public Bezier LineBezier;

    private LineRenderer m_lineRender;

    public void Start()
    {
        if(null == Instace)
        {
            Instace = this;
            AllRef.coreGame = CoreGame.Instance;
            m_lineRender = GetComponent<LineRenderer>();
        }

        GameManager.Instance.ObjDic["player001"] = player.gameObject;

        UpdateInfo(0);

        ResetState();

        InTest = true;
        CoreGame.Instance.goalkeeper_right = Keeper;
        CoreGame.Instance.goalkeeper_right = Keeper;
    }

    /// <summary>创建一个比赛</summary>
    public void CreateMatch()
    {
        DefineManager man = DefineManager.Ins;

        MatchManager.MyCards = new List<int>();
        List<object> ids = new List<object>();
        int cnt = man.i_ids.Count;
        for (int i = 0; i < cnt; i++)
        {
            ids.Add(man.i_ids[i]);
            MatchManager.MyCards.Add(man.i_ids[i]);
        }
            

        if(null != MatchManager.m_leftMatchData)
            foreach (var val in MatchManager.m_leftMatchData)
                if (null != val.Value.playerBody)
                    val.Value.ResetData();

        if (null != MatchManager.m_rightMatchData)
            foreach (var val in MatchManager.m_rightMatchData)
                if (null != val.Value.playerBody)
                    val.Value.ResetData();

        man.i_fstList = new List<int>() { 7};
        man.i_secList = new List<int>() { 9,10};
        man.i_thrList = new List<int>() { 10,11};

        MatchManager.NeedInited = true;
        MatchManager.CreatePlayerWithDefender(0,ids, man.TestPoint, man.i_fstList, man.i_secList, man.i_thrList, man.i_keeperId);
       
        MatchManager.InitPlayer();

        player.gameObject.SetActive(false);
        AttackPlayer.gameObject.SetActive(false);

        foreach (var val in MatchManager.m_leftMatchData)
            if (null != val.Value.playerBody && val.Value.InitPostion.Count > 0)
                val.Value.playerControl.transform.position = val.Value.InitPostion[0];

        foreach (var val in MatchManager.m_rightMatchData)
            if (null != val.Value.playerBody && val.Value.InitPostion.Count > 0)
                val.Value.playerControl.transform.position = val.Value.InitPostion[0];
    }

    public void UpdateInfo(float dis)
    {
        PlayerManager pfg = InstanceProxy.Get<PlayerManager>();
        TD_PassOrShootItem itm = pfg.GetPassItem(dis);
        force = itm.force;
        airDrag = itm.airDrag;
        groundDrag = itm.drag;

        EndPostion.z = itm.distance;

        if(null != itm.addFore)
        {
            string[] ys = itm.addFore.Split(',');
            Yforce = float.Parse(ys[1]);
        }

        m_curItm = itm;
    }

    public void Update()
    {
        if (null != LineBezier && null != m_lineRender)
        {
            bool canDraw = DefineManager.Ins.DrawBallLine;
            if (canDraw)
            {
                for (int i = 1; i <= 100; i++)
                {
                    Vector3 vec = LineBezier.GetPointAtTime((float)(i * 0.01));
                    m_lineRender.SetPosition(i - 1, vec);
                }
            }
        }

        if (null != EndObject)
            EndObject.position = EndPostion;

        if(m_oldEndPostion != EndPostion && EndPostion.z > 0)
        {
            PlayerManager pfg = InstanceProxy.Get<PlayerManager>();
        }

        if (m_curItm == null)
            return;

        if (m_curItm.distance == EndPostion.z)
        {
            m_curItm.force = force;
            m_curItm.airDrag = airDrag;
            m_curItm.drag = groundDrag;
            m_curItm.addFore = "0," + Yforce + ",0";
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameProxy.Instance.GotoMainCity();
        }

        UpdateBezier();
    }

    public void UpdateBezier()
    {
        if (null == PSItem)
            return;

        TD_PassOrShootItem item = PSItem;
        Vector3 EndPosition =   new Vector3(0,0,50);
        Vector3 StartPosition = Vector3.zero;

        float TotalDistance = (StartPosition - EndPosition).magnitude;

        Vector3 dir = EndPosition - StartPosition;
        dir = dir.normalized;

        if (item.P1_OffsetAngle != 0)
        {
            dir = GeomUtil.RotationMatrix(dir, item.P1_OffsetAngle);
        }

        if (item.relativeDistacnePer > 1f)
            item.relativeDistacnePer = 1f;

       Vector3 P1 = StartPosition + dir * TotalDistance *item.relativeDistacnePer;
        P1.x -= StartPosition.x;
        P1.z -= StartPosition.z;
        P1.y = item.typeRadHeight;

        Vector3 P2 = Vector3.zero;

        if (float.IsNaN(P1.x) || float.IsNaN(P1.y) || float.IsNaN(P1.z))
            return;

        if (null == LineBezier)
            LineBezier = new Bezier(StartPosition, P1, P2, EndPosition);
        else
            LineBezier.SetPoint(StartPosition, P1, P2, EndPosition);
    }

    private E_AtartNodeOper GetWillState(E_StepResult res)
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

    public void ResetState()
    {
        AllRef.Ball.BallRigidbody.velocity = Vector3.zero;
        AllRef.RotateSelector.player = player;
        AllRef.Ball.owner = player.gameObject;
        AllRef.Ball.transform.position = player.transform.position;

        MatchPlayerItem item = new MatchPlayerItem();
        item.IsLeft = true;
        item.playerBody = player.gameObject;
        MatchManager.m_matchPlayerDic = new Dictionary<GameObject, MatchPlayerItem>();
        MatchManager.m_matchPlayerDic[item.playerBody] = item;

        if (null != ShootTestManager.Instace)
            player.transform.position = Vector3.zero;

        if (null != MatchManager.FollowShootPlayer && null != MatchManager.FollowShootPlayer.playerBody)
            MatchManager.FollowShootPlayer.playerBody.transform.position = Vector3.zero;

        AttackPlayer.transform.position = new Vector3(2.68f,0,1.46f);
    }

    public UnityEngine.Events.UnityAction OpenOperWindow;
    private bool gui_go = false;
    public void OnGUI()
    {
        if (null == AllRef.Ball)
            return;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (GUI.Button(new Rect(0, 0, 120, 40), "打开操作窗口"))
        {
            UnityEditor.EditorApplication.ExecuteMenuItem("UGame/Shoot Test");
        }

        if (GUI.Button(new Rect(0, 41, 120, 40), "==="))
        {
            MatchManager.UseFollowShoot = true;
            MatchManager.FollowShootResult = E_StepResult.con_result_reshoot_succ;
        }

        if (GUI.Button(new Rect(0, 82, 100, 40), "协防"))
        {
           MatchManager.CreateXieFangPlayer(MatchManager.AtkPlayer.oponents[0]);
        }
#endif
    }

    public void CreateVarturalMatch()
    {
        if (null == KBEngine.KBEngineApp.app)
        {
            if(PlayerPrefs.HasKey("UserServer"))
                KBEMain.StartClientap(PlayerPrefs.GetString("UserServer"));
            else
                KBEMain.StartClientap("127.0.0.1");
        }

        MatchManager.Stop = false;
        AllRef.HaveTarck = false;
        AllRef.Ball.MatchReset();

        Keeper.MatchReset();

        //1:Card 2:Monster
        List<int> tIds = new List<int>() { 65,12,33,44,45,46,47,48,49,89,90};
        for(int i = 0; i < tIds.Count; i++)
        {
            KBEMain.gameapp.CreateClientEntity(tIds[i], 1, 1);
        }

        List<int> tMds = new List<int>() { 651, 121, 331, 441, 451, 461, 471, 481, 491, 891, 901 };
        for (int i = 0; i < tMds.Count; i++)
            KBEMain.gameapp.CreateClientEntity(tIds[i], 2, 1);

        DefineManager dm = DefineManager.Ins;
        if (dm.TestPoint.Count == 0 || dm.TestPoint[0] == 0)
            dm.TestPoint = new List<int>() { 75, 46, 26 };

        GameProxy.Instance.SetSceneType(EScene.PVE);

        MatchManager.StepOperResult[1] = DefineManager.Ins.FstResult;
        MatchManager.StepOperResult[2] = DefineManager.Ins.SecResult;
        MatchManager.StepOperResult[3] = DefineManager.Ins.ThrResult;

        MatchManager.InitPositon();
        MatchManager.FocusPlayer = null;
        Test_CreatCardAndMonster(null, null);
    }

    public void Save_Timer(bool save = true)
    {
        PlayerManager pfg = ProxyInstance.InstanceProxy.Get<PlayerManager>();

        int disId;
        if(LastDistance == (int)LastDistance)
        {
            disId = (int)LastDistance;
        }
        else
        {
            disId = (int)LastDistance + 1;
        }
        
        float angle = Vector3.Angle(AllRef.Ball.BallBezier.StartPosition, AllRef.Ball.BallBezier.EndPosition);
        TD_PassDisTimer tItm = null;
        TD_PassDisTimer nearItm = null;

        int inserInd = 0;
        int cnt = pfg.m_passDisTimer.Count;
        for (int j = 0; j < cnt; j++)
        {
            TD_PassDisTimer val = pfg.m_passDisTimer[j];

            if (val.dis == disId && null == tItm)
            {
                tItm = val;
                break;
            }

            if (null == nearItm && val.dis < disId)
            {
                nearItm = val;
                inserInd = j;
            }
            else if (null != nearItm && nearItm.dis < disId && val.dis > nearItm.dis)
            {
                nearItm = val;
                inserInd = j;
            }
        }

        if (null == tItm)
        {
            tItm = new TD_PassDisTimer();
            tItm.dis = disId;
            pfg.m_passDisTimer.Insert(inserInd, tItm);
        }

        tItm.timer = LastTimer;
        if(save)
            ProxyInstance.InstanceProxy.Get<PlayerManager>().save();
    }

    /// <summary>创建 卡牌和怪物</summary>
    /// <param name="cards"></param>
    /// <param name="monsters"></param>
    public void Test_CreatCardAndMonster(List<int> cards, List<int> monsters)
    {
        if (null == cards)
            cards = new List<int> { 1,2,3,4,5,6,7,8,9,10,11};

        if (null == monsters)
            monsters = new List<int> { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

        CreateMatch();
    }

    public void OnDestroy()
    {
        InTest = false;
    }
}
