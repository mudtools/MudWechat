// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表选择题「其他」选项的答案（官方 <c>option_extend_reply</c> 元素；读取收集表答案响应体嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormOptionExtendReply
{
    /// <summary>获取或设置其他选项的答案 id（官方 <c>option_reply</c>）。</summary>
    [JsonPropertyName("option_reply")]
    public uint? OptionReply { get; set; }

    /// <summary>获取或设置其他选项的答案字符串（官方 <c>extend_text</c>）。</summary>
    [JsonPropertyName("extend_text")]
    public string? ExtendText { get; set; }
}
