// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Locallife;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「本地生活-团购券」域契约守卫（CLL 系列：Locallife 域 6 端点路由 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量 + 与电商优惠券的概念辨析 + 官方历史拼写照抄）。
/// </summary>
/// <remarks>
/// <para>
/// <b>概念辨析（守卫措辞锁定）</b>：本域是本地生活<b>团购券</b>（<c>/channels/ec/voucher/*</c>，
/// 到店核销凭证：code 核销 + 分账结算），<b>不是</b>电商<b>优惠券</b>
/// （<see cref="ChannelsMarketingContractGuards"/>，<c>/channels/ec/coupon/*</c>）。
/// 路由面由 CH-R2 锁定，两域前缀天然无交叠。
/// </para>
/// <para>
/// <b>官方历史拼写照抄（不得「顺手修正」）</b>：撤销核销请求体的 <c>reovke_vouchers</c>
/// 为官方参数表原文拼写（revoke 的官方笔误），字段照抄原文——修正即破坏线上契约。
/// </para>
/// <para>
/// <b>限频</b>：查询 / 核销 / 撤销 / 账单类接口官方限频「单商户 200QPS」（官方注意事项原文）。
/// </para>
/// </remarks>
public class ChannelsLocallifeContractGuards
{
    private const string LocallifeRegistryGroupName = "Locallife";

    /// <summary>Locallife 域官方路由表（6 端点：/channels/ec/voucher/*，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] LocallifeRoutes =
    {
        (typeof(IChannelsLocallifeService), nameof(IChannelsLocallifeService.UploadVoucherCodesAsync), typeof(PostAttribute), "/channels/ec/voucher/codes/upload"),
        (typeof(IChannelsLocallifeService), nameof(IChannelsLocallifeService.GetVoucherAsync), typeof(PostAttribute), "/channels/ec/voucher/get"),
        (typeof(IChannelsLocallifeService), nameof(IChannelsLocallifeService.GetVoucherListAsync), typeof(PostAttribute), "/channels/ec/voucher/get_list"),
        (typeof(IChannelsLocallifeService), nameof(IChannelsLocallifeService.ConsumeVoucherAsync), typeof(PostAttribute), "/channels/ec/voucher/consume"),
        (typeof(IChannelsLocallifeService), nameof(IChannelsLocallifeService.RevokeVoucherAsync), typeof(PostAttribute), "/channels/ec/voucher/revoke"),
        (typeof(IChannelsLocallifeService), nameof(IChannelsLocallifeService.GetVoucherBillListAsync), typeof(PostAttribute), "/channels/ec/voucher/get_bill_list"),
    };

