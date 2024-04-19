using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 角色挂载的技能生成器
/// </summary>
public class SkillGeneratorComponent : MonoBehaviour
{
    [SerializeField]
    protected SkillGenerateSettings[] _skillGenerateSettings;

    [SerializeField]
    private SkillRuntimeData[] _skillRuntimeDatasDebug;

    protected Dictionary<int, SkillRuntimeData> _generatedSkillRuntimeData = new Dictionary<int, SkillRuntimeData>();

    // Start is called before the first frame update
    void Start()
    {
        InitSkillGenerateSettings();
        InitSkills();
    }

    // Update is called once per frame
    void Update()
    {
        _skillRuntimeDatasDebug = _generatedSkillRuntimeData.Values.ToArray<SkillRuntimeData>();
    }

    protected void InitSkillGenerateSettings()
    {
        if (_skillGenerateSettings?.Length > 0)
        {
            for (int i = 0; i < _skillGenerateSettings.Length; ++i)
            {
                // 允许releaseSkillAudioSourceObject为空
                //if (_skillGenerateSettings[i].animationObject == null)
                //{
                //    _skillGenerateSettings[i].animationObject = gameObject;
                //}
                if (_skillGenerateSettings[i].skillSource == null)
                {
                    _skillGenerateSettings[i].skillSource = gameObject;
                }
                // 允许releaseSkillAudioSourceObject为空
                //if (_skillGenerateSettings[i].releaseSkillAudioSourceObject == null)
                //{
                //    _skillGenerateSettings[i].releaseSkillAudioSourceObject = gameObject;
                //}
            }
        }
    }

    protected void InitSkills()
    {
        SkillDataManager manager = SkillDataManager.Singleton;
        if (manager != null)
        {
            foreach (SkillGenerateSettings generateSettings in _skillGenerateSettings)
            {
                int id = generateSettings.ownedSkillId;
                if (manager.FindSkill(id, out SkillStaticSettings settings))
                {
                    SkillRuntimeData data = new SkillRuntimeData(settings);
                    InitSkill(data, generateSettings);
                }

            }
        }
    }

    /// <summary>
    /// 初始化该角色拥有的技能
    /// </summary>
    /// <param name="data"></param>
    /// <param name="generateSettings">aa</param>
    protected void InitSkill(SkillRuntimeData data, SkillGenerateSettings generateSettings)
    {
        GameObject skillGameObjectTransformRefObject = generateSettings.skillSource;
        data.generateSettings = generateSettings;
        data.generatedData.owner = skillGameObjectTransformRefObject;
        _generatedSkillRuntimeData.Add(data.basicConfig.skillId, data);
    }

    /// <summary>
    /// 技能释放条件判断
    /// </summary>
    /// <param name="id">技能ID</param>
    /// <returns></returns>
    public SkillRuntimeData PrepareSkill(int id)
    {
        SkillRuntimeData data = null;
        foreach (var s in _generatedSkillRuntimeData)
        {
            if (s.Value.basicConfig.skillId == id)
            {
                data = s.Value;
            }
        }
        if (data != null && data.generatedData.cdRemain <= 0f)//这里还有技能消耗值的判断
        {
            return data;
        }
        else
        {
            Debug.Log($"Skill {id} CD");
            return null;
        }
    }
    /// <summary>
    /// 生成技能
    /// </summary>
    /// <param name="data">技能运行时数据</param>
    public void GenerateSkill(SkillRuntimeData data)
    {
        GameObject skillGameObjectTransformRefObject = data.generatedData.owner;
        GameObject skillGameObject = null;
        if (data.postLoadSettings.skillSourcePrefab != null && skillGameObjectTransformRefObject != null)
        {
            // 创建技能预制体
            skillGameObject = Instantiate(
                data.postLoadSettings.skillSourcePrefab,
                skillGameObjectTransformRefObject.transform.position,
                skillGameObjectTransformRefObject.transform.rotation); // GameObjectPool.instance.CreateObject(data.prefabName, data.skillPrefab, transform.position, transform.rotation)
        }
        else
        {
            skillGameObject = skillGameObjectTransformRefObject;
        }

        if (skillGameObject != null)
        {
            //传递技能数据给技能释放器
            SkillEntityComponentBase skillEntity = skillGameObject.GetComponent<SkillEntityComponentBase>();
            skillEntity.skillData = data;
            //使用技能
            skillEntity.ReleaseSkill();

            StartCoroutine(CoolTimeDown(data));//开启冷却
        }
        else
        {
            Debug.Log($"Failed to GenerateSkill [{data.basicConfig.skillId}]{data.basicConfig.name}");
        }
    }
    /// <summary>
    /// 协程实现技能冷却
    /// </summary>
    /// <param name="data">技能运行时数据</param>
    /// <returns></returns>
    private IEnumerator CoolTimeDown(SkillRuntimeData data)
    {
        data.generatedData.cdRemain = data.staticSettings.basicConfig.skillCd;
        while (data.generatedData.cdRemain > 0f)
        {
            yield return new WaitForFixedUpdate();// (1f);
            data.generatedData.cdRemain = Mathf.Max(0f, data.generatedData.cdRemain - Time.deltaTime);
        }
    }


}
