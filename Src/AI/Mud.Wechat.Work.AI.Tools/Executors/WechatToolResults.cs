// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.AI.Agent.Tools;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 「查无」的唯一表达：<b>业务空是成功答案</b>，不是上游故障。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须分开</b>：官方接口「成功但业务载荷为空」有两种完全不同的含义 ——
/// ① <b>业务空</b>（权限内确实没有这个对象 / 这一页没有数据）⇒ 模型应当据实回答「没有」；
/// ② <b>信封异常</b>（应有 <c>data</c> 却整块缺失）⇒ 上游行为变了，必须让运维看见。
/// 历史实现把两者混成一个 <c>errcode != 0</c> 分支返回错误文本，于是「查无此人」被模型当成「调用失败」，
/// 转而重试或换个工具乱试。
/// </para>
/// <para>
/// <b>判定点必须在中立读取器之前</b>：<c>WechatApiResultReader.Read</c> 对空载荷显式返回
/// <c>empty_payload</c> 失败（防「空成功被当成查到了空结果」）。这条规则服务于第 ② 类，不能改；
/// 第 ① 类由执行器在调用 <c>Read</c> 之前（或之后读 DTO 自身属性）判定 —— 本类型只提供<b>形状</b>。
/// </para>
/// <para>
/// <b>为什么错误也走不到这里</b>：上游失败一律经 <c>WechatToolOutcomeErrors.From</c>（结构化载荷），
/// 本类型只产出成功结果 —— 两个语义不共用一个出口，模型才能区分「没有」与「失败」。
/// </para>
/// </remarks>
internal static class WechatToolResults
{
    /// <summary>「查无此对象」的固定提示（模型可执行的下一步）。</summary>
    public const string NotFoundHint =
        "未查到该对象：请确认 ID 有效、未被删除，且对象在当前应用可见范围内；"
        + "可先用对应的列表工具取有效 ID，不要原样重复调用。";

    /// <summary>「空页」的固定提示。</summary>
    public const string EmptyListHint =
        "本页没有数据（业务空，不是调用失败）：可放宽时间范围或过滤条件后重试；"
        + "若上游还有下一页，请带上 page_token 继续。";

    /// <summary>单对象查无（成功）：<c>found=false</c> + 可执行提示。</summary>
    /// <param name="toolName">契约工具名（<c>WechatToolNames.*</c>）。</param>
    /// <returns>成功语义的工具结果。</returns>
    public static ToolResult NotFound(string toolName)
        => ToolResultPipeline.OkJson(
            new JsonObject
            {
                ["found"] = false,
                ["tool"] = toolName,
                ["hint"] = NotFoundHint,
            },
            maxLength: 0);

    /// <summary>列表空页（成功）：<c>items=[]</c> + <c>has_more=false</c> + 可执行提示。</summary>
    /// <param name="toolName">契约工具名（<c>WechatToolNames.*</c>）。</param>
    /// <returns>成功语义的工具结果。</returns>
    public static ToolResult EmptyList(string toolName)
    {
        var envelope = ToolResultJsons.PageEnvelope(hasMore: false, pageToken: null);
        envelope["tool"] = toolName;
        envelope["hint"] = EmptyListHint;
        return ToolResultPipeline.OkJson(envelope, maxLength: 0);
    }
}
