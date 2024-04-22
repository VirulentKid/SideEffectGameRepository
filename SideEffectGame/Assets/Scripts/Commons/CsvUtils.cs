using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Reflection;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

/// <summary>
/// CSV工具类
/// </summary>
public static class CsvUtils
{
    public const string Suffix = ".csv";//CSV文件后缀

    ///// <summary>
    ///// 解析某一行
    ///// </summary>
    ///// row：从0开始
    //public static List<string> ParseRow(string csvPath, int row, List<int> ignoreColIndex = null)
    //{
    //    if (row < 0)
    //    {
    //        Debug.LogError($"解析的行数不能为负数，CSV文件：{csvPath}，行数：{row + 1}");
    //        return null;
    //    }
    //    if (string.IsNullOrEmpty(csvPath) || Path.GetExtension(csvPath) != Suffix)
    //    {
    //        Debug.LogError($"CSV文件路径有误：{csvPath}");
    //        return null;
    //    }
    //    string[] lineStrArray = File.ReadAllLines(csvPath);
    //    if (lineStrArray == null)
    //    {
    //        return null;
    //    }
    //    if (row > lineStrArray.Length - 1)
    //    {
    //        Debug.LogError($"超出表格的最大行数，CSV文件：{csvPath}，表格行数：{lineStrArray.Length}，要解析的行数：{row + 1}");
    //        return null;
    //    }
    //    List<string> ret = new List<string>();
    //    string rowStr = lineStrArray[row].Replace("\r", "");
    //    string[] cellStrArray = rowStr.Split(',');
    //    for (int col = 0; col < cellStrArray.Length; col++)
    //    {
    //        if (ignoreColIndex != null && ignoreColIndex.Contains(col))
    //        {
    //            continue;
    //        }
    //        ret.Add(cellStrArray[col]);
    //    }
    //    return ret;
    //}

    ///// <summary>
    ///// 解析某一列
    ///// </summary>
    ///// col：从0开始
    //public static List<string> ParseCol(string csvPath, int col, List<int> ignoreRowIndex = null)
    //{
    //    if (col < 0)
    //    {
    //        Debug.LogError($"解析的列数不能为负数，CSV文件：{csvPath}，列数：{col + 1}");
    //        return null;
    //    }
    //    if (string.IsNullOrEmpty(csvPath) || Path.GetExtension(csvPath) != Suffix)
    //    {
    //        Debug.LogError($"CSV文件路径有误：{csvPath}");
    //        return null;
    //    }
    //    string[] lineStrArray = File.ReadAllLines(csvPath);
    //    if (lineStrArray == null)
    //    {
    //        return null;
    //    }
    //    string[] tempCellStrArray = lineStrArray[0].Replace("\r", "").Split(',');
    //    if (col > tempCellStrArray.Length - 1)
    //    {
    //        Debug.LogError($"超出表格的最大列数，CSV文件：{csvPath}，表格列数：{tempCellStrArray.Length}，要解析的列数：{col + 1}");
    //        return null;
    //    }
    //    List<string> ret = new List<string>();
    //    for (int row = 0; row < lineStrArray.Length; row++)
    //    {
    //        if (ignoreRowIndex != null && ignoreRowIndex.Contains(row))
    //        {
    //            continue;
    //        }
    //        string rowStr = lineStrArray[row];
    //        string[] cellStrArray = rowStr.Replace("\r", "").Split(',');
    //        ret.Add(cellStrArray[col]);
    //    }
    //    return ret;
    //}

    /// <summary>
    /// 解析所有行（文件）
    /// </summary>
    public static List<List<string>> ParseRowAllFromFile(string csvPath, List<int> ignoreRow = null, List<int> ignoreCol = null)
    {
        if (string.IsNullOrEmpty(csvPath) || Path.GetExtension(csvPath) != Suffix)
        {
            Debug.LogError($"CSV文件路径有误：{csvPath}");
            return null;
        }
        string[] lineStrArray = File.ReadAllLines(csvPath);
        return ParseRowAllFromLineArray(lineStrArray, ignoreRow, ignoreCol);
    }

    /// <summary>
    /// 解析所有行（字符串）
    /// </summary>
    public static List<List<string>> ParseRowAllFromString(string csvString, List<int> ignoreRow = null, List<int> ignoreCol = null)
    {

        List<List<string>> ret = new List<List<string>>();
        string[] lineStrArray = csvString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return ParseRowAllFromLineArray(lineStrArray, ignoreRow, ignoreCol);

    }
    static List<List<string>> ParseRowAllFromLineArray(string[] lineStrArray, List<int> ignoreRow = null, List<int> ignoreCol = null)
    {
        List<List<string>> ret = new List<List<string>>();
        if (lineStrArray == null)
        {
            return null;
        }
        for (int row = 0; row < lineStrArray.Length; row++)
        {
            if (ignoreRow != null && ignoreRow.Contains(row))
            {
                continue;
            }
            List<string> rowStrList = new List<string>();
            string rowStr = lineStrArray[row].Replace("\r", "");
            string[] cellStrArray = rowStr.Split(',');
            for (int col = 0; col < cellStrArray.Length; col++)
            {
                if (ignoreCol != null && ignoreCol.Contains(col))
                {
                    continue;
                }
                rowStrList.Add(cellStrArray[col].Trim());
            }
            if (rowStrList.Count > 0 && !(rowStrList.Count == 1 && rowStrList[0].Length == 0))
            {
                ret.Add(rowStrList);
            }
        }
        return ret;
    }
}

