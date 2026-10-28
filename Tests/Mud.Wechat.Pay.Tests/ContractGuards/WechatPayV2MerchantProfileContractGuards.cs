// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.HttpUtils;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.Tests.ContractGuards;
using Mud.Wechat.Pay.Tests.Credential;
using Mud.Wechat.Pay.V2.DataModels.PayV2;
using Mud.Wechat.Pay.V2.Extensions;
using Mud.Wechat.Pay.V2.Secure;
using Mud.Wechat.Pay.V2.Security;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.ContractGuards;

/// <summary>
/// 微信支付 APIv2「子商户配置 / 小微商户 / 境外机构子商户 / 商户媒体」四域契约守卫（<b>9 端点 = 3 + 2 + 3 + 1</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-18 逐页核验证书栏、域名栏与参数表）：
/// 新增支付目录 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115404"/>、
/// 查询子商户配置 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115413"/>、
/// 新增关注配置 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115399"/>、
/// 修改小微银行资料 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115349"/>、
/// 修改小微联系人 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115350"/>、
/// 境外机构进件 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636642"/>、
/// 修改机构子商户 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636644"/>、
/// 查询机构子商户 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636643"/>、
/// 商户上传图片 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013641477"/>。</para>
/// <para>
/// <b>本守卫锁四件「按通用形状补齐必错」的事</b>：
/// ① <b>第三个域名</b> —— 后两族官方「接口说明」栏只印 <c>apihk.mch</c>，全文不出现 <c>api.mch</c>；
/// 前两族虽要证书却仍挂 <c>api.mch</c>（<c>/secapi/</c> 前缀 ≠ 证书域名）。
/// ② <b>签名面在同域内不统一</b> —— 支付目录两支无 <c>nonce_str</c>／<c>sign_type</c>（落 MD5），
/// 关注配置与小微两支 <c>sign_type</c> 必填且<b>仅</b> HMAC-SHA256，机构进件三支官方「仅支持MD5」。
/// 默认值即契约面，故本守卫逐型断言 <c>sign_type</c> 的默认值与<b>有无</b>。
/// ③ <b>字段名 <c>app_id</c> vs <c>appid</c></b> —— 机构进件族用带下划线的 <c>app_id</c>，其余各族用 <c>appid</c>。
/// ④ <b>唯一的 multipart 端点</b> —— 请求是 <c>multipart/form-data</c>、应答仍是 XML，
/// 文件分节 <c>media</c> 不参与签名（完整性由<b>小写</b> MD5 的 <c>media_hash</c> 承载）。
/// </para>
/// <para>
/// <b>为什么由源文本断言</b>：四族全部手写、不带 <c>[HttpClientApi]</c> ⇒ 反射读不到路由与域名，
/// 与 <see cref="WechatPayV2CertFamilyContractGuards"/> 同一处置（<c>SourcePath</c> + 方法体文本）。
/// </para>
/// </remarks>
public class WechatPayV2MerchantProfileContractGuards
{
    /// <summary>四族接口 ⇒ 官方路由清单（逐支按接口方法声明顺序对齐）。</summary>
    private static readonly Dictionary<Type, string[]> FamilyRoutes = new()
    {
        [typeof(IWechatPayV2SubMerchantConfigService)] = new[]
        {
            "/secapi/mch/addsubdevconfig",
            "/secapi/mch/querysubdevconfig",
            "/secapi/mkt/addrecommendconf",
        },
        [typeof(IWechatPayV2MerchantMicroService)] = new[]
        {
            "/applyment/micro/modifyarchives",
            "/applyment/micro/modifycontactinfo",
        },
        [typeof(IWechatPayV2InstitutionSubService)] = new[]
        {
            "/secapi/mch/addInstitutionsub",
            "/secapi/mch/modifyInstitutionsub",
            "/secapi/mch/queryInstitutionsub",
        },
        [typeof(IWechatPayV2MerchantMediaService)] = new[]
        {
            "/secapi/mch/uploadmedia",
        },
    };

    /// <summary>四族接口 ⇒ 实现类源文件。</summary>
    private static readonly Dictionary<Type, string> ImplementationFiles = new()
    {
        [typeof(IWechatPayV2SubMerchantConfigService)] = "Secure/WechatPayV2SubMerchantConfigService.cs",
        [typeof(IWechatPayV2MerchantMicroService)] = "Secure/WechatPayV2MerchantMicroService.cs",
        [typeof(IWechatPayV2InstitutionSubService)] = "Secure/WechatPayV2InstitutionSubService.cs",
        [typeof(IWechatPayV2MerchantMediaService)] = "Secure/WechatPayV2MerchantMediaService.cs",
    };

    /// <summary>
    /// 挂在 <c>api.mch</c>（<see cref="WechatPayV2HttpClientNames.BaseAddress"/>）上的族：
    /// 子商户配置三支 + 小微两支。它们要证书，但官方域名不是 <c>api2.mch</c>。
    /// </summary>
    private static readonly Type[] ApiMchFamilies =
    {
        typeof(IWechatPayV2SubMerchantConfigService),
        typeof(IWechatPayV2MerchantMicroService),
    };

