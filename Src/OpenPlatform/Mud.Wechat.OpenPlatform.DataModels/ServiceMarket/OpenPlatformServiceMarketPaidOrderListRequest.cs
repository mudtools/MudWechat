// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.ServiceMarket;

/// <summary>
/// 获取用户已支付订单列表请求（<c>POST /wxa/servicemarket/get_paid_order_list</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/service_market/interface/value_added/servicemarket_get_paid_order_list.html"/></para>
/// <para>官方约束：<b>仅第三方平台可调用</b>（<c>component_access_token</c>）、须在服务端调用；
/// <c>appid</c>/<c>openid</c> 按 <c>buyer_type</c> 二选一过滤（1 小程序 / 2 微信用户）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class OpenPlatformServiceMarketPaidOrderListRequest
{
    /// <summary>服务 ID（官方 <c>service_id</c>，number，必填）。</summary>
    [JsonPropertyName("service_id")]
    public long ServiceId { get; set; }

    /// <summary>小程序 AppId（官方 <c>appid</c>，按小程序过滤时传入）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>用户 OpenId（官方 <c>openid</c>，按微信用户过滤时传入）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>分页起始位置（官方 <c>offset</c>，number）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>分页每页数量（官方 <c>limit</c>，number，默认 10）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>购买方类型（官方 <c>buyer_type</c>，number：1 小程序 / 2 微信用户）。</summary>
    [JsonPropertyName("buyer_type")]
    public int? BuyerType { get; set; }
}
