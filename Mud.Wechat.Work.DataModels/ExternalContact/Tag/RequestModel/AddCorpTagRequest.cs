// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Tag;

/// <summary>
/// 添加企业客户标签请求体（<c>/cgi-bin/externalcontact/add_corp_tag</c>）。
/// <para>填写 <see cref="GroupId"/> 表示在指定标签组下添加标签（此时 <see cref="GroupName"/>
/// 与标签组 <see cref="Order"/> 被忽略）；未填写则按 <see cref="GroupName"/> 新建标签组
/// （同名标签组会复用已存在的组；不支持创建空标签组）。每个企业最多可配置 10000 个企业标签。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class AddCorpTagRequest
{
    /// <summary>
    /// 获取或设置标签组 id（在指定标签组下添加标签时填写）。
    /// </summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>
    /// 获取或设置标签组名称，最长 30 字符（仅新建标签组时有效）。
    /// </summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }

    /// <summary>
    /// 获取或设置标签组次序值，官方有效范围 [0, 2^32)，值大的排序靠前。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }

    /// <summary>
    /// 获取或设置添加的标签列表（官方必填，不可为空）。
    /// </summary>
    [JsonPropertyName("tag")]
    public List<CorpTagCreateItem>? Tag { get; set; }

    /// <summary>
    /// 获取或设置授权方安装的应用 agentid（仅旧的第三方多应用套件需要填，否则忽略）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }
}
