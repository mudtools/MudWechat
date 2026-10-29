// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询剧目审核信息」请求体（<c>POST /wxa/sec/vod/getdramalatestauditinfo</c>）：按审核类型查最近一次审核详情。
/// </summary>
/// <remarks>
/// <para><b><c>audit_type</c> 必填</b>（官方原文）：0 首次提审 / 1 再次提审 / 2 替换剧集提审 / 3 修改剧目基本信息审核，见 <see cref="WxaVodDramaAuditTypes"/>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdramalatestauditinfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetDramaLatestAuditInfoRequest
{
    /// <summary>drama_id（官方类型 <c>number</c>，<b>必填</b>）：剧目id</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>audit_type（官方类型 <c>number</c>，<b>必填</b>）：审核类型。0为首次提审；1为再次提审；2为替换剧集提审；3为修改剧目基本信息审核。</summary>
    [JsonPropertyName("audit_type")]
    public long? AuditType { get; set; }
}
