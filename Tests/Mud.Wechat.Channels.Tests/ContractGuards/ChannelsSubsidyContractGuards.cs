// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Subsidy;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「国补管理-发票」域契约守卫（CSU 系列：Subsidy 域 3 端点路由 + 令牌绑定 +
/// DTO 字段与分建裁决 + JSON 上下文登记 + 枚举常量 + 官方矛盾照录）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方调用限制（三端点一致，守卫措辞锁定）</b>：①<b>不支持云调用、不支持第三方平台代调用</b>
/// （官方文档原文，故本域无权限集 id——与 Funds 的权限集 138 / Marketing 的 132 / Locallife 的
/// 129+164 不同，不得照抄他域措辞）；②订单必须是使用了国补的订单 id；③只能在订单确认收货之后调用；
/// ④同一笔订单最多可以上传 5 次发票信息。
/// </para>
/// <para>
/// <b>官方矛盾照录</b>：query_invoicing_info 官方示例把开票总金额写作 <c>invoice_tatal_amount</c>
/// （拼写瑕疵）——按参数表 <c>invoice_total_amount</c> 建模；upload_invoice_info 的 <c>invoice_date</c>
/// 类型列标注 string、说明与示例为数字时间戳——按数字时间戳建模。
/// </para>
/// </remarks>
public class ChannelsSubsidyContractGuards
{
    private const string SubsidyRegistryGroupName = "Subsidy";

    /// <summary>Subsidy 域官方路由表（3 端点：/channels/ec/subsidy/*，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] SubsidyRoutes =
    {
        (typeof(IChannelsSubsidyService), nameof(IChannelsSubsidyService.QueryInvoicingInfoAsync), typeof(PostAttribute), "/channels/ec/subsidy/query_invoicing_info"),
        (typeof(IChannelsSubsidyService), nameof(IChannelsSubsidyService.UploadInvoiceInfoAsync), typeof(PostAttribute), "/channels/ec/subsidy/upload_invoice_info"),
        (typeof(IChannelsSubsidyService), nameof(IChannelsSubsidyService.UploadInvoiceFileAsync), typeof(PostAttribute), "/channels/ec/subsidy/upload_invoice_file"),
    };

    /// <summary>契约守卫 CSU1：Subsidy 域 3 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void SubsidyEndpoints_ShouldMatchOfficialRoutes()
    {
        // 3 端点 = 开票信息查询 1 + 发票信息上传 1 + 发票文件上传 1。
        SubsidyRoutes.Should().HaveCount(3, "国补发票域官方开放面为开票信息查询/发票信息上传/发票文件上传 3 端点");
        SubsidyRoutes.Select(r => r.Route).Distinct().Should().HaveCount(3);

        SubsidyRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/subsidy/", StringComparison.Ordinal))
            .Should().Be(3, "3 端点全部走 /channels/ec/subsidy/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in SubsidyRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CSU2：Subsidy 域 3 端点全部为 POST 且均携带 JSON [Body]（官方无空请求体端点）。</summary>
    [Fact]
    public void SubsidyEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in SubsidyRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST（国补发票域无 GET 端点）");

            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().ContainSingle($"{iface.Name}.{method} 官方参数表均含请求体字段，必须声明 [Body]");
        }
    }

    /// <summary>契约守卫 CSU3：令牌绑定与注册形态。</summary>
    [Fact]
    public void SubsidyInterface_ShouldBindChannelsToken_AndRegisterUnderSubsidyGroup()
    {
        var token = typeof(IChannelsSubsidyService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Subsidy 域 3 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsSubsidyService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(SubsidyRegistryGroupName, "必须挂 Subsidy 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Subsidy");
        typeof(ChannelsServiceBuilder).GetMethod("AddSubsidyApi").Should().NotBeNull("必须提供 AddSubsidyApi 链式注册方法");
    }

    /// <summary>契约守卫 CSU4：Subsidy 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void SubsidyDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = SubsidyJsonContext.Default;

        var domainTypes = typeof(ChannelsSubsidyQueryInvoicingRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Subsidy"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(7,
            "Subsidy 域 3 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（查询请求 1 + 查询响应 1 + 开票信息 1 + 发票信息上传请求 1 + 发票文件上传请求 1 + 文件上传响应 1 + media_id 回包 1 = 7；" +
            "upload_invoice_info 响应仅 errcode/errmsg 故直接复用基线 ChannelsResponse 不建 DTO）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Subsidy 域命名空间，必须登记进 SubsidyJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(SubsidyRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Subsidy");
        }

