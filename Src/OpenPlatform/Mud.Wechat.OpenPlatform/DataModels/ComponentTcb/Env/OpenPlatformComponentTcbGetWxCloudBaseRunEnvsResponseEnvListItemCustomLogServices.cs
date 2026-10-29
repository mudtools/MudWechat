// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>获取云托管环境信息·env_list(Array).custom_log_services。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/cloudenv-mgnt/api_getwxcloudbaserunenvs.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponseEnvListItemCustomLogServices
{
    /// <summary>cls所属地域</summary>
    [JsonPropertyName("cls_region")]
    public string? ClsRegion { get; set; }

    /// <summary>cls日志集ID</summary>
    [JsonPropertyName("cls_logset_id")]
    public string? ClsLogsetId { get; set; }

    /// <summary>cls日志主题ID</summary>
    [JsonPropertyName("cls_topic_id")]
    public string? ClsTopicId { get; set; }

    /// <summary>创建时间</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}
