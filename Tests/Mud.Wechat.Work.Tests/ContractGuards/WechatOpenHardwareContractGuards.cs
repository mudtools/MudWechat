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
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.OpenHardware;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 企业微信「开放硬件」（智能硬件 · 硬件云端接入）域契约守卫（OH 系列）：
/// 17 端点路由表、<b>两级外部凭证面</b>划分、无 <c>[Token]</c> 形态、DTO 上下文登记与官方字段名陷阱。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域形态例外（追踪号 P2-QY-OPENHW-TOKENFACE-04，2026-10-13 裁决 / 2026-10-18 逐页复核定型）</b>：
/// 官方硬件云端接入面不消费 <c>access_token</c> / <c>suite_access_token</c> / <c>provider_access_token</c>
/// 任一应用令牌链路，而是自成两级<b>外部凭证</b>：<c>model_access_token</c>（型号凭证，95992 换取）与
/// <c>device_access_token</c>（设备凭证，95993/96022 换取）。二者由硬件厂商持有、<b>不在</b>本 SDK
/// 多应用令牌管理面内 ⇒ 落位为<b>独立可注入接口、无父接口、无应用类型子接口</b>，
/// 形态对齐 <c>IWechatWorkPromotionQrCodeContactSyncService</c> 与 <c>IWechatWorkProviderAuthenticationUrl</c>
/// 的「显式 <c>[Query]</c> 传凭证」既存例外。
/// </para>
/// <para>
/// 因此本域<b>不进入</b>契约守卫 G5（Query 令牌注入白名单）与 TO 系列（令牌归属域），
/// 两支凭证参数名的脱敏归属由 G7（<see cref="WechatContractGuards"/> 的
/// <c>QueryCredentialParams_ShouldBeRedactionRegisteredOrExplicitlyExempted</c>）裁定。
/// </para>
/// <para>
/// <b>官方反直觉点（勿「顺手修正」）</b>：① <c>gen_id_dynamic_qrcode</c>（98024）的凭证面是
/// <b>型号</b>而非设备（同 <c>device/</c> 前缀却用 <c>model_access_token</c>，逐页取证结论）；
/// ② 96037「按 id 拉取用户」响应<b>不含</b> <c>next_cursor</c> / <c>perm_version</c>，与 95984 分页响应
/// 形态不同 ⇒ 两个响应 DTO 不共用；③ 95986 体温 <c>temperature</c> 为<b>字符串</b>且失败项带
/// 逐条 <c>errcode</c>/<c>errmsg</c>（95997/95985 的 <c>fail_list</c> 仅为键三元组）；
/// ④ 官方 SKIT 转述的两处文档号与现行页面不符（<c>get_userinfo_by_page</c> 实为 95984、
/// <c>model/register_sn</c> 实为 95980），本守卫以逐页取证为准。
/// </para>
/// </remarks>
public class WechatOpenHardwareContractGuards
{
    private const string OpenHardwareRegistryGroupName = "OpenHardware";

    private const string OpenHardwareNamespace = "Mud.Wechat.Work.DataModels.OpenHardware";

    /// <summary>型号凭证参数名（官方 Query 参数，非 SDK 管理令牌）。</summary>
    private const string ModelCredentialParam = "model_access_token";

    /// <summary>设备凭证参数名（官方 Query 参数，非 SDK 管理令牌）。</summary>
    private const string DeviceCredentialParam = "device_access_token";

