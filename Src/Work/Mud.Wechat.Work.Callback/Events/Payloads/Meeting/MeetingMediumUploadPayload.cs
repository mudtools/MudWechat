// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>素材上传结果事件载荷（<c>medium_upload</c>；官方 path 98775 自建）。</summary>
/// <remarks>
/// <para>
/// 通过 API 上传素材（视频、图片、文件）后，有上传结果时推送。系统触发 ⇒ 信封 <c>FromUserName</c> 固定 <c>sys</c>。
/// </para>
/// <para>
/// <b>官方业务限制（不得弱化）</b>：<see cref="AllUploadStatus"/> 为 <c>false</c> 时
/// 「存在素材上传失败，需重新上传<b>全部</b>素材」——处理器不得只重传失败项。
/// </para>
/// <para>
/// <b>对象列表形态（全仓首个）</b>：<see cref="UploadInfos"/> 的官方 <c>UploadInfo</c> 是
/// <b>根下重复同名兄弟元素</b>（无包装容器），经根层同名兄弟合并投影 +
/// <see cref="WechatPayloadConverter.RepeatMediumUploadItems"/> 读取（单元素报文同样覆盖）。
/// </para>
/// <para>
/// <b>开放面</b>：官方仅在企业自建文档树提供该事件（第三方/代开发无对应回调）⇒ 仅自建开放。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/98775">path 98775 素材上传结果（企业自建）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.MeetingChange,
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.MediumUpload })]
public sealed partial class MeetingMediumUploadPayload : WechatCallbackPayload
{
    /// <summary>会议 ID（官方 <c>MeetingId</c>）。</summary>
    [PayloadField("MeetingId")]
    public string? MeetingId { get; set; }

    /// <summary>
    /// 上传事件是否成功（官方 <c>AllUploadStatus</c>）：true 全部素材上传成功；
    /// false 存在素材上传失败，需重新上传全部素材。
    /// </summary>
    [PayloadField("AllUploadStatus")]
    public bool? AllUploadStatus { get; set; }

    /// <summary>上传的素材对象列表（官方 <c>UploadInfo</c>，根下重复同名兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    [PayloadField("UploadInfo", Method = nameof(WechatPayloadConverter.RepeatMediumUploadItems))]
    public List<WechatCallbackMeetingMediumUploadItem> UploadInfos { get; set; } =
        new List<WechatCallbackMeetingMediumUploadItem>();
}
