// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律责任纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「小程序码 / 二维码」图片通道（3 端点）—— <b>独立请求形态</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不是 <c>[HttpClientApi]</c> 接口</b>：三端点<b>成功时返回图片二进制流</b>
/// （<c>Content-Type: image/*</c>），<b>失败时才返回 JSON</b>（<c>{"errcode":…}</c>）。
/// 声明式管线按 JSON 反序列化，会把图片字节交给 JSON 解析器、或把错误体当图片交出 ——
/// 两种都是静默错误。故本通道走 <c>IBaseHttpClient.SendRawAsync</c> 原始响应直读 +
/// <b>Content-Type 分支判错</b>（与公众号线 <c>IMpMediaDownloadService</c> 同款先例）。
/// </para>
/// <para>
/// <b>令牌</b>：非 <c>[Token]</c> 声明式注入，而是<b>手工取当前应用令牌</b>并拼到 Query
/// （<c>access_token</c>，官方契约）——因此本接口<b>不进</b> Query 令牌白名单（白名单只收
/// <c>[Token]</c> 声明式接口，与公众号线 <c>IMpMediaDownloadService</c> 同款处置）。
/// </para>
/// <para>
/// <b>令牌自愈</b>：命中失效码（<c>40001</c> / <c>40014</c> / <c>42001</c>）时失效本应用令牌并
/// <b>重试一次</b>，重试仍失败按业务异常上抛（<see cref="Abstractions.WxaException"/>）。
/// </para>
/// </remarks>
public interface IWxaCodeService
{
    /// <summary>
    /// 获取<b>不限量</b>小程序码（<c>POST /wxa/getwxacodeunlimit</c>）。
    /// 官方文档：<c>qrcode-link/qr-code/api_getunlimitedqrcode.html</c>。
    /// </summary>
    /// <param name="request">生成请求（<c>scene</c> 必填，≤32 可见字符），见 <see cref="DataModels.QrCodeLink.WxaCodeUnlimitRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片内容流，见 <see cref="WxaCodeResult"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；成功响应为图片二进制。</para>
    /// <para><b>总量不限</b>（与限量码的本质差异），但 <c>scene</c> <b>最长 32 个可见字符</b>。</para>
    /// <para>官方错误码：<c>40001</c>（令牌，走自愈）/ <c>41030</c>（<c>page</c> 不存在或未发布）/ <c>45009</c>（调用量超限）。</para>
    /// </remarks>
    Task<WxaCodeResult> GetUnlimitedCodeAsync(
        DataModels.QrCodeLink.WxaCodeUnlimitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取<b>限量</b>小程序码（<c>POST /wxa/getwxacode</c>）。
    /// 官方文档：<c>qrcode-link/qr-code/api_getqrcode.html</c>。
    /// </summary>
    /// <param name="request">生成请求（<c>path</c> 必填），见 <see cref="DataModels.QrCodeLink.WxaCodeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片内容流，见 <see cref="WxaCodeResult"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；成功响应为图片二进制。</para>
    /// <para><b>总量上限 10 万个</b>（官方原文）；<c>path</c> 不得带参数（带参数请用不限量码的 <c>scene</c>）。</para>
    /// </remarks>
    Task<WxaCodeResult> GetCodeAsync(
        DataModels.QrCodeLink.WxaCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建<b>二维码 C</b>（<c>POST /cgi-bin/wxaapp/createwxaqrcode</c>）。
    /// 官方文档：<c>qrcode-link/qr-code/api_createqrcode.html</c>。
    /// </summary>
    /// <param name="request">生成请求（<c>path</c> 必填），见 <see cref="DataModels.QrCodeLink.WxaQrCodeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片内容流，见 <see cref="WxaCodeResult"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；成功响应为图片二进制。</para>
    /// <para>生成的是<b>二维码</b>而非小程序码，样式不可定制；同样受<b>总量 10 万个</b>限制。</para>
    /// <para><b>路由前缀跨包</b>：<c>/cgi-bin/wxaapp/</c> 是本线唯一的 <c>/cgi-bin/</c> 前缀端点
    /// （其余在 <c>/wxa/</c>）；与公众号线 <c>/cgi-bin/*</c> 端点<b>无重叠</b>（MP-X1 断言）。</para>
    /// </remarks>
    Task<WxaCodeResult> CreateQrCodeAsync(
        DataModels.QrCodeLink.WxaQrCodeRequest request,
        CancellationToken cancellationToken = default);
}
