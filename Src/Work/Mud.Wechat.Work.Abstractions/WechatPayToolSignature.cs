// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions;

/// <summary>
/// 收银台签名算法（官方 <see href="https://developer.work.weixin.qq.com/document/path/98768">path 98768 签名算法</see>）：
/// 非空参数构造成 <c>key=value</c> 集合 → 按 ASCII 字典序排序 → 以 <c>&amp;</c> 拼接为
/// <c>stringA</c> → 以收银台 API 调用密钥为 key 做 <c>HMAC-SHA256</c> → <c>Base64</c> 编码得 <c>sig</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>密钥获取路径</b>：工作台 → 企业微信服务商助手 → 工具 → 收银台 → 收银台 API 调用密钥。
/// 本类型<b>只做纯计算</b>，不托管密钥：调用方自行持有密钥并显式传入，
/// 以免密钥成为无消费点的配置属性。
/// </para>
/// <para>
/// <b>官方规则逐条落地</b>：
/// ① 键值对整体按 ASCII 码从小到大排序（字典序，区分大小写）；
/// ② 参数值为空（<see langword="null"/> 或空串）不参与签名；
/// ③ 传送的 <c>sig</c> 参数不参与签名；
/// ④ 接口可能增加字段，验证签名时必须支持增加的扩展字段
/// （本实现不维护白名单，未知字段一律参与签名）；
/// ⑤ 节点是元组（嵌套对象 / 对象数组）时不直接参与签名，递归地用其子节点签名。
/// </para>
/// <para>
/// <b>签名时效</b>：<c>nonce_str</c> 长度须在 32 字节以内且 15 分钟内不重复；
/// <c>ts</c> 为 unix 时间戳（中国时区、精确到秒），业务系统机器时间与腾讯的时间相差不能超过 15 分钟。
/// </para>
/// <para>
/// <b>适用范围</b>：收银台「收款工具」族 4 个端点（open_order / close_order / get_order_list /
/// get_order_detail）必须签名；同域「发票管理」族与「应用版本付费」族的官方参数表<b>不含</b>签名三要素，勿附加。
/// </para>
/// </remarks>
public static class WechatPayToolSignature
{
    /// <summary>不参与签名的保留字段名（官方：传送的 sig 参数不参与签名）。</summary>
    private const string SignatureFieldName = "sig";

    /// <summary>
    /// 按官方签名算法计算 <c>sig</c>（入参为扁平键值对，请先经 <see cref="Flatten"/> 摊平嵌套节点）。
    /// </summary>
    /// <param name="secret">收银台 API 调用密钥（工作台 → 企业微信服务商助手 → 工具 → 收银台 → 收银台 API 调用密钥）。</param>
    /// <param name="parameters">参与签名的参数键值对（可含空值与 <c>sig</c>，二者会被剔除）。</param>
    /// <returns>Base64 编码的 <c>HMAC-SHA256</c> 签名值。</returns>
    /// <exception cref="ArgumentException">密钥为空白。</exception>
    public static string Sign(string secret, IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        if (secret == null)
        {
            throw new ArgumentNullException(nameof(secret));
        }

        // netstandard2.0 无 string.IsNullOrWhiteSpace 的 [NotNullWhen] 标注，须显式判空。
        if (secret.Trim().Length == 0)
        {
            throw new ArgumentException("收银台 API 调用密钥不可为空白。", nameof(secret));
        }

        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        return ComputeHmacSha256(secret, BuildSignString(parameters));
    }

    /// <summary>
    /// 按官方签名算法计算 <c>sig</c>（自动递归摊平嵌套节点，等价于 <c>Sign(secret, Flatten(parameters))</c>）。
    /// </summary>
    /// <param name="secret">收银台 API 调用密钥。</param>
    /// <param name="parameters">
    /// 参与签名的参数；值支持字符串、数值/布尔等标量（按不变文化格式化）、
    /// 嵌套 IDictionary&lt;string, object?&gt; 与对象数组（元素须为 IDictionary&lt;string, object?&gt; 形态）。
    /// </param>
    /// <returns>Base64 编码的 <c>HMAC-SHA256</c> 签名值。</returns>
    /// <exception cref="ArgumentException">密钥为空白，或出现 SDK 无法摊平的节点形态。</exception>
    public static string SignNested(string secret, IEnumerable<KeyValuePair<string, object?>> parameters)
        => Sign(secret, Flatten(parameters));

