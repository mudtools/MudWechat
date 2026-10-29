// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 校验服务授权跳转码请求体（<c>POST /product/service/check_auth</c>）。
/// </summary>
/// <remarks>
/// <para><c>code</c> 由小商店服务市场跳转到第三方 URL 时携带，过期或不存在返回错误码 1。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductServiceCheckAuthRequest
{
    /// <summary>跳转码（必填）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
