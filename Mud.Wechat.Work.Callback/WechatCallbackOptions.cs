// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调接收配置（回调运维面，对应 Mud.Feishu.Webhook 的配置包；v1 方案 §5.2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>多应用（v1.2 D3）</b>：回调凭据以 <see cref="Apps"/> 字典为<b>唯一来源</b>——键为
/// <see cref="WechatAppConfig.AppKey"/> 形状的应用键（路由 <c>/wechat/{AppKey}</c>），通配
/// <see cref="WildcardAppKey"/> 承接「通讯录同步助手」（无 AppKey 形态）与「全局」处理器注册（D11）。
/// 项目未发布，旧的单体顶层字段（PushToken/PushEncodingAESKey/CorpId）已删除，不做兼容垫片。
/// </para>
/// <para>
/// 原主配置中的 <c>PushEncodingAESKey</c> / <c>PushToken</c> 迁入本配置
/// （回调推送密钥属回调运维面，不进主配置，见产品规划 §7.1 / 详细设计 §8.4）。
/// 应用级配置类 <see cref="WechatAppCallbackOptions"/> 与本类<b>同类文件</b>放置——
/// <c>audit-config-keys.ps1</c> 按文件扫描配置属性消费点。
/// </para>
/// </remarks>
public class WechatCallbackOptions
{
    /// <summary>
    /// 通配应用键：承接通讯录同步助手（无 AppKey 形态）的回调路由，同时作为「全局」处理器/拦截器的注册桶（D11）。
    /// </summary>
    /// <remarks>
    /// <c>"*"</c> 不满足 <see cref="WechatAppKeyValidator"/> 的应用键形状（首字符须字母/数字），
    /// <see cref="Validate"/> 对其显式豁免——也正因形状非法，通配键与任何真实应用键不可能碰撞。
    /// </remarks>
    public const string WildcardAppKey = "*";

    /// <summary>
    /// 回调路由前缀（中间件路径形如 <c>/{GlobalRoutePrefix}/{AppKey}</c>）。
    /// </summary>
    /// <remarks>消费点：<c>WechatCallbackMiddleware</c> 路径提取。</remarks>
    public string GlobalRoutePrefix { get; set; } = "wechat";

    /// <summary>
    /// 回调请求体最大字节数（逐块按字节计数校验，防多字节字符按字符计数超限）。
    /// </summary>
    /// <remarks>消费点：<c>WechatCallbackMiddleware</c> 请求体读取；超限返回 413。</remarks>
    public int MaxRequestBodySize { get; set; } = 1_048_576;

    /// <summary>
    /// 回调来源 IP 白名单（空 = 不限制）；条目支持精确 IP 与 IPv4 CIDR 段
    /// （如 <c>101.226.103.0/24</c>，段值可经官方 <c>getcallbackip</c> 接口获取）。
    /// </summary>
    /// <remarks>消费点：<c>WechatCallbackMiddleware</c> 前置校验；不在列返回 403。</remarks>
    public List<string> AllowedSourceIPs { get; set; } = new();

    /// <summary>
    /// 事件分发软超时（毫秒）。<b>必须小于企业微信 5 秒应答契约</b>（默认 4500ms）：
    /// 超时以 503 应答触发企业微信重推（v1.2 D7；v1.1 的 30s 默认值永远赶不上重推窗口）。
    /// </summary>
    /// <remarks>消费点：<c>WechatCallbackDispatcher</c> 软超时 CTS。</remarks>
    public int EventHandlingTimeoutMs { get; set; } = 4_500;

    /// <summary>
    /// 事件分发最大并发数（信号量容量；构造期取值，运行期热更不改变已建容量，v1 方案 §10.8）。
    /// </summary>
    /// <remarks>消费点：<c>WechatCallbackDispatcher</c> 并发闸。</remarks>
    public int MaxConcurrentEvents { get; set; } = 10;

    /// <summary>
    /// 多应用回调凭据（键 = 应用键 <see cref="WechatAppConfig.AppKey"/> 或通配 <see cref="WildcardAppKey"/>）。
    /// </summary>
    /// <remarks>
    /// 消费点：接收器按传入 appKey 解析凭据（精确键优先，回退通配键）、中间件路由匹配、
    /// <see cref="Validate"/> 启动期/请求期校验。<b>回调凭据唯一来源</b>——不回流 <see cref="WechatAppConfig"/>（CB5 守卫）。
    /// </remarks>
    public Dictionary<string, WechatAppCallbackOptions> Apps { get; set; } = new();