    /// <summary>
    /// 构造官方定义、参与签名的 <c>stringA</c>：非空、非 <c>sig</c> 的键值对按 ASCII 字典序排序后以 <c>&amp;</c> 拼接。
    /// </summary>
    /// <param name="parameters">参与签名的参数键值对（可含空值与 <c>sig</c>，二者会被剔除）。</param>
    /// <returns>签名原文 <c>stringA</c>；无任何参与字段时返回空串。</returns>
    public static string BuildSignString(IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        // OrderBy 为稳定排序：官方示例 2 中同名键（credit_orderid 等）可重复出现，
        // 排序须保留传入的相对次序，不得按 key 去重。
        // 官方规则「如果参数的值为空不参与签名」：null 与空串均视为空，一律剔除。
        var ordered = parameters
            .Where(p => p.Key != null && p.Key.Length > 0 && !string.Equals(p.Key, SignatureFieldName, StringComparison.Ordinal))
            .Where(p => !string.IsNullOrEmpty(p.Value))
            .Select(p => new KeyValuePair<string, string>(p.Key, p.Value!))
            .OrderBy(p => p.Key, StringComparer.Ordinal);

        return string.Join("&", ordered.Select(p => p.Key + "=" + p.Value));
    }

    /// <summary>
    /// 递归摊平嵌套节点（元组以子节点参与签名，官方 98768 规则），
    /// 输出可直接交给 <c>Sign</c> 的扁平键值对。
    /// </summary>
    /// <param name="parameters">含嵌套结构的参数键值对。</param>
    /// <returns>扁平化后的键值对（保持传入顺序，不排序；同名键保留重复项）。</returns>
    /// <exception cref="ArgumentException">出现 SDK 无法摊平的节点形态。</exception>
    public static List<KeyValuePair<string, string?>> Flatten(IEnumerable<KeyValuePair<string, object?>> parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        var result = new List<KeyValuePair<string, string?>>();
        foreach (var pair in parameters)
        {
            FlattenInto(pair.Key, pair.Value, result);
        }

        return result;
    }

    /// <summary>摊平单个节点到目标集合。</summary>
    private static void FlattenInto(string? key, object? value, ICollection<KeyValuePair<string, string?>> target)
    {
        if (key == null || key.Length == 0 || value == null)
        {
            return;
        }

        if (value is string text)
        {
            AddScalar(target, key, text);
            return;
        }

        if (value is IDictionary<string, object?> nested)
        {
            foreach (var item in nested)
            {
                FlattenInto(item.Key, item.Value, target);
            }

            return;
        }

        // 元组（对象数组）：官方示例 2 中 credit_order_list 不直接参与签名，而以其子节点参与。
        if (value is System.Collections.IEnumerable sequence)
        {
            foreach (var item in sequence)
            {
                if (item is IDictionary<string, object?> itemMap)
                {
                    foreach (var pair in itemMap)
                    {
                        FlattenInto(pair.Key, pair.Value, target);
                    }

                    continue;
                }

                throw new ArgumentException(
                    $"收银台签名节点 '{key}' 的数组元素须为 IDictionary<string, object?> 形态（实际：{item?.GetType().FullName ?? "null"}）。",
                    nameof(key));
            }

            return;
        }

        if (value is IFormattable formattable)
        {
            AddScalar(target, key, formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture));
            return;
        }

        if (value is bool flag)
        {
            AddScalar(target, key, flag ? "true" : "false");
            return;
        }

        AddScalar(target, key, value.ToString());
    }

    /// <summary>追加标量键值对（官方：值为空不参与签名）。</summary>
    private static void AddScalar(ICollection<KeyValuePair<string, string?>> target, string key, string? text)
    {
        if (text == null || text.Length == 0)
        {
            return;
        }

        target.Add(new KeyValuePair<string, string?>(key, text));
    }

    /// <summary>以收银台支付密钥为 key 做 HMAC-SHA256 并 Base64 编码（官方签名算法第二步）。</summary>
    private static string ComputeHmacSha256(string secret, string signString)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret)))
        {
            return Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signString)));
        }
    }
}