// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 网络研讨会「暖场配置信息」对象（<c>webinar_warm_up_upload</c> 事件的 <c>WarmUpInfo</c> 节点；官方 path 98773）。
/// </summary>
/// <remarks>
/// 经上游 G-ADR-17 的 <c>Object</c> 嵌套通道声明化：单节点对象（非列表），元素缺失 ⇒ <c>null</c>。
/// 官方页面该节内三个 CDATA 值的闭合标签缺 <c>&gt;</c>（官方原文如此），语义以参数表为准。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackMeetingWarmUpInfo
{
    /// <summary>暖场图片地址（官方 <c>WarmUpInfo/WarmUpPicture</c>）。</summary>
    [PayloadField("WarmUpPicture")]
    public string? WarmUpPicture { get; set; }

    /// <summary>暖场视频地址（官方 <c>WarmUpInfo/WarmUpVideo</c>）。</summary>
    [PayloadField("WarmUpVideo")]
    public string? WarmUpVideo { get; set; }

    /// <summary>上传结果（官方 <c>WarmUpInfo/UploadStatus</c>：1 成功 / 0 失败）。</summary>
    [PayloadField("UploadStatus")]
    public long? UploadStatus { get; set; }

    /// <summary>上传失败时的失败原因（官方 <c>WarmUpInfo/ErrorMsg</c>；成功时缺失）。</summary>
    [PayloadField("ErrorMsg")]
    public string? ErrorMsg { get; set; }
}
