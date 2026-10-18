// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 申请开通小程序直播回包（<c>POST /wxa/business/applyliveinfo</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>errcode=0</c> 仅表示申请已受理</b>：官方会推送 <c>wxa_live_apply_event</c> 通知，
/// 审核结果以通知为准；<c>errcode=2</c> 表示近 90 天无支付行为、暂不满足开通条件。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpApplyLiveInfoResponse : OpenPlatformResponse
{
    /// <summary>操作类型（官方回传 <c>apply</c>）。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }
}
