// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 礼品卡货架主题（官方 <c>theme_list</c> 项；9 字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftTheme
{
    /// <summary>获取或设置主题图片 URL（官方 <c>theme_pic_url</c>，必填）。</summary>
    [JsonPropertyName("theme_pic_url")]
    public string ThemePicUrl { get; set; } = string.Empty;

    /// <summary>获取或设置主题标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置标题颜色（官方 <c>title_color</c>，必填）。</summary>
    [JsonPropertyName("title_color")]
    public string TitleColor { get; set; } = string.Empty;

    /// <summary>获取或设置卡券列表（官方 <c>item_list</c>，必填）。</summary>
    [JsonPropertyName("item_list")]
    public List<MpCardGiftThemeCardItem> ItemList { get; set; } = new();

    /// <summary>获取或设置图片列表（官方 <c>pic_item_list</c>，必填）。</summary>
    [JsonPropertyName("pic_item_list")]
    public List<MpCardGiftThemePictureItem> PictureItemList { get; set; } = new();

    /// <summary>获取或设置分类索引（官方 <c>category_index</c>，必填）。</summary>
    [JsonPropertyName("category_index")]
    public int CategoryIndex { get; set; }

    /// <summary>获取或设置是否优先展示规格标题（官方 <c>show_sku_title_first</c>，可选）。</summary>
    [JsonPropertyName("show_sku_title_first")]
    public bool? ShowSkuTitleFirst { get; set; }

    /// <summary>获取或设置是否为 Banner 主题（官方 <c>is_banner</c>，可选）。</summary>
    [JsonPropertyName("is_banner")]
    public bool? IsBanner { get; set; }
}
