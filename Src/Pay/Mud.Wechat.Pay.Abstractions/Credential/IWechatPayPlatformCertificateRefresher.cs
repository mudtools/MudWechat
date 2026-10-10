// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 平台证书<b>按需刷新</b>端口：验签遇到未知 <c>Wechatpay-Serial</c> 时补齐证书
/// （设计方案 §2.6 闸① —— 「未知 serial ⇒ 触发一次刷新后仍失败即拒」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要它</b>：官方在证书到期前会<b>并行</b>下发新旧两本平台证书，且新版证书可能在
/// 宿主尚未拉取时就出现在应答 / 回调头里。若没有按需刷新，那段时间的验签会<b>持续失败</b>
/// （fail-closed 是正确的，但可用性受损）——刷新端口把「失败」收敛为「一次自愈机会」。
/// </para>
/// <para>
/// <b>调用时机是本端口定义的一部分</b>：<b>只</b>允许在「序列号未知」时调用，
/// <b>不得</b>在「签名与报文体不匹配」时调用 —— 后者是攻击者可以随意构造的形态，
/// 若据此触发下载，每次伪造请求都会变成一次证书仓库往返（<b>放大型 DoS</b>）。
/// 实现方仍须自带最小刷新间隔与同商户单飞（双保险，见默认实现 <c>WechatPayPlatformCertificateRefresher</c>）。
/// </para>
/// <para>
/// <b>fail-closed 红线</b>（§2.6）：本方法<b>只</b>影响「能否补齐证书」，
/// <b>绝不</b>放宽验签结论 —— 返回 <c>false</c> 时调用方必须<b>照常拒绝</b>；
/// 「刷新失败即放行」是明令禁止的降级。
/// </para>
/// <para>
/// <b>可选注册</b>：未注册本端口时，未知序列号直接拒绝（仍是 fail-closed，只是少了自愈机会）。
/// 回调包经 <c>IServiceProvider</c> 可选解析，故<b>仅装回调</b>的宿主不注册也不会报错。
/// </para>
/// </remarks>
public interface IWechatPayPlatformCertificateRefresher
{
    /// <summary>
    /// 尝试让指定序列号的平台证书在本地可用。
    /// </summary>
    /// <param name="merchant">商户配置（刷新须以该商户的 APIv3 密钥解密证书密文）。</param>
    /// <param name="serialNumber">应答 / 回调头里的 <c>Wechatpay-Serial</c>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>刷新后该序列号是否可用（<c>true</c> 仅表示「证书已就绪」，<b>不</b>表示验签通过）。</returns>
    /// <remarks>
    /// <b>实现要求</b>：① 已命中时不下载（幂等）；② 同一商户并发调用<b>不</b>叠加下载（单飞）；
    /// ③ 最小刷新间隔内的重复请求直接返回 <c>false</c>（防未知序列号风暴）；
    /// ④ 失败<b>不抛</b>（返回 <c>false</c>，由调用方按「未知序列号」拒绝）——但取消令牌触发的
    /// <see cref="OperationCanceledException"/> 除外，它须原样上抛。
    /// </remarks>
    Task<bool> TryRefreshAsync(
        WechatPayMerchantConfig merchant,
        string serialNumber,
        CancellationToken cancellationToken = default);
}
