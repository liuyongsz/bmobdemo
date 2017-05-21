using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;
public class MailInfo
{
    public int type;    //邮件类型
    public string title;//邮件标题
    public string name; //邮件发件人
    public string text; //邮件内容
    public int time;    //邮件发送时间
    public string attachment; //邮件附近
    public int state;   //邮件状态
    public string extern_info;
}
public class emailpanel : BasePanel
{    
    public Transform closeReadbtn;
    public Transform sendMail;
    public Transform readMail;
    public UIGrid emailGrid;
    public UISprite sendMailBtn;
    public UISprite closeSendMailBtn;
    public UISprite backBtn;
    public UISprite writebtn;
    public UISprite getallreward;
    public UISprite onekeydeltn;
    public UISprite chooseFriend;
    public UIToggle systembtn;
    public UIToggle privatebtn;
    public UIToggle guildebtn;
    public UILabel systemText;
    public UILabel privateText;
    public UILabel guildeText;  
    public UILabel contentText;
    public UILabel peopleText;
    public UILabel zhutiText;
    public UILabel readcontent;
    public UILabel sendPeopletext;
    public UILabel timetext;
    public UILabel num;
    public UIInput peopleInputField;
    public UIInput contentInputField;
    public UIInput zhutiInputField;
    public UILabel readMailTitle;
}

public class EmailMediator : UIMediator<emailpanel>
{
   
    public static int SelectMailType = 0;
  
    public enum Click_Type
    {
        SEND_MAIL,
        READ_MAIL,
    }
    private MailInfo chooseMailInfo;
    public static EmailMediator emailMediator;
    public static List<MailInfo> mailInfo = new List<MailInfo>();

    public static List<MailInfo> currentMailList = new List<MailInfo>();


    public EmailMediator() : base("emailpanel")
    {
        m_isprop = false;
        m_ismain = false;      
        RegistPanelCall(NotificationID.Mail_Show, base.OpenPanel);
        RegistPanelCall(NotificationID.Mail_Hide, base.ClosePanel);
    }

    /// <summary>
    /// 绑定点击事件
    /// </summary>
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(m_Panel.backBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.systembtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.privatebtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.guildebtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.getallreward.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.onekeydeltn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.writebtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.sendMail.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.closeReadbtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.sendMailBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.closeSendMailBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(m_Panel.chooseFriend.gameObject).onClick = OnClick;
    }
    protected override void OnShow(INotification notification)
    {
        ServerCustom.instance.SendClientMethods("onClientGetFriendList");
        m_Panel.emailGrid.enabled = true;
        m_Panel.emailGrid.BindCustomCallBack(UpdateMailGrid);
        m_Panel.emailGrid.StartCustom();
        emailMediator = Facade.RetrieveMediator("EmailMediator") as EmailMediator;
        AddMailGridShow(0);

    }

