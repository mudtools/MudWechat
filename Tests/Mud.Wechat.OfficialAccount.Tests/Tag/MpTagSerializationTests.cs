// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Tag;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Tag;

/// <summary>
/// 标签域 DTO 与官方报文的双向映射（用官方页给出的<b>原文示例</b>作为夹具）。
/// </summary>
/// <remarks>
/// 价值：DTO 字段名正确性无法靠「看起来合理」保证，必须用官方示例报文回放——
/// 这也是 AOT 源生成上下文（<c>TagJsonContext</c>）的真实使用路径。
/// </remarks>
public class MpTagSerializationTests
{
    /// <summary>S1：<c>createTag</c> 官方返回示例解析（官方示例原文：<c>{"tag":{"id":134,"name":"广东"}}</c>）。</summary>
    [Fact]
    public void CreateTagResponse_ShouldParseOfficialSample()
    {
        const string json = """{"tag":{"id":134,"name":"广东"}}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(json, TagJsonContext.Default.MpCreateTagResponse);

        response.Should().NotBeNull();
        response!.IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0");
        response.Tag.Should().NotBeNull();
        response.Tag!.Id.Should().Be(134);
        response.Tag.Name.Should().Be("广东");
        response.Tag.Count.Should().BeNull("createTag 官方不下发 count ⇒ 必须为 null（不得以 0 冒充「无粉丝」）");
    }

    /// <summary>S2：<c>getTags</c> 官方返回示例解析（含 <c>count</c>）。</summary>
    [Fact]
    public void GetTagsResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"tags":[{"id":1,"name":"每天一罐可乐星人","count":0},{"id":2,"name":"星标组","count":0}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(json, TagJsonContext.Default.MpGetTagsResponse);

        response!.Tags.Should().HaveCount(2);
        response.Tags![0].Id.Should().Be(1);
        response.Tags[0].Name.Should().Be("每天一罐可乐星人");
        response.Tags[0].Count.Should().Be(0, "getTags 路径下 count 为 0 表示标签下无粉丝（与 null 语义不同）");
        response.Tags[1].Id.Should().Be(2);
    }

    /// <summary>S3：创建标签请求体序列化必须为官方形态 <c>{"tag":{"name":"…"}}</c>。</summary>
    /// <remarks>
    /// 断言经 <see cref="JsonDocument"/> 做<b>语义</b>比对而非字符串比对：源生成上下文的默认编码器会把
    /// 非 ASCII 转义为 <c>\uXXXX</c>（合法 JSON，服务端可正确解析），字符串断言会因此误红。
    /// </remarks>
    [Fact]
    public void CreateTagRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpCreateTagRequest { Tag = new MpTagName { Name = "广东" } };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, TagJsonContext.Default.MpCreateTagRequest));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "tag" },
            "创建标签请求体官方只有外层 tag 对象");
        root.GetProperty("tag").GetProperty("name").GetString().Should().Be("广东");
        root.GetProperty("tag").TryGetProperty("id", out _).Should().BeFalse("创建标签请求体官方不含 id");
        root.GetProperty("tag").TryGetProperty("count", out _).Should().BeFalse();
    }

    /// <summary>S4：编辑 / 删除标签请求体形态（<c>tag.id</c> 必填）。</summary>
    [Fact]
    public void UpdateAndDeleteRequests_ShouldCarryTagId()
    {
        using var update = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            new MpUpdateTagRequest { Tag = new MpTagIdName { Id = 134, Name = "广东" } },
            TagJsonContext.Default.MpUpdateTagRequest));
        update.RootElement.GetProperty("tag").GetProperty("id").GetInt32().Should().Be(134);
        update.RootElement.GetProperty("tag").GetProperty("name").GetString().Should().Be("广东");

        using var delete = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            new MpDeleteTagRequest { Tag = new MpTagId { Id = 134 } },
            TagJsonContext.Default.MpDeleteTagRequest));
        delete.RootElement.GetProperty("tag").GetProperty("id").GetInt32().Should().Be(134,
            "官方字段表把 tag.id 误标为「非必填」，但语义必填（否则 44002）⇒ SDK 必须真实下发");
    }

    /// <summary>S5：批量打 / 取消标签请求体字段名（官方 <c>openid_list</c> + <c>tagid</c>）。</summary>
    [Fact]
    public void TagMembersRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpTagMembersRequest { OpenIdList = new() { "o1", "o2" }, TagId = 12 };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, TagJsonContext.Default.MpTagMembersRequest));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "openid_list", "tagid" },
            "字段名照抄官方（tagid 无下划线）");
        root.GetProperty("openid_list").EnumerateArray().Select(e => e.GetString())
            .Should().BeEquivalentTo(new[] { "o1", "o2" });
        root.GetProperty("tagid").GetInt32().Should().Be(12);
    }

    /// <summary>S6：<c>45171</c> 部分失败响应携带 <c>fail_openid_list</c>（官方指引用它做定向重试）。</summary>
    [Fact]
    public void TagMembersResponse_ShouldParsePartialFailure()
    {
        const string json = """{"errcode":45171,"errmsg":"some openid fail","fail_openid_list":["o1","o9"]}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(json, TagJsonContext.Default.MpTagMembersResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(MpErrorCodes.SomeOpenIdFailed);
        response.FailOpenIdList.Should().BeEquivalentTo(new[] { "o1", "o9" },
            "部分失败必须可定向重试，否则调用方只能整批重放（会对已成功用户重复打标）");
    }

    /// <summary>S7：获取标签下粉丝列表响应（<c>count</c> / <c>data.openid</c> / <c>next_openid</c>）。</summary>
    [Fact]
    public void GetTagFansResponse_ShouldParsePagedPayload()
    {
        const string json = """
            {"count":2,"data":{"openid":["o1","o2"]},"next_openid":"o2"}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(json, TagJsonContext.Default.MpGetTagFansResponse);

        response!.Count.Should().Be(2);
        response.Data!.OpenId.Should().BeEquivalentTo(new[] { "o1", "o2" });
        response.NextOpenId.Should().Be("o2", "分页游标须回传给下一页请求");
    }

    /// <summary>S8：获取用户标签列表响应（<c>tagid_list</c>）。</summary>
    [Fact]
    public void GetTagIdListResponse_ShouldParseTagIdList()
    {
        const string json = """{"tagid_list":[128,2]}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(json, TagJsonContext.Default.MpGetTagIdListResponse);

        response!.TagIdList.Should().BeEquivalentTo(new[] { 128, 2 });
    }

    /// <summary>S9：失败响应判定（errcode 透传 + <c>IsSuccess</c> 为 false）。</summary>
    [Fact]
    public void FailedResponse_ShouldExposeErrorCode()
    {
        const string json = """{"errcode":45056,"errmsg":"reach max tag limit"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(json, TagJsonContext.Default.MpGetTagsResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(MpErrorCodes.TagCountExceeded);
        response.Tags.Should().BeNull("失败响应不含 tags 字段");
    }

    /// <summary>S10：<c>AddTagApi</c> 注册后 <see cref="IMpTagService"/> 可从 DI 解析（生成器注册链闭环）。</summary>
    [Fact]
    public void AddTagApi_ShouldRegisterTagClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(config =>
        {
            config.AppKey = "mp-tag";
            config.AppId = "wx-tag";
            config.AppSecret = "s-tag";
        });
        services.AddMpServices(builder => builder.AddTagApi());

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IMpTagService>().Should().NotBeNull(
            "AddTagApi 必须完成 AddTagWebApiHttpClient() 的注册（模块注册组契约）");
    }
}
