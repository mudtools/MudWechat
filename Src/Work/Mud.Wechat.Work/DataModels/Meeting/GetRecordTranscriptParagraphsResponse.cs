// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取录制转写段落信息响应体（<c>/cgi-bin/meeting/record/transcript/get_paragraph_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetRecordTranscriptParagraphsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置声纹识别状态：0 - 未完成；1 - 已完成。
    /// <para>声纹识别是针对一台设备多人讲话场景自动区分为多个发言人的能力，与录制转写生成过程独立；无需声纹识别或声纹识别已完成时该值为 1。</para>
    /// </summary>
    [JsonPropertyName("audio_detect")]
    public int? AudioDetect { get; set; }

    /// <summary>获取或设置段落列表（详见 <see cref="RecordTranscriptParagraph"/>）。</summary>
    [JsonPropertyName("paragraphs")]
    public List<RecordTranscriptParagraph>? Paragraphs { get; set; }
}
