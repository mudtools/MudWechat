# 高级功能（成员申请单）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AdvancedFeature` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/AdvancedFeature/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedFeature/)（[父接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedFeature/IWechatWorkAdvancedFeatureService.cs) · [自建子接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedFeature/IWechatWorkAdvancedFeatureService_Internal.cs)） |
| DTO 源文件 | [AdvancedFeatureApprovalDtos.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AdvancedFeature/AdvancedFeatureApprovalDtos.cs)（8 个顶层类型）· 源生成上下文 [AdvancedFeatureJsonContext.g.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Generated/AdvancedFeatureJsonContext.g.cs) |
| 契约守卫 | [WechatAdvancedFeatureContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAdvancedFeatureContractGuards.cs)（AF1 路由与动词 / AF2 仅自建层级 / AF3 令牌与归属域 / AF4 8 DTO 登记与字段陷阱）+ [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1 归属域声明 / TO3 归属键矩阵） |
| 官方文档 | 99880（设置审批信息）· 99883（批量获取申请单 id）· 99885（获取申请单详情），三页均只标自建应用 · 页面最后更新 **本次未重取**，见「证据基础」与 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 3 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AF1 路由/动词/响应形态与 `process_list.node_list` 包装形态 / AF2 层级与 3 端点计数 / AF3 令牌与归属域键 / AF4 8 个 DTO 的上下文登记、缩写字段名、布尔 `has_more`、两套 `approval_status` 类型 + TO1/TO3 归属域规则 + 两份接口与一份 DTO 文件的 XML 注释逐字段核验 2026-10-29 · 官方：**本次未重取官方页**，官方面事实（三页应用类型表文案、必填项、节点值域、分页默认与上限）全部经接口/DTO XML 注释转述，凡只有官方页正文能支撑的项（频率数值、专属错误码、页面更新日期）一律进 §7 |

## 1. 这个接口族做什么

企业微信的「高级账号」（邮件 / 文档 / 微盘 / 会议）不是默认能力：企业成员要在客户端提交申请单，由管理员审批后才开通。本族让**企业自建应用**把这套申请单的审批流程**托管到自己的审批系统**——E1 把审批状态、审批系统里的跳转链接与审批节点列表写回企微；E2 按成员与应用类型批量拉取该成员的申请单 id；E3 按申请单 id 取详情（申请人、类型、状态、审批 id、申请原因）。

使用时机有两类：① 企业已有自建审批流（OA、工单系统），希望成员在企微提交申请单后由自家系统审批、再把结果回写企微；② 做申请单对账或报表——先用 E2 枚举 id、再用 E3 逐单取详情。

调用凭证是**企业自建应用的 `access_token`**，归属域被钉在 `Wechat.AccessToken@Internal`，第三方应用与代开发应用在本族**没有任何子接口**（官方三页逐页标注「暂不支持」）。

**不该用本族的场景**：你要处理的是企业自有审批单据（请假、报销、合同用印等 OA 审批），那是 [Approval](../Approval/Approval.md) 域（`/cgi-bin/oa/*`，三类应用有不同开放面）；你要购买或查询高级接口的订单与代付，那是 [AdvancedApi](../AdvancedApi/AdvancedApi.md) 域（`/cgi-bin/advanced_api/*`，走 `provider_access_token`）——名字里都带「高级」，凭证面与章节树都不同，见 §5。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（本族唯一开放面） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用同样校验 |
| 必填配置属性 | `CorpId`（本企业 CorpId）· `AgentSecret`（即 `corpsecret`） | 同上；缺任一项抛 `InvalidOperationException`（`自建应用 {AppKey} 缺少 CorpId。` / `自建应用 {AppKey} 缺少 AgentSecret。`），不留到首次调用 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键 | `WechatTokenManagerKeys.InternalAccessToken`（常量值 `Wechat.AccessToken@Internal`），**只由子接口** `[Token]` 声明；父接口不声明 | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；第三方/代开发应用上下文命中该键抛 `WechatTokenOwnerMismatchException`（AF3 + TO3 锁定） |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入，**不进方法签名** | 每次请求 |

`access_token` 由 SDK 侧自建应用令牌管理器以 `CorpId + AgentSecret` 自动换取、缓存并刷新，**开发者不接触令牌值**；失效自愈链路见 §6。

### 2.2 权限与业务前置

- **后台配置前置**（父接口 remarks 引官方应用类型表原文）：自建应用须被**配置到「高级功能 - 成员申请管理 - 通过 api 设置申请流程」中**——这是官方给自建应用列的准入条件。它是管理员在企微管理后台完成的配置，不是 SDK 配置项。
- **申请单的来源是成员本人**：E2 的 `userid` 是该成员的身份标识，申请单由成员在企微客户端发起（父接口 `<summary>`：「企业成员向管理员申请『高级账号』」）。SDK 不代成员发起申请，只读写审批侧信息。
- **E1 的链接与节点受官方约束**：`approval_url` 须以 `http://` 或 `https://` 开头；节点下的待处理人、已处理人各最多 100 个且**不可同时为空**（接口与 DTO 注释）。
- **个人信息**：`userid` / `applicant` / `current_approvers` / `completed_approvers` 是成员身份，`request_reason` 是成员填写的自由文本理由 —— 一律不得写入日志、遥测或异常消息（接口与 DTO remarks）。示例里也只取计数或状态，不打印这些值。

成员是否还需落在应用的通讯录可见范围内，接口注释未留档 ⇒ 见 §7，不要按其他域的习惯推断。

### 2.3 官方开放的应用类型

三页的应用类型表逐页一致（父接口 remarks 引官方原文）：「自建应用：配置到『高级功能 - 成员申请管理 - 通过 api 设置申请流程』中；代开发应用：暂不支持；第三方应用：暂不支持」。落到代码形态：

- 公共父接口 `IWechatWorkAdvancedFeatureService` 承载全部 3 个端点，`IsAbstract = true`、不注册 DI；
- 继承链上**只有一个**子接口 `IWechatWorkInternalAdvancedFeatureService`，且它**零端点**（纯标记 + 归属域声明），AF2 用「继承者集合等价于单元素」把第三方/代开发子接口钉死为零；
- ⇒ 第三方与代开发应用**没有通向本族的 DI 入口，也没有凭据路径**（`Wechat.AccessToken@Internal` 只由自建应用装配）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAdvancedFeatureService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`），承载全部端点 | 3（E1/E2/E3） | 否 |
| `IWechatWorkInternalAdvancedFeatureService` | `Mud.Wechat.Work` | 企业自建应用子接口（标记子接口，零端点） | 0 | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AdvancedFeature` / `AddAdvancedFeatureApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AdvancedFeature`（子接口 `RegistryGroupName`，父接口恒为空；子接口 `InheritedFrom = nameof(WechatWorkAdvancedFeatureService)`）
- 端点索引：**E1** `SetApprovalDetailAsync`（写入审批状态、链接与全量节点）· **E2** `GetApplyIdListAsync`（按成员 + 类型翻页拉申请单 id）· **E3** `GetApprovalInfoAsync`（按 `apply_id` 取详情）

最终用户注入**自建子接口**；父接口零 DI 注册，解析它是装配错误。

---

## 4. 端点

### E1. `SetApprovalDetailAsync` — 设置申请单审批信息（覆盖式全量写入）

**签名**

```csharp
Task<WechatWorkResponse> SetApprovalDetailAsync(
    [Body] SetAdvancedFeatureApprovalDetailRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_feature/set_approval_detail` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/advanced_feature/set_approval_detail?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建应用凭证） |
| 适用应用类型 | 仅企业自建应用 |
| 请求体编码 | `application/json`，源生成序列化（`AdvancedFeatureJsonContext`，AF4 已登记 8 个契约类型） |
| 官方文档 | 99880 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ApplyId` | `apply_id` | `string?` | 是 | 申请单 id，来自 E2 的 `apply_id_list`（DTO 注释） |
| `request.ApprovalId` | `approval_id` | `string?` | 是 | 应用侧生成的审批 id。生成后与 `apply_id` **一一对应、不可改变**（DTO 注释原文） |
| `request.ApprovalStatus` | `approval_status` | `int?` | 是 | 审批单状态：1 审批中 / 2 已驳回 / 3 已同意 / 101 已撤销（本字段 4 个值；`102 未到流程` 只属节点字段，见下表与 §5） |
| `request.ApprovalUrl` | `approval_url` | `string?` | 是 | 审批单跳转链接，须以 `http://` 或 `https://` 开头（DTO 注释） |
| `request.ProcessList` | `process_list` | `AdvancedFeatureApprovalProcessList?` | 是 | 节点容器；官方必填的是容器内的 `process_list.node_list`（AF1 锁定该包装形态） |
| `request.ProcessList.NodeList` | `node_list` | `IList<AdvancedFeatureApprovalNode>?` | 是 | 审批节点列表。变更节点信息时须传**全量**节点（官方原文明示，DTO remarks） |
| —（不进方法签名） | `access_token` | — | 是 | 自建应用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

`node_list` 的每个元素（`AdvancedFeatureApprovalNode`）：

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `node.NodeApprovalStatus` | `node_apv_status` | `int?` | 是 | 节点状态：1 审批中 / 2 已驳回 / 3 已同意 / 101 已撤销 / 102 未到流程（**缩写照抄官方**，勿写成 `node_approval_status`） |
| `node.NodeApprovalRelation` | `node_apv_rel` | `int?` | 是 | 多人审批方式：1 会签 / 2 或签 / 3 依次审批（字段名同样是缩写） |
| `node.CurrentApprovers` | `current_approvers` | `IList<string>?` | 否\* | 当前节点待处理人，**最多 100 个**；与已处理人**不可同时为空**（DTO remarks） |
| `node.CompletedApprovers` | `completed_approvers` | `IList<string>?` | 否\* | 当前节点已处理人，**最多 100 个**；同上 |
| `node.ApprovalUpdateTime` | `apv_update_time` | `long?` | 否 | 节点更新时间。字段名是 `apv_` 不是 `approval_`；单位本文未取证（§7） |

\* 官方必填性表述是「两个列表不可同时为空」，不是「各自必填」：至少一侧要有值。SDK 不做参数校验，空集合照样发出，错误以 `errcode` 返回。

```json
{
  "apply_id": "APPLY_ID_FAKE_0001",
  "approval_id": "APPROVAL_ID_FAKE_0001",
  "approval_status": 1,
  "approval_url": "https://approval.example.com/detail/APPROVAL_ID_FAKE_0001",
  "process_list": {
    "node_list": [
      {
        "node_apv_status": 1,
        "node_apv_rel": 1,
        "current_approvers": ["mud_approver_a", "mud_approver_b"],
        "completed_approvers": [],
        "apv_update_time": 1761696000
      },
      {
        "node_apv_status": 102,
        "node_apv_rel": 3,
        "current_approvers": ["mud_approver_c"],
        "completed_approvers": [],
        "apv_update_time": 1761696000
      }
    ]
  }
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

本端点**没有业务负载**：官方响应只有 `errcode` / `errmsg`，故 SDK 直接返回 `WechatWorkResponse`，不为它新建空响应 DTO（接口 `<returns>` 与 AF1 的响应形态断言）。

```json
{ "errcode": 0, "errmsg": "ok" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;            // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException（判错出口）
using Mud.Wechat.Work.DataModels;              // WechatWorkResponse
using Mud.Wechat.Work.DataModels.AdvancedFeature; // 请求 DTO

// 宿主为 ASP.NET Core（Web SDK）+ ImplicitUsings 时，System.Collections.Generic /
// Microsoft.Extensions.DependencyInjection（GetRequiredService）/ Microsoft.Extensions.Logging
// 由宿主隐式提供；换控制台等非 Web 宿主须自行补齐 using。

// ① 注册自建应用：CorpId + AgentSecret 两项，Validate() 在启动期校验，缺项即抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:CorpSecret"];   // 即 corpsecret；一律从配置/环境变量读取
});

