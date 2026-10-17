// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 开放平台「获取体验版二维码」图片通道（<c>GET /wxa/get_qrcode</c>）—— <b>独立请求形态</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不是 <c>[HttpClientApi]</c> 接口</b>：本端点<b>成功时返回图片二进制流</b>
/// （官方原文：请求正常时开发者可直接把返回的二进制结果保存成图片，响应头 <c>Content-Type: image/jpeg</c> +
/// <c>Content-disposition: attachment; filename="QRCode.jpg"</c>），<b>失败时才返回 JSON</b>（<c>{"errcode":…}</c>）。
/// 声明式管线按 JSON 反序列化，会把图片字节交给 JSON 解析器、或把错误体当图片交出 —— 两种都是静默错误。
/// 故本通道走 <c>SendRawAsync</c> 原始响应直读 + <b>Content-Type 分支判错</b>
/// （与小程序线 <c>IWxaCodeService</c>、公众号线 <c>IMpMediaDownloadService</c> 同款先例）。
/// </para>
/// <para>
/// <b>令牌</b>：非 <c>[Token]</c> 声明式注入，而是从当前授权方作用域取 <c>authorizer_access_token</c> 并拼到 Query
/// （参数名 <c>access_token</c>，官方契约）——因此本接口<b>不进</b>声明式契约面（守卫 OP-B1 的反射枚举不含它，
/// 由 QRC 系列守卫单独锁定）。
/// </para>
/// <para>
/// <b>令牌自愈</b>：命中 <c>40001</c> / <c>40014</c> 时失效本授权方令牌并<b>重试一次</b>，重试仍失败按业务异常上抛。
/// </para>
/// </remarks>
public interface IOpenPlatformMpTrialQrCodeService
{
    /// <summary>
    /// 获取小程序<b>体验版二维码</b>（<c>get_qrcode</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getTrialQRCode.html"/>。
    /// </summary>
    /// <param name="path">
    /// 扫码后直接进入的页面路径（官方 <c>path</c>，可选，<b>传原文</b>如 <c>page/index?action=1</c>）。
    /// 官方注意事项要求「<c>path</c> 需要进行一次 urlencode」⇒ 本通道负责该次编码，调用方<b>不得</b>自行预编码（否则二次编码落不到页面）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片内容流，见 <see cref="OpenPlatformMpTrialQrCodeResult"/>（由调用方释放）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b> <c>/wxa/get_qrcode</c>，Query 携带 <c>access_token</c>（值可用 <c>authorizer_access_token</c>，权限集 18、86）。</para>
    /// <para>官方错误码：<c>-1</c> 系统繁忙 / <c>40001</c>·<c>40014</c> 令牌失效（走自愈重试一次）。</para>
    /// </remarks>
    Task<OpenPlatformMpTrialQrCodeResult> GetTrialQrCodeAsync(
        string? path = null,
        CancellationToken cancellationToken = default);
}
