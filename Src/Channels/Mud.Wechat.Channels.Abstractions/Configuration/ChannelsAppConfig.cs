// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Configuration;

/// <summary>
/// 微信小店 / 视频号（channels 生态）应用配置。
/// </summary>
/// <remarks>
/// <para>
/// 公共形状（AppKey / BaseUrl / AllowCustomBaseUrl / TimeoutSeconds / TokenRefreshThreshold / IsDefault
/// 与三条通用校验）由公用层 <see cref="WechatAppConfigBase"/> 承担；本类只声明小店专属字段。
/// </para>
/// <para>
/// AOT：不使用 <c>required</c>（ConfigurationBinder 以 <c>new T()</c> 构造 ⇒ <c>CS9035</c>），
/// 必填校验统一走 <see cref="Validate"/>。
/// </para>
/// <para>
/// <b>凭据面唯一形态</b>：小店为「自建」应用（<c>AppId</c> + <c>AppSecret</c>，token + stable_token
/// 双通道）。官方无「第三方代小店」形态的配置面，本类不暴露任何相关字段。
/// </para>
/// </remarks>
public class ChannelsAppConfig : WechatAppConfigBase
{
    /// <summary>
    /// 小店账号的唯一凭证（AppID；wx 开头，但与公众号 / 小程序 AppID <b>不互通</b>）。
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 唯一凭证密钥（AppSecret）。
    /// </summary>
    /// <remarks>永不落日志 / 遥测 / 异常消息（组件脱敏词表已覆盖 <c>secret</c>）。</remarks>
    public string AppSecret { get; set; } = string.Empty;

    /// <summary>
    /// 令牌通道选择：<c>true</c> → 稳定版通道（<c>POST /cgi-bin/stable_token</c>，官方推荐，默认）；
    /// <c>false</c> → 普通通道（<c>GET /cgi-bin/token</c>）。
    /// </summary>
    /// <remarks>
    /// <b>真实消费点</b>：<c>AddChannelsApp</c> 注册期据此装配对应的令牌管理器实现
    /// （<c>ChannelsStableAccessTokenManager</c> / <c>ChannelsStandardAccessTokenManager</c>）。
    /// 接口层的令牌路由键恒为 <c>ChannelsTokenTypes.AccessToken</c>，通道差异不外泄到接口声明面。
    /// </remarks>
    public bool UseStableToken { get; set; } = true;

    /// <inheritdoc />
    protected override string DefaultBaseUrl => WechatApiHosts.OfficialAccountBaseUrl;

    /// <summary>
    /// 校验小店配置：通用规则由基类承担，本类追加账号凭据必填。
    /// </summary>
    /// <exception cref="InvalidOperationException">配置非法时抛出（启动阶段即失败）。</exception>
    public override void Validate()
    {
        base.Validate();

        if (string.IsNullOrWhiteSpace(AppId))
        {
            throw new InvalidOperationException($"小店 {AppKey} 缺少 AppId。");
        }

        if (string.IsNullOrWhiteSpace(AppSecret))
        {
            throw new InvalidOperationException($"小店 {AppKey} 缺少 AppSecret。");
        }
    }

    /// <summary>
    /// 返回掩码后的配置描述（AppKey / AppId / BaseUrl / 通道；AppSecret 永不写入日志）。
    /// </summary>
    public override string ToString()
        => $"ChannelsAppConfig(AppKey={AppKey}, AppId={AppId}, BaseUrl={BaseUrl}, UseStableToken={UseStableToken})";
}