// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 回调域契约守卫（CB1~CB9，对齐《回调解决方案 v1》§7）：包依赖边界、官方事件键覆盖、
/// 内置授权族兜底处理器、事件 DTO 官方字段、回调凭据唯一来源、echo/被动应答协议、信封无 XML 依赖、
/// 加解密 32 块填充互操作（CB8）与指纹闸次序（CB9）。
/// </summary>
public class WechatCallbackContractGuards
{
    /// <summary>官方事件键全集（授权 InfoType 6 + 通讯录 ChangeType 7 + 异步 Event 1）。</summary>
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
    /// （授权 InfoType 6 + 通讯录 ChangeType 7 + 异步 Event 1），且 <see cref="WechatCallbackEvent.EventTypeKey"/>
    /// 判别优先级为 InfoType → ChangeType → Event（v1 方案 D4）。
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
    /// 契约守卫 CB4：通讯录变更/异步任务事件 DTO 必须暴露官方字段（v1 方案 §6 字段速查）。
    /// </summary>
    [Fact]
    public void ContactChangeEventDtos_ShouldExposeOfficialFields()
    {
        AssertProperties(typeof(UserCreatedEvent), "create_user",
            "UserID", "Name", "Department", "MainDepartment", "IsLeaderInDept", "DirectLeader",
            "Position", "Mobile", "Gender", "Email", "BizMail", "Status", "Avatar", "Alias",
            "Telephone", "Address", "ExtAttr");
        AssertProperties(typeof(UserUpdatedEvent), "update_user",
            "UserID", "NewUserID", "Name", "Department", "MainDepartment", "IsLeaderInDept",
            "DirectLeader", "Position", "Mobile", "Gender", "Email", "BizMail", "Status",
            "Avatar", "Alias", "Telephone", "Address", "ExtAttr");
        AssertProperties(typeof(UserDeletedEvent), "delete_user", "UserID");
        AssertProperties(typeof(PartyCreatedEvent), "create_party", "Id", "Name", "ParentId", "Order");
        AssertProperties(typeof(PartyUpdatedEvent), "update_party", "Id", "Name", "ParentId");
        AssertProperties(typeof(PartyDeletedEvent), "delete_party", "Id");
        AssertProperties(typeof(TagUpdatedEvent), "update_tag",
            "TagId", "AddUserItems", "DelUserItems", "AddPartyItems", "DelPartyItems");
        AssertProperties(typeof(BatchJobResultEvent), "batch_job_result", "JobId", "JobType", "ErrCode", "ErrMsg");

        static void AssertProperties(Type dtoType, string eventName, params string[] expected)
        {
            var props = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => p.Name)
                .ToList();
            foreach (var field in expected)
            {
                props.Should().Contain(field, $"{dtoType.Name}（{eventName}）缺少官方字段 {field}");
            }
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
    [Fact]
    public void CallbackEnvelope_ShouldNotExposeXmlTypes()
    {
        var callbackDir = Path.Combine(GetSolutionRoot(), "Mud.Wechat.Work.Abstractions", "Callback");
        Directory.Exists(callbackDir).Should().BeTrue($"未找到 Abstractions 回调契约目录：{callbackDir}");

        foreach (var file in Directory.GetFiles(callbackDir, "*.cs"))
        {
            var source = File.ReadAllText(file);
            source.Should().NotContain("System.Xml.Linq", $"{file} 不得引入 XML 命名空间");
            source.Should().NotContain("XDocument", $"{file} 不得出现 XDocument");
            source.Should().NotContain("XElement", $"{file} 不得出现 XElement");
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
