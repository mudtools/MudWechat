# 审批申请数据（提交 / 批量取单号 / 详情 / 旧版数据）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Approval` 域 · 审批申请数据族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkApprovalService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalService.cs) · [IWechatWorkApprovalService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalService_Internal.cs) · [IWechatWorkApprovalService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalService_Provider.cs) · [IWechatWorkApprovalService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalService_ThirdParty.cs)（目录：[Interfaces/Approval/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/)） |
| DTO 源文件 | [DataModels/Approval/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/) 与 [DataModels/Approval/RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/RequestModel/)（`RequestModel/` 仅作组织、不入命名空间，全部为 `Mud.Wechat.Work.DataModels.Approval`） |
| 契约守卫 | [WechatApprovalContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs)（AP1a 公共面 3 路由 / AP1b 旧接口仅自建 / AP2 层级与各子接口声明端点数 / AP3 审批模块 16 接口令牌绑定 / AP4 域内 109 型上下文登记 / AP5 字段陷阱）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 自建 91853（E1）/ 91816（E2）/ 91983（E3）/ 91530（E4）；第三方 92632（E1）/ 94603（E2）/ 92634（E3）；服务商代开发 96507（E1）/ 96509（E2）/ 96510（E3）。文档号一律取自接口 XML 注释的 `<remarks>`；**本次未重取官方页**，页面更新日期见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 2 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AP1a `ApprovalRoutes`（本族 3 条逐路由/逐动词）+ AP1b `ApprovalLegacyRoutes`（1 条 + 两条 `BeNull` 反向断言）+ AP2 层级（父接口恰 3、自建子接口恰 1、代开发/第三方子接口恰 0）+ AP3 令牌绑定 + AP4 109 型登记（端点级清单含本族 8 型）+ AP5 字段陷阱 + 接口与 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md) 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承本域 2026-10-18 核验留档，本次未重取；凡只有官方页能支撑的项（频率以外的专属码、页面更新日期）一律进 §7 |

## 1. 这个接口族做什么

企业微信「审批应用」的四支 HTTP 端点，覆盖审批单据的**写入与读取**：E1 以申请人身份按 `template_id` 提交一张审批单（返回审批单号 `sp_no`），E2 在时间窗内按模板/申请人/部门/状态筛选批量取审批单号，E3 按 `sp_no` 取整张单据的完整详情（状态、审批流程记录、抄送、控件值、备注、流程树），E4 是旧版「获取审批数据」——一次返回请假/报销模板的结构化字段与自定义模板的原始 `apply_data`，官方已声明后续不再维护。

定位键只有两个，且层级不同：`template_id` 是**审批模板**（单据的类型定义，从「获取审批模板详情」或审批模板编辑页链接获得），`sp_no` 是**审批单实例**的主键（E1 提交成功即返回、E2 列表给出、E3 用它取详情）。旧接口 E4 另用数字形态的 `sp_num`（`long`），与 `sp_no`（`string`）不是同一取值空间，不能互喂（§5）。

使用时机是**「把企业内的审批动作接到自己的系统里」**：HR/费控系统建单与对账、单据归档、按状态做下游联动。它是「审批应用」这条单据线上唯一的 HTTP 面，不含模板定义（[ApprovalTemplate.md](./ApprovalTemplate.md)）与假期余额（[Vacation.md](./Vacation.md)）。

**不该用本族的场景**：要按自定义单号（`thirdNo`）查「审批流程引擎」的状态，走 [ApprovalEngine.md](./ApprovalEngine.md)——那是另一套单据体系与另一套状态码；要实时感知状态变化应接回调 `sys_approval_change`（`Mud.Wechat.Work.Callback` 包），本族 E3 只用于主动查询与兜底。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 自建 `WechatAppType.Internal`；第三方 `WechatAppType.ThirdParty`；服务商代开发 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()`；运行期动态注册应用时 `WechatAppManager` 同样校验 |
| 必填配置属性（自建） | `CorpId` · `AgentSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动即失败，不留到首次调用） |
| 必填配置属性（第三方 / 代开发） | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，四项齐备 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键（自建） | `WechatTokenManagerKeys.InternalAccessToken`（值 `Wechat.AccessToken@Internal`），由 `IWechatWorkInternalApprovalService` 的 `[Token]` 声明；**公共父接口不声明归属键** | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；在自建应用上下文里消费第三方/代开发子接口（或反之）抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |
| 令牌归属键（第三方 / 代开发） | `WechatTokenManagerKeys.CorpAccessToken`（值 `Wechat.AccessToken@Corp`），由两个空标记子接口的 `[Token]` 声明 | 同上 |
| 令牌注入 | `TokenInjectionMode.Query`，参数名恒为 `access_token`，**不进方法签名**（AP3 对审批模块 16 个接口逐一断言） | 每次请求 |

`access_token` 由 SDK 按当前应用上下文与 scope 自动换取、缓存并刷新，**开发者不接触令牌值**。第三方/代开发的令牌是**授权企业级**（一企一份，scope = `authCorpId`），调用前必须进入目标授权企业作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时写「应用 + 企业」两级上下文、释放时逆序还原）。`permanent_code` 是凭据，本文与示例只写参数名，不得进日志、遥测或异常消息。

### 2.2 权限与业务前置

以下逐条出自接口 XML 注释与 DTO 类级/属性级 `<remarks>`（官方页本次未重取）：

- **权限口径（三类通用，接口 `remarks` 原文）**：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。自 2023-12-01 0 点起不再支持通过**系统应用 secret** 调用本域接口（存量企业暂不受影响）。
- **E1**：频率限制 600 次/分钟；模板的控件为必填属性时表单中对应控件必须有值；**暂不支持通过接口提交【打卡补卡】【调班】模板审批单**；申请人是「以此员工身份提交」，须在应用可见范围内。
- **E2**：一次最多 100 条，频率不可超过 600 次/分；`endtime` 需大于 `starttime`，跨度不能超过 **31 天**；**自建应用仅允许获取应用可见范围内申请人提交的表单**。
- **E3**：频率限制 600 次/分钟；单张 `sp_no`，无批量形态。
- **E4（旧）**：仅自建应用开放，频率 600 次/分；一次最多 100 条，超出用 `next_spnum` 分页；`endtime > starttime` 且跨度不超过 **30 天**；**无法获取管理员已删除的审批单数据**；官方推荐改用 E2 + E3，本端点后续不再维护、逐步下线。

### 2.3 官方开放的应用类型

