// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「修改服务器域名」应答（<c>modify_domain</c>）：回显六类有效域名 + 对应六类 <c>invalid_*</c>（被过滤项） + <c>no_icp_domain</c>（缺 ICP 备案被过滤）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/modifyServerDomain.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpModifyDomainResponse : OpenPlatformResponse
{
    /// <summary>有效的 request 域名（官方 <c>requestdomain</c>）。</summary>
    [JsonPropertyName("requestdomain")]
    public List<string>? RequestDomain { get; set; }

    /// <summary>有效的 socket 域名（官方 <c>wsrequestdomain</c>）。</summary>
    [JsonPropertyName("wsrequestdomain")]
    public List<string>? WsRequestDomain { get; set; }

    /// <summary>有效的上传域名（官方 <c>uploaddomain</c>）。</summary>
    [JsonPropertyName("uploaddomain")]
    public List<string>? UploadDomain { get; set; }

    /// <summary>有效的下载域名（官方 <c>downloaddomain</c>）。</summary>
    [JsonPropertyName("downloaddomain")]
    public List<string>? DownloadDomain { get; set; }

    /// <summary>有效的 udp 域名（官方 <c>udpdomain</c>）。</summary>
    [JsonPropertyName("udpdomain")]
    public List<string>? UdpDomain { get; set; }

    /// <summary>有效的 tcp 域名（官方 <c>tcpdomain</c>）。</summary>
    [JsonPropertyName("tcpdomain")]
    public List<string>? TcpDomain { get; set; }

    /// <summary>无效 request 域名（官方 <c>invalid_requestdomain</c>）。</summary>
    [JsonPropertyName("invalid_requestdomain")]
    public List<string>? InvalidRequestDomain { get; set; }

    /// <summary>无效 socket 域名（官方 <c>invalid_wsrequestdomain</c>）。</summary>
    [JsonPropertyName("invalid_wsrequestdomain")]
    public List<string>? InvalidWsRequestDomain { get; set; }

    /// <summary>无效上传域名（官方 <c>invalid_uploaddomain</c>）。</summary>
    [JsonPropertyName("invalid_uploaddomain")]
    public List<string>? InvalidUploadDomain { get; set; }

    /// <summary>无效下载域名（官方 <c>invalid_downloaddomain</c>）。</summary>
    [JsonPropertyName("invalid_downloaddomain")]
    public List<string>? InvalidDownloadDomain { get; set; }

    /// <summary>无效 udp 域名（官方 <c>invalid_udpdomain</c>）。</summary>
    [JsonPropertyName("invalid_udpdomain")]
    public List<string>? InvalidUdpDomain { get; set; }

    /// <summary>无效 tcp 域名（官方 <c>invalid_tcpdomain</c>）。</summary>
    [JsonPropertyName("invalid_tcpdomain")]
    public List<string>? InvalidTcpDomain { get; set; }

    /// <summary>无 ICP 备案被过滤的域名（官方 <c>no_icp_domain</c>）。</summary>
    [JsonPropertyName("no_icp_domain")]
    public List<string>? NoIcpDomain { get; set; }
}
