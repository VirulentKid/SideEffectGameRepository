using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayCastSkillSelector : ISkillSelector
{
    public SkillSelectResult[] SelectTarget(SkillRuntimeData data, GameObject skillEntityObject)
    {
        SkillAttackType attackType = data.basicConfig.attackType;

        // 处理Self
        if (attackType == SkillAttackType.Self)
        {
            return ISkillSelector.SelectFromGameObjects(new GameObject[] { skillEntityObject });
        }

        //根据技能数据中得标签 获取所有目标
        LayerMask layerMask = ISkillSelector.GetLayerMaskFromStrings(data.basicConfig.attackTargetLayers);
        Transform skillTransform = skillEntityObject.transform;
        List<GameObject> gameObjectsWithTags = new List<GameObject>();
        Dictionary<GameObject, Vector3> hitPoints = new Dictionary<GameObject, Vector3>();
        switch (attackType)
        {
            case SkillAttackType.AOE:
                {
                    RaycastHit[] hits = Physics.RaycastAll(skillTransform.position, skillTransform.forward, data.basicConfig.attackDistance, layerMask);
                    foreach (RaycastHit hit in hits)
                    {
                        if (data.basicConfig.attackTargetTags.Length == 0 || data.basicConfig.attackTargetTags.Contains(hit.transform.gameObject.tag))
                        {
                            gameObjectsWithTags.Add(hit.transform.gameObject);
                            hitPoints.Add(hit.transform.gameObject, hit.point);
                        }
                    }
                }
                break;
            case SkillAttackType.Single:
                {
                    if (Physics.Raycast(skillTransform.position, skillTransform.forward, out RaycastHit hit, data.basicConfig.attackDistance, layerMask))
                    {
                        if (data.basicConfig.attackTargetTags.Length == 0 || data.basicConfig.attackTargetTags.Contains(hit.transform.gameObject.tag))
                        {
                            gameObjectsWithTags.Add(hit.transform.gameObject);
                            hitPoints.Add(hit.transform.gameObject, hit.point);
                        }
                    }
                }
                break;
        }

        return ISkillSelector.SelectFromGameObjects(gameObjectsWithTags, hitPoints);
    }
}
