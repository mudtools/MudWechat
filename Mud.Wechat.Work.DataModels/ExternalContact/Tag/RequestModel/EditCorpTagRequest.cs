// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Tag;

/// <summary>
/// 编辑企业客户标签请求体（<c>/cgi-bin/externalcontact/edit_corp_tag</c>）。
/// <para>仅可修改标签 / 标签组的名称与次序值；修改后的标签组不能和已有的标签组重名，
/// 标签也不能和同一标签组下的其他标签重名。应用仅能编辑本应用创建的标签。</para>
/// </summary>
public class EditCorpTagRequest
{
    /// <summary>
    /// 获取或设置标签或标签组的 id（官方必填）。
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 获取或设置新的标签 / 标签组名称，最长 30 字符（不可与已有同名冲突）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置新的次序值，官方有效范围 [0, 2^32)，值大的排序靠前。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }

    /// <summary>
    /// 获取或设置授权方安装的应用 agentid（仅旧的第三方多应用套件需要填，否则忽略）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }
}
