﻿﻿using CodeAssistant.Settings;

using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace CodeAssistant.Helpers;

internal static class StandardXmlFormatter
{
    public static string Format(string xml, CSProjFormatSettings settings)
    {
        var doc = XDocument.Parse(xml, LoadOptions.None);

        var xmlSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = settings.IndentChars,
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace,
            OmitXmlDeclaration = true,
            // 让 UI 暴露的开关真正生效
            NewLineOnAttributes = settings.NewLineOnAttributes,
        };

        // XmlWriterSettings 没有直接控制空元素展开的开关，
        // 通过把 IsEmpty 置为 false 让 Writer 输出 <Foo></Foo> 形式。
        if (settings.ExpandEmptyElements)
        {
            foreach (var el in doc.Descendants().Where(e => e.IsEmpty))
            {
                el.Value = string.Empty; // 赋值会自动将 IsEmpty 置为 false
            }
        }

        // 使用 InvariantCulture，避免不同区域设置下数字/日期格式化差异
        using var sw = new StringWriter(CultureInfo.InvariantCulture);
        using var writer = XmlWriter.Create(sw, xmlSettings);

        doc.Save(writer);
        writer.Flush();

        return sw.ToString();
    }
}
