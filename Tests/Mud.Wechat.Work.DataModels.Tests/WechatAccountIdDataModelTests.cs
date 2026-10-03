// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work.DataModels.Tests;

/// <summary>
/// 「账号ID」域 DTO JSON 契约测试：官方示例 JSON → DTO 字段逐一断言（含官方契约陷阱锁定）。
/// </summary>
public class WechatAccountIdDataModelTests
{
    [Fact]
    public void ConvertExternalUserIdToPendingIdRequest_ShouldSerialize_UnderExternalUseridKey_WithoutListSuffix()
    {
        // 官方契约陷阱：external_userid 查询 pending_id 的请求体数组字段名为 external_userid（无 _list 后缀）。
        var json = JsonSerializer.Serialize(
            new ConvertExternalUserIdToPendingIdRequest
            {
                ExternalUserId = new() { "oAAAAAAA", "oBBBBB" },
                ChatId = "wrOgQhDgAA",
            },
            AccountIdJsonContext.Default.ConvertExternalUserIdToPendingIdRequest);

        json.Should().Contain("\"external_userid\":[\"oAAAAAAA\",\"oBBBBB\"]",
            "官方请求体数组字段名不带 _list 后缀，与其它转换接口不同");
        json.Should().Contain("\"chat_id\":\"wrOgQhDgAA\"");
    }

    [Fact]
    public void ConvertTmpExternalUserIdResponse_ShouldDeserialize_WithUserTypeDiscriminatedSuperset()
    {
        // 官方按 user_type 返回不同字段：可空超集覆盖 1-客户（external_userid）与 2/3/4（corpid + userid）。
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "results": [
            { "tmp_external_userid": "ouXXX1", "external_userid": "EXTERNAL_USER_ID" },
            { "tmp_external_userid": "ouXXX2", "corpid": "CORPID", "userid": "USERID" }
          ],
          "invalid_tmp_external_userid_list": ["ouXXX3"]
        }
        """;
        var resp = JsonSerializer.Deserialize(
            json, AccountIdJsonContext.Default.ConvertTmpExternalUserIdResponse);

        resp!.IsSuccess.Should().BeTrue();
        resp.Results.Should().HaveCount(2);
        resp.Results![0].ExternalUserId.Should().Be("EXTERNAL_USER_ID");
        resp.Results[0].CorpId.Should().BeNull();
        resp.Results[0].UserId.Should().BeNull();
        resp.Results[1].ExternalUserId.Should().BeNull();
        resp.Results[1].CorpId.Should().Be("CORPID");
        resp.Results[1].UserId.Should().Be("USERID");
        resp.InvalidTmpExternalUserIdList.Should().BeEquivalentTo(new[] { "ouXXX3" });
    }

    [Fact]
    public void FinishOpenIdMigrationRequest_ShouldSerialize_WithOpenIdTypeArray_AndOmitAgentid_WhenNull()
    {
        // 官方契约：openid_type 为数组；第三方场景（99375）无 agentid 字段、代开发场景（99378）携带。
        var json = JsonSerializer.Serialize(
            new FinishOpenIdMigrationRequest
            {
                CorpId = "ww-1",
                OpenIdType = new() { 1, 3 },
            },
            AccountIdJsonContext.Default.FinishOpenIdMigrationRequest);

        json.Should().Contain("\"corpid\":\"ww-1\"");
        json.Should().Contain("\"openid_type\":[1,3]");
        json.Should().NotContain("\"agentid\"", "agentid 为可选字段（仅代开发场景携带），null 时不落 JSON");
    }

    [Fact]
    public void OpenUserIdToUserIdResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        // openuserid_to_userid 双场景（95884 对接 / 101521 智能机器人）共用响应结构。
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "userid_list": [ { "open_userid": "xxx", "userid": "aaa" } ],
          "invalid_open_userid_list": ["yyy"]
        }
        """;
        var resp = JsonSerializer.Deserialize(
            json, AccountIdJsonContext.Default.OpenUserIdToUserIdResponse);

