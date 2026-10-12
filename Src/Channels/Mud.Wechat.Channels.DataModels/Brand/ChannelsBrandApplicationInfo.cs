// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>
/// 品牌资质申请单（官方 <c>brand</c> 对象与两个列表接口 <c>brands</c> 数组元素字段集一致 ⇒ 共用；
/// 覆盖 <c>getbrandlogic</c> / <c>getbrandlistlogic</c> / <c>getvalidbrandlistlogic</c> 三端点）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandApplicationInfo
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>；本形态官方类型为 number，与品牌库列表 string 型不一致，照抄官方）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }

    /// <summary>获取或设置商标分类号（官方 <c>classification_no</c>，取值范围 1-45）。</summary>
    [JsonPropertyName("classification_no")]
    public string? ClassificationNo { get; set; }

    /// <summary>获取或设置商标类型（官方 <c>trade_mark_symbol</c>，见 <see cref="ChannelsBrandTradeMarkSymbols"/>）。</summary>
    [JsonPropertyName("trade_mark_symbol")]
    public int? TradeMarkSymbol { get; set; }

    /// <summary>获取或设置注册信息（官方 <c>register_details</c>）。</summary>
    [JsonPropertyName("register_details")]
    public ChannelsBrandRegisterDetails? RegisterDetails { get; set; }

    /// <summary>获取或设置申请信息（官方 <c>application_details</c>）。</summary>
    [JsonPropertyName("application_details")]
    public ChannelsBrandApplicationDetails? ApplicationDetails { get; set; }

    /// <summary>获取或设置商标授权类型（官方 <c>grant_type</c>，见 <see cref="ChannelsBrandGrantTypes"/>）。</summary>
    [JsonPropertyName("grant_type")]
    public int? GrantType { get; set; }

    /// <summary>获取或设置品牌信息（官方 <c>grant_details</c>）。</summary>
    [JsonPropertyName("grant_details")]
    public ChannelsBrandGrantDetails? GrantDetails { get; set; }

    /// <summary>获取或设置申请单状态（官方 <c>status</c>，见 <see cref="ChannelsBrandStatuses"/>；生效中列表仅返回 3 / 5）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置审核结果（官方 <c>audit_result</c>）。</summary>
    [JsonPropertyName("audit_result")]
    public ChannelsBrandAuditResult? AuditResult { get; set; }
}
