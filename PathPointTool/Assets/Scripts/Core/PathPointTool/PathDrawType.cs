/*
 * Description:             PathDrawType.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// PathDrawType.cs
    /// 路线绘制类型
    /// </summary>
    public enum PathDrawType
    {
        Line = 1,               // 直线
        Bezier,                 // 二阶Bezier曲线
        ThreeBezier,            // 三姐Bezier曲线
    }
}