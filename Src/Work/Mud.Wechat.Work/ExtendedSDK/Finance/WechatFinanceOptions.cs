// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// 会话内容存档 C SDK 封装的配置面（配置节 <c>WechatFinance</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>WechatAppConfig</c> 无关</b>：存档机器人不是普通自建应用 —— 它的 <c>secret</c> 只用于原生库
/// <c>Init</c>，不发 <c>gettoken</c>、不进令牌链路，也不参与 <c>[Token]</c> 注入。把机器人塞进
/// <see cref="WechatAppConfig"/> 会让「应用凭据」与「存档凭据」两套语义互相污染（并且会诱使实现者去
/// 注册 <c>[HttpClientApi]</c> 客户端，而本域根本没有 HTTP 端点）。故本配置自成一面，键为机器人键。
/// </para>
/// <para>
/// <b>不落 <c>WechatModule</c> 枚举</b>：模块枚举与 <c>Add{域}Api()</c> 是「HTTP 业务域」的注册单位
/// （由 <c>WechatInterfaceNamespaceContractGuards</c> 与模块一致性守卫锁定）。本域是原生封装，
/// 注册入口为 <c>AddWechatFinanceSdk()</c>，与模块面<b>零交叉</b>（守卫 FIN-B6 断言枚举里不得出现 Finance）。
/// </para>
/// <para>
/// <b>secret 与私钥一律只登记「密钥名」</b>：真实值经组件 <c>ISecretProvider</c> 运行期取用，
/// 与微信支付侧同一治理（守卫 FIN-B2）。<see cref="Validate"/> 会在启动期点名拒绝「把 PEM 贴进配置」。
/// </para>
/// </remarks>
public class WechatFinanceOptions
{
    /// <summary>默认配置节名。</summary>
    public const string DefaultSectionName = "WechatFinance";

    /// <summary>
    /// 官方单片上限（512KB）。契约留档 + 测试的边界锚点（守卫 FIN-B3b 以它构造 512KB 边界分片），
    /// 生产代码<b>不</b>据此截断数据（真实分片尺寸以原生回写为准）。
    /// </summary>
    public const int OfficialMediaShardBytes = 512 * 1024;

    /// <summary>
    /// 原生库绝对路径（<c>WeWorkFinanceSdk.dll</c> / <c>libWeWorkFinanceSdk_C.so</c> 的完整路径）。
    /// 留空 = 按平台默认名探测。
    /// </summary>
    /// <remarks>
    /// 消费点：<c>FinanceNativeLibrary.SetProbePath</c>。<b>进程级唯一</b>（一个进程只装载一份原生实现），
    /// 多机器人给出不同值即抛。<c>netstandard2.0</c> 档无 <c>NativeLibrary</c> API ⇒ 配了值会点名抛错，
    /// 不静默忽略。原生库<b>不随 nupkg 分发</b>，由宿主按平台部署。
    /// </remarks>
    public string NativeLibraryPath { get; set; } = string.Empty;

    /// <summary>
    /// 原生调用超时（<b>秒</b>）的全局默认值，机器人未单独给出时使用。
    /// </summary>
    /// <remarks>消费点：<c>WechatFinanceRobotOptions.ResolveTimeoutSeconds</c> → 原生 <c>timeout</c> 入参。</remarks>
    public int DefaultTimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// 媒体分片拉取失败时的<b>同游标</b>重试次数（0 = 关闭重试）。
    /// </summary>
    /// <remarks>
    /// 消费点：<c>FinanceMediaAssembler</c>。只对 <see cref="WechatFinanceNativeCodes.IsMediaShardRetryable"/>
    /// 命中的返回码生效；重试<b>不推进游标</b>（推进游标等于丢片）。
    /// </remarks>
    public int MediaShardRetryCount { get; set; } = 3;

    /// <summary>媒体分片重试的退避间隔（毫秒）。</summary>
    /// <remarks>消费点：<c>FinanceMediaAssembler</c>（<c>Thread.Sleep</c> 的等待长度）。</remarks>
    public int MediaShardRetryDelayMs { get; set; } = 500;

