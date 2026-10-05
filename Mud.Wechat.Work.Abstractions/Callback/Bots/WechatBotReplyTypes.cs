// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人应答类型常量（<c>msgtype</c> / <c>response_type</c> 取值，官方 101031 / 101138 / 101463）。
/// </summary>
/// <remarks>
/// 各传输的<b>支持面不同</b>：HTTP 被动回复支持 <see cref="Text"/>（仅欢迎语）/ <see cref="TemplateCard"/> /
/// <see cref="Stream"/> / <see cref="StreamWithTemplateCard"/> / <see cref="UpdateTemplateCard"/>；
/// HTTP 主动回复（101138）支持 <see cref="Markdown"/> / <see cref="TemplateCard"/>；
/// 长连接回复另支持 <see cref="Markdown"/> 与媒体消息（<c>media_id</c>）。
/// </remarks>
public static class WechatBotReplyTypes
{
    /// <summary>文本消息（官方仅「进入会话回调事件」支持被动回复；长连接经 <c>aibot_respond_welcome_msg</c>）。</summary>
    public const string Text = "text";

    /// <summary>markdown 消息（HTTP 主动回复 101138 与长连接回复支持；被动回复不支持）。</summary>
    public const string Markdown = "markdown";

    /// <summary>流式消息（回调地址模式下企微反复推送刷新、机器人逐次写回累计内容）。</summary>
    public const string Stream = "stream";

    /// <summary>流式消息 + 模板卡片（首次回复必须携带 <c>stream.id</c>；同一消息的卡片只能回复一次）。</summary>
    public const string StreamWithTemplateCard = "stream_with_template_card";

    /// <summary>模板卡片消息（复用消息域 <c>TemplateCardBody</c> 结构体）。</summary>
    public const string TemplateCard = "template_card";

    /// <summary>更新模板卡片应答的 <c>response_type</c> 取值（模板卡片事件专用，<c>task_id</c> 须与回调一致）。</summary>
    public const string UpdateTemplateCard = "update_template_card";
}
