using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SDKInterfaceDefault : U3SDKInterface
{
    //初始化
    public override void Init()
    {

    }

    //登录
    public override void Login()
    {

    }

    //自定义登录，用于腾讯应用宝，QQ登录，customData="QQ";微信登录，customData="WX"
    public override void LoginCustom(string customData)
    {

    }

    //切换帐号
    public override void SwitchLogin()
    {

    }

    //登出
    public override bool Logout()
    {
        return false;
    }

    //显示个人中心
    public override bool ShowAccountCenter()
    {
        return false;
    }

    //上传游戏数据
    public override void SubmitGameData(U3ExtraGameData data)
    {

    }

    //调用SDK的退出确认框,返回false，说明SDK不支持退出确认框，游戏需要使用自己的退出确认框
    public override bool SDKExit()
    {
        return false;
    }

    //调用SDK支付界面
    public override void Pay(U3PayParams data)
    {

    }

    //SDK是否支持退出确认框
    public override bool IsSupportExit()
    {
        return false;
    }

    //SDK是否支持用户中心
    public override bool IsSupportAccountCenter()
    {
        return false;
    }

    //SDK是否支持登出
    public override bool IsSupportLogout()
    {
        return false;
    }
}