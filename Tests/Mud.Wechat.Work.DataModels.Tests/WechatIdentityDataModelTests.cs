// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Identity;

namespace Mud.Wechat.Work.DataModels.Tests;

/// <summary>
/// 「身份验证」域 DTO JSON 契约测试：官方示例 JSON → DTO 字段逐一断言（含官方契约陷阱锁定）。
/// </summary>
public class WechatIdentityDataModelTests
{
    [Fact]
    public void GetUserDetailRequest_ShouldSerialize_UnderUserTicketKey()
    {
        var json = JsonSerializer.Serialize(
            new GetUserDetailRequest { UserTicket = "USER_TICKET" },
            IdentityJsonContext.Default.GetUserDetailRequest);
        json.Should().Contain("\"user_ticket\":\"USER_TICKET\"");
    }

    [Fact]
    public void GetUserInfoResponse_ShouldDeserialize_WithMemberAndNonMemberMutuallyExclusiveSuperset()
    {
        // 企业成员形态（snsapi_privateinfo + 可见范围内）。
        const string memberJson = """
            { "errcode": 0, "errmsg": "ok", "userid": "zhangsan", "user_ticket": "USER_TICKET", "user_doc_ticket": "USER_DOC_TICKET" }
            """;
        var member = JsonSerializer.Deserialize(memberJson, IdentityJsonContext.Default.GetUserInfoResponse);
        member.Should().NotBeNull();
        member!.IsSuccess.Should().BeTrue();
        member.Userid.Should().Be("zhangsan");
        member.UserTicket.Should().Be("USER_TICKET");
        member.UserDocTicket.Should().Be("USER_DOC_TICKET");
        member.Openid.Should().BeNull();
        member.ExternalUserid.Should().BeNull();

        // 非企业成员形态（与成员字段互斥，DTO 为可空超集）。
        const string nonMemberJson = """
            { "errcode": 0, "errmsg": "ok", "openid": "oAAAA", "external_userid": "woBBBB" }
            """;
        var nonMember = JsonSerializer.Deserialize(nonMemberJson, IdentityJsonContext.Default.GetUserInfoResponse);
        nonMember.Should().NotBeNull();
        nonMember!.Userid.Should().BeNull();
        nonMember.UserTicket.Should().BeNull();
        nonMember.Openid.Should().Be("oAAAA");
        nonMember.ExternalUserid.Should().Be("woBBBB");
    }

    [Fact]
    public void GetUserDetailResponse_ShouldDeserialize_WithGenderAsStringSensitiveFields()
    {
        var json = """
            { "errcode": 0, "errmsg": "ok", "userid": "lisi", "gender": "1",
              "avatar": "http://shp.qpic.cn/bizmp/xxxxxxxxxxx/0",
              "qr_code": "https://open.work.weixin.qq.com/wwopen/userQRCode?vcode=vc",
              "mobile": "13800000000", "email": "zhangsan@gzdev.com",
              "biz_mail": "zhangsan@qyycs2.wecom.work", "address": "广州市海珠区新港中路" }
            """;
        var resp = JsonSerializer.Deserialize(json, IdentityJsonContext.Default.GetUserDetailResponse);
        resp.Should().NotBeNull();
        resp!.IsSuccess.Should().BeTrue();
        resp.Userid.Should().Be("lisi");
        // 官方契约陷阱：gender 为字符串枚举（"0"/"1"/"2"），不是数字。
        resp.Gender.Should().Be("1");
        resp.Avatar.Should().Be("http://shp.qpic.cn/bizmp/xxxxxxxxxxx/0");
        resp.QrCode.Should().Be("https://open.work.weixin.qq.com/wwopen/userQRCode?vcode=vc");
        resp.Mobile.Should().Be("13800000000");
        resp.Email.Should().Be("zhangsan@gzdev.com");
        resp.BizMail.Should().Be("zhangsan@qyycs2.wecom.work");
        resp.Address.Should().Be("广州市海珠区新港中路");
    }

