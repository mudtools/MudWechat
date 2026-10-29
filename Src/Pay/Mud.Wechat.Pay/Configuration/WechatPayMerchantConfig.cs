// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Configuration;

/// <summary>
/// 微信支付商户凭据配置（单商户一份；多商户由 <c>WechatPayMerchantManager</c> 按 <see cref="MerchantKey"/> 分槽）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么本类不继承 <c>WechatAppConfigBase</c></b>（有意偏离另两条产品线）：
/// </para>
/// <list type="bullet">
/// <item><description><c>TokenRefreshThreshold</c> —— 支付<b>没有 <c>access_token</c></b>，阈值无消费点；</description></item>
/// <item><description><c>BaseUrl</c> + <c>AllowCustomBaseUrl</c> —— 继承即为<b>每个商户开一个 SSRF 白名单豁免口</b>；
/// 支付入口域名由官方固定（<c>api.mch.weixin.qq.com</c> / <c>api2.mch.weixin.qq.com</c>）且<b>已在</b>
/// <c>WechatApiHosts.AllowedBaseUrlDomains</c> 白名单内，本线<b>零改动</b>，不应给商户级覆盖留门；</description></item>
/// <item><description><c>AppKey</c> —— 支付的自然键是商户号（<c>mchid</c>），另设 AppKey 会造成「同商户两个键」的分裂。</description></item>
/// </list>
/// <para>
/// <b>凭据不落配置</b>：私钥原文与 APIv3 密钥<b>绝不</b>出现在本配置里（AGENTS §8 安全默认不得削弱 + §7 红线）。
/// 本类只存<b>密钥名</b>（<see cref="PrivateKeySecretName"/> / <see cref="ApiKeySecretName"/>），
/// 实际值一律经组件 <c>ISecretProvider</c> 在运行期取用（见 <c>IWechatPayMerchantCredentialProvider</c>）。
/// 这样配置文件可以安全入库 / 进版本控制 / 进配置中心，泄露不等于泄露私钥。
/// </para>
/// <para>
/// <b>AOT 约束</b>：不使用 <c>required</c>（ConfigurationBinder 以 <c>new T()</c> 构造 ⇒ <c>CS9035</c>），
/// 必填校验统一走 <see cref="Validate"/>（启动期 fail-fast）。
/// </para>
/// <para>
/// <b>审计约束</b>：本文件必须登记进 <c>scripts/audit-config-keys.ps1</c> 的 <c>$configFiles</c> 清单，
/// 否则属性新增即为门禁盲区。
/// </para>
/// </remarks>
public class WechatPayMerchantConfig
{
    /// <summary>商户号（普通商户形态必填；服务商形态必须留空）。</summary>
    public string MchId { get; set; } = string.Empty;

    /// <summary>商户 API 证书序列号（写入 <c>Authorization</c> 的 <c>serial_no</c>，供官方反查公钥）。</summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// 商户私钥在 <c>ISecretProvider</c> 中的<b>名称</b>（不是私钥本身）。
    /// </summary>
    /// <remarks>取值须为 <c>ISecretProvider</c> 已登记的名字（如 "pay:merchant:mchid1900000000:key"）。</remarks>
    public string PrivateKeySecretName { get; set; } = string.Empty;

    /// <summary>
    /// APIv3 密钥（32 字节）在 <c>ISecretProvider</c> 中的<b>名称</b>（不是密钥本身）。
    /// </summary>
    /// <remarks>用于回调 <c>resource</c> 的 AES-256-GCM 解密与请求侧敏感字段加密。</remarks>
    public string ApiKeySecretName { get; set; } = string.Empty;

    /// <summary>
    /// APIv2 签名密钥（<c>api_key</c>，32 位）在 <c>ISecretProvider</c> 中的<b>名称</b>（不是密钥本身）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>与 <see cref="ApiKeySecretName"/> 是两把不同的密钥，不得合并</b>：前者是 APIv3 的
    /// <c>AEAD_AES_256_GCM</c> 密钥（回调 <c>resource</c> 解密 + 敏感字段加密），本字段是 APIv2 的
    /// MD5/HMAC-SHA256 签名密钥；两者在商户平台<b>分别设置、互不相同</b>，且轮换节奏独立。
    /// 若压成同一个密钥名，只配置 APIv3 的商户会被静默拿去签 APIv2 请求，症状是官方恒返回
    /// <c>SIGNERROR</c> 而配置面看不出任何异常。
    /// </para>
    /// <para>留空表示该商户不使用 APIv2 线（APIv3-only 形态），<see cref="Validate"/> 不强制。</para>
    /// </remarks>
    public string ApiV2KeySecretName { get; set; } = string.Empty;

