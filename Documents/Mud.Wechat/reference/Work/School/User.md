# 学生与家长管理 · 家校通讯录成员、批量操作与自动同步

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 学生与家长管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolUserService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/User/IWechatWorkSchoolUserService.cs)（父，承载全部 16 支）＋ 自建 / 第三方 / 代开发三个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/User/IWechatWorkSchoolUserService_Internal.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/User/IWechatWorkSchoolUserService_ThirdParty.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/User/IWechatWorkSchoolUserService_Provider.cs)） |
| DTO 源文件 | [DataModels/School/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/) ＋ [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/RequestModel/)（本族 22 个文件，`SerializerClassName = School` → `SchoolJsonContext`） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1c 16 路由 + 4 个 `userid` / `department_id` Query 断言 / SCH2 父 16 + 三子 0 / SCH3 令牌绑定 / SCH4 DTO 登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 学生：创建 [92325](https://developer.work.weixin.qq.com/document/path/92325) / [92035](https://developer.work.weixin.qq.com/document/path/92035) / [100145](https://developer.work.weixin.qq.com/document/path/100145) · 删除 [92326](https://developer.work.weixin.qq.com/document/path/92326) / [92039](https://developer.work.weixin.qq.com/document/path/92039) / [100146](https://developer.work.weixin.qq.com/document/path/100146) · 更新 [92327](https://developer.work.weixin.qq.com/document/path/92327) / [92041](https://developer.work.weixin.qq.com/document/path/92041) / [100147](https://developer.work.weixin.qq.com/document/path/100147) · 批量建 [92328](https://developer.work.weixin.qq.com/document/path/92328) / [92037](https://developer.work.weixin.qq.com/document/path/92037) / [100148](https://developer.work.weixin.qq.com/document/path/100148) · 批量删 [92329](https://developer.work.weixin.qq.com/document/path/92329) / [92040](https://developer.work.weixin.qq.com/document/path/92040) / [100149](https://developer.work.weixin.qq.com/document/path/100149) · 批量改 [92330](https://developer.work.weixin.qq.com/document/path/92330) / [92042](https://developer.work.weixin.qq.com/document/path/92042) / [100150](https://developer.work.weixin.qq.com/document/path/100150)；家长：创建 [92331](https://developer.work.weixin.qq.com/document/path/92331) / [92077](https://developer.work.weixin.qq.com/document/path/92077) / [100151](https://developer.work.weixin.qq.com/document/path/100151) · 删除 [92332](https://developer.work.weixin.qq.com/document/path/92332) / [92079](https://developer.work.weixin.qq.com/document/path/92079) / [100152](https://developer.work.weixin.qq.com/document/path/100152) · 更新 [92333](https://developer.work.weixin.qq.com/document/path/92333) / [92081](https://developer.work.weixin.qq.com/document/path/92081) / [100153](https://developer.work.weixin.qq.com/document/path/100153) · 批量建 [92334](https://developer.work.weixin.qq.com/document/path/92334) / [92078](https://developer.work.weixin.qq.com/document/path/92078) / [100154](https://developer.work.weixin.qq.com/document/path/100154) · 批量删 [92335](https://developer.work.weixin.qq.com/document/path/92335) / [92080](https://developer.work.weixin.qq.com/document/path/92080) / [100155](https://developer.work.weixin.qq.com/document/path/100155) · 批量改 [92336](https://developer.work.weixin.qq.com/document/path/92336) / [92082](https://developer.work.weixin.qq.com/document/path/92082) / [100156](https://developer.work.weixin.qq.com/document/path/100156)；读取：详情 [92337](https://developer.work.weixin.qq.com/document/path/92337) / [92038](https://developer.work.weixin.qq.com/document/path/92038) / [96738](https://developer.work.weixin.qq.com/document/path/96738) · 部门学生 [92338](https://developer.work.weixin.qq.com/document/path/92338) / [92043](https://developer.work.weixin.qq.com/document/path/92043) / [96739](https://developer.work.weixin.qq.com/document/path/96739) · 部门家长 [92446](https://developer.work.weixin.qq.com/document/path/92446) / [92627](https://developer.work.weixin.qq.com/document/path/92627) / [96741](https://developer.work.weixin.qq.com/document/path/96741) · 自动同步 [92345](https://developer.work.weixin.qq.com/document/path/92345) / [92083](https://developer.work.weixin.qq.com/document/path/92083) / [100157](https://developer.work.weixin.qq.com/document/path/100157)（依次自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1c / SCH2 / SCH3 / SCH4 断言 + 16 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

家校通讯录里只有两类「人」——学生与家长；本族是它们的全量读写面：

| 分组 | 端点 |
|---|---|
| 学生（单条） | E1 创建 · E2 删除 · E3 更新 |
| 学生（批量） | E4 批量创建 · E5 批量删除 · E6 批量更新 |
| 家长（单条） | E7 创建 · E8 删除 · E9 更新 |
| 家长（批量） | E10 批量创建 · E11 批量删除 · E12 批量更新 |
| 读取 | E13 按 userid 读详情 · E14 读部门学生 · E15 读部门家长 |
| 配置 | E16 家校通讯录自动同步模式（**不可逆**） |

**不该用本族的场景**：

- 部门（班级 / 年级 / 学段 / 校区）的增删改查在 [Department.md](./Department.md)；学生挂班靠 `department`（班级 id 列表），但班级本身不在这里建。
- 「学校通知」二维码与关注模式在 [Basic.md](./Basic.md)；老师查看范围在 [Setting.md](./Setting.md)。
- 家校网页授权取当前家长 / 学生身份在 [Auth.md](./Auth.md)。
- **外部联系人详情**（`externalcontact/get`）与**发送学校通知**不在本域（分属客户联系域 / 消息推送域）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` 三选一 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp`（企业级，scope = `authCorpId`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 自建应用须配置到「家校沟通 - 读取和编辑家校通讯录的应用」（读取族亦接受「读取家校通讯录的应用」或「家长可使用应用」）；第三方应用写入族须「家校沟通」使用和编辑权限、读取族须使用权限；代开发写入族须「家校通讯录编辑」权限、读取族须「家校沟通」权限。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 官方容量与顺序约束（逐条来自接口 / DTO 注释）：
  - 批量增删改**每次 ≤100** 条；单学生所属班级 `department` **≤20** 个；单家长孩子 `children` **≤10** 个。
  - 每个班级学生总数 **≤3 万**；官方建议「创建部门」与「创建成员」**串行化**处理。
  - `new_student_userid` / `new_parent_userid` **每人只能改一次**，且不得与已存在的 UserID 相同。
  - 删除学生前**必须先删其家长**；删除家长不要求先删孩子。
- 第三方 / 代开发调用前须 `using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))`（`permanentCode` 是凭据，**不得落日志**）。
- **未成年人个人信息**：`name`、`mobile`、`student_userid`、`parent_userid`、`external_userid` 全是个人数据。示例用假值（「张小明」、`+8613800000000`、`stu_0001`），任何日志 / 遥测 / 异常消息都不得携带。
- 手机号可见性：**第三方应用读不到家长手机号**；代开发应用需管理员授权手机号权限才返回。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E16 | ✅ | ✅ | ✅ |

三类开放面等宽 ⇒ 16 支全部在公共父接口，三个应用类型子接口是**零端点空标记**。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolUserService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 16 | 否 |
| `IWechatWorkInternalSchoolUserService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartySchoolUserService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderSchoolUserService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`。
- 端点索引：E1 `CreateStudentAsync` · E2 `DeleteStudentAsync` · E3 `UpdateStudentAsync` · E4 `BatchCreateStudentAsync` · E5 `BatchDeleteStudentAsync` · E6 `BatchUpdateStudentAsync` · E7 `CreateParentAsync` · E8 `DeleteParentAsync` · E9 `UpdateParentAsync` · E10 `BatchCreateParentAsync` · E11 `BatchDeleteParentAsync` · E12 `BatchUpdateParentAsync` · E13 `GetSchoolUserAsync` · E14 `ListDepartmentStudentsAsync` · E15 `ListDepartmentParentsAsync` · E16 `SetArchSyncModeAsync`。

---

## 4. 端点

### E1. `CreateStudentAsync` — 创建学生

```csharp
[Post("/cgi-bin/school/user/create_student")]
Task<WechatWorkResponse> CreateStudentAsync(
    [Body] SchoolCreateStudentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/create_student` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolCreateStudentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `StudentUserid` | `student_userid` | `string?` | 是 | 学校内唯一；1~64 字节，仅数字 / 字母 / `_-@.`，首字符须数字或字母；不区分大小写；可与通讯录成员 UserID 相同 |
| `Name` | `name` | `string?` | 是 | 1~32 字符 |
| `Department` | `department` | `List<int>?` | 是 | 所属班级 id 列表，**≤20** 个 |
| `Mobile` | `mobile` | `string?` | 否 | 学生手机号 |
| `ToInvite` | `to_invite` | `bool?` | 否 | 是否发起邀请（默认 `true`）；**仅验证的学校**才能发起 |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- 参数字段超长时**整个请求被拦掉**（不是逐字段降级）。
- `student_userid` 是唯一键；姓名与手机号都不唯一，**不能拿来定位或去重**。

### E2. `DeleteStudentAsync` — 删除学生

```csharp
[Get("/cgi-bin/school/user/delete_student")]
Task<WechatWorkResponse> DeleteStudentAsync(
    [Query("userid")] string userId, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/user/delete_student`（**官方即 GET，`userid` 走 Query**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `userId` | `userid` | `string` | 是 | 学生的 userid（Query，SCH1c 专项断言） |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- 学生还有家长时**不允许删除**，必须先删家长（E8 或 E11）。

### E3. `UpdateStudentAsync` — 更新学生

```csharp
[Post("/cgi-bin/school/user/update_student")]
Task<WechatWorkResponse> UpdateStudentAsync(
    [Body] SchoolUpdateStudentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/update_student` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolUpdateStudentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `StudentUserid` | `student_userid` | `string?` | 是 | 要更新的学生 userid（定位键） |
| `NewStudentUserid` | `new_student_userid` | `string?` | 否 | 变更后的 UserID；**每个学生仅能改一次**，不得与已存在 UserID 相同 |
| `Name` | `name` | `string?` | 否 | 1~32 字符 |
| `Department` | `department` | `List<int>?` | 否 | 班级 id 列表，≤20 个 |
| `Mobile` | `mobile` | `string?` | 否 | 学生手机号 |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- 改 `new_student_userid` 后，旧 ID 不再可用，宿主侧映射要同步改写。
- 每个班级学生总数 ≤3 万，批量搬班前先算容量。

### E4. `BatchCreateStudentAsync` — 批量创建学生

```csharp
[Post("/cgi-bin/school/user/batch_create_student")]
Task<SchoolBatchStudentResultResponse> BatchCreateStudentAsync(
    [Body] SchoolBatchCreateStudentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/batch_create_student` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolBatchCreateStudentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Students` | `students` | `List<SchoolCreateStudentRequest>?` | 是 | 每次**最多 100 个**；元素字段同 E1 |

**响应参数**（`SchoolBatchStudentResultResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `result_list[].student_userid` | `ResultList[].StudentUserid` | `string?` | 失败学生的 userid |
| `result_list[].errcode` | `ResultList[].ErrorCode` | `int?` | 该学生的操作返回码 |
| `result_list[].errmsg` | `ResultList[].ErrorMessage` | `string?` | 返回码描述 |

**本端点注意事项**

- **整体 `errcode` 非 0 时仍可能带 `result_list` 明细**；不能只判外层码就整批重投。
- 按 `student_userid` 逐条记账，已成功的不要再提交。

### E5. `BatchDeleteStudentAsync` — 批量删除学生

```csharp
[Post("/cgi-bin/school/user/batch_delete_student")]
Task<SchoolBatchStudentResultResponse> BatchDeleteStudentAsync(
    [Body] SchoolBatchDeleteStudentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/batch_delete_student`（**批量删除是 POST**，与 E2 的 GET 不同） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolBatchDeleteStudentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Useridlist` | `useridlist` | `List<string>?` | 是 | 每次**最多 100 个** |

**响应参数**：同 E4 的 `result_list` 形态。

**本端点注意事项**

- 仍有家长的学生删除会失败并出现在 `result_list`；批量删学生前先批量删家长。

### E6. `BatchUpdateStudentAsync` — 批量更新学生

```csharp
[Post("/cgi-bin/school/user/batch_update_student")]
Task<SchoolBatchStudentResultResponse> BatchUpdateStudentAsync(
    [Body] SchoolBatchUpdateStudentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/batch_update_student` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolBatchUpdateStudentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Students` | `students` | `List<SchoolUpdateStudentRequest>?` | 是 | 每次**最多 100 个**；元素字段同 E3（`new_student_userid` 同样只能改一次） |

**响应参数**：同 E4 的 `result_list` 形态。

### E7. `CreateParentAsync` — 创建家长

```csharp
[Post("/cgi-bin/school/user/create_parent")]
Task<WechatWorkResponse> CreateParentAsync(
    [Body] SchoolCreateParentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/create_parent` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolCreateParentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `ParentUserid` | `parent_userid` | `string?` | 是 | 学校内唯一；1~64 字节，字符与首字符规则同学生 |
| `Mobile` | `mobile` | `string?` | 是 | 家长手机号 |
| `Children` | `children` | `List<SchoolParentChildItem>?` | 是 | **最多 10 个**；元素为 `student_userid` + `relation` |
| `ToInvite` | `to_invite` | `bool?` | 否 | 默认 `true`；仅验证的学校能发起邀请 |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- `children[].relation` 是自由文本（最长 32 字节），本仓不建枚举。
- 家长手机号是必填，也是个人数据，不得落日志。

### E8. `DeleteParentAsync` — 删除家长

```csharp
[Get("/cgi-bin/school/user/delete_parent")]
Task<WechatWorkResponse> DeleteParentAsync(
    [Query("userid")] string userId, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/user/delete_parent`（官方即 GET，`userid` 走 Query） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `userId` | `userid` | `string` | 是 | 家长的 userid（Query） |

**响应参数**：仅 `errcode` / `errmsg`。

### E9. `UpdateParentAsync` — 更新家长

```csharp
[Post("/cgi-bin/school/user/update_parent")]
Task<WechatWorkResponse> UpdateParentAsync(
    [Body] SchoolUpdateParentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/update_parent` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolUpdateParentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `ParentUserid` | `parent_userid` | `string?` | 是 | 要更新的家长 userid（定位键） |
| `NewParentUserid` | `new_parent_userid` | `string?` | 否 | 变更后的 UserID；**每个家长仅能更新一次** |
| `Mobile` | `mobile` | `string?` | 否 | 家长手机号 |
| `Children` | `children` | `List<SchoolParentChildItem>?` | 否 | **全量更新**：传非空列表 = 全量覆盖；空列表 = **忽略该字段**（不会清空孩子关系） |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- 「清空孩子关系」**不能**靠传空数组实现——官方语义是忽略。要解绑需逐条构造目标列表覆盖。
- 与 E3 的 `department` 不同：`children` 没有「不超过 20」的说法，上限是 10。

### E10. `BatchCreateParentAsync` — 批量创建家长

```csharp
[Post("/cgi-bin/school/user/batch_create_parent")]
Task<SchoolBatchParentResultResponse> BatchCreateParentAsync(
    [Body] SchoolBatchCreateParentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/batch_create_parent` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolBatchCreateParentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Parents` | `parents` | `List<SchoolCreateParentRequest>?` | 是 | 每次**最多 100 个**；每个家长 `children` ≤10 |

**响应参数**（`SchoolBatchParentResultResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `result_list[].parent_userid` | `ResultList[].ParentUserid` | `string?` | 失败家长的 userid |
| `result_list[].errcode` | `ResultList[].ErrorCode` | `int?` | 该家长的操作返回码 |
| `result_list[].errmsg` | `ResultList[].ErrorMessage` | `string?` | 返回码描述 |

### E11. `BatchDeleteParentAsync` — 批量删除家长

```csharp
[Post("/cgi-bin/school/user/batch_delete_parent")]
Task<SchoolBatchParentResultResponse> BatchDeleteParentAsync(
    [Body] SchoolBatchDeleteParentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/batch_delete_parent`（批量删除是 POST） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolBatchDeleteParentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Useridlist` | `useridlist` | `List<string>?` | 是 | 每次**最多 100 个** |

**响应参数**：同 E10 的 `result_list` 形态。

### E12. `BatchUpdateParentAsync` — 批量更新家长

```csharp
[Post("/cgi-bin/school/user/batch_update_parent")]
Task<SchoolBatchParentResultResponse> BatchUpdateParentAsync(
    [Body] SchoolBatchUpdateParentRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/user/batch_update_parent` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolBatchUpdateParentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Parents` | `parents` | `List<SchoolUpdateParentRequest>?` | 是 | 每次**最多 100 个**；`children` 全量语义与 E9 相同 |

**响应参数**：同 E10 的 `result_list` 形态。

### E13. `GetSchoolUserAsync` — 读取学生或家长

```csharp
[Get("/cgi-bin/school/user/get")]
Task<GetSchoolUserResponse> GetSchoolUserAsync(
    [Query("userid")] string userId, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/user/get`（官方即 GET，`userid` 走 Query） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `userId` | `userid` | `string` | 是 | 学生或家长的 userid，1~64 字节，不区分大小写 |

**响应参数**（`GetSchoolUserResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `user_type` | `UserType` | `int?` | `1` = 学生（`student` 返回）；`2` = 家长（`parent` 返回） |
| `student` | `Student` | `SchoolStudentInfo?` | 学生详情（`user_type = 1`） |
| `parent` | `Parent` | `SchoolParentInfo?` | 家长详情（`user_type = 2`） |

`SchoolStudentInfo`：`student_userid` / `name` / `department`（`List<int>?`）/ `parents`（`List<SchoolParentInfo>?`）。

`SchoolParentInfo`：`parent_userid` / `relation` / `mobile` / `is_subscribe`（0/1）/ `external_userid` / `children`（`List<SchoolChildInfo>?`）。

`SchoolChildInfo`：`student_userid` / `relation` / `name`（仅读取部门家长详情时返回）。

**本端点注意事项**

- 必须按 `user_type` 取对应分支；另一分支为 `null` 是正常形态。
- `is_subscribe` 是「家长是否关注学校通知」；`external_userid` 仅在已关注时返回。
- 第三方应用读到的 `mobile` 为空是官方限制，不是解析失败。

### E14. `ListDepartmentStudentsAsync` — 获取部门学生详情

```csharp
[Get("/cgi-bin/school/user/list")]
Task<SchoolDepartmentStudentsResponse> ListDepartmentStudentsAsync(
    [Query("department_id")] int departmentId, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/user/list`（官方即 GET，`department_id` 走 Query） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `departmentId` | `department_id` | `int` | 是 | 部门（班级）id，走 Query |

**响应参数**（`SchoolDepartmentStudentsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `students[]` | `Students` | `List<SchoolStudentInfo>?` | 学生列表（每个学生带 `parents`） |

**本端点注意事项**

- **无 cursor / limit 分页**；只返回该部门直属学生，不含子部门。要取子树须「先取子部门 → 再递归取学生」。
- 一个班可能上万学生，响应体量需自行控制（本接口无分页可切）。

### E15. `ListDepartmentParentsAsync` — 获取部门家长详情

```csharp
[Get("/cgi-bin/school/user/list_parent")]
Task<SchoolDepartmentParentsResponse> ListDepartmentParentsAsync(
    [Query("department_id")] int departmentId, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/user/list_parent`（官方即 GET，`department_id` 走 Query） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `departmentId` | `department_id` | `int` | 是 | 部门（班级）id，走 Query |

**响应参数**（`SchoolDepartmentParentsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `parents[]` | `Parents` | `List<SchoolParentInfo>?` | 家长列表（每个家长带 `children`，孩子带 `name`） |

**本端点注意事项**

- 同样是**无分页 + 不含子部门**；家长手机号第三方不可读、代开发需授权。

### E16. `SetArchSyncModeAsync` — 设置家校通讯录自动同步模式

```csharp
[Post("/cgi-bin/school/set_arch_sync_mode")]
Task<WechatWorkResponse> SetArchSyncModeAsync(
    [Body] SchoolSetArchSyncModeRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/set_arch_sync_mode` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolSetArchSyncModeRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `ArchSyncMode` | `arch_sync_mode` | `int?` | 是 | `1` = 禁止标签同步至家校通讯录；`2` = 禁止家校通讯录同步至标签；`3` = 双向都禁止 |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- **不可逆**：一旦设置「禁止自动同步」，**无法再次开启**。上线前务必确认，不要放在初始化脚本里无脑执行。

**调用示例**（建学生 → 批量建 → 读详情 / 部门列表 → 建家长 → 全量更新孩子 → 自动同步）

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

var users = provider.GetRequiredService<IWechatWorkInternalSchoolUserService>();
int classDeptId = 100001;                        // 已由 Department.md 的 E1 建好的班级 id

// E1：student_userid 是唯一键；name / mobile 不唯一
var created = await users.CreateStudentAsync(new SchoolCreateStudentRequest
{
    StudentUserid = "stu_0001",                  // 假值
    Name = "张小明",                              // 未成年人假值
    Mobile = "+8613800000000",                   // 假值；第三方应用读不到
    Department = new List<int> { classDeptId },  // ≤20 个班级
    ToInvite = false,                            // 仅验证的学校可发邀请
}, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/school/user/create_student");

// E4：批量创建单批 ≤100；外层 errcode 非 0 也可能带 result_list
var batch = await users.BatchCreateStudentAsync(new SchoolBatchCreateStudentRequest
{
    Students = new List<SchoolCreateStudentRequest>
    {
        new SchoolCreateStudentRequest
        {
            StudentUserid = "stu_0002",
            Name = "李小红",
            Department = new List<int> { classDeptId },
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(batch, "/cgi-bin/school/user/batch_create_student");
foreach (SchoolStudentBatchResultItem item in batch.ResultList ?? new List<SchoolStudentBatchResultItem>())
{
    logger.LogWarning("学生 {Student} 失败：errcode={Errcode}", item.StudentUserid, item.ErrorCode);
}

// E13：按 user_type 取分支（1 学生 / 2 家长）
var detail = await users.GetSchoolUserAsync("stu_0001", cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/school/user/get");
SchoolStudentInfo? student = detail.UserType == 1 ? detail.Student : null;
logger.LogInformation("学生命中：{Hit}，班级 {DeptCount} 个", student is not null, student?.Department?.Count ?? 0);

// E14 / E15：官方无分页、不含子部门；取子树须自行递归
var students = await users.ListDepartmentStudentsAsync(classDeptId, cancellationToken);
WechatWorkException.ThrowIfFailed(students, "/cgi-bin/school/user/list");
logger.LogInformation("班级学生 {Count} 人", students.Students?.Count ?? 0);

var parents = await users.ListDepartmentParentsAsync(classDeptId, cancellationToken);
WechatWorkException.ThrowIfFailed(parents, "/cgi-bin/school/user/list_parent");
logger.LogInformation("班级家长 {Count} 人", parents.Parents?.Count ?? 0);

// E7：children ≤10；relation 是自由文本
var parentCreated = await users.CreateParentAsync(new SchoolCreateParentRequest
{
    ParentUserid = "par_0001",
    Mobile = "+8613800000000",
    Children = new List<SchoolParentChildItem>
    {
        new SchoolParentChildItem { StudentUserid = "stu_0001", Relation = "父亲" },
    },
    ToInvite = false,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(parentCreated, "/cgi-bin/school/user/create_parent");

// E9：children 是全量更新——传空数组是「忽略」，不是「清空」
var parentUpdated = await users.UpdateParentAsync(new SchoolUpdateParentRequest
{
    ParentUserid = "par_0001",
    Children = new List<SchoolParentChildItem>
    {
        new SchoolParentChildItem { StudentUserid = "stu_0001", Relation = "父亲" },
        new SchoolParentChildItem { StudentUserid = "stu_0002", Relation = "监护人" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(parentUpdated, "/cgi-bin/school/user/update_parent");

// E16：不可逆——一旦禁止自动同步就无法再开启，切勿放进无条件初始化流程
var sync = await users.SetArchSyncModeAsync(
    new SchoolSetArchSyncModeRequest { ArchSyncMode = 3 }, cancellationToken);
WechatWorkException.ThrowIfFailed(sync, "/cgi-bin/school/set_arch_sync_mode");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 唯一键 | 只有 `student_userid` / `parent_userid` 唯一；`name` / `mobile` 不唯一 | 按姓名或手机号 upsert → 覆盖无关成员 |
| 单删 vs 批删 | 单删是 **GET + Query `userid`**；批删是 **POST + body `useridlist`** | 统一成一种动词 → 打坏 SCH1c |
| `children` 全量语义 | 空数组 = 忽略字段，不是清空 | 以为能解绑全部孩子 |
| `new_*_userid` | 每人**只能改一次** | 第二次改直接失败 |
| 删除顺序 | 学生有家长时不可删，须先删家长 | 批量删学生大量失败 |
| 批量结果 | 外层 `errcode` 与 `result_list` 是两层判断 | 只看外层码 → 漏失败明细 |
| 部门列表无分页 | E14 / E15 没有 cursor / limit，且不含子部门 | 以为一次能拿全校 |
| 家长手机号 | 第三方读不到；代开发需授权 | 把空 `mobile` 当数据缺失重试 |
| `external_userid` | 仅在家长已关注时返回，且跨校同值 | 当学校内唯一键 |
| 自动同步 | E16 **不可逆** | 误设「禁止同步」后无法恢复 |
| 批量上限 | ≤100 / ≤20 / ≤10 都是官方判定，SDK 不裁剪 | 传超量期望自动拆批 |

SDK 侧通用约束：不自动判错、不做本地必填与容量校验、不聚合批次；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 批量端点仍须看 `result_list` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

> 各端点的细分业务码（唯一键冲突、超容量、字段超长等）本批未逐条摘录 ⇒ 见 §7；批量端点逐条失败以 `result_list[].errcode` 表达。

其余业务码以官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）为准；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 细分业务错误码 | 接口注释未逐条列「唯一键冲突 / 超 3 万 / 字段超长」的具体码 ⇒ 未逐项核验 | 官方各端点页 + 90455 |
| `to_invite` 的实际投递形态 | 注释只说「发起邀请」，未说明邀请以何通道送达 ⇒ 未核验 | 官方 92325 / 92331 |
| `relation` 取值 | 注释是自由文本（≤32 字节），无枚举 ⇒ 未核验是否有官方推荐值 | 官方 92331 |
| `order` 与读取的可见性 | 部门侧 `order` 仅 API 设置后返回；学生 / 家长侧无排序字段 | 官方 92343 |
| 本族 QPS | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方 90454 |
| 「递归取子树」的官方示例 | 注释只说「先取子部门再递归」，未给官方示例代码 | 官方 92338 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Department.md](./Department.md)（班级 / 年级树）· [Basic.md](./Basic.md)（关注模式）· [Setting.md](./Setting.md)（手机号转外部联系人）· [Auth.md](./Auth.md)（家长 / 学生身份）· [HealthReport.md](./HealthReport.md)
- 邻域：客户联系域 `externalcontact/get`（`is_subscribe` / `subscriber_info`）· 消息推送域「学校通知」
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[创建学生 92325](https://developer.work.weixin.qq.com/document/path/92325) · [读取学生或家长 92337](https://developer.work.weixin.qq.com/document/path/92337) · [设置自动同步模式 92345](https://developer.work.weixin.qq.com/document/path/92345)
