// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback;

/// <summary>
/// 公众号 / 服务号回调事件信封（解密并解析后的结构化结果）。
/// </summary>
/// <remarks>
/// <para>
/// 实现叶层中立契约 <c>Mud.Wechat.Abstractions.Callback.IWechatCallbackEnvelope</c>（只读字段视图），
/// 并补充公众号专有字段（<c>MsgId</c> / <c>MsgDataId</c> / <c>Idx</c> / <c>EncryptType</c> 与请求期安全模式快照）。
/// </para>
/// <para>
/// <b>事件键（<see cref="EventTypeKey"/>）</b>：<c>Event</c>（<c>MsgType == event</c>）优先 → <c>MsgType</c> → 空串。
/// 公众号无 <c>InfoType</c>/<c>ChangeType</c> 层，也<b>不</b>设事件族矩阵闸（官方无套件授权族语义）。
/// </para>
/// <para>
/// 信封只承载字段契约，<b>不引入 XML 类型</b>：<c>XML → 信封</c> 的解析由
/// <c>Mud.Wechat.OfficialAccount.Callback.MpCallbackReceiver</c> 完成。
/// </para>
/// </remarks>
public sealed class MpCallbackEnvelope : Mud.Wechat.Abstractions.Callback.IWechatCallbackEnvelope
{
    // ——— 公共字段（与叶层中立契约同形） ———

    /// <summary>接收方：公众号的原始 ID（ToUserName 节点）。</summary>
    public string? ToUserName { get; set; }

    /// <summary>发送方：用户 OpenID（FromUserName 节点）。</summary>
    public string? FromUserName { get; set; }

    /// <summary>消息创建时间（CreateTime 节点，Unix 秒字符串）。</summary>
    public string? CreateTime { get; set; }

    /// <summary>消息类型（MsgType 节点：<c>text</c>/<c>image</c>/…/<c>event</c>）。</summary>
    public string? MsgType { get; set; }

    /// <summary>事件类型（Event 节点；普通消息为空）。</summary>
    public string? Event { get; set; }

    /// <summary>
    /// 解密后的原始明文（安全/兼容模式的密文解密结果；明文模式即请求体原文）。
    /// </summary>
    /// <remarks><b>敏感载体</b>：不得整体写入日志/遥测/异常消息。</remarks>
    public string? DecryptedXml { get; set; }

    /// <summary>时间戳（URL 查询参数，验签用）。</summary>
    public string? TimeStamp { get; set; }

    /// <summary>随机数（URL 查询参数，验签用）。</summary>
    public string? Nonce { get; set; }

    /// <summary>事件归属应用键（路由路径段 <c>/{前缀}/{AppKey}</c>；未命中回调凭据时 <c>null</c>）。</summary>
    public string? AppKey { get; internal set; }

    // ——— 公众号专有字段 ———

    /// <summary>消息 ID（MsgId 节点，64 位整型；<b>仅普通消息</b>携带，官方推荐按其做业务级排重）。</summary>
    public string? MsgId { get; set; }

    /// <summary>消息数据 ID（MsgDataId 节点，仅消息来自文章时携带）。</summary>
    public string? MsgDataId { get; set; }

    /// <summary>多图文消息序号（Idx 节点，从 1 开始；仅多图文携带）。</summary>
    public string? Idx { get; set; }

    /// <summary>请求期 <c>encrypt_type</c> 查询参数（示例值 <c>aes</c>；明文推送时缺省）。</summary>
    public string? EncryptType { get; set; }

    /// <summary>
    /// 请求期安全模式快照（按回调配置解析；只读事实，配置权威仍是 <c>MpCallbackOptions</c>）。
    /// </summary>
    public MpCallbackSecurityMode SecurityMode { get; internal set; } = MpCallbackSecurityMode.Safe;

    /// <summary>
    /// 报文实际形态：<c>true</c> = 密文（含 <c>&lt;Encrypt&gt;</c>），<c>false</c> = 明文。
    /// </summary>
    /// <remarks>
    /// 与配置期望（<see cref="SecurityMode"/>）区分：兼容模式下两者可不同（配置声明期望，实际形态按报文判定）。
    /// </remarks>
    public bool IsEncrypted { get; internal set; }

    /// <inheritdoc />
    public string EventTypeKey
    {
        get
        {
            if (Event != null && Event.Length > 0)
            {
                return Event;
            }

            return MsgType ?? string.Empty;
        }
    }
}
