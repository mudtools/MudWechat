// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
