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

    /// <summary>
    /// 官方 path 100080 的安全管理事件：<c>Event = security</c>，事件键取 <c>ChangeType</c>。
    /// </summary>
    private static WechatCallbackEvent SecurityEvent(string changeType, string plainXml) => new()
    {
        Event = WechatCallbackEventTypes.Security,
        ChangeType = changeType,
        DecryptedXml = plainXml,
    };

    /// <summary>
    /// 官方 path 94670 的微信客服事件：<c>Event</c> 节点自身即事件键（无 <c>ChangeType</c> 分组段）。
    /// </summary>
    private static WechatCallbackEvent KfEvent(string eventKey, string plainXml) => new()
    {
        Event = eventKey,
        DecryptedXml = plainXml,
    };

    /// <summary>
    /// 官方 path 90240 的消息与事件：<c>Event</c> 节点自身即事件键（无 <c>ChangeType</c> 分组段）。
    /// </summary>
    private static WechatCallbackEvent SelfEvent(string eventKey, string plainXml) => new()
    {
        Event = eventKey,
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

    // ------------------------------------------------------------------ 安全管理族（信封外无业务字段）

    [Fact]
    public void Read_ShouldMatchDomainIpChanged_WithEnvelopeOnlyMessage()
    {
        // 官方 100080 样报文：标准信封 + Event=security + ChangeType=change_domain_ip，参数表无业务字段。
        var result = CreateReader().Read<SecurityDomainIpChangedPayload>(
            SecurityEvent(WechatCallbackEventTypes.ChangeDomainIp,
                "<xml><ToUserName><![CDATA[ww-corp]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                "<CreateTime>1403610513</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[security]]></Event><ChangeType><![CDATA[change_domain_ip]]></ChangeType></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched, "信封外无业务字段的载荷仍须按契约命中并产出实例");
        result.Payload!.Should().NotBeNull();
        result.EventTypeKey.Should().Be(WechatCallbackEventTypes.ChangeDomainIp);
    }

    [Fact]
    public void Read_ShouldRejectForeignFamily_WhenEventEnvelopeDiffers()
    {
        // B4：Event=change_contact 报文即便 ChangeType 撞名也不得进入安全管理载荷（RequiredEvent 隔离）。
        var reader = CreateReader();
        var evt = Event(WechatCallbackEventTypes.ChangeDomainIp,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[change_domain_ip]]></ChangeType></xml>");

        reader.Read<SecurityDomainIpChangedPayload>(evt).Status
            .Should().Be(WechatPayloadReadStatus.ContractMismatch,
                "契约登记了 RequiredEvent=security；change_contact 报文不得命中安全管理载荷");
    }

    // ------------------------------------------------------------------ 微信客服族

    [Fact]
    public void Read_ShouldMapKfMsgOrEventNotification()
    {
        // 官方 94670 样报文：外层仅 Token + OpenKfId，内容须调 sync_msg 拉取（三模式文档同构，ADR-14）。
        var result = CreateReader().Read<KfMsgOrEventPayload>(
            KfEvent(WechatCallbackEventTypes.KfMsgOrEvent,
                "<xml><ToUserName><![CDATA[ww12345678910]]></ToUserName><CreateTime>1348831860</CreateTime>" +
                "<MsgType><![CDATA[event]]></MsgType><Event><![CDATA[kf_msg_or_event]]></Event>" +
                "<Token><![CDATA[ENCApHxnGDNAVNY4AaSJKj4Tb5mwsEMzxhFmHVGcra996NR]]></Token>" +
                "<OpenKfId><![CDATA[wkxxxxxxx]]></OpenKfId></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.Token.Should().Be("ENCApHxnGDNAVNY4AaSJKj4Tb5mwsEMzxhFmHVGcra996NR",
            "官方 Token 为 sync_msg 拉取校验令牌（10 分钟内有效）");
        payload.OpenKfId.Should().Be("wkxxxxxxx");
        result.EventTypeKey.Should().Be(WechatCallbackEventTypes.KfMsgOrEvent, "无 ChangeType 分组段 ⇒ Event 即事件键");
    }

    [Fact]
    public void Read_ShouldFallBackToGeneric_WhenKfAccountAuthChangeNotRegistered()
    {
        // kf_account_auth_change 的 AuthAdd/DelOpenKfId 为同级重名多节点形态，超出声明面（ADR-4 降级）：
        // 未登记契约 ⇒ 读取器返回 GenericCallbackPayload（GenericFallback 是正常降级，不是失败）。
        var reader = CreateReader();
        var result = reader.Read<GenericCallbackPayload>(
            KfEvent(WechatCallbackEventTypes.KfAccountAuthChange,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                "<CreateTime>1348831860</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[kf_account_auth_change]]></Event>" +
                "<AuthAddOpenKfId><![CDATA[wkxxxx1]]></AuthAddOpenKfId>" +
                "<AuthAddOpenKfId><![CDATA[wkxxxx2]]></AuthAddOpenKfId>" +
                "<AuthDelOpenKfId><![CDATA[wkxxxx3]]></AuthDelOpenKfId></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.GenericFallback,
            "官方键未登记契约 ⇒ 降级为通用载荷（宿主以 Values/原文读取；全量列表须解析 DecryptedXml）");
        result.Payload!.Values["AuthAddOpenKfId"].Should().Be("wkxxxx2",
            "Values 对同名重复子节点取最后一个（已文档化语义；首个值须从 DecryptedXml 原文获取）");
        result.Payload!.Values["AuthDelOpenKfId"].Should().Be("wkxxxx3");
    }

    [Fact]
    public void Read_ShouldReturnNullToken_WhenKfNotificationOmitsOptionalNodes()
    {
        // 权限分层：字段缺失 ⇒ null 不抛（处理器不得假设 Token/OpenKfId 必有值）。
        var result = CreateReader().Read<KfMsgOrEventPayload>(
            KfEvent(WechatCallbackEventTypes.KfMsgOrEvent,
                "<xml><Event><![CDATA[kf_msg_or_event]]></Event><OpenKfId><![CDATA[wkA]]></OpenKfId></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.Token.Should().BeNull();
        result.Payload!.OpenKfId.Should().Be("wkA");
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
            Event = "host_private_event",
            DecryptedXml = "<xml><Event><![CDATA[host_private_event]]></Event>" +
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

    // ---------------------------------------- 官方 path 90240 消息与事件（三模式正文一致）

    [Fact]
    public void Read_ShouldMapMenuKeyEvent_WhenClickEvent()
    {
        // 官方 90240「点击菜单拉取消息的事件推送」样报文。
        var result = CreateReader().Read<PlainEventPayload>(
            SelfEvent(WechatCallbackEventTypes.Click,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[FromUser]]></FromUserName>" +
                "<CreateTime>123456789</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[click]]></Event><EventKey><![CDATA[EVENTKEY]]></EventKey>" +
                "<AgentID>1</AgentID></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.EventKey.Should().Be("EVENTKEY");
        result.Payload!.AgentId.Should().Be("1");
    }

    [Fact]
    public void Read_ShouldReturnNullEventKey_WhenSubscribeOmitsNode()
    {
        // 官方 90240「成员关注及取消关注事件」：报文无 EventKey 节点（参数表标注「该值为空」）。
        var result = CreateReader().Read<PlainEventPayload>(
            SelfEvent(WechatCallbackEventTypes.Subscribe,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[UserID]]></FromUserName>" +
                "<CreateTime>1348831860</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[subscribe]]></Event><AgentID>1</AgentID></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.EventKey.Should().BeNull("关注事件无 EventKey 节点 ⇒ 缺失即 null，处理器不得假设必有值");
        result.Payload!.AgentId.Should().Be("1");
    }

    [Fact]
    public void Read_ShouldMapScanCodeInfo_WhenScanCodeEvent()
    {
        // 官方 90240「扫码推事件的事件推送」样报文。
        var result = CreateReader().Read<MenuScanCodePayload>(
            SelfEvent(WechatCallbackEventTypes.ScanCodePush,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[FromUser]]></FromUserName>" +
                "<CreateTime>1408090502</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[scancode_push]]></Event><EventKey><![CDATA[6]]></EventKey>" +
                "<ScanCodeInfo><ScanType><![CDATA[qrcode]]></ScanType><ScanResult><![CDATA[1]]></ScanResult></ScanCodeInfo>" +
                "<AgentID>1</AgentID></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.EventKey.Should().Be("6");
        payload.ScanCodeInfo.Should().NotBeNull();
        payload.ScanCodeInfo!.ScanType.Should().Be("qrcode");
        payload.ScanCodeInfo!.ScanResult.Should().Be("1");
        payload.AgentId.Should().Be("1");
    }

    [Fact]
    public void Read_ShouldReturnNullScanCodeInfo_WhenContainerAbsent()
    {
        var result = CreateReader().Read<MenuScanCodePayload>(
            SelfEvent(WechatCallbackEventTypes.ScanCodeWaitMsg,
                "<xml><Event><![CDATA[scancode_waitmsg]]></Event><EventKey><![CDATA[6]]></EventKey></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.ScanCodeInfo.Should().BeNull("容器节点缺失 ⇒ 二级结构为 null，不抛");
    }

    [Fact]
    public void Read_ShouldMapPicsInfo_WhenPicEvent()
    {
        // 官方 90240「弹出系统拍照发图的事件推送」样报文（三层嵌套 PicList/item/PicMd5Sum）。
        var result = CreateReader().Read<MenuPicPayload>(
            SelfEvent(WechatCallbackEventTypes.PicSysPhoto,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[FromUser]]></FromUserName>" +
                "<CreateTime>1408090651</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[pic_sysphoto]]></Event><EventKey><![CDATA[6]]></EventKey>" +
                "<SendPicsInfo><Count>1</Count><PicList><item>" +
                "<PicMd5Sum><![CDATA[1b5f7c23b5bf75682a53e7b6d163e185]]></PicMd5Sum>" +
                "</item></PicList></SendPicsInfo><AgentID>1</AgentID></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.SendPicsInfo.Should().NotBeNull();
        payload.SendPicsInfo!.Count.Should().Be(1L);
        payload.SendPicsInfo!.PicList.Should().HaveCount(1);
        payload.SendPicsInfo!.PicList[0].PicMd5Sum.Should().Be("1b5f7c23b5bf75682a53e7b6d163e185");
        payload.SendPicsInfo!.PicList.Should().OnlyContain(item => !string.IsNullOrEmpty(item.PicMd5Sum));
    }

    [Fact]
    public void Read_ShouldMapSendLocationInfo_WhenLocationSelectEvent()
    {
        // 官方 90240「弹出地理位置选择器的事件推送」样报文（坐标为小数、Poiname 为空串）。
        var result = CreateReader().Read<MenuLocationSelectPayload>(
            SelfEvent(WechatCallbackEventTypes.LocationSelect,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[FromUser]]></FromUserName>" +
                "<CreateTime>1408091189</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[location_select]]></Event><EventKey><![CDATA[6]]></EventKey>" +
                "<SendLocationInfo><Location_X><![CDATA[23]]></Location_X><Location_Y><![CDATA[113]]></Location_Y>" +
                "<Scale><![CDATA[15]]></Scale><Label><![CDATA[ 广州市海珠区客村艺苑路 106号]]></Label>" +
                "<Poiname><![CDATA[]]></Poiname></SendLocationInfo>" +
                "<AgentID>1</AgentID><AppType><![CDATA[wxwork]]></AppType></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.EventKey.Should().Be("6");
        payload.SendLocationInfo.Should().NotBeNull();
        payload.SendLocationInfo!.LocationX.Should().Be(23d, "官方坐标为小数 ⇒ ParseReal（Number<long> 只解析整数）");
        payload.SendLocationInfo!.LocationY.Should().Be(113d);
        payload.SendLocationInfo!.Scale.Should().Be(15L);
        payload.SendLocationInfo!.Label.Should().Be(" 广州市海珠区客村艺苑路 106号");
        payload.SendLocationInfo!.Poiname.Should().BeNullOrEmpty("官方示例 Poiname 为空串 ⇒ Text 归一为 null");
        payload.AppType.Should().Be("wxwork");
    }

    [Fact]
    public void Read_ShouldMapDecimalCoordinates_WhenLocationReported()
    {
        // 官方 90240「上报地理位置」样报文（Latitude/Longitude/Precision 均为小数）。
        var result = CreateReader().Read<LocationReportedPayload>(
            SelfEvent(WechatCallbackEventTypes.Location,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[FromUser]]></FromUserName>" +
                "<CreateTime>123456789</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[LOCATION]]></Event><Latitude>23.104</Latitude><Longitude>113.320</Longitude>" +
                "<Precision>65.000</Precision><AgentID>1</AgentID>" +
                "<AppType><![CDATA[wxwork]]></AppType></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.Latitude.Should().Be(23.104);
        payload.Longitude.Should().Be(113.320);
        payload.Precision.Should().Be(65.000);
        payload.AgentId.Should().Be("1");
        payload.AppType.Should().Be("wxwork");
    }

    [Fact]
    public void Read_ShouldMapApprovalInfo_WhenOpenApprovalChange()
    {
        // 官方 90240「审批状态通知事件」样报文（业务字段全在 ApprovalInfo 包装节点内）。
        var result = CreateReader().Read<ApprovalStatusChangedPayload>(
            SelfEvent(WechatCallbackEventTypes.OpenApprovalChange,
                "<xml><ToUserName><![CDATA[wwddddccc7775555aaa]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                "<CreateTime>1527838022</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[open_approval_change]]></Event><AgentID>1</AgentID><ApprovalInfo>" +
                "<ThirdNo><![CDATA[201806010001]]></ThirdNo><OpenSpName><![CDATA[付款]]></OpenSpName>" +
                "<OpenTemplateId><![CDATA[1234567890]]></OpenTemplateId><OpenSpStatus>1</OpenSpStatus>" +
                "<ApplyTime>1527837645</ApplyTime><ApplyUserName><![CDATA[xiaoming]]></ApplyUserName>" +
                "<ApplyUserId><![CDATA[1]]></ApplyUserId><ApplyUserParty><![CDATA[产品部]]></ApplyUserParty>" +
                "<ApplyUserImage><![CDATA[http://www.qq.com/xxx.png]]></ApplyUserImage>" +
                "<ApprovalNodes><ApprovalNode><NodeStatus>1</NodeStatus><NodeAttr>1</NodeAttr><NodeType>1</NodeType>" +
                "<Items><Item><ItemName><![CDATA[xiaohong]]></ItemName><ItemUserId><![CDATA[2]]></ItemUserId>" +
                "<ItemImage><![CDATA[http://www.qq.com/xxx.png]]></ItemImage><ItemStatus>1</ItemStatus>" +
                "<ItemSpeech><![CDATA[]]></ItemSpeech><ItemOpTime>0</ItemOpTime></Item></Items>" +
                "</ApprovalNode></ApprovalNodes>" +
                "<NotifyNodes><NotifyNode><ItemName><![CDATA[xiaogang]]></ItemName>" +
                "<ItemUserId><![CDATA[3]]></ItemUserId><ItemImage><![CDATA[http://www.qq.com/xxx.png]]></ItemImage>" +
                "</NotifyNode></NotifyNodes><approverstep>0</approverstep></ApprovalInfo></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.ThirdNo.Should().Be("201806010001");
        payload.OpenSpName.Should().Be("付款");
        payload.OpenTemplateId.Should().Be("1234567890");
        payload.OpenSpStatus.Should().Be(1L);
        payload.ApplyTime.Should().Be(1527837645L);
        payload.ApplyUserName.Should().Be("xiaoming");
        payload.ApplyUserId.Should().Be("1");
        payload.ApplyUserParty.Should().Be("产品部");
        payload.ApplyUserImage.Should().Be("http://www.qq.com/xxx.png");
        payload.ApproverStep.Should().Be(0L, "官方节点名为全小写 approverstep");

        payload.ApprovalNodes.Should().HaveCount(1);
        payload.ApprovalNodes[0].NodeStatus.Should().Be(1L);
        payload.ApprovalNodes[0].NodeAttr.Should().Be(1L);
        payload.ApprovalNodes[0].NodeType.Should().Be(1L);
        payload.ApprovalNodes[0].Items.Should().HaveCount(1);
        payload.ApprovalNodes[0].Items[0].ItemName.Should().Be("xiaohong");
        payload.ApprovalNodes[0].Items[0].ItemUserId.Should().Be("2");
        payload.ApprovalNodes[0].Items[0].ItemStatus.Should().Be(1L);
        payload.ApprovalNodes[0].Items[0].ItemSpeech.Should().BeNullOrEmpty("官方 ItemSpeech 为空串 ⇒ Text 归一为 null");
        payload.ApprovalNodes[0].Items[0].ItemOpTime.Should().Be(0L);

        payload.NotifyNodes.Should().HaveCount(1);
        payload.NotifyNodes[0].ItemName.Should().Be("xiaogang");
        payload.NotifyNodes[0].ItemUserId.Should().Be("3");
    }

    [Fact]
    public void Read_ShouldScopeIntoApprovalInfo_WhenOnlyWrapperPresent()
    {
        // ScopeFallback：根无任何映射元素 ⇒ 作用域下移到 ApprovalInfo（与 batch_job_result 的 BatchJob 同机制）。
        var result = CreateReader().Read<ApprovalStatusChangedPayload>(
            SelfEvent(WechatCallbackEventTypes.OpenApprovalChange,
                "<xml><Event><![CDATA[open_approval_change]]></Event><AgentID>1</AgentID>" +
                "<ApprovalInfo><ThirdNo><![CDATA[n-1]]></ThirdNo><OpenSpStatus>3</OpenSpStatus></ApprovalInfo></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.ThirdNo.Should().Be("n-1", "作用域在 ApprovalInfo 内 ⇒ 包装节点子元素可绑定");
        payload.OpenSpStatus.Should().Be(3L);
        payload.ApplyUserName.Should().BeNull("权限分层/字段缺失 ⇒ null，处理器不得假设必有值");
        payload.ApprovalNodes.Should().BeEmpty();
        payload.NotifyNodes.Should().BeEmpty();
        // 根级 AgentID 不在 ApprovalInfo 作用域内 ⇒ 单作用域绑定下不建该字段
        // （应用归属由回调路由的 AppKey 承载），见 ApprovalStatusChangedPayload 的类注释。
    }

    [Fact]
    public void Read_ShouldMapTemplateCardSelectedItems_WhenCardEvent()
    {
        // 官方 90240「模板卡片事件推送」样报文（SelectedItems 含二级 OptionIds 嵌套）。
        var reader = CreateReader();

        var full = reader.Read<TemplateCardEventPayload>(
            SelfEvent(WechatCallbackEventTypes.TemplateCardEvent,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[FromUser]]></FromUserName>" +
                "<CreateTime>123456789</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[template_card_event]]></Event><EventKey><![CDATA[key111]]></EventKey>" +
                "<TaskId><![CDATA[taskid111]]></TaskId><CardType><![CDATA[text_notice]]></CardType>" +
                "<ResponseCode><![CDATA[ResponseCode]]></ResponseCode><AgentID>1</AgentID><SelectedItems>" +
                "<SelectedItem><QuestionKey><![CDATA[QuestionKey1]]></QuestionKey><OptionIds>" +
                "<OptionId><![CDATA[OptionId1]]></OptionId><OptionId><![CDATA[OptionId2]]></OptionId>" +
                "</OptionIds></SelectedItem><SelectedItem><QuestionKey><![CDATA[QuestionKey2]]></QuestionKey>" +
                "<OptionIds><OptionId><![CDATA[OptionId3]]></OptionId><OptionId><![CDATA[OptionId4]]></OptionId>" +
                "</OptionIds></SelectedItem></SelectedItems></xml>"));

        full.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = full.Payload!;
        payload.EventKey.Should().Be("key111");
        payload.TaskId.Should().Be("taskid111");
        payload.CardType.Should().Be("text_notice");
        payload.ResponseCode.Should().Be("ResponseCode");
        payload.AgentId.Should().Be("1");
        payload.SelectedItems.Should().HaveCount(2);
        payload.SelectedItems[0].QuestionKey.Should().Be("QuestionKey1");
        payload.SelectedItems[0].OptionIds.Should().Equal(new[] { "OptionId1", "OptionId2" });
        payload.SelectedItems[1].QuestionKey.Should().Be("QuestionKey2");
        payload.SelectedItems[1].OptionIds.Should().Equal(new[] { "OptionId3", "OptionId4" });

        // 右上角菜单事件与按钮事件共用载荷，仅少 SelectedItems 节点（可空超集）。
        var menu = reader.Read<TemplateCardEventPayload>(
            SelfEvent(WechatCallbackEventTypes.TemplateCardMenuEvent,
                "<xml><Event><![CDATA[template_card_menu_event]]></Event><EventKey><![CDATA[key111]]></EventKey>" +
                "<TaskId><![CDATA[taskid111]]></TaskId><CardType><![CDATA[text_notice]]></CardType>" +
                "<ResponseCode><![CDATA[ResponseCode]]></ResponseCode><AgentID>1</AgentID></xml>"));

        menu.Status.Should().Be(WechatPayloadReadStatus.Matched);
        menu.Payload!.SelectedItems.Should().BeEmpty("右上角菜单事件不携带 SelectedItems ⇒ 空列表（不抛）");
        menu.Payload!.EventKey.Should().Be("key111");
    }

    [Fact]
    public void Read_ShouldMapEffectTime_WhenAlertEvent()
    {
        var withTime = CreateReader().Read<AgentAlertPayload>(
            SelfEvent(WechatCallbackEventTypes.InactiveAlert,
                "<xml><Event><![CDATA[inactive_alert]]></Event><AgentID>1</AgentID>" +
                "<EffectTime>1764518400</EffectTime></xml>"));

        withTime.Status.Should().Be(WechatPayloadReadStatus.Matched);
        withTime.Payload!.EffectTime.Should().Be(1764518400L);
        withTime.Payload!.AgentId.Should().Be("1");

        var withoutTime = CreateReader().Read<AgentAlertPayload>(
            SelfEvent(WechatCallbackEventTypes.LowActiveAlert,
                "<xml><Event><![CDATA[low_active_alert]]></Event><AgentID>1</AgentID></xml>"));

        withoutTime.Payload!.EffectTime.Should().BeNull();
        withoutTime.Payload!.AgentId.Should().Be("1");
    }

    [Fact]
    public void Read_ShouldRejectForeignPayloadType_WhenEventKeyBelongsToOtherContract()
    {
        // 事件键已登记到别的载荷 ⇒ 请求的载荷类型不匹配（宿主接线错误），不是降级。
        var result = CreateReader().Read<PlainEventPayload>(
            SelfEvent(WechatCallbackEventTypes.Location, "<xml><Event><![CDATA[LOCATION]]></Event></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.ContractMismatch);
        result.Diagnostic.Should().Contain(nameof(LocationReportedPayload));
    }

    // ---------------------------------------- 事件键级开放面（ADR-15）：90240 三模式差异

    [Theory]
    [InlineData(WechatAppType.Internal, true)]
    [InlineData(WechatAppType.ThirdParty, true)]
    [InlineData(WechatAppType.Provider, true)]
    public void SubscribeContract_ShouldOpenForAllThreeModes(WechatAppType appType, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.Subscribe, out var contract).Should().BeTrue();
        var evt = SelfEvent(WechatCallbackEventTypes.Subscribe, "<xml><Event><![CDATA[subscribe]]></Event></xml>");

        contract!.IsOpenFor(evt, appType, WechatCallbackChannel.App).Should().Be(expected,
            "90240 / 90376 / 96468 三份文档正文一致 ⇒ 关注事件三类应用均开放（ADR-14）");
    }

    [Theory]
    [InlineData(WechatAppType.Internal, true)]
    [InlineData(WechatAppType.ThirdParty, false)]
    [InlineData(WechatAppType.Provider, false)]
    public void ShareAgentChangeContract_ShouldOpenOnlyForInternal(WechatAppType appType, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.ShareAgentChange, out var contract).Should().BeTrue();
        var evt = SelfEvent(WechatCallbackEventTypes.ShareAgentChange,
            "<xml><Event><![CDATA[share_agent_change]]></Event></xml>");

        contract!.IsOpenFor(evt, appType, WechatCallbackChannel.App).Should().Be(expected,
            "官方触发时机为「把自建应用共享给下级企业」⇒ 仅企业自建应用（90240 企业互联共享应用事件回调）");
    }

    [Theory]
    [InlineData(WechatAppType.Internal, true)]
    [InlineData(WechatAppType.ThirdParty, true)]
    [InlineData(WechatAppType.Provider, false)]
    public void OpenApprovalChangeContract_ShouldExcludeProvider(WechatAppType appType, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.OpenApprovalChange, out var contract).Should().BeTrue();
        var evt = SelfEvent(WechatCallbackEventTypes.OpenApprovalChange,
            "<xml><Event><![CDATA[open_approval_change]]></Event></xml>");

        contract!.IsOpenFor(evt, appType, WechatCallbackChannel.App).Should().Be(expected,
            "官方触发时机为「自建/第三方应用调用审批流程引擎」⇒ 不含服务商代开发（90240 审批状态通知事件）");
    }

    // ---------------------------------------- 客户联系变更族 / 获客助手族（92130/92277/96361/97299/98958/99485）

    /// <summary>第三方应用的指令回调（套件信封）：无 <c>Event</c> 节点，族事件值在 <c>InfoType</c> 节点（官方 92277）。</summary>
    private static WechatCallbackEvent SuiteEvent(string infoType, string? changeType, string plainXml) => new()
    {
        InfoType = infoType,
        ChangeType = changeType,
        DecryptedXml = plainXml,
    };

    [Fact]
    public void Read_ShouldMapExternalContactFields_WhenAddExternalContact()
    {
        // 官方 92130「添加企业客户事件」样报文（企业自建 Event 信封）。
        var result = CreateReader().Read<ExternalContactChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalContact,
            ChangeType = "add_external_contact",
            DecryptedXml = "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<CreateTime>1403610513</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[change_external_contact]]></Event>" +
                           "<ChangeType><![CDATA[add_external_contact]]></ChangeType>" +
                           "<UserID><![CDATA[zhangsan]]></UserID>" +
                           "<ExternalUserID><![CDATA[woAJ2GCAAAXtWyujaWJHDDGi0mAAAA]]></ExternalUserID>" +
                           "<State><![CDATA[teststate]]></State>" +
                           "<WelcomeCode><![CDATA[WELCOMECODE]]></WelcomeCode></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.EventTypeKey.Should().Be("change_external_contact", "客户联系族以族事件值为键");
        var payload = result.Payload!;
        payload.UserId.Should().Be("zhangsan");
        payload.ExternalUserId.Should().Be("woAJ2GCAAAXtWyujaWJHDDGi0mAAAA");
        payload.State.Should().Be("teststate");
        payload.WelcomeCode.Should().Be("WELCOMECODE");
        payload.Source.Should().BeNull("仅删除企业客户事件携带 Source");
        payload.FailReason.Should().BeNull("仅接替失败事件携带 FailReason");
    }

    [Fact]
    public void Read_ShouldMapSameKey_WhenThirdPartySuiteEnvelope()
    {
        // 官方 92277 样报文（第三方指令回调，套件信封）：外层事件值在 InfoType ⇒ 与 Event 信封同键。
        var evt = SuiteEvent(WechatCallbackEventTypes.ChangeExternalContact, "add_external_contact",
            "<xml><SuiteId><![CDATA[ww4asffe99e54c0f4c]]></SuiteId>" +
            "<AuthCorpId><![CDATA[wxf8b4f85f3a794e77]]></AuthCorpId>" +
            "<InfoType><![CDATA[change_external_contact]]></InfoType><TimeStamp>1403610513</TimeStamp>" +
            "<ChangeType><![CDATA[add_external_contact]]></ChangeType>" +
            "<UserID><![CDATA[zhangsan]]></UserID>" +
            "<ExternalUserID><![CDATA[woAJ2GCAAAXtWyujaWJHDDGi0mACH71w]]></ExternalUserID>" +
            "<State><![CDATA[teststate]]></State></xml>");

        evt.EventTypeKey.Should().Be("change_external_contact", "套件信封与 Event 信封产出同一事件键（三模式键统一）");
        evt.EventFamily.Should().Be(WechatCallbackEventFamily.ExternalContactChange,
            "套件信封的 InfoType 承载客户联系族事件值，不得误判为授权族");

        var result = CreateReader().Read<ExternalContactChangedPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.UserId.Should().Be("zhangsan");
        result.Payload!.ExternalUserId.Should().Be("woAJ2GCAAAXtWyujaWJHDDGi0mACH71w");
        result.Payload!.WelcomeCode.Should().BeNull("第三方样报文无 WelcomeCode 节点 ⇒ null");
    }

    [Fact]
    public void Read_ShouldMapSourceAndFailReason_WhenDeleteOrTransferFail()
    {
        var reader = CreateReader();

        var deleted = reader.Read<ExternalContactChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalContact,
            ChangeType = "del_external_contact",
            DecryptedXml = "<xml><Event><![CDATA[change_external_contact]]></Event>" +
                           "<ChangeType><![CDATA[del_external_contact]]></ChangeType>" +
                           "<UserID><![CDATA[zhangsan]]></UserID>" +
                           "<ExternalUserID><![CDATA[wo-x]]></ExternalUserID>" +
                           "<Source><![CDATA[DELETE_BY_TRANSFER]]></Source></xml>",
        });
        deleted.Payload!.Source.Should().Be("DELETE_BY_TRANSFER", "客户因在职继承自动被转接成员删除");
        deleted.Payload!.WelcomeCode.Should().BeNull();

        var transferFail = reader.Read<ExternalContactChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalContact,
            ChangeType = "transfer_fail",
            DecryptedXml = "<xml><Event><![CDATA[change_external_contact]]></Event>" +
                           "<ChangeType><![CDATA[transfer_fail]]></ChangeType>" +
                           "<FailReason><![CDATA[customer_refused]]></FailReason>" +
                           "<UserID><![CDATA[zhangsan]]></UserID>" +
                           "<ExternalUserID><![CDATA[wo-x]]></ExternalUserID></xml>",
        });
        transferFail.Payload!.FailReason.Should().Be("customer_refused");
        transferFail.Payload!.State.Should().BeNull("权限/形态分层 ⇒ 缺失即 null，处理器不得假设必有值");
    }

    [Fact]
    public void Read_ShouldMapExternalChatMemberChange_WhenUpdate()
    {
        // 官方 92130「客户群变更事件」样报文（update 携带成员与版本字段）。
        var result = CreateReader().Read<ExternalChatChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalChat,
            ChangeType = "update",
            DecryptedXml = "<xml><Event><![CDATA[change_external_chat]]></Event>" +
                           "<ChatId><![CDATA[wrx7HUARsKwGRaQBVKPBTcEyzdHA4HrQ]]></ChatId>" +
                           "<ChangeType><![CDATA[update]]></ChangeType>" +
                           "<UpdateDetail><![CDATA[add_member]]></UpdateDetail>" +
                           "<JoinScene>1</JoinScene><QuitScene>0</QuitScene><MemChangeCnt>10</MemChangeCnt>" +
                           "<MemChangeList><Item>Jack</Item><Item>Rose</Item></MemChangeList>" +
                           "<LastMemVer>9c3f97c2ada667dfb5f6d03308d963e1</LastMemVer>" +
                           "<CurMemVer>71217227bbd112ecfe3a49c482195cb4</CurMemVer></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.ChatId.Should().Be("wrx7HUARsKwGRaQBVKPBTcEyzdHA4HrQ");
        payload.UpdateDetail.Should().Be("add_member");
        payload.JoinScene.Should().Be(1, "0 = 成员邀请入群，3 = 扫码入群");
        payload.QuitScene.Should().Be(0, "0 = 自己退群，1 = 群主/管理员移出");
        payload.MemberChangeCount.Should().Be(10);
        payload.MemberChangeList.Should().Equal("Jack", "Rose");
        payload.LastMemberVersion.Should().Be("9c3f97c2ada667dfb5f6d03308d963e1");
        payload.CurrentMemberVersion.Should().Be("71217227bbd112ecfe3a49c482195cb4");
    }

    [Fact]
    public void Read_ShouldKeepChatAndTagKeysIsolated_WhenSameBareChangeType()
    {
        // B4：客户群族与企业客户标签族的 ChangeType 同为裸 create —— 族事件值键 + 载荷类型隔离。
        var reader = CreateReader();

        var chatCreate = reader.Read<ExternalChatChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalChat,
            ChangeType = "create",
            DecryptedXml = "<xml><Event><![CDATA[change_external_chat]]></Event>" +
                           "<ChatId><![CDATA[CHAT_ID]]></ChatId><ChangeType><![CDATA[create]]></ChangeType></xml>",
        });
        chatCreate.Status.Should().Be(WechatPayloadReadStatus.Matched);
        chatCreate.Payload!.ChatId.Should().Be("CHAT_ID");

        // 同为裸 create 的标签报文（Event = change_external_tag）不得命中客户群载荷。
        var tagCreate = reader.Read<ExternalTagChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalTag,
            ChangeType = "create",
            DecryptedXml = "<xml><Event><![CDATA[change_external_tag]]></Event>" +
                           "<Id><![CDATA[TAG_ID]]></Id><TagType><![CDATA[tag]]></TagType>" +
                           "<ChangeType><![CDATA[create]]></ChangeType><StrategyId>1</StrategyId></xml>",
        });
        tagCreate.Status.Should().Be(WechatPayloadReadStatus.Matched);
        tagCreate.Payload!.TagId.Should().Be("TAG_ID");
        tagCreate.Payload!.TagType.Should().Be("tag");
        tagCreate.Payload!.StrategyId.Should().Be("1");

        // 标签报文请求客户群载荷类型 ⇒ 契约载荷不一致（宿主接线错误），不是降级。
        var crossType = reader.Read<ExternalChatChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalTag,
            ChangeType = "create",
            DecryptedXml = "<xml><Id>TAG_ID</Id></xml>",
        });
        crossType.Status.Should().Be(WechatPayloadReadStatus.ContractMismatch);
    }

    [Fact]
    public void Read_ShouldMapExternalTagShuffle_WhenStrategyIdIsTextual()
    {
        // 官方 92130「企业客户标签重排事件」：StrategyId 为字符串形态 ⇒ 按文本承载。
        var result = CreateReader().Read<ExternalTagChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalTag,
            ChangeType = "shuffle",
            DecryptedXml = "<xml><Event><![CDATA[change_external_tag]]></Event>" +
                           "<Id><![CDATA[TAG_ID]]></Id><StrategyId><![CDATA[STRATEGY_ID]]></StrategyId>" +
                           "<ChangeType><![CDATA[shuffle]]></ChangeType></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.TagId.Should().Be("TAG_ID");
        result.Payload!.StrategyId.Should().Be("STRATEGY_ID");
        result.Payload!.TagType.Should().BeNull("重排事件无 TagType 节点 ⇒ null");
    }

    [Fact]
    public void Read_ShouldMapAcquisitionFields_WhenMessageFromCustomer()
    {
        // 官方 97299「成员多次收消息事件」样报文（企业自建 Event 信封）。
        var result = CreateReader().Read<CustomerAcquisitionPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.CustomerAcquisition,
            ChangeType = "message_from_customer",
            DecryptedXml = "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<CreateTime>1403610513</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[customer_acquisition]]></Event>" +
                           "<ChangeType><![CDATA[message_from_customer]]></ChangeType>" +
                           "<UserID><![CDATA[zhangsan]]></UserID>" +
                           "<ExternalUserID><![CDATA[woAJ2GCAAAXtWyujaWJHDDGi0mAAAA]]></ExternalUserID>" +
                           "<LinkId><![CDATA[cawcdea7783d7330c6]]></LinkId><ChatSeq>3</ChatSeq>" +
                           "<ChatKey><![CDATA[CHATKEY]]></ChatKey></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.EventTypeKey.Should().Be("customer_acquisition", "获客助手族以族事件值为键");
        var payload = result.Payload!;
        payload.UserId.Should().Be("zhangsan");
        payload.ExternalUserId.Should().Be("woAJ2GCAAAXtWyujaWJHDDGi0mAAAA");
        payload.LinkId.Should().Be("cawcdea7783d7330c6");
        payload.ChatSeq.Should().Be(3);
        payload.ChatKey.Should().Be("CHATKEY");
        payload.Price.Should().BeNull("仅价格调整事件携带 Price");
    }

    [Fact]
    public void Read_ShouldMapAcquisitionFields_WhenBalanceOnly()
    {
        // 官方 97299「企业使用量即将耗尽事件」：无业务字段节点 ⇒ 全字段 null 不抛。
        var result = CreateReader().Read<CustomerAcquisitionPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.CustomerAcquisition,
            ChangeType = "balance_low",
            DecryptedXml = "<xml><Event><![CDATA[customer_acquisition]]></Event>" +
                           "<ChangeType><![CDATA[balance_low]]></ChangeType></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.LinkId.Should().BeNull();
        result.Payload!.ExpireTime.Should().BeNull();
        result.Payload!.ChatKey.Should().BeNull();
    }

    [Fact]
    public void Read_ShouldMapAcquisitionComponentFields_WhenSuiteEnvelope()
    {
        // 官方 99485 组件形态（第三方套件信封）：service_balance_consumed / change_price。
        var reader = CreateReader();

        var consumed = reader.Read<CustomerAcquisitionPayload>(SuiteEvent(
            WechatCallbackEventTypes.CustomerAcquisition, "service_balance_consumed",
            "<xml><SuiteId><![CDATA[ww4asffe99e54c0f4c]]></SuiteId>" +
            "<AuthCorpId><![CDATA[wxf8b4f85f3a794e77]]></AuthCorpId>" +
            "<InfoType><![CDATA[customer_acquisition]]></InfoType><TimeStamp>1403610513</TimeStamp>" +
            "<ChangeType><![CDATA[service_balance_consumed]]></ChangeType>" +
            "<LinkId><![CDATA[cawcdea7783d7330c6]]></LinkId><State><![CDATA[STATE]]></State>" +
            "<OnceKey><![CDATA[ONCEKEY]]></OnceKey></xml>"));
        consumed.Status.Should().Be(WechatPayloadReadStatus.Matched);
        consumed.Payload!.LinkId.Should().Be("cawcdea7783d7330c6");
        consumed.Payload!.State.Should().Be("STATE");
        consumed.Payload!.OnceKey.Should().Be("ONCEKEY");

        var priceChanged = reader.Read<CustomerAcquisitionPayload>(SuiteEvent(
            WechatCallbackEventTypes.CustomerAcquisition, "change_price",
            "<xml><InfoType><![CDATA[customer_acquisition]]></InfoType><TimeStamp>1709222400</TimeStamp>" +
            "<ChangeType><![CDATA[change_price]]></ChangeType>" +
            "<Price>30000</Price><EffectiveTime>1709222400</EffectiveTime></xml>"));
        priceChanged.Status.Should().Be(WechatPayloadReadStatus.Matched);
        priceChanged.Payload!.Price.Should().Be(30000, "官方价格单位为分");
        priceChanged.Payload!.EffectiveTime.Should().Be(1709222400L);
    }

    [Fact]
    public void Read_ShouldMapPermitChange_WhenSuiteEnvelope()
    {
        // 官方 92277「客户可建联成员范围变动事件」：无 ChangeType 分组段、无业务字段节点 ⇒ 键为 InfoType 本身。
        var evt = SuiteEvent(WechatCallbackEventTypes.CustomerAcquisitionPermitChange, changeType: null,
            "<xml><SuiteId><![CDATA[ww4asffe99e54c0f4c]]></SuiteId>" +
            "<AuthCorpId><![CDATA[wxf8b4f85f3a794e77]]></AuthCorpId>" +
            "<InfoType><![CDATA[customer_acquisition_permit_change]]></InfoType>" +
            "<TimeStamp>1403610513</TimeStamp></xml>");

        evt.EventTypeKey.Should().Be("customer_acquisition_permit_change");
        evt.EventFamily.Should().Be(WechatCallbackEventFamily.CustomerAcquisition);

        var result = CreateReader().Read<CustomerAcquisitionPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.LinkId.Should().BeNull("该事件无业务字段节点 ⇒ 复用载荷全字段为 null");
    }

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.Suite, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, false)]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.Suite, false)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.Suite, false)]
    public void ExternalContactContract_ShouldOpenForOfficialMatrix(
        WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.ChangeExternalContact, out var contract)
            .Should().BeTrue();
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeExternalContact,
            ChangeType = "add_external_contact",
        };

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "官方接入矩阵：自建·代开发×应用数据通道 + 第三方×套件指令通道（92130/92277/96361）");
    }

    [Fact]
    public void AcquisitionContract_ShouldDeclareBothChannels_WhenSameKeyDeclaredTwice()
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.CustomerAcquisition, out var contract)
            .Should().BeTrue();

        contract!.OpenSurfaces.Should().HaveCount(2,
            "同键双特性声明（自建·代开发×App + 第三方×Suite）合并为一条多组开放面契约");
        contract.OpenSurfaces.Should().Contain(s =>
            s.SupportedAppTypes == (WechatAppTypeSet.Internal | WechatAppTypeSet.Provider) &&
            s.RequiredChannel == WechatCallbackChannel.App);
        contract.OpenSurfaces.Should().Contain(s =>
            s.SupportedAppTypes == WechatAppTypeSet.ThirdParty &&
            s.RequiredChannel == WechatCallbackChannel.Suite);
        contract.RequiredEvent.Should().Be("customer_acquisition", "RequiredEvent 缺省 = 逐键自指");
    }

    // ---------------------------------------- 收银台·应用版本付费订单回调族（91929~91933 / 99353）

    [Fact]
    public void Read_ShouldMapPayToolVersionOrderFields_WhenOpenOrder()
    {
        // 官方 91929 样报文（第三方指令回调，套件信封）：OrderId + OperatorId。
        var evt = SuiteEvent(WechatCallbackEventTypes.OpenOrder, changeType: null,
            "<xml><SuiteId><![CDATA[ww4asffe99e54c0aaa]]></SuiteId>" +
            "<PaidCorpId><![CDATA[wxf8b4f85f3a794aaa]]></PaidCorpId>" +
            "<InfoType><![CDATA[open_order]]></InfoType><TimeStamp>1403610513</TimeStamp>" +
            "<OrderId><![CDATA[ORDERID]]></OrderId><OperatorId><![CDATA[OPERATORID]]></OperatorId></xml>");

        evt.EventTypeKey.Should().Be("open_order");
        evt.EventFamily.Should().Be(WechatCallbackEventFamily.Authorization, "套件信封（InfoType 非空）⇒ 授权族");

        var result = CreateReader().Read<PayToolVersionOrderPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var payload = result.Payload!;
        payload.PaidCorpId.Should().Be("wxf8b4f85f3a794aaa",
            "官方报文无 AuthCorpId / FromUserName 节点 ⇒ 购买方 corpid 只能取自 PaidCorpId");
        payload.OrderId.Should().Be("ORDERID");
        payload.OperatorId.Should().Be("OPERATORID");
        payload.OldOrderId.Should().BeNull("下单成功通知不含改单字段");
        payload.NewOrderId.Should().BeNull("下单成功通知不含改单字段");
        evt.AuthCorpId.Should().BeNull("本族报文无 AuthCorpId/FromUserName ⇒ 信封不得伪造授权企业");
    }

    [Fact]
    public void Read_ShouldMapOldAndNewOrderId_WhenChangeOrder()
    {
        // 官方 91930 样报文：改单通知携带 OldOrderId / NewOrderId，无 OrderId。
        var result = CreateReader().Read<PayToolVersionOrderPayload>(
            SuiteEvent(WechatCallbackEventTypes.ChangeOrder, changeType: null,
                "<xml><SuiteId><![CDATA[ww4asffe99e54c0aaa]]></SuiteId>" +
                "<PaidCorpId><![CDATA[wxf8b4f85f3a794aaa]]></PaidCorpId>" +
                "<InfoType><![CDATA[change_order]]></InfoType><TimeStamp>1403610513</TimeStamp>" +
                "<OldOrderId><![CDATA[OLD_ORDER_ID]]></OldOrderId>" +
                "<NewOrderId><![CDATA[NEW_ORDER_ID]]></NewOrderId></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.OldOrderId.Should().Be("OLD_ORDER_ID");
        result.Payload!.NewOrderId.Should().Be("NEW_ORDER_ID");
        result.Payload!.OrderId.Should().BeNull("改单通知不含 OrderId 节点，须用 NewOrderId");
        result.Payload!.OperatorId.Should().BeNull();
    }

    [Theory]
    [InlineData(WechatCallbackEventTypes.PayForAppSuccess)]
    [InlineData(WechatCallbackEventTypes.Refund)]
    [InlineData(WechatCallbackEventTypes.CancelOrder)]
    public void Read_ShouldMapOrderIdOnly_WhenSingleOrderFieldEvent(string eventKey)
    {
        var result = CreateReader().Read<PayToolVersionOrderPayload>(
            SuiteEvent(eventKey, changeType: null,
                "<xml><SuiteId><![CDATA[ww4asffe99e54c0aaa]]></SuiteId>" +
                "<PaidCorpId><![CDATA[wxf8b4f85f3a794aaa]]></PaidCorpId>" +
                $"<InfoType><![CDATA[{eventKey}]]></InfoType><TimeStamp>1403610513</TimeStamp>" +
                "<OrderId><![CDATA[ORDERID]]></OrderId></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.OrderId.Should().Be("ORDERID");
        result.Payload!.OperatorId.Should().BeNull();
        result.Payload!.OldOrderId.Should().BeNull();
        result.Payload!.NewOrderId.Should().BeNull();
    }

    [Fact]
    public void Read_ShouldMapNoOrderField_WhenEditionChanged()
    {
        // 官方 91933 样报文：应用版本变更通知仅四个信封字段（官方 InfoType 拼写为 change_editon）。
        var evt = SuiteEvent(WechatCallbackEventTypes.ChangeEditon, changeType: null,
            "<xml><SuiteId><![CDATA[ww4asffe99e54c0aaa]]></SuiteId>" +
            "<PaidCorpId><![CDATA[wxf8b4f85f3a794aaa]]></PaidCorpId>" +
            "<InfoType><![CDATA[change_editon]]></InfoType><TimeStamp>1403610513</TimeStamp></xml>");

        evt.EventTypeKey.Should().Be("change_editon", "官方键值拼写为 change_editon（少一个字母 i）");

        var result = CreateReader().Read<PayToolVersionOrderPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.PaidCorpId.Should().Be("wxf8b4f85f3a794aaa");
        result.Payload!.OrderId.Should().BeNull();
        result.Payload!.OperatorId.Should().BeNull();
        result.Payload!.OldOrderId.Should().BeNull();
        result.Payload!.NewOrderId.Should().BeNull();
    }

    [Theory]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.Suite, true)]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, false)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.Suite, false)]
    public void PayToolVersionOrderContract_ShouldOpenForOfficialMatrix(
        WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.PayForAppSuccess, out var contract)
            .Should().BeTrue();
        var evt = new WechatCallbackEvent { InfoType = WechatCallbackEventTypes.PayForAppSuccess };

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "收银台应用版本付费回调族官方仅在第三方应用开发文档树提供 ⇒ 第三方 × 套件指令通道");
        contract.RequiredFamily.Should().Be(WechatCallbackEventFamily.Authorization);
        contract.RequiredEvent.Should().Be(WechatCallbackEventTypes.PayForAppSuccess,
            "RequiredEvent 缺省 = 逐键自指（InfoType 即事件键）");
    }

    // ---------------------------------------- 邮箱族（97495/97517/97506 + 100180；族事件值为键）

    /// <summary>邮箱族事件：<c>Event</c> 节点即事件键（<c>receive_email</c> 跨族同名，族事件值消歧）。</summary>
    private static WechatCallbackEvent EmailEvent(string eventKey, string plainXml) => new()
    {
        Event = eventKey,
        ChangeType = "receive_email",
        DecryptedXml = plainXml,
    };

    [Fact]
    public void Read_ShouldMapAppEmailAmount_WhenReceiveEmail()
    {
        // 官方 97495 样报文：Amount 以 CDATA 承载数值。
        var result = CreateReader().Read<AppEmailChangedPayload>(
            EmailEvent(WechatCallbackEventTypes.AppEmailChange,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                "<CreateTime>1668831860</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[app_email_change]]></Event><ChangeType><![CDATA[receive_email]]></ChangeType>" +
                "<Amount><![CDATA[2]]></Amount></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.Amount.Should().Be(2, "官方样报文 Amount=2（CDATA 数值文本）");
    }

    [Fact]
    public void Read_ShouldMapPublicEmailFields_WhenReceiveEmail()
    {
        // 官方 100180 样报文：Id/Amount 为裸数字文本节点（无 CDATA），较应用邮箱多 Id 节点。
        var result = CreateReader().Read<PublicEmailChangedPayload>(
            EmailEvent(WechatCallbackEventTypes.PublicEmailChange,
                "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                "<CreateTime>1668831860</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                "<Event><![CDATA[public_email_change]]></Event><ChangeType><![CDATA[receive_email]]></ChangeType>" +
                "<Id>1</Id><Amount>2</Amount></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.Id.Should().Be("1");
        result.Payload!.Amount.Should().Be(2L);
    }

    [Fact]
    public void Read_ShouldKeepEmailKeysIsolated_WhenSameReceiveEmailChangeType()
    {
        // receive_email 跨族同名 ⇒ 键为族事件值：应用邮箱报文不得命中公共邮箱载荷（RequiredEvent 拦截）。
        var reader = CreateReader();

        reader.Read<PublicEmailChangedPayload>(EmailEvent(WechatCallbackEventTypes.AppEmailChange,
                "<xml><Event><![CDATA[app_email_change]]></Event>" +
                "<ChangeType><![CDATA[receive_email]]></ChangeType><Amount>1</Amount></xml>"))
            .Status.Should().Be(WechatPayloadReadStatus.ContractMismatch,
                "应用邮箱报文的 Event 与公共邮箱契约的族前置条件不一致");
        reader.Read<AppEmailChangedPayload>(EmailEvent(WechatCallbackEventTypes.PublicEmailChange,
                "<xml><Event><![CDATA[public_email_change]]></Event>" +
                "<ChangeType><![CDATA[receive_email]]></ChangeType><Id>1</Id><Amount>1</Amount></xml>"))
            .Status.Should().Be(WechatPayloadReadStatus.ContractMismatch);
    }

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, true)]
    public void AppEmailContract_ShouldOpenForAllThreeModes(WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.AppEmailChange, out var contract).Should().BeTrue();
        var evt = EmailEvent(WechatCallbackEventTypes.AppEmailChange, "<xml><Amount>1</Amount></xml>");

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "邮件回调通知官方 97495/97517/97506 三份文档逐字一致 ⇒ 三类应用均开放");
    }

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, false)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, false)]
    public void PublicEmailContract_ShouldOpenOnlyForInternal(
        WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.PublicEmailChange, out var contract).Should().BeTrue();
        var evt = EmailEvent(WechatCallbackEventTypes.PublicEmailChange, "<xml><Id>1</Id><Amount>1</Amount></xml>");

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "官方第三方/代开发无「管理公共邮箱」回调事件 ⇒ 仅企业自建开放");
    }

    // ---------------------------------------- 文档族（doc_change 5 键；97833~97835/98095/98096 等）

    [Theory]
    [InlineData(WechatCallbackEventTypes.DocMemberChange)]
    [InlineData(WechatCallbackEventTypes.DeleteDoc)]
    public void Read_ShouldMapDocIdSiblings_WhenDocEvent(string changeType)
    {
        // 官方样报文：DocId 为根下重复同名兄弟元素（无包装容器）。
        var result = CreateReader().Read<DocChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.DocChange,
            ChangeType = changeType,
            DecryptedXml = "<xml><ToUserName><![CDATA[toUser]]></ToUserName>" +
                           "<FromUserName><![CDATA[fromUser]]></FromUserName>" +
                           "<CreateTime>1348831860</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[doc_change]]></Event>" +
                           $"<ChangeType><![CDATA[{changeType}]]></ChangeType>" +
                           "<DocId><![CDATA[wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA]]></DocId>" +
                           "<DocId><![CDATA[wcjgewCwAAqeJcPI1d8Pwbjt7nttzBBB]]></DocId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.DocIds.Should().Equal(new[]
            {
                "wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA", "wcjgewCwAAqeJcPI1d8Pwbjt7nttzBBB",
            }, "重复同名兄弟元素经合并投影全量读取");
        result.Payload!.FormIds.Should().BeEmpty("文档类事件不携带 FormId");
    }

    [Theory]
    [InlineData(WechatCallbackEventTypes.FormComplete)]
    [InlineData(WechatCallbackEventTypes.DeleteForm)]
    [InlineData(WechatCallbackEventTypes.FormSettingsChange)]
    public void Read_ShouldMapFormIdSiblings_WhenFormEvent(string changeType)
    {
        var result = CreateReader().Read<DocChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.DocChange,
            ChangeType = changeType,
            DecryptedXml = "<xml><Event><![CDATA[doc_change]]></Event>" +
                           $"<ChangeType><![CDATA[{changeType}]]></ChangeType>" +
                           "<FormId><![CDATA[form-1]]></FormId><FormId><![CDATA[form-2]]></FormId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.FormIds.Should().Equal("form-1", "form-2");
        result.Payload!.DocIds.Should().BeEmpty("收集表类事件不携带 DocId");
    }

    [Fact]
    public void Read_ShouldMapSingleDocId_WhenOnlyOneSiblingPresent()
    {
        // 恰 1 个元素时未触发合并投影，RepeatSiblings 须从叶节点自有文本取值。
        var result = CreateReader().Read<DocChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.DocChange,
            ChangeType = WechatCallbackEventTypes.DeleteDoc,
            DecryptedXml = "<xml><Event><![CDATA[doc_change]]></Event>" +
                           "<ChangeType><![CDATA[delete_doc]]></ChangeType>" +
                           "<DocId><![CDATA[doc-solo]]></DocId></xml>",
        });

        result.Payload!.DocIds.Should().Equal("doc-solo");
    }

    [Fact]
    public void Read_ShouldRejectDocMessage_WhenContactPayloadRequested()
    {
        // B4：doc_change 报文即便携带同名形态字段也不得进入通讯录载荷（RequiredEvent 隔离）。
        var reader = CreateReader();
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.DocChange,
            ChangeType = WechatCallbackEventTypes.DeleteUser,
            DecryptedXml = "<xml><Event><![CDATA[doc_change]]></Event>" +
                           "<ChangeType><![CDATA[delete_user]]></ChangeType></xml>",
        };

        reader.Read<ContactUserChangedPayload>(evt).Status
            .Should().Be(WechatPayloadReadStatus.ContractMismatch, "契约登记 RequiredEvent=change_contact");
    }

    // ------------------------ 智能表格族（smart_sheet_change 6 键；100986/100987 等）

    [Fact]
    public void Read_ShouldMapFieldIds_WhenFieldChanged()
    {
        // 官方 100987 样报文：DocId/SheetId 单节点，FieldId 重复兄弟元素；官方限制一次最多 1000 个。
        var result = CreateReader().Read<SmartSheetFieldChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.SmartSheetChange,
            ChangeType = WechatCallbackEventTypes.AddFiled,
            DecryptedXml = "<xml><ToUserName><![CDATA[toUser]]></ToUserName>" +
                           "<FromUserName><![CDATA[fromUser]]></FromUserName>" +
                           "<CreateTime>1348831860</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[smart_sheet_change]]></Event>" +
                           "<ChangeType><![CDATA[add_filed]]></ChangeType>" +
                           "<DocId><![CDATA[dcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA]]></DocId>" +
                           "<SheetId><![CDATA[SheetId]]></SheetId>" +
                           "<FieldId><![CDATA[FieldId1]]></FieldId><FieldId><![CDATA[FieldId2]]></FieldId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.DocId.Should().Be("dcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA");
        result.Payload!.SheetId.Should().Be("SheetId");
        result.Payload!.FieldIds.Should().Equal("FieldId1", "FieldId2");
    }

    [Fact]
    public void Read_ShouldMapRecordIds_WhenRecordChanged()
    {
        var result = CreateReader().Read<SmartSheetRecordChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.SmartSheetChange,
            ChangeType = WechatCallbackEventTypes.UpdateRecord,
            DecryptedXml = "<xml><Event><![CDATA[smart_sheet_change]]></Event>" +
                           "<ChangeType><![CDATA[update_record]]></ChangeType>" +
                           "<DocId><![CDATA[doc-1]]></DocId><SheetId><![CDATA[sheet-1]]></SheetId>" +
                           "<RecordId><![CDATA[RecordId1]]></RecordId><RecordId><![CDATA[RecordId2]]></RecordId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.DocId.Should().Be("doc-1");
        result.Payload!.SheetId.Should().Be("sheet-1");
        result.Payload!.RecordIds.Should().Equal("RecordId1", "RecordId2");
    }

    [Fact]
    public void Read_ShouldReturnNullSheetId_WhenSmartSheetMessageOmitsNode()
    {
        // 缺失字段 ⇒ null 不抛（处理器不得假设必有值）。
        var result = CreateReader().Read<SmartSheetFieldChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.SmartSheetChange,
            ChangeType = WechatCallbackEventTypes.DeleteFiled,
            DecryptedXml = "<xml><Event><![CDATA[smart_sheet_change]]></Event>" +
                           "<ChangeType><![CDATA[delete_filed]]></ChangeType>" +
                           "<DocId><![CDATA[doc-1]]></DocId><FieldId><![CDATA[f-1]]></FieldId></xml>",
        });

        result.Payload!.SheetId.Should().BeNull("官方未携带 SheetId ⇒ null");
        result.Payload!.FieldIds.Should().Equal("f-1");
    }

    [Fact]
    public void Read_ShouldRejectSmartSheetMessage_WhenDocPayloadRequested()
    {
        // 智能表格与文档族共享「根下 id 列表」形态，但 Event 值不同 ⇒ RequiredEvent 隔离。
        var reader = CreateReader();
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.SmartSheetChange,
            ChangeType = WechatCallbackEventTypes.DeleteDoc,
            DecryptedXml = "<xml><Event><![CDATA[smart_sheet_change]]></Event>" +
                           "<ChangeType><![CDATA[delete_doc]]></ChangeType></xml>",
        };

        reader.Read<DocChangedPayload>(evt).Status
            .Should().Be(WechatPayloadReadStatus.ContractMismatch, "契约登记 RequiredEvent=doc_change");
    }

    // ---------------------------------------- 同名叶兄弟合并投影（Values 全量袋语义回归）

    [Fact]
    public void Values_ShouldKeepLastWins_WhenRepeatedLeafSiblingsCoalesced()
    {
        // ADR-5 既有语义：Values 全量袋同名重复子节点取最后一个 —— 合并投影不得改变该行为。
        var result = CreateReader().Read<GenericCallbackPayload>(new WechatCallbackEvent
        {
            Event = "host_private_event",
            DecryptedXml = "<xml><Event><![CDATA[host_private_event]]></Event>" +
                           "<DocId><![CDATA[first]]></DocId><DocId><![CDATA[second]]></DocId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.GenericFallback);
        result.Payload!.Values["DocId"].Should().Be("second",
            "合并容器的 Value 取末位成员文本 ⇒ 全量袋「同名取最后」不变");
    }

    [Fact]
    public void Project_ShouldNotCoalesce_WhenRepeatedSiblingsAreComplex()
    {
        // 复杂兄弟重复（包装容器 + 项序列）不合并：ItemsObject 通道仍逐项绑定既有报文形态。
        var result = CreateReader().Read<ContactUserChangedPayload>(
            Event(WechatCallbackEventTypes.CreateUser,
                "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[create_user]]></ChangeType>" +
                "<UserID><![CDATA[zhangsan]]></UserID>" +
                "<ExtAttr><Item Name=\"工号\" Type=\"0\"><Text>A1001</Text></Item>" +
                "<Item Name=\"职位\" Type=\"0\"><Text>工程师</Text></Item></ExtAttr></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.ExtAttr.Should().HaveCount(2, "ExtAttr/Item 复杂兄弟重复不受合并投影影响");
        result.Payload!.ExtAttr[0].Name.Should().Be("工号");
        result.Payload!.ExtAttr[1].Name.Should().Be("职位");
    }

    // ---------------------------------------- 日程族（97728/97730/97731/97732/98111 等；Event 节点即事件键）

    [Theory]
    [InlineData(WechatCallbackEventTypes.DeleteCalendar)]
    [InlineData(WechatCallbackEventTypes.ModifyCalendar)]
    public void Read_ShouldMapCalendarFields_WhenCalendarEvent(string eventKey)
    {
        var result = CreateReader().Read<CalendarChangedPayload>(new WechatCallbackEvent
        {
            Event = eventKey,
            DecryptedXml = "<xml><ToUserName><![CDATA[toUser]]></ToUserName>" +
                           "<FromUserName><![CDATA[fromUser]]></FromUserName>" +
                           "<CreateTime>1348831860</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           $"<Event><![CDATA[{eventKey}]]></Event>" +
                           "<CalId><![CDATA[wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA]]></CalId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched, "日程族无 ChangeType 分组段 ⇒ Event 节点即事件键");
        result.Payload!.CalId.Should().Be("wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA");
    }

    [Fact]
    public void Read_ShouldMapScheduleFields_WhenRespondSchedule()
    {
        var result = CreateReader().Read<ScheduleChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.RespondSchedule,
            DecryptedXml = "<xml><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[respond_schedule]]></Event>" +
                           "<CalId><![CDATA[wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA]]></CalId>" +
                           "<ScheduleId><![CDATA[17c7d2bd9f20d652840f72f59e796AAA]]></ScheduleId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.CalId.Should().Be("wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA");
        result.Payload!.ScheduleId.Should().Be("17c7d2bd9f20d652840f72f59e796AAA");
    }

    [Fact]
    public void Read_ShouldRejectCalendarMessage_WhenSchedulePayloadRequested()
    {
        // 日历与日程是不同的 Event 值（键不同）⇒ 逐键自指的 RequiredEvent 隔离。
        var result = CreateReader().Read<ScheduleChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.DeleteCalendar,
            DecryptedXml = "<xml><Event><![CDATA[delete_calendar]]></Event>" +
                           "<CalId><![CDATA[c]]></CalId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.ContractMismatch);
    }

    // ---------------------------------------- 会议族（meeting_change / meeting_statistics；ChangeType 为键）

    [Fact]
    public void Read_ShouldMapMeetingFlatFields_WhenModifyMeeting()
    {
        // 官方 99081 样报文：旧式信封，无 FromUserTmpOpenId 节点 ⇒ null。
        var result = CreateReader().Read<MeetingChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.ModifyMeeting,
            DecryptedXml = "<xml><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[modify_meeting]]></ChangeType>" +
                           "<MeetingId><![CDATA[wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA]]></MeetingId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.MeetingId.Should().Be("wcjgewCwAAqeJcPI1d8Pwbjt7nttzAAA");
        result.Payload!.FromUserTmpOpenId.Should().BeNull("修改/取消会议报文无 FromUserTmpOpenId 节点");
    }

    [Fact]
    public void Read_ShouldMapMeetingFlatFields_WhenMemberJoinsMeeting()
    {
        var result = CreateReader().Read<MeetingChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.JoinMeeting,
            DecryptedXml = "<xml><FromUserName><![CDATA[userId]]></FromUserName>" +
                           "<FromUserTmpOpenId><![CDATA[tmpOpenId]]></FromUserTmpOpenId>" +
                           "<MsgType><![CDATA[event]]></MsgType><Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[join_meeting]]></ChangeType>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.FromUserTmpOpenId.Should().Be("tmpOpenId");
        result.Payload!.MeetingId.Should().Be("m-1");
    }

    [Fact]
    public void Read_ShouldMapMeetingFlatFields_WhenRecordingStarted()
    {
        // 云录制族与会议平铺事件同构（信封 + TmpOpenId + MeetingId），共用 MeetingChangedPayload。
        var result = CreateReader().Read<MeetingChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.StartRecording,
            DecryptedXml = "<xml><Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[start_recording]]></ChangeType>" +
                           "<FromUserTmpOpenId><![CDATA[tmpOpenId]]></FromUserTmpOpenId>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.MeetingId.Should().Be("m-1");
    }

    [Fact]
    public void Read_ShouldMapEnrollFields_WhenUserEnrolls()
    {
        var result = CreateReader().Read<MeetingEnrollPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.Enroll,
            DecryptedXml = "<xml><Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[enroll]]></ChangeType>" +
                           "<FromUserTmpOpenId><![CDATA[tmpOpenId]]></FromUserTmpOpenId>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId>" +
                           "<EnrollId><![CDATA[EnrollId01]]></EnrollId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.MeetingId.Should().Be("m-1");
        result.Payload!.EnrollId.Should().Be("EnrollId01");
    }

    [Fact]
    public void Read_ShouldMapOperatedUser_WhenRoleChanged()
    {
        // 官方 98397 样报文：OperatedUser 单节点对象（UserId + TmpOpenId + UserRole）。
        var result = CreateReader().Read<MeetingMemberChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.RoleChange,
            DecryptedXml = "<xml><FromUserName><![CDATA[userId]]></FromUserName>" +
                           "<FromUserTmpOpenId><![CDATA[tmpOpenId]]></FromUserTmpOpenId>" +
                           "<OperatedUser><UserId><![CDATA[userId]]></UserId>" +
                           "<TmpOpenId><![CDATA[tmpOpenId]]></TmpOpenId><UserRole>2</UserRole></OperatedUser>" +
                           "<MsgType><![CDATA[event]]></MsgType><Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[role_change]]></ChangeType>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var operated = result.Payload!.OperatedUser;
        operated.Should().NotBeNull();
        operated!.UserId.Should().Be("userId");
        operated.TmpOpenId.Should().Be("tmpOpenId");
        operated.UserRole.Should().Be(2, "官方 UserRole=2 ⇒ 主持人权限");
    }

    [Fact]
    public void Read_ShouldReturnNullOperatedUserRole_WhenWaitingRoomEvent()
    {
        // 可空超集：等候室事件不带 UserRole ⇒ null，处理器不得假设必有值。
        var result = CreateReader().Read<MeetingMemberChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.QuitWaitingRoom,
            DecryptedXml = "<xml><Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[quit_waiting_room]]></ChangeType>" +
                           "<OperatedUser><UserId><![CDATA[userId]]></UserId>" +
                           "<TmpOpenId><![CDATA[tmpOpenId]]></TmpOpenId></OperatedUser>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId></xml>",
        });

        result.Payload!.OperatedUser!.UserRole.Should().BeNull();
    }

    [Fact]
    public void Read_ShouldMapWarmUpInfo_WhenWebinarWarmUpUploaded()
    {
        var result = CreateReader().Read<MeetingWarmUpUploadPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.WebinarWarmUpUpload,
            DecryptedXml = "<xml><FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[webinar_warm_up_upload]]></ChangeType>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId>" +
                           "<WarmUpInfo><WarmUpPicture><![CDATA[https://image.qq.com/12519.png]]></WarmUpPicture>" +
                           "<WarmUpVideo><![CDATA[https://image.qq.com/a135.mp4]]></WarmUpVideo>" +
                           "<UploadStatus>1</UploadStatus></WarmUpInfo></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.MeetingId.Should().Be("m-1");
        var warmUp = result.Payload!.WarmUpInfo;
        warmUp.Should().NotBeNull();
        warmUp!.WarmUpPicture.Should().Be("https://image.qq.com/12519.png");
        warmUp.WarmUpVideo.Should().Be("https://image.qq.com/a135.mp4");
        warmUp.UploadStatus.Should().Be(1);
        warmUp.ErrorMsg.Should().BeNull("上传成功时官方不返回 ErrorMsg");
    }

    [Fact]
    public void Read_ShouldMapMediumUploadInfos_WhenRepeatedSiblingsPresent()
    {
        // 官方 98775 样报文：UploadInfo 为根下重复同名复杂兄弟元素（对象列表、无包装容器）。
        var result = CreateReader().Read<MeetingMediumUploadPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.MediumUpload,
            DecryptedXml = "<xml><FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[medium_upload]]></ChangeType>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId>" +
                           "<AllUploadStatus>false</AllUploadStatus>" +
                           "<UploadInfo><MediumUrl><![CDATA[https://image.qq.com/12519.png]]></MediumUrl>" +
                           "<MediumType>2</MediumType><UploadStatus>0</UploadStatus>" +
                           "<ErrorMsg><![CDATA[low-resolution]]></ErrorMsg></UploadInfo>" +
                           "<UploadInfo><MediumUrl><![CDATA[https://image.qq.com/12520.png]]></MediumUrl>" +
                           "<MediumType>2</MediumType><UploadStatus>1</UploadStatus></UploadInfo></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.AllUploadStatus.Should().BeFalse("官方以 true/false 文本承载布尔");
        var infos = result.Payload!.UploadInfos;
        infos.Should().HaveCount(2, "重复复杂兄弟元素经合并投影全量读取");
        infos[0].MediumUrl.Should().Be("https://image.qq.com/12519.png");
        infos[0].MediumType.Should().Be(2);
        infos[0].UploadStatus.Should().Be(0);
        infos[0].ErrorMsg.Should().Be("low-resolution");
        infos[1].MediumUrl.Should().Be("https://image.qq.com/12520.png");
        infos[1].UploadStatus.Should().Be(1);
        infos[1].ErrorMsg.Should().BeNull();
    }

    [Fact]
    public void Read_ShouldMapSingleMediumUploadInfo_WhenOnlyOneSiblingPresent()
    {
        // 恰 1 个元素时未触发合并投影，RepeatMediumUploadItems 须按单项绑定。
        var result = CreateReader().Read<MeetingMediumUploadPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.MediumUpload,
            DecryptedXml = "<xml><Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[medium_upload]]></ChangeType>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId>" +
                           "<AllUploadStatus>true</AllUploadStatus>" +
                           "<UploadInfo><MediumUrl><![CDATA[https://image.qq.com/1.png]]></MediumUrl>" +
                           "<MediumType>2</MediumType><UploadStatus>1</UploadStatus></UploadInfo></xml>",
        });

        result.Payload!.AllUploadStatus.Should().BeTrue();
        result.Payload!.UploadInfos.Should().ContainSingle().Which.MediumUrl.Should().Be("https://image.qq.com/1.png");
    }

    [Fact]
    public void Read_ShouldMapRoomResponse_WhenMeetingRoomAnswered()
    {
        // 官方 98783 样报文：MeetingRoomId 与 MraAddress 二选一；系统触发无 FromUserTmpOpenId。
        var result = CreateReader().Read<MeetingRoomResponsePayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.MeetingRoomResponse,
            DecryptedXml = "<xml><FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[meeting_room_response]]></ChangeType>" +
                           "<MeetingId><![CDATA[m-1]]></MeetingId>" +
                           "<MeetingRoomId><![CDATA[mRidadc]]></MeetingRoomId>" +
                           "<MraAddress><Protocol>1</Protocol>" +
                           "<DialString><![CDATA[DialString]]></DialString></MraAddress>" +
                           "<RoomResponseStatus>2</RoomResponseStatus></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.MeetingRoomId.Should().Be("mRidadc");
        var address = result.Payload!.MraAddress;
        address.Should().NotBeNull();
        address!.Protocol.Should().Be(1, "官方 Protocol=1 ⇒ SIP");
        address.DialString.Should().Be("DialString");
        result.Payload!.RoomResponseStatus.Should().Be(2, "官方 2 ⇒ 入会中");
        result.Payload!.FromUserTmpOpenId.Should().BeNull("会议室应答为系统触发，无操作者临时 ID");
    }

    [Fact]
    public void Read_ShouldMapMeetingStatus_WhenQuickMeetingStarted()
    {
        // 官方 99648：Event=meeting_statistics（独立 Event 值），键取 ChangeType=start_meeting。
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingStatistics,
            ChangeType = WechatCallbackEventTypes.StartMeeting,
            DecryptedXml = "<xml><FromUserName><![CDATA[fromUser]]></FromUserName>" +
                           "<Event><![CDATA[meeting_statistics]]></Event>" +
                           "<ChangeType><![CDATA[start_meeting]]></ChangeType>" +
                           "<Status>1</Status></xml>",
        };

        evt.EventTypeKey.Should().Be("start_meeting", "会议统计族键取 ChangeType（与 meeting_change 族键域互异）");

        var result = CreateReader().Read<MeetingStatisticsPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.Status.Should().Be(1, "官方 Status=1 ⇒ 会议发起成功");
    }

    [Fact]
    public void Read_ShouldRejectMeetingMessage_WhenRequiredEventDiffers()
    {
        // B4：meeting_change 报文不得进入 meeting_statistics 族载荷（RequiredEvent 隔离）。
        var result = CreateReader().Read<MeetingStatisticsPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.StartMeeting,
            DecryptedXml = "<xml><Event><![CDATA[meeting_change]]></Event>" +
                           "<ChangeType><![CDATA[start_meeting]]></ChangeType>" +
                           "<Status>1</Status></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.ContractMismatch,
            "start_meeting 键在 meeting_statistics 族下的契约要求 Event=meeting_statistics");
    }

    // ---------------------------------------- 日程/会议族开放面（ADR-15）

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, true)]
    public void ModifyMeetingContract_ShouldOpenForAllThreeModes(
        WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.ModifyMeeting, out var contract).Should().BeTrue();
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MeetingChange,
            ChangeType = WechatCallbackEventTypes.ModifyMeeting,
        };

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "官方第三方 97451 / 代开发 97459 均提供修改/取消会议回调 ⇒ 三类应用开放");
    }

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, false)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, false)]
    public void InternalOnlyMeetingContracts_ShouldOpenOnlyForInternal(
        WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        // join_meeting 与 start_meeting（meeting_statistics 族）官方仅自建文档树提供。
        foreach (var key in new[] { WechatCallbackEventTypes.JoinMeeting, WechatCallbackEventTypes.StartMeeting })
        {
            CreateRegistry().TryResolve(key, out var contract).Should().BeTrue();
            var evt = new WechatCallbackEvent
            {
                Event = key == WechatCallbackEventTypes.StartMeeting
                    ? WechatCallbackEventTypes.MeetingStatistics
                    : WechatCallbackEventTypes.MeetingChange,
                ChangeType = key,
            };

            contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
                $"事件键 {key} 官方仅在企业自建文档树提供");
        }
    }

    [Theory]
    [InlineData(WechatAppType.Internal)]
    [InlineData(WechatAppType.ThirdParty)]
    [InlineData(WechatAppType.Provider)]
    public void ScheduleContracts_ShouldOpenForAllThreeModes(WechatAppType appType)
    {
        foreach (var key in new[] { WechatCallbackEventTypes.DeleteCalendar, WechatCallbackEventTypes.RespondSchedule })
        {
            CreateRegistry().TryResolve(key, out var contract).Should().BeTrue();
            var evt = new WechatCallbackEvent { Event = key };

            contract!.IsOpenFor(evt, appType, WechatCallbackChannel.App).Should().BeTrue(
                "日程回调通知官方三份文档（自建/第三方/代开发）正文逐字一致 ⇒ 三类应用开放");
        }
    }

    // ------------------- 家校沟通族（92032/92052/92050/92051/96716/96717 + 97281；族事件值为键）

    [Fact]
    public void Read_ShouldMapSchoolContactMemberFields_WhenCreateStudent()
    {
        // 官方 92032「新增学生」样报文（企业自建 Event 信封）。
        var result = CreateReader().Read<SchoolContactChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeSchoolContact,
            ChangeType = "create_student",
            DecryptedXml = "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<CreateTime>1403610513</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[change_school_contact]]></Event>" +
                           "<ChangeType><![CDATA[create_student]]></ChangeType>" +
                           "<Id><![CDATA[xiaoming]]></Id></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.EventTypeKey.Should().Be("change_school_contact", "家校沟通族以族事件值为键");
        result.Payload!.Id.Should().Be("xiaoming", "官方 Id 为学生的家校通讯录 userid");
        result.Payload!.NewId.Should().BeNull("NewId 仅第三方 update_student/update_parent 且 userid 变更时携带");
    }

    [Fact]
    public void Read_ShouldMapSchoolContactSameKey_WhenThirdPartySuiteEnvelope()
    {
        // 官方 92051「编辑学生」样报文（第三方指令回调，套件信封）：外层事件值在 InfoType ⇒ 与 Event 信封同键。
        var evt = SuiteEvent(WechatCallbackEventTypes.ChangeSchoolContact, "update_student",
            "<xml><SuiteId><![CDATA[ww4asffe99e54c0f4c]]></SuiteId>" +
            "<AuthCorpId><![CDATA[wxf8b4f85f3a794e77]]></AuthCorpId>" +
            "<InfoType><![CDATA[change_school_contact]]></InfoType><TimeStamp>1403610513</TimeStamp>" +
            "<ChangeType><![CDATA[update_student]]></ChangeType>" +
            "<Id><![CDATA[zhangsan]]></Id>" +
            "<NewId><![CDATA[zhangsan2]]></NewId></xml>");

        evt.EventTypeKey.Should().Be("change_school_contact", "套件信封与 Event 信封产出同一事件键（三模式键统一，ADR-14）");
        evt.EventFamily.Should().Be(WechatCallbackEventFamily.SchoolContactChange,
            "套件信封的 InfoType 承载家校沟通族事件值，不得误判为授权族");

        var result = CreateReader().Read<SchoolContactChangedPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.Id.Should().Be("zhangsan");
        result.Payload!.NewId.Should().Be("zhangsan2", "官方 92051：NewId 只在 userid 被修改时回调");
    }

    [Fact]
    public void Read_ShouldMapSchoolContactDepartmentFields_WhenCreateDepartment()
    {
        // 官方 92052「创建部门」样报文（企业自建 Event 信封；ChangeType 取参数表拼写 create_department）。
        var result = CreateReader().Read<SchoolContactChangedPayload>(new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeSchoolContact,
            ChangeType = "create_department",
            DecryptedXml = "<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<CreateTime>1403610513</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           "<Event><![CDATA[change_school_contact]]></Event>" +
                           "<ChangeType><![CDATA[create_department]]></ChangeType>" +
                           "<Id><![CDATA[1]]></Id></xml>",
        });

        result.Status.Should().Be(WechatPayloadReadStatus.Matched, "部门事件与成员事件同键（同结构族 ⇒ 共用一份载荷）");
        result.Payload!.Id.Should().Be("1", "部门事件的 Id 为家校通讯录部门 id");
        result.Payload!.NewId.Should().BeNull("部门事件无 NewId 节点");
    }

    [Fact]
    public void Read_ShouldKeepSchoolContactKeyIsolated_WhenParentSubscribe()
    {
        // 家长关注（ChangeType=subscribe）不得落入 90240 消息族的 subscribe 键 —— 族事件值键消歧的核心断言。
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeSchoolContact,
            ChangeType = "subscribe",
            DecryptedXml = "<xml><Event><![CDATA[change_school_contact]]></Event>" +
                           "<ChangeType><![CDATA[subscribe]]></ChangeType>" +
                           "<Id><![CDATA[zhangsan]]></Id></xml>",
        };

        evt.EventTypeKey.Should().Be("change_school_contact",
            "若逐 ChangeType 键则与本仓已登记的 90240 subscribe 键在契约注册表撞键");

        var matched = CreateReader().Read<SchoolContactChangedPayload>(evt);
        matched.Status.Should().Be(WechatPayloadReadStatus.Matched);
        matched.Payload!.Id.Should().Be("zhangsan");

        var rejected = CreateReader().Read<PlainEventPayload>(evt);
        rejected.Status.Should().Be(WechatPayloadReadStatus.ContractMismatch,
            "90240 的 subscribe 契约要求 Event=subscribe，家校报文 Event=change_school_contact ⇒ RequiredEvent 隔离");
    }

    [Fact]
    public void Read_ShouldMapSchoolContactBatchItems_WhenRepeatedSiblingsPresent()
    {
        // 官方 97281 样报文（第三方指令回调，套件信封）：ChangeList 为根下重复同名复杂兄弟元素（对象列表）。
        var evt = SuiteEvent(WechatCallbackEventTypes.ChangeSchoolContactBatch, changeType: null,
            "<xml><SuiteId><![CDATA[wwSuiteId]]></SuiteId>" +
            "<AuthCorpId><![CDATA[wxAuthCorpId]]></AuthCorpId>" +
            "<InfoType><![CDATA[change_school_contact_batch]]></InfoType>" +
            "<TimeStamp>1403610513</TimeStamp>" +
            "<ChangeList><TimeStamp>1403610513</TimeStamp>" +
            "<ChangeType><![CDATA[create_student]]></ChangeType>" +
            "<Id><![CDATA[zhangsan]]></Id></ChangeList>" +
            "<ChangeList><TimeStamp>1403610514</TimeStamp>" +
            "<ChangeType><![CDATA[update_parent]]></ChangeType>" +
            "<Id><![CDATA[zhangsan-baba]]></Id>" +
            "<NewId><![CDATA[zhangsan-baba-new]]></NewId></ChangeList>" +
            "<ChangeList><TimeStamp>1403610515</TimeStamp>" +
            "<ChangeType><![CDATA[create_department]]></ChangeType>" +
            "<Id><![CDATA[1]]></Id></ChangeList></xml>");

        evt.EventTypeKey.Should().Be("change_school_contact_batch", "批量变更事件键为 InfoType 本身");
        evt.EventFamily.Should().Be(WechatCallbackEventFamily.SchoolContactChange,
            "批量变更事件按外层事件值归入家校沟通族，不得误判为授权族");

        var result = CreateReader().Read<SchoolContactBatchChangedPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var items = result.Payload!.ChangeItems;
        items.Should().HaveCount(3, "重复复杂兄弟元素经合并投影全量读取");
        items[0].ChangeType.Should().Be("create_student");
        items[0].Id.Should().Be("zhangsan");
        items[0].TimeStamp.Should().Be(1403610513);
        items[0].NewId.Should().BeNull();
        items[1].ChangeType.Should().Be("update_parent");
        items[1].NewId.Should().Be("zhangsan-baba-new", "update 项且 userid 被修改时携带 NewId");
        items[1].TimeStamp.Should().Be(1403610514);
        items[2].ChangeType.Should().Be("create_department");
        items[2].Id.Should().Be("1", "批量项的 Id 亦承载家校通讯录部门 id");
        items[2].NewId.Should().BeNull();
    }

    [Fact]
    public void Read_ShouldMapSingleSchoolContactBatchItem_WhenOnlyOneSiblingPresent()
    {
        // 恰 1 个元素时未触发合并投影，RepeatSchoolContactChangeItems 须按单项绑定。
        var result = CreateReader().Read<SchoolContactBatchChangedPayload>(
            SuiteEvent(WechatCallbackEventTypes.ChangeSchoolContactBatch, changeType: null,
                "<xml><SuiteId><![CDATA[wwSuiteId]]></SuiteId>" +
                "<AuthCorpId><![CDATA[wxAuthCorpId]]></AuthCorpId>" +
                "<InfoType><![CDATA[change_school_contact_batch]]></InfoType>" +
                "<TimeStamp>1403610513</TimeStamp>" +
                "<ChangeList><TimeStamp>1403610513</TimeStamp>" +
                "<ChangeType><![CDATA[unsubscribe]]></ChangeType>" +
                "<Id><![CDATA[zhangsan]]></Id></ChangeList></xml>"));

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        var item = result.Payload!.ChangeItems.Should().ContainSingle().Subject;
        item.ChangeType.Should().Be("unsubscribe");
        item.Id.Should().Be("zhangsan");
        item.TimeStamp.Should().Be(1403610513);
    }

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.Suite, true)]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.Suite, false)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.Suite, false)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, false)]
    public void SchoolContactContract_ShouldOpenForOfficialMatrix(
        WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.ChangeSchoolContact, out var contract)
            .Should().BeTrue();
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeSchoolContact,
            ChangeType = "create_student",
        };

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "官方接入矩阵：自建·代开发×应用数据通道（92032/92052/96716/96717）+ 第三方×套件指令通道（92050/92051）");
    }

    // ---------------------------------------- 会话内容存档族（95039；仅自建）

    [Fact]
    public void Read_ShouldMapMsgAuditNotifyFields_WhenEventEnvelope()
    {
        // 官方 95039 样报文（企业自建 Event 信封）：Event 节点即事件键（逐键自指），信封外仅 AgentID。
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.MsgAuditNotify,
            DecryptedXml = "<xml><ToUserName><![CDATA[CorpID]]></ToUserName>" +
                           "<FromUserName><![CDATA[sys]]></FromUserName>" +
                           "<CreateTime>1629101687</CreateTime><MsgType><![CDATA[event]]></MsgType>" +
                           "<AgentID>2000004</AgentID>" +
                           "<Event><![CDATA[msgaudit_notify]]></Event></xml>",
        };

        evt.EventTypeKey.Should().Be("msgaudit_notify", "无 InfoType/ChangeType 段 ⇒ Event 节点即事件键");
        evt.EventFamily.Should().Be(WechatCallbackEventFamily.Unknown,
            "会话内容存档不归类既有事件族，族闸放行、开放面由事件键级声明承载（同邮箱族口径）");

        var result = CreateReader().Read<MsgAuditNotifyPayload>(evt);

        result.Status.Should().Be(WechatPayloadReadStatus.Matched);
        result.Payload!.AgentId.Should().Be("2000004");
        result.Payload!.Values.Should().Contain("Event", "msgaudit_notify",
            "Values 为全量袋（ADR-5），信封字段 Event 亦在袋中（仅凭据节点被排除）");
    }

    [Theory]
    [InlineData(WechatAppType.Internal, WechatCallbackChannel.App, true)]
    [InlineData(WechatAppType.ThirdParty, WechatCallbackChannel.App, false)]
    [InlineData(WechatAppType.Provider, WechatCallbackChannel.App, false)]
    public void MsgAuditContract_ShouldOpenOnlyForInternal(WechatAppType appType, WechatCallbackChannel channel, bool expected)
    {
        CreateRegistry().TryResolve(WechatCallbackEventTypes.MsgAuditNotify, out var contract).Should().BeTrue();
        var evt = new WechatCallbackEvent { Event = WechatCallbackEventTypes.MsgAuditNotify };

        contract!.IsOpenFor(evt, appType, channel).Should().Be(expected,
            "会话内容存档官方仅在企业自建应用开发文档树提供（95039；第三方/代开发无对应事件回调）");
        contract.RequiredFamily.Should().Be(WechatCallbackEventFamily.Unknown);
        contract.RequiredEvent.Should().Be(WechatCallbackEventTypes.MsgAuditNotify,
            "RequiredEvent 缺省 = 逐键自指（Event 节点即事件键）");
    }
}
