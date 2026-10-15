// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>创建服务版本请求。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseserviceversion.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest
{
    /// <summary>环境ID</summary>
    [JsonPropertyName("env_id")]
    public string? EnvId { get; set; }

    /// <summary>枚举（package/repository/image/jar/war)</summary>
    [JsonPropertyName("upload_type")]
    public string? UploadType { get; set; }

    /// <summary>流量占比</summary>
    [JsonPropertyName("flow_ratio")]
    public double? FlowRatio { get; set; }

    /// <summary>Cpu的大小，单位：核</summary>
    [JsonPropertyName("cpu")]
    public double? Cpu { get; set; }

    /// <summary>Mem的大小，单位：G</summary>
    [JsonPropertyName("mem")]
    public double? Mem { get; set; }

    /// <summary>最小副本数，最小值：0</summary>
    [JsonPropertyName("min_num")]
    public long? MinNum { get; set; }

    /// <summary>副本最大数，最大值：50</summary>
    [JsonPropertyName("max_num")]
    public long? MaxNum { get; set; }

    /// <summary>策略类型(枚举值：比如cpu)</summary>
    [JsonPropertyName("policy_type")]
    public string? PolicyType { get; set; }

    /// <summary>策略阈值</summary>
    [JsonPropertyName("policy_threshold")]
    public double? PolicyThreshold { get; set; }

    /// <summary>服务端口</summary>
    [JsonPropertyName("container_port")]
    public long? ContainerPort { get; set; }

    /// <summary>服务名称</summary>
    [JsonPropertyName("server_name")]
    public string? ServerName { get; set; }

    /// <summary>repository的类型(coding/gitlab/github/coding)</summary>
    [JsonPropertyName("repository_type")]
    public string? RepositoryType { get; set; }

    /// <summary>Dockerfile地址</summary>
    [JsonPropertyName("dockerfile_path")]
    public string? DockerfilePath { get; set; }

    /// <summary>构建目录</summary>
    [JsonPropertyName("build_dir")]
    public string? BuildDir { get; set; }

    /// <summary>环境变量</summary>
    [JsonPropertyName("env_params")]
    public string? EnvParams { get; set; }

    /// <summary>repository地址</summary>
    [JsonPropertyName("repository")]
    public string? Repository { get; set; }

    /// <summary>分支</summary>
    [JsonPropertyName("branch")]
    public string? Branch { get; set; }

    /// <summary>版本备注</summary>
    [JsonPropertyName("version_remark")]
    public string? VersionRemark { get; set; }

    /// <summary>代码包名字</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>代码包的版本</summary>
    [JsonPropertyName("package_version")]
    public string? PackageVersion { get; set; }

    /// <summary>Image的详情</summary>
    [JsonPropertyName("image_info")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestImageInfo? ImageInfo { get; set; }

    /// <summary>Github等拉取代码的详情</summary>
    [JsonPropertyName("code_detail")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestCodeDetail? CodeDetail { get; set; }

    /// <summary>私有镜像秘钥信息</summary>
    [JsonPropertyName("image_secret_info")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestImageSecretInfo? ImageSecretInfo { get; set; }

    /// <summary>私有镜像 认证名称</summary>
    [JsonPropertyName("image_pull_secret")]
    public string? ImagePullSecret { get; set; }

    /// <summary>用户自定义采集日志路径</summary>
    [JsonPropertyName("custom_logs")]
    public string? CustomLogs { get; set; }

    /// <summary>延迟多长时间开始健康检查（单位s）</summary>
    [JsonPropertyName("initial_delay_seconds")]
    public long? InitialDelaySeconds { get; set; }

    /// <summary>cfs挂载信息</summary>
    [JsonPropertyName("mount_volume_info")]
    public List<OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestMountVolumeInfoItem>? MountVolumeInfo { get; set; }

    /// <summary>4 代表只能微信链路访问</summary>
    [JsonPropertyName("access_type")]
    public long? AccessType { get; set; }

    /// <summary>es信息</summary>
    [JsonPropertyName("es_info")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestEsInfo? EsInfo { get; set; }

    /// <summary>是否使用统一域名</summary>
    [JsonPropertyName("enable_union")]
    public bool? EnableUnion { get; set; }

    /// <summary>服务路径</summary>
    [JsonPropertyName("server_path")]
    public string? ServerPath { get; set; }

    /// <summary>容器的描述文件</summary>
    [JsonPropertyName("sidecar_specs")]
    public List<OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestSidecarSpecsItem>? SidecarSpecs { get; set; }

    /// <summary>安全特性</summary>
    [JsonPropertyName("security")]
    public OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestSecurity? Security { get; set; }

    /// <summary>服务磁盘挂载</summary>
    [JsonPropertyName("service_volumes")]
    public List<OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestServiceVolumesItem>? ServiceVolumes { get; set; }

    /// <summary>是否创建JnsGw 0未传默认创建 1创建 2不创建</summary>
    [JsonPropertyName("is_create_jns_gw")]
    public long? IsCreateJnsGw { get; set; }

    /// <summary>数据卷挂载参数</summary>
    [JsonPropertyName("service_volume_mounts")]
    public List<OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestServiceVolumeMountsItem>? ServiceVolumeMounts { get; set; }

    /// <summary>是否有Dockerfile：0-default has, 1-has, 2-has not</summary>
    [JsonPropertyName("has_dockerfile")]
    public long? HasDockerfile { get; set; }

    /// <summary>基础镜像</summary>
    [JsonPropertyName("base_image")]
    public string? BaseImage { get; set; }

    /// <summary>容器启动入口命令</summary>
    [JsonPropertyName("entry_point")]
    public string? EntryPoint { get; set; }

    /// <summary>仓库语言</summary>
    [JsonPropertyName("repo_language")]
    public string? RepoLanguage { get; set; }

    /// <summary>文件名</summary>
    [JsonPropertyName("upload_filename")]
    public string? UploadFilename { get; set; }

    /// <summary>自动扩缩容策略组</summary>
    [JsonPropertyName("policy_detail")]
    public List<OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestPolicyDetailItem>? PolicyDetail { get; set; }
}
