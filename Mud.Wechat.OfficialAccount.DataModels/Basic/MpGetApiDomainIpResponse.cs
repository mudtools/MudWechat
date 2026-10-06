// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Basic;

/// <summary>
/// 获取微信 API 服务器 IP 响应（<c>GET /cgi-bin/get_api_domain_ip</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方建议<b>每天请求 1 次</b>以更新 IP 列表（出口 / 入口 IP 可能变动，跨运营商高峰可能丢包）。
/// </para>
/// <para>
/// <b>失败形态（v2 依据官方错误响应示例修正）</b>：失败时官方返回 <c>errcode</c> / <c>errmsg</c>，
/// <c>ip_list</c> <b>整字段缺省</b>（并非空数组）⇒ <see cref="IpList"/> 为可空集合，
/// 调用方不得假定「非 null 即成功」，应优先经 <c>MpException.ThrowIfFailed</c> 判错。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class MpGetApiDomainIpResponse : MpResponse
{
    /// <summary>微信服务器 IP 地址列表（失败时缺省为 <c>null</c>）。</summary>
    [JsonPropertyName("ip_list")]
    public List<string>? IpList { get; set; }
}
