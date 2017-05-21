using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CommonFun
{
    public static void TurnControlPlayer(Transform transform, Vector3 target,ref float currentAcceleration)
    {
        // rotar hasta conseguir la direccion demandada por el input
        float angle = Vector3.Angle(target - transform.position, transform.forward) / 4.0f;
        currentAcceleration *= 1.0f - Mathf.Clamp(angle / 180.0f, 0.0f, 0.5f);
        Vector3 relativePosDir = transform.InverseTransformPoint(target);

        float face = relativePosDir.x > 0.0f ? 1f : -1f;
        transform.Rotate(0.0f, face * angle * Time.deltaTime * 25.0f, 0.0f);
    }
    /// <summary>
    /// 归属控制
    /// </summary>
    public static void PlayerCaseControling()
    {

    } 

    /// <summary>
    /// 球给到某个球员的脚下
    /// </summary>
    public static void BallToPlayerFoot()
    {

    }

    /// <summary>
    /// 球给到某个球员的脚下
    /// </summary>
    public static void BallToPlayerFoot(Transform target)
    {
       
    }

    /// <summary>
    /// 随机选择己方玩家
    /// </summary>
    public static void RandomSelectedPlayer()
    {
        int count = Players.Count;

        int rndIndex = UnityEngine.Random.Range(0,count);

        GameObject player = m_players[rndIndex];

        SetSelectedPlayer(player);
    }

    /// <summary>
    /// 设置选择球员
    /// </summary>
    /// <param name="player"></param>
    public static void SetSelectedPlayer(GameObject player)
    {

    }
    
    private static List<GameObject> m_players;
    /// <summary>
    /// 我方玩家
    /// </summary>
    public static List<GameObject> Players
    {
        get
        {
            if(null == m_players)
                m_players = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerTeam1"));

            return m_players;
        }
        set
        {
            m_players = null;
        }
    }

    private static List<GameObject> m_oponents;
    /// <summary>
    /// 对方玩家
    /// </summary>
    public static List<GameObject> Oponents
    {
        get
        {
            if (null == m_oponents)
                m_oponents = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerTeam1"));

            return m_oponents;
        }
        set
        {
            m_oponents = null;
        }
    }
		
	public static void Debug(string content,string color="#c3ff55")
	{
		string str = string.Format("<color={0}>{1}</color>", color,content);
		UnityEngine.Debug.Log (str);
	}
}
