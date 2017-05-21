using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;
using System.Collections.Generic;

/// <summary>
/// 顾问状态
/// </summary>
public enum adviser_state_Type
{
    target=1,   //设目标
    side=2,     //已拉拢
    other=3     //未拉拢
}
/// <summary>
/// 讨好类型
/// </summary>

public enum Operation_Type
{
    way1 =1,//拍马屁
    way2 =2,//拜访
    way3 =3,//宴请
    way4 =4,//送礼
    way5 =5,//流言
    way6 =6,//蜚语
    way7 =7,//反间
    way8 =8,//美人计
    way9 =9,//晓之以理
    way10 =10,//诱之以利
}

public class guildcounselorpanel : BasePanel
{
    
    public UISprite offBtn;
    public UISprite incidentBtn;        //事件
    public UISprite explainBtn;         //说明
    public UIGrid counselorGrid;        //顾问列表
    public UITexture whole_length;      //顾问全身像
    public UISprite targetBtn;          //设目标
    public UISprite amityBtn;           //讨好
    public UISprite inciteBtn;          //挑拨
    public UISprite cozyuptoBtn;        //拉拢
    public UILabel count;               //已拉拢/可拉拢
    public UIGrid rankGrid;             //友好度排行
    public UILabel mguildrank;          //我的公会对该顾问友好度排行
    public UILabel mguildname;          //我的公会名字
    public UILabel mguildamity;         //我的公会对该顾问友好度
    public UITexture buffpicture;       //顾问加成效果图
    public UILabel buff;                //加成
    public Transform playuptopanel;     //讨好
    public UITexture Phead;             //所选顾问头像
    public UILabel Pname;               //顾问名字
    public UILabel Potherguildname;     //顾问所属公会名字
    public UILabel Potheramity;         //顾问所属公会友好度
    public UISprite Pour;               //我方
    public UILabel Pourguildname;       //公会名字
    public UILabel Pouramity;           //顾问对我方友好度
    public UILabel Psuccessrate;
    public UILabel Pneeditem;
    public UILabel PneedEuro;
    public UILabel Pgetcontribution;
    public UILabel Pamityvalue;
    public UISprite surepalyuptoBtn;      //讨好按钮
    public UIToggle playupto1;          //拍马屁
    public UIToggle playupto2;          //拜访
    public UIToggle playupto3;          //宴请
    public UIToggle playupto4;          //送礼
    public UISprite offoplayuptoBtn;
    public Transform instigatepanel;    //挑拨
    public UITexture Ihead;             //所选顾问头像
    public UILabel Iname;               //顾问名字
    public UILabel Iotherguildname;     //顾问所属公会名字
    public UILabel Iotheramity;         //顾问所属公会友好度
    public UISprite Iour;               //我方
    public UILabel Iourguildname;       //公会名字
    public UILabel Iouramity;           //顾问对我方友好度
    public UILabel Isuccessrate;
    public UILabel Ineeditem;
    public UILabel IneedEuro;
    public UILabel Igetcontribution;
    public UILabel Iamityvalue;
    public UILabel Itheir;
    public UISprite sureinstigateBtn;      //挑拨按钮
    public UIToggle instigate1;         //流言
    public UIToggle instigate2;         //蜚语
    public UIToggle instigate3;         //反间
    public UIToggle instigate4;         //美人计
    public UISprite offinstigateBtn;
    public Transform ropepanel;         //拉拢
    public UITexture Rhead;             //所选顾问头像
    public UILabel Rname;               //顾问名字
    public UILabel Rotherguildname;     //顾问所属公会名字
    public UILabel Rotheramity;         //顾问所属公会友好度
    public UISprite Rour;               //我方
    public UILabel Rourguildname;       //公会名字
    public UILabel Rouramity;           //顾问对我方友好度
    public UILabel Rsuccessrate;
    public UILabel Rneedfund;
    public UILabel Rgetcontribution;
    public UILabel Ramityvalue;
    public UILabel Rtheir;
    public UILabel Rcount;
    public UILabel Rneeddiamond;
    public UISprite sureropeBtn;         //拉拢按钮
    public UIToggle rope1;             //晓之以理
    public UIToggle rope2;             //诱之以利
    public UISprite offropeBtn;

}
public class GuildCounselorMediator : UIMediator<guildcounselorpanel>
{
    private guildcounselorpanel panel
    {
        get
        {
            return m_Panel as guildcounselorpanel;
        }
    }
    public static GuildCounselorMediator guildcounselorMediator;   
    private adviser_state_Type adviserState;
    private GuildAdviser mCurAdviser;
    private Operation_Type mDealId;  // 顾问处理ID

