// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相关法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// 营销单元定向设置（官方 <c>targeting</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>一份类型服务三支端点</b>（2026-10-10 逐页比对 <c>adgroups/get</c> 应答表、<c>adgroups/add</c>
/// 与 <c>adgroups/update</c> 请求表）：三张表里 <c>targeting</c> 子树的<b>字段名、类型与层级完全一致</b>，
/// 差异只在「请求侧某些字段标 <c>*</c>」。⇒ 建单一类型而非三份复制，否则改名时必然只改到其中一份。
/// </para>
/// <para>
/// <b>请求侧 <c>*</c> 是「条件必填」而非「无条件必填」</b>：<c>location_types</c> 仅在传
/// <c>geo_location</c> 时必填、<c>min</c>/<c>max</c> 仅在传 <c>age</c> 元素时必填、
/// <c>excluded_dimension</c> 仅在传 <c>excluded_converted_audience</c> 时必填 ⇒
/// SDK <b>不做</b>本地必填校验（校验正确形态是「父结构在场时子字段必填」，本地拦截会把
/// 「不传该父结构」这一合法形态一起挡掉），越界由应答 <c>code</c> 表达。
/// </para>
/// <para>
/// <b>应答侧本类型是超集形态</b>：官方 <c>adgroups/get</c> 明确「定向详细设置，存放所有定向条件」，
/// 未使用的分支不会出现在应答里 ⇒ 全部字段可空。
/// </para>
/// <para>
/// <b>官方标注「即将下线」的字段</b>（<c>education</c> / <c>marital_status</c> / <c>device_brand_model</c> /
/// <c>user_os</c> / <c>network_type</c> / <c>device_price</c> / <c>game_consumption_level</c>）照录保留：
/// 官方原文均为「该功能即将下线，仅部分行业灰度开放」，<b>未给出下线时间点</b> ⇒
/// 删除字段会让已灰度开放的账号无处读写该定向，保留成本仅是一段注释。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupTargeting
{
    /// <summary>地理位置定向（官方 <c>geo_location</c>，<c>struct</c>）。</summary>
    [JsonPropertyName("geo_location")]
    public AdsAdgroupGeoLocation? GeoLocation { get; set; }

    /// <summary>是否使用地域优选（官方 <c>geo_location_auto_audience</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("geo_location_auto_audience")]
    public bool? GeoLocationAutoAudience { get; set; }

    /// <summary>性别定向，仅单选（官方 <c>gender</c>，<c>enum[]</c>）。</summary>
    [JsonPropertyName("gender")]
    public List<string>? Gender { get; set; }

    /// <summary>年龄定向（官方 <c>age</c>，<c>struct[]</c>）。官方范围 18~66、步长不小于 4，66 代表「66 岁及以上」。</summary>
    [JsonPropertyName("age")]
    public List<AdsAgeRange>? Age { get; set; }

    /// <summary>用户学历（官方 <c>education</c>，<c>enum[]</c>；官方标注即将下线、仅部分行业灰度开放）。</summary>
    [JsonPropertyName("education")]
    public List<string>? Education { get; set; }

    /// <summary>应用安装状态（官方 <c>app_install_status</c>，<c>enum[]</c>，当且仅当推广目标类型为 ANDROID、IOS 时使用）。</summary>
    [JsonPropertyName("app_install_status")]
    public List<string>? AppInstallStatus { get; set; }

    /// <summary>婚恋状态（官方 <c>marital_status</c>，<c>enum[]</c>；官方标注即将下线、仅部分行业灰度开放）。</summary>
    [JsonPropertyName("marital_status")]
    public List<string>? MaritalStatus { get; set; }

    /// <summary>排除已转化人群行为定向（官方 <c>excluded_converted_audience</c>，<c>struct</c>）。</summary>
    [JsonPropertyName("excluded_converted_audience")]
    public AdsExcludedConvertedAudience? ExcludedConvertedAudience { get; set; }

    /// <summary>定向用户群 id（官方 <c>custom_audience</c>，<c>integer[]</c>）。
    /// 官方限制：<c>custom_audience</c> 与 <c>excluded_custom_audience</c> 个数之和不能超过 200。</summary>
    [JsonPropertyName("custom_audience")]
    public List<long>? CustomAudience { get; set; }

    /// <summary>排除用户群 id（官方 <c>excluded_custom_audience</c>，<c>integer[]</c>，个数限制见 <see cref="CustomAudience"/>）。</summary>
    [JsonPropertyName("excluded_custom_audience")]
    public List<long>? ExcludedCustomAudience { get; set; }

    /// <summary>设备品牌型号定向（官方 <c>device_brand_model</c>，<c>struct</c>；官方标注即将下线）。</summary>
    [JsonPropertyName("device_brand_model")]
    public AdsDeviceBrandModel? DeviceBrandModel { get; set; }

    /// <summary>操作系统定向（官方 <c>user_os</c>，<c>enum[]</c>；官方标注即将下线，且对
    /// <c>marketing_carrier_type</c> 为 ANDROID/IOS 应用时会与载体联动，详见官方页原文举例）。</summary>
    [JsonPropertyName("user_os")]
    public List<string>? UserOs { get; set; }

    /// <summary>联网方式定向（官方 <c>network_type</c>，<c>enum[]</c>；官方标注即将下线）。</summary>
    [JsonPropertyName("network_type")]
    public List<string>? NetworkType { get; set; }

    /// <summary>设备价格定向（官方 <c>device_price</c>，<c>enum[]</c>；官方标注即将下线）。</summary>
    [JsonPropertyName("device_price")]
    public List<string>? DevicePrice { get; set; }

    /// <summary>微信营销行为定向（官方 <c>wechat_ad_behavior</c>，<c>struct</c>，当且仅当投放微信营销时有效）。</summary>
    [JsonPropertyName("wechat_ad_behavior")]
    public AdsWechatAdBehavior? WechatAdBehavior { get; set; }

    /// <summary>游戏消费能力（官方 <c>game_consumption_level</c>，<c>enum[]</c>；官方标注即将下线）。</summary>
    [JsonPropertyName("game_consumption_level")]
    public List<string>? GameConsumptionLevel { get; set; }

    /// <summary>排除操作系统定向（官方 <c>excluded_os</c>，<c>enum[]</c>。官方原文：
    /// <c>ANDROID_PURE_MODE</c> 枚举目前仅支持 <c>MARKETING_CARRIER_TYPE_APP_ANDROID</c> 载体类型）。</summary>
    [JsonPropertyName("excluded_os")]
    public List<string>? ExcludedOs { get; set; }
}