    /// <summary>
    /// 存档机器人配置（键 = 机器人键，形状经 <see cref="WechatAppKeyValidator"/> 校验）。
    /// </summary>
    /// <remarks>
    /// 消费点：<c>WechatWorkFinanceClientFactory.GetClientAsync(robotKey)</c> 按此表定位配置并缓存客户端。
    /// 键即「宿主传给工厂的那个字符串」，同时也是诊断文本里的机器人标识。
    /// </remarks>
    public Dictionary<string, WechatFinanceRobotOptions> Robots { get; set; } = new();

    /// <summary>
    /// 校验配置完整性（启动期/注册期调用；缺失或形态非法即抛）。
    /// </summary>
    public void Validate()
    {
        if (Robots.Count == 0)
        {
            throw new InvalidOperationException(
                "会话存档配置缺少 Robots（至少配置一个存档机器人：WechatFinance:Robots:{机器人键}:...）。");
        }

        if (DefaultTimeoutSeconds <= 0)
        {
            throw new InvalidOperationException(
                $"WechatFinance:DefaultTimeoutSeconds 必须为正数（当前 {DefaultTimeoutSeconds}）。");
        }

        if (MediaShardRetryCount < 0)
        {
            throw new InvalidOperationException(
                $"WechatFinance:MediaShardRetryCount 不得为负（当前 {MediaShardRetryCount}；0 = 关闭重试）。");
        }

        if (MediaShardRetryDelayMs < 0)
        {
            throw new InvalidOperationException(
                $"WechatFinance:MediaShardRetryDelayMs 不得为负（当前 {MediaShardRetryDelayMs}）。");
        }

        foreach (var pair in Robots)
        {
            WechatAppKeyValidator.Validate(pair.Key);

            if (pair.Value == null)
            {
                throw new InvalidOperationException($"会话存档配置 Robots[\"{pair.Key}\"] 为 null。");
            }

            pair.Value.Validate(pair.Key, DefaultTimeoutSeconds);
        }
    }
}

/// <summary>
/// 单个存档机器人的配置（<c>corpid</c> + 存档 <c>secret</c> 的<b>密钥名</b> + RSA 私钥版本映射）。
/// </summary>
/// <remarks>
/// 与 <see cref="WechatFinanceOptions"/> 同类文件放置 —— <c>audit-config-keys.ps1</c> 按文件扫描配置属性消费点。
/// </remarks>
public class WechatFinanceRobotOptions
{
    /// <summary>
    /// 企业 ID（原生 <c>Init</c> 的 <c>corpid</c>）。
    /// </summary>
    /// <remarks>消费点：<c>WechatWorkFinanceClient</c> 初始化。存档能力按企业开通，故这里是企业 ID 而非应用密钥。</remarks>
    public string CorpId { get; set; } = string.Empty;

    /// <summary>
    /// 存档机器人 <c>secret</c> 在 <c>ISecretProvider</c> 中的<b>名称</b>（不是 secret 本身）。
    /// </summary>
    /// <remarks>
    /// 消费点：<c>WechatWorkFinanceClientFactory</c> 取值后送原生 <c>Init</c>。取值须为 ISecretProvider 已登记的名字
    /// （如 <c>finance:robot:archive-01:secret</c>）。
    /// <see cref="Validate"/> 的启动期判据<b>只能拦住 PEM/长 base64 形态</b>的密钥体误填（见
    /// <c>ValidateSecretName</c>）—— 存档 secret 本身是短串，贴进本属性不会被启动期拒绝，只会落到
    /// 运行期「ISecretProvider 查无密钥名」的点名异常；最终防线是把真实值登记进 ISecretProvider 而非配置文件。
    /// </remarks>
    public string SecretSecretName { get; set; } = string.Empty;

    /// <summary>
    /// 代理地址（形如 <c>http://127.0.0.1:8080</c>；留空 = 不走代理）。
    /// </summary>
    /// <remarks>消费点：原生 <c>GetChatData</c> / <c>GetMediaData</c> 的 <c>proxy</c> 入参。</remarks>
    public string ProxyAddress { get; set; } = string.Empty;

