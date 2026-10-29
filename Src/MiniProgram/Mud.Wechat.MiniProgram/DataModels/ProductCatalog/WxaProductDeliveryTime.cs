// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 同城配送时间（官方 <c>delivery_time</c>）。
/// </summary>
/// <remarks>
/// <para>读写同形；营业时间按「时 + 分」，送达范围单位米、送达时间单位分钟。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductDeliveryTime
{
    /// <summary>开店时间，小时。</summary>
    [JsonPropertyName("start_business_hour")]
    public int? StartBusinessHour { get; set; }

    /// <summary>开店时间，分钟。</summary>
    [JsonPropertyName("start_business_minute")]
    public int? StartBusinessMinute { get; set; }

    /// <summary>关店时间，小时。</summary>
    [JsonPropertyName("end_business_hour")]
    public int? EndBusinessHour { get; set; }

    /// <summary>关店时间，分钟。</summary>
    [JsonPropertyName("end_business_minute")]
    public int? EndBusinessMinute { get; set; }

    /// <summary>可预定时间，分钟。</summary>
    [JsonPropertyName("book_time")]
    public int? BookTime { get; set; }

    /// <summary>送达范围（米）。</summary>
    [JsonPropertyName("first_val_meter")]
    public int? FirstValMeter { get; set; }

    /// <summary>送达时间（分钟）。</summary>
    [JsonPropertyName("first_val_minutes")]
    public int? FirstValMinutes { get; set; }

    /// <summary>送达范围增量（米）。</summary>
    [JsonPropertyName("second_val_meter")]
    public int? SecondValMeter { get; set; }

    /// <summary>送达时间增量（分钟）。</summary>
    [JsonPropertyName("second_val_minutes")]
    public int? SecondValMinutes { get; set; }
}