    /// <summary>挂在 <c>apihk.mch</c>（<see cref="WechatPayV2HttpClientNames.GlobalBaseAddress"/>）上的族。</summary>
    private static readonly Type[] ApiHkFamilies =
    {
        typeof(IWechatPayV2InstitutionSubService),
        typeof(IWechatPayV2MerchantMediaService),
    };

    /// <summary>本批 17 型 DTO（4 族请求 + 应答 + 基类），须逐型登记进 <c>PayV2JsonContext</c>。</summary>
    private static readonly Type[] GuardedDtos =
    {
        typeof(V2SubDevConfigAddRequest), typeof(V2SubDevConfigQueryRequest),
        typeof(V2SubDevConfigAddResponse), typeof(V2SubDevConfigQueryResponse),
        typeof(V2RecommendConfigAddRequest), typeof(V2RecommendConfigAddResponse),
        typeof(V2MicroArchivesModifyRequest), typeof(V2MicroContactInfoModifyRequest),
        typeof(V2MicroModifyResponse),
        typeof(V2InstitutionSubAddRequest), typeof(V2InstitutionSubModifyRequest),
        typeof(V2InstitutionSubQueryRequest), typeof(V2InstitutionSubResponse),
        typeof(V2InstitutionSubAddResponse), typeof(V2InstitutionSubQueryResponse),
        typeof(V2UploadMediaRequest), typeof(V2UploadMediaResponse),
    };

