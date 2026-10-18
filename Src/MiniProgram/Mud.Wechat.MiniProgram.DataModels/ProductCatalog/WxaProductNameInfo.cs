// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 名称信息（官方 <c>name_info</c>）。
/// </summary>
/// <remarks>
/// <para><c>nickname</c> 4~30 字符且平台内唯一；<c>abbr</c> 4~10 字符；<c>introduction</c> 4~120 字符；<c>naming_other_stuff</c> 在错误码 210047 时必填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductNameInfo
{
    /// <summary>小程序名称（必填）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>小程序简称（必填）。</summary>
    [JsonPropertyName("abbr")]
    public string? Abbr { get; set; }

    /// <summary>介绍内容（必填）。</summary>
    [JsonPropertyName("introduction")]
    public string? Introduction { get; set; }

    /// <summary>补充材料 media id 数组（210047 时必填）。</summary>
    [JsonPropertyName("naming_other_stuff")]
    public List<string>? NamingOtherStuff { get; set; }
}
