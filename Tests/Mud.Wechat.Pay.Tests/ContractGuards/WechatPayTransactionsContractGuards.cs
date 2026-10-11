// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 基础交易域（直连下单四族）契约守卫 <b>TX1~TX3</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>核验状态与本守卫的断言边界（必读）</b>：JSAPI 页（docId <c>4012791897</c>）为本仓
/// 2026-10-09 逐页核验过的<b>锚点</b>；Native <c>4012791877</c> / APP <c>4013070347</c> /
/// H5 <c>4012791834</c> 三页的<b>路由与字段表取自本地 SKIT <c>TenpayV3</c> 的登记</b>
/// （2026-10-10 对齐基准），<b>官方逐页核验待补</b>（微信支付文档中心为 SPA，正文不可达）。
/// 因此本守卫断言的是「<b>四族之间的结构分合关系</b>」——该关系由 SKIT 的顶层字段集与嵌套类型
/// 继承图直接给出（<c>Native/Jsapi/H5</c> 均写作「继承 <c>App</c> 的同名嵌套类型」⇒ 子表逐项一致），
/// <b>不</b>声称各页的必填性与取值枚举已经官方核验。
/// </para>
/// <para>
/// <b>SKIT 已核出的三处族差异</b>：① <c>payer</c> <b>只出现在 JSAPI</b> 请求（Native/APP/H5 皆无）；
/// ② <c>subsidy_info</c> <b>只出现在 APP</b> 请求；③ <c>h5_info</c> <b>只出现在 H5</b> 的
/// <c>scene_info</c>（其继承 app 的 Scene 再补该节）。应答侧：JSAPI/APP 同为 <c>prepay_id</c>、
/// Native 为 <c>code_url</c>、H5 为 <c>h5_url</c>。
/// </para>
/// <para>
/// <b>待核的嵌套字段差（留档，不静默采纳）</b>：SKIT 的 app/native 嵌套 <c>Scene</c> 另含
/// <c>device_ip</c>、<c>store_info.out_id</c>，<c>Settlement</c> 另含 <c>subsidy_amount</c>；
/// 本仓在 JSAPI 页逐字段核验时官方<b>未列</b>这几项 ⇒ <see cref="JsapiSceneInfo"/> /
/// <see cref="JsapiSettleInfo"/> 保持已核验形态。若 native/app/h5 页核验后官方确有，
/// 处置是<b>为该族新建</b>场景/结算类型，不是改动 JSAPI 那支（TX2 锁死该复用关系）。
/// </para>
/// </remarks>
public class WechatPayTransactionsContractGuards
{
    private const string RegistryGroupName = "Transactions";
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Transactions";

