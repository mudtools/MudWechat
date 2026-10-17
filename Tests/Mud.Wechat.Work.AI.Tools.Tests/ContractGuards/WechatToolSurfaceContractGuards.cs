using FluentAssertions;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Generated;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.ContractGuards;

/// <summary>
/// 工具面契约守卫（权威描述，改动策展面须同批更新本文件）。
/// </summary>
/// <remarks>
/// <para>WT-G1：<b>名字契约冻结</b> —— 已策展工具集与只读/写划分是模型可见面，增删即破坏宿主提示词与白名单。</para>
/// <para>WT-G2：<b>契约由 SDK 事实派生</b> —— <c>Source</c> 指向的 SDK 端点决定 HTTP 方法/路由/输出 Schema，
/// 手改契约或 Source 写错都会让本守卫失配（编译期还能拦 Source 解析，路由漂移只有本守卫能拦）。</para>
/// <para>WT-G3：<b>Schema 带厂商扩展</b> —— <c>x-wechat</c> 子块（风险/身份/scope/source）是授权与审计的读面。</para>
/// </remarks>
public class WechatToolSurfaceContractGuards
{
    /// <summary>WT-G1：工具名集合 = 冻结清单（顺序即生成序，按名字排序）。</summary>
    [Fact]
    public void GeneratedToolNames_ShouldMatchFrozenSet()
    {
        WechatToolNames.All.Should().Equal(
            "contacts.department.child_ids",
            "contacts.department.get",
            "contacts.department.list",
            "contacts.tag.list",
            "contacts.tag.members",
            "contacts.user.get",
            "contacts.user.list",
            "contacts.user.simple_list");

        WechatToolNames.ReadonlyAll.Should().Equal(WechatToolNames.All, "当前策展面只有只读工具");
        WechatToolNames.WriteAll.Should().BeEmpty("写工具须在 S3 显式开放（白名单默认封闭）");
    }

    /// <summary>WT-G1：名字契约表 / 契约表 / Schema 表三处同源同序。</summary>
    [Fact]
    public void GeneratedSurfaces_ShouldStayInSync()
    {
        WechatToolContracts.AllNames.Should().Equal(WechatToolNames.All);
        WechatToolContracts.ByToolName.Keys.Should().BeEquivalentTo(WechatToolNames.All);
        WechatToolSchemas.SchemaByToolName.Keys.Should().BeEquivalentTo(WechatToolNames.All);
    }

    /// <summary>WT-G2：HTTP 方法 / 路由 / 风险 / 身份 / scope 全部来自 SDK 与声明事实。</summary>
    /// <param name="toolName">工具名。</param>
    /// <param name="source">SDK 源。</param>
    /// <param name="route">由 SDK 特性派生的路由。</param>
    [Theory]
    [InlineData("contacts.department.child_ids", "IWechatWorkDepartmentsService.GetChildDepartmentIdListAsync", "/cgi-bin/department/simplelist")]
    [InlineData("contacts.department.get", "IWechatWorkDepartmentsService.GetDepartmentAsync", "/cgi-bin/department/get")]
    [InlineData("contacts.department.list", "IWechatWorkDepartmentsService.GetDepartmentListAsync", "/cgi-bin/department/list")]
    [InlineData("contacts.tag.list", "IWechatWorkTagsService.GetTagListAsync", "/cgi-bin/tag/list")]
    [InlineData("contacts.tag.members", "IWechatWorkTagsService.GetTagMembersAsync", "/cgi-bin/tag/get")]
    [InlineData("contacts.user.get", "IWechatWorkUsersService.GetUserAsync", "/cgi-bin/user/get")]
    [InlineData("contacts.user.list", "IWechatWorkUsersService.GetUserDetailListAsync", "/cgi-bin/user/list")]
    [InlineData("contacts.user.simple_list", "IWechatWorkUsersService.GetUserSimpleListAsync", "/cgi-bin/user/simplelist")]
    public void Contracts_ShouldDeriveFromSdkFacts(string toolName, string source, string route)
    {
        var contract = WechatToolContracts.ByToolName[toolName];

        contract.Name.Should().Be(toolName);
        contract.SdkSource.Should().Be(source);
        contract.Route.Should().Be(route);
        contract.HttpMethod.Should().Be("GET");
        contract.Risk.Should().Be(WechatToolRisk.Read);
        contract.IsWrite.Should().BeFalse();
        contract.Identity.Should().Be("tenant", "企微工具统一走企业级 access_token（身份轴由剖面标记表推导）");
        var expectedScope = toolName switch
        {
            var name when name.StartsWith("contacts.user", StringComparison.Ordinal) => "contacts.user.read",
            var name when name.StartsWith("contacts.tag", StringComparison.Ordinal) => "contacts.tag.read",
            _ => "contacts.department.read",
        };
        contract.RequiredScopes.Should().Contain(expectedScope);
        contract.Description.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>WT-G3：每个 Schema 都带 <c>x-wechat</c> 与 <c>output_schema</c>（后者由 SDK 返回类型派生）。</summary>
    [Fact]
    public void Schemas_ShouldCarryWechatExtensionAndOutputSchema()
    {
        foreach (var (toolName, schema) in WechatToolSchemas.SchemaByToolName)
        {
            schema.Should().Contain("\"x-wechat\"", $"{toolName} 的授权与审计读面依赖该扩展块");
            schema.Should().Contain("\"output_schema\"", $"{toolName} 缺少输出契约会让模型只能靠描述猜返回结构");
            schema.Should().Contain("\"required_scopes\"");
        }
    }

    /// <summary>WT-G1：风险枚举成员名须与层 1 词表的三档一一对应。</summary>
    [Fact]
    public void RiskEnum_ShouldMatchPlatformVocabulary()
    {
        Enum.GetNames<WechatToolRisk>().Should().Equal("Read", "Write", "HighRiskWrite");
    }
}
