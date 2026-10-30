# 通讯录 · 部门管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Contacts` 域 · 部门管理子族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkDepartmentsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkDepartmentsService.cs)（父，3 支读取面）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkDepartmentsService_Internal.cs)（3）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkDepartmentsService_ThirdParty.cs)（3）· [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkDepartmentsService_Provider.cs)（空标记） |
| DTO 源文件 | [DataModels/Contacts/Department/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/Department/)（8 型，命名空间 `Mud.Wechat.Work.DataModels.Contacts.Department`） |
| 契约守卫 | [WechatDepartmentsContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDepartmentsContractGuards.cs)（D1 路由表 / D2 层级与代开发空标记 / D3 令牌绑定 / D4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 列表 90208/90344/96274 · 子部门 ID 95350/95406/96275 · 单个详情 95351/95407/96276 · 创建 90205/90341 · 更新 90206/90342 · 删除 90207/90343 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：D1~D4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

通讯录组织树侧的读写：

| 能力 | 端点 | 用途 |
|---|---|---|
| 递归取整棵树 | E1 `department/list` | 指定部门及其**所有子孙**；不传 id 取全量。官方标注性能较低 |
| 递归取 ID 骨架 | E2 `department/simplelist` | 只回 `id`/`parentid`/`order`，2022-08 加固后官方**推荐替代** E1 |
| 取单节点详情 | E3 `department/get` | 单个部门的名称、父部门、负责人、排序 |
| 写入 | E4 create / E5 update / E6 delete | 自建与第三方各声明一次；**单次一个部门** |

与成员族（[Users.md](./Users.md)）同构：读取面三类应用一致并收敛父接口，写入面按官方分文档形态在**自建 + 第三方**两支子接口各自重复声明（同路由、令牌作用域不同），代开发子接口为空标记。D1 因此是 **9 处声明 / 6 条去重路由**。

两条要先记住的：

