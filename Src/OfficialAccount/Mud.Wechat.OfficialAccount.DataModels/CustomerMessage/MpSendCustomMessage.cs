// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>
/// 发送客服消息请求体（<c>sendCustomMessage</c>，<c>POST /cgi-bin/message/custom/send</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>「扁平 + 按 msgtype 二选一」的官方形态</b>：除 <c>touser</c>/<c>msgtype</c> 外，各消息体字段
/// 与 <c>msgtype</c> <b>同层</b>（非嵌套），只有与 <c>msgtype</c> 对应的那一个非空。SDK 忠实建模该形态
/// （无法用「多态基类」表达，因为官方报文没有类型判别字段）。
/// </para>
/// <para>
/// <b>分支复用</b>：<see cref="Image"/> / <see cref="Voice"/> 与 <c>mpnews</c> 分支的字段集相同
/// （均仅 <c>media_id</c>）⇒ 共用 <c>MpMediaMessage</c>，避免三个同构类漂移。
/// </para>
/// <para>
/// <b>下发额度（官方明文，业务硬约束）</b>：
/// </para>
/// <list type="table">
/// <item><description>用户发送消息 → <b>5 条 / 48 小时</b>有效期</description></item>
/// <item><description>点击自定义菜单 → 3 条 / 1 分钟（<b>仅</b> <c>click</c>、<c>scancode_push</c>、<c>scancode_waitmsg</c> 三类菜单会触发客服接口）</description></item>
/// <item><description>关注公众号 → 3 条 / 1 分钟</description></item>
/// <item><description>扫描二维码 → 3 条 / 1 分钟</description></item>
/// </list>
/// <para>⇒ 调用方必须在额度窗口内发消息，且不得假定「随时可推」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpSendCustomMessageRequest
{
    /// <summary>用户的 OpenID。</summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; } = string.Empty;

    /// <summary>消息类型（取值见 <c>MpCustomMessageTypes</c>）。</summary>
    [JsonPropertyName("msgtype")]
    public string MsgType { get; set; } = string.Empty;

    /// <summary>文本消息体（<c>msgtype = text</c> 时必填；<c>content</c> 支持插入跳小程序的文字链）。</summary>
    [JsonPropertyName("text")]
    public MpTextMessage? Text { get; set; }

    /// <summary>图片消息体（<c>msgtype = image</c> 时必填）。</summary>
    [JsonPropertyName("image")]
    public MpMediaMessage? Image { get; set; }

    /// <summary>语音消息体（<c>msgtype = voice</c> 时必填）。</summary>
    [JsonPropertyName("voice")]
    public MpMediaMessage? Voice { get; set; }

    /// <summary>视频消息体（<c>msgtype = video</c> 时必填）。</summary>
    [JsonPropertyName("video")]
    public MpVideoMessage? Video { get; set; }

    /// <summary>音乐消息体（<c>msgtype = music</c> 时必填）。</summary>
    [JsonPropertyName("music")]
    public MpMusicMessage? Music { get; set; }

    /// <summary>图文消息（跳转外链，<c>msgtype = news</c> 时必填；官方条数限制 <b>1 条以内</b>）。</summary>
    [JsonPropertyName("news")]
    public MpNewsMessage? News { get; set; }

    /// <summary>
    /// 图文消息（跳转图文消息页面，<c>msgtype = mpnews</c> 时必填；条数限制 1 条以内，超限 <c>45008</c>）。
    /// </summary>
    /// <remarks><b>官方标注「草稿灰度完成后不再支持」</b>⇒ 新代码应改用 <see cref="MpNewsArticle"/>。</remarks>
    [JsonPropertyName("mpnews")]
    public MpMediaMessage? MpNews { get; set; }

    /// <summary>图文消息（<c>msgtype = mpnewsarticle</c> 时必填；使用「发布」接口得到的 <c>article_id</c>）。</summary>
    [JsonPropertyName("mpnewsarticle")]
    public MpMpNewsArticleMessage? MpNewsArticle { get; set; }

    /// <summary>菜单消息体（<c>msgtype = msgmenu</c> 时必填）。</summary>
    [JsonPropertyName("msgmenu")]
    public MpMsgMenuMessage? MsgMenu { get; set; }

    /// <summary>卡券消息体（<c>msgtype = wxcard</c> 时必填）。</summary>
    [JsonPropertyName("wxcard")]
    public MpWxCardMessage? WxCard { get; set; }

    /// <summary>小程序卡片消息体（<c>msgtype = miniprogrampage</c> 时必填）。</summary>
    [JsonPropertyName("miniprogrampage")]
    public MpMiniProgramPageMessage? MiniProgramPage { get; set; }

    /// <summary>以某个客服账号来发消息（可选；不填则用默认客服身份）。</summary>
    [JsonPropertyName("customservice")]
    public MpCustomServiceInfo? CustomService { get; set; }

    /// <summary>AI 消息上下文（可选；<c>is_ai_msg = 1</c> 时消息下方出现「内容由第三方 AI 生成」灰色标记）。</summary>
    [JsonPropertyName("aimsgcontext")]
    public MpAimsgContext? AiMsgContext { get; set; }

    /// <summary>子商户 ID（<b>普通账号无需传</b>）。</summary>
    [JsonPropertyName("businessid")]
    public string? BusinessId { get; set; }
}

