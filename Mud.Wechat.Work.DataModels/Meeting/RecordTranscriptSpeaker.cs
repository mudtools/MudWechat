// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 录制转写发言人对象（获取录制转写详情响应 <c>paragraphs.speaker_info</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RecordTranscriptSpeaker
{
    /// <summary>
    /// 获取或设置发言人对应的企业成员 userid。
    /// <para>官方条件返回：仅当发言人属于同一企业时返回；非本企业成员不返回该字段。</para>
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }
}
