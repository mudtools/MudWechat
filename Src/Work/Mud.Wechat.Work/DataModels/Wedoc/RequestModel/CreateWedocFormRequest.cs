// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 创建收集表请求体（<c>/cgi-bin/wedoc/create_form</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：问题数组不超过 200 个；定时重复与定时结束互斥，若都填优先定时重复；
/// 开启定时重复时 <c>fill_in_range</c> 必填；各题型的额外设置约束见 <see cref="WedocFormExtendSetting"/> 与 <see cref="WedocFormItem"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class CreateWedocFormRequest
{
    /// <summary>获取或设置空间 spaceid（官方 <c>spaceid</c>，非必填）。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置父目录 fileid（官方 <c>fatherid</c>，非必填），在根目录时为空间 spaceid。</summary>
    [JsonPropertyName("fatherid")]
    public string? Fatherid { get; set; }

    /// <summary>获取或设置收集表信息（官方 <c>form_info</c>，必填）。</summary>
    [JsonPropertyName("form_info")]
    public WedocFormInfo? FormInfo { get; set; }
}