E1~E3 为**三类应用公共面**，全部声明在公共父接口 `IWechatWorkApprovalService` 上（AP1a 锁 3 条路由，AP2 锁父接口 `DeclaredOnly` 恰 3 个方法）。E4「获取审批数据（旧）」官方仅自建开放，由唯一自建子接口承载（AP1b 锁 1 条路由，并对代开发/第三方子接口做 `BeNull` 反向断言）；代开发与第三方子接口是**零端点空标记**——它们只为类型化注入与令牌归属而存在，自身不新增任何方法。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkApprovalService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空，不可直接给最终用户） | 3（E1~E3） | 否 |
| `IWechatWorkInternalApprovalService` | `Mud.Wechat.Work` | 企业自建应用子接口 | 1（E4，另继承 E1~E3） | 是 |
| `IWechatWorkProviderApprovalService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 0（空标记，继承 E1~E3） | 是 |
| `IWechatWorkThirdPartyApprovalService` | `Mud.Wechat.Work` | 第三方应用子接口 | 0（空标记，继承 E1~E3） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Approval` / `AddApprovalApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Approval`；三个子接口的 `InheritedFrom = nameof(WechatWorkApprovalService)`（继承父接口生成实现类，避免生成器重复实现公共端点——AP2 断言）
- 端点索引：**E1** `ApplyApprovalAsync`（提交审批申请） · **E2** `BatchGetApprovalNumbersAsync`（批量获取审批单号） · **E3** `GetApprovalDetailAsync`（获取审批申请详情） · **E4** `GetApprovalDataAsync`（获取审批数据（旧），仅自建）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，仅作契约基座）。本族 9 个端点级契约类型（四对请求/响应 + `ApprovalInfoFilter`）与审批域其余类型合计 **109 型**全部登记进 `ApprovalJsonContext`（`internal`，示例不可引用；AP4 断言域内顶层类型 `HaveCount(109)` 且 `SerializerClassName` 恒为 `Approval`）。

---

## 4. 端点

### E1. `ApplyApprovalAsync` — 提交审批申请

**签名**

```csharp
Task<ApplyApprovalResponse> ApplyApprovalAsync(
    [Body] ApplyApprovalRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/applyevent` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/oa/applyevent?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；第三方/代开发须先进入授权企业作用域，见 §2.1） |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 请求体编码 | `application/json`，源生成序列化（`ApprovalJsonContext`，AP4 锁定登记） |
| 官方文档 | 自建 91853 · 第三方 92632 · 代开发 96507（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.CreatorUserid` | `creator_userid` | `string?` | 是 | 申请人 userid；以此员工身份提交，须在应用可见范围内 |
| `request.TemplateId` | `template_id` | `string?` | 是 | 审批模板 id（企业内模板**实例** id；第三方/代开发场景不是服务商后台对应模板的 id）；**暂不支持打卡补卡/调班模板** |
| `request.UseTemplateApprover` | `use_template_approver` | `int?` | 是 | 审批人模式：`0` 由本请求的 `process` 指定审批人/抄送人、`1` 使用模板在管理后台配置的流程（要求流程中无「申请人自选」节点，支持条件审批）；默认 `0` |
| `request.ChooseDepartment` | `choose_department` | `int?` | 否 | 提单者提单部门 id，不填默认为主部门 |
| `request.Process` | `process` | `ApprovalProcessInfo?` | 是 | 新版流程列表；`use_template_approver` 为 `0` 时必填（官方口径为条件必填），子字段见下表 |
| `request.ApplyData` | `apply_data` | `ApprovalApplyData?` | 是 | 各控件值；必填控件必须有值、选填可为空，SDK 不校验 |
| `request.SummaryList` | `summary_list` | `List<ApprovalSummaryLine>?` | 是 | 显示在审批通知卡片/审批列表的摘要，**最多 3 行**，每行文字建议 ≤ 20 字符 |
| —（不进方法签名） | `access_token` | — | 是 | 应用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

`process` 子结构（`ApprovalProcessInfo` → `node_list[]` → `ApprovalProcessNodeItem`）：

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Process.NodeList` | `node_list` | `List<ApprovalProcessNodeItem>?` | 是 | 流程节点列表 |
| `NodeList[].Type` | `type` | `int?` | 是 | 节点类型：`1` 审批人 / `2` 抄送人 / `3` 办理人 |
| `NodeList[].ApvRel` | `apv_rel` | `int?` | 是 | 多人审批方式：`1` 会签 / `2` 或签 / `3` 依次审批；`type` 为 `1`、`3` 时必填 |
| `NodeList[].Userid` | `userid` | `List<string>?` | 是 | 用户 id 列表。**JSON 键是单数 `userid`、值是数组**，不是 `userid_list` |

`apply_data` 子结构（`ApprovalApplyData` → `contents[]` → `ApprovalControlItem`）：

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `ApplyData.Contents` | `contents` | `List<ApprovalControlItem>?` | 是 | 一个元素 = 一个控件；每个控件必须带模板中配置的 `id` |
| `Contents[].Control` | `control` | `string?` | 是 | 控件类型（18 值），取值与 `value` 子结构的对应关系见 §5 |
| `Contents[].Id` | `id` | `string?` | 是 | 控件唯一 id；从「获取审批模板详情」获得，**较早创建的模板控件 id 为数字串形态**，不要按 `int`/`long` 建模 |
| `Contents[].Value` | `value` | `ApprovalControlValue?` | 是 | 控件值超集袋，按 `control` 取用其中一项（§5） |
| `Contents[].Title` | `title` | `List<ApprovalLangTextItem>?` | 否 | 控件名称（多语言）。**提单请求体不带此字段**，仅详情/模板详情响应返回 |
| `Contents[].Hidden` | `hidden` | `int?` | 否 | `1` 表示控件被隐藏。**仅详情响应返回** |

