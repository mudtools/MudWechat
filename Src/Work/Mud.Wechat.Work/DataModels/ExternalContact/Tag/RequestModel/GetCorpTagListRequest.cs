// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Tag;

/// <summary>
/// 获取企业标签库请求体（<c>/cgi-bin/externalcontact/get_corp_tag_list</c>）。
/// <para><see cref="TagId"/> 与 <see cref="GroupId"/> 均为空时返回所有标签；
/// 同时传递时官方忽略 <see cref="TagId"/>，仅以 <see cref="GroupId"/> 作为过滤条件。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class GetCorpTagListRequest
{
    /// <summary>
    /// 获取或设置要查询的标签 id 列表。
    /// </summary>
    [JsonPropertyName("tag_id")]
    public List<string>? TagId { get; set; }

    /// <summary>
    /// 获取或设置要查询的标签组 id 列表（返回该标签组以及其下的所有标签信息）。
    /// </summary>
    [JsonPropertyName("group_id")]
    public List<string>? GroupId { get; set; }
}
