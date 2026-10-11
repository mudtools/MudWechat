// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Tests.DataModels.Common;

/// <summary>
/// <see cref="AdsQueryJson"/> 的<b>线格式</b>测试 —— 逐字符断言，因为本类的产物直接进 Query，
/// 一个多余空格或一处键名漂移都是官方 <c>code != 0</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须逐字符</b>：<c>fields</c> / <c>filtering</c> 是「单个 Query 键承载 JSON 字面量」的形态
/// （组件的 <c>[Query]</c> 对数组会展开成重复同名参数、对复杂类型按 C# 属性名展平，两种都不是官方线格式，
/// 推导见 <see cref="AdsQueryJson"/> 的 remarks）。因此编码器输出的<b>字节</b>就是契约，
/// 用 <c>JsonDocument</c> 做语义等价断言挡不住「键名写成 PascalCase 但仍合法 JSON」这一类漂移。
/// </para>
/// <para><b>两种转义形态</b>：<c>Utf8JsonWriter</c> 的默认编码器把中文写作 <c>\uXXXX</c>、把 <c>"</c> 写作
/// <c>\u0022</c>。前者与官方 curl 示例的裸 UTF-8 <b>线字节不同</b>、后者与 <c>\"</c> 同理，
/// 但 JSON 语义等价（应答侧解码后同串）⇒ 都钉成显式断言而非留作意外
/// （见 <see cref="Fields_ShouldEscapeNonAsciiAsUnicodeEscape"/>
/// 与 <see cref="Filtering_ShouldEscapeQuoteAndBackslash_WhenValueContainsStructuralCharacters"/>）。</para>
/// </remarks>
public class AdsQueryJsonTests
{
    /// <summary>官方示例的 <c>fields=[]</c> 形态：空列表编码为 <c>[]</c>，而不是 <c>null</c> 或空串。</summary>
    [Fact]
    public void Fields_ShouldEncodeEmptyArray_WhenNoFieldGiven()
    {
        AdsQueryJson.Fields().Should().Be("[]");
    }

    /// <summary>字段名之间<b>无空格</b>（紧凑 JSON），且顺序即入参顺序。</summary>
    [Fact]
    public void Fields_ShouldEncodeCompactArrayInGivenOrder_WhenMultipleFields()
    {
        AdsQueryJson.Fields("account_id", "daily_budget", "system_status")
            .Should().Be("[\"account_id\",\"daily_budget\",\"system_status\"]");
    }

    /// <summary>官方把 <c>fields</c> 标必填但示例用 <c>[]</c> 传空 ⇒ <c>null</c> 集合按空列表处理，不得抛。</summary>
    [Fact]
    public void Fields_ShouldEncodeEmptyArray_WhenCollectionIsNull()
    {
        AdsQueryJson.Fields(null).Should().Be("[]");
    }

    /// <summary>集合内的 <c>null</c> 项被跳过（而非写成 <c>null</c> 元素污染官方数组）。</summary>
    [Fact]
    public void Fields_ShouldSkipNullItems_WhenCollectionContainsNull()
    {
        AdsQueryJson.Fields("account_id", null, "memo")
            .Should().Be("[\"account_id\",\"memo\"]");
    }

    /// <summary>
    /// 非 ASCII 取 <c>\uXXXX</c> 转义（<c>Utf8JsonWriter</c> 默认编码器的非 ASCII 转义集）。
    /// </summary>
    /// <remarks>
    /// 与官方 curl 示例的裸 UTF-8 <b>线字节不同但 JSON 语义等价</b> ⇒ 一并断言解码回来的字符串与原文一致，
    /// 这样「编码器换成宽松 Encoder」的改动会在此变红而不是静默改变上送字节。
    /// </remarks>
    [Fact]
    public void Fields_ShouldEscapeNonAsciiAsUnicodeEscape()
    {
        var encoded = AdsQueryJson.Fields("腾讯");

        encoded.Should().Be("[\"\\u817E\\u8BAF\"]");
        using var document = JsonDocument.Parse(encoded);
        document.RootElement[0].GetString().Should().Be("腾讯");
    }

