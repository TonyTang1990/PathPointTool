/*
 * Description:             UnityUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UnityUtilities.cs
/// Unity静态工具类
/// </summary>
public static class UnityUtilities
{
    #region UV部分
    /// <summary>
    /// 左下UV
    /// </summary>
    public static Vector2 BottomLeftUV = new Vector2(0f, 0f);

    /// <summary>
    /// 左上UV
    /// </summary>
    public static Vector2 TopLeftUV = new Vector2(0f, 1f);

    /// <summary>
    /// 右上UV
    /// </summary>
    public static Vector2 TopRightUV = new Vector2(1f, 1f);

    /// <summary>
    /// 右下UV
    /// </summary>
    public static Vector2 BottomRightUV = new Vector2(1f, 0f);
    #endregion
}