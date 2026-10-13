// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Journal;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 汇报模块（Journal 模块）契约守卫：路由表、接口层级、令牌绑定、上下文登记与字段名/线格式锁定。
/// <para>
/// 官方「汇报」文档树（分类 93392）只在<b>企业自建应用开发</b>根下存在页面
/// （93393 获取汇报记录号列表 / 93394 获取汇报记录详情 / 93395 获取汇报统计数据），
/// 第三方与服务商代开发两棵树均无对应页面
/// ⇒ 「零端点父接口 + 仅自建子接口承载端点」，继承链上不得出现第三方 / 代开发子接口。
/// </para>
/// <para>
/// 与「审批」域分属官方 OA 的两棵子树：本域全部路由为 <c>/cgi-bin/oa/journal/</c>，
/// 审批侧为 <c>/cgi-bin/oa/applyevent</c>、<c>/cgi-bin/oa/getapprovalinfo</c> 等，互不承载。
/// </para>
/// </summary>
public class WechatJournalContractGuards
{
    private const string JournalRegistryGroupName = "Journal";

    /// <summary>
    /// 汇报域官方路由表（3 端点，官方页 93393/93394/93395，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkInternalJournalService),
            nameof(IWechatWorkInternalJournalService.GetJournalRecordListAsync),
            typeof(PostAttribute), "/cgi-bin/oa/journal/get_record_list"),
        (typeof(IWechatWorkInternalJournalService),
            nameof(IWechatWorkInternalJournalService.GetJournalRecordDetailAsync),
            typeof(PostAttribute), "/cgi-bin/oa/journal/get_record_detail"),
        (typeof(IWechatWorkInternalJournalService),
            nameof(IWechatWorkInternalJournalService.GetJournalStatListAsync),
            typeof(PostAttribute), "/cgi-bin/oa/journal/get_stat_list"),
    };

    /// <summary>
    /// 契约守卫 JR1：路由与官方契约逐条一致，且请求面参数名照抄官方原文
    /// （starttime / endtime 无下划线，翻页三件套 cursor / limit / next_cursor + endflag）。
    /// </summary>
    [Fact]
    public void JournalRoutesAndRequestShape_ShouldMatchOfficialContract()
    {
        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须声明于自建子接口");
            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方本域时间参数名为 starttime / endtime（无下划线），与会议室域的 start_time / end_time 不同口径。
        var recordRequestNames = new (PropertyInfo Property, string JsonName)[]
        {
            (typeof(JournalRecordListRequest).GetProperty(nameof(JournalRecordListRequest.StartTimestamp))!, "starttime"),
            (typeof(JournalRecordListRequest).GetProperty(nameof(JournalRecordListRequest.EndTimestamp))!, "endtime"),
            (typeof(JournalRecordListRequest).GetProperty(nameof(JournalRecordListRequest.Cursor))!, "cursor"),
            (typeof(JournalRecordListRequest).GetProperty(nameof(JournalRecordListRequest.Limit))!, "limit"),
            (typeof(JournalRecordListRequest).GetProperty(nameof(JournalRecordListRequest.FilterList))!, "filters"),
        };
        foreach (var (property, jsonName) in recordRequestNames)
        {
            property.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonName,
                $"汇报记录号列表请求参数名照官方原文 {jsonName}");
        }

        typeof(JournalRecordListRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name).Should().BeEquivalentTo(
                new[] { nameof(JournalRecordListRequest.StartTimestamp), nameof(JournalRecordListRequest.EndTimestamp),
                    nameof(JournalRecordListRequest.Cursor), nameof(JournalRecordListRequest.Limit),
                    nameof(JournalRecordListRequest.FilterList) },
                "get_record_list 的请求面即这五个参数，不得凭空增删");

        var statRequestNames = new (PropertyInfo Property, string JsonName)[]
        {
            (typeof(JournalStatListRequest).GetProperty(nameof(JournalStatListRequest.StartTimestamp))!, "starttime"),
            (typeof(JournalStatListRequest).GetProperty(nameof(JournalStatListRequest.EndTimestamp))!, "endtime"),
            (typeof(JournalStatListRequest).GetProperty(nameof(JournalStatListRequest.TemplateId))!, "template_id"),
        };
        foreach (var (property, jsonName) in statRequestNames)
        {
            property.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(jsonName,
                $"汇报统计请求参数名照官方原文 {jsonName}");
        }
    }

    /// <summary>
    /// 契约守卫 JR2：仅单类应用开放域的接口层级——汇报文档树官方仅自建开放，
    /// 「零端点父接口 + 仅自建子接口承载端点」，继承链上不得出现第三方 / 代开发子接口。
    /// </summary>
    [Fact]
    public void JournalSingleAppTypeFamily_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var parent = typeof(IWechatWorkJournalService);
        var child = typeof(IWechatWorkInternalJournalService);

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { child },
            "继承链上不得出现自建之外的子接口：汇报文档树只在自建应用开发根下存在页面");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("零端点父接口：端点全部声明于唯一的自建子接口");

        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(JournalRegistryGroupName,
            $"{child.Name} 必须挂 {JournalRegistryGroupName} 注册组");
        childApi.InheritedFrom.Should().Be(parent.Name.Substring(1),
            "生成器契约：子接口必须以 InheritedFrom 指向父接口实现类名");

        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(Routes.Length, "本域 3 个端点全部承载于唯一的自建子接口");
    }

    /// <summary>
    /// 契约守卫 JR3：令牌绑定——Journal 模块两个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；自建子接口必须声明 InternalAccessToken 归属域键，父接口不声明归属域）。
    /// </summary>
    [Fact]
    public void JournalTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var child = typeof(IWechatWorkInternalJournalService);
        var parent = typeof(IWechatWorkJournalService);

        foreach (var iface in new[] { child, parent })
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }

        child.GetCustomAttribute<TokenAttribute>()!.TokenManagerKey
            .Should().Be(WechatTokenManagerKeys.InternalAccessToken,
                $"{child.Name} 官方仅自建开放，必须声明自建应用级凭据归属域（错配由咽喉点 fail-fast）");

        // 反射陷阱：父接口未显式书写 TokenManagerKey 时读到的是特性构造参数默认值
        // （TokenTypes.AccessToken），故只能断言「不是任何真实归属域键」。
        var parentKey = parent.GetCustomAttribute<TokenAttribute>()!.TokenManagerKey;
        parentKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
            $"{parent.Name} 公共父接口不得声明凭据归属域（调用方须按应用类型选择子接口）");
        parentKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
            $"{parent.Name} 公共父接口不得声明凭据归属域（调用方须按应用类型选择子接口）");
    }

    /// <summary>
    /// 契约守卫 JR4：汇报域的请求 / 响应 DTO 必须全量登记进 AOT JSON 上下文
    /// （记录族 + 统计族共 23 个契约面类型落 JournalJsonContext）。
    /// </summary>
    [Fact]
    public void JournalDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = JournalJsonContext.Default;

        var requiredTypes = new[]
        {
            // 汇报记录族（93393 / 93394）。
            typeof(JournalRecordListRequest), typeof(JournalRecordFilter), typeof(JournalRecordListResponse),
            typeof(JournalRecordDetailRequest), typeof(JournalRecordDetailResponse), typeof(JournalRecordInfo),
            typeof(JournalApplyData), typeof(JournalApplyContent), typeof(JournalControlValue),
            typeof(JournalSchoolContactValue), typeof(JournalDocumentValue), typeof(JournalWedriveFileValue),
            typeof(JournalComment),
            // 汇报统计族（93395）。
            typeof(JournalStatListRequest), typeof(JournalStatListResponse), typeof(JournalStat),
            typeof(JournalReporterRange), typeof(JournalReceiverRange),
            typeof(JournalDepartmentRef), typeof(JournalTagRef), typeof(JournalLeaderRef),
            typeof(JournalReporter), typeof(JournalReportItem),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是汇报模块契约面类型，必须登记进 JournalJsonContext（AOT 源生成）");
        }

        requiredTypes.Should().HaveCount(23, "本域契约面类型总数漂移须同步核对官方三页参数表");
    }

    /// <summary>
    /// 契约守卫 JR5：官方字段名原文保留——三处官方反直觉点必须锁定
    /// （汇报 UUID 在列表侧为 journaluuid / journaluuid_list、在详情侧为 journal_uuid；
    /// 汇报时间在统计条目为 reporttime、在详情为 report_time；
    /// 统计侧部门与标签为 open_partyid / open_tagid 而非 partyid / tagid）。
    /// </summary>
    [Fact]
    public void JournalOfficialFieldNames_ShouldBePreservedVerbatim()
    {
        var expected = new (Type Owner, string Property, string JsonName, string Reason)[]
        {
            (typeof(JournalRecordDetailRequest), nameof(JournalRecordDetailRequest.JournalId),
                "journaluuid", "详情入参官方为 journaluuid（无下划线）"),
            (typeof(JournalRecordListResponse), nameof(JournalRecordListResponse.JournalIdList),
                "journaluuid_list", "列表回传官方为 journaluuid_list（无下划线）"),
            (typeof(JournalRecordInfo), nameof(JournalRecordInfo.JournalId),
                "journal_uuid", "详情报文内官方为 journal_uuid（带下划线，与入参不同形）"),
            (typeof(JournalReportItem), nameof(JournalReportItem.JournalId),
                "journaluuid", "统计条目内官方为 journaluuid（无下划线）"),
            (typeof(JournalRecordInfo), nameof(JournalRecordInfo.ReportTimestamp),
                "report_time", "汇报时间官方详情侧为 report_time（带下划线）"),
            (typeof(JournalReportItem), nameof(JournalReportItem.ReportTimestamp),
                "reporttime", "汇报时间官方统计侧为 reporttime（无下划线，与详情侧不同形）"),
            (typeof(JournalStat), nameof(JournalStat.StatisticsBeginTimestamp),
                "stat_begin_time", "统计范围起点官方为 stat_begin_time（勿与 cycle_begin_time 混用）"),
            (typeof(JournalStat), nameof(JournalStat.CycleBeginTimestamp),
                "cycle_begin_time", "汇报周期起点官方为 cycle_begin_time"),
            (typeof(JournalReporterRange), nameof(JournalReporterRange.DepartmentList),
                "party_list", "部门列表官方容器名为 party_list"),
            (typeof(JournalDepartmentRef), nameof(JournalDepartmentRef.OpenDepartmentId),
                "open_partyid", "部门 ID 官方为 open_partyid（不是 partyid）"),
            (typeof(JournalTagRef), nameof(JournalTagRef.OpenTagId),
                "open_tagid", "标签 ID 官方为 open_tagid（不是 tagid）"),
            (typeof(JournalReceiverRange), nameof(JournalReceiverRange.LeaderList),
                "leader_list", "上级范围容器名官方为 leader_list"),
            (typeof(JournalReporter), nameof(JournalReporter.ReportItemList),
                "itemlist", "汇报条目列表官方为 itemlist（无下划线）"),
            (typeof(JournalStat), nameof(JournalStat.UnreportedReporterList),
                "unreport_list", "未汇报列表官方为 unreport_list"),
            (typeof(JournalRecordInfo), nameof(JournalRecordInfo.ReadedReceiverList),
                "readed_receivers", "已读接收人官方拼写为 readed_receivers（非 reader / read）"),
            (typeof(JournalControlValue), nameof(JournalControlValue.PunchCorrection),
                "punch_correction", "补卡控件值官方为 punch_correction"),
            (typeof(JournalControlValue), nameof(JournalControlValue.WedriveFiles),
                "wedrive_files", "微盘文件控件值官方为 wedrive_files"),
            (typeof(JournalApplyData), nameof(JournalApplyData.ContentList),
                "contents", "汇报内容列表官方为 contents"),
            (typeof(JournalRecordInfo), nameof(JournalRecordInfo.SysJournalData),
                "sys_journal_data", "「汇报」模板为特殊模板，其表单官方不在 apply_data 中返回而落 info.sys_journal_data"),
        };

        foreach (var (owner, property, jsonName, reason) in expected)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            prop!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                .Should().Be(jsonName, reason);
        }

        // 详情响应只有一层 info 容器，不得另建包装 DTO。
        typeof(JournalRecordDetailResponse).GetProperty(nameof(JournalRecordDetailResponse.Info))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("info", "详情响应容器官方为 info");
    }

    /// <summary>
    /// 契约守卫 JR6：官方以数字承载的字段一律以数值类型原样承载
    /// （endflag / flag 为数字 0/1 的布尔语义，本仓不引入数值-布尔转换器；
    /// commentid / tocommentid 官方线格式为数字，须 long 而非 string——声明为 string 会在读取数字时直接抛 JsonException）。
    /// </summary>
    [Fact]
    public void JournalNumericallyCarriedFields_ShouldNotBeDeclaredAsStringOrBoolean()
    {
        var numericalBooleans = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(JournalRecordListResponse), nameof(JournalRecordListResponse.Ending), "endflag"),
            (typeof(JournalReportItem), nameof(JournalReportItem.LateFlag), "flag"),
        };

        foreach (var (owner, property, jsonName) in numericalBooleans)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            prop!.PropertyType.Should().Be(typeof(int?),
                $"{jsonName} 官方以数字 0/1 承载布尔语义，须以 int? 原样承载；改成 bool? 即破坏线格式");
        }

        var numericalIds = new (Type Owner, string Property, string JsonName, Type Expected)[]
        {
            (typeof(JournalComment), nameof(JournalComment.CommentId), "commentid", typeof(long)),
            (typeof(JournalComment), nameof(JournalComment.ToCommentId), "tocommentid", typeof(long?)),
        };

        foreach (var (owner, property, jsonName, expected) in numericalIds)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            prop!.PropertyType.Should().Be(expected,
                $"{jsonName} 官方线格式为数字，须以 {expected} 承载；改成 string 会在读取数字时抛 JsonException");
        }
    }

    /// <summary>
    /// 契约守卫 JR7：请求侧「必填」以类型系统表达（B8 官方正文复核结论，2026-10-13）。
    /// <para>
    /// 93393 参数表把 <c>cursor</c> 与 <c>limit</c> 均标为<b>必须=是</b>（官方原文「游标首次请求传 0，非首次请求携带上一次
    /// 请求返回的 next_cursor」，故 cursor 的缺省值是 0 而非「不传」）；93395 参数表把 <c>template_id</c> 标为<b>必须=是</b>
    /// 且「不多于 256 字节」，即本接口按单一汇报表单取统计、<b>无可选参数</b>。
    /// 三者早期依第三方 SDK 建模为可空（允许请求体省略必填参数 ⇒ 官方回 301066 请求参数错误），现已收为非可空。
    /// </para>
    /// </summary>
    [Fact]
    public void JournalOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable()
    {
        var required = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(JournalRecordListRequest), nameof(JournalRecordListRequest.Cursor), "cursor"),
            (typeof(JournalRecordListRequest), nameof(JournalRecordListRequest.Limit), "limit"),
            (typeof(JournalStatListRequest), nameof(JournalStatListRequest.TemplateId), "template_id"),
        };

        foreach (var (owner, property, jsonName) in required)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            IsNullableMember(prop!).Should().BeFalse(
                $"{jsonName} 官方参数表标为必填，不得声明为可空（可空即允许请求体省略必填参数 ⇒ 官方回 301066 请求参数错误）");
        }

        // 对照侧：filters 官方标「必须=否」，须保持可空以便请求体省略。
        IsNullableMember(typeof(JournalRecordListRequest)
                .GetProperty(nameof(JournalRecordListRequest.FilterList))!)
            .Should().BeTrue("filters 官方为可选参数，收敛为非可空会强制每个请求都带过滤条件");
    }

    /// <summary>
    /// 属性是否可空：值类型的 <see cref="Nullable{T}"/>，或引用类型的可空标注
    /// （<c>PropertyType</c> 不含引用可空性信息，须走 Nullable Context API）。
    /// </summary>
    private static bool IsNullableMember(PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) != null)
        {
            return true;
        }

        return new NullabilityInfoContext().Create(property).WriteState == NullabilityState.Nullable;
    }
}
