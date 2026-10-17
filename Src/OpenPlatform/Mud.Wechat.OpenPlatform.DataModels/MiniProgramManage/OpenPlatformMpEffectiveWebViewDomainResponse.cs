// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「获取有效 webview 域名」应答（<c>get_effective_webviewdomain</c>，官方为 POST、无请求体）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/getEffectiveJumpDomain.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpEffectiveWebViewDomainResponse : OpenPlatformResponse
{
    /// <summary>公众平台后台配置的 webview 域名（官方 <c>mp_webviewdomain</c>，array）。</summary>
    [JsonPropertyName("mp_webviewdomain")]
    public List<string>? MpWebViewDomain { get; set; }

    /// <summary>第三方接口配置的 webview 域名（官方 <c>third_webviewdomain</c>，array）。</summary>
    [JsonPropertyName("third_webviewdomain")]
    public List<string>? ThirdWebViewDomain { get; set; }

    /// <summary><c>setwebviewdomain_directly</c> 配置的 webview 域名（官方 <c>direct_webviewdomain</c>，array）。</summary>
    [JsonPropertyName("direct_webviewdomain")]
    public List<string>? DirectWebViewDomain { get; set; }

    /// <summary>实际生效的 webview 域名（官方 <c>effective_webviewdomain</c>，array）。</summary>
    [JsonPropertyName("effective_webviewdomain")]
    public List<string>? EffectiveWebViewDomain { get; set; }
}
