using PureMVC.Patterns;
using PureMVC.Interfaces;
using System.Collections.Generic;
using System;
using System.Reflection;

public enum EScene
{
    NONE,
    LOGIN,
    ALL,
    BATTLE,
    MAINCITY,
    PVE,
    LOADING,
    END,
}


public class ViewCommand: SimpleCommand
{

    static bool m_int = false;
    private  void Init()
    {
        if (m_int) return;
        Facade.RegisterCommand(NotificationID.UPDATE_SCENE_MEDIATOR, typeof(ViewCommand));

        m_int = true;
        RegisterUIMediator();
    }


    public override void Execute(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationID.UPDATE_SCENE_MEDIATOR:
                //EnterScene((notification.Body as ChangeSceneMsg).m_scenetype);
                break;
            case NotificationID.START_UP_PUREMVC:
                Init();
                break;
        }
    }

    /// <summary>注册场景的UIMediator</summary>
    private void RegisterUIMediator()
    {
        Facade.RegisterMediator(new MainMediator());
        Facade.RegisterMediator(new PromptMediator());
        Facade.RegisterMediator(new StoreMediator());
        Facade.RegisterMediator(new PlayerMediator());
        Facade.RegisterMediator(new EmailMediator());
        Facade.RegisterMediator(new GoldMediator());
        Facade.RegisterMediator(new CardMediator());
        Facade.RegisterMediator(new SystemTextMediator());
        Facade.RegisterMediator(new CreateMediator());
        Facade.RegisterMediator(new FriendMediator());
        Facade.RegisterMediator(new PowerMediator());
        Facade.RegisterMediator(new ChatMediator());
        Facade.RegisterMediator(new BagMediator());
        Facade.RegisterMediator(new BabyMediator());
        Facade.RegisterMediator(new ItemMediator());
        Facade.RegisterMediator(new TeamMediator());
        Facade.RegisterMediator(new BallerLevelMediator()); 
        Facade.RegisterMediator(new ui_balloperMediator());
        Facade.RegisterMediator(new ui_opertestMediator());
        Facade.RegisterMediator(new ui_matchInfoMediator());
        Facade.RegisterMediator(new ui_matchInfoMediator());
        Facade.RegisterMediator(new ServerMediator());
        Facade.RegisterMediator(new LoginMediator());
        Facade.RegisterMediator(new UpdateResourcesMediator());
        Facade.RegisterMediator(new EquipMediator());
        Facade.RegisterMediator(new EquipStarMediator());
        Facade.RegisterMediator(new EquipStrongMediator());
        Facade.RegisterMediator(new EquipMakeMediator());
        Facade.RegisterMediator(new EquipStrongResultMediator());
        Facade.RegisterMediator(new EquipChooseMediator());
        Facade.RegisterMediator(new EquipInformationMediator());
        Facade.RegisterMediator(new EquipInsetMediator());
        Facade.RegisterMediator(new GemChooseMediator());
        Facade.RegisterMediator(new EquipInheritMediator());
        Facade.RegisterMediator(new GemCompoundMediator());
        Facade.RegisterMediator(new TeamFormineMediator());
        Facade.RegisterMediator(new BenchMediator()); 
        Facade.RegisterMediator(new BenchChooseMediator());
        Facade.RegisterMediator(new FormationPropActiveMediator());
        Facade.RegisterMediator(new CheckpointMediator());
        Facade.RegisterMediator(new GameShopMediator());
        Facade.RegisterMediator(new ArenaMediator());
        Facade.RegisterMediator(new BuyItemMediator());
        Facade.RegisterMediator(new RankMediator());
        Facade.RegisterMediator(new GuildListMediator());
        Facade.RegisterMediator(new GuildCreatMediator());
        Facade.RegisterMediator(new GuildInfoMediator());
        Facade.RegisterMediator(new GuildMainMediator());
        Facade.RegisterMediator(new GuildOfficeMediator());
        Facade.RegisterMediator(new GuildDonationMediator());
        Facade.RegisterMediator(new GuildLVUpMediator());
        Facade.RegisterMediator(new GuildSpeedMediator());
        Facade.RegisterMediator(new GuildAlterNameMediator());
        Facade.RegisterMediator(new GuildAlterNoticeMediator());
        Facade.RegisterMediator(new GuildAlterInfoMediator());
        Facade.RegisterMediator(new GuildInteractMediator());
        Facade.RegisterMediator(new GuildTacticMediator());
        Facade.RegisterMediator(new GuildCounselorMediator());
        Facade.RegisterMediator(new SandTableMediator());
        Facade.RegisterMediator(new WorldBossMediator());
        Facade.RegisterMediator(new BattleEndMediator());
    }
}

