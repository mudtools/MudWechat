// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback;

/// <summary>
/// 被动回复处理器（公众号独有能力：企微侧一律回 <c>success</c>，无被动回复体）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>「如无特定要求，回复空串或者 success（无需加密）即可，其他回包内容需加密处理」</b>。
/// 故本接口返回 <c>null</c> 时 SDK 回明文 <c>success</c>；返回非 null 时按当前安全模式加密回写
/// （安全/兼容模式加密 XML；明文模式明文 XML）。
/// </para>
/// <para>
/// <b>5 秒约束</b>：被动回复必须在平台断连前完成；处理器内不得做长耗时同步 IO。
/// 平台在 5 秒内收不到响应会断连并<b>重试共 3 次</b>；超时由软超时（默认 4000ms）兜底为 <c>success</c>。
/// </para>
/// </remarks>
public interface IMpCallbackReplyHandler
{
    /// <summary>尝试构造被动回复体。</summary>
    /// <param name="envelope">回调事件信封（回包 <c>FromUserName</c> 取信封 <c>ToUserName</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns><c>null</c> = 无回复（SDK 回明文 <c>success</c>）；非 null = 需回写的回复体。</returns>
    Task<MpCallbackReply?> TryReplyAsync(MpCallbackEnvelope envelope, CancellationToken cancellationToken = default);
}

/// <summary>
/// 被动回复体（<c>MsgType</c> + XML 字段袋）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何用字段袋而非逐类型 DTO</b>：被动回复的 XML 字段名即官方节点名（如 <c>Content</c>/<c>MediaId</c>），
/// 且各 <c>MsgType</c> 的字段集官方仍在演进（本方案 §12 V9 未核验完毕）；字段袋形态让宿主在
/// <b>零 SDK 改动</b>下构造新形态，同时保留 <see cref="Text"/> 便捷构造器覆盖最常见场景。
/// </para>
/// <para>
/// <b>ns2.0 红线</b>：<c>netstandard2.0</c> 下禁 <c>init</c>/<c>record</c>/<c>with</c>，
/// 故本类型用构造函数 + 只读属性表达不可变。
/// </para>
/// </remarks>
public sealed class MpCallbackReply
{
    /// <summary>创建被动回复体。</summary>
    /// <param name="msgType">回复消息类型（<c>text</c>/<c>image</c>/<c>voice</c>/<c>video</c>/<c>music</c>/<c>news</c>）。</param>
    /// <param name="fields">XML 字段袋（键 = 官方节点名）。</param>
    /// <exception cref="ArgumentException"><paramref name="msgType"/> 为空白。</exception>
    public MpCallbackReply(string msgType, IReadOnlyDictionary<string, string?> fields)
    {
        if (string.IsNullOrWhiteSpace(msgType))
        {
            throw new ArgumentException("被动回复的消息类型不能为空。", nameof(msgType));
        }

        MsgType = msgType;
        Fields = fields ?? new Dictionary<string, string?>();
    }

    /// <summary>回复消息类型（<c>MsgType</c> 节点值）。</summary>
    public string MsgType { get; }

    /// <summary>XML 字段袋（键 = 官方节点名；<c>null</c> 值写出空节点）。</summary>
    public IReadOnlyDictionary<string, string?> Fields { get; }

    /// <summary>
    /// 构造文本回复（最常见形态）。
    /// </summary>
    /// <param name="content">文本内容（<c>Content</c> 节点）。</param>
    /// <returns>文本类型回复体。</returns>
    public static MpCallbackReply Text(string content)
        => new(
            MpCallbackMessageTypes.Text,
            new Dictionary<string, string?>(StringComparer.Ordinal) { ["Content"] = content });
}
