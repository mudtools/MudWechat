// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Extensions;

/// <summary>
/// 广告线<b>词表外</b>凭据 Query 参数名登记表（守卫 ADS-B5 的落地入口）。
/// </summary>
/// <remarks>
/// <para>
/// 组件的 URL 脱敏（<c>SensitiveUrlRedactor</c>）是<b>精确匹配词表</b>语义：不做前缀/后缀模糊匹配，
/// 所以通用名 <c>token</c> 覆盖不到 <c>user_token</c>。而 <c>user_token</c> 是官方 v3.0「受限接口」
/// 逐请求传入的<b>实名认证令牌</b>（<c>advertiser/update</c>、<c>advertiser/update_daily_budget</c> 两页
/// 均在其「全局参数」之外单列该参数，2026-10-10 逐页核验），一旦出现在
/// <c>ApiException.RequestUri</c>、日志与遥测 URL 中即为凭据明文外泄。
/// </para>
/// <para>
/// <b>处置方式取组件的公开登记门面</b> <c>Mud.HttpUtils.SensitiveUrlKeys.Register</c>
/// （登记后的键<b>无论可观测性开关为何都强制掩码</b>），与企微线群机器人 <c>key</c> 参数的既有做法同形
/// （见守卫 <c>WEB3</c>）。<b>不</b>去改组件静态词表：那是全局面，且本仓无权替上游决定通用键名。
/// </para>
/// <para>
/// 已在组件静态词表内的广告线凭据（<c>access_token</c>、<c>refresh_token</c>、<c>client_secret</c>、
/// <c>authorization_code</c>）<b>不</b>在此登记 —— 重复登记不会出错，但会让「哪些键靠词表、哪些靠登记」
/// 这一分界失去可审计性（守卫 ADS-B5 同时断言本表内容与词表<b>不交叉</b>）。
/// </para>
/// </remarks>
public static class AdsSensitiveQueryKeys
{
    /// <summary>实名认证令牌参数名（官方受限接口逐请求传入）。</summary>
    public const string UserToken = "user_token";

    /// <summary>本线需要登记的全部词表外凭据参数名（新增 Query 凭据时必须同批扩充，守卫 ADS-B5 锁定）。</summary>
    public static string[] All { get; } = { UserToken };

    /// <summary>把 <see cref="All"/> 逐项登记为进程级强制掩码键（幂等；由 <see cref="AdsServiceBuilder.Build"/> 调用）。</summary>
    internal static void RegisterAll()
    {
        // RegisterAll 而非在静态构造里跑：静态构造的执行时机不可控，
        // 「未注册任何模块就抛异常」的路径不应产生进程级全局副作用。
        SensitiveUrlKeys.RegisterAll(All);
    }
}
