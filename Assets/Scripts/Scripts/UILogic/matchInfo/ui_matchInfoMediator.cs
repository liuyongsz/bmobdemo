using PureMVC.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AssetBundles;
using System.Collections.Generic;
using DG.Tweening;

public class ui_matchinfo : BasePanel
{
    public UILabel txtWin;
    public UILabel txtTimer;
    public UILabel txtScore;
    public UIButton btnBack;
    public UITexture headIcon;
    public UITexture imgJudge;

    public UITexture buff0;
    public UITexture buff1;
    public UITexture buff2;
    public UITexture buff3;

    public UILabel txtSkillLeftName0;
    public UILabel txtSkillLeftName1;
}

/// <summary>
/// 比赛信息
/// </summary>
public class ui_matchInfoMediator: UIMediator<ui_matchinfo> {

    private Tween m_tween;
    private string m_timer = "00:00";

    private List<UITexture> m_buffList;

    private List<BuffInfo> m_buffDataList;

    private List<string> m_skillCallList;

    private TweenPosition m_skillTween;
    private TweenAlpha m_alphaTween;
    private bool m_inTween;

    private List<int> m_judeIdList;
    private Tweener m_tweener;

    private string timer_hideJudege = "timer_hideJudege";

    public ui_matchInfoMediator() : base("ui_matchinfo") {

        RegistPanelCall(NotificationID.MatchInfo_Open, base.OpenPanel);
        RegistPanelCall(NotificationID.MatchInfo_Close, base.ClosePanel);

        RegistPanelCall(NotificationID.MatchInfo_Score,OnRec_Score);
        RegistPanelCall(NotificationID.Match_WinSide, OnRec_WinSide);
        RegistPanelCall(NotificationID.MatchInfo_Timer, OnRec_Timer);
        RegistPanelCall(NotificationID.MatchInfo_ShowJudge, OnRec_ShowJudge);
        RegistPanelCall(NotificationID.MatchInfo_AddBuff, OnRec_AddBuff);
        RegistPanelCall(NotificationID.MatchInfo_DelBuff, OnRec_DelBuff);
        RegistPanelCall(NotificationID.MatchInfo_SkillCall, OnRec_SkillCall);
    }   
    
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.btnBack.gameObject).onClick = OnClick_Back;
    }

    protected override void OnShow(INotification notification)
    {
        m_buffList = new List<UITexture>() {m_Panel.buff0,m_Panel.buff1,m_Panel.buff2,m_Panel.buff3};

        //LoadSprite.LoaderImage(m_Panel.headIcon, "jueshetouxiang1", false);
        m_Panel.txtWin.gameObject.SetActive(false);

        UpdateDisplay();

        m_Panel.imgJudge.gameObject.SetActive(false);
    }

    /// <summary>显示裁判</summary>
    /// <param name="notification"></param>
    private void OnRec_ShowJudge(INotification notification)
    {
        int judgeId = (int)notification.Body;

        if (null == m_judeIdList)
            m_judeIdList = new List<int>();

        if (TimerManager.IsHaveTimer(timer_hideJudege))
            TimerManager.Destroy(timer_hideJudege);

        m_judeIdList.Add(judgeId);

        UpdateJudeShow();
    }

    private void UpdateJudeShow()
    {
        if (null == m_judeIdList || m_judeIdList.Count <= 0 || null != m_tweener)
            return;

        int judgeId = m_judeIdList[0];
        m_judeIdList.RemoveAt(0);

        if (m_judeIdList.Count <= 0)
            m_judeIdList = null;

        m_Panel.imgJudge.gameObject.SetActive(true);

        Vector3 startPos;
        Vector3 endPos = m_Panel.imgJudge.transform.position;

        startPos.x = endPos.x;
        startPos.y = -2f;
        startPos.z = endPos.z;
        m_Panel.imgJudge.gameObject.SetActive(true);
        m_Panel.imgJudge.transform.position = startPos;
        m_tweener = m_Panel.imgJudge.transform.DOMove(endPos, 1f);

        m_tweener.OnComplete(OnTween_Judge);

        string judgeStr = judgeId == 0 ? "judgered" : "judgeyellow";
        LoadSprite.LoaderImage(m_Panel.imgJudge, "match/" + judgeStr);
    }

    private void OnTween_Judge()
    {
        m_tweener.Kill();
        m_tweener = null;
        if (m_judeIdList == null)
        {
            TimerManager.AddTimer(timer_hideJudege, 1f, OnTimer_HideJudege);
        }
        else
            UpdateJudeShow();
    }

    private void OnTimer_HideJudege()
    {
        TimerManager.Destroy(timer_hideJudege);

        m_Panel.imgJudge.gameObject.SetActive(false);
    }
    
    private void OnRec_AddBuff(INotification notification)
    {
        if (m_buffDataList == null)
            m_buffDataList = new List<BuffInfo>();

        BuffInfo data = (BuffInfo)notification.Body;
        m_buffDataList.Add(data);

        UpdateBuffDisplay();
    }

    private void OnRec_DelBuff(INotification notification)
    {
        BuffInfo data = (BuffInfo)notification.Body;

        int cnt = m_buffDataList.Count;
        for (int i = 0; i < cnt; i++)
            if (m_buffDataList[i].SkillId == data.SkillId)
                m_buffDataList.RemoveAt(i);

        UpdateBuffDisplay();
    }

    private void OnRec_SkillCall(INotification notification)
    {
        if (null == m_skillCallList)
            m_skillCallList = new List<string>();

        m_skillCallList.Add(notification.Body.ToString());

        UpdateTweenSkill();
    }

    private void UpdateTweenSkill()
    {
        if(m_skillCallList.Count > 0 && !m_inTween)
        {
            m_inTween = true;
            string skillName = m_skillCallList[0];
            m_skillCallList.RemoveAt(0);

            Vector3 endPos = m_Panel.txtSkillLeftName0.transform.position;
            m_Panel.txtSkillLeftName0.text = skillName;

            m_Panel.txtSkillLeftName0.alpha = 1f;

            m_skillTween = TweenPosition.Begin(m_Panel.txtSkillLeftName0.gameObject, 0.5f, endPos);
            EventDelegate.Add(m_skillTween.onFinished, OnTween_SkillCallFinished);

            endPos.x -= 200;
            m_Panel.txtSkillLeftName0.transform.position = endPos;

            if (m_skillCallList.Count == 0)
                m_skillCallList = null;
        }
    }

    /// <summary>技能呐喊缓动结束</summary>
    private void OnTween_SkillCallFinished()
    {
        EventDelegate.Remove(m_skillTween.onFinished, OnTween_SkillCallFinished);

        m_alphaTween = TweenAlpha.Begin(m_Panel.txtSkillLeftName0.gameObject, 1f,0);
        EventDelegate.Add(m_skillTween.onFinished, OnTween_SkillCallAlphaFinished);
    }

    private void OnTween_SkillCallAlphaFinished()
    {
        m_inTween = false;

        EventDelegate.Remove(m_alphaTween.onFinished, OnTween_SkillCallAlphaFinished);

        UpdateTweenSkill();
    }

    private void UpdateBuffDisplay()
    {
        int cnt = m_buffDataList.Count;
        SkillConfig skillCfg = ProxyInstance.InstanceProxy.Get<SkillConfig>();
        TD_Skill skillItem;
        int i = 0;
        for (i = 0; i < cnt;i++)
        {
            skillItem = skillCfg.GetItem(m_buffDataList[i].GetCfgID());
            LoadSprite.LoaderImage(m_buffList[i], "skill/" + skillItem.icon);

            m_buffList[i].gameObject.SetActive(true);
        }

        cnt = m_buffList.Count;
        for(;i < cnt; i++)
        {
            m_buffList[i].gameObject.SetActive(false);
        }
    }

    private void OnClick_Back(GameObject obj)
    {
        CloneProxy.Instance.onClientLeaveClone();

        GameProxy.Instance.GotoMainCity();
    }

    private void UpdateDisplay()
    {
        if (null == m_Panel) return;

        m_Panel.txtTimer.text = m_timer;
        m_Panel.txtScore.text = MatchManager.LeftGoalNum + "-" + MatchManager.RightGoalNum;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (null != m_tween)
            m_tween.Kill();
        m_tween = null;
        m_skillCallList = null;

        if (TimerManager.IsHaveTimer(timer_hideJudege))
            TimerManager.Destroy(timer_hideJudege);

        if (null != m_tweener)
            m_tweener.Kill();
        m_tweener = null;
    }

    private void OnRec_Score(INotification msg)
    {
        UpdateDisplay();
    }

    //时间改变
    private void OnRec_Timer(INotification msg)
    {
        float tm = MatchManager.AtkTimer;
        m_timer = UtilTools.TimeFormat((int)tm);

        UpdateDisplay();
    }

    private void OnRec_WinSide(INotification msg)
    {
        TimerManager.AddTimer("OnRec_WinSide",0.5f,OnTimer_Complete);
    }

    private void OnTimer_Complete()
    {
        if (null == m_Panel) return;

        m_Panel.txtWin.gameObject.SetActive(true);

        m_tween = m_Panel.txtWin.transform.DOShakePosition(3, 6);
        m_tween.OnComplete(OnTween_ShakeComplete);

        TimerManager.Destroy("OnRec_WinSide");

        E_MatchWinSide winSide = MatchManager.Instace.GetWinSide();

        if (winSide == E_MatchWinSide.Left)
        {
            m_Panel.txtWin.text = "A队伍赢了";
        }
        else if (winSide == E_MatchWinSide.Left)
        {
            m_Panel.txtWin.text = "B队伍赢了";
        }
        else
        {
            m_Panel.txtWin.text = "平局";
        }
    }

    private void OnTween_ShakeComplete()
    {
        if (null != m_tween)
            m_tween.Kill();
        m_tween = null;
        m_judeIdList = null;
    }
}
