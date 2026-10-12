// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送拉取配送单信息响应（<c>POST .../order/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGetOrderResponse : WxaExpressLocalResponse
{
    /// <summary>配送状态（<c>order_status</c>；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>配送单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>骑手姓名（<c>rider_name</c>）。</summary>
    [JsonPropertyName("rider_name")]
    public string? RiderName { get; set; }

    /// <summary>骑手电话（<c>rider_phone</c>；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("rider_phone")]
    public string? RiderPhoneNumber { get; set; }

    /// <summary>骑手位置经度（<c>rider_lng</c>；配送中时返回）。</summary>
    [JsonPropertyName("rider_lng")]
    public double? RiderLongitude { get; set; }

    /// <summary>骑手位置纬度（<c>rider_lat</c>；配送中时返回）。</summary>
    [JsonPropertyName("rider_lat")]
    public double? RiderLatitude { get; set; }

    /// <summary>预计还剩多久送达（<c>reach_time</c>，单位：秒；配送中时返回）。</summary>
    [JsonPropertyName("reach_time")]
    public int? ReachTime { get; set; }
}
