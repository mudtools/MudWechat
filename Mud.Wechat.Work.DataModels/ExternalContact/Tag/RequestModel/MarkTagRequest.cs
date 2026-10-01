// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Tag;

/// <summary>
/// 编辑客户企业标签（打标签）请求体（<c>/cgi-bin/externalcontact/mark_tag</c>）。
/// <para><see cref="AddTag"/> 与 <see cref="RemoveTag"/> 不可同时为空；
/// 需确保 <see cref="ExternalUserid"/> 是 <see cref="Userid"/> 的外部联系人；
/// 同一标签组下现已支持多个标签；每个成员对同一个客户最多可添加 3000 个由企业统一配置的标签。
/// 应用只能编辑可见范围内的成员添加的企业客户标签。</para>
/// </summary>
public class MarkTagRequest
{
    /// <summary>
    /// 获取或设置添加外部联系人的企业成员 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置外部联系人 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置要标记的标签 id 列表。
    /// </summary>
    [JsonPropertyName("add_tag")]
    public List<string>? AddTag { get; set; }

    /// <summary>
    /// 获取或设置要移除的标签 id 列表。
    /// </summary>
    [JsonPropertyName("remove_tag")]
    public List<string>? RemoveTag { get; set; }
}
