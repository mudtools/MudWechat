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
    private static readonly IReadOnlyDictionary<string, string?> EmptyFields =
        new Dictionary<string, string?>(StringComparer.Ordinal);

    /// <summary>创建被动回复体（平铺字段袋形态）。</summary>
    /// <param name="msgType">回复消息类型（<c>text</c>/<c>image</c>/<c>voice</c>/<c>video</c>/<c>music</c>/<c>news</c>）。</param>
    /// <param name="fields">XML 字段袋（键 = 官方节点名）。</param>
    /// <exception cref="ArgumentException"><paramref name="msgType"/> 为空白。</exception>
    public MpCallbackReply(string msgType, IReadOnlyDictionary<string, string?> fields)
        : this(msgType, fields, null)
    {
    }

    /// <summary>创建被动回复体（含预渲染的嵌套体）。</summary>
    /// <param name="msgType">回复消息类型。</param>
    /// <param name="fields">平铺字段袋。</param>
    /// <param name="bodyXml">嵌套体 XML（由本类型工厂方法生成的**已转义**片段；直接拼入 <c>&lt;xml&gt;</c> 内）。</param>
    public MpCallbackReply(string msgType, IReadOnlyDictionary<string, string?> fields, string? bodyXml)
    {
        if (string.IsNullOrWhiteSpace(msgType))
        {
            throw new ArgumentException("被动回复的消息类型不能为空。", nameof(msgType));
        }

        MsgType = msgType;
        Fields = fields ?? EmptyFields;
        BodyXml = bodyXml;
    }

    /// <summary>回复消息类型（<c>MsgType</c> 节点值）。</summary>
    public string MsgType { get; }

    /// <summary>XML 字段袋（键 = 官方节点名；<c>null</c> 值写出空节点）。</summary>
    public IReadOnlyDictionary<string, string?> Fields { get; }

    /// <summary>嵌套体 XML（如 <c>&lt;Image&gt;…&lt;/Image&gt;</c> / <c>&lt;Articles&gt;…&lt;/Articles&gt;</c>）；无嵌套时为 <c>null</c>。</summary>
    public string? BodyXml { get; }

    /// <summary>构造文本回复（最常见形态）。</summary>
    /// <param name="content">文本内容（<c>Content</c> 节点）。</param>
    /// <returns>文本类型回复体。</returns>
    public static MpCallbackReply Text(string content)
        => new(
            MpCallbackReplyTypes.Text,
            new Dictionary<string, string?>(StringComparer.Ordinal) { ["Content"] = content });

    /// <summary>构造图片回复（官方：<c>Image/MediaId</c>，媒体 id 须先经素材管理上传）。</summary>
    /// <param name="mediaId">素材 id。</param>
    /// <returns>图片类型回复体。</returns>
    /// <remarks>官方约束：**图片不支持 gif 动图**。</remarks>
    public static MpCallbackReply Image(string mediaId)
        => new(MpCallbackReplyTypes.Image, EmptyFields, MpReplyXml.Container("Image", "MediaId", mediaId));

    /// <summary>构造语音回复（官方：<c>Voice/MediaId</c>）。</summary>
    /// <param name="mediaId">素材 id。</param>
    /// <returns>语音类型回复体。</returns>
    public static MpCallbackReply Voice(string mediaId)
        => new(MpCallbackReplyTypes.Voice, EmptyFields, MpReplyXml.Container("Voice", "MediaId", mediaId));

    /// <summary>构造视频回复（官方：<c>Video/MediaId</c> 必填，<c>Title</c>/<c>Description</c> 可选）。</summary>
    /// <param name="mediaId">素材 id。</param>
    /// <param name="title">标题（可选）。</param>
    /// <param name="description">描述（可选）。</param>
    /// <returns>视频类型回复体。</returns>
    public static MpCallbackReply Video(string mediaId, string? title = null, string? description = null)
        => new(MpCallbackReplyTypes.Video, EmptyFields, MpReplyXml.Containers(
            "Video",
            ("MediaId", mediaId, true),
            ("Title", title, false),
            ("Description", description, false)));

    /// <summary>
    /// 构造音乐回复（官方：<c>Music</c> 内 **仅 <c>ThumbMediaId</c> 必填**，其余可选；
    /// <c>HQMusicUrl</c> 在 WIFI 环境优先播放）。
    /// </summary>
    /// <param name="thumbMediaId">缩略图素材 id（**必填**）。</param>
    /// <param name="musicUrl">音乐链接（可选）。</param>
    /// <param name="hqMusicUrl">高质量音乐链接（可选）。</param>
    /// <param name="title">标题（可选）。</param>
    /// <param name="description">描述（可选）。</param>
    /// <returns>音乐类型回复体。</returns>
    /// <remarks>
    /// <b>官方字段名陷阱</b>：参数表写作 <c>MusicURL</c>，XML 报文为 <c>MusicUrl</c>
    /// —— 本实现以 **XML 报文拼写**为准（协议以报文为准）。
    /// </remarks>
    public static MpCallbackReply Music(
        string thumbMediaId, string? musicUrl = null, string? hqMusicUrl = null,
        string? title = null, string? description = null)
        => new(MpCallbackReplyTypes.Music, EmptyFields, MpReplyXml.Containers(
            "Music",
            ("ThumbMediaId", thumbMediaId, true),
            ("MusicUrl", musicUrl, false),
            ("HQMusicUrl", hqMusicUrl, false),
            ("Title", title, false),
            ("Description", description, false)));

    /// <summary>
    /// 构造图文回复（官方：<c>ArticleCount</c> + <c>Articles/item</c> —— **容器名为 <c>Articles</c>、
    /// 每条为 <c>item</c>（不是 <c>Article</c>）**）。
    /// </summary>
    /// <param name="articles">图文条目（1~8 条）。</param>
    /// <returns>图文类型回复体。</returns>
    /// <exception cref="ArgumentOutOfRangeException">条数为 0 或超过官方上限 8。</exception>
    /// <remarks>
    /// <b>官方条数限制（处理器须自知）</b>：当用户发送文本/图片/语音/视频/图文/地理位置六类消息时
    /// **只能回复 1 条**图文；其余场景**最多 8 条**。SDK 只拦截超过 8 条的非法值（上下文相关的 1 条限制无法在此判定）。
    /// </remarks>
    public static MpCallbackReply News(IReadOnlyList<MpNewsArticle> articles)
    {
        if (articles == null)
        {
            throw new ArgumentNullException(nameof(articles));
        }

        if (articles.Count == 0 || articles.Count > 8)
        {
            throw new ArgumentOutOfRangeException(
                nameof(articles), articles.Count,
                "图文回复条数必须在 1~8 之间（官方上限 8；六类消息场景仅允许 1 条）。");
        }

        var builder = new System.Text.StringBuilder(256);
        builder.Append("<ArticleCount>").Append(articles.Count).Append("</ArticleCount>");
        builder.Append("<Articles>");
        for (var i = 0; i < articles.Count; i++)
        {
            var article = articles[i] ?? throw new ArgumentException("图文条目不得为 null。", nameof(articles));
            builder.Append("<item>");
            builder.Append(MpReplyXml.Element("Title", article.Title));
            builder.Append(MpReplyXml.Element("Description", article.Description));
            builder.Append(MpReplyXml.Element("PicUrl", article.PicUrl));
            builder.Append(MpReplyXml.Element("Url", article.Url));
            builder.Append("</item>");
        }

        builder.Append("</Articles>");
        return new MpCallbackReply(MpCallbackReplyTypes.News, EmptyFields, builder.ToString());
    }

    /// <summary>
    /// 构造「转发到客服」回复（官方「将消息转发到客服」页，P0-e 已核验；I1 补齐的第七型）。
    /// </summary>
    /// <param name="kfAccount">指定会话接入的客服账号（可选；提供时附 <c>TransInfo/KfAccount</c> 节点，
    /// 把消息转给指定客服——须先确认该客服具备接入能力，否则用户会被直接接入且不再通知其他客服）。</param>
    /// <returns>转客服类型回复体（<c>MsgType = transfer_customer_service</c>；无其他专有字段）。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方语义（原文要点）</b>：回复本类型后微信服务器把当次用户消息转发至客服系统；
    /// 「用户被客服接入以后，客服关闭会话以前……用户发送的消息均会被直接转发至客服系统。
    /// 当会话超过 30 分钟客服没有关闭时，微信服务器会自动停止转发至客服」；
    /// 「<b>只针对微信用户发来的消息才进行转发</b>，而对于其他任何事件（比如菜单点击、地理位置上报等）
    /// 都不应该转接」；等待队列中的用户消息仍会推送到开发者 URL。
    /// </para>
    /// <para>
    /// <b>与客服消息接口的额度关系</b>：转客服回复是「5 秒内不能处理完 → 转人工」的标准姿势，
    /// <b>不消耗 48 小时客服下发额度</b>（那是客服消息接口 <c>IMpCustomerMessageService</c> 的约束）。
    /// </para>
    /// </remarks>
    public static MpCallbackReply TransferToCustomerService(string? kfAccount = null)
        => kfAccount is { Length: > 0 }
            ? new(MpCallbackReplyTypes.TransferToCustomerService, EmptyFields,
                MpReplyXml.Container("TransInfo", "KfAccount", kfAccount))
            : new(MpCallbackReplyTypes.TransferToCustomerService, EmptyFields);
}

