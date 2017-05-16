using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using System.Text.RegularExpressions;

public class createpanel : BasePanel
{
    public UISprite randomBtn;
    public UISprite startgameBtn;
    public UISprite striker;
    public UISprite midfield;
    public UISprite defender;
    public UIInput inputField;
    public UILabel jobDesc;

}
public class CreateMediator : UIMediator<createpanel>
{
    private createpanel panel
    {
        get
        {
            return m_Panel as createpanel;
        }
    }
    private CommonInfo info;
    private System.Random random;
    private int Job = -1;
    private string stringAvatarName = string.Empty;

    public CreateMediator() : base("createpanel")
    {
        m_isprop = false;
        RegistPanelCall(NotificationID.Create_Show, OpenPanel);
        RegistPanelCall(NotificationID.Create_Hide, ClosePanel);
    }
    protected override void AddComponentEvents()
    {
        UIEventListener.Get(panel.randomBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.startgameBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.striker.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.midfield.gameObject).onClick = OnClick;
        UIEventListener.Get(panel.defender.gameObject).onClick = OnClick;      
    }
    protected override void OnShow(INotification notification)
    {
        info = CommonConfig.GetCommonInfo(3);
        panel.inputField.label.text = TextManager.GetUIString("UI1054");       
        panel.inputField.characterLimit = info.value;
        EventDelegate.Add(panel.inputField.onChange, NameValueChanged);
    }
    protected void OnClick(GameObject go)
    {
        if (go == panel.startgameBtn.gameObject)
        {
            stringAvatarName = panel.inputField.value;
            if (Job < 0)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UICreate2"), null);
                return;
            }
            if (CheckName(stringAvatarName) != 2)
            {
                if (CheckName(stringAvatarName) == 0)
                    GUIManager.SetPromptInfo(TextManager.GetUIString("UICreate4"), null);
                else
                    GUIManager.SetPromptInfo(TextManager.GetUIString("UICreate3"), null);               
                panel.inputField.value = string.Empty;
            }
            else
            {
                KBEngine.Event.fireIn("reqCreateAvatar",(Byte)Job, stringAvatarName);
            }
        }
        else if (go == panel.striker.gameObject)
        {
            Job = 0;
        }
        else if (go == panel.midfield.gameObject)
        {
            Job = 1;
        }
        else if (go == panel.defender.gameObject)
        {
            Job = 2;
        }
        else if (go == panel.randomBtn.gameObject)
        {
            random = new System.Random();
            int surnameindex = random.Next(1, RandNameConfig.surnameList.Count);
            int nameindex = random.Next(1, RandNameConfig.nameList.Count);
            stringAvatarName = UtilTools.StringBuilder(RandNameConfig.surnameList[surnameindex], RandNameConfig.nameList[nameindex]);
            panel.inputField.value = stringAvatarName;
        }
    }
    void NameValueChanged()
    {
        if (panel.inputField.value.Length >= info.value)
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_31"), null);
    }
    /// <summary>
    /// 判断字符串
    /// </summary>
    private int CheckName(string name)
    {
        char[] charStr = name.ToLower().ToCharArray();
        if (charStr.Length <= 0)
        {
            return 0;
        }
        for (int i = 0; i < charStr.Length; i++)
        {
            if (Regex.IsMatch(charStr[i].ToString(), @"^[\u4e00-\u9fa5]+$"))
            {
                continue;
            }
            int num = Convert.ToInt32(charStr[i]);
            if (num == 32 || (num >= 0 && num <= 64) || (num >= 133 && num <= 140) || (num >= 173 && num <= 177))
            {
                return 1;
            }
        }
        return 2;
    }

    protected override void OnDestroy()
    {
        EventDelegate.Remove(panel.inputField.onChange, NameValueChanged);
        base.OnDestroy();
    }
}