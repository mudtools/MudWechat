# 审批模板（获取详情 / 创建 / 更新 / 复制到企业）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Approval` 域 · 审批模板族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkApprovalTemplateService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalTemplateService.cs) · [IWechatWorkApprovalTemplateService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalTemplateService_Internal.cs) · [IWechatWorkApprovalTemplateService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalTemplateService_Provider.cs) · [IWechatWorkApprovalTemplateService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkApprovalTemplateService_ThirdParty.cs)（目录：[Interfaces/Approval/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/)） |
| DTO 源文件 | [DataModels/Approval/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/) 与 [DataModels/Approval/RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/RequestModel/)（`RequestModel/` 仅作组织、不入命名空间，全部为 `Mud.Wechat.Work.DataModels.Approval`） |
| 契约守卫 | [WechatApprovalContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs)（AP1c 本族 1 + 5 行声明 / 去重 4 路由 + 四条 `BeNull` 反向断言 · AP2 层级：父接口恰 1、自建 2、代开发 2、第三方 1 · AP3 令牌绑定 · AP4 域内 109 型上下文登记（本族端点级 7 型）· AP5 selector 选项 `value` 两形态）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5）· 命名空间二分 [WechatInterfaceNamespaceContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3） |
| 官方文档 | 自建 91982（E1）· 97437（E2）· 97438（E3）；第三方 92631（E1）· 92630（E4）；服务商代开发 96506（E1）· 97439（E2）· 97440（E3）。文档号一律取自接口 XML 注释的 `<remarks>`；**本次未重取官方页**，页面更新日期见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 4 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AP1c `ApprovalTemplateRoutes`（1 条）+ `ApprovalTemplateDiffRoutes`（5 行声明 / 3 条唯一路由，逐接口逐动词）与四条 `BeNull`（第三方不得声明创建/更新、自建/代开发不得声明复制）· AP2 层级计数（父 1、自建 2、代开发 2、第三方 1）+ `InheritedFrom` + 注册组恒空 · AP3 令牌（含本族 4 接口在内的 16 接口逐一断言 `AccessToken` + `Query` + `access_token`）· AP4 本族 7 个端点级 DTO 登记 + 域内 `HaveCount(109)` · AP5 `ApprovalTemplateSelectorOption.Value` 为 `List<ApprovalLangTextItem>`、`ApprovalTemplateSettingSelectorOption.Value` 为 `ApprovalLangTextItem` · TO1~TO3（自建 `@Internal`、第三方/代开发 `@Corp`）· 接口与 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md) §6/§9 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承本域 2026-10-18 核验留档；凡只有官方页能支撑的项（频率以外的专属码、页面更新日期）一律进 §7 |

## 1. 这个接口族做什么

企业微信「审批」模块里**模板定义**的四支 HTTP 端点：E1 按 `template_id` 取模板名称与整套控件树（`property` + `config`），是「拿到控件 id、选项 key、假期类型 id」的唯一读入口；E2 创建模板并返回新模板 id；E3 用 `template_id` 整体替换模板内容（响应只有 `errcode`/`errmsg`）；E4 由第三方应用把服务商后台审核通过的模板复制/更新到授权企业，返回**该企业侧**的 `template_id`。

端点计数要分开看：**6 个方法声明、4 条唯一路由**。E2/E3 在自建子接口与代开发子接口上各声明一次（同路由、同签名、不同接口），E4 只在第三方子接口上声明。这是官方开放面不等宽造成的三向分叉，不是重复代码。

使用时机是**「用代码初始化或批量维护企业审批模板」**：HR/费控系统开通时建模板、服务商把审核过的模板下发到各授权企业、模板改版后整体覆盖。E1 的响应则是后续提交审批单时填 `apply_data` 的取值来源（控件 `id`、单选/多选 `options[].key`、请假 `vacation_list.item[].id`）。

**不该用本族的场景**：要提交审批单、批量取单号、查单据详情（`sp_no` 那套），走 [Approval.md](./Approval.md)；要查假期配置与成员余额走 [Vacation.md](./Vacation.md)；要按 `thirdNo` 查审批流程引擎走 [ApprovalEngine.md](./ApprovalEngine.md)。本族只定义「单据长什么样」，不产生也不查询任何单据实例。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 自建 `WechatAppType.Internal`；第三方 `WechatAppType.ThirdParty`；服务商代开发 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()`；运行期动态注册应用时 `WechatAppManager` 同样校验 |
| 必填配置属性（自建） | `CorpId` · `AgentSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动即失败，不留到首次调用） |
| 必填配置属性（第三方 / 代开发） | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，四项齐备；缺 `SuiteSecret` 同样启动期即抛（即使本族不消费套件令牌） |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键（自建） | `WechatTokenManagerKeys.InternalAccessToken`（值 `Wechat.AccessToken@Internal`），由 `IWechatWorkInternalApprovalTemplateService` 的 `[Token]` 声明；**公共父接口不声明归属键** | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；在自建应用上下文里消费第三方/代开发子接口（或反之）抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |
| 令牌归属键（第三方 / 代开发） | `WechatTokenManagerKeys.CorpAccessToken`（值 `Wechat.AccessToken@Corp`），由两个子接口的 `[Token]` 声明 | 同上 |
| 令牌注入 | `TokenInjectionMode.Query`，参数名恒为 `access_token`，**不进方法签名**（AP3 对审批模块 16 个接口逐一断言） | 每次请求 |

`access_token` 由 SDK 按当前应用上下文与 scope 自动换取、缓存并刷新，**开发者不接触令牌值**。第三方/代开发的令牌是**授权企业级**（一企一份，scope = `authCorpId`），调用前必须进入目标授权企业作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时写「应用 + 企业」两级上下文、释放时逆序还原）。`permanent_code` 是凭据，本文与示例只写参数名，不得进日志、遥测或异常消息。

一个必须说清的边界：**令牌归属校验只能区分「自建」与「非自建」**。第三方应用与代开发应用声明的是同一个归属键 `Wechat.AccessToken@Corp`，因此在第三方应用上下文里解析 `IWechatWorkProviderApprovalTemplateService` **不会**抛 `WechatTokenOwnerMismatchException`——它会照常取到企业令牌并发出请求，由官方以业务错误回应。选错接口只能靠调用方按应用类型决定，SDK 不代做运行时分派（§3、§5）。

