// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 货币类型字段属性（官方 CurrencyFieldProperty）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetCurrencyFieldProperty
{
    /// <summary>
    /// 获取或设置货币类型（官方 <c>currency_type</c>）。
    /// 官方取值：<c>CURRENCY_TYPE_CNY</c>、<c>CURRENCY_TYPE_USD</c>、<c>CURRENCY_TYPE_EUR</c>、<c>CURRENCY_TYPE_GBP</c>、<c>CURRENCY_TYPE_JPY</c>、<c>CURRENCY_TYPE_KRW</c>、<c>CURRENCY_TYPE_HKD</c>、<c>CURRENCY_TYPE_MOP</c>、<c>CURRENCY_TYPE_TWD</c>、<c>CURRENCY_TYPE_AED</c>、<c>CURRENCY_TYPE_AUD</c>、<c>CURRENCY_TYPE_BRL</c>、<c>CURRENCY_TYPE_CAD</c>、<c>CURRENCY_TYPE_CHF</c>、<c>CURRENCY_TYPE_IDR</c>、<c>CURRENCY_TYPE_INR</c>、<c>CURRENCY_TYPE_MXN</c>、<c>CURRENCY_TYPE_MYR</c>、<c>CURRENCY_TYPE_PHP</c>、<c>CURRENCY_TYPE_PLN</c>、<c>CURRENCY_TYPE_RUB</c>、<c>CURRENCY_TYPE_SGD</c>、<c>CURRENCY_TYPE_THB</c>、<c>CURRENCY_TYPE_TRY</c>、<c>CURRENCY_TYPE_VND</c>。
    /// </summary>
    [JsonPropertyName("currency_type")]
    public string? CurrencyType { get; set; }

    /// <summary>获取或设置小数点的位数，即数字精度（官方 <c>decimal_places</c>）。</summary>
    [JsonPropertyName("decimal_places")]
    public int? DecimalPlaces { get; set; }

    /// <summary>获取或设置是否使用千位符（官方 <c>use_separate</c>），设置后使用英文逗号分隔千分位，例如 <c>1,000</c>。</summary>
    [JsonPropertyName("use_separate")]
    public bool? UseSeparate { get; set; }
}