    /// <summary>
    /// APIv2 商户 API 证书（PKCS#12）在 <c>ISecretProvider</c> 中的<b>名称</b>（不是证书本身）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 仅 <c>/secapi/pay/*</c>（退款 / 退款查询 v2 / 撤销 / 企业付款 / 现金红包）需要，这些端点走
    /// <b>双向 TLS</b>（域名 <c>api2.mch.weixin.qq.com</c>，已在 <c>WechatApiHosts</c> 白名单内）。
    /// 证书口令<b>不</b>在本配置里出现（AGENTS §8「凭据不落配置」同一口径），
    /// 与证书材料一并作为 <c>ISecretProvider</c> 的一条记录登记。
    /// </para>
    /// <para>留空表示该商户不调用 secapi 支；调用时由 V2 凭据端口 fail-fast。</para>
    /// </remarks>
    public string ApiV2CertificateSecretName { get; set; } = string.Empty;

    /// <summary>
    /// APIv2 双向证书<b>口令</b>在 <c>ISecretProvider</c> 中的<b>名称</b>（不是口令本身）。
    /// </summary>
    /// <remarks>
    /// 留空时按官方约定回退为<b>商户号</b>（微信下载的 <c>apiclient_cert.p12</c> 默认口令即商户号）。
    /// 商户改过口令才需要配置本项；口令属凭据，<b>绝不</b>直接落配置（AGENTS §8）。
    /// </remarks>
    public string ApiV2CertificatePasswordSecretName { get; set; } = string.Empty;

    /// <summary>服务商商户号（与 <see cref="SubMchId"/> 成对出现；普通商户形态必须留空）。</summary>
    public string SpMchId { get; set; } = string.Empty;

    /// <summary>子商户号（与 <see cref="SpMchId"/> 成对出现；普通商户形态必须留空）。</summary>
    public string SubMchId { get; set; } = string.Empty;

    /// <summary>
    /// 是否为服务商形态（官方请求体以 <c>sp_mchid</c> + <c>sub_mchid</c> 表达，<b>不发送</b> <c>mchid</c>）。
    /// </summary>
    /// <remarks>只表达「是否带 sp_/sub_ 前缀」这一判别式；「两字段是否成对」由 <see cref="Validate"/> 把关。</remarks>
    public bool IsServicePartner =>
        !string.IsNullOrWhiteSpace(SpMchId) || !string.IsNullOrWhiteSpace(SubMchId);

    /// <summary>
    /// 商户在 SDK 内的唯一键：普通商户取 <c>mchid</c>，服务商取 <c>{sp_mchid}:{sub_mchid}</c>。
    /// </summary>
    /// <remarks>
    /// 服务商子商户号<b>全局不唯一</b>（不同服务商下可重复），故必须与 <c>sp_mchid</c> 复合才唯一 ——
    /// 这也是本键不直接叫 <c>MchId</c> 的原因。回调侧按通知中的 <c>mchid</c> 查表即用本键。
    /// </remarks>
    public string MerchantKey => IsServicePartner ? $"{SpMchId}:{SubMchId}" : MchId;

    // ==================== 国密配置属性（方案 v1 §A.5-(4)）====================

    /// <summary>
    /// 签名算法选择：<c>SHA256-RSA2048</c>（默认）或 <c>SM2-WITH-SM3</c>（国密）。
    /// </summary>
    /// <remarks>
    /// <para><b>消费点</b>：<c>WechatPayAuthorizationHandler</c>（授权类型前缀）、<c>WechatPayResponseVerificationHandler</c>（缺头时的兜底算法）。</para>
    /// <para><b>向后兼容</b>：默认值 = 今日行为（RSA）；未开通国密的商户零影响。</para>
    /// </remarks>
    public string SignatureAlgorithm { get; set; } = "SHA256-RSA2048";

    /// <summary>
    /// 是否启用国密模式（<c>true</c> ⇒ 走 SM2 签名/验签 + SM4-GCM 解密）。
    /// </summary>
    /// <remarks>
    /// <para><b>消费点</b>：同上两处 handler；<c>false</c> ⇒ 走 Phase-1 RSA 全链路，行为与今日完全一致。</para>
    /// <para><b>前置条件</b>：须注册 <c>IWechatSmCryptoProvider</c> 真实实现，否则 fail-fast。</para>
    /// </remarks>
    public bool UseShangmi { get; set; } = false;

