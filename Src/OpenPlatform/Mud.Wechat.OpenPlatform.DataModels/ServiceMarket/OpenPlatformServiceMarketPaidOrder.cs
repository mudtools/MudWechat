// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.ServiceMarket;

/// <summary>
/// 已支付订单（官方 <c>order_list</c> 数组项）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/service_market/interface/value_added/servicemarket_get_paid_order_list.html"/></para>
/// <para><c>phone</c> 属个人联系方式，<b>不得</b>进日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class OpenPlatformServiceMarketPaidOrder
{
    /// <summary>订单 ID（官方 <c>order_id</c>，number）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    /// <summary>生效时间戳（官方 <c>effective_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("effective_time")]
    public long EffectiveTime { get; set; }

    /// <summary>过期时间戳（官方 <c>expire_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("expire_time")]
    public long ExpireTime { get; set; }

    /// <summary>规格 ID（官方 <c>specification_id</c>）。</summary>
    [JsonPropertyName("specification_id")]
    public string? SpecificationId { get; set; }

    /// <summary>服务 ID（官方 <c>service_id</c>，number）。</summary>
    [JsonPropertyName("service_id")]
    public long ServiceId { get; set; }

    /// <summary>订单总价（官方 <c>total_price</c>，单位分）。</summary>
    [JsonPropertyName("total_price")]
    public int? TotalPrice { get; set; }

    /// <summary>用户手机号（官方 <c>phone</c>，仅已支付订单返回；个人敏感信息勿落日志）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}
