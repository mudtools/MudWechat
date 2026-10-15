// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>创建云函数请求。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_batchuuloadcloudfunction.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbBatchUploadCloudFunctionRequest
{
    /// <summary>函数名</summary>
    [JsonPropertyName("functionname")]
    public string? Functionname { get; set; }

    /// <summary>环境ID</summary>
    [JsonPropertyName("envs")]
    public List<string>? Envs { get; set; }

    /// <summary>包含函数代码文件的 zip 格式文件，使用该接口时要求将 zip 文件的内容转成 base64 编码，最大支持20M （云开发自动上传云端依赖，无需手动上传）</summary>
    [JsonPropertyName("zipfile")]
    public string? Zipfile { get; set; }

    /// <summary>VPC唯一标识</summary>
    [JsonPropertyName("vpcid")]
    public string? Vpcid { get; set; }

    /// <summary>子网id</summary>
    [JsonPropertyName("subnetid")]
    public string? Subnetid { get; set; }
}
