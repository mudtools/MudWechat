// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.DataCube;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 数据统计域契约守卫（21 端点单域承载）：路由表（全 POST /datacube/*）、请求体同构共用、
/// 响应 DTO 分族共用裁决、反直觉字段名（feed_share _cnt 后缀）、模块注册。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07，21 页全部在服务号域 /doc/service/api/wedata/ 命中）。
/// 关键核验事实：跨度上限措辞逐端点不同（7/15/30/1 天、必须为同一天——不互相套用）；
/// 旧图文 6 端点官方声明「已停止维护」照常建模并标注；getupstreammsghour 的 ref_hour 仅示例有、
/// 字段表未列（缺口照录不建模）。
/// </remarks>
public class MpDataCubeContractGuards
{
    private const string DataCubeRegistryGroupName = "DataCube";

    /// <summary>数据统计域官方路由表（21 端点，全 POST /datacube/*）。</summary>
    private static readonly (string Method, string Route, Type Response)[] DataCubeRoutes =
    {
        (nameof(IMpDataCubeService.GetUserSummaryAsync), "/datacube/getusersummary", typeof(MpUserSummaryResponse)),
        (nameof(IMpDataCubeService.GetUserCumulateAsync), "/datacube/getusercumulate", typeof(MpUserCumulateResponse)),
        (nameof(IMpDataCubeService.GetArticleSummaryAsync), "/datacube/getarticlesummary", typeof(MpArticleSummaryResponse)),
        (nameof(IMpDataCubeService.GetArticleTotalAsync), "/datacube/getarticletotal", typeof(MpArticleTotalResponse)),
        (nameof(IMpDataCubeService.GetUserReadAsync), "/datacube/getuserread", typeof(MpUserReadResponse)),
        (nameof(IMpDataCubeService.GetUserReadHourAsync), "/datacube/getuserreadhour", typeof(MpUserReadResponse)),
        (nameof(IMpDataCubeService.GetUserShareAsync), "/datacube/getusershare", typeof(MpUserShareResponse)),
        (nameof(IMpDataCubeService.GetUserShareHourAsync), "/datacube/getusersharehour", typeof(MpUserShareResponse)),
        (nameof(IMpDataCubeService.GetArticleReadAsync), "/datacube/getarticleread", typeof(MpDataArticleReadResponse)),
        (nameof(IMpDataCubeService.GetArticleShareAsync), "/datacube/getarticleshare", typeof(MpDataArticleShareResponse)),
        (nameof(IMpDataCubeService.GetBizSummaryAsync), "/datacube/getbizsummary", typeof(MpDataBizSummaryResponse)),
        (nameof(IMpDataCubeService.GetArticleTotalDetailAsync), "/datacube/getarticletotaldetail", typeof(MpDataArticleTotalDetailResponse)),
        (nameof(IMpDataCubeService.GetUpstreamMsgAsync), "/datacube/getupstreammsg", typeof(MpUpstreamMsgResponse)),
        (nameof(IMpDataCubeService.GetUpstreamMsgWeekAsync), "/datacube/getupstreammsgweek", typeof(MpUpstreamMsgResponse)),
        (nameof(IMpDataCubeService.GetUpstreamMsgMonthAsync), "/datacube/getupstreammsgmonth", typeof(MpUpstreamMsgResponse)),
        (nameof(IMpDataCubeService.GetUpstreamMsgHourAsync), "/datacube/getupstreammsghour", typeof(MpUpstreamMsgResponse)),
        (nameof(IMpDataCubeService.GetUpstreamMsgDistAsync), "/datacube/getupstreammsgdist", typeof(MpUpstreamMsgDistResponse)),
        (nameof(IMpDataCubeService.GetUpstreamMsgDistWeekAsync), "/datacube/getupstreammsgdistweek", typeof(MpUpstreamMsgDistResponse)),
        (nameof(IMpDataCubeService.GetUpstreamMsgDistMonthAsync), "/datacube/getupstreammsgdistmonth", typeof(MpUpstreamMsgDistResponse)),
        (nameof(IMpDataCubeService.GetInterfaceSummaryAsync), "/datacube/getinterfacesummary", typeof(MpInterfaceSummaryResponse)),
        (nameof(IMpDataCubeService.GetInterfaceSummaryHourAsync), "/datacube/getinterfacesummaryhour", typeof(MpInterfaceSummaryResponse)),
    };

