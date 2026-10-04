// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions;

/// <summary>
/// 令牌归属域：声明「该令牌由哪一类主体持有」，决定可用的应用类型与令牌管理器。
/// </summary>
/// <remarks>
/// 与 <see cref="WechatTokenTypes"/>（业务令牌类型，与官方契约的参数名一一对应）正交：
/// 业务令牌类型不可变（改之会使注入参数名变化，服务端直接拒绝），本枚举承载的是
/// 「同一业务令牌类型在不同应用类型下凭据来源不同」这一差异。
/// </remarks>
internal enum WechatTokenOwner
{
    /// <summary>自建应用自身凭据（<c>corpid</c> + <c>corpsecret</c> 换取的 <c>access_token</c>）。</summary>
    Internal = 0,

    /// <summary>
    /// 授权企业级凭据（第三方 <c>get_corp_token</c> / 代开发 <c>gettoken(permanent_code)</c> 换取，
    /// scope = <c>authCorpId</c>）。
    /// </summary>
    Corp = 1,
}

/// <summary>
/// 令牌查找键的归属域解析与校验（<see cref="WechatTokenManagerKeys"/> 的运行期配套）。
/// </summary>
/// <remarks>
/// <para>
/// 纯字符串运算，零反射、零分配（失败路径除外）⇒ 满足 <c>netstandard2.0</c> 与 AOT/Trim 红线。
/// 调用点唯一：<c>WechatAppContext.GetTokenManager(string)</c>——全仓令牌解析咽喉点，
/// 生成式声明客户端（<c>TokenAttribute.TokenManagerKey</c> → <c>TokenRequest.TokenManagerKey</c>）、
/// 恢复执行器的按键定位均经此收敛。
/// </para>
/// <para>
/// 未携带归属域后缀的键（公共父接口、宿主直调 <c>GetTokenManager("Wechat.XxxAccessToken")</c>）
/// 原样返回、不校验，保证既有语义逐字节不变。
/// </para>
/// </remarks>
internal static class WechatTokenRouting
{
    /// <summary>
    /// 归属域后缀分隔符。刻意避开 <c>:</c>——后者是令牌缓存键三段式
    /// <c>{tokenType}:{appKey}:{scopeKey}</c> 的分段符，混用会造成键别名误读。
    /// </summary>
    internal const char OwnerSeparator = '@';

    /// <summary>
    /// 解析并校验令牌查找键，返回剥离归属域后的基础业务令牌类型。
    /// </summary>
    /// <param name="tokenManagerKey">令牌查找键（可能携带归属域后缀）。</param>
    /// <param name="appKey">当前应用标识（用于错误消息定位）。</param>
    /// <param name="appType">当前应用类型。</param>
    /// <returns>剥离归属域后缀的基础业务令牌类型（供按 <see cref="WechatTokenTypes"/> 路由）。</returns>
    /// <exception cref="InvalidOperationException">归属域后缀未知（键拼写错误）时抛出。</exception>
    /// <exception cref="WechatTokenOwnerMismatchException">
    /// 归属域与当前应用类型不匹配时抛出（接口选错应用类型，fail-fast）。
    /// </exception>
    internal static string ResolveBaseTokenType(string tokenManagerKey, string appKey, WechatAppType appType)
    {
        var separatorIndex = tokenManagerKey.IndexOf(OwnerSeparator);
        if (separatorIndex < 0)
        {
            return tokenManagerKey;
        }

        var baseTokenType = tokenManagerKey.Substring(0, separatorIndex);
        var ownerText = tokenManagerKey.Substring(separatorIndex + 1);

        if (!TryParseOwner(ownerText, out var owner))
        {
            throw new InvalidOperationException(
                $"未知的令牌归属域后缀：'{ownerText}'（合法值：Internal、Corp）。令牌查找键：'{tokenManagerKey}'。");
        }

        if (!IsOwnerAllowed(owner, appType))
        {
            throw new WechatTokenOwnerMismatchException(
                BuildMismatchMessage(tokenManagerKey, owner, appKey, appType),
                tokenManagerKey,
                appKey,
                appType);
        }

        return baseTokenType;
    }

    /// <summary>当前应用类型是否允许该归属域。</summary>
    /// <param name="owner">声明的归属域。</param>
    /// <param name="appType">当前应用类型。</param>
    /// <returns>允许返回 <c>true</c>。</returns>
    internal static bool IsOwnerAllowed(WechatTokenOwner owner, WechatAppType appType)
        => owner == WechatTokenOwner.Internal
            ? appType == WechatAppType.Internal
            : appType != WechatAppType.Internal;

    /// <summary>
    /// 本应用类型可拥有的全部归属域键（<b>单一事实来源</b>：接口声明、恢复注册表、
    /// 契约守卫三者同源，避免「新增键却漏入恢复注册表」导致 errcode 恢复静默降级）。
    /// </summary>
    /// <param name="appType">应用类型。</param>
    /// <returns>该应用类型允许的归属域键集合（非空）。</returns>
    internal static string[] OwnedKeys(WechatAppType appType)
        => appType == WechatAppType.Internal
            ? new[] { WechatTokenManagerKeys.InternalAccessToken }
            : new[] { WechatTokenManagerKeys.CorpAccessToken };

    /// <summary>解析归属域后缀（严格大小写敏感，与键常量的书写逐字一致）。</summary>
    /// <param name="ownerText">后缀文本。</param>
    /// <param name="owner">解析结果。</param>
    /// <returns>识别成功返回 <c>true</c>。</returns>
    internal static bool TryParseOwner(string ownerText, out WechatTokenOwner owner)
    {
        if (string.Equals(ownerText, "Internal", StringComparison.Ordinal))
        {
            owner = WechatTokenOwner.Internal;
            return true;
        }

        if (string.Equals(ownerText, "Corp", StringComparison.Ordinal))
        {
            owner = WechatTokenOwner.Corp;
            return true;
        }

        owner = WechatTokenOwner.Internal;
        return false;
    }

    /// <summary>
    /// 构造归属域不匹配的异常消息：键 + 期望应用类型 + 当前 appKey/AppType + 修复动作。
    /// </summary>
    private static string BuildMismatchMessage(
        string tokenManagerKey, WechatTokenOwner owner, string appKey, WechatAppType appType)
    {
        var requirement = owner == WechatTokenOwner.Internal
            ? "自建应用自身 access_token（corpid + corpsecret 换取），要求应用类型为 Internal"
            : "授权企业级 access_token（scope = authCorpId，经 get_corp_token / gettoken(permanent_code) 换取），要求应用类型为 ThirdParty/Provider";

        var remediation = owner == WechatTokenOwner.Internal
            ? "该接口为「企业自建应用」契约入口：请改用对应的 IWechatWorkThirdParty* / IWechatWorkProvider* 子接口，"
              + "或经 IWechatAppContextSwitcher.UseAppScope(\"<企业自建应用 AppKey>\") 切换到自建应用后再调用。"
            : "该接口为「第三方应用 / 服务商代开发」契约入口：请改用对应的 IWechatWorkInternal* 子接口；"
              + "若确需授权企业级令牌，请经 IWechatAppContextSwitcher.UseAppScope(\"<第三方/代开发应用 AppKey>\") "
              + "切换到该应用并 SetCorp(authCorpId) 后再调用。";

        return $"令牌归属域不匹配：接口声明的查找键 '{tokenManagerKey}' 需要 {requirement}，"
            + $"但当前应用 '{appKey}' 的 AppType={appType}。{remediation}";
    }
}
