// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送充值订单（<c>balance_detail.order_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityChargeOrder
{
    /// <summary>充值订单号（<c>payorder_id</c>）。</summary>
    [JsonPropertyName("payorder_id")]
    public string? PayOrderId { get; set; }

    /// <summary>充值金额（<c>charge_amt</c>；单位：分）。</summary>
    [JsonPropertyName("charge_amt")]
    public int ChargeAmount { get; set; }

    /// <summary>未使用的余额（<c>unused_amt</c>；单位：分）。</summary>
    [JsonPropertyName("unused_amt")]
    public int UnusedAmount { get; set; }

    /// <summary>充值生效时间（<c>begin_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>充值失效时间（<c>end_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }
}
