// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 礼品卡货架（官方 <c>page</c>；add 形态 12 字段，update 追加 page_id）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPage
{
    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>；仅 update 形态使用）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置货架 Banner 图（官方 <c>banner_pic_url</c>，必填）。</summary>
    [JsonPropertyName("banner_pic_url")]
    public string BannerPicUrl { get; set; } = string.Empty;

    /// <summary>获取或设置货架标题（官方 <c>page_title</c>，必填）。</summary>
    [JsonPropertyName("page_title")]
    public string PageTitle { get; set; } = string.Empty;

    /// <summary>获取或设置是否支持多主题（官方 <c>support_multi</c>，可选）。</summary>
    [JsonPropertyName("support_multi")]
    public bool? SupportMulti { get; set; }

    /// <summary>获取或设置是否支持自购（官方 <c>support_buy_for_self</c>，可选）。</summary>
    [JsonPropertyName("support_buy_for_self")]
    public bool? SupportBuyForSelf { get; set; }

    /// <summary>获取或设置主题列表（官方 <c>theme_list</c>，可选）。</summary>
    [JsonPropertyName("theme_list")]
    public List<MpCardGiftTheme>? ThemeList { get; set; }

    /// <summary>获取或设置分类列表（官方 <c>category_list</c>，可选）。</summary>
    [JsonPropertyName("category_list")]
    public List<MpCardGiftCategory>? CategoryList { get; set; }

    /// <summary>获取或设置货架地址（官方 <c>address</c>，必填）。</summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>获取或设置客服电话（官方 <c>service_phone</c>，必填）。</summary>
    [JsonPropertyName("service_phone")]
    public string ServicePhone { get; set; } = string.Empty;

    /// <summary>获取或设置商户描述（官方 <c>biz_description</c>，必填）。</summary>
    [JsonPropertyName("biz_description")]
    public string BizDescription { get; set; } = string.Empty;

    /// <summary>获取或设置是否需要发票（官方 <c>need_receipt</c>，可选）。</summary>
    [JsonPropertyName("need_receipt")]
    public bool? NeedReceipt { get; set; }

    /// <summary>获取或设置自定义单元格 1（官方 <c>cell_1</c>，可选）。</summary>
    [JsonPropertyName("cell_1")]
    public MpCardGiftCustomCell? Cell1 { get; set; }

    /// <summary>获取或设置自定义单元格 2（官方 <c>cell_2</c>，可选）。</summary>
    [JsonPropertyName("cell_2")]
    public MpCardGiftCustomCell? Cell2 { get; set; }
}
