// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.Midas;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小游戏·米大师虚拟支付域（8 端点：正式环境 4 + 沙箱环境 4；
/// 路由族 <c>/cgi-bin/midas/*</c> 与 <c>/cgi-bin/midas/sandbox/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只承载 HTTPS 调用面</b>：官方每页同时给出 HTTPS 调用与云调用（<c>openapi.midas.*</c>）两种方式，
/// SDK 只实现服务端 HTTPS 调用；云调用属微信云开发运行时，不在本包范围。
/// </para>
/// <para>
/// <b>沙箱是独立环境</b>：<c>/cgi-bin/midas/sandbox/*</c> 与正式环境同契约但数据隔离；
/// 沙箱仅用于联调，不得把沙箱 <c>bill_no</c> 与正式环境混用。
/// </para>
/// <para>
/// <b>签名不代算</b>：官方要求 <c>sig</c> 用米大师密钥对「全部参数（含可选参数）+ uri + 密钥」
/// 做 HMAC-SHA256；SDK 不持有米大师密钥，<c>sig</c> 由调用方计算后透传。
/// </para>
/// <para>
/// <b>官方文档侧栏另有 <c>pay_v2.*</c> 新一代虚拟支付</b>（getBalance / pay / cancelPay / present /
/// queryOrder）：本接口以本次取证的路由清单为准，只承载 legacy <c>midas.*</c> 八个路由，
/// 不混入 v2 契约（两者字段与签名面不同）。
/// </para>
/// <para>
/// <b>敏感面</b>：<c>openid</c> / <c>sig</c> / <c>bill_no</c> / <c>user_ip</c> 一律禁入日志、
/// 遥测与异常消息。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Midas", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaMidasService
{
    /// <summary>
    /// 扣除游戏币（<c>POST /cgi-bin/midas/pay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.pay.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasPayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预扣后余额与赠送币扣减额，见 <see cref="WxaMidasPayResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>系统失败可用同 <c>bill_no</c> 重试</b>：官方明示超时或系统失败时游戏币可能已实际扣除，
    /// 用相同 <c>bill_no</c> 重试不会重复扣款；也可改用取消支付接口撤销。
    /// </para>
    /// <para><b>非幂等语义依赖 <c>bill_no</c></b>：更换订单号即视为新交易。</para>
    /// </remarks>
    [Post("/cgi-bin/midas/pay")]
    Task<WxaMidasPayResponse> PayAsync(
        [Body] WxaMidasPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消扣除游戏币订单（<c>POST /cgi-bin/midas/cancelpay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.cancelPay.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasCancelPayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>被取消的订单号，见 <see cref="WxaMidasCancelPayResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>可取消窗口</b>：仅订单号仍在有效期内可取消；90014 已支付确认完成 / 90015 已回退 /
    /// 90016 处理中均表示当前不可取消。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/midas/cancelpay")]
    Task<WxaMidasCancelPayResponse> CancelPayAsync(
        [Body] WxaMidasCancelPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取游戏币余额（<c>POST /cgi-bin/midas/getbalance</c>，查询类端点官方仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.getBalance.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasGetBalanceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>余额、赠送币与累计流水，见 <see cref="WxaMidasGetBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>只读</b>：余额包含赠送币（<c>balance</c>），赠送部分另由 <c>gen_balance</c> 单列。</para>
    /// </remarks>
    [Post("/cgi-bin/midas/getbalance")]
    Task<WxaMidasGetBalanceResponse> GetBalanceAsync(
        [Body] WxaMidasGetBalanceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 赠送游戏币（<c>POST /cgi-bin/midas/present</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.present.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasPresentRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>赠送后余额与订单号，见 <see cref="WxaMidasPresentResponse"/>。</returns>
    /// <remarks>
    /// <para><b>幂等键是 <c>bill_no</c></b>：相同订单号不重复赠送，90012 即订单已存在。</para>
    /// </remarks>
    [Post("/cgi-bin/midas/present")]
    Task<WxaMidasPresentResponse> PresentAsync(
        [Body] WxaMidasPresentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 沙箱·扣除游戏币（<c>POST /cgi-bin/midas/sandbox/pay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.pay.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasPayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>沙箱环境的预扣结果，见 <see cref="WxaMidasPayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>数据隔离</b>：沙箱与正式环境分离，联调通过后须切正式路由重新对账。</para>
    /// </remarks>
    [Post("/cgi-bin/midas/sandbox/pay")]
    Task<WxaMidasPayResponse> PayInSandboxAsync(
        [Body] WxaMidasPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 沙箱·取消扣除游戏币订单（<c>POST /cgi-bin/midas/sandbox/cancelpay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.cancelPay.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasCancelPayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>沙箱环境的取消结果，见 <see cref="WxaMidasCancelPayResponse"/>。</returns>
    [Post("/cgi-bin/midas/sandbox/cancelpay")]
    Task<WxaMidasCancelPayResponse> CancelPayInSandboxAsync(
        [Body] WxaMidasCancelPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 沙箱·获取游戏币余额（<c>POST /cgi-bin/midas/sandbox/getbalance</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.getBalance.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasGetBalanceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>沙箱环境的余额，见 <see cref="WxaMidasGetBalanceResponse"/>。</returns>
    [Post("/cgi-bin/midas/sandbox/getbalance")]
    Task<WxaMidasGetBalanceResponse> GetBalanceInSandboxAsync(
        [Body] WxaMidasGetBalanceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 沙箱·赠送游戏币（<c>POST /cgi-bin/midas/sandbox/present</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/midas.present.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaMidasPresentRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>沙箱环境的赠送结果，见 <see cref="WxaMidasPresentResponse"/>。</returns>
    [Post("/cgi-bin/midas/sandbox/present")]
    Task<WxaMidasPresentResponse> PresentInSandboxAsync(
        [Body] WxaMidasPresentRequest request,
        CancellationToken cancellationToken = default);
}
