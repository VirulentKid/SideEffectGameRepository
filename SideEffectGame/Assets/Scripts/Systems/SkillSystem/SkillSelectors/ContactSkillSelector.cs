using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 通过实体碰撞接触选择
/// 在碰撞检测或接触检测后给colliders或collders后进行目标选择
/// </summary>
public class ContactSkillSelector : ISkillSelector, IContactSkillContext
{
    public Collider[] colliders { set; get; }
    public Collision[] collisions { set; get; }

    public SkillSelectResult[] SelectTarget(SkillRuntimeData data, GameObject skillEntityObject)
    {

        SkillAttackType attackType = data.basicConfig.attackType;

        // 处理Self
        if (attackType == SkillAttackType.Self)
        {
            return ISkillSelector.SelectFromGameObjects(new GameObject[] { skillEntityObject });
        }

        int objectLength = 0;
        // 没有检测到接触目标时，目标为空
        if (colliders != null && colliders.Length > 0)
        {
            objectLength = colliders.Length;
        }
        if (collisions != null && collisions.Length > 0)
        {
            objectLength = collisions.Length;
        }

        if (objectLength == 0)
        {
            return new SkillSelectResult[0];
        }


        //根据技能数据中得标签 获取所有目标
        LayerMask layerMask = ISkillSelector.GetLayerMaskFromStrings(data.basicConfig.attackTargetLayers);
        Transform skillTransform = skillEntityObject.transform;
        List<GameObject> gameObjectsWithTags = new List<GameObject>();
        Dictionary<GameObject, Vector3> hitPoints = new Dictionary<GameObject, Vector3>();

        for (int i = 0; i < objectLength; ++i)
        {
            GameObject targetObject = null;
            if (colliders != null && colliders.Length > i)
            {
                targetObject = colliders[i].gameObject;
            }
            if (collisions != null && collisions.Length > i)
            {
                targetObject = collisions[i].gameObject;
            }

            if (data.basicConfig.attackTargetTags.Length == 0 || data.basicConfig.attackTargetTags.Contains(targetObject.tag))
            {
                gameObjectsWithTags.Add(targetObject);

                Vector3 targetPoint = Vector3.zero;
                if (colliders != null && colliders.Length > i)
                {
                    targetPoint = colliders[i].ClosestPoint(skillEntityObject.transform.position);
                }
                if (collisions != null && collisions.Length > i)
                {
                    ContactPoint firstContactPoint = collisions[i].GetContact(0);
                    targetPoint = firstContactPoint.point;
                }

                hitPoints.Add(targetObject, targetPoint);
            }
        }

        return ISkillSelector.SelectFromGameObjects(gameObjectsWithTags, hitPoints);
    }
}
