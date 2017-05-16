using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KBEngine
{
    public class Avatar : KBEngine.GameObject
    {
        public static Avatar Instance;
        #region 注册
        public Avatar()
        {
            Instance = this;
        }

        public override void __init__()
        {
            KBEngine.Event.registerIn(this, "callServerMethod");
        }

        public override void onDestroy()
        {
            if (isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }
        }

        /// <summary>
        /// 客户端向服务的发送消息
        /// </summary>
        public void callServerMethod(List<object> args)
        {
            string funcName = args[0].ToString();
            args.RemoveAt(0);
            baseCall(funcName, args.ToArray());
        }

        #endregion
                  
        public void processOutEvent(string methodName ,object[] param)
        {  
            KBEngine.Event.fireOut(methodName, param);
        }
    }
}
