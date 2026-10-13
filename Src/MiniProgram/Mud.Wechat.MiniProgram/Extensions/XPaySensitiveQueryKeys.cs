// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Extensions;

/// <summary>
/// 虚拟支付线<b>词表外</b>凭据 Query 参数名登记表（守卫 XP6 的落地入口）。
/// </summary>
/// <remarks>
/// <para>
/// 组件的 URL 脱敏（<c>SensitiveUrlRedactor</c>）是<b>精确匹配词表</b>语义：不做前缀/后缀模糊匹配，
/// 所以词表里的 <c>sig</c> / <c>sign</c> / <c>signature</c> 三支<b>覆盖不到</b> <c>pay_sig</c>。
/// 而 <c>pay_sig</c> 是官方虚拟支付 28 个端点逐请求强制传入的<b>支付签名</b>（官方调用示例逐字
/// <c>?access_token=ACCESS_TOKEN&amp;pay_sig=PAY_SIG</c>，2026-10-13 逐页核验），一旦出现在
/// <c>ApiException.RequestUri</c>、日志与遥测 URL 中即为凭据明文外泄。
/// </para>
/// <para>
/// <b>处置方式取组件的公开登记门面</b> <c>Mud.HttpUtils.SensitiveUrlKeys.RegisterAll</c>
/// （登记后的键<b>无论可观测性开关为何都强制掩码</b>），与企微线群机器人 <c>key</c>（守卫 WEB3）、
/// 广告线 <c>user_token</c>（守卫 ADS-B5）的既有做法同形。<b>不</b>去改组件静态词表：那是全局面，
/// 且本仓无权替上游决定通用键名。AGENTS §8 的「补齐词表 / 登记豁免」二选一在此取<b>登记</b>一支。
/// </para>
/// <para>
/// <b>本域的 <c>signature</c>（用户态签名）不在本表</b>：它已在组件静态词表内（精确匹配 <c>signature</c>），
/// 重复登记不会出错，但会让「哪些键靠词表、哪些靠登记」这一分界失去可审计性 ——
/// 守卫 XP6 同时断言本表内容与词表<b>不交叉</b>。
/// </para>
/// <para>
/// <b>登记时机</b>：由 <c>MiniProgramServiceBuilder</c> 的 XPay 模块注册委托调用，<b>只在使用虚拟支付线时</b>
/// 产生进程级副作用（比在 <c>Build()</c> 里无条件登记更收敛）。
/// </para>
/// </remarks>
public static class XPaySensitiveQueryKeys
{
    /// <summary>支付签名参数名（官方 28 端点逐请求传入，词表未覆盖）。</summary>
    public const string PaySig = "pay_sig";

    /// <summary>本线需要登记的全部词表外凭据参数名（新增 Query 凭据时必须同批扩充，守卫 XP6 锁定）。</summary>
    public static string[] All { get; } = { PaySig };

    /// <summary>把 <see cref="All"/> 逐项登记为进程级强制掩码键（幂等；由 XPay 模块注册委托调用）。</summary>
    internal static void RegisterAll()
    {
        // 不在静态构造里跑：静态构造执行时机不可控，
        // 「未注册任何模块就抛异常」的路径不应产生进程级全局副作用。
        SensitiveUrlKeys.RegisterAll(All);
    }
}
