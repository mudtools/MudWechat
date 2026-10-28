// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>延时调用云函数请求。官方页：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/functions/api_adddelayedfunctiontask.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseAddDelayedFunctionTaskRequest
{
    /// <summary>环境ID</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>函数名称</summary>
    [JsonPropertyName("function_name")]
    public string? FunctionName { get; set; }

    /// <summary>发送的数据包，格式必须为JSONString</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }

    /// <summary>延迟时间，单位：秒，合法范围：6秒-30天</summary>
    [JsonPropertyName("delay_time")]
    public long? DelayTime { get; set; }
}
