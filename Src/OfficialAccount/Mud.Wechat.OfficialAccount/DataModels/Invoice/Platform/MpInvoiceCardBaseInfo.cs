// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 发票卡券模板基础信息（官方 <c>invoice_info.base_info</c>）。
/// </summary>
/// <remarks>
/// 官方原文约束：<c>title</c>（收款方列表显示）<b>上限 9 个汉字</b>；<c>custom_url_name</c> /
/// <c>promotion_url_name</c> ≤ 5 汉字且与对应 url 同用；两个 <c>*_sub_title</c> ≤ 6 汉字；
/// <c>logo_url</c> 须走永久素材接口（官方字段说明原文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCardBaseInfo
{
    /// <summary>获取或设置发票商家 LOGO（官方 <c>logo_url</c>，必填；须走永久素材接口）。</summary>
    [JsonPropertyName("logo_url")]
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>获取或设置收款方（官方 <c>title</c>，必填；列表显示，上限 9 个汉字）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置自定义入口名称（官方 <c>custom_url_name</c>；≤5 汉字，与 custom_url 同用）。</summary>
    [JsonPropertyName("custom_url_name")]
    public string? CustomUrlName { get; set; }

    /// <summary>获取或设置自定义入口外链（官方 <c>custom_url</c>；会携带发票参数）。</summary>
    [JsonPropertyName("custom_url")]
    public string? CustomUrl { get; set; }

    /// <summary>获取或设置自定义入口右侧 tips（官方 <c>custom_url_sub_title</c>；≤6 汉字）。</summary>
    [JsonPropertyName("custom_url_sub_title")]
    public string? CustomUrlSubTitle { get; set; }

    /// <summary>获取或设置营销场景自定义入口名称（官方 <c>promotion_url_name</c>）。</summary>
    [JsonPropertyName("promotion_url_name")]
    public string? PromotionUrlName { get; set; }

    /// <summary>获取或设置营销场景入口外链（官方 <c>promotion_url</c>；会携带发票参数）。</summary>
    [JsonPropertyName("promotion_url")]
    public string? PromotionUrl { get; set; }

    /// <summary>获取或设置营销入口右侧 tips（官方 <c>promotion_url_sub_title</c>；≤6 汉字）。</summary>
    [JsonPropertyName("promotion_url_sub_title")]
    public string? PromotionUrlSubTitle { get; set; }
}
