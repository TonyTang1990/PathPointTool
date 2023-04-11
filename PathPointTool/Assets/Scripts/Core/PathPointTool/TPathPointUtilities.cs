/*
 * Description:             TPathPointUtilities.cs
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
    /// TPathPointUtilities.cs
    /// 路点静态工具类
    /// </summary>
    public static class TPathPointUtilities
    {
        /// <summary>
        /// 获取导出目录全路径
        /// </summary>
        /// <returns></returns>
        public static string GetExportFolderFullPath()
        {
            return PathUtilities.GetAssetFullPath(TPathPointConst.ExportFolderProjectRelativePath);
        }

        /// <summary>
        /// 确保导出目录存在
        /// </summary>
        public static void MakeSureExportFolderExist()
        {
            var exportFolderFullPath = GetExportFolderFullPath();
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
            var exportFolderFullPath = GetExportFolderFullPath();
            var fileName = GetExportFileNameByType(pathType);
            return Path.Combine(exportFolderFullPath, fileName);
        }
    }
}