### 2.2 权限与业务前置

以下逐条出自接口 XML 注释与 DTO 类级/属性级 `<remarks>`（官方页本次未重取）：

- **权限口径（三类通用，父接口 `remarks` 原文）**：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。自 2023-12-01 0 点起不再支持通过**系统应用 secret** 调用本域接口（存量企业暂不受影响）。
- **E1**：频率 600 次/分钟；第三方应用只能获取**本第三方应用添加的**模板详情，自建应用 Secret 只能获取**企业自建模板**的详情。
- **E2**：频率 600 次/分钟；模板名称不得与现有模板重名且长度 ≤ **40** 字符；控件名称不得与现有控件重名且长度 ≤ **40** 字符（`Attendance` 的外出/出差/加班控件 `title` **固定**为「外出」「出差」「加班」，不支持自定义）；控件说明 ≤ **80** 字符；**一个模板中只能拥有一类假勤控件类型**（`Vacation` 与 `Attendance` 二选一）；明细控件不能为空数组，且明细中不能设置假勤控件与明细控件；模板控件为必填属性时，表单中对应控件必须有值。
- **E3**：**仅能更新本应用创建的模板**；频率 600 次/分钟；模板已配置**自定义打印格式**时无法通过 API 修改（错误码 **301115**）；名称与控件名的重复性、长度限制同 E2。更新后管理后台与审批应用内更新原模板内容，**已配置的审批流程和规则不变**。
- **E4**：频率 600 次/分钟；`access_token` **决定复制/更新的目标企业**，须保证作用域准确；企业之前没有该服务商模板时执行「复制」，已有时执行「更新」（覆盖旧版本）；企业设为**条件审批**的控件若被删除，条件审批流程可能失效；调用后企业管理后台出现「第三方模板」分类，企业设置的审批规则**仅在服务商调用「提交审批申请」且使用「复用管理后台流程」模式时**生效。

### 2.3 官方开放的应用类型

E1 是**三类应用公共面**，声明在公共父接口上。E2/E3 **只对自建与代开发开放**，官方权限表对第三方应用标注「暂不支持」——两族方法分别声明在自建与代开发子接口上，第三方子接口**没有**这两个方法（AP1c 两条 `BeNull`）。E4「复制/更新模板到企业」**仅第三方**开放，自建与代开发子接口**没有**该方法（AP1c 另两条 `BeNull`）。

因此第三方应用是**补集而非子集**：注入 `IWechatWorkThirdPartyApprovalTemplateService` 得到 E1 + E4，但拿不到 E2/E3；注入自建/代开发子接口得到 E1 + E2 + E3，但拿不到 E4。AP2 把声明面钉成 1/2/2/1（父/自建/代开发/第三方）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkApprovalTemplateService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空，不可直接给最终用户） | 1（E1） | 否 |
| `IWechatWorkInternalApprovalTemplateService` | `Mud.Wechat.Work` | 企业自建应用子接口 | 2（E2、E3，另继承 E1） | 是 |
| `IWechatWorkProviderApprovalTemplateService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 2（E2、E3，另继承 E1） | 是 |
| `IWechatWorkThirdPartyApprovalTemplateService` | `Mud.Wechat.Work` | 第三方应用子接口 | 1（E4，另继承 E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Approval` / `AddApprovalApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Approval`；三个子接口的 `InheritedFrom = nameof(WechatWorkApprovalTemplateService)`（继承父接口生成实现类，避免生成器重复实现公共端点——AP2 断言）
- 端点索引：**E1** `GetTemplateDetailAsync`（获取模板详情） · **E2** `CreateTemplateAsync`（创建模板，自建 + 代开发） · **E3** `UpdateTemplateAsync`（更新模板，自建 + 代开发） · **E4** `CopyTemplateAsync`（复制/更新模板到企业，仅第三方）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，仅作契约基座）。**E2/E3 怎么选**：

| 应用类型 | 解析哪个接口 | 令牌归属键 | 官方文档 |
|---|---|---|---|
| 自建 | `IWechatWorkInternalApprovalTemplateService` | `Wechat.AccessToken@Internal` | 97437 / 97438 |
| 代开发 | `IWechatWorkProviderApprovalTemplateService`，且先 `UseCorpScope` | `Wechat.AccessToken@Corp` | 97439 / 97440 |
| 第三方 | 两个都**不**解析（官方暂不支持 E2/E3）；只有 E1 + E4 | `Wechat.AccessToken@Corp` | — |

这两个方法**不得合并成一个公共方法**：一旦合并，第三方子接口就会（直接或经继承）拿到官方未开放的端点，AP1c 的反向断言与 AP2 的 1/2/2/1 计数会同时打红；运行期则表现为「接口能调、官方报错」。同路由、不同接口，靠 DI 接口区分，不做运行时多态。

本族 7 个端点级契约类型（四对请求/响应 + `UpdateApprovalTemplateRequest`）与审批域其余类型合计 **109 型**全部登记进 `ApprovalJsonContext`（`internal`，示例不可引用；AP4 断言域内顶层类型 `HaveCount(109)` 且 `SerializerClassName` 恒为 `Approval`）。

---

## 4. 端点

### E1. `GetTemplateDetailAsync` — 获取审批模板详情

**签名**

```csharp
Task<GetApprovalTemplateDetailResponse> GetTemplateDetailAsync(
    [Body] GetApprovalTemplateDetailRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/gettemplatedetail` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/oa/gettemplatedetail?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；第三方/代开发须先进入授权企业作用域，见 §2.1） |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 请求体编码 | `application/json`，源生成序列化（`ApprovalJsonContext`，AP4 锁定登记） |
