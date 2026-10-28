// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.MerchantStore;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-7 商户门店管理域契约守卫（<b>5 端点</b>：创建 + 更新 + 查询 + 绑定收款号 + 解绑收款号）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（2026-10-14 十页逐字段核验，商户面与服务商面成对）：<c>4014085766</c> / <c>4013948546</c> 创建、
/// <c>4014085768</c> / <c>4013948551</c> 更新、<c>4014085769</c> / <c>4013948577</c> 查询、
/// <c>4014085771</c> / <c>4013948579</c> 绑定收款商户号、<c>4014085773</c> / <c>4013948580</c> 解绑收款商户号。
/// 逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>「十个文档页 ⇒ 五个端点」是与同批风险域相反的归并裁决</b>：本域商户面与服务商面<b>路由逐字符相同</b>，
/// 字段表只差一个 <c>sub_mchid</c>（查询面在 query、其余四面在包体），应答表零差异
/// ⇒ 判据是「官方字段表是否相同」，<b>不是页面数量</b>。 MST-B6 因此锁「两支共用同一类型」
/// 且「本域不存在 <c>Partner</c> 前缀分身」。同批的
/// <see cref="WechatPayMerchantRiskManageContractGuards"/> 走的是相反裁决（字段表不同 ⇒ 同路由分建两形态），
/// 两条守卫并置即是本程序「按字段表归并」口径的可执行说明。
/// </para>
/// <para>
/// <b>本守卫要守住的四处「顺手就会写错」</b>：
/// ① 更新支是 <b>PATCH 且 204 无包体</b>，而同域的创建 / 查询 / 绑定 / 解绑四支都是 200 + JSON
/// ——「写操作都 204」与「读操作都 200」两条直觉在本域各错一半；
/// ② 更新请求<b>没有</b> <c>store_recipient</c>（收款号增删走独立两支）⇒ 照抄创建请求即多出官方不接受的字段；
/// ③ 经纬度官方是 <b>string(32)</b> 而非数值，<c>store_id</c> 官方是 integer 但走 path；
/// ④ <b>写支成功 ≠ 生效</b>：本域是三方确认流（收款号超管确认 + 地址核实），
/// 结论只在查询应答的 <c>store_state</c> 里 ⇒ 状态机 6 值中 3 值是「等待他方动作」。
/// </para>
/// <para>
/// <b>本章不含「分页查门店」与「删除门店」</b>：官方十页内无这两支 HTTP 面
/// （<c>STORE_STATE_CLOSED</c> 的删除动作官方写明是「登录商户平台删除」）⇒ MST-B2 带正对照锁零声明，
/// 正对照特意取同批落地的风险域 <b>DELETE 动词</b>：它证明「动词扫描机制」本身在生效，
/// 本域零 DELETE 不是扫描失效。
/// </para>
/// </remarks>
public class WechatPayMerchantStoreContractGuards
{
    private const string RegistryGroupName = "MerchantStore";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MerchantStore";

    private const string Prefix = "/v3/merchant-store/";

    private const string StoresRoute = Prefix + "stores";

    private const string StoreByIdRoute = StoresRoute + "/{storeId}";

