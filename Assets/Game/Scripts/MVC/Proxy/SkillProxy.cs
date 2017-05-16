using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProxyInstance;
using System;

public enum SkllResFlag
{
    //# 技能和操作不匹配
    not_match_skill = 1,
    //# 错误的操作
    worong_op = 2,
}

public struct BuffInfo
{
    public int TargetDid;
    public int SkillId;

    public int GetCfgID()
    {
        return SkillId / 10000;
    }
}

public class CoachInfo
{
    public int id;
    public int limitTime;
    public int useTime;
    public int color;
    public int isLock;
    public string cost;
    public UITexture icon;
    public UILabel timeLabel;
    public UISprite lockSpr;
    public UISprite mask;
    public UISprite coachColor;
}

/// <summary>
/// 副本
/// </summary>
public class SkillProxy : Proxy<SkillProxy>
{

    public SkillProxy()
        : base(ProxyID.Skill)
    {
        KBEngine.Event.registerOut(this, "onSkillSucc");
        KBEngine.Event.registerOut(this, "onSkillError");
        KBEngine.Event.registerOut(this, "onAddBuffer");
        KBEngine.Event.registerOut(this, "onDelBuffer");
        KBEngine.Event.registerOut(this, "noticeClientEffect");
        KBEngine.Event.registerOut(this, "onGetCoachList");
        KBEngine.Event.registerOut(this, "onCloneGM");
        KBEngine.Event.registerOut(this, "onSkillLevelUpSucess"); 
        KBEngine.Event.registerOut(this, "onUnLockCoach"); 
        KBEngine.Event.registerOut(this, "onLevelUpCoachSucess");
        KBEngine.Event.registerOut(this, "onAddCoachTimeSucess"); 
        KBEngine.Event.registerOut(this, "onSelectSkillSucess");
        KBEngine.Event.registerOut(this, "onSkillNotUseTime");
    }

    public void noticeClientEffect(object cardId, object skillId, object effType)
    {
        Skill_EffectType type = (Skill_EffectType)effType.ToInt();
        int cid = cardId.ToInt();
        int sid = skillId.ToInt();

       MatchManager.SkillEffectType = type;

        MatchPlayerItem item = MatchManager.GetItemByDid(cid);
        SkillControl skillCon = new SkillControl();

        SkillAIConfig aicfg = InstanceProxy.Get<SkillAIConfig>();
        TD_SkillAI aiItm = aicfg.GetItem(sid);

        CommonFun.Debug("noticeClientEffect cardId:" + cid + " skillId:" +sid + " type:" + type);

        skillCon.PraseEffFuntion(aiItm);
        if (skillCon.UseGrassTactical)
        {
            item.oponents[0].WoCaoSuccess = true;
            CommonFun.Debug("Dan jia jin changs");
        }

        if (skillCon.UseGoGetBallPlayerFront)
            YueWeiSkill(item);

        switch (type)
        {
            case Skill_EffectType.effect_help_defend: AssistDefendSkill(item); break;
            case Skill_EffectType.effect_GoalPost_Help: SkillControl.ShootToGoalPost = true; break;
        }
    }

    /// <summary>越位陷阱</summary>
    private void YueWeiSkill(MatchPlayerItem item)
    {
       
    }

    /// <summary>协防补位</summary>
    private void AssistDefendSkill(MatchPlayerItem item)
    {
        item.Skill_Control.AddDefAssist = true;
    }

