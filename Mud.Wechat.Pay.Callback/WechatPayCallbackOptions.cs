// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 微信支付 APIv3 回调接收配置（节名 <c>WechatPayCallback</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>凭据唯一来源</b>：商户身份与密钥名<b>不在本配置里</b>，而是复用 <c>AddPayApp</c> 注册的
/// <c>IWechatPayMerchantManager</c>（商户键 → <c>ApiKeySecretName</c> → 经 <c>ISecretProvider</c> 取 APIv3 密钥）。
/// 这是「回调包只需 <c>AddPayApp</c> 即可完成验签与解密」的形态保证（方案 §6），也避免商户清单在两处漂移。
/// </para>
/// <para>
/// <b>路由</b>：<c>/{GlobalRoutePrefix}/{MerchantKey}</c>，默认前缀 <c>pay</c>
/// （企微 <c>wechat</c> / 公众号 <c>mp</c>，同宿主不冲突）。<c>notify_url</c> 由宿主在下单时指定，
/// 必须与本路由一致；未在 <c>IWechatPayMerchantManager</c> 登记的商户键<b>一律 fail-closed 拒绝</b>，
/// <b>不得</b>回落到默认商户（否则会把某商户的资金通知派给另一商户的处理器）。
/// </para>
/// <para>
/// <b>AOT</b>：不使用 <c>required</c>（ConfigurationBinder 以 <c>new T()</c> 构造 ⇒ <c>CS9035</c>），
/// 校验统一走 <see cref="Validate"/>。
/// </para>
/// </remarks>
public sealed class WechatPayCallbackOptions
{
    /// <summary>配置节名。</summary>
    public const string SectionName = "WechatPayCallback";

    /// <summary>回调路由前缀（形如 <c>/{GlobalRoutePrefix}/{MerchantKey}</c>）。</summary>
    public string GlobalRoutePrefix { get; set; } = "pay";

    /// <summary>请求体字节上限（默认 256KB；APIv3 通知报文很小，超限即拒以省资源）。</summary>
    public int MaxRequestBodySize { get; set; } = 262_144;

    /// <summary>时间戳时效窗容差（秒；默认 300，与 <c>WechatPaySignatureMessages.DefaultTimestampSkewSeconds</c> 同值）。</summary>
    public int AllowClockSkewSeconds { get; set; } = 300;

    /// <summary>
    /// 抗重放指纹保留窗口（秒；默认 600 = 2× 时效窗）。
    /// </summary>
    /// <remarks>必须 <b>≥</b> 时效窗：否则指纹先过期、而报文仍在时效窗内，重放即可穿过指纹闸。</remarks>
    public int ReplayWindowSeconds { get; set; } = 600;

    /// <summary>事件处理软超时（毫秒；默认 4000，<b>必须 &lt; 5000</b> —— 平台 5 秒断连契约）。</summary>
    public int EventHandlingTimeoutMs { get; set; } = 4_000;

    /// <summary>是否启用一次性指纹闸（默认开启；<b>生产不得关闭</b>，仅排障用）。</summary>
    public bool RequireReplayGuard { get; set; } = true;

    /// <summary>校验配置（缺失必填项或非法取值时抛出）。</summary>
    /// <exception cref="InvalidOperationException">配置非法时抛出（启动阶段即失败）。</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(GlobalRoutePrefix) || GlobalRoutePrefix.Contains("/"))
        {
            throw new InvalidOperationException("回调路由前缀不能为空白且不得包含 '/'。");
        }

        if (MaxRequestBodySize <= 0)
        {
            throw new InvalidOperationException("MaxRequestBodySize 必须为正数。");
        }

        if (AllowClockSkewSeconds <= 0)
        {
            throw new InvalidOperationException("AllowClockSkewSeconds 必须为正数。");
        }

        if (ReplayWindowSeconds < AllowClockSkewSeconds)
        {
            throw new InvalidOperationException(
                $"ReplayWindowSeconds（{ReplayWindowSeconds}）不得小于 AllowClockSkewSeconds（{AllowClockSkewSeconds}）：" +
                "指纹保留窗口短于时效窗时，重放会穿过指纹闸。");
        }

        // 平台 5 秒断连契约不可协商：软超时必须严格小于 5000ms。
        if (EventHandlingTimeoutMs <= 0 || EventHandlingTimeoutMs >= 5000)
        {
            throw new InvalidOperationException(
                $"EventHandlingTimeoutMs 必须落在 (0, 5000) 区间（当前 {EventHandlingTimeoutMs}）：" +
                "微信支付在 5 秒内收不到 HTTP 200 会判定通知失败并按策略重试。");
        }
    }
}
