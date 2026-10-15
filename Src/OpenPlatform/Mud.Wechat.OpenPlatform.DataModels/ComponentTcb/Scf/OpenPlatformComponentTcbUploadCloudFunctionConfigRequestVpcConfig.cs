// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>更新云函数配置·vpc_config。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_uploadcloudfunctionconfig.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbUploadCloudFunctionConfigRequestVpcConfig
{
    /// <summary>vpcid</summary>
    [JsonPropertyName("vpcid")]
    public string? Vpcid { get; set; }

    /// <summary>子网id</summary>
    [JsonPropertyName("subnetid")]
    public string? Subnetid { get; set; }
}
