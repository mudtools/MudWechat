// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>直播状态变更事件载荷（<c>living_status_change</c>；官方 path 94145/94308/96842）。</summary>
/// <remarks>
/// <para>
/// 一场完整直播会经历预约/开始/结束等状态变更，状态变化后推送本事件（无 <c>ChangeType</c> 分组段，
/// 状态经 <see cref="Status"/> 判别）。<b>仅 API 创建的预约/立即直播才会回调</b>，且调用创建直播接口的
/// 应用要配置好回调 URL（官方原文）。
/// </para>
/// <para>
/// <b>闭合值域</b>：<see cref="Status"/> —— 0 预约中 / 1 直播中 / 2 已结束 / 4 已取消；
/// 3 已过期仅第三方页文档列出且注明「目前没有回调」（可空超集容忍）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/94145">path 94145 直播回调事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/94308">path 94308（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96842">path 96842（服务商代开发）</see>
/// （三份 XML 逐字节一致，ADR-14）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.LivingStatusChange })]
public sealed partial class LivingStatusChangedPayload : WechatCallbackPayload
{
    /// <summary>直播 ID（官方 <c>LivingId</c>）。</summary>
    [PayloadField("LivingId")]
    public string? LivingId { get; set; }

    /// <summary>
    /// 直播状态（官方 <c>Status</c>，闭合值域）：0 预约中 / 1 直播中 / 2 已结束 / 4 已取消；
    /// 3 已过期（仅第三方文档列出，注明目前没有回调）。
    /// </summary>
    [PayloadField("Status")]
    public long? Status { get; set; }

    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页面查看）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }
}
