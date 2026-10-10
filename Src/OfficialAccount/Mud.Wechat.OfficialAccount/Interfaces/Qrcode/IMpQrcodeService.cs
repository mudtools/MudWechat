// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「服务号二维码 → 带参二维码」域 SDK（1 端点，<b>服务号专属</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务号域 <see href="https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodes/api_createqrcode.html"/>
/// （2026-10-07 逐页核验：subscription 订阅号域 404，服务号域命中；官方适用范围为
/// 「服务号（仅认证）」散文形态——与其他域的双列表格写法不同，照录）。
/// </para>
/// <para>
/// <b>扫码事件闭环</b>：官方原文「如果用户还未关注公众号，则用户可以关注公众号，关注后微信会将
/// 带场景值关注事件（<see cref="Abstractions.Callback.MpCallbackEventTypes.Subscribe"/>，携带
/// EventKey/Ticket）推送给开发者。如果用户已经关注公众号……微信会将带场景值扫描事件
/// （<see cref="Abstractions.Callback.MpCallbackEventTypes.Scan"/>）推送给开发者」。
/// </para>
/// <para>
/// <b>令牌路由</b>：消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入 <c>access_token</c>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Qrcode", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpQrcodeService
{
    /// <summary>
    /// 生成带参数的二维码（创建二维码 ticket）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodes/api_createqrcode.html"/>
    /// （官方接口英文名 <c>createQRCode</c>）。
    /// </summary>
    /// <param name="request">创建请求（action_name 四形态 + scene_id/scene_str 按形态携带其一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码 ticket（<c>ticket</c>/<c>expire_seconds</c>/<c>url</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpQrcodeCreateRequest"/>；
    /// 临时二维码（QR_SCENE/QR_STR_SCENE）须设置 expire_seconds（≤2592000 秒）；
    /// 永久二维码（QR_LIMIT_SCENE/QR_LIMIT_STR_SCENE）无过期时间、数量上限 10 万个。
    /// </para>
    /// <para>
    /// <b>换图（官方原文，SDK 不代下载）</b>：ticket 换图走
    /// <c>GET https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=TICKET</c>（TICKET 须 UrlEncode；
    /// 无须登录态；正确时返回图片，错误时 HTTP 404）——详见 <see cref="MpQrcodeCreateResponse"/> remarks。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40052</c>（invalid action name，action 值有误）/
    /// <c>40053</c>（invalid action info，官方解决方案列留空，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/qrcode/create")]
    Task<MpQrcodeCreateResponse> CreateQrcodeAsync(
        [Body] MpQrcodeCreateRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>二维码类型（官方 <c>action_name</c> 四形态）。</summary>
public static class MpQrcodeActionNames
{
    /// <summary>临时整型（须设 expire_seconds；scene_id 为 32 位非 0 整型）。</summary>
    public const string QrScene = "QR_SCENE";

    /// <summary>临时字符串（须设 expire_seconds；scene_str 长度 1~64）。</summary>
    public const string QrStrScene = "QR_STR_SCENE";

    /// <summary>永久整型（scene_id 1~100000；数量上限 10 万个）。</summary>
    public const string QrLimitScene = "QR_LIMIT_SCENE";

    /// <summary>永久字符串（scene_str 长度 1~64；数量上限 10 万个）。</summary>
    public const string QrLimitStrScene = "QR_LIMIT_STR_SCENE";
}
