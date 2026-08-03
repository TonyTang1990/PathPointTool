/*
 * Description:             GameLaucher.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/18
 */

using PathPoint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameLaucher.cs
/// 游戏启动测试
/// </summary>
public class GameLaucher : MonoBehaviour
{
    /// <summary>
    /// 加载PathData1路径按钮
    /// </summary>
    [Header("加载PathData1路径按钮")]
    public Button BtnLoadPathData1;

    /// <summary>
    /// 加载PathData2路径按钮
    /// </summary>
    [Header("加载PathData2路径按钮")]
    public Button BtnLoadPathData2;

    /// <summary>
    /// 开始路线缓动按钮
    /// </summary>
    [Header("开始路线缓动按钮")]
    public Button BtnStartPathMove;

    /// <summary>
    /// 暂停路线缓动按钮
    /// </summary>
    [Header("暂停路线缓动按钮")]
    public Button BtnPausePathMove;

    /// <summary>
    /// 继续路线缓动按钮
    /// </summary>
    [Header("继续路线缓动按钮")]
    public Button BtnResumePathMove;

    /// <summary>
    /// 路线缓动对象
    /// </summary>
    [Header("路线缓动对象")]
    public GameObject PathMoveGo;
    
    /// <summary>
    /// 已加载导出的路点数据
    /// </summary>
    private TPathDataExport mLoadedPathDataExport;

    /// <summary>
    /// 运行时路线缓动Tweener
    /// </summary>
    private TPathTweener mPathTweener;

    private void Awake()
    {
        BtnLoadPathData1.onClick.AddListener(OnBtnLoadPathData1);
        BtnLoadPathData2.onClick.AddListener(OnBtnLoadPathData2);
        BtnStartPathMove.onClick.AddListener(OnBtnStartPathMove);
        BtnPausePathMove.onClick.AddListener(OnBtnPausePathMove);
        BtnResumePathMove.onClick.AddListener(OnBtnResumePathMove);
    }

    /// <summary>
    /// 响应加载PathData1路径按钮点击
    /// </summary>
    private void OnBtnLoadPathData1()
    {
        Debug.Log($"GameLaucher:OnBtnLoadPathData1()");
        mLoadedPathDataExport = TPathUtilities.LoadTPathDataExport("PathData1", TPathExportType.Json, TPathType.Normal);
    }
    
    /// <summary>
    /// 响应加载PathData2路径按钮点击
    /// </summary>
    private void OnBtnLoadPathData2()
    {
        Debug.Log($"GameLaucher:OnBtnLoadPathData2()");
        mLoadedPathDataExport = TPathUtilities.LoadTPathDataExport("PathData2", TPathExportType.Json, TPathType.Normal);
    }

    /// <summary>
    /// 相应开始路线移动按钮点击
    /// </summary>
    private void OnBtnStartPathMove()
    {
        Debug.Log($"GameLaucher:OnBtnStartPathMove()");
        if(mPathTweener != null)
        {
            TPathTweenerManager.Singleton.RemovePathTween(mPathTweener);
            mPathTweener = null;
        }
        if(mLoadedPathDataExport == null)
        {
            Debug.LogError($"没有加载有效的路点数据，无法开始路线缓动！");
            return;
        }
        mPathTweener = TPathTweenerManager.Singleton.DoPathTweenByTPathDataExport(PathMoveGo.transform, mLoadedPathDataExport,
                                                                                 () =>
                                                                                 {
                                                                                     Debug.Log($"运行时路线缓动完成！");
                                                                                     mPathTweener = null;
                                                                                 },
                                                                                 (index) =>
                                                                                 {
                                                                                     Debug.Log($"经过路点:{index}");
                                                                                     var pathPointData = mLoadedPathDataExport.GetPathPointData(index);
                                                                                     if(pathPointData != null)
                                                                                     {
                                                                                         Debug.Log($"路点数据:{pathPointData.ToString()}");
                                                                                     }
                                                                                 },
                                                                                 () =>
                                                                                 {
                                                                                     Debug.Log($"路线缓动循环开始！");
                                                                                 });
    }

    /// <summary>
    /// 相应开始路线移动按钮点击
    /// </summary>
    private void OnBtnPausePathMove()
    {
        Debug.Log($"GameLaucher:OnBtnPausePathMove()");
        if(mPathTweener == null)
        {
            Debug.Log($"没有开启路线缓动，暂停路线缓动失败！");
            return;
        }
        mPathTweener.Pause();
    }

    /// <summary>
    /// 相应开始路线移动按钮点击
    /// </summary>
    private void OnBtnResumePathMove()
    {
        Debug.Log($"GameLaucher:OnBtnResumePathMove()");
        if (mPathTweener == null)
        {
            Debug.Log($"没有开启路线缓动，继续路线缓动失败！");
            return;
        }
        mPathTweener.Resume();
    }
}