    ///<summary>屏蔽抢断 once</summary>
    public void onCloneGM(CloneGMFlag flag)
    {
        int type = (int)flag;
        KBEngine.Avatar.Instance.cellCall("onCloneGM", new object[] { type });
    }
    public void onGetCoachList(List<object> list)
    {
        SkillPanel.coachInfoList.Clear();
        CoachInfo coach;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> info = list[i] as Dictionary<string, object>;
            coach = new CoachInfo();
            coach.id = int.Parse(info["id"].ToString());
            coach.limitTime = int.Parse(info["limitTime"].ToString());
            coach.useTime = int.Parse(info["useTime"].ToString());
            coach.color = int.Parse(info["color"].ToString());
            coach.isLock = int.Parse(info["isLock"].ToString());
            coach.cost = info["cost"].ToString();
            SkillPanel.coachInfoList.Add(coach);
        }
        TimerManager.Destroy("coachTime");
        TimerManager.AddTimerRepeat("coachTime", 1, UpdateCoachTime);
    }

    void UpdateCoachTime()
    {
        for (int i = 0; i < SkillPanel.coachInfoList.Count; ++i)
        {
            if (SkillPanel.coachInfoList[i].isLock == 0 || SkillPanel.coachInfoList[i].useTime == 0)
                continue;
            SkillPanel.coachInfoList[i].useTime--;
            if (SkillPanel.coachInfoList[i].mask != null && SkillPanel.coachInfoList[i].timeLabel != null)
            {
                SkillPanel.coachInfoList[i].mask.fillAmount = SkillPanel.coachInfoList[i].useTime * 1.0f / SkillPanel.coachInfoList[i].limitTime;
                if (SkillPanel.coachInfoList[i].useTime == 0)
                {
                    SkillPanel.coachInfoList[i].timeLabel.gameObject.SetActive(false);
                    continue;
                }
                if (SkillPanel.coachInfoList[i].timeLabel.color != Color.green && SkillPanel.coachInfoList[i].useTime < SkillPanel.coachInfoList[i].limitTime)
                    SkillPanel.coachInfoList[i].timeLabel.color = Color.green;
                else if (SkillPanel.coachInfoList[i].timeLabel.color != Color.red && SkillPanel.coachInfoList[i].useTime >= SkillPanel.coachInfoList[i].limitTime)
                    SkillPanel.coachInfoList[i].timeLabel.color = Color.red;
                SkillPanel.coachInfoList[i].timeLabel.text = UtilTools.formatDuring(SkillPanel.coachInfoList[i].useTime);
            }
        }
        if (SkillPanel.skillPanel == null || SkillPanel.skillPanel.cloneCoachLabel == null)
            return;
        if (SkillPanel.coachInfoList[SkillPanel.skillPanel.chooseCoachIndex].useTime == 0)
            SkillPanel.skillPanel.cloneCoachLabel.gameObject.SetActive(false);
        SkillPanel.skillPanel.cloneCoachLabel.text = SkillPanel.coachInfoList[SkillPanel.skillPanel.chooseCoachIndex].timeLabel.text;
        SkillPanel.skillPanel.cloneCoachLabel.color = SkillPanel.coachInfoList[SkillPanel.skillPanel.chooseCoachIndex].timeLabel.color;
    }
    ///<summary>设置技能ID</summary>
    public void onGmSetSkill(int skillId)
    {
        KBEngine.Avatar.Instance.cellCall("onGmSetSkill", new object[] { skillId });

        GUIManager.SetJumpText("GM Set Skill Success");
    }

    /// <summary>接收Buff</summary>
    public void onAddBuffer(object targetDid, object buffId)
    {
        BuffInfo info = new BuffInfo();
        info.TargetDid = int.Parse(targetDid.ToString());
        info.SkillId = int.Parse(buffId.ToString());

        MatchPlayerItem itm = MatchManager.GetItemByDid(info.TargetDid);
        if (null != itm)
        {
            SkillConfig skillCfg = ProxyInstance.InstanceProxy.Get<SkillConfig>();
            itm.playerControl.Skill_Control.EffIcon = skillCfg.GetItem(info.GetCfgID()).icon;
        }

        Facade.SendNotification(NotificationID.MatchInfo_AddBuff, info);
    }

    public void onDelBuffer(object targetDid, object buffId)
    {
        BuffInfo info = new BuffInfo();
        info.TargetDid = int.Parse(targetDid.ToString());
        info.SkillId = int.Parse(buffId.ToString());

        MatchPlayerItem itm = MatchManager.GetItemByDid(info.TargetDid);
        if (null != itm)
        {
            itm.playerControl.Skill_Control.EffIcon = string.Empty;
        }

        Facade.SendNotification(NotificationID.MatchInfo_DelBuff, info);
    }

    /// <summary>技能 操作成功</summary>
    public void onSkillSucc(List<object> skillIDs)
    {
        if (skillIDs.Count == 0)
            return;

        CommonFun.Debug("Skill Success");

        MatchPlayerItem itm = MatchManager.GetPlayerItem(AllRef.Ball.owner);

        SkillAIConfig skillCfg = ProxyInstance.InstanceProxy.Get<SkillAIConfig>();

        int cnt = skillIDs.Count;
        for (int i = 0; i < cnt; i++)
        {
            string skillId = skillIDs[i].ToString();
            TD_SkillAI skillItm = skillCfg.GetItem(skillId);

            if (!string.IsNullOrEmpty(skillItm.trail))
                itm.playerControl.Skill_Control.Trail = skillItm.trail;

            if (!string.IsNullOrEmpty(skillItm.passTrail))
                itm.playerControl.Skill_Control.PassTrail = skillItm.passTrail;

            if (skillItm.shootSkillID > 0)
                itm.playerControl.Skill_Control.ShootSkillID = skillItm.shootSkillID;

            itm.playerControl.Skill_Control.PraseEffFuntion(skillItm);
        }

        if (itm.Skill_Control.UseGrassTactical)
        {
            itm.Skill_Control.UseGrassTactical = false;

            itm.oponents[0].Skill_Control.UseGrassTactical = true;
        }

        if (itm.Skill_Control.UseStraightBehind)
        {
            itm.Skill_Control.UseStraightBehind = false;
            MatchPlayerItem willItm = MatchManager.GetToTarget(itm);
            willItm.Skill_Control.UseStraightBehind = true;
        }

        if (itm.Skill_Control.AddDefAssist)
            itm.Skill_Control.AddDefAssist = false;

        if (itm.Skill_Control.KeepStealBall)
            itm.playerControl.KeeperStealBall();

        if (itm.Skill_Control.UseGoGetBallPlayerFront)
        {
            itm.Skill_Control.UseGoGetBallPlayerFront = false;
            MatchPlayerItem willItm = MatchManager.GetToTarget(AllRef.Ball.OwnerItem);
            willItm.Skill_Control.UseGoGetBallPlayerFront = true;
        }

        string trail = itm.playerControl.Skill_Control.Trail;
        if (!string.IsNullOrEmpty(trail))
        {
            ResourceManager.Instance.LoadPrefab(trail, OnLoaded_TrailComplete);
        }


        trail = itm.playerControl.Skill_Control.PassTrail;
        if (!string.IsNullOrEmpty(trail))
        {
            ResourceManager.Instance.LoadPrefab(trail, OnLoaded_PassTrailComplete);
        }
    }

    private void OnLoaded_TrailComplete(string key, GameObject obj)
    {
        if (AllRef.Ball.Trail != null)
            GameObject.DestroyObject(AllRef.Ball.Trail.gameObject);

        obj.transform.SetParent(AllRef.Ball.transform);
        obj.transform.localPosition = Vector3.zero;
        AllRef.Ball.Trail = obj.transform;
        AllRef.Ball.ShootShowTrail = true;
        AllRef.Ball.ShowTrail(false);

        MatchPlayerItem itm = MatchManager.GetPlayerItem(AllRef.Ball.owner);
        //itm.playerControl.Do_Shoot(itm.playerControl.Skill_Control.UseTuiShe);
    }

    private void OnLoaded_PassTrailComplete(string key, GameObject obj)
    {
        if (AllRef.Ball.PassTrail != null)
            GameObject.DestroyObject(AllRef.Ball.PassTrail.gameObject);

        obj.transform.SetParent(AllRef.Ball.transform);
        obj.transform.localPosition = Vector3.zero;
        AllRef.Ball.PassTrail = obj.transform;
        AllRef.Ball.ShowPassTrail(false);
    }

    /// <summary>技能 操作失败</summary>
    public void onSkillError(object flag)
    {
        int index = int.Parse(flag.ToString());
        switch (index)
        {
            case 3:
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_7"));
                break;
            case 5:
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_4"));
                break;
        }
    }
    public void onUnLockCoach(object obj)
    {
        int index = int.Parse(obj.ToString());
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_3"));
        SkillPanel.coachInfoList[--index].isLock = 1;
        if (TeamMediator.skillPanel == null)
            return;
        TeamMediator.skillPanel.UnLockCoachSucess(index);
    }

    public void onSkillLevelUpSucess(object flag, object time, object coach)
    {
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_1"));
        SkillPanel.coachInfoList[int.Parse(coach.ToString()) - 1].useTime = int.Parse(time.ToString());
        if (TeamMediator.skillPanel == null)
            return;
        TeamMediator.skillPanel.SkillLevelUpSucess(int.Parse(flag.ToString()), int.Parse(coach.ToString()));
    }
    public void onSkillNotUseTime(object flag, object time, object coach)
    {
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_13"));
        SkillPanel.coachInfoList[int.Parse(coach.ToString()) - 1].useTime = int.Parse(time.ToString());
        if (TeamMediator.skillPanel == null)
            return;
        TeamMediator.skillPanel.SkillLevelUpSucess(int.Parse(flag.ToString()), int.Parse(coach.ToString()));
    }
    
    public void onLevelUpCoachSucess(object flag, object time)
    { 
        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_5"));
        SkillPanel.coachInfoList[int.Parse(flag.ToString()) - 1].color++;
        SkillPanel.coachInfoList[int.Parse(flag.ToString()) - 1 ].limitTime = int.Parse(time.ToString());
        if (TeamMediator.skillPanel == null)
            return;
        TeamMediator.skillPanel.LevelUpCoach(int.Parse(flag.ToString()));
    }
    public void onAddCoachTimeSucess(object coach, object flag, object time)
    {
        switch(int.Parse(flag.ToString()))
        {
            case 0:
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_8"));
                break;
            case 1:
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_9"));
                break;
            case 2:
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_skill_10"));
                break;
        }
        SkillPanel.coachInfoList[int.Parse(coach.ToString()) - 1].useTime = int.Parse(time.ToString());
        if (TeamMediator.skillPanel == null)
            return;
        TeamMediator.skillPanel.AddCoachTime(int.Parse(coach.ToString()));
    }
    public void onSelectSkillSucess(object newSkillID, object skillLevel)
    {
        if (TeamMediator.skillPanel == null)
            return;
        TeamMediator.skillPanel.SeleceSkillSucess(int.Parse(newSkillID.ToString()), int.Parse(skillLevel.ToString()));
    }
}

