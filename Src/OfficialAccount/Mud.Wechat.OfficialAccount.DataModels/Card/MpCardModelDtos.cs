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

/// <summary>
/// 卡券基本信息（官方 <c>base_info</c>，<b>查询方向</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与建卡方向的差异</b>（守卫 CD2 锁定，两者不得合并）：本型<b>新增</b> <c>id</c> / <c>status</c> /
/// <c>create_time</c> / <c>update_time</c>；<b>不含</b> <c>sub_merchant_info</c>、<c>sku</c> 的建卡形态
/// （查询返回带 <c>total_quantity</c>，见 <see cref="MpCardQuerySku"/>）与六个 <c>*_app_brand_*</c> 小程序入口字段。
/// </para>
/// <para>
/// <b>方向不可逆</b>：本型是应答形态，多个标量在应答侧为「必有值」（非可空），不得拿去做请求体。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQueryBaseInfo
{
    /// <summary>获取或设置卡券模板编号（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置商户 Logo URL（官方 <c>logo_url</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>获取或设置券码类型（官方 <c>code_type</c>）。</summary>
    [JsonPropertyName("code_type")]
    public string? CodeType { get; set; }

    /// <summary>获取或设置商户名（官方 <c>brand_name</c>）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>获取或设置卡券名（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置卡券颜色（官方 <c>color</c>）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>获取或设置使用提醒（官方 <c>notice</c>）。</summary>
    [JsonPropertyName("notice")]
    public string? Notice { get; set; }

    /// <summary>获取或设置使用说明（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置商品信息（官方 <c>sku</c>，查询形态含剩余与总量，见 <see cref="MpCardQuerySku"/>）。</summary>
    [JsonPropertyName("sku")]
    public MpCardQuerySku? Sku { get; set; }

    /// <summary>获取或设置使用日期（官方 <c>date_info</c>，与建卡同表，复用 <see cref="MpCardDateInfo"/>）。</summary>
    [JsonPropertyName("date_info")]
    public MpCardDateInfo? DateInfo { get; set; }

    /// <summary>获取或设置卡券状态（官方 <c>status</c>；状态枚举值<b>待官方逐页核验</b>，SDK 不建枚举常量）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置是否自定义券码（官方 <c>use_custom_code</c>）。</summary>
    [JsonPropertyName("use_custom_code")]
    public bool UseCustomCode { get; set; }

    /// <summary>获取或设置自定义券码模式（官方 <c>get_custom_code_mode</c>）。</summary>
    [JsonPropertyName("get_custom_code_mode")]
    public string? GetCustomCodeMode { get; set; }

    /// <summary>获取或设置是否绑定 openid（官方 <c>bind_openid</c>）。</summary>
    [JsonPropertyName("bind_openid")]
    public bool? BindOpenId { get; set; }

    /// <summary>获取或设置客服电话（官方 <c>service_phone</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>获取或设置门店 id 列表（官方 <c>location_id_list</c>）。</summary>
    [JsonPropertyName("location_id_list")]
    public List<long>? LocationIdList { get; set; }

    /// <summary>获取或设置是否全部门店可用（官方 <c>use_all_locations</c>）。</summary>
    [JsonPropertyName("use_all_locations")]
    public bool? UseAllLocations { get; set; }

    /// <summary>获取或设置顶部居中入口文字（官方 <c>center_title</c>）。</summary>
    [JsonPropertyName("center_title")]
    public string? CenterTitle { get; set; }

    /// <summary>获取或设置顶部居中入口提示语（官方 <c>center_sub_title</c>）。</summary>
    [JsonPropertyName("center_sub_title")]
    public string? CenterSubTitle { get; set; }

    /// <summary>获取或设置顶部居中入口外链（官方 <c>center_url</c>）。</summary>
    [JsonPropertyName("center_url")]
    public string? CenterUrl { get; set; }

    /// <summary>获取或设置自定义入口名称（官方 <c>custom_url_name</c>）。</summary>
    [JsonPropertyName("custom_url_name")]
    public string? CustomUrlName { get; set; }

    /// <summary>获取或设置自定义入口提示语（官方 <c>custom_url_sub_title</c>）。</summary>
    [JsonPropertyName("custom_url_sub_title")]
    public string? CustomUrlSubTitle { get; set; }

    /// <summary>获取或设置自定义入口外链（官方 <c>custom_url</c>）。</summary>
    [JsonPropertyName("custom_url")]
    public string? CustomUrl { get; set; }

    /// <summary>获取或设置营销入口名称（官方 <c>promotion_url_name</c>）。</summary>
    [JsonPropertyName("promotion_url_name")]
    public string? PromotionUrlName { get; set; }

    /// <summary>获取或设置营销入口提示语（官方 <c>promotion_url_sub_title</c>）。</summary>
    [JsonPropertyName("promotion_url_sub_title")]
    public string? PromotionUrlSubTitle { get; set; }

    /// <summary>获取或设置营销入口外链（官方 <c>promotion_url</c>）。</summary>
    [JsonPropertyName("promotion_url")]
    public string? PromotionUrl { get; set; }

    /// <summary>获取或设置每人可领券数上限（官方 <c>get_limit</c>）。</summary>
    [JsonPropertyName("get_limit")]
    public int? GetLimit { get; set; }

    /// <summary>获取或设置每人可核销数上限（官方 <c>use_limit</c>）。</summary>
    [JsonPropertyName("use_limit")]
    public int? UseLimit { get; set; }

    /// <summary>获取或设置领取页面是否可分享（官方 <c>can_share</c>）。</summary>
    [JsonPropertyName("can_share")]
    public bool CanShare { get; set; }

    /// <summary>获取或设置是否可转赠好友（官方 <c>can_give_friend</c>）。</summary>
    [JsonPropertyName("can_give_friend")]
    public bool CanGiveFriend { get; set; }

    /// <summary>获取或设置第三方来源名（官方 <c>source</c>）。</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>获取或设置最近更新时间戳（官方 <c>update_time</c>，秒）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>获取或设置创建时间戳（官方 <c>create_time</c>，秒）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}

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

// ---------------------------------------------------------------- 跨向共用子表

/// <summary>
/// 卡券高级信息（官方 <c>advanced_info</c>）—— 建卡与查询<b>两向同表</b>，故共用一型（守卫 CD2 锁定复用关系）。
/// </summary>
/// <remarks><b>修改方向无本对象</b>：官方「修改卡券」只开放 <c>base_info</c> 与各券型标量，不含 <c>advanced_info</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardAdvancedInfo
{
    /// <summary>获取或设置使用门槛（官方 <c>use_condition</c>，选填）。</summary>
    [JsonPropertyName("use_condition")]
    public MpCardUseCondition? UseCondition { get; set; }

    /// <summary>获取或设置封面摘要（官方 <c>abstract</c>，选填；<b>官方键名为 <c>abstract</c></b>）。</summary>
    [JsonPropertyName("abstract")]
    public MpCardAbstractInfo? Abstract { get; set; }

    /// <summary>获取或设置图文列表（官方 <c>text_image_list</c>，选填）。</summary>
    [JsonPropertyName("text_image_list")]
    public List<MpCardTextImage>? TextImageList { get; set; }

    /// <summary>获取或设置商家服务类型（官方 <c>business_service</c>，选填；取值表<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("business_service")]
    public List<string>? BusinessService { get; set; }

    /// <summary>获取或设置使用时段限制（官方 <c>time_limit</c>，选填）。</summary>
    [JsonPropertyName("time_limit")]
    public List<MpCardTimeLimit>? TimeLimit { get; set; }
}

/// <summary>使用门槛（官方 <c>advanced_info.use_condition</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUseCondition
{
    /// <summary>获取或设置可用商品类目（官方 <c>accept_category</c>，选填）。</summary>
    [JsonPropertyName("accept_category")]
    public string? AcceptCategory { get; set; }

    /// <summary>获取或设置不可用商品类目（官方 <c>reject_category</c>，选填）。</summary>
    [JsonPropertyName("reject_category")]
    public string? RejectCategory { get; set; }

    /// <summary>获取或设置满减门槛（官方 <c>least_cost</c>，单位：分）。</summary>
    [JsonPropertyName("least_cost")]
    public int? LeastCost { get; set; }

    /// <summary>获取或设置可用类型门槛（官方 <c>object_use_for</c>，选填）。</summary>
    [JsonPropertyName("object_use_for")]
    public string? ObjectUseFor { get; set; }

    /// <summary>获取或设置是否可与其他优惠同享（官方 <c>can_use_with_other_discount</c>，选填）。</summary>
    [JsonPropertyName("can_use_with_other_discount")]
    public bool? CanUseWithOtherDiscount { get; set; }
}

/// <summary>封面摘要（官方 <c>advanced_info.abstract</c>；类型名带 <c>Info</c> 后缀以避开 <c>abstract</c> 关键字形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardAbstractInfo
{
    /// <summary>获取或设置摘要简介（官方 <c>abstract</c>，18 字符以内；<b>属性名与官方键同名</b>）。</summary>
    [JsonPropertyName("abstract")]
    public string? Abstract { get; set; }

    /// <summary>获取或设置封面图片 URL 列表（官方 <c>icon_url_list</c>，最多 3 张）。</summary>
    [JsonPropertyName("icon_url_list")]
    public List<string>? IconUrlList { get; set; }
}

/// <summary>高级信息图文项（官方 <c>advanced_info.text_image_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardTextImage
{
    /// <summary>获取或设置图片描述（官方 <c>text</c>）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>获取或设置封面图片 URL（官方 <c>image_url</c>，须为微信素材接口取到的图片 URL）。</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}

/// <summary>使用时段限制（官方 <c>advanced_info.time_limit[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardTimeLimit
{
    /// <summary>获取或设置限制类型（官方 <c>type</c>，星期/每天/节假日枚举<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置起始小时（官方 <c>begin_hour</c>，格式 H，0~23）。</summary>
    [JsonPropertyName("begin_hour")]
    public int? BeginHour { get; set; }

    /// <summary>获取或设置起始分钟（官方 <c>begin_minute</c>，格式 m，0~59）。</summary>
    [JsonPropertyName("begin_minute")]
    public int? BeginMinute { get; set; }

    /// <summary>获取或设置结束小时（官方 <c>end_hour</c>）。</summary>
    [JsonPropertyName("end_hour")]
    public int? EndHour { get; set; }

    /// <summary>获取或设置结束分钟（官方 <c>end_minute</c>）。</summary>
    [JsonPropertyName("end_minute")]
    public int? EndMinute { get; set; }
}

/// <summary>
/// 使用日期（官方 <c>base_info.date_info</c>）—— 建卡 / 查询 / 修改<b>三向同表</b>，共用一型。
/// </summary>
/// <remarks>
/// <b>字段组互斥</b>：<c>fixed_term</c> / <c>fixed_begin_term</c> 与 <c>begin_timestamp</c> /
/// <c>end_timestamp</c> 由 <c>type</c> 判别取哪一组（官方 <c>type</c> 取值表<b>待逐页核验</b>）。
/// SDK <b>不做本地互斥校验</b>，越界由官方错误码表达。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardDateInfo
{
    /// <summary>获取或设置使用时间类型（官方 <c>type</c>，判别取哪一组日期字段；取值<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置起用时间戳（官方 <c>begin_timestamp</c>，秒）。</summary>
    [JsonPropertyName("begin_timestamp")]
    public long? BeginTimestamp { get; set; }

    /// <summary>获取或设置过期时间戳（官方 <c>end_timestamp</c>，秒）。</summary>
    [JsonPropertyName("end_timestamp")]
    public long? EndTimestamp { get; set; }

    /// <summary>获取或设置自领取后有效天数（官方 <c>fixed_term</c>）。</summary>
    [JsonPropertyName("fixed_term")]
    public int? FixedTerm { get; set; }

    /// <summary>获取或设置自领取后生效天数（官方 <c>fixed_begin_term</c>）。</summary>
    [JsonPropertyName("fixed_begin_term")]
    public int? FixedBeginTerm { get; set; }
}

/// <summary>商品信息（官方 <c>base_info.sku</c>，<b>建卡方向</b>：仅 <c>quantity</c>，总量上限 100000000）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateSku
{
    /// <summary>获取或设置卡券库存数量（官方 <c>quantity</c>，必填）。</summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}

/// <summary>商品信息（官方 <c>base_info.sku</c>，<b>查询方向</b>：额外返回 <c>total_quantity</c>，与建卡不同表）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQuerySku
{
    /// <summary>获取或设置剩余库存（官方 <c>quantity</c>）。</summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>获取或设置总库存（官方 <c>total_quantity</c>）。</summary>
    [JsonPropertyName("total_quantity")]
    public int TotalQuantity { get; set; }
}

/// <summary>子商户信息（官方 <c>base_info.sub_merchant_info</c>，仅建卡方向）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantInfo
{
    /// <summary>获取或设置子商户号（官方 <c>merchant_id</c>，微信支付服务商模式下的子商户 ID）。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }
}

// ---------------------------------------------------------------- 会员卡 / 礼品卡子表

/// <summary>自定义会员信息类目（官方 <c>custom_field1..3</c>）—— 礼品卡与会员卡<b>同表</b>，建卡与修改共用一型。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCustomField
{
    /// <summary>获取或设置类目名称类型（官方 <c>name_type</c>，会员信息类目模板名）。</summary>
    [JsonPropertyName("name_type")]
    public string? NameType { get; set; }

    /// <summary>获取或设置类目自定义名称（官方 <c>name</c>，4 字符以内）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置类目跳转外链（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>自定义会员服务入口（官方 <c>custom_cell1..3</c>）—— 建卡与修改同表，共用一型。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCustomCell
{
    /// <summary>获取或设置入口名称（官方 <c>name</c>，5 字符以内）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置入口右侧提示语（官方 <c>tips</c>，5 字符以内）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置入口跳转链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 积分规则（官方 <c>member_card.bonus_rule</c>）—— 建卡与查询<b>同表</b>，共用一型；单位均为「分」或「积分」。
/// </summary>
/// <remarks>与 <c>bonus_rules</c>（字符串形态的旧口径）<b>并存且不可互换</b>，故两字段都保留。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardBonusRule
{
    /// <summary>获取或设置消费获取积分的金额单位（官方 <c>cost_money_unit</c>，分）。</summary>
    [JsonPropertyName("cost_money_unit")]
    public int? CostMoneyUnit { get; set; }

    /// <summary>获取或设置对应增加的积分（官方 <c>increase_bonus</c>）。</summary>
    [JsonPropertyName("increase_bonus")]
    public int? IncreaseBonus { get; set; }

    /// <summary>获取或设置单次可获取积分上限（官方 <c>max_increase_bonus</c>）。</summary>
    [JsonPropertyName("max_increase_bonus")]
    public int? MaxIncreaseBonus { get; set; }

    /// <summary>获取或设置初始积分（官方 <c>init_increase_bonus</c>）。</summary>
    [JsonPropertyName("init_increase_bonus")]
    public int? InitIncreaseBonus { get; set; }

    /// <summary>获取或设置抵扣所需积分单位（官方 <c>cost_bonus_unit</c>）。</summary>
    [JsonPropertyName("cost_bonus_unit")]
    public int? CostBonusUnit { get; set; }

    /// <summary>获取或设置积分抵扣金额（官方 <c>reduce_money</c>，分）。</summary>
    [JsonPropertyName("reduce_money")]
    public int? ReduceMoney { get; set; }

    /// <summary>获取或设置积分抵扣门槛（官方 <c>least_money_to_use_bonus</c>，分）。</summary>
    [JsonPropertyName("least_money_to_use_bonus")]
    public int? LeastMoneyToUseBonus { get; set; }

    /// <summary>获取或设置单次可用积分上限（官方 <c>max_reduce_bonus</c>）。</summary>
    [JsonPropertyName("max_reduce_bonus")]
    public int? MaxReduceBonus { get; set; }
}

/// <summary>微信支付信息（官方 <c>member_card.base_info.pay_info</c>，仅会员卡）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayInfo
{
    /// <summary>获取或设置刷卡信息（官方 <c>swipe_card</c>）。</summary>
    [JsonPropertyName("swipe_card")]
    public MpCardSwipeCardInfo? SwipeCard { get; set; }
}

/// <summary>刷卡信息（官方 <c>pay_info.swipe_card</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSwipeCardInfo
{
    /// <summary>获取或设置是否支持微信支付刷卡（官方 <c>is_swipe_card</c>）。</summary>
    [JsonPropertyName("is_swipe_card")]
    public bool IsSwipeCard { get; set; }
}

/// <summary>礼品卡信息（官方 <c>general_card.base_info.giftcard_info</c>，仅礼品卡建卡方向）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftCardPriceInfo
{
    /// <summary>获取或设置礼品卡价格（官方 <c>price</c>，单位：分）。</summary>
    [JsonPropertyName("price")]
    public int Price { get; set; }
}

/// <summary>
/// 卡券消息入口（官方 <c>modify_msg_operation</c> / <c>activate_msg_operation</c>，仅修改会员卡方向）。
/// </summary>
/// <remarks>
/// <b>为何只出现在修改方向</b>：该对象配置「修改信息 / 领卡后」推送给用户的入口按钮，属运营期设置，
/// 建卡页字段表不含（对齐基准：SKIT <c>CardUpdateRequest.Types.MembershipCard.Types.Operation</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMessageOperation
{
    /// <summary>获取或设置入口卡片（官方 <c>url_cell</c>）。</summary>
    [JsonPropertyName("url_cell")]
    public MpCardUrlCell? UrlCell { get; set; }
}

/// <summary>消息入口按钮（官方 <c>*_msg_operation.url_cell</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUrlCell
{
    /// <summary>获取或设置生效的卡券模板列表（官方 <c>card_id_list</c>，空表示对全部卡生效）。</summary>
    [JsonPropertyName("card_id_list")]
    public List<string>? CardIdList { get; set; }

    /// <summary>获取或设置按钮失效时间戳（官方 <c>end_time</c>，秒）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置按钮文字（官方 <c>text</c>）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>获取或设置按钮跳转外链（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置按钮跳转小程序原始 id（官方 <c>app_brand_id</c>）。</summary>
    [JsonPropertyName("app_brand_id")]
    public string? AppBrandId { get; set; }

    /// <summary>获取或设置按钮跳转小程序页面路径（官方 <c>app_brand_pass</c>）。</summary>
    [JsonPropertyName("app_brand_pass")]
    public string? AppBrandPass { get; set; }
}
