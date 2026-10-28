// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「车辆支付申请」请求（官方 <c>vehicle/payapply</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehiclePayApplyRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = string.Empty;

    /// <summary>获取或设置订单总金额（官方 <c>fee</c>，单位分，必填）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置商户订单号（官方 <c>order_no</c>，可选）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>获取或设置通知书类型（官方 <c>payment_notice_type</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_type")]
    public int? PaymentNoticeType { get; set; }

    /// <summary>获取或设置通知书号（官方 <c>payment_notice_no</c>，可选）。</summary>
    [JsonPropertyName("payment_notice_no")]
    public string? PaymentNoticeNo { get; set; }

    /// <summary>获取或设置执收单位编码（官方 <c>department_code</c>，必填）。</summary>
    [JsonPropertyName("department_code")]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>获取或设置执收单位名称（官方 <c>department_name</c>，必填）。</summary>
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，可选）。</summary>
    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    /// <summary>获取或设置商品标记（官方 <c>goods_tag</c>，可选）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>获取或设置交易场景（官方 <c>trade_scene</c>，必填）。</summary>
    [JsonPropertyName("trade_scene")]
    public string TradeScene { get; set; } = string.Empty;

    /// <summary>获取或设置出场场景信息（官方 <c>scene_info</c>，必填）。</summary>
    [JsonPropertyName("scene_info")]
    public MpNontaxVehiclePayScene SceneInfo { get; set; } = new();
}
