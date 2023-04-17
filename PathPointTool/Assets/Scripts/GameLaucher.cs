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
    /// 测试运行时路线缓动
    /// </summary>
    private List<Vector3> mPointPosList = new List<Vector3>()
    {
        new Vector3(20, 0, 0),
        new Vector3(20, 0, 2),
        new Vector3(22, 0, 2),
        new Vector3(24, 0, 2),
        new Vector3(24, 0, 4),
        new Vector3(26, 0, 4),
        new Vector3(26, 0, 6),
        new Vector3(28, 0, 6),
        new Vector3(28, 0, 4),
    };

    /// <summary>
    /// 运行时路线缓动Tweener
    /// </summary>
    private TPathTweener mPathTweener;

    private void Awake()
    {
        BtnStartPathMove.onClick.AddListener(OnBtnStartPathMove);
        BtnPausePathMove.onClick.AddListener(OnBtnPausePathMove);
        BtnResumePathMove.onClick.AddListener(OnBtnResumePathMove);
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
        mPathTweener = TPathTweenerManager.Singleton.DoPathTweenByPoints(PathMoveGo.transform, mPointPosList,
                                                                         10, false, false, () =>
                                                                         {
                                                                             Debug.Log($"运行时路线缓动完成！");
                                                                             mPathTweener = null;
                                                                         }, TPathwayType.Bezier);
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