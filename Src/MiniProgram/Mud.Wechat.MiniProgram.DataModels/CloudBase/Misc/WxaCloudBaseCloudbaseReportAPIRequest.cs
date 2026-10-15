// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>云开发上报接口请求。官方页：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_cloudbasereportapi.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseCloudbaseReportAPIRequest
{
    /// <summary>上报动作，目前支持（sendSmsTask：发送短信；openH5：H5 打开）</summary>
    [JsonPropertyName("report_action")]
    public string? ReportAction { get; set; }

    /// <summary>环境 ID</summary>
    [JsonPropertyName("env_id")]
    public string? EnvId { get; set; }

    /// <summary>活动 ID</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>任务 ID【report_action 取 sendSmsTask 时必填】</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>下发手机号数量【report_action 取 sendSmsTask 时必填】</summary>
    [JsonPropertyName("phone_count")]
    public string? PhoneCount { get; set; }

    /// <summary>渠道 ID（云开发 CMS 使用 _cms_sms_）【report_action 取 openH5 时必填】</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>会话 ID【report_action 取 openH5 时必填】</summary>
    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }
}
