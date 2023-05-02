/*
 * Description:             TPathTweenerManager.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/16
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathTweenerManager.cs
    /// 路线缓动管理单例类
    /// </summary>
    [DisallowMultipleComponent]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    public class TPathTweenerManager : MonoBehaviour
    {
        /// <summary>
        /// 单例对象
        /// </summary>
        public static TPathTweenerManager Singleton
        {
            get
            {
                if(mSingleton == null)
                {
                    Init();
                    Debug.Log($"未初始化路线PathTweenerManager,自动调用PathTweenerManager.Init()方法初始化！");
                }
                return mSingleton;
            }
        }
        private static TPathTweenerManager mSingleton;

        /// <summary>
        /// 路线缓动对象列表
        /// </summary>
        private List<TPathTweener> mTPathTweenerList;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if(mSingleton != null)
            {
                Debug.LogWarning($"请勿反复初始化TPathTweenerManager，当前已经初始化完成！");
                return;
            }
            var tpathTweenerManagerGO = new GameObject("TPathTweenerManager");
            tpathTweenerManagerGO.AddComponent<TPathTweenerManager>();
        }

#if UNITY_EDITOR
        static TPathTweenerManager()
        {
            Debug.Log($"static PathTweenerManager.PathTweenerManager()");
            UnregisterEditorUpdate();
        }

        [InitializeOnLoadMethod]
        private static void OnInitializedOnLoadMethod()
        {
            Debug.Log($"static TPathTweenerManager.OnInitializedOnLoadMethod()");
            var pathTweenerManager = GameObject.FindObjectOfType<TPathTweenerManager>();
            if (pathTweenerManager != null)
            {
                Debug.Log($"找回场景里TPathTweenerManager单例组件对象！");
                mSingleton = pathTweenerManager;
                mSingleton.MembersInit();
                UnregisterEditorUpdate();
                RegisterEditorUpdate();
            }
        }

        /// <summary>
        /// 注入Editor Update
        /// </summary>
        private static void RegisterEditorUpdate()
        {
            EditorApplication.update += EditorUpdate;
            Debug.Log($"TPathTweenerManager注入EditorApplication.update");
        }

        /// <summary>
        /// 取消Editor Update注入
        /// </summary>
        private static void UnregisterEditorUpdate()
        {
            EditorApplication.update -= EditorUpdate;
            Debug.Log($"TPathTweenerManager取消注入EditorApplication.update");
        }

        /// <summary>
        /// Editor更新
        /// </summary>
        private static void EditorUpdate()
        {
            mSingleton?.Update();
        }
#endif

        private void Awake()
        {
            if (mSingleton != null)
            {
                Debug.LogError($"不允许挂载多个TPathTweenerManager脚本，自动删除！");
                DestroyImmediate(this);
                return;
            }
            Debug.Log($"PathTweenerManager模块初始化！");
            mSingleton = this;
            if(Application.isPlaying)
            {
                DontDestroyOnLoad(this);
            }
            MembersInit();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnregisterEditorUpdate();
                RegisterEditorUpdate();
            }
#endif
        }

        /// <summary>
        /// 响应销毁
        /// </summary>
        private void OnDestroy()
        {
            if(mSingleton == this)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnregisterEditorUpdate();
                }
#endif
                RemoveAllPathTweens();
                mSingleton = null;
            }
        }

        /// <summary>
        /// 成员初始化
        /// </summary>
        private void MembersInit()
        {
            mTPathTweenerList = new List<TPathTweener>();
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            var deltaTime = Time.deltaTime;
            for (int i = mTPathTweenerList.Count - 1; i >= 0; i--)
            {
                mTPathTweenerList[i].Update(deltaTime);
            }
        }

        /// <summary>
        /// 指定路点列表路线移动
        /// </summary>
        /// <param name="target"></param>
        /// <param name="points"></param>
        /// <param name="duration"></param>
        /// <param name="isLoop"></param>
        /// <param name="updateFoward"></param>
        /// <param name="completeCB"></param>
        /// <param name="pathwayType"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public TPathTweener DoPathTweenByPoints(Transform target, IEnumerable<Vector3> points, float duration,
                                                bool isLoop = false, bool updateFoward = false, Action completeCB = null,
                                                TPathwayType pathwayType = TPathwayType.Liner,
                                                EasingFunction.Ease ease = EasingFunction.Ease.Linear, int segment = 10)
        {
            if (target == null)
            {
                Debug.LogError($"不允许对空对象进行指定路点列表移动！");
                return null;
            }
            var tpathTweener = ObjectPool.Singleton.pop<TPathTweener>();
            tpathTweener.InitByPoints(target, points, duration, pathwayType, isLoop, updateFoward, completeCB, ease, segment);
            AddTPathTPathTweener(tpathTweener);
            return tpathTweener;
        }

        /// <summary>
        /// 指定移动对象列表路线移动
        /// </summary>
        /// <param name="target"></param>
        /// <param name="transforms"></param>
        /// <param name="duration"></param>
        /// <param name="isLoop"></param>
        /// <param name="updateFoward"></param>
        /// <param name="completeCB"></param>
        /// <param name="pathwayType"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public TPathTweener DoPathTweenByTransforms(Transform target, IEnumerable<Transform> transforms, float duration,
                                                    bool isLoop = false, bool updateFoward = false, Action completeCB = null,
                                                    TPathwayType pathwayType = TPathwayType.Liner,
                                                    EasingFunction.Ease ease = EasingFunction.Ease.Linear, int segment = 10)
        {
            if (target == null)
            {
                Debug.LogError($"不允许对空对象进行指定对象列表移动！");
                return null;
            }
            var tpathTweener = ObjectPool.Singleton.pop<TPathTweener>();
            tpathTweener.InitByTransforms(target, transforms, duration, pathwayType, isLoop, updateFoward, completeCB, ease, segment);
            AddTPathTPathTweener(tpathTweener);
            return tpathTweener;
        }

        /// <summary>
        /// 添加指定路点移动对象
        /// </summary>
        /// <param name="tpathTweener"></param>
        /// <returns></returns>
        private bool AddTPathTPathTweener(TPathTweener tpathTweener)
        {
            if(tpathTweener == null)
            {
                Debug.LogError($"不允许添加空的路点缓动对象，添加失败！");
                return false;
            }
            int tpathTweenerIndex;
            if(IsExistTPathTweenerByUID(tpathTweener.UID, out tpathTweenerIndex))
            {
                Debug.LogError($"不允许重复添加相同路点缓动UID:{tpathTweener.UID}的对象，添加失败！");
                return false;
            }
            mTPathTweenerList.Add(tpathTweener);
            return true;
        }

        /// <summary>
        /// 移除指定UID的路线缓动对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool RemovePathTweenByUID(int uid)
        {
            int tpathTweenerIndex;
            if (IsExistTPathTweenerByUID(uid, out tpathTweenerIndex))
            {
                ObjectPool.Singleton.push<TPathTweener>(mTPathTweenerList[tpathTweenerIndex]);
                mTPathTweenerList.RemoveAt(tpathTweenerIndex);
                return true;
            }
            Debug.LogError($"找不到路径缓动UID:{uid}的缓动对象，移除指定路线缓动UID失败！");
            return false;
        }

        /// <summary>
        /// 移除指定路线缓动对象
        /// </summary>
        /// <param name="tpathTweener"></param>
        /// <returns></returns>
        public bool RemovePathTween(TPathTweener tpathTweener)
        {
            if(tpathTweener == null)
            {
                Debug.LogError($"不允许移除空路线缓动对象！");
                return false;
            }
            int tpathTweenerIndex;
            if(IsExistTPathTweenerByUID(tpathTweener.UID, out tpathTweenerIndex))
            {
                ObjectPool.Singleton.push<TPathTweener>(mTPathTweenerList[tpathTweenerIndex]);
                mTPathTweenerList.RemoveAt(tpathTweenerIndex);
                return true;
            }
            Debug.LogError($"找不到路径缓动UID:{tpathTweener.UID}的缓动对象，移除路线缓动失败！");
            return false;
        }

        /// <summary>
        /// 移除所有缓动路线
        /// </summary>
        private void RemoveAllPathTweens()
        {
            for (int i = mTPathTweenerList.Count - 1; i >= 0; i++)
            {
                ObjectPool.Singleton.push<TPathTweener>(mTPathTweenerList[i]);
            }
            mTPathTweenerList.Clear();
        }

        /// <summary>
        /// 是否存在指定TPathTweener UID对象
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsExistTPathTweenerByUID(int uid, out int index)
        {
            index = 0;
            for (int i = mTPathTweenerList.Count - 1; i >= 0; i++)
            {
                if (mTPathTweenerList[i].UID == uid)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }
}