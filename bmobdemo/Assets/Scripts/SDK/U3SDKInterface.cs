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
 
    //��ʼ��
    public abstract void Init();
 
    //��¼
    public abstract void Login();
 
    //�Զ����¼��������ѶӦ�ñ���QQ��¼��customData="QQ";΢�ŵ�¼��customData="WX"
    public abstract void LoginCustom(string customData);
 
    //�л��ʺ�
    public abstract void SwitchLogin();
 
    //�ǳ�
    public abstract bool Logout();
 
    //��ʾ��������
    public abstract bool ShowAccountCenter();
 
    //�ϴ���Ϸ����
    public abstract void SubmitGameData(U3ExtraGameData data);
 
    //����SDK���˳�ȷ�Ͽ�,����false��˵��SDK��֧���˳�ȷ�Ͽ���Ϸ��Ҫʹ���Լ����˳�ȷ�Ͽ�
    public abstract bool SDKExit();
 
    //����SDK֧������
    public abstract void Pay(U3PayParams data);
 
    //SDK�Ƿ�֧���˳�ȷ�Ͽ�
    public abstract bool IsSupportExit();
 
    //SDK�Ƿ�֧���û�����
    public abstract bool IsSupportAccountCenter();
 
    //SDK�Ƿ�֧�ֵǳ�
    public abstract bool IsSupportLogout();
 
    //ȥU3Server��ȡ��Ϸ�����ţ������߼��Ƿ�����Ϸ��������Ȼ����Ϸ������ȥU3Server��ȡ������
    //������
    public U3PayParams reqOrder(U3PayParams data)
    {
        //TODO ȥ��Ϸ��������ȡ������
 
        //����
        data.orderID = "345435634534";
        data.extension = "test";
 
        return data;
    }
 
}