| 官方文档 | 自建 91982 · 第三方 92631 · 代开发 96506（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.TemplateId` | `template_id` | `string?` | 是 | 企业内模板实例 id。来源：模板编辑页链接、获取审批申请详情、审批状态变化回调；**较早创建的模板 id 是数字串形态**（如 `1910324946027731_1688852032423522_1808577376_15111111111`），按 `string` 承载，不要解析成整数 |
| —（不进方法签名） | `access_token` | — | 是 | 应用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

```json
{ "template_id": "mud-TEMPLATEID-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `template_names` | `TemplateNames` | `List<ApprovalLangTextItem>?` | 模板名称；配置了多语言时含中英文，默认 `zh_CN` |
| `template_content` | `TemplateContent` | `ApprovalTemplateContent?` | 模板控件信息根节点 |

`template_names[]` 与 `template_content.controls[]`：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `template_names[].text` | `Text` | `string?` | 名称文本 |
| `template_names[].lang` | `Lang` | `string?` | `zh_CN`（下划线，不是 `zh-CN`）或 `en` |
| `template_content.controls` | `Controls` | `List<ApprovalTemplateControl>?` | 控件数组；明细控件的 `children` 内部结构同 `controls` |
| `controls[].property` | `Property` | `ApprovalTemplateControlProperty?` | 控件基础属性，见下表 |
| `controls[].config` | `Config` | `ApprovalTemplateControlConfig?` | 控件配置超集，按 `control` 取用其中一项，见下表 |

`controls[].property`（请求侧 E2/E3 共用同一类型）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `control` | `Control` | `string?` | 控件类型，模板侧共 **17 值**（无 `PunchCorrection`）；取值全集与单据侧 18 值的差异见 [Approval.md §5](./Approval.md#5-陷阱与易混对照) |
| `id` | `Id` | `string?` | 控件 id；较早模板为数字串形态，仍是 `string` |
| `title` | `Title` | `List<ApprovalLangTextItem>?` | 控件名称（多语言，默认 `zh_CN`） |
| `placeholder` | `Placeholder` | `List<ApprovalLangTextItem>?` | 控件说明（向申请者展示，多语言） |
| `require` | `Require` | `int?` | `1` 必填 / `0` 非必填。**是 `int` 不是 `bool`** |
| `un_print` | `UnPrint` | `int?` | **语义反转**：`1` 不参与打印 / `0` 参与打印 |

`controls[].config` 的 11 个可空字段（响应侧超集；`null` 字段不写出）：

| `config` 键 | 属性（C#） | 类型 | 适用 `control` 与子字段 |
|---|---|---|---|
| `date` | `Date` | `ApprovalTemplateDateConfig?` | `Date`；`type` = `day`（日期）/ `hour`（日期+时间） |
| `selector` | `Selector` | `ApprovalTemplateSelectorConfig?` | `Selector`；`type` = `single`/`multi`、`options[]`、`op_relations[]`，详见下 |
| `contact` | `Contact` | `ApprovalTemplateContactConfig?` | `Contact`；`type` = `single`/`multi`、`mode` = `user`（成员）/`department`（部门） |
| `table` | `Table` | `ApprovalTemplateTableConfig?` | `Table`；`children[]`（元素同 `controls[]`），明细递归 |
| `attendance` | `Attendance` | `ApprovalTemplateAttendanceConfig?` | `Attendance`（出差/加班/外出）；`type`（**响应侧** 1 请假 / 3 出差 / 4 外出 / 5 加班）、`date_range` |
| `vacation_list` | `VacationList` | `ApprovalVacationListConfig?` | `Vacation`（请假）；`item[]`：`id`（`int?`，发起申请时为请假控件赋值的选项 key）+ `name`（多语言数组） |
| `tips` | `Tips` | `ApprovalTipsContent?` | `Tips`；`tips_content[]` 富文本，详见下 |
| `file` | `File` | `ApprovalTemplateFileConfig?` | `File`；`is_only_photo` = `1` 仅拍照 / `0` 否 |
| `location` | `Location` | `ApprovalTemplateLocationConfig?` | `Location`；`distance` 支持 `100`/`200`/`300` |
| `related_approval` | `RelatedApproval` | `ApprovalTemplateRelatedApprovalConfig?` | `RelatedApproval`；`template_id` 是**单数键、数组值**（可关联的模板 id 列表；不填表示可关联全部模板） |
| `date_range` | `DateRange` | `ApprovalTemplateDateRangeConfig?` | `DateRange`（时长控件）或 `attendance.date_range`；`type` = `hour`（精确到分钟）/ `halfday`（上午/下午） |

`config.selector`（响应侧）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `selector.type` | `Type` | `string?` | `single` 单选 / `multi` 多选 |
| `selector.options[].key` | `Key` | `string?` | 选项唯一 id；是提交审批单时为单选/多选控件赋值的取值 |
| `selector.options[].value` | `Value` | `List<ApprovalLangTextItem>?` | **响应侧 `value` 是数组**（多语言，如 `[{"text":"选项1","lang":"zh_CN"}]`）；AP5 锁定 |
| `selector.op_relations[].key` | `Key` | `string?` | 关联的选项 key |
| `selector.op_relations[].relation_list[].related_control_id` | `RelatedControlId` | `string?` | 被关联的控件 id |
| `selector.op_relations[].relation_list[].action` | `Action` | `int?` | 关联动作：`1` 关联控件显示/隐藏状态 |

`config.tips`（`ApprovalTipsContent`，与单据侧 `new_tips` 共用结构）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `tips.tips_content[].lang` | `Lang` | `string?` | `zh_CN` / `en` |
| `tips.tips_content[].text.sub_text[].type` | `Type` | `int?` | `1` 纯文本 / `2` 链接 |
| `tips.tips_content[].text.sub_text[].content.plain_text.content` | `Content` | `string?` | 纯文本内容 |
| `tips.tips_content[].text.sub_text[].content.link.title` / `.url` | `Title` / `Url` | `string?` | 链接标题 / URL（创建/更新场景 URL ≤ 600 字符） |

`tips` 的官方限制（DTO 类级 `<remarks>`）：纯文本+链接标题总长度 ≤ **800** 字符；每段说明文字**只支持一个链接**。

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "template_names": [ { "text": "差旅报销（占位）", "lang": "zh_CN" } ],
  "template_content": {
    "controls": [
      {
        "property": {
          "control": "Selector",
          "id": "mud-CONTROLID-FAKE-0001",
          "title": [ { "text": "费用类型", "lang": "zh_CN" } ],
          "placeholder": [ { "text": "请选择费用类型", "lang": "zh_CN" } ],
          "require": 1,
          "un_print": 0
        },
        "config": {
          "selector": {
            "type": "single",
            "options": [
              { "key": "mud-OPTIONKEY-FAKE-0001", "value": [ { "text": "差旅费", "lang": "zh_CN" } ] }
            ],
            "op_relations": [
              {
                "key": "mud-OPTIONKEY-FAKE-0001",
                "relation_list": [ { "related_control_id": "mud-CONTROLID-FAKE-0002", "action": 1 } ]
              }
            ]
          }
        }
      }
    ]
  }
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                            // IWechatWorkInternalApprovalTemplateService / AddWechatWorkServices
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

// ③ 解析自建子接口（父接口 IsAbstract，不可注入）。
var templates = provider.GetRequiredService<IWechatWorkInternalApprovalTemplateService>();

// ④ 业务输入在段内声明：模板 id 来自模板编辑页链接或审批回调。
var templateId = "mud-TEMPLATEID-FAKE-0001";

var response = await templates.GetTemplateDetailAsync(
    new GetApprovalTemplateDetailRequest { TemplateId = templateId },
    cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!response.IsSuccess)
{
    logger.LogError("获取审批模板详情失败 {ErrorCode}: {ErrorMessage}", response.ErrorCode, response.ErrorMessage);
    throw new InvalidOperationException($"获取审批模板详情失败：{response.ErrorCode}");
}

// ⑥ 模板定义含成员/部门/金额等业务配置：只按需取控件 id 与选项 key，不整体写日志。
var controls = response.TemplateContent?.Controls ?? new List<ApprovalTemplateControl>();
var selectorValue = (List<ApprovalLangTextItem>?)null;
foreach (var control in controls)
{
    if (control.Property?.Control == "Selector")
    {
        // 响应侧 options[].value 是数组（多语言）；请求侧是单对象，二者不能互抄（§5）。
        selectorValue = control.Config?.Selector?.Options?[0].Value;
    }
}
```

**本端点注意事项**

- 单次频率上限 600 次/分钟；SDK 不做本地限流，超限由 `errcode` 表达。
- **只读单模板**：请求体只有 `template_id`，没有批量形态；要列模板只能由宿主自行维护 id 清单。
- 第三方应用只能取**自己添加的**模板；自建 Secret 只能取**企业自建**模板。取不到时先怀疑可见面，而不是重试。
- `config` 是**超集袋**：同一个类承载 11 类配置，只有与 `control` 对应的那一项非空。SDK 不分派、不校验，调用方自己按 `control` 取字段。
- `table` 的 `children` 与顶层 `controls` 同构，可多层嵌套；解析时须递归，不能只读第一层。

### E2. `CreateTemplateAsync` — 创建审批模板

**签名**（同名方法、同路由，分别声明在两个子接口上）

```csharp
// 自建应用：IWechatWorkInternalApprovalTemplateService
Task<CreateApprovalTemplateResponse> CreateTemplateAsync(
    [Body] CreateApprovalTemplateRequest request,
    CancellationToken cancellationToken = default);

// 服务商代开发：IWechatWorkProviderApprovalTemplateService
Task<CreateApprovalTemplateResponse> CreateTemplateAsync(
    [Body] CreateApprovalTemplateRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/approval/create_template` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建应用（解析 `IWechatWorkInternalApprovalTemplateService`）· 服务商代开发（解析 `IWechatWorkProviderApprovalTemplateService`）；**第三方官方标注暂不支持** |
| 接口归属 | 同一路由两条方法，靠 DI 接口区分；不得合并（§3） |
| 官方文档 | 自建 97437 · 代开发 97439（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.TemplateName` | `template_name` | `List<ApprovalLangTextItem>?` | 是 | 模板名称数组。**JSON 键是单数、值是数组**；不得与现有模板重名，名称 ≤ **40** 字符 |
| `request.TemplateContent` | `template_content` | `ApprovalTemplateSettingContent?` | 是 | 控件设置根节点，由多个表单控件及其内容组成 |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入 |

`template_content.controls[]` 及控件属性：

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `TemplateContent.Controls` | `controls` | `List<ApprovalTemplateSettingControl>?` | 未标注 | 控件数组，排列顺序与管理端展示一致；明细 `children` 内部同构 |
| `Controls[].Property` | `property` | `ApprovalTemplateControlProperty?` | 未标注 | 控件基础属性，见下表（与 E1 响应侧**同类型**） |
| `Controls[].Config` | `config` | `ApprovalTemplateSettingControlConfig?` | 未标注 | 控件配置超集；文本/多行文本、数字、金额、收款账户、请假控件**无需填写** |
| `Property.Control` | `control` | `string?` | 未标注 | 控件类型（模板侧 17 值，无 `PunchCorrection`），见 [Approval.md §5](./Approval.md#5-陷阱与易混对照) |
| `Property.Id` | `id` | `string?` | 未标注 | 控件 id，字符串（老模板为数字串） |
| `Property.Title` | `title` | `List<ApprovalLangTextItem>?` | 未标注 | 控件名称；≤ **40** 字符、不得与现有控件重名；`Attendance` 的 `title` 固定为「外出」「出差」「加班」 |
| `Property.Placeholder` | `placeholder` | `List<ApprovalLangTextItem>?` | 未标注 | 控件说明；≤ **80** 字符 |
| `Property.Require` | `require` | `int?` | 未标注 | `1` 必填 / `0` 非必填 |
| `Property.UnPrint` | `un_print` | `int?` | 未标注 | `1` 不参与打印 / `0` 参与打印（**语义反转**） |

控件配置（请求侧 10 个可空字段；与响应侧同名但类型不同，见 §5）：

| `config` 键 | 属性（C#） | 类型 | 适用 `control` 与请求侧差异 |
|---|---|---|---|
| `date` | `Date` | `ApprovalTemplateDateConfig?` | `Date`；`type` = `day`/`hour` |
| `selector` | `Selector` | `ApprovalTemplateSettingSelectorConfig?` | `Selector`；`type` + `options[]`；**请求侧 `options[].value` 是单对象** |
| `contact` | `Contact` | `ApprovalTemplateContactConfig?` | `Contact`；`type` = `single`/`multi`、`mode` = `user`/`department` |
| `tips` | `Tips` | `ApprovalTipsContent?` | `Tips`；`tips_content[]` 富文本（结构同 E1） |
| `file` | `File` | `ApprovalTemplateFileConfig?` | `File`；`is_only_photo` |
| `table` | `Table` | `ApprovalTemplateSettingTableConfig?` | `Table`；**请求侧多 `print_format`**（`0` 合并一行 / `1` 拆分多行）+ `children[]`（不能为空数组，不能含假勤/明细控件） |
| `attendance` | `Attendance` | `ApprovalTemplateSettingAttendanceConfig?` | `Attendance`；**请求侧 `type` 仅 3 出差 / 4 外出 / 5 加班**，另带 `date_range` |
| `location` | `Location` | `ApprovalTemplateLocationConfig?` | `Location`；`distance` = `100`/`200`/`300` |
| `related_approval` | `RelatedApproval` | `ApprovalTemplateRelatedApprovalConfig?` | `RelatedApproval`；`template_id` 单数键、数组值 |
| `date_range` | `DateRange` | `ApprovalTemplateDateRangeConfig?` | `DateRange` 或 `attendance.date_range`；`type` = `hour`/`halfday`，**请求侧另可设** `official_holiday`（`0` 自然日 / `1` 工作日）与 `perday_duration`（一天的秒数，> 0 且 ≤ 86400） |

**请求侧 `selector.options[]`（与 E1 响应侧的形态差异是本族第一坑，AP5 锁定）**：

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Selector.Options[].Key` | `key` | `string?` | 未标注 | 选项唯一 id |
| `Selector.Options[].Value` | `value` | `ApprovalLangTextItem?` | 未标注 | **请求侧 `value` 是单对象**（如 `{"text":"选项1","lang":"zh_CN"}`），不是数组 |

> 表中「未标注」表示接口与 DTO 注释均未逐项给出官方必填口径，本文不推断（§7）。

```json
{
  "template_name": [ { "text": "差旅报销（占位）", "lang": "zh_CN" } ],
  "template_content": {
    "controls": [
      {
        "property": {
          "control": "Selector",
          "id": "mud-CONTROLID-FAKE-0001",
          "title": [ { "text": "费用类型", "lang": "zh_CN" } ],
          "placeholder": [ { "text": "请选择费用类型", "lang": "zh_CN" } ],
          "require": 1,
          "un_print": 0
        },
        "config": {
          "selector": {
            "type": "single",
            "options": [
              { "key": "mud-OPTIONKEY-FAKE-0001", "value": { "text": "差旅费", "lang": "zh_CN" } }
            ]
          }
        }
      }
    ]
  }
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `template_id` | `TemplateId` | `string?` | 创建成功后返回的模板 id；是后续 E3、提交审批单的 `template_id` |

```json
{ "errcode": 0, "errmsg": "ok", "template_id": "mud-TEMPLATEID-FAKE-0002" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;   // IWechatAppContextSwitcher / UseCorpScope
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;       // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.Approval;

// 前置：AddWechatApp（AppType = Provider）+ AddWechatWorkServices(b => b.AddApprovalApi()) 已装配。
// 自建应用把下面的 Provider 子接口换成 IWechatWorkInternalApprovalTemplateService，并去掉 UseCorpScope 段。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var providerTemplates = provider.GetRequiredService<IWechatWorkProviderApprovalTemplateService>();
var authCorpId = config["Wechat:AuthCorpId"]!;
var permanentCode = config["Wechat:PermanentCode"]!;   // 凭据：只写参数名，不得进日志

// 模板名与控件名是业务配置：示例全部使用占位值。
var templateName = new List<ApprovalLangTextItem>
{
    new ApprovalLangTextItem { Text = "差旅报销（占位）", Lang = "zh_CN" },
};
var settingContent = new ApprovalTemplateSettingContent
{
    Controls = new List<ApprovalTemplateSettingControl>
    {
        new ApprovalTemplateSettingControl
        {
            Property = new ApprovalTemplateControlProperty
            {
                Control = "Selector",
                Id = "mud-CONTROLID-FAKE-0001",
                Title = new List<ApprovalLangTextItem>
                {
                    new ApprovalLangTextItem { Text = "费用类型", Lang = "zh_CN" },
                },
                Require = 1,
                UnPrint = 0,
            },
            Config = new ApprovalTemplateSettingControlConfig
            {
                Selector = new ApprovalTemplateSettingSelectorConfig
                {
                    Type = "single",
                    Options = new List<ApprovalTemplateSettingSelectorOption>
                    {
                        new ApprovalTemplateSettingSelectorOption
                        {
                            Key = "mud-OPTIONKEY-FAKE-0001",
                            // 请求侧 value 是单对象；响应侧同名字段是数组，不能互抄。
                            Value = new ApprovalLangTextItem { Text = "差旅费", Lang = "zh_CN" },
                        },
                    },
                },
            },
        },
    },
};

using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var created = await providerTemplates.CreateTemplateAsync(
        new CreateApprovalTemplateRequest { TemplateName = templateName, TemplateContent = settingContent },
        cancellationToken);

    // 创建模板响应的 template_id 是唯一业务字段；判错不能只看 HTTP 200。
    WechatWorkException.ThrowIfFailed(created, "/cgi-bin/oa/approval/create_template");
    string? newTemplateId = created.TemplateId;
}
```

**本端点注意事项**

- 单次频率上限 600 次/分钟；无批量形态（一次一个模板）。
- **同一路由两条方法**：自建解析 Internal 子接口、代开发解析 Provider 子接口。两者签名看起来一样，但接口不同；合并成一个方法会让第三方拿到官方未开放的端点（§3）。
- 第三方应用**不得**调用本端点（官方暂不支持）；第三方上下文里解析 Provider 子接口不会 fail-fast，只会由官方 `errcode` 表达。
- 一个模板只能有一类假勤控件：`Vacation` 与 `Attendance` 二选一；`Attendance` 的 `title` 固定为「外出」「出差」「加班」。
- 明细控件 `children` 不能为空数组，且明细内不能放假勤控件或明细控件；这条 SDK 不本地拦截。
- 模板正文可能包含成员、部门、金额等业务配置：示例数据全部占位，实际报文**不得整体写进日志、遥测或异常消息**。

### E3. `UpdateTemplateAsync` — 更新审批模板

**签名**（同名方法、同路由，分别声明在两个子接口上）

```csharp
// 自建应用：IWechatWorkInternalApprovalTemplateService
Task<WechatWorkResponse> UpdateTemplateAsync(
    [Body] UpdateApprovalTemplateRequest request,
    CancellationToken cancellationToken = default);

// 服务商代开发：IWechatWorkProviderApprovalTemplateService
Task<WechatWorkResponse> UpdateTemplateAsync(
    [Body] UpdateApprovalTemplateRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/approval/update_template` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建应用（`IWechatWorkInternalApprovalTemplateService`）· 服务商代开发（`IWechatWorkProviderApprovalTemplateService`）；**第三方官方标注暂不支持** |
| 接口归属 | 同一路由两条方法，靠 DI 接口区分；不得合并（§3） |
| 官方文档 | 自建 97438 · 代开发 97440（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.TemplateId` | `template_id` | `string?` | 是 | 要更新的企业侧模板 id；**只能更新本应用创建的模板** |
| `request.TemplateName` | `template_name` | `List<ApprovalLangTextItem>?` | 是 | 模板名称数组；规则同 E2（≤ 40 字符、不重名） |
| `request.TemplateContent` | `template_content` | `ApprovalTemplateSettingContent?` | 是 | 新的控件设置，结构与 E2 的 `template_content` **完全相同**（控件 + 请求侧配置超集） |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入 |

`TemplateContent` 的 `controls[]` / `property` / `config` 逐字段说明与 E2 一致，不重复；唯二差异是多了 `template_id`，以及响应**没有业务负载**。

```json
{
  "template_id": "mud-TEMPLATEID-FAKE-0002",
  "template_name": [ { "text": "差旅报销（占位·修订）", "lang": "zh_CN" } ],
  "template_content": {
    "controls": [
      {
        "property": {
          "control": "Text",
          "id": "mud-CONTROLID-FAKE-0001",
          "title": [ { "text": "事由", "lang": "zh_CN" } ],
          "placeholder": [ { "text": "请填写事由", "lang": "zh_CN" } ],
          "require": 1,
          "un_print": 0
        }
      }
    ]
  }
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（返回类型就是裸 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

```json
{ "errcode": 0, "errmsg": "ok" }
```

**调用示例**

与 E2 同一段已经覆盖「创建 → 更新」的连续调用；更新差异只有三处：换成 `UpdateApprovalTemplateRequest`、补 `template_id`、响应只判错不取业务字段：

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Approval;

// 前置同 E2；此处假定 templateName / settingContent / newTemplateId 已在同一作用域内构造。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var internalTemplates = provider.GetRequiredService<IWechatWorkInternalApprovalTemplateService>();
var templateName = new List<ApprovalLangTextItem>
{
    new ApprovalLangTextItem { Text = "差旅报销（占位·修订）", Lang = "zh_CN" },
};
var settingContent = new ApprovalTemplateSettingContent
{
    Controls = new List<ApprovalTemplateSettingControl>
    {
        new ApprovalTemplateSettingControl
        {
            Property = new ApprovalTemplateControlProperty
            {
                Control = "Text",
                Id = "mud-CONTROLID-FAKE-0001",
                Title = new List<ApprovalLangTextItem>
                {
                    new ApprovalLangTextItem { Text = "事由", Lang = "zh_CN" },
                },
                Require = 1,
                UnPrint = 0,
            },
        },
    },
};
var templateId = "mud-TEMPLATEID-FAKE-0002";

var updated = await internalTemplates.UpdateTemplateAsync(
    new UpdateApprovalTemplateRequest
    {
        TemplateId = templateId,
        TemplateName = templateName,
        TemplateContent = settingContent,
    },
    cancellationToken);

// 响应只有 errcode/errmsg：不判错就完全看不出更新是否生效。
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/oa/approval/update_template");
```

**本端点注意事项**

- **响应只有 `errcode`/`errmsg`**：返回类型是 `WechatWorkResponse`，没有模板 id、没有变更摘要；必须判 `IsSuccess` / `ThrowIfFailed`。
- **更新是整体替换 `template_content`**：不是增量补丁。未在请求中出现的控件不会保留。
- 已配置**自定义打印格式**的模板不能经 API 修改，错误码 **301115**（接口注释留档）。
- 只能更新**本应用创建的**模板；第三方应用官方未开放本端点。
- 更新后已配置的审批流程和规则**不变**（模板内容变了，流程配置不动）。

### E4. `CopyTemplateAsync` — 复制/更新模板到企业

**签名**

```csharp
Task<CopyApprovalTemplateResponse> CopyTemplateAsync(
    [Body] CopyApprovalTemplateRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/approval/copytemplate` |
| 凭据参数 | `access_token`（Query 注入；**该令牌决定目标企业**，必须先 `UseCorpScope`） |
| 适用应用类型 | **仅第三方应用**（AP1c：自建/代开发子接口不得声明本端点） |
| 官方文档 | 92630（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.OpenTemplateId` | `open_template_id` | `string?` | 是 | **服务商后台**模板的唯一 id（服务商后台「应用管理 - 审批模板」编辑页 / 审批状态回调）；不是企业侧 `template_id` |
| —（不进方法签名） | `access_token` | — | 是 | 授权企业级凭证；作用域里的 `authCorpId` 决定复制/更新到哪家企业 |

```json
{ "open_template_id": "mud-OPENTEMPLATEID-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `template_id` | `TemplateId` | `string?` | **目标企业侧**的模板 id；同一个 `open_template_id` 复制到不同企业会得到不同 `template_id` |

```json
{ "errcode": 0, "errmsg": "ok", "template_id": "mud-TEMPLATEID-FAKE-0003" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Approval;

// 前置：AddWechatApp（AppType = ThirdParty）+ AddWechatWorkServices(b => b.AddApprovalApi()) 已装配。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var thirdPartyTemplates = provider.GetRequiredService<IWechatWorkThirdPartyApprovalTemplateService>();
var authCorpId = config["Wechat:AuthCorpId"]!;           // 决定目标企业
var permanentCode = config["Wechat:PermanentCode"]!;     // 凭据：只写参数名，不得进日志
var openTemplateId = "mud-OPENTEMPLATEID-FAKE-0001";     // 服务商后台模板 id

using (switcher.UseCorpScope("thirdparty-a", authCorpId, permanentCode))
{
    var copied = await thirdPartyTemplates.CopyTemplateAsync(
        new CopyApprovalTemplateRequest { OpenTemplateId = openTemplateId },
        cancellationToken);

    WechatWorkException.ThrowIfFailed(copied, "/cgi-bin/oa/approval/copytemplate");

    // 企业侧 id：同一服务商模板复制到不同企业会不同，不能拿 open_template_id 顶替。
    string? enterpriseTemplateId = copied.TemplateId;
}
```

**本端点注意事项**

- **「复制」与「更新」是同一个端点**：企业没有该服务商模板时复制，已有时覆盖旧版本。重复调用会**覆盖**企业在用版本，不是幂等无操作。
- `access_token` 决定目标企业：必须 `UseCorpScope(appKey, authCorpId, permanentCode)`，作用域错就是往错的企业复制。
- 企业设为**条件审批**的控件若在新版本中被删除，条件审批流程可能失效；这是官方留档的业务风险，SDK 不预检。
- 自建/代开发子接口**没有**本方法（AP1c 两条 `BeNull`）；第三方应用是本端点的唯一入口。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E2/E3 的自建版与代开发版（同路由两方法） | 两接口**同名、同路由、同签名**：`IWechatWorkInternalApprovalTemplateService` vs `IWechatWorkProviderApprovalTemplateService`；第三方两者都没有 | 合并成一个公共方法 ⇒ 第三方拿到官方未开放端点（AP1c/AP2 打红）；第三方误解析 Provider 接口时**令牌归属校验不会拦**（两者同为 `@Corp`），只会在官方侧报错 |
| selector 选项 `value` 两形态（AP5） | E1 响应：`ApprovalTemplateSelectorOption.Value` 是 `List<ApprovalLangTextItem>`（数组）；E2/E3 请求：`ApprovalTemplateSettingSelectorOption.Value` 是单个 `ApprovalLangTextItem`（对象） | 把响应 JSON 直接回填成请求体（或反之）⇒ 官方按空值/错值处理，选项静默丢失；手写报文时数组写成对象是最常见错法 |
| `template_id` vs `open_template_id` | `template_id` 是企业内模板实例（E1/E2/E3 与提单）；`open_template_id` 是**服务商后台**模板 id（仅 E4） | 拿 `open_template_id` 调 E1/E3 取不到；E4 返回的 `template_id` 才是企业侧 id，且一企业一份 |
| `template_name` / `template_names` | **键名单数、值是数组**（`List<ApprovalLangTextItem>`）；响应按复数 `template_names` | 按字符串传 `template_name` 会被官方拒；按复数键读请求侧恒 `null` |
| `related_approval.template_id` | 键名单数、值是**模板 id 数组** | 写成单字符串或改键名 ⇒ 关联审批单配置失效 |
| `template_content` 两侧不同类型 | 响应 `ApprovalTemplateContent` / `ApprovalTemplateControl` / `ApprovalTemplateControlConfig`；请求 `ApprovalTemplateSettingContent` / `...SettingControl` / `...SettingControlConfig`；**JSON 键同名** | 响应对象不能直接赋给请求属性（编译期类型不同）；按 JSON 手抄会漏掉请求侧 `table.print_format` 或响应侧 `selector.op_relations` |
| `un_print` | `1` = **不**参与打印、`0` = 参与打印（官方字段名即 `un_print`） | 按字段名直觉取反 ⇒ 关键字段不打印 |
| `require` | 是 `int?`（1/0），不是 `bool` | 传 `true`/`false` 序列化失败或官方不识别 |
| 控件 `id` | 是 `string?`；较早创建的模板为数字串 | 用 `int`/`long` 建模会丢精度或反序列化失败 |
| `attendance.type` 两侧不等宽 | 响应侧注释 1 请假 / 3 出差 / 4 外出 / 5 加班；请求侧只允许 3/4/5；请假模板走 `Vacation` + `vacation_list` | 在创建请求里给 `attendance.type` 填 1 ⇒ 与「请假控件」语义冲突；请假模板要配 `Vacation` 且 `config` 留空 |
| `vacation_list` 只在响应侧 | 请求侧 `ApprovalTemplateSettingControlConfig` **没有** `vacation_list`；请假控件无需 config | 想「按响应结构回写请求」会找不到该属性；强塞到错误层级不会生效 |
| `table.print_format` 只在请求侧 | 响应侧 `ApprovalTemplateTableConfig` 只有 `children`（官方全字段示例里的 `stat_field` 未建模，DTO 明确说明） | 从响应读 `print_format` 恒 `null`；以为响应没返回就是没配置 |
| `config` 是超集袋 | 响应 11 个可空字段、请求 10 个（差 `vacation_list`）；同一 `config` 键在不同控件下含义不同 | 按 `control` 以外的方式取值会拿到 `null` 或取错配置；SDK 不替你做分派 |
| 复制 ≠ 幂等 | E4 是「复制或更新」二义合一：企业已有该模板时**覆盖旧版本** | 当幂等接口重复调用会静默覆盖企业在用版本；删除条件审批控件还会让条件流程失效 |
| 令牌归属只能区分自建/非自建 | 自建 `@Internal` vs 第三方/代开发 `@Corp`；第三方与代开发之间**没有**归属闸 | 以为「选错 Provider/ThirdParty 会抛 `WechatTokenOwnerMismatchException`」⇒ 实际请求已发出，只能靠官方 `errcode` 发现 |

另外四条 SDK 侧行为约束，写在这里是因为它们会改变代码写法：

- **不自动判错、不裁剪**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`；40/80 字符、假勤控件数量、明细非空这些规则 SDK 不代校验。
- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改 Header）。`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但 `permanent_code` 与模板正文（成员/部门/金额）需调用方自己避免打印。
- **企业作用域是第三方/代开发的前置动作而非可选项**：令牌归属键 `Wechat.AccessToken@Corp`（一企一份，scope = `authCorpId`），调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`；在自建上下文里消费 `_ThirdParty` / `_Provider` 子接口会抛 `WechatTokenOwnerMismatchException`。
- **模板正文是业务配置**：`template_content` 可含成员、部门、金额、说明文字与链接，示例一律用占位值；落日志时只写 `template_id` / 控件数量级别，不写正文。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径（不代表模板语义一定符合预期，如控件配置被官方静默忽略） |
| 301115 | —（本仓未建常量） | 模板已配置自定义打印格式，不支持 API 修改 | **仅 E3**；接口 XML 注释留档，出现即应停用更新路径或先移除自定义打印格式 |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效/无效 | 族别失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效 | 同上 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷信号 |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号（多为误调到服务商级接口） |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」；若这些码稳定出现，说明刷新本身在失败（自建：`AgentSecret` 失效或应用被停用；第三方/代开发：`permanent_code` 失效、授权已取消或企业作用域未切换），加重试不解决。

本族各端点的其它专属业务错误码（含频率超限码）接口注释未留档 ⇒ 见 §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（AP1c/AP2/AP3/AP4/AP5 + TO1~TO3 + 接口与 DTO XML 注释 + 域 README 2026-10-18）；文档号照抄注释，**页面更新日期与文案未逐项核验** | 逐页渲染取文：91982 / 97437 / 97438 / 92631 / 92630 / 96506 / 97439 / 97440 |
| 嵌套控件字段的官方必填性 | `controls`、`property`、`config` 及其子字段在接口与 DTO 注释中**均未标注**官方必填，本文参数表写「未标注」而不推断 | 官方 97437 / 97439 参数表；若要落成本地校验，先逐项核验 |
| 请求侧 `selector` 是否支持 `op_relations` | 响应侧 `ApprovalTemplateSelectorConfig` 有 `op_relations`，请求侧 `ApprovalTemplateSettingSelectorConfig` **没有**该属性 ⇒ 官方创建/更新是否接受控件关联**未逐项核验** | 官方 97437 / 97438；如需支持须同批改 DTO 与 AP4 类型清单 |
| 响应侧明细 `stat_field` | `ApprovalTemplateTableConfig` 类级注释明说官方全字段示例含 `stat_field` 数组但无参数说明，**本模型不建模**（反序列化忽略） | 官方 91982 / 92631 / 96506 明细控件段 |
| 明细 `print_format` 的响应形态 | 请求侧 `ApprovalTemplateSettingTableConfig.PrintFormat` 有定义，响应侧无对应字段；官方是否在响应另给打印格式字段**未逐项核验** | 官方 91982 与 97437 明细控件段 |
| `attendance.type = 1` 的归属 | 响应侧注释把 1 写作「请假」，但请假模板实际由 `Vacation` + `vacation_list` 承载；两套口径如何对应**未逐项核验** | 官方 91982 假勤控件段 |
| 各端点频率以外的专属错误码 | 接口注释只留「600 次/分钟」与 E3 的 301115，**未留档其它 errcode 表**（含限流返回码） ⇒ **未逐项核验** | 官方各参数页 + 全局错误码 90455；不在 SDK 侧硬编码码表 |
| 超过名称/说明长度上限时的官方行为 | 注释只给 40/80 字符上限，未说明是整单拒绝还是截断 ⇒ **未逐项核验** | 官方 97437 / 97438 参数说明 |
| E4「复制/更新」的判定时点 | 注释只写「企业之前未有则复制、已有则更新」，未说明判定依据是企业内是否存在同源模板还是同 `open_template_id` ⇒ **未逐项核验** | 官方 92630 |
| 仓内注释口径差异（源码侧） | `ApprovalTemplateControl.cs:23` 写「有配置信息的控件类型：Date、Selector、Contact、Table、Attendance、Vacation、Tips」共 7 类，而 `ApprovalTemplateControlConfig.cs` 实际声明 **11** 个配置字段（另含 `file`/`location`/`related_approval`/`date_range`）；本文按字段声明取 11 类，注释未同批修订 | 以 [ApprovalTemplateControlConfig.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/ApprovalTemplateControlConfig.cs) 为准；注释侧待修订 |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 12 支端点速览 + 控件体系与开放面速览）：[Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md)
- 兄弟族：[Approval.md](./Approval.md)（审批申请数据：提交 / 批量取单号 / 详情 / 旧版数据；控件 `control` 取值 ↔ `value` 子结构对照表在其 §5） · [ApprovalEngine.md](./ApprovalEngine.md)（按 `thirdNo` 查审批流程引擎）
- 兄弟族·假期配置与成员余额：[Vacation.md](./Vacation.md)
- 审批状态回调（不在本族）：[SysApprovalChangedPayload.cs](../../../../../Src/Work/Mud.Wechat.Work.Callback/Events/Payloads/Approval/SysApprovalChangedPayload.cs)（`sys_approval_change`，`Mud.Wechat.Work.Callback` 包）
- 契约守卫源码：[WechatApprovalContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) · [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与域 README §9
- 官方页（文档号照接口注释留档，本次未重取正文）：[91982](https://developer.work.weixin.qq.com/document/path/91982) · [97437](https://developer.work.weixin.qq.com/document/path/97437) · [97438](https://developer.work.weixin.qq.com/document/path/97438) · [92631](https://developer.work.weixin.qq.com/document/path/92631) · [92630](https://developer.work.weixin.qq.com/document/path/92630) · [96506](https://developer.work.weixin.qq.com/document/path/96506) · [97439](https://developer.work.weixin.qq.com/document/path/97439) · [97440](https://developer.work.weixin.qq.com/document/path/97440)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
