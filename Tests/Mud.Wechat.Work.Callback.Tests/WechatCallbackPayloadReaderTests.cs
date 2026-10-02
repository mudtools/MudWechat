// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 载荷读取器测试（取代旧 <c>WechatCallbackEventParserTests</c>）：
/// 官方样报文逐字段解析、6 态 <see cref="WechatPayloadReadStatus"/>、双报文布局、
/// <c>Values</c> 全量袋与敏感节点脱敏、结构族互斥与 <c>Event</c> 键隔离。
/// </summary>
/// <remarks>
/// 本文件同时是「上游 <c>PayloadFieldMapGenerator</c> 在真实消费方可用」的端到端证据：
/// 载荷映射表由生成器产出，若特性声明有误（元素名/形态/转换器方法）将在编译期报
/// <c>PAYLOAD004/006/007</c>，无需等到运行期。
/// </remarks>
public class WechatCallbackPayloadReaderTests
{
    private static WechatPayloadContractRegistry CreateRegistry()
    {
        var registry = new WechatPayloadContractRegistry();
        OfficialPayloadContracts.RegisterAll(registry);
        return registry;
    }

    private static WechatCallbackPayloadReader CreateReader(WechatPayloadContractRegistry? registry = null)
        => new(registry ?? CreateRegistry());

    private static WechatCallbackEvent Event(string changeType, string plainXml) => new()
    {
        Event = WechatCallbackEventTypes.ChangeContact,
        ChangeType = changeType,
        DecryptedXml = plainXml,
    };

    private static WechatCallbackEvent ChainEvent(string changeType, string plainXml) => new()
    {
        Event = WechatCallbackEventTypes.ChangeChain,
        ChangeType = changeType,
        DecryptedXml = plainXml,
    };

    // ------------------------------------------------------------------ 成员事件

    [Fact]
    public void Read_ShouldMapOfficialMemberFields()
    {
        var evt = Event(WechatCallbackEventTypes.CreateUser,
            "<xml><ToUserName><![CDATA[ww-corp]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
            "<CreateTime>1700000000</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
            "<Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[create_user]]></ChangeType>" +
            "<UserID><![CDATA[zhangsan]]></UserID><Name><![CDATA[张三]]></Name>" +
            "<Department>1,2,3</Department><MainDepartment>1</MainDepartment>" +
            "<IsLeaderInDept>1,0,0</IsLeaderInDept><DirectLeader><![CDATA[lisi|wangwu]]></DirectLeader>" +
            "<Position><![CDATA[工程师]]></Position><Mobile><![CDATA[13800000000]]></Mobile>" +
            "<Gender>1</Gender><Email><![CDATA[z@corp.com]]></Email><Status>1</Status>" +
            "<Avatar><![CDATA[http://a/b.png]]></Avatar><Alias><![CDATA[z]]></Alias>" +
            "<Telephone><![CDATA[010-123]]></Telephone><Address><![CDATA[北京]]></Address>" +
            "<ExtAttr><Item Name=\"工号\" Type=\"0\"><Text>A1001</Text></Item></ExtAttr></xml>");

        var result = CreateReader().Read<ContactUserChangedPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.UserId.Should().Be("zhangsan");
        payload.Name.Should().Be("张三");
        payload.DepartmentIds.Should().Equal(1L, 2L, 3L);
        payload.MainDepartmentId.Should().Be("1");
        payload.LeaderInDeptFlags.Should().Equal(1, 0, 0);
        payload.DirectLeaderIds.Should().Equal("lisi", "wangwu");
        payload.Position.Should().Be("工程师");
        payload.Mobile.Should().Be("13800000000");
        payload.Gender.Should().Be(WechatUserGender.Male, "官方 Gender=1 ⇒ 男性（Method 绑定经 F1 传 n?.Value）");
        payload.Email.Should().Be("z@corp.com");
        payload.Status.Should().Be(WechatUserStatus.Activated, "官方 Status=1 ⇒ 已激活");
        payload.Avatar.Should().Be("http://a/b.png");
        payload.Alias.Should().Be("z");
        payload.Telephone.Should().Be("010-123");
        payload.Address.Should().Be("北京");
        payload.ExtAttr.Should().HaveCount(1);
        payload.ExtAttr[0].Name.Should().Be("工号");
        payload.ExtAttr[0].Type.Should().Be("0");
        payload.ExtAttr[0].Value.Should().Be("A1001");
    }

    [Fact]
    public void Read_ShouldReturnNullFields_WhenSensitiveFieldsAbsent()
    {
        // 2022-08-15 后新 URL：通讯录助手仅回调 UserID/Department 子集 —— 缺失必须解析为 null 不抛。
        var evt = Event(WechatCallbackEventTypes.CreateUser,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[create_user]]></ChangeType>" +
            "<UserID><![CDATA[lisi]]></UserID><Department>1,2</Department></xml>");

