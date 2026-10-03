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
        payload.SendPicsInfo!.PicMd5Sums.Should().HaveCount(1);
        payload.SendPicsInfo!.PicMd5Sums[0].Should().Be("1b5f7c23b5bf75682a53e7b6d163e185");
        payload.SendPicsInfo!.PicMd5Sums.Should().NotContain(string.Empty);
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
}
