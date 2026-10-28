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
using Mud.Wechat.Pay.DataModels.MerchantAlterApply;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-7 商户资料变更域契约守卫（<b>12 端点</b>：主体 4 + 超级管理员 4 + 商家简称 4）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（2026-10-14 十六页逐字段核验）：主体族 <c>4014090649</c> / <c>4014090655</c> /
/// <c>4014090667</c> / <c>4014090672</c>；超级管理员族 <c>4015617906</c> / <c>4015617911</c> /
/// <c>4015617915</c> / <c>4015617919</c>；商家简称族 <c>4016166071</c> / <c>4016166074</c> /
/// <c>4016166085</c> / <c>4016166090</c>。逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>缺口清单的出处错配在此机械化收口</b>：清单用主体族四个 docId（<c>4014090649</c> 等）同时承载
/// 联系人族四行，逐页核得两族各有独立页面（联系人族的现行四页是「修改超级管理员」<c>4015617906</c> 系）
/// ⇒ 八行是八个页面、两支独立路由族。<b>路由字符串本身清单抄对了</b>，错的是出处归属
/// —— MAA-B2 因此逐支锁「方法 ↔ docId」配对，而不是锁某个不存在的路由形态。
/// </para>
/// <para>
/// <b>本域的归并裁决是「逐类型」而不是「逐族」</b>，三族十二支里同时出现三种处置：
/// ① 三族「提交」应答表相同 ⇒ <b>共用</b>一个类型；
/// ② 超管族与简称族「撤销」应答表相同 ⇒ <b>共用</b>，而主体族撤销回 <c>{result}</c> ⇒ <b>分建</b>；
/// ③ 三族「查询」<b>各自一型</b>——主体族与超管族的字段名与长度逐字相同，唯一差异是 <c>state</c> 值清单
/// （6 值含 <c>EDITTING</c> vs 5 值）；简称族另有 <c>sub_mchid</c> 定位键且<b>无</b> <c>audit_reject_detail</c>。
/// ②③ 与同批 <see cref="WechatPayMerchantStoreContractGuards"/>（十页归并五端点）和
/// <see cref="WechatPayMerchantRiskManageContractGuards"/>（同路由分建两形态）并置，
/// 构成「按官方字段表归并」口径的三种取值。
/// </para>
/// <para>
/// <b>本守卫要守住的五处「顺手就会写错」</b>：
/// ① 定位键分族：主体与超管用 <c>merchant_code</c>、简称族用 <c>sub_mchid</c>，而「按业务单号查」三族的
/// 路径<b>字面段</b>同为 <c>/merchant/</c> ⇒ 只看路径模板会以为三族同键；
/// ② 加密头<b>只覆盖两支</b>：主体族与超管族的提交页把 <c>Wechatpay-Serial</c> 列为参数表行，
/// 简称族页内<b>零个</b>加密字段 ⇒ 按「变更族都要加密」即错；
/// ③ <c>state</c> 两处误拼 <c>EDITTING</c>（三 T）/ <c>MODIFING</c>（单 F）；
/// ④ <c>out_request_no</c> 在提交侧是 64 / 124 / 124、在查询应答侧一律 64（同域两页不一致，官方原文如此）；
/// ⑤ 主体族「按申请单号查」的 PATH <c>apply_id</c> 是 string(32) 而撤销支是 string(64)
/// —— 本 SDK 的 path 参数按线内惯例统一声明为 <c>string</c>，MAA-B1 锁的是<b>位置与顺序</b>不是 CLR 宽度。
/// </para>
/// </remarks>
public class WechatPayMerchantAlterApplyContractGuards
{
    private const string RegistryGroupName = "MerchantAlterApply";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MerchantAlterApply";

    private const string Prefix = "/v3/mchalterapply/";

    private const string SubjectRoute = Prefix + "mchsubjectalterapplyment";

    private const string ContactRoute = Prefix + "mchcontactalterapplyment";

    private const string MerchantNameRoute = Prefix + "merchantnamealterapplyment";

