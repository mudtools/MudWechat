// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「交易保障」域契约守卫（TG 系列：16 端点路由与动词 + 令牌绑定 + GET 参数形态 +
/// DTO 登记计数与 JSON 上下文登记 + 字段抽样与共用 DTO 裁决 + 官方反直觉点锁定）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-13 逐页核验，transaction-guarantee 分区全集 16 端点）</b>：
/// 基础 2（<c>/wxaapi/wxamptrade/*</c>，<see cref="IWxaTradeGuaranteeService"/>）+
/// 评价管理 9（<c>/wxaapi/comment/*</c>，<see cref="IWxaTradeCommentService"/>）+
/// 投诉管理 5（<c>/wxaapi/minishop/*</c> 历史前缀，<see cref="IWxaTradeComplaintService"/>）。
/// </para>
/// <para>
/// <b>与前缀禁令的关系（本域最需要防误读的一处）</b>：投诉管理 5 条沿用 <c>minishop</c> 前缀，
/// 而 P3-R1 历史上按前缀整族禁止该入口。官方已把这 5 页整族迁入小程序
/// <c>transaction-guarantee/complaint</c> 章节（适用范围「小程序」、令牌 <c>access_token</c>、
/// 权限集 76）⇒ P3-R1 于 2026-10-13 <b>收窄为清单式在册</b>（见
/// <c>MiniProgramP3RulingsContractGuards</c>）：店铺 / 商品管理面仍零声明，本 5 条在册。
/// 两侧守卫合起来才是该前缀的完整闸门。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）、端点聚合计数
/// （MP-X5 / ContractBaseline）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaTransactionGuaranteeContractGuards
{
    private const string RegistryGroupName = "TransactionGuarantee";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee";

    /// <summary>
    /// 交易保障域官方路由表（16 端点：基础 2 + 评价管理 9 + 投诉管理 5；路由与动词照官方原文）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaTradeGuaranteeService), nameof(IWxaTradeGuaranteeService.GetGuaranteeStatusAsync), typeof(GetAttribute), "/wxaapi/wxamptrade/get_guarantee_status"),
        (typeof(IWxaTradeGuaranteeService), nameof(IWxaTradeGuaranteeService.GetPenaltyListAsync), typeof(GetAttribute), "/wxaapi/wxamptrade/get_penalty_list"),

        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.GetCommentListAsync), typeof(GetAttribute), "/wxaapi/comment/mpcommentlist/get"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.GetCommentInfoAsync), typeof(GetAttribute), "/wxaapi/comment/commentinfo/get"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.GetReplyAndCommentReplyListAsync), typeof(GetAttribute), "/wxaapi/comment/replyandcommentreplylist/get"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.AddReplyAsync), typeof(PostAttribute), "/wxaapi/comment/reply/add"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.DeleteReplyAsync), typeof(PostAttribute), "/wxaapi/comment/reply/delete"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.AddCommentReplyAsync), typeof(PostAttribute), "/wxaapi/comment/commentreply/add"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.DeleteCommentReplyAsync), typeof(PostAttribute), "/wxaapi/comment/commentreply/delete"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.ConfirmCompromiseAsync), typeof(PostAttribute), "/wxaapi/comment/confirmcompromise"),
        (typeof(IWxaTradeCommentService), nameof(IWxaTradeCommentService.ResetApiKfQuotaAsync), typeof(PostAttribute), "/wxaapi/comment/apikfquota/reset"),

        (typeof(IWxaTradeComplaintService), nameof(IWxaTradeComplaintService.GetComplaintOrderDetailAsync), typeof(GetAttribute), "/wxaapi/minishop/complaintOrderDetail"),
        (typeof(IWxaTradeComplaintService), nameof(IWxaTradeComplaintService.RespondComplaintAsync), typeof(PostAttribute), "/wxaapi/minishop/bussiRespondComplaint"),
        (typeof(IWxaTradeComplaintService), nameof(IWxaTradeComplaintService.SupplyProofAsync), typeof(PostAttribute), "/wxaapi/minishop/bussiSupplyProof"),
        (typeof(IWxaTradeComplaintService), nameof(IWxaTradeComplaintService.SupplyRefundAsync), typeof(PostAttribute), "/wxaapi/minishop/bussiSupplyRefund"),
        (typeof(IWxaTradeComplaintService), nameof(IWxaTradeComplaintService.AppealAsync), typeof(PostAttribute), "/wxaapi/minishop/busiAppeal"),
    };

    /// <summary>契约守卫 TG1：16 端点路由与动词必须与官方契约一致（前缀只允许三个官方族）。</summary>
    [Fact]
    public void TransactionGuaranteeEndpoints_ShouldMatchOfficialRoutes()
    {
        // 16 端点 = 基础 2 + 评价管理 9 + 投诉管理 5（官方 transaction-guarantee 分区全集）。
        Routes.Should().HaveCount(16,
            "交易保障域官方开放面为 16 端点（官方文档 2026-10-13 逐页核验：basic 2 + comment 9 + complaint 5）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(16, "路由表不得出现重复条目");

        var allowedPrefixes = new[] { "/wxaapi/wxamptrade/", "/wxaapi/comment/", "/wxaapi/minishop/" };

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文拼写，禁止改写）");
            allowedPrefixes.Any(p => route.StartsWith(p, StringComparison.Ordinal)).Should().BeTrue(
                $"{route} 必须落在官方三个路由族之一（/wxaapi/wxamptrade/ + /wxaapi/comment/ + /wxaapi/minishop/）");
        }

        // 反直觉点锁定：投诉族三个商家动作为 bussi*、申诉为 busiAppeal，官方两族拼写不一致，禁止「顺手规整」。
        Routes.Where(static r => r.Route.StartsWith("/wxaapi/minishop/", StringComparison.Ordinal))
            .Select(static r => r.Route)
            .Should().Contain("/wxaapi/minishop/bussiRespondComplaint")
            .And.Contain("/wxaapi/minishop/busiAppeal",
                "官方拼写为 bussi（三个 s）与 busi 两族混用，照录不规整");
    }

    /// <summary>
    /// 契约守卫 TG2：GET 端点参数形态 —— 官方在 GET 页给出「请求体」表，但示例一律走查询串
    /// ⇒ GET 侧必须只有 <c>[Query]</c>、零 <c>[Body]</c>；POST 侧恰有一个 <c>[Body]</c> 请求体参数。
    /// </summary>
    [Fact]
    public void TransactionGuaranteeEndpoints_ShouldRespectGetQueryAndPostBodyShapes()
    {
        // GET 6 端点的查询参数名集（照官方参数表，含域内唯一的「无业务参数」端点）。
        var expectedQueryParams = new Dictionary<string, string[]>
        {
            [nameof(IWxaTradeGuaranteeService.GetGuaranteeStatusAsync)] = Array.Empty<string>(),
            [nameof(IWxaTradeGuaranteeService.GetPenaltyListAsync)] = new[] { "offset", "limit" },
            [nameof(IWxaTradeCommentService.GetCommentListAsync)] = new[] { "startTime", "endTime", "filterType", "offset", "limit" },
            [nameof(IWxaTradeCommentService.GetCommentInfoAsync)] = new[] { "commentId" },
            [nameof(IWxaTradeCommentService.GetReplyAndCommentReplyListAsync)] = new[] { "commentId" },
            [nameof(IWxaTradeComplaintService.GetComplaintOrderDetailAsync)] = new[] { "complaintOrderId" },
        };

        var getRoutes = Routes.Where(static r => r.HttpAttribute == typeof(GetAttribute)).ToList();
        getRoutes.Should().HaveCount(6, "官方动词为 GET 的端点为 6（basic 2 + comment 3 + complaint 1）");

        foreach (var (iface, method, _, route) in getRoutes)
        {
            var target = iface.GetMethod(method)!;
            target.GetParameters()
                .SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{route} 官方动词为 GET——全仓零 GET + [Body] 先例，参数须走 [Query]");

            var queryNames = target.GetParameters()
                .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
                .Where(static n => n != null)
                .Select(static n => n!)
                .ToArray();

            queryNames.Should().BeEquivalentTo(expectedQueryParams[method],
                because: $"{route} 的业务参数名须逐字照官方参数表（camelCase 域内原样，不做 snake_case 归一）");
        }

        var postRoutes = Routes.Where(static r => r.HttpAttribute == typeof(PostAttribute)).ToList();
        postRoutes.Should().HaveCount(10, "官方动词为 POST 的端点为 10（comment 6 + complaint 4）");

        foreach (var (iface, method, _, route) in postRoutes)
        {
            var target = iface.GetMethod(method)!;
            target.GetParameters()
                .Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Should().Be(1, $"{route} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 TG3：三接口令牌绑定与注册组（复用公众号 <c>MpTokenTypes.AccessToken</c>，MP-X2/X3 同口径）。</summary>
    [Fact]
    public void TransactionGuaranteeInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup()
    {
        var interfaces = new[]
        {
            typeof(IWxaTradeGuaranteeService),
            typeof(IWxaTradeCommentService),
            typeof(IWxaTradeComplaintService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方适用范围「小程序」账号类型）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（改绑小店令牌或新增类型会让单槽 Resolve 返回 null，errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3）");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 TransactionGuarantee 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");
        }

        // 三接口同组 ⇒ DI 侧由同一条生成入口装载；注册面漂移由 MiniProgramContractGuards 的装配用例兜住。
        interfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域三接口共用一个注册组（AddTransactionGuaranteeWebApiHttpClient）");
    }

    /// <summary>契约守卫 TG4：DTO 全量登记进 AOT JSON 上下文；命名空间清单锁死（常量类恰 2）。</summary>
    [Fact]
    public void TransactionGuaranteeDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaTradeGuaranteeStatusResponse).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("交易保障域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 41 DTO = 基础 3（状态响应 + 违规列表响应 + 违规条目）
        //          + 评价管理 30（请求 6 + 共用操作响应 1 + 列表族 10 + 详情族 7 + 评论回复族 6）
        //          + 投诉管理 8（详情响应 + 投诉单 + 投诉信息 + 进度节点 + 退货单 + 请求 3）。
        dtoTypes.Should().HaveCount(41,
            "交易保障域 16 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫"
            + "（basic 3 + comment 30 + complaint 8 = 41）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 TransactionGuarantee");
        }

        var contextType = assembly.GetType(DtoNamespace + ".TransactionGuaranteeJsonContext");
        contextType.Should().NotBeNull(
            "TransactionGuaranteeJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"TransactionGuarantee\")]，"
                + "必须登记进 TransactionGuaranteeJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：除已登记 DTO、生成上下文与两个官方枚举常量类外不得出现任何新类型。
        var constClasses = new[] { nameof(WxaCommentFilterType), nameof(WxaComplaintBussiHandle) };
        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType && !constClasses.Contains(t.Name))
            .ToList();

        unregistered.Should().BeEmpty(
            "交易保障域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文、亦非已登记常量类的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        constClasses.Select(n => assembly.GetType(DtoNamespace + "." + n))
            .Should().OnlyContain(t => t != null,
            "两个官方枚举常量类必须存在（评价过滤类型 + 回应投诉操作）");
    }

    /// <summary>契约守卫 TG5：官方字段名抽样 + 类型口径 + 共用 DTO 裁决 + 官方反直觉点锁定。</summary>
    [Fact]
    public void TransactionGuaranteeDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 基础域（/wxaapi/wxamptrade/*，官方参数表字段为 camelCase，照录不蛇形化）。
        AssertJsonProperty<WxaTradeGuaranteeStatusResponse>(nameof(WxaTradeGuaranteeStatusResponse.IsActived), "isActived", "官方原文拼写");
        AssertJsonProperty<WxaTradeGuaranteeStatusResponse>(nameof(WxaTradeGuaranteeStatusResponse.Msg), "msg", "官方原文拼写");
        AssertJsonProperty<WxaTradePenaltyListResponse>(nameof(WxaTradePenaltyListResponse.AppealList), "appealList", "官方原文拼写（列表字段名为 appealList 而非 penaltyList）");
        AssertJsonProperty<WxaTradePenaltyItem>(nameof(WxaTradePenaltyItem.IllegalWording), "illegalWording", "官方原文拼写");
        AssertJsonProperty<WxaTradePenaltyItem>(nameof(WxaTradePenaltyItem.ComplaintOrderId), "complaintOrderId", "官方原文拼写");

        // 评价管理：列表 / 详情 / 评论回复三层字段。
        AssertJsonProperty<WxaCommentListItem>(nameof(WxaCommentListItem.WxPayId), "wxPayId", "官方原文拼写（非 wxpay_id）");
        AssertJsonProperty<WxaCommentListItem>(nameof(WxaCommentListItem.ExtInfo), "extInfo", "官方原文拼写");
        AssertJsonProperty<WxaCommentExtInfo>(nameof(WxaCommentExtInfo.IsAlreadySendTmpl), "isAlreadySendTmpl", "官方原文拼写（和解前置判据）");
        AssertJsonProperty<WxaCommentContent>(nameof(WxaCommentContent.Txt), "txt", "官方原文拼写（评价文本字段官方即 txt）");
        AssertJsonProperty<WxaCommentOldContent>(nameof(WxaCommentOldContent.Ext), "ext", "官方原文拼写（旧评价文本字段为 ext 而非 txt）");
        AssertJsonProperty<WxaCommentReplyObject>(nameof(WxaCommentReplyObject.Nickname), "nickname", "官方原文小写拼写（与 userInfo 侧 nickName 不同源，不得统一）");
        AssertJsonProperty<WxaCommentReplyObject>(nameof(WxaCommentReplyObject.ImgUrl), "imgUrl", "官方原文拼写");
        AssertJsonProperty<WxaCommentCompromiseRequest>(nameof(WxaCommentCompromiseRequest.PicList), "picList", "官方原文拼写（和解图片为 mediaId 集合）");
        AssertJsonProperty<WxaCommentReplyDeleteRequest>(nameof(WxaCommentReplyDeleteRequest.CommentReplyId), "commentReplyId", "官方原文拼写（回复层定位键）");

        // 投诉管理。
        AssertJsonProperty<WxaComplaintOrder>(nameof(WxaComplaintOrder.OutTradeNo), "outTradeNo", "官方原文拼写");
        AssertJsonProperty<WxaComplaintOrder>(nameof(WxaComplaintOrder.AppealState), "appealState", "官方原文拼写");
        AssertJsonProperty<WxaComplaintProgressItem>(nameof(WxaComplaintProgressItem.BlameResult), "blameResult", "官方原文拼写（决定走退货还是补充凭证）");
        AssertJsonProperty<WxaComplaintReturnBill>(nameof(WxaComplaintReturnBill.WaybillId), "waybillId", "官方原文拼写");
        AssertJsonProperty<WxaComplaintRespondRequest>(nameof(WxaComplaintRespondRequest.BussiHandle), "bussiHandle", "官方原文拼写（bussi 与路由同源，非 busi）");
        AssertJsonProperty<WxaComplaintRefundProofRequest>(nameof(WxaComplaintRefundProofRequest.AcceptReturn), "acceptReturn", "官方原文拼写");

        // 类型口径锁定（官方参数表逐页口径，含两处「同名字段跨页类型不同」的照录）。
        typeof(WxaCommentListItem).GetProperty(nameof(WxaCommentListItem.Amount))!.PropertyType
            .Should().Be(typeof(long?), "评价列表 amount 官方参数表为 number");
        typeof(WxaCommentDetail).GetProperty(nameof(WxaCommentDetail.Amount))!.PropertyType
            .Should().Be(typeof(string), "评价详情 info.content.amount 官方参数表为 string（与列表侧口径不同，照录不统一）");
        typeof(WxaComplaintOrder).GetProperty("ReturnId", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("投诉单主体官方参数表不含 returnId（退货单号只出现在 returnBill 与退款凭证请求体）");
        typeof(WxaComplaintOrder).GetProperty(nameof(WxaComplaintOrder.TotalCost))!.PropertyType
            .Should().Be(typeof(string), "totalCost 官方参数表为 string");
        typeof(WxaComplaintDetailResponse).GetProperty(nameof(WxaComplaintDetailResponse.Item))!.PropertyType
            .Should().Be(typeof(WxaComplaintProgressItem), "官方把投诉进度建为单数对象 item，不是数组");
        typeof(WxaComplaintReturnBill).GetProperty(nameof(WxaComplaintReturnBill.ReturnId))!.PropertyType
            .Should().Be(typeof(string), "详情侧 returnBill.returnId 官方参数表为 string");
        typeof(WxaComplaintRefundProofRequest).GetProperty(nameof(WxaComplaintRefundProofRequest.ReturnId))!.PropertyType
            .Should().Be(typeof(long?), "退款凭证请求体 returnId 官方参数表为 number（与详情侧口径不同，各自照录）");

        // 共用 DTO 裁决 1：补充凭证与申诉的官方请求体三字段完全一致 ⇒ 同一类型。
        typeof(IWxaTradeComplaintService).GetMethod(nameof(IWxaTradeComplaintService.SupplyProofAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaComplaintProofRequest));
        typeof(IWxaTradeComplaintService).GetMethod(nameof(IWxaTradeComplaintService.AppealAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaComplaintProofRequest));

        // 共用 DTO 裁决 2：评价管理 6 个写端点应答字段集完全一致 ⇒ 共用 WxaCommentOpResponse。
        var writeEndpoints = Routes
            .Where(static r => r.HttpAttribute == typeof(PostAttribute)
                               && r.Interface == typeof(IWxaTradeCommentService))
            .ToList();
        writeEndpoints.Should().HaveCount(6, "评价管理写端点为 6（评论 2 + 回复 2 + 确认和解 + 重置 quota）");
        foreach (var (iface, method, _, route) in writeEndpoints)
        {
            iface.GetMethod(method)!.ReturnType
                .Should().Be(typeof(Task<WxaCommentOpResponse>),
                    $"{route} 官方应答字段集（errcode / success / errmsg）与其余写端点一致 ⇒ 共用响应类型");
        }

        // 历史/边界锁定：官方未公布的枚举不得凭空建常量类。
        typeof(WxaComplaintRefundProofRequest).GetProperty(nameof(WxaComplaintRefundProofRequest.AcceptReturn))!
            .PropertyType.Should().Be(typeof(long?),
                "acceptReturn 官方无枚举表 ⇒ 只保留 number 口径，不建常量类（凭空造 0/1 即违反照录原则）");

        // 应答基类：本域全部响应继承 WxaResponse（errcode 判错与全线同源）。
        var responseDtos = typeof(WxaTradeGuaranteeStatusResponse).Assembly.GetTypes()
            .Where(static t => !t.IsNested
                               && t.Namespace == DtoNamespace
                               && t.IsClass
                               && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();

        responseDtos.Should().HaveCount(7, "本域官方应答对象为 7（basic 2 + comment 4 + complaint 1）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "交易保障域响应 DTO 必须继承 WxaResponse（判错走单点判定）");

        // 官方枚举常量类取值锁定。
        typeof(WxaCommentFilterType).GetFields()
            .Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5, 6 },
                because: "filterType 官方枚举恰 6 值（全部差评/全部好评/差评待处理/开发者待回复/差评已改评/全部评价）");
        typeof(WxaComplaintBussiHandle).GetFields()
            .Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 1, 2 },
                because: "bussiHandle 官方只公布 1 同意和解 / 2 拒绝和解");
    }

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }
}
