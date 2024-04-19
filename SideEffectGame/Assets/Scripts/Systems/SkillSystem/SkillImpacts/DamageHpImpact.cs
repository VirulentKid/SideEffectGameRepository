using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHpImpact : IImpactEffect
{
    protected SkillRuntimeData data;
    public void Execute(SkillEntityComponentBase entity)
    {
        data = entity.skillData;
        if (data != null)
        {
            GameObject damageSource = data.generatedData.owner;
            float attackDamage = data.basicConfig.damageHp;
            foreach (SkillSelectResult skillSelectResult in data.generatedData.skillSelectResults)
            {
                if (skillSelectResult.target != null)
                {
                    ReceiveDamageComponent T;
                    Transform targetTransform = skillSelectResult.target.transform;
                    while (targetTransform != null)
                    {
                        if (targetTransform.TryGetComponent(out T))
                        {
                            T.ReceiveDamage(damageSource, attackDamage);
                            break;
                        }
                        targetTransform = targetTransform.parent;
                    }
                }
            }
        }
    }
}
