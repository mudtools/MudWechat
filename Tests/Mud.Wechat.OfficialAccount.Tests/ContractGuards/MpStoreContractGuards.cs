// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Store;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// P4 首域「微信门店 → 门店小程序」契约守卫（<c>/wxa/*</c> 12 端点）。
/// </summary>
/// <remarks>
/// <para>
/// 官方事实来源（逐页核验 2026-10-07，12 页全服务号域命中）。关键核验：
/// ① 开放面极窄（错误码 43104「仅开放给电商类目」）；② 无新票据体系、与支付不耦合；
/// ③ 12 页全部支持第三方代调用（权限集 8-10、13 / 8-10、13、37）；④ 无频率数值；
/// ⑤ 适用范围页间不一致（4 页含小程序，8 页仅公众号 / 服务号），全部无「仅认证」标注。
/// </para>
/// <para>
/// <b>官方文档冲突三段式处置的守卫锁定（勿「顺手修正」）</b>：
/// 存在性取并集（<c>business.base_info</c> 全树）／标量类型取示例（经纬度 double、<c>reason</c> string）／
/// 容器形态：表扁平化取示例（<c>base_info</c> 嵌套）、表 array vs 示例字符串取表（<c>qualification_list</c>）。
/// </para>
/// </remarks>
public class MpStoreContractGuards
{
    private const string RegistryGroupName = "Store";

    /// <summary>官方路由表（12 端点；10 POST + 2 GET）。</summary>
    private static readonly (Type HttpAttribute, string Route, string Method)[] Routes =
    {
        (typeof(GetAttribute), "/wxa/get_merchant_category", nameof(IMpStoreService.GetMerchantCategoriesAsync)),
        (typeof(PostAttribute), "/wxa/apply_merchant", nameof(IMpStoreService.ApplyMerchantAsync)),
        (typeof(PostAttribute), "/wxa/get_merchant_audit_info", nameof(IMpStoreService.GetMerchantAuditInfoAsync)),
        (typeof(PostAttribute), "/wxa/modify_merchant", nameof(IMpStoreService.ModifyMerchantAsync)),
        (typeof(GetAttribute), "/wxa/get_district", nameof(IMpStoreService.GetDistrictListAsync)),
        (typeof(PostAttribute), "/wxa/search_map_poi", nameof(IMpStoreService.SearchMapPoiAsync)),
        (typeof(PostAttribute), "/wxa/add_store", nameof(IMpStoreService.AddStoreAsync)),
        (typeof(PostAttribute), "/wxa/get_store_info", nameof(IMpStoreService.GetStoreInfoAsync)),
        (typeof(PostAttribute), "/wxa/get_store_list", nameof(IMpStoreService.GetStoreListAsync)),
        (typeof(PostAttribute), "/wxa/del_store", nameof(IMpStoreService.DeleteStoreAsync)),
        (typeof(PostAttribute), "/wxa/update_store", nameof(IMpStoreService.UpdateStoreAsync)),
        (typeof(PostAttribute), "/wxa/create_map_poi", nameof(IMpStoreService.CreateMapPoiAsync)),
    };

    /// <summary>契约守卫 ST1：12 端点路由 / 方法与官方契约一致（含方法核验裁决锁定）。</summary>
    [Fact]
    public void StoreEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Select(r => r.Route).Distinct().Should().HaveCount(Routes.Length, "各端点路由互不重复");
        Routes.Should().OnlyContain(r => r.Route.StartsWith("/wxa/", StringComparison.Ordinal),
            "本域 12 端点全在 /wxa/ 前缀下（官方路径安排；前缀不入命名空间）");

        foreach (var (httpAttribute, route, method) in Routes)
        {
            var target = FindMethod(method);
            var attr = target.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{method} 必须声明 {httpAttribute.Name} 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");
        }

