// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>网络研讨会暖场上传结果事件载荷（<c>webinar_warm_up_upload</c>；官方 path 98773 自建）。</summary>
/// <remarks>
/// <para>
/// 仅 API 创建的网络研讨会触发：通过 API 上传暖场图片或视频后推送上传结果。
/// 系统触发 ⇒ 信封 <c>FromUserName</c> 固定 <c>sys</c>。
/// </para>
/// <para>
/// 官方页面 <see cref="WarmUpInfo"/> 内的 CDATA 闭合标签缺 <c>&gt;</c>（官方原文如此），
/// 字段语义以参数表为准。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建文档树提供该事件（第三方/代开发无对应回调）⇒ 仅自建开放。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/98773">path 98773 网络研讨会暖场上传结果（企业自建）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.WebinarWarmUpUpload })]
public sealed partial class MeetingWarmUpUploadPayload : WechatCallbackPayload
{
    /// <summary>会议 ID（官方 <c>MeetingId</c>）。</summary>
    [PayloadField("MeetingId")]
    public string? MeetingId { get; set; }

    /// <summary>暖场配置信息（官方 <c>WarmUpInfo</c>：WarmUpPicture + WarmUpVideo + UploadStatus + ErrorMsg；节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("WarmUpInfo")]
    public WechatCallbackMeetingWarmUpInfo? WarmUpInfo { get; set; }
}
