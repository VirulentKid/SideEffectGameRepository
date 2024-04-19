using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorSkillSelector : ISkillSelector
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
        Transform skillTransform = skillEntityObject.transform;
        List<GameObject> gameObjectsWithTags = new List<GameObject>();
        GameObject[] tempGOArray;
        for (int i = 0; i < data.basicConfig.attackTargetTags.Length; ++i)
        {
            tempGOArray = GameObject.FindGameObjectsWithTag(data.basicConfig.attackTargetTags[i]);
            gameObjectsWithTags.AddRange(tempGOArray);
        }
        LayerMask layerMask = ISkillSelector.GetLayerMaskFromStrings(data.basicConfig.attackTargetLayers);
        Predicate<GameObject> FindPredicate = go =>
            ((layerMask | go.layer) != 0) &&
            Vector3.Distance(go.transform.position, skillTransform.position) <= data.basicConfig.attackDistance &&
            Vector3.Angle(skillTransform.forward, go.transform.position - skillTransform.position) <= data.basicConfig.attackAngle / 2;
        //判断攻击范围
        switch (attackType)
        {
            case SkillAttackType.AOE:
                gameObjectsWithTags = gameObjectsWithTags.FindAll(FindPredicate);
                break;
            case SkillAttackType.Single:
                {
                    float minDistance = -1;
                    GameObject nearestGameObject = null;
                    foreach (GameObject go in gameObjectsWithTags)
                    {
                        if (FindPredicate(go))
                        {
                            float curDistance = Vector3.Distance(go.transform.position, skillTransform.position);
                            if (nearestGameObject == null || minDistance > curDistance)
                            {
                                minDistance = curDistance;
                                nearestGameObject = go;
                            }
                        }
                    }
                    gameObjectsWithTags.Clear();
                    gameObjectsWithTags.Add(nearestGameObject);
                }
                break;
            default:
                gameObjectsWithTags.Clear();
                break;
        }

        return ISkillSelector.SelectFromGameObjects(gameObjectsWithTags);
    }
}