    /// <summary>无过滤条件时与官方「传空视为无限制」一致：编码为 <c>[]</c>。</summary>
    [Fact]
    public void Filtering_ShouldEncodeEmptyArray_WhenNoConditionGiven()
    {
        AdsQueryJson.Filtering().Should().Be("[]");
        AdsQueryJson.Filtering(null).Should().Be("[]");
    }

    /// <summary>
    /// 单条条件的<b>逐字符</b>线格式：三键名 <c>field</c> / <c>operator</c> / <c>values</c> 照官方原文，
    /// 且 <c>values</c> 恒出现（官方 struct 把它列为必填）。
    /// </summary>
    [Fact]
    public void Filtering_ShouldEncodeOfficialFieldNamesInOrder()
    {
        var encoded = AdsQueryJson.Filtering(new AdsFiltering
        {
            Field = "corporation_name",
            Operator = "EQUALS",
            Values = new List<string> { "example.com" },
        });

        encoded.Should().Be("[{\"field\":\"corporation_name\",\"operator\":\"EQUALS\",\"values\":[\"example.com\"]}]");
    }

    /// <summary>多条条件按入参顺序排列，元素之间以逗号紧邻（官方数组最大长度约束写在端点 XML，编码器不拦）。</summary>
    [Fact]
    public void Filtering_ShouldKeepItemOrder_WhenMultipleConditions()
    {
        var encoded = AdsQueryJson.Filtering(
            new AdsFiltering { Field = "a", Operator = "EQUALS", Values = new List<string> { "1" } },
            new AdsFiltering { Field = "b", Operator = "IN", Values = new List<string> { "2", "3" } });

        encoded.Should().Be(
            "[{\"field\":\"a\",\"operator\":\"EQUALS\",\"values\":[\"1\"]}," +
            "{\"field\":\"b\",\"operator\":\"IN\",\"values\":[\"2\",\"3\"]}]");
    }

    /// <summary><c>values</c> 为 <c>null</c> 时仍写出空数组：官方必填键不得因本地留空而整键消失。</summary>
    [Fact]
    public void Filtering_ShouldAlwaysWriteValuesKey_WhenValuesIsNull()
    {
        var encoded = AdsQueryJson.Filtering(new AdsFiltering { Field = "memo", Operator = "CONTAINS" });

        encoded.Should().Be("[{\"field\":\"memo\",\"operator\":\"CONTAINS\",\"values\":[]}]");
    }

    /// <summary>逐字段省略：<c>Field</c>/<c>Operator</c> 留空时不写出该键（而不是写 <c>null</c>），与 DTO 的 <c>WhenWritingNull</c> 同口径。</summary>
    [Fact]
    public void Filtering_ShouldOmitBlankKeys_WhenFieldOrOperatorIsNull()
    {
        var encoded = AdsQueryJson.Filtering(new AdsFiltering { Values = new List<string> { "x" } });

        encoded.Should().Be("[{\"values\":[\"x\"]}]");
    }

    /// <summary>集合内的 <c>null</c> 条件被跳过（调用方常以「数组里带空洞」的方式构造过滤条件）。</summary>
    [Fact]
    public void Filtering_ShouldSkipNullItems()
    {
        var encoded = AdsQueryJson.Filtering(
            null,
            new AdsFiltering { Field = "a", Operator = "EQUALS", Values = new List<string> { "1" } },
            null);

        encoded.Should().Be("[{\"field\":\"a\",\"operator\":\"EQUALS\",\"values\":[\"1\"]}]");
    }

    /// <summary>
    /// 含引号/反斜杠的过滤值必须被转义 —— 否则一条值就能截断整个 JSON 字面量（注入面）。
    /// </summary>
    /// <remarks>
    /// 引号取 <c>\u0022</c> 而非 <c>\"</c>：这是 <c>JavaScriptEncoder</c> 默认行为（把 <c>"</c> 归入
    /// 「需转义字符」集）。两种形态 JSON 语义相同，但线字节不同 ⇒ 钉成显式断言，
    /// 避免日后有人「顺手换成宽松编码器」却没人发现官方网关的差异。
    /// </remarks>
    [Fact]
    public void Filtering_ShouldEscapeQuoteAndBackslash_WhenValueContainsStructuralCharacters()
    {
        var encoded = AdsQueryJson.Filtering(new AdsFiltering
        {
            Field = "corporation_name",
            Operator = "EQUALS",
            Values = new List<string> { "a\"b\\c" },
        });

        encoded.Should().Be(
            "[{\"field\":\"corporation_name\",\"operator\":\"EQUALS\",\"values\":[\"a\\u0022b\\\\c\"]}]");

        using var document = JsonDocument.Parse(encoded);
        document.RootElement[0].GetProperty("values")[0].GetString().Should().Be("a\"b\\c");
    }

