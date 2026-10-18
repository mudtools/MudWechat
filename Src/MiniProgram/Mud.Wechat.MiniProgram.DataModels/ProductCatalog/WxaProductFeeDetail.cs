// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 特殊时段费用项（官方 <c>fee_details[]</c>）。
/// </summary>
/// <remarks>
/// <para>按「时 + 分」描述时段，<c>fee</c> 为该时段费用（分）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductFeeDetail
{
    /// <summary>开始时间，小时。</summary>
    [JsonPropertyName("start_hour")]
    public int? StartHour { get; set; }

    /// <summary>开始时间，分钟。</summary>
    [JsonPropertyName("start_minute")]
    public int? StartMinute { get; set; }

    /// <summary>结束时间，小时。</summary>
    [JsonPropertyName("end_hour")]
    public int? EndHour { get; set; }

    /// <summary>结束时间，分钟。</summary>
    [JsonPropertyName("end_minute")]
    public int? EndMinute { get; set; }

    /// <summary>该时段费用（分）。</summary>
    [JsonPropertyName("fee")]
    public long? Fee { get; set; }
}