    /// <summary>四族接口必须是<b>无 <c>[HttpClientApi]</c></b> 的普通公共接口，且落线根命名空间。</summary>
    [Fact]
    public void V2_MerchantProfile_Interfaces_ShouldBePlainPublicWithoutHttpClientApi()
    {
        foreach (var family in FamilyRoutes.Keys)
        {
            family.IsPublic.Should().BeTrue($"{family.Name} 是宿主直接注入的公开面");
            family.GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false).Should().BeEmpty(
                $"{family.Name} 承载需商户证书的端点：证书按商户绑定 ⇒ 不得走源生成");
            family.Namespace.Should().Be("Mud.Wechat.Pay.V2",
                "because 可注入接口落线根命名空间（与 IWechatPayV2SecureService 同面）");
            // 合并前还断言「且落 Mud.Wechat.Pay.V2 程序集」；2026-10 包合并后 APIv2/APIv3 同程序集，
            // 该边界的真实承载物是命名空间（上面那条），程序集名断言属 D3 反模式，删除。
        }
    }

    /// <summary>方法集与端点总数不得漂移（本批 9 支：3 + 2 + 3 + 1）。</summary>
    [Fact]
    public void V2_MerchantProfile_MethodSets_ShouldMatchOfficialFamilies()
    {
        foreach (var (interfaceType, routes) in FamilyRoutes)
        {
            var methods = DeclaredMethods(interfaceType);
            methods.Should().HaveCount(routes.Length, $"{interfaceType.Name} 端点数须与官方族页面支数一致");

            foreach (var method in methods)
            {
                method.Name.Should().EndWith("Async", "because 全仓异步方法命名口径");
                method.ReturnType.Should().BeAssignableTo(typeof(Task));

                var token = method.GetParameters().Last();
                token.ParameterType.Should().Be(typeof(CancellationToken));
                token.HasDefaultValue.Should().BeTrue(
                    "because 官方每支都是「请求体 + 可选取消令牌」，缺省值让调用方可省略");
            }
        }

        FamilyRoutes.Sum(f => DeclaredMethods(f.Key).Length).Should().Be(9,
            "because 子商户配置 3 + 小微 2 + 机构进件 3 + 媒体上传 1");
    }

    /// <summary>逐支路由绑定：实现类里每个方法之后的第一条路径字面量必须等于官方路由。</summary>
    [Fact]
    public void V2_MerchantProfile_EachMethod_ShouldBindTheOfficialRoute()
    {
        foreach (var (interfaceType, routes) in FamilyRoutes)
        {
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay", ImplementationFiles[interfaceType]));
            var methods = DeclaredMethods(interfaceType);
            methods.Should().HaveSameCount(routes,
                $"{interfaceType.Name} 的方法数与守卫路由清单须同批（否则 Zip 会静默截断，把漏配的端点藏起来）");

            foreach (var (method, expectedRoute) in methods.Zip(routes))
            {
                var callIndex = text.IndexOf(method.Name + "(", StringComparison.Ordinal);
                callIndex.Should().BeGreaterThanOrEqualTo(0,
                    $"{interfaceType.Name}.{method.Name} 在 {ImplementationFiles[interfaceType]} 中找不到 ⇒ 实现与接口脱钩");

                var routeIndex = text.IndexOf('\"', callIndex);
                var routeEnd = text.IndexOf('\"', routeIndex + 1);
                var actualRoute = text.Substring(routeIndex + 1, routeEnd - routeIndex - 1);

                actualRoute.Should().Be(expectedRoute,
                    $"{method.Name} 的路由必须照抄官方本页（{expectedRoute}）——本族无特性载体，源文本是唯一的路由契约面");
            }
        }
    }

    /// <summary>
    /// 域名口径：五支挂 <c>api.mch</c>、四支挂 <c>apihk.mch</c>，八支<b>都不许</b>出现 <c>SecureBaseAddress</c>；
    /// 两个域名常量本身的字面值同时锁定。
    /// </summary>
    [Fact]
    public void V2_MerchantProfile_Domains_ShouldBeApiMchAndApiHk_NeverSecureBaseAddress()
    {
        foreach (var family in ApiMchFamilies)
        {
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay", ImplementationFiles[family]));
            Occurrences(text, "WechatPayV2HttpClientNames.BaseAddress, request")
                .Should().Be(FamilyRoutes[family].Length,
                    $"{family.Name} 的每一支须逐支显式钉在 api.mch 上（不靠默认值）");
            Occurrences(text, "SecureBaseAddress, request").Should().Be(0,
                "because 官方本页域名是 api.mch；把实参位换成 api2.mch 该域名下无这些路由 ⇒ 404" +
                "（ remarks 里讨论 api2.mch 是有意为之，故只钉实参位不钉全文）");
        }

        foreach (var family in ApiHkFamilies)
        {
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay", ImplementationFiles[family]));
            Occurrences(text, "WechatPayV2HttpClientNames.GlobalBaseAddress, request")
                .Should().Be(FamilyRoutes[family].Length,
                    $"{family.Name} 的每一支须逐支显式钉在 apihk.mch 上（官方「接口说明」栏只印这个域名）");

            // 「Names.BaseAddress, request」带前导点 ⇒ 不会被 GlobalBaseAddress 命中，这里数的正是 api.mch 实参位。
            Occurrences(text, "WechatPayV2HttpClientNames.BaseAddress, request").Should().Be(0,
                $"because {family.Name} 官方域名只有 apihk.mch；国内域名下这些路由不存在");
            Occurrences(text, "SecureBaseAddress, request").Should().Be(0);
        }

        WechatPayV2HttpClientNames.BaseAddress.Should().Be("https://api.mch.weixin.qq.com");
        WechatPayV2HttpClientNames.SecureBaseAddress.Should().Be("https://api2.mch.weixin.qq.com");
        WechatPayV2HttpClientNames.GlobalBaseAddress.Should().Be("https://apihk.mch.weixin.qq.com",
            "because global 站四页「接口说明」栏逐字给出该域名");
    }

    /// <summary>
    /// <b>SSRF 闭集</b>：证书通道的域名白名单必须同时收口四个常量 ——
    /// 少一个 ⇒ 该域族的请求被拒（假红），多一个商户可控值 ⇒ 绕过组件 UrlValidator（真漏）。
    /// <para>
    /// 第四项 <c>FraudBaseAddress</c>（<c>fraud.mch</c>）由获取防欺诈公钥
    /// （<c>/risk/getpublickey</c>）一页的「接口地址」栏引出：该页逐字印
    /// <c>https://fraud.mch.weixin.qq.com/risk/getpublickey</c>，既非 <c>api.mch</c> 亦非 <c>api2.mch</c>
    ///（同前缀的 <c>/risk/getviolation</c> 官方给的却是 <c>api.mch</c> ⇒ 域名与路由前缀各页各判）。
    /// </para>
    /// </summary>
    [Fact]
    public void V2_MerchantProfile_SecureChannelWhitelist_ShouldBeClosedSetOfFourConstants()
    {
        var text = File.ReadAllText(
            SourcePath("Mud.Wechat.Pay",  "Secure/WechatPayV2SecureChannelProvider.cs"));

        foreach (var constant in new[] { "BaseAddress", "SecureBaseAddress", "GlobalBaseAddress", "FraudBaseAddress" })
        {
            text.Should().Contain($"WechatPayV2HttpClientNames.{constant}",
                $"because 域名白名单是 {constant} 四项闭集，缺项即误拒");
        }

        Occurrences(text, "baseAddress != WechatPayV2HttpClientNames.").Should().Be(4,
            "because 校验恰由四个编译期常量的不等式组成（新增第五域名须同批改本断言与常量表）");

        WechatPayV2HttpClientNames.FraudBaseAddress.Should().Be("https://fraud.mch.weixin.qq.com",
            "because 防欺诈公钥页「接口地址」栏逐字给出该域名");

        text.Should().NotContain("merchant.BaseUrl",
            "because 通道域名绝不经商户配置（闭集是 SSRF 防线的落点）");
    }

    /// <summary>
    /// 媒体上传是本线<b>唯一</b>的 multipart 端点：走 <c>PostMultipartAsync</c>、文件分节名恒为官方 <c>media</c>，
    /// 且不得混用 XML 出口（两套出口对同一支各签一次 = 必错其一）。
    /// </summary>
    [Fact]
    public void V2_MerchantProfile_MediaUpload_ShouldUseTheOnlyMultipartExit()
    {
        var media = File.ReadAllText(
            SourcePath("Mud.Wechat.Pay", ImplementationFiles[typeof(IWechatPayV2MerchantMediaService)]));

        Occurrences(media, "PostMultipartAsync<").Should().Be(1);
        Occurrences(media, "PostAsync<").Should().Be(0,
            "because 本族请求载体是表单；XML 出口会把字段拼成 XML 体，官方按 form-data 解析必然取不到文件");
        media.Should().Contain("MediaFieldName = \"media\"",
            "because form-data 的 name 属性是官方字段名，写错即官方读不到文件分节");

        // 文件分节天然不参与签名：收发器只对 DTO 字段 + sign 建表。
        var invoker = File.ReadAllText(SourcePath("Mud.Wechat.Pay",  "Secure/WechatPayV2SecureInvoker.cs"));
        invoker.Should().Contain("new MultipartFormDataContent");
        invoker.Should().Contain("content.Add(filePart, fileFieldName, fileName)",
            "because 文件必须作为独立分节附加，才能既不参与签名又保持官方读取形态");
        Occurrences(invoker, "VerifyResponse(merchant, apiKey, path, text)").Should().Be(2,
            "because multipart 出口的应答同样是 XML、同样必须验签 —— 换载体不得漏验签");

        // 其余三支仍走 XML 出口。
        foreach (var family in ApiMchFamilies.Append<
                     Type>(typeof(IWechatPayV2InstitutionSubService)))
        {
            var text = File.ReadAllText(SourcePath("Mud.Wechat.Pay", ImplementationFiles[family]));
            Occurrences(text, "PostMultipartAsync<").Should().Be(0,
                $"{family.Name} 官方请求格式是 XML，不得改用表单出口");
        }
    }

    /// <summary>
    /// 签名面即契约面：<c>sign_type</c> 的<b>有无</b>与<b>默认值</b>逐型锁定。
    /// 补齐一个本该没有的 <c>sign_type</c>、或清空一个必填的默认值，症状都只是官方回 <c>SIGNERROR</c>，
    /// 排查成本极高 ⇒ 只靠注释不够，须打红。
    /// </summary>
    [Fact]
    public void V2_MerchantProfile_SignTypeSurface_ShouldMatchEachOfficialPage()
    {
        // 关注配置 + 小微两支：sign_type 必填且仅 HMAC-SHA256，且该字段参与 sign（默认值即契约面）。
        foreach (var dto in new[]
                 {
                     typeof(V2RecommendConfigAddRequest), typeof(V2MicroArchivesModifyRequest),
                     typeof(V2MicroContactInfoModifyRequest),
                 })
        {
            JsonNames(dto).Should().Contain("sign_type", $"{dto.Name} 官方要求 sign_type 必填");
            DeclaredDefaultOf(dto, "SignType").Should().Be("HMAC-SHA256",
                $"{dto.Name} 的 sign_type 默认值属契约面：清空即把报文降成 MD5 而被官方拒");
        }

        // 支付目录两支 + 机构进件三支：官方页无 sign_type ⇒ 多补一个即签名口径错位。
        foreach (var dto in new[]
                 {
                     typeof(V2SubDevConfigAddRequest), typeof(V2SubDevConfigQueryRequest),
                     typeof(V2InstitutionSubAddRequest), typeof(V2InstitutionSubModifyRequest),
                     typeof(V2InstitutionSubQueryRequest),
                 })
        {
            JsonNames(dto).Should().NotContain("sign_type", $"{dto.Name} 官方字段表没有 sign_type");
            JsonNames(dto).Should().NotContain("nonce_str", $"{dto.Name} 官方字段表没有 nonce_str");
        }

        // 媒体上传支的 sign_type 官方为选填 ⇒ 不设默认值（设了等于替宿主改签名算法）。
        JsonNames(typeof(V2UploadMediaRequest)).Should().Contain("sign_type");
        DeclaredDefaultOf(typeof(V2UploadMediaRequest), "SignType").Should().BeNull(
            "because 官方「其他或者不填为MD5方式」⇒ 默认值会改变算法，属宿主决策而非 SDK 决策");
    }

    /// <summary>
    /// 字段名逐型锁定：机构族用 <c>app_id</c>（带下划线）、其余族用 <c>appid</c>；
    /// 关注配置与风控一样<b>没有</b> <c>appid</c>；媒体上传的请求面<b>不含</b>文件字段本身。
    /// </summary>
    [Fact]
    public void V2_MerchantProfile_FieldNames_ShouldBeVerbatimOfficial()
    {
        JsonNames(typeof(V2InstitutionSubAddRequest)).Should().Contain("app_id")
            .And.NotContain("appid",
                "because 机构进件族官方原文即 app_id（带下划线），这是本线唯一的例外拼写");
        JsonNames(typeof(V2InstitutionSubQueryRequest)).Should().Contain("app_id");
        JsonNames(typeof(V2SubDevConfigAddRequest)).Should().Contain("appid");

        JsonNames(typeof(V2RecommendConfigAddRequest)).Should().NotContain("appid")
            .And.NotContain("app_id",
                "because 官方表就 8 行，主体标识只有 mch_id + sub_mch_id ⇒ 照抄不得补");

        // 逐型行数即官方参数表行数（新增/删除字段属契约面变更，须同批改此处）。
        JsonNames(typeof(V2SubDevConfigAddRequest)).Should().HaveCount(5);
        JsonNames(typeof(V2SubDevConfigQueryRequest)).Should().HaveCount(4);
        JsonNames(typeof(V2RecommendConfigAddRequest)).Should().HaveCount(8);
        JsonNames(typeof(V2MicroArchivesModifyRequest)).Should().HaveCount(11);
        JsonNames(typeof(V2MicroContactInfoModifyRequest)).Should().HaveCount(10);
        JsonNames(typeof(V2UploadMediaRequest)).Should().HaveCount(4,
            "because 文件本体是 form-data 分节、不是报文字段 ⇒ 请求 DTO 只有 4 行");
        JsonNames(typeof(V2InstitutionSubAddRequest)).Should().HaveCount(34);
        JsonNames(typeof(V2InstitutionSubModifyRequest)).Should().HaveCount(33,
            "because 修改比新增多 sub_mch_id、少 merchant_remark 与 attach");

        JsonNames(typeof(V2InstitutionSubModifyRequest)).Should().Contain("sub_mch_id")
            .And.NotContain("merchant_remark").And.NotContain("attach");

        // 小微两支的协议版本默认值（官方「默认值1.0」）。
        DeclaredDefaultOf(typeof(V2MicroArchivesModifyRequest), "Version").Should().Be("1.0");
        DeclaredDefaultOf(typeof(V2MicroContactInfoModifyRequest), "Version").Should().Be("1.0");
    }

    /// <summary>
    /// 敏感字段承载<b>密文</b>，且 SDK 不代宿主猜测哪些字段要加密：
    /// 官方标敏感的 3 支（<c>account_number</c>/<c>mobile_phone</c>/<c>email</c>）在各自声明处的文档块里
    /// 逐一点名「密文」与「禁止写入日志与异常消息」，官方未标的 <c>merchant_name</c> 不得被误标。
    /// </summary>
    [Fact]
    public void V2_MerchantProfile_SensitiveFields_ShouldBeDocumentedAsCiphertext()
    {
        var micro = File.ReadAllLines(SourcePath(
            "Mud.Wechat.Pay", "PayV2/V2MerchantMicroDtos.cs"));

        foreach (var field in new[] { "account_number", "mobile_phone", "email" })
        {
            var doc = DocBlockAbove(micro, $"[JsonPropertyName(\"{field}\")]");

            doc.Should().Contain("密文",
                $"because 官方把 {field} 标为需要加密处理的敏感字段 ⇒ 该属性承载的是 RSA-OAEP 后的 Base64，不是明文");
            doc.Should().Contain("禁止写入日志与异常消息",
                $"because {field} 属用户身份／金融信息，PAY-B7 同线口径：密文与明文都不出");
        }

        // 官方未标敏感的那支不得被顺手标成密文，否则宿主会照注释去加密一个官方按明文接收的字段。
        var merchantName = DocBlockAbove(micro, "[JsonPropertyName(\"merchant_name\")]");
        merchantName.Should().NotContain("密文");
        merchantName.Should().Contain("明文");

        // 加密口径本身（OAEP-SHA1 + Base64 + 平台证书公钥）由本守卫的加密器用例逐字节验证。
        var encrypter = File.ReadAllText(SourcePath("Mud.Wechat.Pay",  "Security/WechatPayV2SensitiveFieldEncrypter.cs"));
        encrypter.Should().Contain("RSAEncryptionPadding.OaepSHA1",
            "because 官方 Java 示例是 OAEPWithSHA-1AndMGF1Padding；用 SHA-256 会得到对方解不开的密文");
        encrypter.Should().Contain("Convert.ToBase64String");
    }

    /// <summary>
    /// <b>加密器的真实消费点与算法口径</b>：自签 RSA 证书 → 加密 → 用私钥按 OAEP-SHA1 解回必须等于原文；
    /// 换 OAEP-SHA256 解必须失败（证明钉死的是 SHA-1 而非「任一 OAEP」）。
    /// </summary>
    [Fact]
    public void V2_MerchantProfile_Encrypter_ShouldProduceOaepSha1Base64()
    {
        using var rsa = RSA.Create(2048);
        using var certificate = SelfSignedCertificate(rsa);

        var plaintext = "6228480000000000000";
        var cipher = WechatPayV2SensitiveFieldEncrypter.Encrypt(plaintext, certificate);

        Convert.FromBase64String(cipher).Length.Should().Be(256,
            "because 2048 位 RSA 的 OAEP 密文恒为模数长度；Base64 解码即得原始密文字节");

        using (var decryptor = certificate.GetRSAPrivateKey()!)
        {
            Encoding.UTF8.GetString(
                    decryptor.Decrypt(Convert.FromBase64String(cipher), RSAEncryptionPadding.OaepSHA1))
                .Should().Be(plaintext);

            Action wrongPadding = () => decryptor.Decrypt(
                Convert.FromBase64String(cipher), RSAEncryptionPadding.OaepSHA256);
            wrongPadding.Should().Throw<CryptographicException>(
                "because 官方口径是 SHA-1；若本类用的是 SHA-256 则此处会解得开 ⇒ 该断言即算法钉死的证据");
        }
    }

    /// <summary>加密器不得回显明文：空值走 <see cref="ArgumentException"/>，且消息里不含入参内容。</summary>
    [Fact]
    public void V2_MerchantProfile_Encrypter_ShouldNotEchoPlaintextInErrors()
    {
        using var rsa = RSA.Create(2048);
        using var certificate = SelfSignedCertificate(rsa);

        var blank = "   ";
        var error = Assert.Throws<ArgumentException>(
            () => WechatPayV2SensitiveFieldEncrypter.Encrypt(blank, certificate));
        error.ParamName.Should().Be("plaintext");

        // 异常消息不得内插实参：空白串与空串走同一分支，两条消息必须逐字相同。
        var empty = Assert.Throws<ArgumentException>(
            () => WechatPayV2SensitiveFieldEncrypter.Encrypt(string.Empty, certificate));
        empty.Message.Should().Be(error.Message,
            "because 消息一旦内插实参，明文就会随日志与异常上报流出（PAY-B7）");

        // 非 RSA 公钥（EC）⇒ 明确报「配错了证书」，而不是静默用别的算法加密。
        using var ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        using var ecCertificate = SelfSignedCertificate(ec);
        var wrongKey = Assert.Throws<InvalidOperationException>(
            () => WechatPayV2SensitiveFieldEncrypter.Encrypt("123", ecCertificate));
        wrongKey.Message.Should().Contain("不是 RSA 密钥");
    }

    /// <summary>按序列号取证书那条重载<b>必须 fail-closed</b>：查不到即抛，不回落到「任意证书」。</summary>
    [Fact]
    public void V2_MerchantProfile_Encrypter_ShouldFailClosedOnUnknownSerial()
    {
        using var rsa = RSA.Create(2048);
        using var certificate = SelfSignedCertificate(rsa);

        var store = new SingleCertificateStore(certificate);

        WechatPayV2SensitiveFieldEncrypter
            .Encrypt("13800000000", store, certificate.SerialNumber)
            .Should().NotBeNullOrWhiteSpace();

        var error = Assert.Throws<InvalidOperationException>(
            () => WechatPayV2SensitiveFieldEncrypter.Encrypt("13800000000", store, "UNKNOWN-SERIAL"));
        error.Message.Should().NotContain("13800000000", "because 明文手机号不得进异常消息（PAY-B7）");

        Assert.Throws<InvalidOperationException>(
            () => WechatPayV2SensitiveFieldEncrypter.Encrypt("13800000000", store, null));
    }

    /// <summary>本批 17 型 DTO 须全部登记进 <c>PayV2JsonContext</c>（AOT 免反射的唯一途径）。</summary>
    [Fact]
    public void V2_MerchantProfile_Dtos_ShouldBeRegisteredInPayV2JsonContext()
    {
        GuardedDtos.Should().HaveCount(17,
            "because 子商户配置 6 + 小微 3 + 机构进件 6 + 媒体 2；逐型清点，漏一型即运行期回落反射");

        var contextType = typeof(V2SubDevConfigAddRequest).Assembly
            .GetType("Mud.Wechat.Pay.V2.DataModels.PayV2.PayV2JsonContext", throwOnError: false);
        contextType.Should().NotBeNull();
        var context = (global::System.Text.Json.Serialization.JsonSerializerContext)
            contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;

        foreach (var dto in GuardedDtos)
        {
            context.GetTypeInfo(dto).Should().NotBeNull(
                $"{dto.Name} 未登记进 PayV2JsonContext ⇒ 重跑 scripts/GenerateJsonContext.ps1");
        }
    }

    /// <summary>四族都是手写族 ⇒ 各持模块成员、注册器为 <c>TryAddSingleton</c>、且<b>没有</b>同名生成注册器。</summary>
    [Fact]
    public void V2_MerchantProfile_HandWrittenModules_ShouldBindSingletonServicesInBuilder()
    {
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",  "Extensions/PayV2ServiceBuilder.cs"));

        foreach (var module in new[]
                 {
                     nameof(PayV2Module.SubMerchantConfigV2), nameof(PayV2Module.MerchantMicroV2),
                     nameof(PayV2Module.InstitutionSubV2), nameof(PayV2Module.MerchantMediaV2),
                 })
        {
            builderSource.Should().Contain($"[PayV2Module.{module}]", "because 成员无注册器即「静默不注册」");
            builderSource.Should().NotContain($"Add{module}WebApiHttpClient()",
                $"{module} 是手写族，出现生成注册器即说明证书族被误挂到声明式管道上");
            Enum.TryParse<PayV2Module>(module, out _).Should().BeTrue();
        }

        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2SubMerchantConfigService, WechatPayV2SubMerchantConfigService>");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2MerchantMicroService, WechatPayV2MerchantMicroService>");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2InstitutionSubService, WechatPayV2InstitutionSubService>");
        builderSource.Should().Contain("TryAddSingleton<IWechatPayV2MerchantMediaService, WechatPayV2MerchantMediaService>");

        // 与模块成员总账的接线处：四族必须同时进 HandWrittenModules，否则「成员 ≡ 注册组 ∪ 手写名单」的不变式失真。
        WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().Contain(new[]
        {
            nameof(PayV2Module.SubMerchantConfigV2), nameof(PayV2Module.MerchantMicroV2),
            nameof(PayV2Module.InstitutionSubV2), nameof(PayV2Module.MerchantMediaV2),
        });
        WechatPayV2CertFamilyContractGuards.HandWrittenModules.Should().NotContain(
            nameof(PayV2Module.DepositV2), "because 押金是一域两形态，确有 DepositV2 生成注册组");
    }

    /// <summary>DI 可解析 + 跨 scope 同一实例；只装别的模块时解析不到本族服务（模块隔离有效）。</summary>
    [Fact]
    public void V2_MerchantProfile_Services_ShouldResolveAsSingletonsPerModule()
    {
        using var provider = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(
                PayV2Module.SubMerchantConfigV2, PayV2Module.MerchantMicroV2,
                PayV2Module.InstitutionSubV2, PayV2Module.MerchantMediaV2)));

        var sub = provider.GetRequiredService<IWechatPayV2SubMerchantConfigService>();
        var micro = provider.GetRequiredService<IWechatPayV2MerchantMicroService>();
        var institution = provider.GetRequiredService<IWechatPayV2InstitutionSubService>();
        var media = provider.GetRequiredService<IWechatPayV2MerchantMediaService>();

        sub.Should().BeOfType<WechatPayV2SubMerchantConfigService>();
        micro.Should().BeOfType<WechatPayV2MerchantMicroService>();
        institution.Should().BeOfType<WechatPayV2InstitutionSubService>();
        media.Should().BeOfType<WechatPayV2MerchantMediaService>();

        using (var scope = provider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IWechatPayV2InstitutionSubService>()
                .Should().BeSameAs(institution, "because 证书通道是商户级复用，服务必须 Singleton");
        }

        using var baseOnly = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddModules(PayV2Module.BasePayV2)));

        baseOnly.GetService<IWechatPayV2SubMerchantConfigService>().Should().BeNull();
        baseOnly.GetService<IWechatPayV2MerchantMediaService>().Should().BeNull();

        using var allApis = BuildProvider(static services =>
            services.AddWechatPayV2Api(static builder => builder.AddAllApis()));
        allApis.GetRequiredService<IWechatPayV2MerchantMediaService>().Should().NotBeNull(
            "because AddAllApis 必须覆盖到手写族（它们没有生成注册器，最易被漏在 AddAllApis 之外）");
    }

    /// <summary>本批 9 条路由与其余 APIv2 族<b>零交叠</b>（同一路径两条走法 = 两套签名口径）。</summary>
    [Fact]
    public void V2_MerchantProfile_Routes_ShouldBeDistinct()
    {
        var routes = FamilyRoutes.Values.SelectMany(static r => r).ToList();

        routes.Should().HaveCount(9);
        routes.Distinct().Should().HaveCount(9);

        // /secapi/mch/* 与 /secapi/pay/* 是两个不同的官方前缀：前者本批四支，后者属基础支付证书五支。
        routes.Should().NotContain(r => r.StartsWith("/secapi/pay/", StringComparison.Ordinal),
            "because /secapi/pay/* 归 IWechatPayV2SecureService（api2.mch）");

        // 最易混的一对：新增支付目录 vs 商户图片上传同名前缀 /secapi/mch/，但域名不同（api.mch vs apihk.mch）。
        routes.Should().Contain("/secapi/mch/addsubdevconfig").And.Contain("/secapi/mch/uploadmedia");
    }

    private static MethodInfo[] DeclaredMethods(Type interfaceType)
        => interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static List<string?> JsonNames(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .ToList();

    /// <summary>
    /// 取 DTO 属性声明处的<b>初始化默认值</b>（契约面）：把 <c>sign_type</c>／<c>version</c> 的默认值
    /// 从「注释里写着」升级为「断言锁着」。
    /// </summary>
    /// <remarks>
    /// 属性初始化器编译后不留运行时元数据 ⇒ 反射拿不到，只能读源文本。
    /// 解析先定位类型声明、再在该类型之后的第一行属性声明上取值，返回 <c>null</c> 表示「无默认值」。
    /// </remarks>
    private static string? DeclaredDefaultOf(Type dto, string propertyName)
    {
        var source = DtoSourceText(dto);

        var classIndex = source.IndexOf($"class {dto.Name}", StringComparison.Ordinal);
        classIndex.Should().BeGreaterThanOrEqualTo(0, $"{dto.Name} 的类型声明不在登记的源文件里");

        var declaration = $"public string? {propertyName} {{ get; set; }}";
        var declarationIndex = source.IndexOf(declaration, classIndex, StringComparison.Ordinal);
        declarationIndex.Should().BeGreaterThanOrEqualTo(0,
            $"{dto.Name}.{propertyName} 未以全仓 DTO 口径声明（[JsonPropertyName] + 可空自动属性）");

        var lineEnd = source.IndexOf('\n', declarationIndex);
        var line = source.Substring(declarationIndex, (lineEnd < 0 ? source.Length : lineEnd) - declarationIndex);

        const string initializer = "} = \"";
        var valueIndex = line.IndexOf(initializer, StringComparison.Ordinal);
        if (valueIndex < 0)
        {
            return null;
        }

        var valueStart = valueIndex + initializer.Length;
        var valueEnd = line.IndexOf('"', valueStart);
        valueEnd.Should().BeGreaterThan(valueStart - 1, $"{dto.Name}.{propertyName} 的默认值字面量未闭合");

        return line.Substring(valueStart, valueEnd - valueStart);
    }

    private static readonly Dictionary<Type, string> DtoSourceFiles = new()
    {
        [typeof(V2RecommendConfigAddRequest)] = "PayV2/V2SubMerchantConfigDtos.cs",
        [typeof(V2MicroArchivesModifyRequest)] = "PayV2/V2MerchantMicroDtos.cs",
        [typeof(V2MicroContactInfoModifyRequest)] = "PayV2/V2MerchantMicroDtos.cs",
        [typeof(V2UploadMediaRequest)] = "PayV2/V2MerchantMediaDtos.cs",
    };

    private static string DtoSourceText(Type dto)
    {
        DtoSourceFiles.TryGetValue(dto, out var relative);
        relative.Should().NotBeNull($"{dto.Name} 未登记源文件 ⇒ 新 DTO 文件须同批补 DtoSourceFiles");
        return File.ReadAllText(SourcePath("Mud.Wechat.Pay", relative!));
    }

    /// <summary>
    /// 取某属性声明行正上方的 XML 文档块（连续 <c>///</c> 行）拼成的字符串。
    /// 「文档写在别的字段上」不算数 ⇒ 警示必须钉在承载该字段的那一行上。
    /// </summary>
    private static string DocBlockAbove(string[] lines, string declarationMarker)
    {
        var declaration = Array.FindIndex(lines, l => l.Contains(declarationMarker, StringComparison.Ordinal));
        declaration.Should().BeGreaterThanOrEqualTo(0, $"找不到属性声明行 {declarationMarker} ⇒ 字段名漂移或本守卫未同批更新");

        var builder = new StringBuilder();
        for (var i = declaration - 1; i >= 0; i--)
        {
            var trimmed = lines[i].TrimStart();
            if (!trimmed.StartsWith("///", StringComparison.Ordinal))
            {
                break;
            }

            builder.Insert(0, trimmed);
            builder.Insert(0, '\n');
        }

        builder.Length.Should().BeGreaterThan(0, $"{declarationMarker} 的声明处无 XML 文档 ⇒ 公共属性必须写文档（AGENTS §7）");
        return builder.ToString();
    }

    /// <summary>
    /// 自签一张「测试用平台证书」：RSA 分支供加密器正常路径使用，EC 分支用于验证
    /// 「配错证书类型 ⇒ 明确报错，不静默换算法」。<c>SerialNumber</c> 由系统随机生成即可 ——
    /// 本类只把它当作 <c>cert_sn</c> 的同源标识回传给存储。
    /// </summary>
    private static X509Certificate2 SelfSignedCertificate(RSA rsa)
        => new CertificateRequest("CN=pay-v2-test-platform-cert", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            .CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));

    private static X509Certificate2 SelfSignedCertificate(ECDsa ecDsa)
        => new CertificateRequest("CN=pay-v2-test-platform-cert", ecDsa, HashAlgorithmName.SHA256)
            .CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));

    private static int Occurrences(string text, string needle)
    {
        var count = 0;
        var index = text.IndexOf(needle, StringComparison.Ordinal);
        while (index >= 0)
        {
            count++;
            index = text.IndexOf(needle, index + needle.Length, StringComparison.Ordinal);
        }

        return count;
    }

    private static ServiceProvider BuildProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
            new Dictionary<string, string>
            {
                ["pay:merchant:1900000000:key"] = "placeholder",
                ["pay:merchant:1900000000:apikey"] = new string('a', 32),
                ["pay:merchant:1900000000:apiv2key"] = new string('k', 32),
            }));

        services.AddPayApp(new List<WechatPayMerchantConfig>
        {
            new()
            {
                MchId = "1900000000",
                SerialNumber = WechatPayGoldenVectors.MerchantSerialNumber,
                PrivateKeySecretName = "pay:merchant:1900000000:key",
                ApiKeySecretName = "pay:merchant:1900000000:apikey",
                ApiV2KeySecretName = "pay:merchant:1900000000:apiv2key",
            },
        });

        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(Root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

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

    /// <summary>只装一本证书的假存储：用于验证「未知序列号 ⇒ fail-closed，不回落到任意证书」。</summary>
    private sealed class SingleCertificateStore : IWechatPayPlatformCertificateStore
    {
        private readonly X509Certificate2 _certificate;

        public SingleCertificateStore(X509Certificate2 certificate) => _certificate = certificate;

        public bool TryGetCertificate(string? serialNumber, out X509Certificate2? certificate)
        {
            var match = !string.IsNullOrWhiteSpace(serialNumber)
                        && string.Equals(serialNumber, _certificate.SerialNumber, StringComparison.OrdinalIgnoreCase);
            certificate = match ? _certificate : null;
            return match;
        }

        public bool TryGetPublicKey(string? serialNumber, out byte[]? publicKey, out string? algorithm)
        {
            if (TryGetCertificate(serialNumber, out var cert) && cert != null)
            {
                publicKey = cert.GetRSAPublicKey()?.ExportSubjectPublicKeyInfo();
                algorithm = "SHA256-RSA2048";
                return publicKey != null;
            }

            publicKey = null;
            algorithm = null;
            return false;
        }
    }
}
