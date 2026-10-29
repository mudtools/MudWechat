# 审批流程引擎（按 `thirdNo` 查询审批单当前状态）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Approval` 域 · 审批流程引擎族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkApprovalEngineService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalEngineService.cs) · [IWechatWorkApprovalEngineService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalEngineService_Internal.cs) · [IWechatWorkApprovalEngineService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalEngineService_Provider.cs) · [IWechatWorkApprovalEngineService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalEngineService_ThirdParty.cs)（目录：[Interfaces/Approval/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/)） |
| DTO 源文件 | [GetOpenApprovalDataRequest.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/RequestModel/GetOpenApprovalDataRequest.cs) · [GetOpenApprovalDataResponse.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/GetOpenApprovalDataResponse.cs) · [OpenApprovalData.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/OpenApprovalData.cs) · [OpenApprovalNode.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/OpenApprovalNode.cs) · [OpenApprovalNodeItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/OpenApprovalNodeItem.cs) · [OpenNotifyNode.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/OpenNotifyNode.cs)（全部落 `Mud.Wechat.Work.DataModels.Approval`；`RequestModel/` 仅作组织） |
| 契约守卫 | [WechatApprovalContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs)（AP1e 本族 1 条路由 / AP2 父接口恰 1 方法 + 三个空标记子接口各 0 / AP3 审批模块 16 接口令牌绑定 / AP4 域内 109 型上下文登记 / AP5 字段陷阱）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 自建 90269 / 第三方 93798 / 服务商代开发 97114（文档号取自接口 XML 注释 `<remarks>`）；**本次未重取官方页**，页面更新日期未取得，见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 2 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AP1e（`ApprovalEngineRoutes` 1 条，逐路由/逐动词 `typeof(PostAttribute)`）+ AP2（父接口 `DeclaredOnly` 恰 1 方法、`IsAbstract = true`、`RegistryGroupName` 空；三个子接口各 0 方法、挂 `Approval` 注册组、`InheritedFrom = WechatWorkApprovalEngineService`）+ AP3（含本族 4 接口在内的 16 接口逐一断言 `AccessToken` + `Query` + `access_token`）+ AP4（本族 9 型登记进 `ApprovalJsonContext`，域内顶层类型 `HaveCount(109)`）+ AP5（`thirdNo` / `OpenSpstatus` / `ApplyUsername` / `ItemUserId` 四条 `JsonPropertyName` 断言）+ 接口与 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md) §1/§4/§5 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承本域 2026-10-18 核验留档；凡只有官方页能支撑的项（频率、专属错误码、页面更新日期）一律进 §7 |

## 1. 这个接口族做什么

企业微信「审批流程引擎」把审批流程的能力嵌到自建应用 / 第三方应用里：开发者在应用内通过 JS-SDK 发起审批申请，`thirdNo` 由接入方的审批系统（开发者或第三方审批系统）自行定义并随申请带上，系统按配置好的流程自动通知审批人；每次状态变化由「审批状态变化通知回调」（`open_approval_change`）推送。本 SDK 只承载这条链路上**唯一的 HTTP 查询端点**——按 `thirdNo` 主动拉一次审批单的当前状态（E1），用于回调丢失、对账、或进入页面时的首屏兜底。

族的边界很窄：**没有**「发起申请」「批量查询」「撤销 / 转交 / 加签」端点，全部只有 E1 一支（AP1e `HaveCount(1)`）。发起与推进发生在 JS-SDK 与审批端内，SDK 侧只读。

**两套审批单据体系互不相通。** 这是本族最容易写错的前提：企业微信里同时存在两套彼此独立、键空间不重叠的审批数据，**同一套判定器、同一个主键变量都不能跨用**。

