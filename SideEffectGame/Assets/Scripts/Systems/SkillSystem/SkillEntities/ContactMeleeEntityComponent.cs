using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// 物理接触型平A技能实体
/// </summary>
public class ContactMeleeEntityComponent : SkillEntityComponentBase
{
    /// <summary>
    /// 技能实体拥有的碰撞体，用于判断是否击中目标
    /// </summary>
    [SerializeField]
    private Collider _colliderComponent = null;

    // Start is called before the first frame update
    void Start()
    {
        _colliderComponent = GetComponent<Collider>();
        _colliderComponent.enabled = false;
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
        if (_colliderComponent != null)
        {
            _colliderComponent.enabled = false;
        }
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
        if (_colliderComponent != null)
        {
            _colliderComponent.enabled = true;
        }
        ////执行选区算法
        //PutTargetsIntoSkillData();
        ////执行影响算法
        //ImpactTargets();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            //执行选区算法
            CollideTargets(new Collision[] { collision });
            //执行影响算法
            ImpactTargets();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 暂时不用
    }
}
