// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>更新云函数配置请求。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_uploadcloudfunctionconfig.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbUploadCloudFunctionConfigRequest
{
    /// <summary>环境ID</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>函数名</summary>
    [JsonPropertyName("functionname")]
    public string? Functionname { get; set; }

    /// <summary>内存大小</summary>
    [JsonPropertyName("memorysize")]
    public double? Memorysize { get; set; }

    /// <summary>超时时间，单位：秒</summary>
    [JsonPropertyName("timeout")]
    public long? Timeout { get; set; }

    /// <summary>环境变量</summary>
    [JsonPropertyName("environment_variables")]
    public List<OpenPlatformComponentTcbUploadCloudFunctionConfigRequestEnvironmentVariablesItem>? EnvironmentVariables { get; set; }

    /// <summary>公网访问配置</summary>
    [JsonPropertyName("public_net_config")]
    public OpenPlatformComponentTcbUploadCloudFunctionConfigRequestPublicNetConfig? PublicNetConfig { get; set; }

    /// <summary>私有网络配置</summary>
    [JsonPropertyName("vpc_config")]
    public OpenPlatformComponentTcbUploadCloudFunctionConfigRequestVpcConfig? VpcConfig { get; set; }
}
