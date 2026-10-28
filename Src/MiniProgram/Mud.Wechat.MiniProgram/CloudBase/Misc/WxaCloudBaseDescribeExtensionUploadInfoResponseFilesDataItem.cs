// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.CloudBase;

/// <summary>描述扩展上传文件信息·FilesData(Array)。官方页：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cloudbase/others/api_describeextensionuploadinfo.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "CloudBase")]
public class WxaCloudBaseDescribeExtensionUploadInfoResponseFilesDataItem
{
    /// <summary>模板里使用的地址</summary>
    [JsonPropertyName("CodeUri")]
    public string? CodeUri { get; set; }

    /// <summary>上传文件的临时地址，含签名</summary>
    [JsonPropertyName("UploadUrl")]
    public string? UploadUrl { get; set; }

    /// <summary>自定义密钥。如果为空，则表示不需要加密</summary>
    [JsonPropertyName("CustomKey")]
    public string? CustomKey { get; set; }

    /// <summary>文件大小限制，单位M，客户端上传前需要主动检查文件大小，超过限制的文件会被删除。</summary>
    [JsonPropertyName("MaxSize")]
    public long? MaxSize { get; set; }
}
