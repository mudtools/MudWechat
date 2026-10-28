// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>查看云托管版本响应。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcsversiondetail.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse : OpenPlatformResponse
{
    /// <summary>备注</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>dockerfile地址</summary>
    [JsonPropertyName("dockerfile_path")]
    public string? DockerfilePath { get; set; }

    /// <summary>构建目录</summary>
    [JsonPropertyName("build_dir")]
    public string? BuildDir { get; set; }

    /// <summary>cpu</summary>
    [JsonPropertyName("cpu")]
    public double? Cpu { get; set; }

    /// <summary>内存</summary>
    [JsonPropertyName("mem")]
    public double? Mem { get; set; }

    /// <summary>最小副本数</summary>
    [JsonPropertyName("minnum")]
    public long? Minnum { get; set; }

    /// <summary>最大副本数</summary>
    [JsonPropertyName("maxnum")]
    public long? Maxnum { get; set; }

    /// <summary>策略类型</summary>
    [JsonPropertyName("policy_type")]
    public string? PolicyType { get; set; }

    /// <summary>策略阈值</summary>
    [JsonPropertyName("policy_threshold")]
    public double? PolicyThreshold { get; set; }

    /// <summary>环境变量</summary>
    [JsonPropertyName("env_params")]
    public string? EnvParams { get; set; }

    /// <summary>创建时间</summary>
    [JsonPropertyName("created_time")]
    public string? CreatedTime { get; set; }

    /// <summary>更新时间</summary>
    [JsonPropertyName("updated_time")]
    public string? UpdatedTime { get; set; }

    /// <summary>IP</summary>
    [JsonPropertyName("version_ip")]
    public string? VersionIp { get; set; }

    /// <summary>端口</summary>
    [JsonPropertyName("version_port")]
    public long? VersionPort { get; set; }

    /// <summary>状态</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>代码包名称</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>代码包版本</summary>
    [JsonPropertyName("package_version")]
    public string? PackageVersion { get; set; }

    /// <summary>上传类型</summary>
    [JsonPropertyName("upload_type")]
    public string? UploadType { get; set; }

    /// <summary>仓库类型</summary>
    [JsonPropertyName("repo_type")]
    public string? RepoType { get; set; }

    /// <summary>仓库</summary>
    [JsonPropertyName("repo")]
    public string? Repo { get; set; }

    /// <summary>分支</summary>
    [JsonPropertyName("branch")]
    public string? Branch { get; set; }

    /// <summary>日志</summary>
    [JsonPropertyName("custom_logs")]
    public string? CustomLogs { get; set; }

    /// <summary>镜像地址</summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>容器端口</summary>
    [JsonPropertyName("container_port")]
    public long? ContainerPort { get; set; }
}