public class CsvParser
{
    protected List<string> _headers = null;
    protected List<List<string>> _rows = null;
    protected Dictionary<string, int> _headerIndices = new Dictionary<string, int>();

    public int lineCount
    {
        get
        {
            return _rows?.Count ?? 0;
        }
    }

    public CsvParser(string csvString, List<int> ignoreRow = null, List<int> ignoreCol = null)
    {
        _rows = CsvUtils.ParseRowAllFromString(csvString, ignoreRow, ignoreCol);
        _headers = _rows[0];
        for (int i = 0; i < _headers.Count; ++i)
        {
            _headerIndices.Add(_headers[i], i);
        }
    }

    public string GetItem(int i, int j)
    {
        return _rows[i][j];
    }
    public bool TryGetItem(out string result, int i, int j)
    {
        if (i < 0 || i >= _rows.Count)
        {
            result = default(string);
            return false;
        }
        if (j < 0 || j >= _rows[i].Count)
        {
            result = default(string);
            return false;
        }
        result = _rows[i][j];
        return true;
    }

    //public bool TryGetItem<I, T>(out I result, int i, int j) where I : IEnumerable<T> where T : struct
    //{
    //    string item = _rows[i][j];

    //    if (item == null)
    //    {
    //        result = default(I); // 返回类型的默认值
    //        return false;
    //    }
    //}

    public bool TryGetItem<T>(out T result, int i, int j) where T : struct
    {
        string item = _rows[i][j];

        if (item == null)
        {
            result = default(T); // 返回类型的默认值
            return false;
        }

        Type type = typeof(T);
        // 泛型Nullable判断，取其中的类型
        if (type.IsGenericType)
        {
            type = type.GetGenericArguments()[0];
        }

        if (TryGetItemWithType(out object resultObj, i, j, type))
        {
            result = (T)resultObj;
            return true;
        }
        result = default(T);
        return false;


        //// string直接返回转换
        //if (type.Name.ToLower() == "string")
        //{

        //}

        //MethodInfo TryParseMethod = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
        //    new Type[] { typeof(string), type.MakeByRefType() },
        //    new ParameterModifier[] { new ParameterModifier(2) });
        //object[] parameters = new object[] { item, Activator.CreateInstance(type) };
        //if (TryParseMethod == null)
        //{
        //    result = default(T);
        //    return false;
        //}
        //bool success = (bool)TryParseMethod.Invoke(null, parameters);
        //if (success)
        //{
        //    result = (T)parameters[1];
        //    return true;
        //}
        //result = default(T);
        //return false;
    }

    protected bool TryParseWithType(out object result, string item, Type type)
    {
        if (type.IsArray)
        {
            Type elementType = type.GetElementType();
            string[] items = item.Split('|', StringSplitOptions.RemoveEmptyEntries);
            object[] results = new object[items.Length];
            for (int i = 0; i < items.Length; ++i)
            {
                string subItem = items[i];
                if (!TryParseWithType(out results[i], subItem, elementType))
                {
                    result = type.Default();
                    return false;
                }
            }
            result = results;
            return true;
        }


        if (type.Name.ToLower() == "string")
        {
            result = item;
            return true;
        }

        if (type.IsEnum)
        {
            return Enum.TryParse(type, item, out result);
        }

        MethodInfo TryParseMethod = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
            new Type[] { typeof(string), type.MakeByRefType() },
            new ParameterModifier[] { new ParameterModifier(2) });
        if (TryParseMethod == null)
        {

            //MethodInfo ParseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
            //    new Type[] { typeof(string) },
            //    new ParameterModifier[] { new ParameterModifier(1) });
            //if (ParseMethod == null)
            //{
                result = type.Default();
                return false;
            //}
            //object[] parameters1 = new object[] { item };
            //result = ParseMethod.Invoke(null, parameters1);
            //return true;
        }
        object[] parameters = new object[] { item, Activator.CreateInstance(type) };
        bool success = (bool)TryParseMethod.Invoke(null, parameters);
        if (success)
        {
            result = parameters[1];
            return true;
        }
        result = type.Default();
        return false;
    }

    public bool TryGetItemWithType(out object result, int i, int j, Type type)
    {
        string item = _rows[i][j];
        return TryParseWithType(out result, item, type);
    }

    public bool TryGetItem<T>(out T result, string header, int row) where T : struct
    {
        if (_headerIndices.TryGetValue(header, out int column))
        {
            return TryGetItem<T>(out result, row, column);
        }
        result = default(T);
        return false;
    }
    public bool TryGetItemWithType(out object result, string header, int row, Type type)
    {
        if (_headerIndices.TryGetValue(header, out int column))
        {
            return TryGetItemWithType(out result, row, column, type);
        }
        result = type.Default();
        return false;
    }
}
