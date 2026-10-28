// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 核销券信息（官方 <c>CouponCodeInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>未核销订单无后两键</b>：<c>verifier_nickname</c> / <c>verify_time</c> 仅已核销订单出现。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderCouponCodeInfo
{
    /// <summary>券码开始生效时间，秒级时间戳。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>券码过期时间，秒级时间戳。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>核销方式：0 每天都能核销 / 1 节假日不可用。</summary>
    [JsonPropertyName("verify_type")]
    public long? VerifyType { get; set; }

    /// <summary>买家联系方式（敏感）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>核销人微信昵称（仅已核销订单）。</summary>
    [JsonPropertyName("verifier_nickname")]
    public string? VerifierNickname { get; set; }

    /// <summary>核销时间（仅已核销订单）。</summary>
    [JsonPropertyName("verify_time")]
    public long? VerifyTime { get; set; }
}