// ② 注册本域模块（令牌底座随 AddWechatApp 装配）。
builder.Services.AddWechatWorkServices(b => b.AddAdvancedFeatureApi());

// ③ 解析自建子接口。父接口零端点且不注册 DI，解析它会失败。
IWechatWorkInternalAdvancedFeatureService advancedFeature =
    provider.GetRequiredService<IWechatWorkInternalAdvancedFeatureService>();

string applyId = "APPLY_ID_FAKE_0001";              // 假值：E2 的 apply_id_list 元素
string approvalId = "APPROVAL_ID_FAKE_0001";        // 本系统生成的审批 id，与 applyId 一一对应后不可改

// ④ 全量节点：这里两个节点只是示例；真实变更必须把该申请单的全部节点都列出来。
var processList = new AdvancedFeatureApprovalProcessList
{
    NodeList = new List<AdvancedFeatureApprovalNode>
    {
        new AdvancedFeatureApprovalNode
        {
            NodeApprovalStatus = 1,          // 1 审批中
            NodeApprovalRelation = 1,        // 1 会签
            CurrentApprovers = new List<string> { "mud_approver_a", "mud_approver_b" },
            CompletedApprovers = new List<string>(),
            ApprovalUpdateTime = 1761696000L,
        },
        new AdvancedFeatureApprovalNode
        {
            NodeApprovalStatus = 102,        // 102 未到流程
            NodeApprovalRelation = 3,        // 3 依次审批
            CurrentApprovers = new List<string> { "mud_approver_c" },
            CompletedApprovers = new List<string>(),
            ApprovalUpdateTime = 1761696000L,
        },
    },
};

