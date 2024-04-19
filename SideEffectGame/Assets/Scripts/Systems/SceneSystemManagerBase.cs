using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSystemManagerBase : MonoBehaviour
{
    static protected Dictionary<Type, SceneSystemManagerBase> _sceneSystems = new Dictionary<Type, SceneSystemManagerBase>();

    static public T GetSingleton<T>() where T : SceneSystemManagerBase, new()
    {
        Type templateType = typeof(T);
        if (!_sceneSystems.ContainsKey(templateType))
        {
            _sceneSystems.Add(templateType, new T());
        }
        return _sceneSystems[templateType] as T;
    }

    static public void RegisterSceneSystemManager(SceneSystemManagerBase Value)
    {
        Type templateType = Value.GetType();
        _sceneSystems.Add(templateType, Value);
    }

    static public void UnregisterSceneSystemManager(SceneSystemManagerBase Value)
    {
        Type templateType = Value.GetType();
        _sceneSystems.Remove(templateType);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void Awake()
    {
        SceneSystemManagerBase.RegisterSceneSystemManager(this);
    }

    protected void OnDestroy()
    {
        SceneSystemManagerBase.UnregisterSceneSystemManager(this);
    }
}