/// <summary>
/// 地理位置定向（官方 <c>geo_location</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <b>官方三选一约束（原文，SDK 不本地拦截）</b>：<c>regions</c>、<c>business_districts</c>、
/// <c>custom_locations</c> <b>不能同时为空</b>。另官方对 <c>location_types</c> 设了两条选择限制：
/// 微信流量仅能选 <c>LIVE_IN</c>（常住）；使用商圈或自定义地理位置时仅可选 <c>VISITED_IN</c>（去过）/
/// <c>LIVE_IN</c>（常住）。这类「跨字段组合约束」的正确判定面是官方网关（它同时知道版位与载体），
/// 本地校验只会造出假阳性。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupGeoLocation
{
    /// <summary>地点类型（官方 <c>location_types</c>，<c>enum[]</c>，请求侧<b>条件必填</b>）。</summary>
    [JsonPropertyName("location_types")]
    public List<string>? LocationTypes { get; set; }

    /// <summary>省市区县列表（官方 <c>regions</c>，<c>integer[]</c>，取值可通过 <c>targeting_tags/get</c> 接口获取）。</summary>
    [JsonPropertyName("regions")]
    public List<long>? Regions { get; set; }

    /// <summary>商圈 id 列表（官方 <c>business_districts</c>，<c>integer[]</c>，取值可通过 <c>targeting_tags/get</c> 接口获取）。</summary>
    [JsonPropertyName("business_districts")]
    public List<long>? BusinessDistricts { get; set; }

    /// <summary>自定义地理位置列表（官方 <c>custom_locations</c>，<c>struct[]</c>，使用火星系坐标）。</summary>
    [JsonPropertyName("custom_locations")]
    public List<AdsCustomLocation>? CustomLocations { get; set; }
}

/// <summary>
/// 自定义地理位置（官方 <c>custom_locations[]</c> 元素，<c>struct</c>）。
/// 官方类型标注为「火星系坐标」⇒ 坐标值以 <see cref="double"/> 承载，不做本地精度裁剪。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsCustomLocation
{
    /// <summary>经度，单位度（官方 <c>longitude</c>，<c>float64</c>，请求侧<b>条件必填</b>）。</summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>纬度，单位度（官方 <c>latitude</c>，<c>float64</c>，请求侧<b>条件必填</b>）。</summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>半径，单位米（官方 <c>radius</c>，<c>integer</c>，请求侧<b>条件必填</b>）。</summary>
    [JsonPropertyName("radius")]
    public long? Radius { get; set; }
}

