// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.ServiceMarket;

/// <summary>
/// 服务用户（购买方）信息（官方 <c>buyer</c> / <c>buyer_list</c> 数组项）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/service_market/interface/value_added/servicemarket_get_service_buyer_list.html"/></para>
/// <para><c>appid</c>（小程序购买方）与 <c>openid</c>（微信用户购买方）按 <c>buyer_type</c> 二选一出现；
/// 二者均属用户标识，<b>不得</b>落日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class OpenPlatformServiceMarketBuyer
{
    /// <summary>小程序 AppId（官方 <c>appid</c>，buyer_type = 1 时返回）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>用户 OpenId（官方 <c>openid</c>，buyer_type = 2 时返回）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>服务 ID（官方 <c>service_id</c>，number）。</summary>
    [JsonPropertyName("service_id")]
    public long ServiceId { get; set; }

    /// <summary>规格有效期列表（官方 <c>spec_list</c>）。</summary>
    [JsonPropertyName("spec_list")]
    public List<OpenPlatformServiceMarketSpecification>? SpecificationList { get; set; }
}
