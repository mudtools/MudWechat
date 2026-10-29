// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 流量包详情（官方 <c>package_list(Array)</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b>单位为 MB、十进制换算</b>（官方原文 1GB=1000MB）；剩余流量 = <c>all</c> - <c>used</c>。</para>
/// <para><b>「剩余可用流量」是官方口径而非本字段算术</b>（官方原文）：仅 <c>status=1</c>（有效）的流量包余额之和。</para>
/// <para><b><c>all</c> 与 <c>is_deleted</c> 照录官方名</b>（<c>all</c> 非 C# 关键字，属性名直接用 <c>All</c>）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_listpackages.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodPackageInfo
{
    /// <summary>start_time（官方类型 <c>number</c>）：有效期起始时间</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>end_time（官方类型 <c>number</c>）：有效期截止时间</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>used（官方类型 <c>number</c>）：流量包已消耗流量的数值（MB）</summary>
    [JsonPropertyName("used")]
    public long? Used { get; set; }

    /// <summary>all（官方类型 <c>number</c>）：流量包总额（MB），剩余流量=流量包总额-已消耗流量</summary>
    [JsonPropertyName("all")]
    public long? All { get; set; }

    /// <summary>order_id（官方类型 <c>string</c>）：订单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：流量包状态，参考请求参数status中的说明</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>is_deleted（官方类型 <c>number</c>）：是否已删除（失效），例如已退款</summary>
    [JsonPropertyName("is_deleted")]
    public long? IsDeleted { get; set; }

    /// <summary>package_id（官方类型 <c>string</c>）：流量包编号</summary>
    [JsonPropertyName("package_id")]
    public string? PackageId { get; set; }
}
