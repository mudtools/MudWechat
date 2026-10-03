// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 企业客户变更事件载荷（<b>结构族</b>：官方事件键 <c>change_external_contact</c>；
/// 官方 92130 企业自建 / 92277 第三方 / 96361 服务商代开发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>族事件值即事件键</b>：本族 <c>ChangeType</c>（<c>add_external_contact</c> / <c>edit_external_contact</c> /
/// <c>add_half_external_contact</c> / <c>del_external_contact</c> / <c>del_follow_user</c> / <c>transfer_fail</c>）
/// 中 <c>del_follow_user</c> 与获客助手族同名，逐 <c>ChangeType</c> 键无法消歧 ⇒ 事件键为族事件值，
/// 具体类别由信封 <c>ChangeType</c> 判别（ADR-1 结构族模型，同 <c>ChainChangedPayload</c>）。
/// </para>
/// <para>
/// <b>双信封同键</b>：企业自建/代开发的应用数据回调为 <c>Event</c> 信封；第三方应用的指令回调 URL
/// 为套件信封（<c>SuiteId</c>/<c>AuthCorpId</c>/<c>InfoType</c>/<c>TimeStamp</c>，官方 92277）。
/// 两种信封产出同一事件键 ⇒ 一份载荷覆盖三模式（外层 <c>SuiteId</c>/<c>AuthCorpId</c> 由信封承载，不入载荷）。
/// </para>
/// <para>
/// <b>权限分层（处理器不得假设必有值）</b>：接收本族事件需「客户联系-可调用接口的应用」（自建）或
/// 「客户联系→客户基础信息」权限（第三方/代开发）；接收接替失败事件需「分配在职或离职成员的客户」权限。
/// 仅客户端/管理端操作触发回调，API 操作不产生回调。
/// </para>
/// <para>
/// <b>获客助手组件形态</b>：第三方「获客助手」的添加成功事件（官方 99485）复用本事件键并额外携带
/// <see cref="LinkId"/>（获客链接 id）—— 经 <see cref="LinkId"/> 是否有值即可识别该形态。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.ExternalContactChange,
    SupportedAppTypes = WechatAppTypeSet.Internal | WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeExternalContact })]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.ExternalContactChange,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeExternalContact })]
public sealed partial class ExternalContactChangedPayload : WechatCallbackPayload
{
    /// <summary>企业服务人员 UserId（官方 <c>UserID</c>；获客助手组件形态可能不返回）。</summary>
    [PayloadField("UserID")]
    public string? UserId { get; set; }

    /// <summary>外部联系人的 userid（官方 <c>ExternalUserID</c>；注意不是企业成员的账号）。</summary>
    [PayloadField("ExternalUserID")]
    public string? ExternalUserId { get; set; }

    /// <summary>添加渠道（官方 <c>State</c>：「联系我」的 state 参数或获客链接的 customer_channel 参数）。</summary>
    [PayloadField("State")]
    public string? State { get; set; }

    /// <summary>
    /// 欢迎语 code（官方 <c>WelcomeCode</c>；仅添加/免验证添加事件携带）。
    /// 双方已开始聊天、已用过免验证事件的 code 发过欢迎语、或添加的是商务伙伴（自动递名片）时不返回。
    /// </summary>
    [PayloadField("WelcomeCode")]
    public string? WelcomeCode { get; set; }

    /// <summary>
    /// 删除来源（官方 <c>Source</c>；仅删除企业客户事件携带）：
    /// <c>DELETE_BY_TRANSFER</c> = 客户因在职继承自动被转接成员删除。
    /// </summary>
    [PayloadField("Source")]
    public string? Source { get; set; }

    /// <summary>
    /// 接替失败原因（官方 <c>FailReason</c>；仅客户接替失败事件携带）：
    /// <c>customer_refused</c>（客户拒绝）/ <c>customer_limit_exceed</c>（接替成员客户数达上限）。
    /// </summary>
    [PayloadField("FailReason")]
    public string? FailReason { get; set; }

    /// <summary>获客链接 id（官方 <c>LinkId</c>；仅获客助手组件的添加成功事件携带，官方 99485）。</summary>
    [PayloadField("LinkId")]
    public string? LinkId { get; set; }
}