    void UpdateMailGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        MailInfo info = item.oData as MailInfo;
        UILabel label = item.mScripts[2] as UILabel;
        UILabel time = item.mScripts[3] as UILabel;
        UISprite seeBtn = item.mScripts[4] as UISprite;
        UILabel content = item.mScripts[5] as UILabel;
        UILabel people = item.mScripts[6] as UILabel;
        UISprite haveRead = item.mScripts[7] as UISprite;
        UIEventListener.Get(seeBtn.gameObject).onClick = OnItemClick;
        if (info.state == 2)
        {
            haveRead.spriteName = "youjiandiban";
            seeBtn.spriteName = "anniu1";
            seeBtn.GetComponent<BoxCollider>().enabled = true;
        }
        else 
        {
            haveRead.spriteName = "yiyuedudiban";
            seeBtn.spriteName = "wufadianjianniu";
            seeBtn.GetComponent<BoxCollider>().enabled = false;
        }
        label.text = info.title.ToString();
        content.text = info.text.ToString();
        people.text = info.name;
        TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        int cstName = (int)timeSpan.TotalSeconds;
        time.text = UtilTools.formatDuring(cstName - info.time) + TextManager.GetUIString("UIEmail003");
    }
    /// <summary>
    /// 点击邮件Item
    /// </summary>
    void OnItemClick(GameObject obj)
    {
        chooseMailInfo = obj.GetComponentInParent<UIGridItem>().oData as MailInfo;
        MailProxy.Instance.ReadMail(chooseMailInfo);
        chooseMailInfo.state = 0;
        m_Panel.emailGrid.UpdateCustomData(chooseMailInfo);
        ClickComBtn(Click_Type.READ_MAIL);
    }
    /// <summary>
    /// 初始化邮件grid
    /// </summary>
    public void AddMailGridShow(int type)
    {
        if (currentMailList.Count > 0)
        {
            currentMailList.Clear();
        }
        for (int i = 0; i < mailInfo.Count; ++i)
        {
            if (mailInfo[i].type == type)
            {
                currentMailList.Add(mailInfo[i]);
            }
        }
        m_Panel.num.text = mailInfo.Count.ToString();
        m_Panel.emailGrid.AddCustomDataList(AddListGrid(currentMailList));   
    }

    List<object> AddListGrid(List<MailInfo> list)
    {
        List<object> listObj = new List<object>();
        for (int i = 0; i < list.Count; ++i)
        {
            listObj.Add(list[i]);
        }
        return listObj;
    }
    /// <summary>
    /// 二级界面的事件类型
    /// </summary>
    public void ClickComBtn(Click_Type type)
    {
        if (Click_Type.SEND_MAIL == type)
        {
            if (m_Panel.peopleInputField.value == string.Empty)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UIEmail004"), null);
                return;
            }
            if (m_Panel.zhutiInputField.value == string.Empty)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UIEmail005"), null);
                return;
            }
            MailProxy.Instance.SendMail(1, 1, m_Panel.zhutiText.text, m_Panel.contentText.text);
            m_Panel.sendMail.gameObject.SetActive(false);          
        }
        else if (Click_Type.READ_MAIL == type)
        {
            m_Panel.readMail.gameObject.SetActive(true);
            m_Panel.readMailTitle.text = chooseMailInfo.title;
            m_Panel.readcontent.text = chooseMailInfo.text;
            m_Panel.sendPeopletext.text = chooseMailInfo.name;
            m_Panel.timetext.text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
    void CloseComBtn(Click_Type type)
    {
        if (Click_Type.SEND_MAIL == type)
        {           
            m_Panel.sendMail.gameObject.SetActive(false);
        }
        else if (Click_Type.READ_MAIL == type)
        {
            m_Panel.readMail.gameObject.SetActive(false);
            m_Panel.readcontent.text = string.Empty;
        }
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    void OnClick(GameObject go)
    {
        if (go == m_Panel.backBtn.gameObject)
        {
            ClosePanel(null);
        }
        else if (go == m_Panel.systembtn.gameObject)
        {
            m_Panel.systemText.color = Color.black;
            m_Panel.privateText.color = Color.white;
            m_Panel.guildeText.color = Color.white;
            SelectMailType = 0;
            AddMailGridShow(SelectMailType);
        }
        else if (go == m_Panel.privatebtn.gameObject)
        {
            m_Panel.systemText.color = Color.white;
            m_Panel.privateText.color = Color.black;
            m_Panel.guildeText.color = Color.white;
            SelectMailType = 1;
            AddMailGridShow(SelectMailType);
        }
        else if (go == m_Panel.guildebtn.gameObject)
        {
            m_Panel.systemText.color = Color.white;
            m_Panel.privateText.color = Color.white;
            m_Panel.guildeText.color = Color.black;
            SelectMailType = 3;
            AddMailGridShow(SelectMailType);
        }
        else if (go == m_Panel.getallreward.gameObject)
        {
            List<string> reward = new List<string>();
            for (int i = 0; i < currentMailList.Count; ++i)
            {
                reward.Add(currentMailList[i].attachment);
            }
            if (currentMailList.Count <= 0 || reward.Count <= 0)
            {

            }
        }
        else if (go == m_Panel.onekeydeltn.gameObject)
        {
            MailProxy.Instance.DelAllMailByType(SelectMailType);
        }
        else if (go == m_Panel.sendMailBtn.gameObject)
        {
            ClickComBtn(Click_Type.SEND_MAIL);
        }
        else if (go == m_Panel.closeSendMailBtn.gameObject)
        {
            CloseComBtn(Click_Type.SEND_MAIL);
        }
        else if (go == m_Panel.closeReadbtn.gameObject)
        {
            CloseComBtn(Click_Type.READ_MAIL);
        }
        else if (go == m_Panel.writebtn.gameObject)
        {
            m_Panel.sendMail.gameObject.SetActive(true);
            m_Panel.contentInputField.value = string.Empty;
            m_Panel.peopleInputField.value = string.Empty;
            m_Panel.zhutiInputField.value = string.Empty;
            m_Panel.contentInputField.label.text = " 请输入邮件内容...";
            m_Panel.peopleInputField.label.text = " 请输入名字...";
            m_Panel.zhutiInputField.label.text = " 请输入邮件主题...";
        }
        else if (go == m_Panel.chooseFriend.gameObject)
        {
            ItemMediator.panelType = PanelType.ChooseFriend;
            Facade.SendNotification(NotificationID.ItemInfo_Show);
        }
    }
    protected override void OnDestroy()
    {

        base.OnDestroy();
    }
}