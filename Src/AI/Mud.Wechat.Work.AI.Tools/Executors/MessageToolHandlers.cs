// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 应用消息执行器：<c>message.send</c>（受控写，文本）。
/// </summary>
/// <remarks>
/// <para>
/// <b>部分失败必须回传</b>：官方在 <c>errcode = 0</c> 时仍可能返回 <c>invalid_user</c> 等字段（部分收件人无效），
/// 执行器把它们透出，模型才能如实回答「发给谁失败了」，而不是笼统报「已发送」。
/// </para>
/// <para>
/// <b>只开放 touser 轴</b>：按部门/标签群发风险高且难以核对，工具面不提供。
/// </para>
/// </remarks>
/// <param name="message">应用消息 SDK。</param>
public sealed class MessageToolHandlers(IWechatWorkMessageService message)
{
    /// <summary>官方单条文本上限（字节）。</summary>
    private const int MaxContentBytes = 2048;

    private readonly IWechatWorkMessageService _message = message ?? throw new ArgumentNullException(nameof(message));

    /// <summary>执行 <c>message.send</c>（受控写）。</summary>
    /// <param name="args">模型侧入参（<c>touser</c> / <c>content</c> / <c>agentid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkMessageSendTool))]
    public async Task<ToolResult> SendAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = MessageSendArgs.Unpack(args);

        // content / touser 的「必填且非空白」由声明面 Required 与生成的解包器收口（缺失/空白抛参数类错误）。
        // 官方按 UTF-8 字节计，20 字符的中文就可能超过 60 字节，必须按字节而不是按字符校验。
        var byteCount = System.Text.Encoding.UTF8.GetByteCount(request.Content);
        if (byteCount > MaxContentBytes)
        {
            return ToolResult.FromError($"参数非法：content 超过官方上限（{byteCount} > {MaxContentBytes} 字节），请精简后重发。");
        }

        var response = await _message.SendTextMessageAsync(
            new MessageSendTextRequest
            {
                ToUser = request.Touser,
                AgentId = request.Agentid,
                MsgType = "text",
                Safe = request.Safe ?? 0,
                Text = new MessageTextBody { Content = request.Content },
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "发送应用消息失败。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["msgid"] = outcome.Data?.MsgId,
            ["invalid_user"] = outcome.Data?.InvalidUser,
            ["invalid_party"] = outcome.Data?.InvalidParty,
            ["invalid_tag"] = outcome.Data?.InvalidTag,
            ["unlicensed_user"] = outcome.Data?.UnlicensedUser,
        }));
    }
}