WechatWorkResponse setResponse = await advancedFeature.SetApprovalDetailAsync(
    new SetAdvancedFeatureApprovalDetailRequest
    {
        ApplyId = applyId,
        ApprovalId = approvalId,
        ApprovalStatus = 1,                  // 审批单级状态：1/2/3/101，不含 102
        ApprovalUrl = "https://approval.example.com/detail/APPROVAL_ID_FAKE_0001",
        ProcessList = processList,
    },
    cancellationToken);

// ⑤ 判错：官方 errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
WechatWorkException.ThrowIfFailed(setResponse, "/cgi-bin/advanced_feature/set_approval_detail");

// ⑥ 无业务负载可读；只记单号与节点数，不记审批人 userid（成员身份）。
logger.LogInformation("申请单 {ApplyId} 审批信息已写入，全量节点 {NodeCount} 个",
    applyId, processList.NodeList?.Count ?? 0);
```

**本端点注意事项**

- 这是**覆盖式全量写入**：官方要求「变更审批节点信息需要全量节点都传入」。只把本次变动的节点发过去，等价于**删除其余节点**，而 `errcode` 仍为 0 —— 静默丢节点，无异常、无回滚。调用方须先在本地持有完整节点快照，再做「改一个、发全部」。
- `approval_id` 一旦确定就与 `apply_id` 绑定、**不可改变**（DTO 注释）。每次同步都重新生成一个 `approval_id`，等于每轮都在拆掉上一轮建立的对应关系。
- `approval_url` 不是可选项：官方必填且必须以 `http://` / `https://` 开头；传相对路径或 `ftp://` 不会被 SDK 拦下，由官方报错。
- 本端点无业务负载，**成功判定只有 `errcode`**；不要指望响应里回显写入后的节点——响应只有 `errcode` / `errmsg`，E3 的详情也不含节点列表。