        // 常量类（静态常量类不参与序列化）：Subsidy 域 1 个，不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().ContainSingle(
            "Subsidy 域常量类 = 发票类型（invoice_type）");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CSU5：官方字段名与分建裁决锁定（抽样关键端点 + 矛盾照录）。</summary>
    [Fact]
    public void SubsidyDataModels_ShouldLockOfficialFieldsAndVerdicts()
    {
        // 查询开票信息。
        AssertJsonProperty<ChannelsSubsidyQueryInvoicingRequest>("order_id", "订单 id（必须是国补订单）");
        AssertJsonProperty<ChannelsSubsidyQueryInvoicingResponse>("data", "回包信息");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("name", "发票抬头（订单归属人姓名）");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("tax_no", "发票税号（订单归属人身份证号）");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("payment_amount", "用户实付金额");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("subsidy_amount", "国家补贴金额");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("invoice_total_amount", "开票总金额（官方示例拼写 invoice_tatal_amount——矛盾照录）");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("bar_code", "69 码");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("sn_code", "sn 码");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("imei1", "imei 码");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("is_invoice_uploaded", "是否已上传发票信息");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("efficiency", "能效/水效等级");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("goods_model", "商品型号（广东银联来源）");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("buyer_phone", "购买人电话");
        AssertJsonProperty<ChannelsSubsidyInvoicingInfo>("give_profit_amount", "商家让利金额");

        // 上传发票信息。
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_code", "发票代码（类型 90 可选填）");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_no", "发票号码");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_date", "开票时间（说明与示例为秒级时间戳，类型列 string——矛盾照录）");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_media_id", "发票文件 media_id");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_type", "发票类型");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_check_code", "发票校验码");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_total_amount", "税价合计（分）");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_tax_amount", "发票税额（分）");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("sales_enterprise_name", "销售企业名称");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("tax_payer", "销售方纳税人识别号");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("service_name", "货物或应税劳务服务名");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceInfoRequest>("invoice_goods_model", "规格型号");

        // 上传发票文件。
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceFileRequest>("file_link", "文件链接（公网可访问）");
        AssertJsonProperty<ChannelsSubsidyUploadInvoiceFileResponse>("data", "回包信息");
        AssertJsonProperty<ChannelsSubsidyInvoiceMediaData>("media_id", "发票文件 media_id");

        // 分建裁决：本域无跨端点共用 DTO——三端点请求/响应字段集互不相同；
        // upload_invoice_info 响应仅 errcode/errmsg ⇒ 直接复用基线 ChannelsResponse（不建 DTO）。
        typeof(IChannelsSubsidyService).GetMethod(nameof(IChannelsSubsidyService.UploadInvoiceInfoAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsResponse>),
                "upload_invoice_info 官方响应仅 errcode/errmsg——返回基线 ChannelsResponse");

        // 响应侧 invoice_total_amount 按参数表字段名建模（不得跟随官方示例拼写 invoice_tatal_amount）。
        typeof(ChannelsSubsidyInvoicingInfo).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "invoice_tatal_amount")
            .Should().BeFalse("禁止照抄官方示例的拼写瑕疵 invoice_tatal_amount——字段名以参数表为权威");
    }

    /// <summary>契约守卫 CSU6：枚举常量字面量锁定。</summary>
    [Fact]
    public void SubsidyEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsSubsidyInvoiceTypes.VatSpecial.Should().Be("01");
        ChannelsSubsidyInvoiceTypes.PaperGeneral.Should().Be("04");
        ChannelsSubsidyInvoiceTypes.DigitalSpecial.Should().Be("09");
        ChannelsSubsidyInvoiceTypes.ElectronicGeneral.Should().Be("10");
        ChannelsSubsidyInvoiceTypes.DigitalGeneral.Should().Be("90");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
