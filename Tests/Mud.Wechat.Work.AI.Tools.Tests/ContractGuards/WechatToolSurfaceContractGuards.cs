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
    /// <summary>WT-G1：工具名集合 = 冻结清单（只读段在前、写段在后，各自按名排序）。</summary>
    [Fact]
    public void GeneratedToolNames_ShouldMatchFrozenSet()
    {
        WechatToolNames.ReadonlyAll.Should().Equal(
            [
                "contacts.department.child_ids",
                "contacts.department.get",
                "contacts.department.list",
                "contacts.tag.list",
                "contacts.tag.members",
                "contacts.user.get",
                "contacts.user.list",
                "contacts.user.simple_list",
                "wedoc.doc.base_info",
            ]);

        // 期望值必须是集合：`.Equal("a", "b", ..., "理由")` 会绑定到 params 重载，把理由当成第 6 个元素。
        WechatToolNames.WriteAll.Should().Equal(
            [
                "contacts.tag.create",
                "wedoc.doc.create",
                "wedoc.doc.delete",
                "wedoc.doc.rename",
                "wedoc.doc.share",
            ],
            "写面逐个登记：白名单默认封闭与「高危写恒需审批」都依赖这份分类");

        WechatToolNames.All.Should().Equal([.. WechatToolNames.ReadonlyAll, .. WechatToolNames.WriteAll]);
        WechatToolNames.IsWriteTool("wedoc.doc.delete").Should().BeTrue();
        WechatToolNames.IsWriteTool("wedoc.doc.base_info").Should().BeFalse("POST 方法不等于写操作（微文档读端点也是 POST）");
    }

    /// <summary>WT-G1：名字契约表 / 契约表 / Schema 表三处同源同序。</summary>
    [Fact]
    public void GeneratedSurfaces_ShouldStayInSync()
    {
        // 契约表的顺序是按名排序（与 Names 的「只读段 + 写段」不同），故按集合比对。
        WechatToolContracts.AllNames.Should().BeEquivalentTo(WechatToolNames.All);
        WechatToolContracts.ByToolName.Keys.Should().BeEquivalentTo(WechatToolNames.All);
        WechatToolSchemas.SchemaByToolName.Keys.Should().BeEquivalentTo(WechatToolNames.All);
    }

    /// <summary>WT-G2：HTTP 方法 / 路由 / 风险 / 身份 / scope 全部来自 SDK 与声明事实。</summary>
    /// <param name="toolName">工具名。</param>
    /// <param name="source">SDK 源。</param>
    /// <param name="route">由 SDK 特性派生的路由。</param>
    /// <param name="httpMethod">HTTP 方法（由 SDK 上的 <c>[Get]</c>/<c>[Post]</c> 派生）。</param>
    /// <param name="risk">风险档（由 <c>IsWrite</c> 与写动词表派生）。</param>
    /// <param name="scope">工具声明的权限点。</param>
    [Theory]
    [InlineData("contacts.department.child_ids", "IWechatWorkDepartmentsService.GetChildDepartmentIdListAsync", "/cgi-bin/department/simplelist", "GET", WechatToolRisk.Read, "contacts.department.read")]
    [InlineData("contacts.department.get", "IWechatWorkDepartmentsService.GetDepartmentAsync", "/cgi-bin/department/get", "GET", WechatToolRisk.Read, "contacts.department.read")]
    [InlineData("contacts.department.list", "IWechatWorkDepartmentsService.GetDepartmentListAsync", "/cgi-bin/department/list", "GET", WechatToolRisk.Read, "contacts.department.read")]
    [InlineData("contacts.tag.create", "IWechatWorkTagsService.CreateTagAsync", "/cgi-bin/tag/create", "POST", WechatToolRisk.Write, "contacts.tag.write")]
    [InlineData("contacts.tag.list", "IWechatWorkTagsService.GetTagListAsync", "/cgi-bin/tag/list", "GET", WechatToolRisk.Read, "contacts.tag.read")]
    [InlineData("contacts.tag.members", "IWechatWorkTagsService.GetTagMembersAsync", "/cgi-bin/tag/get", "GET", WechatToolRisk.Read, "contacts.tag.read")]
    [InlineData("contacts.user.get", "IWechatWorkUsersService.GetUserAsync", "/cgi-bin/user/get", "GET", WechatToolRisk.Read, "contacts.user.read")]
    [InlineData("contacts.user.list", "IWechatWorkUsersService.GetUserDetailListAsync", "/cgi-bin/user/list", "GET", WechatToolRisk.Read, "contacts.user.read")]
    [InlineData("contacts.user.simple_list", "IWechatWorkUsersService.GetUserSimpleListAsync", "/cgi-bin/user/simplelist", "GET", WechatToolRisk.Read, "contacts.user.read")]
    [InlineData("wedoc.doc.base_info", "IWechatWorkWedocService.GetDocumentBaseInfoAsync", "/cgi-bin/wedoc/get_doc_base_info", "POST", WechatToolRisk.Read, "wedoc.doc.read")]
    [InlineData("wedoc.doc.create", "IWechatWorkWedocService.CreateDocumentAsync", "/cgi-bin/wedoc/create_doc", "POST", WechatToolRisk.Write, "wedoc.doc.write")]
    [InlineData("wedoc.doc.delete", "IWechatWorkWedocService.DeleteDocumentAsync", "/cgi-bin/wedoc/del_doc", "POST", WechatToolRisk.HighRiskWrite, "wedoc.doc.write")]
    [InlineData("wedoc.doc.rename", "IWechatWorkWedocService.RenameDocumentAsync", "/cgi-bin/wedoc/rename_doc", "POST", WechatToolRisk.Write, "wedoc.doc.write")]
    [InlineData("wedoc.doc.share", "IWechatWorkWedocService.ShareDocumentAsync", "/cgi-bin/wedoc/doc_share", "POST", WechatToolRisk.Write, "wedoc.doc.share")]
    public void Contracts_ShouldDeriveFromSdkFacts(
        string toolName,
        string source,
        string route,
        string httpMethod,
        WechatToolRisk risk,
        string scope)
    {
        var contract = WechatToolContracts.ByToolName[toolName];

        contract.Name.Should().Be(toolName);
        contract.SdkSource.Should().Be(source);
        contract.Route.Should().Be(route);
        contract.HttpMethod.Should().Be(httpMethod);
        contract.Risk.Should().Be(risk);
        contract.IsWrite.Should().Be(risk != WechatToolRisk.Read);
        contract.Identity.Should().Be("tenant", "企微工具统一走企业级 access_token（身份轴由剖面标记表推导）");
        contract.RequiredScopes.Should().Contain(scope);
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
