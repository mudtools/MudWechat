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