/// <summary>图文回复条目（官方 <c>Articles/item</c> 的四个必填字段）。</summary>
/// <remarks>官方图片要求：支持 JPG/PNG，建议大图 360×200、小图 200×200。</remarks>
public sealed class MpNewsArticle
{
    /// <summary>创建图文条目。</summary>
    /// <param name="title">标题（必填）。</param>
    /// <param name="description">描述（必填）。</param>
    /// <param name="picUrl">图片链接（必填）。</param>
    /// <param name="url">点击跳转链接（必填）。</param>
    public MpNewsArticle(string title, string description, string picUrl, string url)
    {
        Title = title;
        Description = description;
        PicUrl = picUrl;
        Url = url;
    }

    /// <summary>图文标题。</summary>
    public string Title { get; }

    /// <summary>图文描述。</summary>
    public string Description { get; }

    /// <summary>图片链接。</summary>
    public string PicUrl { get; }

    /// <summary>跳转链接。</summary>
    public string Url { get; }
}

/// <summary>被动回复 XML 片段构造（转义 + 嵌套容器；供 <see cref="MpCallbackReply"/> 工厂方法使用）。</summary>
internal static class MpReplyXml
{
    internal static string Container(string containerName, string childName, string? childValue)
        => "<" + containerName + ">" + Element(childName, childValue) + "</" + containerName + ">";

