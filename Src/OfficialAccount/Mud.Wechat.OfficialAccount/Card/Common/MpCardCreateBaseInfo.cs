// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- base_info 三形态
//  建卡 / 查卡 / 改卡三张 base_info 表**逐项不同**（详见各类型 remarks），按本仓「表相同则共用、
//  表不同则分建」纪律分建三型；shared 的子表（date_info / advanced_info 族）只建一次。

/// <summary>
/// 卡券基本信息（官方 <c>base_info</c>，<b>建卡方向</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>对齐基准</b>：SKIT <c>CardCreateRequest.Types.GrouponCard.Types.Base</c>（2026-10-10），
/// 官方「创建卡券」页逐页核验待补。字段名照官方原文，<b>不得驼峰化</b>（守卫 CD3 锁定）。
/// </para>
/// <para>
/// <b>与查询/修改方向的差异</b>（三者不可互相替换）：本型独有 <c>sub_merchant_info</c> / <c>sku</c> /
/// <c>use_custom_code</c> / <c>get_custom_code_mode</c> / <c>bind_openid</c> 与六个
/// <c>*_app_brand_user_name</c> / <c>*_app_brand_pass</c> 小程序入口字段；<b>无</b> <c>id</c> /
/// <c>status</c> / <c>create_time</c> / <c>update_time</c>（模板号与状态由官方生成，建卡时不得携带）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateBaseInfo
{
    /// <summary>获取或设置子商户信息（官方 <c>sub_merchant_info</c>，选填；<b>官方键名即为 <c>sub_merchant_info</c>，非 <c>sub_merchant</c></b>）。</summary>
    [JsonPropertyName("sub_merchant_info")]
    public MpCardSubMerchantInfo? SubMerchantInfo { get; set; }

    /// <summary>获取或设置商户 Logo URL（官方 <c>logo_url</c>，必填，须为微信素材接口取到的图片 URL）。</summary>
    [JsonPropertyName("logo_url")]
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>获取或设置券码类型（官方 <c>code_type</c>，必填；取值表<b>待官方逐页核验</b>，SDK 不建枚举常量）。</summary>
    [JsonPropertyName("code_type")]
    public string CodeType { get; set; } = string.Empty;

    /// <summary>获取或设置商户名（官方 <c>brand_name</c>，必填，30 字符以内）。</summary>
    [JsonPropertyName("brand_name")]
    public string BrandName { get; set; } = string.Empty;

    /// <summary>获取或设置卡券名（官方 <c>title</c>，必填，单品类 12 字符以内 / 多品类 30 字符以内）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置卡券颜色（官方 <c>color</c>，必填；官方给出的 12 种色值枚举<b>待逐页核验</b>，SDK 不建枚举常量）。</summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    /// <summary>获取或设置使用提醒（官方 <c>notice</c>，必填；类型不同上限不同，会员卡外 12 字符）。</summary>
    [JsonPropertyName("notice")]
    public string Notice { get; set; } = string.Empty;

    /// <summary>获取或设置使用说明（官方 <c>description</c>，必填，700 字符以内）。</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>获取或设置商品信息（官方 <c>sku</c>，建卡必填，见 <see cref="MpCardCreateSku"/>）。</summary>
    [JsonPropertyName("sku")]
    public MpCardCreateSku? Sku { get; set; }

    /// <summary>获取或设置使用日期（官方 <c>date_info</c>，必填，见 <see cref="MpCardDateInfo"/>）。</summary>
    [JsonPropertyName("date_info")]
    public MpCardDateInfo? DateInfo { get; set; }

    /// <summary>获取或设置是否自定义券码（官方 <c>use_custom_code</c>，选填）。</summary>
    [JsonPropertyName("use_custom_code")]
    public bool? UseCustomCode { get; set; }

    /// <summary>获取或设置自定义券码模式（官方 <c>get_custom_code_mode</c>，选填；取值<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("get_custom_code_mode")]
    public string? GetCustomCodeMode { get; set; }

    /// <summary>获取或设置是否绑定 openid（官方 <c>bind_openid</c>，选填）。</summary>
    [JsonPropertyName("bind_openid")]
    public bool? BindOpenId { get; set; }

    /// <summary>获取或设置客服电话（官方 <c>service_phone</c>，选填）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>获取或设置门店 id 列表（官方 <c>location_id_list</c>，选填；门店面为单批上限极小的整型 id）。</summary>
    [JsonPropertyName("location_id_list")]
    public List<long>? LocationIdList { get; set; }

    /// <summary>获取或设置是否全部门店可用（官方 <c>use_all_locations</c>，选填）。</summary>
    [JsonPropertyName("use_all_locations")]
    public bool? UseAllLocations { get; set; }

    /// <summary>获取或设置顶部居中入口文字（官方 <c>center_title</c>，选填，5 字符以内）。</summary>
    [JsonPropertyName("center_title")]
    public string? CenterTitle { get; set; }

    /// <summary>获取或设置顶部居中入口提示语（官方 <c>center_sub_title</c>，选填，10 字符以内）。</summary>
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

    /// <summary>获取或设置自定义入口名称（官方 <c>custom_url_name</c>，选填，5 字符以内）。</summary>
    [JsonPropertyName("custom_url_name")]
    public string? CustomUrlName { get; set; }

    /// <summary>获取或设置自定义入口提示语（官方 <c>custom_url_sub_title</c>，选填，6 字符以内）。</summary>
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

    /// <summary>获取或设置营销入口名称（官方 <c>promotion_url_name</c>，选填，5 字符以内）。</summary>
    [JsonPropertyName("promotion_url_name")]
    public string? PromotionUrlName { get; set; }

    /// <summary>获取或设置营销入口提示语（官方 <c>promotion_url_sub_title</c>，选填，6 字符以内）。</summary>
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

    /// <summary>获取或设置每人可领券数上限（官方 <c>get_limit</c>，选填，0 表示不限）。</summary>
    [JsonPropertyName("get_limit")]
    public int? GetLimit { get; set; }

    /// <summary>获取或设置每人可核销数上限（官方 <c>use_limit</c>，选填，0 表示不限）。</summary>
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
