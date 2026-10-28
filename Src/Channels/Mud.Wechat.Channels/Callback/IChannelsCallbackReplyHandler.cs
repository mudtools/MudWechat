// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Callback;

/// <summary>
/// 微信小店回调结构化应答（少数事件要求商家<b>同步返回业务数据</b>，其余事件回明文 <c>success</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>哪些事件需要结构化应答</b>（2026-10 逐页核验官方 notify 文档）：会员三事件
/// <c>channels_ec_vip_user_info_get</c>（返回 is_vip/identity/outer_userid，true 即静默关联）、
/// <c>channels_ec_gen_order_info_init</c>（返回会员优惠金额拆解）、<c>channels_ec_order_use_vip_discount</c>
/// （返回 result 确认/拦截下单）—— 其余 39 个事件官方均为「success 或空字符串（无需加密）」。
/// </para>
/// <para>
/// <b>回包加密</b>：小店强制安全模式，结构化回包经 <see cref="Mud.Wechat.Abstractions.Callback.WechatCallbackCrypto"/>
/// 加密并签名后以 <c>{"Encrypt","MsgSignature","TimeStamp","Nonce"}</c> 信封回写
/// （官方 dev_before/message_push：「其他回包内容需加密」）；Nonce 回填 URL 参数中的 nonce。
/// </para>
/// </remarks>
public sealed class ChannelsCallbackReply
{
    private ChannelsCallbackReply(string json)
    {
        Json = json;
    }

    /// <summary>应答明文 JSON（回写前由回写器加密签名；<b>敏感载体</b>：不得整体写入日志 / 遥测 / 异常消息）。</summary>
    public string Json { get; }

    /// <summary>以明文 JSON 创建应答。</summary>
    /// <param name="json">应答明文 JSON（建议经源生成 JSON 上下文序列化，AOT 安全；勿手写拼接）。</param>
    /// <returns>应答实例。</returns>
    /// <exception cref="ArgumentException"><paramref name="json"/> 为空白。</exception>
    public static ChannelsCallbackReply Create(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("结构化应答 JSON 不能为空白（无须应答时返回 null 即可）。", nameof(json));
        }

        return new ChannelsCallbackReply(json);
    }
}

/// <summary>
/// 小店回调结构化应答处理器（仅官方要求同步返回业务数据的事件需要注册）。
/// </summary>
/// <remarks>
/// <para>
/// 匹配语义与事件处理器一致：<see cref="SupportedEventType"/> 精确匹配（填
/// <c>ChannelsCallbackEventTypes</c> 常量）；空串/null = 兜底应答处理器。
/// 分发器在事件处理器执行后按「appKey 专属 → 全局」顺序取<b>第一个</b>返回非 <c>null</c> 的应答；
/// 无应答处理器或全部返回 <c>null</c> ⇒ 中间件回明文 <c>success</c>。
/// </para>
/// <para>
/// 应答体承载在分发<b>同一软超时窗口</b>内（官方 5 秒未收到应答即判推送失败并重推），
/// 实现须快速返回；对重投递幂等。
/// </para>
/// </remarks>
public interface IChannelsCallbackReplyHandler
{
    /// <summary>支持的事件类型键（= <see cref="WechatChannelsCallbackEvent.EventTypeKey"/>）；空串/null = 兜底应答处理器。</summary>
    string SupportedEventType { get; }

    /// <summary>构造结构化应答。</summary>
    /// <param name="envelope">回调事件信封（解密后的 JSON 明文经接收器校验）。</param>
    /// <param name="cancellationToken">取消令牌（链接请求中止与分发软超时）。</param>
    /// <returns>结构化应答；<c>null</c> = 本处理器无应答（回明文 success）。</returns>
    Task<ChannelsCallbackReply?> TryReplyAsync(
        WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken = default);
}
