public abstract class U3SDKInterface{
 
    public delegate void LoginSucHandler(U3LoginResult data);
    public delegate void LogoutHandler();
 
    private static U3SDKInterface _instance;
 
    public LoginSucHandler OnLoginSuc;
    public LogoutHandler OnLogout;
 
 
    public static U3SDKInterface Instance
    {
        get
        {
            if (_instance == null)
            {
#if UNITY_EDITOR || UNITY_STANDLONE
                _instance = new SDKInterfaceDefault();
#elif UNITY_ANDROID
                _instance = new SDKInterfaceAndroid();
#elif UNITY_IOS
                _instance = new SDKInterfaceIOS();
#endif
            }
 
            return _instance;
        }
    }
 
    //初始化
    public abstract void Init();
 
    //登录
    public abstract void Login();
 
    //自定义登录，用于腾讯应用宝，QQ登录，customData="QQ";微信登录，customData="WX"
    public abstract void LoginCustom(string customData);
 
    //切换帐号
    public abstract void SwitchLogin();
 
    //登出
    public abstract bool Logout();
 
    //显示个人中心
    public abstract bool ShowAccountCenter();
 
    //上传游戏数据
    public abstract void SubmitGameData(U3ExtraGameData data);
 
    //调用SDK的退出确认框,返回false，说明SDK不支持退出确认框，游戏需要使用自己的退出确认框
    public abstract bool SDKExit();
 
    //调用SDK支付界面
    public abstract void Pay(U3PayParams data);
 
    //SDK是否支持退出确认框
    public abstract bool IsSupportExit();
 
    //SDK是否支持用户中心
    public abstract bool IsSupportAccountCenter();
 
    //SDK是否支持登出
    public abstract bool IsSupportLogout();
 
    //去U3Server获取游戏订单号，这里逻辑是访问游戏服务器，然后游戏服务器去U3Server获取订单号
    //并返回
    public U3PayParams reqOrder(U3PayParams data)
    {
        //TODO 去游戏服务器获取订单号
 
        //测试
        data.orderID = "345435634534";
        data.extension = "test";
 
        return data;
    }
 
}