    /// <summary>
    /// <c>Authorization</c> 头中的 <c>mchid</c> 值（字段名照官方原文，不「纠正」）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 服务商形态填 <b><c>sp_mchid</c></b>：签名用的是<b>服务商</b>的 API 证书私钥与序列号
    /// （本配置对服务商只提供<b>一套</b> <see cref="PrivateKeySecretName"/>/<see cref="SerialNumber"/>，
    /// 即服务商自己的证书）；子商户<b>没有</b> API 证书，故不参与签名，<c>sub_mchid</c> 只出现在请求体。
    /// </para>
    /// <para>
    /// 这与官方 SDK 形态一致：<c>WechatPay2Credential</c> 构造时即绑定<b>单个</b> mchid。
    /// </para>
    /// </remarks>
    public string AuthorizationMchId => IsServicePartner ? SpMchId : MchId;

    /// <summary>
    /// 校验本商户配置（启动期调用，非法即抛，避免带着坏配置跑真实交易）。
    /// </summary>
    /// <exception cref="InvalidOperationException">配置非法时抛出。</exception>
    public void Validate()
    {
        ValidateIdentifier(MchId, nameof(MchId), mustBePresent: !IsServicePartner);
        ValidateIdentifier(SpMchId, nameof(SpMchId), mustBePresent: false);
        ValidateIdentifier(SubMchId, nameof(SubMchId), mustBePresent: false);

        if (IsServicePartner)
        {
            if (string.IsNullOrWhiteSpace(SpMchId))
            {
                throw new InvalidOperationException("服务商形态必须同时提供 SpMchId 与 SubMchId。");
            }

            if (string.IsNullOrWhiteSpace(SubMchId))
            {
                throw new InvalidOperationException("服务商形态必须同时提供 SpMchId 与 SubMchId。");
            }

            if (!string.IsNullOrWhiteSpace(MchId))
            {
                throw new InvalidOperationException(
                    $"服务商形态不得同时填写 MchId（当前 {MchId}）：" +
                    "官方服务商请求体以 sp_mchid + sub_mchid 表达、不发送 mchid，共存会令 DTO 映射无从择一。");
            }
        }

        if (string.IsNullOrWhiteSpace(SerialNumber))
        {
            throw new InvalidOperationException($"{MerchantKey}：SerialNumber（商户 API 证书序列号）不能为空。");
        }

        if (SerialNumber.Length > 128 || SerialNumber.Any(char.IsWhiteSpace))
        {
            throw new InvalidOperationException($"{MerchantKey}：SerialNumber 格式非法（含空白或超长）。");
        }

        ValidateSecretName(PrivateKeySecretName, nameof(PrivateKeySecretName), MerchantKey);
        ValidateSecretName(ApiKeySecretName, nameof(ApiKeySecretName), MerchantKey);

        // APIv2 三项一律**可选**（APIv3-only 商户留空即合法），但一旦填写就必须是「名字」而非密钥本身；
        // 口令名脱离证书名单独出现是配置错误（没人会用它），故要求成对。
        if (!string.IsNullOrWhiteSpace(ApiV2KeySecretName))
        {
            ValidateSecretName(ApiV2KeySecretName, nameof(ApiV2KeySecretName), MerchantKey);
        }

        if (!string.IsNullOrWhiteSpace(ApiV2CertificateSecretName))
        {
            ValidateSecretName(ApiV2CertificateSecretName, nameof(ApiV2CertificateSecretName), MerchantKey);
        }

        if (!string.IsNullOrWhiteSpace(ApiV2CertificatePasswordSecretName))
        {
            ValidateSecretName(ApiV2CertificatePasswordSecretName, nameof(ApiV2CertificatePasswordSecretName), MerchantKey);

            if (string.IsNullOrWhiteSpace(ApiV2CertificateSecretName))
            {
                throw new InvalidOperationException(
                    $"{MerchantKey}：配置了 {nameof(ApiV2CertificatePasswordSecretName)} 却没配 " +
                    $"{nameof(ApiV2CertificateSecretName)}：口令没有对应的证书材料可解，属配置残缺。");
            }
        }
    }

