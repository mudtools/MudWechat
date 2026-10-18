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
                "approval.get",
                "checkin.record.list",
                "contacts.department.child_ids",
                "contacts.department.get",
                "contacts.department.list",
                "contacts.tag.list",
                "contacts.tag.members",
                "contacts.user.get",
                "contacts.user.list",
                "contacts.user.simple_list",
                "externalcontact.contact.get",
                "externalcontact.contact.list",
                "externalcontact.follow_user.list",
                "externalcontact.groupchat.get",
                "externalcontact.groupchat.list",
                "externalcontact.groupmsg.list",
                "externalcontact.groupmsg.send_result",
                "externalcontact.groupmsg.task_list",
                "externalcontact.moment.comments",
                "externalcontact.moment.list",
                "externalcontact.moment.publish_list",
                "externalcontact.product_album.get",
                "externalcontact.product_album.list",
                "externalcontact.resigned.unassigned_list",
                "externalcontact.statistics.groupchat",
                "externalcontact.statistics.groupchat_by_day",
                "externalcontact.statistics.user_behavior",
                "externalcontact.tag.list",
                "journal.record.get",
                "journal.record.list",
                "kf.account.list",
                "kf.servicer.list",
                "kf.session.get",
                "living.get",
                "living.list",
                "living.watch_stat",
                "meeting.attendees",
                "meeting.get",
                "meeting.list",
                "wedoc.doc.base_info",
            ]);

        // 期望值必须是集合：`.Equal("a", "b", ..., "理由")` 会绑定到 params 重载，把理由当成第 6 个元素。
        WechatToolNames.WriteAll.Should().Equal(
            [
                "contacts.tag.create",
                "externalcontact.groupmsg.send",
                "kf.message.send",
                "message.send",
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
    [InlineData("approval.get", "IWechatWorkApprovalService.GetApprovalDetailAsync", "/cgi-bin/oa/getapprovaldetail", "POST", WechatToolRisk.Read, "approval.read")]
    [InlineData("checkin.record.list", "IWechatWorkInternalCheckinRecordService.GetCheckinDataAsync", "/cgi-bin/checkin/getcheckindata", "POST", WechatToolRisk.Read, "checkin.record.read")]
    [InlineData("contacts.department.child_ids", "IWechatWorkDepartmentsService.GetChildDepartmentIdListAsync", "/cgi-bin/department/simplelist", "GET", WechatToolRisk.Read, "contacts.department.read")]
    [InlineData("contacts.department.get", "IWechatWorkDepartmentsService.GetDepartmentAsync", "/cgi-bin/department/get", "GET", WechatToolRisk.Read, "contacts.department.read")]
    [InlineData("contacts.department.list", "IWechatWorkDepartmentsService.GetDepartmentListAsync", "/cgi-bin/department/list", "GET", WechatToolRisk.Read, "contacts.department.read")]
    [InlineData("contacts.tag.create", "IWechatWorkTagsService.CreateTagAsync", "/cgi-bin/tag/create", "POST", WechatToolRisk.Write, "contacts.tag.write")]
    [InlineData("contacts.tag.list", "IWechatWorkTagsService.GetTagListAsync", "/cgi-bin/tag/list", "GET", WechatToolRisk.Read, "contacts.tag.read")]
    [InlineData("contacts.tag.members", "IWechatWorkTagsService.GetTagMembersAsync", "/cgi-bin/tag/get", "GET", WechatToolRisk.Read, "contacts.tag.read")]
    [InlineData("contacts.user.get", "IWechatWorkUsersService.GetUserAsync", "/cgi-bin/user/get", "GET", WechatToolRisk.Read, "contacts.user.read")]
    [InlineData("contacts.user.list", "IWechatWorkUsersService.GetUserDetailListAsync", "/cgi-bin/user/list", "GET", WechatToolRisk.Read, "contacts.user.read")]
    [InlineData("contacts.user.simple_list", "IWechatWorkUsersService.GetUserSimpleListAsync", "/cgi-bin/user/simplelist", "GET", WechatToolRisk.Read, "contacts.user.read")]
    [InlineData("externalcontact.contact.get", "IWechatWorkExternalContactCustomerService.GetCustomerDetailAsync", "/cgi-bin/externalcontact/get", "GET", WechatToolRisk.Read, "externalcontact.contact.read")]
    [InlineData("externalcontact.contact.list", "IWechatWorkExternalContactCustomerService.GetCustomerListAsync", "/cgi-bin/externalcontact/list", "GET", WechatToolRisk.Read, "externalcontact.contact.read")]
    [InlineData("externalcontact.follow_user.list", "IWechatWorkExternalContactFollowUserService.GetFollowUserListAsync", "/cgi-bin/externalcontact/get_follow_user_list", "GET", WechatToolRisk.Read, "externalcontact.follow_user.read")]
    [InlineData("externalcontact.groupchat.get", "IWechatWorkExternalContactGroupChatService.GetGroupChatDetailAsync", "/cgi-bin/externalcontact/groupchat/get", "POST", WechatToolRisk.Read, "externalcontact.groupchat.read")]
    [InlineData("externalcontact.groupchat.list", "IWechatWorkExternalContactGroupChatService.GetGroupChatListAsync", "/cgi-bin/externalcontact/groupchat/list", "POST", WechatToolRisk.Read, "externalcontact.groupchat.read")]
    [InlineData("externalcontact.moment.comments", "IWechatWorkExternalContactMomentService.GetMomentCommentsAsync", "/cgi-bin/externalcontact/get_moment_comments", "POST", WechatToolRisk.Read, "externalcontact.moment.read")]
    [InlineData("externalcontact.moment.list", "IWechatWorkExternalContactMomentService.GetMomentListAsync", "/cgi-bin/externalcontact/get_moment_list", "POST", WechatToolRisk.Read, "externalcontact.moment.read")]
    [InlineData("externalcontact.moment.publish_list", "IWechatWorkExternalContactMomentService.GetMomentPublishListAsync", "/cgi-bin/externalcontact/get_moment_task", "POST", WechatToolRisk.Read, "externalcontact.moment.read")]
    [InlineData("externalcontact.groupmsg.list", "IWechatWorkExternalContactGroupMsgService.GetGroupMsgListAsync", "/cgi-bin/externalcontact/get_groupmsg_list_v2", "POST", WechatToolRisk.Read, "externalcontact.groupmsg.read")]
    [InlineData("externalcontact.groupmsg.send", "IWechatWorkExternalContactGroupMsgService.AddGroupMsgTemplateAsync", "/cgi-bin/externalcontact/add_msg_template", "POST", WechatToolRisk.Write, "externalcontact.groupmsg.write")]
    [InlineData("externalcontact.groupmsg.send_result", "IWechatWorkExternalContactGroupMsgService.GetGroupMsgSendResultAsync", "/cgi-bin/externalcontact/get_groupmsg_send_result", "POST", WechatToolRisk.Read, "externalcontact.groupmsg.read")]
    [InlineData("externalcontact.groupmsg.task_list", "IWechatWorkExternalContactGroupMsgService.GetGroupMsgTaskListAsync", "/cgi-bin/externalcontact/get_groupmsg_task", "POST", WechatToolRisk.Read, "externalcontact.groupmsg.read")]
    [InlineData("externalcontact.product_album.get", "IWechatWorkExternalContactProductAlbumService.GetProductAlbumAsync", "/cgi-bin/externalcontact/get_product_album", "POST", WechatToolRisk.Read, "externalcontact.product_album.read")]
    [InlineData("externalcontact.product_album.list", "IWechatWorkExternalContactProductAlbumService.GetProductAlbumListAsync", "/cgi-bin/externalcontact/get_product_album_list", "POST", WechatToolRisk.Read, "externalcontact.product_album.read")]
    [InlineData("externalcontact.resigned.unassigned_list", "IWechatWorkExternalContactResignedInheritanceService.GetUnassignedListAsync", "/cgi-bin/externalcontact/get_unassigned_list", "POST", WechatToolRisk.Read, "externalcontact.inheritance.read")]
    [InlineData("externalcontact.statistics.groupchat", "IWechatWorkExternalContactStatisticsService.GetGroupChatStatisticAsync", "/cgi-bin/externalcontact/groupchat/statistic", "POST", WechatToolRisk.Read, "externalcontact.statistics.read")]
    [InlineData("externalcontact.statistics.groupchat_by_day", "IWechatWorkExternalContactStatisticsService.GetGroupChatStatisticGroupByDayAsync", "/cgi-bin/externalcontact/groupchat/statistic_group_by_day", "POST", WechatToolRisk.Read, "externalcontact.statistics.read")]
    [InlineData("externalcontact.statistics.user_behavior", "IWechatWorkExternalContactStatisticsService.GetUserBehaviorDataAsync", "/cgi-bin/externalcontact/get_user_behavior_data", "POST", WechatToolRisk.Read, "externalcontact.statistics.read")]
    [InlineData("externalcontact.tag.list", "IWechatWorkExternalContactTagService.GetCorpTagListAsync", "/cgi-bin/externalcontact/get_corp_tag_list", "POST", WechatToolRisk.Read, "externalcontact.tag.read")]
    [InlineData("journal.record.get", "IWechatWorkInternalJournalService.GetJournalRecordDetailAsync", "/cgi-bin/oa/journal/get_record_detail", "POST", WechatToolRisk.Read, "journal.record.read")]
    [InlineData("journal.record.list", "IWechatWorkInternalJournalService.GetJournalRecordListAsync", "/cgi-bin/oa/journal/get_record_list", "POST", WechatToolRisk.Read, "journal.record.read")]
    [InlineData("kf.account.list", "IWechatWorkKfAccountService.GetAccountListAsync", "/cgi-bin/kf/account/list", "POST", WechatToolRisk.Read, "kf.account.read")]
    [InlineData("kf.message.send", "IWechatWorkKfSessionService.SendTextMessageAsync", "/cgi-bin/kf/send_msg", "POST", WechatToolRisk.Write, "kf.message.write")]
    [InlineData("kf.servicer.list", "IWechatWorkKfServicerService.GetServicerListAsync", "/cgi-bin/kf/servicer/list", "GET", WechatToolRisk.Read, "kf.servicer.read")]
    [InlineData("kf.session.get", "IWechatWorkKfSessionService.GetServiceStateAsync", "/cgi-bin/kf/service_state/get", "POST", WechatToolRisk.Read, "kf.session.read")]
    [InlineData("living.get", "IWechatWorkLivingService.GetLivingInfoAsync", "/cgi-bin/living/get_living_info", "GET", WechatToolRisk.Read, "living.read")]
    [InlineData("living.list", "IWechatWorkLivingService.GetUserAllLivingIdAsync", "/cgi-bin/living/get_user_all_livingid", "POST", WechatToolRisk.Read, "living.read")]
    [InlineData("living.watch_stat", "IWechatWorkLivingService.GetWatchStatAsync", "/cgi-bin/living/get_watch_stat", "POST", WechatToolRisk.Read, "living.read")]
    [InlineData("meeting.attendees", "IWechatWorkInternalMeetingAdvancedService.GetAttendeeListAsync", "/cgi-bin/meeting/get_attendee_list", "POST", WechatToolRisk.Read, "meeting.read")]
    [InlineData("message.send", "IWechatWorkMessageService.SendTextMessageAsync", "/cgi-bin/message/send", "POST", WechatToolRisk.Write, "message.send.write")]
    [InlineData("meeting.get", "IWechatWorkInternalMeetingService.GetMeetingInfoAsync", "/cgi-bin/meeting/get_info", "POST", WechatToolRisk.Read, "meeting.read")]
    [InlineData("meeting.list", "IWechatWorkMeetingService.GetUserMeetingIdListAsync", "/cgi-bin/meeting/get_user_meetingid", "POST", WechatToolRisk.Read, "meeting.read")]
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
