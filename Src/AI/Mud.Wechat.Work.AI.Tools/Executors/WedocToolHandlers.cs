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
using Mud.Wechat.Work.DataModels.Wedoc;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 微文档域执行器：<c>wedoc.doc.*</c> 五个工具（1 只读 + 3 受控写 + 1 高危写）落到
/// <see cref="IWechatWorkWedocService"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>互斥参数在这里收口</b>：官方要求 <c>docid</c> 与 <c>formid</c> 只能填其中一个，Schema 无法表达
/// 「恰好一个」，故每个写工具在调用前经 <see cref="ResolveTargetId"/> 校验；不合法时回填可读错误而不是让上游 errcode 说话
/// （上游对「两个都填」的错误码含义不稳定，模型难以据此纠错）。
/// </para>
/// <para>
/// <b>写工具的成功文本不编造业务事实</b>：改名/删除的上游响应无业务负载，执行器只回填
/// <c>{ok: true, docid|formid}</c>；模型可据 target 复述，但不得假装读到了新名称或删除结果。
/// </para>
/// </remarks>
/// <param name="wedoc">微文档域 SDK（应用上下文由回合作用域给定）。</param>
public sealed class WedocToolHandlers(IWechatWorkWedocService wedoc)
{
    private readonly IWechatWorkWedocService _wedoc = wedoc ?? throw new ArgumentNullException(nameof(wedoc));

    /// <summary>执行 <c>wedoc.doc.base_info</c>。</summary>
    /// <param name="args">模型侧入参（<c>docid</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkWedocDocBaseInfoTool))]
    public async Task<ToolResult> GetBaseInfoAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = WedocDocBaseInfoArgs.Unpack(args);

        var response = await _wedoc
            .GetDocumentBaseInfoAsync(new GetWedocDocumentBaseInfoRequest { Docid = request.Docid }, cancellationToken)
            .ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.DocBaseInfo is not { } info)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "文档不存在或无权访问（doc_base_info 为空）。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["docid"] = info.Docid,
            ["doc_name"] = info.DocName,
            ["doc_type"] = info.DocType,
            ["create_time"] = info.CreateTime,
            ["modify_time"] = info.ModifyTime,
        }));
    }

    /// <summary>执行 <c>wedoc.doc.create</c>。</summary>
    /// <param name="args">模型侧入参（<c>doc_name</c>/<c>doc_type</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkWedocDocCreateTool))]
    public async Task<ToolResult> CreateDocumentAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = WedocDocCreateArgs.Unpack(args);

        if (!string.IsNullOrWhiteSpace(request.Spaceid) && string.IsNullOrWhiteSpace(request.Fatherid))
        {
            return ToolResult.FromError("指定 spaceid 时必须同时给出 fatherid（官方约束：空间内必须落在某个父目录）。");
        }

        var response = await _wedoc.CreateDocumentAsync(
            new CreateWedocDocumentRequest
            {
                DocName = request.DocName,
                DocType = request.DocType,
                Spaceid = request.Spaceid,
                Fatherid = request.Fatherid,
                AdminUsers = request.AdminUsers is { Length: > 0 } admins ? [.. admins] : null,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data is not { } created)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "文档创建失败：上游未返回 docid。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["docid"] = created.Docid,
            ["url"] = created.Url,
            ["doc_name"] = request.DocName,
            ["doc_type"] = request.DocType,
        }));
    }

    /// <summary>执行 <c>wedoc.doc.rename</c>。</summary>
    /// <param name="args">模型侧入参（<c>new_name</c> 必填；<c>docid</c>/<c>formid</c> 二选一）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkWedocDocRenameTool))]
    public async Task<ToolResult> RenameDocumentAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = WedocDocRenameArgs.Unpack(args);
        if (!TryResolveTarget(request.Docid, request.Formid, out var docid, out var formid, out var targetError))
        {
            return ToolResult.FromError(targetError!);
        }

        var response = await _wedoc.RenameDocumentAsync(
            new RenameWedocDocumentRequest { Docid = docid, Formid = formid, NewName = request.NewName },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "重命名失败。");
        }

        return Ok(docid, formid, ("new_name", request.NewName));
    }

    /// <summary>执行 <c>wedoc.doc.delete</c>（高危写）。</summary>
    /// <param name="args">模型侧入参（<c>docid</c>/<c>formid</c> 二选一）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkWedocDocDeleteTool))]
    public async Task<ToolResult> DeleteDocumentAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = WedocDocDeleteArgs.Unpack(args);
        if (!TryResolveTarget(request.Docid, request.Formid, out var docid, out var formid, out var targetError))
        {
            return ToolResult.FromError(targetError!);
        }

        var response = await _wedoc.DeleteDocumentAsync(
            new DeleteWedocDocumentRequest { Docid = docid, Formid = formid },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "删除失败。");
        }

        return Ok(docid, formid);
    }

    /// <summary>执行 <c>wedoc.doc.share</c>。</summary>
    /// <param name="args">模型侧入参（<c>docid</c>/<c>formid</c> 二选一）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkWedocDocShareTool))]
    public async Task<ToolResult> ShareDocumentAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = WedocDocShareArgs.Unpack(args);
        if (!TryResolveTarget(request.Docid, request.Formid, out var docid, out var formid, out var targetError))
        {
            return ToolResult.FromError(targetError!);
        }

        var response = await _wedoc.ShareDocumentAsync(
            new ShareWedocDocumentRequest { Docid = docid, Formid = formid },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data is not { } shared)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "分享链接获取失败（share_url 为空）。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["share_url"] = shared.ShareUrl,
            ["docid"] = docid,
            ["formid"] = formid,
        }));
    }

    /// <summary>docid / formid 互斥校验：恰好给出一个。</summary>
    private static bool TryResolveTarget(
        string? docid,
        string? formid,
        out string? resolvedDocId,
        out string? resolvedFormId,
        out string? error)
    {
        var hasDoc = !string.IsNullOrWhiteSpace(docid);
        var hasForm = !string.IsNullOrWhiteSpace(formid);

        switch (hasDoc, hasForm)
        {
            case (true, false):
                resolvedDocId = docid;
                resolvedFormId = null;
                error = null;
                return true;
            case (false, true):
                resolvedDocId = null;
                resolvedFormId = formid;
                error = null;
                return true;
            case (true, true):
                resolvedDocId = null;
                resolvedFormId = null;
                error = "参数 docid 与 formid 只能填其中一个（官方约束），请只提供其一。";
                return false;
            default:
                resolvedDocId = null;
                resolvedFormId = null;
                error = "缺少参数：docid 与 formid 必须提供其中之一。";
                return false;
        }
    }

    private static ToolResult Ok(string? docid, string? formid, params (string Key, string? Value)[] extras)
    {
        var payload = new JsonObject { ["ok"] = true, ["docid"] = docid, ["formid"] = formid };
        foreach (var (key, value) in extras)
        {
            // 泛型实参必须是具体类型：以 object 实例化会产出 JsonValueCustomized<object>，
            // 其 WriteTo 在无 TypeInfoResolver 的默认 options 上调用 MakeReadOnly() ⇒ 运行期 InvalidOperationException。
            payload[key] = JsonValue.Create(value);
        }

        return ToolResult.FromText(ToolResultJson.ToText(payload));
    }
}
