// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 已开通服务条目（官方 <c>service_list[]</c>）。
/// </summary>
/// <remarks>
/// <para>服务类型 2 商品 / 3 订单 / 4 物流；<c>expire_time</c> 为字符串而非时间戳。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductServiceListItem
{
    /// <summary>服务 ID。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>服务名称。</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }

    /// <summary>过期时间（字符串）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    /// <summary>服务类型（2 商品 / 3 订单 / 4 物流）。</summary>
    [JsonPropertyName("service_type")]
    public int? ServiceType { get; set; }
}
