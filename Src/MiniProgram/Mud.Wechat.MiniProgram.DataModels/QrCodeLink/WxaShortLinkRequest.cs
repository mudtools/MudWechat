// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>生成 ShortLink 请求体（<c>POST /wxa/genwxashortlink</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaShortLinkRequest
{
    /// <summary>
    /// 小程序页面路径（<c>page_url</c>，必填）：官方要求该页面<b>已配置 <c>sitemap.json</c></b>；
    /// <b>不支持 <c>query</c> 参数</b>（与 URL Link / Scheme 的本质差异）。
    /// </summary>
    [JsonPropertyName("page_url")]
    public string? PageUrl { get; set; }

    /// <summary>短链标题（<c>page_title</c>，选填）。</summary>
    [JsonPropertyName("page_title")]
    public string? PageTitle { get; set; }

    /// <summary>是否永久有效（<c>is_permanent</c>，选填；<c>true</c> 时忽略失效参数）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）；取值语义以官方页面为准。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，选填）。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效绝对时间（<c>expire_time</c>，选填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}
