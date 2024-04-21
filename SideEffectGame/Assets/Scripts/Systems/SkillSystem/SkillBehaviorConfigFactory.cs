using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 效果算法接口
/// </summary>
public interface IImpactEffect
{
    /// <summary>
    /// 处理效果
    /// </summary>
    /// <param name="skillEntity">技能实体</param>
    void Execute(SkillEntityComponentBase skillEntity);
}

/// <summary>
/// 范围选择算法接口
/// </summary>
public interface ISkillSelector
{
    static public LayerMask GetLayerMaskFromStrings(string[] strings, bool allForEmptyStrings = true)
    {
        return (allForEmptyStrings && strings.Length == 0) ? Physics.AllLayers : LayerMask.GetMask(strings);
    }
    
    /// <summary>
    /// 从GameObjects列表中获取SkillSelectResult[]
    /// </summary>
    /// <param name="GameObjects">技能选择的目标物体</param>
    /// <param name="hitPoints">技能命中的位置（可选）</param>
    /// <returns></returns>
    static protected SkillSelectResult[] SelectFromGameObjects(
        IEnumerable<GameObject> GameObjects, 
        Dictionary<GameObject, Vector3> hitPoints = null)
    {
        List<SkillSelectResult> targets = new List<SkillSelectResult>();
        foreach (GameObject go in GameObjects)
        {
            Vector3 position = hitPoints?[go] ?? go.transform.position;
            targets.Add(new SkillSelectResult
            {
                position = position,
                target = go
            });
        }
        return targets.ToArray();
    }

    /// <summary>
    /// 选择目标
    /// </summary>
    /// <param name="data">技能运行时数据</param>
    /// <param name="skillTransform">技能实体</param>
    /// <returns></returns>
    SkillSelectResult[] SelectTarget(SkillRuntimeData data, GameObject skillEntityObject);
}

public interface IContactSkillContext
{
    public Collider[] colliders { set; get; }
    public Collision[] collisions { set; get; }
}

/// <summary>
/// 技能行为实例工厂，用反射来实现
/// </summary>
public class SkillBehaviorConfigFactory 
{
    /// <summary>
    /// 范围选择算法
    /// </summary>
    /// <param name="data">技能运行时数据</param>
    /// <returns></returns>
    public static ISkillSelector CreateSkillSelector(SkillRuntimeData data)
    {
        if (data.basicConfig.selectorType == SelectorType.None)
        {
            return null;
        }
        string className = string.Format("{0}SkillSelector", data.basicConfig.selectorType);
        return CreateObject<ISkillSelector>(className);
    }

    /// <summary>
    /// 效果算法
    /// </summary>
    /// <param name="data">技能运行时数据</param>
    /// <returns></returns>
    public static IImpactEffect[] CreateImpactEffects(SkillRuntimeData data)
    {
        IImpactEffect[] impacts = new IImpactEffect[data.basicConfig.impactType.Length];
        for (int i = 0; i < data.basicConfig.impactType.Length; i++)
        {
            string className = string.Format("{0}Impact", data.basicConfig.impactType[i]);
            IImpactEffect newEffect = CreateObject<IImpactEffect>(className);
            if (newEffect != null)
            {
                impacts[i] = newEffect;
            }
        }
        return impacts;
    }

    /// <summary>
    /// 创建接口实例
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    /// <param name="className">实例的类名</param>
    /// <returns></returns>
    private static T CreateObject<T>(string className) where T : class
    {
        Type type = Type.GetType(className);
        if (type == null)
        {
            return null;
        }
        object obj = Activator.CreateInstance(type);
        return obj as T;
    }
}