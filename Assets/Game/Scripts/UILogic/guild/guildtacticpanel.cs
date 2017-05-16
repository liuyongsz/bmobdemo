using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System.Collections.Generic;
using System;

public class guildtacticpanel : BasePanel
{
    public Transform offBtn;
    public Transform immuneBtn;
    public Transform incidentBtn;
    public Transform RankBtn;
    public UILabel time;
    public UIGrid guildGrid;
    //------------------------------
    public Transform immunepanel;    //购买公会保护界面
    public Transform off_btn;
    public Transform sub_btn;
    public UILabel count_label;     
    public Transform add_btn;
    public UILabel money_label;
    public Transform sure_btn;
    //------------------------------
    public Transform appealpanel;     //上诉界面
    public Transform offappeal_btn;
    public UILabel appealconsume1;
    public UILabel appealresult1;
    public UILabel appealaward1;
    public UILabel appealsuccess1;
    public Transform appealBtn1;
    public UILabel appealconsume2;
    public UILabel appealresult2;
    public UILabel appealaward2;
    public UILabel appealsuccess2;
    public Transform appealBtn2;
    //-----------------------------
    public Transform exposurepanel;   //曝光界面
    public Transform offexposure_btn;
    public UILabel exposureconsume1;
    public UILabel exposureresult1;
    public UILabel exposureaward1;
    public UILabel exposuresuccess1;
    public UIButton executeBtn1;
    public UILabel exposureconsume2;
    public UILabel exposureresult2;
    public UILabel exposureaward2;
    public UILabel exposuresuccess2;
    public Transform executeBtn2;


}
public class GuildTacticMediator : UIMediator<guildtacticpanel>
{
    private guildtacticpanel panel
    {
        get
        {
            return m_Panel as guildtacticpanel;
        }
    }
    public static GuildTacticMediator guildtacticMediator;
    private   int count = 1;
    private int mMaxCount = 6;
    private GuildInfo mSelctGuild; //当前选择的公会
    private int id;
    public GuildTacticMediator() : base("guildtacticpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildTactic_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildTactic_Hide, ClosePanel);
    }
    protected override void OnShow(INotification notification)
    {
        if (guildtacticMediator == null)
        {
            guildtacticMediator = Facade.RetrieveMediator("GuildTacticMediator") as GuildTacticMediator;
        }
        panel.immunepanel.gameObject.SetActive(false);
        panel.appealpanel.gameObject.SetActive(false);
        panel.exposurepanel.gameObject.SetActive(false);
        SetCutOffTime();
        ServerCustom.instance.SendClientMethods("onClientGetGuildValueRank", 0);
        panel.guildGrid.enabled = true;
        panel.guildGrid.BindCustomCallBack(UpdateGuildGrid);
        panel.guildGrid.StartCustom();

        InitAppealExposureInfo();
    }

    public void SetGuildListGrid()
    {
        List<object> list = new List<object>();
        foreach (GuildInfo guildinfo in RankMediator.guildRankList.Values)
        {
            if (list.Count< mMaxCount)
            {
                list.Add(guildinfo);
            }
         
        }
        panel.guildGrid.AddCustomDataList(list);
    }
    private void UpdateGuildGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        GuildInfo info = item.oData as GuildInfo;
        UILabel rank = item.mScripts[0] as UILabel;
        UITexture emblem = item.mScripts[1] as UITexture;
        UILabel guildname = item.mScripts[2] as UILabel;
        UILabel leader = item.mScripts[3] as UILabel;       
        UILabel reputation = item.mScripts[4] as UILabel;
        UILabel fund = item.mScripts[5] as UILabel;
        UISprite appealBtn = item.mScripts[6] as UISprite;
        UISprite exposureBtn = item.mScripts[7] as UISprite;
        rank.text = info.ranking.ToString();
        guildname.text = info.guildName;
        leader.text = info.leader;
        reputation.text = info.reputation.ToString();
        fund.text = info.guildFunds.ToString();

