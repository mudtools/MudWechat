// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「修改业务域名 / webview 域名」请求（<c>setwebviewdomain</c> / <c>setwebviewdomain_directly</c> 共用）：以 <c>action</c> 分派 add/delete/set/get，最多 300 域名。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/domain-management/modifyJumpDomain.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpWebViewDomainRequest
{
    /// <summary>操作类型（官方 <c>action</c>，取值 <c>add</c>/<c>delete</c>/<c>set</c>/<c>get</c>）。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>webview 域名列表（官方 <c>webviewdomain</c>，array）。</summary>
    [JsonPropertyName("webviewdomain")]
    public List<string>? WebViewDomain { get; set; }
}
