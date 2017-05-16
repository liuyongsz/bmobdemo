using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class benchchoosepanel : BasePanel
{
    public Transform closeBtn;
    public UIGrid chooseGrid;
}

public class BenchChooseMediator : UIMediator<benchchoosepanel>
{

    public static BenchChooseMediator benchChooseMediator;

    //球员ＩＤ
    private int player_id;

    private List<TeamBaller> mBallerList = new List<TeamBaller>();

    private benchchoosepanel panel
    {
        get
        {
            return m_Panel as benchchoosepanel;
        }
    }
    public BenchChooseMediator() : base("benchchoosepanel")
    {
        m_isprop = true;
        setDepth = 9;

        RegistPanelCall(NotificationID.BenchChoose_Show, OpenPanel);
        RegistPanelCall(NotificationID.BenchChoose_Hide, ClosePanel);

    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;
        
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {
        if (benchChooseMediator == null)
        {
            benchChooseMediator = Facade.RetrieveMediator("BenchChooseMediator") as BenchChooseMediator;
        }

        player_id = GameConvert.IntConvert(notification.Body);

        panel.chooseGrid.enabled = true;
        panel.chooseGrid.BindCustomCallBack(ChooseGrid_UpdateItem);
        panel.chooseGrid.StartCustom();

        SetInfo();
    }
    /// <summary>
    /// 设置一信息
    /// </summary>
    public void SetInfo()
    {
        if (null == m_Panel) return;
        mBallerList = GetCanBenchBallerList();

        List<object> listObj = new List<object>();
        BenchInfo info = null;
        for (int i = 0; i < mBallerList.Count; i++)
        {
            listObj.Add(mBallerList[i]);
        }

        panel.chooseGrid.AddCustomDataList(listObj);

    }

    private void ChooseGrid_UpdateItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        TeamBaller info = item.oData as TeamBaller;
        item.onClick = ClickbenchItem;

        UILabel name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture icon = item.mScripts[2] as UITexture;

        name.text = TextManager.GetItemString(info.configId);

        color.spriteName = "color" + info.star;

        UtilTools.SetTextColor(name, info.star);
        
        LoadSprite.LoaderHead(icon, "Card"+info.configId, false);
        
    }
    
    private void OnClick(GameObject go)
    {
        Facade.SendNotification(NotificationID.BenchChoose_Hide);
    }
    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickbenchItem(UIGridItem data)
    {
        TeamBaller info = data.oData as TeamBaller;

        if (player_id <= 0)
            ServerCustom.instance.SendClientMethods(FormationProxy.OnClientBenchBaller, 1, info.id);
        else
            ServerCustom.instance.SendClientMethods(FormationProxy.OnClientExchangeBench, player_id, info.id);


        Facade.SendNotification(NotificationID.BenchChoose_Hide);

    }
    private void  benchRefresh(INotification notification)
    {
        if (GUIManager.HasView("benchchoosepanel"))
        {
            SetInfo();
        }
    }

   private  List<TeamBaller> GetCanBenchBallerList()
    {
        List<TeamBaller> list = new List<TeamBaller>();

        var card_enumerator = TeamMediator.teamList.Values.GetEnumerator();
        while (card_enumerator.MoveNext())
        {
            if (card_enumerator.Current.inTeam == 0 && card_enumerator.Current.bench == 0)
                list.Add(card_enumerator.Current);
        }
        list.Sort(CompareItem);
        return list;

    }
    /// <summary>
    /// 排序
    /// </summary>
    public int CompareItem(TeamBaller baller1, TeamBaller baller2)
    {

        if (baller1.star > baller2.star)
            return -1;
        else if (baller1.star < baller2.star)
            return 1;
        else
            return 0;

     
    }
   
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

}