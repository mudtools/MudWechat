// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 卡券基本信息（官方 <c>base_info</c>，<b>修改方向</b>）—— <b>全部字段选填</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>与建卡方向的差异</b>（守卫 CD2 锁定）：本型<b>不含</b> <c>sub_merchant_info</c> / <c>sku</c> /
/// <c>use_custom_code</c> / <c>get_custom_code_mode</c> / <c>bind_openid</c>（券码与库存形态建卡后不可改，
/// 库存走 <c>/card/modifystock</c>）；其余字段与建卡同名但<b>语义为「传即覆盖」</b>，故一律可空。
/// </para>
/// <para><b>含六个 <c>*_app_brand_*</c> 小程序入口字段</b>（与查询方向不同 —— 查询应答不回传该组）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateBaseInfo
{
    /// <summary>获取或设置商户 Logo URL（官方 <c>logo_url</c>，选填）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>获取或设置券码类型（官方 <c>code_type</c>，选填）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>获取或设置商户名（官方 <c>brand_name</c>，选填）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>获取或设置卡券名（官方 <c>title</c>，选填）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置卡券颜色（官方 <c>color</c>，选填）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>获取或设置使用提醒（官方 <c>notice</c>，选填）。</summary>
    [JsonPropertyName("notice")]
    public string? Notice { get; set; }

    /// <summary>获取或设置使用说明（官方 <c>description</c>，选填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置使用日期（官方 <c>date_info</c>，选填，与建卡同表）。</summary>
    [JsonPropertyName("date_info")]
    public MpCardDateInfo? DateInfo { get; set; }

    /// <summary>获取或设置客服电话（官方 <c>service_phone</c>，选填）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>获取或设置门店 id 列表（官方 <c>location_id_list</c>，选填）。</summary>
    [JsonPropertyName("location_id_list")]
    public List<long>? LocationIdList { get; set; }

    /// <summary>获取或设置是否全部门店可用（官方 <c>use_all_locations</c>，选填）。</summary>
    [JsonPropertyName("use_all_locations")]
    public bool? UseAllLocations { get; set; }

    /// <summary>获取或设置顶部居中入口文字（官方 <c>center_title</c>，选填）。</summary>
    [JsonPropertyName("center_title")]
    public string? CenterTitle { get; set; }

    /// <summary>获取或设置顶部居中入口提示语（官方 <c>center_sub_title</c>，选填）。</summary>
    [JsonPropertyName("center_sub_title")]
    public string? CenterSubTitle { get; set; }

    /// <summary>获取或设置顶部居中入口外链（官方 <c>center_url</c>，选填）。</summary>
    [JsonPropertyName("center_url")]
    public string? CenterUrl { get; set; }

    /// <summary>获取或设置顶部居中入口的小程序原始 id（官方 <c>center_app_brand_user_name</c>，选填）。</summary>
    [JsonPropertyName("center_app_brand_user_name")]
    public string? CenterAppBrandUserName { get; set; }

    /// <summary>获取或设置顶部居中入口的小程序页面路径（官方 <c>center_app_brand_pass</c>，选填）。</summary>
    [JsonPropertyName("center_app_brand_pass")]
    public string? CenterAppBrandPass { get; set; }

    /// <summary>获取或设置自定义入口名称（官方 <c>custom_url_name</c>，选填）。</summary>
    [JsonPropertyName("custom_url_name")]
    public string? CustomUrlName { get; set; }

    /// <summary>获取或设置自定义入口提示语（官方 <c>custom_url_sub_title</c>，选填）。</summary>
    [JsonPropertyName("custom_url_sub_title")]
    public string? CustomUrlSubTitle { get; set; }

    /// <summary>获取或设置自定义入口外链（官方 <c>custom_url</c>，选填）。</summary>
    [JsonPropertyName("custom_url")]
    public string? CustomUrl { get; set; }

    /// <summary>获取或设置自定义入口的小程序原始 id（官方 <c>custom_app_brand_user_name</c>，选填）。</summary>
    [JsonPropertyName("custom_app_brand_user_name")]
    public string? CustomAppBrandUserName { get; set; }

    /// <summary>获取或设置自定义入口的小程序页面路径（官方 <c>custom_app_brand_pass</c>，选填）。</summary>
    [JsonPropertyName("custom_app_brand_pass")]
    public string? CustomAppBrandPass { get; set; }

    /// <summary>获取或设置营销入口名称（官方 <c>promotion_url_name</c>，选填）。</summary>
    [JsonPropertyName("promotion_url_name")]
    public string? PromotionUrlName { get; set; }

    /// <summary>获取或设置营销入口提示语（官方 <c>promotion_url_sub_title</c>，选填）。</summary>
    [JsonPropertyName("promotion_url_sub_title")]
    public string? PromotionUrlSubTitle { get; set; }

    /// <summary>获取或设置营销入口外链（官方 <c>promotion_url</c>，选填）。</summary>
    [JsonPropertyName("promotion_url")]
    public string? PromotionUrl { get; set; }

    /// <summary>获取或设置营销入口的小程序原始 id（官方 <c>promotion_app_brand_user_name</c>，选填）。</summary>
    [JsonPropertyName("promotion_app_brand_user_name")]
    public string? PromotionAppBrandUserName { get; set; }

    /// <summary>获取或设置营销入口的小程序页面路径（官方 <c>promotion_app_brand_pass</c>，选填）。</summary>
    [JsonPropertyName("promotion_app_brand_pass")]
    public string? PromotionAppBrandPass { get; set; }

    /// <summary>获取或设置每人可领券数上限（官方 <c>get_limit</c>，选填）。</summary>
    [JsonPropertyName("get_limit")]
    public int? GetLimit { get; set; }

    /// <summary>获取或设置每人可核销数上限（官方 <c>use_limit</c>，选填）。</summary>
    [JsonPropertyName("use_limit")]
    public int? UseLimit { get; set; }

    /// <summary>获取或设置领取页面是否可分享（官方 <c>can_share</c>，选填）。</summary>
    [JsonPropertyName("can_share")]
    public bool? CanShare { get; set; }

    /// <summary>获取或设置是否可转赠好友（官方 <c>can_give_friend</c>，选填）。</summary>
    [JsonPropertyName("can_give_friend")]
    public bool? CanGiveFriend { get; set; }

    /// <summary>获取或设置第三方来源名（官方 <c>source</c>，选填）。</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }
}
