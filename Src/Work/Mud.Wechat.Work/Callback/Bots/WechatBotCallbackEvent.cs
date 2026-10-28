// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人回调事件信封（解密后的结构化结果）。
/// </summary>
/// <remarks>
/// <para>
/// <b>信封只承载「路由 + 分派 + 便捷只读快照」字段，载荷本体经强类型属性直出</b>
/// （<see cref="Message"/> / <see cref="Event"/>）：适配层（HTTP 中间件 / 长连接客户端）完成
/// JSON 明文 → 超集 DTO 的反序列化并注入信封，处理器无需自行反序列化、也无需依赖 <c>internal</c>
/// 的 <c>AibotJsonContext</c>（生成上下文为 internal，外部宿主不可直读）。
/// </para>
/// <para>
/// <b>不复制载荷字段</b>：信封<b>不得</b>再逐一镜像 <c>card_type</c>/<c>event_key</c>/<c>task_id</c>/
/// <c>selected_items</c>/<c>feedback</c>/<c>stream</c>/<c>quote</c> 等载荷内字段——两套并行表示必然漂移，
/// 载荷唯一事实来源是 <see cref="Message"/> / <see cref="Event"/>（明文原文经 <see cref="DecryptedJson"/> 兜底）。
/// </para>
/// <para>
/// <b>明文为敏感材料</b>：<see cref="DecryptedJson"/> 含用户消息与（长连接）媒体 <c>aeskey</c>，
/// 不得写入日志、遥测或异常消息。
/// </para>
/// </remarks>
public class WechatBotCallbackEvent
{
    /// <summary>
    /// 事件归属的回调配置键（路由路径段 <c>/{GlobalRoutePrefix}/{BotKey}</c>，与 <c>WechatCallbackOptions.Apps</c> 字典键一致）。
    /// </summary>
    /// <remarks>
    /// <b>权威来源</b>：宿主可据此经 <c>WechatCallbackOptions.ResolveApp</c> 反查凭据；
    /// 仅由 SDK 适配层写入（处理器不得改写）。
    /// </remarks>
    public string? AppKey { get; internal set; }

    /// <summary>智能机器人 id（报文 <c>aibotid</c>，= 后台配置的 BotID）。</summary>
    public string? AibotId { get; set; }

    /// <summary>
    /// 本次回调的唯一性标志（报文 <c>msgid</c>）。
    /// </summary>
    /// <remarks>
    /// 官方明确「可能因网络原因重复回调」⇒ <b>幂等由宿主负责</b>。回调地址模式另有指纹闸（fail-closed）；
    /// 长连接帧无签名、<b>无指纹闸</b>，去重完全依赖本字段。
    /// </remarks>
    public string? MsgId { get; set; }

    /// <summary>会话 id（报文 <c>chatid</c>；仅群聊类型返回）。</summary>
    public string? ChatId { get; set; }

    /// <summary>会话类型（报文 <c>chattype</c>：<c>single</c> 单聊 / <c>group</c> 群聊）。</summary>
    public string? ChatType { get; set; }

    /// <summary>事件触发者的 corpid（报文 <c>from.corpid</c>；企业内部智能机器人官方不返回）。</summary>
    public string? FromCorpId { get; set; }

    /// <summary>事件触发者的 userid（报文 <c>from.userid</c>；可能为企业主体下的加密 userid）。</summary>
    public string? FromUserId { get; set; }

    /// <summary>报文顶层 <c>msgtype</c>（消息族为 <c>text</c>/<c>image</c>/…/<c>stream</c>；事件族恒为 <c>event</c>）。</summary>
    public string? MsgType { get; set; }

    /// <summary>事件类型（报文 <c>event.eventtype</c>；<b>消息回调为 <c>null</c></b>）。</summary>
    public string? EventType { get; set; }

    /// <summary>
    /// 支持主动回复消息的临时 url（报文 <c>response_url</c>）。
    /// </summary>
    /// <remarks>
    /// 官方 101138：每个 url <b>仅可调用一次</b>、有效期 <b>1 小时</b>，超期或重复调用均失败且不可重试。
    /// 流式消息刷新（<c>msgtype=stream</c>）与 <c>feedback_event</c> <b>不携带</b>本字段。
    /// </remarks>
    public string? ResponseUrl { get; set; }

    /// <summary>消息族载荷（报文为消息回调时非空；可空超集，覆盖 7 种 <c>msgtype</c>）。</summary>
    public AibotMessageCallback? Message { get; set; }

    /// <summary>事件族载荷（报文为事件回调时非空；可空超集，覆盖 4 种 <c>eventtype</c>）。</summary>
    public AibotEventCallback? Event { get; set; }

    /// <summary>
    /// 解密后的明文 JSON 原文（兜底访问通道）。
    /// </summary>
    /// <remarks><b>敏感材料</b>：含用户消息内容与（长连接）媒体 <c>aeskey</c>，不得写入日志/遥测/异常消息。</remarks>
    public string? DecryptedJson { get; set; }

    /// <summary>是否为事件回调（报文 <c>event.eventtype</c> 非空）。</summary>
    public bool IsEventCallback => EventType != null && EventType.Length > 0;

    /// <summary>
    /// 处理器匹配键（对齐 XML 侧 <c>WechatCallbackEvent.EventTypeKey</c> 语义）：
    /// 事件回调取 <c>event.eventtype</c>，消息回调取顶层 <c>msgtype</c>；两者皆空返回空串（仅兜底处理器可见）。
    /// </summary>
    public string EventTypeKey => EventType != null && EventType.Length > 0
        ? EventType
        : MsgType ?? string.Empty;
}
