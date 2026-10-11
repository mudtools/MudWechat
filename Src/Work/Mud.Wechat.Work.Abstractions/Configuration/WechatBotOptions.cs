// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Configuration;

/// <summary>
/// 智能机器人长连接配置面（配置节 <c>WechatBots</c>，官方 101463）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>WechatCallbackOptions</code> 的关系</b>：回调地址模式的机器人凭据（Token / EncodingAESKey）在
/// <c>WechatCallbackOptions.Apps</c>（<c>Channel = Bot</c>）；本类型只承载<b>长连接</b>模式专属凭据与参数。
/// 官方 101463：「API 模式『长连接 / 回调地址』二选一，切换即失效」⇒ 同一 BotKey
/// <b>不得</b>同时出现在两个配置面（跨面互斥在长连接启动期校验，见
/// <c>WechatBotLongConnectionServiceCollectionExtensions</c> —— 两个配置面分属两个包，本类型无法独自判定）。
/// </para>
/// <para>
/// <b>凭据面</b>：<see cref="WechatBotAppOptions.BotSecret"/> 是长连接<b>专用</b>密钥（官方原文「与回调地址模式的
/// Token/EncodingAESKey 不同」），经 <c>aibot_subscribe</c> 帧明文上送 —— 属<b>凭据</b>，
/// 不得进日志 / 遥测 / 异常消息（对齐全线凭据治理口径）。
/// </para>
/// </remarks>
public class WechatBotOptions
{
    /// <summary>默认配置节名。</summary>
    public const string DefaultSectionName = "WechatBots";

    /// <summary>
    /// 长连接机器人配置（键 = 机器人键，形状经 <see cref="WechatAppKeyValidator"/> 校验）。
    /// </summary>
    /// <remarks>
    /// 键即「宿主在长连接服务/日志里使用的机器人标识」，同时也是连接租约（<c>IWechatBotConnectionLease</c>）
    /// 的分槽键 —— 多机器人各持一条连接与一份租约，互不覆盖。
    /// </remarks>
    public Dictionary<string, WechatBotAppOptions> Bots { get; set; } = new();

    /// <summary>
    /// 校验配置完整性（启动期调用；缺失或形态非法即抛）。
    /// </summary>
    public void Validate()
    {
        if (Bots.Count == 0)
        {
            throw new InvalidOperationException(
                "智能机器人长连接配置缺少 Bots（至少配置一个机器人：WechatBots:Bots:{机器人键}:...）。");
        }

        foreach (var pair in Bots)
        {
            WechatAppKeyValidator.Validate(pair.Key);

            if (pair.Value == null)
            {
                throw new InvalidOperationException($"智能机器人长连接配置 Bots[\"{pair.Key}\"] 为 null。");
            }

            pair.Value.Validate(pair.Key);
        }
    }
}

/// <summary>单个长连接机器人的配置（官方 <c>bot_id</c> + 长连接专用 <c>secret</c> + 保活与重连参数）。</summary>
public class WechatBotAppOptions
{
    /// <summary>
    /// 机器人标识（官方 <c>aibot_subscribe</c> 帧体的 <c>bot_id</c>，必填）。
    /// </summary>
    /// <remarks>消费点：<c>WechatBotConnection</c> 订阅帧。官方帧体字段名恒为 <c>bot_id</c>（下划线）。</remarks>
    public string BotId { get; set; } = string.Empty;

    /// <summary>
    /// 长连接专用密钥（官方 <c>aibot_subscribe</c> 帧体的 <c>secret</c>，必填）。
    /// </summary>
    /// <remarks>
    /// 消费点：<c>WechatBotConnection</c> 订阅帧。<b>属凭据</b>：不得进日志 / 遥测 / 异常消息；
    /// 与回调地址模式的 <c>Token</c> / <c>EncodingAESKey</c> 不是同一个东西（官方原文）。
    /// </remarks>
    public string BotSecret { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用长连接模式（<c>true</c> = 长连接服务为该机器人建连；缺省 <c>false</c> = 不参与长连接）。
    /// </summary>
    /// <remarks>消费点：长连接启动期为 <c>true</c> 的机器人各建一条 <c>WechatBotConnection</c>。</remarks>
    public bool EnableLongConnection { get; set; }

    /// <summary>
    /// 心跳间隔（秒，官方建议 30；服务端长时间未收到心跳会主动断开）。
    /// </summary>
    /// <remarks>消费点：<c>WechatBotConnection</c> 的 ping 定时器。<c>0</c> = 沿用默认 30 秒。</remarks>
    public int PingIntervalSeconds { get; set; }

    /// <summary>
    /// 断线重连的基础退避（毫秒，指数退避起点；缺省 <c>0</c> = 沿用默认 1000 毫秒）。
    /// </summary>
    /// <remarks>
    /// 消费点：<c>WechatBotConnection</c> 重连延迟 = base × 2^attempt（封顶 max）。
    /// 收到 <c>disconnected_event</c>（新连接踢旧连接）<b>必须</b>先退避再夺租约（防乒乓，官方 101463 + 方案 §6.4-9）。
    /// </remarks>
    public int ReconnectBaseDelayMs { get; set; }

    /// <summary>
    /// 断线重连的退避上限（毫秒；缺省 <c>0</c> = 沿用默认 30000 毫秒）。
    /// </summary>
    /// <remarks>消费点：<see cref="ReconnectBaseDelayMs"/> 的封顶值。</remarks>
    public int ReconnectMaxDelayMs { get; set; }

    /// <summary>长连接保活与重连参数的缺省值（官方建议心跳 30 秒）。</summary>
    public const int DefaultPingIntervalSeconds = 30;

    /// <summary>重连基础退避缺省值（毫秒）。</summary>
    public const int DefaultReconnectBaseDelayMs = 1000;

    /// <summary>重连退避上限缺省值（毫秒）。</summary>
    public const int DefaultReconnectMaxDelayMs = 30000;

    /// <summary>
    /// 校验本机器人配置（由 <see cref="WechatBotOptions.Validate"/> 逐机器人调用）。
    /// </summary>
    /// <param name="botKey">机器人键（诊断文本使用）。</param>
    public void Validate(string botKey)
    {
        if (string.IsNullOrWhiteSpace(BotId))
        {
            throw new InvalidOperationException($"智能机器人「{botKey}」缺少 BotId（官方 aibot_subscribe 帧体的 bot_id）。");
        }

        if (string.IsNullOrWhiteSpace(BotSecret))
        {
            throw new InvalidOperationException($"智能机器人「{botKey}」缺少 BotSecret（长连接专用密钥；官方原文与回调地址模式的 Token/EncodingAESKey 不同）。");
        }

        if (PingIntervalSeconds < 0)
        {
            throw new InvalidOperationException($"智能机器人「{botKey}」的 PingIntervalSeconds 不得为负（当前 {PingIntervalSeconds}；0 = 沿用默认 30 秒）。");
        }

        if (ReconnectBaseDelayMs < 0 || ReconnectMaxDelayMs < 0)
        {
            throw new InvalidOperationException($"智能机器人「{botKey}」的重连退避参数不得为负（0 = 沿用默认值）。");
        }

        if (ReconnectMaxDelayMs > 0 && ReconnectBaseDelayMs > ReconnectMaxDelayMs)
        {
            throw new InvalidOperationException($"智能机器人「{botKey}」的重连基础退避（{ReconnectBaseDelayMs}ms）不得大于退避上限（{ReconnectMaxDelayMs}ms）。");
        }
    }
}