| 键 | 归属体系 | 在本族出现的位置 | 从哪来 |
|---|---|---|---|
| `thirdNo` | **审批流程引擎**体系 | E1 请求体；响应 `data.ThirdNo` | 开发者在发起申请时**自行定义**（官方不生成、不落库），即接入方审批系统一侧的单号；请求侧拼写为小驼峰 `thirdNo`，响应侧为 PascalCase `ThirdNo` |
| `OpenTemplateId` | 同上（引擎侧模板 id） | 响应 `data.OpenTemplateId` | 官方注释只写「审批模板id」，与本表下两行的关系未留档（§7） |
| `sp_no` | 「审批应用」体系 | **不出现** | 提交审批申请（`ApplyApprovalAsync`）返回或批量接口拉取 |
| `template_id` | 「审批应用」体系 | **不出现** | 审批模板编辑页链接、模板详情、审批状态回调 |

- 守卫注释把这条边界写成断言原文：「审批单据体系（`thirdNo` / `OpenTemplateId`）与『审批应用』单据体系（`sp_no` / `template_id`）相互独立」。
- 两套状态码也**不同源**：本族 `OpenSpstatus` 只有 `1/2/3/4` 四值，而「审批应用」单据级 `sp_status` 是 `1/2/3/4/6/7/10`（另有节点级 1/2/3/4/11/12/13、流程节点级 14/15）。**不得**用同一个枚举或同一个 `switch` 处理两处。

**不该用本族的场景**：要提交审批单、按时间窗批量取单号、取单据详情（`sp_no` + `template_id` 那套），走 [Approval.md](./Approval.md)；要建/改审批模板，走 [ApprovalTemplate.md](./ApprovalTemplate.md)；要查假期配置与成员余额，走 [Vacation.md](./Vacation.md)。这几族与本族的键空间不互通，混用不是「查得慢」，而是查不到或查到别的系统的数据。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 自建 `WechatAppType.Internal`；第三方 `WechatAppType.ThirdParty`；服务商代开发 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()`；运行期动态注册应用同样校验 |
| 必填配置属性（自建） | `CorpId` · `AgentSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动即失败） |
| 必填配置属性（第三方 / 代开发） | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，四项齐备 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键（自建） | `WechatTokenManagerKeys.InternalAccessToken`（值 `Wechat.AccessToken@Internal`），由 `IWechatWorkInternalApprovalEngineService` 的 `[Token]` 声明；**公共父接口不声明归属键** | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；在自建上下文里消费第三方/代开发子接口（或反之）抛 `WechatTokenOwnerMismatchException`（fail-fast） |
| 令牌归属键（第三方 / 代开发） | `WechatTokenManagerKeys.CorpAccessToken`（值 `Wechat.AccessToken@Corp`），由两个空标记子接口的 `[Token]` 声明 | 同上 |
| 令牌注入 | `TokenInjectionMode.Query`，参数名恒为 `access_token`，**不进方法签名**（AP3 对 16 个接口逐一断言） | 每次请求 |

`access_token` 由 SDK 按当前应用上下文与 scope 自动换取、缓存、刷新，**开发者不接触令牌值**。

第三方 / 代开发的令牌是**授权企业级**（一企一份，scope = `authCorpId`），调用前必须进入目标企业作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时写「应用 + 企业」两级上下文、释放时逆序还原）。未切换就直接调用时，`CorpTokenManager` 会以 `InvalidOperationException` 明确拒绝（「企业级令牌必须以 authCorpId 作为 scope 获取」），不会静默取到默认应用的令牌。`permanent_code` 是凭据，本文与示例只写参数名，不得进日志、遥测或异常消息。

### 2.2 权限与业务前置

以下逐条出自接口 XML 注释（官方页本次未重取）：

- **权限口径（三类通用）**：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。
- **实时性口径**：状态变化的实时感知应使用「审批状态变化通知回调」（`open_approval_change` 事件）；E1 是**主动查询兜底**，不是订阅替代品。
- **回调开关**：自建应用的审批状态通知需在「管理后台 - 自建应用 - 设置 API 接收」中开启「审批状态通知事件」。这是后台开关，**不是 SDK 配置属性**，SDK 不提供对应配置项。
- **发起侧不在本 SDK**：引擎的审批申请由应用内 JS-SDK 发起（`thirdNo` 随申请自定义），SDK 侧无对应 HTTP 端点。

### 2.3 官方开放的应用类型

