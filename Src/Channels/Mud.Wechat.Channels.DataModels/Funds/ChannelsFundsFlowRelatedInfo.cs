// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>资金流水关联信息（官方 <c>related_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsFundsFlowRelatedInfo
{
    /// <summary>
    /// 获取或设置关联类型（官方 <c>related_type</c>）：1 订单，2 售后，3 提现，4 运费险，5 服务保障，
    /// 6 送礼物，7 群送礼关联订单号列表，8 同城配送门店 id。
    /// </summary>
    [JsonPropertyName("related_type")]
    public int? RelatedType { get; set; }

    /// <summary>获取或设置关联订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置关联售后单号（官方 <c>aftersale_id</c>）。</summary>
    [JsonPropertyName("aftersale_id")]
    public string? AftersaleId { get; set; }

    /// <summary>获取或设置关联提现单号（官方 <c>withdraw_id</c>）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }

    /// <summary>获取或设置记账时间（官方 <c>bookkeeping_time</c>）。</summary>
    [JsonPropertyName("bookkeeping_time")]
    public string? BookkeepingTime { get; set; }

    /// <summary>获取或设置关联运费险单号（官方 <c>insurance_id</c>）。</summary>
    [JsonPropertyName("insurance_id")]
    public string? InsuranceId { get; set; }

    /// <summary>获取或设置关联支付单号（官方 <c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置关联保障单号（官方 <c>guarantee_id</c>）。</summary>
    [JsonPropertyName("guarantee_id")]
    public long? GuaranteeId { get; set; }

    /// <summary>获取或设置关联礼物单号（官方 <c>present_id</c>）。</summary>
    [JsonPropertyName("present_id")]
    public long? PresentId { get; set; }

    /// <summary>
    /// 获取或设置群送礼关联订单号列表（官方 <c>group_present_sub_order_id_list</c>）——
    /// 群礼物与订单之间是一对多关系，多个订单号通过「,」来分隔（官方类型为 string）。
    /// </summary>
    [JsonPropertyName("group_present_sub_order_id_list")]
    public string? GroupPresentSubOrderIdList { get; set; }

    /// <summary>获取或设置同城配送门店 id（官方 <c>intra_city_shop_id</c>）。</summary>
    [JsonPropertyName("intra_city_shop_id")]
    public long? IntraCityShopId { get; set; }
}