    /// <summary>
    /// 解析指定应用键的回调凭据（精确键优先，回退通配 <see cref="WildcardAppKey"/>）。
    /// </summary>
    /// <param name="appKey">应用键（来自回调路由路径段）。</param>
    /// <returns>命中的应用凭据；未命中返回 <c>null</c>（调用方按「未知应用」处置）。</returns>
    public WechatAppCallbackOptions? ResolveApp(string appKey)
    {
        if (!string.IsNullOrEmpty(appKey) && Apps.TryGetValue(appKey, out var app))
        {
            return app;
        }

        return Apps.TryGetValue(WildcardAppKey, out var wildcard) ? wildcard : null;
    }

    /// <summary>
    /// 校验回调配置完整性（缺失必填项抛出 <see cref="InvalidOperationException"/>）。
    /// </summary>
    /// <remarks>
    /// 请求期由接收器对<b>命中应用</b>做单应用校验（避免一个应用的配置错误拖垮全部回调路由）；
    /// 本方法为启动期全量校验入口（宿主可显式调用）。
    /// </remarks>
    public void Validate()
    {
        if (Apps.Count == 0)
        {
            throw new InvalidOperationException(
                $"回调配置缺少 Apps（至少配置一个应用的回调凭据；通讯录同步助手使用通配 \"{WildcardAppKey}\" 键）。");
        }

        foreach (var pair in Apps)
        {
            if (pair.Key != WildcardAppKey)
            {
                WechatAppKeyValidator.Validate(pair.Key);
            }

            if (pair.Value == null)
            {
                throw new InvalidOperationException($"回调配置 Apps[\"{pair.Key}\"] 为 null。");
            }

            pair.Value.Validate(pair.Key);
        }
    }
}

/// <summary>
/// 单个应用的回调凭据（v1 方案 §5.2；<b>无 AppKey 属性</b>——字典键即路由与诊断唯一权威，v1.2 CB5）。
/// </summary>
public class WechatAppCallbackOptions
{
    /// <summary>
    /// 回调推送加解密 Token（URL 参数 msg_signature 验签密钥）。
    /// </summary>
    /// <remarks>消费点：<c>WechatCallbackReceiver</c> 验签（<see cref="WechatCallbackCrypto.VerifySignature"/>）。</remarks>
    public string PushToken { get; set; } = string.Empty;

    /// <summary>
    /// 回调消息加解密密钥 EncodingAESKey（43 位字符，AES-256-CBC）。
    /// </summary>
    /// <remarks>消费点：<c>WechatCallbackReceiver</c> 解密/加密（<see cref="WechatCallbackCrypto.Decrypt"/>）。</remarks>
    public string PushEncodingAESKey { get; set; } = string.Empty;

    /// <summary>
    /// 回调报文的<b>接收方 ID</b>（解密明文尾部 <c>receiveid</c>，参与明文完整性校验）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 语义按回调形态区分（v1 方案 §5.2「接收方形态」）：<b>企业自建应用回调</b>为企业 <c>CorpId</c>；
    /// <b>第三方应用 / 服务商代开发的套件回调</b>为 <c>SuiteId</c>；<b>通讯录同步助手</b>（通配键）
    /// 可留空（密文 receiveid 可能为空）。
    /// </para>
    /// <para>
    /// 消费点：非空时接收器会校验解密明文的 <c>receiveid</c> 与本值一致，不一致即拒绝；
    /// 留空则跳过校验并输出一次性告警。
    /// </para>
    /// </remarks>
    public string CorpId { get; set; } = string.Empty;

    /// <summary>
    /// 校验单应用回调凭据完整性（缺失必填项抛出 <see cref="InvalidOperationException"/>）。
    /// </summary>
    /// <param name="appKey">归属应用键（仅用于异常消息定位）。</param>
    public void Validate(string appKey)
    {
        if (string.IsNullOrWhiteSpace(PushToken))
        {
            throw new InvalidOperationException($"回调配置 Apps[\"{appKey}\"] 缺少 PushToken。");
        }

        if (string.IsNullOrWhiteSpace(PushEncodingAESKey) || PushEncodingAESKey.Length != 43)
        {
            throw new InvalidOperationException($"回调配置 Apps[\"{appKey}\"] 的 PushEncodingAESKey 必须为 43 位字符。");
        }
    }
}