    internal static string Containers(string containerName, params (string Name, string? Value, bool Required)[] children)
    {
        var builder = new System.Text.StringBuilder(128);
        builder.Append('<').Append(containerName).Append('>');
        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            if (child.Required && string.IsNullOrEmpty(child.Value))
            {
                throw new ArgumentException($"被动回复 {containerName} 的 {child.Name} 为官方必填字段，不得为空。");
            }

            if (child.Value != null)
            {
                builder.Append(Element(child.Name, child.Value));
            }
        }

        builder.Append("</").Append(containerName).Append('>');
        return builder.ToString();
    }

    internal static string Element(string name, string? value)
    {
        var builder = new System.Text.StringBuilder(64);
        builder.Append('<').Append(name).Append('>');
        AppendEscaped(builder, value);
        builder.Append("</").Append(name).Append('>');
        return builder.ToString();
    }

    private static void AppendEscaped(System.Text.StringBuilder builder, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        foreach (var ch in value!)
        {
            switch (ch)
            {
                case '&':
                    builder.Append("&amp;");
                    break;
                case '<':
                    builder.Append("&lt;");
                    break;
                case '>':
                    builder.Append("&gt;");
                    break;
                case '"':
                    builder.Append("&quot;");
                    break;
                default:
                    builder.Append(ch);
                    break;
            }
        }
    }
}
