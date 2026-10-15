// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>查看云托管版本列表·items(Array)。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcsversionlist.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbGetContainerServiceVersionListResponseItemsItem
{
    /// <summary>版本名</summary>
    [JsonPropertyName("version_name")]
    public string? VersionName { get; set; }

    /// <summary>状态</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>创建时间</summary>
    [JsonPropertyName("created_time")]
    public string? CreatedTime { get; set; }

    /// <summary>流量占比</summary>
    [JsonPropertyName("flow_ratio")]
    public double? FlowRatio { get; set; }

    /// <summary>构建id</summary>
    [JsonPropertyName("buildid")]
    public long? Buildid { get; set; }

    /// <summary>上传类型</summary>
    [JsonPropertyName("upload_type")]
    public string? UploadType { get; set; }

    /// <summary>最小副本数</summary>
    [JsonPropertyName("min_replicas")]
    public long? MinReplicas { get; set; }

    /// <summary>最大副本数</summary>
    [JsonPropertyName("max_replicas")]
    public long? MaxReplicas { get; set; }
}
