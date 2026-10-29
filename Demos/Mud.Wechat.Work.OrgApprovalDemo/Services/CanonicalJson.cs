// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mud.Wechat.Work.OrgApprovalDemo.Services;

/// <summary>
/// 规范化 JSON 与指纹（方案 §6.5 第 3 步）：<b>键排序</b>后序列化再取 SHA256。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须先排序</b>：官方应答的属性顺序不保证稳定，而 <c>oa_template.content_hash</c> 是
/// 「内容没变就跳过、不动 version」的唯一判据。不排序会把「顺序变了」误判成「内容变了」，
/// 表现是每次同步都 <c>version++</c>，版本列表被灌满同内容快照。
/// </para>
/// <para>
/// 序列化统一 <c>UnsafeRelaxedJsonEscaping</c>：模板标题是中文，默认 escaper 会把它们写成 <c>\uXXXX</c>，
/// 虽然不影响哈希一致性，但会把 <c>raw_json</c> 变成不可读的墙，失去「落库存原样供人查」的意义。
/// </para>
/// </remarks>
public static class CanonicalJson
{
    // 两套选项只差缩进；Encoder 必须一致，否则同一份内容在「指纹计算」与「落库展示」两条路径上
    // 会产出不同字节（哈希随之漂移）。
    private static readonly JsonSerializerOptions CompactOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static readonly JsonSerializerOptions PrettyOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>把任意对象序列化为 <see cref="JsonNode"/>（DTO 上的 <c>JsonPropertyName</c> 生效）。</summary>
    public static JsonNode? ToNode<T>(T value) => JsonSerializer.SerializeToNode(value);

    /// <summary>规范化：对象键按序号排序、数组保持原序、去空白。</summary>
    public static string Normalize(JsonNode? node)
        => (Sort(node) ?? new JsonObject()).ToJsonString(CompactOptions);

    /// <summary>规范化后以缩进形态输出（供 <c>raw_json</c> 落库，人可读）。</summary>
    public static string NormalizeIndented(JsonNode? node)
        => (Sort(node) ?? new JsonObject()).ToJsonString(PrettyOptions);

    /// <summary>对文本取 SHA256（小写十六进制）。</summary>
    public static string Sha256(string text)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();

    /// <summary>对一组字段值取指纹：用不可见分隔符拼接后哈希，避免「a|b」与「a|b|」类歧义。</summary>
    public static string Digits(params string?[] parts)
    {
        var builder = new StringBuilder(parts.Length * 16);
        foreach (var part in parts)
        {
            builder.Append(part ?? "\u0000null");
            builder.Append('\u001f');
        }

        return Sha256(builder.ToString());
    }

    private static JsonNode? Sort(JsonNode? node) => node switch
    {
        JsonObject obj => SortObject(obj),
        JsonArray array => SortArray(array),
        _ => node?.DeepClone(),
    };

    private static JsonObject SortObject(JsonObject source)
    {
        var result = new JsonObject();
        foreach (var pair in source.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            result[pair.Key] = Sort(pair.Value);
        }

        return result;
    }

    private static JsonArray SortArray(JsonArray source)
    {
        var result = new JsonArray();
        foreach (var item in source)
        {
            result.Add(Sort(item));
        }

        return result;
    }
}
