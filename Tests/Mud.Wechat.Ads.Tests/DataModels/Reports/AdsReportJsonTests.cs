// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization.Metadata;

namespace Mud.Wechat.Ads.Tests.DataModels.Reports;

/// <summary>
/// 报表族 DTO 的<b>源生成上下文</b>序列化测试（照官方四页的应答/请求样例逐字段核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须走 <c>ReportsJsonContext.Default</c></b>：反射版 <c>JsonSerializer</c> 在 Native AOT 下没有
/// 元数据，用它测试等于「JIT 下绿、AOT 下炸」（守卫 ADS-B6 拦的就是这种混用）。本类同时是
/// 「生成物是否真的覆盖到四支闭合应答类型」的<b>运行期</b>证据 —— 生成器漏类型时编译期守卫抓不到。
/// </para>
/// <para>
/// <b>样例来源</b>：2026-10-10 对 <c>daily_reports/get</c> / <c>hourly_reports/get</c> /
/// <c>async_reports/add</c> / <c>async_reports/get</c> 四页的逐页核验（留档 §7.8）。官方示例里
/// <c>date_range</c> 的值带空格（<c>{"start_date": "2024-01-01", …}</c>），那是 curl 命令行里的字面量，
/// JSON 语义与紧凑形态等价 ⇒ 序列化断言取<b>紧凑形态</b>（编码器 <c>Utf8JsonWriter</c> 的产物）。
/// </para>
/// <para>
/// <b>两条本族独有的断言主线</b>：① 同步报表的<b>行按透传建模</b> ⇒ 必须证明「键名不被命名策略改写」
/// 且「值保留原始 JSON 形态」（金额整数分 / 比率浮点 / 维度数组各按其型）；
/// ② 异步链路的<b>双层判定</b> ⇒ 必须证明 <c>result.code</c> 缺省时读到 <c>null</c> 而不是 <c>0</c>
/// （非空会把「任务未完成」读成「任务成功」，是本域最危险的一类假绿）。
/// </para>
/// </remarks>
public class AdsReportJsonTests
{
    private static readonly IJsonTypeInfoResolver Resolver =
        JsonTypeInfoResolver.Combine(ReportsJsonContext.Default, CommonJsonContext.Default);

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = Resolver,
    };

    /// <summary>
    /// <c>daily_reports/get</c> 应答样例：外层信封 + <c>page_info</c> 落到声明属性，
    /// <c>data.list[]</c> 的行按<b>官方键名原样</b>入袋、值保留原始 JSON 形态。
    /// </summary>
    [Fact]
    public void DailyReportResponse_ShouldBindOfficialSample_AndKeepRowKeysVerbatim()
    {
        const string json = """
        {
          "code": 0,
          "message": "",
          "message_cn": "",
          "data": {
            "list": [
              {
                "adgroup_id": 1234567890,
                "adgroup_name": "示例营销单元",
                "date": "2024-01-01",
                "site_set": ["WECHAT_MOMENTS", "TENCENT_NEWS"],
                "view_count": 10000,
                "cost": 123456,
                "ctr": 0.0123
              }
            ],
            "page_info": { "page": 1, "page_size": 2000, "total_number": 1, "total_page": 1 }
          }
        }
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsDailyReportResponse);

        response!.IsSuccess.Should().BeTrue();
        var row = response.Data!.List!.Single();

        row.Keys.Should().Equal(new[]
        {
            "adgroup_id", "adgroup_name", "date", "site_set", "view_count", "cost", "ctr",
        }, "行必须按请求侧 fields 的官方键名原样可读：改名或漏键会让调用方按 fields 取值时拿不到数据");

        row["adgroup_id"].GetInt64().Should().Be(1234567890L, "官方 id 面是 int64，非 int32");
        row["adgroup_name"].GetString().Should().Be("示例营销单元");
        row["view_count"].ValueKind.Should().Be(JsonValueKind.Number);

        // 金额单位是分（官方原文），SDK 不做单位换算 ⇒ 原值入袋，换算责任在宿主。
        row["cost"].GetInt64().Should().Be(123456L);
        row["ctr"].GetDouble().Should().Be(0.0123d, "比率类指标是浮点，取整会静默归零");

        row["site_set"].ValueKind.Should().Be(JsonValueKind.Array, "维度数组保留数组形态，不得被压成字符串");
        row["site_set"].EnumerateArray().Select(static e => e.GetString())
            .Should().Equal(new[] { "WECHAT_MOMENTS", "TENCENT_NEWS" });

        response.Data.PageInfo!.Page.Should().Be(1);
        response.Data.PageInfo.PageSize.Should().Be(2000);
        response.Data.PageInfo.TotalNumber.Should().Be(1);
        response.Data.PageInfo.TotalPage.Should().Be(1);
    }

    /// <summary>
    /// 字典键<b>不</b>经命名策略转换：上下文设了 <c>PropertyNamingPolicy = SnakeCaseLower</c>，
    /// 但 <c>DictionaryKeyPolicy</c> 未设 ⇒ 驼峰键也必须原样入袋。
    /// </summary>
    /// <remarks>
    /// 官方返回的都是 snake_case，因此「策略命中键名」在上一条样例里<b>看不出来</b>（snake 再转一次还是 snake）。
    /// 本条用一支驼峰键把这条区分打开：若有人给上下文补 <c>DictionaryKeyPolicy</c>、或把行改成
    /// 走属性名映射的形态，这里的键会被改写而立即变红。
    /// </remarks>
    [Fact]
    public void DailyReportResponse_ShouldNotRewriteRowKeys_WhenKeyIsNotSnakeCase()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{"list":[{"viewCount":1,"cost_ratio":2}]}}
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsDailyReportResponse);

        var row = response!.Data!.List!.Single();
        row.Keys.Should().Equal(new[] { "viewCount", "cost_ratio" },
            "字典键必须逐字符照官方报文入袋：命名策略只作用于 CLR 属性名");
        row.ContainsKey("view_count").Should().BeFalse("不得凭空造出「转换后」的别名键");
    }

    /// <summary>
    /// <c>hourly_reports/get</c> 与 daily 共用一支载荷 ⇒ 两支闭合信封的 <c>Data</c> 声明类型必须是同一支
    /// <see cref="AdsReportListData"/>（各建一份同形载荷即双源），且同一份报文喂给两支得到等价形状。
    /// </summary>
    [Fact]
    public void HourlyReportResponse_ShouldShareTheSamePayloadContract_WithDailyReports()
    {
        const string json = """
        {
          "code": 0,
          "message": "",
          "message_cn": "",
          "data": {
            "list": [{ "hour": 9, "date": "2024-01-01", "view_count": 12 }],
            "page_info": { "page": 1, "page_size": 2000, "total_number": 1, "total_page": 1 }
          }
        }
        """;

        var daily = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsDailyReportResponse);
        var hourly = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsHourlyReportResponse);

        hourly!.Data!.Should().BeOfType<AdsReportListData>();
        daily!.Data!.Should().BeOfType<AdsReportListData>();

        hourly.Data.List!.Single().Keys.Should().Equal(new[] { "hour", "date", "view_count" });
        hourly.Data.List!.Single()["hour"].GetInt32().Should().Be(9, "hourly 页的小时维度是整数");
        hourly.Data.PageInfo!.PageSize.Should().Be(2000);
    }

    /// <summary>官方未返回 <c>list</c> 时不得凭空造出列表（区分「空列表」与「字段缺省」）。</summary>
    [Fact]
    public void DailyReportResponse_ShouldLeaveListNull_WhenDataOmitsIt()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{}}
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsDailyReportResponse);

        response!.Data.Should().NotBeNull();
        response.Data!.List.Should().BeNull();
        response.Data.PageInfo.Should().BeNull();
    }

    /// <summary>
    /// <c>async_reports/add</c> 请求体：根键是 <c>report_fields</c>（<b>不是</b> <c>fields</c>）、
    /// 日期是单支 <c>date</c>（<b>不存在</b> <c>date_range</c>），且未填字段整键省略。
    /// </summary>
    [Fact]
    public void AsyncReportAddRequest_ShouldSerializeOfficialBodyShape_WhenOmittingUnsetKeys()
    {
        var request = new AdsAsyncReportAddRequest
        {
            AccountId = 12345678,
            TaskName = "每日报表任务",
            Level = "REPORT_LEVEL_ADGROUP",
            Granularity = "DAILY",
            Date = "2024-01-01",
            ReportFields = new List<string> { "view_count", "cost" },
            GroupBy = new List<string> { "date" },
            Filtering = new List<AdsFiltering>
            {
                new() { Field = "adgroup_id", Operator = "EQUALS", Values = new List<string> { "123" } },
            },
        };

        var json = JsonSerializer.Serialize(request, ReportsJsonContext.Default.AdsAsyncReportAddRequest);

        json.Should().Be(
            "{\"account_id\":12345678,\"task_name\":\"\\u6BCF\\u65E5\\u62A5\\u8868\\u4EFB\\u52A1\"," +
            "\"report_fields\":[\"view_count\",\"cost\"],\"level\":\"REPORT_LEVEL_ADGROUP\"," +
            "\"filtering\":[{\"field\":\"adgroup_id\",\"operator\":\"EQUALS\",\"values\":[\"123\"]}]," +
            "\"group_by\":[\"date\"],\"granularity\":\"DAILY\",\"date\":\"2024-01-01\"}");

        json.Should().NotContain("\"fields\"", "键名写成 fields 会被官方静默忽略：任务建出来但没有报表字段");
        json.Should().NotContain("date_range", "本页官方只有单支 date（配 granularity）");
        json.Should().NotContain("organization_id", "未填字段必须整键省略（WhenWritingNull），而不是写 null");
    }

    /// <summary>
    /// 请求侧 <c>time_line</c> 与同步 Query 侧同名参数一样是<b>字符串枚举</b>（官方无本地枚举可校验）⇒
    /// 原样上送；空列表 <c>report_fields=[]</c> 与「不传」在线上不可区分的是官方，不是本类 ——
    /// <c>report_fields</c> 官方必填，空列表仍会被写出（不能被 <c>WhenWritingNull</c> 吃掉）。
    /// </summary>
    [Fact]
    public void AsyncReportAddRequest_ShouldKeepEmptyReportFieldsArray()
    {
        var request = new AdsAsyncReportAddRequest
        {
            AccountId = 1,
            ReportFields = new List<string>(),
            TimeLine = "REQUEST_TIME",
        };

        var json = JsonSerializer.Serialize(request, ReportsJsonContext.Default.AdsAsyncReportAddRequest);

        json.Should().Contain("\"report_fields\":[]");
        json.Should().Contain("\"time_line\":\"REQUEST_TIME\"");
    }

    /// <summary><c>async_reports/add</c> 的 <c>data</c> 官方只回 <c>task_id</c>。</summary>
    [Fact]
    public void AsyncReportAddResponse_ShouldBindTaskIdOnly()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{"task_id":987654321}}
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsAsyncReportAddResponse);

        response!.IsSuccess.Should().BeTrue();
        response.Data!.TaskId.Should().Be(987654321L);
    }

    /// <summary>
    /// <c>async_reports/get</c> 的成功样例：三层 <c>result.data.file_info_list[]{file_id, md5}</c> 全部落到声明属性，
    /// 且内层 <c>result</c> <b>没有</b> <c>message_cn</c> 支（与外层信封不同）。
    /// </summary>
    [Fact]
    public void AsyncReportGetResponse_ShouldBindThreeLevelResultChain()
    {
        const string json = """
        {
          "code": 0,
          "message": "",
          "message_cn": "",
          "data": {
            "list": [
              {
                "task_id": 987654321,
                "task_name": "每日报表任务",
                "status": "TASK_STATUS_COMPLETED",
                "created_time": 1704067200,
                "result": {
                  "code": 0,
                  "message": "",
                  "data": {
                    "file_info_list": [
                      { "file_id": 555000111, "md5": "06f9460ae725eead045b9e28f06d2dab" }
                    ]
                  }
                }
              }
            ],
            "page_info": { "page": 1, "page_size": 10, "total_number": 1, "total_page": 1 }
          }
        }
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsAsyncReportGetResponse);

        var task = response!.Data!.List!.Single();
        task.TaskId.Should().Be(987654321L);
        task.Status.Should().Be("TASK_STATUS_COMPLETED");
        task.CreatedTime.Should().Be(1704067200L);

        task.Result!.Code.Should().Be(0, "外层 code == 0 只表示「查询任务这个请求成功」，任务本身成没成看 result.code");
        task.Result.Data!.FileInfoList!.Single().FileId.Should().Be(555000111L);
        task.Result.Data.FileInfoList.Single().Md5.Should().Be("06f9460ae725eead045b9e28f06d2dab");
    }

    /// <summary>
    /// <b>双层判定的假绿防线</b>：任务未完成时官方把 <c>result</c> 整支缺省、或 <c>result</c> 在而
    /// <c>code</c> 缺省 ⇒ 两者都必须读成 <c>null</c>，绝不能读成 <c>0</c>（0 = 成功）。
    /// </summary>
    [Fact]
    public void AsyncReportGetResponse_ShouldLeaveInnerCodeNull_WhenTaskIsNotFinished()
    {
        const string json = """
        {
          "code": 0,
          "message": "",
          "message_cn": "",
          "data": {
            "list": [
              { "task_id": 1, "status": "TASK_STATUS_NEW" },
              { "task_id": 2, "status": "TASK_STATUS_EXECUTING", "result": { "message": "" } },
              { "task_id": 3, "status": "TASK_STATUS_COMPLETED", "result": { "code": 12345, "message": "quota exceeded" } }
            ]
          }
        }
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsAsyncReportGetResponse);

        response!.IsSuccess.Should().BeTrue("外层成功不代表任何一条任务成功");

        var tasks = response.Data!.List!.ToDictionary(static t => t.TaskId!.Value, static t => t);
        tasks[1].Result.Should().BeNull("官方未完成时整字段缺省，不得被造出空对象");
        tasks[2].Result!.Code.Should().BeNull("result.code 缺省必须读成 null，非可空会把它读成 0 = 成功");
        tasks[3].Result!.Code.Should().Be(12345, "任务侧失败是内层判定面，外层 0 挡不住");
        tasks[3].Status.Should().Be("TASK_STATUS_COMPLETED", "status 与 result.code 是两个正交面：完成 ≠ 成功");
    }

    /// <summary>
    /// 轮询契约的可执行证据：<c>status</c> 是<b>应答</b>字段（本页 <c>filtering.field</c> 官方只给
    /// <c>{task_id, task_name}</c>）⇒ 「等到完成」只能读回 <see cref="AdsAsyncReportTaskInfo.Status"/>。
    /// </summary>
    [Fact]
    public void AsyncReportTaskInfo_ShouldExposeStatusForPolling_WhenFilterCannotCarryIt()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{"list":[{"task_id":7,"status":"TASK_STATUS_COMPLETED"}]}}
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsAsyncReportGetResponse);

        var task = response!.Data!.List!.Single();
        task.Status.Should().Be("TASK_STATUS_COMPLETED");
        task.Result.Should().BeNull("官方本页示例在 completed 时仍可能不带 result，调用方须两者都判");

        // 过滤条件面：AdsFiltering 的 values 是字符串列表，status 值同样只能作为应答读回，不构成请求侧类型。
        AdsQueryJson.Filtering(new AdsFiltering
        {
            Field = "task_id",
            Operator = "EQUALS",
            Values = new List<string> { "7" },
        }).Should().Be("[{\"field\":\"task_id\",\"operator\":\"EQUALS\",\"values\":[\"7\"]}]");
    }

    /// <summary>
    /// 生成上下文必须能解析本域<b>闭合</b>应答类型与请求体（开放泛型 <c>AdsResponse&lt;T&gt;</c> 不登记）。
    /// </summary>
    [Fact]
    public void ReportsJsonContext_ShouldProvideTypeInfoForRegisteredTypes()
    {
        foreach (var type in new[]
                 {
                     typeof(AdsDailyReportResponse),
                     typeof(AdsHourlyReportResponse),
                     typeof(AdsAsyncReportAddResponse),
                     typeof(AdsAsyncReportGetResponse),
                     typeof(AdsAsyncReportAddRequest),
                     typeof(AdsReportListData),
                     typeof(AdsAsyncReportTaskInfo),
                     typeof(AdsAsyncTaskResult),
                     typeof(AdsAsyncTaskFileInfo),
                 })
        {
            Resolver.GetTypeInfo(type, Options)
                .Should().NotBeNull($"{type.Name} 无源生成元数据即 Native AOT 下静默失败");
        }
    }

    /// <summary>
    /// 共用子树必须<b>同一 CLR 类型</b>：异步任务列表的 <c>page_info</c> 与同步报表、客户账号域回的是
    /// 同一支 <see cref="AdsPageInfo"/> ⇒ 官方改分页字段时只有一处要改（分别建模即三源）。
    /// </summary>
    [Fact]
    public void ReportsPayloads_ShouldReuseCommonPageInfoType()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{"list":[],"page_info":{"page":2,"page_size":100,"total_number":0,"total_page":0}}}
        """;

        var asyncGet = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsAsyncReportGetResponse);
        var daily = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsDailyReportResponse);

        asyncGet!.Data!.PageInfo.Should().BeOfType<AdsPageInfo>();
        daily!.Data!.PageInfo.Should().BeOfType<AdsPageInfo>();
        daily.Data.PageInfo!.Page.Should().Be(2);

        // 两支 list 形态不同形（同步透传字典 / 异步固定结构），空数组各自落成空集合而不是 null。
        daily.Data.List.Should().BeEmpty();
        asyncGet.Data.List.Should().BeEmpty();
    }

    /// <summary>
    /// 分页上限（<c>page * page_size &lt;= 20000</c>）与 <c>group_by</c> 数组上限（同步页 40 /
    /// <c>async_reports/add</c> 页 5）<b>只写在官方说明文字里且逐页不同</b> ⇒ 请求侧不做本地拦截，
    /// 编码线格式的部分由 <c>AdsQueryJsonTests</c> 逐字符锁定。
    /// </summary>
    /// <remarks>
    /// 本条只断言「响应侧读到越界值也不炸」：官方在超限时回 <c>code != 0</c>，而 <c>ThrowIfFailed</c>
    /// 是唯一判错入口 —— 若把 <c>page_size</c> 之类当业务字段做强校验，官方回的空 <c>data</c> 会先在这里抛，
    /// 掩盖真正的官方错误码。
    /// </remarks>
    [Fact]
    public void DailyReportResponse_ShouldBindEmptyData_WhenOfficialRejectsOversizedPaging()
    {
        const string json = """
        {"code":133416,"message":"page size is out of range","message_cn":"分页参数超出上限","data":null}
        """;

        var response = JsonSerializer.Deserialize(json, ReportsJsonContext.Default.AdsDailyReportResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(133416, "越界分页是官方判定并以 code 表达，SDK 不在请求侧预拦截");
        response.Data.Should().BeNull("官方失败信封的 data 整支缺省，不得被造出空载荷");

        var act = () => WechatAdsException.ThrowIfFailed(
            response, "https://api.e.qq.com/v3.0/daily_reports/get");
        act.Should().Throw<WechatAdsException>().Which.MessageCn.Should().BeNull(
            "ThrowIfFailed 走公用契约面（只有 code / message），中文支不可达是既有不对称");
    }
}