三类应用**开放面完全一致**：唯一端点声明在公共父接口 `IWechatWorkApprovalEngineService` 上（AP1e 锁 1 条路由，AP2 锁父接口 `DeclaredOnly` 恰 1 个方法）；`_Internal` / `_Provider` / `_ThirdParty` 三个子接口均**零端点空标记**（AP2 逐个断言 `DeclaredOnly` 方法数 = 0），只为类型化注入与令牌归属而存在。因此本族不存在「某类应用多一个端点」的差异面，也没有因官方未开放而缺席的子接口。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkApprovalEngineService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、`RegistryGroupName` 恒空，不可直接给最终用户） | 1（E1） | 否 |
| `IWechatWorkInternalApprovalEngineService` | `Mud.Wechat.Work` | 企业自建应用子接口 | 0（空标记，继承 E1） | 是 |
| `IWechatWorkProviderApprovalEngineService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 0（空标记，继承 E1） | 是 |
| `IWechatWorkThirdPartyApprovalEngineService` | `Mud.Wechat.Work` | 第三方应用子接口 | 0（空标记，继承 E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Approval` / `AddApprovalApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Approval`；三个子接口的 `InheritedFrom = nameof(WechatWorkApprovalEngineService)`（继承父接口生成实现类，避免生成器重复实现公共端点——AP2 断言）
- 端点索引：**E1** `GetOpenApprovalDataAsync`（按 `thirdNo` 查审批单当前状态）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，仅作契约基座）。本族 9 个契约类型（`GetOpenApprovalDataRequest` / `GetOpenApprovalDataResponse` / `OpenApprovalData` / `OpenApprovalNodes` / `OpenApprovalNode` / `OpenApprovalNodeItems` / `OpenApprovalNodeItem` / `OpenNotifyNodes` / `OpenNotifyNode`）与审批域其余类型合计 **109 型**全部登记进 `ApprovalJsonContext`（`internal`，示例不可引用；AP4 断言域内顶层类型 `HaveCount(109)` 且 `SerializerClassName` 恒为 `Approval`）。

---

## 4. 端点

### E1. `GetOpenApprovalDataAsync` — 按 `thirdNo` 查询审批单当前状态

**签名**

```csharp
Task<GetOpenApprovalDataResponse> GetOpenApprovalDataAsync(
    [Body] GetOpenApprovalDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/corp/getopenapprovaldata` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/corp/getopenapprovaldata?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；第三方/代开发须先进入授权企业作用域，见 §2.1） |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 请求体编码 | `application/json`，源生成序列化（`ApprovalJsonContext`，AP4 锁定登记） |