    /// <summary>代理口令在 <c>ISecretProvider</c> 中的<b>名称</b>（留空 = 代理无凭据）。</summary>
    /// <remarks>消费点：<c>WechatWorkFinanceClientFactory</c> → 原生 <c>passwd</c> 入参。</remarks>
    public string ProxyPasswordSecretName { get; set; } = string.Empty;

    /// <summary>
    /// 本机器人的原生调用超时（<b>秒</b>）；<c>0</c> = 沿用
    /// <see cref="WechatFinanceOptions.DefaultTimeoutSeconds"/>。
    /// </summary>
    /// <remarks>消费点：<see cref="ResolveTimeoutSeconds"/>。</remarks>
    public int TimeoutSeconds { get; set; }

    /// <summary>
    /// <c>publickey_ver</c> → RSA 私钥<b>密钥名</b>的映射（键为版本号的十进制字符串，如 <c>"1"</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 消费点：<c>WechatWorkFinanceClient.DecryptChatRecordAsync</c> 按记录上的版本号取名字、再经
    /// <c>ISecretProvider</c> 取私钥。官方支持密钥轮换 ⇒ 映射是<b>必填链路</b>而非优化项：
    /// 只配一版私钥时，历史密文（旧版本号）必然解不开。
    /// </para>
    /// <para>
    /// 键用字符串而非 <c>int</c>：配置绑定源生成器对字典键类型的转换面不覆盖 <c>int</c>，
    /// 且反射绑定在本仓 AOT 红线下不可用。<see cref="TryGetPrivateKeySecretName"/> 负责转换与校验。
    /// </para>
    /// </remarks>
    public Dictionary<string, string> PrivateKeySecretNames { get; set; } = new();

    /// <summary>解析本机器人实际生效的原生超时（秒）。</summary>
    /// <param name="defaultTimeoutSeconds">全局默认值。</param>
    /// <returns><see cref="TimeoutSeconds"/> 非正时回落到默认值。</returns>
    public int ResolveTimeoutSeconds(int defaultTimeoutSeconds)
        => TimeoutSeconds > 0 ? TimeoutSeconds : defaultTimeoutSeconds;

    /// <summary>按 <c>publickey_ver</c> 取 RSA 私钥的密钥名。</summary>
    /// <param name="publicKeyVersion">记录上的版本号。</param>
    /// <param name="secretName">命中的密钥名（未命中为 <c>null</c>）。</param>
    /// <returns>是否命中。</returns>
    public bool TryGetPrivateKeySecretName(int publicKeyVersion, out string? secretName)
    {
        secretName = null;

        // 版本号 → 键的转换必须走 InvariantCulture：配置键是文化无关的字面量，
        // 用 ToString()（当前文化）在非英语文化下可能带分组符，从而查不到自己配的键。
        return PrivateKeySecretNames.TryGetValue(publicKeyVersion.ToString(CultureInfo.InvariantCulture), out secretName);
    }

    /// <summary>校验本机器人配置（由 <see cref="WechatFinanceOptions.Validate"/> 逐机器人调用）。</summary>
    /// <param name="robotKey">机器人键（诊断文本使用）。</param>
    /// <param name="defaultTimeoutSeconds">全局默认超时（用于回落后校验）。</param>
    public void Validate(string robotKey, int defaultTimeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(CorpId))
        {
            throw new InvalidOperationException($"会话存档机器人「{robotKey}」缺少 CorpId。");
        }

        ValidateSecretName(SecretSecretName, nameof(SecretSecretName), robotKey, required: true);

        if (TimeoutSeconds < 0)
        {
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」的 TimeoutSeconds 不得为负（当前 {TimeoutSeconds}；0 = 沿用全局默认）。");
        }

        if (ResolveTimeoutSeconds(defaultTimeoutSeconds) <= 0)
        {
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」生效超时非正（{ResolveTimeoutSeconds(defaultTimeoutSeconds)}）。");
        }

