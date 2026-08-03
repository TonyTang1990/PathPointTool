/*
 * Description:             TPathPointData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathPointData.cs
    /// 路点数据组件
    /// </summary>
    [Serializable]
    public class TPathPointData
    {
        /// <summary>
        /// 坐标位置
        /// </summary>
        [Header("坐标位置")]
        public Vector3 Position;

        /// <summary>
        /// 路点类型
        /// </summary>
        [Header("路点类型")]
        public TPathPointType PathPointType = TPathPointType.Normal;

        /// <summary>
        /// 等待路点的等待时长
        /// </summary>
        [Header("等待路点的等待时长")]
        public float WaitTime = 0f;

        /// <summary>
        /// 跳跃路点的跳跃动画名称
        /// </summary>
        [Header("跳跃路点的跳跃动画名称")]
        public string JumpAnimName = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TPathPointData()
        {
            Position = Vector3.zero;
            PathPointType = TPathPointType.Normal;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="position"></param>
        /// <param name="pathPointType"></param>
        public TPathPointData(Vector3 position, TPathPointType pathPointType = TPathPointType.Normal)
        {
            Position = position;
            PathPointType = pathPointType;
        }

        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Position:{Position}, PathPointType:{PathPointType}, WaitTime:{WaitTime}, JumpAnimName:{JumpAnimName}";
        }
    }
}