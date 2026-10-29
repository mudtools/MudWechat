// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 说明文字内容（Tips 控件的 value.new_tips 与模板 config.tips 共用结构，元素为不同语言的富文本说明文字）。
/// </summary>
/// <remarks>
/// <para>官方限制：纯文本+链接标题总长度不能超过 800 个字符；每个说明文字中只支持包含一个链接。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTipsContent
{
    /// <summary>
    /// 获取或设置说明文字数组，元素为不同语言的富文本说明文字。
    /// </summary>
    [JsonPropertyName("tips_content")]
    public List<ApprovalTipsContentItem>? TipsContent { get; set; }
}
