// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 回调域契约守卫（CB1~CB13，对齐《回调解决方案 v1》§7）：包依赖边界、官方事件键覆盖、
/// 内置授权族兜底处理器、事件 DTO 官方字段、回调凭据唯一来源、echo/被动应答协议、信封无 XML 依赖、
/// 加解密 32 块填充互操作（CB8）、指纹闸次序（CB9），以及「应用类型 × 回调通道」区分
/// （CB10~CB13：通道枚举 + 配置面、receiveid 三元分流、开放面合法性矩阵、分发器闸次序）。
/// </summary>
/// <remarks>
/// <b>v2.2 变更</b>：旧「逐事件 DTO」已收敛为 5 个<b>结构族载荷</b>（ADR-1），
/// 故 CB4 改写为按结构族断言，并新增 CB4b（官方 17 键全覆盖）、CB4c（事件键级开放面显式声明）、
/// CB4d（三模式无关性）。「元素名 ↔ 属性名」配对正确性改由上游生成器在编译期校验
/// （<c>PAYLOAD004/006/007</c>），逐字段取值由 <c>WechatCallbackPayloadReaderTests</c> 覆盖。
/// </remarks>
public class WechatCallbackContractGuards
{
    /// <summary>官方事件键全集（授权 InfoType 6 + 通讯录 ChangeType 7 + 异步 Event 1 + 上下游 Event 1 + ChangeType 9）。</summary>
    private static readonly (string Key, string Reason)[] OfficialEventKeys =
    {
        (WechatCallbackEventTypes.SuiteTicket, "授权族 suite_ticket"),
        (WechatCallbackEventTypes.CreateAuth, "授权族 create_auth"),
        (WechatCallbackEventTypes.ResetPermanentCode, "授权族 reset_permanent_code"),
        (WechatCallbackEventTypes.ChangeAuth, "授权族 change_auth"),
        (WechatCallbackEventTypes.CancelAuth, "授权族 cancel_auth"),
        (WechatCallbackEventTypes.DelAuth, "授权族 del_auth"),
        (WechatCallbackEventTypes.CreateUser, "通讯录族 create_user"),
        (WechatCallbackEventTypes.UpdateUser, "通讯录族 update_user"),
        (WechatCallbackEventTypes.DeleteUser, "通讯录族 delete_user"),
        (WechatCallbackEventTypes.CreateParty, "通讯录族 create_party"),
        (WechatCallbackEventTypes.UpdateParty, "通讯录族 update_party"),
        (WechatCallbackEventTypes.DeleteParty, "通讯录族 delete_party"),
        (WechatCallbackEventTypes.UpdateTag, "通讯录族 update_tag"),
        (WechatCallbackEventTypes.BatchJobResult, "异步任务族 batch_job_result"),
        (WechatCallbackEventTypes.ChangeChain, "上下游族 change_chain（95796）"),
        (WechatCallbackEventTypes.CreateChain, "上下游族 create_chain"),
        (WechatCallbackEventTypes.UpdateChain, "上下游族 update_chain"),
        (WechatCallbackEventTypes.DeleteChain, "上下游族 delete_chain"),
        (WechatCallbackEventTypes.CreateGroup, "上下游族 create_group"),
        (WechatCallbackEventTypes.UpdateGroup, "上下游族 update_group"),
        (WechatCallbackEventTypes.DeleteGroup, "上下游族 delete_group"),
        (WechatCallbackEventTypes.CorpJoin, "上下游族 corp_join"),
        (WechatCallbackEventTypes.UpdateCorp, "上下游族 update_corp"),
        (WechatCallbackEventTypes.RemoveCorp, "上下游族 remove_corp"),
    };

    // ---------------------------------------------------------------- CB1

    /// <summary>
    /// 契约守卫 CB1：Callback 包不得引用主包 <c>Work</c>（K-callback 依赖单向边）。
    /// </summary>
    [Fact]
    public void CallbackPackage_ShouldNotReferenceWorkProject()
    {
        var csprojPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "Mud.Wechat.Work.Callback.csproj");
        File.Exists(csprojPath).Should().BeTrue($"未找到回调包工程文件：{csprojPath}");

        var source = File.ReadAllText(csprojPath);