        var result = CreateReader().Read<ContactUserChangedPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.UserId.Should().Be("lisi");
        payload.DepartmentIds.Should().Equal(1L, 2L);
        payload.Name.Should().BeNull("敏感字段未授权时不返回，处理器不得假设必有值");
        payload.Mobile.Should().BeNull();
        payload.Gender.Should().BeNull();
        payload.ExtAttr.Should().BeEmpty();
    }

    [Fact]
    public void Read_ShouldMapUpdatedAndDeletedMember()
    {
        var updated = CreateReader().Read<ContactUserChangedPayload>(
            Event(WechatCallbackEventTypes.UpdateUser,
                "<xml><ChangeType><![CDATA[update_user]]></ChangeType><UserID><![CDATA[old-id]]></UserID>" +
                "<NewUserID><![CDATA[new-id]]></NewUserID><Department>5</Department></xml>"));
        updated.Payload!.UserId.Should().Be("old-id");
        updated.Payload!.NewUserId.Should().Be("new-id");

        var deleted = CreateReader().Read<ContactUserChangedPayload>(
            Event(WechatCallbackEventTypes.DeleteUser,
                "<xml><ChangeType><![CDATA[delete_user]]></ChangeType><UserID><![CDATA[zhangsan]]></UserID></xml>"));
        deleted.Payload!.UserId.Should().Be("zhangsan");
        deleted.Payload!.Name.Should().BeNull();
    }

    // ------------------------------------------------------------------ 部门 / 标签

    [Fact]
    public void Read_ShouldMapPartyFields()
    {
        var result = CreateReader().Read<ContactPartyChangedPayload>(
            Event(WechatCallbackEventTypes.CreateParty,
                "<xml><ChangeType><![CDATA[create_party]]></ChangeType><Id>2</Id>" +
                "<Name><![CDATA[研发部]]></Name><ParentId>1</ParentId><Order>10</Order></xml>"));

        result.Payload!.PartyId.Should().Be("2");
        result.Payload!.Name.Should().Be("研发部");
        result.Payload!.ParentId.Should().Be("1");
        result.Payload!.Order.Should().Be(10L);
    }

    [Fact]
    public void Read_ShouldMapTagLists()
    {
        var result = CreateReader().Read<ContactTagChangedPayload>(
            Event(WechatCallbackEventTypes.UpdateTag,
                "<xml><ChangeType><![CDATA[update_tag]]></ChangeType><TagId>7</TagId>" +
                "<AddUserItems><![CDATA[zhangsan,lisi]]></AddUserItems><DelUserItems><![CDATA[wangwu]]></DelUserItems>" +
                "<AddPartyItems>4,5</AddPartyItems><DelPartyItems>6</DelPartyItems></xml>"));

        var payload = result.Payload!;
        payload.TagId.Should().Be("7");
        payload.AddedUserIds.Should().Equal(new[] { "zhangsan", "lisi" }, "UserId 串映射为 List<string>");
        payload.RemovedUserIds.Should().Equal("wangwu");
        payload.AddedPartyIds.Should().Equal(new[] { 4L, 5L }, "部门 id 串映射为 List<long>");
        payload.RemovedPartyIds.Should().Equal(6L);
    }

    // ------------------------------------------------------------------ 上下游（结构族互斥 + 键隔离）

    [Fact]
    public void Read_ShouldMapChainNestedLists()
    {
        var result = CreateReader().Read<ChainChangedPayload>(
            ChainEvent(WechatCallbackEventTypes.CreateGroup,
                "<xml><Event><![CDATA[change_chain]]></Event><ChangeType><![CDATA[create_group]]></ChangeType>" +
                "<ChainId><![CDATA[chain-xyz]]></ChainId><GroupIds><GroupId>5</GroupId><GroupId>6</GroupId></GroupIds></xml>"));

        result.Payload!.ChainId.Should().Be("chain-xyz");
        result.Payload!.GroupIds.Should().Equal("5", "6");
        result.Payload!.CorpIds.Should().BeEmpty();
    }

    [Fact]
    public void Read_ShouldRejectForeignFamily_WhenEventKeyIsolated()
    {
        // B4：change_contact 事件即便 ChangeType 同名也不得进入上下游载荷（Event 键隔离）。
        var reader = CreateReader();
        var evt = Event(WechatCallbackEventTypes.CreateChain,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[create_chain]]></ChangeType>" +
            "<ChainId><![CDATA[c]]></ChainId></xml>");

        reader.Read<ChainChangedPayload>(evt).Status
            .Should().Be(WechatPayloadReadStatus.ContractMismatch,
                "契约登记了 RequiredEvent=change_chain；change_contact 报文不得命中上下游载荷");
    }

    // ------------------------------------------------------------------ 异步任务（双布局）

    [Fact]
    public void Read_ShouldSupportBothBatchJobLayouts()
    {
        var reader = CreateReader();

        var topLevel = reader.Read<BatchJobCompletedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.BatchJobResult,
            DecryptedXml = "<xml><Event><![CDATA[batch_job_result]]></Event><JobId><![CDATA[job-abc]]></JobId>" +
                           "<JobType><![CDATA[replace_user]]></JobType><ErrCode>0</ErrCode><ErrMsg>ok</ErrMsg></xml>",
        });
        topLevel.Payload!.JobId.Should().Be("job-abc", "通讯录布局：字段在顶层");

        var wrapped = reader.Read<BatchJobCompletedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.BatchJobResult,
            DecryptedXml = "<xml><Event><![CDATA[batch_job_result]]></Event>" +
                           "<BatchJob><JobId><![CDATA[chain-job-1]]></JobId><JobType><![CDATA[import_chain_contact]]></JobType>" +
                           "<ErrCode>0</ErrCode><ErrMsg>ok</ErrMsg></BatchJob></xml>",
        });
        wrapped.Payload!.JobId.Should().Be("chain-job-1", "上下游布局：字段包在 BatchJob 内（ScopeFallback）");
        wrapped.Payload!.JobType.Should().Be("import_chain_contact");
    }

    // ------------------------------------------------------------------ 键不匹配 / 非法输入

    [Fact]
    public void Read_ShouldReturnKeyMismatch_WhenEventTypeKeyAbsent()
    {
        var result = CreateReader().Read<ContactUserChangedPayload>(new WechatCallbackEvent
        {
            DecryptedXml = "<xml><Other>x</Other></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.KeyMismatch);
        result.Payload.Should().BeNull();
    }

    [Fact]
    public void Read_ShouldReturnMalformedPayload_WhenPlainTextIsNotXml()
    {
        var result = CreateReader().Read<ContactUserChangedPayload>(
            Event(WechatCallbackEventTypes.CreateUser, "not-xml-at-all"));

        result.Status.Should().Be(WechatPayloadReadStatus.MalformedPayload, "明文非 XML 返回状态而非抛异常");
    }

    [Fact]
    public void Read_ShouldReturnEnvelopeMissing_WhenPlainTextAbsent()
    {
        var result = CreateReader().Read<ContactUserChangedPayload>(
            new WechatCallbackEvent { ChangeType = WechatCallbackEventTypes.CreateUser, DecryptedXml = null });

        result.Status.Should().Be(WechatPayloadReadStatus.EnvelopeMissing);
    }

    [Fact]
    public void Read_ShouldReturnEnvelopeMissing_WhenEventNull()
    {
        CreateReader().Read<ContactUserChangedPayload>(null!)
            .Status.Should().Be(WechatPayloadReadStatus.EnvelopeMissing, "不抛（沿用旧解析器语义）");
    }

    [Fact]
    public void Read_ShouldReturnContractMismatch_WhenPayloadTypeDiffers()
    {
        var result = CreateReader().Read<ContactPartyChangedPayload>(
            Event(WechatCallbackEventTypes.CreateUser, "<xml><UserID>u</UserID></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.ContractMismatch);
        result.Diagnostic.Should().Contain("ContactUserChangedPayload");
    }

    // ------------------------------------------------------------------ 未知事件降级（ADR-4）

    [Fact]
    public void Read_ShouldFallbackToGeneric_WhenEventKeyNotRegistered()
    {
        var result = CreateReader().Read<GenericCallbackPayload>(new WechatCallbackEvent
        {
            Event = "change_external_contact",
            DecryptedXml = "<xml><Event><![CDATA[change_external_contact]]></Event>" +
                           "<ExternalUserID><![CDATA[wo-x]]></ExternalUserID><State><![CDATA[s]]></State></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.GenericFallback);
        result.Payload.Should().NotBeNull();
        result.Payload!.Values.Should().ContainKey("ExternalUserID", "Values 全量袋 ⇒ 官方新增字段免升级读取");
        result.Payload!.Values["ExternalUserID"].Should().Be("wo-x");
        result.Payload!.Values.Should().ContainKey("State");
    }

    // ------------------------------------------------------------------ 事件键级开放面（ADR-15）

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, false)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, false)]
    public void ChainContracts_ShouldOpenOnlyForInternal(
        WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.CreateChain, out var contract).Should().BeTrue();
        var evt = ChainEvent(WechatCallbackEventTypes.CreateChain, "<xml><ChainId>c</ChainId></xml>");

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "上下游变更族官方仅向企业自建应用开放（95796）");
    }

    [Theory]
    [InlineData(WechatAppType.Internal)]
    [InlineData(WechatAppType.ThirdParty)]
    [InlineData(WechatAppType.Provider)]
    public void ContactContracts_ShouldOpenForAllThreeModes(WechatAppType appType)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.CreateUser, out var contract).Should().BeTrue();
        var evt = Event(WechatCallbackEventTypes.CreateUser, "<xml><UserID>u</UserID></xml>");

        contract!.IsOpenFor(evt, appType, WechatCallbackChannel.App).Should().BeTrue(
            "通讯录变更族三类应用均开放 ⇒ 一份契约覆盖三模式（ADR-14）");
    }

    [Fact]
    public void Contracts_ShouldRejectForeignEvent_WhenRequiredEventDeclared()
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.CreateUser, out var contract).Should().BeTrue();

        // 同名 ChangeType 但 Event 为 change_chain ⇒ 族前置条件拦截（B4）。
        var foreign = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeChain,
            ChangeType = WechatCallbackEventTypes.CreateUser,
        };

        contract!.IsOpenFor(foreign, WechatAppType.Internal, WechatCallbackChannel.App).Should().BeFalse();
    }
}