    /// <summary>
    /// MST-B1：5 端点的方法与路由照官方原文；更新支<b>必须</b>返回裸 <see cref="Task"/>（204），
    /// 另四支各有应答类型；path 恒为 <c>storeId</c>，query 只在查询支出现 <c>sub_mchid</c>。
    /// </summary>
    [Fact]
    public void MerchantStoreEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentForUpdate()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantStoreService.CreateStoreAsync), StoresRoute);
        AssertRoute<PatchAttribute>(nameof(IWechatPayMerchantStoreService.UpdateStoreAsync), StoreByIdRoute);
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantStoreService.QueryStoreAsync), StoreByIdRoute);
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantStoreService.BindStoreRecipientsAsync),
            StoreByIdRoute + "/recipients/bind");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantStoreService.UnbindStoreRecipientsAsync),
            StoreByIdRoute + "/recipients/unbind");

        var routes = RoutesOf(typeof(IWechatPayMerchantStoreService));
        routes.Should().HaveCount(5, "本域 5 端点（十页归并而来，见 MST-B6）；数量变化须同批更新 MST-B1 与 Tests/ContractBaseline.cs");
        // 计数双锁（与 MRB-B1 同形，方向相反）：去重后的 (方法, 路由) 是 5，但<b>路由字符串</b>只有 4 条 ——
        // PATCH（更新）与 GET（查询）共用 /stores/{store_id}。只锁其中一个数就会在
        // 「有人把两支合并成一支」或「有人再拆出第五条同路径路由」时静默通过。
        DistinctVerbRoutes().Should().HaveCount(5, "五条 (方法, 路由) 各不相同：本域没有同方法同路由的多形态分身");
        routes.Distinct().Should().HaveCount(4,
            "PATCH 与 GET 共用同一条 /stores/{store_id}：按路由字符串去重后只有 4 条（不是 5）");

        // 204 面：本域恰一支无应答包体（更新），另四支有类型。
        NoContentMethodsOf(typeof(IWechatPayMerchantStoreService))
            .Should().BeEquivalentTo(new[] { nameof(IWechatPayMerchantStoreService.UpdateStoreAsync) },
            "官方仅「更新品牌门店」成功应答为 204：把创建/查询写成裸 Task 即丢掉 store_id 与 store_state");
        FindMethod(nameof(IWechatPayMerchantStoreService.CreateStoreAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantStoreCreateResponse>));
        FindMethod(nameof(IWechatPayMerchantStoreService.QueryStoreAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantStoreQueryResponse>));

        // path 形态：除创建支外，四支都以 store_id 定位（包体里不得再塞 store_id）。
        PathKeysOf(nameof(IWechatPayMerchantStoreService.CreateStoreAsync)).Should().BeEmpty();
        foreach (var methodName in new[]
                 {
                     nameof(IWechatPayMerchantStoreService.UpdateStoreAsync),
                     nameof(IWechatPayMerchantStoreService.QueryStoreAsync),
                     nameof(IWechatPayMerchantStoreService.BindStoreRecipientsAsync),
                     nameof(IWechatPayMerchantStoreService.UnbindStoreRecipientsAsync),
                 })
        {
            PathKeysOf(methodName).Should().BeEquivalentTo(new[] { "storeId" },
                $"{methodName}：门店 ID 只在 path，包体再塞一份即造出双写入口");
        }

        // query 形态：只有查询支带 sub_mchid（服务商面的唯一差异点，且不在包体）。
        QueryNamesOf(nameof(IWechatPayMerchantStoreService.QueryStoreAsync))
            .Should().BeEquivalentTo(new[] { "sub_mchid" },
            "服务商面与商户面的请求差异只有这一个 query 参数");
        QueryNamesOf(nameof(IWechatPayMerchantStoreService.CreateStoreAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantStoreService.UpdateStoreAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantStoreService.BindStoreRecipientsAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantStoreService.UnbindStoreRecipientsAsync)).Should().BeEmpty();

        // 包体形态：PATCH 有包体（三棵子树），GET 无包体。
        BodyTypeOf(nameof(IWechatPayMerchantStoreService.UpdateStoreAsync))
            .Should().Be(typeof(MerchantStoreUpdateRequest));
        BodyTypeOf(nameof(IWechatPayMerchantStoreService.CreateStoreAsync))
            .Should().Be(typeof(MerchantStoreCreateRequest));
        FindMethod(nameof(IWechatPayMerchantStoreService.QueryStoreAsync)).GetParameters()
            .Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
            "GET 不得带请求体");

        // 两支 POST 写操作走「动词段」而不是 PATCH（与直觉相反，照页）。
        FindMethod(nameof(IWechatPayMerchantStoreService.BindStoreRecipientsAsync))
            .GetCustomAttribute<PostAttribute>()!.RequestUri.Should().EndWith("/recipients/bind");
    }

    /// <summary>
    /// MST-B2：路由全在 <c>/v3/merchant-store/</c> 前缀下 + 两处「官方无此 HTTP 面」零声明
    /// （分页查门店列表 / 删除门店），正对照取同批风险域的 DELETE 动词以证扫描机制生效。
    /// </summary>
    [Fact]
    public void MerchantStoreRoutes_ShouldStayOnOwnPrefixAndNotDeclareListOrDeleteFaces()
    {
        var routes = RoutesOf(typeof(IWechatPayMerchantStoreService));

        routes.Should().HaveCount(5);
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(5, "本域五支全在 /v3/merchant-store/ 下，无第二前缀");

        // 零声明①：官方无「分页查门店列表」⇒ 除创建支外每条路由都带 {storeId}。
        routes.Where(static r => r != StoresRoute)
            .Should().OnlyContain(static r => r.Contains("{storeId}", StringComparison.Ordinal),
            "本域没有列表端点：任何不带门店 ID 的路由都是臆造的");
        // 亦无列表分页参数（本域 query 只有 sub_mchid）。
        QueryNamesOf(nameof(IWechatPayMerchantStoreService.QueryStoreAsync))
            .Should().NotContain(new[] { "offset", "limit", "page", "page_size" },
            "官方门店面无分页参数：从别域照抄游标即造出官方忽略的查询条件");

        // 零声明②：官方无「删除门店」的 HTTP 面（CLOSED 态由商户平台删除，不可恢复）。
        FindMethod(nameof(IWechatPayMerchantStoreService.UpdateStoreAsync))
            .GetCustomAttribute<HttpMethodAttribute>()
            .Should().BeAssignableTo<PatchAttribute>();
        typeof(IWechatPayMerchantStoreService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().NotContain(static a => a is DeleteAttribute,
            "官方十页内无删除门店端点：做成 API 即与「商户平台删除、不可恢复」的官方口径冲突");
        // 正对照：同批落地的风险域确有 DELETE 动词 ⇒ 上面那条零声明不是「扫描把 DELETE 整体漏掉」。
        typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().Contain(static a => a is DeleteAttribute,
            "正对照：DELETE 动词在本仓可被基类扫描捕获，门店域的零 DELETE 是真裁决");

        // 零声明③：门店面不含风控面路由（两域同批落地，交集必须为空）。
        routes.Intersect(RoutesOf(typeof(IWechatPayMerchantRiskManageService)))
            .Should().BeEmpty("门店面与风险面是两个域，路由交集非空即说明有人把端点挂错了域");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", RegistryGroupName))
            .Should().BeTrue("正对照：Interfaces/MerchantStore/ 确实已落位");
    }

    /// <summary>
    /// MST-B3：官方字段名逐项锁定 + <b>三棵 vs 四棵子树之差</b>（更新请求无 <c>store_recipient</c>）
    /// + 经纬度恒 <see cref="string"/>。
    /// </summary>
    [Fact]
    public void MerchantStoreDtos_ShouldExposeOfficialFieldNamesAndSubtreeDifference()
    {
        // 四棵子树的容器名 + 各自叶子（创建 / 查询 / 绑定三支共用同一批子树类型）。
        JsonNameShouldBe<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.StoreBasics), "store_basics");
        JsonNameShouldBe<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.StoreAddress), "store_address");
        JsonNameShouldBe<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.StoreBusiness), "store_business");
        JsonNameShouldBe<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.StoreRecipient), "store_recipient");
        JsonNameShouldBe<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.SubMchId), "sub_mchid");

        JsonNameShouldBe<MerchantStoreBasics>(nameof(MerchantStoreBasics.StoreReferenceId), "store_reference_id");
        JsonNameShouldBe<MerchantStoreBasics>(nameof(MerchantStoreBasics.BrandName), "brand_name");
        JsonNameShouldBe<MerchantStoreBasics>(nameof(MerchantStoreBasics.StoreName), "store_name");
        JsonNameShouldBe<MerchantStoreBasics>(nameof(MerchantStoreBasics.BranchName), "branch_name");
        JsonNameShouldBe<MerchantStoreBasics>(nameof(MerchantStoreBasics.BrandId), "brand_id");

        JsonNameShouldBe<MerchantStoreAddress>(nameof(MerchantStoreAddress.AddressCode), "address_code");
        JsonNameShouldBe<MerchantStoreAddress>(nameof(MerchantStoreAddress.AddressDetail), "address_detail");
        JsonNameShouldBe<MerchantStoreAddress>(nameof(MerchantStoreAddress.AddressComplements), "address_complements");
        JsonNameShouldBe<MerchantStoreBusinessInfo>(nameof(MerchantStoreBusinessInfo.ServicePhone), "service_phone");
        JsonNameShouldBe<MerchantStoreBusinessInfo>(nameof(MerchantStoreBusinessInfo.BusinessHours), "business_hours");

        JsonNameShouldBe<MerchantStoreRecipientInfo>(nameof(MerchantStoreRecipientInfo.MchId), "mchid");
        JsonNameShouldBe<MerchantStoreRecipientInfo>(nameof(MerchantStoreRecipientInfo.CompanyName), "company_name");

        // 应答侧独有的状态节点与失败项。
        JsonNameShouldBe<MerchantStoreQueryResponse>(nameof(MerchantStoreQueryResponse.StoreState), "store_state");
        JsonNameShouldBe<MerchantStoreState>(nameof(MerchantStoreState.StoreStateValue), "store_state_value");
        JsonNameShouldBe<MerchantStoreState>(nameof(MerchantStoreState.StoreStateMessage), "store_state_message");
        JsonNameShouldBe<MerchantStoreState>(nameof(MerchantStoreState.AddressFailedReason), "address_failed_reason");
        JsonNameShouldBe<MerchantStoreCreateResponse>(nameof(MerchantStoreCreateResponse.StoreId), "store_id");
        JsonNameShouldBe<MerchantStoreRecipientOperateResponse>(nameof(MerchantStoreRecipientOperateResponse.FailedStoreRecipient), "failed_store_recipient");
        JsonNameShouldBe<MerchantStoreFailedRecipient>(nameof(MerchantStoreFailedRecipient.MchId), "mchid");
        JsonNameShouldBe<MerchantStoreFailedRecipient>(nameof(MerchantStoreFailedRecipient.FailedReason), "failed_reason");

        // 三棵 vs 四棵：更新请求没有 store_recipient（收款号增删走 bind / unbind 两支）。
        DeclaredJsonNamesOf<MerchantStoreUpdateRequest>()
            .Should().BeEquivalentTo(new[] { "sub_mchid", "store_basics", "store_address", "store_business" },
            "更新支只有三棵子树：照抄创建请求即多出一个官方不接受的 store_recipient");
        DeclaredJsonNamesOf<MerchantStoreCreateRequest>()
            .Should().Contain("store_recipient");
        DeclaredJsonNamesOf<MerchantStoreQueryResponse>()
            .Should().BeEquivalentTo(new[] { "store_basics", "store_address", "store_business", "store_recipient", "store_state" },
            "查询应答 = 四棵子树 + store_state（请求侧没有 store_state）");

        // 失败项与请求项不是同一类型：第二列是 failed_reason 而非 company_name。
        DeclaredJsonNamesOf<MerchantStoreFailedRecipient>()
            .Should().BeEquivalentTo(new[] { "mchid", "failed_reason" });

        // 经纬度官方是 string(32)：建成 double? 会在历史含度分秒文本的数据上反序列化炸。
        PropertyTypeOf<MerchantStoreAddress>(nameof(MerchantStoreAddress.Longitude)).Should().Be(typeof(string));
        PropertyTypeOf<MerchantStoreAddress>(nameof(MerchantStoreAddress.Latitude)).Should().Be(typeof(string));
        PropertyTypeOf<MerchantStoreBasics>(nameof(MerchantStoreBasics.BrandId)).Should().Be(typeof(long?),
            "brand_id 官方是 integer（分配的品牌 ID），本域唯一整型 ID 字段");

        // 全域名单形态：官方名单全小写下划线，出现大写字母即被驼峰化（防「顺手纠正」）。
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotBeEmpty("参照集为空会让下面的名单形态断言静默恒真");
        allNames.Should().NotContain(static n => n.Any(static c => char.IsUpper(c)),
            "官方名单全小写：出现大写字母即说明有人把 snake_case 驼峰化了");
        allNames.Should().Contain(static n => n.Contains('_'),
            "正对照：本域确有下划线名单，上面的形态断言不是空集恒真");
        // 官方无数字打尾段名单（风险共建面的 extra_fields1 才需要容数字）：本域名单形态更简单，仍需正对照。
        allNames.Should().NotContain(static n => n.Any(static c => char.IsDigit(c)),
            "本域十页名单均不含数字尾段：出现数字即说明跨域抄了字段名");

        foreach (var type in DomainTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset),
                    $"{type.Name}.{property.Name} 不得用 DateTimeOffset");
                property.PropertyType.Should().NotBe(typeof(DateTime));
            }
        }
    }

    /// <summary>
    /// MST-B4：门店状态取值集（6 值）与「三档等待他方动作」的子集锁定 + 常量类名册恰 1 个。
    /// </summary>
    [Fact]
    public void MerchantStoreStateValues_ShouldMatchOfficialBulletListAndPendingSubset()
    {
        ValuesOf(typeof(MerchantStoreStateValues)).Should().BeEquivalentTo(new[]
        {
            "STORE_STATE_VALID", "STORE_STATE_ADDRESS_VERIFYING", "STORE_STATE_ADDRESS_FAILED",
            "STORE_STATE_RECIPIENT_REFUSED", "STORE_STATE_CLOSED", "STORE_STATE_RECIPIENT_VERIFYING",
        }, "官方 store_state_value 六值（内联印在查询页「可选取值」列表）：值集差异的权威出处");

        // 「写支成功 ≠ 生效」的可执行说明：六值中三档是等待他方动作、一档是失败终态。
        ValuesOf(typeof(MerchantStoreStateValues)).Should().Contain(new[]
        {
            MerchantStoreStateValues.RecipientVerifying,
            MerchantStoreStateValues.AddressVerifying,
        }, "收款号待确认与地址核实中：这两档只能靠轮询查询支观测，写支的 200/204 不代表生效");
        ValuesOf(typeof(MerchantStoreStateValues)).Should().NotContain("STORE_STATE_DELETED",
            "官方无删除态取值：CLOSED 之后由商户平台删除且不可恢复，本域不得伪造该值");

        ConstantClassNames().Should().BeEquivalentTo(new[] { nameof(MerchantStoreStateValues) },
            "本域常量类恰 1 个：其余字段官方无取值字典（见 MST-B5）");

        // 绑定与解绑的必填性相反（同一字段名、两支包装层不同 ⇒ 各建请求类型）。
        DeclaredJsonNamesOf<MerchantStoreRecipientBindRequest>()
            .Should().BeEquivalentTo(new[] { "sub_mchid", "store_recipient" });
        DeclaredJsonNamesOf<MerchantStoreRecipientUnbindRequest>()
            .Should().BeEquivalentTo(new[] { "sub_mchid", "store_recipient" });
        BodyTypeOf(nameof(IWechatPayMerchantStoreService.BindStoreRecipientsAsync))
            .Should().Be(typeof(MerchantStoreRecipientBindRequest));
        BodyTypeOf(nameof(IWechatPayMerchantStoreService.UnbindStoreRecipientsAsync))
            .Should().Be(typeof(MerchantStoreRecipientUnbindRequest));
        BodyTypeOf(nameof(IWechatPayMerchantStoreService.BindStoreRecipientsAsync))
            .Should().NotBe(BodyTypeOf(nameof(IWechatPayMerchantStoreService.UnbindStoreRecipientsAsync)),
            "两支各建请求类型：共用即把 bind 的必填语义抹平（官方 unbind 页该字段标选填）");

        // 两支应答共用同一类型（官方两页应答表逐字段相同）。
        FindMethod(nameof(IWechatPayMerchantStoreService.BindStoreRecipientsAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantStoreService.UnbindStoreRecipientsAsync)).ReturnType,
            "bind / unbind 应答表同为 failed_store_recipient ⇒ 共用一个应答类型");
    }

    /// <summary>
    /// MST-B5：「官方未给 / 不建模」裁决的可机械化部分 —— 本域<b>无加密字段</b>（十页参数表均无
    /// <c>Wechatpay-Serial</c>）、无分页游标、无官方口径分歧下的客户端校验。
    /// </summary>
    /// <remarks>
    /// 本域两处官方口径自相矛盾：<c>address_detail</c> 参数表写 string(100) 而散文要求 4-64 字符；
    /// <c>service_phone</c> 创建页写「仅数字和 -」而更新页写「最多两个、以 ; 分隔」
    /// ⇒ SDK <b>两侧都不校验</b>，宿主按更严口径写。类型面因此必须保持宽松的 string 承载。
    /// </remarks>
    [Fact]
    public void MerchantStoreUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted()
    {
        // 本域无加密面：十页参数表内无 Wechatpay-Serial ⇒ DTO 不出现密文 / 加密载体字段。
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotContain(static n => n.Contains("encrypt", StringComparison.Ordinal)
                                                 || n.Contains("serial", StringComparison.Ordinal)
                                                 || n == "phone" || n == "certificates_number",
            "门店面十页均无加密字段：把风险共建面的密文字段抄过来即造出官方不认的载荷");

        // 官方口径分歧的两字段保持 string 承载（不做格式校验、不收窄类型）。
        PropertyTypeOf<MerchantStoreAddress>(nameof(MerchantStoreAddress.AddressDetail)).Should().Be(typeof(string));
        PropertyTypeOf<MerchantStoreBusinessInfo>(nameof(MerchantStoreBusinessInfo.ServicePhone)).Should().Be(typeof(string));
        PropertyTypeOf<MerchantStoreAddress>(nameof(MerchantStoreAddress.AddressCode)).Should().Be(typeof(string),
            "address_code 官方要求「只能为数字」但类型仍是 string：收窄为整型会丢掉前导零");

        // 本域不建模门店列表 / 删除 ⇒ 类型名不得出现 List / Delete 分身。
        DomainTypeNames().Should().NotContain(static n => n.Contains("List", StringComparison.Ordinal)
                                                         || n.Contains("Delete", StringComparison.Ordinal),
            "官方无列表与删除门店端点：伪造其 DTO 即造出永远为空的假类型");

        // 门店 ID 在包体只出现在应答侧（请求侧一律走 path）。
        DeclaredJsonNamesOf<MerchantStoreCreateResponse>().Should().BeEquivalentTo(new[] { "store_id" },
            "创建应答只回 store_id：后续四支全部以它定位");
        DeclaredJsonNamesOf<MerchantStoreUpdateRequest>().Should().NotContain("store_id",
            "门店 ID 只在 path，包体再带一份即双写入口");
    }

    /// <summary>
    /// MST-B6：判错面继承 + 本域「十页归并五端点」的共用裁决（子树类型同一引用、无 <c>Partner</c> 分身）。
    /// </summary>
    [Fact]
    public void MerchantStoreResponses_ShouldDeriveFromBaseAndFollowSharedSubtreeRulings()
    {
        foreach (var responseType in new[]
                 {
                     typeof(MerchantStoreCreateResponse),
                     typeof(MerchantStoreQueryResponse),
                     typeof(MerchantStoreRecipientOperateResponse),
                 })
        {
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        foreach (var requestType in new[]
                 {
                     typeof(MerchantStoreCreateRequest),
                     typeof(MerchantStoreUpdateRequest),
                     typeof(MerchantStoreRecipientBindRequest),
                     typeof(MerchantStoreRecipientUnbindRequest),
                     typeof(MerchantStoreBasics),
                     typeof(MerchantStoreAddress),
                     typeof(MerchantStoreRecipientInfo),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求类型，不得挂在应答基类上");
        }

        // 共用①：创建 / 更新 / 查询三支共用同一批子树类型（同一 CLR 类型引用，不是字段名相同的两个类）。
        PropertyTypeOf<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.StoreBasics))
            .Should().Be(PropertyTypeOf<MerchantStoreUpdateRequest>(nameof(MerchantStoreUpdateRequest.StoreBasics)));
        PropertyTypeOf<MerchantStoreQueryResponse>(nameof(MerchantStoreQueryResponse.StoreAddress))
            .Should().Be(typeof(MerchantStoreAddress),
            "查询应答与创建 / 更新请求共用同一棵地址子树类型（不是字段名恰好相同的两个类）");
        PropertyTypeOf<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.StoreRecipient))
            .Should().Be(typeof(List<MerchantStoreRecipientInfo>));
        PropertyTypeOf<MerchantStoreRecipientBindRequest>(nameof(MerchantStoreRecipientBindRequest.StoreRecipient))
            .Should().Be(PropertyTypeOf<MerchantStoreCreateRequest>(nameof(MerchantStoreCreateRequest.StoreRecipient)),
            "收款号子树被创建 / 绑定 / 解绑 / 查询四处共用（官方四页字段表逐行相同）");

        // 共用②：本域没有服务商分身类型或分身方法（十页归并为五端点的直接体现）。
        DomainTypeNames().Should().NotContain(static n => n.Contains("Partner", StringComparison.Ordinal),
            "本域商户面与服务商面字段表相同 ⇒ 归并为一套，不得出现 Partner 前缀分身");
        typeof(IWechatPayMerchantStoreService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static m => m.Name)
            .Should().NotContain(static n => n.Contains("Partner", StringComparison.Ordinal));

        // store_state 只出现在查询应答（请求侧与创建应答都没有）⇒ 状态闭环只有一条路径。
        DeclaredJsonNamesOf<MerchantStoreQueryResponse>().Should().Contain("store_state");
        DeclaredJsonNamesOf<MerchantStoreCreateRequest>().Should().NotContain("store_state");
        DeclaredJsonNamesOf<MerchantStoreCreateResponse>().Should().NotContain("store_state");

        // 本域不得为 204 端点伪造应答类型。
        DomainTypeNames().Should().NotContain("MerchantStoreUpdateResponse",
            "更新支是 204 无包体：伪造应答类型即造出永远为空的假字段");
    }

    /// <summary>
    /// MST-B7：本域 13 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void MerchantStoreDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(13,
            "门店 DTO：主文件 9（三棵共享子树 + 收款号项 + 状态 + 创建请求/应答 + 更新请求 + 查询应答）+ "
            + "收款号文件 4（绑定请求 + 解绑请求 + 共用应答 + 失败项）= 13");

        foreach (var type in domainTypes)
        {
            MerchantStoreJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MerchantStoreJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（本域类型 = 13 DTO + 1 常量类，上下文类已排除）。
        DomainTypeNames().Should().HaveCount(14, "本域类型 = 13 DTO + 1 常量类");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", 
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MerchantStoreJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MerchantStore 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMerchantStoreWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayMerchantStoreService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayMerchantStoreService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static (string Verb, string Route)[] DistinctVerbRoutes()
        => typeof(IWechatPayMerchantStoreService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Select(static a => (a.GetType().Name, a.RequestUri!)))
            .Distinct()
            .ToArray();

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayMerchantStoreService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayMerchantStoreService.{methodName} 必须存在");

    private static Type BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Single(static p => p.GetCustomAttribute<PathAttribute>() == null
                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                 && p.ParameterType != typeof(CancellationToken))
            .ParameterType;

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] NoContentMethodsOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static IEnumerable<Type> DomainTypes()
        => typeof(MerchantStoreCreateRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(MerchantStoreCreateRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(MerchantStoreCreateRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
            .Select(static t => t.Name)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
        => JsonNameShouldBe(typeof(T), propertyName, expectedJsonName);

    private static void JsonNameShouldBe(Type type, string propertyName, string expectedJsonName)
    {
        var property = type.GetProperty(propertyName);
        property.Should().NotBeNull($"{type.Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    private static Type PropertyTypeOf<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在");
        return property!.PropertyType;
    }

    private static string[] DeclaredJsonNamesOf<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] ValuesOf(Type constantsType)
        => constantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
