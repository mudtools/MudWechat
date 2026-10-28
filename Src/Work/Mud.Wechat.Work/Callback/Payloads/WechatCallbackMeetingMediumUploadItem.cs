// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 会议「素材上传结果」事件（<c>medium_upload</c>，官方 path 98775）的单个素材项
/// （官方 <c>UploadInfo</c>，根下重复同名兄弟元素、无包装容器）。
/// </summary>
/// <remarks>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；元素定位与「单/多形态分派」由
/// <c>WechatPayloadConverter.RepeatMediumUploadItems</c> 承担（该事件为全仓首个「根下重复复杂兄弟」
/// 官方形态，依赖根层同名兄弟合并投影）。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackMeetingMediumUploadItem
{
    /// <summary>素材对象的地址（官方 <c>MediumUrl</c>）。</summary>
    [PayloadField("MediumUrl")]
    public string? MediumUrl { get; set; }

    /// <summary>素材类型（官方 <c>MediumType</c>：1 视频 / 2 图片 / 3 文件）。</summary>
    [PayloadField("MediumType")]
    public long? MediumType { get; set; }

    /// <summary>素材的上传结果（官方 <c>UploadStatus</c>：1 成功 / 0 失败）。</summary>
    [PayloadField("UploadStatus")]
    public long? UploadStatus { get; set; }

    /// <summary>上传失败时的失败原因（官方 <c>ErrorMsg</c>；成功时缺失）。</summary>
    [PayloadField("ErrorMsg")]
    public string? ErrorMsg { get; set; }
}
