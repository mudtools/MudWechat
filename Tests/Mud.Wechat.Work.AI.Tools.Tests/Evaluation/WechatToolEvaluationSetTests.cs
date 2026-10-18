using System.Text.Json.Nodes;
using FluentAssertions;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools.Generated;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Evaluation;

/// <summary>
/// 评测集（surface fitness，20 条真实请求）：把「模型该选哪个工具、这个工具是否自举得起来」落成可回归的断言。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不是「跑一次模型看它选什么」</b>：那需要真实 LLM（不确定、慢、有成本），只能进 nightly。
/// 本套件守的是<b>前置条件</b>——工具存在、风险档正确、前置工具存在、必填参数不依赖模型拿不到的东西、
/// 描述不是空壳。这五条一旦退化，模型再聪明也选不对；它们全都能在 CI 里确定性验证。
/// </para>
/// <para>
/// <b>自举规则</b>：若某工具把「不透明 ID」（<c>userid</c>/<c>sp_no</c>/<c>msgid</c>/<c>schedule_id</c>/<c>tag_id</c>…）
/// 列为必填，本用例<b>必须</b>声明一个提供该 ID 的前置工具——否则这条链路在真实组织里走不通（模型只能猜或反问）。
/// 宿主直接提供的 ID（<c>template_id</c>/<c>cal_id</c>）例外：它们来自用户在审批应用/日历里的选择。
/// </para>
/// <para>
/// <b>不承诺「模型一定选对」</b>：这是筛选器，不是证明。真正的选择质量要靠夜间评测（本文件已给出用例形态）。
/// </para>
/// </remarks>
public class WechatToolEvaluationSetTests
{
    /// <summary>需要「上一跳工具」提供的不透明 ID（模型无法凭空产出）。</summary>
    private static readonly HashSet<string> OpaqueIds = new(StringComparer.Ordinal)
    {
        "userid",
        "sp_no",
        "msgid",
        "schedule_id",
        "tag_id",
        "docid",
        "creator_userid",
        "external_userid",
    };

    /// <summary>
    /// 工具面无需自举的 ID：① 宿主/用户在对应应用里提供（<c>template_id</c>/<c>cal_id</c>）；
    /// ② 由回合事实注入（<c>userid_list</c> 可填「发起人自己」，装配器每回合给出）。
    /// </summary>
    private static readonly HashSet<string> HostProvidedIds = new(StringComparer.Ordinal)
    {
        "template_id",
        "cal_id",
        "userid_list",
    };

