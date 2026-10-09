// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 「当前商户」环境上下文 —— 决定<b>这一笔支付请求用哪个商户的私钥签名</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要环境态</b>：APIv3 的 <c>mchid</c>/<c>sp_mchid</c>/<c>sub_mchid</c> 位于<b>请求体</b>，
/// 而请求签名串<b>含请求体原文</b> ⇒ 传输层无法「先反读请求体、再决定用哪把私钥」（读写顺序与签名计算冲突）。
/// 商户必须在请求发出<b>之前</b>就已确定，因此采用环境态而非请求期推断。
/// </para>
/// <para>
/// <b>默认商户语义</b>（对齐企微 <c>DefaultAppKey</c>）：仅注册一个商户时，它<b>就是</b>默认，
/// 单商户宿主零样板 —— 直接 <c>ITransactionsService.CreateOrderAsync(...)</c> 即可。
/// 注册多个商户且未开作用域时 <b>fail-fast</b>（不静默挑第一个，否则多商户宿主会拿错私钥签名）。
/// </para>
/// <para>
/// 作用域形态对齐本仓既有的 <c>IWechatAppContextSwitcher.UseCorpScope(...)</c>：一次性
/// <c>using</c>、可嵌套、释放时<b>还原上一层</b>（而非无条件清空）且幂等。
/// </para>
/// </remarks>
public interface IWechatPayMerchantContext
{
    /// <summary>
    /// 解析当前应当使用的商户配置。
    /// </summary>
    /// <returns>当前商户配置。</returns>
    /// <exception cref="InvalidOperationException">
    /// 未注册任何商户；或已注册多个商户且当前未处于 <see cref="UseMerchant(string)"/> 作用域内（fail-fast，不静默取默认）。
    /// </exception>
    WechatPayMerchantConfig ResolveCurrent();

    /// <summary>
    /// 开启「当前商户」作用域，返回的对象释放时还原上一层商户。
    /// </summary>
    /// <param name="merchantKey">商户键（普通商户为 <c>mchid</c>，服务商为 <c>{sp_mchid}:{sub_mchid}</c>）。</param>
    /// <returns>一次性作用域（<c>using</c> 包裹调用）。</returns>
    /// <exception cref="ArgumentException"><paramref name="merchantKey"/> 为 null 或空白。</exception>
    /// <exception cref="InvalidOperationException">未注册该商户。</exception>
    IDisposable UseMerchant(string merchantKey);
}
