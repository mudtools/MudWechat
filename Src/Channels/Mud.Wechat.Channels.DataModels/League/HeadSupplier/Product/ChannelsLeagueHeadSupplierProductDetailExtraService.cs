// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 机构侧服务保障信息（官方 <c>item.product_info.extra_service</c>，8 个开关）。
/// </summary>
/// <remarks>
/// <para>
/// 商家侧 <see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsProductExtraService"/> 只有 5 个开关
/// （无 <c>pay_after_use</c> / <c>buy_with_confidence</c> / <c>deliver_after_check</c>）⇒ 字段集不同，分建 DTO。
/// </para>
/// <para>
/// 官方原文把 <c>damage_guarantee</c> 描述为「是否支持假一赔三」、把 <c>fake_one_pay_three</c> 描述为「是否支持坏损包退」，
/// 与两个字段名的直觉语义相反 ⇒ 本仓<b>照抄官方原文</b>，不做「纠错式」调换，注释与官方一一对应。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProductDetailExtraService
{
    /// <summary>获取或设置是否支持七天无理由退货（官方 <c>seven_day_return</c>：0 不支持 / 1 支持 / 2 支持（定制商品除外）/ 3 支持（使用后不支持））。</summary>
    [JsonPropertyName("seven_day_return")]
    public int? SevenDayReturn { get; set; }

    /// <summary>获取或设置先用后付（官方 <c>pay_after_use</c>：0 不支持 / 1 支持；店铺已开通先用后付时，支持的类目商品上架后自动打开）。</summary>
    [JsonPropertyName("pay_after_use")]
    public int? PayAfterUse { get; set; }

    /// <summary>获取或设置是否支持运费险（官方 <c>freight_insurance</c>：0 不支持 / 1 支持；需商户开通运费险服务且当前类目支持才会生效）。</summary>
    [JsonPropertyName("freight_insurance")]
    public int? FreightInsurance { get; set; }

    /// <summary>获取或设置坏损包退开关（官方 <c>damage_guarantee</c>：0/1；官方原文描述为「是否支持假一赔三」，与字段名直觉相反 ⇒ 照录）。</summary>
    [JsonPropertyName("damage_guarantee")]
    public int? DamageGuarantee { get; set; }

    /// <summary>获取或设置假一赔三开关（官方 <c>fake_one_pay_three</c>：0/1；官方原文描述为「是否支持坏损包退」，与字段名直觉相反 ⇒ 照录）。</summary>
    [JsonPropertyName("fake_one_pay_three")]
    public int? FakeOnePayThree { get; set; }

    /// <summary>获取或设置是否支持换货（官方 <c>exchange_support</c>：0 不支持 / 1 支持）。</summary>
    [JsonPropertyName("exchange_support")]
    public int? ExchangeSupport { get; set; }

    /// <summary>获取或设置是否满足放心买（官方 <c>buy_with_confidence</c>：0 不满足 / 1 满足）。</summary>
    [JsonPropertyName("buy_with_confidence")]
    public int? BuyWithConfidence { get; set; }

    /// <summary>获取或设置是否满足先检后发（官方 <c>deliver_after_check</c>：0 不满足 / 1 满足）。</summary>
    [JsonPropertyName("deliver_after_check")]
    public int? DeliverAfterCheck { get; set; }
}