| 官方文档 | 自建 90269 · 第三方 93798 · 代开发 97114（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ThirdNo` | `thirdNo` | `string?` | 是 | 开发者发起申请时自定义的审批单号（引擎体系主键，不是 `sp_no`）；官方字段名为**小驼峰 `thirdNo`**，无下划线（AP5 锁定） |
| —（不进方法签名） | `access_token` | — | 是 | 应用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`ThirdNo` 为 `null` 或空串照样发出，错误由官方 `errcode` 返回。单次只查一个单号，**无批量形态**。

```json
{ "thirdNo": "mud-THIRDNO-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `data` | `Data` | `OpenApprovalData?` | 审批单当前状态；子字段见下表 |

`data` 子字段（`OpenApprovalData`，**全部为官方原文 PascalCase**）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `data.ThirdNo` | `ThirdNo` | `string?` | 审批单编号（回显请求的 `thirdNo`，注意首字母大小写与请求侧相反） |
| `data.OpenTemplateId` | `OpenTemplateId` | `string?` | 审批模板 id |
| `data.OpenSpName` | `OpenSpName` | `string?` | 审批模板名称 |
| `data.OpenSpstatus` | `OpenSpStatus` | `int?` | 单据状态：`1` 审批中 / `2` 已通过 / `3` 已驳回 / `4` 已撤销（**仅此四值**；JSON 键的小写 `s` 是官方原文，见 §5） |
| `data.ApplyTime` | `ApplyTime` | `long?` | 提交申请时间（Unix 时间戳，**响应侧为数字**） |
| `data.ApplyUsername` | `ApplyUsername` | `string?` | 提交者姓名（官方即 `ApplyUsername`，AP5 锁定） |
| `data.ApplyUserParty` | `ApplyUserParty` | `string?` | 提交者所在部门（注释只写「所在部门」，未区分名称与 id，§7） |
| `data.ApplyUserImage` | `ApplyUserImage` | `string?` | 提交者头像（注释未写形态，§7） |
| `data.ApplyUserId` | `ApplyUserId` | `string?` | 提交者 userid |
| `data.ApprovalNodes` | `ApprovalNodes` | `OpenApprovalNodes?` | 审批流程信息；**对象包裹数组**：数组键是 `ApprovalNode` |
| `data.NotifyNodes` | `NotifyNodes` | `OpenNotifyNodes?` | 抄送信息；**对象包裹数组**：数组键是 `NotifyNode`，可能有多个抄送人 |
| `data.ApproverStep` | `ApproverStep` | `int?` | 当前审批节点：`0` 第一个审批节点、`1` 第二个……（**0 基序号**，不是「第几步」的 1 基计法） |

`ApprovalNodes` → `ApprovalNode[]`（`OpenApprovalNode`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `ApprovalNodes.ApprovalNode[].NodeStatus` | `NodeStatus` | `int?` | 节点审批操作状态：`1` 审批中 / `2` 已同意 / `3` 已驳回 / `4` 已转审 |
| `ApprovalNodes.ApprovalNode[].NodeAttr` | `NodeAttr` | `int?` | 节点审批方式：`1` 或签 / `2` 会签 |
| `ApprovalNodes.ApprovalNode[].NodeType` | `NodeType` | `int?` | 节点类型：`1` 固定成员 / `2` 标签 / `3` 上级（**与「审批应用」单据侧的 `node_type` 语义不同**，见 §5） |
| `ApprovalNodes.ApprovalNode[].Items` | `Items` | `OpenApprovalNodeItems?` | 节点分支信息，**对象包裹数组**：数组键是 `Item`；节点为标签或上级时一个节点可能有多个分支 |

`Items` → `Item[]`（`OpenApprovalNodeItem`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `Items.Item[].ItemName` | `ItemName` | `string?` | 分支审批人姓名 |
| `Items.Item[].ItemParty` | `ItemParty` | `string?` | 分支审批人所在部门（同 `ApplyUserParty` 的口径缺口，§7） |
| `Items.Item[].ItemImage` | `ItemImage` | `string?` | 分支审批人头像（形态未在注释里写明） |
| `Items.Item[].ItemUserId` | `ItemUserId` | `string?` | 分支审批人 userid（官方即 `ItemUserId`，AP5 锁定） |
| `Items.Item[].ItemStatus` | `ItemStatus` | `int?` | 分支审批人操作状态：`1` 审批中 / `2` 已同意 / `3` 已驳回 / `4` 已转审 |
| `Items.Item[].ItemSpeech` | `ItemSpeech` | `string?` | 分支审批人审批意见 |
| `Items.Item[].ItemOpTime` | `ItemOpTime` | `long?` | 分支审批人操作时间（Unix 时间戳） |

`NotifyNodes` → `NotifyNode[]`（`OpenNotifyNode`，四项，无状态字段）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `NotifyNodes.NotifyNode[].ItemName` | `ItemName` | `string?` | 抄送人姓名 |
| `NotifyNodes.NotifyNode[].ItemParty` | `ItemParty` | `string?` | 抄送人所在部门 |
| `NotifyNodes.NotifyNode[].ItemImage` | `ItemImage` | `string?` | 抄送人头像 |
| `NotifyNodes.NotifyNode[].ItemUserId` | `ItemUserId` | `string?` | 抄送人 userid |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "data": {
    "ThirdNo": "mud-THIRDNO-FAKE-0001",
    "OpenTemplateId": "mud-OPENTEMPLATEID-FAKE-0001",
    "OpenSpName": "（占位模板名）",
    "OpenSpstatus": 1,
    "ApplyTime": 1569546000,
    "ApplyUsername": "（占位姓名）",
    "ApplyUserParty": "（占位部门）",
    "ApplyUserImage": "https://example.invalid/avatar-1.png",
    "ApplyUserId": "mud-USERID-FAKE-0001",
    "ApprovalNodes": {
      "ApprovalNode": [
        {
          "NodeStatus": 1,
          "NodeAttr": 1,
          "NodeType": 1,
          "Items": {
            "Item": [
              {
                "ItemName": "（占位姓名）",
                "ItemParty": "（占位部门）",
                "ItemImage": "https://example.invalid/avatar-2.png",
                "ItemUserId": "mud-USERID-FAKE-0002",
                "ItemStatus": 1,
                "ItemSpeech": "",
                "ItemOpTime": 1569547000
              }
            ]
          }
        }
      ]
    },
    "NotifyNodes": {
      "NotifyNode": [
        {
          "ItemName": "（占位姓名）",
          "ItemParty": "（占位部门）",
          "ItemImage": "https://example.invalid/avatar-3.png",
          "ItemUserId": "mud-USERID-FAKE-0003"
        }
      ]
    },
    "ApproverStep": 0
  }
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                          // IWechatWorkInternalApprovalEngineService / AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;             // AddWechatApp 扩展方法落此命名空间（极易漏）
using Mud.Wechat.Work.Abstractions.Enums;       // WechatAppType
using Mud.Wechat.Work.DataModels.Approval;      // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，Microsoft.Extensions.DependencyInjection（GetRequiredService）与
// Microsoft.Extensions.Logging（logger 扩展方法）由 Web SDK 隐式 using 提供；
// builder / config / provider / logger / cancellationToken 由宿主提供。

// ① 注册自建应用：凭据从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"]!;             // 配置项缺失时启动期 Validate() 会抛，不在这里判空
    o.AgentSecret = config["Wechat:AgentSecret"]!;   // 凭据：只从配置读取，不得进日志
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配）。
builder.Services.AddWechatWorkServices(b => b.AddApprovalApi());

// ③ 解析子接口：本族没有 _Internal 专属端点，三个子接口能力等价，只为令牌归属分面。
var engine = provider.GetRequiredService<IWechatWorkInternalApprovalEngineService>();

// ④ 业务输入段内声明：thirdNo 是发起申请时自定义的单号，不是 sp_no。
var thirdNo = "mud-THIRDNO-FAKE-0001";

var response = await engine.GetOpenApprovalDataAsync(
    new GetOpenApprovalDataRequest { ThirdNo = thirdNo },
    cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!response.IsSuccess)
{
    // 审批单含申请人/审批人姓名、部门、头像、意见：日志只写 errcode/errmsg 与 thirdNo。
    logger.LogError("查询审批单状态失败 {ErrorCode}: {ErrorMessage}，thirdNo={ThirdNo}",
        response.ErrorCode, response.ErrorMessage, thirdNo);
    throw new InvalidOperationException($"查询审批单状态失败：{response.ErrorCode}");
}

var data = response.Data
    ?? throw new InvalidOperationException($"查询审批单状态返回空 data：{thirdNo}");

// ⑥ OpenSpstatus 只有 1/2/3/4 四值，窄于「审批应用」单据侧的 sp_status，不要复用同一张常量表。
var statusText = data.OpenSpStatus switch
{
    1 => "审批中",
    2 => "已通过",
    3 => "已驳回",
    4 => "已撤销",
    _ => "未知",
};

// ⑦ 两层「对象包裹数组」：先取 ApprovalNodes.ApprovalNode，再取 node.Items.Item。
var approvalNodes = data.ApprovalNodes?.ApprovalNode;
if (approvalNodes is not null)
{
    foreach (var node in approvalNodes)
    {
        var branchItems = node.Items?.Item;
        if (branchItems is null)
        {
            continue;
        }

        foreach (var item in branchItems)
        {
            // 只记状态码：ItemName / ItemParty / ItemImage / ItemUserId / ItemSpeech 属成员数据。
            logger.LogInformation("节点类型 {NodeType} 分支状态 {ItemStatus}", node.NodeType, item.ItemStatus);
        }
    }
}

logger.LogInformation("thirdNo {ThirdNo} 当前状态 {OpenSpstatus}（{StatusText}），当前审批节点序号 {ApproverStep}",
    data.ThirdNo, data.OpenSpStatus, statusText, data.ApproverStep);
```

**第三方 / 代开发的调用片段**（企业作用域是前置动作，不是可选项）

```csharp
using Mud.Wechat.Work;                              // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions.Authentication;  // IWechatAppContextSwitcher（UseCorpScope 落此命名空间）
using Mud.Wechat.Work.DataModels.Approval;          // 请求 DTO

// 前置：AddWechatApp(AppType = ThirdParty 或 Provider) + AddWechatWorkServices(b => b.AddApprovalApi()) 已装配；
// DI 的 using 与上方示例相同。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var appKey = "thirdparty-a";
var authCorpId = "mud-AUTHCORPID-FAKE-0001";                       // 目标授权企业
var permanentCode = config["Wechat:AuthCorpPermanentCode"];        // 凭据：只从配置读，不得进日志

using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))
{
    var engine = provider.GetRequiredService<IWechatWorkThirdPartyApprovalEngineService>();

    var response = await engine.GetOpenApprovalDataAsync(
        new GetOpenApprovalDataRequest { ThirdNo = "mud-THIRDNO-FAKE-0002" },
        cancellationToken);

    // 不切作用域时 CorpTokenManager 会抛 InvalidOperationException，而不是静默取默认应用令牌。
    if (!response.IsSuccess)
    {
        throw new InvalidOperationException($"查询审批单状态失败：{response.ErrorCode}");
    }
}
```

**本端点注意事项**

- 纯查询语义，官方仍是 **POST**（AP1e 逐条断言 `typeof(PostAttribute)`）；按「查询用 GET」改写动词即打红守卫，也会被官方拒。
- `thirdNo` 官方必填、SDK 不校验；空值原样发出，错误以 `errcode` 回来（具体码未留档，§7）。
- 单号是**单值入参**：没有批量查询端点，也没有分页；要对账一批单号只能宿主自己循环，并自行承担频率（本端点频率上限未在注释里留档，§7）。
- 响应两级都是**对象包裹数组**（`ApprovalNodes.ApprovalNode` / `Items.Item` / `NotifyNodes.NotifyNode`）：只判外层对象非空就遍历会拿到空序列，见 §5。
- `data` 为 `null` 的成功应答形态未留档，示例按「无 data 即异常」处理，不要把它当成「审批中」。
- 响应含提交者与审批人的姓名、部门、头像、意见：**不得整体序列化进日志、遥测或异常消息**；需要留痕时只写 `thirdNo`、`OpenSpstatus`、`ApproverStep`。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `thirdNo`（本族）vs `sp_no` / `template_id`（审批应用） | 两套单据体系相互独立，键空间不重叠（守卫注释原文） | 拿 `sp_no` 当 `thirdNo` 传：官方查不到该单，表现为非 0 errcode 或空 data；反向则「审批应用」里永远找不到引擎单 |
| 请求 `thirdNo` vs 响应 `ThirdNo` | 请求侧官方原文是**小驼峰** `thirdNo`（AP5 锁定），响应侧是 PascalCase `ThirdNo`；同一语义、两种拼写 | 按「响应键去掉首字母大写」或「请求键加首字母大写」互推，抓包对不上、字典取值恒 `null` |
| `OpenSpstatus`（JSON）vs `OpenSpStatus`（C# 属性） | JSON 键里是小写 `s`（官方原文），SDK 属性名是 `OpenSpStatus`；AP5 锁 JSON 名 | 自己写 DTO / 用 `JsonDocument` 按 `OpenSpStatus` 取键恒 `null`；改 SDK 的 `[JsonPropertyName]` 会打红 AP5 |
| `ApplyUsername` / `ItemUserId` | 官方原文 PascalCase，不是 `apply_username` / `item_userid` | 按蛇形键取值恒 `null`，姓名与 userid 静默丢失 |
| `ApprovalNodes` / `NotifyNodes` / `Items` 的包裹层 | 三个都是「对象包裹数组」：真实数组在 `ApprovalNode` / `NotifyNode` / `Item` 键下 | 把外层对象当数组遍历（编译不过）；或只判外层对象非空就继续，内层数组仍可能为 `null`——误报「无审批节点」，示例按两层判空 |
| 本族 `NodeType`（`1` 固定成员 / `2` 标签 / `3` 上级）vs 单据侧 `node_type`（`1` 审批人 / `2` 抄送人 / `3` 办理人） | C# 属性同名 `NodeType`、JSON 键大小写不同，**语义完全不同**：这里是「节点成员来源」，那边是「节点角色」 | 共用一张常量表会把「标签节点」判成「抄送人」，审批进度展示整体错位 |
| 本族 `OpenSpstatus`（1/2/3/4）vs 单据级 `sp_status`（1/2/3/4/6/7/10） | 本族状态域**更窄**：没有 6/7/10，也没有节点级 11~15；注释只给这四个值的语义 | 两套状态机放进同一张枚举 / 同一个判定器：单据侧的 6/7/10 在本族永不可能出现，`default` 分支到底算「未知」还是「终态」无法自洽，终态判断会静默错 |
| `ApproverStep` 的基数 | `0` 表示第一个审批节点，`1` 表示第二个——**0 基** | 按 1 基解释会整体偏移一位，把「第一节点审批中」显示成「第二节点」 |
| 「查询用 GET」 | 本族唯一端点官方即 POST | 改动词打红 AP1e，且官方不接受 |
| `ApplyTime` / `ItemOpTime` | 响应侧是 `long?`（Unix 秒），与「审批应用」的字符串时间戳（`starttime`）形态相反 | 按字符串解析会抛格式异常；跨族复制时间解析器会踩两套口径 |
| 令牌归属 | 自建 `@Internal`、第三方/代开发 `@Corp`；父接口不声明归属键 | 在自建上下文消费 `_ThirdParty` 子接口抛 `WechatTokenOwnerMismatchException`；第三方不切企业作用域调 `_Internal` 同样 fail-fast |
| 个人信息 | `ApplyUsername` / `ApplyUserParty` / `ApplyUserImage` / `ApplyUserId` / `ItemName` / `ItemParty` / `ItemImage` / `ItemUserId` / `ItemSpeech` | 整体写进日志/遥测/异常消息即泄露；SDK 不代脱敏，`WechatWorkException.RequestUri` 只处理 query 不含响应体 |

另外三条 SDK 侧行为约束，写在这里是因为它们会改变代码写法：

- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。判 `IsSuccess` 或显式调 `WechatWorkException.ThrowIfFailed(response, "/cgi-bin/corp/getopenapprovaldata")`。
- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改 Header）。`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但审批报文里的个人信息需调用方自己避免打印。
- **本族只读、状态变化不在此处订阅**：SDK 没有发起 / 撤销 / 转交 / 加签端点；实时感知走 `open_approval_change` 回调（`Mud.Wechat.Work.Callback` 包，事件常量 `WechatCallbackEventTypes.OpenApprovalChange`），E1 只作主动查询与兜底。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径（**不代表 `data` 一定非空**，仍需判空） |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效 / 无效 | 族别失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效 | 同上 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少 `access_token` 参数 / 凭证缺失（含永久授权码无效等场景） | 不自愈，属装配/配置缺陷信号 |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号（多为误调到服务商级接口） |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍为 200。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」。若这些码稳定出现，说明刷新本身在失败（自建：`AgentSecret` 失效或应用被停用；第三方/代开发：`permanent_code` 失效、授权已取消、或未切换企业作用域），加重试不解决。

