// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization.Metadata;

namespace Mud.Wechat.Ads.Tests.DataModels.Advertiser;

/// <summary>
/// 客户账号域 DTO 的<b>源生成上下文</b>序列化测试（照官方应答样例反序列化、照官方请求表序列化）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么走 <c>AdvertiserJsonContext.Default</c> 而不是反射重载</b>：反射版在 Native AOT 下没有元数据，
/// 用它测试等于「JIT 下绿、AOT 下炸」（守卫 ADS-B6 拦的就是这种混用）。
/// 本类同时也就成了「生成物是否真的覆盖到闭合应答类型」的运行期证据 —— 生成器漏类型时
/// <c>GetTypeInfo</c> 返回 <c>null</c> 或直接抛，编译期守卫抓不到。
/// </para>
/// <para><b>样例报文</b>取自官方 <c>advertiser/get</c> / <c>advertiser/update_daily_budget</c> 页的应答示例（2026-10-10 逐页核验），
/// 数字字段以官方<b>字段表</b>类型（integer）书写；官方示例里把数字写成占位符字符串的自相矛盾之处
/// 记录在 <see cref="AdsAdvertiserInfo"/> 的 remarks，SDK 不为其放宽类型。</para>
/// </remarks>
public class AdsAdvertiserJsonTests
{
    private static readonly IJsonTypeInfoResolver Resolver =
        JsonTypeInfoResolver.Combine(AdvertiserJsonContext.Default, CommonJsonContext.Default);

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = Resolver,
    };

    /// <summary>官方 <c>advertiser/get</c> 应答样例必须逐字段落到 snake_case 声明的属性上。</summary>
    [Fact]
    public void AdvertiserGetResponse_ShouldBindOfficialSample()
    {
        const string json = """
        {
          "code": 0,
          "message": "",
          "message_cn": "",
          "data": {
            "list": [
              {
                "account_id": 12345678,
                "daily_budget": 1000000,
                "registration_type": "REGISTRATION_TYPE_ENTERPRISE",
                "corporation_name": "示例科技有限公司",
                "corporation_licence": "91110000000000000X",
                "certification_image_id": "2000012345",
                "certification_image": "https://example.com/licence.jpg",
                "individual_qualification": {
                  "name": "张三",
                  "identification_number": "110101199001010000",
                  "identification_front_image_id": "2000012346",
                  "identification_back_image_id": "2000012347"
                },
                "area_code": 110100,
                "mdm_id": 555,
                "mdm_name": "示例主体",
                "system_industry_id": 2010,
                "introduction_url": "https://example.com",
                "corporate_brand_name": "示例品牌",
                "memo": "备注",
                "system_status": "CUSTOMER_STATUS_NORMAL",
                "reject_message": "",
                "is_adx": false,
                "business_alias": "alias-01",
                "contact_person": "李四",
                "contact_person_email": "lisi@example.com",
                "contact_person_telephone": "010-88888888",
                "contact_person_mobile": "+8613900000000",
                "websites": [
                  {
                    "website_domain": "www.example.com",
                    "icp_image_id": "2000012348",
                    "system_status": "WEBSITE_STATUS_DENIED",
                    "reject_message": "备案信息不符"
                  }
                ],
                "agency_account_id": 9999,
                "operators": [
                  {
                    "operator_id": 777,
                    "operator_name": "王五",
                    "qq": 10000,
                    "wechat_account_id": "wx_10000",
                    "is_master": true
                  }
                ]
              }
            ],
            "page_info": { "page": 1, "page_size": 10, "total_number": 1, "total_page": 1 },
            "cursor_page_info": { "page_size": 10, "total_number": 1, "has_more": false, "cursor": 11 }
          }
        }
        """;

        var response = JsonSerializer.Deserialize(json, AdvertiserJsonContext.Default.AdsAdvertiserGetResponse);

        response!.IsSuccess.Should().BeTrue();
        var info = response.Data!.List!.Single();
        info.AccountId.Should().Be(12345678);
        info.DailyBudget.Should().Be(1000000);
        info.RegistrationType.Should().Be("REGISTRATION_TYPE_ENTERPRISE");
        info.CorporationName.Should().Be("示例科技有限公司");
        info.AreaCode.Should().Be(110100);
        info.IsAdx.Should().BeFalse();
        info.SystemStatus.Should().Be("CUSTOMER_STATUS_NORMAL");
        info.IndividualQualification!.IdentificationFrontImageId.Should().Be("2000012346",
            "官方示例比字段表多出两支图片 id，超集建模才装得下官方已返回的数据");
        info.Websites!.Single().WebsiteDomain.Should().Be("www.example.com");
        info.Websites.Single().RejectMessage.Should().Be("备案信息不符");

        // operators.wechat_account_id 官方标 string（与 authorizer_info 的同名 integer 字段形态不同）。
        info.Operators!.Single().WechatAccountId.Should().Be("wx_10000");
        info.Operators.Single().IsMaster.Should().BeTrue();

        response.Data.PageInfo!.TotalPage.Should().Be(1);
        response.Data.CursorPageInfo!.Cursor.Should().Be(11);
        response.Data.CursorPageInfo.HasMore.Should().BeFalse();
    }

    /// <summary>游标模式下官方只回 <c>cursor_page_info</c>；两支分页都建可空 ⇒ 未返回的那支必须为 <c>null</c>。</summary>
    [Fact]
    public void AdvertiserGetResponse_ShouldLeaveUnusedPaginationNull_WhenCursorModeOnly()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{"list":[],"cursor_page_info":{"page_size":10,"total_number":0}}}
        """;

        var response = JsonSerializer.Deserialize(json, AdvertiserJsonContext.Default.AdsAdvertiserGetResponse);

        response!.Data!.PageInfo.Should().BeNull("普通分页元信息在游标模式下不返回，不得被造出默认零值");
        response.Data.CursorPageInfo!.HasMore.Should().BeNull("官方示例里 cursor_page_info 字段集会缺项");
        response.Data.CursorPageInfo.Cursor.Should().BeNull();
    }

    /// <summary>
    /// 请求体序列化必须<b>省略 null 字段</b> —— 官方 <c>advertiser/update</c> 是「不传即不改」的局部更新，
    /// 把未填字段序列化成 <c>null</c> 上送会把它变成一次全量覆盖。
    /// </summary>
    [Fact]
    public void AdvertiserUpdateRequest_ShouldOmitNullFields_WhenSerializingPatch()
    {
        var request = new AdsAdvertiserUpdateRequest { AccountId = 12345678, DailyBudget = 2000000 };

        var json = JsonSerializer.Serialize(request, AdvertiserJsonContext.Default.AdsAdvertiserUpdateRequest);

        json.Should().Be("{\"account_id\":12345678,\"daily_budget\":2000000}");
        json.Should().NotContain("corporation_name");
        json.Should().NotContain("websites");
    }

    /// <summary>空数组与「不传」在官方语义上不同（前者清空推广链接）⇒ 空列表必须被写出，不能被 <c>WhenWritingNull</c> 吃掉。</summary>
    [Fact]
    public void AdvertiserUpdateRequest_ShouldKeepEmptyWebsitesArray()
    {
        var request = new AdsAdvertiserUpdateRequest
        {
            AccountId = 1,
            Websites = new List<AdsAdvertiserWebsite>(),
        };

        var json = JsonSerializer.Serialize(request, AdvertiserJsonContext.Default.AdsAdvertiserUpdateRequest);

        json.Should().Contain("\"websites\":[]");
    }

    /// <summary>批量日预算请求体的根键名照官方原文（<c>update_daily_budget_spec</c>），元素键同为 snake_case。</summary>
    [Fact]
    public void UpdateDailyBudgetRequest_ShouldUseOfficialRootKeyAndItemKeys()
    {
        var request = new AdsAdvertiserUpdateDailyBudgetRequest
        {
            UpdateDailyBudgetSpec = new List<AdsUpdateDailyBudgetSpec>
            {
                new() { AccountId = 11, DailyBudget = 300000, UseMinDailyBudget = true },
                new() { AccountId = 22, DailyBudget = 0 },
            },
        };

        var json = JsonSerializer.Serialize(
            request, AdvertiserJsonContext.Default.AdsAdvertiserUpdateDailyBudgetRequest);

        json.Should().Be(
            "{\"update_daily_budget_spec\":[" +
            "{\"account_id\":11,\"daily_budget\":300000,\"use_min_daily_budget\":true}," +
            "{\"account_id\":22,\"daily_budget\":0}]}");
    }

    /// <summary>
    /// <b>双层失败语义</b>：外层 <c>code == 0</c> 只表示受理成功，逐条成败看 <c>data.list[i].code</c>，
    /// 另有 <c>fail_id_list</c> 汇总 —— 三个判定面必须都能从同一份应答里读到。
    /// </summary>
    [Fact]
    public void UpdateDailyBudgetResponse_ShouldExposePerItemCodeAndFailIdList()
    {
        const string json = """
        {
          "code": 0,
          "message": "",
          "message_cn": "",
          "data": {
            "list": [
              { "code": 0, "message": "", "message_cn": "", "account_id": 11, "daily_budget": 300000, "use_min_daily_budget": false },
              { "code": 12345, "message": "daily budget out of range", "message_cn": "日预算超出允许区间", "account_id": 22, "daily_budget": 0, "use_min_daily_budget": true }
            ],
            "fail_id_list": [22]
          }
        }
        """;

        var response = JsonSerializer.Deserialize(json,
            AdvertiserJsonContext.Default.AdsAdvertiserUpdateDailyBudgetResponse);

        response!.IsSuccess.Should().BeTrue("外层只表达「请求被受理」");
        response.Data!.FailIdList.Should().Equal(new long[] { 22L }, "fail_id_list 是与逐条 code 并列的第二个失败判定面");

        var items = response.Data.List!;
        items.Should().HaveCount(2);
        items[0].Code.Should().Be(0);
        items[1].Code.Should().Be(12345);
        items[1].MessageCn.Should().Be("日预算超出允许区间");

        // 「期望下调、实际上调」的反直觉形态：use_min_daily_budget 为 true 时逐条 daily_budget 才是生效值。
        items[1].UseMinDailyBudget.Should().BeTrue();
        items[1].AccountId.Should().Be(22);
    }

    /// <summary>信封判错契约：<c>code</c> → <see cref="AdsResponse.ErrorCode"/>、<c>message</c> → <see cref="AdsResponse.ErrorMessage"/>，<c>message_cn</c> 单独暴露。</summary>
    [Fact]
    public void AdsResponse_ShouldMapEnvelopeToCommonContract()
    {
        const string json = """
        {"code":12345,"message":"invalid access_token","message_cn":"令牌无效","data":null}
        """;

        var response = JsonSerializer.Deserialize(json, AdvertiserJsonContext.Default.AdsAdvertiserUpdateResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(12345);
        response.ErrorMessage.Should().Be("invalid access_token", "message_cn 不得顶替英文权威支");
        response.MessageCn.Should().Be("令牌无效");
        response.Data.Should().BeNull();
    }

    /// <summary>
    /// 失败信封交给 <c>ThrowIfFailed</c> 必须抛广告线异常类型并带官方 <c>code</c>。
    /// </summary>
    /// <remarks>
    /// <b>守卫路径拿不到中文支</b>：唯一入口是公用契约 <c>IWechatApiResponse</c>（只有 <c>code</c> /
    /// <c>message</c>），故经 <c>ThrowIfFailed</c> 抛出的异常 <see cref="WechatAdsException.MessageCn"/>
    /// 恒为 <c>null</c>。这条不对称在此钉住，避免有人以为异常消息里能稳定拿到中文描述
    /// （中文支的可达路径见 <see cref="WechatAdsException_ShouldCarryBothMessageBranches_WhenConstructedWithCn"/>）。
    /// </remarks>
    [Fact]
    public void ThrowIfFailed_ShouldRaiseWechatAdsException_WhenCodeIsNotZero()
    {
        var response = new AdsAdvertiserUpdateResponse
        {
            Code = 12345,
            Message = "quota exceeded",
            MessageCn = "超出配额",
        };

        var act = () => WechatAdsException.ThrowIfFailed(response, "https://api.e.qq.com/v3.0/advertiser/update");

        var thrown = act.Should().Throw<WechatAdsException>().Which;
        thrown.ErrorCode.Should().Be(12345);
        thrown.Message.Should().Contain("quota exceeded");
        thrown.MessageCn.Should().BeNull();
    }

    /// <summary>宿主直接构造时两支都进消息（英文支恒显示、中文支非空时追加）。</summary>
    [Fact]
    public void WechatAdsException_ShouldCarryBothMessageBranches_WhenConstructedWithCn()
    {
        var exception = new WechatAdsException(12345, "quota exceeded", "超出配额");

        exception.Message.Should().Contain("quota exceeded").And.Contain("超出配额");
        exception.MessageCn.Should().Be("超出配额");
    }

    /// <summary>官方 <c>message_cn</c> 常为空串 ⇒ 空支不产生「message_cn=」这种空段落。</summary>
    [Fact]
    public void WechatAdsException_ShouldOmitBlankCnBranch_WhenCnIsEmpty()
    {
        new WechatAdsException(1, "m", string.Empty).Message.Should().NotContain("message_cn");
    }

    /// <summary>
    /// <b>ADS-B5 的异常面证据</b>：请求地址在构造期剥离 Query ⇒ 承载于 Query 的
    /// <c>access_token</c> / <c>user_token</c> 不会随 <c>RequestUri</c> 进日志或 APM。
    /// </summary>
    [Fact]
    public void WechatAdsException_ShouldStripQueryFromRequestUri()
    {
        var exception = new WechatAdsException(
            11001,
            "invalid token",
            null,
            "https://api.e.qq.com/v3.0/advertiser/get?access_token=secret-token-value&user_token=secret-user-token");

        exception.RequestUri.Should().Be("https://api.e.qq.com/v3.0/advertiser/get");
        exception.RequestUri.Should().NotContain("secret-token-value");
    }

    /// <summary>
    /// 生成上下文必须能解析本域的<b>闭合</b>应答类型（开放泛型 <c>AdsResponse&lt;T&gt;</c> 不登记，见其 remarks）。
    /// </summary>
    [Fact]
    public void AdvertiserJsonContext_ShouldProvideTypeInfoForClosedResponseTypes()
    {
        foreach (var type in new[]
                 {
                     typeof(AdsAdvertiserGetResponse),
                     typeof(AdsAdvertiserUpdateResponse),
                     typeof(AdsAdvertiserUpdateDailyBudgetResponse),
                     typeof(AdsAdvertiserUpdateRequest),
                     typeof(AdsAdvertiserUpdateDailyBudgetRequest),
                 })
        {
            Resolver.GetTypeInfo(type, Options)
                .Should().NotBeNull($"{type.Name} 无源生成元数据即 Native AOT 下静默失败");
        }
    }

    /// <summary>官方未返回 <c>list</c> 时不得凭空造出列表（区分「空列表」与「字段缺省」）。</summary>
    [Fact]
    public void AdvertiserGetResponse_ShouldLeaveListNull_WhenDataOmitsIt()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{}}
        """;

        var response = JsonSerializer.Deserialize(json, AdvertiserJsonContext.Default.AdsAdvertiserGetResponse);

        response!.Data.Should().NotBeNull();
        response.Data!.List.Should().BeNull();
    }
}
