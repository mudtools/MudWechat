// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 重新提交商品审核应答（<c>POST /wxaapi/broadcast/goods/audit</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_resubmitaudit.html"/>。
/// </para>
/// <para>
/// <b>新审核单 ID</b>：官方示例返回 <c>{ "errcode": 0, "auditId": 525022184 }</c> ——
/// 重提审产生<b>新的</b> <c>auditId</c>，宿主须以本值覆盖此前保存的审核单 ID（撤回审核要用它）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveResubmitAuditResponse : WxaResponse
{
    /// <summary>新的审核单 ID（<c>auditId</c>，官方类型 number）。</summary>
    [JsonPropertyName("auditId")]
    public long? AuditId { get; set; }
}
