using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;
/// <summary>
/// ����
/// </summary>
public class CloneProxy: Proxy<CloneProxy>
{
    public int IntoCloneID = 1001;

    public CloneProxy()
        : base(ProxyID.Clone)
    {
        KBEngine.Event.registerOut(this, "onAtkAndDefID");
        KBEngine.Event.registerOut(this, "onCurAttackIndex");
        KBEngine.Event.registerOut(this, "onOprateResult");
        KBEngine.Event.registerOut(this, "onGameOver");
        KBEngine.Event.registerOut(this, "onRoundEnd");
        KBEngine.Event.registerOut(this, "onTotalAttackTimes");
        KBEngine.Event.registerOut(this, "onMyCardIdList");
        KBEngine.Event.registerOut(this, "onSelectSkill");
        KBEngine.Event.registerOut(this, "onRedOut");
        KBEngine.Event.registerOut(this, "onYellowCard");

        KBEngine.Event.registerOut(this, "onReady");
        KBEngine.Event.registerOut(this, "onClientPlayAnimFinish");

        KBEngine.Event.registerOut(this, "onGetAllCloneInfo");
    }
 

    public void onReady()
    {
        //��ʼս��
        KBEngine.Avatar.Instance.cellCall("onClientBeginFight", new object[] { });
    }

    /// <summary>�Լ�����Id</summary>
    public void onMyCardIdList(List<object> selfCardIds)
    {
        MatchManager.MyCards = selfCardIds.ToIntList();
    }

    /// <summary>��������</summary>
    public void onTotalAttackTimes(object val)
    {
        int times = val.ToInt();
        MatchManager.CalAtkNum(times);
    }

    /// <summary>��������</summary>
    public void onGameOver()
    {
        if(null != DefineManager.Ins && DefineManager.Ins.TestStep)
        {

        }
        else
            MatchManager.GameOver = true;
    }

    /// <summary>5����˳�����</summary>
    public void GameOver()
    {
        TimerManager.AddTimer("ExitClone", 5f, OnTimer_ExitClone);
    }

    private void OnTimer_ExitClone()
    {
        MatchManager.DestroyData();
        GameProxy.Instance.GotoMainCity();
        CloneProxy.Instance.onClientLeaveClone();
    }

    /// <summary>������븱��</summary>
    public void onClientReqEnterClone()
    {
        GameProxy.Instance.GotoPVE();
    }

    /// <summary>���Ʒ���</summary>
    public void onRedOut(object cardDid)
    {
        int did = cardDid.ToInt();

        Facade.SendNotification(NotificationID.MatchInfo_ShowJudge, 0);
    }

    /// <summary>������ʾ</summary>
    public void onYellowCard(object cardDid)
    {
        int did = cardDid.ToInt();

        Facade.SendNotification(NotificationID.MatchInfo_ShowJudge, 1);
    }

    /// <summary>֪ͨ��һ����ʼ</summary>
    public void onClientPlayAnimFinish()
    {
        if (MatchManager.GameOver || null != ShootTestManager.Instace) return;

        Debug.Log("onClientPlayAnimFinish");
        KBEngine.Avatar.Instance.cellCall("onClientPlayAnimFinish", new object[] { });
    }

    /// <summary>���Ͳ���ѡ��</summary>
    public void onClientSelectOp(E_AtartNodeOper operFlag,List<int> cardIdList = null)
    {
        int flag = (int)operFlag;

        List<object> cards = new List<object>();

        if (null == cardIdList)
            cardIdList = new List<int>();
        int cnt = cardIdList.Count;
        for (int i = 0; i < cnt; i++)
            cards.Add(cardIdList[i]);

        KBEngine.Avatar.Instance.cellCall("onClientSelectOp", flag, cards, new List<object>());
    }

    /// <summary>ǿ���˳�����</summary>
    public void onClientLeaveClone()
    {
        MatchManager.DestroyData();
        ServerCustom.instance.SendClientMethods("onClientLeaveClone");

        CommonFun.Debug("onClientLeaveClone");
    }

    /// <summary>���븱��</summary>
    public void Send_EnterClone(int id)
    {
        ServerCustom.instance.SendClientMethods("onClientReqEnterClone", id);
    }

