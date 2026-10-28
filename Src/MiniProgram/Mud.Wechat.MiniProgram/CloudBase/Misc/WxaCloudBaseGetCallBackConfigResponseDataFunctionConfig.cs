// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>获取消息推送配置·data.function_config。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/msg-push/api_getcallbackconfig.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseGetCallBackConfigResponseDataFunctionConfig
{
    /// <summary>状态</summary>
    [JsonPropertyName("enable")]
    public bool? Enable { get; set; }

    /// <summary>配置项</summary>
    [JsonPropertyName("callbacks")]
    public List<WxaCloudBaseGetCallBackConfigResponseDataFunctionConfigCallbacksItem>? Callbacks { get; set; }
}
