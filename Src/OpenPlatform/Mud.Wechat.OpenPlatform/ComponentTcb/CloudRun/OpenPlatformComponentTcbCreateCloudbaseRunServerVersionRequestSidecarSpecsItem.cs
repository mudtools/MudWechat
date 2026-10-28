// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>创建服务版本·sidecar_specs(Array)。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseserviceversion.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestSidecarSpecsItem
{
    /// <summary>容器镜像</summary>
    [JsonPropertyName("container_image")]
    public string? ContainerImage { get; set; }

    /// <summary>容器端口</summary>
    [JsonPropertyName("container_port")]
    public long? ContainerPort { get; set; }

    /// <summary>容器的名称</summary>
    [JsonPropertyName("container_name")]
    public string? ContainerName { get; set; }

    /// <summary>kv的 json 字符串</summary>
    [JsonPropertyName("env_var")]
    public string? EnvVar { get; set; }

    /// <summary>InitialDelaySeconds 延迟多长时间启动健康检查</summary>
    [JsonPropertyName("initial_delay_seconds")]
    public long? InitialDelaySeconds { get; set; }

    /// <summary>CPU大小</summary>
    [JsonPropertyName("cpu")]
    public double? Cpu { get; set; }

    /// <summary>内存大小（单位：M）</summary>
    [JsonPropertyName("mem")]
    public double? Mem { get; set; }

    /// <summary>安全特性</summary>
    [JsonPropertyName("security")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestSidecarSpecsItemSecurity? Security { get; set; }

    /// <summary>挂载信息</summary>
    [JsonPropertyName("volume_mount_infos")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestSidecarSpecsItemVolumeMountInfos? VolumeMountInfos { get; set; }
}
