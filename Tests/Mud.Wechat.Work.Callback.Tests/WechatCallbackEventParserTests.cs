// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Callback.Events;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 强类型事件解析器测试（v1 方案 §5.5 / §8）：7 类通讯录变更 + 异步任务官方报文逐一解析、
/// 事件键不匹配返回 null、敏感字段缺失不抛、逗号/竖线分隔串转换助手、ExtAttr 嵌套解析。
/// </summary>
public class WechatCallbackEventParserTests
{
    private static WechatCallbackEvent Event(string changeType, string plainXml)
        => new()
        {
            Event = WechatCallbackEventTypes.ChangeContact,
            ChangeType = changeType,
            DecryptedXml = plainXml,
        };

    private static WechatCallbackEvent BatchEvent(string plainXml)
        => new()
        {
            Event = WechatCallbackEventTypes.BatchJobResult,
            DecryptedXml = plainXml,
        };

    // ---------------------------------------------------------------- 成员事件

    [Fact]
    public void ParseUserCreated_ShouldMapOfficialFields()
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

        var dto = WechatCallbackEventParser.ParseUserCreated(evt);

        dto.Should().NotBeNull();
        dto!.UserID.Should().Be("zhangsan");
        dto.Name.Should().Be("张三");
        dto.Department.Should().Be("1,2,3");
        dto.MainDepartment.Should().Be("1");
        dto.IsLeaderInDept.Should().Be("1,0,0");
        dto.DirectLeader.Should().Be("lisi|wangwu");
        dto.Position.Should().Be("工程师");
        dto.Mobile.Should().Be("13800000000");
        dto.Gender.Should().Be("1");
        dto.Email.Should().Be("z@corp.com");
        dto.Status.Should().Be("1");
        dto.Avatar.Should().Be("http://a/b.png");
        dto.Alias.Should().Be("z");
        dto.Telephone.Should().Be("010-123");
        dto.Address.Should().Be("北京");
        dto.ExtAttr.Should().HaveCount(1);
        dto.ExtAttr[0].Name.Should().Be("工号");
        dto.ExtAttr[0].Type.Should().Be("0");
        dto.ExtAttr[0].Value.Should().Be("A1001");
    }

    [Fact]
    public void ParseUserCreated_ShouldReturnNull_WhenSensitiveFieldsAbsent()
    {
        // 2022-08-15 后新 URL：通讯录助手仅回调 UserID/Department 子集——字段缺失必须解析为 null 不抛。
        var evt = Event(WechatCallbackEventTypes.CreateUser,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[create_user]]></ChangeType>" +
            "<UserID><![CDATA[lisi]]></UserID><Department>1,2</Department></xml>");

        var dto = WechatCallbackEventParser.ParseUserCreated(evt);

        dto.Should().NotBeNull();
        dto!.UserID.Should().Be("lisi");
        dto.Name.Should().BeNull("敏感字段未授权时不返回，处理器不得假设必有值");
        dto.Mobile.Should().BeNull();
        dto.ExtAttr.Should().BeEmpty();
    }

    [Fact]
    public void ParseUserUpdated_ShouldMapNewUserId()
    {
        var evt = Event(WechatCallbackEventTypes.UpdateUser,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[update_user]]></ChangeType>" +
            "<UserID><![CDATA[old-id]]></UserID><NewUserID><![CDATA[new-id]]></NewUserID>" +
            "<Department>5</Department></xml>");

        var dto = WechatCallbackEventParser.ParseUserUpdated(evt);

        dto.Should().NotBeNull();
        dto!.UserID.Should().Be("old-id");
        dto.NewUserID.Should().Be("new-id");
        dto.Department.Should().Be("5");
    }

    [Fact]
    public void ParseUserDeleted_ShouldMapUserId()
    {
        var evt = Event(WechatCallbackEventTypes.DeleteUser,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[delete_user]]></ChangeType>" +
            "<UserID><![CDATA[zhangsan]]></UserID></xml>");

        var dto = WechatCallbackEventParser.ParseUserDeleted(evt);

        dto!.UserID.Should().Be("zhangsan");
    }

    // ---------------------------------------------------------------- 部门事件

    [Fact]
    public void ParsePartyCreated_ShouldMapOfficialFields()
    {
        var evt = Event(WechatCallbackEventTypes.CreateParty,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[create_party]]></ChangeType>" +
            "<Id>2</Id><Name><![CDATA[研发部]]></Name><ParentId>1</ParentId><Order>10</Order></xml>");

        var dto = WechatCallbackEventParser.ParsePartyCreated(evt);

        dto!.Id.Should().Be("2");
        dto.Name.Should().Be("研发部");
        dto.ParentId.Should().Be("1");
        dto.Order.Should().Be("10");
    }

    [Fact]
    public void ParsePartyUpdated_ShouldMapFields()
    {
        var evt = Event(WechatCallbackEventTypes.UpdateParty,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[update_party]]></ChangeType>" +
            "<Id>2</Id><Name><![CDATA[研发部]]></Name><ParentId>3</ParentId></xml>");

        var dto = WechatCallbackEventParser.ParsePartyUpdated(evt);

        dto!.Id.Should().Be("2");
        dto.ParentId.Should().Be("3");
    }

    [Fact]
    public void ParsePartyDeleted_ShouldMapId()
    {
        var evt = Event(WechatCallbackEventTypes.DeleteParty,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[delete_party]]></ChangeType><Id>2</Id></xml>");

        var dto = WechatCallbackEventParser.ParsePartyDeleted(evt);

        dto!.Id.Should().Be("2");
    }

    // ---------------------------------------------------------------- 标签 / 异步任务

    [Fact]
    public void ParseTagUpdated_ShouldMapMemberLists()
    {
        var evt = Event(WechatCallbackEventTypes.UpdateTag,
            "<xml><Event><![CDATA[change_contact]]></Event><ChangeType><![CDATA[update_tag]]></ChangeType>" +
            "<TagId>7</TagId><AddUserItems><![CDATA[zhangsan,lisi]]></AddUserItems>" +
            "<DelUserItems><![CDATA[wangwu]]></DelUserItems><AddPartyItems>4,5</AddPartyItems>" +
            "<DelPartyItems>6</DelPartyItems></xml>");

        var dto = WechatCallbackEventParser.ParseTagUpdated(evt);

        dto!.TagId.Should().Be("7");
        dto.AddUserItems.Should().Be("zhangsan,lisi");
        dto.DelUserItems.Should().Be("wangwu");
        dto.AddPartyItems.Should().Be("4,5");
        dto.DelPartyItems.Should().Be("6");
    }

    [Fact]
    public void ParseBatchJobResult_ShouldMapOfficialFields()
    {
        var evt = BatchEvent(
            "<xml><Event><![CDATA[batch_job_result]]></Event><JobId><![CDATA[job-abc]]></JobId>" +
            "<JobType><![CDATA[replace_user]]></JobType><ErrCode>0</ErrCode><ErrMsg>ok</ErrMsg></xml>");

        var dto = WechatCallbackEventParser.ParseBatchJobResult(evt);

        dto!.JobId.Should().Be("job-abc");
        dto.JobType.Should().Be("replace_user");
        dto.ErrCode.Should().Be("0");
        dto.ErrMsg.Should().Be("ok");
    }

    // ---------------------------------------------------------------- 键不匹配 / 非法输入

    [Fact]
    public void Parser_ShouldReturnNull_WhenEventTypeKeyMismatched()
    {
        // 解析器按信封 ChangeType 判定（不重读报文）：create_user 解析器拒绝 delete_party 事件。
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeContact,
            ChangeType = WechatCallbackEventTypes.DeleteParty,
            DecryptedXml = "<xml><ChangeType><![CDATA[delete_party]]></ChangeType><Id>2</Id></xml>",
        };

        WechatCallbackEventParser.ParseUserCreated(evt).Should().BeNull("ChangeType 与目标 DTO 不匹配");
    }

    [Fact]
    public void Parser_ShouldReturnNull_WhenDecryptedXmlMissing()
    {
        var evt = new WechatCallbackEvent
        {
            Event = WechatCallbackEventTypes.ChangeContact,
            ChangeType = WechatCallbackEventTypes.CreateUser,
            DecryptedXml = null,
        };

        WechatCallbackEventParser.ParseUserCreated(evt).Should().BeNull("明文缺失返回 null 不抛");
    }

    [Fact]
    public void Parser_ShouldReturnNull_WhenDecryptedXmlMalformed()
    {
        var evt = Event(WechatCallbackEventTypes.CreateUser, "not-xml-at-all");

        WechatCallbackEventParser.ParseUserCreated(evt).Should().BeNull("明文非 XML 返回 null 不抛");
    }

    [Fact]
    public void Parser_ShouldReturnNull_WhenEventNull()
    {
        WechatCallbackEventParser.ParseUserCreated(null!).Should().BeNull();
        WechatCallbackEventParser.ParseBatchJobResult(null!).Should().BeNull();
    }

    // ---------------------------------------------------------------- 转换助手

    [Fact]
    public void ParseIdList_ShouldSplitCommaSeparatedIds()
    {
        WechatCallbackEventParser.ParseIdList("1,2,3").Should().Equal(1L, 2L, 3L);
        WechatCallbackEventParser.ParseIdList(" 4 , 5 ").Should().Equal(4L, 5L);
        WechatCallbackEventParser.ParseIdList("").Should().BeEmpty();
        WechatCallbackEventParser.ParseIdList(null).Should().BeEmpty();
        WechatCallbackEventParser.ParseIdList("1,x,3").Should().Equal(new[] { 1L, 3L }, "非法片段跳过");
    }

    [Fact]
    public void ParseTextList_ShouldSplitBySeparator()
    {
        WechatCallbackEventParser.ParseTextList("lisi|wangwu", '|').Should().Equal("lisi", "wangwu");
        WechatCallbackEventParser.ParseTextList("zhangsan,lisi", ',').Should().Equal("zhangsan", "lisi");
        WechatCallbackEventParser.ParseTextList("", ',').Should().BeEmpty();
        WechatCallbackEventParser.ParseTextList(null, '|').Should().BeEmpty();
    }
}
