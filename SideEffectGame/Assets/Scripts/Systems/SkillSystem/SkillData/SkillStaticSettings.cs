using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// 技能运行时配置，包含运行时读取的数据
/// </summary>
[Serializable]
//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkillStaticSettingsSingle", order = 1)]
public class SkillStaticSettings // : ScriptableObject
{
    public SkillBasicConfig basicConfig = new SkillBasicConfig();
    public SkillPostLoadSettings postLoadSettings = new SkillPostLoadSettings();
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkillStaticSettingsList", order = 1)]
public class SkillStaticSettingsObject : ScriptableObject
{
    [SerializeField]
    public TextAsset csv;

    [SerializeField]
    public SkillStaticSettings[] skillSettings; // 技能列表

    public bool ParseFromCSV()
    {
        if (csv == null)
        {
            return false;
        }
        CsvParser csvParser = new CsvParser(csv.text);
        int lineCount = csvParser.lineCount;
        if (lineCount == 0)
        {
            return false;
        }

        skillSettings = new SkillStaticSettings[lineCount - 1];
        for (int i = 0; i < lineCount - 1; ++i)
        {
            object basicConfigObject = new SkillBasicConfig();

            Type structType = typeof(SkillBasicConfig);
            FieldInfo[] fields = structType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach(FieldInfo field in fields)
            {
                Type fieldType = field.FieldType;
                if (csvParser.TryGetItemWithType(out object value, field.Name, i + 1, fieldType))
                {
                    if (fieldType.IsArray)
                    {
                        Array arrayValue = value as Array;
                        Type elementType = fieldType.GetElementType();
                        Array newArrayValue = Array.CreateInstance(elementType, arrayValue.Length);
                        Array.Copy(arrayValue, newArrayValue, arrayValue.Length);
                        field.SetValue(basicConfigObject, newArrayValue);
                    }
                    else
                    {
                        field.SetValue(basicConfigObject, value);
                    }
                }
                else
                {
                    Debug.Log($"Failed to parse {field.Name} for row {i + 1}!");
                }
            }
            SkillStaticSettings skillSetting = new SkillStaticSettings();
            skillSetting.basicConfig = (SkillBasicConfig)basicConfigObject;
            skillSettings[i] = skillSetting;
        }
        return true;
    }
}
