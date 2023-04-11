/*
 * Description:             PathPointSystemEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// PathPointSystemEditor.cs
    /// 路点系统组件Editor
    /// </summary>
    [CustomEditor(typeof(PathPointSystem))]
    public class PathPointSystemEditor : Editor
    {
        /// <summary>
        /// 目标组件
        /// </summary>
        private PathPointSystem mTarget;

        /// <summary>
        /// Type属性
        /// </summary>
        private SerializedProperty mTypeProperty;

        /// <summary>
        /// DrawType属性
        /// </summary>
        private SerializedProperty mDrawTypeProperty;

        /// <summary>
        /// PathPointStartPos属性
        /// </summary>
        private SerializedProperty mPathPointStartPosProperty;

        /// <summary>
        /// PathPointGap属性
        /// </summary>
        private SerializedProperty mPathPointGapProperty;

        /// <summary>
        /// DrawPathPointDistance属性
        /// </summary>
        private SerializedProperty mDrawPathPointDistanceProperty;

        /// <summary>
        /// PathPointSphereSize属性
        /// </summary>
        private SerializedProperty mPathPointSphereSizeProperty;

        /// <summary>
        /// PathPointSphereColor属性
        /// </summary>
        private SerializedProperty mPathPointSphereColorProperty;

        /// <summary>
        /// PathDrawColor属性
        /// </summary>
        private SerializedProperty mPathDrawColorProperty;

        /// <summary>
        /// PathPointDrawColor属性
        /// </summary>
        private SerializedProperty mPathPointDrawColorProperty;

        /// <summary>
        /// PathPointList属性
        /// </summary>
        private SerializedProperty mPathPointListProperty;

        /// <summary>
        /// 路径相关属性变化
        /// </summary>
        private bool mPathPointRelativePropertyChange;

        /// <summary>
        /// 绘制的路点列表
        /// </summary>
        private List<Vector3> mDrawPathPointList = new List<Vector3>();

        private void OnEnable()
        {
            InitTarget();
            InitProperties();
            UpdateDrawDatas();
        }

        /// <summary>
        /// 初始化目标组件
        /// </summary>
        private void InitTarget()
        {
            mTarget ??= (target as PathPointSystem);
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        private void InitProperties()
        {
            mTypeProperty ??= serializedObject.FindProperty("Type");
            mDrawTypeProperty ??= serializedObject.FindProperty("DrawType");
            mPathPointStartPosProperty ??= serializedObject.FindProperty("PathPointStartPos");
            mPathPointGapProperty ??= serializedObject.FindProperty("PathPointGap");
            mDrawPathPointDistanceProperty ??= serializedObject.FindProperty("DrawPathPointDistance");
            mPathPointSphereSizeProperty ??= serializedObject.FindProperty("PathPointSphereSize");
            mPathPointSphereColorProperty ??= serializedObject.FindProperty("PathPointSphereColor");
            mPathDrawColorProperty ??= serializedObject.FindProperty("PathDrawColor");
            mPathPointDrawColorProperty ??= serializedObject.FindProperty("PathPointDrawColor");
            mPathPointListProperty ??= serializedObject.FindProperty("PathPointList");
    }

        /// <summary>
        /// Inspector自定义显示
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 确保对SerializaedObject和SerializedProperty的数据修改每帧同步
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(mTypeProperty);
            EditorGUILayout.PropertyField(mPathPointStartPosProperty);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mPathPointGapProperty);
            if (EditorGUI.EndChangeCheck())
            {
                mPathPointRelativePropertyChange = true;
            }
            EditorGUILayout.PropertyField(mDrawPathPointDistanceProperty);
            EditorGUILayout.PropertyField(mPathPointSphereSizeProperty);
            EditorGUILayout.PropertyField(mPathPointSphereColorProperty);
            EditorGUILayout.PropertyField(mPathDrawColorProperty);
            EditorGUILayout.PropertyField(mPathPointDrawColorProperty);
            DrawPathPointListProperty();
            EditorGUILayout.EndVertical();

            // 确保对SerializedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();

            if (mPathPointRelativePropertyChange)
            {
                Debug.Log($"路点相关数据属性变化，更新相关数据！");
                mTarget.UpdatePathPointNames();
                UpdateDrawDatas();
            }
        }

        /// <summary>
        /// 绘制路点列表属性
        /// </summary>
        private void DrawPathPointListProperty()
        {
            EditorGUILayout.BeginVertical();
            var pathPointNum = mPathPointListProperty.arraySize;
            for (int i = pathPointNum - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                var pathPointProperty = mPathPointListProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(pathPointProperty);
                if(GUILayout.Button("+", GUILayout.Width(40f)))
                {
                    AddPathPointByIndex(i);
                    mPathPointRelativePropertyChange = true;
                }
                if (GUILayout.Button("-", GUILayout.Width(40f)))
                {
                    RemovePathPointByIndex(i);
                    mPathPointRelativePropertyChange = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            if(GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                AddPathPointByIndex(pathPointNum);
                mPathPointRelativePropertyChange = true;
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 指定位置索引添加路点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool AddPathPointByIndex(int index)
        {
            var pathPointNum = mPathPointListProperty.arraySize;
            if (index < 0 || index > pathPointNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{pathPointNum}，添加路点失败！");
                return false;
            }
            var newPathPoint = mTarget.ConstructNewPathPoint(index);
            mPathPointListProperty.InsertArrayElementAtIndex(index);
            var newPathPointProperty = mPathPointListProperty.GetArrayElementAtIndex(index);
            newPathPointProperty.objectReferenceValue = newPathPoint.transform;
            return true;
        }

        /// <summary>
        /// 移除指定索引的路点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RemovePathPointByIndex(int index)
        {
            var pathPointNum = mPathPointListProperty.arraySize;
            if (index < 0 || index >= pathPointNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{pathPointNum - 1}，移除路点失败！");
                return false;
            }
            mTarget.DestroyPathPointByIndex(index);
            mPathPointListProperty.DeleteArrayElementAtIndex(index);
            return true;
        }

        /// <summary>
        /// 更新绘制数据
        /// </summary>
        private void UpdateDrawDatas()
        {
            mDrawPathPointList.Clear();
            if(mDrawTypeProperty.intValue == (int)PathDrawType.Line)
            {
                for(int i = 0, length = mPathPointListProperty.arraySize; i < length; i++)
                {
                    var pathPointProperty = mPathPointListProperty.GetArrayElementAtIndex(i);
                    if(pathPointProperty.objectReferenceValue != null)
                    {
                        var pathPointTransform = pathPointProperty.objectReferenceValue as Transform;
                        mDrawPathPointList.Add(pathPointTransform.position);
                    }
                }
            }
            else if(mDrawTypeProperty.intValue == (int)PathDrawType.Bezier)
            {
                var drawPathPointDistance = mDrawPathPointDistanceProperty.floatValue;
                var maxPathPointIndex = Mathf.Clamp(mPathPointListProperty.arraySize - 1, 0 , int.MaxValue);
                for (int i = 0, length = mPathPointListProperty.arraySize; i < length; i+=2)
                {
                    var firstIndex = i;
                    var secondIndex = Mathf.Clamp(i + 1, 0, maxPathPointIndex);
                    var thirdIndex = Mathf.Clamp(i + 2, 0, maxPathPointIndex);
                    var firstProperty = mPathPointListProperty.GetArrayElementAtIndex(firstIndex);
                    var secondProperty = mPathPointListProperty.GetArrayElementAtIndex(secondIndex);
                    var thirdProperty = mPathPointListProperty.GetArrayElementAtIndex(thirdIndex);
                    var firstTransform = firstProperty.objectReferenceValue as Transform;
                    var secondTransform = secondProperty.objectReferenceValue as Transform;
                    var thirdTransform = thirdProperty.objectReferenceValue as Transform;

                    //mDrawPathPointList.Add(pathPointTransform.position);
                }
            }
            else if(mDrawTypeProperty.intValue == (int)PathDrawType.ThreeBezier)
            {

            }
        }

        /// <summary>
        /// 选中场景绘制
        /// </summary>
        private void OnSceneGUI()
        {
            DrawPathPointLines();
        }

        /// <summary>
        /// 绘制路点连线
        /// </summary>
        private void DrawPathPointLines()
        {

        }
    }
}