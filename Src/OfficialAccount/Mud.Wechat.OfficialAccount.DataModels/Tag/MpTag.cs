// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 公众号标签（用户标签）。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段来源与「同类型两形态」的显式处置</b>：官方在不同端点返回的标签字段并<b>不相同</b>——
/// </para>
/// <list type="bullet">
/// <item><c>createTag</c> 响应：<c>{ "tag": { "id": number, "name": string } }</c>（<b>无 <c>count</c></b>）；</item>
/// <item><c>getTags</c> 响应：<c>tags: [{ "id": number, "name": string, "count": number }]</c>（<b>含 <c>count</c></b>）。</item>
/// </list>
/// <para>
/// 故 <see cref="Count"/> 建模为<b>可空</b>：<c>createTag</c> 路径下官方不下发 ⇒ <c>null</c>，
/// 调用方不得据 <c>null</c> 推断「标签下无粉丝」（真实 0 由 <c>getTags</c> 下发）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpTag
{
    /// <summary>标签 id，由微信分配。</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>标签名（UTF8 编码；创建/编辑时官方限制 30 个字符以内）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>标签下粉丝数（仅 <c>getTags</c> 下发；<c>createTag</c> 缺省为 <c>null</c>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

/// <summary>标签的「名称」形态（创建标签请求体的 <c>tag</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpTagName
{
    /// <summary>标签名（官方：30 个字符以内）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>标签的「标识 + 名称」形态（编辑标签请求体的 <c>tag</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpTagIdName
{
    /// <summary>标签 id，由微信分配。</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>修改后的标签名（UTF8 编码）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>标签的「标识」形态（删除标签请求体的 <c>tag</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpTagId
{
    /// <summary>标签 id。</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