        // 仅两个只读端点官方标 GET，其余 10 个 POST。
        Routes.Count(r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(2,
            "仅 get_merchant_category / get_district 官方标 GET");
        Routes.Where(r => r.HttpAttribute == typeof(GetAttribute)).Select(r => r.Method)
            .Should().BeEquivalentTo(new[]
            {
                nameof(IMpStoreService.GetMerchantCategoriesAsync),
                nameof(IMpStoreService.GetDistrictListAsync),
            });

        // 方法核验裁决（官方三处矛盾）：get_merchant_audit_info 裁决为 POST + 请求体，
        // 防被「按官方调用方式章修正回 GET」——GET 无请求体，audit_id 将无处安放。
        FindMethod(nameof(IMpStoreService.GetMerchantAuditInfoAsync))
            .GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                "官方标 GET 但参数章定义为 REQUEST PAYLOAD ⇒ SDK 裁决 POST（官方 GET 标注照录于 XML）");
    }

    /// <summary>契约守卫 ST2：参数位置（GET 无请求体 / POST 有请求体）与共用 DTO 裁决锁定。</summary>
    [Fact]
    public void StoreEndpoints_ShouldKeepOfficialParameterPlacement()
    {
        foreach (var (_, route, method) in Routes)
        {
            var hasBody = HasBody(method);
            if (route is "/wxa/get_merchant_category" or "/wxa/get_district")
            {
                hasBody.Should().BeFalse($"{method} 官方为 GET 且无请求体");
            }
            else
            {
                hasBody.Should().BeTrue($"{method} 官方定义为 REQUEST PAYLOAD（请求体）");
            }
        }

        // get_store_info 与 del_store 请求体字段集一致（均仅 poi_id）⇒ 共用 DTO（防重复造 DTO）。
        foreach (var method in new[]
                 {
                     nameof(IMpStoreService.GetStoreInfoAsync),
                     nameof(IMpStoreService.DeleteStoreAsync),
                 })
        {
            FindMethod(method).GetParameters()
                .Should().Contain(p => p.ParameterType == typeof(MpStorePoiRequest),
                    $"{method} 官方请求体仅 poi_id ⇒ 共用 MpStorePoiRequest");
        }

        // 三个「仅 errcode/errmsg」端点复用 MpResponse（防空壳 DTO 增生）。
        foreach (var method in new[]
                 {
                     nameof(IMpStoreService.ApplyMerchantAsync),
                     nameof(IMpStoreService.ModifyMerchantAsync),
                     nameof(IMpStoreService.DeleteStoreAsync),
                 })
        {
            TaskResultType(method).Should().Be(typeof(MpResponse),
                $"{method} 官方响应仅 errcode/errmsg ⇒ 复用 MpResponse");
        }
    }

    /// <summary>契约守卫 ST3：DTO 官方字段名锁定（逐类型）。</summary>
    [Fact]
    public void StoreShapes_ShouldLockOfficialFieldNames()
    {
        // 类目树。
        AssertJsonProperty<MpMerchantCategoryResponse>("data", "类目数据");
        AssertJsonProperty<MpMerchantCategoryData>("all_category_info", "类目信息");
        AssertJsonProperty<MpMerchantCategoryInfo>("categories", "类目列表");
        AssertJsonProperty<MpMerchantCategory>("id", "类目 id");
        AssertJsonProperty<MpMerchantCategory>("level", "一级或二级类目");
        AssertJsonProperty<MpMerchantCategory>("sensitive_type", "0 不用特殊处理 / 1 需添加证件");
        AssertJsonProperty<MpMerchantCategory>("name", "示例独有（字段表未收录）");
        AssertJsonProperty<MpMerchantCategory>("children", "示例独有（自引用子类目）");

        // 主体申请 / 修改。
        AssertJsonProperty<MpApplyMerchantRequest>("first_catid", "一级类目 id");
        AssertJsonProperty<MpApplyMerchantRequest>("second_catid", "二级类目 id");
        AssertJsonProperty<MpApplyMerchantRequest>("headimg_mediaid", "头像临时素材 mediaid");
        AssertJsonProperty<MpApplyMerchantRequest>("nickname", "昵称 4~30 字符（中文算两个）");
        AssertJsonProperty<MpApplyMerchantRequest>("intro", "门店小程序介绍");
        AssertJsonProperty<MpApplyMerchantRequest>("qualification_list", "类目相关证件 mediaid");
        AssertJsonProperty<MpApplyMerchantRequest>("org_code", "营业执照或组织代码证 mediaid");
        AssertJsonProperty<MpApplyMerchantRequest>("other_files", "补充材料 mediaid");
        AssertJsonProperty<MpModifyMerchantRequest>("headimg_mediaid", "不改可传空值");
        AssertJsonProperty<MpModifyMerchantRequest>("intro", "不改可传空值");

        // 主体审核结果。
        AssertJsonProperty<MpMerchantAuditInfoRequest>("audit_id", "审核单 id");
        AssertJsonProperty<MpMerchantAuditResult>("status", "0 未提交 / 1 成功 / 2 审核中 / 3 失败 / 4 管理员拒绝");
        AssertJsonProperty<MpMerchantAuditResult>("reason", "仅 status 为 3 / 4 时返回");

        // 省市区。
        AssertJsonProperty<MpDistrictResponse>("status", "状态码");
        AssertJsonProperty<MpDistrictResponse>("message", "状态描述");
        AssertJsonProperty<MpDistrictResponse>("data_version", "数据版本");
        AssertJsonProperty<MpDistrictResponse>("result", "二维数组：省 / 市 / 区");
        AssertJsonProperty<MpDistrictInfo>("id", "区域 id（districtid）");
        AssertJsonProperty<MpDistrictInfo>("fullname", "完整名称");
        AssertJsonProperty<MpDistrictInfo>("pinyin", "拼音列表");
        AssertJsonProperty<MpDistrictInfo>("location", "坐标");
        AssertJsonProperty<MpDistrictInfo>("cidx", "下属地区 id 列表");
        AssertJsonProperty<MpDistrictLocation>("lat", "纬度");
        AssertJsonProperty<MpDistrictLocation>("lng", "经度");

        // 地图点位搜索。
        AssertJsonProperty<MpMapPoiSearchRequest>("districtid", "省市区 id");
        AssertJsonProperty<MpMapPoiSearchRequest>("keyword", "搜索关键词");
        AssertJsonProperty<MpMapPoiSearchData>("item", "信息数组");
        AssertJsonProperty<MpMapPoiItem>("branch_name", "门店名称");
        AssertJsonProperty<MpMapPoiItem>("sosomap_poi_uid", "add_store 的 map_poi_id 取值来源");
        AssertJsonProperty<MpMapPoiItem>("data_supply", "地图数据（官方语义未定义，照录）");
        AssertJsonProperty<MpMapPoiItem>("pic_urls", "门店图片列表（元素类型官方未给出）");
        AssertJsonProperty<MpMapPoiItem>("card_id_list", "卡券列表（元素类型官方未给出）");

        // 门店 CRUD。
        AssertJsonProperty<MpStorePictureList>("list", "pic_list JSON 字符串内的 list");
        AssertJsonProperty<MpAddStoreRequest>("map_poi_id", "腾讯地图点位 id（取自 sosomap_poi_uid）");
        AssertJsonProperty<MpAddStoreRequest>("pic_list", "门店图片（JSON 字符串）");
        AssertJsonProperty<MpAddStoreRequest>("contract_phone", "联系电话");
        AssertJsonProperty<MpAddStoreRequest>("hour", "营业时间 11:11-12:12");
        AssertJsonProperty<MpAddStoreRequest>("credential", "经营资质证件号");
        AssertJsonProperty<MpAddStoreRequest>("company_name", "主体名字");
        AssertJsonProperty<MpAddStoreRequest>("card_id", "卡券 id（仅透传，不消费 api_ticket）");
        AssertJsonProperty<MpAddStoreRequest>("qualification_list", "相关证明材料 mediaid");
        AssertJsonProperty<MpAddStoreRequest>("poi_id", "门店迁移用");
        AssertJsonProperty<MpStoreAuditResult>("audit_id", "审核单 id");
        AssertJsonProperty<MpStorePoiRequest>("poi_id", "门店 id");
        AssertJsonProperty<MpStoreInfoResponse>("business", "业务信息（仅见示例，字段表未收录）");
        AssertJsonProperty<MpStoreBusiness>("base_info", "门店基本信息");
        AssertJsonProperty<MpStoreBaseInfo>("business_name", "门店名称");
        AssertJsonProperty<MpStoreBaseInfo>("photo_list", "图片列表");
        AssertJsonProperty<MpStorePhoto>("photo_url", "图片 url");
        AssertJsonProperty<MpStoreBaseInfo>("open_time", "门店开放时间");
        AssertJsonProperty<MpStoreBaseInfo>("qualification_num", "营业执照号");
        AssertJsonProperty<MpStoreBaseInfo>("qualification_name", "营业执照的名称");
        AssertJsonProperty<MpStoreListRequest>("offset", "从 0 开始计数");
        AssertJsonProperty<MpStoreListRequest>("limit", "获取门店个数（官方未给上限）");
        AssertJsonProperty<MpStoreListResponse>("business_list", "门店列表");
        AssertJsonProperty<MpStoreListResponse>("total_count", "门店总数");
        AssertJsonProperty<MpStoreListItem>("base_info", "嵌套口径取示例（字段表扁平化不可信）");
        AssertJsonProperty<MpStoreListItem>("categories", "示例独有（字段表未收录）");
        AssertJsonProperty<MpUpdateStoreRequest>("poi_id", "门店 id");
        AssertJsonProperty<MpUpdateStoreRequest>("pic_list", "门店图片（JSON 字符串）");
        AssertJsonProperty<MpUpdateStoreRequest>("contract_phone", "联系电话");
        AssertJsonProperty<MpUpdateStoreRequest>("hour", "营业时间");
        AssertJsonProperty<MpUpdateStoreRequest>("card_id", "卡券 id");
        AssertJsonProperty<MpStoreUpdateResult>("has_audit_id", "1 需要 / 0 不需要");
        AssertJsonProperty<MpStoreUpdateResult>("audit_id", "审核单 id");
        AssertJsonProperty<MpCreateMapPoiRequest>("introduct", "官方字段原文（疑 introduction 拼写，不得规范化）");
        AssertJsonProperty<MpCreateMapPoiRequest>("districtid", "腾讯地图省市区 id");
        AssertJsonProperty<MpCreateMapPoiRequest>("license", "营业执照 url");
        AssertJsonProperty<MpCreateMapPoiRequest>("photo", "门店图片 url");
        AssertJsonProperty<MpCreateMapPoiResult>("base_id", "审核单 id");
        AssertJsonProperty<MpCreateMapPoiResult>("rich_id", "官方说明为 -（语义未定义，照录）");
    }

    /// <summary>契约守卫 ST4：官方文档冲突三段式处置的**负向**锁定（防「顺手修正」）。</summary>
    [Fact]
    public void StoreDocumentConflicts_ShouldKeepAdjudicatedShapes()
    {
        // ② 标量类型冲突取官方返回示例：经纬度 double、审核 reason string。
        typeof(MpDistrictLocation).GetProperty(nameof(MpDistrictLocation.Latitude))!.PropertyType
            .Should().Be(typeof(double?), "字段表标 string、示例为数字 ⇒ 取示例（double）");
        typeof(MpDistrictLocation).GetProperty(nameof(MpDistrictLocation.Longitude))!.PropertyType
            .Should().Be(typeof(double?));
        typeof(MpStoreBaseInfo).GetProperty(nameof(MpStoreBaseInfo.Longitude))!.PropertyType
            .Should().Be(typeof(double?), "get_store_list / get_store_info 同口径");
        typeof(MpStoreBaseInfo).GetProperty(nameof(MpStoreBaseInfo.Latitude))!.PropertyType
            .Should().Be(typeof(double?));
        typeof(MpMerchantAuditResult).GetProperty(nameof(MpMerchantAuditResult.Reason))!.PropertyType
            .Should().Be(typeof(string), "字段表标 number、示例为字符串 ⇒ 取示例（string）");

        // ③-a 嵌套结构冲突取示例：store_list 条目必须经 base_info 承载，不得把子字段扁平化为平级。
        var listItemProps = JsonNamesOf<MpStoreListItem>();
        listItemProps.Should().Contain("base_info");
        listItemProps.Should().NotContain("business_name",
            "官方字段表把 base_info 子字段列为平级，属表的抽象失真 ⇒ 取示例（嵌套）");
        listItemProps.Should().NotContain("status");

        // ③-b 表 array vs 示例字符串 ⇒ 取字段表（List<string>）。
        typeof(MpAddStoreRequest).GetProperty(nameof(MpAddStoreRequest.QualificationList))!.PropertyType
            .Should().Be(typeof(List<string>), "字段表标 array ⇒ 取表口径（示例单字符串为真机局部形态）");

        // 存在性 ①：get_store_info 的整个 business.base_info 树必须建模（字段表竟只列 errcode/errmsg）。
        typeof(MpStoreInfoResponse).GetProperty(nameof(MpStoreInfoResponse.Business)).Should().NotBeNull();
        typeof(MpStoreBusiness).GetProperty(nameof(MpStoreBusiness.BaseInfo)).Should().NotBeNull();

        // 官方示例中出现但参数表未定义的字段不得建模（update_store 的 map_poi_id）。
        JsonNamesOf<MpUpdateStoreRequest>().Should().NotContain("map_poi_id",
            "map_poi_id 仅见官方请求示例、参数表未定义（无类型/必填/说明）⇒ 不建模");

        // create_map_poi 的 introduct 为官方字段原文，不得「规范化」为 introduction。
        JsonNamesOf<MpCreateMapPoiRequest>().Should().Contain("introduct");
        JsonNamesOf<MpCreateMapPoiRequest>().Should().NotContain("introduction");
    }

    /// <summary>契约守卫 ST5：DTO 全量登记进上下文 + 模块枚举 / 注册入口 / Query 令牌白名单。</summary>
    [Fact]
    public void StoreModuleAndDataModels_ShouldBeRegistered()
    {
        const string ns = "Mud.Wechat.OfficialAccount.DataModels.Store";
        var domainTypes = typeof(MpStorePoiRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == ns
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(34, "门店域 DTO：类目 4 + 主体 3 + 审核 3 + 修改 1 + 省市区 3 + 地图点位 4 + 门店 13 + 建店 3");
        foreach (var type in domainTypes)
        {
            StoreJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 StoreJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        Enum.GetNames(typeof(MpModule)).Should().Contain("Store");
        typeof(MpServiceBuilder).GetMethod("AddStoreApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();
        queryInterfaces.Should().Contain(nameof(IMpStoreService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");

        var token = typeof(IMpStoreService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull();
        token!.Name.Should().Be("access_token");
        token.TokenType.Should().Be(MpTokenTypes.AccessToken);
    }

    /// <summary>契约守卫 ST6：枚举常量与已核验错误码锁定（含 43104 电商类目门槛）。</summary>
    [Fact]
    public void StoreConstants_ShouldMatchVerifiedOfficialValues()
    {
        MpMerchantAuditStatuses.NotSubmitted.Should().Be(0);
        MpMerchantAuditStatuses.Approved.Should().Be(1);
        MpMerchantAuditStatuses.Auditing.Should().Be(2);
        MpMerchantAuditStatuses.Rejected.Should().Be(3);
        MpMerchantAuditStatuses.AdminRejected.Should().Be(4);

        MpStoreStatuses.Approved.Should().Be(1);
        MpStoreStatuses.Auditing.Should().Be(2);
        MpStoreStatuses.Rejected.Should().Be(3);

        MpStoreCategorySensitiveTypes.Normal.Should().Be(0);
        MpStoreCategorySensitiveTypes.QualificationRequired.Should().Be(1);

        // 主体审核状态 ≠ 门店审核状态（勿混用：前者含「未提交 / 管理员拒绝」两态）。
        MpMerchantAuditStatuses.AdminRejected.Should().NotBe(MpStoreStatuses.Approved);

        MpErrorCodes.StoreAppIdPermissionDenied.Should().Be(43104,
            "官方原文「仅开放给电商类目（电商平台、商家自营、跨境电商）」——本域开放面的关键约束");
        MpErrorCodes.InvalidArgs.Should().Be(40097);
        MpErrorCodes.StoreSupplementRequired.Should().Be(85024);
        MpErrorCodes.StoreAdminPhoneBindLimit.Should().Be(85025);
        MpErrorCodes.StoreAdminWeChatBindLimit.Should().Be(85026);
        MpErrorCodes.StoreAdminIdCardBindLimit.Should().Be(85027);
        MpErrorCodes.StoreContractorBindLimit.Should().Be(85028);
        MpErrorCodes.StoreNicknameUsed.Should().Be(85029);
        MpErrorCodes.StoreNicknameSizeInvalid.Should().Be(85030);
        MpErrorCodes.StoreNicknameForbidden.Should().Be(85031);
        MpErrorCodes.StoreNicknameComplained.Should().Be(85032);
        MpErrorCodes.StoreNicknameIllegal.Should().Be(85033);
        MpErrorCodes.StoreNicknameProtected.Should().Be(85034);
        MpErrorCodes.StoreNicknameContractorMismatch.Should().Be(85035);
        MpErrorCodes.StoreIntroductionIllegal.Should().Be(85036);
        MpErrorCodes.StoreAlreadyAdded.Should().Be(85038);
        MpErrorCodes.StoreNotAccessible.Should().Be(85039);
        MpErrorCodes.StoreAlreadyBound.Should().Be(85040);
        MpErrorCodes.StoreCredentialUsed.Should().Be(85041);
        MpErrorCodes.StoreNearbyLimitReached.Should().Be(85042);
        MpErrorCodes.StoreHeadImageQuotaLimit.Should().Be(85049);
        MpErrorCodes.StoreAuditing.Should().Be(85050);
        MpErrorCodes.StoreMerchantNotApplied.Should().Be(85053);
        MpErrorCodes.StorePoiIdRequired.Should().Be(85054);
        MpErrorCodes.StoreMapPoiIdInvalid.Should().Be(85055);
        MpErrorCodes.StoreMediaIdInvalid.Should().Be(85056);
        MpErrorCodes.StorePoiNotExists.Should().Be(65115);
        MpErrorCodes.StoreStatusInvalid.Should().Be(65118);
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IMpStoreService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IMpStoreService.{methodName} 必须存在");

    private static Type TaskResultType(string methodName)
        => FindMethod(methodName).ReturnType.GetGenericArguments()[0];

    private static bool HasBody(string methodName)
        => FindMethod(methodName).GetParameters().Any(p => p.GetCustomAttribute<BodyAttribute>() != null);

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
