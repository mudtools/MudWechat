// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「修改服务器域名」请求（<c>modify_domain</c> / <c>modify_domain_directly</c> 共用）：以 <c>action</c> 分派 add/delete/set/get，六类域名数组各最多 200，每月修改 50 次。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/modifyServerDomain.html"/>。
/// </summary>
/// <remarks><c>action=get</c> 时省略六类域名数组；<c>set</c> 为覆盖语义、<c>add</c> 为追加。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpModifyDomainRequest
{
    /// <summary>操作类型（官方必填 <c>action</c>，取值 <c>add</c>/<c>delete</c>/<c>set</c>/<c>get</c>）。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>request 合法域名列表（官方 <c>requestdomain</c>，array）。</summary>
    [JsonPropertyName("requestdomain")]
    public List<string>? RequestDomain { get; set; }

    /// <summary>socket 合法域名列表（官方 <c>wsrequestdomain</c>，array）。</summary>
    [JsonPropertyName("wsrequestdomain")]
    public List<string>? WsRequestDomain { get; set; }

    /// <summary>上传文件域名列表（官方 <c>uploaddomain</c>，array）。</summary>
    [JsonPropertyName("uploaddomain")]
    public List<string>? UploadDomain { get; set; }

    /// <summary>下载文件域名列表（官方 <c>downloaddomain</c>，array）。</summary>
    [JsonPropertyName("downloaddomain")]
    public List<string>? DownloadDomain { get; set; }

    /// <summary>udp 合法域名列表（官方 <c>udpdomain</c>，array）。</summary>
    [JsonPropertyName("udpdomain")]
    public List<string>? UdpDomain { get; set; }

    /// <summary>tcp 合法域名列表（官方 <c>tcpdomain</c>，array）。</summary>
    [JsonPropertyName("tcpdomain")]
    public List<string>? TcpDomain { get; set; }
}
