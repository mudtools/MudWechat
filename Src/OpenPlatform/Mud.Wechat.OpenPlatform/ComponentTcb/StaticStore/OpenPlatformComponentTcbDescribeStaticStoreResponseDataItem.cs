// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

/// <summary>查看静态网站状态·data(Array)。官方页：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_getstaticstore.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "ComponentTcb")]
public class OpenPlatformComponentTcbDescribeStaticStoreResponseDataItem
{
    /// <summary>环境名称</summary>
    [JsonPropertyName("env")]
    public string? Env { get; set; }

    /// <summary>静态域名</summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>COS桶</summary>
    [JsonPropertyName("bucket")]
    public string? Bucket { get; set; }

    /// <summary>所在区域</summary>
    [JsonPropertyName("regoin")]
    public string? Regoin { get; set; }

    /// <summary>状态</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
