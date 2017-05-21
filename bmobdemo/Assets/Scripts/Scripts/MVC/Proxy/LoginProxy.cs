using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class LoginProxy : Proxy<LoginProxy>
{
    private string m_account;
    private string m_passwd ;

    private Dictionary<System.UInt64, Dictionary<string, object>> m_avatarList = null;

    public LoginProxy()
        : base(ProxyID.Login)
    {  
        KBEngine.Event.registerOut(this, "onCreateAvatarFail");
        KBEngine.Event.registerOut(this, "onLoginFailed");
        KBEngine.Event.registerOut(this, "onVersionNotMatch");
        KBEngine.Event.registerOut(this, "onScriptVersionNotMatch");
        KBEngine.Event.registerOut(this, "onLoginBaseappFailed");
        KBEngine.Event.registerOut(this, "onLoginSuccessfully");
        KBEngine.Event.registerOut(this, "onLoginBaseapp");
        KBEngine.Event.registerOut(this, "Loginapp_importClientMessages");
        KBEngine.Event.registerOut(this, "Baseapp_importClientMessages");
        KBEngine.Event.registerOut(this, "Baseapp_importClientEntityDef");
    }

    public delegate void NetEventFunctionxxx(params object[] datas);

    /// <summary>
    /// 发送登录请求
    /// </summary>
    /// <param name="account"></param>
    /// <param name="pwd"></param>
    public void Send_Login(string account,string pwd)
    {
        //m_passwd = pwd;
        //m_account = account;

        if (account.Length > 0 && account.Length > 5)
        {           
            KBEngine.Event.fireIn("login", account, account, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
        }
        else
        {
            err("account or password is error, length < 6!(账号或者密码错误，长度必须大于5!)");
        }
    }

    public void Send_RegisterAccount(string account, string pwd)
    {
        Debug.Log("stringAccount:" + account);
        Debug.Log("stringPasswd:" + pwd);

        if (account.Length > 0 && pwd.Length > 5)
        {
            info("connect to server...(连接到服务端...)");

            KBEngine.Event.fireIn("createAccount", account, pwd, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
        }
        else
        {
            err("account or password is error, length < 6!(账号或者密码错误，长度必须大于5!)");
        }
    }
    /// <summary>
    /// 创建角色
    /// </summary>
    public void Send_CreateAvatar()
    {
        Facade.SendNotification(NotificationID.Login_Hide);
        Facade.SendNotification(NotificationID.Create_Show);       
    }
    /// <summary>
    /// 创建角色失败
    /// </summary>
    public void onCreateAvatarFail()
    {
        GUIManager.SetPromptInfo(TextManager.GetUIString("UICreate5"), null);    
    }

    public void onLoginFailed(System.UInt16 failedcode)
    {
        ServerMediator.loginSucess = false;
        if (failedcode == 20)
        {
            err("login is failed(登陆失败), err=" + KBEngineApp.app.serverErr(failedcode) + ", " + System.Text.Encoding.ASCII.GetString(KBEngineApp.app.serverdatas()));
        }
        else
        {
            err("login is failed(登陆失败), err=" + KBEngineApp.app.serverErr(failedcode));
        }
    }

    public void onVersionNotMatch(string verInfo, string serVerInfo)
    {
        err("");
    }

    public void onScriptVersionNotMatch(string verInfo, string serVerInfo)
    {
        err("");
    }

    public void onLoginBaseappFailed(System.UInt16 failedcode)
    {
        err("loginBaseapp is failed(登陆网关失败), err=" + KBEngineApp.app.serverErr(failedcode));
    }

    public void onLoginBaseapp()
    {
        info("connect to loginBaseapp, please wait...(连接到网关， 请稍后...)");
    }

    public void onLoginSuccessfully(System.UInt64 rndUUID, System.Int32 eid, Account accountEntity)
    {
        ServerMediator.loginSucess = true;
        // 无角色需要创建创建
        if (accountEntity.lastSelCharacter <= 0)
        {
            Send_CreateAvatar();
        }        
        info("login is successfully!(登陆成功!)");
    }

    public void Loginapp_importClientMessages()
    {
        info("Loginapp_importClientMessages ...");
    }

    public void Baseapp_importClientMessages()
    {
        info("Baseapp_importClientMessages ...");
    }

    public void Baseapp_importClientEntityDef()
    {
        info("importClientEntityDef ...");
    }   

    public void selectAvatarGame(System.UInt64 dbid)
    {
        Dbg.DEBUG_MSG("Account::selectAvatarGame: dbid=" + dbid);

        GameProxy.Instance.GotoMainCity();

        Facade.SendNotification(NotificationID.Login_Hide);
    }
     
    public void err(string s)
    {
        KBEngine.Dbg.ERROR_MSG(s);
    }

    public void info(string s)
    {
        KBEngine.Dbg.DEBUG_MSG(s);
    }

}//end ResourceConfig