    /// <summary>
    /// 返回掩码后的配置描述（<b>密钥名只暴露前缀，密钥值永不出现</b>）。
    /// </summary>
    /// <returns>配置摘要文本。</returns>
    public override string ToString()
        => $"{nameof(WechatPayMerchantConfig)}(" +
           $"MerchantKey={MerchantKey}, " +
           $"SerialNumber={SerialNumber}, " +
           $"PrivateKeySecretName={Mask(PrivateKeySecretName)}, " +
           $"ApiKeySecretName={Mask(ApiKeySecretName)})";

    /// <summary>商户号类标识校验：必填判定 + 字符集收敛。</summary>
    /// <remarks>
    /// 字符集收敛到 <c>[A-Za-z0-9._-]</c>（与 <c>WechatAppKeyValidator</c> 同集合）的<b>唯一理由是防键别义</b>：
    /// <see cref="MerchantKey"/> 是字典键，若商户号里出现 <c>:</c>，两个不同商户组合会映射到同一键 ⇒ 凭据串号。
    /// 故此处不放宽（mchid 官方为纯数字，正常输入必通过）。
    /// </remarks>
    /// <remarks>
    /// 参数名刻意<b>不叫</b> <c>required</c>：该词是被 AB-G10 禁止的成员修饰符，用作标识符会让
    /// 守卫的源码扫描误报（守卫按 <c>\brequired\b</c> 匹配）。此处是布尔入参，不是成员修饰符。
    /// </remarks>
    private static void ValidateIdentifier(string value, string name, bool mustBePresent)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (mustBePresent)
            {
                throw new InvalidOperationException($"{name} 不能为空。");
            }

            return;
        }

        if (value.Length > 128)
        {
            throw new InvalidOperationException($"{name} 超过 128 字符。");
        }

        foreach (var c in value)
        {
            if (!(char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-'))
            {
                throw new InvalidOperationException(
                    $"{name} 含非法字符 '{c}'（仅允许字母、数字与 . _ -）。" +
                    "MerchantKey 是字典键，出现 ':' 等分隔符会造成键别义 ⇒ 跨商户凭据串号。");
            }
        }
    }

    /// <summary>
    /// 密钥名校验：必须是<b>名字</b>而不是密钥本身。
    /// </summary>
    /// <remarks>
    /// 这是最常见的配置事故 —— 字段叫 <c>PrivateKeySecretName</c>，却把 PEM 私钥整段贴了进去。
    /// 不拦的话，症状是运行期 <c>ISecretProvider</c> 查无此名、抛出一条「找不到名字 -----BEGIN ...」的
    /// 离奇错误，<b>并把私钥头带进异常消息</b>（等于把私钥洒进日志）。启动期直接点名比什么都清楚。
    /// </remarks>
    private static void ValidateSecretName(string value, string name, string merchantKey)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{merchantKey}：{name} 不能为空。");
        }

        // 「像密钥」的判定必须**先于**空白/长度判定：粘贴进来的 PEM 本身带换行，
        // 若先查空白，报错会变成无关的「含空白」，把真正的原因（填错成密钥值）盖掉。
        if (value.IndexOf("-----BEGIN", StringComparison.OrdinalIgnoreCase) >= 0 ||
            LooksLikeBase64KeyMaterial(value))
        {
            throw new InvalidOperationException(
                $"{merchantKey}：{name} 看起来填的是**密钥原文**而不是 ISecretProvider 中的密钥名。" +
                "私钥与 APIv3 密钥绝不入配置，只登记名字、运行期经 ISecretProvider 取用。");
        }

        if (value.Length > 256 || value.Any(char.IsWhiteSpace))
        {
            throw new InvalidOperationException(
                $"{merchantKey}：{name} 格式非法（密钥名不应含空白或超过 256 字符）。");
        }
    }

    /// <summary>
    /// 是否形如被误填进来的 RSA 密钥 Base64 体。
    /// </summary>
    /// <remarks>
    /// RSA 密钥/证书的 Base64 体恒以 <c>MII</c> 开头；配合<b>长度与字符集</b>联合判定，
    /// 既能抓住「把整段密钥贴进密钥名」，又不会误伤含 <c>MII</c> 子串的正常名字（如 <c>mii-service</c>）——
    /// <c>Validate()</c> 在启动期硬失败，误伤即部署阻断，故阈值宁可保守。
    /// </remarks>
    private static bool LooksLikeBase64KeyMaterial(string value)
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

    /// <summary>密钥名掩码（仅用于诊断文本）。</summary>
    private static string Mask(string value)
        => string.IsNullOrEmpty(value) ? "(未配置)" : value.Substring(0, Math.Min(8, value.Length)) + "***";
}
