// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目审核信息（官方 <c>audit_detail</c>：<c>status</c> + <c>audit_type</c> + 两个时间戳）。
/// </summary>
/// <remarks>
/// <para><b><c>audit_type</c> 官方标注「事件通知接口才返回」</b>⇒ 查询类接口（getdrama / listdramas）里通常为空；<c>getdramalatestauditinfo</c> 的 <c>audit_detail</c> 与本类型字段面相同，共用之。</para>
/// <para><b>与媒资审核信息不共用</b>：媒资侧 <c>audit_detail</c> 多 <c>reason</c>/<c>evidence_material_id_list</c>、无 <c>audit_type</c>，见 <see cref="WxaVodMediaAuditDetail"/>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodDramaAuditDetail
{
    /// <summary>status（官方类型 <c>number</c>）：0为无效值；1为审核中；2为最终失败；3为审核通过；4为驳回重填</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>audit_type（官方类型 <c>number</c>）：事件通知接口才返回：0为首次提审；1为再次提审；2为替换剧集提审；3为修改剧目基本信息审核。</summary>
    [JsonPropertyName("audit_type")]
    public long? AuditType { get; set; }

    /// <summary>create_time（官方类型 <c>number</c>）：提审时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>audit_time（官方类型 <c>number</c>）：审核时间戳。</summary>
    [JsonPropertyName("audit_time")]
    public long? AuditTime { get; set; }
}
