// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档成员项（官方 <c>doc_member_list</c> / <c>update_file_member_list</c> / <c>del_file_member_list</c> 元素；
/// 获取文档权限信息响应与修改文档成员与权限请求共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：获取文档权限信息响应参数表将 <c>userid</c> 标注为 bytes，示例为字符串，
/// 本模型以字符串承载两种形态。
/// 官方业务限制：文档成员仅支持按人配置（type = 1）；成员权限取值 <c>1</c> 只读、<c>2</c> 读写、<c>7</c> 管理员；
/// 修改成员的两个列表批次大小均最大 100。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocMember
{
    /// <summary>
    /// 获取或设置文档成员类型（官方 <c>type</c>，必填）。
    /// 官方取值：<c>1</c> 用户（仅支持按人配置）。
    /// </summary>
    [JsonPropertyName("type")]
    public uint? Type { get; set; }

    /// <summary>获取或设置企业内成员的 userid（官方 <c>userid</c>）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置外部用户临时 id（官方 <c>tmp_external_userid</c>），同一用户在不同文档中该 id 不一致。</summary>
    [JsonPropertyName("tmp_external_userid")]
    public string? TmpExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置该成员的权限（官方 <c>auth</c>）；更新成员列表时必填，删除成员列表时不传。
    /// 官方取值：<c>1</c> 只读、<c>2</c> 读写、<c>7</c> 管理员。
    /// </summary>
    [JsonPropertyName("auth")]
    public uint? Auth { get; set; }
}
