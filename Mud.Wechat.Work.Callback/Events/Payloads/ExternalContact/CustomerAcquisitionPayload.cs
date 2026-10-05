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
/// 获客助手事件通知载荷（<b>结构族</b>：官方事件键 <c>customer_acquisition</c> 与
/// <c>customer_acquisition_permit_change</c>；官方 97299 企业自建 / 97402·99485 第三方 / 98958 代开发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>族事件值即事件键</b>：本族 <c>ChangeType</c>（额度族 <c>balance_low</c>/<c>balance_exhausted</c>/
/// <c>balance_increased</c>、链接族 <c>link_unavailable</c>/<c>delete_link</c>、转化族
/// <c>open_profile</c>/<c>friend_request</c>/<c>customer_start_chat</c>/<c>message_from_customer</c>/
/// <c>del_follow_user</c>、额度过期 <c>quota_expire_soon</c>，及第三方组件形态
/// <c>service_balance_low</c>/<c>service_balance_exhausted</c>/<c>service_balance_consumed</c>/<c>change_price</c>）
/// 中 <c>del_follow_user</c> 与客户联系变更族同名 ⇒ 事件键为族事件值，具体类别由信封 <c>ChangeType</c> 判别。
/// 各类别的专属字段互斥出现（权限/形态分层），处理器<b>不得假设必有值</b>。
/// </para>
/// <para>
/// <b>双信封同键</b>：企业自建/代开发为 <c>Event</c> 信封；第三方为套件信封（指令回调 URL，
/// <c>InfoType</c> 节点承载族事件值，官方 97402/99485）。两种信封产出同一事件键 ⇒ 一份载荷覆盖三模式。
/// </para>
/// <para>
/// <b>官方行为要点</b>：通过 API 删除获客链接不触发 <c>delete_link</c> 回调；
/// <c>open_profile</c> 仅部分经营类目企业支持；<c>customer_start_chat</c> / <c>message_from_customer</c> /
/// <c>del_follow_user</c> 仅对微信客户生效；自 2024-12-19 起 <c>message_from_customer</c> 不再回调
/// <c>UserID</c>/<c>ExternalUserID</c>/<c>ChatSeq</c>（改经 <see cref="ChatKey"/> 查询收消息数据，凭据约 30 分钟有效）。
/// </para>
/// <para>
/// <b>可建联范围变动</b>：<c>customer_acquisition_permit_change</c>（官方 92277，仅第三方）无业务字段节点，
/// 复用本载荷（全字段为 <c>null</c>），类别由信封判别。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97299">path 97299 获客助手 事件通知（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97402">path 97402 获客助手 事件通知（第三方，套件信封）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/99485">path 99485 获客助手组件（第三方组件形态）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98958">path 98958 获客助手 事件通知（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.CustomerAcquisition,
    SupportedAppTypes = WechatAppTypeSet.Internal | WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.CustomerAcquisition })]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.CustomerAcquisition,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.CustomerAcquisition })]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.CustomerAcquisition,
    SupportedAppTypes = WechatAppTypeSet.ThirdParty,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.CustomerAcquisitionPermitChange })]
public sealed partial class CustomerAcquisitionPayload : WechatCallbackPayload
{
    /// <summary>获客链接 id（官方 <c>LinkId</c>；链接异常/删除/打开/好友请求/收消息/删除成员/组件消耗等事件携带）。</summary>
    [PayloadField("LinkId")]
    public string? LinkId { get; set; }

    /// <summary>获客链接配置的 customer_channel 值（官方 <c>State</c>；用于识别客户来源渠道）。</summary>
    [PayloadField("State")]
    public string? State { get; set; }

    /// <summary>额度过期时间戳（官方 <c>ExpireTime</c>；仅使用量即将过期事件携带，过期前 14/7/3/2/1 天各回调一次）。</summary>
    [PayloadField("ExpireTime")]
    public long? ExpireTime { get; set; }

    /// <summary>即将过期的获客额度数（官方 <c>ExpireQuotaNum</c>；仅使用量即将过期事件携带）。</summary>
    [PayloadField("ExpireQuotaNum")]
    public long? ExpireQuotaNum { get; set; }

    /// <summary>企业服务人员 UserId（官方 <c>UserID</c>；仅收消息/客户删除成员事件携带，2024-12-19 起多次收消息事件不再回调）。</summary>
    [PayloadField("UserID")]
    public string? UserId { get; set; }

    /// <summary>外部联系人的 userid（官方 <c>ExternalUserID</c>；非企业成员账号，2024-12-19 起多次收消息事件不再回调）。</summary>
    [PayloadField("ExternalUserID")]
    public string? ExternalUserId { get; set; }

    /// <summary>成员收消息次数（官方 <c>ChatSeq</c>；仅多次收消息事件携带，第 3/5/10 次触发，2024-12-19 起不再回调）。</summary>
    [PayloadField("ChatSeq")]
    public long? ChatSeq { get; set; }

    /// <summary>
    /// 收消息的会话信息凭据（官方 <c>ChatKey</c>；仅多次收消息事件携带，
    /// 可调「获取获客链接使用成员接受消息数据」接口查询详情，回调后约 30 分钟内有效。
    /// <b>敏感凭据</b>：不得写入日志、遥测或异常消息）。
    /// </summary>
    [PayloadField("ChatKey")]
    public string? ChatKey { get; set; }

    /// <summary>拼接的 once_key 参数（官方 <c>OnceKey</c>；仅服务商使用量消耗事件携带，官方 99485）。</summary>
    [PayloadField("OnceKey")]
    public string? OnceKey { get; set; }

    /// <summary>调整后的获客链接服务商代付价格，单位分（官方 <c>Price</c>；仅价格调整事件携带，官方 99485）。</summary>
    [PayloadField("Price")]
    public long? Price { get; set; }

    /// <summary>价格调整生效时间戳（官方 <c>EffectiveTime</c>；仅价格调整事件携带，官方 99485）。</summary>
    [PayloadField("EffectiveTime")]
    public long? EffectiveTime { get; set; }
}
