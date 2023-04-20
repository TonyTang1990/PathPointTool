/*
 * Description:             TPathUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/11
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathUtilities.cs
    /// 路线静态工具类
    /// </summary>
    public static class TPathUtilities
    {
        /// <summary>
        /// TPathTweener的UID
        /// </summary>
        private static int PathTweenerUID = 0;

        /// <summary>
        /// 获取下一个有效TPathTweener UID
        /// </summary>
        /// <returns></returns>
        public static int GetNextPathTweenerUID()
        {
            return PathTweenerUID;
        }

        /// <summary>
        /// 获取指定路点路线类型的导出目录全路径
        /// </summary>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string GetExportFolderFullPath(TPathType pathType)
        {
            var baseExportFolderFullPath = PathUtilities.GetAssetFullPath(TPathConst.ExportFolderProjectRelativePath);
            return Path.Combine(baseExportFolderFullPath, pathType.ToString());
        }

        /// <summary>
        /// 确保导出目录存在
        /// </summary>
        /// <param name="pathType"></param>
        public static void MakeSureExportFolderExist(TPathType pathType)
        {
            var exportFolderFullPath = GetExportFolderFullPath(pathType);
            FolderUtilities.CheckAndCreateSpecificFolder(exportFolderFullPath);
        }

        /// <summary>
        /// 获取指定路径类型的导出文件名
        /// </summary>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string GetExportFileNameByType(TPathType pathType)
        {
            var nowDate = DateTime.Now;
            return $"{pathType.ToString()}_{nowDate.Month}_{nowDate.Day}_{nowDate.Hour}_{nowDate.Minute}_{nowDate.Second}.csv";
        }

        /// <summary>
        /// 获取指定路径类型的导出文件全路径
        /// </summary>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string GetExportFileFullPathByType(TPathType pathType)
        {
            var exportFolderFullPath = GetExportFolderFullPath(pathType);
            var fileName = GetExportFileNameByType(pathType);
            return Path.Combine(exportFolderFullPath, fileName);
        }

        /// <summary>
        /// 获取指定路线类型的分段顶点数量
        /// </summary>
        /// <param name="pathwayType"></param>
        /// <returns></returns>
        public static int GetSegmentPointNumByType(TPathwayType pathwayType)
        {
            if(pathwayType == TPathwayType.Liner)
            {
                return 2;
            }
            else if(pathwayType == TPathwayType.Bezier)
            {
                return 3;
            }
            else if(pathwayType == TPathwayType.CubicBezier)
            {
                return 4;
            }
            else
            {
                Debug.LogError($"不支持的路线类型:{pathwayType.ToString()}，获取路线分段顶点数量失败！");
                return 0;
            }
        }
    }
}