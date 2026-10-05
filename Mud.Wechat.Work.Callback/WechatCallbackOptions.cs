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
/// 经 <see cref="WechatAppKeyValidator"/> 形状校验的应用键（路由 <c>/wechat/{AppKey}</c>），通配
/// <see cref="WildcardAppKey"/> 承接「通讯录同步助手」（无 AppKey 形态）与「全局」处理器注册（D11）。
/// 项目未发布，旧的单体顶层字段（PushToken/PushEncodingAESKey/CorpId）已删除，不做兼容垫片。
/// </para>
/// <para>
/// <b>多套件（第三方应用 / 服务商代开发）</b>：每个套件（或企业自建应用）各登记一个 <see cref="Apps"/> 条目——
/// 多套件宿主在 <c>AddWechatCallback</c> 的配置委托里逐套件写入独立 Token / AESKey / 接收方 ID，
/// 路由由中间件按 <c>/{GlobalRoutePrefix}/{AppKey}</c> 路径段选取；<b>无单/多套件模式之分</b>。
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
    /// 多应用回调凭据（键 = 应用键 <c>AppKey</c>（形状经 <see cref="WechatAppKeyValidator"/> 校验）
    /// 或通配 <see cref="WildcardAppKey"/>）。
    /// </summary>
    /// <remarks>
    /// 消费点：接收器按传入 appKey 解析凭据（精确键优先，回退通配键）、中间件路由匹配、
    /// <see cref="Validate"/> 启动期/请求期校验。<b>回调凭据唯一来源</b>——不回流
    /// <see cref="Mud.Wechat.Work.Abstractions.Configuration.WechatAppConfig"/>（CB5 守卫）。
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
    /// <remarks>消费点：<c>WechatCallbackReceiver</c> 解密/加密
    /// （<see cref="WechatCallbackCrypto.Decrypt(string, string, out string)"/>）。</remarks>
    public string PushEncodingAESKey { get; set; } = string.Empty;

    /// <summary>
    /// 回调条目对应的<b>应用类型</b>（企业自建应用 / 第三方应用 / 服务商代开发）。
    /// </summary>
    /// <remarks>
    /// 消费点：接收器 <c>ValidateReceiveId</c> 按「AppType + Channel」组合选择 receiveid 校验语义
    /// （自建静态 CorpId / 第三方·代开发套件静态 SuiteId / 第三方·代开发数据动态授权企业 CorpId）；
    /// <see cref="Validate"/> 校验类型/通道组合是否合法。默认 <see cref="WechatAppType.Internal"/>
    /// 与既有「企业自建回调」配置保持向后兼容。
    /// </remarks>
    public WechatAppType AppType { get; set; } = WechatAppType.Internal;

    /// <summary>
    /// 回调条目承载的<b>回调通道</b>（应用数据事件 / 套件指令票据）。
    /// </summary>
    /// <remarks>
    /// 消费点：接收器 <c>ValidateReceiveId</c> 判别动态/静态 receiveid 校验；<see cref="Validate"/>
    /// 校验「自建应用不得占用套件通道」的非法状态。默认 <see cref="WechatCallbackChannel.App"/>
    /// 与既有「企业自建应用数据回调」配置保持向后兼容。
    /// </remarks>
    public WechatCallbackChannel Channel { get; set; } = WechatCallbackChannel.App;

    /// <summary>
    /// 回调报文的<b>接收方 ID</b>（解密明文尾部 <c>receiveid</c>，参与明文完整性校验）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 语义按「应用类型 × 回调通道」区分（v1 方案 §5.2「接收方形态」）：
    /// <b>企业自建应用</b>（App 通道）为企业 <c>CorpId</c>；
    /// <b>第三方应用 / 服务商代开发的套件通道</b>（Suite）为 <c>SuiteId</c>；
    /// <b>第三方应用 / 服务商代开发的数据通道</b>（App）为<b>动态授权企业 CorpId</b>——随授权企业变化，
    /// 不能静态预置，留空且由接收器改为比对解密明文外层 <c>ToUserName</c>（授权企业 CorpId）；
    /// <b>通讯录同步助手</b>（通配键）可留空（密文 receiveid 可能为空）。
    /// </para>
    /// <para>
    /// 消费点：非空时接收器校验解密明文的 <c>receiveid</c> 与本值一致，不一致即拒绝；
    /// 留空则跳过静态校验（自建/套件通道输出一次性告警，第三方·代开发数据通道静默走动态校验）；
    /// <b>明文未携带 receiveid</b> 时同样跳过校验（兼容官方「个人主体第三方为空串」形态，90968）。
    /// </para>
    /// </remarks>
    public string ReceiveId { get; set; } = string.Empty;

    /// <summary>
    /// 校验单应用回调凭据完整性（缺失必填项或类型/通道组合非法时抛出 <see cref="InvalidOperationException"/>）。
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

        // 通配键（通讯录同步助手 / 全局桶）：无类型·通道语义，ReceiveId 恒可选，跳过类型/通道校验。
        if (appKey == WechatCallbackOptions.WildcardAppKey)
        {
            return;
        }

        // 智能机器人通道（Bot，v1.2）：官方 101033 明确「企业内部智能机器人场景 ReceiveId 为 ""」⇒
        // 该通道 receiveid 恒为空串，配置非空即为非法状态（不可表达）；且官方文档树仅自建开放。
        if (Channel == WechatCallbackChannel.Bot)
        {
            if (AppType != WechatAppType.Internal)
            {
                throw new InvalidOperationException(
                    $"回调配置 Apps[\"{appKey}\"] 的智能机器人通道（Channel=Bot）仅企业自建应用可用" +
                    $"（官方智能机器人文档树位于「企业自建应用开发」分类下，无第三方 / 服务商代开发开放面）。");
            }

            if (!string.IsNullOrEmpty(ReceiveId))
            {
                throw new InvalidOperationException(
                    $"回调配置 Apps[\"{appKey}\"] 的智能机器人通道（Channel=Bot）不得配置 ReceiveId：" +
                    "官方 101033 明确企业内部智能机器人场景 receiveid 恒为\"\"（空字符串）。");
            }

            return;
        }

        // 企业自建应用只存在「应用数据回调」，不拥有套件指令/票据回调（suite_ticket 等仅服务商形态）。
        if (AppType == WechatAppType.Internal && Channel != WechatCallbackChannel.App)
        {
            throw new InvalidOperationException(
                $"回调配置 Apps[\"{appKey}\"] 的企业自建应用（AppType=Internal）不能配置套件通道（Channel=Suite）。");
        }

        // 套件通道是第三方应用 / 服务商代开发的专属形态（指令回调 URL 承载 suite_ticket 与授权族事件）。
        if (Channel == WechatCallbackChannel.Suite &&
            AppType != WechatAppType.ThirdParty && AppType != WechatAppType.Provider)
        {
            throw new InvalidOperationException(
                $"回调配置 Apps[\"{appKey}\"] 的套件通道（Channel=Suite）仅第三方应用 / 服务商代开发可用。");
        }
    }

    /// <summary>
    /// 判定事件族是否对当前「应用类型 × 回调通道」合法（开放面闸，v1 方案 §5.4.3 决策表）。
    /// </summary>
    /// <param name="family">事件族（<see cref="WechatCallbackEventFamily"/>，由 <c>WechatCallbackEvent.EventFamily</c> 判别）。</param>
    /// <returns><c>true</c> = 许可分发；<c>false</c> = 不适用于当前回调条目，分发器拒绝（返回 200，不触发企业微信重推）。</returns>
    /// <remarks>
    /// <para>官方开放面：</para>
    /// <list type="bullet">
    /// <item><description>授权族（suite_ticket / 授权通知）仅<b>套件通道</b>——企业自建无套件指令回调；</description></item>
    /// <item><description>上下游变更族仅<b>自建应用 + 应用数据通道</b>（官方 95796：第三方/代开发暂不支持）；</description></item>
    /// <item><description>通讯录变更族 / 异步任务族经<b>应用数据通道</b>承载（三类应用均开放）；</description></item>
    /// <item><description>客户联系/获客助手族：<b>自建·代开发 × 应用数据通道</b>（Event 信封）+
    /// <b>第三方 × 套件指令通道</b>（指令回调 URL，InfoType 信封，官方 92277/97402/99485）；</description></item>
    /// <item><description>无法判别的族不拦截（协议外报文交由兜底处理器自行处置）。</description></item>
    /// </list>
    /// </remarks>
    public bool IsEventFamilyAllowed(WechatCallbackEventFamily family)
    {
        // 智能机器人通道（Bot）：报文为 JSON 且不承载 XML 事件信封 ⇒ 本闸恒拒绝（显式默认拒绝，
        // 防止新增枚举值在 switch 中落 default 分支被 fail-open 放行）。Bot 事件由其自身分发面判定。
        if (Channel == WechatCallbackChannel.Bot)
        {
            return false;
        }

        switch (family)
        {
            case WechatCallbackEventFamily.Authorization:
                // 授权族仅第三方/代开发的套件通道（suite_ticket / create_auth / change_auth / cancel_auth / del_auth）。
                return Channel == WechatCallbackChannel.Suite &&
                       (AppType == WechatAppType.ThirdParty || AppType == WechatAppType.Provider);

            case WechatCallbackEventFamily.ChainChange:
                // 上下游变更族（change_chain）官方仅向自建应用开放（配置到「上下游-可调用接口的应用」）。
                return Channel == WechatCallbackChannel.App && AppType == WechatAppType.Internal;

            case WechatCallbackEventFamily.ContactChange:
            case WechatCallbackEventFamily.BatchJob:
                // 通讯录变更族与异步任务族经应用数据回调 URL 承载，三类应用均开放。
                return Channel == WechatCallbackChannel.App;

            case WechatCallbackEventFamily.ExternalContactChange:
            case WechatCallbackEventFamily.CustomerAcquisition:
                // 客户联系/获客助手族的官方接入方式按应用模式分通道（92130/92277/96361/97299/98958/99485）：
                // 自建与代开发经应用数据回调 URL（Event 信封）；第三方经套件指令回调 URL（InfoType 信封，92277）。
                if (Channel == WechatCallbackChannel.App)
                {
                    return AppType == WechatAppType.Internal || AppType == WechatAppType.Provider;
                }

                if (Channel == WechatCallbackChannel.Suite)
                {
                    return AppType == WechatAppType.ThirdParty;
                }

                return false;

            case WechatCallbackEventFamily.Unknown:
            default:
                // 无法判别的事件族（协议外报文）不拦截，交由处理器（兜底）自行处置。
                return true;
        }
    }
}
