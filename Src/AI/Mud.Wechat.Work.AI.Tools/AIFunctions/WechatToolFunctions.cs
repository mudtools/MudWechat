// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.AI;
using Mud.Wechat.Work.AI.Tools.Generated;
using Mud.Wechat.Work.AI.Tools.Security;

namespace Mud.Wechat.Work.AI.Tools.AIFunctions;

/// <summary>
/// 工具面 → MAF <see cref="AIFunction"/> 的投影（<b>按白名单投影</b>：未启用的工具不进模型视野）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么投影集取白名单而不是注册表全集</b>：注册表是「引擎里有什么」，白名单是「本轮允不允许」。
/// 若把全集投给模型，被拒绝的工具会在 tool_call 上反复被尝试（模型看到 schema 就认为可调），
/// 既浪费回合预算也让拒绝路径变成常态；只投启用集则「看不见即不可试」。
/// </para>
/// <para>
/// <b>顺序确定</b>：按工具名排序投影，保证同一策略下提示词与工具列表逐字节稳定（便于回归与缓存）。
/// </para>
/// </remarks>
public static class WechatToolFunctions
{
    /// <summary>按策略投影启用集的全部工具。</summary>
    /// <param name="dispatcher">执行链（提供策略；工具调用最终回到它）。</param>
    /// <returns>已启用且契约齐全的工具函数（顺序按工具名）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dispatcher"/> 为 <see langword="null"/>。</exception>
    public static IReadOnlyList<AIFunction> Create(WechatToolDispatcher dispatcher)
    {
        if (dispatcher is null)
        {
            throw new ArgumentNullException(nameof(dispatcher));
        }

        var functions = new List<AIFunction>();
        foreach (var toolName in dispatcher.Policy.EnabledTools)
        {
            if (!WechatToolContracts.ByToolName.TryGetValue(toolName, out var contract)
                || !WechatToolSchemas.SchemaByToolName.TryGetValue(toolName, out var schema))
            {
                // 契约缺失 = 生成物与策展面不一致（构建期本应拦下）；此处跳过并交由注册表侧的诊断暴露。
                continue;
            }

            functions.Add(new WechatToolFunction(dispatcher, contract.Name, contract.Description, schema));
        }

        return functions;
    }
}
