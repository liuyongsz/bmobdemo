using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class formationpropactivepanel : BasePanel
{
    public Transform closeBtn;
    public UIGrid chooseGrid;
}

public class FormationPropActiveMediator : UIMediator<formationpropactivepanel>
{

    public static FormationPropActiveMediator formationPropActiveMediator;

    //球员ＩＤ
    private int player_id;

    private List<PropShow> mList = new List<PropShow>();

    private formationpropactivepanel panel
    {
        get
        {
            return m_Panel as formationpropactivepanel;
        }
    }
    public FormationPropActiveMediator() : base("formationpropactivepanel")
    {
        m_isprop = true;
        setDepth = 8;
        RegistPanelCall(NotificationID.FormationSysActive_Show, OpenPanel);
        RegistPanelCall(NotificationID.FormationSysActive_Hide, ClosePanel);

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
        if (formationPropActiveMediator == null)
        {
            formationPropActiveMediator = Facade.RetrieveMediator("FormationPropActiveMediator") as FormationPropActiveMediator;
        }
      

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
        mList = GePropShowList();

        List<object> listObj = new List<object>();
        BenchInfo info = null;
        for (int i = 0; i < mList.Count; i++)
        {
            listObj.Add(mList[i]);
        }

        panel.chooseGrid.AddCustomDataList(listObj);

    }

    private void ChooseGrid_UpdateItem(UIGridItem item)
    {

        if (item == null || item.mScripts == null || item.oData == null)
            return;

        PropShow info = item.oData as PropShow;
        item.onClick = ClickbenchItem;

        UILabel name = item.mScripts[0] as UILabel;
        UILabel prop = item.mScripts[1] as UILabel;
        UILabel value = item.mScripts[3] as UILabel;

        name.text = TextManager.GetPropsString("formation_sys_"+info.id);

        string propName = TextManager.GetUIString(info.propName);
        prop.text = propName + "+" + info.curValue;

        string content = TextManager.GetUIString("UI2086");
        if (info.nextValue == 0)
            value.text = TextManager.GetUIString("UI2088");
        else
            value.text = string.Format(content, info.nextLevel, propName, info.nextValue);



    }
    
    private void OnClick(GameObject go)
    {
        Facade.SendNotification(NotificationID.FormationSysActive_Hide);
    }
    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    private void ClickbenchItem(UIGridItem data)
    {
    
    }
    private void  benchRefresh(INotification notification)
    {
        if (GUIManager.HasView("formationpropactivepanel"))
        {
            SetInfo();
        }
    }

   private  List<PropShow> GePropShowList()
    {
        List<PropShow> list = new List<PropShow>();

        PropShow propInfo = null;
        var enumerator = TeamFormationSystemConfig.m_data.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            propInfo = new PropShow();
            TeamFormationSystem sysInfo = enumerator.Current;

            FormationSysInfo info = TeamFormationSystemConfig.GetFormationSys(sysInfo.id);

            propInfo.id = sysInfo.id;
            propInfo.propName = sysInfo.valueType;

            int strongLevel = info.strongLevel;

            string[] conditionArr = sysInfo.activeCondition.Split(',');
            string[] propArr = sysInfo.addProp.Split(',');

            for( int i=0; i<conditionArr.Length; i++)
            {
                int index = 0;
                int needLevel = GameConvert.IntConvert(conditionArr[i]);

                if (strongLevel < needLevel)
                {
                    index = i - 1;
                    propInfo.curValue = index < 0 ? 0 : GameConvert.IntConvert(propArr[index]);

                    int nextIndex = index == (propArr.Length - 1) ?-1 : (index + 1);

                    propInfo.nextLevel = nextIndex < 0 ? 0 : GameConvert.IntConvert(conditionArr[nextIndex]);

                    propInfo.nextValue = nextIndex<0?0 : GameConvert.IntConvert(propArr[nextIndex]);

                    break;
                }

            }
            list.Add(propInfo);
            

        }
        return list;

    }
  
   
    /// <summary>
    /// 界面关闭
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

}


public class PropShow
{

    public int id;

    public string propName;

    public int  curValue;
    public int nextLevel;
    public int  nextValue;
}