    /// <summary>������ �� ��Ӧ�ķ�����</summary>
    public void onAtkAndDefID(object atkTimer,List<object> ids,List<object> poses,List<object> fstList, List<object> secList, List<object> thrList, object keeperId)
    {
        List<int> fLst = new List<int>();
        int cnt = fstList.Count;
        for (int i = 0; i < cnt; i++) fLst.Add(int.Parse(fstList[i].ToString()));

        List<int> sLst = new List<int>();
        cnt = secList.Count;
        for (int i = 0; i < cnt; i++) sLst.Add(int.Parse(secList[i].ToString()));

        List<int> tLst = new List<int>();
        cnt = thrList.Count;
        for (int i = 0; i < cnt; i++) tLst.Add(int.Parse(thrList[i].ToString()));

        List<int> tPs = new List<int>();
        cnt = poses.Count;
        for (int i = 0; i < cnt; i++) tPs.Add(int.Parse(poses[i].ToString()));

        MatchManager.CreatePlayerWithDefender(int.Parse(atkTimer.ToString()),ids, tPs,fLst, sLst,tLst,keeperId);
    }

    /// <summary>��ǰ�ֵĲ������</summary>
    public void onOprateResult(object setp,object val)
    {
        if (null != DefineManager.Ins && DefineManager.Ins.TestStep)
        {
            if(DefineManager.Ins.FstResult != E_StepResult.Null)
                MatchManager.Set_ServerOperResult(1, DefineManager.Ins.FstResult);
            if (DefineManager.Ins.SecResult != E_StepResult.Null)
                MatchManager.Set_ServerOperResult(2, DefineManager.Ins.SecResult);
            if (DefineManager.Ins.ThrResult != E_StepResult.Null)
                MatchManager.Set_ServerOperResult(3, DefineManager.Ins.ThrResult);
        }
        else
        {
            E_StepResult res = (E_StepResult)int.Parse(val.ToString());

            if(    res == E_StepResult.con_result_reshoot_fail 
                || res == E_StepResult.con_result_reshoot_succ)
            {
                MatchManager.UseFollowShoot = true;
                MatchManager.FollowShootResult = res;

                string str =  "StepResult : " + res + " Step:" + setp.ToString();
                CommonFun.Debug(str, "#00AACC");
            }
            else
            {
                MatchManager.Set_ServerOperResult(int.Parse(setp.ToString()), res);

                MatchPlayerItem ballItm = MatchManager.GetPlayerItem(AllRef.Ball.owner);
                if (res == E_StepResult.con_result_gk_steal)
                {
                    ballItm.playerControl.Skill_Control.KeepStealBall = true;
                    CloneProxy.Instance.onClientPlayAnimFinish();
                }
            }
        }
    }

    /// <summary>����ѡ���б�</summary>
    public void onSelectSkill(object setp,List<object> cardList)
    {
        E_StepResult res = E_StepResult.select;
        MatchManager.Set_ServerOperResult(int.Parse(setp.ToString()), res);

        List<int> lst = UtilTools.ToIntList(cardList);
        Facade.SendNotification(NotificationID.MatchInfo_CardList, lst);
    }

    public void onCurAttackIndex(object val)
    {
        Debug.Log("onCurAttackIndex : " + val);

        MatchManager.AttackIndex = int.Parse(val.ToString());
    }

    /// <summary>���ֽ���</summary>
    public void onRoundEnd(object myScore,object opScore)
    {
        MatchManager.LeftGoalNum = myScore.ToInt();
        MatchManager.RightGoalNum = myScore.ToInt();

        Facade.SendNotification(NotificationID.MatchInfo_Score);

        if (null != DefineManager.Ins && DefineManager.Ins.TestStep)
        {

        }
        else
            MatchManager.RoundEnd = true;
    }

    /// <summary>�ܿ����� UI</summary>
    public void onClientGetAllCloneInfo()
    {
        ServerCustom.instance.SendClientMethods("onClientGetAllCloneInfo");
    }

    public void onGetAllCloneInfo(List<object> levels)
    {
        Dictionary<string, object> itm;
        int cnt = levels.Count;
        List<CloneVO> vos = new List<CloneVO>();
        for (int i = 0; i < cnt;i++)
        {
            CloneVO vo = new CloneVO();
            itm = levels[i] as Dictionary<string, object>;
            
            vo.cloneID = itm["cloneID"].ToInt();
            vo.star = itm["star"].ToInt();
            vo.resetCount = itm["star"].ToInt();

            vos.Add(vo);
        }
     
        Facade.SendNotification(NotificationID.Clone_Inflo, vos);
    }
}//end ResourceConfig
