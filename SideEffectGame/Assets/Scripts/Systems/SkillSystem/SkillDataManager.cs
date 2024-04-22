using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataManager : SceneSystemManagerBase
{
    protected readonly string _skillPrefabBasePath = "Prefabs/Skills/";
    protected readonly string _VFXPrefabBasePath = "Prefabs/VFXs/";
    protected readonly string _AFXBasePath = "Audios/";

    [SerializeField]
    protected SkillStaticSettingsObject _skillSettingsObject; // 技能列表

    static protected SkillDataManager _singleton = null;
    static public SkillDataManager Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = SceneSystemManagerBase.GetSingleton<SkillDataManager>();
            }
            return _singleton;
        }
    }

    new void Awake()
    {
        SceneSystemManagerBase.RegisterSceneSystemManager(this);
        if (_skillSettingsObject != null)
        {
            _skillSettingsObject.ParseFromCSV();
            InitSkills();
        }
    }

    new void OnDestroy()
    {
        SceneSystemManagerBase.UnregisterSceneSystemManager(this);
    }

    /// <summary>
    /// 查询技能
    /// </summary>
    /// <param name="id">技能ID</param>
    /// <param name="settings">out 技能配置</param>
    /// <returns>bool 是否能找到</returns>
    public bool FindSkill(int id, out SkillStaticSettings settings)
    {
        if (id >= 0 && id < _skillSettingsObject.skillSettings.Length)
        {
            settings = _skillSettingsObject.skillSettings[id];
            return true;
        }
        settings = new SkillStaticSettings();
        return false;
    }

    /// <summary>
    /// 初始化技能
    /// </summary>
    private void InitSkills()
    {
        // TODO: 路径由其他资源管理器解析（资源名->路径名）
        for (int i = 0; i < _skillSettingsObject.skillSettings.Length; ++i)
        {
            if (_skillSettingsObject.skillSettings[i].basicConfig.skillPrefabName != "")
            {
                _skillSettingsObject.skillSettings[i].postLoadSettings.skillSourcePrefab = Resources.Load<GameObject>(_skillPrefabBasePath + _skillSettingsObject.skillSettings[i].basicConfig.skillPrefabName);
            }
            if (_skillSettingsObject.skillSettings[i].basicConfig.releaseVFXName != "")
            {
                _skillSettingsObject.skillSettings[i].postLoadSettings.releaseVFXPrefab = Resources.Load<GameObject>(_VFXPrefabBasePath + _skillSettingsObject.skillSettings[i].basicConfig.releaseVFXName);
            }
            if (_skillSettingsObject.skillSettings[i].basicConfig.releaseAFXName != "")
            {
                _skillSettingsObject.skillSettings[i].postLoadSettings.releaseAFXClip = Resources.Load<AudioClip>(_AFXBasePath + _skillSettingsObject.skillSettings[i].basicConfig.releaseAFXName);
            }
            if (_skillSettingsObject.skillSettings[i].basicConfig.hitVFXName != "")
            {
                _skillSettingsObject.skillSettings[i].postLoadSettings.hitVFXPrefab = Resources.Load<GameObject>(_VFXPrefabBasePath + _skillSettingsObject.skillSettings[i].basicConfig.hitVFXName);
            }
            if (_skillSettingsObject.skillSettings[i].basicConfig.hitAFXName != "")
            {
                _skillSettingsObject.skillSettings[i].postLoadSettings.hitAFXClip = Resources.Load<AudioClip>(_AFXBasePath + _skillSettingsObject.skillSettings[i].basicConfig.hitAFXName);
            }
            // TODO: skillIcon
            // TODO: 其他运行时资源加载
        }
    }
}