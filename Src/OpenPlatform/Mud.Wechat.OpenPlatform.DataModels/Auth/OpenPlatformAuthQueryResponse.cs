// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Auth;

/// <summary>
/// 小程序认证进度查询应答（官方 <c>queryauth</c>；2026-10-17 逐页核验：task_status / apply_status / orderid 官方标 number，其余标 string）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_queryauth.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class OpenPlatformAuthQueryResponse : OpenPlatformResponse
{
    /// <summary>任务状态（官方 <c>task_status</c>，number）。</summary>
    [JsonPropertyName("task_status")]
    public long TaskStatus { get; set; }

    /// <summary>申请状态（官方 <c>apply_status</c>，number）。</summary>
    [JsonPropertyName("apply_status")]
    public long ApplyStatus { get; set; }

    /// <summary>认证订单号（官方 <c>orderid</c>，number）。</summary>
    [JsonPropertyName("orderid")]
    public long OrderId { get; set; }

    /// <summary>授权方小程序 appid（官方 <c>appid</c>，string）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>认证跳转链接（官方 <c>auth_url</c>，string）。</summary>
    [JsonPropertyName("auth_url")]
    public string? AuthUrl { get; set; }

    /// <summary>驳回补充理由（官方 <c>refill_reason</c>，string）。</summary>
    [JsonPropertyName("refill_reason")]
    public string? RefillReason { get; set; }

    /// <summary>失败原因（官方 <c>fail_reason</c>，string）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }
}