    public GuildCounselorMediator() : base("guildcounselorpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildCounselor_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildCounselor_Hide, ClosePanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (guildcounselorMediator == null)
        {
            guildcounselorMediator = Facade.RetrieveMediator("GuildCounselorMediator") as GuildCounselorMediator;
        }
        OnClientAdviserList();

        panel.counselorGrid.enabled = true;
        panel.counselorGrid.BindCustomCallBack(UpdateCounselorGrid);
        panel.counselorGrid.StartCustom();
        
        panel.rankGrid.enabled = true;
        panel.rankGrid.BindCustomCallBack(UpdateRankGrid);
        panel.rankGrid.StartCustom();

    }
    private void  InitAdviserNum()
    {
        GuildBuildInfo buildInfo = GuildBuildConfig.mGuildBuildDict[3];
        GuildUpCounselor upConuselor = GuildUpCounselorConfig.GetGuildUpCounselor(buildInfo.level);
        int hasNum = GuildCounselorConfig.GetRopeAdviserNum(PlayerMediator.playerInfo.guildDBID);
        int totalNum = upConuselor.counselorNum;

        panel.count.text = hasNum + "/" + totalNum;
    }

    private void SetAdviserTexture()
    {

        AdviserInfo info = GuildCounselorConfig.GetAdviserInfo(mCurAdviser.id);

        string buffType = null;
        float buff = 0;
        buffType = GuildCounselorConfig.GetGuildCounselor(info.configID).buffType;
        buff = GuildCounselorConfig.GetGuildCounselor(info.configID).buff;
        if (buff < 1)
        {
            panel.buff.text = TextManager.GetUIString(buffType) +"+"+ (buff * 100) + "%";
        }
        else
        {
            panel.buff.text = TextManager.GetUIString(buffType) +"+"+ buff;
        }


        InitAdviserNum();

        panel.mguildname.text = GuildMainMediator.mMyGuild.name;
        LoadSprite.LoaderAdviser(panel.whole_length, "adviser" + info.configID);


        panel.mguildamity.text = mCurAdviser.friendliness.ToString();
        panel.Pouramity.text = mCurAdviser.friendliness.ToString();
        panel.Iouramity.text = mCurAdviser.friendliness.ToString();
        panel.Rouramity.text = mCurAdviser.friendliness.ToString();

        LoadSprite.LoaderAdviser(panel.Phead,"adviserHead"+info.configID);
        string strname = GuildCounselorConfig.GetGuildCounselor(info.configID).counselorname;
        panel.Pname.text = TextManager.GetItemString(strname);
        panel.Potherguildname.text = info.guilidName.ToString();
        panel.Potheramity.text = info.friendliness.ToString();
        panel.Pourguildname.text = GuildMainMediator.mMyGuild.name;
        if (info.guilidName == GuildMainMediator.mMyGuild.name)
        {
            panel.Pour.gameObject.SetActive(false);
            panel.Pourguildname.gameObject.SetActive(false);
            panel.Pouramity.gameObject.SetActive(false);
        }


        LoadSprite.LoaderAdviser(panel.Ihead, "adviserHead" + info.configID);
        panel.Iname.text = TextManager.GetItemString(strname);
        panel.Iotherguildname.text = info.guilidName.ToString();
        panel.Iotheramity.text = info.friendliness.ToString();
        panel.Iourguildname.text = GuildMainMediator.mMyGuild.name;
        if (info.guilidName == GuildMainMediator.mMyGuild.name)
        {
            panel.Iour.gameObject.SetActive(false);
            panel.Iourguildname.gameObject.SetActive(false);
            panel.Iouramity.gameObject.SetActive(false);
        }


        LoadSprite.LoaderAdviser(panel.Rhead, "adviserHead" + info.configID);
        panel.Rname.text = TextManager.GetItemString(strname);
        panel.Rotherguildname.text = info.guilidName.ToString();
        panel.Rotheramity.text = info.friendliness.ToString();
        panel.Rourguildname.text = GuildMainMediator.mMyGuild.name;
        if (info.guilidName == GuildMainMediator.mMyGuild.name)
        {
            panel.Rour.gameObject.SetActive(false);
            panel.Rourguildname.gameObject.SetActive(false);
            panel.Iouramity.gameObject.SetActive(false);
        }
    }


