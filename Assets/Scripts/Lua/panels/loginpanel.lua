xlua.hotfix(CS.LoginMediator, {

 OnShow = function(self)
    print("fromlua====OnStart")
	print("Hello Lua")
    CS.PanelManager.OpenPanel("TestPanel3")
 end
})