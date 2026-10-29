// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>查询短信记录响应。官方页：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_describesmsrecords.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseDescribeSmsRecordsResponse : WxaResponse
{
    /// <summary>发送记录列表</summary>
    [JsonPropertyName("SmsRecords")]
    public List<WxaCloudBaseDescribeSmsRecordsResponseSmsRecordsItem>? SmsRecords { get; set; }

    /// <summary>记录总数</summary>
    [JsonPropertyName("TotalCount")]
    public long? TotalCount { get; set; }

    /// <summary>唯一请求 ID，每次请求都会返回。定位问题时需要提供该次请求的 RequestId。</summary>
    [JsonPropertyName("RequestId")]
    public string? RequestId { get; set; }
}
