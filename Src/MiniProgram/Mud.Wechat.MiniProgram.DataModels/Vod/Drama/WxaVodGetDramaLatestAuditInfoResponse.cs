// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询剧目审核信息」响应。
/// </summary>
/// <remarks>
/// <para><b>与 <c>getdrama</c> 的 <c>audit_detail</c> 分工（官方原文）</b>：getdrama 侧的 <c>audit_type</c> 仅表示 0 或 1 的审核信息，替换剧集与修改基本信息的审核须走本页按 <c>audit_type</c> 查。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdramalatestauditinfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetDramaLatestAuditInfoResponse : WxaResponse
{
    /// <summary>audit_detail（官方类型 <c>object</c>）：审核信息，区别于getdrama接口的audit_detail，getdrama接口的audit_type仅表示audit_type=0或1的审核信息。</summary>
    [JsonPropertyName("audit_detail")]
    public WxaVodDramaAuditDetail? AuditDetail { get; set; }
}
