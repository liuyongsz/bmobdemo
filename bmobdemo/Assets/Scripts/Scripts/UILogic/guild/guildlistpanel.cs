using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using System;
using System.Collections.Generic;

public class guildlistpanel:BasePanel
{
    public UIGrid guildGrid;
    public UIButton off_ben;
    public UIButton left_btn;
    public UIButton right_btn;
    public Transform creat;
    public UIButton search_btn;
    public UIButton find_btn;
    public Transform find;                //重新查找、返回、关键字
    public Transform findguildinfo;       //查找公会界面
    public UIButton newsearch_btn;
    public UIButton return_btn;
    public UIButton offinfo_btn;
    public UIButton sure_btn;
    public UILabel page_label;
    public UIInput inputSearch;
    public UILabel searchContent;

}

public class GuildListMediator: UIMediator<guildlistpanel>
{
    private guildlistpanel panel
    {
        get
        {
            return m_Panel as guildlistpanel;
        }
    }
    bool isfind=false;

    public UIGridItem mCurItem;
    public GuildBase mGuildBaseInfo;
    private int mPageCount;
    private int mCurPage = 1;
    private int mOnePageNum = 6;
    private List<GuildInfo> mCurlist = new List<GuildInfo>();
    public static GuildListMediator guildlistMediator;

    public GuildListMediator() : base("guildlistpanel")
    {
        m_isprop = true;
        RegistPanelCall(NotificationID.GuildList_Show, OpenPanel);
        RegistPanelCall(NotificationID.GuildList_Hide, ClosePanel);
    }

    protected override void OnShow(INotification notification)
    {
        if (guildlistMediator == null)
        {
            guildlistMediator = Facade.RetrieveMediator("GuildListMediator") as GuildListMediator;
        }

        mGuildBaseInfo = GuildBaseConfig.GetGuildBase(1);
        panel.guildGrid.enabled = true;
        panel.guildGrid.BindCustomCallBack(UpdateGuildGrid);
        panel.guildGrid.StartCustom();
        
        OnClientGuildList();
    }

    //设置公会数据
    public void SetData()
    {
        Facade.SendNotification(NotificationID.Gold_Hide);
        if(!GUIManager.HasView("guildlistpanel"))
        {
            return; 
        }
        mCurlist = GuildBaseConfig.mGuildList;
        mCurlist.Sort(CompareGuild);

        SetGuildRank();
        SetGuildListGrid();
    }
    private void SetGuildRank()
    {
        for(int i=0;i<mCurlist.Count;i++)
        {
            GuildInfo info = mCurlist[i];
            info.rank = (i + 1);
        }
    }
    //设置搜索列表
    public void SetSearchList(string search)
    {
        mCurlist = GetSearchGuildList(search);
        mCurlist.Sort(CompareGuild);

        mCurPage = 1;
        SetGuildListGrid();
    }
    //刷新
    public void RefreshApplyItem()
    {
        if (GUIManager.HasView("guildlistpanel"))
            SetGuildListGrid();
    }
    //设置列表
    private void SetGuildListGrid()
    {

        float pageNum = (float)mCurlist.Count / (float)mOnePageNum;
        mPageCount = Mathf.CeilToInt(pageNum);
        
        int maxNum = mCurlist.Count;
        int begin = (mCurPage - 1) * mOnePageNum;
        int end = mCurPage * mOnePageNum> maxNum? maxNum : mCurPage * mOnePageNum;
        List<object> listObj = new List<object>();
        int i = 0;
        for (i = begin; i < end; i++)
        {
            listObj.Add(mCurlist[i]);
        }
        panel.guildGrid.AddCustomDataList(listObj);

        if (mPageCount == 0)
            panel.page_label.text = "0/0";
        else
            panel.page_label.text = string.Format("{0}/{1}", mCurPage, mPageCount);

    }
    private void UpdateGuildGrid(UIGridItem item)
    {
        
        GuildInfo info = item.oData as GuildInfo;
        item.onClick = OnClickItem;
        UILabel rank_label = item.mScripts[0] as UILabel;
        UITexture guildhead_texture = item.mScripts[1] as UITexture;
        UILabel guildname_label = item.mScripts[2] as UILabel;
        UILabel camp_Label = item.mScripts[3] as UILabel;
        UILabel level_label = item.mScripts[4] as UILabel;
        UILabel num_label = item.mScripts[5] as UILabel;
        UITexture leader_texture = item.mScripts[6] as UITexture;
        UILabel leadername_label = item.mScripts[7] as UILabel;
        UISprite join_btn = item.mScripts[8] as UISprite;
        UISprite cancel_btn = item.mScripts[9] as UISprite;
        guildname_label.text = info.guildName;
        camp_Label.text = info.camp.ToString();
        level_label.text = info.level.ToString();
        num_label.text =string.Format("{0}/{1}",info.count, mGuildBaseInfo.maxMemberNum);
        leadername_label.text = info.leader;
        rank_label.text = info.rank.ToString();
        bool isjoin = GuildBaseConfig.mApplyGuildIdList.Contains(info.id);

        join_btn.transform.gameObject.SetActive(!isjoin);
        cancel_btn.transform.gameObject.SetActive(isjoin);

        UIEventListener.Get(join_btn.transform.gameObject).onClick = OnClickJoin;
        UIEventListener.Get(cancel_btn.transform.gameObject).onClick = OnClickJoin;
        
    }

