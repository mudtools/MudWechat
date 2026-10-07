// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Template;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Template;

/// <summary>
/// 模板消息域 DTO 与官方报文的双向映射（用官方页给出的<b>原文示例</b>作为夹具）。
/// </summary>
public class MpTemplateSerializationTests
{
    /// <summary>S1：发送模板消息请求体序列化（官方示例原文回放；data 仅 value、无 color）。</summary>
    [Fact]
    public void SendTemplateMessageRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpSendTemplateMessageRequest
        {
            ToUser = "OPENID",
            TemplateId = "ngqIpbwh8bUfcSsECmogfXcV14J0tQlEpBO27izEYtY",
            Url = "http://weixin.qq.com/download",
            MiniProgram = new MpTemplateMiniProgram
            {
                AppId = "wx9183c0f44097c93",
                PagePath = "index?foo=bar",
            },
            ClientMsgId = "MSG_000001",
            Data = new Dictionary<string, MpTemplateDataValue>
            {
                ["time4"] = new MpTemplateDataValue { Value = "2025-11-29 10:00:00~12:00:00" },
                ["thing7"] = new MpTemplateDataValue { Value = "TIT园地" },
            },
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, TemplateJsonContext.Default.MpSendTemplateMessageRequest));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(
            new[] { "touser", "template_id", "url", "miniprogram", "data", "client_msg_id" });
        root.GetProperty("miniprogram").GetProperty("pagepath").GetString().Should().Be("index?foo=bar");
        root.GetProperty("data").GetProperty("thing7").GetProperty("value").GetString().Should().Be("TIT园地");
        root.GetProperty("data").GetProperty("thing7").EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "value" }, "data 值形态官方仅 {value}（无 color）");
    }

    /// <summary>S2：发送模板消息官方成功响应解析（msgid 为 number）。</summary>
    [Fact]
    public void SendTemplateMessageResponse_ShouldParseOfficialSample()
    {
        const string json = """{"errcode":0,"errmsg":"ok","msgid":200228332}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, TemplateJsonContext.Default.MpSendTemplateMessageResponse);

        response!.IsSuccess.Should().BeTrue();
        response.MsgId.Should().Be(200228332);
    }

    /// <summary>S3：参数错误响应解析（官方示例 errcode 47003）。</summary>
    [Fact]
    public void SendTemplateMessageResponse_ShouldParseParamError()
    {
        const string json = """{"errcode":47003,"errmsg":"thing01.DATA is invalid"}""";

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, TemplateJsonContext.Default.MpSendTemplateMessageResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(47003, "参数不符合模板规则为 47003（与 47001 不同码）");
    }

    /// <summary>S4：设置行业请求体序列化（官方示例 {"industry_id1":"1","industry_id2":"4"}）。</summary>
    [Fact]
    public void SetIndustryRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpSetIndustryRequest { IndustryId1 = "1", IndustryId2 = "4" };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, TemplateJsonContext.Default.MpSetIndustryRequest));
        var root = document.RootElement;

        root.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "industry_id1", "industry_id2" });
        root.GetProperty("industry_id1").GetString().Should().Be("1", "官方示例行业编号为字符串形态");
    }

    /// <summary>S5：获取行业信息官方返回示例解析。</summary>
    [Fact]
    public void GetIndustryResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"primary_industry":{"first_class":"运输与仓储","second_class":"快递"},
            "secondary_industry":{"first_class":"IT科技","second_class":"互联网|电子商务"}}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, TemplateJsonContext.Default.MpGetIndustryResponse);

        response!.IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0");
        response.PrimaryIndustry!.FirstClass.Should().Be("运输与仓储");
        response.SecondaryIndustry!.SecondClass.Should().Be("互联网|电子商务");
    }

    /// <summary>S6：选用模板请求 / 响应（官方示例：类目模板纯数字 id + 关键词按顺序）。</summary>
    [Fact]
    public void AddTemplate_ShouldRoundTripOfficialShape()
    {
        var request = new MpAddTemplateRequest
        {
            TemplateIdShort = "47123",
            KeywordNameList = new List<string> { "时间", "地点", "金额" },
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, TemplateJsonContext.Default.MpAddTemplateRequest));
        var root = document.RootElement;
        root.GetProperty("template_id_short").GetString().Should().Be("47123");
        root.GetProperty("keyword_name_list").EnumerateArray().Select(e => e.GetString())
            .Should().Equal(new[] { "时间", "地点", "金额" });

        const string responseJson = """{"errcode":0,"errmsg":"ok","template_id":"Doclyl5uP7Aciu-qZ7mJNPtWkbkYnWBWVja26EGbNyk"}""";
        var response = System.Text.Json.JsonSerializer.Deserialize(
            responseJson, TemplateJsonContext.Default.MpAddTemplateResponse);
        response!.TemplateId.Should().Be("Doclyl5uP7Aciu-qZ7mJNPtWkbkYnWBWVja26EGbNyk");
    }

    /// <summary>S7：获取已选用模板列表官方返回示例解析（deputy_industry 键名锁定）。</summary>
    [Fact]
    public void GetAllPrivateTemplateResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"template_list":[{"template_id":"iPk5sOIt5X_flOVKn5GrTFpncEYTojx6ddbt8WYoV5s",
            "title":"领取奖金提醒","primary_industry":"IT科技","deputy_industry":"互联网|电子商务",
            "content":"{{result.DATA}}","example":"您已提交领奖申请"}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, TemplateJsonContext.Default.MpGetAllPrivateTemplateResponse);

        response!.IsSuccess.Should().BeTrue();
        response.TemplateList.Should().HaveCount(1);
        response.TemplateList![0].DeputyIndustry.Should().Be("互联网|电子商务",
            "列表页二级行业键为 deputy_industry（与行业查询页 secondary_industry 不同，官方原文）");
        response.TemplateList[0].Title.Should().Be("领取奖金提醒");
    }

    /// <summary>S8：查询拦截的模板消息请求 / 响应（官方示例；msginfo 形态矛盾照录为单对象）。</summary>
    [Fact]
    public void QueryBlockTmplMsg_ShouldRoundTripOfficialShape()
    {
        var request = new MpQueryBlockTmplMsgRequest
        {
            TmplMsgId = "f9KbMio42Ks3kzLwPP1aDkl2k98avIFbPtdLJ_yY7QY",
            LargestId = 0,
            Limit = 100,
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, TemplateJsonContext.Default.MpQueryBlockTmplMsgRequest));
        document.RootElement.GetProperty("largest_id").GetInt64().Should().Be(0);

        const string responseJson = """
            {"errcode":0,"errmsg":"ok","msginfo":{"id":"2","tmpl_msg_id":"HFmHVgm58MAURBrS71k41100",
            "title":"测试标题","content":"测试内容","send_timestamp":1788796187,"openid":"oxx6ZuPvZLe1VC"}}
            """;
        var response = System.Text.Json.JsonSerializer.Deserialize(
            responseJson, TemplateJsonContext.Default.MpQueryBlockTmplMsgResponse);
        response!.MsgInfo!.Id.Should().Be("2", "id 按官方字段表 string 形态建模（示例为数字的矛盾已照录）");
        response.MsgInfo.SendTimestamp.Should().Be(1788796187);
    }
}
