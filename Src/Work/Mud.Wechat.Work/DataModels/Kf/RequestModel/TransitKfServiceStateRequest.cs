// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 变更会话状态请求体（<c>/cgi-bin/kf/service_state/trans</c>）。
/// <para>
/// 目标状态（service_state）变更范围：0 - 未处理，1 - 由智能助手接待，2 - 待接入池排队中，
/// 3 - 由人工接待；4 - 已结束/未开始不允许通过 API 变更。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class TransitKfServiceStateRequest
{
    /// <summary>
    /// 获取或设置客服账号 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置微信客户的 external_userid（官方必填）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置目标会话状态（官方必填；0 - 未处理，1 - 由智能助手接待，2 - 待接入池排队中，3 - 由人工接待）。
    /// </summary>
    [JsonPropertyName("service_state")]
    public int? ServiceState { get; set; }

    /// <summary>
    /// 获取或设置接待人员 userid（第三方 / 代开发应用填密文 userid 即 open_userid）。
    /// <para>
    /// <see cref="ServiceState"/> 为 3（由人工接待）时必填，且接待人员须处于「正在接待」中；
    /// 接待人员未在企业微信激活时官方返回错误码 95014。
    /// </para>
    /// </summary>
    [JsonPropertyName("servicer_userid")]
    public string? ServicerUserId { get; set; }
}
