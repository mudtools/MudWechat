// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Invoice;

namespace Mud.Wechat.OfficialAccount.Tests.Invoice;

/// <summary>
/// 微信发票域 DTO 与官方报文的双向映射（官方页示例夹具）。
/// </summary>
public class MpInvoiceSerializationTests
{
    /// <summary>IT-S1：<c>setbizattr</c> 请求序列化（按 action 三选一携带字段）。</summary>
    [Fact]
    public void BizAttrRequest_ShouldSerializePerAction()
    {
        var setAuthField = new MpInvoiceBizAttrRequest
        {
            AuthField = new MpInvoiceAuthField
            {
                UserField = new MpInvoiceUserField { ShowTitle = 1, ShowEmail = 1 },
                BizField = new MpInvoiceBizField { ShowTitle = 1, ShowTaxNumber = 1 },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            setAuthField, InvoiceJsonContext.Default.MpInvoiceBizAttrRequest));
        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "auth_field" }, "set_auth_field 只携带 auth_field（其余为 null 不输出）");
        document.RootElement.GetProperty("auth_field").GetProperty("user_field").GetProperty("show_title")
            .GetInt32().Should().Be(1);

        var setContact = new MpInvoiceBizAttrRequest
        {
            Contact = new MpInvoiceContact { TimeOut = 300, Phone = "020-12345678" },
        };
        using var contactDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            setContact, InvoiceJsonContext.Default.MpInvoiceBizAttrRequest));
        contactDoc.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "contact" });
    }

    /// <summary>IT-S2：<c>setbizattr</c> 响应解析（get_auth_field 形态，含 require_* 回填）。</summary>
    [Fact]
    public void BizAttrResponse_ShouldParseGetAuthFieldShape()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","auth_field":{
            "user_field":{"show_title":1,"show_phone":0,"show_email":1,"require_phone":0,"require_email":1},
            "biz_field":{"show_title":1,"show_tax_no":1,"require_tax_no":1}}}
            """;

        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceBizAttrResponse);

        response!.IsSuccess.Should().BeTrue();
        response.AuthField!.UserField!.RequireEmail.Should().Be(1, "require_email 仅 get_auth_field 返回");
        response.AuthField.BizField!.RequireTaxNumber.Should().Be(1);
        response.PayMchInfo.Should().BeNull("get_auth_field 不返回 paymch_info");
    }

    /// <summary>IT-S3：<c>getauthdata</c> 请求 / 响应。</summary>
    [Fact]
    public void AuthDataRequestAndResponse_ShouldMatchOfficialShape()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceAuthDataRequest { OrderId = "1234", SPappId = "wxabcd" },
            InvoiceJsonContext.Default.MpInvoiceAuthDataRequest));
        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "order_id", "s_pappid" });

        const string json = """{"errcode":0,"errmsg":"ok","invoice_status":"INVOICE_REIMBURSE_INIT","auth_time":1474875876}""";
        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceAuthDataResponse);

        response!.InvoiceStatus.Should().Be(MpInvoiceReimburseStatuses.Init);
        response.AuthTime.Should().Be(1474875876);
    }

    /// <summary>IT-S4：<c>getauthurl</c> 请求序列化（八字段，含授权页 ticket 与授权类型）。</summary>
    [Fact]
    public void AuthUrlRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpInvoiceAuthUrlRequest
        {
            SPappId = "wxabcd",
            OrderId = "1234",
            Money = 11,
            Timestamp = 1474875876,
            Source = MpInvoiceAuthSources.Web,
            RedirectUrl = "https://mp.weixin.qq.com",
            Ticket = "tttt",
            Type = MpInvoiceAuthUrlTypes.FillFields,
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, InvoiceJsonContext.Default.MpInvoiceAuthUrlRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[]
            {
                "s_pappid", "order_id", "money", "timestamp", "source", "redirect_url", "ticket", "type",
            });
        document.RootElement.GetProperty("type").GetInt32().Should().Be(1, "支付后开票业务只能 type=1");

        const string json = """{"errcode":0,"errmsg":"ok","auth_url":"https://mp.weixin.qq.com/x","appid":"wx123"}""";
        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceAuthUrlResponse);
        response!.AuthUrl.Should().Be("https://mp.weixin.qq.com/x");
        response.AppId.Should().Be("wx123", "仅 source=wxa 时才有");
    }

    /// <summary>IT-S5：<c>rejectinsert</c> 请求序列化（url 可选）。</summary>
    [Fact]
    public void RejectInsertRequest_ShouldSerializeToOfficialShape()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceRejectInsertRequest { SPappId = "wxabcd", OrderId = "1234", Reason = "重复开票" },
            InvoiceJsonContext.Default.MpInvoiceRejectInsertRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "s_pappid", "order_id", "reason" }, "url 为可选项，未设置时不输出");
    }

    /// <summary>IT-S6：<c>seturl</c> 响应解析（s_pappid 须从 invoice_url 解析）。</summary>
    [Fact]
    public void SetUrlResponse_ShouldParseInvoiceUrl()
    {
        const string json = """{"errcode":0,"errmsg":"ok","invoice_url":"https://mp.weixin.qq.com/bizmall/authinvoice?action=list&s_pappid=wxabcd"}""";

        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceSetUrlResponse);

        response!.InvoiceUrl.Should().Contain("s_pappid=wxabcd", "官方未单列 s_pappid 字段 ⇒ 须从 URL 解析");
    }

    /// <summary>IT-S7：<c>platform/setpdf</c> 响应解析（s_media_id 以字符串承载）。</summary>
    [Fact]
    public void SetPdfResponse_ShouldParseStringMediaId()
    {
        const string json = """{"errcode":0,"errmsg":"ok","s_media_id":"3015806758683707"}""";

        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceSetPdfResponse);

        response!.SMediaId.Should().Be("3015806758683707",
            "官方类型标 string 但描述写「64 位整数」（示例仅 16 位）⇒ 以字符串承载不生歧义");
    }

    /// <summary>IT-S8：<c>platform/getpdf</c> 请求 / 响应。</summary>
    [Fact]
    public void GetPdfRequestAndResponse_ShouldMatchOfficialShape()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceGetPdfRequest { SMediaId = "3015806758683707" },
            InvoiceJsonContext.Default.MpInvoiceGetPdfRequest));
        document.RootElement.GetProperty("action").GetString().Should().Be("get_url", "官方要求填 get_url");

        const string json = """{"errcode":0,"errmsg":"ok","pdf_url":"https://x/y.pdf","pdf_url_expire_time":7200}""";
        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceGetPdfResponse);
        response!.PdfUrlExpireTime.Should().Be(7200);
    }

    /// <summary>IT-S9：<c>platform/updatestatus</c> 请求序列化（冲红用 CANCEL）。</summary>
    [Fact]
    public void UpdateStatusRequest_ShouldSerializeCancelForRedFlush()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceUpdateStatusRequest
            {
                CardId = "pjZ8Yt5crPbAouhFqFf6JFgZv4Lc",
                Code = "1234567890",
                ReimburseStatus = MpInvoiceReimburseStatuses.Cancel,
            },
            InvoiceJsonContext.Default.MpInvoiceUpdateStatusRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "card_id", "code", "reimburse_status" });
        document.RootElement.GetProperty("reimburse_status").GetString()
            .Should().Be("INVOICE_REIMBURSE_CANCEL", "官方原文：电子发票冲红置为该值（表现为卡券被核销）");
    }

    /// <summary>IT-S10：<c>platform/createcard</c> 请求 / 响应。</summary>
    [Fact]
    public void CreateCardRequestAndResponse_ShouldMatchOfficialShape()
    {
        var request = new MpInvoiceCreateCardRequest
        {
            InvoiceInfo = new MpInvoiceTemplateInfo
            {
                Payee = "测试-收款方",
                Type = "广东省增值税普通发票",
                BaseInfo = new MpInvoiceCardBaseInfo
                {
                    LogoUrl = "https://x/logo.png",
                    Title = "收款方",
                    CustomUrlName = "入口",
                    CustomUrl = "https://x/entry",
                },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, InvoiceJsonContext.Default.MpInvoiceCreateCardRequest));
        var info = document.RootElement.GetProperty("invoice_info");
        info.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "base_info", "payee", "type" });
        info.GetProperty("base_info").EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "logo_url", "title", "custom_url_name", "custom_url" },
                "营销入口三字段未设置时不输出");

        const string json = """{"errcode":0,"errmsg":"ok","card_id":"pjZ8Yt5crPbAouhFqFf6JFgZv4Lc"}""";
        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceCreateCardResponse);
        response!.CardId.Should().Be("pjZ8Yt5crPbAouhFqFf6JFgZv4Lc");
    }

    /// <summary>IT-S11：<c>insert</c> 请求序列化（三层嵌套 + 商品明细）与响应。</summary>
    [Fact]
    public void InsertRequestAndResponse_ShouldMatchOfficialShape()
    {
        var request = new MpInvoiceInsertRequest
        {
            OrderId = "1234",
            CardId = "pjZ8Yt5crPbAouhFqFf6JFgZv4Lc",
            AppId = "wxabcd",
            CardExt = new MpInvoiceCardExt
            {
                NonceStr = "abcd",
                UserCard = new MpInvoiceUserCard
                {
                    InvoiceUserData = new MpInvoiceUserData
                    {
                        Fee = 358,
                        FeeWithoutTax = 300,
                        Tax = 58,
                        Title = "某某公司",
                        BillingTime = 1474875876,
                        BillingNumber = "12345678",
                        SPdfMediaId = "3015806758683707",
                        Items = new List<MpInvoiceItem>
                        {
                            new MpInvoiceItem { Name = "商品", Number = 1, Unit = "个", Price = 300 },
                        },
                    },
                },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, InvoiceJsonContext.Default.MpInvoiceInsertRequest));
        var userData = document.RootElement.GetProperty("card_ext").GetProperty("user_card")
            .GetProperty("invoice_user_data");
        userData.GetProperty("fee").GetInt32().Should().Be(358);
        userData.GetProperty("s_pdf_media_id").GetString().Should().Be("3015806758683707");
        userData.GetProperty("info").EnumerateArray().Should().HaveCount(1);
        userData.EnumerateObject().Select(p => p.Name)
            .Should().NotContain("pdf_url", "pdf_url 属报销查询方向字段，插卡时不输出（超集共用）");

        const string json = """{"errcode":0,"errmsg":"ok","code":"1234567890","openid":"oABC","unionid":"uABC"}""";
        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceInsertResponse);
        response!.Code.Should().Be("1234567890");
        response.UnionId.Should().Be("uABC");
    }

    /// <summary>IT-S12：<c>reimburse/getinvoiceinfo</c> 请求（官方栏标「无」但示例有）与响应。</summary>
    [Fact]
    public void ReimburseQueryRequestAndResponse_ShouldMatchOfficialShape()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceReimburseQueryRequest { CardId = "pjZ8", EncryptCode = "fbdt/fWy1V" },
            InvoiceJsonContext.Default.MpInvoiceReimburseQueryRequest));
        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "card_id", "encrypt_code" });

        const string json = """
            {"errcode":0,"errmsg":"ok","card_id":"pjZ8","begin_time":1,"end_time":2,"openid":"oABC",
            "type":"广东省增值税普通发票","payee":"某某公司","detail":"detail",
            "user_info":{"fee":358,"title":"某某公司","billing_no":"12345678","billing_code":"",
            "fee_without_tax":300,"tax":58,"pdf_url":"https://x/y.pdf","trip_pdf_ur":"https://x/z.pdf",
            "reimburse_status":"INVOICE_REIMBURSE_INIT",
            "info":[{"name":"商品","num":1,"unit":"个","price":300}]}}
            """;

        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceReimburseInfo);

        response!.CardId.Should().Be("pjZ8");
        response.UserInfo!.Fee.Should().Be(358);
        response.UserInfo.TripPdfUrl.Should().Be("https://x/z.pdf", "官方字段原文 trip_pdf_ur（疑笔误，照录）");
        response.UserInfo.PdfUrl.Should().Be("https://x/y.pdf");
        response.UserInfo.ReimburseStatus.Should().Be(MpInvoiceReimburseStatuses.Init);
        response.UserInfo.Items!.Should().HaveCount(1);
        response.UserInfo.SPdfMediaId.Should().BeNull("报销查询方向不返回 media_id（超集共用）");
    }

    /// <summary>IT-S13：<c>reimburse/getinvoicebatch</c> 条目复用单张响应类型。</summary>
    [Fact]
    public void ReimburseBatchGetResponse_ShouldParseItems()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","item_list":[
            {"card_id":"pjZ8","begin_time":1,"end_time":2,"openid":"oABC","type":"普票","payee":"公司",
             "detail":"d","user_info":{"fee":358,"reimburse_status":"INVOICE_REIMBURSE_LOCK"}}]}
            """;

        var response = JsonSerializer.Deserialize(json, InvoiceJsonContext.Default.MpInvoiceReimburseBatchGetResponse);

        response!.ItemList.Should().HaveCount(1);
        var item = response.ItemList![0];
        item.CardId.Should().Be("pjZ8", "条目为同一 MpInvoiceReimburseInfo 类型（errcode 缺省 0）");
        item.IsSuccess.Should().BeTrue();
        item.UserInfo!.ReimburseStatus.Should().Be(MpInvoiceReimburseStatuses.Lock);
    }

    /// <summary>IT-S14：<c>reimburse/updatestatusbatch</c> 请求序列化。</summary>
    [Fact]
    public void ReimburseBatchUpdateRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpInvoiceReimburseBatchUpdateRequest
        {
            OpenId = "oABC",
            ReimburseStatus = MpInvoiceReimburseStatuses.Lock,
            InvoiceList = new List<MpInvoiceRef>
            {
                new MpInvoiceRef { CardId = "cardid_1", EncryptCode = "encrypt_code_1" },
                new MpInvoiceRef { CardId = "cardid_2", EncryptCode = "encrypt_code_2" },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, InvoiceJsonContext.Default.MpInvoiceReimburseBatchUpdateRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "openid", "reimburse_status", "invoice_list" });
        var list = document.RootElement.GetProperty("invoice_list");
        list.EnumerateArray().Should().HaveCount(2);
        list[0].EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "card_id", "encrypt_code" });
    }

    /// <summary>IT-S15：极速开发票三端点的请求 / 响应。</summary>
    [Fact]
    public void FastInvoiceEndpoints_ShouldMatchOfficialShape()
    {
        // 录入抬头到用户微信
        using var userTitle = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceUserTitleUrlRequest
            {
                Title = "某某公司", TaxNumber = "1286715052", UserFill = 0, OutTitleId = "abc",
            },
            InvoiceJsonContext.Default.MpInvoiceUserTitleUrlRequest));
        userTitle.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "title", "tax_no", "user_fill", "out_title_id" },
                "phone / addr / bank_type / bank_no 未设置时不输出");

        const string userTitleJson = """{"url":"https://xxx","errcode":0,"errmsg":"ok"}""";
        var userTitleResp = JsonSerializer.Deserialize(userTitleJson, InvoiceJsonContext.Default.MpInvoiceUserTitleUrlResponse);
        userTitleResp!.Url.Should().Be("https://xxx");

        // 获取商户专属抬头链接
        using var selectTitle = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceSelectTitleUrlRequest { Attach = "attach", BizName = "商户名" },
            InvoiceJsonContext.Default.MpInvoiceSelectTitleUrlRequest));
        selectTitle.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "attach", "biz_name" });

        const string selectTitleJson = """{"errcode":0,"errmsg":"ok","url":"https://yyy"}""";
        var selectResp = JsonSerializer.Deserialize(selectTitleJson, InvoiceJsonContext.Default.MpInvoiceSelectTitleUrlResponse);
        selectResp!.Url.Should().Be("https://yyy");

        // 解析扫描的抬头二维码
        using var scan = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpInvoiceScanTitleRequest { ScanText = "xxx_scan_data_xxx" },
            InvoiceJsonContext.Default.MpInvoiceScanTitleRequest));
        scan.RootElement.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(new[] { "scan_text" });

        const string scanJson = """
            {"errcode":0,"errmsg":"ok","title_type":0,"title":"某某公司","phone":"020-12345678",
            "tax_no":"1286715052","addr":"地址","bank_type":"工商银行","bank_no":"622202"}
            """;
        var scanResp = JsonSerializer.Deserialize(scanJson, InvoiceJsonContext.Default.MpInvoiceScanTitleResponse);
        scanResp!.TitleType.Should().Be(MpInvoiceTitleTypes.Business);
        scanResp.TaxNumber.Should().Be("1286715052");
        scanResp.BankNumber.Should().Be("622202");
    }

    /// <summary>IT-S16：族级共用错误码形态解析（抽样：72023 / 40078 / 73016）。</summary>
    [Fact]
    public void InvoiceErrors_ShouldParseIntoSharedMpResponse()
    {
        foreach (var (code, name) in new[]
                 {
                     (40078, nameof(MpErrorCodes.InvoiceCardStatusInvalid)),
                     (72023, nameof(MpErrorCodes.InvoiceLockedByOthers)),
                     (73016, nameof(MpErrorCodes.InvoiceNsrsbhEmpty)),
                 })
        {
            var json = "{\"errcode\":" + code + ",\"errmsg\":\"error\"}";
            var response = JsonSerializer.Deserialize(json, CommonJsonContext.Default.MpResponse);

            response!.ErrorCode.Should().Be(code, $"{name} 取值锁定");
            response.IsSuccess.Should().BeFalse();
        }
    }
}
