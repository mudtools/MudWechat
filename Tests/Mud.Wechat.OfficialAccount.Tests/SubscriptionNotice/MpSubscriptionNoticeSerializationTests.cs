// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.SubscriptionNotice;

/// <summary>
/// 订阅通知域 DTO 与官方报文的双向映射（用官方页给出的<b>原文示例</b>作为夹具）。
/// </summary>
public class MpSubscriptionNoticeSerializationTests
{
    /// <summary>S1：bizsend 请求体序列化（官方示例回放；data 键名原样透传）。</summary>
    [Fact]
    public void SendSubscribeNoticeRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpSendSubscribeNoticeRequest
        {
            ToUser = "OPENID",
            TemplateId = "ngqIpbwh8bUfcSsECmogfXcV14J0tQlEpBO27izEYtY",
            MiniProgram = new Mud.Wechat.OfficialAccount.DataModels.Template.MpTemplateMiniProgram
            {
                AppId = "xiaochengxuappid12345",
                PagePath = "index?foo=bar",
            },
            ClientMsgId = "MSG_000001",
            MiniProgramState = "formal",
            Lang = "zh_CN",
            Data = new Dictionary<string, MpSubscribeNoticeDataValue>
            {
                ["phrase3"] = new MpSubscribeNoticeDataValue { Value = "审核通过" },
                ["name1"] = new MpSubscribeNoticeDataValue { Value = "订阅" },
                ["date2"] = new MpSubscribeNoticeDataValue { Value = "2019-12-25 09:42" },
            },
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, SubscriptionNoticeJsonContext.Default.MpSendSubscribeNoticeRequest));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(
            new[] { "touser", "template_id", "data", "miniprogram_state", "lang", "miniprogram", "client_msg_id" },
            "page 未设置时不序列化（可选字段缺省形态）");
        root.GetProperty("data").GetProperty("phrase3").GetProperty("value").GetString().Should().Be("审核通过");
        root.GetProperty("miniprogram").GetProperty("appid").GetString().Should().Be("xiaochengxuappid12345");
    }

    /// <summary>S2：bizsend 官方成功响应解析（仅 errcode/errmsg）。</summary>
    [Fact]
    public void SendSubscribeNoticeResponse_ShouldParseOfficialSample()
    {
        const string json = """{"errcode":0,"errmsg":"ok"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SubscriptionNoticeJsonContext.Default.MpSendSubscribeNoticeResponse);

        response!.IsSuccess.Should().BeTrue();
    }

    /// <summary>S3：addtemplate 请求体序列化（官方示例 tid "401" + kidList 数字数组）。</summary>
    [Fact]
    public void AddSubscribeTemplateRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpAddSubscribeTemplateRequest
        {
            Tid = "401",
            KidList = new List<int> { 1, 2 },
            SceneDesc = "测试数据",
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, SubscriptionNoticeJsonContext.Default.MpAddSubscribeTemplateRequest));
        var root = document.RootElement;

        root.GetProperty("tid").GetString().Should().Be("401", "官方示例 tid 为字符串形态");
        root.GetProperty("kidList").EnumerateArray().Select(e => e.GetInt32())
            .Should().Equal(new[] { 1, 2 }, "kidList 为数字数组（kid id 形态）");
        root.GetProperty("sceneDesc").GetString().Should().Be("测试数据");
    }

    /// <summary>S4：addtemplate 官方响应解析（priTmplId）。</summary>
    [Fact]
    public void AddSubscribeTemplateResponse_ShouldParseOfficialSample()
    {
        const string json = """{"errmsg":"ok","errcode":0,"priTmplId":"9Aw5ZV1j9xdWTFEkqCpZ7jWySL7aGN6rQom4gXINfJs"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SubscriptionNoticeJsonContext.Default.MpAddSubscribeTemplateResponse);

        response!.IsSuccess.Should().BeTrue();
        response.PriTmplId.Should().Be("9Aw5ZV1j9xdWTFEkqCpZ7jWySL7aGN6rQom4gXINfJs");
    }

    /// <summary>S5：gettemplate 官方返回示例解析（含 keywordEnumValueList 与 type=3 长期订阅）。</summary>
    [Fact]
    public void SubscribeTemplateListResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","data":[
            {"priTmplId":"9Aw5ZV1j9xdWTFEkqCpZ7mIBbSC34khK55OtzUPl0rU","title":"报名结果通知",
            "content":"会议时间:{{date2.DATA}}\n会议地点:{{thing1.DATA}}\n","example":"会议时间:2016年8月8日","type":2},
            {"priTmplId":"cy_DfOZL7lypxHh3ja3DyAUbn1GYQRGwezuy5LBTFME","title":"洗衣机故障提醒",
            "content":"完成时间:{{time1.DATA}}","example":"完成时间:2021年10月21日","type":3,
            "keywordEnumValueList":[{"keywordCode":"enum_string2.DATA","enumValueList":["客厅","餐厅"]}]}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SubscriptionNoticeJsonContext.Default.MpSubscribeTemplateListResponse);

        response!.Data.Should().HaveCount(2);
        response.Data![0].Type.Should().Be(2, "一次性订阅");
        response.Data[1].Type.Should().Be(3, "长期订阅");
        response.Data[1].KeywordEnumValueList.Should().HaveCount(1);
        response.Data[1].KeywordEnumValueList![0].KeywordCode.Should().Be("enum_string2.DATA");
        response.Data[1].KeywordEnumValueList[0].EnumValueList.Should().Contain("客厅");
    }

    /// <summary>S6：deltemplate 请求体序列化（官方形态 {"priTmplId":…}）。</summary>
    [Fact]
    public void DeleteSubscribeTemplateRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpDeleteSubscribeTemplateRequest
        {
            PriTmplId = "wDYzYZVxobJivW9oMpSCpuvACOfJXQIoKUm0PY397Tc",
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, SubscriptionNoticeJsonContext.Default.MpDeleteSubscribeTemplateRequest));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "priTmplId" },
            "官方请求体字段为 priTmplId（逐页核验）");
    }

    /// <summary>S7：getcategory 官方返回示例解析（data 元素仅 id/name）。</summary>
    [Fact]
    public void SubscribeCategoryListResponse_ShouldParseOfficialSample()
    {
        const string json = """{"errcode":0,"errmsg":"ok","data":[{"id":616,"name":"公交"}]}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SubscriptionNoticeJsonContext.Default.MpSubscribeCategoryListResponse);

        response!.Data.Should().HaveCount(1);
        response.Data![0].Id.Should().Be(616);
        response.Data[0].Name.Should().Be("公交");
    }

    /// <summary>S8：getpubtemplatetitles 官方返回示例解析（categoryId 字符串形态照示例）。</summary>
    [Fact]
    public void SubscribePubTemplateTitlesResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","count":55,"data":[{"tid":99,"title":"付款成功通知","type":2,"categoryId":"616"}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SubscriptionNoticeJsonContext.Default.MpSubscribePubTemplateTitlesResponse);

        response!.Count.Should().Be(55);
        response.Data.Should().HaveCount(1);
        response.Data![0].Tid.Should().Be(99, "官方示例 tid 为数字形态");
        response.Data[0].CategoryId.Should().Be("616", "官方示例 categoryId 为字符串形态（与类型表矛盾，照录）");
    }

    /// <summary>S9：getpubtemplatekeywords 官方返回示例解析。</summary>
    [Fact]
    public void SubscribePubTemplateKeywordsResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","data":[{"kid":1,"name":"物品名称","example":"名称","rule":"thing"}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, SubscriptionNoticeJsonContext.Default.MpSubscribePubTemplateKeywordsResponse);

        response!.Data.Should().HaveCount(1);
        response.Data![0].Kid.Should().Be(1);
        response.Data[0].Rule.Should().Be("thing", "rule 即 bizsend data 值的类型前缀来源");
    }
}
