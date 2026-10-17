// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 关联商家条目（官方 <c>list[]</c> 元素，<c>get_ec_list</c> 应答子对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsEcRelationItem
{
    /// <summary>获取或设置小店商家 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置商家店铺名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置关联状态（官方 <c>status</c>；枚举 0 关联申请（待审核 / 已主动拒绝 / 超时自动拒绝，
    /// 须结合 <see cref="BindAuditStatus"/> 详判）/ 1 已关联 / 2 已取消关联）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置关联状态详情（官方 <c>bind_audit_status</c>；枚举 0 待审核 / 1 已关联（Accept）/
    /// 2 已主动拒绝 / 3 超时自动拒绝 / 5 已取消关联——官方无 4，照录）。
    /// </summary>
    [JsonPropertyName("bind_audit_status")]
    public int? BindAuditStatus { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置商家店铺头图（官方 <c>head_img</c>）。</summary>
    [JsonPropertyName("head_img")]
    public string? HeadImg { get; set; }
}
