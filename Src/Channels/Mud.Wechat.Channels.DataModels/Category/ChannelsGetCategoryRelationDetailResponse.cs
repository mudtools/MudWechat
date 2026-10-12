// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>获取店铺的类目权限详情（<c>get_category_relation_detail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRelationDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置类目 ID（官方 <c>category_id</c>）。</summary>
    [JsonPropertyName("category_id")]
    public long? CategoryId { get; set; }

    /// <summary>获取或设置类目状态（官方 <c>status</c>，见 <see cref="ChannelsCategoryRelationStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置失效原因（官方 <c>uneffective_reason</c>）。</summary>
    [JsonPropertyName("uneffective_reason")]
    public string? UneffectiveReason { get; set; }

    /// <summary>获取或设置审核材料（官方 <c>audit_info</c>；与审核单两接口字段集一致 ⇒ 共用 <see cref="ChannelsCategoryAuditInfo"/>）。</summary>
    [JsonPropertyName("audit_info")]
    public ChannelsCategoryAuditInfo? AuditInfo { get; set; }

    /// <summary>获取或设置生效时间（官方 <c>effective_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("effective_time")]
    public long? EffectiveTime { get; set; }

    /// <summary>获取或设置失效时间（官方 <c>uneffective_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("uneffective_time")]
    public long? UneffectiveTime { get; set; }

    /// <summary>获取或设置类目资质 ID（官方 <c>qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }
}
