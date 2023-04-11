/*
 * Description:             TPathPointConst.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/11
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathPointConst.cs
    /// 路点常量
    /// </summary>
    public static class TPathPointConst
    {
        /// <summary>
        /// 导出目录相对工程目录路径
        /// </summary>
        public const string ExportFolderProjectRelativePath = "/PathPointExport/";

        /// <summary>
        /// Transform类型信息
        /// </summary>
        public static Type TransformType = typeof(Transform);
    }
}