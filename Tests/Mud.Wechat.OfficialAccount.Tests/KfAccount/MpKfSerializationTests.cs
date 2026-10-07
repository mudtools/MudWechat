// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.KfAccount;
using Mud.Wechat.OfficialAccount.DataModels.KfSession;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.KfAccount;

/// <summary>客服管理 + 会话控制 DTO 映射与 DI 注册。</summary>
public class MpKfSerializationTests
{
    /// <summary>K1：客服账号列表解析（<c>kf_id</c> 为<b>字符串</b>）。</summary>
    [Fact]
    public void GetKfListResponse_ShouldParseAccountList()
    {
        const string json = """
            {"kf_list":[
              {"kf_account":"test1@test","kf_nick":"客服1","kf_headimgurl":"https://example.com/a.png","kf_id":"1001"}
            ]}
            """;

        var response = JsonSerializer.Deserialize(json, KfAccountJsonContext.Default.MpGetKfListResponse);

        response!.IsSuccess.Should().BeTrue();
        response.KfList.Should().HaveCount(1);
        response.KfList![0].KfAccount.Should().Be("test1@test");
        response.KfList[0].KfNick.Should().Be("客服1");
        response.KfList[0].KfId.Should().Be("1001", "官方 getkflist 页 kf_id 为字符串");
    }

    /// <summary>K2：在线客服列表解析（<c>kf_id</c> 为<b>数值</b> + 在线状态 + 接待中会话数）。</summary>
    [Fact]
    public void GetOnlineKfListResponse_ShouldParseOnlineAccounts()
    {
        const string json = """
            {"kf_online_list":[
              {"kf_account":"test1@test","status":1,"kf_id":1001,"accepted_case":2,"kf_openid":"o1"}
            ]}
            """;

        var response = JsonSerializer.Deserialize(json, KfAccountJsonContext.Default.MpGetOnlineKfListResponse);

        response!.KfOnlineList.Should().HaveCount(1);
        response.KfOnlineList![0].Status.Should().Be(MpKfOnlineStatus.WebOnline);
        response.KfOnlineList[0].KfId.Should().Be(1001, "官方在线列表页 kf_id 为数值 ⇒ 与账号列表页形态不同");
        response.KfOnlineList[0].AcceptedCase.Should().Be(2, "可用于按负载分配客服");
    }

    /// <summary>K3：客服账号增删改与邀请绑定的请求体字段名（官方字段表照抄）。</summary>
    [Fact]
    public void KfAccountRequests_ShouldSerializeToOfficialShape()
    {
        using var addDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpAddKfAccountRequest { KfAccount = "test1@test", Nickname = "客服1" },
            KfAccountJsonContext.Default.MpAddKfAccountRequest));
        addDoc.RootElement.GetProperty("kf_account").GetString().Should().Be("test1@test");
        addDoc.RootElement.GetProperty("nickname").GetString().Should().Be("客服1");
        addDoc.RootElement.TryGetProperty("password", out _).Should().BeFalse("官方无 password 字段");
        addDoc.RootElement.TryGetProperty("business_id", out _).Should().BeFalse("普通账号不填 business_id（null 不序列化）");

        using var updateDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpUpdateKfAccountRequest { KfAccount = "test1@test", Nickname = "客服2" },
            KfAccountJsonContext.Default.MpUpdateKfAccountRequest));
        updateDoc.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "kf_account", "nickname" }, "官方修改页无 business_id");

        using var delDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpDelKfAccountRequest { KfAccount = "test1@test" },
            KfAccountJsonContext.Default.MpDelKfAccountRequest));
        delDoc.RootElement.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "kf_account" });

        using var inviteDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInviteKfWorkerRequest { KfAccount = "test1@test", InviteWx = "wxid_abc" },
            KfAccountJsonContext.Default.MpInviteKfWorkerRequest));
        inviteDoc.RootElement.GetProperty("invite_wx").GetString().Should().Be("wxid_abc");
    }

    /// <summary>K4：创建 / 关闭会话请求体（<b>共用同一 DTO</b>）序列化。</summary>
    [Fact]
    public void KfSessionRequest_ShouldSerializeForBothCreateAndClose()
    {
        var request = new MpKfSessionRequest { KfAccount = "test1@test", OpenId = "o1" };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, KfSessionJsonContext.Default.MpKfSessionRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "kf_account", "openid" },
                "官方创建 / 关闭会话字段表一致 ⇒ 共用 DTO");
    }

    /// <summary>K5：会话状态与会话列表解析（<c>createtime</c> + <c>kf_account</c> / <c>openid</c>）。</summary>
    [Fact]
    public void KfSessionResponses_ShouldParse()
    {
        const string statusJson = """{"createtime":1464710400,"kf_account":"test1@test"}""";
        var status = JsonSerializer.Deserialize(statusJson, KfSessionJsonContext.Default.MpGetKfSessionResponse);
        status!.CreateTime.Should().Be(1464710400);
        status.KfAccount.Should().Be("test1@test");

        const string listJson = """{"sessionlist":[{"createtime":1464710400,"openid":"o1"}]}""";
        var list = JsonSerializer.Deserialize(listJson, KfSessionJsonContext.Default.MpGetKfSessionListResponse);
        list!.SessionList.Should().HaveCount(1);
        list.SessionList![0].OpenId.Should().Be("o1");
    }

    /// <summary>K6：未接入会话列表解析（官方元素字段仅 <c>latest_time</c> + <c>openid</c>）。</summary>
    [Fact]
    public void GetWaitCaseResponse_ShouldParseWaitingCases()
    {
        const string json = """{"count":2,"waitcaselist":[{"latest_time":1464710400,"openid":"o1"},{"latest_time":1464710500,"openid":"o2"}]}""";

        var response = JsonSerializer.Deserialize(json, KfSessionJsonContext.Default.MpGetWaitCaseResponse);

        response!.Count.Should().Be(2);
        response.WaitCaseList.Should().HaveCount(2);
        response.WaitCaseList![0].LatestTime.Should().Be(1464710400);
        response.WaitCaseList[1].OpenId.Should().Be("o2");
    }

    /// <summary>K7：失败响应判定（<c>65415</c> 客服不在线 / <c>65405</c> 账号数超限）。</summary>
    [Fact]
    public void FailedResponses_ShouldExposeErrorCodes()
    {
        var session = JsonSerializer.Deserialize(
            """{"errcode":65415,"errmsg":"the worker is not online"}""",
            KfSessionJsonContext.Default.MpGetKfSessionResponse);
        session!.IsSuccess.Should().BeFalse();
        session.ErrorCode.Should().Be(MpErrorCodes.WorkerNotOnline);

        var accounts = JsonSerializer.Deserialize(
            """{"errcode":65405,"errmsg":"custom service account number reach limit"}""",
            KfAccountJsonContext.Default.MpGetKfListResponse);
        accounts!.ErrorCode.Should().Be(MpErrorCodes.KfAccountCountExceeded);
        accounts.KfList.Should().BeNull("失败响应不含 kf_list");
    }

    /// <summary>K8：两个域的注册入口均可解析出服务（生成器注册链闭环）。</summary>
    [Fact]
    public void KfApis_ShouldBeResolvableFromDi()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(config =>
        {
            config.AppKey = "mp-kf";
            config.AppId = "wx-kf";
            config.AppSecret = "s-kf";
        });
        services.AddMpServices(builder => builder.AddKfAccountApi().AddKfSessionApi());

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IMpKfAccountService>().Should().NotBeNull();
        provider.GetRequiredService<IMpKfSessionService>().Should().NotBeNull();
    }
}
