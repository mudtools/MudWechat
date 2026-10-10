// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions;

/// <summary>
/// 客服消息 <c>msgtype</c> 取值常量（官方「发送客服消息」各分支字段表）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档内部不一致（本 SDK 的处置）</b>：该页 <c>msgtype</c> 字段说明只列了
/// 「<c>text</c> 文本；<c>image</c> 图片；<c>link</c> 图文链接；<c>miniprogrampage</c> 小程序卡片」四种，
/// 但<b>分支字段表</b>实际给出了 <c>voice</c>/<c>video</c>/<c>music</c>/<c>news</c>/<c>mpnews</c>/
/// <c>mpnewsarticle</c>/<c>msgmenu</c>/<c>wxcard</c> 等分支。本 SDK 以<b>分支字段表</b>为准（常量全覆盖），
/// 并在接口 remarks 记录该不一致。
/// </para>
/// <para>
/// <b>已弃用分支</b>：<see cref="MpNews"/>（<c>mpnews</c>）官方标注「草稿灰度完成后不再支持」
/// ⇒ 新代码应改用 <see cref="MpNewsArticle"/>（<c>mpnewsarticle</c> + 发布接口得到的 <c>article_id</c>）。
/// </para>
/// </remarks>
public static class MpCustomMessageTypes
{
    /// <summary>文本消息。</summary>
    public const string Text = "text";

    /// <summary>图片消息。</summary>
    public const string Image = "image";

    /// <summary>语音消息。</summary>
    public const string Voice = "voice";

    /// <summary>视频消息。</summary>
    public const string Video = "video";

    /// <summary>音乐消息。</summary>
    public const string Music = "music";

    /// <summary>图文消息（跳转外链）。</summary>
    public const string News = "news";

    /// <summary>图文消息（跳转图文消息页面）；<b>官方标注草稿灰度完成后不再支持</b>，条数限制 1 条以内（超限 <c>45008</c>）。</summary>
    public const string MpNews = "mpnews";

    /// <summary>图文消息（跳转图文消息页面，使用「发布」系列接口得到的 <c>article_id</c>）——<b>推荐替代 <see cref="MpNews"/></b>。</summary>
    public const string MpNewsArticle = "mpnewsarticle";

    /// <summary>菜单消息。</summary>
    public const string MsgMenu = "msgmenu";

    /// <summary>卡券消息。</summary>
    public const string WxCard = "wxcard";

    /// <summary>小程序卡片消息。</summary>
    public const string MiniProgramPage = "miniprogrampage";

    /// <summary>图文链接（官方 <c>msgtype</c> 说明中出现；分支字段表未单列，SDK 不单独建模）。</summary>
    public const string Link = "link";
}

/// <summary>
/// 客服输入状态 <c>command</c> 取值常量（官方「客服输入状态」）。
/// </summary>
/// <remarks>
/// 官方约束：下发输入状态需<b>之前 30 秒内与该用户有过消息交互</b>（否则 <c>45080</c>）；
/// 已在输入状态时<b>不可重复下发</b>（<c>45081</c>）。
/// </remarks>
public static class MpTypingCommands
{
    /// <summary>对用户下发「正在输入」状态。</summary>
    public const string Typing = "Typing";

    /// <summary>取消对用户的「正在输入」状态。</summary>
    public const string CancelTyping = "CancelTyping";
}

/// <summary>
/// 客服聊天记录操作码（官方「获取聊天记录」<c>recordlist[].opercode</c>）。
/// </summary>
public static class MpMsgRecordOperCodes
{
    /// <summary>客服发送信息。</summary>
    public const int WorkerSent = 2002;

    /// <summary>客服接收消息。</summary>
    public const int WorkerReceived = 2003;
}
