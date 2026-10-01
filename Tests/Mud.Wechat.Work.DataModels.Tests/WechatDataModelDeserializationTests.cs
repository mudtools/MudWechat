// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Contacts.Batch;
using Mud.Wechat.Work.DataModels.Contacts.ContactRules;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.DataModels.Contacts.Export;
using Mud.Wechat.Work.DataModels.Contacts.Tags;
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.DataModels.CorpGroup;
using Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;
using Mud.Wechat.Work.DataModels.CorpGroup.Rules;
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
    public void GetCustomizedAuthUrlResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","qrcode_url":"https://open.work.weixin.qq.com/3rdapp/qr/xxx","expires_in":864000}""";
        var resp = JsonSerializer.Deserialize<GetCustomizedAuthUrlResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.QrcodeUrl.Should().Be("https://open.work.weixin.qq.com/3rdapp/qr/xxx");
        resp.ExpiresIn.Should().Be(864000);
    }

    [Fact]
    public void GetCustomizedAuthUrlResponse_ShouldDeserialize_WithErrorCode()
    {
        var json = """{"errcode":40001,"errmsg":"invalid credential"}""";
        var resp = JsonSerializer.Deserialize<GetCustomizedAuthUrlResponse>(json);

        resp!.IsSuccess.Should().BeFalse();
        resp.ErrorCode.Should().Be(40001);
        resp.QrcodeUrl.Should().BeNull("可选字段缺省");
        resp.ExpiresIn.Should().Be(0);
    }

    [Fact]
    public void GetCustomizedAuthUrlRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var json = JsonSerializer.Serialize(new GetCustomizedAuthUrlRequest
        {
            State = "abc123",
            TemplateIdList = new() { "dk1", "dk2" },
        });

        json.Should().Contain("\"state\":\"abc123\"");
        json.Should().Contain("\"templateid_list\":[\"dk1\",\"dk2\"]");
    }

    [Fact]
    public void GetCustomizedAuthUrlRequest_ShouldOmit_WhenStateIsNull_ViaJsonContext()
    {
        var json = JsonSerializer.Serialize(
            new GetCustomizedAuthUrlRequest { TemplateIdList = new() { "dk1" } },
            ProviderAuthenticationJsonContext.Default.GetCustomizedAuthUrlRequest);

        json.Should().NotContain("\"state\"", "state 为可选字段，null 时不落 JSON（JsonContext 的 WhenWritingNull）");
        json.Should().Contain("\"templateid_list\":[\"dk1\"]");
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

    [Fact]
    public void UserInfo_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "userid": "zhangsan",
          "name": "张三",
          "department": [1, 2],
          "order": [1, 2],
          "position": "后台工程师",
          "gender": "1",
          "email": "zhangsan@qq.com",
          "is_leader_in_dept": [1, 0],
          "direct_leader": ["lisi"],
          "main_department": 1,
          "extattr": {
            "attrs": [
              { "type": 0, "name": "文本名称", "text": { "value": "文本" } },
              { "type": 1, "name": "网页名称", "web": { "url": "http://www.test.com", "title": "标题" } }
            ]
          },
          "status": 1,
          "external_position": "产品经理",
          "external_profile": {
            "external_corp_name": "企业简称",
            "wechat_channels": { "nickname": "视频号名称", "status": 1 },
            "external_attr": [
              { "type": 2, "name": "测试app", "miniprogram": { "appid": "wx8bd80126147dfa38", "pagepath": "/index", "title": "my miniprogram" } }
            ]
          },
          "open_userid": "woAAAA"
        }
        """;
        var resp = JsonSerializer.Deserialize<UserInfo>(json);

        resp!.ErrorCode.Should().Be(0);
        resp.UserId.Should().Be("zhangsan");
        resp.Department.Should().Equal(new[] { 1, 2 });
        resp.Gender.Should().Be("1", "官方示例 gender 按字符串传输");
        resp.ExtAttr!.Attrs.Should().HaveCount(2);
        resp.ExtAttr.Attrs[0].Text!.Value.Should().Be("文本");
        resp.ExtAttr.Attrs[1].Web!.Url.Should().Be("http://www.test.com");
        resp.ExternalProfile!.WechatChannels!.Nickname.Should().Be("视频号名称");
        resp.ExternalProfile.ExternalAttr![0].MiniProgram!.AppId.Should().Be("wx8bd80126147dfa38");
        resp.OpenUserId.Should().Be("woAAAA");
        resp.Status.Should().Be(1);
        resp.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetUserSimpleListResponse_ShouldDeserialize()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "userlist": [
            { "userid": "zhangsan", "name": "张三", "department": [1, 2], "open_userid": "woAAAA" }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetUserSimpleListResponse>(json);

        resp!.UserList.Should().HaveCount(1);
        resp.UserList![0].UserId.Should().Be("zhangsan");
        resp.UserList[0].OpenUserId.Should().Be("woAAAA");
    }

    [Fact]
    public void ListUserIdsResponse_ShouldDeserialize_WithSelfAndThirdPartyShapes()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "next_cursor": "aaaaaaaaa",
          "dept_user": [
            { "userid": "zhangsan", "department": 1 },
            { "open_userid": "woAAAAAAAA", "department": 2 }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<ListUserIdsResponse>(json);

        resp!.NextCursor.Should().Be("aaaaaaaaa");
        resp.DeptUser.Should().HaveCount(2);
        resp.DeptUser![0].UserId.Should().Be("zhangsan", "自建应用返回 userid");
        resp.DeptUser[1].OpenUserId.Should().Be("woAAAAAAAA", "第三方/代开发返回 open_userid");
        resp.DeptUser[1].Department.Should().Be(2);
    }

    [Fact]
    public void CreateUserResponse_ShouldDeserialize_WithCreatedDepartmentList()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "created",
          "created_department_list": { "department_info": [ { "name": "新部门", "id": 12 } ] }
        }
        """;
        var resp = JsonSerializer.Deserialize<CreateUserResponse>(json);

        resp!.CreatedDepartmentList!.DepartmentInfo.Should().HaveCount(1);
        resp.CreatedDepartmentList.DepartmentInfo![0].Name.Should().Be("新部门");
        resp.CreatedDepartmentList.DepartmentInfo[0].Id.Should().Be(12);
    }

    [Fact]
    public void InviteMembersResponse_ShouldDeserialize_WithInvalidLists()
    {
        var json = """
        { "errcode": 0, "errmsg": "ok", "invaliduser": ["UserID1"], "invalidparty": [1], "invalidtag": [101] }
        """;
        var resp = JsonSerializer.Deserialize<InviteMembersResponse>(json);

        resp!.InvalidUser.Should().Equal(new[] { "UserID1" });
        resp.InvalidParty.Should().Equal(new[] { 1 });
        resp.InvalidTag.Should().Equal(new[] { 101 });
    }

    [Fact]
    public void CheckMemberAuthResponse_ShouldDeserialize()
    {
        var json = """{ "errcode": 0, "errmsg": "ok", "is_member_auth": true }""";
        var resp = JsonSerializer.Deserialize<CheckMemberAuthResponse>(json);

        resp!.IsMemberAuth.Should().BeTrue();
    }

    [Fact]
    public void ListSelectedTicketUserResponse_ShouldDeserialize()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "operator_open_userid": "woOperator",
          "open_userid_list": ["woA", "woB"],
          "unauth_open_userid_list": ["woC"],
          "total": 3
        }
        """;
        var resp = JsonSerializer.Deserialize<ListSelectedTicketUserResponse>(json);

        resp!.OperatorOpenUserId.Should().Be("woOperator");
        resp.OpenUserIdList.Should().Equal(new[] { "woA", "woB" });
        resp.UnauthOpenUserIdList.Should().Equal(new[] { "woC" });
        resp.Total.Should().Be(3);
    }

    [Fact]
    public void CreateUserRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var request = new CreateUserRequest
        {
            UserId = "zhangsan",
            Name = "张三",
            Department = [1, 2],
            ExtAttr = new UserExtAttr
            {
                Attrs = [new UserExtAttrItem { Type = 0, Name = "文本名称", Text = new UserExtAttrText { Value = "文本" } }],
            },
            ExternalProfile = new UserExternalProfile
            {
                ExternalCorpName = "企业简称",
                WechatChannels = new UserWechatChannels { Nickname = "视频号名称" },
            },
        };

        // 生产管线 = 域 JsonContext（WhenWritingNull，Generated/ 源生成）：未赋值的可空属性不应出现在载荷中。
        // 断言只锚定键名与 ASCII 值（源生成默认编码器将非 ASCII 转义为 \uXXXX）。
        var json = JsonSerializer.Serialize(request, UsersJsonContext.Default.CreateUserRequest);
        json.Should().Contain("\"userid\":\"zhangsan\"");
        json.Should().Contain("\"department\":[1,2]");
        json.Should().Contain("\"extattr\":{\"attrs\":[{\"type\":0,");
        json.Should().Contain("\"external_corp_name\":");
        json.Should().Contain("\"wechat_channels\":{\"nickname\":");
        json.Should().Contain("\"external_profile\":{");
        json.Should().NotContain("to_invite", "未赋值的可空属性不应序列化（WhenWritingNull）");
        json.Should().NotContain("\"order\":", "未赋值的可空集合不应序列化为空数组（避免『不填』被改写为『空列表』语义）");
        json.Should().NotContain("\"is_leader_in_dept\":");
    }

    [Fact]
    public void GetDepartmentListResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "department": [
            { "id": 2, "name": "广州研发中心", "name_en": "RDGZ", "department_leader": ["zhangsan", "lisi"], "parentid": 1, "order": 10 },
            { "id": 3, "name": "邮箱产品部", "name_en": "mail", "department_leader": ["lisi"], "parentid": 2, "order": 40 }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetDepartmentListResponse>(json);

        resp!.ErrorCode.Should().Be(0);
        resp.Department.Should().HaveCount(2);
        resp.Department![0].Id.Should().Be(2);
        resp.Department[0].Name.Should().Be("广州研发中心");
        resp.Department[0].DepartmentLeader.Should().Equal(new[] { "zhangsan", "lisi" });
        resp.Department[0].ParentId.Should().Be(1);
        resp.Department[0].Order.Should().Be(10);
        resp.Department[1].ParentId.Should().Be(2);
        resp.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetDepartmentResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "department": { "id": 2, "name": "广州研发中心", "name_en": "RDGZ", "department_leader": ["zhangsan"], "parentid": 1, "order": 10 }
        }
        """;
        var resp = JsonSerializer.Deserialize<GetDepartmentResponse>(json);

        resp!.Department!.Id.Should().Be(2);
        resp.Department.Name.Should().Be("广州研发中心");
        resp.Department.NameEn.Should().Be("RDGZ");
        resp.Department.Order.Should().Be(10);
    }

    [Fact]
    public void GetChildDepartmentIdListResponse_ShouldDeserialize()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "department_id": [
            { "id": 2, "parentid": 1, "order": 10 },
            { "id": 3, "parentid": 2, "order": 40 }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetChildDepartmentIdListResponse>(json);

        resp!.DepartmentIds.Should().HaveCount(2);
        resp.DepartmentIds![0].Id.Should().Be(2);
        resp.DepartmentIds[1].ParentId.Should().Be(2);
    }

    [Fact]
    public void CreateDepartmentResponse_ShouldDeserialize()
    {
        var json = """{ "errcode": 0, "errmsg": "created", "id": 2 }""";
        var resp = JsonSerializer.Deserialize<CreateDepartmentResponse>(json);

        resp!.Id.Should().Be(2);
        resp.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateDepartmentRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var request = new CreateDepartmentRequest { Name = "RDGZ", ParentId = 1 };

        // 生产管线 = 域 JsonContext（WhenWritingNull，Generated/ 源生成）：断言只锚定键名与 ASCII 值。
        var json = JsonSerializer.Serialize(request, DepartmentJsonContext.Default.CreateDepartmentRequest);
        json.Should().Contain("\"name\":\"RDGZ\"");
        json.Should().Contain("\"parentid\":1");
        json.Should().NotContain("name_en", "未赋值的可空属性不应序列化（WhenWritingNull）");
        json.Should().NotContain("\"id\":", "未显式指定部门 id 时不传，由官方自动生成");
        json.Should().NotContain("\"order\":", "未赋值的可空属性不应序列化（WhenWritingNull）");
    }

    [Fact]
    public void GetTagMembersResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "tagname": "测试标签",
          "userlist": [
            { "userid": "zhangsan", "name": "张三" },
            { "userid": "lisi", "name": "李四" }
          ],
          "partylist": [2, 4]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetTagMembersResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.TagName.Should().Be("测试标签");
        resp.UserList.Should().HaveCount(2);
        resp.UserList![0].UserId.Should().Be("zhangsan");
        resp.UserList![0].Name.Should().Be("张三");
        resp.UserList![1].UserId.Should().Be("lisi");
        resp.PartyList.Should().Equal(new[] { 2, 4 });
    }

    [Fact]
    public void GetTagMembersResponse_ShouldTolerate_MissingNameField()
    {
        // 官方停返口径：name 字段分阶段停返（第三方自 2020-06-30 起未授权不返回）。
        var json = """{"errcode":0,"errmsg":"ok","userlist":[{"userid":"zhangsan"}]}""";
        var resp = JsonSerializer.Deserialize<GetTagMembersResponse>(json);

        resp!.TagName.Should().BeNull("字段缺省时不得抛异常");
        resp.UserList![0].Name.Should().BeNull();
        resp.PartyList.Should().BeEmpty("partylist 缺省时退化为空集合");
    }

    [Fact]
    public void ChangeTagMembersResponse_ShouldDeserialize_PartialInvalidWithPipeSeparatedList()
    {
        // 官方契约：invalidlist 是竖线分隔的字符串（不再是数组），invalidparty 仍为数组。
        var json = """{"errcode":0,"errmsg":"ok","invalidlist":"usr1|usr2|usr","invalidparty":[2,4]}""";
        var resp = JsonSerializer.Deserialize<ChangeTagMembersResponse>(json);

        resp!.IsSuccess.Should().BeTrue("部分合法时 errcode 仍为 0");
        resp.InvalidList.Should().Be("usr1|usr2|usr");
        resp.InvalidParty.Should().Equal(new[] { 2, 4 });
    }

    [Fact]
    public void GetTagListResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "taglist": [
            { "tagid": 1, "tagname": "a" },
            { "tagid": 2, "tagname": "b" }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetTagListResponse>(json);

        resp!.TagList.Should().HaveCount(2);
        resp.TagList![0].TagId.Should().Be(1);
        resp.TagList![0].TagName.Should().Be("a");
        resp.TagList![1].TagId.Should().Be(2);
    }

    [Fact]
    public void CreateTagResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"created","tagid":12}""";
        var resp = JsonSerializer.Deserialize<CreateTagResponse>(json);

        resp!.TagId.Should().Be(12);
    }

    [Fact]
    public void CreateTagRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        // 生产管线 = 域 JsonContext（WhenWritingNull，Generated/ 源生成）：断言只锚定键名与 ASCII 值。
        var json = JsonSerializer.Serialize(
            new CreateTagRequest { TagName = "a", TagId = 12 },
            TagsJsonContext.Default.CreateTagRequest);

        json.Should().Contain("\"tagname\":\"a\"");
        json.Should().Contain("\"tagid\":12");
    }

    [Fact]
    public void AddTagMembersRequest_ShouldOmitNullCollections_ViaJsonContext()
    {
        // 官方语义：userlist 与 partylist 不能同时为空；二者均为可选，
        // 序列化不得将「不填」改写为空数组（请求侧集合不设默认值）。
        var json = JsonSerializer.Serialize(
            new AddTagMembersRequest { TagId = 1, UserList = new() { "zhangsan" } },
            TagsJsonContext.Default.AddTagMembersRequest);

        json.Should().Contain("\"tagid\":1");
        json.Should().Contain("\"userlist\":[\"zhangsan\"]");
        json.Should().NotContain("\"partylist\"", "partylist 为可选字段，null 时不落 JSON（WhenWritingNull）");
    }

    [Fact]
    public void GetContactRulesResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "rules": [
            {
              "rule_id": 10001,
              "rule_type": 1,
              "range": { "userid": ["zhangsan"], "partyid": [2], "tagid": [101] },
              "whitelist": { "userid": ["lisi"] },
              "is_allowed_search": true,
              "is_allowed_conversation": false
            },
            {
              "rule_id": 10002,
              "rule_type": 3,
              "range": { "partyid": [1] },
              "exclude": { "partyid": [2, 4] }
            }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetContactRulesResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.Rules.Should().HaveCount(2);

        resp.Rules![0].RuleId.Should().Be(10001);
        resp.Rules[0].RuleType.Should().Be(1);
        resp.Rules[0].Range!.UserIds.Should().Equal(new[] { "zhangsan" });
        resp.Rules[0].Range!.PartyIds.Should().Equal(new[] { 2 });
        resp.Rules[0].Range!.TagIds.Should().Equal(new[] { 101 });
        resp.Rules[0].Whitelist!.UserIds.Should().Equal(new[] { "lisi" });
        resp.Rules[0].Exclude.Should().BeNull();
        resp.Rules[0].IsAllowedSearch.Should().BeTrue();
        resp.Rules[0].IsAllowedConversation.Should().BeFalse();

        resp.Rules[1].RuleType.Should().Be(3);
        resp.Rules[1].Exclude!.PartyIds.Should().Equal(new[] { 2, 4 });
        resp.Rules[1].Whitelist.Should().BeNull();
    }

    [Fact]
    public void CreateContactRulesResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","rule_ids":[10001,10002]}""";
        var resp = JsonSerializer.Deserialize<CreateContactRulesResponse>(json);

        resp!.RuleIds.Should().Equal(new[] { 10001, 10002 });
    }

    [Fact]
    public void CreateContactRulesRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        // 生产管线 = 域 JsonContext（WhenWritingNull，Generated/ 源生成）：断言只锚定键名与 ASCII 值。
        var json = JsonSerializer.Serialize(
            new CreateContactRulesRequest
            {
                Rules =
                [
                    new ContactRule
                    {
                        RuleType = 1,
                        Range = new ContactRuleRange { UserIds = ["zhangsan"], PartyIds = [2] },
                        IsAllowedSearch = true,
                    },
                ],
            },
            ContactRulesJsonContext.Default.CreateContactRulesRequest);

        json.Should().Contain("\"rules\":[{");
        json.Should().Contain("\"rule_type\":1");
        json.Should().Contain("\"range\":{\"userid\":[\"zhangsan\"],\"partyid\":[2]");
        json.Should().Contain("\"is_allowed_search\":true");
        json.Should().NotContain("\"rule_id\"", "创建时规则 ID 由官方生成，null 时不落 JSON（WhenWritingNull）");
        json.Should().NotContain("\"exclude\"", "未赋值的可空范围不应序列化（WhenWritingNull）");
    }

    [Fact]
    public void GetBatchJobResultResponse_ShouldDeserialize_UserTaskResult()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "status": 3,
          "type": "replace_user",
          "total": 2,
          "percentage": 100,
          "result": [
            { "userid": "lisi", "errcode": 0, "errmsg": "ok" },
            { "userid": "zhangsan", "errcode": 40007, "errmsg": "not exist" }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetBatchJobResultResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.Status.Should().Be(3);
        resp.Type.Should().Be("replace_user");
        resp.Total.Should().Be(2);
        resp.Percentage.Should().Be(100);
        resp.Result.Should().HaveCount(2);
        resp.Result![0].UserId.Should().Be("lisi");
        resp.Result[0].ErrCode.Should().Be(0);
        resp.Result[1].UserId.Should().Be("zhangsan");
        resp.Result[1].ErrCode.Should().Be(40007);
        resp.Result[1].ErrMsg.Should().Be("not exist");
        resp.Result[1].PartyId.Should().BeNull("成员任务不含部门字段");
    }

    [Fact]
    public void GetBatchJobResultResponse_ShouldDeserialize_PartyTaskResult()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "status": 3,
          "type": "replace_party",
          "total": 2,
          "percentage": 100,
          "result": [
            { "action": 1, "partyid": 1, "errcode": 0, "errmsg": "ok" },
            { "action": 4, "partyid": 2, "errcode": 0, "errmsg": "ok" }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetBatchJobResultResponse>(json);

        resp!.Type.Should().Be("replace_party");
        resp.Result.Should().HaveCount(2);
        resp.Result![0].Action.Should().Be(1);
        resp.Result[0].PartyId.Should().Be(1);
        resp.Result[1].Action.Should().Be(4);
        resp.Result[1].PartyId.Should().Be(2);
        resp.Result[1].UserId.Should().BeNull("部门任务不含成员字段");
    }

    [Fact]
    public void BatchJobResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","jobid":"job-123456"}""";
        var resp = JsonSerializer.Deserialize<BatchJobResponse>(json);

        resp!.JobId.Should().Be("job-123456");
    }

    [Fact]
    public void BatchImportUsersRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        // 生产管线 = 域 JsonContext（WhenWritingNull，Generated/ 源生成）：断言只锚定键名与 ASCII 值。
        var json = JsonSerializer.Serialize(
            new BatchImportUsersRequest { MediaId = "MEDIA-1", ToInvite = false },
            BatchJsonContext.Default.BatchImportUsersRequest);

        json.Should().Contain("\"media_id\":\"MEDIA-1\"");
        json.Should().Contain("\"to_invite\":false");
        json.Should().NotContain("\"callback\"", "未赋值的回调不应序列化（WhenWritingNull）");
    }

    [Fact]
    public void GetExportResultResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "status": 2,
          "data_list": [
            { "url": "https://download.example/f1", "size": 1024, "md5": "aaa" },
            { "url": "https://download.example/f2", "size": 2048, "md5": "bbb" }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetExportResultResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.Status.Should().Be(2, "官方任务状态：0 未处理 / 1 处理中 / 2 完成 / 3 异常失败");
        resp.DataList.Should().HaveCount(2);
        resp.DataList![0].Url.Should().Be("https://download.example/f1");
        resp.DataList[0].Size.Should().Be(1024);
        resp.DataList[0].Md5.Should().Be("aaa");
        resp.DataList[1].Md5.Should().Be("bbb");
    }

    [Fact]
    public void ExportJobResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","jobid":"jobid_export_1"}""";
        var resp = JsonSerializer.Deserialize<ExportJobResponse>(json);

        resp!.JobId.Should().Be("jobid_export_1");
    }

    [Fact]
    public void ExportRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        // 生产管线 = 域 JsonContext（WhenWritingNull，Generated/ 源生成）：断言只锚定键名与 ASCII 值。
        var json = JsonSerializer.Serialize(
            new ExportRequest { EncodingAesKey = "IJUiXNpvGbODwKEBSEsAeOAPAhkqHqNCF6g19t9wfg2", BlockSize = 1000000 },
            ExportJsonContext.Default.ExportRequest);

        json.Should().Contain("\"encoding_aeskey\":\"IJUiXNpvGbODwKEBSEsAeOAPAhkqHqNCF6g19t9wfg2\"");
        json.Should().Contain("\"block_size\":1000000");
    }

    [Fact]
    public void ExportTagUsersRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var json = JsonSerializer.Serialize(
            new ExportTagUsersRequest { TagId = 1, EncodingAesKey = "KEY-43-CHARS-xxxxxxxxxxxxxxxxxxxxxx" },
            ExportJsonContext.Default.ExportTagUsersRequest);

        json.Should().Contain("\"tagid\":1");
        json.Should().Contain("\"encoding_aeskey\":\"KEY-43-CHARS-xxxxxxxxxxxxxxxxxxxxxx\"");
        json.Should().NotContain("\"block_size\"", "未赋值的 block_size 不应序列化（WhenWritingNull，官方默认 10^6）");
    }

    [Fact]
    public void ListAppShareInfoResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "ending": 1,
          "corp_list": [
            { "corpid": "wwcorpid1", "corp_name": "corp-1", "agentid": 1111 },
            { "corpid": "wwcorpid2", "corp_name": "corp-2", "agentid": 1112 }
          ],
          "next_cursor": "next_cursor_1"
        }
        """;
        var resp = JsonSerializer.Deserialize<ListAppShareInfoResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.Ending.Should().Be(1, "1 表示拉取完毕，0 表示数据没有拉取完");
        resp.CorpList.Should().HaveCount(2);
        resp.CorpList![0].CorpId.Should().Be("wwcorpid1");
        resp.CorpList[0].CorpName.Should().Be("corp-1");
        resp.CorpList[0].AgentId.Should().Be(1111);
        resp.NextCursor.Should().Be("next_cursor_1");
    }

    [Fact]
    public void GetCorpGroupTokenResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","access_token":"downstream-token","expires_in":7200}""";
        var resp = JsonSerializer.Deserialize<GetCorpGroupTokenResponse>(json);

        resp!.AccessToken.Should().Be("downstream-token");
        resp.ExpiresIn.Should().Be(7200);
    }

    [Fact]
    public void TransferMiniProgramSessionResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","userid":"abcdef","session_key":"DGAuy2KVaGcnsUrXk8ERgw=="}""";
        var resp = JsonSerializer.Deserialize<TransferMiniProgramSessionResponse>(json);

        resp!.UserId.Should().Be("abcdef");
        resp.SessionKey.Should().Be("DGAuy2KVaGcnsUrXk8ERgw==");
    }

    [Fact]
    public void UnionidToExternalUserIdResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "external_userid_info": [
            { "corpid": "AAAAA", "external_userid": "BBBB" },
            { "corpid": "CCCCC", "external_userid": "DDDDD" }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<UnionidToExternalUserIdResponse>(json);

        resp!.ExternalUserIdInfo.Should().HaveCount(2);
        resp.ExternalUserIdInfo![0].CorpId.Should().Be("AAAAA");
        resp.ExternalUserIdInfo[0].ExternalUserId.Should().Be("BBBB");
        resp.ExternalUserIdInfo[1].ExternalUserId.Should().Be("DDDDD");
    }

    [Fact]
    public void UnionidToPendingIdResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","pending_id":"PENDINGID"}""";
        var resp = JsonSerializer.Deserialize<UnionidToPendingIdResponse>(json);

        resp!.PendingId.Should().Be("PENDINGID");
    }

    [Fact]
    public void ExternalUserIdToPendingIdResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "result": [
            { "external_userid": "oAAAAAAA", "pending_id": "pAAAAA" },
            { "external_userid": "oBBBBB", "pending_id": "pBBBBB" }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<ExternalUserIdToPendingIdResponse>(json);

        resp!.Result.Should().HaveCount(2);
        resp.Result![0].ExternalUserId.Should().Be("oAAAAAAA");
        resp.Result[0].PendingId.Should().Be("pAAAAA");
        resp.Result[1].PendingId.Should().Be("pBBBBB");
    }

    [Fact]
    public void ExternalUserIdToPendingIdRequest_ShouldOmitNullChatId_ViaJsonContext()
    {
        var json = JsonSerializer.Serialize(
            new ExternalUserIdToPendingIdRequest { ExternalUserIds = new() { "oAAAAAAA" } },
            CorpGroupJsonContext.Default.ExternalUserIdToPendingIdRequest);

        json.Should().Contain("\"external_userid\":[\"oAAAAAAA\"]");
        json.Should().NotContain("\"chat_id\"", "chat_id 为可选字段，null 时不落 JSON（WhenWritingNull）");
    }

    [Fact]
    public void ListAppShareInfoRequest_ShouldOmitOptionalFields_ViaJsonContext()
    {
        var json = JsonSerializer.Serialize(
            new ListAppShareInfoRequest { AgentId = 1111, BusinessType = 1 },
            CorpGroupJsonContext.Default.ListAppShareInfoRequest);

        json.Should().Contain("\"agentid\":1111");
        json.Should().Contain("\"business_type\":1");
        json.Should().NotContain("\"corpid\"", "未赋值的可空字段不应序列化（WhenWritingNull）");
        json.Should().NotContain("\"limit\"", "limit 不填表示拉取全量，null 时不得改写为 0");
        json.Should().NotContain("\"cursor\"", "首次调用不填游标，null 时不落 JSON");
    }

    [Fact]
    public void GetChainCorpInfoListResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "has_more": true,
          "next_cursor": "cursor-2",
          "group_corps": [
            { "groupid": 2, "corpid": "wwxxxx", "corp_name": "corp-name", "custom_id": "c-1", "invite_userid": "zhangsan", "is_joined": 1 },
            { "groupid": 2, "corp_name": "pending-corp", "pending_corpid": "wwyyyy", "is_joined": 0 }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetChainCorpInfoListResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.HasMore.Should().BeTrue();
        resp.NextCursor.Should().Be("cursor-2");
        resp.GroupCorps.Should().HaveCount(2);
        resp.GroupCorps![0].CorpId.Should().Be("wwxxxx");
        resp.GroupCorps[0].CustomId.Should().Be("c-1");
        resp.GroupCorps[0].IsJoined.Should().Be(1, "官方列表示例以 1/0 整型传输 is_joined");
        resp.GroupCorps[1].PendingCorpId.Should().Be("wwyyyy");
        resp.GroupCorps[1].CorpId.Should().BeNull("未加入企业不含 corpid");
    }

    [Fact]
    public void GetChainCorpInfoResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        { "errcode": 0, "errmsg": "ok", "corp_name": "corp-name", "qualification_status": 2, "custom_id": "c-1", "groupid": 1, "is_joined": true }
        """;
        var resp = JsonSerializer.Deserialize<GetChainCorpInfoResponse>(json);

        resp!.CorpName.Should().Be("corp-name");
        resp.QualificationStatus.Should().Be(2, "1 未验证 / 2 已验证 / 3 已认证");
        resp.GroupId.Should().Be(1);
        resp.IsJoined.Should().BeTrue("官方详情示例以布尔传输 is_joined");
    }

    [Fact]
    public void GetChainGroupResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "groups": [
            { "groupid": 2, "group_name": "group-2", "parentid": 1, "order": 1 },
            { "groupid": 3, "group_name": "group-3", "parentid": 2, "order": 4294967295 }
          ]
        }
        """;
        var resp = JsonSerializer.Deserialize<GetChainGroupResponse>(json);

        resp!.Groups.Should().HaveCount(2);
        resp.Groups![0].ParentId.Should().Be(1);
        resp.Groups[1].Order.Should().Be(4294967295, "order 官方范围 [0, 2^32)，以 long 承载");
    }

    [Fact]
    public void GetChainImportResultResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "status": 3,
          "result": {
            "chain_id": "chain-1",
            "import_status": 2,
            "fail_list": [
              {
                "corp_name": "fail-corp",
                "custom_id": "c-9",
                "errcode": 670016,
                "errmsg": "invalid contact identity",
                "contact_info_list": [
                  { "mobile": "13000000001", "errcode": 670016, "errmsg": "invalid contact identity" }
                ]
              }
            ]
          }
        }
        """;
        var resp = JsonSerializer.Deserialize<GetChainImportResultResponse>(json);

        resp!.Status.Should().Be(3, "1 任务开始 / 2 进行中 / 3 已完成");
        resp.Result!.ChainId.Should().Be("chain-1");
        resp.Result.ImportStatus.Should().Be(2, "1 全部成功 / 2 部分成功 / 3 全部失败");
        resp.Result.FailList.Should().HaveCount(1);
        resp.Result.FailList![0].ErrCode.Should().Be(670016);
        resp.Result.FailList[0].ContactInfoList![0].Mobile.Should().Be("13000000001");
    }

    [Fact]
    public void ImportChainContactsRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var json = JsonSerializer.Serialize(
            new ImportChainContactsRequest
            {
                ChainId = "chain-1",
                ContactList =
                [
                    new ChainImportCorpItem
                    {
                        CorpName = "corp-name",
                        CustomId = "c-1",
                        ContactInfoList =
                        [
                            new ChainImportContactItem { Name = "name-1", IdentityType = 2, Mobile = "13000000001" },
                        ],
                    },
                ],
            },
            ChainContactsJsonContext.Default.ImportChainContactsRequest);

        json.Should().Contain("\"chain_id\":\"chain-1\"");
        json.Should().Contain("\"corp_name\":\"corp-name\"");
        json.Should().Contain("\"custom_id\":\"c-1\"");
        json.Should().Contain("\"identity_type\":2");
        json.Should().Contain("\"mobile\":\"13000000001\"");
        json.Should().NotContain("\"user_custom_id\"", "未赋值的可空字段不应序列化（WhenWritingNull）");
        json.Should().NotContain("\"group_path\"", "未赋值的可空字段不应序列化（WhenWritingNull）");
    }

    [Fact]
    public void GetChainListResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        { "errcode": 0, "errmsg": "ok", "chains": [ { "chain_id": "chainid1", "chain_name": "chain-1" } ] }
        """;
        var resp = JsonSerializer.Deserialize<GetChainListResponse>(json);

        resp!.Chains.Should().HaveCount(1);
        resp.Chains![0].ChainId.Should().Be("chainid1");
        resp.Chains[0].ChainName.Should().Be("chain-1");
    }

    [Fact]
    public void GetChainRuleInfoResponse_ShouldDeserialize_WithOfficialJsonContract()
    {
        var json = """
        {
          "errcode": 0,
          "errmsg": "ok",
          "rule_info": {
            "owner_corp_range": { "departmentids": ["departmentid1"], "userids": ["userid1", "userid2"] },
            "member_corp_range": { "groupids": ["groupid1"], "corpids": ["corpid1", "corpid2"] }
          }
        }
        """;
        var resp = JsonSerializer.Deserialize<GetChainRuleInfoResponse>(json);

        resp!.IsSuccess.Should().BeTrue();
        resp.RuleInfo!.OwnerCorpRange!.DepartmentIds.Should().Equal(new[] { "departmentid1" });
        resp.RuleInfo.OwnerCorpRange.UserIds.Should().Equal(new[] { "userid1", "userid2" });
        resp.RuleInfo.MemberCorpRange!.GroupIds.Should().Equal(new[] { "groupid1" });
        resp.RuleInfo.MemberCorpRange.CorpIds.Should().Equal(new[] { "corpid1", "corpid2" });
    }

    [Fact]
    public void ListChainRuleIdsResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","rule_ids":[1,2]}""";
        var resp = JsonSerializer.Deserialize<ListChainRuleIdsResponse>(json);

        resp!.RuleIds.Should().Equal(new[] { 1, 2 });
    }

    [Fact]
    public void AddChainRuleResponse_ShouldDeserialize()
    {
        var json = """{"errcode":0,"errmsg":"ok","rule_id":1}""";
        var resp = JsonSerializer.Deserialize<AddChainRuleResponse>(json);

        resp!.RuleId.Should().Be(1);
    }

    [Fact]
    public void AddChainRuleRequest_ShouldSerialize_WithSnakeCaseJsonKeys()
    {
        var json = JsonSerializer.Serialize(
            new AddChainRuleRequest
            {
                ChainId = "Chxxxxxx",
                RuleInfo = new ChainRuleInfo
                {
                    OwnerCorpRange = new ChainRuleOwnerRange { UserIds = new() { "userid1" } },
                    MemberCorpRange = new ChainRuleMemberRange { CorpIds = new() { "corpid1" } },
                },
            },
            RulesJsonContext.Default.AddChainRuleRequest);

        json.Should().Contain("\"chain_id\":\"Chxxxxxx\"");
        json.Should().Contain("\"rule_info\":{\"owner_corp_range\":{\"userids\":[\"userid1\"]}");
        json.Should().Contain("\"member_corp_range\":{\"corpids\":[\"corpid1\"]}");
        json.Should().NotContain("\"departmentids\"", "未赋值的可空列表不应序列化（WhenWritingNull）");
        json.Should().NotContain("\"groupids\"", "未赋值的可空列表不应序列化（WhenWritingNull）");
    }
}