```json
{
  "creator_userid": "mud-USERID-FAKE-0001",
  "template_id": "mud-TEMPLATEID-FAKE-0001",
  "use_template_approver": 0,
  "choose_department": 2,
  "process": {
    "node_list": [
      { "type": 1, "apv_rel": 1, "userid": ["mud-USERID-FAKE-0002"] }
    ]
  },
  "apply_data": {
    "contents": [
      { "control": "Money", "id": "mud-CONTROLID-FAKE-0001", "value": { "new_money": "1200" } }
    ]
  },
  "summary_list": [
    { "summary_info": [ { "text": "差旅报销", "lang": "zh_CN" } ] }
  ]
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `sp_no` | `SpNo` | `string?` | 提交成功后返回的审批单号；是 E2/E3 的主键 |

```json
{ "errcode": 0, "errmsg": "ok", "sp_no": "mud-SPNO-FAKE-0001" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;                            // IWechatWorkInternalApprovalService / AddWechatWorkServices（builder 实例方法无需额外 using）
using Mud.Wechat.Work.Abstractions;               // AddWechatApp 扩展方法落此命名空间（极易漏）
using Mud.Wechat.Work.Abstractions.Enums;         // WechatAppType
using Mud.Wechat.Work.DataModels.Approval;        // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，Microsoft.Extensions.DependencyInjection（GetRequiredService）与
// Microsoft.Extensions.Logging（logger 扩展方法）由 Web SDK 隐式 using 提供；
// builder / config / provider / logger / cancellationToken 由宿主提供。

// ① 注册自建应用：凭据从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:AgentSecret"];   // 凭据：只从配置读取，不得进日志
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配）。
builder.Services.AddWechatWorkServices(b => b.AddApprovalApi());

// ③ 解析自建子接口（只有它带 E4；E1~E3 由它继承）。
var approval = provider.GetRequiredService<IWechatWorkInternalApprovalService>();

// ④ 业务输入段内声明为占位值：模板 id 与控件 id 来自「获取审批模板详情」。
var templateId = "mud-TEMPLATEID-FAKE-0001";
var applicantUserId = "mud-USERID-FAKE-0001";      // 须在应用可见范围内
var approverUserId = "mud-USERID-FAKE-0002";
var controlId = "mud-CONTROLID-FAKE-0001";

var response = await approval.ApplyApprovalAsync(
    new ApplyApprovalRequest
    {
        CreatorUserid = applicantUserId,
        TemplateId = templateId,
        UseTemplateApprover = 0,                    // 0 = 本请求用 process 指定审批人，此时 process 必填
        Process = new ApprovalProcessInfo
        {
            NodeList = new List<ApprovalProcessNodeItem>
            {
                new ApprovalProcessNodeItem
                {
                    Type = 1,
                    ApvRel = 1,
                    Userid = new List<string> { approverUserId },  // JSON 键是单数 userid
                },
            },
        },
        ApplyData = new ApprovalApplyData
        {
            Contents = new List<ApprovalControlItem>
            {
                new ApprovalControlItem
                {
                    Control = "Money",                  // 金额控件：值按字符串传输
                    Id = controlId,
                    Value = new ApprovalControlValue { NewMoney = "1200" },
                },
            },
        },
        SummaryList = new List<ApprovalSummaryLine>
        {
            new ApprovalSummaryLine
            {
                SummaryInfo = new List<ApprovalLangTextItem>
                {
                    new ApprovalLangTextItem { Text = "差旅报销", Lang = "zh_CN" },  // 下划线，不是 zh-CN
                },
            },
        },
    },
    cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!response.IsSuccess)
{
    // 审批单据含申请人/审批人/金额等个人信息：日志只写 errcode/errmsg，不写请求体。
    logger.LogError("提交审批申请失败 {ErrorCode}: {ErrorMessage}", response.ErrorCode, response.ErrorMessage);
    throw new InvalidOperationException($"提交审批申请失败：{response.ErrorCode}");
}

// ⑥ sp_no 是 string 主键，后续 E2 列表与 E3 入参都用它（不是旧接口的 long sp_num）。
string spNo = response.SpNo!;
```

**本端点注意事项**

- 单次频率上限 600 次/分钟（接口注释）；SDK 不做本地限流，超限由 `errcode` 表达。
- 模板必填控件必须有值，**SDK 不校验**：漏填的必填控件由官方拒绝，不是静默成功。
- 暂不支持通过接口提交【打卡补卡】【调班】模板审批单；补卡控件（`PunchCorrection`）与收款账户控件（`BankAccount`）只出现在「获取审批申请详情」的官方附录中，提单侧可写性见 §7。
- `use_template_approver = 1` 时须保证模板流程中**没有「申请人自选」节点**，否则官方报错；这条组合合法性 SDK 不在本地拦截。

### E2. `BatchGetApprovalNumbersAsync` — 批量获取审批单号

以下 E2~E4 的凭据面与 E1 一致（`Wechat.AccessToken` + Query 注入 `access_token` + `application/json` 源生成序列化），表中只列差异。

**签名**

```csharp
Task<BatchGetApprovalNumbersResponse> BatchGetApprovalNumbersAsync(
    [Body] BatchGetApprovalNumbersRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/getapprovalinfo` |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 官方文档 | 自建 91816 · 第三方 94603 · 代开发 96509（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Starttime` | `starttime` | `string?` | 是 | 时间范围开始（Unix 秒）。**官方请求示例按字符串传输（如 `"1569546000"`），AP5 锁定 `typeof(string)`**，不要改成 `long` |
| `request.Endtime` | `endtime` | `string?` | 是 | 时间范围结束（Unix 秒，同为字符串）；需大于 `starttime`，跨度不能超过 **31 天** |
| `request.NewCursor` | `new_cursor` | `string?` | 是 | 分页游标，默认空串；后续填上页返回的 `new_next_cursor` |
| `request.Size` | `size` | `int?` | 是 | 一次拉取数量，默认 100、**上限 100**；返回的 `sp_no_list` 个数可能小于 `size` |
| `request.Filters` | `filters` | `List<ApprovalInfoFilter>?` | 否 | 筛选条件，可多个；组合语义见下表说明 |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入 |

`filters` 元素（`ApprovalInfoFilter`）：

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Filter.Key` | `key` | `string?` | 是 | 筛选类型：`template_id` 模板 / `creator` 申请人 / `department` 提单者所在部门 / `sp_status` 审批状态 / `record_type` 审批单类型属性 |
| `Filter.Value` | `value` | `string?` | 是 | 筛选值，**恒为字符串**：模板 id / 申请人 userid / 部门 id / 状态（`1` 审批中、`2` 已通过、`3` 已驳回、`4` 已撤销、`6` 通过后撤销、`7` 已删除、`10` 已支付）/ `record_type`（`1` 请假、`2` 打卡补卡、`3` 出差、`4` 外出、`5` 加班、`6` 调班、`7` 会议室预定、`8` 退款审批、`9` 红包报销审批） |

组合语义（DTO 类级 remarks 原文）：**仅 `department` 支持同时配置多个条件**；不同类型之间为「与」、同类型之间为「或」；`record_type` 仅对 **2021-05-31 之后**新提交的审批单生效，历史单无法按表单类型过滤。SDK 不校验组合合法性。

```json
{
  "starttime": "1569546000",
  "endtime": "1569600000",
  "new_cursor": "",
  "size": 100,
  "filters": [
    { "key": "sp_status", "value": "2" },
    { "key": "department", "value": "2" },
    { "key": "department", "value": "3" }
  ]
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `sp_no_list` | `SpNoList` | `List<string>?` | 满足条件的审批单号列表；个数可能小于请求 `size` |
| `new_next_cursor` | `NewNextCursor` | `string?` | 下一页游标；**返回结果没有该字段即表示已拉取完**（接口注释原文） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "sp_no_list": ["mud-SPNO-FAKE-0001"],
  "new_next_cursor": "mud-CURSOR-FAKE-0001"
}
```

**本端点注意事项**

- **纯查询语义，官方仍即 POST**（AP1a 逐条断言 `typeof(PostAttribute)`）；按「查询用 GET」改写动词即打红守卫，也会被官方拒。
- 结束条件是**游标字段缺失**，不是「返回条数 < `size`」、也不是「`sp_no_list` 为空」；旧的 `cursor` / `next_cursor` 官方标注待废弃，本仓**不建模**。
- 自建应用只能看到**应用可见范围内申请人**提交的表单——「拉不到」优先怀疑可见范围，而不是重试。
- 31 天（本端点）与 30 天（E4）是两个不同数字，不要共用一个常量。

### E3. `GetApprovalDetailAsync` — 获取审批申请详情

**签名**

```csharp
Task<GetApprovalDetailResponse> GetApprovalDetailAsync(
    [Body] GetApprovalDetailRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/getapprovaldetail` |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 官方文档 | 自建 91983 · 第三方 92634 · 代开发 96510（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.SpNo` | `sp_no` | `string?` | 是 | 审批单号（E1 返回 / E2 列表给出）。**无批量形态**：一次一张 |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入 |

```json
{ "sp_no": "mud-SPNO-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `info` | `Info` | `ApprovalDetailInfo?` | 单据详情根节点，子字段见下表 |

`info` 子字段（`ApprovalDetailInfo`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `info.sp_no` | `SpNo` | `string?` | 审批编号 |
| `info.sp_name` | `SpName` | `string?` | 审批申请类型名称（审批模板名称） |
| `info.sp_status` | `SpStatus` | `int?` | **单据级**状态：`1` 审批中 / `2` 已通过 / `3` 已驳回 / `4` 已撤销 / `6` 通过后撤销 / `7` 已删除 / `10` 已支付（注释仅列此 7 值，无 5/8/9） |
| `info.template_id` | `TemplateId` | `string?` | 审批模板 id |
| `info.apply_time` | `ApplyTime` | `long?` | 提交时间（Unix 秒，响应侧是数字） |
| `info.applyer` | `Applyer` | `ApprovalApplyer?` | 申请人：`userid` + `partyid`（`partyid` 按字符串传输） |
| `info.batch_applyer` | `BatchApplyer` | `List<ApprovalUserRef>?` | 批量申请人；**与 `applyer` 互斥** |
| `info.sp_record` | `SpRecord` | `List<ApprovalSpRecord>?` | 审批流程记录（审批节点树） |
| `info.notifyer` | `Notifyer` | `List<ApprovalUserRef>?` | 抄送人。**官方字段名即为 `notifyer`（非 notifier）**，AP5 锁定 |
| `info.apply_data` | `ApplyData` | `ApprovalApplyData?` | 控件值，结构与 E1 请求的 `apply_data` 相同（另带 `title` 与 `hidden`） |
| `info.comments` | `Comments` | `List<ApprovalComment>?` | 备注节点 |
| `info.process_list` | `ProcessList` | `ApprovalProcessListInfo?` | 流程列表（与 `sp_record` 并行的第二棵树，不互替） |

`sp_record[]`（`ApprovalSpRecord`）与 `details[]`（`ApprovalSpRecordDetail`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `sp_record[].sp_status` | `SpStatus` | `int?` | 节点状态：`1` 审批中 / `2` 已同意 / `3` 已驳回 / `4` 已转审 / `11` 已退回 / `12` 已加签 / `13` 已同意并加签 |
| `sp_record[].approverattr` | `Approverattr` | `int?` | 节点审批方式：`1` 或签 / `2` 会签 |
| `sp_record[].details[]` | `Details` | `List<ApprovalSpRecordDetail>?` | 分支详情，一个节点多个审批人 |
| `details[].approver` | `Approver` | `ApprovalUserRef?` | 分支审批人（只有 `userid`） |
| `details[].speech` | `Speech` | `string?` | 审批意见 |
| `details[].sp_status` | `SpStatus` | `int?` | 分支状态，取值同节点级（1/2/3/4/11/12/13） |
| `details[].sptime` | `Sptime` | `long?` | 审批操作时间戳，**`0` 表示未操作** |
| `details[].media_id` | `MediaId` | `List<string>?` | 审批意见附件（微盘文件无法获取） |

`comments[]`（`ApprovalComment`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `comments[].commentUserInfo` | `CommentUserInfo` | `ApprovalUserRef?` | 备注人。**官方字段名为驼峰 `commentUserInfo`**，AP5 锁定 |
| `comments[].commenttime` | `Commenttime` | `long?` | 备注时间戳 |
| `comments[].commentcontent` | `Commentcontent` | `string?` | 备注文本 |
| `comments[].commentid` | `Commentid` | `string?` | 备注 id |
| `comments[].media_id` | `MediaId` | `List<string>?` | 备注附件 id 列表 |

`process_list.node_list[]`（`ApprovalProcessNodeInfo`）与 `sub_node_list[]`（`ApprovalSubNodeInfo`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `process_list.node_list[].node_type` | `NodeType` | `int?` | `1` 审批人 / `2` 抄送人 / `3` 办理人 |
| `process_list.node_list[].sp_status` | `SpStatus` | `int?` | `1` 审批中 / `2` 同意 / `3` 驳回 / `4` 转审 / `11` 退回给指定审批人 / `12` 加签 / `13` 同意并加签 / **`14` 办理 / `15` 转交** |
| `process_list.node_list[].apv_rel` | `ApvRel` | `int?` | 多人办理方式：`1` 会签 / `2` 或签 / `3` 依次审批（与 `sp_record[].approverattr` 的 1/2 语义相反，见 §5） |
| `process_list.node_list[].sub_node_list[]` | `SubNodeList` | `List<ApprovalSubNodeInfo>?` | 子节点 |
| `sub_node_list[].userid` | `Userid` | `string?` | 处理人（单值） |
| `sub_node_list[].speech` | `Speech` | `string?` | 审批/办理意见 |
| `sub_node_list[].sp_yj` | `SpYj` | `int?` | 子节点状态，**字段名是 `sp_yj` 而非 `sp_status`**，取值 1/2/3/4/11/12/13/14/15 |
| `sub_node_list[].sptime` | `Sptime` | `long?` | 操作时间 |
| `sub_node_list[].media_ids` | `MediaIds` | `List<string>?` | 附件（复数 `media_ids`，与 `details[].media_id` 单数形态不同） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "info": {
    "sp_no": "mud-SPNO-FAKE-0001",
    "sp_name": "差旅报销",
    "sp_status": 2,
    "template_id": "mud-TEMPLATEID-FAKE-0001",
    "apply_time": 1569546000,
    "applyer": { "userid": "mud-USERID-FAKE-0001", "partyid": "2" },
    "sp_record": [
      {
        "sp_status": 2,
        "approverattr": 2,
        "details": [
          {
            "approver": { "userid": "mud-USERID-FAKE-0002" },
            "speech": "同意",
            "sp_status": 2,
            "sptime": 1569547000,
            "media_id": []
          }
        ]
      }
    ],
    "notifyer": [ { "userid": "mud-USERID-FAKE-0003" } ],
    "apply_data": {
      "contents": [
        {
          "control": "Money",
          "id": "mud-CONTROLID-FAKE-0001",
          "title": [ { "text": "报销金额", "lang": "zh_CN" } ],
          "value": { "new_money": "1200" },
          "hidden": 0
        }
      ]
    },
    "comments": [
      {
        "commentUserInfo": { "userid": "mud-USERID-FAKE-0002" },
        "commenttime": 1569547200,
        "commentcontent": "已归档",
        "commentid": "mud-COMMENTID-FAKE-0001",
        "media_id": []
      }
    ],
    "process_list": {
      "node_list": [
        {
          "node_type": 1,
          "sp_status": 2,
          "apv_rel": 1,
          "sub_node_list": [
            { "userid": "mud-USERID-FAKE-0002", "speech": "同意", "sp_yj": 2, "sptime": 1569547000, "media_ids": [] }
          ]
        }
      ]
    }
  }
}
```

**本端点注意事项**

- 响应内含申请人、审批人、抄送人、备注、金额、附件、位置等个人信息：**不得整体序列化进日志、遥测或异常消息**；需要留痕时只写 `sp_no` 与单据级 `sp_status`。
- `applyer` 与 `batch_applyer` **互斥**：只读 `Applyer` 会在批量提单场景拿到 `null`，须两者都判。
- `sp_record` 与 `process_list` 是两棵**并行**的树：前者是审批节点与分支审批人，后者是流程节点与子节点（多出 `node_type` 与 14/15 状态），不互相替代。
- 600 次/分钟；想「按 E2 的 `sp_no_list` 拉全部详情」只能宿主自己循环，与 E2 共用同一频率额度。

### E4. `GetApprovalDataAsync` — 获取审批数据（旧）

**签名**

```csharp
Task<GetApprovalDataResponse> GetApprovalDataAsync(
    [Body] GetApprovalDataRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/corp/getapprovaldata` |
| 适用应用类型 | **仅企业自建应用**（AP1b：第三方/代开发子接口不得声明本端点） |
| 官方文档 | 91530（仅自建页，本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Starttime` | `starttime` | `long?` | 是 | 开始时间（Unix 秒，**本端点是数字**，与 E2 的字符串形态相反） |
| `request.Endtime` | `endtime` | `long?` | 是 | 结束时间（Unix 秒）；需大于 `starttime`，跨度不超过 **30 天** |
| `request.NextSpnum` | `next_spnum` | `long?` | 否 | 分页游标：上一页返回的最后一个审批单号（数字形态），不填从该时间段第一张开始 |
| —（不进方法签名） | `access_token` | — | 是 | 自建应用凭证，由 `[Token]` 在 Query 注入 |

```json
{ "starttime": 1569546000, "endtime": 1569600000 }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `count` | `Count` | `int?` | 本次拉取的审批单个数，最大 100 |
| `total` | `Total` | `int?` | 时间段内总审批单个数；大于 100 时用 `next_spnum` 多次拉取 |
| `next_spnum` | `NextSpnum` | `long?` | 本次列表最后一个审批单号，作为下一次的 `next_spnum` |
| `data` | `Data` | `List<GetApprovalDataItem>?` | 审批记录列表 |

`data[]`（`GetApprovalDataItem`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `spname` | `Spname` | `string?` | 审批名称（请假/报销/自定义审批）。**官方连写拼写 `spname`**，AP5 锁定 |
| `apply_name` | `ApplyName` | `string?` | 申请人姓名 |
| `apply_org` | `ApplyOrg` | `string?` | 申请人部门 |
| `approval_name` | `ApprovalName` | `List<string>?` | 审批人姓名列表 |
| `notify_name` | `NotifyName` | `List<string>?` | 抄送人姓名列表 |
| `sp_status` | `SpStatus` | `int?` | 旧版状态：`1` 审批中 / `2` 已通过 / `3` 已驳回 / `4` 已**取消** / `6` 通过后撤销 / `10` 已支付（注释仅列此 6 值、**无 7**；`4` 的措辞与 E3 的「已撤销」不同） |
| `sp_num` | `SpNum` | `long?` | 审批单号（**数字**形态，不能当 E3 的 `sp_no` 用） |
| `mediaids` | `Mediaids` | `List<string>?` | 附件 `media_id` 列表（可用 `media/get` 获取）。**官方连写拼写 `mediaids`**，AP5 锁定 |
| `apply_time` | `ApplyTime` | `long?` | 提交时间（Unix 秒） |
| `apply_user_id` | `ApplyUserId` | `string?` | 提交者 userid |
| `expense` | `Expense` | `ApprovalExpenseData?` | 报销数据（仅报销模板） |
| `leave` | `Leave` | `ApprovalLeaveData?` | 请假数据（仅请假模板） |
| `comm` | `Comm` | `ApprovalCommData?` | 自定义模板数据（`apply_data` 为 **JSON 字符串**，需二次解析） |

`expense` / `leave` / `comm` 子结构：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `expense.expense_type` | `ExpenseType` | `int?` | `1` 差旅费 / `2` 交通费 / `3` 招待费 / `4` 其他报销 |
| `expense.reason` | `Reason` | `string?` | 报销事由 |
| `expense.item[]` | `Item` | `List<ApprovalExpenseItem>?` | 报销明细，**历史单据字段，新申请单据不再提供** |
| `expense.item[].expenseitem_type` | `ExpenseitemType` | `int?` | `1` 飞机票 / `2` 火车票 / `3` 的士费 / `4` 住宿费 / `5` 餐饮费 / `6` 礼品费 / `7` 活动费 / `8` 通讯费 / `9` 补助 / `10` 其他 |
| `expense.item[].time` | `Time` | `long?` | 发生时间（Unix 秒） |
| `expense.item[].sums` | `Sums` | `long?` | 金额，**单位元**（与发票域的「分」口径相反，勿混） |
| `expense.item[].reason` | `Reason` | `string?` | 明细事由 |
| `leave.timeunit` | `Timeunit` | `int?` | `0` 半天 / `1` 小时 |
| `leave.leave_type` | `LeaveType` | `int?` | `1` 年假 / `2` 事假 / `3` 病假 / `4` 调休假 / `5` 婚假 / `6` 产假 / `7` 陪产假 / `8` 其他 |
| `leave.start_time` / `end_time` | `StartTime` / `EndTime` | `long?` | 请假起止（Unix 秒） |
| `leave.duration` | `Duration` | `long?` | 请假时长，**单位小时** |
| `leave.reason` | `Reason` | `string?` | 请假事由 |
| `comm.apply_data` | `ApplyData` | `string?` | 单据数据（JSON 字符串）。官方近期优化过格式：**老格式为对象**（键为控件 id）、**新格式为数组**（元素含 `id/type/value/title`），本仓按传输形态以字符串承载，由调用方判断后二次解析 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "count": 1,
  "total": 1,
  "next_spnum": 1300000000000000000,
  "data": [
    {
      "spname": "差旅报销",
      "apply_name": "mud-NAME-FAKE-0001",
      "apply_org": "（占位部门）",
      "approval_name": ["mud-NAME-FAKE-0002"],
      "notify_name": [],
      "sp_status": 2,
      "sp_num": 1300000000000000000,
      "mediaids": [],
      "apply_time": 1569546000,
      "apply_user_id": "mud-USERID-FAKE-0001",
      "expense": {
        "expense_type": 1,
        "reason": "（占位事由）",
        "item": [
          { "expenseitem_type": 2, "time": 1569546000, "sums": 1200, "reason": "（占位事由）" }
        ]
      }
    }
  ]
}
```

**本端点注意事项**

- **处于下线路径**：官方明确推荐改用 E2 + E3，本端点后续不再维护。新建系统不要以 E4 为主路径。
- 分页三键是 `starttime` / `endtime` / `next_spnum`——**没有 `cursor` / `size`**（以 DTO 为准，勿按 E2 的形态套用）。
- 无法获取管理员已删除的审批单；`total > 100` 时必须靠 `next_spnum` 翻页。
- `sp_num` 是 `long`，E3 的 `sp_no` 是 `string`，两者不可互换（§5）；旧版 `sp_status` 无 `7`（已删除不出现）且 `4` 写作「已取消」。

**E4 片段**（仅自建，翻页取单号）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatWorkException
using Mud.Wechat.Work.DataModels.Approval;

// 前置：AddWechatApp（AppType = Internal）+ AddWechatWorkServices(b => b.AddApprovalApi()) 已装配；
// DI / Logging 的 using 与 E1 相同，由宿主隐式提供。
var approval = provider.GetRequiredService<IWechatWorkInternalApprovalService>();  // E4 只在自建子接口上
var starttime = 1569546000L;   // 本端点是数字时间戳，与 E2 的字符串形态相反
var endtime = 1569600000L;
var legacyNumbers = new List<long>();
long? nextSpnum = null;

do
{
    var page = await approval.GetApprovalDataAsync(
        new GetApprovalDataRequest { Starttime = starttime, Endtime = endtime, NextSpnum = nextSpnum },
        cancellationToken);

    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/corp/getapprovaldata");

    // 旧版单据含申请人姓名/部门/请假事由/金额：不得整体写入日志或遥测。
    foreach (var item in page.Data ?? new List<GetApprovalDataItem>())
    {
        if (item.SpNum is long number)
        {
            legacyNumbers.Add(number);   // sp_num 是 long，不能当 E3 的 sp_no（string）用
        }
    }

    nextSpnum = page.NextSpnum;
} while (nextSpnum is not null);
```

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `starttime` / `endtime` 的类型（E2 vs E4） | E2 是 `string?`，官方示例为 `"1569546000"`，**AP5 锁定 `typeof(string)`**；E4 是 `long?` | 把 E2 改成 `long` 直接打红 AP5；按同一封装层传参会序列化出数字，与官方字符串契约不符，可能取不到时间范围而表现为静默空结果 |
| `sp_no`（`string`）vs `sp_num`（`long`） | E1 返回、E2 列表、E3 入参都是字符串 `sp_no`；E4 的 `sp_num` 是**数字**且属旧体系 | 拿 `sp_num` 调 E3 必然失败；把 `sp_no` 解析成整数会丢字符 |
| `notifyer`（抄送人） | 官方拼写是 `notifyer`，**不是 notifier**（AP5 `JsonNameShouldBe`） | 自写 DTO 或按 `notifier` 从字典取值恒 `null`，抄送人静默丢失 |
| `commentUserInfo`（备注人） | 蛇形报文里的**驼峰**键名（AP5 锁定） | 按 `comment_user_info` 取值恒 `null` |
| `spname` / `mediaids`（E4） | 官方连写拼写（AP5 锁定），不是 `sp_name` / `media_ids` | 旧接口字段静默为 `null`，附件与审批名丢失 |
| `file_size`（附件控件） | 官方「类型为 number」与「没有可以填空字符串」**自相矛盾**；本仓以 `string?` 承载（AP5 锁定） | 改成数值类型后，官方返回空串或数字两种形态之一时反序列化失败 |
| `control` ↔ `value` 子键 | 同一个 `value` 袋承载 18 类控件，必须按 `control` 选字段（下表） | 给 Text 控件填 `new_money` 官方不报错，但表单值为空 |
| `title` / `hidden`（`contents[]`） | **仅详情（及模板详情）响应返回**，E1 请求体不带 | 提单时填充无用；从自己发出的请求里回读 `title` 恒 `null` |
| 三层 `sp_status` | 单据级 1/2/3/4/6/7/10；审批节点与分支 1/2/3/4/11/12/13；流程节点多 14/15；E4 旧版 1/2/3/4/6/10（**无 7**） | 用一张全局枚举判定会把合法值当未知（如节点级 11 退回） |
| `apv_rel` vs `approverattr` | 两个字段名不同、语义也不同：`process.node_list[].apv_rel`（请求）与 `process_list.node_list[].apv_rel`（响应）都是「`1` 会签 / `2` 或签 / `3` 依次审批」；而 `sp_record[].approverattr` 是「`1` 或签 / `2` 会签」 | 把 `approverattr` 当 `apv_rel` 读会把会签/或签判反；两者混用同一张常量表时，请求侧与响应侧的同一个 `2` 会指向相反语义 |
| `new_cursor` / `new_next_cursor` | 旧 `cursor` / `next_cursor` 官方待废弃、本仓不建模；结束条件是 `new_next_cursor` **字段缺失**，不是空串、也不是 `sp_no_list` 为空 | 按「返回条数 < `size`」判结束会提前截断；按空串判可能死循环 |
| 31 天 vs 30 天 | E2 跨度 ≤ **31 天**；E4 ≤ **30 天** | 共用一个常量会被官方拒绝 |
| 「查询用 GET」 | 本族 4 支**全部 POST**（含 `getapprovalinfo` / `getapprovaldetail` / `getapprovaldata`） | 改动词打红 AP1a/AP1b，且官方不接受 |
| E4 与 E2+E3 | E4 是旧版数据格式且已被官方标注不再维护；E2+E3 是新版主路径 | 新系统用 E4 会锁死在将下线接口上，且拿不到流程树与备注 |
| `apply_data.contents[]` 的递归（Table 控件） | `children[]` 的每项 `list` 是子控件数组；**提交时不能是空数组**，至少一条子明细，且必须包含模板中设置的全部子控件（为空时把全部子控件值设为空） | 空数组被官方拒绝；漏子控件导致整单提交失败 |
| 个人信息 | `getapprovaldetail` / `getapprovaldata` 返回申请人、审批人、请假事由、金额、附件、位置 | 整体写进日志/遥测/异常消息即泄露；SDK 不代脱敏，`WechatWorkException.RequestUri` 只处理 query 不含请求体 |

**`control` 取值 ↔ `value` 子结构对照**（单据侧 18 值，抄自 `ApprovalControlItem.Control` 注释与 `ApprovalControlValue` 属性；模板侧无 `PunchCorrection`，两清单不等宽属官方）：

| `control` 取值 | `value` 子键（JSON） | C# 属性 / 类型 | 说明与限制 |
|---|---|---|---|
| `Text` | `text` | `Text` / `string?` | 文本；内容不支持包含换行符 |
| `Textarea` | `text` | `Text` / `string?` | 多行文本与 `Text` **共用 `text` 键** |
| `Number` | `new_number` | `NewNumber` / `string?` | 官方请求与响应示例均按字符串传输（如 `"700"`） |
| `Money` | `new_money` | `NewMoney` / `string?` | 同上；金额单位见 §7 |
| `Date` | `date` | `Date` / `ApprovalDateValue?` | `type` = `day`/`hour`，`s_timestamp` 为字符串时间戳；`timezone_info` 仅非 UTC+8 时返回 |
| `Selector` | `selector` | `Selector` / `ApprovalSelectorValue?` | `type` = `single`/`multi`；`options[].key` + `options[].value`（多语言数组）；`exp_type`（`int?`）仅假勤场景出现 |
| `Contact` | `members` / `departments` | `Members` / `List<ApprovalMemberItem>?`；`Departments` / `List<ApprovalDepartmentItem>?` | 同一个 control 两个并列键：成员 `userid`+`name`、部门 `openapi_id`（字符串）+`name` |
| `Tips` | `new_tips` | `NewTips` / `ApprovalTipsContent?` | 说明文字；**提单无需赋值**（后台自动填充），仅详情返回 |
| `File` | `files` | `Files` / `List<ApprovalFileItem>?` | 一个审批单**全局仅支持 6 个附件** |
| `Table` | `children` | `Children` / `List<ApprovalTableChild>?` | 明细控件；每项 `list` 递归为子控件数组（见上表递归约束） |
| `Attendance` | `attendance` | `Attendance` / `ApprovalAttendanceValue?` | `type`：`1` 请假 / `2` 补卡 / `3` 出差 / `4` 外出 / `5` 加班；加班跨度 ≤ 7 天 |
| `Vacation` | `vacation` | `Vacation` / `ApprovalVacationValue?` | 内层 = `selector`（请假类型，固定单选）+ `attendance`（时间范围） |
| `PunchCorrection` | `punch_correction` | `PunchCorrection` / `ApprovalPunchCorrectionValue?` | **仅详情附录出现**，提单侧见 §7 |
| `Location` | `location` | `Location` / `ApprovalLocationValue?` | `latitude`/`longitude` 按字符串传输，精确 6 位小数 |
| `RelatedApproval` | `related_approval` | `RelatedApproval` / `List<ApprovalRelatedApprovalItem>?` | 响应侧回填模板名、状态、提单者、提单时间 |
| `Formula` | `formula` | `Formula` / `ApprovalFormulaValue?` | 提交表单时**无需填写**，后台自动计算 |
| `DateRange` | `date_range` | `DateRange` / `ApprovalDateRangeValue?` | `type` = `halfday`/`hour`；`new_begin`/`new_end` 为秒；`halfday` 只能取当天 00:00 或 12:00 |
| `BankAccount` | `bank_account` | `BankAccount` / `ApprovalBankAccountValue?` | **仅详情附录出现**；`account_type` 1 对公 / 2 个人 |

**这类结构不做运行时多态**：`value` 是**声明类型的超集袋**（`ApprovalControlValue` 共 18 个可空属性，序列化时 `null` 字段不写出），不是 `Dictionary`、也不是回调载荷的 `Values` 袋——AOT 源生成按声明类型序列化，宿主必须自己按 `control` 选字段、自己决定读取哪一项。SDK 不会根据 `control` 帮你分派，也不会在字段与控件类型不匹配时报错（官方亦然）。

另外四条 SDK 侧行为约束，写在这里是因为它们会改变代码写法：

- **不自动判错、不裁剪批量**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`；≤100 条与 30/31 天的上限 SDK 不代校验、不代分批。
- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改 Header）。`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但 `permanent_code` 与审批报文里的个人信息需调用方自己避免打印。
- **企业作用域是第三方/代开发的前置动作而非可选项**：令牌归属键 `Wechat.AccessToken@Corp`（一企一份，scope = `authCorpId`），调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`；在自建上下文里消费 `_ThirdParty` / `_Provider` 子接口会在 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`。
- **审批状态变化不在本族**：本族只有 HTTP 查询面；实时感知走 `sys_approval_change` 回调（`Mud.Wechat.Work.Callback`），本族 E3 只作主动查询与兜底。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径（注意：`errcode == 0` 不代表「模板必填控件都填对了」，官方在提交时即校验，填错会在本端点返回非 0） |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效/无效 | 族别失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效 | 同上 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷信号 |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号（多为误调到服务商级接口） |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」；若这些码稳定出现，说明刷新本身在失败（自建：`AgentSecret` 失效或应用被停用；第三方/代开发：`permanent_code` 失效、授权已取消或企业作用域未切换），加重试不解决。

本族各端点的专属业务错误码（含频率超限码）接口注释未留档 ⇒ 见 §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（AP1a/AP1b/AP2/AP3/AP4/AP5 + 接口与 DTO XML 注释 + 域 README 2026-10-18）；文档号照抄注释，**页面更新日期与文案未逐项核验** | 逐页渲染取文：91853 / 91816 / 91983 / 91530 与第三方 92632 / 94603 / 92634、代开发 96507 / 96509 / 96510 |
| 端点级专属错误码与频率超限码 | 四支端点的接口注释只留「600 次/分」等业务限制，**未留档任何 errcode 表**（含限流返回码） ⇒ **未逐项核验** | 官方各参数页 + 全局错误码 90455；不在 SDK 侧硬编码码表 |
| `new_money` 的金额单位 | DTO 注释只留「按字符串传输（如 `"700"`）」，**元/分不明** ⇒ **未逐项核验** | 官方 91853；按金额做判定前先核验，勿借用发票域的「分」口径 |
| `PunchCorrection` / `BankAccount` 能否在 E1 赋值 | 两类控件的注释均写「仅在获取审批申请详情的官方附录中出现」，提单附录未包含 ⇒ 可写性**未逐项核验** | 官方 91853 附录；勿在 SDK 侧加「禁止赋值」的本地闸 |
| `file_size` 的官方矛盾 | 官方在同一字段上写「类型为 number」与「没有可以填空字符串」；本仓以 `string?` 兼容两态（AP5），但官方是否真的接受空串、以及数字态是否仍返回**未逐项核验** | 官方 91983 / 91853 附件控件段 |
| `record_type` 的取值全集 | 注释只列 `1`~`9`；是否新增值、以及 2021-05-31 之前历史单的确切表现 ⇒ **未逐项核验** | 官方 91816 |
| `use_template_approver` / `process` 的组合细则 | 注释只写「为 0 时 process 必填」「为 1 时流程中不得有『申请人自选』节点」；其余非法组合由应答 `errcode` 表达，SDK 不本地拦截 ⇒ 细则**未逐项核验** | 官方 91853 |
| `batch_applyer` 的出现条件 | 注释只写「与 `applyer` 互斥」；何种提单形态会产生批量申请人（由哪个入口发起）本仓无留档 ⇒ **未逐项核验** | 官方 91983 |
| 超过批量上限（>100）的官方行为 | E2 / E4 注释只给上限数值，未说明是整批拒绝还是部分处理 ⇒ **未逐项核验** | 实测或 90454 |
| E4 的下线时间表 | 官方只写「后续将不再维护、逐步下线」，**无具体日期** ⇒ **未逐项核验** | 官方 91530 页面公告 |
| 仓内注释口径差异（README / DTO） | 三处：① 域 README §6（`README.md:207`）写 `ApprovalControlValue` 为「17 个可空字段」，实际 DTO 有 **18 个**可空属性（`Text`/`NewNumber`/`NewMoney`/`Date`/`Selector`/`Members`/`Departments`/`NewTips`/`Files`/`Children`/`DateRange`/`Vacation`/`Attendance`/`PunchCorrection`/`Location`/`RelatedApproval`/`Formula`/`BankAccount`）；② 域 README §4（`README.md:152-155`）把 `process.node_list[].apv_rel`（`1` 会签 / `2` 或签）与 `sp_record[].approverattr`（`1` 或签 / `2` 会签）并述为「`apv_rel` 两侧不一致」，实际不一致的是两个**不同字段**；③ `BatchGetApprovalNumbersRequest.Size` 的属性注释（`BatchGetApprovalNumbersRequest.cs:44`）写「用 `next_cursor` 判断是否拉完」，与同文件类级 remarks（旧 `cursor`/`next_cursor` 待废弃）及响应字段 `new_next_cursor` 不一致。本文一律按 DTO 声明取值，README 与属性注释未同批修订 | 以 [ApprovalControlValue.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/ApprovalControlValue.cs)、[ApprovalProcessNodeItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/ApprovalProcessNodeItem.cs)/[ApprovalSpRecord.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/ApprovalSpRecord.cs)、[BatchGetApprovalNumbersRequest.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/RequestModel/BatchGetApprovalNumbersRequest.cs) 为准；注释侧待修订 |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 12 支端点速览 + 两套单据体系与三套状态码的权威速览）：[Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md)
- 兄弟族：[ApprovalTemplate.md](./ApprovalTemplate.md)（模板详情 / 创建 / 更新 / 复制，`template_id` 与 `open_template_id` 的分界） · [Vacation.md](./Vacation.md)（假期配置与成员余额，单位为秒） · [ApprovalEngine.md](./ApprovalEngine.md)（按 `thirdNo` 查流程引擎，另一套单据体系）
- 审批状态回调（不在本族）：[SysApprovalChangedPayload.cs](../../../../../Src/Work/Mud.Wechat.Work.Callback/Events/Payloads/Approval/SysApprovalChangedPayload.cs)（`sys_approval_change`）
- 契约守卫源码：[WechatApprovalContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与域 README §9
- 官方页（文档号照接口注释留档，本次未重取正文）：[91853](https://developer.work.weixin.qq.com/document/path/91853) · [91816](https://developer.work.weixin.qq.com/document/path/91816) · [91983](https://developer.work.weixin.qq.com/document/path/91983) · [91530](https://developer.work.weixin.qq.com/document/path/91530)（第三方 92632 / 94603 / 92634、代开发 96507 / 96509 / 96510）
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
