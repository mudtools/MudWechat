// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 用户授权信息变更事件载荷（<c>user_info_modified</c> / <c>user_authorization_revoke</c> /
/// <c>user_authorization_cancellation</c>；官方 H5 授权页，V5 已核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族合并</b>：三键的报文结构一致（信封 + <c>OpenID</c> + <c>UnionID</c> + <c>AppID</c> + <c>RevokeInfo</c>），
/// 仅语义不同 ⇒ 合并为一个载荷。
/// </para>
/// <para>
/// <b>合规动作（处理器义务）</b>：资料变更 ⇒ 更新/清理本地头像昵称；资料撤回 ⇒ 及时删除用户信息
/// （可按 <see cref="RevokeInfo"/> 精确清理对应范围）；完成注销 ⇒ 删除或匿名化处理。
/// <b>幂等</b>：重复投递不得重复删除/重复更新出错。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpAuthorizationEventTypes.UserInfoModified,
    MpAuthorizationEventTypes.UserAuthorizationRevoke,
    MpAuthorizationEventTypes.UserAuthorizationCancellation,
})]
public sealed partial class MpAuthorizationEventPayload : MpCallbackPayload
{
    /// <summary>授权用户 OpenID（官方 <c>OpenID</c>）。</summary>
    [PayloadField("OpenID")]
    public string? OpenId { get; set; }

    /// <summary>授权用户 UnionID（官方 <c>UnionID</c>）。</summary>
    [PayloadField("UnionID")]
    public string? UnionId { get; set; }

    /// <summary>服务号的 AppID（官方 <c>AppID</c>）。</summary>
    [PayloadField("AppID")]
    public string? AppId { get; set; }

    /// <summary>
    /// 撤回的 H5 授权信息（官方 <c>RevokeInfo</c>）：201 地址 / 202 发票信息 / 203 卡券信息 / 204 麦克风 /
    /// 205 昵称和头像 / 206 位置信息 / 207 选中的图片或视频。
    /// </summary>
    /// <remarks>仅 <c>user_authorization_revoke</c> 携带；官方示例的 XML 中该值域与取值表存在出入（示例值 <c>1</c>），**以取值表为准**。</remarks>
    [PayloadField("RevokeInfo")]
    public string? RevokeInfo { get; set; }
}
