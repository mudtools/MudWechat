// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Transport;

/// <summary>
/// 微信支付 APIv2 <b>XML ⇄ 扁平字段表</b>编解码器。
/// </summary>
/// <remarks>
/// <para><b>为什么以「扁平字段表」为中间形态</b>：APIv2 的签名算法作用在<b>扁平键值对</b>上
/// （ASCII 升序、排除空值与 <c>sign</c>），而报文载体是 XML，两者之间必须有一层与类型无关的
/// 中间表示。若改由反射读 DTO 属性会破 AOT 严格模式，故本类的唯一入口是
/// <see cref="System.Text.Json.JsonElement"/>（由源生成 <c>PayV2JsonContext</c> 产出，无元数据缺失）。</para>
/// <para><b>空值口径</b>：官方要求空值参数<b>不进报文也不进签名</b>，故 <see cref="WriteXml"/> 与
/// <see cref="ReadFields"/> 双向都丢弃空串/<see langword="null"/>。</para>
/// <para><b>无类型信息陷阱</b>：XML 所有叶子值都是字符串，反序列化到 <c>int?</c> / <c>long?</c>
/// 必须开 <see cref="System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString"/>，
/// 该选项由 <see cref="WechatPayV2ContentSerializer"/> 统一持有，不在本类里造第二个 options。</para>
/// </remarks>
internal static class WechatPayV2XmlCodec
{
    /// <summary>APIv2 报文根元素名（官方固定为 <c>xml</c>，无命名空间）。</summary>
    internal const string RootName = "xml";

    /// <summary>
    /// 把应答 XML 读成扁平字段表（只取根元素的<b>直接</b>子节点叶子；重复同名兄弟取首个）。
    /// </summary>
    /// <param name="xml">应答报文原文。</param>
    /// <returns>有序字段表；签名与反序列化共用它。</returns>
    /// <remarks>
    /// 官方签名的作用域即「根元素的顶层参数」，深层嵌套结构（如 <c>&lt;sub_items&gt;</c>）
    /// 不参与签名，故本方法有意不递归 —— 递归展平会<b>凭空造出</b>签名项导致校验必败。
    /// </remarks>
    internal static List<KeyValuePair<string, string?>> ReadFields(string xml)
    {
        var fields = new List<KeyValuePair<string, string?>>();
        if (string.IsNullOrWhiteSpace(xml))
        {
            return fields;
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xml, LoadOptions.None);
        }
        catch (Exception)
        {
            // 官方在参数格式错误 / 证书缺失等场景返回 HTML 或空体，非致命 ⇒ 交回调用方按
            // return_code 缺失判定失败，不在解码层抛（抛会丢掉官方原文，排障无从下手）。
            return fields;
        }

        var root = doc.Root;
        if (root is null)
        {
            return fields;
        }

        foreach (var element in root.Elements())
        {
            if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
            {
                fields.Add(new KeyValuePair<string, string?>(element.Name.LocalName, element.Value));
            }
        }

        return fields;
    }

    /// <summary>
    /// 把字段表写成官方形态的 XML 报文（空值不写入，值做 XML 转义，不使用 CDATA）。
    /// </summary>
    /// <param name="fields">字段表（顺序即报文顺序；建议 <c>sign</c> 放最后，与官方示例一致）。</param>
    /// <returns><c>&lt;xml&gt;...&lt;/xml&gt;</c> 形态报文。</returns>
    internal static string WriteXml(IEnumerable<KeyValuePair<string, string?>> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        var builder = new StringBuilder(256).Append('<').Append(RootName).Append('>');
        foreach (var field in fields)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                continue;
            }

            builder.Append('<').Append(field.Key).Append('>')
                .Append(Escape(field.Value))
                .Append("</").Append(field.Key).Append('>');
        }

        return builder.Append("</").Append(RootName).Append('>').ToString();
    }

    /// <summary>
    /// 由请求 DTO 的 <see cref="JsonElement"/> 形态导出字段表（嵌套对象/数组以其紧凑 JSON 文本入表）。
    /// </summary>
    /// <param name="root">DTO 序列化得到的 JSON 根元素。</param>
    /// <returns>字段表，键为官方参数名（已由 <c>[JsonPropertyName]</c> 钉死为下划线小写）。</returns>
    internal static List<KeyValuePair<string, string?>> ToFields(JsonElement root)
    {
        var fields = new List<KeyValuePair<string, string?>>();
        if (root.ValueKind != JsonValueKind.Object)
        {
            return fields;
        }

        foreach (var property in root.EnumerateObject())
        {
            var value = RenderValue(property.Value);
            if (!string.IsNullOrEmpty(value))
            {
                fields.Add(new KeyValuePair<string, string?>(property.Name, value));
            }
        }

        return fields;
    }

    /// <summary>
    /// 把扁平字段表编成 JSON 文本，供源生成上下文反序列化回强类型 DTO。
    /// </summary>
    /// <param name="fields">字段表。</param>
    /// <returns>JSON 对象文本（全部值以字符串形态出现，配合 <c>AllowReadingFromString</c> 读回数值）。</returns>
    /// <remarks>仅 <c>net6.0</c> 分支使用：低 TFM 无源生成上下文，走反射 options（已开 <c>AllowReadingFromString</c>）。</remarks>
    internal static string ToJson(IEnumerable<KeyValuePair<string, string?>> fields)
        => ToJson(fields, typeInfo: null);

