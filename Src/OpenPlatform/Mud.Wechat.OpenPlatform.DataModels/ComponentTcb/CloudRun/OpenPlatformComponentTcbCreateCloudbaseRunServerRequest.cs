// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>创建云托管服务请求。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseservice.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbCreateCloudbaseRunServerRequest
{
    /// <summary>环境id</summary>
    [JsonPropertyName("env_id")]
    public string? EnvId { get; set; }

    /// <summary>服务名称</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }

    /// <summary>是否开通外网访问</summary>
    [JsonPropertyName("is_public")]
    public bool? IsPublic { get; set; }

    /// <summary>镜像仓库</summary>
    [JsonPropertyName("image_repo")]
    public string? ImageRepo { get; set; }

    /// <summary>服务描述</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>es信息</summary>
    [JsonPropertyName("es_info")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerRequestEsInfo? EsInfo { get; set; }

    /// <summary>日志类型; es/cls</summary>
    [JsonPropertyName("log_type")]
    public string? LogType { get; set; }

    /// <summary>vpc信息</summary>
    [JsonPropertyName("vpc_info")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerRequestVpcInfo? VpcInfo { get; set; }

    /// <summary>0/1</summary>
    [JsonPropertyName("public_access")]
    public long? PublicAccess { get; set; }
}
