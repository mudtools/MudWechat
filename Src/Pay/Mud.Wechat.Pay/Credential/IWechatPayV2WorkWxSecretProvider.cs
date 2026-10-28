// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Credential;

/// <summary>
/// 企业微信自建应用 <b>Secret</b> 的取用端口（仅服务于 APIv2 的 <c>workwx_sign</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么这是一个端口而不是一个配置属性</b>：<c>workwx_sign</c> 的密钥是
/// 「请求里 <c>agentid</c> 对应的企业微信自建应用 Secret」——它<b>属于企业微信应用</b>，
/// 不属于微信支付商户。若把它挂进 <c>WechatPayMerchantConfig</c>，一是形态错位
/// （一个商户号可对应 N 个 agentid，配置面装不下这个 1:N），二是会让支付线凭空多出一份
/// 企微凭据的落盘面。故按本仓一贯的「配置面只存密钥名、值一律经 <c>ISecretProvider</c> 取用」
/// 收敛成宿主可实现的可选取数端口。
/// </para>
/// <para>
/// <b>与支付线的依赖边界</b>：本端口<b>不引用</b>企业微信程序集（七线零交叉引用）。
/// 宿主若已装配企微线，可写一个薄适配器把 <c>agentid</c> 换成
/// <c>IWechatAppManager</c> 里的应用 Secret；若未装配，也可从自身密钥系统直接给值。
/// </para>
/// <para>
/// <b>未注册即 fail-fast</b>：两支需要 <c>workwx_sign</c> 的写入端点在解析不到本端口
/// （或本端口对该 <c>agentid</c> 返回 <c>null</c>）时直接抛错并点名本端口，<b>不</b>降级为
/// 「只带 <c>sign</c> 发出去」——官方对这两支是强制双签，单签报文只会换回 SIGNERROR，
/// 且把配置缺失伪装成签名错误。
/// </para>
/// </remarks>
public interface IWechatPayV2WorkWxSecretProvider
{
    /// <summary>取指定企业微信应用的 Secret。</summary>
    /// <param name="agentId">
    /// 企业微信自建应用 agentid；请求未带 <c>agentid</c> 时传 <see langword="null"/>，
    /// 由实现方决定其默认应用（沿用「企业支付」系统应用 Secret 的历史商户即在此返回该 Secret）。
    /// </param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>Secret 值；返回 <see langword="null"/> 视为「该 agentid 无可用凭据」，调用方 fail-fast。</returns>
    Task<string?> GetSecretAsync(string? agentId, CancellationToken cancellationToken);
}
