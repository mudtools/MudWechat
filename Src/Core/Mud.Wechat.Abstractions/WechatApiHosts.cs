// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions;

/// <summary>
/// 微信系 + 腾讯广告 API 域名常量与 SSRF 白名单（**单一事实来源**）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何是常量类而非 DI 抽象</b>：默认域名是**编译期常量**（不随环境 / 时间变化；变化的只有经
/// <see cref="Configuration.WechatAppConfigBase.BaseUrl"/> 的覆盖，仍受白名单约束），引入
/// 「默认域名注册表」这类 DI 抽象只会增加一层无消费方的间接与注册顺序负担（YAGNI）。
/// </para>
/// <para>
/// <b>白名单为何必须在此单点持有</b>：组件 <c>UrlValidator</c> 的白名单是<b>进程级全局静态</b>。
/// 本仓以增量入口 <c>AddAllowedDomain</c> 登记（只加不删、HashSet 幂等，多 SDK/多产品线共存互不影响；
/// 组件配置热更重放也不会清除运行期桶）——但「白名单包含哪些域名」仍必须是单一事实源：若各产品线
/// 各自维护并登记自己的清单，「是否该放行某新域名」将无从审计。故白名单数组在此单点定义，并由
/// <see cref="TokenManager.WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/> 统一登记。
/// 语义矩阵：<c>ConfigureAllowedDomains</c> = 完全替换（含清空运行期桶；仅"我就是唯一真相源"的
/// 宿主/测试使用，本仓生产代码不使用，由 AB-G4/AB-G9 锁定）；<c>AddAllowedDomain</c> = 追加（共存场景唯一正确入口）。
/// </para>
/// <para>
/// 白名单判定语义（组件侧）：host 等于域或以其子域结尾即放行。因此 <c>weixin.qq.com</c> 一项即覆盖
/// <c>api.weixin.qq.com</c>（公众号）、<c>qyapi.weixin.qq.com</c>（企业微信）、
/// <c>api.mch.weixin.qq.com</c>（微信支付）；<c>work.weixin.qq.com</c> 为历史冗余项，保留以避免行为变更。
/// </para>
/// <para>
/// <b>唯一的后缀级例外：腾讯广告 <c>e.qq.com</c></b>。广告线（Marketing API v3.0）是本仓第一条
/// <b>非微信域</b>产品线，其 <c>api.e.qq.com</c> 不被任何既有条目覆盖 ⇒ 白名单必须<b>并集追加</b>一条后缀域，
/// 而不是逐主机登记（逐主机会让放行面随端点增长失控）。
/// 代价是<b>全局放行面被扩大</b>：任一产品线的 <c>BaseUrl</c> 现在都可指向 <c>*.e.qq.com</c> 而无需
/// <see cref="Configuration.WechatAppConfigBase.AllowCustomBaseUrl"/>。已接受的判定理由与替代方案的取舍
/// 见守卫 ADS-B4 与 AB-G9；<b>再新增非微信后缀域须走同等评审</b>，不得照抄本行「顺手加一条」。
/// </para>
/// </remarks>
public static class WechatApiHosts
{
    /// <summary>企业微信 API 默认域名（企业主体 / 服务商接口统一）。</summary>
    public const string WorkBaseUrl = "https://qyapi.weixin.qq.com";

    /// <summary>微信公众号 / 服务号 API 默认域名（订阅号与服务号同域名）。</summary>
    public const string OfficialAccountBaseUrl = "https://api.weixin.qq.com";

    /// <summary>
    /// 腾讯广告 Marketing API v3.0 默认域名（OAuth 与业务接口同主机，仅路径前缀不同：
    /// <c>/oauth/*</c> 与 <c>/v3.0/{resource}/{action}</c>）。
    /// </summary>
    public const string AdsBaseUrl = "https://api.e.qq.com";

    /// <summary>
    /// <see cref="Configuration.WechatAppConfigBase.AllowCustomBaseUrl"/> 为 <c>false</c> 时的
    /// BaseUrl 域名白名单（SSRF 防线；host 等于域或以其子域结尾即放行）。
    /// </summary>
    /// <remarks>
    /// 全仓唯一来源：各产品线的 <c>AddXxxApp</c> 均经
    /// <see cref="TokenManager.WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/> 以<b>本数组</b>
    /// 调用组件 <c>UrlValidator.AddAllowedDomain</c> 逐域增量登记（只加不删，HashSet 幂等，
    /// 多产品线/多 SDK 共存互不影响）。
    /// </remarks>
    public static readonly string[] AllowedBaseUrlDomains =
    {
        "weixin.qq.com",
        "work.weixin.qq.com",
        "e.qq.com",
    };
}
