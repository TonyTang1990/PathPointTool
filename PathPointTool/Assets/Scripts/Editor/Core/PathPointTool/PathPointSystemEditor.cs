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
        /// PathPointStartPos属性
        /// </summary>
        private SerializedProperty mPathPointStartPosProperty;

        /// <summary>
        /// PathPointGap属性
        /// </summary>
        private SerializedProperty mPathPointGapProperty;

        /// <summary>
        /// PathPointSphereColor属性
        /// </summary>
        private SerializedProperty mPathPointSphereColorProperty;

        /// <summary>
        /// PathDrawColor属性
        /// </summary>
        private SerializedProperty mPathDrawColorProperty;

        /// <summary>
        /// PathPointList属性
        /// </summary>
        private SerializedProperty mPathPointListProperty;

        /// <summary>
        /// 路径相关属性变化
        /// </summary>
        private bool mPathPointRelativePropertyChange;

        private void OnEnable()
        {
            InitTarget();
            InitProperties();
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
            mPathPointStartPosProperty ??= serializedObject.FindProperty("PathPointStartPos");
            mPathPointGapProperty ??= serializedObject.FindProperty("PathPointGap");
            mPathPointSphereColorProperty ??= serializedObject.FindProperty("PathPointSphereColor");
            mPathDrawColorProperty ??= serializedObject.FindProperty("PathDrawColor");
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
            EditorGUILayout.PropertyField(mPathPointSphereColorProperty);
            EditorGUILayout.PropertyField(mPathDrawColorProperty);
            DrawPathPointListProperty();
            EditorGUILayout.EndVertical();

            // 确保对SerializedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();

            if (mPathPointRelativePropertyChange)
            {

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
                    mTarget?.AddPathPointByIndex(i);
                    mPathPointRelativePropertyChange = true;
                }
                if (GUILayout.Button("-", GUILayout.Width(40f)))
                {
                    mTarget?.AddPathPointByIndex(i);
                    mPathPointRelativePropertyChange = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            if(GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                mTarget?.AddPathPointByIndex(pathPointNum);
                mPathPointRelativePropertyChange = true;
            }
            EditorGUILayout.EndVertical();
        }
    }
}