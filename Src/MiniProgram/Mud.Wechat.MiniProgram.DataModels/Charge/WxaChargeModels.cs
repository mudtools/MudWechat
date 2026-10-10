// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Charge;

/// <summary>付费能力资源包用量（<c>resources[]</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Charge")]
public class WxaChargeResourceUsage
{
    /// <summary>资源包 ID（<c>resource_id</c>）。</summary>
    [JsonPropertyName("resource_id")]
    public string? ResourceId { get; set; }

    /// <summary>资源包名称（<c>resource_name</c>）。</summary>
    [JsonPropertyName("resource_name")]
    public string? ResourceName { get; set; }

    /// <summary>已用数量（<c>used</c>）。</summary>
    [JsonPropertyName("used")]
    public long? Used { get; set; }

    /// <summary>总量（<c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>资源包到期时间（<c>expire_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}

/// <summary>查询购买资源包用量情况应答（<c>POST /wxa/charge/usage/get</c>）。</summary>
/// <remarks>官方文档：<c>charge/api_getusagedetail.html</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Charge")]
public class WxaChargeUsageResponse : WxaResponse
{
    /// <summary>资源包用量列表（<c>resources</c>），见 <see cref="WxaChargeResourceUsage"/>。</summary>
    [JsonPropertyName("resources")]
    public List<WxaChargeResourceUsage>? Resources { get; set; }
}

/// <summary>付费能力最近用量（<c>resources[]</c>）。</summary>
/// <remarks>「最近三个月平均用量」以官方字段与口径为准；SDK 不做本地计算。</remarks>
[HttpJsonSerializable(SerializerClassName = "Charge")]
public class WxaChargeResourceAverage
{
    /// <summary>付费能力标识（<c>resource_id</c>）。</summary>
    [JsonPropertyName("resource_id")]
    public string? ResourceId { get; set; }

    /// <summary>付费能力名称（<c>resource_name</c>）。</summary>
    [JsonPropertyName("resource_name")]
    public string? ResourceName { get; set; }

    /// <summary>最近三个月平均用量（<c>average</c>）。</summary>
    [JsonPropertyName("average")]
    public long? Average { get; set; }
}

/// <summary>获取某个付费能力的最近用量数据应答（<c>POST /wxa/charge/usage/get_recent_average</c>）。</summary>
/// <remarks>官方文档：<c>charge/api_getrecentaverageusage.html</c>；查询某个付费能力最近三个月的平均用量。</remarks>
[HttpJsonSerializable(SerializerClassName = "Charge")]
public class WxaChargeRecentAverageResponse : WxaResponse
{
    /// <summary>最近用量列表（<c>resources</c>），见 <see cref="WxaChargeResourceAverage"/>。</summary>
    [JsonPropertyName("resources")]
    public List<WxaChargeResourceAverage>? Resources { get; set; }
}