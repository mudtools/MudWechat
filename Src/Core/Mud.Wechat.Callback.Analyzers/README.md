# Mud.Wechat.Callback.Analyzers

Mud.Wechat 回调处理器的**契约分析器**（Roslyn `DiagnosticAnalyzer`）：在编译期校验「处理器的 `SupportedEventType`」与「载荷类的契约声明」是否一致，把运行期的「键写错 → 事件被静默丢弃」提前变成编译错误。

**只诊断、不发射**——运行期行为零变化；`IsPackable=false`，不新增独立 nupkg，而是**随四个回调宿主包内嵌下发**（`analyzers/dotnet/cs`）。

## 内容

单一分析器 `WechatCallbackHandlerAnalyzer`，双 Profile（企微 `[WechatCallbackContract]` + 公众号 `[MpCallbackContract]`），以载荷类型上的契约特性为唯一权威：

| 诊断 | 级别 | 触发条件 |
|---|---|---|
| **MUDCB002** | Error | `SupportedEventType` 的键 ∉ 该载荷的契约键集 ⇒ 运行期会静默跳过该事件（消灭「注册了但从不触发」） |
| **MUDCB003** | Info | 键不是编译期常量（无法静态校验，降级为提示） |
| **MUDCB004** | Warning | 键写成字符串字面量而非 `WechatCallbackEventTypes` / `MpCallbackEventTypes` 常量 |
| **MUDCB005** | Warning | `CompilationEnd`：具体处理器类型未出现在 `AddHandler` / `AddWechatCallback` / `AddMpCallback` 的实参、且无泛型实参或 `typeof` 引用 ⇒ 疑似漏注册 |

豁免（不报）：空键或 `null`（兜底处理器）/ `GenericCallbackPayload` / 类型未标契约特性。

MUDCB005 是**启发式规则、非安全闸**——跨程序集的注册编排不在覆盖范围。严重级别可经 `.editorconfig` 调整。

## 用法

宿主侧无需任何配置：引用 `Mud.Wechat.Work.Callback` / `Mud.Wechat.OfficialAccount.Callback` / `Mud.Wechat.Pay.Callback` 任一 NuGet 包，分析器即随包生效。写错键立即打红：

```csharp
public sealed class MyHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    // 该载荷的契约键集里没有 "create_party" ⇒ MUDCB002（Error）
    public override string SupportedEventType => "create_party";

    public override Task HandleAsync(WechatCallbackEvent evt,
        ContactUserChangedPayload payload, CancellationToken ct) => Task.CompletedTask;
}
```

正确写法引用常量，且载荷契约确有此键：

```csharp
public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
```

SDK 仓库内的消费方式（源码工程直接挂分析器）：

```xml
<ProjectReference Include="..\..\Core\Mud.Wechat.Callback.Analyzers\Mud.Wechat.Callback.Analyzers.csproj"
                  OutputItemType="Analyzer" ReferenceOutputAssembly="false" PrivateAssets="all" />
```

## 依赖

- `Microsoft.CodeAnalysis.CSharp` 4.11.0（`PrivateAssets=all`，禁引 Workspaces）+ `Microsoft.CodeAnalysis.Analyzers` 5.6.0
- **不引用任何被分析的程序集**（红线）：识别类型只靠硬编码 metadata name
- `AdditionalFiles`：`AnalyzerReleases.Shipped.md` / `AnalyzerReleases.Unshipped.md`（MUDCB002~005 的 ID/级别登记）
- 根 `Directory.Build.props` 注入的 5 个运行时依赖（`DependencyInjection.Abstractions` / `Logging.Abstractions` / `Configuration.Binder` / `Options` / `System.Text.Json`）在此显式 `Remove`，保持分析器程序集最小化

下发点（各宿主 csproj 里的 `None Include=... PackagePath="analyzers/dotnet/cs"`）：`Mud.Wechat.Work.Callback`、`Mud.Wechat.OfficialAccount.Callback`、`Mud.Wechat.Pay.Callback`。

## 说明

- **目标框架仅 `netstandard2.0`**（覆写根的多 TFM，Roslyn 组件硬约束）；`IsPackable=false` + `IsRoslynComponent` + `EnforceExtendedAnalyzerRules`。
- **metadata name 静默空跑陷阱**（本工程最易失守处）：分析器按 `GetTypeByMetadataName` 定位 `WechatCallbackContractAttribute` / `MpCallbackContractAttribute` / 处理器基类等类型，取不到即返回 `null` ⇒ **0 诊断且无报错**，守卫形同虚设。Abstractions 侧改命名空间/改名必须同批改这些常量，并由守卫 `WechatCallbackContractGuards` 的 **CB24 反射断言**兜住（唯一能发现该漂移的形态）。
- **全部注册入口必须带异常兜底**：分析器自身抛异常会以 `AD0001` 让宿主**整次编译失败**，故 `Initialize` 与逐符号访问均需 try 包裹。
- 与 `Mud.Wechat.Callback.Generator` 的分工：Generator 发射运行期登记数据（`RegisterAll`）并校验「声明完整性」（MUDCB001/006）；本工程校验「使用侧一致性」（MUDCB002~005）。两者不可互相替代。
- 分析器行为由 `Tests/Mud.Wechat.Work.Callback.Tests` 与 `Tests/Mud.Wechat.OfficialAccount.Callback.Tests` 的 `*HandlerAnalyzerTests` 锁定；宿主包必须内嵌 `analyzers/dotnet/cs` 这一形态由 `WechatAbstractionsContractGuards` 的 **AB-G7** 锁定。
