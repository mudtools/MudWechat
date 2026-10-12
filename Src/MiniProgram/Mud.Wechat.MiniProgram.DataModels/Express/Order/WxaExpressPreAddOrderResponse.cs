// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送询价应答（<c>service_trans_id</c> / <c>distance</c> / <c>est_fee</c> 等）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPreAddOrderResponse : WxaResponse
{
    /// <summary>运力 ID（<c>service_trans_id</c>；下单时须传回）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>配送距离（<c>distance</c>；单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>预估费用（<c>est_fee</c>；单位：分）。</summary>
    [JsonPropertyName("est_fee")]
    public int EstimatedFee { get; set; }

    /// <summary>预计送达时间（<c>expected_finished_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("expected_finished_time")]
    public long? ExpectedFinishTime { get; set; }

    /// <summary>承诺送达时间（<c>promise_delivery_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("promise_delivery_time")]
    public long? PromiseDeliveryTime { get; set; }
}
