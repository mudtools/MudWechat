// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.SmartApi;

namespace Mud.Wechat.OfficialAccount.Tests.SmartApi;

/// <summary>
/// 智能接口域 DTO 与官方报文的双向映射（用官方页给出的<b>原文示例</b>作为夹具）。
/// </summary>
public class MpSmartApiSerializationTests
{
    /// <summary>SM-S1：<c>ocr/idcard</c> 官方正面返回示例解析。</summary>
    [Fact]
    public void IdCardResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","type":"Front","name":"张三","id":"123456789012345678",
            "addr":"广东省广州市XXX","gender":"男","nationality":"汉"}
            """;

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpOcrIdCardResponse);

        response!.IsSuccess.Should().BeTrue();
        response.Type.Should().Be("Front");
        response.Name.Should().Be("张三");
        response.Id.Should().Be("123456789012345678");
        response.Gender.Should().Be("男");
        response.Nationality.Should().Be("汉");
        response.ValidDate.Should().BeNull("正面不返回有效期");
    }

    /// <summary>SM-S2：<c>ocr/bankcard</c> 字段表键 <c>number</c> 与示例键 <c>id</c> 双形态均可解析（官方矛盾照录）。</summary>
    [Fact]
    public void BankCardResponse_ShouldParseBothOfficialKeyForms()
    {
        const string tableForm = """{"number":"622213XXXXXXXXX"}""";
        const string sampleForm = """{"id":"622213XXXXXXXXX"}""";

        var byTable = JsonSerializer.Deserialize(tableForm, SmartApiJsonContext.Default.MpOcrBankCardResponse);
        var bySample = JsonSerializer.Deserialize(sampleForm, SmartApiJsonContext.Default.MpOcrBankCardResponse);

        byTable!.Number.Should().Be("622213XXXXXXXXX");
        byTable.Id.Should().BeNull();
        bySample!.Id.Should().Be("622213XXXXXXXXX");
        bySample.Number.Should().BeNull("两种键形态互不覆盖（超集承载，不做归一化）");
    }

    /// <summary>SM-S3：<c>ocr/driving</c> 官方字段表 + 示例独有字段一并解析。</summary>
    [Fact]
    public void DrivingResponse_ShouldParseFieldTableAndSampleOnlyFields()
    {
        const string json = """
            {"plate_num":"粤A12345","vehicle_type":"小型轿车","owner":"张三","addr":"广东省广州市",
            "use_character":"非营运","model":"BMW7201","vin":"LVSHCAMB9AA000000","engine_num":"123456",
            "register_date":"2010-01-01","issue_date":"2010-01-10","plate_num_b":"粤A12345","record":"号牌",
            "passengers_num":"5","total_quality":"2000","prepare_quality":"1500",
            "overall_size":"4800x1800x1500",
            "card_position_front":{"pos":{"left_top":{"x":1,"y":2},"right_bottom":{"x":3,"y":4}}},
            "img_size":{"w":3120,"h":4208}}
            """;

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpOcrDrivingResponse);

        response!.PlateNumber.Should().Be("粤A12345");
        response.PlateNumberB.Should().Be("粤A12345", "官方 plate_num_b 说明与 plate_num 完全相同（照录）");
        response.PrepareQuality.Should().Be("1500");
        response.OverallSize.Should().Be("4800x1800x1500", "示例独有字段（字段表未收录）");
        response.CardPositionFront!.Position!.LeftTop!.X.Should().Be(1);
        response.CardPositionFront.Position.RightBottom!.Y.Should().Be(4);
        response.ImageSize!.Width.Should().Be(3120);
        response.ImageSize.Height.Should().Be(4208);
    }

    /// <summary>SM-S4：<c>ocr/drivinglicense</c> 官方示例解析（含示例独有的 nationality）。</summary>
    [Fact]
    public void DrivingLicenseResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"id_num":"1234567890","name":"张三","sex":"男","nationality":"汉","address":"广东省广州市",
            "birth_date":"1990-01-01","issue_date":"2010-01-01","car_class":"C1",
            "valid_from":"2010-01-01","valid_to":"2020-01-01","official_seal":"某某公安交警支队"}
            """;

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpOcrDrivingLicenseResponse);

        response!.IdNumber.Should().Be("1234567890");
        response.CarClass.Should().Be("C1");
        response.ValidTo.Should().Be("2020-01-01");
        response.OfficialSeal.Should().Be("某某公安交警支队");
        response.Nationality.Should().Be("汉", "示例独有字段（字段表未收录）");
    }

    /// <summary>SM-S5：<c>ocr/bizlicense</c> 官方示例解析（cert_position / img_size 嵌套）。</summary>
    [Fact]
    public void BizLicenseResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"reg_num":"123456","serial":"1","legal_representative":"张三","enterprise_name":"某某公司",
            "type_of_organization":"有限责任公司","address":"广东省广州市","type_of_enterprise":"有限公司",
            "business_scope":"技术开发","registered_capital":"100万","paid_in_capital":"100万",
            "valid_period":"2010-01-01至2030-01-01","registered_date":"2010-01-01",
            "cert_position":{"left_top":{"x":1,"y":2},"right_top":{"x":3,"y":2},
            "right_bottom":{"x":3,"y":4},"left_bottom":{"x":1,"y":4}},
            "img_size":{"w":800,"h":600}}
            """;

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpOcrBizLicenseResponse);

        response!.EnterpriseName.Should().Be("某某公司");
        response.BusinessScope.Should().Be("技术开发");
        response.ValidPeriod.Should().Be("2010-01-01至2030-01-01");
        response.CertPosition!.LeftBottom!.X.Should().Be(1);
        response.CertPosition.RightTop!.Y.Should().Be(2);
        response.ImageSize!.Width.Should().Be(800);
    }

    /// <summary>SM-S6：<c>ocr/comm</c> 官方示例解析（<c>items[].text</c> 见示例、字段表漏列）。</summary>
    [Fact]
    public void CommResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"items":[{"pos":{"left_top":{"x":1,"y":2},"right_top":{"x":3,"y":2},
            "right_bottom":{"x":3,"y":4},"left_bottom":{"x":1,"y":4}},"text":"腾讯"}],
            "img_size":{"w":100,"h":200}}
            """;

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpOcrCommResponse);

        response!.Items.Should().HaveCount(1);
        response.Items![0].Text.Should().Be("腾讯", "官方字段表漏列 text，示例中存在 ⇒ 超集保留");
        response.Items[0].Position!.LeftTop!.X.Should().Be(1);
        response.ImageSize!.Height.Should().Be(200);
    }

    /// <summary>SM-S7：<c>ocr/menu</c> 官方示例解析（字段表标 object ⇒ 按对象建模）。</summary>
    [Fact]
    public void MenuResponse_ShouldParseOfficialSample()
    {
        const string json = """{"content":{"menu_items":[{"name":"鱼香肉丝","price":38.5}]}}""";

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpOcrMenuResponse);

        response!.Content.Should().NotBeNull();
        response.Content!.MenuItems.Should().HaveCount(1);
        response.Content.MenuItems![0].Name.Should().Be("鱼香肉丝");
        response.Content.MenuItems[0].Price.Should().Be(38.5);
    }

    /// <summary>SM-S8：<c>img/aicrop</c> 官方示例解析（results[] + img_size）。</summary>
    [Fact]
    public void AiCropResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"results":[{"crop_left":0,"crop_top":0,"crop_right":100,"crop_bottom":200},
            {"crop_left":10,"crop_top":10,"crop_right":110,"crop_bottom":210}],
            "img_size":{"w":300,"h":400}}
            """;

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpImageAiCropResponse);

        response!.Results.Should().HaveCount(2, "ratios 最多 5 个 ⇒ 结果最多 5 条");
        response.Results![0].CropRight.Should().Be(100);
        response.Results[1].CropTop.Should().Be(10);
        response.ImageSize!.Width.Should().Be(300);
    }

    /// <summary>SM-S9：<c>img/qrcode</c> 官方示例解析（code_results[] 含坐标）。</summary>
    [Fact]
    public void ImageQrcodeResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"code_results":[{"type_name":"QR_CODE","data":"http://weixin.qq.com/r/xxx",
            "pos":{"left_top":{"x":1,"y":2},"right_top":{"x":3,"y":2},
            "right_bottom":{"x":3,"y":4},"left_bottom":{"x":1,"y":4}}}],
            "img_size":{"w":500,"h":600}}
            """;

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpImageQrcodeResponse);

        response!.CodeResults.Should().HaveCount(1);
        response.CodeResults![0].TypeName.Should().Be("QR_CODE");
        response.CodeResults[0].Data.Should().Be("http://weixin.qq.com/r/xxx");
        response.CodeResults[0].Position!.RightBottom!.Y.Should().Be(4);
    }

    /// <summary>SM-S10：条码 / PDF417 不返回坐标 ⇒ 条目坐标须可空。</summary>
    [Fact]
    public void ImageQrcodeResponse_ShouldTolerateMissingPosition()
    {
        const string json = """{"code_results":[{"type_name":"EAN_13","data":"6901234567892"}]}""";

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpImageQrcodeResponse);

        response!.CodeResults![0].Position.Should().BeNull("官方原文：条码和 PDF417 暂不返回位置坐标");
    }

    /// <summary>SM-S11：语音识别结果解析。</summary>
    [Fact]
    public void VoiceRecoResultResponse_ShouldParseOfficialSample()
    {
        const string json = """{"result":"你好，世界"}""";

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpVoiceRecoResultResponse);

        response!.Result.Should().Be("你好，世界");
        response.IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0");
    }

    /// <summary>SM-S12：微信翻译请求体序列化恒为 <c>{"content": …}</c>（lfrom / lto 走 Query，不得进 body）。</summary>
    [Fact]
    public void TranslateRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpVoiceTranslateRequest { Content = "hello" };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, SmartApiJsonContext.Default.MpVoiceTranslateRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "content" }, "官方 body 仅 content");
        document.RootElement.GetProperty("content").GetString().Should().Be("hello");
    }

    /// <summary>SM-S13：微信翻译响应解析（from_content / to_content）。</summary>
    [Fact]
    public void TranslateResponse_ShouldParseOfficialSample()
    {
        const string json = """{"from_content":"hello","to_content":"你好"}""";

        var response = JsonSerializer.Deserialize(json, SmartApiJsonContext.Default.MpVoiceTranslateResponse);

        response!.FromContent.Should().Be("hello");
        response.ToContent.Should().Be("你好");
    }

    /// <summary>SM-S14：上传语音响应为错误形态时 <see cref="MpResponse"/> 承载 errcode（40010）。</summary>
    [Fact]
    public void VoiceUploadError_ShouldParseIntoSharedMpResponse()
    {
        const string json = """{"errcode":40010,"errmsg":"invalid voice size"}""";

        var response = JsonSerializer.Deserialize(json, CommonJsonContext.Default.MpResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(40010);
    }
}
