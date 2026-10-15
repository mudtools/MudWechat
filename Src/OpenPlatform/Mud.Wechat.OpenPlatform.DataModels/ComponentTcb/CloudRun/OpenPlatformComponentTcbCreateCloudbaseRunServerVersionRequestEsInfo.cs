// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>创建服务版本·es_info。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseserviceversion.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequestEsInfo
{
    /// <summary>es的id</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>secret名字</summary>
    [JsonPropertyName("secret_name")]
    public string? SecretName { get; set; }

    /// <summary>ip地址</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>端口</summary>
    [JsonPropertyName("port")]
    public long? Port { get; set; }

    /// <summary>索引</summary>
    [JsonPropertyName("index")]
    public string? Index { get; set; }

    /// <summary>用户名</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>密码</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
