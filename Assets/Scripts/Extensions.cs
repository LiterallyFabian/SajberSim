using SajberSim.Steam;
using SajberSim.Translation;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public static class Extensions
{
    /// <summary>
    /// Sorts a list of numbers (1, 9, 10 instead of 1, 10, 9)
    /// </summary>
    public static IEnumerable<string> NumberSort(this IEnumerable<string> list)
    {
        int maxLen = list.Select(s => s.Length).Max();

        return list.Select(s => new
        {
            OrgStr = s,
            SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
        })
        .OrderBy(x => x.SortStr)
        .Select(x => x.OrgStr);
    }
    /// <summary>
    /// Get friendly description from enum value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum value)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name != null)
        {
            FieldInfo field = type.GetField(name);
            if (field != null)
            {
                DescriptionAttribute attr =
                       Attribute.GetCustomAttribute(field,
                         typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null)
                {
                    return attr.Description;
                }
            }
        }
        return value.ToString();
    }
    public static string Name(this Stats.List value)
    {
        return Translate.Get(value.ToString());
    }
}