    //添加公会顾问列表
    public void SetAdviserList()
    {
        List<object> list = new List<object>();
        List<GuildAdviser> adviserList = GuildMainMediator.mMyGuild.adviserList;
        for (int i =0; i< adviserList.Count; i++)
        {
            if (mCurAdviser == null)
            {
                mCurAdviser = adviserList[0];
                OnClientAdviserGuildList(mCurAdviser.id);

            }
            else
            {
                if (mCurAdviser.id == adviserList[i].id)
                    mCurAdviser = adviserList[i];
            }
            list.Add(adviserList[i]);
        }
        SetAdviserTexture();
        panel.counselorGrid.AddCustomDataList(list);


    }

    //添加右侧信息列表
    public void SetAdviserGuildList()
    {
        List<AdviserGuildInfo> adviserguildlist = GuildCounselorConfig.mGuildFriendsList;
        adviserguildlist.Sort(AdviserSort);
        List<object> list = new List<object>();
        for( int i=0; i<adviserguildlist.Count; i++)
        {
            AdviserGuildInfo item = adviserguildlist[i];
            item.rank = i + 1;
            list.Add(item);
        }
      
        panel.rankGrid.AddCustomDataList(list);
    }
    //刷新顾问列表
    private void UpdateCounselorGrid(UIGridItem item)
    {
        GuildAdviser info = item.oData as GuildAdviser;
        AdviserInfo adviser = GuildCounselorConfig.GetAdviserInfo(info.id);

        UITexture head = item.mScripts[0] as UITexture;
        UISprite biaoqian = item.mScripts[1] as UISprite;
        UILabel biaoqianlabel = item.mScripts[2] as UILabel;
        UILabel name = item.mScripts[3] as UILabel;
        UILabel amity = item.mScripts[4] as UILabel;
        UISprite designate = item.mScripts[5] as UISprite;
        item.onClick = OnClickAdviser;
        string strname = GuildCounselorConfig.GetGuildCounselor(adviser.configID).counselorname;
        name.text = TextManager.GetItemString(strname);
        amity.text = info.friendliness.ToString();

        LoadSprite.LoaderAdviser(head, "adviserHead" + adviser.configID);

        string content = PlayerMediator.playerInfo.guildDBID == adviser.guildDBID ? "UI_GuildConuselor29" : "UI_GuildConuselor30";
        biaoqianlabel.text = TextManager.GetUIString(content);
    }
    //刷新公会列表
    private void UpdateRankGrid(UIGridItem item)
    {
        AdviserGuildInfo info = item.oData as AdviserGuildInfo;
        UILabel rankitem = item.mScripts[0] as UILabel;
        UILabel guildname = item.mScripts[1] as UILabel;
        UILabel amity = item.mScripts[2] as UILabel;
        UISprite lalong = item.mScripts[3] as UISprite;
        rankitem.text = info.rank.ToString();
        guildname.text = info.guildName;
        amity.text = info.friendliness.ToString();

        AdviserInfo data = GuildCounselorConfig.GetAdviserInfo(mCurAdviser.id);
        lalong.gameObject.SetActive(data.guildDBID == info.guildID);
    }
    //点击顾问
    private void OnClickAdviser(UIGridItem item)
    {
        if (item == null || item.oData == null)
            return;

        mCurAdviser = item.oData as GuildAdviser;

        SetAdviserTexture();
    }



    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.offBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.incidentBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.explainBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.targetBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.amityBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.inciteBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.cozyuptoBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.offoplayuptoBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.offinstigateBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.offropeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.surepalyuptoBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sureinstigateBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sureropeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.playupto1.gameObject).onClick = OnClickPalyUpTo;
        UIEventListener.Get(panel.playupto2.gameObject).onClick = OnClickPalyUpTo;
        UIEventListener.Get(panel.playupto3.gameObject).onClick = OnClickPalyUpTo;
        UIEventListener.Get(panel.playupto4.gameObject).onClick = OnClickPalyUpTo;
        UIEventListener.Get(panel.instigate1.gameObject).onClick = OnClickInstigate;
        UIEventListener.Get(panel.instigate2.gameObject).onClick = OnClickInstigate;
        UIEventListener.Get(panel.instigate3.gameObject).onClick = OnClickInstigate;
        UIEventListener.Get(panel.instigate4.gameObject).onClick = OnClickInstigate;
        UIEventListener.Get(panel.rope1.gameObject).onClick = OnClickRope;
        UIEventListener.Get(panel.rope2.gameObject).onClick = OnClickRope;
    }

    private void OnClick(GameObject go)
    {
        switch (go.transform.name)
        {
            case "offBtn":
                {
                    Facade.SendNotification(NotificationID.GuildCounselor_Hide);
                }
                break;
            case "incidentBtn":
                {

                }
                break;
            case "explainBtn":
                {

                }
                break;
            case "targetBtn":
                {

                }
                break;
            case "amityBtn":
                {
                    panel.playuptopanel.gameObject.SetActive(true);
                    SetPlayuotoInfo(1);
                    mDealId = Operation_Type.way1;
                }
                break;
            case "inciteBtn":
                {
                    panel.instigatepanel.gameObject.SetActive(true);
                    SetInstigateInfo(5);
                    mDealId = Operation_Type.way5;

                }
                break;
            case "cozyuptoBtn":
                {
                    panel.ropepanel.gameObject.SetActive(true);
                    SetRopeInfo(9);
                    mDealId = Operation_Type.way9;
                }
                break;
            case "offoplayuptoBtn":
                {
                    panel.playuptopanel.gameObject.SetActive(false);
                }
                break;
            case "offinstigateBtn":
                {
                    panel.instigatepanel.gameObject.SetActive(false);
                }
                break;
            case "offropeBtn":
                {
                    panel.ropepanel.gameObject.SetActive(false);
                }
                break;
            case "surepalyuptoBtn":
            case "sureinstigateBtn":
            case "sureropeBtn":
                {
                    SendType();
                }
                break;
     
            default:
                break;
        }
    }

    //讨好toggle
    private void OnClickPalyUpTo(GameObject go)
    {

        if (go == panel.playupto1.gameObject)
        {
            SetPlayuotoInfo(1);
            mDealId = Operation_Type.way1;
        }
        else if (go == panel.playupto2.gameObject)
        {
            SetPlayuotoInfo(2);
            mDealId = Operation_Type.way2;

        }
        else if (go == panel.playupto3.gameObject)
        {
            SetPlayuotoInfo(3);
            mDealId = Operation_Type.way3;

        }
        else if (go == panel.playupto4.gameObject)
        {
            SetPlayuotoInfo(4);
            mDealId = Operation_Type.way4;

        }
    }
    //挑拨toggle
    private void OnClickInstigate(GameObject go)
    {
        if (go == panel.instigate1.gameObject)
        {
            SetInstigateInfo(5);
            mDealId = Operation_Type.way5;

        }
        else if (go == panel.instigate2.gameObject)
        {
            SetInstigateInfo(6);
            mDealId = Operation_Type.way6;

        }
        else if (go == panel.instigate3.gameObject)
        {
            SetInstigateInfo(7);
            mDealId = Operation_Type.way7;

        }
        else if (go == panel.instigate4.gameObject)
        {
            SetInstigateInfo(8);
            mDealId = Operation_Type.way8;

        }
    }

    private void OnClickRope(GameObject go)
    {
        if (go=panel.rope1.gameObject)
        {            
            SetRopeInfo(9);
            mDealId = Operation_Type.way9;

        }
        else if (go=panel.rope2.gameObject)
        {
            SetRopeInfo(10);
            mDealId = Operation_Type.way10;

        }
    }



    //友好度排序
    private int AdviserSort(AdviserGuildInfo info1, AdviserGuildInfo info2)
    {
        if (info1.friendliness < info2.friendliness)        
            return 1;
        else if (info1.friendliness > info2.friendliness)        
            return -1;
        else
            return 0;
    }

    //讨好信息
    private void SetPlayuotoInfo(int id)
    {
        string[] item = null;
        int itemid = 0;
        int needitem = 0;
        int hasitem = 0;
        GuildCounseloroperation info = GuildCounseloroperationConfig.GetGuildCounseloroperation(id);
        panel.Psuccessrate.text = info.successrate.ToString() + "%";
        item = info.consumeitem.Split(':');
        itemid = GameConvert.IntConvert(item[0]);
        needitem = GameConvert.IntConvert(item[1]);
        hasitem = ItemManager.GetBagItemCount(itemid.ToString());
        panel.Pneeditem.text = hasitem + "/" + needitem;
        panel.PneedEuro.text = info.consumeEuro.ToString();
        panel.Pgetcontribution.text = info.contribute.ToString();
        panel.Pamityvalue.text = info.addamity.ToString();
    }
    //挑拨信息
    private void SetInstigateInfo(int id)
    {
        string[] item = null;
        int itemid = 0;
        int needitem = 0;
        int hasitem = 0;
        GuildCounseloroperation info = GuildCounseloroperationConfig.GetGuildCounseloroperation(id);
        panel.Isuccessrate.text = info.successrate.ToString() + "%";
        item = info.consumeitem.Split(':');
        itemid = GameConvert.IntConvert(item[0]);
        needitem = GameConvert.IntConvert(item[1]);
        hasitem = ItemManager.GetBagItemCount(itemid.ToString());
        panel.Ineeditem.text = hasitem + "/" + needitem;
        panel.IneedEuro.text = info.consumeEuro.ToString();
        panel.Igetcontribution.text = info.contribute.ToString();
        panel.Iamityvalue.text = info.addamity.ToString();
        panel.Itheir.text = info.addamity.ToString();

    }
    //拉拢信息
    private void SetRopeInfo(int id)
    {
        GuildCounseloroperation info = GuildCounseloroperationConfig.GetGuildCounseloroperation(id);
        panel.Rsuccessrate.text = info.successrate.ToString() + "%";
        panel.Rneedfund.text = info.consumefund.ToString();
        panel.Rgetcontribution.text = info.contribute.ToString();
        panel.Ramityvalue.text = info.addamity.ToString(); 
        panel.Rtheir.text = info.addamity.ToString();
        panel.Rcount.text = "2/2";//剩余次数  待修改
        panel.Rneeddiamond.text = info.consumediamond.ToString();
    }
    

    //客户端请求顾问列表
    private void OnClientAdviserList()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAdviserList);
    }
    //获取顾问公会友好度列表
    private void OnClientAdviserGuildList(int adviserID)
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAdviserGuildList, adviserID);

    }

    private void SendType()
    {
        int id = GameConvert.IntConvert(mDealId);

        GuildCounseloroperation info = GuildCounseloroperationConfig.GetGuildCounseloroperation(id);
        string[] item = null;
        int needitem = 0;
        int itemid = 0;
        int hasitem = 0;
        item = info.consumeitem.Split(':');
        needitem = GameConvert.IntConvert(item[1]);
        hasitem = ItemManager.GetBagItemCount(itemid.ToString());
        if (hasitem < needitem)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err30"));
            return;
        }
        else if (info.consumediamond > PlayerMediator.playerInfo.diamond)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err3"));
            return;
        }
        else if (info.consumeEuro > PlayerMediator.playerInfo.euro)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err14"));
            return;
        }
        else if (info.consumefund > GuildMainMediator.mMyGuild.guildFunds)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err17"));
            return;
        }
        if (id < 9)
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAdviserFriend, mCurAdviser.id, id);
        else
        {
            int ropleID = id - 8;
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAdviserRope, mCurAdviser.id, ropleID);
        }
    }
}