    /// <summary>
    /// <c>date_range</c> 的逐字符线格式：<b>单个</b> JSON 对象、两键名照官方原文、<b>键之间无空格</b>。
    /// </summary>
    /// <remarks>
    /// 官方 curl 写成 <c>-d 'date_range={"start_date": "2024-01-01", "end_date": "2024-01-01"}'</c> ——
    /// 冒号后的空格是命令行里 JSON 字面量的一部分。两种形态 JSON 语义等价（网关解码后同串）⇒
    /// 本条钉紧凑形态（<c>Utf8JsonWriter</c> 的产物），并留一条语义等价断言，
    /// 免得「照 curl 逐字补空格」被当成契约修正。
    /// </remarks>
    [Fact]
    public void DateRange_ShouldEncodeSingleCompactObject()
    {
        var encoded = AdsQueryJson.DateRange(new AdsDateRange
        {
            StartDate = "2024-01-01",
            EndDate = "2024-01-31",
        });

        encoded.Should().Be("{\"start_date\":\"2024-01-01\",\"end_date\":\"2024-01-31\"}");

        // 语义等价核验（DeepEquals 是 .NET 9 才有的 API，此处逐键比对，四档 TFM 与 net8 测试工程都可用）。
        using var official = JsonDocument.Parse(
            """
            {"start_date": "2024-01-01", "end_date": "2024-01-31"}
            """);
        using var ours = JsonDocument.Parse(encoded);
        ours.RootElement.GetProperty("start_date").GetString()
            .Should().Be(official.RootElement.GetProperty("start_date").GetString());
        ours.RootElement.GetProperty("end_date").GetString()
            .Should().Be(official.RootElement.GetProperty("end_date").GetString());
        ours.RootElement.EnumerateObject().Should().HaveCount(2, "官方形态多出的键（如空格造成的伪键）不能存在");
    }

    /// <summary><c>dateRange</c> 为 <c>null</c> 或属性留空时编码为空对象 <c>{}</c>，而不是 <c>null</c> 串。</summary>
    [Fact]
    public void DateRange_ShouldEncodeEmptyObject_WhenNothingGiven()
    {
        AdsQueryJson.DateRange(null).Should().Be("{}");
        AdsQueryJson.DateRange(new AdsDateRange()).Should().Be("{}");
    }

    /// <summary>逐字段省略：只填一支时另一支整键消失（而不是写 <c>null</c>），与 DTO 的 <c>WhenWritingNull</c> 同口径。</summary>
    [Fact]
    public void DateRange_ShouldOmitBlankKey_WhenOnlyStartDateGiven()
    {
        AdsQueryJson.DateRange(new AdsDateRange { StartDate = "2024-01-01" })
            .Should().Be("{\"start_date\":\"2024-01-01\"}");
    }

    /// <summary>
    /// <c>date</c> 值里带引号必须转义：官方把整个值当 JSON 字面量解析，一条未转义的引号就能截断参数
    /// 并把后续内容留在 Query 串里（注入面）。
    /// </summary>
    [Fact]
    public void DateRange_ShouldEscapeQuote_WhenValueContainsStructuralCharacter()
    {
        var encoded = AdsQueryJson.DateRange(new AdsDateRange { StartDate = "2024-01-0\"" });

        encoded.Should().Be("{\"start_date\":\"2024-01-0\\u0022\"}");
        using var document = JsonDocument.Parse(encoded);
        document.RootElement.GetProperty("start_date").GetString().Should().Be("2024-01-0\"");
    }

