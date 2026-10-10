// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 录制转写搜索命中对象（搜索录制转写响应 <c>hits</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RecordTranscriptHit
{
    /// <summary>获取或设置命中段落 ID。</summary>
    [JsonPropertyName("pid")]
    public string? Pid { get; set; }

    /// <summary>获取或设置命中句子 ID。</summary>
    [JsonPropertyName("sid")]
    public string? Sid { get; set; }

    /// <summary>获取或设置命中偏移量。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置命中长度。</summary>
    [JsonPropertyName("length")]
    public int? Length { get; set; }
}
