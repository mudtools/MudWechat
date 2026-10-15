// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>配置云托管版本请求。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/setcsversionconfig.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest
{
    /// <summary>环境ID</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>服务名称</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }

    /// <summary>服务名称</summary>
    [JsonPropertyName("version_name")]
    public string? VersionName { get; set; }

    /// <summary>最小副本数，最小值：0</summary>
    [JsonPropertyName("minnum")]
    public long? Minnum { get; set; }

    /// <summary>副本最大数，最大值：50</summary>
    [JsonPropertyName("maxnum")]
    public long? Maxnum { get; set; }

    /// <summary>备注</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>服务端口</summary>
    [JsonPropertyName("container_port")]
    public long? ContainerPort { get; set; }

    /// <summary>用户自定义采集日志路径</summary>
    [JsonPropertyName("custom_logs")]
    public string? CustomLogs { get; set; }

    /// <summary>环境变量</summary>
    [JsonPropertyName("env_params")]
    public string? EnvParams { get; set; }
}