/// <summary>文本消息体（<c>text</c> 分支）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpTextMessage
{
    /// <summary>
    /// 文本内容。
    /// </summary>
    /// <remarks>
    /// 支持插入跳小程序的文字链：<c>data-miniprogram-appid</c>（须与该公众号有绑定关系）+
    /// <c>data-miniprogram-path</c>（与 <c>app.json</c> 一致，可带参数）；对不支持该属性的旧客户端
    /// （6.5.16 以下）仍走 <c>href</c> 中的网页链接。
    /// </remarks>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

/// <summary>媒体消息体（<c>image</c> / <c>voice</c> / <c>mpnews</c> 三分支共用：字段集均仅 <c>media_id</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMediaMessage
{
    /// <summary>媒体 ID（通过素材上传接口获得）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;
}

/// <summary>视频消息体（<c>video</c> 分支：<c>media_id</c> 与 <c>thumb_media_id</c> 必填）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpVideoMessage
{
    /// <summary>视频媒体 ID（通过素材上传接口获得）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;

    /// <summary>缩略图媒体 ID（通过素材上传接口获得）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string ThumbMediaId { get; set; } = string.Empty;

    /// <summary>视频标题（可选）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>视频描述（可选）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>音乐消息体（<c>music</c> 分支：<c>title</c>/<c>description</c>/<c>musicurl</c>/<c>thumb_media_id</c> 必填）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMusicMessage
{
    /// <summary>音乐标题。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>音乐描述。</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>音乐链接。</summary>
    [JsonPropertyName("musicurl")]
    public string MusicUrl { get; set; } = string.Empty;

    /// <summary>高质量音乐链接（可选）。</summary>
    [JsonPropertyName("hqmusicurl")]
    public string? HqMusicUrl { get; set; }

    /// <summary>缩略图媒体 ID。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string ThumbMediaId { get; set; } = string.Empty;
}

/// <summary>图文消息体（<c>news</c> 分支；官方条数限制 1 条以内）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpNewsMessage
{
    /// <summary>图文条目列表（官方限制 1 条以内）。</summary>
    [JsonPropertyName("articles")]
    public List<MpNewsArticle> Articles { get; set; } = new();
}

/// <summary>图文条目（<c>news.articles</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpNewsArticle
{
    /// <summary>消息标题。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>消息描述。</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>封面图片 url。</summary>
    [JsonPropertyName("picurl")]
    public string PicUrl { get; set; } = string.Empty;

    /// <summary>跳转 url。</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

/// <summary>发布图文消息体（<c>mpnewsarticle</c> 分支）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMpNewsArticleMessage
{
    /// <summary>发布文章 ID（由「发布」系列接口获得）。</summary>
    [JsonPropertyName("article_id")]
    public string ArticleId { get; set; } = string.Empty;
}

/// <summary>菜单消息体（<c>msgmenu</c> 分支）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMsgMenuMessage
{
    /// <summary>菜单描述（可选）。</summary>
    [JsonPropertyName("head_content")]
    public string? HeadContent { get; set; }

    /// <summary>菜单内容（必填）。</summary>
    [JsonPropertyName("list")]
    public List<MpMsgMenuItem> List { get; set; } = new();

    /// <summary>菜单结尾（可选）。</summary>
    [JsonPropertyName("tail_content")]
    public string? TailContent { get; set; }
}

/// <summary>菜单项（<c>msgmenu.list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMsgMenuItem
{
    /// <summary>菜单值。</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>菜单项文本。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

/// <summary>卡券消息体（<c>wxcard</c> 分支）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpWxCardMessage
{
    /// <summary>卡券 ID。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}

/// <summary>小程序卡片消息体（<c>miniprogrampage</c> 分支；四字段官方均必填）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMiniProgramPageMessage
{
    /// <summary>小程序卡片标题。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>小程序 APPID。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>小程序页面路径（与 <c>app.json</c> 对齐，支持参数）。</summary>
    [JsonPropertyName("pagepath")]
    public string PagePath { get; set; } = string.Empty;

    /// <summary>卡片封面 media_id（image 类型；官方建议尺寸 520*416）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string ThumbMediaId { get; set; } = string.Empty;
}

/// <summary>指定客服账号（<c>customservice</c> 分支）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpCustomServiceInfo
{
    /// <summary>客服账号。</summary>
    [JsonPropertyName("kf_account")]
    public string KfAccount { get; set; } = string.Empty;
}

/// <summary>AI 消息上下文（<c>aimsgcontext</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpAimsgContext
{
    /// <summary>消息下方是否增加灰色标记「内容由第三方 AI 生成」（<c>0</c> 不增加 / <c>1</c> 增加）。</summary>
    [JsonPropertyName("is_ai_msg")]
    public int? IsAiMsg { get; set; }
}
