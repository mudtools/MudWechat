// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.Wechat.Pay.V2.DataModels.PayV2;

namespace Mud.Wechat.Pay.V2.Transport;

/// <summary>
/// APIv2 报文序列化器：<b>DTO ⇄ 扁平字段表 ⇄ XML</b>，替换组件默认的 JSON 载体。
/// </summary>
/// <remarks>
/// <para>
/// <b>注入位置</b>：<see cref="HttpClientFactoryEnhancedClient"/> 构造末位形参
/// <c>contentSerializer</c>（组件为每条产品线留的<b>唯一</b>序列化扩展点）。
/// <b>不用</b> <c>SerializationMethod.Xml</c> / <c>SendXmlAsync</c> / <c>PostAsXmlAsync</c> ——
/// 那条路走 <c>XmlSerializer</c> 反射，AOT 严格模式下直接是错误。
/// </para>
/// <para>
/// <b>签名不在本类</b>：APIv2 的 <c>sign</c> 在报文体内，而取密钥是异步
/// （<c>ISecretProvider.GetSecretAsync</c>），<see cref="ToHttpContent{T}"/> 却是同步方法 ⇒
/// 只能由 <see cref="WechatPayV2SigningHandler"/> 在发出前补写（与 APIv3 把 <c>Authorization</c>
/// 头放在 Handler 里同一分工）。本类产出的报文<b>不含</b> <c>sign</c>，读入时亦<b>不校验</b>签名。
/// </para>
/// <para>
/// <b>类型来源</b>：net8+ 全程经源生成 <c>PayV2JsonContext</c> 的 <c>JsonTypeInfo</c>，
/// 零反射；应答侧数值字段由 <c>WechatPayV2XmlCodec.ToJson</c> 按目标属性类型还原
/// （XML 值无类型，<c>"100"</c> 直读 <c>int?</c> 会抛）。
/// </para>
/// <para>
/// <b>报文形态约定</b>：<see cref="Serialize{T}"/> 返回 <b>XML</b> 文本，因此
/// <see cref="Deserialize{T}"/> 的「json」形参在本实现里承载的同样是 XML 文本 ——
/// 两者是一对逆运算，不与组件的 JSON 语义混用。
/// </para>
/// </remarks>
internal sealed class WechatPayV2ContentSerializer : IHttpContentSerializer
{
    /// <summary>官方 APIv2 请求的内容类型（charset 固定 UTF-8）。</summary>
    internal const string XmlMediaType = "text/xml";

#if !NET8_0_OR_GREATER
    /// <summary>
    /// 低 TFM 反射 options：无源生成上下文可用（<c>PayV2JsonContext</c> 整体 <c>#if NET8_0_OR_GREATER</c>），
    /// 数值字段改由 <c>AllowReadingFromString</c> 在读侧收口。
    /// </summary>
    private static readonly JsonSerializerOptions ReflectionOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
#endif

    /// <inheritdoc />
    public HttpContent ToHttpContent<T>(T item, object? options = null)
        => new StringContent(Serialize(item, options), Encoding.UTF8, XmlMediaType);

    /// <inheritdoc />
    public async Task<T?> FromHttpContentAsync<T>(
        HttpContent content, object? options = null, CancellationToken cancellationToken = default)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        var payload = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return Deserialize<T>(payload, options);
    }

    /// <inheritdoc />
    public string Serialize<T>(T item, object? options = null)
    {
        if (item is null)
        {
            return WechatPayV2XmlCodec.WriteXml(Array.Empty<KeyValuePair<string, string?>>());
        }

#if NET8_0_OR_GREATER
        var typeInfo = GetTypeInfo<T>();
        var root = JsonSerializer.SerializeToElement(item, typeInfo);
        return WechatPayV2XmlCodec.WriteXml(WechatPayV2XmlCodec.ToFields(root));
#else
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(item, ReflectionOptions));
        return WechatPayV2XmlCodec.WriteXml(WechatPayV2XmlCodec.ToFields(document.RootElement));
#endif
    }

    /// <summary>
    /// 把请求 DTO 展成<b>未签名</b>的扁平字段表（双向证书通道在此表上算 <c>sign</c> 后再成报文）。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="Serialize{T}"/> 同源（同一个 <c>JsonTypeInfo</c> → 同一张字段表），只是不落成 XML：
    /// 签名作用于字段表，若先写 XML 再 <c>ReadFields</c> 读回，等于把自家编解码绕一圈、
    /// 还给「XML 转义 ↔ 反转义」留了出错面。
    /// </remarks>
    internal List<KeyValuePair<string, string?>> ToFields<T>(T item)
    {
        if (item is null)
        {
            return new List<KeyValuePair<string, string?>>();
        }

#if NET8_0_OR_GREATER
        return WechatPayV2XmlCodec.ToFields(JsonSerializer.SerializeToElement(item, GetTypeInfo<T>()));
#else
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(item, ReflectionOptions));
        return WechatPayV2XmlCodec.ToFields(document.RootElement);