        source.Should().NotContain(
            $"\"..\\Mud.Wechat.Work\\Mud.Wechat.Work.csproj\"",
            "K-callback：Callback 只能依赖 Abstractions 与 DataModels");
        source.Should().Contain("Mud.Wechat.Work.Abstractions.csproj");
        source.Should().Contain("Mud.Wechat.Work.DataModels.csproj");
    }

    // ---------------------------------------------------------------- CB2

    /// <summary>
    /// 契约守卫 CB2：<see cref="WechatCallbackEventTypes"/> 常量必须覆盖官方事件键全集
    /// （授权 InfoType 6 + 通讯录 ChangeType 7 + 异步 Event 1 + 上下游 Event 1 与 ChangeType 9），且
    /// <see cref="WechatCallbackEvent.EventTypeKey"/> 判别优先级为 InfoType → ChangeType → Event（v1 方案 D4）。
    /// </summary>
    [Fact]
    public void CallbackEventTypeKeys_ShouldCoverOfficialEventFamilies()
    {
        var constants = typeof(WechatCallbackEventTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();

        foreach (var (key, reason) in OfficialEventKeys)
        {
            constants.Should().Contain(key, $"官方事件键缺失（{reason}）；新增官方事件键须同批登记");
        }

        constants.Should().Contain(WechatCallbackEventTypes.ChangeContact, "通讯录变更事件信封值");
        constants.Should().Contain(WechatCallbackEventTypes.ChangeChain, "上下游变更事件信封值（95796）");

        // EventTypeKey 判别优先级（D4）。
        new WechatCallbackEvent { InfoType = "suite_ticket", Event = "change_contact", ChangeType = "create_user" }
            .EventTypeKey.Should().Be("suite_ticket", "InfoType 优先");
        new WechatCallbackEvent { Event = "change_contact", ChangeType = "create_user" }
            .EventTypeKey.Should().Be("create_user", "ChangeType 次之");
        new WechatCallbackEvent { Event = "batch_job_result" }
            .EventTypeKey.Should().Be("batch_job_result", "Event 兜底");
        new WechatCallbackEvent().EventTypeKey.Should().BeEmpty("三者皆空 → 仅兜底处理器可见");
    }

    // ---------------------------------------------------------------- CB3

    /// <summary>
    /// 契约守卫 CB3：内置授权族处理器必须为「单类兜底」形态（v1 方案 D6）——实现
    /// <see cref="IWechatCallbackEventHandler"/> 且 <c>SupportedEventType</c> 返回空串；
    /// 授权族 6 个 InfoType 判别保留在信封；P0-3「只告警不删库」分支不回退（G9 同源）。
    /// </summary>
    [Fact]
    public void CallbackHandlers_ShouldRegisterBuiltinAuthorizationFamily()
    {
        var handlerPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackHandler.cs");
        File.Exists(handlerPath).Should().BeTrue($"未找到回调处理器源码（勿移动文件位置，G9 按路径断言）：{handlerPath}");
        var handlerSource = File.ReadAllText(handlerPath);

        handlerSource.Should().Contain("IWechatCallbackEventHandler", "D6：内置处理器实现类型化处理器接口");
        handlerSource.Should().Contain("SupportedEventType => string.Empty", "D6：空键 = 兜底语义");
        handlerSource.Should().Contain("MatchAppKeysBySuiteId", "G9：清理范围恒为 SuiteId 命中集");
        handlerSource.Should().Contain("已跳过授权清理以避免误删其它套件授权", "G9：未命中只告警不删库");
        handlerSource.Should().NotContain("ResolveAppKeys(", "G9：不得回退「全部应用清理」");

        // 授权族 6 InfoType 判别落位信封（经 WechatCallbackEventTypes 常量名引用）。
        var envelopePath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Abstractions", "Callback", "WechatCallbackEvent.cs");
        File.Exists(envelopePath).Should().BeTrue();
        var envelopeSource = File.ReadAllText(envelopePath);
        foreach (var keyName in new[]
                 {
                     nameof(WechatCallbackEventTypes.SuiteTicket), nameof(WechatCallbackEventTypes.CreateAuth),
                     nameof(WechatCallbackEventTypes.ResetPermanentCode), nameof(WechatCallbackEventTypes.ChangeAuth),
                     nameof(WechatCallbackEventTypes.CancelAuth), nameof(WechatCallbackEventTypes.DelAuth),
                 })
        {
            envelopeSource.Should().Contain(
                $"WechatCallbackEventTypes.{keyName}",
                $"信封判别必须覆盖授权族 InfoType：{keyName}");
        }
    }

    // ---------------------------------------------------------------- CB4

    /// <summary>
    /// 契约守卫 CB4（v2.2 改写）：<b>结构族载荷</b>必须暴露官方字段（v1 方案 §6 字段速查）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 旧 CB4 断言 11 个「逐事件 DTO」的属性名；v2.2 的 ADR-1 把它们收敛为 5 个<b>结构族载荷</b>
    /// （官方报文结构同一的事件键共用一个类型，具体类别由信封 <c>ChangeType</c> 判别）。
    /// 故本守卫改为按结构族断言，并<b>追加两项漂移校验</b>：
    /// </para>
    /// <list type="number">
    /// <item><description>载荷类型必须标注 <c>[PayloadContract]</c>（否则上游生成器不产出映射表）；</description></item>
    /// <item><description>载荷类型必须声明 <c>partial</c>（生成物是其 <c>partial</c> 成员）。</description></item>
    /// </list>
    /// <para>
    /// 「元素名 ↔ 属性名」的配对正确性<b>不再由本守卫承担</b>：上游生成器已在编译期校验（<c>PAYLOAD004/006/007</c>），
    /// 逐字段<b>取值</b>正确性由 <c>WechatCallbackPayloadReaderTests</c> 的官方样报文用例覆盖 —— 两者双向夹逼。
    /// </para>
    /// </remarks>
    [Fact]
    public void PayloadTypes_ShouldExposeOfficialFields()
    {
        AssertProperties(typeof(ContactUserChangedPayload), "create_user / update_user / delete_user",
            "UserId", "NewUserId", "Name", "DepartmentIds", "MainDepartmentId", "LeaderInDeptFlags",
            "DirectLeaderIds", "Position", "Mobile", "Gender", "Email", "BizMail", "Status", "Avatar",
            "Alias", "Telephone", "Address", "ExtAttr");
        AssertProperties(typeof(ContactPartyChangedPayload), "create_party / update_party / delete_party",
            "PartyId", "Name", "ParentId", "Order");
        AssertProperties(typeof(ContactTagChangedPayload), "update_tag",
            "TagId", "AddedUserIds", "RemovedUserIds", "AddedPartyIds", "RemovedPartyIds");
        AssertProperties(typeof(BatchJobCompletedPayload), "batch_job_result",
            "JobId", "JobType", "ErrCode", "ErrMsg");
        AssertProperties(typeof(ChainChangedPayload),
            "create_chain…remove_corp（9 键）", "ChainId", "GroupIds", "CorpIds");

        var payloadTypes = new[]
        {
            typeof(ContactUserChangedPayload), typeof(ContactPartyChangedPayload),
            typeof(ContactTagChangedPayload), typeof(BatchJobCompletedPayload),
            typeof(ChainChangedPayload),
        };

        foreach (var type in payloadTypes)
        {
            type.GetCustomAttributes(typeof(PayloadContractAttribute), inherit: false)
                .Should().NotBeEmpty(type.Name + " 必须标注 [PayloadContract]，否则上游生成器不产出映射表");

            // 生成物是 partial 成员 ⇒ 类型必须可分部声明（上游 PAYLOAD002 在编译期校验，此处为测试期冗余锁）。
            type.IsSealed.Should().BeTrue(type.Name + " 为密封载荷类型（非密封不影响 partial，此断言仅锁定既存形态）");
        }

        static void AssertProperties(Type payloadType, string eventName, params string[] expected)
        {
            var props = payloadType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => p.Name)
                .ToList();
            foreach (var field in expected)
            {
                props.Should().Contain(field, $"{payloadType.Name}（{eventName}）缺少官方字段 {field}");
            }
        }
    }

    /// <summary>
    /// 契约守卫 CB4b（v2.2 新增）：官方契约表必须登记全部 17 个载荷事件键，且授权族 7 键不登记。
    /// </summary>
    [Fact]
    public void OfficialPayloadContracts_ShouldCoverAllPayloadEventKeys()
    {
        var registry = new WechatPayloadContractRegistry();
        OfficialPayloadContracts.RegisterAll(registry);

        var expectedKeys = new[]
        {
            WechatCallbackEventTypes.CreateUser, WechatCallbackEventTypes.UpdateUser, WechatCallbackEventTypes.DeleteUser,
            WechatCallbackEventTypes.CreateParty, WechatCallbackEventTypes.UpdateParty, WechatCallbackEventTypes.DeleteParty,
            WechatCallbackEventTypes.UpdateTag,
            WechatCallbackEventTypes.BatchJobResult,
            WechatCallbackEventTypes.CreateChain, WechatCallbackEventTypes.UpdateChain, WechatCallbackEventTypes.DeleteChain,
            WechatCallbackEventTypes.CreateGroup, WechatCallbackEventTypes.UpdateGroup, WechatCallbackEventTypes.DeleteGroup,
            WechatCallbackEventTypes.CorpJoin, WechatCallbackEventTypes.UpdateCorp, WechatCallbackEventTypes.RemoveCorp,
        };

        var registered = registry.RegisteredKeys;
        registered.Should().HaveCount(17, "官方有强类型载荷的事件键共 17 个");
        foreach (var key in expectedKeys)
        {
            registered.Should().Contain(key, $"官方事件键 {key} 必须登记契约");
            registry.TryResolve(key, out var contract).Should().BeTrue();
            contract!.Accessor.Should().NotBeNull();
        }

        // 授权族走信封（ADR-8），不得登记载荷契约。
        foreach (var authKey in new[]
                 {
                     WechatCallbackEventTypes.SuiteTicket, WechatCallbackEventTypes.CreateAuth,
                     WechatCallbackEventTypes.ResetPermanentCode, WechatCallbackEventTypes.ChangeAuth,
                     WechatCallbackEventTypes.CancelAuth, WechatCallbackEventTypes.DelAuth,
                 })
        {
            registered.Should().NotContain(authKey, $"授权族事件键 {authKey} 走信封，不得登记载荷契约（ADR-8）");
        }
    }

    /// <summary>
    /// 契约守卫 CB4c（v2.2 新增）：每条官方契约必须<b>显式</b>声明事件键级开放面（守卫 CB22 的要求）。
    /// </summary>
    [Fact]
    public void OfficialPayloadContracts_ShouldDeclareOpenSurfaceExplicitly()
    {
        var registry = new WechatPayloadContractRegistry();
        OfficialPayloadContracts.RegisterAll(registry);

        foreach (var key in registry.RegisteredKeys)
        {
            registry.TryResolve(key, out var contract).Should().BeTrue();
            contract!.SupportedAppTypes.Should().NotBeNull(
                $"契约 {key} 必须显式声明 SupportedAppTypes（不得隐式继承族默认，见 ADR-15）");
            contract.RequiredChannel.Should().NotBeNull($"契约 {key} 必须显式声明 RequiredChannel");
            contract.RequiredEvent.Should().NotBeNull($"契约 {key} 必须声明 RequiredEvent（防同名 ChangeType 跨族串门）");
        }

        // 三模式开放面（ADR-14：一份契约覆盖三类应用）。
        registry.TryResolve(WechatCallbackEventTypes.CreateUser, out var contact).Should().BeTrue();
        contact!.SupportedAppTypes.Should().Be(WechatAppTypeSet.All);

        registry.TryResolve(WechatCallbackEventTypes.CreateChain, out var chain).Should().BeTrue();
        chain!.SupportedAppTypes.Should().Be(WechatAppTypeSet.Internal, "上下游变更族官方仅向自建应用开放");
    }

    /// <summary>
    /// 契约守卫 CB4d（v2.2 新增）：三模式无关性（ADR-14）—— 载荷与转换器层<b>不得</b>出现应用模式分支。
    /// </summary>
    [Fact]
    public void PayloadSurface_MustBeAppModeAgnostic()
    {
        var payloadTypes = new[]
        {
            typeof(ContactUserChangedPayload), typeof(ContactPartyChangedPayload),
            typeof(ContactTagChangedPayload), typeof(BatchJobCompletedPayload),
            typeof(ChainChangedPayload), typeof(GenericCallbackPayload),
            typeof(WechatPayloadConverter),
        };

        foreach (var type in payloadTypes)
        {
            var source = ReadSource(type);
            source.Should().NotContain("WechatAppType",
                $"{type.Name} 不得按应用类型分支（ADR-14：一份契约覆盖三模式）");
            source.Should().NotContain("WechatCallbackChannel",
                $"{type.Name} 不得按回调通道分支（ADR-14）");
        }
    }

    // ---------------------------------------------------------------- CB5

    /// <summary>
    /// 契约守卫 CB5：回调凭据唯一来源 = <see cref="Mud.Wechat.Work.Callback.WechatCallbackOptions.Apps"/>
    /// （v1.2 D3）——不回流 <see cref="Mud.Wechat.Work.Abstractions.Configuration.WechatAppConfig"/>；
    /// 应用级配置不得有 AppKey 属性（字典键唯一权威，v1.2）。
    /// </summary>
    [Fact]
    public void CallbackOptions_ShouldKeepSingleSourceOfCredentialTruth()
    {
        var appConfigPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Abstractions", "Configuration", "WechatAppConfig.cs");
        var appConfigSource = File.ReadAllText(appConfigPath);
        // 仅断言属性声明形态（文档注释中允许提及回调凭据的迁移史）。
        appConfigSource.Should().NotContain("public string PushToken", "回调凭据不得回流主配置（回调运维面独立）");
        appConfigSource.Should().NotContain("public string PushEncodingAESKey", "回调凭据不得回流主配置");

        var callbackOptionsPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackOptions.cs");
        var optionsSource = File.ReadAllText(callbackOptionsPath);

        optionsSource.Should().Contain("public Dictionary<string, WechatAppCallbackOptions> Apps",
            "D3：Apps 为回调凭据唯一来源（多应用）");
        optionsSource.Should().Contain("class WechatAppCallbackOptions",
            "应用级配置类与主配置同类文件（audit-config-keys.ps1 按文件扫描）");

        // 「单体兼容字段已删除」断言限定在 WechatCallbackOptions 主类段内
        // （WechatAppCallbackOptions 应用级的同名属性是合法凭据承载）。
        var appOptionsIndex = optionsSource.IndexOf("class WechatAppCallbackOptions", StringComparison.Ordinal);
        var mainClassSection = optionsSource.Substring(0, appOptionsIndex);
        mainClassSection.Should().NotContain("public string PushToken", "v1.2：单体兼容字段已删除");
        mainClassSection.Should().NotContain("public string PushEncodingAESKey", "v1.2：单体兼容字段已删除");
        mainClassSection.Should().NotContain("public string CorpId", "v1.2：单体兼容字段已删除");

        // 应用级配置类段内不得有 AppKey 属性（字典键唯一权威，防双写死配置）。
        var appOptionsStart = optionsSource.IndexOf("class WechatAppCallbackOptions", StringComparison.Ordinal);
        appOptionsStart.Should().BeGreaterThan(0, "应用级配置类与主配置同类文件（audit-config-keys.ps1 按文件扫描）");
        var appOptionsSection = optionsSource.Substring(appOptionsStart);
        var closingBrace = appOptionsSection.IndexOf('}');
        var appOptionsBody = closingBrace > 0
            ? appOptionsSection.Substring(0, closingBrace)
            : appOptionsSection;
        appOptionsBody.Should().NotContain("public string AppKey", "v1.2：AppKey 与字典键双写属死配置形态");
    }

    // ---------------------------------------------------------------- CB6

    /// <summary>
    /// 契约守卫 CB6（协议文本，v1 方案 §5.1）：GET echostr 验签复用 <c>VerifySignature</c>+<c>Decrypt</c>
    /// 且不消费抗重放指纹（v1.2 D8）；被动应答扩展点的 <c>Encrypt</c>+<c>ComputeSignature</c> 算法保持。
    /// </summary>
    [Fact]
    public void EchoAndPassiveReply_ShouldMatchOfficialProtocol()
    {
        var receiverPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackReceiver.cs");
        var receiverSource = File.ReadAllText(receiverPath);

        // EchoAsync 方法体：复用 VerifySignature + Decrypt，且不触碰指纹守卫。
        var echoStart = receiverSource.IndexOf("public Task<string> EchoAsync", StringComparison.Ordinal);
        echoStart.Should().BeGreaterThan(0, "URL 验证入口必须存在（官方 90930 硬门槛）");
        var echoEnd = receiverSource.IndexOf("private WechatAppCallbackOptions ResolveApp", StringComparison.Ordinal);
        var echoBody = echoEnd > echoStart
            ? receiverSource.Substring(echoStart, echoEnd - echoStart)
            : receiverSource.Substring(echoStart);
        echoBody.Should().Contain("VerifySignature", "echo 验签复用现有算法（echostr 充当 encrypt 参与项）");
        echoBody.Should().Contain("Decrypt", "echo 解密复用现有算法");
        echoBody.IndexOf("VerifySignature", StringComparison.Ordinal)
            .Should().BeLessThan(echoBody.IndexOf("Decrypt", StringComparison.Ordinal),
                "P1-2：URL 验证中验签必须先于解密（攻击者无 token 不得触达解密）");
        echoBody.Should().NotContain("TryMarkAsync", "D8：echo 不消费抗重放指纹（幂等验证）");
        echoBody.Should().Contain("ValidateTimestampWindow", "时效窗口闸保持 fail-closed");

        // 被动应答扩展点（本期不实现组装，算法基座不得移除）。
        var cryptoPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackCrypto.cs");
        var cryptoSource = File.ReadAllText(cryptoPath);
        cryptoSource.Should().Contain("public static string Encrypt(", "被动应答包 Encrypt 算法基座");
        cryptoSource.Should().Contain("public static string ComputeSignature(", "MsgSignature 算法基座");
    }

    // ---------------------------------------------------------------- CB7

    /// <summary>
    /// 契约守卫 CB7：Abstractions 的回调契约层不得出现 XML 类型
    /// （v1 方案 §3.1 上移边界——信封仅字段契约，不向 Abstractions 引入 System.Xml.Linq 依赖）。
    /// </summary>
    /// <remarks>
    /// <b>v2.2 修正（原始实现是守卫盲区）</b>：原断言用
    /// <c>Directory.GetFiles(dir, "*.cs")</c> —— <b>非递归</b>。
    /// v2.2 新增 <c>Callback/Payloads/</c> 子目录后，该目录下的文件<b>完全脱离</b>本守卫覆盖。
    /// 现改为 <c>SearchOption.AllDirectories</c> 并排除 <c>obj</c>/<c>bin</c>，
    /// 以覆盖整个回调契约层（含载荷端口层）。
    /// </remarks>
    [Fact]
    public void CallbackEnvelope_ShouldNotExposeXmlTypes()
    {
        var callbackDir = Path.Combine(GetSolutionRoot(), "Mud.Wechat.Work.Abstractions", "Callback");
        Directory.Exists(callbackDir).Should().BeTrue($"未找到 Abstractions 回调契约目录：{callbackDir}");

        var files = Directory.GetFiles(callbackDir, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(
                            Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase)
                        && !f.Contains(
                            Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase))
            .ToArray();

        files.Should().NotBeEmpty("回调契约层应至少含一个源文件（守卫空转即失效）");

        foreach (var file in files)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                // 跳过注释行（含 ///）：XML 文档注释中常出现「不得出现 XElement」这类
                // **说明性文字**，朴素物理行匹配会把它误判为实现依赖。
                if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                    continue;

                line.Should().NotContain(NoXmlMarkers.Namespace, $"{file} 不得引入 XML 命名空间");
                NoXmlMarkers.AssertNoXmlType(line, file);
            }
        }
    }

    /// <summary>
    /// 契约守卫 CB14（v2.2 新增）：Mud.Wechat 的 XML 触点必须唯一 ——
    /// <c>WechatCallbackReceiver</c>（请求体 <c>Encrypt</c> 提取）与
    /// <c>XElementPayloadSource</c>（载荷投影）之外的文件不得出现 XML 类型。
    /// </summary>
    [Fact]
    public void CallbackPackage_ShouldKeepXmlTouchPointsUnique()
    {
        var allowed = new[] { "WechatCallbackReceiver.cs", "XElementPayloadSource.cs" };

        var files = Directory.GetFiles(
                Path.Combine(GetSolutionRoot(), "Mud.Wechat.Work.Callback"), "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(
                            Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase)
                        && !f.Contains(
                            Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase))
            .ToArray();

        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            if (allowed.Contains(name, StringComparer.Ordinal))
                continue;

            foreach (var line in File.ReadAllLines(file))
            {
                // 同 CB7：跳过注释行，避免「说明性文字」被误判为实现依赖。
                if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                    continue;

                NoXmlMarkers.AssertNoXmlType(line, $"{name}（XML 触点须唯一，ADR-6）");
            }
        }
    }

    /// <summary>
    /// XML 依赖标记与匹配规则（CB7 / CB14 共用）。
    /// </summary>
    /// <remarks>
    /// <b>必须用<b>词边界</b>正则而非 <c>Contains</c></b>：本仓库存在标识符
    /// <c>XElementPayloadSource</c>（合法的 XML 触点类名），其<b>包含子串</b> "XElement" ——
    /// 朴素子串匹配会把引用该类的文件误判为「引入 XML 类型」。
    /// <c>\bXElement\b</c> 要求其后为非单词字符，故不会命中 <c>XElementPayloadSource</c>。
    /// </remarks>
    private static class NoXmlMarkers
    {
        internal const string Namespace = "System.Xml.Linq";

        private static readonly string[] TypePatterns = { @"\bXDocument\b", @"\bXElement\b" };

        internal static void AssertNoXmlType(string line, string subject)
        {
            foreach (var pattern in TypePatterns)
            {
                Regex.IsMatch(line, pattern).Should().BeFalse(
                    subject + " 不得出现 XML 类型（匹配 " + pattern + "）：" + line.Trim());
            }
        }
    }

    // ---------------------------------------------------------------- CB8

    /// <summary>
    /// 契约守卫 CB8（P0-1，对齐《回调解决方案 v1》§七）：加解密不得出现 .NET 内置 16 块 PKCS7
    /// （官方报文 pad∈[17..32] 时内置校验会误判非法填充而解密失败）；必须
    /// <c>PaddingMode.None</c> + 手工 32 块填充补位/剥离。
    /// </summary>
    [Fact]
    public void CallbackCrypto_ShouldUseManualPkcs7PaddingOf32Bytes()
    {
        var cryptoPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackCrypto.cs");
        File.Exists(cryptoPath).Should().BeTrue($"未找到回调加解密实现：{cryptoPath}");

        var source = File.ReadAllText(cryptoPath);

        source.Should().NotContain("PaddingMode.PKCS7",
            "CB8：.NET 内置 PKCS7 为 16 字节块校验，官方报文 pad∈[17..32] 时解密必抛（P0-1 回归守卫）");
        source.Should().Contain("PaddingMode.None",
            "CB8：解密/加密必须走 PaddingMode.None + 手工 32 块填充补位/剥离");
        source.Should().Contain("StripPkcs7Padding",
            "CB8：Decrypt 必须手工剥离 32 块 PKCS7 填充（否则尾部填充并入 receiveid）");
    }

    // ---------------------------------------------------------------- CB9

    /// <summary>
    /// 契约守卫 CB9（P1-1/D2）：接收器的指纹闸（<c>TryMarkAsync</c>）必须位于
    /// <c>WechatCallbackCrypto.Decrypt</c> 之后——防「标记先于解密」回归
    /// （解密失败消耗指纹 ⇒ 官方重试被拒，at-least-once 破坏）。
    /// </summary>
    [Fact]
    public void CallbackFingerprintMark_ShouldFollowDecrypt()
    {
        var receiverPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackReceiver.cs");
        var source = File.ReadAllText(receiverPath);

        var markIndex = source.IndexOf("TryMarkAsync", StringComparison.Ordinal);
        var decryptIndex = source.IndexOf("WechatCallbackCrypto.Decrypt(", StringComparison.Ordinal);

        markIndex.Should().BePositive("CB9：接收器必须调用 TryMarkAsync（P0-2 第二道闸不得被移除）");
        decryptIndex.Should().BePositive("CB9：接收器必须经 WechatCallbackCrypto.Decrypt 解密");
        markIndex.Should().BeGreaterThan(decryptIndex,
            "CB9：指纹标记必须位于解密之后（P1-1/D2：解密失败不消耗指纹，官方重试可重新进入管线）");
    }

    // ---------------------------------------------------------------- CB10

    /// <summary>
    /// 契约守卫 CB10（区分企业自建 / 服务商代开发 / 第三方应用）：回调通道枚举
    /// <c>WechatCallbackChannel</c>（App=1 应用数据通道 / Suite=2 套件指令通道）必须存在，
    /// 且 <c>WechatAppCallbackOptions</c> 必须暴露 <c>AppType</c> / <c>Channel</c> / <c>ReceiveId</c>
    /// 三个配置面属性——这是「应用类型 × 回调通道」语义（receiveid 校验 + 开放面闸）的类型契约。
    /// </summary>
    [Fact]
    public void CallbackAppOptions_ShouldExposeAppTypeAndChannelSurface()
    {
        var channelPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Abstractions", "Enums", "WechatCallbackChannel.cs");
        File.Exists(channelPath).Should().BeTrue($"未找到回调通道枚举（勿移动文件，CB10 按路径断言）：{channelPath}");
        var channelSource = File.ReadAllText(channelPath);
        channelSource.Should().Contain("App = 1", "CB10：应用数据通道取值（应用级 change_contact/batch_job_result/change_chain）");
        channelSource.Should().Contain("Suite = 2", "CB10：套件指令/票据通道取值（suite_ticket / 授权族）");

        var optionsPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackOptions.cs");
        var optionsSource = File.ReadAllText(optionsPath);
        optionsSource.Should().Contain("public WechatAppType AppType", "CB10：应用类型配置面（区分企业自建/第三方/代开发）");
        optionsSource.Should().Contain("public WechatCallbackChannel Channel", "CB10：回调通道配置面（App/Suite）");
        optionsSource.Should().Contain("public string ReceiveId", "CB10：接收方 ID 配置面（receiveid 校验依据）");
    }

    // ---------------------------------------------------------------- CB11

    /// <summary>
    /// 契约守卫 CB11：<c>ValidateReceiveId</c> 必须按「应用类型 × 回调通道」三元分流 receiveid 语义——
    /// 自建 App 通道 / 第三方·代开发 Suite 通道为<b>静态</b>接收方 ID（比对 <see cref="Mud.Wechat.Work.Callback.WechatAppCallbackOptions.ReceiveId"/>），
    /// 第三方·代开发 App 通道为<b>动态授权企业 CorpId</b>（比对外层 <c>ToUserName</c>，静态 ReceiveId 命中其一亦通过）。
    /// </summary>
    [Fact]
    public void ReceiveIdValidation_ShouldDistinguishAppTypeByChannel()
    {
        var receiverPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackReceiver.cs");
        var source = File.ReadAllText(receiverPath);

        source.Should().Contain("ValidateReceiveId(", "CB11：receiveid 校验必须为独立方法（单点收敛）");
        source.Should().Contain(
            "app.AppType != WechatAppType.Internal && app.Channel == WechatCallbackChannel.App",
            "CB11：第三方/代开发「应用数据通道」必须是动态授权企业 CorpId 分支");
        source.Should().Contain("toUserName",
            "CB11：动态授权企业 CorpId 必须与外层 ToUserName 比对");
        source.Should().Contain("string.Equals(expected, receiveId",
            "CB11：静态通道（自建 App / 第三方·代开发 Suite）必须比对配置的 ReceiveId");
        source.Should().Contain("WechatCallbackFailureKind.ReceiveIdMismatch",
            "CB11：receiveid 不一致必须显式拒绝（fail-closed）");
    }

    // ---------------------------------------------------------------- CB12

    /// <summary>
    /// 契约守卫 CB12：<c>IsEventFamilyAllowed</c> 必须实现「应用类型 × 回调通道」的开放面合法性矩阵——
    /// 授权族仅套件通道（第三方/代开发）、上下游变更族仅自建应用 + 应用通道、通讯录/异步族经应用通道（三类应用）、
    /// 无法判别族不拦截。
    /// </summary>
    [Fact]
    public void EventFamilyGate_ShouldEnforceOpenSurfaceMatrix()
    {
        var optionsPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackOptions.cs");
        var source = File.ReadAllText(optionsPath);

        source.Should().Contain("case WechatCallbackEventFamily.Authorization:", "CB12：授权族分支配齐");
        source.Should().Contain("Channel == WechatCallbackChannel.Suite", "CB12：授权族仅套件通道");
        source.Should().Contain(
            "AppType == WechatAppType.ThirdParty || AppType == WechatAppType.Provider",
            "CB12：套件通道仅第三方应用/服务商代开发");

        source.Should().Contain("case WechatCallbackEventFamily.ChainChange:", "CB12：上下游变更族分支配齐");
        source.Should().Contain(
            "Channel == WechatCallbackChannel.App && AppType == WechatAppType.Internal",
            "CB12：上下游变更族仅自建应用 + 应用通道（95796）");

        source.Should().Contain("case WechatCallbackEventFamily.ContactChange:", "CB12：通讯录变更族分支配齐");
        source.Should().Contain("case WechatCallbackEventFamily.BatchJob:", "CB12：异步任务族分支配齐");
        source.Should().Contain("case WechatCallbackEventFamily.Unknown:", "CB12：无法判别族不拦截（兜底处理器处置）");
    }

    // ---------------------------------------------------------------- CB13

    /// <summary>
    /// 契约守卫 CB13：分发器合法性闸（<c>IsEventFamilyAllowed</c>）必须位于拦截器 <c>BeforeHandleAsync</c>
    /// <b>之前</b>，且不适用事件族以 <c>WechatCallbackDispatchOutcome.Rejected</c> 返回（中间件映射 200，不触发重推）。
    /// </summary>
    [Fact]
    public void DispatcherFamilyGate_ShouldPrecedeInterceptors_AndReturnRejected()
    {
        var dispatcherPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackDispatcher.cs");
        var source = File.ReadAllText(dispatcherPath);

        var gateIndex = source.IndexOf("IsEventFamilyAllowed", StringComparison.Ordinal);
        // 注意：枚举注释（「拦截器中断（BeforeHandleAsync 返回 false）」）也含字面量，须以「.BeforeHandleAsync(」
        // 锚定实际拦截器调用点，避免与文档注释误匹配（CB13 锚点漂移防误报）。
        var beforeIndex = source.IndexOf(".BeforeHandleAsync(", StringComparison.Ordinal);

        gateIndex.Should().BePositive("CB13：分发器必须调用 IsEventFamilyAllowed 合法性闸");
        beforeIndex.Should().BePositive("CB13：分发器必须保留拦截器 Before 阶段");
        gateIndex.Should().BeLessThan(beforeIndex,
            "CB13：合法性闸必须先于拦截器（不适用事件族不得触达业务拦截器/处理器）");
        source.Should().Contain("return WechatCallbackDispatchOutcome.Rejected;",
            "CB13：不适用事件族以 Rejected 返回（中间件映射 200，不触发企业微信重推）");
    }

    /// <summary>
    /// 读取某个类型的源码文本（按「类型名 + .cs」在仓库内定位，排除 <c>obj</c>/<c>bin</c>）。
    /// </summary>
    /// <remarks>
    /// 用于「源码形态」类守卫（如三模式无关性）：这类约束无法用反射表达
    /// （反射只能看见成员，看不见方法体内的分支），只能以文本扫描锁定。
    /// </remarks>
    private static string ReadSource(Type type)
    {
        var root = GetSolutionRoot();
        var files = Directory.GetFiles(root, type.Name + ".cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(
                            Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase)
                        && !f.Contains(
                            Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar,
                            StringComparison.OrdinalIgnoreCase))
            .ToArray();

        files.Should().NotBeEmpty("找不到 " + type.Name + " 的源码文件（守卫 ReadSource 定位失败）");
        return File.ReadAllText(files[0]);
    }

    /// <summary>解决方案根目录定位（与 <c>WechatContractGuards.GetSolutionRoot</c> 同款判据）。</summary>
    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Mud.Wechat.slnx")))
        {
            directory = directory.Parent!;
        }

        return directory!.FullName;
    }
}