        bool isSelf = info.id == PlayerMediator.playerInfo.guildDBID;
        appealBtn.transform.GetComponent<UIButton>().isEnabled = !isSelf;
        exposureBtn.transform.GetComponent<UIButton>().isEnabled = !isSelf;
        UIEventListener.Get(appealBtn.transform.gameObject).onClick = OnClickItem;
        UIEventListener.Get(exposureBtn.transform.gameObject).onClick = OnClickItem;
    }
    private void OnClickItem(GameObject go)
    {
        Transform parent = go.transform.parent;

        UIGridItem item = parent.GetComponent<UIGridItem>();
        if (item == null)
            return;

        mSelctGuild = item.oData as GuildInfo;

        if (go.transform.name == "appealBtn")
        {
            panel.appealpanel.gameObject.SetActive(true);
            SetAppealInfo();
        }
        else if (go.transform.name == "exposureBtn")
        {
            panel.exposurepanel.gameObject.SetActive(true);
            SetExposureInfo();
        }

    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.offBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.immuneBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.incidentBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.RankBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.off_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sub_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.add_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sure_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.appealBtn1.gameObject).onClick = OnClickExecute;
        UIEventListener.Get(panel.appealBtn2.gameObject).onClick = OnClickExecute;
        UIEventListener.Get(panel.offappeal_btn.gameObject).onClick = OnClickExecute;
        UIEventListener.Get(panel.executeBtn1.gameObject).onClick = OnClickExecute;
        UIEventListener.Get(panel.executeBtn2.gameObject).onClick = OnClickExecute;
        UIEventListener.Get(panel.offexposure_btn.gameObject).onClick = OnClickExecute;

    }



    private void OnClick(GameObject go)
    {
        switch (go.transform.name)
        {
            case "offBtn":
                {
                    Facade.SendNotification(NotificationID.GuildTactic_Hide);
                }
                break;
            case "immuneBtn":      //免疫攻击界面
                {
                    panel.immunepanel.gameObject.SetActive(true);
                }
                break;
            case "incidentBtn":      
                {
                    //事件
                }
                break;
            case "RankBtn":
                {
                    Facade.SendNotification(NotificationID.Rank_Show);
                }
                break;
            case "off_btn":
                {
                    panel.immunepanel.gameObject.SetActive(false);
                }
                break;
            case "sub_btn":
                {
                    if (count == 1)
                        return;
                    count--;
                    SetProtectInfo();
                }
                break;
            case "add_btn":
                {
                    if (count == 999)
                        return;
                    count++;
                    SetProtectInfo();
                }
                break;
            case "sure_btn":
                {
                    int needDiamond = count * GuildBaseConfig.GetGuildBase(1).protectconsume;
                    if (PlayerMediator.playerInfo.diamond < needDiamond)
                    {
                        GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_40"));
                        return;
                    }
                    ServerCustom.instance.SendClientMethods(GuildProxy.OnClientBuyGuildProtect, count);
                    GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild22"));
                    panel.immunepanel.gameObject.SetActive(true);
                }
                break;
            default:
                return;
        }
    }
    private void OnClickExecute(GameObject go)
    {
        if (go==panel.appealBtn1.gameObject)
        {
            id = 1;
            SureDissolve();

        }
        else if (go == panel.appealBtn2.gameObject)
        {
            id = 2;
            SureDissolve();

        }
        else if (go == panel.executeBtn1.gameObject)
        {
            //曝光买通裁判
            id = 3;
            SureDissolve();

        }
        else if (go == panel.executeBtn2.gameObject)
        {
            //曝光踢假球
            id = 4;
            SureDissolve();
        }
        else if (go == panel.offappeal_btn.gameObject)
        {
            panel.appealpanel.gameObject.SetActive(false);
        }
        else if (go == panel.offexposure_btn.gameObject)
        {
            panel.exposurepanel.gameObject.SetActive(false);
        }
    }

    //客户端请求攻击公会
    private void ClinetRequipAppleAndExp()
    {
        GuildAppeal info =  GuildAppealConfig.GetGuildAppeal(id);

        int itemID = GameConvert.IntConvert(info.consume.Split(':')[0]);
        int needNum = GameConvert.IntConvert(info.consume.Split(':')[1]);
        int hasNum = ItemManager.GetBagItemCount(itemID.ToString());

        if(needNum >hasNum)
        {
            GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_guild_err30"));
            return;
        }

        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientAppealExposure,  mSelctGuild.id,id);
        panel.exposurepanel.gameObject.SetActive(false);
        panel.appealpanel.gameObject.SetActive(false);

    }

    public void SureDissolve()
    {
        string title1 = TextManager.GetUIString("UICreate1");
        string content1 = TextManager.GetUIString("UI_GuildTactic20");
        GUIManager.SetPromptInfoChoose(title1, content1, ClinetRequipAppleAndExp);
    }
    public void SetProtectInfo()
    {
        panel.count_label.text = count.ToString();
        panel.money_label.text = (count * GuildBaseConfig.GetGuildBase(1).protectconsume).ToString();
    }
    //设置公会保护时间
    public void SetCutOffTime()
    {
        string  timeStr = "";
        int time = 0;
        if (GuildMainMediator.mMyGuild.protectTime > 0)
        {
            time = (int)GuildMainMediator.mMyGuild.protectTime / 3600;
            if (time == 0)
            {
                timeStr = UtilTools.formatDuring(GuildMainMediator.mMyGuild.protectTime);
                panel.time.color = Color.green;
                panel.time.text = timeStr;
            }
            else
            {
                timeStr = time.ToString();
                panel.time.color = Color.green;
                panel.time.text = timeStr + TextManager.GetUIString("UI_GuildBuild18");
            }
        }

        else if (GuildMainMediator.mMyGuild.protectTime == 0)
        {
            panel.time.color = Color.red;
            panel.time.text = TextManager.GetUIString("UI_GuildTactic19");
        }
        count = 1;
        SetProtectInfo();
    }

    private void InitAppealExposureInfo()
    {
        GuildAppeal appealinfo1 = GuildAppealConfig.GetGuildAppeal(1);
        panel.appealresult1.text = appealinfo1.reduceFund.ToString();
        panel.appealaward1.text = appealinfo1.Addcontribution.ToString();
        panel.appealsuccess1.text = (appealinfo1.successrate * 100).ToString() + "%";

        GuildAppeal appealinfo2 = GuildAppealConfig.GetGuildAppeal(2);       
        panel.appealresult2.text = appealinfo2.reduceFund.ToString();
        panel.appealaward2.text = appealinfo2.Addcontribution.ToString();
        panel.appealsuccess2.text = (appealinfo2.successrate * 100).ToString() + "%";


        GuildAppeal exposureinfo1 = GuildAppealConfig.GetGuildAppeal(3);
        panel.exposureconsume1.text = exposureinfo1.consume.ToString();
        panel.exposureresult1.text = exposureinfo1.reducereputation.ToString();
        panel.exposureaward1.text = exposureinfo1.Addcontribution.ToString();
        panel.exposuresuccess1.text = (exposureinfo1.successrate * 100).ToString() + "%";


        GuildAppeal exposureinfo2 = GuildAppealConfig.GetGuildAppeal(4);
        panel.exposureconsume2.text = exposureinfo2.consume.ToString();
        panel.exposureresult2.text = exposureinfo2.reducereputation.ToString();
        panel.exposureaward2.text = exposureinfo2.Addcontribution.ToString();
        panel.exposuresuccess2.text = (exposureinfo2.successrate * 100).ToString() + "%";

    }
    //上诉界面
    public void SetAppealInfo()
    {
        string[] ItemDateArr = null;
        int itemID = 0;
        int needNum = 0;
        int hasNum = 0;

        GuildAppeal appealinfo1 = GuildAppealConfig.GetGuildAppeal(1);
        ItemDateArr = appealinfo1.consume.Split(':');
        itemID = GameConvert.IntConvert(ItemDateArr[0]);
        needNum = GameConvert.IntConvert(ItemDateArr[1]);
        hasNum = ItemManager.GetBagItemCount(itemID.ToString());
        panel.appealconsume1.text = hasNum + "/" + needNum;


        GuildAppeal appealinfo2 = GuildAppealConfig.GetGuildAppeal(2);
        ItemDateArr = appealinfo2.consume.Split(':');
        itemID = GameConvert.IntConvert(ItemDateArr[0]);
        needNum = GameConvert.IntConvert(ItemDateArr[1]);
        hasNum = ItemManager.GetBagItemCount(itemID.ToString());
        panel.appealconsume2.text = hasNum + "/" + needNum;

     

    }
    //曝光界面
    public void SetExposureInfo()
    {

        string[] ItemDateArr = null;
        int itemID = 0;
        int needNum = 0;
        int hasNum = 0;

        GuildAppeal exposureinfo1 = GuildAppealConfig.GetGuildAppeal(3);
        ItemDateArr = exposureinfo1.consume.Split(':');
        itemID = GameConvert.IntConvert(ItemDateArr[0]);
        needNum = GameConvert.IntConvert(ItemDateArr[1]);
        hasNum = ItemManager.GetBagItemCount(ItemDateArr.ToString());
        panel.exposureconsume1.text = hasNum + "/" + needNum;

        GuildAppeal exposureinfo2 = GuildAppealConfig.GetGuildAppeal(4);
        ItemDateArr = exposureinfo2.consume.Split(':');
        itemID = GameConvert.IntConvert(ItemDateArr[0]);
        needNum = GameConvert.IntConvert(ItemDateArr[1]);
        hasNum = ItemManager.GetBagItemCount(ItemDateArr.ToString());
        panel.exposureconsume2.text = hasNum + "/" + needNum;
    }
}