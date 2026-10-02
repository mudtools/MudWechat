// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 企业微信 IP 信息（<c>/cgi-bin/security/get_server_domain_ip</c> 响应 ip_list 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class ServerIpInfo
{
    /// <summary>
    /// 获取或设置 IP 地址。
    /// </summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>
    /// 获取或设置协议（如 TCP、UDP）。
    /// </summary>
    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }

    /// <summary>
    /// 获取或设置端口号列表。
    /// </summary>
    [JsonPropertyName("port")]
    public List<int>? Port { get; set; }

    /// <summary>
    /// 获取或设置是否必要：0-否 1-是（必要的域名或 IP 被拦截会导致功能异常）。
    /// </summary>
    [JsonPropertyName("is_necessary")]
    public int? IsNecessary { get; set; }

    /// <summary>
    /// 获取或设置 IP 涉及功能的描述。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
