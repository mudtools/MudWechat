// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>滚动更新服务版本·mount_volume_info(Array).nfs_volumes。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_updatecloudbaseserviceversion.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequestMountVolumeInfoItemNfsVolumes
{
    /// <summary>NFS挂载Server</summary>
    [JsonPropertyName("server")]
    public string? Server { get; set; }

    /// <summary>Server路径</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>是否只读</summary>
    [JsonPropertyName("read_only")]
    public bool? ReadOnly { get; set; }

    /// <summary>secret名称</summary>
    [JsonPropertyName("secret_name")]
    public string? SecretName { get; set; }

    /// <summary>临时目录</summary>
    [JsonPropertyName("enable_empty_dir_volume")]
    public bool? EnableEmptyDirVolume { get; set; }
}
