// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>运力更新订单费用请求体（<c>POST /cgi-bin/express/delivery/single_waybill/fee</c>，散单寄件）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderfee"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillFeeRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>是否需要支付（<c>need_pay</c>，必填；0 表示无需支付，1 表示需要支付）。</summary>
    [JsonPropertyName("need_pay")]
    public int NeedPay { get; set; }

    /// <summary>实际快递费用（<c>fee</c>，单位：分；结算价）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>原价（<c>original_fee</c>，单位：分）。</summary>
    [JsonPropertyName("original_fee")]
    public int OriginalFee { get; set; }

    /// <summary>基础运费（<c>base_fee</c>，单位：分）。</summary>
    [JsonPropertyName("base_fee")]
    public int BaseFee { get; set; }

    /// <summary>保价费用（<c>insured_fee</c>，单位：分）。</summary>
    [JsonPropertyName("insured_fee")]
    public int InsuredFee { get; set; }

    /// <summary>其他费用（<c>other_fee</c>，单位：分）。</summary>
    [JsonPropertyName("other_fee")]
    public int OtherFee { get; set; }

    /// <summary>费用备注（<c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>费用商品名称（<c>pay_goods_name</c>）。</summary>
    [JsonPropertyName("pay_goods_name")]
    public string? PayGoodsName { get; set; }
}
