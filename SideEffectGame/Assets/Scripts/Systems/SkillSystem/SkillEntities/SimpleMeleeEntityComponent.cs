using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// 瞬时判定型平A技能实体
/// </summary>
public class SimpleMeleeEntityComponent : SkillEntityComponentBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    protected override void OnSkillReleased()
    {
    }

    protected override void OnSkillHitsTarget(SkillSelectResult skillSelectResult)
    {
        // 大部分都通过ImpactTargets实现
    }

    protected override void OnSkillEnds()
    {
        if (skillData.postLoadSettings.skillSourcePrefab != null)
        {
            // 技能实体是生成的，需要结束生命周期
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 前摇结束时释放技能
    /// </summary>
    protected override void OnWarmUpEnds()
    {
        //执行选区算法
        PutTargetsIntoSkillData();
        //执行影响算法
        ImpactTargets();
    }
}
