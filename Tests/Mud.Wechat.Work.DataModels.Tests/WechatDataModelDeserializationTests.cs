// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.CorpTokenAuthentication;
using Mud.Wechat.Work.DataModels.InternalAppAuthentication;
using Mud.Wechat.Work.DataModels.ProviderAuthentication;

namespace Mud.Wechat.Work.DataModels.Tests;

/// <summary>
/// DTO 反序列化测试：官方示例 JSON → DTO 字段逐一断言（详细设计 §18.5）。
/// </summary>
public class WechatDataModelDeserializationTests
{
    [Fact]
    public void GetTokenResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """{"errcode":0,"errmsg":"ok","access_token":"ac-1","expires_in":7200}""";
        var resp = JsonSerializer.Deserialize<GetTokenResponse>(json);

        resp!.ErrorCode.Should().Be(0);
        resp.ErrorMessage.Should().Be("ok");
        resp.AccessToken.Should().Be("ac-1");
        resp.ExpiresIn.Should().Be(7200);
        resp.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void WechatWorkResponse_ShouldDeserialize_ConcreteBase()
    {
        var json = """{"errcode":0,"errmsg":"ok"}""";
        var resp = JsonSerializer.Deserialize<WechatWorkResponse>(json);

        resp.Should().NotBeNull("基底为具体类才能作为 SetSessionInfoAsync 的返回类型被反序列化");
        resp!.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetProviderTokenResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","provider_access_token":"pat-1","expires_in":7200}""";
        var resp = JsonSerializer.Deserialize<GetProviderTokenResponse>(json);

        resp!.ProviderAccessToken.Should().Be("pat-1");
        resp.ExpiresIn.Should().Be(7200);
    }

    [Fact]
    public void GetSuiteTokenResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","suite_access_token":"sat-1","expires_in":7200}""";
        var resp = JsonSerializer.Deserialize<GetSuiteTokenResponse>(json);

        resp!.SuiteAccessToken.Should().Be("sat-1");
    }

    [Fact]
    public void GetPreAuthCodeResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","pre_auth_code":"pac-1","expires_in":1800}""";
        var resp = JsonSerializer.Deserialize<GetPreAuthCodeResponse>(json);

        resp!.PreAuthCode.Should().Be("pac-1");
        resp.ExpiresIn.Should().Be(1800);
    }

    [Fact]
    public void GetPermanentCodeResponse_ShouldDeserialize_WithNestedAuthInfo()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "access_token": "corp-at",
          "expires_in": 7200,
          "permanent_code": "pc-1",
          "auth_corp_info": { "corpid": "ww-auth", "corp_name": "授权企业" },
          "auth_info": { "agent": [ { "agentid": 1000002, "name": "代开发应用" } ] },
          "auth_user_info": { "userid": "zhangsan" }
        }
        """;
        var resp = JsonSerializer.Deserialize<GetPermanentCodeResponse>(json);

        resp!.AccessToken.Should().Be("corp-at");
        resp.PermanentCode.Should().Be("pc-1");
        resp.AuthCorpInfo.CorpId.Should().Be("ww-auth");
        resp.AuthCorpInfo.CorpName.Should().Be("授权企业");
        resp.AuthInfo.Agents.Should().HaveCount(1);
        resp.AuthInfo.Agents[0].AgentId.Should().Be(1000002);
        resp.AuthUserInfo.UserId.Should().Be("zhangsan");
    }

    [Fact]
    public void GetAuthInfoResponse_ShouldDeserialize()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "dealer_corp_info": { "corpid": "ww-dealer" },
          "auth_corp_info": { "corpid": "ww-auth" },
          "auth_info": { "agent": [] }
        }
        """;
        var resp = JsonSerializer.Deserialize<GetAuthInfoResponse>(json);

        resp!.DealerCorpInfo.CorpId.Should().Be("ww-dealer");
        resp.AuthCorpInfo.CorpId.Should().Be("ww-auth");
        resp.AuthInfo.Agents.Should().BeEmpty();
    }

    [Fact]
    public void GetCorpTokenResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","access_token":"corp-at-2","expires_in":7200}""";
        var resp = JsonSerializer.Deserialize<GetCorpTokenResponse>(json);

        resp!.AccessToken.Should().Be("corp-at-2");
        resp.ExpiresIn.Should().Be(7200);
    }

    [Fact]
    public void RequestModels_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var providerReq = JsonSerializer.Serialize(new GetProviderTokenRequest { CorpId = "ww-1", ProviderSecret = "s1" });
        providerReq.Should().Contain("\"corpid\":\"ww-1\"");
        providerReq.Should().Contain("\"provider_secret\":\"s1\"");

        var suiteReq = JsonSerializer.Serialize(new GetSuiteTokenRequest { SuiteId = "sid", SuiteSecret = "ss", SuiteTicket = "st" });
        suiteReq.Should().Contain("\"suite_id\":\"sid\"");
        suiteReq.Should().Contain("\"suite_secret\":\"ss\"");
        suiteReq.Should().Contain("\"suite_ticket\":\"st\"");

        var corpReq = JsonSerializer.Serialize(new GetCorpTokenRequest { AuthCorpId = "ww-2", PermanentCode = "pc" });
        corpReq.Should().Contain("\"auth_corpid\":\"ww-2\"");
        corpReq.Should().Contain("\"permanent_code\":\"pc\"");
    }

    [Fact]
    public void GetAuthInfoRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var json = JsonSerializer.Serialize(new GetAuthInfoRequest { AuthorizerCorpId = "ww-3", PermanentAuthCode = "pc-3" });
        json.Should().Contain("\"auth_corpid\":\"ww-3\"");
        json.Should().Contain("\"permanent_code\":\"pc-3\"");
    }
}
