// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions;

/// <summary>
/// 微信系 API 域名常量与 SSRF 白名单（**单一事实来源**）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何是常量类而非 DI 抽象</b>：默认域名是**编译期常量**（不随环境 / 时间变化；变化的只有经
/// <see cref="Configuration.WechatAppConfigBase.BaseUrl"/> 的覆盖，仍受白名单约束），引入
/// 「默认域名注册表」这类 DI 抽象只会增加一层无消费方的间接与注册顺序负担（YAGNI）。
/// </para>
/// <para>
/// <b>白名单为何必须在此单点持有</b>：组件 <c>UrlValidator.ConfigureAllowedDomains</c> 是
/// <b>进程级全局静态 + 整体替换语义</b>（调用后白名单<b>恰好</b>等于传入集合，并清空运行期桶）。
/// 若各产品线以各自的白名单分别调用，后调用者会<b>清空</b>前者 —— 多产品线共存时静默失去 SSRF 放行。
/// 故白名单数组在此单点定义，并由
/// <see cref="TokenManager.WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/> 统一登记
/// （同值调用幂等）。
/// </para>
/// <para>
/// 白名单判定语义（组件侧）：host 等于域或以其子域结尾即放行。因此 <c>weixin.qq.com</c> 一项即覆盖
/// <c>api.weixin.qq.com</c>（公众号）、<c>qyapi.weixin.qq.com</c>（企业微信）、
/// <c>api.mch.weixin.qq.com</c>（微信支付）；<c>work.weixin.qq.com</c> 为历史冗余项，保留以避免行为变更。
/// </para>
/// </remarks>
public static class WechatApiHosts
{
    /// <summary>企业微信 API 默认域名（企业主体 / 服务商接口统一）。</summary>
    public const string WorkBaseUrl = "https://qyapi.weixin.qq.com";

    /// <summary>微信公众号 / 服务号 API 默认域名（订阅号与服务号同域名）。</summary>
    public const string OfficialAccountBaseUrl = "https://api.weixin.qq.com";

    /// <summary>
    /// <see cref="Configuration.WechatAppConfigBase.AllowCustomBaseUrl"/> 为 <c>false</c> 时的
    /// BaseUrl 域名白名单（SSRF 防线；host 等于域或以其子域结尾即放行）。
    /// </summary>
    /// <remarks>
    /// 全仓唯一来源：各产品线的 <c>AddXxxApp</c> 均经
    /// <see cref="TokenManager.WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/> 以<b>本数组</b>
    /// 调用组件 <c>UrlValidator.ConfigureAllowedDomains</c>，从而在多产品线共存时保持一致（幂等）。
    /// </remarks>
    public static readonly string[] AllowedBaseUrlDomains =
    {
        "weixin.qq.com",
        "work.weixin.qq.com",
    };
}