    /// <summary>卡券统计官方路由表（B2a 4 端点，全 POST /datacube/*；请求体含 cond_source / card_id，非同构）。</summary>
    private static readonly (string Method, string Route, Type Response)[] CardDataCubeRoutes =
    {
        (nameof(IMpDataCubeService.GetCardBizUinInfoAsync), "/datacube/getcardbizuininfo", typeof(MpCardBizUinInfoResponse)),
        (nameof(IMpDataCubeService.GetCardCardInfoAsync), "/datacube/getcardcardinfo", typeof(MpCardCardInfoResponse)),
        (nameof(IMpDataCubeService.GetCardMemberCardInfoAsync), "/datacube/getcardmembercardinfo", typeof(MpCardMemberCardInfoResponse)),
        (nameof(IMpDataCubeService.GetCardMemberCardDetailAsync), "/datacube/getcardmembercarddetail", typeof(MpCardMemberCardDetailResponse)),
    };

    /// <summary>契约守卫 DC1：25 端点路由与官方契约一致（全 POST /datacube/*）。</summary>
    [Fact]
    public void DataCubeEndpoints_ShouldMatchOfficialRoutes()
    {
        DataCubeRoutes.Should().HaveCount(21, "用户 2 + 图文 10（旧 6 + 新 4）+ 消息 7 + 接口 2");
        DataCubeRoutes.Select(r => r.Route).Distinct().Should().HaveCount(21);
        CardDataCubeRoutes.Should().HaveCount(4, "B2a：卡券统计 4（帐号级 / 券级 / 会员卡 / 会员卡明细）");

        foreach (var (method, route, responseType) in DataCubeRoutes.Concat(CardDataCubeRoutes))
        {
            var target = typeof(IMpDataCubeService).GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"IMpDataCubeService.{method} 必须存在");

            var attr = target!.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull("数据统计 25 端点官方均为 POST");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");

            target.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(responseType),
                $"{method} 响应类型必须与官方 list 元素形态对应");
        }
    }

    /// <summary>契约守卫 DC2：请求体同构共用（21 端点共用 MpDateRangeRequest——N4/N3 裁决）。</summary>
    [Fact]
    public void DateRangeRequest_ShouldBeSharedAcrossAllEndpoints()
    {
        foreach (var (method, _, _) in DataCubeRoutes)
        {
            var target = typeof(IMpDataCubeService).GetMethod(method)!;
            target.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpDateRangeRequest),
                $"{method} 请求体为同构 begin_date/end_date ⇒ 共用 MpDateRangeRequest");
        }

        AssertJsonProperty<MpDateRangeRequest>("begin_date", "起始日期（yyyy-MM-dd）");
        AssertJsonProperty<MpDateRangeRequest>("end_date", "结束日期（各端点跨度上限见 remarks）");
    }

    /// <summary>契约守卫 DC3：响应 DTO 分族共用裁决锁定（N4：字段集一致才共用；日/分时对为超集共用）。</summary>
    [Fact]
    public void ResponseDtos_ShouldLockFamilySharing()
    {
        // 消息族 4 端点（day/week/month/hour）响应完全同构 ⇒ 共用 MpUpstreamMsgItem（官方文档缺口：hour 的 ref_hour 仅示例有、字段表未列——不建模）。
        JsonNamesOf<MpUpstreamMsgItem>().Should().BeEquivalentTo(
            new[] { "ref_date", "msg_type", "msg_user", "msg_count" },
            "消息族四端点元素完全同构；ref_hour 在 hour 端点字段表未列（官方文档缺口，照录不建模）");
        AssertJsonProperty<MpUpstreamMsgDistItem>("count_interval", "0='0'/1='1-5'/2='6-10'/3='10次以上'");

        // 日/分时对超集共用：ref_hour 仅分时端点返回。
        AssertJsonProperty<MpUserReadItem>("ref_hour", "仅 getuserreadhour 返回（超集可空）");
        AssertJsonProperty<MpUserShareItem>("ref_hour", "仅 getusersharehour 返回");
        AssertJsonProperty<MpInterfaceSummaryItem>("ref_hour", "仅 getinterfacesummaryhour 返回");
        AssertJsonProperty<MpInterfaceSummaryItem>("total_time_cost", "总耗时（除以 callback_count 即平均耗时）");

        // 用户族两响应元素不同形（summary 带 user_source 渠道维度）⇒ 各自 DTO，不共用。
        typeof(MpUserSummaryItem).Should().NotBe(typeof(MpUserCumulateItem),
            "summary 带 user_source 渠道维度、cumulate 仅两字段 ⇒ 不共用");
        AssertJsonProperty<MpUserSummaryItem>("user_source", "渠道维度（0/1/17/30/57/100/161/149/200/201）");
        AssertJsonProperty<MpUserCumulateItem>("cumulate_user", "总用户量");

        // 新图文族顶层 is_delay（3 端点 + totaldetail 共 4 个响应）。
        AssertJsonProperty<MpDataArticleReadResponse>("is_delay", "数据是否有延迟（false = 最新）");
        AssertJsonProperty<MpDataArticleShareResponse>("is_delay", "数据是否有延迟");
        AssertJsonProperty<MpDataBizSummaryResponse>("is_delay", "数据是否有延迟");
        AssertJsonProperty<MpDataArticleTotalDetailResponse>("is_delay", "数据是否有延迟");
        AssertJsonProperty<MpDataBizDetail>("zaikan_user", "爱心赞人数（官方拼写 zaikan，勿修正）");
        AssertJsonProperty<MpDataBizDetail>("send_page_count", "发布篇数");
        AssertJsonProperty<MpDataSceneCount>("scene_desc", "阅读场景来源描述");
        AssertJsonProperty<MpDataTotalDetailStat>("praise_money", "赞赏金额（单位分）");
        AssertJsonProperty<MpDataJumpPosition>("position", "1~5 对应 0~20% 至 80%~100% 跳出区间");

        // getarticletotal 的嵌套数组名为 details；getarticletotaldetail 的为 detail_list（两页不同名，照各自页面）。
        AssertJsonProperty<MpArticleTotalItem>("details", "getarticletotal 嵌套数组名");
        AssertJsonProperty<MpDataArticleTotalDetailItem>("detail_list", "getarticletotaldetail 嵌套数组名（与 total 不同名，照各自页面）");

        // 反直觉拼写：feed_share 三组后缀为 _cnt（getarticletotal）。
        AssertJsonProperty<MpArticleTotalStat>("feed_share_from_session_cnt", "官方反直觉 _cnt 后缀（其余为 _count）");
        AssertJsonProperty<MpArticleTotalStat>("feed_share_from_feed_cnt", "官方反直觉 _cnt 后缀");
        AssertJsonProperty<MpArticleTotalStat>("feed_share_from_other_cnt", "官方反直觉 _cnt 后缀");
        AssertJsonProperty<MpArticleTotalStat>("int_page_from_session_read_count", "常规 _count 后缀对照");
    }

    /// <summary>契约守卫 DC4：数据统计域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void DataCubeDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = DataCubeJsonContext.Default;

        var domainTypes = typeof(MpDateRangeRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.DataCube"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 45;
        domainTypes.Should().HaveCount(expectedCount,
            "数据统计域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（共用请求 1 + 用户 4 + 旧图文 8 + 新图文 12 + 消息 4 + 接口 2 + 场景/跳出辅助 3 +" +
            " B2a 卡券统计 11：请求 3（帐号级 / 券级 / 会员卡明细）+ 数据项 4（帐号级 / 券级 / 会员卡 / 明细）+ 响应 4，帐号级请求为会员卡 info 复用）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于数据统计域命名空间，必须登记进 DataCubeJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(DataCubeRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 DataCube");
        }
    }

    /// <summary>契约守卫 DC5：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void DataCubeModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("DataCube");
        typeof(MpServiceBuilder).GetMethod("AddDataCubeApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpDataCubeService),
            "本域必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 DC6：数据统计域错误码常量锁定（官方错误码表跨页复用）。</summary>
    [Fact]
    public void DataCubeErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.DataCubeDateFormatError.Should().Be(61500);
        MpErrorCodes.DataCubeDateRangeError.Should().Be(61501);
        MpErrorCodes.DataCubeDataNotReady.Should().Be(61503);
    }

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
