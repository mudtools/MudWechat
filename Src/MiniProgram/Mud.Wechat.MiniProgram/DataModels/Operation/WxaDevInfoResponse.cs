// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>查询域名配置应答（<c>POST /wxa/getwxadevinfo</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getdomaininfo.html</c>；返回四类网络请求域名白名单配置。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaDevInfoResponse : WxaResponse
{
    /// <summary>request 合法域名配置（<c>request_domain</c>），见 <see cref="WxaDomainGroup"/>。</summary>
    [JsonPropertyName("request_domain")]
    public WxaDomainGroup? RequestDomain { get; set; }
}
