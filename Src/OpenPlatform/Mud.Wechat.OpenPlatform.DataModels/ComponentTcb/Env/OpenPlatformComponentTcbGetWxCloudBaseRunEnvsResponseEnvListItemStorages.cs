// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>获取云托管环境信息·env_list(Array).storages。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/cloudenv-mgnt/api_getwxcloudbaserunenvs.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemStorages
{
    /// <summary>所属地域。当前支持ap-shanghai</summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>桶名，存储资源的唯一标识</summary>
    [JsonPropertyName("bucket")]
    public string? Bucket { get; set; }

    /// <summary>cdn 域名</summary>
    [JsonPropertyName("cdn_domain")]
    public string? CdnDomain { get; set; }

    /// <summary>资源所属用户的腾讯云appId</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }
}