本端点专属的业务错误码（无权限、`thirdNo` 不存在、频率超限等）接口注释未留档 ⇒ 见 §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（AP1e/AP2/AP3/AP4/AP5 + 接口与 DTO XML 注释 + 域 README 2026-10-18）；文档号照抄注释，**页面最后更新日期与正文文案未逐项核验** | 逐页渲染取文：自建 90269 / 第三方 93798 / 代开发 97114 |
| 本端点的频率上限 | 接口注释与 DTO 注释均未写频率数值（同域「审批应用」族普遍写 600 次/分，本族无此句）⇒ **未逐项核验** | 官方 90269 / 93798 / 97114 与附录 90454 |
| 端点专属错误码 | 注释未留档任何 `errcode` 表（无权限、`thirdNo` 不存在、参数非法、限流返回码）⇒ **未逐项核验** | 官方各页 + 全局错误码 90455；不在 SDK 侧硬编码码表 |
| `thirdNo` 的取值约束 | 注释只写「开发者自定义单号」，**长度 / 字符集 / 唯一性作用域未留档** ⇒ **未逐项核验** | 官方 90269 / 93798 / 97114 |
| `OpenTemplateId` 的取值空间 | 注释只写「审批模板id」；与「审批应用」的 `template_id`、服务商后台 `open_template_id` 是否同一空间，源码未留档 ⇒ **未逐项核验**，本文不假设可互换 | 官方 90269；同域 README §1 的键归属表 |
| `ApplyUserParty` / `ItemParty` 是名称还是 id | 注释只写「所在部门」；同域「审批应用」族的 `partyid` 是字符串部门 id、`apply_org` 注释为「申请人部门」，两族口径是否一致不明 ⇒ **未逐项核验** | 官方 90269 响应示例；按部门做匹配前先实测 |
| `ApplyUserImage` / `ItemImage` 的形态 | 注释只写「头像」，未说明是 URL、media_id 还是可空串 ⇒ **未逐项核验** | 官方 90269 |
| 「无审批节点 / 无抄送人」时的报文形态 | 注释未说明字段是缺失、`null` 还是空数组；示例按两层判空处理 ⇒ **未逐项核验** | 官方 90269 响应示例 + 实测 |
| `data` 是否恒存在 | `errcode == 0` 时 `data` 缺失的形态未留档 ⇒ 本文按「缺失即异常」处理，**未逐项核验** | 官方 90269；或实测 |
| `ApproverStep` 在终态（2/3/4）与无流程场景的取值 | 注释只解释 0/1/… 的节点序号语义，未说明单据走完或撤销后停在哪一位 ⇒ **未逐项核验** | 官方 90269；不要把 `ApproverStep + 1` 当「共几步」 |
| 三类应用页面的字段一致性 | 本文按「三类开放面一致」写（接口注释与 AP1e/AP2 支撑）；三个官方页是否逐字段一致**未逐项核验** | 分别取 90269 / 93798 / 97114 比对 |
| `open_approval_change` 载荷的具名 | 域 README §5 该行只写「同为回调侧载荷」未给文件；本文据源码定位为 [ApprovalStatusChangedPayload.cs](../../../../../Src/Work/Mud.Wechat.Work.Callback/Events/Payloads/Messages/ApprovalStatusChangedPayload.cs)，其字段映射属回调域、不在本篇 | 回调侧守卫与载荷契约；README 该行待同批补名 |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 12 支端点速览 + 两套单据体系与三套状态码的权威速览）：[Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md)
- 兄弟族：[Approval.md](./Approval.md)（提交 / 批量取单号 / 详情 / 旧版数据，`sp_no` + `template_id` 体系）· [ApprovalTemplate.md](./ApprovalTemplate.md)（模板详情 / 创建 / 更新 / 复制）· [Vacation.md](./Vacation.md)（假期配置与成员余额，单位为秒）
- 审批流程引擎状态回调（不在本族）：[ApprovalStatusChangedPayload.cs](../../../../../Src/Work/Mud.Wechat.Work.Callback/Events/Payloads/Messages/ApprovalStatusChangedPayload.cs)（`open_approval_change`，官方 90240）
- 契约守卫源码：[WechatApprovalContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与域 README §9
- 官方页（文档号照接口注释留档，本次未重取正文）：[90269（自建）](https://developer.work.weixin.qq.com/document/path/90269) · [93798（第三方）](https://developer.work.weixin.qq.com/document/path/93798) · [97114（服务商代开发）](https://developer.work.weixin.qq.com/document/path/97114)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
