// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 媒资（剧集）审核信息（官方 <c>Res.media_info.audit_detail</c>）。
/// </summary>
/// <remarks>
/// <para><b>与剧目审核信息同名不同面</b>：本类型 <c>status</c> 取值集见 <see cref="WxaVodMediaAuditStatuses"/>，<b>无</b> <c>audit_type</c>；剧目侧的审核信息是 <see cref="WxaVodDramaAuditDetail"/>（多 <c>audit_type</c>、<c>status</c> 的 <c>2</c> 语义为「最终失败」）⇒ 两者<b>不共用</b>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedia.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodMediaAuditDetail
{
    /// <summary>status（官方类型 <c>number</c>）：审核状态，需要注意可能存在单个剧集的状态为审核通过，但是剧目整体是未通过的情况，而能不能获取播放链接取决于剧目的审核状态</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>create_time（官方类型 <c>number</c>）：提审时间戳</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>audit_time（官方类型 <c>number</c>）：审核时间戳</summary>
    [JsonPropertyName("audit_time")]
    public long? AuditTime { get; set; }

    /// <summary>reason（官方类型 <c>string</c>）：审核备注。该值可能为空</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>evidence_material_id_list（官方类型 <c>array</c>）：审核证据截图id列表，截图id可以用作获取永久素材接口的参数来获得截图内容</summary>
    [JsonPropertyName("evidence_material_id_list")]
    public List<string>? EvidenceMaterialIdList { get; set; }
}