### E2. `GetApplyIdListAsync` — 批量获取某成员的高级功能申请单 id

**签名**

```csharp
Task<GetAdvancedFeatureApplyIdListResponse> GetApplyIdListAsync(
    [Body] GetAdvancedFeatureApplyIdListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_feature/get_apply_id_list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/advanced_feature/get_apply_id_list?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建应用凭证） |
| 适用应用类型 | 仅企业自建应用 |
| 请求体编码 | `application/json`（`GetAdvancedFeatureApplyIdListRequest` / `Response` 已登记 `AdvancedFeatureJsonContext`，AF4） |
| 官方文档 | 99883 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.BusinessType` | `business_type` | `int?` | 是 | 申请的高级账号类型：1 邮件 / 2 文档 / 3 微盘 / 4 会议（DTO 注释） |
| `request.UserId` | `userid` | `string?` | 是 | 申请成员 userid（成员身份，不得落日志） |
| `request.Limit` | `limit` | `int?` | 否 | 分页上限，**默认 100、最大 200**；官方**不保证**每次返回条数刚好等于 `limit`（DTO 注释原文） |
| `request.Cursor` | `cursor` | `string?` | 否 | 分页游标，首次可不填；后续回填**上一页响应的 `next_cursor`**。注意请求键是 `cursor`、响应键是 `next_cursor`，**两个不同的字段名** |
| `request.RequestType` | `req_type` | `int?` | 否 | 申请单来源过滤，默认 0：0 所有 / 1 仅 api 单 / 2 非 api 申请单（字段名是 `req_type` 不是 `request_type`） |
| —（不进方法签名） | `access_token` | — | 是 | Query 注入 |

SDK 不做参数校验：`limit` 传 0、负数或超过 200 都照原样发出，行为以官方返回为准（超限行为本文未取证，见 §7）。