- **`order` 在部门侧是「值小靠前」，与成员侧「值大靠前」相反**——同名字段、反向语义（D 组 DTO 注释逐处写明）。
- **E1 与部门侧「新增 IP」限制**：自 2022-08-15 起「通讯录同步」新增 IP 不能再调用 E1（与成员族 `user/get` 同批停返），官方指向 E2。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（D3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager` |

写入面同样要求**通讯录同步助手**（自建）或第三方通讯录应用的 secret 换出的令牌；`permanent_code`、`corpsecret` 都是凭据，只写参数名，**不得进日志、遥测或异常消息**。

### 2.2 权限与业务前置

- 读取面：只能拉取令牌对应应用**权限范围内**的部门；第三方通讯录应用可获取企业所有部门 **ID**（E2）与所有部门详情**除名称外**（E3）。
- 第三方不可获取 `name` / `name_en`（E1 注释留档）；代开发取名称需管理员授权。
- 写入面：须有指定部门的**管理权限**；E5 若要移动部门，还须有**新父部门**的管理权限。
- **部门最大层级 15 层、部门总数 ≤3 万个**；官方建议「创建部门」与「创建成员」串行处理。
- E6 删除约束：**不能删根部门**（根部门 id 恒为 1）、**不能删含子部门或成员的部门**。
- `name` 同层唯一、1~64 个 UTF-8 字符，且**不能包含反斜杠、冒号、星号、问号、引号、尖括号、竖线**等字符（DTO 注释留档）；`name_en` 需在管理后台开启多语言支持才生效。
- `id` 若显式指定必须 **> 1**（不能用 1 占位根部门），不填由官方自动生成。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 读取面 | ✅ | ✅（名称受限） | ✅（继承父接口） |
| E4–E6 写入面 | ✅ 自建子接口 | ✅ 第三方子接口（仅通讯录应用） | ❌ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkDepartmentsService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 3 | 否 |
| `IWechatWorkInternalDepartmentsService` | `Mud.Wechat.Work` | 自建子接口 | 3 | 是 |
| `IWechatWorkThirdPartyDepartmentsService` | `Mud.Wechat.Work` | 第三方子接口 | 3 | 是 |
| `IWechatWorkProviderDepartmentsService` | `Mud.Wechat.Work` | 代开发子接口（**空标记**） | 0 | 是 |

注册组 `Contact`（与成员/标签/导入/导出/查看权限共用），装配入口 `AddContactApi()`。子接口 `InheritedFrom = nameof(WechatWorkDepartmentsService)`（生成器据此复用父接口实现，不重复生成公共端点）。

## 4. 端点

### E1. `GetDepartmentListAsync` — 获取部门列表

```csharp
Task<GetDepartmentListResponse> GetDepartmentListAsync(
    [Query("id")] int? id = null,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/department/list` |
| 声明接口 | 父接口 |
| 令牌 | Query `access_token` |

**请求参数**

| 位置 | 名称 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| Query | `id` | `int?` | 否 | 取该部门**及其所有子孙部门**（递归）；不填默认取全量组织架构 |

**响应参数**（`GetDepartmentListResponse.department[]` → [DepartmentInfo](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/Department/DepartmentInfo.cs)）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `id` | `Id` | `int` | 部门 ID（32 位整型） |
| `parentid` | `ParentId` | `int` | 父部门 ID（根部门为 `1`） |
| `name` | `Name` | `string?` | 同层唯一、1~64 个 UTF-8 字符；第三方不可获取、代开发需授权 |
| `name_en` | `NameEn` | `string?` | 英文名；需后台开启多语言支持；第三方不可获取 |
| `department_leader` | `DepartmentLeader` | `List<string>?` | 部门负责人 userid 列表（仅返回应用可见范围内的负责人） |
| `order` | `Order` | `long?` | **值小靠前**（与成员相反）；范围 `[0, 2^32)`，故 64 位整数承载 |

**本端点注意事项**：官方提示性能较低，大范围同步请改用 E2 + E3 组合；2022-08-15 起「通讯录同步」新增 IP 不能调用本端点。

### E2. `GetChildDepartmentIdListAsync` — 获取子部门 ID 列表

```csharp
Task<GetChildDepartmentIdListResponse> GetChildDepartmentIdListAsync(
    [Query("id")] int? id = null,
    CancellationToken cancellationToken = default);
```

- 路由 `GET /cgi-bin/department/simplelist`（**全连写、无下划线**）。
- 请求 `id`（`int?`，同上：不填取全量）。
- 响应 `department_id[]` → `DepartmentIdInfo`：`id`（`int`）、`parentid`（`int`）、`order`（`long?`，值小靠前）。
- 定位：2022-08 通讯录安全加固后官方推荐的替代接口——先拿 ID 骨架，再按需对少量节点调 E3 取详情。

### E3. `GetDepartmentAsync` — 获取单个部门详情

```csharp
Task<GetDepartmentResponse> GetDepartmentAsync(
    [Query("id")] int id,
    CancellationToken cancellationToken = default);
```

- 路由 `GET /cgi-bin/department/get`；`id` 为 **`int`（必填，非可空）**。
- 响应 `department`（**单个对象**，非数组）→ `DepartmentInfo`，字段同 E1。
- 第三方通讯录应用可取所有部门详情，但**部门名称除外**。

**调用示例**（本族主示例：E2 取骨架 → E3 补详情 → E4 建部门）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Contacts.Department;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "contact-sync";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:ContactSyncSecret"]; // 凭据：通讯录同步助手 secret
});
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

var departments = provider.GetRequiredService<IWechatWorkInternalDepartmentsService>();

// 1) 先用 simplelist 拿 ID 骨架（官方推荐的轻量读法）
var skeleton = await departments.GetChildDepartmentIdListAsync(1, cancellationToken);
WechatWorkException.ThrowIfFailed(skeleton, "/cgi-bin/department/simplelist");

foreach (DepartmentIdInfo node in skeleton.DepartmentIds ?? new List<DepartmentIdInfo>())
{
    // 部门 order：值小靠前（成员 order 是值大靠前，二者不可共用比较器）
    logger.LogInformation("部门 id={Id} 父={ParentId} 次序={Order}", node.Id, node.ParentId, node.Order);
}

// 2) 只给需要名称的节点补一次详情
var detail = await departments.GetDepartmentAsync(2, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/department/get");
logger.LogInformation("部门名称：{Name}", detail.Department?.Name);

// 3) 新建一个部门：单次只能建一个，层级 ≤15、总数 ≤3 万
var created = await departments.CreateDepartmentAsync(
    new CreateDepartmentRequest
    {
        Name = "示例部门",   // 同层唯一、1~64 个 UTF-8 字符，且不含 \ : * ? " < > | 等字符
        ParentId = 1,        // 根部门 id 恒为 1
        Order = 100,         // 值小靠前
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/department/create");
logger.LogInformation("新建部门 id={DepartmentId}", created.Id);
```

### E4. `CreateDepartmentAsync` — 创建部门

```csharp
Task<CreateDepartmentResponse> CreateDepartmentAsync(
    [Body] CreateDepartmentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/department/create` |
| 声明接口 | 自建子接口 **与** 第三方子接口（同路由、各声明一次） |

**请求参数**（`CreateDepartmentRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `name` | `Name` | `string` | 是 | 同层唯一、1~64 个 UTF-8 字符；禁用 `\ : * ? " < > \|` 等字符 |
| `parentid` | `ParentId` | `int` | 是 | 父部门 ID（32 位整型；根部门为 `1`） |
| `id` | `Id` | `int?` | 否 | **指定时必须 >1**；不填由官方自动生成 |
| `order` | `Order` | `long?` | 否 | **值小靠前**，范围 `[0, 2^32)` |
| `name_en` | `NameEn` | `string?` | 否 | 同层唯一、1~64 字符；需后台开启多语言才生效 |

**响应参数**：`id`（`int`，新建部门 ID——**未显式指定 `id` 时必须从这里取回**，再用于建成员）。

**本端点注意事项**：**单次只建一个部门**，没有批量入口，也没有部分成功；层级 ≤15、总数 ≤3 万由官方校验。

### E5. `UpdateDepartmentAsync` — 更新部门

```csharp
Task<WechatWorkResponse> UpdateDepartmentAsync(
    [Body] UpdateDepartmentRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/department/update`（自建 + 第三方各声明一次）；应答仅 `errcode`/`errmsg`。
- 请求：`id`（`int`，**必填**）；`name` / `name_en` / `parentid` / `order` 全部可选，**未指定即不更新**。
- 传 `parentid` 即移动部门，须同时拥有**新父部门**的管理权限。

### E6. `DeleteDepartmentAsync` — 删除部门

```csharp
Task<WechatWorkResponse> DeleteDepartmentAsync(
    [Query("id")] int id,
    CancellationToken cancellationToken = default);
```

- 路由 **`GET`** `/cgi-bin/department/delete`（写入却走 GET，D1 锁 `GetAttribute`）。
- **不能删除根部门**；**不能删除含有子部门或成员的部门**——要先迁空成员、删子部门，自底向上做。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 部门 `order` vs 成员 `order` | 部门**值小靠前**、成员**值大靠前** | 共用比较器 → 排序反向、组织树错序 |
| E1 `department`（数组）vs E3 `department`（单对象） | 同名字段、两种形态 | 复用同一 DTO 包装 → 一侧解析失败 |
| E1 `department/list` vs E2 `department/simplelist` | E2 只有 id/parentid/order、无名称；是官方推荐替代 | 大组织用 E1 → 慢且可能被 IP 限制挡住 |
| E1/E2/E3 的 `id` | E1/E2 是 `int?`（不填取全量）；E3/E6 是 `int`（必填） | 照抄可空签名 → E3 传 null 编译不过或语义漂移 |
| `create` 的 `id` vs 响应的 `id` | 请求 `id` 可省（官方生成）、`>1`；响应 `id` 必回 | 自己造 id 且用 1 → 与根部门冲突 |
| `simplelist` vs `get_userid_by_email` | 同域内连写与下划线两种拼法并存，各自锁定 | 「统一命名」→ 路由漂移 |
| E4 单次建一个 vs 成员批量导入 | 部门无批量 HTTP 入口，批量能力在 [Batch.md](./Batch.md) / [Export.md](./Export.md) 的异步任务 | 在部门族找批量端点 |
| 代开发子接口 | 空标记（D2 断言无端点） | 找 `CreateDepartmentAsync` 于代开发接口 → 不存在 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌 Query 注入是官方契约（`MUD005`）；SDK 不做本地层级/数量校验，越界由 `errcode` 表达。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

部门业务码（如名称重复、层级超限、删除非空部门）**接口注释与守卫均未留档码值** ⇒ 未逐项核验，见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 逐端点业务错误码与数值 QPS | 接口注释只留业务限制（层级 15 / 总数 3 万 / 删除约束），无码值与频率 | 官方 90205–90208 |
| `name` 的禁用字符全集 | DTO 注释写「反斜杠、冒号、星号、问号、引号、尖括号、竖线**等**字符」⇒ 集合非穷举，**未逐项核验** | 官方 90205 |
| 第三方取「所有部门」的确切边界 | E2 注释说第三方通讯录应用可获取企业所有部门 ID、E3 说「部门名字除外」，两者是否同一权限口径**未核验** | 官方 95406/95407 |
| 部门 `parentname` 键 | 域 README 明确「本仓接口/DTO/守卫三类证据均未出现、不建模」 | 见域 README 缺口表 |
| 层级 15 与总数 3 万的计入口径 | 是否含根部门、是否按企业全局统计，注释未说明 | 官方 90205 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§3 定位键与 `order` 语义反转、§4 停返时间线、§5 原子性差异）
- 同域：[Users.md](./Users.md)（成员，含 `department`/`order` 的成员侧口径）· [Tags.md](./Tags.md) · [Batch.md](./Batch.md)（`replaceparty` 全量覆盖部门）· [Export.md](./Export.md) · [ContactRules.md](./ContactRules.md)（隐藏部门规则）
- 官方页：[获取部门列表（自建 90208）](https://developer.work.weixin.qq.com/document/path/90208) · [获取子部门 ID 列表（95350）](https://developer.work.weixin.qq.com/document/path/95350) · [创建部门（90205）](https://developer.work.weixin.qq.com/document/path/90205)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