    /// <summary><c>group_by</c> 与 <c>fields</c> 同为 <c>string[]</c>：紧凑数组、顺序即入参顺序、空集合为 <c>[]</c>。</summary>
    [Fact]
    public void GroupBy_ShouldEncodeCompactArrayInGivenOrder()
    {
        AdsQueryJson.GroupBy().Should().Be("[]");
        AdsQueryJson.GroupBy(null).Should().Be("[]");
        AdsQueryJson.GroupBy("date", "site_set")
            .Should().Be("[\"date\",\"site_set\"]",
                "官方取分版位数据要求 group_by 与 fields 同时带 site_set，编码顺序不影响语义但必须可预期");
    }

    /// <summary>集合内 <c>null</c> 项被跳过（官方数组元素不接受 <c>null</c>）。</summary>
    [Fact]
    public void GroupBy_ShouldSkipNullItems()
    {
        AdsQueryJson.GroupBy("date", null, "channel").Should().Be("[\"date\",\"channel\"]");
    }

    /// <summary>
    /// <c>group_by</c> 超出某一页的上限（同步页 40 / <c>async_reports/add</c> 页 5）时<b>照样编码</b>：
    /// 上限逐页不同 ⇒ 本地拦截必造假阳性，越界由官方 <c>code</c> 表达。
    /// </summary>
    [Fact]
    public void GroupBy_ShouldNotInterceptOversizedList()
    {
        AdsQueryJson.GroupBy("date", "site_set", "channel", "province", "city", "os")
            .Should().Be("[\"date\",\"site_set\",\"channel\",\"province\",\"city\",\"os\"]");
    }

    /// <summary><c>order_by</c> 单条的逐字符线格式：两键名照官方原文，元素之间逗号紧邻。</summary>
    [Fact]
    public void OrderBy_ShouldEncodeOfficialFieldNamesInOrder()
    {
        AdsQueryJson.OrderBy(new AdsOrderBy { SortField = "cost", SortType = "DESCENDING" })
            .Should().Be("[{\"sort_field\":\"cost\",\"sort_type\":\"DESCENDING\"}]");
    }

    /// <summary>空集合与 <c>null</c> 集合都编码为 <c>[]</c>（官方「传空视为无限制」）。</summary>
    [Fact]
    public void OrderBy_ShouldEncodeEmptyArray_WhenNoConditionGiven()
    {
        AdsQueryJson.OrderBy().Should().Be("[]");
        AdsQueryJson.OrderBy(null).Should().Be("[]");
    }

    /// <summary>集合内 <c>null</c> 条件被跳过，其余按入参顺序排列。</summary>
    [Fact]
    public void OrderBy_ShouldSkipNullItemsAndKeepOrder()
    {
        var encoded = AdsQueryJson.OrderBy(
            null,
            new AdsOrderBy { SortField = "cost", SortType = "DESCENDING" },
            new AdsOrderBy { SortField = "view_count", SortType = "ASCENDING" });

        encoded.Should().Be(
            "[{\"sort_field\":\"cost\",\"sort_type\":\"DESCENDING\"}," +
            "{\"sort_field\":\"view_count\",\"sort_type\":\"ASCENDING\"}]");
    }

    /// <summary>逐字段省略：只填 <c>SortField</c> 时 <c>sort_type</c> 整键消失（官方两键都是选填）。</summary>
    [Fact]
    public void OrderBy_ShouldOmitBlankKey_WhenSortTypeIsNull()
    {
        AdsQueryJson.OrderBy(new AdsOrderBy { SortField = "cost" })
            .Should().Be("[{\"sort_field\":\"cost\"}]");
    }

    /// <summary>官方 <c>order_by</c> 数组上限 2，第三条照样编码出去（本地截断会静默丢掉调用方要的排序面）。</summary>
    [Fact]
    public void OrderBy_ShouldNotTruncateBeyondOfficialLimit()
    {
        AdsQueryJson.OrderBy(
                new AdsOrderBy { SortField = "a", SortType = "DESCENDING" },
                new AdsOrderBy { SortField = "b", SortType = "ASCENDING" },
                new AdsOrderBy { SortField = "c", SortType = "DESCENDING" })
            .Should().Be(
                "[{\"sort_field\":\"a\",\"sort_type\":\"DESCENDING\"}," +
                "{\"sort_field\":\"b\",\"sort_type\":\"ASCENDING\"}," +
                "{\"sort_field\":\"c\",\"sort_type\":\"DESCENDING\"}]");
    }
}
