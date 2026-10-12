// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>
/// 类目审核单（官方 <c>list</c> 数组元素与 <c>getbizcatflowdetail</c> 响应 <c>info</c> 对象字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAuditFlow
{
    /// <summary>获取或设置类目 id（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public long? CatId { get; set; }

    /// <summary>获取或设置审核单状态（官方 <c>status</c>，见 <see cref="ChannelsCategoryAuditStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public long? AuditId { get; set; }

    /// <summary>获取或设置资质 id（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置审核材料（官方 <c>audit_info</c>）。</summary>
    [JsonPropertyName("audit_info")]
    public ChannelsCategoryAuditInfo? AuditInfo { get; set; }

    /// <summary>获取或设置审核时间（官方 <c>audit_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("audit_time")]
    public long? AuditTime { get; set; }

    /// <summary>获取或设置驳回原因（官方 <c>audit_reason</c>）。</summary>
    [JsonPropertyName("audit_reason")]
    public string? AuditReason { get; set; }
}