        resp!.IsSuccess.Should().BeTrue();
        resp.UserIdList.Should().ContainSingle();
        resp.UserIdList![0].OpenUserId.Should().Be("xxx");
        resp.UserIdList[0].UserId.Should().Be("aaa");
        resp.InvalidOpenUserIdList.Should().BeEquivalentTo(new[] { "yyy" });
    }

    [Fact]
    public void UserIdToOpenUserIdResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "",
          "open_userid_list": [ { "userid": "aaa", "open_userid": "xxxxx" } ],
          "invalid_userid_list": ["bbb"]
        }
        """;
        var resp = JsonSerializer.Deserialize(
            json, AccountIdJsonContext.Default.UserIdToOpenUserIdResponse);

        resp!.IsSuccess.Should().BeTrue();
        resp.OpenUserIdList.Should().ContainSingle();
        resp.OpenUserIdList![0].UserId.Should().Be("aaa");
        resp.OpenUserIdList[0].OpenUserId.Should().Be("xxxxx");
        resp.InvalidUserIdList.Should().BeEquivalentTo(new[] { "bbb" });
    }

    [Fact]
    public void ServiceUserIdToOpenUserIdResponse_ShouldDeserialize_WithBotScenarioContract()
    {
        // 智能机器人场景（97106 接口二）：open_corpid + items + invalid_open_userid_list。
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "items": [ { "userid": "woxxxx", "open_userid": "wonewxxxx" } ],
          "open_corpid": "wpxnigenogneg",
          "invalid_open_userid_list": ["userid1"]
        }
        """;
        var resp = JsonSerializer.Deserialize(
            json, AccountIdJsonContext.Default.ServiceUserIdToOpenUserIdResponse);

        resp!.IsSuccess.Should().BeTrue();
        resp.OpenCorpId.Should().Be("wpxnigenogneg");
        resp.Items.Should().ContainSingle();
        resp.Items![0].UserId.Should().Be("woxxxx");
        resp.Items[0].OpenUserId.Should().Be("wonewxxxx");
        resp.InvalidOpenUserIdList.Should().BeEquivalentTo(new[] { "userid1" });
    }

    [Fact]
    public void GetOpenIdMigrationResponse_ShouldDeserialize_WithMigrationInfoArray()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "",
          "migration_info": [
            { "openid_type": 1, "status": 0 },
            { "openid_type": 3, "status": 1 }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize(
            json, AccountIdJsonContext.Default.GetOpenIdMigrationResponse);

        resp!.IsSuccess.Should().BeTrue();
        resp.MigrationInfo.Should().HaveCount(2);
        resp.MigrationInfo![0].OpenIdType.Should().Be(1);
        resp.MigrationInfo[0].Status.Should().Be(0);
        resp.MigrationInfo[1].OpenIdType.Should().Be(3);
        resp.MigrationInfo[1].Status.Should().Be(1);
    }

    [Fact]
    public void ConvertUnionidToExternalUserIdResponse_ShouldDeserialize_WithPendingId()
    {
        var json = """
        { "errcode": 0, "errmsg": "ok", "external_userid": "ooAAAAAAAAAAA", "pending_id": "ooBBBBBB" }
        """;
        var resp = JsonSerializer.Deserialize(
            json, AccountIdJsonContext.Default.ConvertUnionidToExternalUserIdResponse);

        resp!.IsSuccess.Should().BeTrue();
        resp.ExternalUserId.Should().Be("ooAAAAAAAAAAA");
        resp.PendingId.Should().Be("ooBBBBBB");
    }

    [Fact]
    public void ConvertUnionidToExternalUserIdRequest_ShouldSerialize_WithMassCallTicket_WhenProvided()
    {
        var json = JsonSerializer.Serialize(
            new ConvertUnionidToExternalUserIdRequest
            {
                Unionid = "oAAAAAAA",
                OpenId = "oBBBB",
                SubjectType = 1,
                MassCallTicket = "TICKET",
            },
            AccountIdJsonContext.Default.ConvertUnionidToExternalUserIdRequest);

        json.Should().Contain("\"unionid\":\"oAAAAAAA\"");
        json.Should().Contain("\"openid\":\"oBBBB\"");
        json.Should().Contain("\"subject_type\":1");
        json.Should().Contain("\"mass_call_ticket\":\"TICKET\"");
    }

    [Fact]
    public void ConvertChatIdResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "items": [ { "chat_id": "xxxxxx", "new_chat_id": "XXXXXX" } ],
          "invalid_chat_id_list": ["zzzzzz"]
        }
        """;
        var resp = JsonSerializer.Deserialize(
            json, AccountIdJsonContext.Default.ConvertChatIdResponse);

        resp!.IsSuccess.Should().BeTrue();
        resp.Items.Should().ContainSingle();
        resp.Items![0].ChatId.Should().Be("xxxxxx");
        resp.Items[0].NewChatId.Should().Be("XXXXXX");
        resp.InvalidChatIdList.Should().BeEquivalentTo(new[] { "zzzzzz" });
    }
}
