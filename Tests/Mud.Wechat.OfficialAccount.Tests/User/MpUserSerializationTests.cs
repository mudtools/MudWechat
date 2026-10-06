// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.User;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.User;

/// <summary>
/// 用户信息域 DTO 与官方报文的双向映射（<c>user/info</c> 用官方<b>原文示例</b>做夹具）。
/// </summary>
public class MpUserSerializationTests
{
    /// <summary>
    /// U1：<c>user/info</c> 官方正常返回示例解析（原文示例逐字段照抄）。
    /// </summary>
    [Fact]
    public void GetUserInfoResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"subscribe":1,"openid":"o6_bmjrPTlm6_2sgVt7hMZOPfL2M","language":"zh_CN",
             "subscribe_time":1382694957,"unionid":"o6_bmasdasdsad6_2sgVt7hMZOPfL","remark":"",
             "groupid":0,"tagid_list":[128,2],"subscribe_scene":"ADD_SCENE_QR_CODE",
             "qr_scene":98765,"qr_scene_str":""}
            """;

        var response = JsonSerializer.Deserialize(json, UserJsonContext.Default.MpUserInfoResponse);

        response.Should().NotBeNull();
        response!.IsSuccess.Should().BeTrue("成功响应不带 errcode");
        response.Subscribe.Should().Be(1);
        response.OpenId.Should().Be("o6_bmjrPTlm6_2sgVt7hMZOPfL2M");
        response.Language.Should().Be("zh_CN");
        response.SubscribeTime.Should().Be(1382694957);
        response.UnionId.Should().Be("o6_bmasdasdsad6_2sgVt7hMZOPfL");
        response.GroupId.Should().Be(0);
        response.TagIdList.Should().BeEquivalentTo(new[] { 128, 2 });
        response.SubscribeScene.Should().Be("ADD_SCENE_QR_CODE");
        response.QrScene.Should().Be(98765);
    }

    /// <summary>U2：<c>user/info</c> 官方错误示例解析（<c>{"errcode":40013,"errmsg":"invalid appid"}</c>）。</summary>
    [Fact]
    public void GetUserInfoResponse_ShouldParseOfficialErrorSample()
    {
        const string json = """{"errcode":40013,"errmsg":"invalid appid"}""";

        var response = JsonSerializer.Deserialize(json, UserJsonContext.Default.MpUserInfoResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(MpErrorCodes.InvalidAppId);
        response.ErrorMessage.Should().Be("invalid appid");
    }

    /// <summary>
    /// U3：<c>subscribe = 0</c>（未关注）时官方「拉取不到其余信息」——载荷缺省也必须能解析。
    /// </summary>
    [Fact]
    public void GetUserInfoResponse_ShouldTolerateUnsubscribedPayload()
    {
        const string json = """{"subscribe":0,"openid":"o6_unsubscribed"}""";

        var response = JsonSerializer.Deserialize(json, UserJsonContext.Default.MpUserInfoResponse);

        response!.Subscribe.Should().Be(0);
        response.UnionId.Should().BeNull("未关注时拉取不到其余信息");
        response.TagIdList.Should().BeNull();
        response.SubscribeScene.Should().BeNull();
        response.IsSuccess.Should().BeTrue("未关注不是接口错误（errcode 仍为 0）");
    }

    /// <summary>U4：批量查询响应解析（官方返回示例说明：列表内可能同时含已关注与未关注用户）。</summary>
    [Fact]
    public void BatchGetUserInfoResponse_ShouldParseMixedPayload()
    {
        const string json = """
            {"user_info_list":[
              {"subscribe":1,"openid":"o6_subscribed","subscribe_time":1382694957,"tagid_list":[128,2]},
              {"subscribe":0,"openid":"o6_not_subscribed"}
            ]}
            """;

        var response = JsonSerializer.Deserialize(json, UserJsonContext.Default.MpBatchGetUserInfoResponse);

        response!.UserInfoList.Should().HaveCount(2);
        response.UserInfoList![0].Subscribe.Should().Be(1);
        response.UserInfoList[0].TagIdList.Should().BeEquivalentTo(new[] { 128, 2 });
        response.UserInfoList[1].Subscribe.Should().Be(0);
        response.UserInfoList[1].TagIdList.Should().BeNull("未关注用户拉取不到其余信息");
    }

    /// <summary>U5：批量查询请求体序列化必须为官方形态 <c>{"user_list":[{"openid":…,"lang":…}]}</c>。</summary>
    [Fact]
    public void BatchGetUserInfoRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpBatchGetUserInfoRequest
        {
            UserList = new()
            {
                new MpBatchUserInfoItem { OpenId = "o1", Lang = "zh_CN" },
                new MpBatchUserInfoItem { OpenId = "o2" },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, UserJsonContext.Default.MpBatchGetUserInfoRequest));

        var list = document.RootElement.GetProperty("user_list");
        list.GetArrayLength().Should().Be(2);
        list[0].GetProperty("openid").GetString().Should().Be("o1");
        list[0].GetProperty("lang").GetString().Should().Be("zh_CN");
        list[1].GetProperty("openid").GetString().Should().Be("o2");
    }

    /// <summary>U6：关注者列表响应解析 + 官方「<c>next_openid</c> 为空表示列表结束」的翻页终止条件。</summary>
    [Fact]
    public void GetFansResponse_ShouldParseAndSignalEndOfList()
    {
        const string lastPage = """{"total":2,"count":2,"data":{"openid":["o1","o2"]},"next_openid":""}""";

        var response = JsonSerializer.Deserialize(lastPage, UserJsonContext.Default.MpGetFansResponse);

        response!.Total.Should().Be(2);
        response.Count.Should().Be(2);
        response.Data!.OpenId.Should().BeEquivalentTo(new[] { "o1", "o2" });
        string.IsNullOrEmpty(response.NextOpenId).Should().BeTrue("官方原文：最后一次返回时 next_openid 可能为空表示列表结束");
    }

    /// <summary>U7：黑名单列表响应解析 + 「请求 begin_openid / 响应 next_openid」游标回填语义。</summary>
    [Fact]
    public void GetBlacklistResponse_ShouldParseAndExposeNextCursor()
    {
        const string json = """{"total":3,"count":1,"data":{"openid":["o9"]},"next_openid":"o9"}""";

        var response = JsonSerializer.Deserialize(json, UserJsonContext.Default.MpGetBlacklistResponse);

        response!.Count.Should().Be(1);
        response.Data!.OpenId.Should().BeEquivalentTo(new[] { "o9" });
        response.NextOpenId.Should().Be("o9", "下一页须把它回填为请求的 begin_openid（字段名不同）");

        using var requestDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpGetBlacklistRequest { BeginOpenId = response.NextOpenId },
            UserJsonContext.Default.MpGetBlacklistRequest));
        requestDoc.RootElement.GetProperty("begin_openid").GetString().Should().Be("o9");
    }

    /// <summary>U8：设置备注名请求体序列化（<c>openid</c> + <c>remark</c>）。</summary>
    [Fact]
    public void UpdateRemarkRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpUpdateRemarkRequest { OpenId = "o1", Remark = "vip" };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, UserJsonContext.Default.MpUpdateRemarkRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "openid", "remark" }, "字段名照抄官方");
    }

    /// <summary>U9：拉黑 / 取消拉黑请求体序列化（共用 DTO，官方字段 <c>openid_list</c>）。</summary>
    [Fact]
    public void BlacklistRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpBlacklistRequest { OpenIdList = new() { "o1", "o2" } };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, UserJsonContext.Default.MpBlacklistRequest));

        document.RootElement.GetProperty("openid_list").EnumerateArray()
            .Select(e => e.GetString()).Should().BeEquivalentTo(new[] { "o1", "o2" });
    }

    /// <summary>U10：<c>AddUserApi</c> 注册后 <see cref="IMpUserService"/> 可从 DI 解析。</summary>
    [Fact]
    public void AddUserApi_ShouldRegisterUserClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(config =>
        {
            config.AppKey = "mp-user";
            config.AppId = "wx-user";
            config.AppSecret = "s-user";
        });
        services.AddMpServices(builder => builder.AddUserApi());

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IMpUserService>().Should().NotBeNull(
            "AddUserApi 必须完成 AddUserWebApiHttpClient() 的注册");
    }
}
