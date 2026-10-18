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
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 通讯录「部门」域执行器：把 <c>contacts.department.*</c> 三个只读工具的调用落到
/// <see cref="IWechatWorkDepartmentsService"/> 的强类型端点上。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么执行器只认 <c>IReadOnlyDictionary&lt;string, object?&gt;</c></b>：模型侧入参是 JSON，落到执行链时
/// 由生成的 Args 解包器完成「snake_case 键 → 强类型实参」的映射；执行器若也吃强类型，就要在两条路径上各维护一份
/// 参数契约（编译期 Schema 与运行期签名会漂移）。
/// </para>
/// <para>
/// <b>两种失败、两个出口（不得合并）</b>：① <b>上游失败</b>经 <see cref="WechatToolOutcomeErrors"/> 产出结构化载荷
/// （分类/子类/可重试/<c>api_code</c>/<c>[next]</c>）；② <b>参数非法</b>抛
/// <see cref="WechatToolArgumentErrors.Invalid"/>，由执行链回填 <c>Validation/invalid_args</c>。
/// 两者都<b>不得</b>用 <see cref="ToolResult.FromError(string)"/> —— 它不产生 <see cref="ToolResult.Error"/>，
/// 会让失败被结算成成功。业务空（<c>found:false</c> / <c>items:[]</c>）则是<b>成功</b>语义，见 <see cref="WechatToolResults"/>。
/// </para>
/// </remarks>
/// <param name="departments">部门域 SDK（应用上下文由回合作用域给定，执行器不自建令牌上下文）。</param>
public sealed class ContactsDepartmentToolHandlers(IWechatWorkDepartmentsService departments)
{
    private readonly IWechatWorkDepartmentsService _departments =
        departments ?? throw new ArgumentNullException(nameof(departments));

    /// <summary>执行 <c>contacts.department.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>department_id</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsDepartmentGetTool))]
    public async Task<ToolResult> GetDepartmentAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        // 生成的解包器承担「必填校验 + 类型转换」；缺参抛 ArgumentException（执行链归类 Validation/invalid_args）。
        var request = ContactsDepartmentGetArgs.Unpack(args);

        var response = await _departments.GetDepartmentAsync(request.DepartmentId, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsDepartmentGet);
        }

        if (outcome.Data?.Department is not { } department)
        {
            return WechatToolResults.NotFound(WechatToolNames.ContactsDepartmentGet);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["id"] = department.Id,
            ["name"] = department.Name,
            ["name_en"] = department.NameEn,
            ["parentid"] = department.ParentId,
            ["department_leader"] = ToJsonArray(department.DepartmentLeader),
        }));
    }

    /// <summary>执行 <c>contacts.department.list</c>。</summary>
    /// <param name="args">模型侧入参（可选 <c>department_id</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsDepartmentListTool))]
    public async Task<ToolResult> ListDepartmentsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var rootId = ContactsDepartmentListArgs.Unpack(args).DepartmentId;

        var response = await _departments.GetDepartmentListAsync(rootId, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsDepartmentList);
        }

        if (outcome.Data?.Department is not { } list)
        {
            return WechatToolResults.EmptyList(WechatToolNames.ContactsDepartmentList);
        }

        var items = new JsonArray();
        foreach (var department in list)
        {
            // 显式转 JsonNode：走集合 Add(JsonNode?) 重载，避免 JsonArray.Add<T> 的 AOT 动态代码路径（IL3050）。
            items.Add((JsonNode)new JsonObject
            {
                ["id"] = department.Id,
                ["name"] = department.Name,
                ["parentid"] = department.ParentId,
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject { ["department"] = items, ["count"] = items.Count }));
    }

    /// <summary>执行 <c>contacts.department.child_ids</c>。</summary>
    /// <param name="args">模型侧入参（可选 <c>department_id</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsDepartmentChildIdsTool))]
    public async Task<ToolResult> ListChildDepartmentIdsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var rootId = ContactsDepartmentChildIdsArgs.Unpack(args).DepartmentId;

        var response = await _departments.GetChildDepartmentIdListAsync(rootId, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsDepartmentChildIds);
        }

        if (outcome.Data?.DepartmentIds is not { } ids)
        {
            return WechatToolResults.EmptyList(WechatToolNames.ContactsDepartmentChildIds);
        }

        var items = new JsonArray();
        foreach (var item in ids)
        {
            items.Add((JsonNode)new JsonObject { ["id"] = item.Id, ["parentid"] = item.ParentId });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject { ["department_id"] = items, ["count"] = items.Count }));
    }

    private static JsonArray ToJsonArray(IEnumerable<string>? values)
    {
        var array = new JsonArray();
        if (values is null)
        {
            return array;
        }

        foreach (var value in values)
        {
            // 同上：显式走 JsonNode 重载（string 经 JsonValue 装箱为节点）。
            array.Add((JsonNode?)JsonValue.Create(value));
        }

        return array;
    }
}
