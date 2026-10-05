// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表单个问题的答案（官方 <c>reply.items</c> 元素；读取收集表答案响应体嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：答案内容按题型承载于不同字段——文本/位置/日期/时间/体温/签名等以 <c>text_reply</c> 字符串承载，
/// 单选/多选/下拉列表以 <c>option_reply</c> 承载，图片/文件以 <c>file_extend_reply</c> 承载，
/// 部门/成员/时长分别以 <c>department_reply</c> / <c>member_reply</c> / <c>duration_reply</c> 承载，取值时须按题型判别。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormReplyItem
{
    /// <summary>获取或设置问题 id（官方 <c>question_id</c>）。</summary>
    [JsonPropertyName("question_id")]
    public ulong? QuestionId { get; set; }

    /// <summary>获取或设置文本类答案（官方 <c>text_reply</c>），文本/位置/日期/时间/体温/签名等题型以字符串承载。</summary>
    [JsonPropertyName("text_reply")]
    public string? TextReply { get; set; }

    /// <summary>获取或设置选择题答案（官方 <c>option_reply</c>），多选题有多个答案，取值为选项 key 列表。</summary>
    [JsonPropertyName("option_reply")]
    public List<uint>? OptionReply { get; set; }

    /// <summary>获取或设置选择题「其他」选项列表（官方 <c>option_extend_reply</c>）。</summary>
    [JsonPropertyName("option_extend_reply")]
    public List<WedocFormOptionExtendReply>? OptionExtendReply { get; set; }

    /// <summary>获取或设置文件题答案列表（官方 <c>file_extend_reply</c>）。</summary>
    [JsonPropertyName("file_extend_reply")]
    public List<WedocFormFileReply>? FileExtendReply { get; set; }

    /// <summary>获取或设置部门题答案（官方 <c>department_reply</c>）。</summary>
    [JsonPropertyName("department_reply")]
    public WedocFormDepartmentReply? DepartmentReply { get; set; }

    /// <summary>获取或设置成员题答案（官方 <c>member_reply</c>）。</summary>
    [JsonPropertyName("member_reply")]
    public WedocFormMemberReply? MemberReply { get; set; }

    /// <summary>获取或设置时长题答案（官方 <c>duration_reply</c>）。</summary>
    [JsonPropertyName("duration_reply")]
    public WedocFormDurationReply? DurationReply { get; set; }
}