/// <summary>年龄区间（官方 <c>age[]</c> 元素，<c>struct</c>；两字段请求侧<b>条件必填</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAgeRange
{
    /// <summary>年龄下限（官方 <c>min</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("min")]
    public long? Min { get; set; }

    /// <summary>年龄上限（官方 <c>max</c>，<c>integer</c>；<c>66</c> 代表 66 岁及 66 岁以上）。</summary>
    [JsonPropertyName("max")]
    public long? Max { get; set; }
}

/// <summary>
/// 排除已转化人群行为定向（官方 <c>excluded_converted_audience</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <b>官方适用前提（原文要点，决定该结构能否出现）</b>：同应用，仅当推广产品类型为 ANDROID/IOS 应用时
/// 可以使用；未选择自定义转化行为（<c>excluded_dimension</c>）时，使用该定向的出价需满足 oCPC、oCPM 营销。
/// 前提依赖账号权限与出价方式（SDK 不可见）⇒ 不做本地拦截。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsExcludedConvertedAudience
{
    /// <summary>排除已转化人群的数据维度（官方 <c>excluded_dimension</c>，<c>enum</c>，请求侧<b>条件必填</b>）。</summary>
    [JsonPropertyName("excluded_dimension")]
    public string? ExcludedDimension { get; set; }

    /// <summary>转化行为（官方 <c>conversion_behavior_list</c>，<c>enum[]</c>，枚举值同优化目标类型）。</summary>
    [JsonPropertyName("conversion_behavior_list")]
    public List<string>? ConversionBehaviorList { get; set; }

    /// <summary>排除天数（官方 <c>excluded_day</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("excluded_day")]
    public string? ExcludedDay { get; set; }
}

/// <summary>
/// 设备品牌型号定向（官方 <c>device_brand_model</c>，<c>struct</c>；官方标注即将下线）。
/// </summary>
/// <remarks>官方原文：<c>excluded_list</c> <b>不能与</b> <c>included_list</c> 同时使用 —— 这是本结构唯一的
/// 互斥约束，且两字段同时在请求里出现时官方直接拒 ⇒ SDK 不本地拦截（互斥判定面在官方）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsDeviceBrandModel
{
    /// <summary>设备品牌型号定向 id 列表（官方 <c>included_list</c>，<c>integer[]</c>，取值可通过 <c>targeting_tags/get</c> 获取）。</summary>
    [JsonPropertyName("included_list")]
    public List<long>? IncludedList { get; set; }

    /// <summary>排除设备品牌型号 id 列表（官方 <c>excluded_list</c>，<c>integer[]</c>，不可与 <see cref="IncludedList"/> 同时使用）。</summary>
    [JsonPropertyName("excluded_list")]
    public List<long>? ExcludedList { get; set; }
}

/// <summary>微信营销行为定向（官方 <c>wechat_ad_behavior</c>，<c>struct</c>，当且仅当投放微信营销时有效）。</summary>
/// <remarks>
/// 官方原文：当枚举值选择【已经添加过企业微信】时<b>必须</b>填入 <c>corp_id</c> 列表 ——
/// 条件必填依赖 <see cref="Actions"/> / <see cref="ExcludedActions"/> 的取值组合，SDK 不预判。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsWechatAdBehavior
{
    /// <summary>微信再营销类型（官方 <c>actions</c>，<c>enum[]</c>：关注过公众号、已安装应用、领取过卡券等，取值见官方页原文列举）。</summary>
    [JsonPropertyName("actions")]
    public List<string>? Actions { get; set; }

    /// <summary>排除的微信再营销类型（官方 <c>excluded_actions</c>，<c>enum[]</c>，取值集同 <see cref="Actions"/>）。</summary>
    [JsonPropertyName("excluded_actions")]
    public List<string>? ExcludedActions { get; set; }

    /// <summary>微信再营销 corp_id 列表（官方 <c>corp_id</c>，<c>string[]</c>；选择「已添加过企业微信」时必填）。</summary>
    [JsonPropertyName("corp_id")]
    public List<string>? CorpId { get; set; }
}
