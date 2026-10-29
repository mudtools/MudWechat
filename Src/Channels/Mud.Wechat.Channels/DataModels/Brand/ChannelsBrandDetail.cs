// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>品牌资质提交详情（官方 <c>brand</c> 对象——「新增」与「更新」两请求体字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandDetail
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，必填；本端点官方类型为 number，与品牌库列表返回的 string 型不一致，照抄官方）。</summary>
    [JsonPropertyName("brand_id")]
    public long BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>，选填，从品牌库获取）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>，选填，从品牌库获取）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }

    /// <summary>获取或设置商标分类号（官方 <c>classification_no</c>，必填，取值范围 1-45）。</summary>
    [JsonPropertyName("classification_no")]
    public string ClassificationNo { get; set; } = string.Empty;

    /// <summary>获取或设置商标类型（官方 <c>trade_mark_symbol</c>，必填，见 <see cref="ChannelsBrandTradeMarkSymbols"/>）。</summary>
    [JsonPropertyName("trade_mark_symbol")]
    public int TradeMarkSymbol { get; set; }

    /// <summary>获取或设置注册信息（官方 <c>register_details</c>，选填；R 标时必填商标注册人与注册号）。</summary>
    [JsonPropertyName("register_details")]
    public ChannelsBrandRegisterDetails? RegisterDetails { get; set; }

    /// <summary>获取或设置申请信息（官方 <c>application_details</c>，选填；TM 标时必填）。</summary>
    [JsonPropertyName("application_details")]
    public ChannelsBrandApplicationDetails? ApplicationDetails { get; set; }

    /// <summary>获取或设置商标授权类型（官方 <c>grant_type</c>，选填，见 <see cref="ChannelsBrandGrantTypes"/>）。</summary>
    [JsonPropertyName("grant_type")]
    public int? GrantType { get; set; }

    /// <summary>获取或设置品牌信息（官方 <c>grant_details</c>，选填；授权品牌必填授权书与授权级数）。</summary>
    [JsonPropertyName("grant_details")]
    public ChannelsBrandGrantDetails? GrantDetails { get; set; }
}