    /// <summary>契约守卫 CLL1：Locallife 域 6 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void LocallifeEndpoints_ShouldMatchOfficialRoutes()
    {
        // 6 端点 = code 预存 1 + 用户查询 2 + 核销 1 + 撤销核销 1 + 账单 1。
        LocallifeRoutes.Should().HaveCount(6, "本地生活团购券域官方开放面为上传code/用户券详情/用户券列表/核销/撤销核销/券账单 6 端点");
        LocallifeRoutes.Select(r => r.Route).Distinct().Should().HaveCount(6);

        LocallifeRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/voucher/", StringComparison.Ordinal))
            .Should().Be(6, "6 端点全部走 /channels/ec/voucher/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in LocallifeRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CLL2：Locallife 域 6 端点全部为 POST 且均携带 JSON [Body]（官方无空请求体端点）。</summary>
    [Fact]
    public void LocallifeEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in LocallifeRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST（团购券域无 GET 端点）");

            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().ContainSingle($"{iface.Name}.{method} 官方参数表均含请求体字段，必须声明 [Body]");
        }

        // 撤销核销请求体字段名照抄官方历史拼写 reovke_vouchers（不得修正为 revoke_vouchers）。
        AssertJsonProperty<ChannelsRevokeVoucherRequest>("reovke_vouchers", "官方参数表历史拼写照抄");
        typeof(ChannelsRevokeVoucherRequest).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "revoke_vouchers")
            .Should().BeFalse("禁止把官方 reovke_vouchers '纠正' 为 revoke_vouchers——字段名照抄官方原文");
    }

    /// <summary>契约守卫 CLL3：令牌绑定与注册形态。</summary>
    [Fact]
    public void LocallifeInterface_ShouldBindChannelsToken_AndRegisterUnderLocallifeGroup()
    {
        var token = typeof(IChannelsLocallifeService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Locallife 域 6 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsLocallifeService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(LocallifeRegistryGroupName, "必须挂 Locallife 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Locallife");
        typeof(ChannelsServiceBuilder).GetMethod("AddLocallifeApi").Should().NotBeNull("必须提供 AddLocallifeApi 链式注册方法");
    }

    /// <summary>契约守卫 CLL4：Locallife 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void LocallifeDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = LocallifeJsonContext.Default;

        var domainTypes = typeof(ChannelsVoucher).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Locallife"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(17,
            "Locallife 域 6 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（上传code 3 + 券详情 2 + 券列表 2 + 核销 2 + 撤销核销 3 + 失败明细共用 1 + 券对象共用 1 + 账单 3 = 17）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Locallife 域命名空间，必须登记进 LocallifeJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(LocallifeRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Locallife");
        }

        // 常量类（静态常量类不参与序列化）：Locallife 域 3 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(3,
            "Locallife 域常量类 = 券状态 + 核销渠道 + 退款账户");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CLL5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void LocallifeDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 上传团购优惠 code。
        AssertJsonProperty<ChannelsVoucherCodesUploadRequest>("product_id", "商品 id");
        AssertJsonProperty<ChannelsVoucherCodesUploadRequest>("sku_id", "sku id");
        AssertJsonProperty<ChannelsVoucherCodesUploadRequest>("codes", "商户上传的券 code 列表");
        AssertJsonProperty<ChannelsVoucherCodesUploadResponse>("total_count", "去重后的 code 总数");
        AssertJsonProperty<ChannelsVoucherCodesUploadResponse>("success_count", "上传成功个数");
        AssertJsonProperty<ChannelsVoucherCodesUploadResponse>("success_codes", "上传成功的 code 列表");
        AssertJsonProperty<ChannelsVoucherCodesUploadResponse>("fail_codes", "导入失败的 code 信息");
        AssertJsonProperty<ChannelsVoucherCodeFailItem>("errcode", "单个 code 上传失败的错误码");

        // 查询用户券详情 / 列表。
        AssertJsonProperty<ChannelsGetVoucherRequest>("encrypted_code", "加密 code（官方说明原文写作 encrypted_data，字段名按参数表）");
        AssertJsonProperty<ChannelsGetVoucherRequest>("code", "用户券 code");
        AssertJsonProperty<ChannelsGetVoucherResponse>("voucher", "用户券详情");
        AssertJsonProperty<ChannelsVoucher>("status", "券状态");
        AssertJsonProperty<ChannelsVoucher>("send_time", "发放时间");
        AssertJsonProperty<ChannelsVoucher>("telphone_no", "用户手机号（官方示例字段，历史拼写照抄）");
        AssertJsonProperty<ChannelsGetVoucherListRequest>("openid", "用户 openid");
        AssertJsonProperty<ChannelsGetVoucherListRequest>("page_size", "页大小（1≤value≤50）");
        AssertJsonProperty<ChannelsGetVoucherListRequest>("page_ctx", "翻页上下文");
        AssertJsonProperty<ChannelsGetVoucherListResponse>("voucher_list", "用户券列表");

        // 核销 / 撤销核销。
        AssertJsonProperty<ChannelsConsumeVoucherRequest>("consume_request_no", "核销幂等串");
        AssertJsonProperty<ChannelsConsumeVoucherRequest>("encrypted_codes", "加密 code 列表");
        AssertJsonProperty<ChannelsConsumeVoucherRequest>("consume_channel", "核销渠道");
        AssertJsonProperty<ChannelsConsumeVoucherRequest>("consume_store_name", "核销门店名称（参数表字段名，示例写作 store_name——矛盾照录）");
        AssertJsonProperty<ChannelsConsumeVoucherRequest>("out_store_id", "商家自定义门店 id");
        AssertJsonProperty<ChannelsConsumeVoucherRequest>("reserve_no", "预约单号");
        AssertJsonProperty<ChannelsConsumeVoucherResponse>("fail_voucher_list", "无法操作的券及原因");
        AssertJsonProperty<ChannelsRevokeVoucherRequest>("revoke_request_no", "撤销幂等串");
        AssertJsonProperty<ChannelsRevokeVoucherItem>("cancel_count", "撤销张数（官方已废弃，照录）");
        AssertJsonProperty<ChannelsVoucherFailItem>("ret_code", "操作结果");

        // 券账单。
        AssertJsonProperty<ChannelsGetVoucherBillListRequest>("bill_date", "账单日期（券发起核销时间）");
        AssertJsonProperty<ChannelsGetVoucherBillListResponse>("bill_list", "券账单列表");
        AssertJsonProperty<ChannelsVoucherBill>("voucher_buy_amount", "券售卖金额（分）");
        AssertJsonProperty<ChannelsVoucherBill>("voucher_pay_amount", "用户支付金额（分）");
        AssertJsonProperty<ChannelsVoucherBill>("mch_favor_amount", "商家优惠减价金额（分）");
        AssertJsonProperty<ChannelsVoucherBill>("platform_favor_amount", "平台优惠减价金额（分）");
        AssertJsonProperty<ChannelsVoucherBill>("mch_settle_amount", "给商家的已结算金额（分）");
        AssertJsonProperty<ChannelsVoucherBill>("finder_settle_amount", "给达人的结算金额（分）");
        AssertJsonProperty<ChannelsVoucherBill>("refund_account", "退款账户");
        AssertJsonProperty<ChannelsVoucherBill>("commission_fee", "平台技术服务费");
        AssertJsonProperty<ChannelsVoucherBill>("time_index", "次序号标识（次卡）");

        // 共用 DTO 裁决 1：getvoucher 响应 voucher / findvoucher voucher_list 元素 / consumevoucher 与
        // withdrawconsume 响应 voucher_list 元素，四端点官方示例字段集一致 ⇒ 共用 ChannelsVoucher。
        typeof(ChannelsGetVoucherResponse).GetProperty(nameof(ChannelsGetVoucherResponse.Voucher))!.PropertyType
            .Should().Be(typeof(ChannelsVoucher));
        typeof(ChannelsGetVoucherListResponse).GetProperty(nameof(ChannelsGetVoucherListResponse.VoucherList))!.PropertyType
            .Should().Be(typeof(List<ChannelsVoucher>));
        typeof(ChannelsConsumeVoucherResponse).GetProperty(nameof(ChannelsConsumeVoucherResponse.VoucherList))!.PropertyType
            .Should().Be(typeof(List<ChannelsVoucher>));
        typeof(ChannelsRevokeVoucherResponse).GetProperty(nameof(ChannelsRevokeVoucherResponse.VoucherList))!.PropertyType
            .Should().Be(typeof(List<ChannelsVoucher>));

        // 共用 DTO 裁决 2：consumevoucher 与 withdrawconsume 响应 fail_voucher_list 元素字段集一致 ⇒ 共用 ChannelsVoucherFailItem。
        typeof(ChannelsConsumeVoucherResponse).GetProperty(nameof(ChannelsConsumeVoucherResponse.FailVoucherList))!.PropertyType
            .Should().Be(typeof(List<ChannelsVoucherFailItem>));
        typeof(ChannelsRevokeVoucherResponse).GetProperty(nameof(ChannelsRevokeVoucherResponse.FailVoucherList))!.PropertyType
            .Should().Be(typeof(List<ChannelsVoucherFailItem>));

        // 分建裁决：上传 code 的失败明细（fail_codes：code/errcode/errmsg）与核销失败明细
        // （fail_voucher_list：code/sku_id/encrypted_code/ret_code/ret_msg）字段集不同 ⇒ 不共用。
        typeof(ChannelsVoucherCodeFailItem).GetProperty(nameof(ChannelsVoucherCodeFailItem.ErrCode))
            .Should().NotBeNull("上传 code 失败明细走 errcode/errmsg（官方 uploadvouchercode 参数表）");
        typeof(ChannelsVoucherFailItem).GetProperty(nameof(ChannelsVoucherFailItem.RetCode))
            .Should().NotBeNull("核销失败明细走 ret_code/ret_msg（官方 consumevoucher 参数表）");
    }

    /// <summary>契约守卫 CLL6：枚举常量字面量锁定。</summary>
    [Fact]
    public void LocallifeEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsVoucherStatuses.Valid.Should().Be(1);
        ChannelsVoucherStatuses.Used.Should().Be(2);
        ChannelsVoucherStatuses.Refund.Should().Be(3);
        ChannelsVoucherStatuses.Expired.Should().Be(4);
        ChannelsVoucherStatuses.Reserved.Should().Be(5);

        ChannelsVoucherConsumeChannels.MiniApp.Should().Be(1);
        ChannelsVoucherConsumeChannels.InStore.Should().Be(2);

        ChannelsVoucherRefundAccounts.Withdrawable.Should().Be(2000);
        ChannelsVoucherRefundAccounts.Deposit.Should().Be(3000);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
