// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 额外服务（官方 <c>extra_service</c> 对象：七天无理由 / 运费险 / 坏损包退 / 假一赔三 / 换货）。
/// </summary>
/// <remarks>
/// 官方说明：为提升商家发品成功率，若入参信息不符合平台规则，平台会将其优化至符合规则的参数值。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductExtraService
{
    /// <summary>获取或设置是否支持七天无理由退货（官方 <c>seven_day_return</c>，见 <see cref="ChannelsSevenDayReturnTypes"/>）。</summary>
    [JsonPropertyName("seven_day_return")]
    public int? SevenDayReturn { get; set; }

    /// <summary>获取或设置是否支持运费险（官方 <c>freight_insurance</c>：0 不支持，1 支持；需商户先开通运费险服务）。</summary>
    [JsonPropertyName("freight_insurance")]
    public int? FreightInsurance { get; set; }

    /// <summary>获取或设置是否支持坏损包退（官方 <c>damage_guarantee</c>：0 不支持，1 支持）。</summary>
    [JsonPropertyName("damage_guarantee")]
    public int? DamageGuarantee { get; set; }

    /// <summary>获取或设置是否支持假一赔三（官方 <c>fake_one_pay_three</c>：0 不支持，1 支持）。</summary>
    [JsonPropertyName("fake_one_pay_three")]
    public int? FakeOnePayThree { get; set; }

    /// <summary>获取或设置是否支持换货（官方 <c>exchange_support</c>：0 关闭支持换货，1 打开支持换货）。</summary>
    [JsonPropertyName("exchange_support")]
    public int? ExchangeSupport { get; set; }
}
