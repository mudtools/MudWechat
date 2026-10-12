// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>
/// 拉黑 / 取消拉黑用户请求体（<c>batchBlacklist</c> 与 <c>batchUnblacklist</c> <b>共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>：官方两页请求体字段表一致（仅 <c>openid_list</c>），差异仅在语义（拉黑 / 取消）与
/// 数量上限表述（两页「注意事项」分别为「一次最多拉黑 20 个用户」「一次最多取消 20 个用户」）。
/// 拆成两个同构类属纯冗余（与标签域 <c>MpTagMembersRequest</c> 同一处置）。
/// </para>
/// <para>响应仅 <c>errcode</c>/<c>errmsg</c>（由 <see cref="MpResponse"/> 承载，故不设专用响应 DTO）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpBlacklistRequest
{
    /// <summary>需要拉黑 / 取消拉黑的 openid 列表（<b>单次最多 20 个</b>）。</summary>
    [JsonPropertyName("openid_list")]
    public List<string> OpenIdList { get; set; } = new();
}
