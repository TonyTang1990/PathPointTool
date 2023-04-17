﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 只允许挂载在GameLauncher上的单例模板MonoBehaviour类
/// 通过抽象单例模板MonoBehaviour的初始化和释放方法
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMonoBehaviourTemplate<T> : MonoBehaviour where T : SingletonMonoBehaviourTemplate<T>
{
    private static T mInstance = null;

    public static T GetInstance()
    {
        return mInstance;
    }

    public void SetInstance(T t)
    {
        if(mInstance == null)
        {
            mInstance = t;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Init()
    {
        return;
    }

    /// <summary>
    /// 移除释放
    /// </summary>
    public virtual void Release()
    {
        return;
    }
}