    /// <summary>20 条真实请求 → 期望工具（顺序无关，按请求语义分组）。</summary>
    public static TheoryData<string, string, bool, string[]> Cases => new()
    {
        { "给张三发条消息说下午三点开会", "message.send", true, ["contacts.user.resolve"] },
        { "刚才发错群了，撤回那条消息", "message.recall", true, ["message.send"] },
        { "发一条 markdown 格式的公告", "message.send", true, [] },
        { "帮我约个周会，明早十点，拉上张三李四", "schedule.add", true, ["contacts.user.resolve"] },
        { "把明天的周会取消掉", "schedule.cancel", true, ["schedule.get"] },
        { "看一下这个日程的时间地点", "schedule.get", false, ["schedule.add"] },
        { "我有哪些待审批的单子", "approval.list", false, [] },
        { "这条审批现在走到哪一步了", "approval.get", false, ["approval.list"] },
        { "这个审批模板要填哪些字段", "approval.template.get", false, [] },
        { "帮我提个请假单，后天一天", "approval.apply", true, ["approval.template.get", "contacts.user.resolve"] },
        { "我还有几天年假", "approval.vacation.quota", false, ["contacts.user.resolve"] },
        { "张三的 userid 是多少", "contacts.user.resolve", false, [] },
        { "研发部都有哪些人", "contacts.user.simple_list", false, ["contacts.department.list"] },
        { "看下公司的部门结构", "contacts.department.list", false, [] },
        { "查一下我昨天的打卡记录", "checkin.record.list", false, [] },
        { "看看这个客户最近的跟进记录", "externalcontact.follow_user.list", false, [] },
        { "这个客户是谁，备注是什么", "externalcontact.contact.get", false, ["externalcontact.contact.list"] },
        { "列出我的客户群", "externalcontact.groupchat.list", false, [] },
        { "客户群发任务现在进展如何", "externalcontact.groupmsg.task_list", false, ["externalcontact.groupmsg.list"] },
        { "建一个叫华东-销售的标签", "contacts.tag.create", true, ["contacts.tag.list"] },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void Case_ShouldBeSelectableAndSelfBootstrappable(
        string request,
        string tool,
        bool isWrite,
        string[] prerequisites)
    {
        WechatToolContracts.ByToolName.Should().ContainKey(
            tool, $"请求「{request}」期望工具 {tool}；工具不存在时模型只能拒绝或瞎选");

        var contract = WechatToolContracts.ByToolName[tool];

        contract.Description.Length.Should().BeGreaterThan(
            60, $"「{request}」对应的 {tool} 描述过短——描述是模型唯一的选型依据，空壳描述等于没有工具");

        contract.IsWrite.Should().Be(isWrite, $"{tool} 的读写分类必须与评测前提一致（写面默认关闭）");
        if (isWrite)
        {
            contract.Risk.Should().NotBe(WechatToolRisk.Read, "写工具的风险档不得退化为只读（否则可绕过审批）");
            WechatToolNames.WriteAll.Should().Contain(tool, "写工具必须落在 WriteAll（默认不启用）");
        }
        else
        {
            contract.Risk.Should().Be(WechatToolRisk.Read);
            WechatToolNames.ReadonlyAll.Should().Contain(tool, "只读工具默认启用；不在 ReadonlyAll 会被静默禁用");
        }

        foreach (var prerequisite in prerequisites)
        {
            WechatToolContracts.ByToolName.Should().ContainKey(
                prerequisite, $"「{request}」的链路依赖 {prerequisite}；缺它则该请求在真实组织里走不通");
        }

        var required = RequiredParameters(contract.ParametersSchemaJson);
        foreach (var parameter in required)
        {
            if (HostProvidedIds.Contains(parameter))
            {
                continue;
            }

            if (OpaqueIds.Contains(parameter))
            {
                prerequisites.Should().NotBeEmpty(
                    $"{tool} 的必填参数 {parameter} 是不透明 ID：评测用例必须声明提供它的前置工具，"
                    + "否则模型只能猜 ID 或反问用户（链路不可自举）");
            }
        }
    }

    [Fact]
    public void RateLimitedError_ShouldTellModelHowToChangeCourse()
    {
        // 45009 是工具面最常见的官方拒绝；「换个说法继续撞」与「缩小范围换路」的分野就在这里。
        var hint = WechatApiErrorCatalog.HintOf(45009);

        hint.Should().NotBeNullOrWhiteSpace();
        hint.Should().Contain("缩小", "限流文案必须指向「缩小查询范围」而不是「稍后重试」");
    }

    [Fact]
    public void ApplyChain_ShouldAdvertiseItsPrerequisiteInDescription()
    {
        // 审批提交是唯一「不先读模板就一定错」的写面：描述里必须显式给出上一跳。
        WechatToolContracts.ByToolName["approval.apply"].Description
            .Should().Contain("approval.template.get");
    }

    /// <summary>从参数 Schema 里取必填参数名（Schema 是扁平对象，取 <c>required</c> 数组即可）。</summary>
    private static string[] RequiredParameters(string schemaJson)
    {
        var root = JsonNode.Parse(schemaJson) as JsonObject;
        return root?["required"] is JsonArray required
            ? [.. required.Select(node => node?.ToString() ?? string.Empty).Where(name => name.Length > 0)]
            : [];
    }
}