    [Fact]
    public void GetUserInfo3rdResponse_ShouldDeserialize_WithCorpMemberAndOpenidOnlyShapes()
    {
        // 用户属于某企业形态（无授权关系时 userid 为密文；expires_in 随 user_ticket 返回）。
        const string memberJson = """
            { "errcode": 0, "errmsg": "ok", "corpid": "wwCorpId", "userid": "USERID",
              "user_ticket": "USER_TICKET", "expires_in": 7200, "open_userid": "wwOpenUserId" }
            """;
        var member = JsonSerializer.Deserialize(memberJson, IdentityJsonContext.Default.GetUserInfo3rdResponse);
        member.Should().NotBeNull();
        member!.IsSuccess.Should().BeTrue();
        member.Corpid.Should().Be("wwCorpId");
        member.Userid.Should().Be("USERID");
        member.UserTicket.Should().Be("USER_TICKET");
        member.ExpiresIn.Should().Be(7200);
        member.OpenUserid.Should().Be("wwOpenUserId");
        member.Openid.Should().BeNull();

        // 用户不属于任何企业形态。
        const string nonMemberJson = """{ "errcode": 0, "errmsg": "ok", "openid": "oAAAA" }""";
        var nonMember = JsonSerializer.Deserialize(nonMemberJson, IdentityJsonContext.Default.GetUserInfo3rdResponse);
        nonMember.Should().NotBeNull();
        nonMember!.Corpid.Should().BeNull();
        nonMember.Userid.Should().BeNull();
        nonMember.Openid.Should().Be("oAAAA");
    }

    [Fact]
    public void GetUserDetail3rdResponse_ShouldDeserialize_WithoutContactSensitiveFields()
    {
        var json = """
            { "errcode": 0, "errmsg": "ok", "corpid": "wwxxxxxxyyyyy", "userid": "lisi", "name": "lisi",
              "gender": "1", "avatar": "http://shp.qpic.cn/bizmp/xxxxxxxxxxx/0",
              "qr_code": "https://open.work.weixin.qq.com/wwopen/userQRCode?vcode=vc" }
            """;
        var resp = JsonSerializer.Deserialize(json, IdentityJsonContext.Default.GetUserDetail3rdResponse);
        resp.Should().NotBeNull();
        resp!.IsSuccess.Should().BeTrue();
        resp.Corpid.Should().Be("wwxxxxxxyyyyy");
        resp.Userid.Should().Be("lisi");
        resp.Name.Should().Be("lisi");
        resp.Gender.Should().Be("1");
        resp.Avatar.Should().Be("http://shp.qpic.cn/bizmp/xxxxxxxxxxx/0");
        resp.QrCode.Should().Be("https://open.work.weixin.qq.com/wwopen/userQRCode?vcode=vc");

        // 官方契约陷阱：第三方版响应面不含手机 / 邮箱 / 企业邮箱 / 地址（与自建/代开发版 GetUserDetailResponse 不同）。
        typeof(GetUserDetail3rdResponse).GetProperty("Mobile").Should().BeNull("第三方版响应面不含 mobile 字段");
        typeof(GetUserDetail3rdResponse).GetProperty("Email").Should().BeNull("第三方版响应面不含 email 字段");
        typeof(GetUserDetail3rdResponse).GetProperty("BizMail").Should().BeNull("第三方版响应面不含 biz_mail 字段");
        typeof(GetUserDetail3rdResponse).GetProperty("Address").Should().BeNull("第三方版响应面不含 address 字段");
    }

    [Fact]
    public void TfaContracts_ShouldSerializeRequests_AndDeserializeResponse()
    {
        var requestJson = JsonSerializer.Serialize(
            new GetTfaInfoRequest { Code = "AUTH_CODE" },
            IdentityJsonContext.Default.GetTfaInfoRequest);
        requestJson.Should().Contain("\"code\":\"AUTH_CODE\"");

        var tfaSuccJson = JsonSerializer.Serialize(
            new TfaSuccRequest { Userid = "USERID", TfaCode = "TFA_CODE" },
            IdentityJsonContext.Default.TfaSuccRequest);
        tfaSuccJson.Should().Contain("\"userid\":\"USERID\"");
        tfaSuccJson.Should().Contain("\"tfa_code\":\"TFA_CODE\"");

        // 官方契约：null 字段不落 JSON（DefaultIgnoreCondition = WhenWritingNull）。
        var partialJson = JsonSerializer.Serialize(
            new TfaSuccRequest { Userid = "USERID" },
            IdentityJsonContext.Default.TfaSuccRequest);
        partialJson.Should().Be("{\"userid\":\"USERID\"}");

        const string responseJson = """{ "errcode": 0, "errmsg": "ok", "userid": "USERID", "tfa_code": "TFA_CODE" }""";
        var resp = JsonSerializer.Deserialize(responseJson, IdentityJsonContext.Default.GetTfaInfoResponse);
        resp.Should().NotBeNull();
        resp!.IsSuccess.Should().BeTrue();
        resp.Userid.Should().Be("USERID");
        resp.TfaCode.Should().Be("TFA_CODE");
    }
}
