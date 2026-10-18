// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 应用消息执行器：<c>message.send</c>（三形态受控写）与 <c>message.recall</c>（纠错配对）。
/// </summary>
/// <remarks>
/// <para>
/// <b>部分失败必须回传</b>：官方在 <c>errcode = 0</c> 时仍可能返回 <c>invalid_user</c> 等字段（部分收件人无效），
/// 执行器把它们透出，模型才能如实回答「发给谁失败了」，而不是笼统报「已发送」。
/// </para>
/// <para>
/// <b>形态校验按官方口径分档</b>：Text/Markdown 按 <b>UTF-8 字节</b>（中文 1 字 3 字节）；TextCard 的标题/描述
/// 按<b>字符</b>（官方 128/512 字符），跳转链接按字节且必须有协议头。按错口径校验会造成「看似合法、服务端截断」。
/// </para>
/// <para>
/// <b>只开放 touser 轴</b>：按部门/标签群发风险高且难以核对，工具面不提供。
/// </para>
/// </remarks>
/// <param name="message">应用消息 SDK。</param>
public sealed class MessageToolHandlers(IWechatWorkMessageService message)
{
    /// <summary>文本 / markdown 正文的官方字节上限。</summary>
    private const int MaxContentBytes = 2048;

    /// <summary>文本卡片标题的官方字符上限。</summary>
    private const int MaxCardTitleChars = 128;

    /// <summary>文本卡片描述的官方字符上限。</summary>
    private const int MaxCardDescriptionChars = 512;

    /// <summary>文本卡片按钮文字的官方字符上限。</summary>
    private const int MaxCardButtonChars = 4;

    /// <summary>跳转链接的官方字节上限。</summary>
    private const int MaxUrlBytes = 2048;

    private readonly IWechatWorkMessageService _message = message ?? throw new ArgumentNullException(nameof(message));

    /// <summary>执行 <c>message.send</c>（受控写；按 <c>msg_type</c> 分派三种形态）。</summary>
    /// <param name="args">模型侧入参（<c>touser</c> / <c>content</c> / <c>agentid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkMessageSendTool))]
    public async Task<ToolResult> SendAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = MessageSendArgs.Unpack(args);
        var msgType = request.MsgType ?? WechatMessageTypes.Text;

        if (msgType is not (WechatMessageTypes.Text or WechatMessageTypes.Markdown or WechatMessageTypes.TextCard))
        {
            return ToolResult.FromError(
                $"参数非法：msg_type 只能是 Text({WechatMessageTypes.Text}) / Markdown({WechatMessageTypes.Markdown}) / TextCard({WechatMessageTypes.TextCard})。");
        }

        MessageSendResponse response;
        if (msgType == WechatMessageTypes.TextCard)
        {
            var invalid = ValidateCard(request.Title, request.Content, request.Url, request.BtnTxt);
            if (invalid is not null)
            {
                return ToolResult.FromError(invalid);
            }

            response = await _message.SendTextCardMessageAsync(
                new MessageSendTextCardRequest
                {
                    ToUser = request.Touser,
                    AgentId = request.Agentid,
                    MsgType = "textcard",
                    TextCard = new MessageTextCardBody
                    {
                        Title = request.Title,
                        Description = request.Content,
                        Url = request.Url,
                        BtnTxt = string.IsNullOrWhiteSpace(request.BtnTxt) ? null : request.BtnTxt,
                    },
                },
                cancellationToken).ConfigureAwait(false);
        }
        else
        {
            // 官方按 UTF-8 字节计：20 个中文就可能超过 60 字节，必须按字节而不是按字符校验。
            var byteCount = Encoding.UTF8.GetByteCount(request.Content);
            if (byteCount > MaxContentBytes)
            {
                return ToolResult.FromError(
                    $"参数非法：content 超过官方上限（{byteCount} > {MaxContentBytes} 字节），请精简后重发。");
            }

            response = msgType == WechatMessageTypes.Markdown
                ? await _message.SendMarkdownMessageAsync(
                    new MessageSendMarkdownRequest
                    {
                        ToUser = request.Touser,
                        AgentId = request.Agentid,
                        MsgType = "markdown",
                        Markdown = new MessageMarkdownBody { Content = request.Content },
                    },
                    cancellationToken).ConfigureAwait(false)
                : await _message.SendTextMessageAsync(
                    new MessageSendTextRequest
                    {
                        ToUser = request.Touser,
                        AgentId = request.Agentid,
                        MsgType = "text",
                        Safe = request.Safe ?? 0,
                        Text = new MessageTextBody { Content = request.Content },
                    },
                    cancellationToken).ConfigureAwait(false);
        }

        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "发送应用消息失败。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["msg_type"] = msgType,
            ["msgid"] = outcome.Data?.MsgId,
            ["invalid_user"] = outcome.Data?.InvalidUser,
            ["invalid_party"] = outcome.Data?.InvalidParty,
            ["invalid_tag"] = outcome.Data?.InvalidTag,
            ["unlicensed_user"] = outcome.Data?.UnlicensedUser,
        }));
    }

    /// <summary>执行 <c>message.recall</c>（撤回 24 小时内的应用消息）。</summary>
    /// <param name="args">模型侧入参（<c>msgid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkMessageRecallTool))]
    public async Task<ToolResult> RecallAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = MessageRecallArgs.Unpack(args);

        var response = await _message.RecallMessageAsync(
            new RecallMessageRequest { MsgId = request.Msgid },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "撤回应用消息失败。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["msgid"] = request.Msgid,
        }));
    }

    /// <summary>文本卡片的三项官方约束（标题/描述按字符、链接按字节且有协议头、按钮文字按字符）。</summary>
    private static string? ValidateCard(string? title, string content, string? url, string? button)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "参数非法：msg_type=TextCard 时 title 必填（卡片标题，≤ 128 字符）。";
        }

        if (title.Length > MaxCardTitleChars)
        {
            return $"参数非法：title 超过官方上限（{title.Length} > {MaxCardTitleChars} 字符）。";
        }

        if (content.Length > MaxCardDescriptionChars)
        {
            return $"参数非法：TextCard 的 content（描述）超过官方上限（{content.Length} > {MaxCardDescriptionChars} 字符）。";
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            return "参数非法：msg_type=TextCard 时 url 必填（须含 http/https 协议头）。";
        }

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return "参数非法：url 必须包含协议头（http:// 或 https://）。";
        }

        var urlBytes = Encoding.UTF8.GetByteCount(url);
        if (urlBytes > MaxUrlBytes)
        {
            return $"参数非法：url 超过官方上限（{urlBytes} > {MaxUrlBytes} 字节）。";
        }

        if (button is { Length: > MaxCardButtonChars })
        {
            return $"参数非法：btn_txt 超过官方上限（{button.Length} > {MaxCardButtonChars} 字）。";
        }

        return null;
    }
}
