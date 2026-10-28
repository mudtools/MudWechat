// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>新建云托管服务版本请求。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/createcsversion.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbCreateContainerServiceVersionRequest
{
    /// <summary>环境ID</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>服务名称</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }

    /// <summary>上传方式（package/repository/image)</summary>
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
    [JsonPropertyName("minnum")]
    public long? Minnum { get; set; }

    /// <summary>副本最大数，最大值：50</summary>
    [JsonPropertyName("maxnum")]
    public long? Maxnum { get; set; }

    /// <summary>策略类型(枚举值：cpu)</summary>
    [JsonPropertyName("policy_type")]
    public string? PolicyType { get; set; }

    /// <summary>策略阈值</summary>
    [JsonPropertyName("policy_threshold")]
    public double? PolicyThreshold { get; set; }

    /// <summary>服务端口</summary>
    [JsonPropertyName("container_port")]
    public long? ContainerPort { get; set; }

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
    public OpenPlatformComponentTcbCreateContainerServiceVersionRequestImageInfo? ImageInfo { get; set; }

    /// <summary>Github等拉取代码的详情</summary>
    [JsonPropertyName("code_detail")]
    public OpenPlatformComponentTcbCreateContainerServiceVersionRequestCodeDetail? CodeDetail { get; set; }

    /// <summary>私有镜像秘钥信息</summary>
    [JsonPropertyName("image_secret_info")]
    public OpenPlatformComponentTcbCreateContainerServiceVersionRequestImageSecretInfo? ImageSecretInfo { get; set; }

    /// <summary>私有镜像 认证名称</summary>
    [JsonPropertyName("image_pulls_secret")]
    public string? ImagePullsSecret { get; set; }

    /// <summary>用户自定义采集日志路径</summary>
    [JsonPropertyName("custom_logs")]
    public string? CustomLogs { get; set; }

    /// <summary>是否开启微信令牌自动推送</summary>
    [JsonPropertyName("mount_wx_token")]
    public bool? MountWxToken { get; set; }
}
