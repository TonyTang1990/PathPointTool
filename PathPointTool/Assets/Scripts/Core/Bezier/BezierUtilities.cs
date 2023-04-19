/*
 * Description:             BezierUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BezierUtilities.cs
/// Bezier静态工具类
/// </summary>
public static class BezierUtilities
{
    // Note：
    // 1. 高阶Bezier曲线计算复杂，推荐用N个3阶Bezier曲线模拟

    /// <summary>
    /// 根据t(0-1)计算一阶Bezier曲线上面对应的点
    /// </summary>
    /// <param name="p0">第一个点</param>
    /// <param name="p1">第二个点</param>
    /// <param name="t">插值(0-1)</param>
    /// <returns></returns>
    public static Vector3 CaculateLinerPoint(Vector3 p0, Vector3 p1, float t)
    {
        // 原始公式:
        /*
        return (1 - t) * p0 + t * p1;
        */
        return (1 - t) * p0 + t * p1;
    }

    /// <summary>
    /// 根据t(0-1)计算二阶贝塞尔曲线上面对应的点
    /// </summary>
    /// <param name="p0">第一个点</param>
    /// <param name="p1">第二个点</param>
    /// <param name="p2">第三个点</param>
    /// <param name="t">插值(0-1)</param>
    /// <returns></returns>
    public static Vector3 CaculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        // 原始公式:
        /*
        var p0p1 = (1 - t) * p0 + t * p1;
        var p1p2 = (1 - t) * p1 + t * p2;
        return (1 - t) * p0p1 + t * p1p2;
        */
        // 简化运算:
        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    /// <summary>
    /// 根据t(0-1)计算三阶贝塞尔曲线上面对应的点
    /// </summary>
    /// <param name="p0">第一个点</param>
    /// <param name="p1">第二个点</param>
    /// <param name="p2">第三个点</param>
    /// <param name="p3">第四个点</param>
    /// <param name="t">插值(0-1)</param>
    /// <returns></returns>
    public static Vector3 CaculateCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // 原始公式:
        /*
        var p0p1 = (1 - t) * p0 + t * p1;
        var p1p2 = (1 - t) * p1 + t * p2;
        var p2p3 = (1 - t) * p2 + t * p3;
        var p0p1p2 = (1 - t) * p0p1 + t * p1p2;
        var p1p2p3 = (1 - t) * p1p2 + t * p2p3;
        return (1 - t) * p0p1p2 + t * p1p2p3;
        */
        // 简化运算:
        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        var ttt = tt * t;
        var uuu = uu * u;
        return uuu * p0 + 3 * uu * t * p1 + 3 * tt * u * p2 + ttt * p3;
    }

    /// <summary>
    /// 获取存储的一阶Bezier曲线细分顶点的数组
    /// </summary>
    /// <param name="p0">第一个点</param>
    /// <param name="p1">第二个点</param>
    /// <param name="segmentNum">细分段数</param>
    /// <returns>存储贝塞尔曲线点的数组</returns>
    public static Vector3[] GetLinerList(Vector3 p0, Vector3 p1, int segmentNum)
    {
        var pathPointNum = segmentNum + 1;
        Vector3[] path = new Vector3[pathPointNum];
        for (int i = 0; i < pathPointNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pathPoint = CaculateLinerPoint(p0, p1, t);
            path[i] = pathPoint;
        }
        return path;
    }

    /// <summary>
    /// 获取存储的二次贝塞尔曲线细分顶点的数组
    /// </summary>
    /// <param name="p0">起始点</param>
    /// <param name="p1">控制点</param>
    /// <param name="p2">目标点</param>
    /// <param name="segmentNum">细分段数</param>
    /// <returns>存储贝塞尔曲线点的数组</returns>
    public static Vector3[] GetBeizerList(Vector3 p0, Vector3 p1, Vector3 p2, int segmentNum)
    {
        var pathPointNum = segmentNum + 1;
        Vector3[] path = new Vector3[pathPointNum];
        for (int i = 0; i < pathPointNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pathPoint = CaculateBezierPoint(p0, p1, p2, t);
            path[i] = pathPoint;
        }
        return path;
    }

    /// <summary>
    /// 获取存储的三次贝塞尔曲线细分顶点的数组
    /// </summary>
    /// <param name="p0">起始点</param>
    /// <param name="p1">控制点1</param>
    /// <param name="p2">控制点2</param>
    /// <param name="p3">目标点</param>
    /// <param name="segmentNum">细分段数</param>
    /// <returns>存储贝塞尔曲线点的数组</returns>
    public static Vector3[] GetCubicBeizerList(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segmentNum)
    {
        var pathPointNum = segmentNum + 1;
        Vector3[] path = new Vector3[pathPointNum];
        for (int i = 0; i < pathPointNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pathPoint = CaculateCubicBezierPoint(p0, p1, p2, p3, t);
            path[i] = pathPoint;
        }
        return path;
    }
}