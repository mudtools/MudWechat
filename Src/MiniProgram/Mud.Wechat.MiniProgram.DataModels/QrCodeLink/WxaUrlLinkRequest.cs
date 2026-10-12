// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>生成 URL Link 请求体（<c>POST /wxa/generate_urllink</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaUrlLinkRequest
{
    /// <summary>小程序页面路径（<c>path</c>，必填；不可带 <c>.html</c> 后缀，可带 query）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>页面参数（<c>query</c>，选填；<b>须已 URL 编码</b>，官方原文）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）；取值语义以官方页面为准，SDK 不做本地校验。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，选填）；单位与上限随 <c>expire_type</c>，以官方页面为准。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效绝对时间（<c>expire_time</c>，选填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>云开发配置（<c>cloud_base</c>，选填），见 <see cref="WxaCloudBase"/>。</summary>
    [JsonPropertyName("cloud_base")]
    public WxaCloudBase? CloudBase { get; set; }
}
