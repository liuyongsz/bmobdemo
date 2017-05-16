using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;

//替补
public class benchpanel : BasePanel
{
    public UILabel ballerName;
    public UILabel fightValue;
    public UITexture ballerIcon;
    public Transform closeBtn;
    public Transform right;
    public Transform left;
    public Transform relateBall;
    public Transform relateprop;

    public UIGrid ballerClipGrid;
}



public class BenchMediator : UIMediator<benchpanel>
{

    public static BenchMediator benchMediator;
  
    private int player_id;
    private int m_max_bench = 10;
    private TeamBaller m_cur_baller;
  
    private int m_index;
    private int m_total_num;
    private List<TeamBaller>ballerList = new List<TeamBaller>();

    private benchpanel panel
    {
        get
        {
            return m_Panel as benchpanel;
        }
    }
    public BenchMediator() : base("benchpanel")
    {
        m_isprop = true;
        setDepth = 7;

        RegistPanelCall(NotificationID.Bench_Show, OpenPanel);
        RegistPanelCall(NotificationID.Bench_Hide, ClosePanel);
        RegistPanelCall(NotificationID.BallerFight_Change, FightChange);

    }

    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.closeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.right.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.left.gameObject).onClick = OnClick;

    }

    /// <summary>
    /// 界面显示
    /// </summary>
    protected override void OnShow(INotification notification)
    {

        if (benchMediator == null)
        {
            benchMediator = Facade.RetrieveMediator("BenchMediator") as BenchMediator;
        }

        ballerList = GetBallerList();

        m_total_num = ballerList.Count-1;

        m_cur_baller = ballerList[0];

        panel.ballerClipGrid.enabled = true;
        panel.ballerClipGrid.BindCustomCallBack(ChooseGrid_UpdateItem);
        panel.ballerClipGrid.StartCustom();

        m_max_bench = BenchConfig.GetBench(1).maxNum;

        SetGridInfo();

        SetBallerInfo();
        SetRelationPropInfo();
    }

    //刷新替补信息
    public  void RefreshBench()
    {
        SetGridInfo();
        SetBallerInfo();
        SetRelationPropInfo();

    }

    private void SetBallerInfo()
    {
        LoadSprite.LoaderPlayerSkin(panel.ballerIcon, m_cur_baller.configId.ToString(), false);
        panel.ballerName.text = TextManager.GetItemString(m_cur_baller.configId.ToString());
        UtilTools.SetTextColor(panel.ballerName,m_cur_baller.star);
        panel.fightValue.text = m_cur_baller.fightValue.ToString();
        SetBallerRelate();

    }
    //设置激活信息
    private void SetRelationPropInfo()
    {
        List<RelationBallerInfo> list = BallerRelateConfig.GetBallerRelateInfo(m_cur_baller.configId);
        UILabel child = null;
        for(int i= 0; i<4; i++)
        {


            child = panel.relateprop.GetChild(i).GetComponent<UILabel>();

            if(i>=list.Count)
            {
                child.gameObject.SetActive(false);
                continue;
            }


            RelationBallerInfo info = list[i];

            string content = TextManager.GetPropsString("relation_content");

            string name = TextManager.GetPropsString(info.relationName);

            string ballerName = "";
            for( int j=0; j<info.ballerList.Count; j++)
            {
                if (j < info.ballerList.Count - 1)
                    ballerName += TextManager.GetItemString(info.ballerList[j].ToString()) + " ,";
                else
                    ballerName += TextManager.GetItemString(info.ballerList[j].ToString());
            }

            string propContent = TextManager.GetUIString(info.propName)+"+"+info.propValue;

            child.text = string.Format(content, name, ballerName, propContent);
            child.color = info.isActive ? Color.green : Color.white;

        }

        panel.relateprop.GetComponent<NGUIGrid>().Reposition();

    }
    //设置球员羁绊
    private void SetBallerRelate()
    {

        List<int> list = GetBallerRelateIdList();
     
        Transform child = null;
        for(int i=0; i<4; i++)
        {
            child = panel.relateBall.GetChild(i);

            if(i<list.Count)
            {
                int ballerId = list[i];
                bool isActive = BallerRelateConfig.IsBallerInteamOrBench(ballerId.ToString());
                UITexture head = child.FindChild("head").GetComponent<UITexture>();
                UISprite color = child.FindChild("color").GetComponent<UISprite>();
                UILabel name = child.FindChild("name").GetComponent<UILabel>();
                
                TD_Player info = Instance.Get<PlayerManager>().GetItem(ballerId);
                LoadSprite.LoaderHead(head, "Head" + ballerId.ToString(), false);
                if (isActive)
                    color.spriteName = "color" + info.initialstar;
                name.text = TextManager.GetItemString(ballerId.ToString());
                UtilTools.SetTextColor(name, info.initialstar);

            }
            else
            {
                child.gameObject.SetActive(false);
            }

        }
        panel.relateBall.GetComponent<NGUIGrid>().Reposition();
    }
    /// <summary>
    /// 设置一信息
    /// </summary>
    public void SetGridInfo()
    {

        int openNum = PlayerMediator.playerInfo.benchSize;
        List<TeamBaller> list = GetBenchBallerList();
       
        List <object> listObj = new List<object>();
        BenchInfo info = null;
        for (int i = 0; i <m_max_bench; i++)
        {
            int index = i + 1;
            info = new BenchInfo();

            info.index = index;
            info.empty = index <= openNum&&i>=list.Count;
            info.isLock = index > openNum;
            if (i < list.Count)
                info.baller = list[i];

            int lastBox = i - 1;
            if (lastBox >= 0)
            {
                BenchInfo lastInfo = listObj[lastBox] as BenchInfo;
                if (lastInfo.baller != null || lastInfo.empty)
                    info.isCanOpen = true;
                else
                    info.isCanOpen = false;
            }

         
            listObj.Add(info);
        }
        
        panel.ballerClipGrid.AddCustomDataList(listObj);
        
    }

    private void ChooseGrid_UpdateItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        BenchInfo info = item.oData as BenchInfo;
        item.onClick = ClickBenchItem;

        UILabel name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture head = item.mScripts[2] as UITexture;
        UILabel num = item.mScripts[3] as UILabel;
        UISprite needMoney = item.mScripts[4] as UISprite;
        UILabel money = item.mScripts[5] as UILabel;

        Transform open = item.transform.FindChild("open");
        Transform close = item.transform.FindChild("close");

        Transform addBg = close.transform.FindChild("addBg");

        open.gameObject.SetActive(info.baller != null);
        close.gameObject.SetActive(info.baller == null);

        addBg.gameObject.SetActive(info.isCanOpen);

        needMoney.gameObject.SetActive(info.isLock);

        if (info.baller != null)
        {
            color.spriteName = "color" + info.baller.star;
            name.text = TextManager.GetItemString(info.baller.configId);

        }
        if (info.isLock)
        {
            BenchBox box = BenchBoxConfig.GetBenchBox(info.index);
            money.text = box.needmoney.ToString();
        }

        if (info.baller != null)
            LoadSprite.LoaderHead(head, "Head" + info.baller.configId.ToString(), false);
        

    }


    private void OnClick(GameObject go)
    {
        switch(go.transform.name)
        {
            case "closeBtn":
                Facade.SendNotification(NotificationID.Bench_Hide);

                break;
            case "left":
                if (m_index >0)
                {
                    m_index--;
                    m_cur_baller = ballerList[m_index];
                    SetBallerInfo();
                    SetRelationPropInfo();
                }
                break;
            case "right":
                if (m_index < m_total_num)
                {
                    m_index++;
                    m_cur_baller = ballerList[m_index];
                    SetBallerInfo();
                    SetRelationPropInfo();
                }
                break;
        }
    }
    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickBenchItem(UIGridItem data)
    {
        BenchInfo info = data.oData as BenchInfo;
        int index = data.GetIndex();
        if(info.empty||info.baller!=null)
        {
            int parem = info.empty ? 0 : info.baller.id;
            Facade.SendNotification(NotificationID.BenchChoose_Show, parem);
            return;
        }
       
        if(info.isLock&&info.isCanOpen)
        {

            BenchBox boxInfo = BenchBoxConfig.GetBenchBox(index + 1);

            if (boxInfo == null)
                return;
            if (boxInfo.needmoney > PlayerMediator.playerInfo.diamond)
            {
                GUIManager.SetPromptInfo("ui_system_40", null);

                return;
            }
            int needMoney = BenchBoxConfig.GetBenchBox(index + 1).needmoney;
            
            ServerCustom.instance.SendClientMethods(FormationProxy.OnClientOpenBench, (index+1));

            return;
        }

    }
    private void FightChange(INotification notification)
    {
        if (GUIManager.HasView("benchpanel"))
        {
            int cardId =GameConvert.IntConvert( notification.Body);

            if(cardId == m_cur_baller.id)
            {
                m_cur_baller = EquipConfig.GetTeamBallerById(m_cur_baller.id);
                panel.fightValue.text = m_cur_baller.fightValue.ToString();

            }
        }
    }

  
    //得到上阵球员
    private List<TeamBaller> GetBallerList()
    {
        List<TeamBaller> list = new List<TeamBaller>();

        var card_enumerator = TeamMediator.teamList.Values.GetEnumerator();
        while (card_enumerator.MoveNext())
        {
            if (card_enumerator.Current.inTeam == 1)
                list.Add(card_enumerator.Current);
        }

        list.Sort(CompareBaller);

        return list;

    }
    //获取替补球员
    private List<TeamBaller> GetBenchBallerList()
    {
        List<TeamBaller> list = new List<TeamBaller>();

        var card_enumerator = TeamMediator.teamList.Values.GetEnumerator();
        while (card_enumerator.MoveNext())
        {
            if (card_enumerator.Current.bench == 1)
                list.Add(card_enumerator.Current);
        }
        return list;
    }

    //获取羁绊球员List
    private List<int> GetBallerRelateIdList()
    {
        List<int> list = new List<int>();
        BallerRelate info = BallerRelateConfig.GetBallerRelate(int.Parse(m_cur_baller.configId));
        string[] arr = info.relate.Split(';');
        
        for (int i = 0; i < arr.Length; i++)
        {
            string[] content = arr[i].Split(',');
            for( int j=0; j<content.Length; j++)
            {
                int id = int.Parse(content[j]);
                if (!list.Contains(id) && id != 0)
                    list.Add(id);
            }
        }

        return list;
    }

    private int CompareBaller(TeamBaller  baller1, TeamBaller baller2)
    {
        if (baller1.isSelf > baller2.isSelf)
            return -1;
        else if (baller1.isSelf < baller2.isSelf)
            return 1;
        else if (baller1.fightValue > baller2.fightValue)
            return -1;
        else if (baller1.fightValue < baller2.fightValue)
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

//替补格子信息
public class BenchInfo
{
    public bool empty; //是否是空
    public TeamBaller baller;//球员信息
    public int index;//id
    public bool isLock;//锁住
    public bool isCanOpen = true;//能否开启
}

public class RelateBallerShow
{

    public int id;
    public bool isActive;
}