```json
{
  "business_type": 2,
  "userid": "mud_zhangsan",
  "limit": 200,
  "req_type": 0
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标；回填到下一次请求的 `cursor`（两者 JSON 名不同） |
| `apply_id_list` | `ApplyIdList` | `IList<string>?` | 申请单 id 列表，元素供 E3 使用；本端点**不返回**申请单内容 |
| `has_more` | `HasMore` | `bool?` | 是否还有下一页。**本域是布尔**（官方样本为 `true`）；「高级接口订单」域的同名字段是整型 0/1，两域不归一（AF4） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "next_cursor": "CURSOR_FAKE_2",
  "apply_id_list": ["APPLY_ID_FAKE_0001", "APPLY_ID_FAKE_0002"],
  "has_more": true
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.AdvancedFeature;

// 前置同 E1：应用与模块已由宿主完成 AddWechatApp + AddAdvancedFeatureApi 注册。
IWechatWorkInternalAdvancedFeatureService advancedFeature =
    provider.GetRequiredService<IWechatWorkInternalAdvancedFeatureService>();

string applyUserId = "mud_zhangsan";   // 假值：申请成员 userid（成员身份，不打印）
const int businessType = 2;            // 2 文档

GetAdvancedFeatureApplyIdListRequest pageRequest = new GetAdvancedFeatureApplyIdListRequest
{
    BusinessType = businessType,
    UserId = applyUserId,
    Limit = 200,                       // 官方最大值；返回条数仍可能少于它
    RequestType = 0,                   // 0 所有
};

GetAdvancedFeatureApplyIdListResponse page =
    await advancedFeature.GetApplyIdListAsync(pageRequest, cancellationToken);
WechatWorkException.ThrowIfFailed(page, "/cgi-bin/advanced_feature/get_apply_id_list");

var allApplyIds = new List<string>();
while (true)
{
    if (page.ApplyIdList != null)
        allApplyIds.AddRange(page.ApplyIdList);

    // 翻页判据只能用 has_more（布尔），不能用「本次条数 == limit」。
    if (page.HasMore != true)
        break;

    page = await advancedFeature.GetApplyIdListAsync(
        new GetAdvancedFeatureApplyIdListRequest
        {
            BusinessType = businessType,
            UserId = applyUserId,
            Limit = 200,
            RequestType = 0,
            Cursor = page.NextCursor,  // 响应用 next_cursor，请求用 cursor
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/advanced_feature/get_apply_id_list");
}

logger.LogInformation("拉到 {Count} 个申请单 id", allApplyIds.Count);
```

**本端点注意事项**

- 官方即 **POST 的查询接口**（AF1 只允许 `PostAttribute`）：改成 GET 会在守卫处打红，抓包时也不要按「查询=GET」推断。
- `has_more` 是**唯一**可靠的继续条件：官方明确不保证返回条数等于 `limit`。用「条数 < limit 就停」会漏页；用「`next_cursor` 为空就停」在本文证据里没有依据（§7）。
- 请求键 `cursor` 与响应键 `next_cursor` 名字不同：回填时把响应名照抄成请求字段是最常见的错位——请求侧 DTO 只有 `Cursor`（JSON `cursor`），没有 `NextCursor` 属性。
- 该接口不返回申请单内容，只有 id；详情必须逐单 E3。

### E3. `GetApprovalInfoAsync` — 获取申请单详情

**签名**

```csharp
Task<GetAdvancedFeatureApprovalInfoResponse> GetApprovalInfoAsync(
    [Body] GetAdvancedFeatureApprovalInfoRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_feature/get_approval_info` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/advanced_feature/get_approval_info?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建应用凭证） |
