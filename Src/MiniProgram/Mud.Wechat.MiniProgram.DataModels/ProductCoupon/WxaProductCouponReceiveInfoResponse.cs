// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券领取设置（响应侧，时间为字符串）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponReceiveInfoResponse
{
    /// <summary>领用结束时间（字符串）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>单人限领张数。</summary>
    [JsonPropertyName("limit_num_one_person")]
    public int? LimitNumOnePerson { get; set; }

    /// <summary>领用开始时间（字符串）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>领用总数。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}
