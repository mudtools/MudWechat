// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.OneCode;

namespace Mud.Wechat.OfficialAccount.Tests.OneCode;

/// <summary>
/// 微信「一物一码」域 DTO 与官方报文的双向映射（官方页示例夹具）。
/// </summary>
public class MpOneCodeSerializationTests
{
    /// <summary>OT-S1：申请二维码请求序列化（code_count 数值 + isv_application_id 幂等键）。</summary>
    [Fact]
    public void ApplyCodeRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpApplyCodeRequest { CodeCount = 10000, IsvApplicationId = "testid124" };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, OneCodeJsonContext.Default.MpApplyCodeRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "code_count", "isv_application_id" });
        document.RootElement.GetProperty("code_count").GetInt32().Should().Be(10000);
        document.RootElement.GetProperty("isv_application_id").GetString().Should().Be("testid124");
    }

    /// <summary>OT-S2：申请二维码响应解析（平级 application_id，无 data 包裹）。</summary>
    [Fact]
    public void ApplyCodeResponse_ShouldParseFlatApplicationId()
    {
        const string json = """{"errcode":0,"errmsg":"ok","application_id":581865877}""";

        var response = JsonSerializer.Deserialize(json, OneCodeJsonContext.Default.MpApplyCodeResponse);

        response!.IsSuccess.Should().BeTrue();
        response.ApplicationId.Should().Be(581865877);
    }

    /// <summary>OT-S3：查询申请单响应解析（status = FINISH + 码段列表）。</summary>
    [Fact]
    public void CodeApplyQueryResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","status":"FINISH","application_id":581865877,
            "isv_application_id":"testid124","code_generate_list":[{"code_start":0,"code_end":9999}],
            "create_time":1611047541,"update_time":1611047600}
            """;

        var response = JsonSerializer.Deserialize(json, OneCodeJsonContext.Default.MpCodeApplyQueryResponse);

        response!.Status.Should().Be(MpCodeApplyStatuses.Finished, "官方唯一示例状态值，用作下载前置判定");
        response.ApplicationId.Should().Be(581865877);
        response.CodeGenerateList.Should().HaveCount(1);
        response.CodeGenerateList![0].CodeStart.Should().Be(0);
        response.CodeGenerateList[0].CodeEnd.Should().Be(9999);
        response.CreateTime.Should().Be(1611047541);
        response.UpdateTime.Should().Be(1611047600);
    }

    /// <summary>OT-S4：查询申请单请求序列化（两路径可空；SDK 不本地校验二选一）。</summary>
    [Fact]
    public void CodeApplyQueryRequest_ShouldSerializeEitherPath()
    {
        using var byApplicationId = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpApplyCodeQueryRequest { ApplicationId = 581865877 },
            OneCodeJsonContext.Default.MpApplyCodeQueryRequest));
        byApplicationId.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "application_id" });
        byApplicationId.RootElement.GetProperty("application_id").GetInt64().Should().Be(581865877,
            "请求侧取字段表口径（官方请求示例写作字符串，不构成口径依据）");

        using var byIsvId = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpApplyCodeQueryRequest { IsvApplicationId = "testid124" },
            OneCodeJsonContext.Default.MpApplyCodeQueryRequest));
        byIsvId.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "isv_application_id" });
    }

    /// <summary>OT-S5：下载二维码包请求 / 响应（buffer 为 JSON 包裹的 base64 字符串）。</summary>
    [Fact]
    public void CodeDownloadRequestAndResponse_ShouldMatchOfficialShape()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpCodeDownloadRequest { ApplicationId = 581865877, CodeStart = 0, CodeEnd = 9999 },
            OneCodeJsonContext.Default.MpCodeDownloadRequest));
        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "application_id", "code_start", "code_end" });

        const string json = """{"errcode":0,"errmsg":"ok","buffer":"QkFTRTY0"}""";
        var response = JsonSerializer.Deserialize(json, OneCodeJsonContext.Default.MpCodeDownloadResponse);

        response!.Buffer.Should().Be("QkFTRTY0", "官方类型列写「formdata 文件 buffer」（非 JSON 类型）⇒ 按返回示例建模为字符串");
    }

    /// <summary>OT-S6：激活二维码请求序列化（wxa_type 缺省时不输出 ⇒ 由官方取默认 0 正式版）。</summary>
    [Fact]
    public void CodeActiveRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpCodeActiveRequest
        {
            ApplicationId = 581865877,
            ActivityName = "活动A",
            ProductBrand = "品牌A",
            ProductTitle = "商品A",
            ProductCode = "6901234567892",
            WxaAppId = "wx1234567890",
            WxaPath = "pages/index/index",
            CodeStart = 0,
            CodeEnd = 9999,
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, OneCodeJsonContext.Default.MpCodeActiveRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[]
            {
                "application_id", "activity_name", "product_brand", "product_title", "product_code",
                "wxa_appid", "wxa_path", "code_start", "code_end",
            }, "wxa_type 为可选项，未设置时不得输出（官方默认 0 正式版）");
    }

    /// <summary>OT-S7：激活二维码指定版本时输出 wxa_type。</summary>
    [Fact]
    public void CodeActiveRequest_ShouldSerializeWxaTypeWhenSet()
    {
        var request = new MpCodeActiveRequest
        {
            ApplicationId = 1,
            ActivityName = "a",
            ProductBrand = "b",
            ProductTitle = "c",
            ProductCode = "d",
            WxaAppId = "wx1",
            WxaPath = "pages/index/index",
            WxaType = MpCodeWxaTypes.Trial,
            CodeStart = 0,
            CodeEnd = 199,
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, OneCodeJsonContext.Default.MpCodeActiveRequest));

        document.RootElement.GetProperty("wxa_type").GetInt32().Should().Be(2);
    }

    /// <summary>OT-S8：查询激活状态请求序列化（两路径；code 为字符串保留前导零）。</summary>
    [Fact]
    public void CodeActiveQueryRequest_ShouldSerializeBothPaths()
    {
        using var byIndex = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpCodeActiveQueryRequest { ApplicationId = 581865877, CodeIndex = 200 },
            OneCodeJsonContext.Default.MpCodeActiveQueryRequest));
        byIndex.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "application_id", "code_index" });

        using var byCode = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpCodeActiveQueryRequest { Code = "012345678" },
            OneCodeJsonContext.Default.MpCodeActiveQueryRequest));
        byCode.RootElement.GetProperty("code").GetString().Should().Be("012345678",
            "九位字符串原始码：前导零必须保留（请求表的 number 标注为误标）");
    }

    /// <summary>OT-S9：查询激活状态响应解析（平级原始码 + 激活信息 + 示例独有的 product_code）。</summary>
    [Fact]
    public void CodeInfoResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","code":"012345678","application_id":581865877,
            "isv_application_id":"testid124","activity_name":"活动A","product_brand":"品牌A",
            "product_title":"商品A","product_code":"test_code","wxa_appid":"wx1234567890",
            "wxa_path":"pages/index/index","wxa_type":0,"code_start":0,"code_end":200}
            """;

        var response = JsonSerializer.Deserialize(json, OneCodeJsonContext.Default.MpCodeInfoResponse);

        response!.Code.Should().Be("012345678");
        response.ActivityName.Should().Be("活动A");
        response.ProductCode.Should().Be("test_code", "示例独有字段（两页字段表均未收录）");
        response.WxaType.Should().Be(MpCodeWxaTypes.Release);
        response.CodeEnd.Should().Be(200, "官方示例值 200 与字段说明举例 9999 不一致（照录）");
    }

    /// <summary>OT-S10：CODE_TICKET 换 CODE 请求 / 响应（与查询激活状态共用响应 DTO）。</summary>
    [Fact]
    public void TicketToCodeRequestAndResponse_ShouldMatchOfficialShape()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpTicketToCodeRequest { OpenId = "oABC123", CodeTicket = "TICKET123" },
            OneCodeJsonContext.Default.MpTicketToCodeRequest));
        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "openid", "code_ticket" });

        const string json = """{"errcode":0,"errmsg":"ok","code":"012345678","wxa_type":1}""";
        var response = JsonSerializer.Deserialize(json, OneCodeJsonContext.Default.MpCodeInfoResponse);

        response!.Code.Should().Be("012345678");
        response.WxaType.Should().Be(MpCodeWxaTypes.Develop,
            "tickettocode 页字段表未列 wxa_type（疑漏），共用超集 DTO 仍可读取");
    }

    /// <summary>OT-S11：通用错误形态（本域错误码面仅通用码 40001）。</summary>
    [Fact]
    public void OneCodeErrors_ShouldParseIntoSharedMpResponse()
    {
        const string json = """{"errcode":40001,"errmsg":"invalid credential access_token isinvalid or not latest"}""";

        var response = JsonSerializer.Deserialize(json, OneCodeJsonContext.Default.MpApplyCodeResponse);

        response!.ErrorCode.Should().Be(MpErrorCodes.InvalidCredential);
        response.IsSuccess.Should().BeFalse();
        response.ApplicationId.Should().BeNull("失败时无业务字段");
    }
}