    private void OnClickItem(UIGridItem item)
    {
        GuildInfo info = item.oData as GuildInfo;
        Facade.SendNotification(NotificationID.GuildInfo_Show , info);
    }
    private void OnClickJoin(GameObject go)
    {
        Transform parent = go.transform.parent;

        UIGridItem item = parent.GetComponent<UIGridItem>();
        if (item == null)
            return;
        mCurItem = item;

        GuildInfo info = item.oData as GuildInfo;

        if (go.transform.name == "join_btn")
        {
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientApplyJoinGuild,info.id);
        }
        else if (go.transform.name == "cancel_btn")
        {
            ServerCustom.instance.SendClientMethods(GuildProxy.OnClientCancelApply, info.id);
        }

    }


    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.off_ben.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.left_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.right_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.search_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.find_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.newsearch_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.return_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.offinfo_btn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.sure_btn.gameObject).onClick = OnClick;

    }

    private void OnClick(GameObject go)
    {
      
        if (go == panel.off_ben.gameObject)
        {
            Facade.SendNotification(NotificationID.GuildList_Hide);
        }
        else if (go == panel.left_btn.gameObject)
        {
            if (mCurPage>1)
            {
                mCurPage--;
                SetGuildListGrid();
            }
            
        }
        else if (go == panel.right_btn.gameObject)
        {
            if (mCurPage < mPageCount)
            {
                mCurPage++;
                SetGuildListGrid();
            }
        
        }
        else if (go == panel.search_btn.gameObject)//创建按钮
        {
            Facade.SendNotification(NotificationID.GuildCreat_Show);
        }
        else if (go == panel.find_btn.gameObject)//查找按钮
        {
            isfind = true;
            panel.inputSearch.value = "";
            panel.searchContent.text = "";
           Findpanel(isfind);
        }
        else if (go == panel.newsearch_btn.gameObject)//重新查找按钮
        {
            panel.inputSearch.value = "";

            panel.findguildinfo.gameObject.SetActive(true);
        }
        else if (go == panel.return_btn.gameObject)
        {
            isfind = false;
            Findpanel(isfind);
            SetData();

        }
        else if (go == panel.offinfo_btn.gameObject)//关闭查找公会界面
        {
            panel.findguildinfo.gameObject.SetActive(false);
            panel.searchContent.text = "";
        }
        else if(go == panel.sure_btn.gameObject) //确定
        {
            string searchStr = panel.inputSearch.value.Trim(' ');
            panel.searchContent.text = searchStr;
            panel.findguildinfo.gameObject.SetActive(false);
            SetSearchList(searchStr);
        }
    
    }
    void Findpanel(bool isopen)
    {
        panel.find.gameObject.SetActive(isopen);
        panel.findguildinfo.gameObject.SetActive(isopen);
        panel.creat.gameObject.SetActive(!isopen);
    }

    //获取搜索公会
    private List<GuildInfo> GetSearchGuildList(string search)
    {
        List<GuildInfo> list = new List<GuildInfo>();
        for( int i=0; i < GuildBaseConfig.mGuildList.Count; i++)
        {
            GuildInfo info = GuildBaseConfig.mGuildList[i];
            if (info.guildName.Contains(search))
                list.Add(info);
        }

        return list;
    }

    //公会排序
    public int CompareGuild(GuildInfo guild1,GuildInfo guild2)
    {
        if (guild1.level > guild2.level)
            return -1;
        else if (guild1.level < guild2.level)
            return 1;
        else if (guild1.count > guild2.count)
            return -1;
        else if (guild1.count < guild2.count)
            return 1;
        else
            return 0;
    }
    //客户端请求公会列表
    private void OnClientGuildList()
    {
        ServerCustom.instance.SendClientMethods(GuildProxy.OnClientGuildList);

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        mCurPage = 1;
        isfind = false;
        Facade.SendNotification(NotificationID.Gold_Show);
    }
}
