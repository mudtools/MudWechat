// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>查询短信记录·SmsRecords(Array)。官方页：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_describesmsrecords.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseDescribeSmsRecordsResponseSmsRecordsItem
{
    /// <summary>手机号码</summary>
    [JsonPropertyName("Mobile")]
    public string? Mobile { get; set; }

    /// <summary>短信内容</summary>
    [JsonPropertyName("Content")]
    public string? Content { get; set; }

    /// <summary>短信内容长度</summary>
    [JsonPropertyName("ContentSize")]
    public long? ContentSize { get; set; }

    /// <summary>计费条数</summary>
    [JsonPropertyName("Fee")]
    public long? Fee { get; set; }

    /// <summary>发送时间</summary>
    [JsonPropertyName("CreateTime")]
    public string? CreateTime { get; set; }

    /// <summary>用户接收时间</summary>
    [JsonPropertyName("ReceivedTime")]
    public string? ReceivedTime { get; set; }

    /// <summary>sent(成功), error(失败)</summary>
    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    /// <summary>备注</summary>
    [JsonPropertyName("Remarks")]
    public string? Remarks { get; set; }
}