        ValidateSecretName(ProxyPasswordSecretName, nameof(ProxyPasswordSecretName), robotKey, required: false);

        if (!string.IsNullOrWhiteSpace(ProxyAddress) && ProxyAddress.Any(char.IsWhiteSpace))
        {
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」的 ProxyAddress 含空白（代理地址须为单个 URI 形态，如 http://127.0.0.1:8080）。");
        }

        if (PrivateKeySecretNames.Count == 0)
        {
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」未配置 PrivateKeySecretNames" +
                "（publickey_ver → RSA 私钥密钥名；缺映射则任何会话记录都无法解密）。");
        }

        foreach (var pair in PrivateKeySecretNames)
        {
            if (!int.TryParse(pair.Key, NumberStyles.None, CultureInfo.InvariantCulture, out var version) || version <= 0)
            {
                throw new InvalidOperationException(
                    $"会话存档机器人「{robotKey}」的 PrivateKeySecretNames 键「{pair.Key}」不是正整数版本号" +
                    "（键须为 publickey_ver 的十进制字符串，如 \"1\"）。");
            }

            ValidateSecretName(pair.Value, $"PrivateKeySecretNames[\"{pair.Key}\"]", robotKey, required: true);
        }
    }

    /// <summary>
    /// 密钥名校验：必须是<b>名字</b>而不是密钥本身。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 这是最常见的配置事故 —— 字段名叫 <c>*SecretName</c>，却把 PEM 私钥整段贴了进去。不拦的话症状是运行期
    /// <c>ISecretProvider</c> 查无此名、抛出一条「找不到名字 -----BEGIN ...」的离奇错误，
    /// <b>并把私钥头带进异常消息</b>（等于把私钥洒进日志）。启动期直接点名比什么都清楚（与支付侧同一判据）。
    /// </para>
    /// <para><b>ns2.0 无 <c>Contains(string, StringComparison)</c></b> ⇒ 一律用 <c>IndexOf(...) &gt;= 0</c>。</para>
    /// </remarks>
    private static void ValidateSecretName(string value, string name, string robotKey, bool required)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (required)
            {
                throw new InvalidOperationException($"会话存档机器人「{robotKey}」缺少 {name}。");
            }

            return;
        }

        if (value.IndexOf("-----BEGIN", StringComparison.OrdinalIgnoreCase) >= 0 ||
            LooksLikeKeyMaterial(value))
        {
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」的 {name} 看起来填的是**密钥原文**而不是 ISecretProvider 中的密钥名。" +
                "存档 secret 与 RSA 私钥绝不入配置，只登记名字、运行期经 ISecretProvider 取用。");
        }

        if (value.Length > 256 || value.Any(char.IsWhiteSpace))
        {
            throw new InvalidOperationException(
                $"会话存档机器人「{robotKey}」的 {name} 格式非法（密钥名不应含空白或超过 256 字符）。");
        }
    }

    /// <summary>
    /// 是否形如被误填进来的密钥 Base64 体（RSA 密钥/证书的 Base64 体恒以 <c>MII</c> 开头且较长）。
    /// </summary>
    /// <remarks>
    /// 判据与支付侧 <c>WechatPayMerchantConfig.LooksLikeBase64KeyMaterial</c> <b>逐字一致</b>：
    /// 必须「以 MII 开头 + 长度 ≥64 + 字符集 ⊆ base64」三者同时成立才算密钥原文。
    /// 该判定误伤即部署阻断（<see cref="Validate"/> 在启动期硬失败），故阈值宁可保守 ——
    /// 「纯字母数字的长密钥名」不算密钥。
    /// </remarks>
    private static bool LooksLikeKeyMaterial(string value)
    {
        const int MinimumKeyMaterialLength = 64;

        if (value.Length < MinimumKeyMaterialLength ||
            !value.StartsWith("MII", StringComparison.Ordinal))
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!(char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='))
            {
                return false;
            }
        }

        return true;
    }
}
