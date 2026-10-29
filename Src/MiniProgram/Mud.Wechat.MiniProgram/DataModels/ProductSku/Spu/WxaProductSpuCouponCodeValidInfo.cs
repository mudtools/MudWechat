// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>券码商品有效期信息（<c>couponcode_info.valid_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProductSku")]
public class WxaProductSpuCouponCodeValidInfo
{
    /// <summary>生效方式：1 指定时间区间生效；2 指定天数生效（购买后 N 天内有效）。</summary>
    [JsonPropertyName("valid_type")]
    public int? ValidType { get; set; }

    /// <summary>有效天数，<c>valid_type = 2</c> 时必填，取值 1~365。</summary>
    [JsonPropertyName("valid_day_num")]
    public int? ValidDayNum { get; set; }

    /// <summary>开始生效日期（时间戳），<c>valid_type = 1</c> 时必填。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>失效日期（时间戳），<c>valid_type = 1</c> 时必填，最大为调用时间戳 + 365 天。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
