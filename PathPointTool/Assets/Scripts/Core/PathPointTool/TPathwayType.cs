/*
 * Description:             TPathwayType.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathwayType.cs
    /// 路线类型
    /// </summary>
    public enum TPathwayType
    {
        Line = 1,               // 直线
        Bezier,                 // 二阶Bezier曲线
        CubicBezier,            // 三姐Bezier曲线
    }
}