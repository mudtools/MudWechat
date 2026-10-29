// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>查询短信记录请求。官方页：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_describesmsrecords.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseDescribeSmsRecordsRequest
{
    /// <summary>环境 ID</summary>
    [JsonPropertyName("EnvId")]
    public string? EnvId { get; set; }

    /// <summary>开始日期, 如:2021-01-01</summary>
    [JsonPropertyName("StartDate")]
    public string? StartDate { get; set; }

    /// <summary>结束日期, 如2021-01-07</summary>
    [JsonPropertyName("EndDate")]
    public string? EndDate { get; set; }

    /// <summary>电话号码</summary>
    [JsonPropertyName("Mobile")]
    public string? Mobile { get; set; }

    /// <summary>查询ID</summary>
    [JsonPropertyName("QueryId")]
    public string? QueryId { get; set; }

    /// <summary>页码(1开始)</summary>
    [JsonPropertyName("PageNumber")]
    public long? PageNumber { get; set; }

    /// <summary>每页条目数</summary>
    [JsonPropertyName("PageSize")]
    public long? PageSize { get; set; }
}
