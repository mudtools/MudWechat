// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Basic;

/// <summary>
/// 获取微信推送服务器 IP 响应（<c>GET /cgi-bin/getcallbackip</c>）。
/// </summary>
/// <remarks>
/// 响应形态与约束与 <see cref="MpGetApiDomainIpResponse"/> 一致（<c>ip_list</c> 字符串数组；
/// 失败时 <c>errcode</c> / <c>errmsg</c> 且 <c>ip_list</c> 缺省）。两者刻意分别声明以保持
/// 「端点 ↔ DTO 一对一」的契约可追溯性（守卫额外断言二者结构同构）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class MpGetCallbackIpResponse : MpResponse
{
    /// <summary>微信推送服务器 IP 地址列表（失败时缺省为 <c>null</c>）。</summary>
    [JsonPropertyName("ip_list")]
    public List<string>? IpList { get; set; }
}
