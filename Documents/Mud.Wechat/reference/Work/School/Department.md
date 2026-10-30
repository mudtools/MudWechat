# 家校部门管理 · 部门树增删改查与自动升年级

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 部门管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolDepartmentService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Department/IWechatWorkSchoolDepartmentService.cs)（父，承载全部 5 支）＋ 自建 / 第三方 / 代开发三个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Department/IWechatWorkSchoolDepartmentService_Internal.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Department/IWechatWorkSchoolDepartmentService_ThirdParty.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Department/IWechatWorkSchoolDepartmentService_Provider.cs)） |
| DTO 源文件 | [DataModels/School/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/RequestModel/)（本族 10 个文件 / 10 型，`SerializerClassName = School` → `SchoolJsonContext`） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1e 5 路由 / SCH2 父 5 + 三子 0 / SCH3 令牌绑定 / SCH4 DTO 登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 创建 [92340](https://developer.work.weixin.qq.com/document/path/92340) / [92296](https://developer.work.weixin.qq.com/document/path/92296) / [100158](https://developer.work.weixin.qq.com/document/path/100158)；更新 [92341](https://developer.work.weixin.qq.com/document/path/92341) / [92297](https://developer.work.weixin.qq.com/document/path/92297) / [100159](https://developer.work.weixin.qq.com/document/path/100159)；删除 [92342](https://developer.work.weixin.qq.com/document/path/92342) / [92298](https://developer.work.weixin.qq.com/document/path/92298) / [100160](https://developer.work.weixin.qq.com/document/path/100160)；列表 [92343](https://developer.work.weixin.qq.com/document/path/92343) / [92299](https://developer.work.weixin.qq.com/document/path/92299) / [96745](https://developer.work.weixin.qq.com/document/path/96745)；自动升年级 [92949](https://developer.work.weixin.qq.com/document/path/92949) / [92950](https://developer.work.weixin.qq.com/document/path/92950) / [100161](https://developer.work.weixin.qq.com/document/path/100161)（依次自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1e / SCH2 / SCH3 / SCH4 断言 + 5 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

家校通讯录不是「学校表 + 年级表 + 班级表」几张独立表，而是**一棵部门树**：校区、学段、年级、班级都是同一 `department` 集合里的节点，用 `type` 区分层级。这一族管的正是这棵树的结构操作：

| 端点 | 用途 |
|---|---|
| E1 `school/department/create` | 建节点（班级 / 年级 / 学段 / 校区），可顺手配管理员 |
| E2 `school/department/update` | 改节点（名称 / 父节点 / 新 id / 入学年份 / 标准年级 / 排序 / 管理员逐条增删改） |
| E3 `school/department/delete` | 删节点（官方即 GET，`id` 走 Query） |
| E4 `school/department/list` | 读树（不填 `id` 取全量，填 `id` 取该部门及其子树） |
| E5 `school/set_upgrade_info` | 配置「每年自动升年级」的开关与执行日期 |

`type` 取值（[SchoolDepartmentInfo.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/SchoolDepartmentInfo.cs)）：

| `type` | 语义 | 关键约束 |
|---|---|---|
| 1 | 班级 | 父部门必须是年级（`type=1` 的节点，其父节点 `type` 必须为 2） |
| 2 | 年级 | 唯一可携带 `register_year` / `standard_grade` 的层级 |
| 3 | 学段 | 学段负责人（管理员 `type=5`）挂在这一层 |
| 4 | 校区 | 校区负责人（管理员 `type=1`）挂在这一层 |
| 5 | 学校 | 根部门，`id` 固定为 1、`parentid` 为 0，由系统预置 |

**不处理部门结构的场景**：学生 / 家长挂到班级、按班级拉人是 [User.md](./User.md)；「可使用的家长范围」里返回的部门 id 列表在 [Basic.md](./Basic.md)（E7，字段名为 `partyid` 且是 `long`）；建班级后是否自动建群属 [Basic.md](./Basic.md) 的「班级群创建方式」。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` 三选一 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（三类同键，**不按应用类型拆分**） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp`（企业级，scope = `authCorpId`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 写入族（E1 / E2 / E3 / E5）：自建应用须配置到「家校沟通 - 读取和编辑家校通讯录的应用」；第三方应用须具有「家校沟通」使用和编辑权限；代开发应用须拥有「家校沟通」可使用家校通讯录-家校通讯录编辑权限。
- 读取族（E4）：自建应用亦接受「家校沟通 - 读取家校通讯录的应用」或「家长可使用应用」列表；第三方须具有「家校沟通」使用权限；代开发须具有「家校沟通」权限。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 第三方 / 代开发调用前须切企业作用域：`using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))`（`permanentCode` 是凭据，**不得落日志**）。
- 建树顺序有硬约束：**先建年级（`type=2`），再建班级（`type=1`）**；班级的父节点类型不对会被官方拒绝。
- 管理员 `userid` 是可关联到具体教职工的标识：示例一律假值，调用日志只记数量。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 创建部门 | ✅ | ✅ | ✅ | 三页齐全 |
| E2 更新部门 | ✅ | ✅ | ✅ | 三页齐全 |
| E3 删除部门 | ✅ | ✅ | ✅ | 三页齐全 |
| E4 部门列表 | ✅ | ✅ | ✅ | 三页齐全 |
| E5 自动升年级 | ✅ | ✅ | ✅ | 三页齐全 |

> 本族是 `School` 域里开放面最整齐的一族：三类应用端点完全一致，所以三个子接口都是**零端点空标记**（SCH2 锁死），服务端权限差异只体现在应用可配置项上，不影响类型面。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolDepartmentService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 5 | 否 |
| `IWechatWorkInternalSchoolDepartmentService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartySchoolDepartmentService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderSchoolDepartmentService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`。
- 端点索引：E1 `CreateDepartmentAsync` · E2 `UpdateDepartmentAsync` · E3 `DeleteDepartmentAsync` · E4 `ListDepartmentsAsync` · E5 `SetUpgradeInfoAsync`。
- 5 支端点**全部声明在父接口**；子接口只是「按应用类型取实现」的类型化入口，`GetMethods(DeclaredOnly)` 恒为空。

---

## 4. 端点

### E1. `CreateDepartmentAsync` — 创建部门

```csharp
[Post("/cgi-bin/school/department/create")]
Task<SchoolCreateDepartmentResponse> CreateDepartmentAsync(
    [Body] SchoolCreateDepartmentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/department/create` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolCreateDepartmentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Name` | `name` | `string?` | 否* | 1~32 个字符，不能含 `:*?"<>/` 等；**设置了入学年份 + 标准年级时该字段被忽略** |
| `Parentid` | `parentid` | `int?` | 是 | 父部门 `id`（班级填年级 id；校区 / 学段 / 年级的父节点按学校实际层级） |
| `Id` | `id` | `int?` | 否 | 指定时必须 **> 1**；不填则由系统自动生成 |
| `Type` | `type` | `int?` | 是 | `1` 班级 / `2` 年级 / `3` 学段 / `4` 校区（`5` 学校由系统预置，不用于创建） |
| `StandardGrade` | `standard_grade` | `int?` | 否 | 取值见官方标准年级对照表；**仅 `type = 2` 时生效** |
| `RegisterYear` | `register_year` | `int?` | 否 | 格式 `YYYY`，范围 1970~2100；**仅 `type = 2` 时生效**；标准年级不填则由系统自动计算 |
| `Order` | `order` | `long?` | 否 | 父节点下的排序值，大者靠前，范围 `[0, 2^32)` |
| `DepartmentAdmins` | `department_admins` | `List<SchoolDepartmentAdminItem>?` | 否 | 管理员条目（**无 `op` 字段**，与 E2 的条目不同型） |

\* 官方参数表未把 `name` 标为必填，但当未设标准年级时它就是节点的名字——实际必传。

`SchoolDepartmentAdminItem` 元素：

| 属性 | JSON | 类型 | 说明 |
|---|---|---|---|
| `Userid` | `userid` | `string?` | 管理端账号，1~64 字节，企业内唯一、不区分大小写 |
| `Type` | `type` | `int?` | `1` 校区负责人 / `2` 年级负责人 / `3` 班主任 / `4` 任课老师 / `5` 学段负责人 |
| `Subject` | `subject` | `string?` | 教师科目，最多 15 字符、只支持一个；**仅班主任与任课老师可设** |

**响应参数**（`SchoolCreateDepartmentResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `id` | `Id` | `int?` | 新建部门的 `id`（自动生成时也是这里回读） |

**本端点注意事项**

- **班的父必须是年级**：`type=1` 的请求若挂到 `type≠2` 的父节点上，官方直接拒绝——建树顺序必须自顶向下。
- **管理员类型必须与部门类型配对**：校区负责人 (`1`) → 校区 (`4`)，年级负责人 (`2`) → 年级 (`2`)，班主任 (`3`) / 任课老师 (`4`) → 班级 (`1`)，学段负责人 (`5`) → 学段 (`3`)。完整合法矩阵仓内未逐项核验（见 §7）。
- 同时传 `register_year` 与 `standard_grade` 时，`name` 被系统生成的年级名覆盖——别拿 `name` 做后续定位。
- 创建成功务必回读 `Id` 并用它建下一层：自动生成的 id 无法从请求推断。

### E2. `UpdateDepartmentAsync` — 更新部门

```csharp
[Post("/cgi-bin/school/department/update")]
Task<WechatWorkResponse> UpdateDepartmentAsync(
    [Body] SchoolUpdateDepartmentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/department/update` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolUpdateDepartmentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Id` | `id` | `int?` | 是 | 目标部门 `id`，必须 > 0 |
| `Name` | `name` | `string?` | 否 | 1~32 字符；**标准年级忽略该字段** |
| `Parentid` | `parentid` | `int?` | 否 | 新父部门 `id`（移动节点） |
| `NewId` | `new_id` | `int?` | 否 | 修改后的部门 id（换键） |
| `RegisterYear` | `register_year` | `int?` | 否 | 仅 `type=2` 生效，1970~2100 |
| `StandardGrade` | `standard_grade` | `int?` | 否 | **传 `0` = 把该部门转为非标准年级**；仅 `type=2` 生效 |
| `Order` | `order` | `long?` | 否 | 排序值，大者靠前，`[0, 2^32)` |
| `DepartmentAdmins` | `department_admins` | `List<SchoolUpdateDepartmentAdminItem>?` | 否 | **逐条 `op` 操作**（与 E1 的条目不同型） |

`SchoolUpdateDepartmentAdminItem` 元素：

| 属性 | JSON | 类型 | 说明 |
|---|---|---|---|
| `Op` | `op` | `int?` | 官方必填：`0` 新增或更新，`1` 删除 |
| `Userid` | `userid` | `string?` | 官方必填，企业管理端账号 |
| `Type` | `type` | `int?` | 管理员类型，取值同 E1 |
| `Subject` | `subject` | `string?` | 科目，仅班主任 / 任课老师可设 |

**响应参数**：仅 `errcode` / `errmsg`（无业务负载）⇒ 返回类型是 `WechatWorkResponse`。

**本端点注意事项**

- **`op` 只在 E2 有**：创建部门的管理员条目没有 `op`，更新部门的有；两个 DTO 官方即不共用，勿合并。
- 管理员列表是**逐条操作**，不是全量覆盖：想「只剩这个人」必须对其他人逐条 `op=1`。
- `standard_grade = 0` 是**回退**语义（转非标准年级），不是「清空字段」；如果只是不想改它，别传该字段。
- 改 `new_id` 会换掉节点的键，引用旧 id 的学生 / 家长 / 下级部门是否自动跟随，注释未留档（见 §7）——换键后请立即用 E4 复核树。

### E3. `DeleteDepartmentAsync` — 删除部门

```csharp
[Get("/cgi-bin/school/department/delete")]
Task<WechatWorkResponse> DeleteDepartmentAsync(
    [Query("id")] int departmentId,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/department/delete`（**官方即 GET，与直觉相反**） |
| 凭据 | Query `access_token` + Query `id` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `departmentId` | `id` | `int` | 否* | 目标部门 `id`，走 **Query**，非可空（SCH1e 专项断言） |

\* 官方参数表把 `id` 标为「否」，属官方原文；但删除不传 id 无意义，接口注释明确「语义上必须传入目标部门 id」。

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- **不能删根部门**（`id=1`），也不能删**含子部门或成员**的部门——需自底向上逐层清空。
- **单个删除是 GET、批量删除才 POST**：本族没有批量删除；学生 / 家长的单个删除同样是 GET（见 [User.md](./User.md)），别「顺手统一成 POST」。
- `id` 被建模为**非可空 `int`**：漏传是编译期问题，不是运行期缺参。

### E4. `ListDepartmentsAsync` — 获取部门列表

```csharp
[Get("/cgi-bin/school/department/list")]
Task<SchoolDepartmentListResponse> ListDepartmentsAsync(
    [Query("id")] int? departmentId = null,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/department/list` |
| 凭据 | Query `access_token` + 可选 Query `id` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `departmentId` | `id` | `int?` | 否 | 不填 = 全量组织架构；填 = 指定部门**及其所有子部门**（走 Query） |

**响应参数**（`SchoolDepartmentListResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `departments` | `Departments` | `List<SchoolDepartmentInfo>?` | 部门节点列表 |

`SchoolDepartmentInfo` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `id` | `Id` | `int?` | 部门 id（根固定为 1） |
| `type` | `Type` | `int?` | 1 班级 / 2 年级 / 3 学段 / 4 校区 / 5 学校 |
| `name` | `Name` | `string?` | 部门名称 |
| `parentid` | `Parentid` | `int?` | 父部门 id（根为 0） |
| `register_year` | `RegisterYear` | `int?` | **条件返回**：仅标准年级 |
| `standard_grade` | `StandardGrade` | `int?` | **条件返回**：仅标准年级 |
| `order` | `Order` | `long?` | **条件返回**：仅当通过 API 设置过 |
| `is_graduated` | `IsGraduated` | `int?` | **条件返回**：仅年级（1 已毕业 / 0 未毕业） |
| `open_group_chat` | `OpenGroupChat` | `int?` | **条件返回**：仅班级（1 开 / 0 关） |
| `group_chat_id` | `GroupChatId` | `string?` | **条件返回**：仅班级且 `open_group_chat=1` |
| `department_admins` | `DepartmentAdmins` | `List<SchoolDepartmentAdminInfo>?` | 管理员列表 |

**本端点注意事项**

- **响应无分页**：一次拿回整棵（或整棵子树），大校需要宿主自行按层分次拉取。
- 条件字段缺省是**正常形态**，不是数据损坏：`order` 缺失说明从未经 API 设置过。
- 父子关系只看 `id` / `parentid` / `type` 三元组，不要假设数组按层级有序。
- 管理员条目在响应里是 `SchoolDepartmentAdminInfo`（只读，无 `op`），与两个请求条目都不共用。

### E5. `SetUpgradeInfoAsync` — 修改自动升年级配置

```csharp
[Post("/cgi-bin/school/set_upgrade_info")]
Task<SchoolSetUpgradeInfoResponse> SetUpgradeInfoAsync(
    [Body] SchoolSetUpgradeInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/set_upgrade_info` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolSetUpgradeInfoRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `UpgradeTime` | `upgrade_time` | `long?` | 否 | 时间戳，**只有月和日有效**；不传按 `0` 处理（= 1 月 1 日） |
| `UpgradeSwitch` | `upgrade_switch` | `int?` | 否 | `1` 开启；不传默认关闭，**传任何非 1 的值也视为关闭** |

**响应参数**（`SchoolSetUpgradeInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `next_upgrade_time` | `NextUpgradeTime` | `long?` | 下次升级时间（只有年月日有效） |

**本端点注意事项**

- 这是**全校级**配置，不是按校区 / 年级分别设置。
- `upgrade_time` 的年与时分秒都被忽略——用「当年任意一天 + 目标月日」构造时间戳即可，不要指望它按年份生效。
- 关闭开关后 `next_upgrade_time` 返回什么，注释未留档（见 §7）。

**调用示例**（建年级 → 建班级 → 配管理员 → 读树 → 设自动升年级 → 删节点）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.School;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "school-a";
    o.AppType = WechatAppType.Internal;          // 第三方 / 代开发换 AppType 并切企业作用域
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

var departments = provider.GetRequiredService<IWechatWorkInternalSchoolDepartmentService>();
const int schoolRootId = 1;                       // 根部门固定为 1
const int gradeYear = 2026;

// E1：先建年级（type=2，标准年级 + 入学年份，此时 name 会被忽略）
var grade = await departments.CreateDepartmentAsync(new SchoolCreateDepartmentRequest
{
    Parentid = schoolRootId,
    Type = 2,
    StandardGrade = 7,                            // 取值见官方标准年级对照表
    RegisterYear = gradeYear,
    Order = 100,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(grade, "/cgi-bin/school/department/create");
int gradeId = grade.Id!.Value;

// E1：再建班级（type=1），父部门必须是年级；管理员类型须与部门类型配对
var klass = await departments.CreateDepartmentAsync(new SchoolCreateDepartmentRequest
{
    Name = "一年级 1 班",                          // 1~32 字符，不含 :*?"<>/
    Parentid = gradeId,
    Type = 1,
    Order = 10,
    DepartmentAdmins =
    [
        new SchoolDepartmentAdminItem
        {
            Userid = "teacher_0001",              // 假值；真实 userid 不得落日志
            Type = 3,                             // 3 班主任 → 只能配到班级
            Subject = "语文",
        },
    ],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(klass, "/cgi-bin/school/department/create");
int classId = klass.Id!.Value;

// E2：更新走逐条 op（op=0 新增/更新、op=1 删除）；standard_grade=0 表示转非标准年级
var updated = await departments.UpdateDepartmentAsync(new SchoolUpdateDepartmentRequest
{
    Id = classId,
    Name = "一年级 2 班",
    Order = 20,
    DepartmentAdmins =
    [
        new SchoolUpdateDepartmentAdminItem { Op = 0, Userid = "teacher_0002", Type = 4, Subject = "数学" },
        new SchoolUpdateDepartmentAdminItem { Op = 1, Userid = "teacher_0001" },
    ],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/school/department/update");

// E4：不填 id 取全量；order 等字段为条件返回，缺失属正常形态
var all = await departments.ListDepartmentsAsync(cancellationToken: cancellationToken);
WechatWorkException.ThrowIfFailed(all, "/cgi-bin/school/department/list");
List<SchoolDepartmentInfo> tree = all.Departments ?? new List<SchoolDepartmentInfo>();
// 子树同理：ListDepartmentsAsync(classId, cancellationToken)
logger.LogInformation("部门树共 {Count} 个节点", tree.Count);

// E5：自动升年级（upgrade_time 只有月日有效；非 1 的 switch 值均视为关闭）
var upgrade = await departments.SetUpgradeInfoAsync(new SchoolSetUpgradeInfoRequest
{
    UpgradeTime = new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
    UpgradeSwitch = 1,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(upgrade, "/cgi-bin/school/set_upgrade_info");
logger.LogInformation("下次升年级时间戳 {Next}", upgrade.NextUpgradeTime);

// E3：删除是 GET、id 走 Query；根部门与含子部门/成员的节点不可删（自底向上清）
var deleted = await departments.DeleteDepartmentAsync(classId, cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/school/department/delete");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 建树顺序 | 班级（`type=1`）的父必须是年级（`type=2`） | 先建班后建年级 → 创建被拒 |
| 管理员条目两类 | E1 用 `SchoolDepartmentAdminItem`（无 `op`），E2 用 `SchoolUpdateDepartmentAdminItem`（有 `op`） | 复用同一 DTO → 更新缺 `op` 或创建多字段 |
| 管理员列表语义 | E2 是**逐条 `op` 增量**，不是全量覆盖 | 以为传列表就覆盖 → 旧管理员还在 |
| `standard_grade = 0` | 更新时表示**转非标准年级**（回退），不是「清空」 | 误传 → 年级被降级 |
| `name` 被忽略 | 同时传了 `register_year` + `standard_grade` 时 `name` 无效 | 用 `name` 定位部门 → 找不到 |
| 删除方法 | E3 是 **GET**，`id` 走 Query | 发 POST → 405 / 404 |
| 删除前置 | 根部门、含子部门或含成员的部门不可删 | 直接删父节点 → 失败且需清理半途状态 |
| 列表 `id` | 可选；填了取「该部门 + 子树」，不是「仅该部门」 | 以为只返回一个节点 |
| 条件响应字段 | `order` / `is_graduated` / 班级群字段 / 年级字段按类型条件返回 | 把缺失当作接口异常 |
| `register_year` 范围 | 1970~2100，格式 YYYY | 传两位数年份 → 被拒 |
| `order` 宽度 | `long`，范围 `[0, 2^32)` | 建模成 `int` → 大值溢出 |
| 自动升年级开关 | 任何非 `1` 的值都视为关闭（含 `2`、`0`） | 传 `2` 以为是「第二种模式」 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不预检应用类型与层级关系；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本族注释未留档**专属**业务错误码：父子类型不匹配、管理员类型与部门类型不配、删除根部门 / 非空部门、`id ≤ 1`、`register_year` 越界等失败，均由官方业务码表达，具体码值以官方页为准（仓内未逐项核验）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 管理员 `type` × 部门 `type` 完整矩阵 | 注释给出「校区负责人→校区、年级负责人→年级、班主任/任课老师→班级、学段负责人→学段」示例，5×5 的完整合法矩阵 ⇒ 未逐项核验 | 官方创建页 92340 / 92296 / 100158 |
| `standard_grade` 取值全集 | 只写「详见官方标准年级对照表」，本仓不建枚举、不列取值 ⇒ 未逐项核验 | 官方对照表 |
| 改 `new_id` 的连锁影响 | 引用旧 id 的学生 / 家长 / 下级部门是否自动跟随 ⇒ 未核验 | 官方更新页 92341 |
| 自动升年级关闭后的 `next_upgrade_time` | 关闭开关时该字段返回 0 / 空 / 下次时间 ⇒ 未核验 | 官方 92949 |
| 「已毕业」年级的后续行为 | `is_graduated=1` 后该年级的班级 / 学生是否只读 ⇒ 未核验 | 官方升年级说明 |
| 本族 QPS / 频率上限 | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方访问频率说明（90454） |
| 删除非空部门的具体错误码 | 只知「不能删除含子部门 / 成员的部门」，码值未留档 | 官方删除页 92342 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Basic.md](./Basic.md)（关注入口 / 家长范围 `partyid`）· [Setting.md](./Setting.md)· [User.md](./User.md)（学生与家长 16 支）· [Auth.md](./Auth.md) · [HealthReport.md](./HealthReport.md) · [Living.md](./Living.md) · [ClassPay.md](./ClassPay.md)
- 数据模型：[SchoolDepartmentInfo.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/SchoolDepartmentInfo.cs) · `RequestModel/SchoolCreateDepartmentRequest.cs` / `SchoolUpdateDepartmentRequest.cs` / `SchoolUpdateDepartmentAdminItem.cs` / `SchoolSetUpgradeInfoRequest.cs`
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[创建部门 92340](https://developer.work.weixin.qq.com/document/path/92340) · [更新部门 92341](https://developer.work.weixin.qq.com/document/path/92341) · [删除部门 92342](https://developer.work.weixin.qq.com/document/path/92342) · [部门列表 92343](https://developer.work.weixin.qq.com/document/path/92343) · [自动升年级 92949](https://developer.work.weixin.qq.com/document/path/92949)
