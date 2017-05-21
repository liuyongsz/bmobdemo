using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProxyInstance;

public class GameProxy : Proxy<GameProxy>
{
    public bool m_IsStart;
    private EScene m_currSceneType;
    private string m_loadingscene;
    private bool _compelteConfig = false;
    /// <summary>
    /// 当前场景类型
    /// </summary>
    public EScene CurrSceneType
    {
        get { return m_currSceneType; }
    }

    public void SetSceneType(EScene val)
    {
        m_currSceneType = val;
    }

    private string m_currSceneName;
    /// <summary>
    /// 当前场景名字
    /// </summary>
    public string CurrSceneName
    {
        get { return m_currSceneName; }
    }


    public GameProxy()
        : base(ProxyID.Game)
    {

    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        if (m_IsStart == false)
        {
            m_IsStart = true;
            this.LoadConfig();
        }
    }

    /// <summary>加载配置表</summary>
    private void LoadConfig()
    {
        TextManager.Init(OnComplete_LoadedLang);

        if (!Define.UpdateMode)
        {
            GameManager.InitManager();
            PureMVC.Patterns.Facade.Instance.SendNotification(NotificationID.Login_Show);
        }
    }

    /// <summary>语言包加载完成</summary>
    private void OnComplete_LoadedLang()
    {  
        TextManager.SetQueryString();
        ItemManager.Init();
        CommonConfig.Init();
        LotterRewardManager.Init();
        SuitConfig.Init();
        ClothesConfig.Init();
        ClothesSlevelConfig.Init();
        ClothesLevelConfig.Init();
        BabyLikingConfig.Init();
        UseEquipConfig.Init();
        ClothesInheritConfig.Init();
        ClothesMapConfig.Init();
        CloneLevelConfig.Init();
        LevelRewardConfig.Init();
        ArenaConfig.Init();
        SkillLevelConfig.Init();
        PropertySkillLevelConfig.Init();
        BossDailyConfig.Init();
        BossRewardConfig.Init();
        InstanceProxy.Get<PieceSwitchConfig>().LoadXml();
        InstanceProxy.Get<PlayerPositionConfig>().LoadXml();
        InstanceProxy.Get<MatchArrayConfig>().LoadXml();
        InstanceProxy.Get<PositionAttributeConfig>().LoadXml();
        InstanceProxy.Get<RandNameConfig>().LoadXml();
        InstanceProxy.Get<SkinConfig>().LoadXml();
        InstanceProxy.Get<MonsterConfig>().LoadXml();
        InstanceProxy.Get<CloneConfig>().LoadXml();
        InstanceProxy.Get<CameraConfig>().LoadXml();
        InstanceProxy.Get<MentalityMaxConfig>().LoadXml();
        InstanceProxy.Get<AbilityConfig>().LoadXml();
        InstanceProxy.Get<SkillConfig>().LoadXml();
        InstanceProxy.Get<SkillAIConfig>().LoadXml();
    }


    /// <summary>
    /// 进入主场景
    /// </summary>
    public void GotoMainCity()
    {
        Facade.SendNotification(NotificationID.CHANGE_SCENE, new SceneVO("s_home", "s_home", false, EScene.MAINCITY));
    }

    public void GotoPVE(int cloneId = 1001)
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("s_footballmatch");
        Facade.SendNotification(NotificationID.CHANGE_SCENE, new SceneVO("s_match1", "s_match1", false, EScene.PVE));
    }


    public void GotoBattle()
    {
        Facade.SendNotification(NotificationID.CHANGE_SCENE, new SceneVO("s_pvp_zyz_lvdi", "s_pvp_zyz_lvdi",false,EScene.BATTLE));
       
    }
   
    public void LoadingScene(string scenename)
    {
        m_loadingscene = scenename;
    }

    public bool isLoadingScene(string scenename)
    {
        return m_loadingscene == scenename;

    }

    /// <summary>
    /// 重置场景
    /// </summary>
    /// <param name="scenename"></param>
    /// <param name="t"></param>
    public void RealEnterScene(string scenename, EScene t)
    {
        m_currSceneType = t;
        m_currSceneName = scenename;
        switch (t)
        {
            case EScene.LOGIN:
                    
                break;
            case EScene.MAINCITY:
               
                Facade.SendNotification(NotificationID.Login_Hide);
                Facade.SendNotification(NotificationID.Show_Main);
                Facade.SendNotification(NotificationID.Gold_Show);
                break;
            case EScene.BATTLE:

              
                break;
            case EScene.PVE:
               

                break;
            case EScene.ALL:
                break;
            case EScene.END:
                break;
            default:
                break;
        }
    }
}
