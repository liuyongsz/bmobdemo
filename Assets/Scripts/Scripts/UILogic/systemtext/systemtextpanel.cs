using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PureMVC.Interfaces;


public class systemtextpanel : BasePanel
{
    public UILabel noticeText;
    public Transform notice;
}

public class SystemTextMediator : UIMediator<systemtextpanel>
{
    public static string textContent = string.Empty;
    private systemtextpanel panel
    {
        get { return m_Panel as systemtextpanel; }
    }

    public SystemTextMediator() : base("systemtextpanel")
    {    
        RegistPanelCall(NotificationID.SystemText_Show, base.OpenPanel);
        RegistPanelCall(NotificationID.SystemText_Hide, base.ClosePanel);
    }
  
    protected override void OnShow(INotification notification)
    {
        SetPanelType();
    }

    protected override void OnDestroy()
    {
        TimerManager.Destroy("notice");
    }

    void SetPanelType()
    {
            m_Panel.notice.gameObject.SetActive(true);
            m_Panel.noticeText.text = "通知通知！~~！@##通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！通知！！";
            TimerManager.AddTimerRepeat("notice", 0, UpdateNotice);   
    }
    void UpdateNotice()
    {
        m_Panel.noticeText.transform.localPosition += new Vector3(-1f, 0, 0);
        if (m_Panel.noticeText.transform.localPosition.x == -m_Panel.noticeText.width)
        {
            TimerManager.Destroy("notice");
            Facade.SendNotification(NotificationID.SystemText_Hide);
        }
    }
}
