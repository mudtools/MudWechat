// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 审核限额（官方 <c>audit_quota</c> 对象，<c>product/getauditquota</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAuditQuota
{
    /// <summary>获取或设置是否已被限制（官方 <c>block_status</c>：1 时提审商品会变为 quota 不足状态）。</summary>
    [JsonPropertyName("block_status")]
    public int? BlockStatus { get; set; }

    /// <summary>获取或设置可用配额（官方 <c>avail_quota</c>，用尽时提审商品会变为 quota 不足状态）。</summary>
    [JsonPropertyName("avail_quota")]
    public long? AvailQuota { get; set; }

    /// <summary>获取或设置总配额（官方 <c>total_quota</c>）。</summary>
    [JsonPropertyName("total_quota")]
    public long? TotalQuota { get; set; }

    /// <summary>获取或设置是否不限制提审（官方 <c>unlimited_type</c>：1 时不受限、其他参数可忽略）。</summary>
    [JsonPropertyName("unlimited_type")]
    public int? UnlimitedType { get; set; }

    /// <summary>获取或设置审核总配额（官方 <c>audit_total_quota</c>）。</summary>
    [JsonPropertyName("audit_total_quota")]
    public long? AuditTotalQuota { get; set; }

    /// <summary>获取或设置审核剩余配额（官方 <c>audit_total_remaining</c>）。</summary>
    [JsonPropertyName("audit_total_remaining")]
    public long? AuditTotalRemaining { get; set; }

    /// <summary>获取或设置新增商品审核总配额（官方 <c>new_product_total_quota</c>）。</summary>
    [JsonPropertyName("new_product_total_quota")]
    public long? NewProductTotalQuota { get; set; }

    /// <summary>获取或设置新增商品审核剩余配额（官方 <c>new_product_remaining</c>）。</summary>
    [JsonPropertyName("new_product_remaining")]
    public long? NewProductRemaining { get; set; }
}
