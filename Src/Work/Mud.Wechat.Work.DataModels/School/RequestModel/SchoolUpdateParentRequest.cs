// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 更新家长请求体（<c>/cgi-bin/school/user/update_parent</c>，家校沟通）。
/// <para>
/// 官方业务限制：参数字段超过长度限制时整个请求会被拦掉；
/// <see cref="Children"/> 为<b>全量更新</b>——官方原文「该字段是全量更新，如果孩子列表为空则忽略该字段」，
/// 即传非空列表为全量覆盖、传空数组<b>不是</b>删除全部孩子关系而是忽略该字段；
/// <see cref="NewParentUserid"/> 每个家长仅能更新一次。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolUpdateParentRequest
{
    /// <summary>
    /// 获取或设置家长 UserID（官方必填）。学校内必须唯一；不区分大小写，
    /// 长度为 1~64 个字节，只能由数字、字母和 "_-@." 四种字符组成，且第一个字符必须是数字或字母。
    /// </summary>
    [JsonPropertyName("parent_userid")]
    public string? ParentUserid { get; set; }

    /// <summary>
    /// 获取或设置更新的家长 UserID。不能与已经存在的家长 UserID 相同，每个家长仅能更新一次。
    /// </summary>
    [JsonPropertyName("new_parent_userid")]
    public string? NewParentUserid { get; set; }

    /// <summary>
    /// 获取或设置家长手机号。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 获取或设置家长的孩子列表（最多 10 个）。官方全量更新语义：
    /// 传非空列表为全量覆盖，孩子列表为空则忽略该字段（不会因此清空孩子关系）。
    /// </summary>
    [JsonPropertyName("children")]
    public List<SchoolParentChildItem>? Children { get; set; }
}