#if NET8_0_OR_GREATER
    /// <summary>
    /// 类型感知版：<b>按目标 DTO 的属性类型</b>决定该字段写成 JSON 数字还是字符串。
    /// </summary>
    /// <param name="fields">字段表（来自应答 XML，全部值天然无类型）。</param>
    /// <param name="typeInfo">目标 DTO 的 <see cref="JsonTypeInfo"/>（源生成上下文产出，零反射）。</param>
    /// <returns>JSON 对象文本。</returns>
    /// <remarks>
    /// <para>
    /// <b>为什么不靠 <c>AllowReadingFromString</c> 一把梭</b>：该选项属 <c>JsonSerializerOptions</c> 级别的定制，
    /// 要作用于源生成上下文就得再造一份 options 并依赖「生成上下文可与改写后的 options 兼容」这一前提；
    /// 前提不成立时症状是运行期 <c>NotSupportedException</c>，而不是编译期报错。
    /// 改用 <see cref="JsonTypeInfo.Properties"/> 直接问元数据「这个字段是不是数值」，
    /// 既不碰 options，也不给「字符串型商户订单号恰好全是数字」留被误判为数字的空间。
    /// </para>
    /// </remarks>
    internal static string ToJson(
        IEnumerable<KeyValuePair<string, string?>> fields, JsonTypeInfo? typeInfo)
    {
        ArgumentNullException.ThrowIfNull(fields);

        var numericNames = BuildNumericNames(typeInfo);
        var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            foreach (var field in fields)
            {
                if (field.Value is null)
                {
                    continue;
                }

                if (numericNames.Contains(field.Key) && TryWriteNumber(writer, field))
                {
                    continue;
                }

                writer.WriteString(field.Key, field.Value);
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>取目标类型中「声明为数值」的 JSON 属性名集合（<c>typeInfo</c> 为 null 时返回空集 ⇒ 全按字符串写）。</summary>
    private static HashSet<string> BuildNumericNames(JsonTypeInfo? typeInfo)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        if (typeInfo is null)
        {
            return names;
        }

        foreach (var property in typeInfo.Properties)
        {
            if (IsNumeric(property.PropertyType))
            {
                names.Add(property.Name);
            }
        }

        return names;
    }

    /// <summary>是否数值属性（含可空形态）。XML 无类型，只有 DTO 声明能给出判据。</summary>
    private static bool IsNumeric(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying == typeof(int) || underlying == typeof(long) ||
               underlying == typeof(short) || underlying == typeof(uint) ||
               underlying == typeof(ulong) || underlying == typeof(double) ||
               underlying == typeof(decimal);
    }

    /// <summary>按整数优先、小数兜底的顺序尝试写数字；失败即由调用方回落成字符串。</summary>
    private static bool TryWriteNumber(Utf8JsonWriter writer, KeyValuePair<string, string?> field)
    {
        var value = field.Value!;
        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integral))
        {
            writer.WriteNumber(field.Key, integral);
            return true;
        }

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var fractional)
            && !double.IsNaN(fractional) && !double.IsInfinity(fractional))
        {
            writer.WriteNumber(field.Key, fractional);
            return true;
        }

        return false;
    }
#else
    /// <summary>
    /// 低 TFM 分支：无源生成上下文可问元数据，故全部值写成字符串，
    /// 由 <c>JsonSerializerOptions.NumberHandling = AllowReadingFromString</c> 在读侧收口。
    /// </summary>
    internal static string ToJson(
        IEnumerable<KeyValuePair<string, string?>> fields, JsonTypeInfo? typeInfo)
    {
        var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            foreach (var field in fields)
            {
                if (field.Value is null)
                {
                    continue;
                }

                writer.WriteString(field.Key, field.Value);
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
#endif

    private static string? RenderValue(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString(),
        JsonValueKind.Number => value.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        JsonValueKind.Null => null,
        // APIv2 顶层参数无结构化对象；官方要求这类字段（detail / scene_info）以 JSON 字符串传入。
        JsonValueKind.Object or JsonValueKind.Array => value.GetRawText(),
        _ => null,
    };

    private static string Escape(string value) => value
        .Replace("&", "&amp;")
        .Replace("<", "&lt;")
        .Replace(">", "&gt;");
}
