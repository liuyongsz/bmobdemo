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
public class FormationProxy : Proxy<FormationProxy>
{
    //�ͻ���������Ա����
    public static string OnClientBallerEnterTeam = "onBallerEnterTeam";
    //���󽻻�������Աλ��
    public static string OnClientBallerExchangePos = "onClientBallerExchangePos";
    //�ͻ��������������
    public static string OnClientActiveFormation = "onClientActiveFormation";
    //�ͻ��������������ID List
    public static string OnClientActiveFormationIDList = "onClientActiveFormationIDList";
    //�ͻ�����������ϵͳ
    public static string OnClientFormationSystem = "onClientFormationSystem";
    //�ͻ�������ʹ������ id
    public static string OnClientUseFormation = "onClientUseFormation";
    //�ͻ���������Ա�油ϯ���� 0 �뿪 1���� 
    public static string OnClientBenchBaller = "onClientBenchBaller";
    //�ͻ���������Ա�油ϯ���� index 
    public static string OnClientOpenBench = "onClientOpenBench";
    //�ͻ�����������ϵͳ����
    public static string OnClientFormationStrong = "onClientFormationStrong";
    //�ͻ��������油����
    public static string OnClientExchangeBench = "onClientExchangeBench";
    //�ͻ�������ı���Աλ��
    public static string OnChangeBallerPos = "onChangeBallerPos";
    //�ͻ����������ͺ������
    public static string OnClientForamtAndRelateProp = "onClientForamtAndRelateProp";
    public FormationProxy()
        : base(ProxyID.Formation)
    {
        
        KBEngine.Event.registerOut(this, "onBallerInTeamSucc");//����ɹ�����
        KBEngine.Event.registerOut(this, "onBallerExchangeSucc");//�����ɹ�����
        KBEngine.Event.registerOut(this, "getFormationSysList");//��������ϵͳ
        KBEngine.Event.registerOut(this, "activeFormationSucc");//�������ͽ����ɹ�
        KBEngine.Event.registerOut(this, "getForamtionIDList");//��������ID
        KBEngine.Event.registerOut(this, "benchResult");//������Ա�油ϯ���
        KBEngine.Event.registerOut(this, "benchBoxOpen");//������Ա�油ϯ����
        KBEngine.Event.registerOut(this, "onFormationStrongSucc");//��������ϵͳ����
        KBEngine.Event.registerOut(this, "benchChangeSucc");    //�油�����ɹ�
        KBEngine.Event.registerOut(this, "changeBallerPosSucc");    //�ı���Աλ�óɹ�
        KBEngine.Event.registerOut(this, "getFormationProp");    //�õ��������Լӳ�
        KBEngine.Event.registerOut(this, "getRelateProp");    //�õ�����Լӳ�


    }

    //�õ���������
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

    //�õ������
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


    //�ı���Աλ�óɹ�
    public void changeBallerPosSucc(object val, object val1)
    {

        int cardId = GameConvert.IntConvert(val);
        int pos = GameConvert.IntConvert(val1);
        //Debug.Log("----changeBallerPosSucc---" + cardId + "------" + pos);

        TeamBaller baller = EquipConfig.GetTeamBallerById(cardId);
        baller.pos = pos;

       
    }
    //�油�����ɹ�
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
    //��������ϵͳ����
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
    //��Ա�油�������
    public void benchBoxOpen(object val)
    {
        PlayerMediator.playerInfo.benchSize = GameConvert.IntConvert(val);
        BenchMediator.benchMediator.SetGridInfo();
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_39"), null);

    }
    //������Ա�油ϯ���
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
    //���ͽ����ɹ�
    public void activeFormationSucc(object val)
    {
        int id = GameConvert.IntConvert(val);
        GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_36"), null);

        if (!TeamFormationConfig.m_activeId_list.Contains(id))
            TeamFormationConfig.m_activeId_list.Add(id);

        TeamFormineMediator.selectPanel.SetSelectInfo();

    }


    //���ؽ�������list
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

    //������Աϵͳ�б�
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
    //����ɹ�
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

    //�����ɹ�
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
/// ��������
/// </summary>
public class AddProp
{
    public string propName;
    public int value;
}