// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Abstractions.Configuration;

/// <summary>
/// 腾讯广告 Marketing API v3.0 应用配置（一个配置 = 一个开发者应用 + 它当前授权的那个投放账号）。
/// </summary>
/// <remarks>
/// <para>
/// 公共形状（AppKey / BaseUrl / AllowCustomBaseUrl / TimeoutSeconds / TokenRefreshThreshold / IsDefault
/// 与三条通用校验）由公用层 <see cref="WechatAppConfigBase"/> 承担；本类只声明广告线专属字段。
/// </para>
/// <para>
/// <b>配置节</b>：<c>WechatAds</c>（注册入口 <c>AddAdsApp</c> 的默认节名）。绑定走源生成器，
/// 调用侧必须 <c>section.Bind(list)</c> 形态，<b>不得</b>用 <c>Configure&lt;T&gt;(IConfiguration)</c> 反射重载。
/// </para>
/// <para>
/// <b>AOT</b>：不使用 <c>required</c>（ConfigurationBinder 以 <c>new T()</c> 构造 ⇒ <c>CS9035</c>），
/// 必填校验统一走 <see cref="Validate"/>。
/// </para>
/// <para>
/// <b>为什么没有 <c>AccountId</c> 配置项</b>：<c>account_id</c> 是<b>授权结果</b>而非配置输入 ——
/// 它来自 <c>oauth/token</c> 应答的 <c>data.authorizer_info.account_id</c>，随令牌落库
/// （<c>AdsAuthorizationState.AccountId</c>）。把它做成配置项会允许「配置说 A 账号、令牌其实属于 B 账号」
/// 这种非法状态（与 <c>WechatAppConfig</c> 不提供独立 <c>TemplateId</c> 同一思路：非法状态不可表达）。
/// 业务请求里的 <c>account_id</c> 由调用方<b>显式</b>传入，SDK 不做隐式注入。
/// </para>
/// </remarks>
public class AdsAppConfig : WechatAppConfigBase
{
    /// <summary>
    /// 开发者应用 id（<c>client_id</c>，官方类型 integer，在开发者官网创建应用后获得）。
    /// </summary>
    /// <remarks>
    /// <b>形态取舍</b>：官方把 <c>client_id</c> 标为 integer，配置面用 <see cref="string"/> 承载
    /// （配置源是 JSON / 环境变量，字符串更稳，且上送时本就进 Query 文本）。
    /// 校验只做「非空 + 全数字 + ≤19 位」，<b>不</b>把它截成 long 以免静默溢出。
    /// </remarks>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 开发者应用密钥（<c>client_secret</c>）。<b>永不</b>写入日志 / 遥测 / 异常消息。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方自相矛盾（照录、不替官方修正）</b>：<c>oauth/token</c> 页写「长度最大 256 字节」，
    /// <c>oauth/refresh_token</c> 页写「应用密码小于 128 个英文字符 / 长度最大 128 字节」。
    /// 故本配置<b>不做</b>本地长度拦截（取哪个上限都是替官方做决定），真实超限由应答 <c>code</c> 表达。
    /// </para>
    /// </remarks>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// 授权回调地址（<c>redirect_uri</c>，换授权码时上送；官方要求主域名与创建应用时登记的回调域名一致，
    /// 仅支持 http/https、<b>不支持指定端口号</b>，携带参数须 urlencode）。
    /// </summary>
    /// <remarks>
    /// <b>可为空</b>：官方把 <c>redirect_uri</c> 列为<b>选填</b>（2026-10-10 核验 <c>oauth/token</c> 参数表，
    /// 无 <c>*</c> 标记）。留空时由 <c>ExchangeAuthorizationCodeAsync</c> 的入参提供 ——
    /// 两处都留空才判为配置/用法错误（<see cref="Validate"/> 不拦，因为「不用授权码流程」的宿主本就可以不填）。
    /// </remarks>
    public string RedirectUri { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override string DefaultBaseUrl => WechatApiHosts.AdsBaseUrl;

    /// <summary>
    /// 校验广告线配置：通用规则由基类承担，本类追加应用凭据必填。
    /// </summary>
    /// <exception cref="InvalidOperationException">配置非法时抛出（启动阶段即失败）。</exception>
    public override void Validate()
    {
        base.Validate();

        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new InvalidOperationException($"腾讯广告应用 {AppKey} 缺少 ClientId（官方 client_id）。");
        }

        if (!IsAllDigits(ClientId))
        {
            throw new InvalidOperationException(
                $"腾讯广告应用 {AppKey} 的 ClientId 必须是纯数字（官方类型 integer）：{ClientId}");
        }

        if (ClientId.Length > 19)
        {
            throw new InvalidOperationException(
                $"腾讯广告应用 {AppKey} 的 ClientId 长度超过 64 位整型的十进制位数（19）：{ClientId.Length}");
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            throw new InvalidOperationException($"腾讯广告应用 {AppKey} 缺少 ClientSecret（官方 client_secret）。");
        }
    }

    /// <summary>
    /// 返回掩码后的配置描述：<b>不含 ClientSecret</b>，也不含任何令牌值。
    /// </summary>
    public override string ToString()
        => $"AdsAppConfig(AppKey={AppKey}, ClientId={ClientId}, BaseUrl={BaseUrl}, RedirectUri={RedirectUri})";

    /// <summary>是否为纯 ASCII 数字（ns2.0 无 <c>char.IsAsciiDigit</c> 聚合助手，手写以免依赖宽字符判定）。</summary>
    private static bool IsAllDigits(string value)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (c < '0' || c > '9')
            {
                return false;
            }
        }

        return value.Length > 0;
    }
}
