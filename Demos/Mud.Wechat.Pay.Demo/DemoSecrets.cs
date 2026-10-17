// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.Demo.Shared;
using System.Text;

namespace Mud.Wechat.Pay.Demo;

/// <summary>
/// 宿主侧 <see cref="ISecretProvider"/> 实现：把「密钥名 → 密钥值」的映射交给环境变量或本机密钥文件。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么 Demo 也要自己实现这个端口</b>：<c>Mud.HttpUtils</c> 只定义端口、不提供实现，
/// 密钥治理权在宿主（生产上是 Vault / KMS / 加密配置中心）。支付线的商户配置
/// <c>WechatPayMerchantConfig</c> 里只能出现<b>密钥名</b>，把 PEM 或 APIv3 密钥贴进配置会在启动期
/// 被 <c>Validate()</c> 直接拒绝（<c>PAY-B7</c> 的配置面防线）。
/// </para>
/// <para>
/// <b>未登记的名字返回空串</b>：SDK 把 <c>string.IsNullOrEmpty</c> 判为「查无此名」并抛点名错误
/// （消息里只截断密钥名、绝不带值），故空串与 <c>null</c> 对该判定器完全等价；
/// 端口签名是 <c>Task&lt;string?&gt;</c>，实现侧照写，不做「反正都是空」的类型游戏。
/// </para>
/// <para>
/// <b>值绝不出现在任何日志、异常消息或 <see cref="object.ToString"/> 里</b>：类型刻意不打印字典内容，
/// <see cref="SecretNames()"/> 只暴露名字。
/// </para>
/// </remarks>
internal sealed class DemoSecretProvider : ISecretProvider
{
    private readonly IReadOnlyDictionary<string, string> _secrets;

    /// <summary>以「密钥名 → 密钥值」映射创建。</summary>
    /// <param name="secrets">映射（大小写敏感，与 SDK 取用时的名字同一口径）。</param>
    public DemoSecretProvider(IReadOnlyDictionary<string, string> secrets)
        => _secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));

    /// <summary>已登记的密钥名（诊断用，不含值）。</summary>
    /// <returns>密钥名集合。</returns>
    public IEnumerable<string> SecretNames() => _secrets.Keys;

    /// <inheritdoc />
    public Task<string?> GetSecretAsync(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return Task.FromResult<string?>(_secrets.TryGetValue(name, out var value) ? value : string.Empty);
    }

    /// <inheritdoc />
    /// <remarks>覆写只为「误把本类型写进日志」这一路径兜底：只输出名字集合。</remarks>
    public override string ToString()
        => $"{nameof(DemoSecretProvider)}(names: {string.Join(", ", _secrets.Keys)})";
}

/// <summary>
/// Demo 侧密钥装配：环境变量名、密钥名常量与「裸 Base64 私钥 → PKCS#8 PEM」归一。
/// </summary>
internal static class DemoSecrets
{
    /// <summary>商户私钥在 <see cref="DemoSecretProvider"/> 中的名字（写进 <c>PrivateKeySecretName</c>）。</summary>
    public const string PrivateKeyName = "pay:private-key";

    /// <summary>APIv3 密钥在 <see cref="DemoSecretProvider"/> 中的名字（写进 <c>ApiKeySecretName</c>）。</summary>
    public const string ApiV3KeyName = "pay:api-v3-key";

    /// <summary>
    /// 从环境读取两份密钥值：私钥取 <c>PAY_PRIVATE_KEY</c>（PEM 或裸 Base64）或 <c>PAY_PRIVATE_KEY_PATH</c>
    /// （指向 <c>apiclient_key.pem</c>），二者必得其一；APIv3 密钥取 <c>PAY_API_V3_KEY</c>。
    /// </summary>
    /// <returns>密钥名 → 密钥值。</returns>
    /// <exception cref="InvalidOperationException">两份私钥来源都缺失时抛出（一次性点名两个变量名）。</exception>
    /// <remarks>
    /// 与 <c>DemoEnv.RequireAll</c> 的分工：其余必填项走聚合校验，此处需要「二选一」语义 ——
    /// <c>RequireAll</c> 的语义是「全部必填」，套上来会变成「两个变量都得配」。
    /// </remarks>
    public static IReadOnlyDictionary<string, string> ReadFromEnvironment()
    {
        var inlineKey = DemoEnv.Optional("PAY_PRIVATE_KEY");
        var keyPath = DemoEnv.Optional("PAY_PRIVATE_KEY_PATH");

        if (inlineKey == null && keyPath == null)
        {
            throw new InvalidOperationException(
                "缺少商户 API 证书私钥：请在 "
                + $"{DemoEnv.FullName("PAY_PRIVATE_KEY")}（PEM 原文）或 "
                + $"{DemoEnv.FullName("PAY_PRIVATE_KEY_PATH")}（apiclient_key.pem 路径）中配置其一。"
                + " 参见 Demos/README.md 的凭据矩阵。");
        }

        var pem = inlineKey ?? File.ReadAllText(keyPath!);

        var apiV3Key = DemoEnv.Require("PAY_API_V3_KEY");

        // 字典按 Ordinal：密钥名是 ISecretProvider 的键，大小写敏感性必须与 SDK 取用侧一致。
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [PrivateKeyName] = NormalizePrivateKeyPem(pem),
            [ApiV3KeyName] = apiV3Key,
        };
    }

    /// <summary>
    /// 把「裸 Base64 私钥」补成 <c>ImportFromPem</c> 认得的 PKCS#8 PEM；已带 PEM 头的原样返回。
    /// </summary>
    /// <param name="value">环境变量或文件里的私钥值。</param>
    /// <returns>PEM 文本。</returns>
    /// <remarks>
    /// <b>为什么要归一</b>：<c>WechatPayMerchantCredentialProvider.LoadPrivateKeyAsync</c> 直接用
    /// <c>RSA.ImportFromPem</c>，它对「没有 <c>-----BEGIN PRIVATE KEY-----</c> 包围」的输入必然抛
    /// <c>FormatException</c>；而运维把 <c>apiclient_key.pem</c> 的内容贴进环境变量时，
    /// 最常见的形态就是只贴中间那一段。这属于<b>宿主的密钥治理</b>（端口正是为此留给宿主的），
    /// SDK 侧刻意不做此兜底 —— 在 SDK 里猜格式会把「配置错误」变成「看起来能用」。
    /// </remarks>
    private static string NormalizePrivateKeyPem(string value)
    {
        if (value.Contains("-----BEGIN", StringComparison.Ordinal))
        {
            return value;
        }

        var base64 = new string(value.Where(static c => !char.IsWhiteSpace(c)).ToArray());
        var builder = new StringBuilder();
        builder.AppendLine("-----BEGIN PRIVATE KEY-----");
        for (var i = 0; i < base64.Length; i += 64)
        {
            builder.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
        }

        builder.Append("-----END PRIVATE KEY-----");
        return builder.ToString();
    }
}
