using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 全局引用
/// </summary>
public class AllRef
{
    public static EditerType EditerType;
    /// <summary> 踢球方向 </summary>
    public static E_ShootDirection ShootDir = E_ShootDirection.Left;
    public static E_PuQiuType KeeperPuQIuType;

    public static Vector3 v0,v1,v2,v3;

    /// <summary> 球 </summary>
    public static Ball Ball;

    public static CoreGame coreGame;

    /// <summary>是否在射门</summary>
    public static bool InShootGoal = false;
    /// <summary> 选择的球员 </summary>
    public static RotateSelector RotateSelector;

    private static Player m_currentPalyer = null;

    public static float StartPassTimer;

    public static bool DoStep;
    public static bool CanMoveToSynPos;
    /// <summary>被截断了</summary>
    public static bool HaveTarck;
    public static bool DoShoot;

    /// <summary>扑球动画延迟时间播放</summary>
    public static float PuQiuDelay = 0;

    /// <summary>
    /// 当前球员
    /// </summary>
    public static Player BallPlayer
    {
        get
        {
            if (null != Ball && null != Ball.owner)
            {
                m_currentPalyer = AllRef.Ball.owner.GetComponent<Player>();
            }

            return m_currentPalyer;
        }
        set
        {
            m_currentPalyer = null;
        }
    }
}