    /// <summary>
    /// TX1：四个下单端点的路由、返回类型与「无 path / query 参数」照官方契约。
    /// </summary>
    [Fact]
    public void TransactionsPrepayEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayTransactionsService.CreateJsapiOrderAsync), "/v3/pay/transactions/jsapi");
        AssertRoute<PostAttribute>(nameof(IWechatPayTransactionsService.CreateNativeOrderAsync), "/v3/pay/transactions/native");
        AssertRoute<PostAttribute>(nameof(IWechatPayTransactionsService.CreateAppOrderAsync), "/v3/pay/transactions/app");
        AssertRoute<PostAttribute>(nameof(IWechatPayTransactionsService.CreateH5OrderAsync), "/v3/pay/transactions/h5");

        // 四族都是纯报文端点：无 path 占位符、无 query 参数（官方四页一致）。
        foreach (var methodName in new[]
                 {
                     nameof(IWechatPayTransactionsService.CreateJsapiOrderAsync),
                     nameof(IWechatPayTransactionsService.CreateNativeOrderAsync),
                     nameof(IWechatPayTransactionsService.CreateAppOrderAsync),
                     nameof(IWechatPayTransactionsService.CreateH5OrderAsync),
                 })
        {
            var method = FindMethod(methodName);
            method.GetParameters()
                .Where(static p => p.GetCustomAttribute<PathAttribute>() != null
                                   || p.GetCustomAttribute<QueryAttribute>() != null)
                .Should().BeEmpty($"{methodName} 官方页无 path / query 参数");
            method.GetParameters().Length.Should().Be(2,
                $"{methodName} 形参恒为「请求体 + CancellationToken」");
        }

        // 应答类型：JSAPI 与 APP 共用（两页应答表逐项一致），Native / H5 各按自身字段独立。
        FindMethod(nameof(IWechatPayTransactionsService.CreateJsapiOrderAsync))
            .ReturnType.Should().Be(typeof(Task<PrepayIdResponse>));
        FindMethod(nameof(IWechatPayTransactionsService.CreateAppOrderAsync))
            .ReturnType.Should().Be(typeof(Task<PrepayIdResponse>),
                "官方 APP 页应答表与 JSAPI 逐项一致（均只有 prepay_id）⇒ 共用 DTO（表相同则共用）");
        FindMethod(nameof(IWechatPayTransactionsService.CreateNativeOrderAsync))
            .ReturnType.Should().Be(typeof(Task<NativePrepayResponse>),
                "Native 应答只有 code_url，与 prepay_id 不可互换");
        FindMethod(nameof(IWechatPayTransactionsService.CreateH5OrderAsync))
            .ReturnType.Should().Be(typeof(Task<H5PrepayResponse>),
                "H5 应答只有 h5_url（可直接跳转的链接），与 prepay_id / code_url 都不同");
    }

    /// <summary>
    /// TX2：四族的<b>结构分合判断</b> —— 该合的合（子表同形）、该分的分（族专属字段），
    /// 且<b>不得</b>让某族传官方该页未定义的字段。
    /// </summary>
    [Fact]
    public void TransactionsPrepayDtos_ShouldFollowTableIdentityRule()
    {
        // ① payer 只属 JSAPI：Native / APP / H5 请求都不得有该属性（有则可传官方未定义字段）。
        typeof(JsapiPrepayRequest).GetProperty(nameof(JsapiPrepayRequest.Payer)).Should().NotBeNull();
        typeof(JsapiPayerInfo).GetProperty(nameof(JsapiPayerInfo.OpenId)).Should().NotBeNull();
        foreach (var requestType in new[]
                 {
                     typeof(NativePrepayRequest),
                     typeof(AppPrepayRequest),
                     typeof(H5PrepayRequest),
                 })
        {
            requestType.GetProperty("Payer").Should().BeNull(
                $"{requestType.Name} 不得有 payer —— 三族下单时服务端拿不到支付者标识");
        }

        // ② subsidy_info 只属 APP（SKIT 侧仅 app 顶层列出该字段）。
        typeof(AppPrepayRequest).GetProperty(nameof(AppPrepayRequest.SubsidyInfo))
            .Should().NotBeNull("APP 页顶层含 subsidy_info（Native / H5 页无）");
        typeof(AppSubsidyInfo).GetProperty(nameof(AppSubsidyInfo.SubsidyDetail)).Should().NotBeNull();
        typeof(AppSubsidyDetail).GetProperty(nameof(AppSubsidyDetail.SubsidyPlan)).Should().NotBeNull();
        typeof(NativePrepayRequest).GetProperty("SubsidyInfo").Should().BeNull("Native 页无 subsidy_info");
        typeof(H5PrepayRequest).GetProperty("SubsidyInfo").Should().BeNull("H5 页无 subsidy_info");

        // ③ h5_info 只属 H5 的场景信息（放进通用 scene_info 会让其它族传官方未定义字段）。
        typeof(H5SceneInfo).GetProperty(nameof(H5SceneInfo.H5Info)).Should().NotBeNull();
        typeof(H5Info).GetProperty(nameof(H5Info.Type)).Should().NotBeNull();
        typeof(JsapiSceneInfo).GetProperty("H5Info").Should().BeNull(
            "h5_info 是 H5 页专属一节 —— 不得塞进各族共用的场景信息");

        // ④ 子表同形 ⇒ 必须复用（该合没合就是重复契约面）。
        foreach (var (requestType, amountProperty, detailProperty, sceneProperty, settleProperty) in new[]
                 {
                     (typeof(JsapiPrepayRequest), nameof(JsapiPrepayRequest.Amount), nameof(JsapiPrepayRequest.Detail), nameof(JsapiPrepayRequest.SceneInfo), nameof(JsapiPrepayRequest.SettleInfo)),
                     (typeof(NativePrepayRequest), nameof(NativePrepayRequest.Amount), nameof(NativePrepayRequest.Detail), nameof(NativePrepayRequest.SceneInfo), nameof(NativePrepayRequest.SettleInfo)),
                     (typeof(AppPrepayRequest), nameof(AppPrepayRequest.Amount), nameof(AppPrepayRequest.Detail), nameof(AppPrepayRequest.SceneInfo), nameof(AppPrepayRequest.SettleInfo)),
                     (typeof(H5PrepayRequest), nameof(H5PrepayRequest.Amount), nameof(H5PrepayRequest.Detail), nameof(H5PrepayRequest.SceneInfo), nameof(H5PrepayRequest.SettleInfo)),
                 })
        {
            requestType.GetProperty(amountProperty)!.PropertyType.Should().Be(typeof(JsapiAmountInfo),
                $"{requestType.Name} 的 amount 与 JSAPI 页同表 ⇒ 共用");
            requestType.GetProperty(detailProperty)!.PropertyType.Should().Be(typeof(JsapiGoodsDetail),
                $"{requestType.Name} 的 detail 与 JSAPI 页同表 ⇒ 共用");
            requestType.GetProperty(settleProperty)!.PropertyType.Should().Be(typeof(JsapiSettleInfo),
                $"{requestType.Name} 的 settle_info 与 JSAPI 页同表 ⇒ 共用");
        }

        // scene_info 是唯一「表不同」的子表：H5 用独立类型，其余三族共用。
        typeof(H5PrepayRequest).GetProperty(nameof(H5PrepayRequest.SceneInfo))!
            .PropertyType.Should().Be(typeof(H5SceneInfo), "H5 场景信息多 h5_info ⇒ 不复用 JsapiSceneInfo");
        typeof(H5SceneInfo).GetProperty(nameof(H5SceneInfo.StoreInfo))!
            .PropertyType.Should().Be(typeof(JsapiStoreInfo), "h5_info 之外的 store_info 与 JSAPI 同表 ⇒ 共用");
        foreach (var requestType in new[] { typeof(JsapiPrepayRequest), typeof(NativePrepayRequest), typeof(AppPrepayRequest) })
        {
            var sceneInfoProperty = requestType.GetProperties()
                .First(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "scene_info");
            sceneInfoProperty.PropertyType.Should().Be(typeof(JsapiSceneInfo),
                $"{requestType.Name} 的 scene_info 与 JSAPI 页同表 ⇒ 共用");
        }

        // ⑤ 三种应答互不混用字段：prepay_id / code_url / h5_url 各自唯一落点。
        typeof(PrepayIdResponse).GetProperty(nameof(PrepayIdResponse.PrepayId)).Should().NotBeNull();
        typeof(PrepayIdResponse).GetProperty("CodeUrl").Should().BeNull();
        typeof(PrepayIdResponse).GetProperty("H5Url").Should().BeNull();
        typeof(NativePrepayResponse).GetProperty(nameof(NativePrepayResponse.CodeUrl)).Should().NotBeNull();
        typeof(NativePrepayResponse).GetProperty("PrepayId").Should().BeNull(
            "code_url 与 prepay_id 不可互换（一个供服务端生成二维码、一个供客户端 SDK 调起）");
        typeof(H5PrepayResponse).GetProperty(nameof(H5PrepayResponse.H5Url)).Should().NotBeNull();
        typeof(H5PrepayResponse).GetProperty("PrepayId").Should().BeNull();
        typeof(NativePrepayResponse).GetProperty("H5Url").Should().BeNull();
    }

    /// <summary>
    /// TX3：域内 DTO 全量登记进 <see cref="TransactionsJsonContext"/> 且分组名一致
    /// （AOT 无元数据即静默失败）+ 判错面继承。
    /// </summary>
    [Fact]
    public void TransactionsDataModels_ShouldBeRegisteredInJsonContext()
    {
        var domainTypes = typeof(JsapiPrepayRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(27,
            "基础交易域 DTO：JSAPI 族 8（请求/金额/支付者/商品详情/单品/场景/门店/结算） + 下单共用应答 1（prepay_id）" +
            " + Native 族 2（请求/应答） + APP 族 4（请求/补贴/明细/方案） + H5 族 4（请求/场景/H5信息/应答）" +
            " + 查单 6（应答/支付者/金额/场景/优惠/优惠单品） + 关单 1 + 小程序调起签名 1");

        foreach (var type in domainTypes)
        {
            TransactionsJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 TransactionsJsonContext（Native AOT 无元数据会静默失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 判错面：四个下单应答都必须承载官方 code/message（4xx 错误体有落点）。
        foreach (var responseType in new[]
                 {
                     typeof(PrepayIdResponse),
                     typeof(NativePrepayResponse),
                     typeof(H5PrepayResponse),
                     typeof(TransactionQueryResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse（判错面）");
        }
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayTransactionsService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayTransactionsService.{methodName} 必须存在");
}
