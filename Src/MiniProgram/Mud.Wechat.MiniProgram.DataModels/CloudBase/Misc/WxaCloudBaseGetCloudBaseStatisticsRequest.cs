// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>获取云开发数据请求。官方页：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_getcloudbasestatistics.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseGetCloudBaseStatisticsRequest
{
    /// <summary>获取动作，目前支持（smsMarketingOverviewData：短信营销概览数据；smsMarketingConversionData：短信营销转化数据；smsMarketingRealTimeData：短信营销实时数据）</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>开始时间戳</summary>
    [JsonPropertyName("begin_date")]
    public long? BeginDate { get; set; }

    /// <summary>结束时间戳</summary>
    [JsonPropertyName("end_date")]
    public long? EndDate { get; set; }

    /// <summary>分页 limit【action 取 smsMarketingOverviewData、smsMarketingConversionData 时必填】</summary>
    [JsonPropertyName("page_limit")]
    public long? PageLimit { get; set; }

    /// <summary>分页 offset【action 取 smsMarketingOverviewData、smsMarketingConversionData 时必填】</summary>
    [JsonPropertyName("page_offset")]
    public long? PageOffset { get; set; }
}
