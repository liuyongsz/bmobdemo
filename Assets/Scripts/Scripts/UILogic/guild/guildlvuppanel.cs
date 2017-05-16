using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;
using System.Collections.Generic;

public class guildlvuppanel : BasePanel
{
    public UIButton off_btn;

    public UILabel upinfo1;
    public UILabel upinfo2;
    public UILabel upinfo3;
    public UILabel rightname;
    public UILabel nextLVneed;
    public UILabel consume;
    public UILabel needtime;
    public UIButton LVUP_btn;

    public UIGrid buildGrid;
}
public class GuildLVUpMediator : UIMediator<guildlvuppanel>
{

    private guildlvuppanel panel
    {
        get
        {
            return m_Panel as guildlvuppanel;
        }
    }

    public static GuildLVUpMediator guildlvupMediator;

    public MyGuildInfo myguildinfo = GuildMainMediator.mMyGuild;
    public GuildBuildInfo guildbuildinfo;
    private UICenterOnChild mCenterOnChild = null;
    public GuildLVUpMediator() : base("guildlvuppanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildLVUp_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildLVUp_Hide, ClosePanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (guildlvupMediator == null)
        {
            guildlvupMediator = Facade.RetrieveMediator("GuildLVUpMediator") as GuildLVUpMediator;
        }

        guildbuildinfo = notification.Body as GuildBuildInfo;
        
        mCenterOnChild = panel.buildGrid.transform.GetComponent<UICenterOnChild>();
        mCenterOnChild.onFinished = OnFinishedFun;

        panel.buildGrid.enabled = true;
        panel.buildGrid.BindCustomCallBack(UpdateBuildGrid);
        panel.buildGrid.StartCustom();
        
        SetUpLvInfo();
        SetBuildGride();
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.LVUP_btn.gameObject).onClick = OnClick;


   
    }
    //
    private void SetBuildGride()
    {
        List<object> listObj = new List<object>();

        List<GuildBuildInfo> list = GuildBuildConfig.GetGuildBuildList();
        for (int i=0; i< list.Count;i++)
        {
            listObj.Add(list[i]);
        }

        panel.buildGrid.AddCustomDataList(listObj);

    }
    private void OnClick(GameObject go)
    {
  
        if (go == panel.LVUP_btn.gameObject)
        {
            int needFunds = GuildMainMediator.guildmainMediator.GetNeedFunds(guildbuildinfo.id, guildbuildinfo.level);

            if (GuildMainMediator.mMyGuild.guildFunds < needFunds)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err17"));
                return;
            }
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientBuildUpgrade, guildbuildinfo.id);
        }

        Facade.SendNotification(NotificationID.GuildLVUp_Hide);
        
    }

    private void UpdateBuildGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        GuildBuildInfo info = item.oData as GuildBuildInfo;

        UILabel name = item.mScripts[0] as UILabel;
        UITexture build = item.mScripts[1] as UITexture;
        UILabel level = item.mScripts[2] as UILabel;
        LoadSprite.LoaderBuild(build, "build" + info.id, false);
        level.text = info.level + TextManager.GetUIString("UIGuild29");
        name.text = TextManager.GetUIString("UIGuildBuild"+ info.id);

        if(info.id == guildbuildinfo.id)
            mCenterOnChild.CenterOn(item.transform);


    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    private void OnFinishedFun()
    {
        if (mCenterOnChild.centeredObject == null) return;

        UIGridItem item = mCenterOnChild.centeredObject.transform.GetComponent<UIGridItem>();
        GuildBuildInfo info = item.oData as GuildBuildInfo;
        if (guildbuildinfo == null|| info==null || info.id == guildbuildinfo.id)
            return;
        guildbuildinfo = info;
        SetUpLvInfo();

    }
    private void SetUpLvInfo()
    {
        //LoadSprite.LoaderBuild(panel.build, "build"+ guildbuildinfo.id, false);

        //panel.buildlv.text = guildbuildinfo.level + TextManager.GetUIString("UIGuild29");
        GuildBuildInfo info = guildbuildinfo;
        if (guildbuildinfo.id==1)           //公会大厅
        {
            //LoadSprite.LoaderImage(panel.build, "bg/qiuchang", false);          //建筑图片
            panel.rightname.text = TextManager.GetUIString("UIGuild24");
            GuildUpHall hallinfo = GuildUpHallConfig.GetGuildUpHall(info.level);
            panel.upinfo1.text = "1:" + string.Format(TextManager.GetUIString("UI_GuildBuild4"), hallinfo.addNum);
            panel.upinfo2.text = "2:" + string.Format(TextManager.GetUIString("UI_GuildBuild5"), hallinfo.reputation);
            panel.upinfo3.text = "3:" + string.Format(TextManager.GetUIString("UI_GuildBuild6"), hallinfo.inspect);
            panel.nextLVneed.text = string.Format(TextManager.GetUIString("UI_GuildBuild0"), info.level);
            panel.consume.text = hallinfo.needFunds.ToString();
            panel.needtime.text = UtilTools.formatDuring(hallinfo.needTime*60*60);
        }
        else if (guildbuildinfo.id == 2)    //公会商城
        {
            panel.rightname.text = TextManager.GetUIString("UIshop11");
            GuildUpShop shopinfo = GuildUpShopConfig.GetGuildUpShop(info.level);
            panel.upinfo1.text = "1:" + TextManager.GetUIString("UI_GuildBuild13");
            panel.upinfo2.text = "2:" + TextManager.GetUIString("UI_GuildBuild14");
            panel.upinfo3.text = "";
        panel.nextLVneed.text = string.Format(TextManager.GetUIString("UI_GuildBuild0"), info.level);
            panel.consume.text = shopinfo.needFunds.ToString();
            panel.needtime.text = UtilTools.formatDuring(shopinfo.needTime * 60*60); 
        }
        else if (guildbuildinfo.id == 3)    //顾问大厅
        {
            panel.rightname.text = TextManager.GetUIString("UIGuild23");
            GuildUpCounselor Counselorinfo = GuildUpCounselorConfig.GetGuildUpCounselor(info.level);
            panel.upinfo1.text = "1:" + string.Format(TextManager.GetUIString("UI_GuildBuild1"), Counselorinfo.preSucc);
            panel.upinfo2.text = "2:" + string.Format(TextManager.GetUIString("UI_GuildBuild2"), Counselorinfo.protectTime);
            panel.upinfo3.text = "3:" + string.Format(TextManager.GetUIString("UI_GuildBuild3"), Counselorinfo.counselorNum);
            panel.nextLVneed.text = string.Format(TextManager.GetUIString("UI_GuildBuild0"), info.level);
            panel.consume.text = Counselorinfo.needFunds.ToString();
            panel.needtime.text = UtilTools.formatDuring(Counselorinfo.needTime * 60*60); 
        }
        else if (guildbuildinfo.id == 4)    //任务大厅
        {
            panel.rightname.text = TextManager.GetUIString("UIGuild21");
            GuildUpTask taskinfo = GuildUpTaskConfig.GetGuildUpTask(info.level);
            panel.upinfo1.text = "1:" + string.Format(TextManager.GetUIString("UI_GuildBuild7"), taskinfo.addNum);
            panel.upinfo2.text = "2:" + string.Format(TextManager.GetUIString("UI_GuildBuild2"), taskinfo.openType);
            panel.upinfo3.text = "";
            panel.nextLVneed.text = string.Format(TextManager.GetUIString("UI_GuildBuild0"), info.level);
            panel.consume.text = taskinfo.needFunds.ToString();
            panel.needtime.text = UtilTools.formatDuring(taskinfo.needTime * 60*60); 
        }
      
    }
}
