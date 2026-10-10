# Mud.Wechat.Callback.Generator

Mud.Wechat 回调契约的**登记发射源生成器**（Roslyn `IIncrementalGenerator`）：扫描载荷类上的契约特性，在编译期发射契约登记表 `RegisterAll` 的方法体。

**中立名工程**——一套代码同时服务两条产品线的两个宿主，`IsPackable=false`，只作构建期资产（`OutputItemType="Analyzer"`），不进发布链。

## 内容

两个生成器，各持一条产品线的宿主闸与契约特性：

| 生成器 | 扫描特性 | 宿主（发射目标） | 产出文件 |
|---|---|---|---|
| `WechatCallbackContractRegistrationGenerator` | `[WechatCallbackContract]`（Abstractions 侧） | `Mud.Wechat.Work.Callback.Events.Payloads.OfficialPayloadContracts` | `OfficialPayloadContracts.RegisterAll.g.cs` |
| `MpCallbackContractRegistrationGenerator` | `[MpCallbackContract]` | `MpPayloadContracts`（公众号 Abstractions 侧） | `MpPayloadContracts.RegisterAll.g.cs` |

发射形态为每事件键一条 `registry.Register(...)`，把「事件键 + `PayloadFieldMap` + 开放面 + 族前置条件 + 必带事件值」一次写全：

```csharp
registry.Register("create_user", ContactUserChangedPayload.PayloadFieldMap,
    surfaces, requiredEvent, requiredFamily);
```

诊断（声明不完整即编译期打红，杜绝「登记缺键 → 运行期静默降级通用载荷」）：

- **MUDCB001**（Error，企微侧）：`OpenSurfaces` / `Channel` / `EventTypes` 缺失，或同一事件键上多份契约声明的 `RequiredEvent` / `RequiredFamily` 彼此矛盾。
- **MUDCB006**（Error，公众号侧）：`[MpCallbackContract]` 声明不完整。

## 用法

新增一个回调事件键的契约，只需在载荷类上声明特性，**不要手改 `RegisterAll` 方法体**（它是生成物）：

```csharp
[PayloadContract]
[WechatCallbackContract(
    WechatCallbackEventTypes.CreateUser,                 // 事件键（可多参覆盖同载荷的多个键）
    OpenSurfaces = new[] { /* 模式集合 × 通道 组合对 */ },
    RequiredFamily = WechatEventFamily.Contacts)]        // 族前置条件
public sealed class ContactUserChangedPayload
{
    public static PayloadFieldMap PayloadFieldMap { get; } = ...;   // 由上游 Mud.HttpUtils 生成
}
```

同一宿主多次声明同一事件键时由生成器合并去重；`requiredEvent` 缺省即「逐键自指」。

宿主工程侧无需额外配置——挂分析器即可：

```xml
<ProjectReference Include="..\..\Core\Mud.Wechat.Callback.Generator\Mud.Wechat.Callback.Generator.csproj"
                  OutputItemType="Analyzer" ReferenceOutputAssembly="false" PrivateAssets="all" />
```

## 依赖

- `Microsoft.CodeAnalysis.CSharp` 4.11.0（`PrivateAssets=all`，禁引 Workspaces ⇒ 满足 `RS1038`）+ `Microsoft.CodeAnalysis.Analyzers` 5.6.0
- `AdditionalFiles`：`AnalyzerReleases.Shipped.md` / `AnalyzerReleases.Unshipped.md`（诊断登记）
- 显式 `Remove` 根 `Directory.Build.props` 注入的四个包（Roslyn 组件工程不承载运行时依赖）

当前消费方：`Mud.Wechat.Work.Callback`（企微宿主）与 `Mud.Wechat.OfficialAccount.Abstractions`（公众号宿主——partial 方法体必须与宿主类型同程序集，故挂在 Abstractions 而非 Callback 包）。

## 说明

- **目标框架仅 `netstandard2.0`**（覆写根的多 TFM）：Roslyn 分析器契约要求单一低 TFM 产物，全仓多档构建复用同一 DLL。
- **`IsPackable=false` + `IncludeBuildOutput=false`**：不进「发布的 nupkg 清单」，也不被 `verify-build.ps1` 的 AOT 冒烟覆盖（其正确性由全量构建 + 守卫闭环承担）。
- 生成器**只识别硬编码 metadata name**（红线：分析器不得引用被分析程序集）。⇒ 被扫描特性或宿主类型的命名空间/改名一旦变动，规则会**静默空跑**（0 发射且无报错）。改动这些常量必须同批核对守卫 `WechatCallbackContractGuards`（CB 系列）与 `CB24` 的反射断言。
- 与 `Mud.Wechat.Callback.Analyzers` 的分工：本工程**发射**运行期登记数据；分析器工程**只诊断不发射**。两者同为 netstandard2.0 单 TFM、同为 `IsPackable=false`，但下发方式不同（本工程不随 nupkg 内嵌，分析器随宿主包内嵌）。
