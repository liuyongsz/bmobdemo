using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

/// <summary>
/// 背包
/// </summary>
public class FormationProxy : Proxy<FormationProxy>
{
    //客户端请求球员上阵
    public static string OnClientBallerEnterTeam = "onBallerEnterTeam";
    //请求交换两个球员位置
    public static string OnClientBallerExchangePos = "onClientBallerExchangePos";
    //客户端请求解锁阵型
    public static string OnClientActiveFormation = "onClientActiveFormation";
    //客户端请求解锁阵型ID List
    public static string OnClientActiveFormationIDList = "onClientActiveFormationIDList";
    //客户端请求阵型系统
    public static string OnClientFormationSystem = "onClientFormationSystem";
    //客户端请求使用阵型 id
    public static string OnClientUseFormation = "onClientUseFormation";
    //客户端请求球员替补席处理 0 离开 1进入 
    public static string OnClientBenchBaller = "onClientBenchBaller";
    //客户端请求球员替补席开启 index 
    public static string OnClientOpenBench = "onClientOpenBench";
    //客户端请求阵型系统升级
    public static string OnClientFormationStrong = "onClientFormationStrong";
    //客户端请求替补交换
    public static string OnClientExchangeBench = "onClientExchangeBench";
    //客户端请求改变球员位置
    public static string OnChangeBallerPos = "onChangeBallerPos";
    //客户端请求阵型和羁绊属性
    public static string OnClientForamtAndRelateProp = "onClientForamtAndRelateProp";
    public FormationProxy()
        : base(ProxyID.Formation)
    {
        
        KBEngine.Event.registerOut(this, "onBallerInTeamSucc");//上阵成功返回
        KBEngine.Event.registerOut(this, "onBallerExchangeSucc");//交换成功返回
        KBEngine.Event.registerOut(this, "getFormationSysList");//返回阵型系统
        KBEngine.Event.registerOut(this, "activeFormationSucc");//返回阵型解锁成功
        KBEngine.Event.registerOut(this, "getForamtionIDList");//返回阵型ID
        KBEngine.Event.registerOut(this, "benchResult");//返回球员替补席结果
        KBEngine.Event.registerOut(this, "benchBoxOpen");//返回球员替补席开启
        KBEngine.Event.registerOut(this, "onFormationStrongSucc");//返回阵型系统升级
        KBEngine.Event.registerOut(this, "benchChangeSucc");    //替补交换成功
        KBEngine.Event.registerOut(this, "changeBallerPosSucc");    //改变球员位置成功
        KBEngine.Event.registerOut(this, "getFormationProp");    //得到阵型属性加成
        KBEngine.Event.registerOut(this, "getRelateProp");    //得到羁绊属性加成


    }