#endif
    }

    // 组件接口的以下三个成员自带 [RequiresUnreferencedCode] / [RequiresDynamicCode]（默认实现是反射式 JSON），
    // 而 IL2046 / IL3051 要求「实现侧标注与接口侧一致」。本实现三者都走源生成 JsonTypeInfo
    // （net6 才回落反射 options，该 TFM 不进 AOT 严格门禁）⇒ 不给免反射的方法挂假标注，
    // 改为按 AGENTS §3 的口径就地抑制并附理由（restore 在 GetFieldNameForProperty 之后）。
#pragma warning disable IL2046, IL3051 // RequiresUnreferencedCode / RequiresDynamicCode 标注在接口与实现间必须一致
    /// <inheritdoc />
    public string Serialize(object? item, Type type, object? options = null)
    {
        if (type is null) { throw new ArgumentNullException(nameof(type)); }

        if (item is null)
        {
            return WechatPayV2XmlCodec.WriteXml(Array.Empty<KeyValuePair<string, string?>>());
        }

#if NET8_0_OR_GREATER
        // 非泛型入口同样走元数据：JsonSerializer.SerializeToElement(object, JsonTypeInfo) 是
        // 源生成上下文支持的免反射重载（options.GetTypeInfo(type) 拿到的就是 JsonTypeInfo<T> 本体）。
        var root = JsonSerializer.SerializeToElement(item, GetTypeInfo(type));
        return WechatPayV2XmlCodec.WriteXml(WechatPayV2XmlCodec.ToFields(root));
#else
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(item, type, ReflectionOptions));
        return WechatPayV2XmlCodec.WriteXml(WechatPayV2XmlCodec.ToFields(document.RootElement));
#endif
    }

    /// <inheritdoc />
    /// <remarks>
    /// 本实现的「json」实为 <b>XML 报文原文</b>（与 <see cref="Serialize{T}"/> 互为逆运算）。
    /// 非 XML（官方参数错误页 / 空体）读成空字段表 ⇒ 得到 <c>default</c>，
    /// 由调用方按 <c>return_code</c> 缺失判失败；<c>T = string</c> 时原样返回报文（下载类端点的 CSV 出口）。
    /// </remarks>
    public T Deserialize<T>(string json, object? options = null)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)json;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            return default!;
        }

        var fields = WechatPayV2XmlCodec.ReadFields(json);
        if (fields.Count == 0)
        {
            return default!;
        }

#if NET8_0_OR_GREATER
        var typeInfo = GetTypeInfo<T>();
        return JsonSerializer.Deserialize(
            WechatPayV2XmlCodec.ToJson(fields, typeInfo), typeInfo)!;
#else
        return JsonSerializer.Deserialize<T>(
            WechatPayV2XmlCodec.ToJson(fields), ReflectionOptions)!;
#endif
    }

    /// <inheritdoc />
    /// <remarks>
    /// 组件用它把属性名映射到线上参数名（表单 / Query 场景）。APIv2 的键名在 DTO 上以
    /// <c>[JsonPropertyName]</c> 钉死为官方原文拼写，故优先取该特性；缺特性时回落 snake_case，
    /// 与 <c>PayV2JsonContext</c> 的生成口径保持一致。
    /// </remarks>
    public string GetFieldNameForProperty(PropertyInfo propertyInfo)
    {
        if (propertyInfo is null)
        {
            throw new ArgumentNullException(nameof(propertyInfo));
        }

        var configured = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        if (!string.IsNullOrEmpty(configured))
        {
            return configured!;
        }

        return JsonNamingPolicy.SnakeCaseLower.ConvertName(propertyInfo.Name);
    }
#pragma warning restore IL2046, IL3051

#if NET8_0_OR_GREATER
    /// <summary>取 DTO 的源生成元数据；未登记进上下文即点名报错（而非回落到反射序列化）。</summary>
    private static JsonTypeInfo<T> GetTypeInfo<T>()
    {
        var typeInfo = PayV2JsonContext.Default.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>;
        return typeInfo ?? throw new InvalidOperationException(
            $"类型 {typeof(T).FullName} 未登记进源生成上下文 PayV2JsonContext。" +
            "APIv2 线禁用反射序列化：请给该 DTO 补 [HttpJsonSerializable(SerializerClassName = \"PayV2\")] " +
            "并重跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1。");
    }

    private static JsonTypeInfo GetTypeInfo(Type type)
    {
        var typeInfo = PayV2JsonContext.Default.GetTypeInfo(type);
        return typeInfo ?? throw new InvalidOperationException(
            $"类型 {type.FullName} 未登记进源生成上下文 PayV2JsonContext（同上处置）。");
    }
#endif
}
