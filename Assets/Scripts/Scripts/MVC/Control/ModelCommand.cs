using PureMVC.Patterns;
using PureMVC.Interfaces;

class ModelCommand: SimpleCommand
{
    public override void Execute(INotification notification)
    {
        Facade.RegisterProxy(GameProxy.Instance);
        Facade.RegisterProxy(LoginProxy.Instance);
        Facade.RegisterProxy(BagProxy.Instance);
        Facade.RegisterProxy(BabyProxy.Instance);
        Facade.RegisterProxy(CommonProxy.Instance);
        Facade.RegisterProxy(PlayerProxy.Instance);
        Facade.RegisterProxy(ChatProxy.Instance);
        Facade.RegisterProxy(MailProxy.Instance);
        Facade.RegisterProxy(StoreProxy.Instance);
        Facade.RegisterProxy(PowerProxy.Instance);
        Facade.RegisterProxy(CardProxy.Instance);
        Facade.RegisterProxy(FriendProxy.Instance);
        Facade.RegisterProxy(EquipProxy.Instance);
        Facade.RegisterProxy(CheckpointProxy.Instance);
        Facade.RegisterProxy(GameShopProxy.Instance);
        Facade.RegisterProxy(RankProxy.Instance);
        Facade.RegisterProxy(ArenaProxy.Instance);
        Facade.RegisterProxy(GuildProxy.Instance);
        Facade.RegisterProxy(WorldBossProxy.Instance);
    }
}