    //得到阵型属性
    public void getFormationProp(object val, List<object> list)
    {
        int formationsysId = GameConvert.IntConvert(val);
        TeamFormationConfig.mForamtionProp.Clear();

        AddProp info;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;

            string name = GameConvert.StringConvert(data["propName"]);
            int  value = GameConvert.IntConvert(data["value"]);
            info = TeamFormationConfig.GetFormationAddProp(name);

            info.propName = name;
            info.value += value;
            TeamFormationConfig.mForamtionProp.Add(info);
        }


    }

    //得到羁绊属性
    public void getRelateProp(object val,object val1, List<object> list)
    {
        int cardId = GameConvert.IntConvert(val);
        int fight = GameConvert.IntConvert(val1);
        

        TeamFormationConfig.RelateAdddPropClear();

        List<AddProp> addList = new List<AddProp>();
        AddProp info;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;

            string name = GameConvert.StringConvert(data["propName"]);
            int value = GameConvert.IntConvert(data["value"]);
            info = TeamFormationConfig.GetRelateAddProp(cardId,name);

            info.propName = name;
            info.value += value;
            addList.Add(info);
        }
        if (TeamFormationConfig.mRelatProp.ContainsKey(cardId))
            TeamFormationConfig.mRelatProp.Add(cardId,addList);

        TeamBaller baller = EquipConfig.GetTeamBallerById(cardId);
        if (baller != null)
            baller.fightValue = fight;
        Facade.SendNotification(NotificationID.BallerFight_Change, cardId);

    }


    //改变球员位置成功
    public void changeBallerPosSucc(object val, object val1)
    {

        int cardId = GameConvert.IntConvert(val);
        int pos = GameConvert.IntConvert(val1);
        //Debug.Log("----changeBallerPosSucc---" + cardId + "------" + pos);

        TeamBaller baller = EquipConfig.GetTeamBallerById(cardId);
        baller.pos = pos;

       
    }
    //替补交换成功
    public void benchChangeSucc(object val,object val1)
    {

        int changId = GameConvert.IntConvert(val);
        int cardId = GameConvert.IntConvert(val1);

        TeamBaller changBaller = EquipConfig.GetTeamBallerById(changId);
        changBaller.bench = 0;

        TeamBaller baller = EquipConfig.GetTeamBallerById(cardId);
        baller.bench = 1;
        //GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_43"), null);

        BenchMediator.benchMediator.RefreshBench();

        TeamFormineMediator.teamFormineMediator.SetBallerGrid();
    }
    //返回阵型系统升级
    public void onFormationStrongSucc(Dictionary<string, object> data)
    {
        FormationSysInfo info = null;

        info = new FormationSysInfo();

        info.id = GameConvert.IntConvert(data["id"]);
        info.strongLevel = GameConvert.IntConvert(data["strongLevel"]);
        info.active = GameConvert.IntConvert(data["active"]);
        info.open = GameConvert.IntConvert(data["open"]);

        if (TeamFormationSystemConfig.m_formation_sys.ContainsKey(info.id))
            TeamFormationSystemConfig.m_formation_sys[info.id] = info;
        else
            TeamFormationSystemConfig.m_formation_sys.Add(info.id, info);

        TeamFormineMediator.mIsCanUpdata = true;

        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_41"), null);
        TeamFormineMediator.strongPanel.SetInfo();
    }
    //球员替补开启结果
    public void benchBoxOpen(object val)
    {
        PlayerMediator.playerInfo.benchSize = GameConvert.IntConvert(val);
        BenchMediator.benchMediator.SetGridInfo();
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_39"), null);

    }
    //返回球员替补席结果
    public void benchResult(object val,object val1)
    {
        int type = GameConvert.IntConvert(val);
        int cardId = GameConvert.IntConvert(val1);

        TeamBaller baller = EquipConfig.GetTeamBallerById(cardId);
        baller.bench = type;
        //string name = TextManager.GetItemString(baller.configId);
        //string content = "";

        //if (type == 1)
        //    content = TextManager.GetSystemString("ui_system_37");
        //else
        //    content = TextManager.GetSystemString("ui_system_38");

        //GUIManager.SetPromptInfo(string.Format(content, name), null);

        BenchMediator.benchMediator.RefreshBench();

        TeamFormineMediator.teamFormineMediator.SetBallerGrid();
    }
    //阵型解锁成功
    public void activeFormationSucc(object val)
    {
        int id = GameConvert.IntConvert(val);
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_36"), null);

        if (!TeamFormationConfig.m_activeId_list.Contains(id))
            TeamFormationConfig.m_activeId_list.Add(id);

        TeamFormineMediator.selectPanel.SetSelectInfo();

    }


    //返回解锁阵型list
    public void getForamtionIDList(List<object> list)
    {
        for(int i=0; i<list.Count;i++)
        {
            int id = GameConvert.IntConvert(list[i]);
            if (!TeamFormationConfig.m_activeId_list.Contains(id))
                TeamFormationConfig.m_activeId_list.Add(id);
        }

        TeamFormineMediator.selectPanel.SetSelectInfo();

    }

    //返回球员系统列表
    public void getFormationSysList(List<object> list)
    {
        FormationSysInfo info = null;
        for (int i = 0; i < list.Count; ++i)
        {
            Dictionary<string, object> data = list[i] as Dictionary<string, object>;

            info = new FormationSysInfo();
            info.id = GameConvert.IntConvert(data["id"]);
            info.strongLevel = GameConvert.IntConvert(data["strongLevel"]);
            info.active = GameConvert.IntConvert(data["active"]);
            info.open = GameConvert.IntConvert(data["open"]);

            if (TeamFormationSystemConfig.m_formation_sys.ContainsKey(info.id))
                TeamFormationSystemConfig.m_formation_sys[info.id] = info;
            else
                TeamFormationSystemConfig.m_formation_sys.Add(info.id, info);
            
        }
        TeamFormineMediator.teamFormineMediator.OnClientActiveFormationIDList();
        TeamFormineMediator.teamFormineMediator.SetSwitchFunc();

    }
    //上阵成功
    public void onBallerInTeamSucc(object val ,object val1 , object val2)
    {
        int cardId = GameConvert.IntConvert(val);
        int cardNotTeam = GameConvert.IntConvert(val1);
        int pos = GameConvert.IntConvert(val2);

        TeamBaller baller = EquipConfig.GetTeamBallerById(cardId);
        baller.inTeam = 1;
        baller.pos = pos;
        baller.bench = 0;
        if (cardNotTeam < 0)
            return;
        TeamBaller ballerNotTeam = EquipConfig.GetTeamBallerById(cardNotTeam);
        ballerNotTeam.inTeam = 0;
        ballerNotTeam.pos = 0;
        ballerNotTeam.bench = 0;

        TeamFormineMediator.teamFormineMediator.SetBallerGrid();

    }

    //交换成功
    public void onBallerExchangeSucc(object val, object val1)
    {
        int cardId = GameConvert.IntConvert(val);
        int exchangId = GameConvert.IntConvert(val1);

        TeamBaller baller = EquipConfig.GetTeamBallerById(cardId);
        TeamBaller changeBaller = EquipConfig.GetTeamBallerById(exchangId);

        int pos = baller.pos;
        baller.pos = changeBaller.pos;

        changeBaller.pos = pos;
    }

}

/// <summary>
/// 属性增加
/// </summary>
public class AddProp
{
    public string propName;
    public int value;
}