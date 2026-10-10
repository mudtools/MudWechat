// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 代码内容块属性（官方 CodeBlockProps，对应 <c>BLOCK_TYPE_CODE</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocCodeBlockProps
{
    /// <summary>
    /// 获取或设置代码语言（官方 <c>code_language</c>，见官方 <c>CodeLanguage</c>）。
    /// <para>官方取值为 <c>CODE_LANGUAGE_</c> 前缀的字符串枚举（如 <c>CODE_LANGUAGE_PYTHON</c>、<c>CODE_LANGUAGE_JAVA</c>、
    /// <c>CODE_LANGUAGE_MARKDOWN</c>、<c>CODE_LANGUAGE_PLAIN_TEXT</c>、<c>CODE_LANGUAGE_UNSPECIFIED</c> 等，
    /// 共八十余种，本仓以字符串承载以兼容官方枚举全量取值。</para>
    /// </summary>
    [JsonPropertyName("code_language")]
    public string? CodeLanguage { get; set; }

    /// <summary>获取或设置是否自动换行（官方 <c>code_wrap</c>）。</summary>
    [JsonPropertyName("code_wrap")]
    public bool? CodeWrap { get; set; }
}