    /// <summary>
    /// 十二支各自的官方出处（2026-10-14 逐页核验所得）。<b>联系人族四行在缺口清单里挂的是主体族四页</b>
    /// ⇒ 本名册是「方法 ↔ docId」配对的权威登记表，MAA-B2 逐对锁定。
    /// </summary>
    private static readonly (string Method, string DocId)[] MethodDocPairs =
    {
        (nameof(IWechatPayMerchantAlterApplyService.SubmitSubjectAlterApplyAsync), "4014090649"),
        (nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByApplyIdAsync), "4014090655"),
        (nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByOutRequestNoAsync), "4014090667"),
        (nameof(IWechatPayMerchantAlterApplyService.RevokeSubjectAlterApplyAsync), "4014090672"),
        (nameof(IWechatPayMerchantAlterApplyService.SubmitContactAlterApplyAsync), "4015617906"),
        (nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByApplyIdAsync), "4015617911"),
        (nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByOutRequestNoAsync), "4015617915"),
        (nameof(IWechatPayMerchantAlterApplyService.RevokeContactAlterApplyAsync), "4015617919"),
        (nameof(IWechatPayMerchantAlterApplyService.SubmitMerchantNameAlterApplyAsync), "4016166071"),
        (nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByApplyIdAsync), "4016166074"),
        (nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByOutRequestNoAsync), "4016166085"),
        (nameof(IWechatPayMerchantAlterApplyService.RevokeMerchantNameAlterApplyAsync), "4016166090"),
    };

    /// <summary>联系人（超级管理员）族四支：出处必须落在 40156179xx，不得沿用主体族四页。</summary>
    private static readonly string[] ContactFamilyMethods = MethodDocPairs
        .Where(static p => p.Method.Contains("Contact", StringComparison.Ordinal))
        .Select(static p => p.Method)
        .ToArray();

    /// <summary>
    /// MAA-B1：12 端点的方法与路由照官方原文（三族各四条）；<b>本域零 204</b>（撤销支是 200 + JSON）；
    /// path 参数位置与顺序逐支锁定；每族两支查询共用同一应答类型（三族之间各建一型，见 MAA-B6）。
    /// </summary>
    [Fact]
    public void MerchantAlterApplyEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentNowhere()
    {
        // 主体族四支
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantAlterApplyService.SubmitSubjectAlterApplyAsync), SubjectRoute);
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByApplyIdAsync),
            SubjectRoute + "/{applyId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByOutRequestNoAsync),
            SubjectRoute + "/merchant/{merchantCode}/out-request-no/{outRequestNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantAlterApplyService.RevokeSubjectAlterApplyAsync),
            SubjectRoute + "/{applyId}/revoke");

        // 超级管理员族四支
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantAlterApplyService.SubmitContactAlterApplyAsync), ContactRoute);
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByApplyIdAsync),
            ContactRoute + "/{applyId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByOutRequestNoAsync),
            ContactRoute + "/merchant/{merchantCode}/out-request-no/{outRequestNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantAlterApplyService.RevokeContactAlterApplyAsync),
            ContactRoute + "/{applyId}/revoke");

        // 商家简称族四支（定位键是 sub_mchid，路径字面段仍是 /merchant/）
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantAlterApplyService.SubmitMerchantNameAlterApplyAsync), MerchantNameRoute);
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByApplyIdAsync),
            MerchantNameRoute + "/{applyId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByOutRequestNoAsync),
            MerchantNameRoute + "/merchant/{subMchId}/out-request-no/{outRequestNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantAlterApplyService.RevokeMerchantNameAlterApplyAsync),
            MerchantNameRoute + "/{applyId}/revoke");

        var routes = Routes();
        routes.Should().HaveCount(12, "三族各四支 = 12 端点；数量变化须同批更新 MAA-B1 与 Tests/ContractBaseline.cs");
        DistinctVerbRoutes().Should().HaveCount(12, "十二条 (方法, 路由) 各不相同：本域没有同方法同路由的多形态分身");
        routes.Distinct().Should().HaveCount(12, "三族前缀互不相同 ⇒ 路由字符串去重后仍是 12");

        // 动词面：本域只有 GET 与 POST，PUT/PATCH/DELETE 一支都没有（撤销走 POST + 动词段）。
        FindMethod(nameof(IWechatPayMerchantAlterApplyService.RevokeSubjectAlterApplyAsync))
            .GetCustomAttribute<HttpMethodAttribute>()
            .Should().BeAssignableTo<PostAttribute>(
                "官方撤销是 POST …/{apply_id}/revoke，不是 DELETE：照 REST 直觉改动词即 404");
        Verbs().Should().BeEquivalentTo(new[] { nameof(PostAttribute), nameof(GetAttribute) });

        // 本域零 204：撤销支成功是 200 + JSON（result 或 {apply_id, state}），不是无包体。
        NoContentMethods().Should().BeEmpty(
            "本域十二支的官方成功应答全是 200 + JSON：出现裸 Task 即丢掉 result/state");

        // path 形态：三支提交无 path；两支「按申请单号」各一个；三支撤销各一个；三支「按业务单号」两个且顺序固定。
        PathKeysOf(nameof(IWechatPayMerchantAlterApplyService.SubmitSubjectAlterApplyAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayMerchantAlterApplyService.SubmitContactAlterApplyAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayMerchantAlterApplyService.SubmitMerchantNameAlterApplyAsync)).Should().BeEmpty();
        foreach (var name in new[]
                 {
                     nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByApplyIdAsync),
                     nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByApplyIdAsync),
                     nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByApplyIdAsync),
                     nameof(IWechatPayMerchantAlterApplyService.RevokeSubjectAlterApplyAsync),
                     nameof(IWechatPayMerchantAlterApplyService.RevokeContactAlterApplyAsync),
                     nameof(IWechatPayMerchantAlterApplyService.RevokeMerchantNameAlterApplyAsync),
                 })
        {
            PathKeysOf(name).Should().BeEquivalentTo(new[] { "applyId" }, $"{name}：只以 apply_id 定位");
        }

        PathKeysOf(nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByOutRequestNoAsync))
            .Should().BeEquivalentTo(new[] { "merchantCode", "outRequestNo" });
        PathKeysOf(nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByOutRequestNoAsync))
            .Should().BeEquivalentTo(new[] { "merchantCode", "outRequestNo" });
        PathKeysOf(nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByOutRequestNoAsync))
            .Should().BeEquivalentTo(new[] { "subMchId", "outRequestNo" },
                "简称族的业务单号查询用 sub_mchid 定位：照抄另两族的 merchantCode 即 400 PARAM_ERROR");

        // query 形态：十二支全部无 query 参数（本域定位只走 path 与包体）。
        Queries().Should().BeEmpty("本域十二页均无 QUERY 参数表行：加游标即造出官方忽略的查询条件");

        // 包体形态：三支提交各有自己的请求类型，其余九支无包体。
        BodyTypeOf(nameof(IWechatPayMerchantAlterApplyService.SubmitSubjectAlterApplyAsync))
            .Should().Be(typeof(MerchantAlterApplySubjectSubmitRequest));
        BodyTypeOf(nameof(IWechatPayMerchantAlterApplyService.SubmitContactAlterApplyAsync))
            .Should().Be(typeof(MerchantAlterApplyContactSubmitRequest));
        BodyTypeOf(nameof(IWechatPayMerchantAlterApplyService.SubmitMerchantNameAlterApplyAsync))
            .Should().Be(typeof(MerchantAlterApplyMerchantNameSubmitRequest));
        BodyMethods().Should().BeEquivalentTo(new[]
        {
            nameof(IWechatPayMerchantAlterApplyService.SubmitSubjectAlterApplyAsync),
            nameof(IWechatPayMerchantAlterApplyService.SubmitContactAlterApplyAsync),
            nameof(IWechatPayMerchantAlterApplyService.SubmitMerchantNameAlterApplyAsync),
        }, "只有提交支带包体：GET 与撤销支带体即与官方参数表冲突");
    }

    /// <summary>
    /// MAA-B2：路由全在 <c>/v3/mchalterapply/</c> 前缀下 + <b>「方法 ↔ docId」逐支配对</b>
    /// （收口清单把主体族四页 docId 挂在联系人族四行上的出处错配）+ 两处零声明
    /// （按业务单号撤销、进件族两前缀），正对照取本域已落位路由以证扫描机制生效。
    /// </summary>
    [Fact]
    public void MerchantAlterApplyRoutes_ShouldStayOnOwnPrefixAndPairEachMethodWithItsOwnDocId()
    {
        var routes = Routes();

        routes.Should().HaveCount(12);
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(12, "本域十二支全在 /v3/mchalterapply/ 下，无第二前缀");

        // 三族前缀齐备且各四支（防止有人把某族三支并成一支）。
        routes.Count(static r => r.StartsWith(SubjectRoute, StringComparison.Ordinal)).Should().Be(4);
        routes.Count(static r => r.StartsWith(ContactRoute, StringComparison.Ordinal)).Should().Be(4);
        routes.Count(static r => r.StartsWith(MerchantNameRoute, StringComparison.Ordinal)).Should().Be(4);

        // 出处错配收口：十二支各自的「官方文档」注释必须指向**本族**的 docId。联系人族四行在清单里
        // 挂的是主体族四页（4014090649 / 655 / 667 / 672），而那四页正文只文档 mchsubjectalterapplyment
        // ⇒ 路由字符串抄对了、出处抄错了；本段把「方法 ↔ docId」逐对钉住，防止后来者按错页复核字段表。
        var docSource = MerchantAlterApplyInterfaceSource();
        docSource.Should().Contain(SubjectRoute, "正对照：接口源文本可读（主体族路由确在其中）");
        docSource.Should().Contain(ContactRoute, "正对照：联系人族现行路由确在其中");
        foreach (var pair in MethodDocPairs)
        {
            DocWindowOf(docSource, pair.Method).Should().Contain(pair.DocId,
                $"{pair.Method} 的官方出处必须是 {pair.DocId}");
        }

        foreach (var method in ContactFamilyMethods)
        {
            DocWindowOf(docSource, method).Should().NotContain("40140906",
                $"{method} 属联系人（超级管理员）族：沿用主体族四页 docId 即清单的原始错配回潮");
        }

        // 零声明②：撤销只有一条定位路径（apply_id），官方无「按业务单号撤销」。
        routes.Where(static r => r.EndsWith("/revoke", StringComparison.Ordinal))
            .Should().OnlyContain(static r => r.Contains("/{applyId}/revoke", StringComparison.Ordinal),
            "三族撤销页的 PATH 只有 apply_id：伪造 out_request_no 撤销即造出官方不认的端点");
        routes.Count(static r => r.EndsWith("/revoke", StringComparison.Ordinal)).Should().Be(3,
            "撤销支恰三支（每族一支）");

        // 零声明③：本域不含进件族路由（/v3/applyment4sub、/v3/applyment4subject 是另两个域）。
        routes.Should().NotContain(static r => r.Contains("applyment4sub", StringComparison.Ordinal)
                                               || r.Contains("applyment4subject", StringComparison.Ordinal),
            "变更域与进件域是三套契约：跨域抄路由即把端点挂错域");
        RoutesOf(typeof(IWechatPayApply4SubMerchantService)).Concat(RoutesOf(typeof(IWechatPayApply4SubjectService)))
            .Intersect(routes).Should().BeEmpty("正对照：进件两域的路由与本域交集为空（域边界成立）");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", RegistryGroupName))
            .Should().BeTrue("正对照：Interfaces/MerchantAlterApply/ 确实已落位");
    }

    /// <summary>
    /// MAA-B3：官方字段名逐项锁定 + 三族定位键分家 + 证件字段两套体系（<c>card_*</c> vs <c>contact_id_*</c>）
    /// + <c>license</c> 美式拼写（本域零 <c>licence</c>）。
    /// </summary>
    [Fact]
    public void MerchantAlterApplyDtos_ShouldExposeOfficialFieldNamesAndPerFamilyKeys()
    {
        // 主体族请求与六棵子树
        JsonNameShouldBe<MerchantAlterApplySubjectSubmitRequest>(nameof(MerchantAlterApplySubjectSubmitRequest.MerchantCode), "merchant_code");
        JsonNameShouldBe<MerchantAlterApplySubjectSubmitRequest>(nameof(MerchantAlterApplySubjectSubmitRequest.OutRequestNo), "out_request_no");
        JsonNameShouldBe<MerchantAlterApplySubjectSubmitRequest>(nameof(MerchantAlterApplySubjectSubmitRequest.AlterScope), "alter_scope");
        JsonNameShouldBe<MerchantAlterApplySubjectSubmitRequest>(nameof(MerchantAlterApplySubjectSubmitRequest.OrganizationType), "organization_type");
        JsonNameShouldBe<MerchantAlterApplySubjectSubmitRequest>(nameof(MerchantAlterApplySubjectSubmitRequest.FinanceInstitution), "finance_institution");
        JsonNameShouldBe<MerchantAlterApplySubjectSubmitRequest>(nameof(MerchantAlterApplySubjectSubmitRequest.UboInfoList), "ubo_info_list");

        JsonNameShouldBe<MerchantAlterApplySubjectBusinessLicenseInfo>(nameof(MerchantAlterApplySubjectBusinessLicenseInfo.LicenseNumber), "license_number");
        JsonNameShouldBe<MerchantAlterApplySubjectBusinessLicenseInfo>(nameof(MerchantAlterApplySubjectBusinessLicenseInfo.LicenseCopy), "license_copy");
        JsonNameShouldBe<MerchantAlterApplySubjectBusinessLicenseInfo>(nameof(MerchantAlterApplySubjectBusinessLicenseInfo.LicensePeriodBegin), "license_period_begin");
        JsonNameShouldBe<MerchantAlterApplySubjectBusinessLicenseInfo>(nameof(MerchantAlterApplySubjectBusinessLicenseInfo.LicensePeriodEnd), "license_period_end");
        JsonNameShouldBe<MerchantAlterApplySubjectCertificateInfo>(nameof(MerchantAlterApplySubjectCertificateInfo.CertType), "cert_type");
        JsonNameShouldBe<MerchantAlterApplySubjectCertificateInfo>(nameof(MerchantAlterApplySubjectCertificateInfo.CertNumber), "cert_number");
        JsonNameShouldBe<MerchantAlterApplySubjectCertificateInfo>(nameof(MerchantAlterApplySubjectCertificateInfo.CertPeriodBegin), "cert_period_begin");
        JsonNameShouldBe<MerchantAlterApplySubjectLegalPersonInfo>(nameof(MerchantAlterApplySubjectLegalPersonInfo.IdHolderType), "id_holder_type");
        JsonNameShouldBe<MerchantAlterApplySubjectLegalPersonInfo>(nameof(MerchantAlterApplySubjectLegalPersonInfo.IdDocType), "id_doc_type");
        JsonNameShouldBe<MerchantAlterApplySubjectLegalPersonInfo>(nameof(MerchantAlterApplySubjectLegalPersonInfo.AuthorizeLetterCopy), "authorize_letter_copy");
        JsonNameShouldBe<MerchantAlterApplySubjectLegalPersonInfo>(nameof(MerchantAlterApplySubjectLegalPersonInfo.CardNumber), "card_number");
        JsonNameShouldBe<MerchantAlterApplySubjectLegalPersonInfo>(nameof(MerchantAlterApplySubjectLegalPersonInfo.AsUbo), "as_ubo");
        JsonNameShouldBe<MerchantAlterApplySubjectFinanceInstitutionInfo>(nameof(MerchantAlterApplySubjectFinanceInstitutionInfo.FinanceType), "finance_type");
        JsonNameShouldBe<MerchantAlterApplySubjectFinanceInstitutionInfo>(nameof(MerchantAlterApplySubjectFinanceInstitutionInfo.FinanceLicensePics), "finance_license_pics");
        JsonNameShouldBe<MerchantAlterApplySubjectAddition>(nameof(MerchantAlterApplySubjectAddition.BankOpenaccountLicense), "bank_openaccount_license");
        JsonNameShouldBe<MerchantAlterApplySubjectAddition>(nameof(MerchantAlterApplySubjectAddition.OpenaccountApproval), "openaccount_approval");

        // 超管族证件体系是 contact_id_*，与主体族 card_* 分家
        JsonNameShouldBe<MerchantAlterApplyContactInfo>(nameof(MerchantAlterApplyContactInfo.ContactName), "contact_name");
        JsonNameShouldBe<MerchantAlterApplyContactInfo>(nameof(MerchantAlterApplyContactInfo.ContactIdDocType), "contact_id_doc_type");
        JsonNameShouldBe<MerchantAlterApplyContactInfo>(nameof(MerchantAlterApplyContactInfo.ContactIdCardNumber), "contact_id_card_number");
        JsonNameShouldBe<MerchantAlterApplyContactInfo>(nameof(MerchantAlterApplyContactInfo.ContactIdDocCopy), "contact_id_doc_copy");
        JsonNameShouldBe<MerchantAlterApplyContactInfo>(nameof(MerchantAlterApplyContactInfo.ContactIdDocCopyBack), "contact_id_doc_copy_back");
        JsonNameShouldBe<MerchantAlterApplyContactInfo>(nameof(MerchantAlterApplyContactInfo.ContactIdDocPeriodBegin), "contact_id_doc_period_begin");
        JsonNameShouldBe<MerchantAlterApplyContactInfo>(nameof(MerchantAlterApplyContactInfo.MobilePhone), "mobile_phone");

        // 简称族：定位键 sub_mchid + 明文简称
        JsonNameShouldBe<MerchantAlterApplyMerchantNameSubmitRequest>(nameof(MerchantAlterApplyMerchantNameSubmitRequest.SubMchId), "sub_mchid");
        JsonNameShouldBe<MerchantAlterApplyMerchantNameSubmitRequest>(nameof(MerchantAlterApplyMerchantNameSubmitRequest.MerchantShortname), "merchant_shortname");

        // 应答侧：三族查询 + 两种撤销 + 共用提交应答
        JsonNameShouldBe<MerchantAlterApplySubjectQueryResponse>(nameof(MerchantAlterApplySubjectQueryResponse.AuditRejectDetail), "audit_reject_detail");
        JsonNameShouldBe<MerchantAlterApplyAuditRejectDetail>(nameof(MerchantAlterApplyAuditRejectDetail.ParamName), "param_name");
        JsonNameShouldBe<MerchantAlterApplyAuditRejectDetail>(nameof(MerchantAlterApplyAuditRejectDetail.RejectReason), "reject_reason");
        JsonNameShouldBe<MerchantAlterApplySubjectRevokeResponse>(nameof(MerchantAlterApplySubjectRevokeResponse.Result), "result");
        JsonNameShouldBe<MerchantAlterApplyRevokeResponse>(nameof(MerchantAlterApplyRevokeResponse.State), "state");
        JsonNameShouldBe<MerchantAlterApplySubmitResponse>(nameof(MerchantAlterApplySubmitResponse.ApplyId), "apply_id");

        // 请求面集合断言：三族提交请求各自封闭，键名分家
        DeclaredJsonNamesOf<MerchantAlterApplySubjectSubmitRequest>().Should().BeEquivalentTo(new[]
        {
            "merchant_code", "out_request_no", "alter_scope", "organization_type", "finance_institution",
            "business_license_info", "certificate_info", "finance_institution_info", "legal_person_info",
            "ubo_info_list", "addition",
        });
        DeclaredJsonNamesOf<MerchantAlterApplyContactSubmitRequest>()
            .Should().BeEquivalentTo(new[] { "merchant_code", "out_request_no", "contact_info" });
        DeclaredJsonNamesOf<MerchantAlterApplyMerchantNameSubmitRequest>()
            .Should().BeEquivalentTo(new[] { "sub_mchid", "out_request_no", "merchant_shortname" },
                "简称族请求三字段全明文：抄另两族的 merchant_code 或加加密字段即 400");
        DeclaredJsonNamesOf<MerchantAlterApplyContactInfo>().Should().NotContain("mobile_phone_number",
            "手机号官方键名是 mobile_phone（无 _number 尾）：补齐直觉命名会静默丢字段");

        // 应答面集合断言：简称族无逐项明细（主体/超管族有）
        DeclaredJsonNamesOf<MerchantAlterApplySubjectQueryResponse>().Should().BeEquivalentTo(new[]
        {
            "merchant_code", "apply_id", "out_request_no", "state", "audit_reject_reason", "audit_reject_detail",
        });
        DeclaredJsonNamesOf<MerchantAlterApplyContactQueryResponse>().Should().BeEquivalentTo(new[]
        {
            "apply_id", "out_request_no", "merchant_code", "state", "audit_reject_reason", "audit_reject_detail",
        });
        DeclaredJsonNamesOf<MerchantAlterApplyMerchantNameQueryResponse>().Should().BeEquivalentTo(new[]
        {
            "apply_id", "out_request_no", "sub_mchid", "state", "audit_reject_reason",
        }, "简称族应答没有 audit_reject_detail：三族查询应答共用一型即造出永不返回的字段");

        // 全域名单形态：全小写下划线、零驼峰、零 licence 英式拼写（本域用美式 license）
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotBeEmpty("参照集为空会让下面的形态断言静默恒真");
        allNames.Should().NotContain(static n => n.Any(static c => char.IsUpper(c)),
            "官方名单全小写：出现大写字母即说明有人把 snake_case 驼峰化了");
        allNames.Should().NotContain(static n => n.Contains("licence", StringComparison.Ordinal),
            "本域是美式 license（进件域才是 licence）：跨域「统一」拼写即静默错参");
        allNames.Should().Contain(static n => n == "license_number",
            "正对照：本域确实用 license 体系，上面的零 licence 断言不是空集恒真");
        allNames.Should().NotContain(static n => n == "identification_name" || n == "identification_number",
            "证件字段在本域是 card_*（进件域才是 identification_*）");

        foreach (var type in DomainTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset),
                    $"{type.Name}.{property.Name} 不得用 DateTimeOffset（官方日期是 yyyy-MM-dd 字符串且可填「长期」）");
                property.PropertyType.Should().NotBe(typeof(DateTime));
            }
        }
    }

    /// <summary>
    /// MAA-B4：三族状态值集（6 / 5 / 5）+ 变更范围与证件类型值集 + 常量类名册恰 9 个。
    /// </summary>
    [Fact]
    public void MerchantAlterApplyValueSets_ShouldMatchOfficialBulletListsPerFamily()
    {
        ValuesOf(typeof(MerchantAlterApplySubjectStates)).Should().BeEquivalentTo(new[]
        {
            "APPLYMENT_STATE_EDITTING", "APPLYMENT_STATE_AUDITING", "APPLYMENT_STATE_REJECTED",
            "APPLYMENT_STATE_MODIFING", "APPLYMENT_STATE_FINISHED", "APPLYMENT_STATE_CANCELED",
        }, "主体族 state 六值（页内「可选取值」逐字，含三 T 的 EDITTING）");

        ValuesOf(typeof(MerchantAlterApplyContactStates)).Should().BeEquivalentTo(new[]
        {
            "APPLYMENT_STATE_AUDITING", "APPLYMENT_STATE_REJECTED", "APPLYMENT_STATE_MODIFING",
            "APPLYMENT_STATE_FINISHED", "APPLYMENT_STATE_CANCELED",
        }, "超管族 state 五值：无 EDITTING 档 ⇒ 与主体族分建一型的直接依据");

        ValuesOf(typeof(MerchantAlterApplyMerchantNameStates)).Should().BeEquivalentTo(new[]
        {
            "APPLYMENT_STATE_AUDITING", "APPLYMENT_STATE_REJECTED", "APPLYMENT_STATE_MODIFING",
            "APPLYMENT_STATE_FINISHED", "APPLYMENT_STATE_CANCELED",
        }, "简称族 state 查询页印五值（撤销页只印 CANCELED 一值，按文档特例不建窄枚举）");

        // 值集相同的两族仍各建一类：合并后任一页扩充取值会静默放宽另两族断言。
        ValuesOf(typeof(MerchantAlterApplyContactStates)).Should().BeEquivalentTo(
            ValuesOf(typeof(MerchantAlterApplyMerchantNameStates)),
            "超管族与简称族的值集当前相同（这正是「值集相同仍分族建类」的口径样例）");
        ValuesOf(typeof(MerchantAlterApplySubjectStates)).Should().Contain("APPLYMENT_STATE_EDITTING");
        ValuesOf(typeof(MerchantAlterApplyContactStates)).Should().NotContain("APPLYMENT_STATE_EDITTING");

        // 两处官方误拼照页：修正拼写即与应答值不匹配
        ValuesOf(typeof(MerchantAlterApplySubjectStates)).Should().NotContain(new[]
        {
            "APPLYMENT_STATE_EDITING", "APPLYMENT_STATE_MODIFYING",
        }, "官方原文是 EDITTING（三 T）与 MODIFING（单 F）：「纠正」拼写等于伪造取值");

        ValuesOf(typeof(MerchantAlterApplyScopes)).Should().BeEquivalentTo(new[]
        {
            "ALTER_SCOPE_FULL", "ALTER_SCOPE_BUSINESS_CERT", "ALTER_SCOPE_UBO",
        }, "alter_scope 三值：留空的官方缺省是「变更全部主体资料」，不是「不改动」");

        ValuesOf(typeof(MerchantAlterApplySubjectTypes)).Should().HaveCount(6)
            .And.Contain("SUBJECT_TYPE_INSTITUTIONS_CLONED",
                "事业单位的官方取值带 _CLONED 后缀（进件枚举复制残留），照页");

        ValuesOf(typeof(MerchantAlterApplyFinanceTypes)).Should().HaveCount(6);
        ValuesOf(typeof(MerchantAlterApplyIdHolderTypes)).Should().BeEquivalentTo(new[] { "LEGAL", "SUPER" });
        ValuesOf(typeof(MerchantAlterApplyRevokeResults)).Should().BeEquivalentTo(new[] { "SUCCESS", "FAIL" });

        // 证件类型值集两族共用一个常量类（字段名不同、值集相同）
        ValuesOf(typeof(MerchantAlterApplyIdDocTypes)).Should().HaveCount(8,
            "id_doc_type 与 contact_id_doc_type 的官方取值清单相同 ⇒ 共用一类，字段名各 DTO 承载");
        ValuesOf(typeof(MerchantAlterApplyIdDocTypes)).Should().Contain(new[]
        {
            "IDENTIFICATION_TYPE_IDCARD", "IDENTIFICATION_TYPE_OVERSEA_PASSPORT",
            "IDENTIFICATION_TYPE_FOREIGN_RESIDENT",
        });

        ConstantClassNames().Should().HaveCount(9,
            "本域常量类恰 9 个（三族 state + scope + 主体类型 + 金融机构类型 + 持有人类型 + 证件类型 + 撤销结果）");
        ConstantClassNames().Should().NotContain(static n => n.Contains("CertType", StringComparison.Ordinal),
            "cert_type 的官方清单含两个「已废弃」值且描述内联印在页内 ⇒ 按 MAA-B5 不建模为常量类");
    }

    /// <summary>
    /// MAA-B5：「官方未给 / 不建模」裁决的可机械化部分 —— 撤销失败是 200 + <c>result=FAIL</c>、
    /// 条件必填不在 SDK 校验、加密面只覆盖两支、简称族全明文。
    /// </summary>
    [Fact]
    public void MerchantAlterApplyUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndPerFamilyEncrypted()
    {
        // 不建模条件校验：六个资料对象官方全标选填，条件写在描述里（八种组合）⇒ SDK 不猜。
        typeof(MerchantAlterApplySubjectSubmitRequest)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().NotContain(static p => p.CustomAttributes.Any(static a =>
                    a.AttributeType.Name.Contains("Required", StringComparison.Ordinal)),
                "条件必填由官方以 PARAM_ERROR 判定：SDK 侧加必填闸会在「仅变更受益人」路径上误拒");

        // 加密面只覆盖两支：主体族的 legal_person_info / ubo_info_list 与超管族的 contact_info 承载密文字符串，
        // 简称族零加密字段 ⇒ 不得给简称支配 Wechatpay-Serial。
        DeclaredJsonNamesOf<MerchantAlterApplySubjectLegalPersonInfo>()
            .Should().Contain(new[] { "card_name", "card_number", "card_address" },
            "三字段官方注明须加密：DTO 只承载密文 string");
        PropertyTypeOf<MerchantAlterApplySubjectLegalPersonInfo>(nameof(MerchantAlterApplySubjectLegalPersonInfo.CardNumber))
            .Should().Be(typeof(string), "密文仍是 string：建成任何结构化类型即与官方口径冲突");
        DeclaredJsonNamesOf<MerchantAlterApplyMerchantNameSubmitRequest>()
            .Should().NotContain(static n => n.Contains("copy", StringComparison.Ordinal)
                                             || n.Contains("number", StringComparison.Ordinal),
            "简称族请求只有三个明文字段：抄证件子树即造出官方不认的载荷");

        // 撤销的失败态是业务结论而不是 HTTP 错误：应答类型必须有 result（主体族）或 state（另两族）。
        DeclaredJsonNamesOf<MerchantAlterApplySubjectRevokeResponse>().Should().BeEquivalentTo(new[] { "result" });
        FindMethod(nameof(IWechatPayMerchantAlterApplyService.RevokeSubjectAlterApplyAsync)).ReturnType
            .Should().NotBe(typeof(Task), "官方撤销成功是 200 + {result}：裸 Task 会把 FAIL 当成功");

        // 本域不含列表 / 分页 / 删除面：类型名不得出现这些分身
        DomainTypeNames().Should().NotContain(static n => n.Contains("List", StringComparison.Ordinal)
                                                         || n.Contains("Page", StringComparison.Ordinal)
                                                         || n.Contains("Delete", StringComparison.Ordinal),
            "官方十二页无变更单列表与删除端点：伪造即造出永远为空的假类型");

        // 「资料内容」类字段在本域是对象（进件域的 submit_data 字符串形态属另一域）：本域不得出现该键
        DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray()
            .Should().NotContain(static n => n == "submit_data" || n == "requested_item_info" || n == "submission_data",
            "那是商户管理记录 / 强化尽调域的字符串字段：跨域抄用即建错类型");
    }

    /// <summary>
    /// MAA-B6：判错面继承 + 三族的共用/分建裁决（提交三族共用、撤销两族共用、查询三族三型、明细项两族共用）。
    /// </summary>
    [Fact]
    public void MerchantAlterApplyResponses_ShouldDeriveFromBaseAndFollowPerTypeSharingRulings()
    {
        foreach (var responseType in new[]
                 {
                     typeof(MerchantAlterApplySubmitResponse),
                     typeof(MerchantAlterApplyRevokeResponse),
                     typeof(MerchantAlterApplySubjectRevokeResponse),
                     typeof(MerchantAlterApplySubjectQueryResponse),
                     typeof(MerchantAlterApplyContactQueryResponse),
                     typeof(MerchantAlterApplyMerchantNameQueryResponse),
                 })
        {
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        foreach (var requestType in new[]
                 {
                     typeof(MerchantAlterApplySubjectSubmitRequest),
                     typeof(MerchantAlterApplyContactSubmitRequest),
                     typeof(MerchantAlterApplyMerchantNameSubmitRequest),
                     typeof(MerchantAlterApplyContactInfo),
                     typeof(MerchantAlterApplySubjectAddition),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求类型，不得挂在应答基类上");
        }

        // 共用①：三族提交应答同一 CLR 类型（三页应答表只有 apply_id 一行且同必填性）
        var submitReturnType = FindMethod(nameof(IWechatPayMerchantAlterApplyService.SubmitSubjectAlterApplyAsync)).ReturnType;
        submitReturnType.Should().Be(typeof(Task<MerchantAlterApplySubmitResponse>));
        FindMethod(nameof(IWechatPayMerchantAlterApplyService.SubmitContactAlterApplyAsync)).ReturnType.Should().Be(submitReturnType);
        FindMethod(nameof(IWechatPayMerchantAlterApplyService.SubmitMerchantNameAlterApplyAsync)).ReturnType.Should().Be(submitReturnType);

        // 共用②：超管族与简称族的撤销应答同一类型；主体族分建
        var contactRevoke = FindMethod(nameof(IWechatPayMerchantAlterApplyService.RevokeContactAlterApplyAsync)).ReturnType;
        contactRevoke.Should().Be(FindMethod(nameof(IWechatPayMerchantAlterApplyService.RevokeMerchantNameAlterApplyAsync)).ReturnType,
            "两页撤销应答表相同（apply_id + state，均选填 string）⇒ 共用一型");
        contactRevoke.Should().NotBe(FindMethod(nameof(IWechatPayMerchantAlterApplyService.RevokeSubjectAlterApplyAsync)).ReturnType,
            "主体族撤销回 {result}：并入共用型即丢字段");

        // 分建：三族查询应答三型（值集与字段缺失是判据）
        var queryTypes = new[]
        {
            FindMethod(nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByApplyIdAsync)).ReturnType,
            FindMethod(nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByApplyIdAsync)).ReturnType,
            FindMethod(nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByApplyIdAsync)).ReturnType,
        }.Distinct().ToArray();
        queryTypes.Should().HaveCount(3, "三族查询应答三型分建：字段名相同但 state 值清单或字段集合不同（守卫 MAA-B3 锁集合）");

        // 族内共用：每族「按申请单号查」与「按业务单号查」同型（同族两页应答表逐字段相同）
        FindMethod(nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByOutRequestNoAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantAlterApplyService.QuerySubjectAlterApplyByApplyIdAsync)).ReturnType);
        FindMethod(nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByOutRequestNoAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantAlterApplyService.QueryContactAlterApplyByApplyIdAsync)).ReturnType);
        FindMethod(nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByOutRequestNoAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantAlterApplyService.QueryMerchantNameAlterApplyByApplyIdAsync)).ReturnType);

        // 共用③：驳回明细项类型被主体族与超管族共用（两页该列表字段表逐字符相同）
        PropertyTypeOf<MerchantAlterApplySubjectQueryResponse>(nameof(MerchantAlterApplySubjectQueryResponse.AuditRejectDetail))
            .Should().Be(PropertyTypeOf<MerchantAlterApplyContactQueryResponse>(nameof(MerchantAlterApplyContactQueryResponse.AuditRejectDetail)),
            "两族 audit_reject_detail 的项类型必须同一引用");
        PropertyTypeOf<MerchantAlterApplySubjectQueryResponse>(nameof(MerchantAlterApplySubjectQueryResponse.AuditRejectDetail))
            .Should().Be(typeof(List<MerchantAlterApplyAuditRejectDetail>));

        // 证件体系分家：主体族的 card_* 类型与超管族的 contact_id_* 类型不是同一个类
        PropertyTypeOf<MerchantAlterApplySubjectSubmitRequest>(nameof(MerchantAlterApplySubjectSubmitRequest.LegalPersonInfo))
            .Should().NotBe(typeof(MerchantAlterApplyContactInfo),
            "两族证件字段体系不同（card_* vs contact_id_*）：复用即静默错参");

        // 与进件域零复用：本域类型名不得带进件前缀
        DomainTypeNames().Should().NotContain(static n => n.StartsWith("Apply4Sub", StringComparison.Ordinal)
                                                         || n.StartsWith("Apply4Subject", StringComparison.Ordinal),
            "进件族嵌套对象不得复用（方案 §2.7 四-10）：本域一律自建 MerchantAlterApply* 类型");
    }

    /// <summary>
    /// MAA-B7：本域 17 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void MerchantAlterApplyDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(17,
            "变更 DTO：主体文件 10（请求 + 六棵子树 + 查询应答 + 撤销应答 + 驳回明细）+ 公共文件 2（共用提交/撤销应答）"
            + "+ 超管文件 3（请求 + 证件对象 + 查询应答）+ 简称文件 2（请求 + 查询应答）= 17");

        foreach (var type in domainTypes)
        {
            MerchantAlterApplyJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MerchantAlterApplyJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（本域类型 = 17 DTO + 9 常量类）
        DomainTypeNames().Should().HaveCount(26, "本域类型 = 17 DTO + 9 常量类");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", 
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MerchantAlterApplyJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MerchantAlterApply 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMerchantAlterApplyWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayMerchantAlterApplyService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayMerchantAlterApplyService)
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

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayMerchantAlterApplyService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayMerchantAlterApplyService.{methodName} 必须存在");

    private static string[] Routes() => RoutesOf(typeof(IWechatPayMerchantAlterApplyService));

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static (string Verb, string Route)[] DistinctVerbRoutes()
        => typeof(IWechatPayMerchantAlterApplyService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Select(static a => (a.GetType().Name, a.RequestUri!)))
            .Distinct()
            .ToArray();

    private static string[] Verbs()
        => typeof(IWechatPayMerchantAlterApplyService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name)
            .Distinct()
            .ToArray();

    private static string[] NoContentMethods()
        => typeof(IWechatPayMerchantAlterApplyService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();

    /// <summary>
    /// 带包体的方法。本线 P0-7 各域**不写** <c>[Body]</c>（体由「非 path / 非 query / 非取消令牌」的
    /// 实参推断，同 <see cref="WechatPayMerchantStoreContractGuards"/>），故谓词与 <c>BodyTypeOf</c> 同形
    /// —— 若按 <c>[Body]</c> 特性筛，本域会恒空并让「只有提交支带包体」的断言静默通过。
    /// </summary>
    private static string[] BodyMethods()
        => typeof(IWechatPayMerchantAlterApplyService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.GetParameters().Any(static p => p.GetCustomAttribute<PathAttribute>() == null
                                                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                                                 && p.ParameterType != typeof(CancellationToken)))
            .Select(static m => m.Name)
            .ToArray();

    private static string[] Queries()
        => typeof(IWechatPayMerchantAlterApplyService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetParameters()
                .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>()))
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static Type BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Single(static p => p.GetCustomAttribute<PathAttribute>() == null
                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                 && p.ParameterType != typeof(CancellationToken))
            .ParameterType;

    private static IEnumerable<Type> DomainTypes()
        => typeof(MerchantAlterApplySubjectSubmitRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(MerchantAlterApplySubjectSubmitRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(MerchantAlterApplySubjectSubmitRequest).Assembly.GetTypes()
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

    /// <summary>本域接口源文件全文（「方法 ↔ docId」配对的读取面，只读一次）。</summary>
    private static string MerchantAlterApplyInterfaceSource()
        => File.ReadAllText(SourcePath("Mud.Wechat.Pay",  "Interfaces", RegistryGroupName,
            "IWechatPayMerchantAlterApplyService.cs"));

    /// <summary>
    /// 取某支方法签名之前最近的一行「官方文档：」出处（120 字符窗口足以覆盖整条 <c>see href</c>）。
    /// 取「最近一条」而不是「全文包含」，是因为接口<b>头部注释块把十二个 docId 一次列齐</b>——
    /// 用全文比对时任何一支都会命中任何 docId，配对断言将恒真。
    /// </summary>
    private static string DocWindowOf(string source, string methodName)
    {
        var signatureIndex = source.IndexOf(methodName + "(", StringComparison.Ordinal);
        signatureIndex.Should().BeGreaterThan(0, $"{methodName} 必须声明在接口源文件内");

        var markerIndex = source.LastIndexOf("官方文档：", signatureIndex, StringComparison.Ordinal);
        markerIndex.Should().BeGreaterThan(0,
            $"{methodName} 的注释必须带「官方文档：」出处行（AGENTS §2 加端点即补出处）");

        var length = Math.Min(120, source.Length - markerIndex);
        return source.Substring(markerIndex, length);
    }

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
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
