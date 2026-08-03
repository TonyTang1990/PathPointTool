/*
 * Description:             TPathUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/11
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
        /// 下一个TPathTweener的UID
        /// </summary>
        private static int NextPathTweenerUID = 0;

        /// <summary>
        /// 默认绘制Icon
        /// </summary>
        private const string DEFAULT_DRAW_ICON = "PathPointTool/pathpoint";

        /// <summary>
        /// 路点类型和绘制Icon映射Map<路点类型, 绘制Icon>
        /// </summary>
        private static Dictionary<TPathPointType, string> mPathTypeDrawIconMap = new Dictionary<TPathPointType, string>()
        {
            { TPathPointType.Invalide, "PathPointTool/invalide" },
            { TPathPointType.Normal, "PathPointTool/normalPathPoint" },
            { TPathPointType.Wait, "PathPointTool/waitPathPoint" },
            { TPathPointType.Jump, "PathPointTool/jumpPathPoint" },
        };

        /// <summary>
        /// 导出类型和文件扩展名映射Map<导出类型, 文件扩展名>
        /// </summary>
        private static Dictionary<TPathExportType, string> mPathExportTypeExtensionMap = new Dictionary<TPathExportType, string>()
        {
            { TPathExportType.Json, ".json" },
        };

        /// <summary>
        /// 获取下一个有效TPathTweener UID
        /// </summary>
        /// <returns></returns>
        public static int GetNextPathTweenerUID()
        {
            return NextPathTweenerUID++;
        }

        /// <summary>
        /// 获取指定导出类型的文件扩展名
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        private static string GetExportTypeExtension(TPathExportType exportType)
        {
            if(!mPathExportTypeExtensionMap.TryGetValue(exportType, out var extension))
            {
                Debug.LogError($"不支持的导出类型:{exportType}，获取导出文件扩展名失败！");
                return string.Empty;
            }
            return extension;
        }

        /// <summary>
        /// 获取指定导出文件名和导出类型的完整导出文件名
        /// Note:
        /// 为空表示无有效导出文件名
        /// </summary>
        /// <param name="exportFileName"></param>
        /// <param name="exportType"></param>
        /// <returns></returns>
        private static string GetExportFileName(string exportFileName, TPathExportType exportType)
        {
            var extension = GetExportTypeExtension(exportType);
            if(string.IsNullOrEmpty(extension))
            {
                Debug.LogError($"获取导出类型:{exportType}的文件扩展名失败！");
                return string.Empty;
            }
            return $"{exportFileName}{extension}";
        }

        /// <summary>
        /// 获取指定导出类型和路点路线类型的导出目录全路径
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string GetExportFolderFullPath(TPathExportType exportType, TPathType pathType)
        {
            var baseExportFolderFullPath = PathUtilities.GetAssetFullPath(TPathConst.ExportFolderProjectRelativePath);
            var exportFolderFullPath = Path.Combine(baseExportFolderFullPath, exportType.ToString(), pathType.ToString());
            return exportFolderFullPath;
        }

        /// <summary>
        /// 获取指定导出文件全路径
        /// </summary>
        /// <param name="exportFileName"></param>
        /// <param name="exportType"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string GetExportFileFullPath(string exportFileName, TPathExportType exportType, TPathType pathType)
        {
            var exportFolderFullPath = GetExportFolderFullPath(exportType, pathType);
            var exportFileNameWithExtension = GetExportFileName(exportFileName, exportType);
            return Path.Combine(exportFolderFullPath, exportFileNameWithExtension);
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
            else if(pathwayType == TPathwayType.CRSpline)
            {
                return 4;
            }
            else
            {
                Debug.LogError($"不支持的路线类型:{pathwayType.ToString()}，获取路线分段顶点数量失败！");
                return 0;
            }
        }

        /// <summary>
        /// 获取指定路线类型的分段顶点步长
        /// </summary>
        /// <param name="pathwayType"></param>
        /// <returns></returns>
        public static int GetSegmentStepNumByType(TPathwayType pathwayType)
        {
            if (pathwayType == TPathwayType.Liner)
            {
                return 1;
            }
            else if (pathwayType == TPathwayType.Bezier)
            {
                return 2;
            }
            else if (pathwayType == TPathwayType.CubicBezier)
            {
                return 3;
            }
            else if (pathwayType == TPathwayType.CRSpline)
            {
                return 1;
            }
            else
            {
                Debug.LogError($"不支持的路线类型:{pathwayType.ToString()}，获取路线分段步长失败！");
                return 0;
            }
        }

        /// <summary>
        /// 获取指定路点类型的绘制Icon
        /// </summary>
        /// <param name="pathPointType"></param>
        /// <returns></returns>
        public static string GetDrawIconByPathPointType(TPathPointType pathPointType)
        {
            var drawIcon = DEFAULT_DRAW_ICON;
            if(!mPathTypeDrawIconMap.TryGetValue(pathPointType, out drawIcon))
            {
                Debug.LogError($"找不到路点类型:{pathPointType}的绘制Icon！");
            }
            return drawIcon;
        }

        /// <summary>
        /// 加载指定导出文件名，导出类型和路点路线类型的TPathDataExport
        /// </summary>
        /// <param name="exportFileName"></param>
        /// <param name="exportType"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static TPathDataExport LoadTPathDataExport(string exportFileName, TPathExportType exportType = TPathExportType.Json,
                                                          TPathType pathType = TPathType.Normal)
        {
            // 不同的资源加载方式请自行封装
            var exportFileFullPath = GetExportFileFullPath(exportFileName, exportType, pathType);
            var exportAssetFileRelativePath = PathUtilities.GetAssetsRelativeFolderPath(exportFileFullPath);
            var pathDataExportAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(exportAssetFileRelativePath);
            var pathDataExport = JsonUtility.FromJson<TPathDataExport>(pathDataExportAsset.text);
            if(pathDataExport == null)
            {
                Debug.LogError($"加载导出文件:{exportAssetFileRelativePath}失败，无法获取有效的TPathDataExport！");
                return null;
            }
            return pathDataExport;
        }
    }
}