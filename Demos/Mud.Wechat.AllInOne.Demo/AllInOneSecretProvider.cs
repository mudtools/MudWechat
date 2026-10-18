// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using Mud.HttpUtils;
using Mud.Wechat.Demo.Shared;

namespace Mud.Wechat.AllInOne.Demo;

/// <summary>
/// 宿主侧密钥端口实现（支付线）：配置对象里只出现<b>密钥名</b>，值在运行期由这里给出。
/// </summary>
/// <remarks>
/// <para>
/// <b><c>AddPayApp</c> 刻意不代注册本端口</b>：密钥治理权在宿主（生产上是 Vault / KMS / 加密配置中心），
/// 漏注册时首次取用凭据即给出点名错误。这也是支付线与其余六线最大的装配差异 —— 它没有
/// <c>access_token</c>，凭据是商户私钥对<b>每个请求</b>的 RSA-SHA256 签名。
/// </para>
/// <para>
/// <b>本 Demo 允许「一名不存」</b>：DM-06 要求私钥齐备才起进程，因为它的下单/退款要真签名；
/// 本宿主只挂通知接收，私钥缺席时验签必然失败（401/403），这正是「这条线活着但凭据不足」的诚实形态。
/// 故 <c>PAY_PRIVATE_KEY</c> / <c>PAY_API_V3_KEY</c> 在这里是<b>可选</b>的，读不到就留空字典。
/// </para>
/// <para>
/// <b>值绝不出现在日志、异常消息或 <see cref="object.ToString"/> 里</b>：<see cref="SecretNames"/> 只暴露名字；
/// 未登记的名字返回空串（SDK 把 <c>IsNullOrEmpty</c> 判为「查无此名」）。
/// </para>
/// </remarks>
internal sealed class AllInOneSecretProvider : ISecretProvider
{
    /// <summary>商户私钥在端口里的名字（写进 <c>WechatPayMerchantConfig.PrivateKeySecretName</c>）。</summary>
    public const string PrivateKeyName = "pay:private-key";

    /// <summary>APIv3 密钥在端口里的名字（写进 <c>ApiKeySecretName</c>）。</summary>
    public const string ApiV3KeyName = "pay:api-v3-key";

    private readonly IReadOnlyDictionary<string, string> _secrets;

    public AllInOneSecretProvider(IReadOnlyDictionary<string, string> secrets)
        => _secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));

    /// <summary>已登记的密钥名（诊断用，不含值）。</summary>
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
    /// <remarks>覆写只为「误把本类型写进日志」兜底：只输出名字集合。</remarks>
    public override string ToString()
        => $"{nameof(AllInOneSecretProvider)}(names: {string.Join(", ", _secrets.Keys)})";

    /// <summary>从环境读取两份密钥值；缺失即跳过（见类型注释里的「允许一名不存」）。</summary>
    public static IReadOnlyDictionary<string, string> ReadOptionalFromEnvironment()
    {
        var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);

        // 私钥二选一：内联 PEM（或裸 Base64）/ 指向 apiclient_key.pem 的路径。
        var inlineKey = DemoEnv.Optional("PAY_PRIVATE_KEY");
        var keyPath = DemoEnv.Optional("PAY_PRIVATE_KEY_PATH");
        if (inlineKey != null)
        {
            dictionary[AllInOneSecretProvider.PrivateKeyName] = NormalizePrivateKeyPem(inlineKey);
        }
        else if (keyPath != null)
        {
            dictionary[AllInOneSecretProvider.PrivateKeyName] = NormalizePrivateKeyPem(File.ReadAllText(keyPath));
        }

        var apiV3Key = DemoEnv.Optional("PAY_API_V3_KEY");
        if (apiV3Key != null)
        {
            dictionary[AllInOneSecretProvider.ApiV3KeyName] = apiV3Key;
        }

        return dictionary;
    }

    /// <summary>
    /// 把「裸 Base64 私钥」补成 <c>RSA.ImportFromPem</c> 认得的 PKCS#8 PEM；已带 PEM 头的原样返回。
    /// </summary>
    /// <remarks>
    /// 运维把 <c>apiclient_key.pem</c> 贴进环境变量时最常见的形态就是只贴中间那段；
    /// <c>ImportFromPem</c> 对无 <c>-----BEGIN PRIVATE KEY-----</c> 包围的输入必然抛 <c>FormatException</c>。
    /// 这属<b>宿主的密钥治理</b>（端口正是留给宿主的），SDK 侧刻意不猜格式。
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