    /// <summary>开放硬件域官方路由表（17 端点，全部 POST；凭证面见第三列）。</summary>
    private static readonly (string Method, Type Request, Type? Response, string Credential)[] OpenHardwareEndpoints =
    {
        (nameof(IWechatWorkOpenHardwareService.GetModelTokenAsync),
            typeof(GetModelTokenRequest), typeof(GetModelTokenResponse), null!),
        (nameof(IWechatWorkOpenHardwareService.GetDeviceSecretAsync),
            typeof(GetDeviceSecretRequest), typeof(GetDeviceSecretResponse), ModelCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.GetDeviceTokenAsync),
            typeof(GetDeviceTokenRequest), typeof(GetDeviceTokenResponse), ModelCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.RegisterDeviceSnAsync),
            typeof(RegisterDeviceSnRequest), typeof(RegisterDeviceSnResponse), ModelCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.UnregisterDeviceSnAsync),
            typeof(UnregisterDeviceSnRequest), typeof(WechatWorkResponse), ModelCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.GetDeviceDetailAsync),
            typeof(GetDeviceDetailRequest), typeof(GetDeviceDetailResponse), ModelCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.ReportDeviceStatusAsync),
            typeof(ReportDeviceStatusRequest), typeof(WechatWorkResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.GetHardwareUserByPageAsync),
            typeof(GetHardwareUserPageRequest), typeof(GetHardwareUserPageResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.GetHardwareUserByIdsAsync),
            typeof(GetHardwareUserByIdsRequest), typeof(GetHardwareUserByIdsResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.ReportFirmwareUpgradeResultAsync),
            typeof(ReportFirmwareUpgradeResultRequest), typeof(WechatWorkResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.ReportAccessDataAsync),
            typeof(ReportAccessDataRequest), typeof(ReportAccessDataResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.ReportBioInfoResultAsync),
            typeof(ReportBioInfoResultRequest), typeof(WechatWorkResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.ReportCheckinDataAsync),
            typeof(ReportCheckinDataRequest), typeof(ReportCheckinDataResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.ReportRemoteOpenResultAsync),
            typeof(ReportRemoteOpenResultRequest), typeof(WechatWorkResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.ReportTemperatureDataAsync),
            typeof(ReportTemperatureDataRequest), typeof(ReportTemperatureDataResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.GenDeviceLoginQrcodeAsync),
            typeof(GenDeviceLoginQrcodeRequest), typeof(GenDeviceQrcodeResponse), DeviceCredentialParam),
        (nameof(IWechatWorkOpenHardwareService.GenDeviceIdQrcodeAsync),
            typeof(GenDeviceIdQrcodeRequest), typeof(GenDeviceQrcodeResponse), ModelCredentialParam),
    };

    /// <summary>端点 → 官方路由（逐页取证，文档号见各接口 XML 注释）。</summary>
    private static readonly Dictionary<string, string> OpenHardwareRoutes = new(StringComparer.Ordinal)
    {
        [nameof(IWechatWorkOpenHardwareService.GetModelTokenAsync)] = "/cgi-bin/openhw/get_model_token",
        [nameof(IWechatWorkOpenHardwareService.GetDeviceSecretAsync)] = "/cgi-bin/openhw/get_device_secret",
        [nameof(IWechatWorkOpenHardwareService.GetDeviceTokenAsync)] = "/cgi-bin/openhw/get_device_token",
        [nameof(IWechatWorkOpenHardwareService.RegisterDeviceSnAsync)] = "/cgi-bin/openhw/model/register_sn",
        [nameof(IWechatWorkOpenHardwareService.UnregisterDeviceSnAsync)] = "/cgi-bin/openhw/model/unregister_sn",
        [nameof(IWechatWorkOpenHardwareService.GetDeviceDetailAsync)] = "/cgi-bin/openhw/device/get_device_detail",
        [nameof(IWechatWorkOpenHardwareService.ReportDeviceStatusAsync)] = "/cgi-bin/openhw/device/report_device_status",
        [nameof(IWechatWorkOpenHardwareService.GetHardwareUserByPageAsync)] = "/cgi-bin/openhw/device/get_userinfo_by_page",
        [nameof(IWechatWorkOpenHardwareService.GetHardwareUserByIdsAsync)] = "/cgi-bin/openhw/device/get_userinfo_by_ids",
        [nameof(IWechatWorkOpenHardwareService.ReportFirmwareUpgradeResultAsync)] = "/cgi-bin/openhw/device/report_firmware_upgrade_result",
        [nameof(IWechatWorkOpenHardwareService.ReportAccessDataAsync)] = "/cgi-bin/openhw/device/report_access_data",
        [nameof(IWechatWorkOpenHardwareService.ReportBioInfoResultAsync)] = "/cgi-bin/openhw/device/report_bio_info_result",
        [nameof(IWechatWorkOpenHardwareService.ReportCheckinDataAsync)] = "/cgi-bin/openhw/device/report_checkin_data",
        [nameof(IWechatWorkOpenHardwareService.ReportRemoteOpenResultAsync)] = "/cgi-bin/openhw/device/report_remote_open_result",
        [nameof(IWechatWorkOpenHardwareService.ReportTemperatureDataAsync)] = "/cgi-bin/openhw/device/report_temperature_data",
        [nameof(IWechatWorkOpenHardwareService.GenDeviceLoginQrcodeAsync)] = "/cgi-bin/openhw/device/gen_login_qrcode",
        [nameof(IWechatWorkOpenHardwareService.GenDeviceIdQrcodeAsync)] = "/cgi-bin/openhw/device/gen_id_dynamic_qrcode",
    };

    /// <summary>
    /// 契约守卫 OH1：17 端点的路由、方法形态（恒 POST）与请求体必须与官方契约一致；
    /// 凭证面划分锁定为「型号 6 / 设备 10 / 无凭证 1」，且 <c>gen_id_dynamic_qrcode</c> 归<b>型号</b>面。
    /// </summary>
    [Fact]
    public void OpenHardwareEndpoints_ShouldMatchOfficialRoutesAndCredentialFace()
    {
        OpenHardwareEndpoints.Should().HaveCount(17,
            "硬件云端接入面官方现行 17 端点（95992/95993/96022/95980/95981/95982 + 95983~95986/95997/95999/96000/96037/96048 + 98023/98024）");

        foreach (var (method, request, _, credential) in OpenHardwareEndpoints)
        {
            var target = typeof(IWechatWorkOpenHardwareService).GetMethod(
                method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{method} 必须声明于 IWechatWorkOpenHardwareService");

            var post = target!.GetCustomAttribute<PostAttribute>();
            post.Should().NotBeNull($"{method} 官方即 POST（含查询类端点，勿改成 GET）");
            post!.RequestUri.Should().Be(OpenHardwareRoutes[method],
                $"{method} 路由必须与官方取证一致");

            var body = target.GetParameters().SingleOrDefault(p => p.GetCustomAttribute<BodyAttribute>() is not null);
            body.Should().NotBeNull($"{method} 必须有 [Body] 请求体");
            body!.ParameterType.Should().Be(request, $"{method} 的请求体类型漂移");

            var credentialParam = target.GetParameters()
                .SingleOrDefault(p => p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name is ModelCredentialParam or DeviceCredentialParam);
            if (credential is null)
            {
                credentialParam.Should().BeNull(
                    "get_model_token 以 model_id/model_secret/model_ticket 三要素换凭证，Query 上不承载任何凭证参数");
            }
            else
            {
                credentialParam.Should().NotBeNull($"{method} 必须以 [Query(\"{credential}\")] 显式承载外部凭证");
                credentialParam!.GetCustomAttribute<QueryAttribute>()!.Name.Should().Be(credential,
                    $"{method} 的凭证面漂移（型号面 / 设备面不得互换）");
            }
        }

        OpenHardwareEndpoints.Count(e => e.Credential == ModelCredentialParam).Should().Be(6,
            "型号凭证面：get_device_secret / get_device_token / register_sn / unregister_sn / get_device_detail / gen_id_dynamic_qrcode");
        OpenHardwareEndpoints.Count(e => e.Credential == DeviceCredentialParam).Should().Be(10,
            "设备凭证面：report_device_status / 两支用户拉取 / 四支结果上报 + report_access_data / report_temperature_data / gen_login_qrcode");
        OpenHardwareEndpoints.Count(e => e.Credential is null).Should().Be(1,
            "仅 get_model_token 无凭证参数（它本身就是换取型号凭证的端点）");
    }

    /// <summary>
    /// 契约守卫 OH2：形态锁定——本域为独立可注入接口，<b>无父接口、无应用类型子接口、不声明 <c>[Token]</c></b>，
    /// 且不进入 G5 白名单 / TO 归属域；注册组与实现类生成形态符合 N1/N2 分区。
    /// </summary>
    [Fact]
    public void OpenHardwareInterfaces_ShouldBeStandaloneWithoutTokenAttribute()
    {
        var asm = typeof(IWechatWorkOpenHardwareService).Assembly;

        var derived = asm.GetTypes()
            .Where(t => t.IsInterface && t != typeof(IWechatWorkOpenHardwareService)
                        && typeof(IWechatWorkOpenHardwareService).IsAssignableFrom(t))
            .ToList();
        derived.Should().BeEmpty(
            "两级凭证为硬件厂商持有的外部凭证、与应用类型无关 ⇒ 不得出现父接口或自建/第三方/代开发子接口");

        var api = typeof(IWechatWorkOpenHardwareService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("IWechatWorkOpenHardwareService 必须声明 [HttpClientApi]");
        api!.IsAbstract.Should().BeFalse("本域接口是可注入面，不得标 IsAbstract");
        api.RegistryGroupName.Should().Be(OpenHardwareRegistryGroupName, "必须挂 OpenHardware 注册组");

        typeof(IWechatWorkOpenHardwareService).GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "不得声明 [Token]：官方凭证既非 access_token / suite_access_token / provider_access_token 任一链路，"
            + "声明后生成器会注入本不该存在的令牌参数");

        // 生成实现类随可注入接口落运行时面 .Internal（N2 通则的域内复述）。
        asm.GetType($"Mud.Wechat.Work.Internal.WechatWorkOpenHardwareService")
            .Should().NotBeNull("生成器实现类须落 Mud.Wechat.Work.Internal");

        OpenHardwareRoutes.Count.Should().Be(17, "路由表与端点表须同批维护");
        typeof(IWechatWorkOpenHardwareService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(17, "本域接口承载全部 17 条官方端点");
    }

    /// <summary>
    /// 契约守卫 OH3：开放硬件域全部契约面类型必须登记进 <c>OpenHardwareJsonContext</c>（AOT 源生成），
    /// 且 <c>SerializerClassName</c> 统一为域段 <c>OpenHardware</c>；类型数漂移须先核对官方文档再同批调整。
    /// </summary>
    [Fact]
    public void OpenHardwareDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = OpenHardwareJsonContext.Default;

        var endpointContractTypes = OpenHardwareEndpoints
            .SelectMany(e => e.Response is null
                ? new[] { e.Request }
                : new[] { e.Request, e.Response })
            .Where(t => t != typeof(WechatWorkResponse))
            .Distinct()
            .ToList();

        foreach (var type in endpointContractTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是开放硬件端点契约面类型，必须登记进 OpenHardwareJsonContext（AOT 源生成）");
        }

        var domainTypes = typeof(GetModelTokenRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == OpenHardwareNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(39,
            "开放硬件域契约面类型数漂移（17 端点 × 请求/响应 + 嵌套结构 + 三支 fail_list 形态）须先核对官方 95980~98024 再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于开放硬件域命名空间，必须登记进 OpenHardwareJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("OpenHardware",
                $"{type.Name} 的 SerializerClassName 必须为开放硬件域段 OpenHardware");
        }
    }

    /// <summary>
    /// 契约守卫 OH4：官方字段名陷阱锁定（照抄原文拼写，勿「顺手驼峰化 / 复数化」）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 关键差异点：<c>user_item</c>（单数键承载列表，95984/96037）、<c>useritems</c>（无下划线复数，
    /// 95984 响应内层）、<c>bind_corpinfo</c>（无下划线复合，95982）、<c>qrcode_content</c>（非
    /// <c>qr_code_content</c>，98023/98024）、<c>openhw</c> 路由段与既有 <c>hardware</c> / <c>devicemgmt</c>
    /// 域前缀互不重叠；体温 <c>temperature</c> 为字符串而非数值。
    /// </para>
    /// </remarks>
    [Fact]
    public void OpenHardwareDtoFieldNames_ShouldMatchOfficialSpelling()
    {
        JsonNameShouldBe(typeof(GetModelTokenRequest), "ModelId", "model_id");
        JsonNameShouldBe(typeof(GetModelTokenRequest), "ModelSecret", "model_secret");
        JsonNameShouldBe(typeof(GetModelTokenRequest), "ModelTicket", "model_ticket");
        JsonNameShouldBe(typeof(GetModelTokenResponse), "ModelAccessToken", "model_access_token");
        JsonNameShouldBe(typeof(GetDeviceSecretRequest), "AuthCode", "auth_code");
        JsonNameShouldBe(typeof(GetDeviceSecretResponse), "DeviceSecret", "device_secret");
        JsonNameShouldBe(typeof(GetDeviceSecretResponse), "DeviceAccessToken", "device_access_token");
        JsonNameShouldBe(typeof(GetDeviceTokenRequest), "DeviceSn", "device_sn");

        JsonNameShouldBe(typeof(RegisterDeviceSnResponse), "QrCode", "qr_code");
        JsonNameShouldBe(typeof(GetDeviceDetailResponse), "DeviceDetail", "device_detail");
        JsonNameShouldBe(typeof(OpenHardwareDeviceDetail), "BindCorpInfo", "bind_corpinfo");
        JsonNameShouldBe(typeof(OpenHardwareDeviceDetail), "DefaultName", "default_name");
        JsonNameShouldBe(typeof(OpenHardwareBindCorpInfo), "OpenCorpId", "open_corpid");

        JsonNameShouldBe(typeof(GetHardwareUserPageRequest), "OnlyPermVersion", "only_perm_version");
        JsonNameShouldBe(typeof(GetHardwareUserByIdsRequest), "UserItem", "user_item");
        JsonNameShouldBe(typeof(GetHardwareUserPageResponse), "NextCursor", "next_cursor");
        JsonNameShouldBe(typeof(GetHardwareUserPageResponse), "PermVersion", "perm_version");
        JsonNameShouldBe(typeof(OpenHardwareUserInfo), "UserItems", "useritems");
        JsonNameShouldBe(typeof(OpenHardwareUserItem), "PassRule", "pass_rule");
        // 96037 响应不含 next_cursor / perm_version（与 95984 形态不同，两 DTO 不共用）。
        typeof(GetHardwareUserByIdsResponse).GetProperty("NextCursor").Should().BeNull(
            "按 id 拉取用户（96037）官方响应无游标/权限版本字段，不得复用分页响应形态");

        JsonNameShouldBe(typeof(ReportDeviceStatusRequest), "UpgradableVersionDesc", "upgradable_version_desc");
        JsonNameShouldBe(typeof(ReportFirmwareUpgradeResultRequest), "OperId", "oper_id");
        JsonNameShouldBe(typeof(ReportBioInfoResultRequest), "BioInfoType", "bio_info_type");
        JsonNameShouldBe(typeof(ReportAccessDataRequest), "AccessData", "access_data");
        JsonNameShouldBe(typeof(OpenHardwareAccessData), "PassMethod", "pass_method");
        JsonNameShouldBe(typeof(ReportCheckinDataRequest), "CheckinData", "checkin_data");
        JsonNameShouldBe(typeof(ReportTemperatureDataRequest), "TemperatureData", "temperature_data");
        JsonNameShouldBe(typeof(OpenHardwareTemperatureData), "Temperature", "temperature");
        JsonNameShouldBe(typeof(ReportAccessDataResponse), "FailList", "fail_list");
        JsonNameShouldBe(typeof(ReportCheckinDataResponse), "FailList", "fail_list");
        JsonNameShouldBe(typeof(ReportTemperatureDataResponse), "FailList", "fail_list");
        // 失败项两形态：95997/95985 仅键三元组，95986 逐条带 errcode/errmsg。
        typeof(OpenHardwareFailedDataKey).GetProperty("ErrCode").Should().BeNull(
            "通行/打卡失败项官方仅 open_userid/user_type/timestamp 三元组");
        JsonNameShouldBe(typeof(OpenHardwareFailedTemperatureItem), "ErrCode", "errcode");

        JsonNameShouldBe(typeof(GenDeviceQrcodeResponse), "QrcodeContent", "qrcode_content");
        JsonNameShouldBe(typeof(ReportRemoteOpenResultRequest), "Type", "type");
    }

    /// <summary>JSON 字段名断言（属性映射的官方字段名必须与官方原文一致）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
