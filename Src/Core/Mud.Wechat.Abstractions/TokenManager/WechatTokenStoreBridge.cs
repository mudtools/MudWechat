// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.TokenManager;

/// <summary>
/// 令牌缓存装配桥（各产品线令牌管理器共用）：把可选持久化仓储接线为组件
/// <c>ITokenCache&lt;CredentialToken&gt;</c>，或退回进程内缓存。
/// </summary>
/// <remarks>
/// <para>
/// <b>键映射（单射）</b>：持久化键 = <c>{tokenTypeKey}:{cacheKey}</c>，其中 <c>tokenTypeKey</c> 形如
/// <c>{产品线令牌类型}:{AppKey}</c>（公众号再带通道段）。不同产品线 / 不同通道 / 不同应用的键空间
/// 互不覆盖，故同一份存储实例可安全服务全部产品线。
/// </para>
/// <para>
/// <b>读穿透与写穿均由组件桥接器承担，SDK <u>不得</u>自建叠层</b>：
/// </para>
/// <list type="bullet">
/// <item><b>读</b>：桥接器实现 <c>IAsyncTokenCache&lt;T&gt;</c>，管理器在异步管线
/// <c>GetOrRefreshTokenAsync</c> 上做能力探测（<c>is IAsyncTokenCache&lt;T&gt;</c>）后走
/// <b>真读穿透</b>（镜像未命中 ⇒ 直达 store 读取并回填镜像）⇒ <b>多实例冷启动天然共享令牌，
/// 无需任何显式水合</b>；</item>
/// <item><b>写</b>：异步写穿 + 补偿重放 + 失败计数（不抛）。</item>
/// </list>
/// <para>
/// 故本桥刻意<b>只做接线</b>（构造桥接器 + 键映射），不重复实现水合 / 重试 / 解码：
/// 早前版本曾在外层叠加「首次取令牌前显式水合」，实测为<b>纯冗余</b>（读穿透已覆盖该场景，
/// 且会掩盖 store 故障的真实传播路径），已移除。
/// </para>
/// </remarks>
internal static class WechatTokenStoreBridge
{
    /// <summary>
    /// 创建令牌缓存：提供持久化仓储时装配写穿 / 读穿透桥接器，否则退回进程内并发字典缓存。
    /// </summary>
    /// <param name="tokenStore">持久化仓储（可为 null）。</param>
    /// <param name="tokenTypeKey">令牌类型键（<c>{令牌类型}:{AppKey}</c>）。</param>
    /// <param name="logger">日志器（桥接器诊断用）。</param>
    /// <returns>令牌缓存实例。</returns>
    public static ITokenCache<CredentialToken> CreateCache(
        IWechatTokenStore? tokenStore,
        string tokenTypeKey,
        ILogger logger)
    {
        if (tokenStore is null)
        {
            return new ConcurrentDictionaryTokenCache<CredentialToken>();
        }

        return new TokenStoreBackedTokenCache<CredentialToken>(
            tokenStore,
            valueAdapter: AdaptCredentialToken,
            valueFactory: CreateCredentialToken,
            storeKeyMapper: cacheKey => BuildStoreKey(tokenTypeKey, cacheKey),
            logger: logger);
    }

    /// <summary>构建令牌类型键（<c>{tokenTypeKeyPrefix}:{appKey}</c>）。</summary>
    /// <param name="tokenTypeKeyPrefix">令牌类型前缀（产品线常量；公众号含通道段）。</param>
    /// <param name="appKey">应用键。</param>
    /// <returns>令牌类型键。</returns>
    public static string BuildTokenTypeKey(string? tokenTypeKeyPrefix, string? appKey)
        => (tokenTypeKeyPrefix ?? string.Empty) + ":" + (appKey ?? string.Empty);

    /// <summary>构建持久化键（<c>{tokenTypeKey}:{cacheKey}</c>；读写两方向同源）。</summary>
    /// <param name="tokenTypeKey">令牌类型键。</param>
    /// <param name="cacheKey">镜像侧缓存键（scope）。</param>
    /// <returns>持久化键。</returns>
    public static string BuildStoreKey(string tokenTypeKey, string cacheKey)
        => tokenTypeKey + ":" + cacheKey;

    private static TokenStoreValue? AdaptCredentialToken(CredentialToken? token)
        => token is null
            ? null
            : new TokenStoreValue(
                accessToken: WechatTokenBridgeCodec.EncodeToken(token.AccessToken, token.Expire),
                refreshToken: null,
                expiresInSeconds: WechatTokenBridgeCodec.RemainingSeconds(token.Expire));

    private static CredentialToken? CreateCredentialToken(TokenStoreValue value)
    {
        var (accessToken, expireTimestampMs) = WechatTokenBridgeCodec.DecodeToken(value.AccessToken);
        if (string.IsNullOrEmpty(accessToken) || expireTimestampMs <= 0)
        {
            return null;
        }

        return new CredentialToken
        {
            AccessToken = accessToken,
            Expire = expireTimestampMs,
            IssuedAt = 0,
        };
    }
}
