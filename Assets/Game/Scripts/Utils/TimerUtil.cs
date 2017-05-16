using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUtil: MonoBehaviour
{
    /// <summary>
    /// 获取时间字符串(分秒)
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string MinuteAndSecond(int second,string sign = ":")
    {
        int min = second / 60;
        int sec = second % 60;

        string secStr = sec > 0 ? sec.ToString() : "00";
      
        if(min <= 0)    return "00";

        string minStr = min > 0 ? min.ToString() : "00";

        return minStr + sign + secStr;
    }
}