| 适用应用类型 | 仅企业自建应用 |
| 请求体编码 | `application/json`（`GetAdvancedFeatureApprovalInfoRequest` / `Response` / `AdvancedFeatureApprovalInfo` 已登记 `AdvancedFeatureJsonContext`，AF4） |
| 官方文档 | 99885 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ApplyId` | `apply_id` | `string?` | 是 | 申请单 id，来自 E2 的 `apply_id_list`（DTO 注释「必填」） |
| —（不进方法签名） | `access_token` | — | 是 | Query 注入 |

单值入参：一次只取一张申请单，批量枚举是调用方职责（E2 → 循环 E3）。

```json
{ "apply_id": "APPLY_ID_FAKE_0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `approval_info` | `ApprovalInfo` | `AdvancedFeatureApprovalInfo?` | 申请单详情容器（AF4 锁定 JSON 名） |
| `approval_info.applicant` | `Applicant` | `string?` | 申请人 userid（成员身份，不得落日志） |
| `approval_info.create_time` | `CreateTime` | `long?` | 申请单创建时间戳（单位本文未取证，§7） |
| `approval_info.business_type` | `BusinessType` | `int?` | 高级账号类型：1 邮件 / 2 文档 / 3 微盘 / 4 会议 |
| `approval_info.apply_id` | `ApplyId` | `string?` | 申请单 id（回显） |
| `approval_info.approval_id` | `ApprovalId` | `string?` | 自建应用中的审批 id |
| `approval_info.approval_url` | `ApprovalUrl` | `string?` | 自建应用中的审批单跳转链接 |
| `approval_info.approval_status` | `ApprovalStatus` | `int?` | 审批单状态：0 历史申请记录（无流程信息）/ 1 审批人审批中 / 2 审批人驳回 / 3 审批人已同意 / 4 管理员驳回 / 5 管理员已分配 —— **与 E1 请求的同名字段不是一套值域** |
| `approval_info.approval_type` | `ApprovalType` | `int?` | 申请单类型：0 默认申请 / 1 自定义审批 / 2 企业 openapi |
| `approval_info.request_reason` | `RequestReason` | `string?` | 申请原因，成员填写的**自由文本**；SDK 不解析不改写，宿主不得落日志/遥测（DTO remarks） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "approval_info": {
    "applicant": "mud_zhangsan",
    "create_time": 1761696000,
    "business_type": 2,
    "apply_id": "APPLY_ID_FAKE_0001",
    "approval_id": "APPROVAL_ID_FAKE_0001",
    "approval_url": "https://approval.example.com/detail/APPROVAL_ID_FAKE_0001",
    "approval_status": 3,
    "approval_type": 2,
    "request_reason": "示例理由文本"
  }
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.AdvancedFeature;

// 前置同 E1：应用与模块已注册。取详情是按 id 单条调用，批量请先走 E2。
IWechatWorkInternalAdvancedFeatureService advancedFeature =
    provider.GetRequiredService<IWechatWorkInternalAdvancedFeatureService>();

string applyId = "APPLY_ID_FAKE_0001";   // 假值：E2 拉到的 apply_id

GetAdvancedFeatureApprovalInfoResponse infoResponse = await advancedFeature.GetApprovalInfoAsync(
    new GetAdvancedFeatureApprovalInfoRequest { ApplyId = applyId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(infoResponse, "/cgi-bin/advanced_feature/get_approval_info");

// approval_info 可空（errcode==0 但官方未返回容器时按「无详情」处理，不要直接解引用）。
AdvancedFeatureApprovalInfo? info = infoResponse.ApprovalInfo;

// 查询面 approval_status：0/1/2/3/4/5，与 E1 请求的 1/2/3/101 不是一套值域。
int? queryStatus = info?.ApprovalStatus;

// applicant / request_reason 属个人信息，日志里只留单号与状态码。
logger.LogInformation("申请单 {ApplyId} 查询到状态码 {Status}", applyId, queryStatus);
```

**本端点注意事项**

- **同名字段、两套值域**：本端点 `approval_status` 的 0（历史申请记录，无流程信息）与 4（管理员驳回）、5（管理员已分配）在设置面都不存在；设置面的 101（已撤销）在本面也不存在。把 E1 写入的 1/2/3 直接与查询结果比对，恰好能对上 1/2/3，**会掩盖 4/5/0 的语义差**，报表口径必须分别映射。
- `approval_type = 2` 的官方值是「企业 openapi」，`req_type = 1` 是 E2 的「仅 api 单」过滤维度 —— 两者分属响应字段与请求过滤，取值对应关系官方未留档（§7），不要用一个去推另一个。
- `ApprovalInfo` 为可空容器：`errcode == 0` 不保证容器非空，取字段前先判空。
- `request_reason` 是自由文本，长度与字符集无官方留档（§7）；不得用它拼 SQL、文件名或日志。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族 `approval_status`（E1 请求，1/2/3/101）与 E3 响应的 `approval_status`（0/1/2/3/4/5） | 同 JSON 名、两套值域：查询面多出 0 历史无流程、4 管理员驳回、5 管理员已分配，设置面多出 101 已撤销 | 用一套枚举常量解释两侧，4/5/0 会被当成「未知」或误映射成「驳回/同意」；状态机判断错、报表分类错，且无异常。SDK 不归一：两处各自声明为 `int?`，DTO remarks 明确「不复用同一枚举常量」，AF4 锁定该类型 |
| E1 请求的 `approval_status` 与节点上的 `node_apv_status` | 前者是审批单级（1/2/3/101），后者是节点级（1/2/3/101/**102 未到流程**） | 把 102 写进单级 `approval_status`（官方是否接受未留档，§7）；或在本地按单级值域校验节点值，误拒合法的「未到流程」节点 |
| E1 的「写」与普通 PATCH | E1 是**覆盖式全量写入**：官方明确变更节点须全量传入 | 只传本次变动的节点 = 删除其余节点，`errcode` 仍为 0；审批链在无告警的情况下残缺 |
| `apply_id` 与 `approval_id` | 前者是企微申请单 id（E2 产出），后者是应用侧审批 id；二者**一一对应且不可改变** | 每轮同步重新生成 `approval_id` ⇒ 对应关系断裂，后续写入与 E3 的 `approval_id` 对不上 |
| 本域 `has_more`（布尔）与 [AdvancedApi](../AdvancedApi/AdvancedApi.md) 域 `has_more`（整型 0/1） | 同名字段、两种 JSON 类型 | 复用同一段反序列化/判续逻辑，布尔侧把 `0` 当「还有下一页」或反之，翻页直接错乱 |
| E2 请求键 `cursor` 与响应键 `next_cursor` | 请求用 `cursor`、响应用 `next_cursor`（两个不同 JSON 名） | 把响应字段名照抄进下一次请求 ⇒ 游标不被识别，永远从第一页开始，循环不收敛 |
| 本族（`/cgi-bin/advanced_feature/*`）与 [AdvancedApi](../AdvancedApi/AdvancedApi.md) 域（`/cgi-bin/advanced_api/*`） | 名字都带「高级」：本族是自建应用 + `access_token` + `Wechat.AccessToken@Internal`；AdvancedApi 是服务商 `provider_access_token`，管高级接口订单与代付 | 拿服务商配置调本族（或反之）在 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`；按「高级」关键词检索文档会把两套凭证面混在一处 |
| 本族的「成员申请单」与 [Approval](../Approval/Approval.md) 域的「审批申请」（`/cgi-bin/oa/*`） | 本族只是「高级账号开通申请」的审批**托管**，路由前缀 `advanced_feature`；Approval 域是企业自有 OA 审批单据（提交、详情、模板），路由前缀 `oa` | 用 Approval 域的提交/详情端点去找高级账号申请单，找不到对应 `apply_id`；两域数据模型不可互换 |
| 三个端点「全部 POST」 | 含两支纯查询（E2/E3）官方也是 POST | 按 REST 习惯改成 GET，守卫 AF1 直接打红；抓包工具里按 GET 复现会拿到 404/405，误判为「接口下线」 |
| 个人信息与工单文本 | `userid` / `applicant` / `current_approvers` / `completed_approvers` / `request_reason` | 直接 `LogInformation("{@request}", request)` 会把成员身份与申请理由写进日志/遥测，属合规缺陷（接口与 DTO remarks 明令禁止） |

另外两条 SDK 侧行为约束，会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改用 Header）。`access_token` 在组件 `SensitiveUrlRedactor` 词表内，异常消息里的 `RequestUri` 构造期已剥离 query 与 userinfo；但自己打日志时仍不要把完整 URL 打出去。
- **不自动判错**：声明式客户端只保证 HTTP 传输成功，业务错误一律以 `errcode` 承载；必须判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`。E1 无业务负载，判错就是唯一的成功信号。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径；E1 的 0 只表示「写入请求被接受」，不表示节点集合符合你的预期（见 §5） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | `access_token` 失效 / 过期 / 关联凭证失效 | 本族族别失效码，命中即走令牌自愈链路，业务侧通常看不到 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈；正常装配下不应出现，出现即令牌链未装配或 `AppKey` 解析不到（`WechatAppContext.GetTokenManager` 的前置失败） |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | `suite_access_token` / `provider_access_token` 失效 | 本族不消费这两种令牌；出现即**凭据族别错配**的信号（请求里带的是服务商/套件令牌，例如错调了 AdvancedApi 域的路由）。注意：用第三方配置注入本族子接口会在本地抛 `WechatTokenOwnerMismatchException`，请求根本发不出去，不会以 42011 的形态出现 |

令牌失效自愈的判定集合与判定器同源（`WechatTokenInvalidationDetector`）：`{40014, 42001, 42007, 42009, 42011}`，命中后的链路是「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」。本族是 `access_token` + `@Internal`，**若 40014/42001 稳定复现，说明刷新本身在失败**：依次排查 `AgentSecret`（corpsecret）是否正确、自建应用是否被停用、同一 `AppKey` 是否被多处并发刷新挤掉。加重试不解决这三类成因。

本族三支端点的专属业务错误码（例如 `apply_id` 不存在、`approval_url` 非法、节点超限、状态流转不被允许分别返回什么码）接口注释未留档、本文亦未向官方页取证 ⇒ **未逐项核验**，见 §7。需要按码分流时先核验，不要在 SDK 侧硬编码码表。其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方页面最后更新日期 | 本次未重取官方页（任务纪律：官方取证由文档负责人串行完成），正文官方面事实全部经接口/DTO XML 注释转述 ⇒ **未逐项核验** | 99880 / 99883 / 99885 渲染取文后回填元信息表 |
| 三支端点的独立频率数值 | 接口 remarks 明确「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」，但全局数值未留档 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| 三支端点的专属业务错误码 | 注释未留档错误码表；令牌失效码由通用判定器覆盖 ⇒ **未逐项核验** | 官方附录「全局错误码」（90455）逐端点核对后补接口 remarks |
| 成员可见范围/权限前置 | 注释只留档「配置到『高级功能 - 成员申请管理 - 通过 api 设置申请流程』」，未说明成员是否须在应用可见范围内、管理员权限是否有额外要求 ⇒ **未逐项核验** | 99883 / 99885 页面正文 |
| `limit` 非法值或超 200 的官方行为 | 只写「默认 100、最大 200」，未说明超过 200 是截断、报错还是按 200 处理 ⇒ **未逐项核验** | 99883 页实测 |
| 游标语义（`cursor` ↔ `next_cursor`） | 两个键名由 DTO 分别留档，「回填上一页 `next_cursor`」是按名对应；游标有效期、`has_more=true` 但游标为空的边界未留档 ⇒ **未逐项核验** | 99883 页面正文 |
| 单级 `approval_status` 是否也接受 102 | 请求 DTO 只列 1/2/3/101，102 仅出现在节点字段；官方是否允许单级取 102 无留档 ⇒ **未逐项核验** | 99880 页面的参数表 |
| 节点集合的边界 | 「须全量传入」已留档，但 `node_list` 传空数组是清空还是报错、单张申请单的节点数量上限均无留档 ⇒ **未逐项核验** | 99880 页面正文 |
| 状态流转合法性 | 已同意（3）之后能否改回审批中（1）、能否重复提交相同内容是否为幂等，均无留档 ⇒ **未逐项核验** | 99880 页面正文与实测 |
| 时间戳单位 | `create_time` / `apv_update_time` 声明为 `long?`，注释只写「时间戳」，秒/毫秒未留档 ⇒ **未逐项核验** | 99885 / 99880 页面的响应示例 |
| `req_type` 与 `approval_type` 的对应关系 | DTO 各自只写值域与过滤语义，未说明「仅 api 单」是否等价于 `approval_type = 2`、非 api 单能否被 E1 写入审批信息 ⇒ **未逐项核验** | 99883 与 99880 两页对照 |
| `request_reason` / `approval_url` 的长度与字符集上限 | 注释只写 `approval_url` 须以 http(s) 开头，未给长度上限；`request_reason` 无任何长度/字符集留档 ⇒ **未逐项核验** | 99880 / 99885 页面正文 |
| 本域**没有域 README** | 源码目录 [Interfaces/AdvancedFeature/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedFeature/) 下只有两个 `.cs` 文件，无 `README.md` ⇒ 本篇没有 L4 域总览可链，族切分与端点速览均以本文与守卫为准 | 若后续补 README，同批在 §8 增加链接 |
| 守卫与注释的表述粒度差异 | AF4 与域级 remarks 把设置面状态概述为「1/2/3/101」，未提节点级 **102 未到流程**；接口 `SetApprovalDetailAsync` `<summary>` 与 `AdvancedFeatureApprovalNode` 注释明确 102 属节点。二者不矛盾（不同字段），但只读守卫概述会漏掉 102 ⇒ 本文按注释原文分「单级 4 值 / 节点级 5 值」描述；是否需要把守卫概述补全，留待维护者定夺 | AF4 断言（权威：类型为 `int?`、字段名为 `node_apv_status`）＋ `SetAdvancedFeatureApprovalDetailRequest.ApprovalStatus` 与 `AdvancedFeatureApprovalNode.NodeApprovalStatus` 注释 |
| 同批兄弟篇目尚未落盘 | §1/§5/§8 链接的 [AdvancedApi](../AdvancedApi/AdvancedApi.md) 与 [Approval](../Approval/Approval.md) 在本文撰写时工作副本中尚不存在（同批其他篇目在写）⇒ 链接可能暂时失效，合并时须确认两篇已落盘 | `Documents/Mud.Wechat/reference/Work/` 目录清单 |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 兄弟族（同叫「高级」但凭证面相反）：[AdvancedApi.md](../AdvancedApi/AdvancedApi.md) — 高级接口订单与代付（`/cgi-bin/advanced_api/*`，`provider_access_token`）
- 邻域（同叫「审批」但对象不同）：[Approval.md](../Approval/Approval.md) — OA 审批申请、模板、假期管理与审批流程引擎（`/cgi-bin/oa/*`）
- 接口源码目录：[Src/Work/Mud.Wechat.Work/Interfaces/AdvancedFeature/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedFeature/)（无域 README，见 §7）
- 契约守卫：[WechatAdvancedFeatureContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAdvancedFeatureContractGuards.cs)（AF1~AF4）· [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）
- 自建应用令牌管理器：[Src/Work/Mud.Wechat.Work/Authentication/](../../../../../Src/Work/Mud.Wechat.Work/Authentication/)
- 凭据与多应用作用域：`guide/配置与凭据.md`（**尚未成文**），当前参考 [Documents/Mud.Wechat/README.md](../../../README.md)
- 官方页：[设置审批信息（99880）](https://developer.work.weixin.qq.com/document/path/99880) · [批量获取申请单 id（99883）](https://developer.work.weixin.qq.com/document/path/99883) · [获取申请单详情（99885）](https://developer.work.weixin.qq.com/document/path/99885)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
