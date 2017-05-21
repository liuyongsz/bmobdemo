using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

public class guildinfopanel : BasePanel
{
    public UIButton off_btn;
    public UILabel guildname;
    public UIButton look_btn1;       //查看首领
    public UIButton look_btn2;       //查看副首领
    public UIButton look_btn3;       //查看副首领
    public UILabel guildbrief;       //公会简介
    public UITexture emblem;         //会徽
    public UILabel leadername;       //会长
    public UILabel F_leadername1;    //副会长
    public UILabel F_leadername2;    //副会长
}
public class GuildInfoMediator : UIMediator<guildinfopanel>
{
    GuildInfo mInfo;
    private guildinfopanel panel
    {
        get
        {
            return m_Panel as guildinfopanel;
        }
    }
    public List<GuildMemberInfo> mMemberList;
    public static GuildInfoMediator guildinfoMediator;
    private Dictionary<Transform, GuildMemberInfo> mTransMemInfoDict = new Dictionary<Transform, GuildMemberInfo>();

    public GuildInfoMediator() : base("guildinfopanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildInfo_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildInfo_Hide, ClosePanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (guildinfoMediator == null)
        {
            guildinfoMediator = Facade.RetrieveMediator("GuildInfoMediator") as GuildInfoMediator;
        }
        mInfo = notification.Body as GuildInfo;
        if(mInfo!=null)
            panel.guildname.text = mInfo.guildName;
        else
            panel.guildname.text = GuildMainMediator.mMyGuild.name;
        
    
        OnClientGetViceIntroduce();
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.look_btn1.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.look_btn2.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.look_btn3.gameObject).onClick = OnClick;
    }

    public void SetGuildInfo(string intro,List<GuildMemberInfo> list)
    {
        if (!GUIManager.HasView("guildinfopanel"))
            return;
        mTransMemInfoDict.Clear();
        mMemberList = list;
        for (int i =0;i<list.Count; i++)
        {
            if (list[i].power == 5)
            {
                panel.leadername.text = list[i].playerName;
                mTransMemInfoDict.Add(panel.look_btn1.transform, list[i]);
            }
            else
            {
                if(string.IsNullOrEmpty(panel.F_leadername1.text))
                {
                    panel.F_leadername1.text = list[i].playerName;
                    mTransMemInfoDict.Add(panel.look_btn2.transform, list[i]);
                }
                else
                {
                    panel.look_btn2.gameObject.SetActive(false);
                }
                if (string.IsNullOrEmpty(panel.F_leadername2.text))
                {
                    panel.F_leadername2.text = list[i].playerName;
                    mTransMemInfoDict.Add(panel.look_btn3.transform, list[i]);
                }
                else
                {
                    panel.look_btn3.gameObject.SetActive(false);
                }
            }
        }
        panel.guildbrief.text = intro;
      
        //LoadSprite.LoaderItem(panel.emblem, mInfo.guildName);
        //GuildMemberInfo Finfo.playerName = panel.F_leadername1.text;        

    }

    private void OnClick(GameObject go)
    {
        if (go == panel.off_btn.gameObject)
        {
            Facade.SendNotification(NotificationID.GuildInfo_Hide);
        }
        else
        {
            if(mTransMemInfoDict.ContainsKey(go.transform))
            {
                GuildMemberInfo info = mTransMemInfoDict[go.transform];
                UtilTools.RoleShowInfo(info.id);
            }
            Facade.SendNotification(NotificationID.GuildInfo_Hide);

        }
    }
    //申请公会副会长及简介
    public void OnClientGetViceIntroduce()
    {
        int id = mInfo != null ? mInfo.id : GuildMainMediator.mMyGuild.id;
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientGetViceIntroduce, id);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
