# 家校管理配置 · 老师查看班级模式与手机号转外部联系人

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 家校管理配置族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolSettingService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Setting/IWechatWorkSchoolSettingService.cs)（父，承载全部 3 支）＋ 自建 / 第三方两个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Setting/IWechatWorkSchoolSettingService_Internal.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Setting/IWechatWorkSchoolSettingService_ThirdParty.cs)） |
| DTO 源文件 | [DataModels/School/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/)（本族 4 个文件 / 6 型，`SerializerClassName = School` → `SchoolJsonContext`） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1b 3 路由 / SCH2 父 3 + 仅自建·第三方两子 0 / SCH3 令牌绑定 / SCH4 DTO 登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 老师查看班级模式 [92652](https://developer.work.weixin.qq.com/document/path/92652)（自建 / 第三方同页）；手机号转外部联系人 [92506](https://developer.work.weixin.qq.com/document/path/92506)（自建 / 第三方同页） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1b / SCH2 / SCH3 / SCH4 断言 + 3 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

三支端点都是「家校全局配置 / 批量转译」，且官方**只向企业自建与第三方应用开放**：

| 端点 | 用途 |
|---|---|
| E1 / E2 `school/set_teacher_view_mode` · `get_teacher_view_mode` | 读写老师可查看的班级范围（全部 / 仅负责范围） |
| E3 `externalcontact/batch_to_external_userid` | 批量把家长手机号转成外部联系人 `external_userid` |

**不该用本族的场景**：

- 「是否允许扫码关注」「班级群创建方式」在 [Basic.md](./Basic.md)。
- 学生 / 家长本人的读写与批量增删改在 [User.md](./User.md)；E3 只做「手机号 → 外部联系人 ID」的转译，不建家长记录。
- **服务商代开发官方未开放本族**（官方文档树无对应页）⇒ 本族**不设代开发子接口**；代开发应用请改用自建应用或第三方应用配置。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal` / `ThirdParty`（代开发无契约面） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 `Wechat.AccessToken@Corp`（企业级，scope = `authCorpId`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 自建应用须配置到「家校沟通 - 读取和编辑家校通讯录的应用」；第三方应用须具有「家校沟通」使用权限。
- 第三方调用前须 `using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))`（`permanentCode` 是凭据，**不得落日志**）。
- **E3 在家长关注「学校通知」之后才会拿到 `external_userid`**：未关注的手机号即使「转换成功」也可能没有 `external_userid`（见 §5）。
- 手机号、`external_userid` 属可关联到具体个人的标识：示例一律假值，日志只记计数与状态。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 老师查看班级模式 | ✅ | ✅ | ❌（无文档页，无子接口） |
| E3 手机号转外部联系人 | ✅ | ✅ | ❌（无文档页，无子接口） |

⇒ 继承链上**恰好只有**自建与第三方两个子接口；加代开发子接口会被 SCH2 的 `BeEquivalentTo` 当场打红。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolSettingService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 3 | 否 |
| `IWechatWorkInternalSchoolSettingService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartySchoolSettingService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`。
- 端点索引：E1 `SetTeacherViewModeAsync` · E2 `GetTeacherViewModeAsync` · E3 `BatchToExternalUserIdAsync`。

---

## 4. 端点

### E1. `SetTeacherViewModeAsync` — 设置「老师可查看班级」的模式

```csharp
[Post("/cgi-bin/school/set_teacher_view_mode")]
Task<WechatWorkResponse> SetTeacherViewModeAsync(
    [Body] SetSchoolTeacherViewModeRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/set_teacher_view_mode` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 |

**请求参数**（`SetSchoolTeacherViewModeRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `ViewMode` | `view_mode` | `int?` | 是 | `1` = 全部班级；`2` = 仅负责范围内的班级 |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- 「负责范围」由部门管理员（班主任 / 任课老师）配置决定，不是本端点设置的内容；本端点只切换**查看口径**。

### E2. `GetTeacherViewModeAsync` — 获取「老师可查看班级」的模式

```csharp
[Get("/cgi-bin/school/get_teacher_view_mode")]
Task<GetSchoolTeacherViewModeResponse> GetTeacherViewModeAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/get_teacher_view_mode`（官方即 GET） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 |

**响应参数**（`GetSchoolTeacherViewModeResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `view_mode` | `ViewMode` | `int?` | `1` = 全部班级；`2` = 仅负责范围内的班级 |

### E3. `BatchToExternalUserIdAsync` — 手机号转外部联系人 ID

```csharp
[Post("/cgi-bin/externalcontact/batch_to_external_userid")]
Task<SchoolBatchToExternalUserIdResponse> BatchToExternalUserIdAsync(
    [Body] SchoolBatchToExternalUserIdRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/batch_to_external_userid`（**`externalcontact` 段**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 |

**请求参数**（`SchoolBatchToExternalUserIdRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Mobiles` | `mobiles` | `List<string>?` | 是 | 家长手机号列表，**一次最多 100 个** |

**响应参数**（`SchoolBatchToExternalUserIdResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `success_list[].mobile` | `SuccessList[].Mobile` | `string?` | 转换成功的手机号 |
| `success_list[].external_userid` | `SuccessList[].ExternalUserid` | `string?` | 外部联系人 ID；**家长关注后才返回**，可能为空 |
| `fail_list[].errcode` | `FailList[].ErrCode` | `int?` | 该手机号的失败返回码 |
| `fail_list[].errmsg` | `FailList[].ErrMsg` | `string?` | 失败描述 |
| `fail_list[].mobile` | `FailList[].Mobile` | `string?` | 失败的手机号 |

**本端点注意事项**

- **「出现在 success_list」≠「拿到了 `external_userid`」**：未关注「学校通知」的家长会成功返回手机号但没有外部联系人 ID。判可用性要看 `ExternalUserid` 是否非空。
- 单批 ≤100 由官方判定，SDK 不裁剪；越批请调用方自行分片。
- 手机号与 `external_userid` 都不得进日志 / 遥测。

**调用示例**（设置并读回查看模式 → 批量转手机号 → 分流处理结果）

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
    o.AppType = WechatAppType.Internal;          // 官方仅自建 / 第三方，无代开发子接口
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

var setting = provider.GetRequiredService<IWechatWorkInternalSchoolSettingService>();

// E1 / E2：1 全部班级 / 2 仅负责范围
var setView = await setting.SetTeacherViewModeAsync(
    new SetSchoolTeacherViewModeRequest { ViewMode = 2 }, cancellationToken);
WechatWorkException.ThrowIfFailed(setView, "/cgi-bin/school/set_teacher_view_mode");

var view = await setting.GetTeacherViewModeAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(view, "/cgi-bin/school/get_teacher_view_mode");
logger.LogInformation("老师查看模式 {Mode}", view.ViewMode);

// E3：单批 ≤100；手机号属个人数据，示例为假值
var convert = await setting.BatchToExternalUserIdAsync(
    new SchoolBatchToExternalUserIdRequest
    {
        Mobiles = new List<string> { "+8613800000001", "+8613800000002" },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(convert, "/cgi-bin/externalcontact/batch_to_external_userid");

// 注意：success_list 里也可能没有 external_userid（家长尚未关注「学校通知」）
foreach (SchoolMobileConvertSuccessItem item in convert.SuccessList ?? new List<SchoolMobileConvertSuccessItem>())
{
    bool followed = !string.IsNullOrEmpty(item.ExternalUserid);
    logger.LogInformation("手机号尾号 {Tail} 已关注：{Followed}",
        item.Mobile is { Length: >= 4 } m ? m[^4..] : "****", followed);
}

foreach (SchoolMobileConvertFailItem item in convert.FailList ?? new List<SchoolMobileConvertFailItem>())
{
    logger.LogWarning("转换失败：errcode={Errcode} errmsg={Errmsg}", item.ErrCode, item.ErrMsg);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 成功 ≠ 拿到 ID | `success_list` 元素可以只有 `mobile`、没有 `external_userid`（家长未关注） | 用 `SuccessList.Count` 当可触达家长数 |
| 应用类型 | 官方仅自建 + 第三方；无代开发子接口 | 注入不存在的 `IWechatWorkProviderSchoolSettingService` |
| 路由段 | E3 在 `externalcontact` 段，E1/E2 在 `school` 段 | 自造路由 |
| 与 Basic 的关注模式 | 本族 `view_mode` 是「老师看班级」，Basic 的 `subscribe_mode` 是「家长扫码关注」 | 两套开关混为一谈 |
| 与 User 的家长创建 | E3 只转译 ID，不创建家长记录 | 以为转完就能读家长详情 |
| 批量上限 | ≤100 是官方限制，SDK 不分片 | 传 500 个期望自动拆批 |
| 手机号日志 | 手机号 / `external_userid` 属个人标识 | 明文落日志 |

SDK 侧通用约束：不自动判错、不做本地必填与批量校验、令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 仍须逐条看 `fail_list` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

> E3 的逐条失败以 `fail_list[].errcode` 表达，外层 `errcode` 仍可能为 0；两者要分别判断。

其余业务码以官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）为准；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E3 成功条目的 `external_userid` 缺失语义 | 注释只说「关注后才返回」，未说明未关注时该条目是否仍进 `success_list` ⇒ 未核验 | 官方 92506 |
| E3 手机号格式要求 | 是否必须带国家码、是否接受座机 ⇒ 未核验 | 官方 92506 |
| 本族 QPS | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方访问频率说明（90454） |
| `view_mode` 对既有权限的即时性 | 切换后老师端是否立即生效 ⇒ 未核验 | 官方 92652 |
| 官方代开发无页 | 代开发文档树无本族页 ⇒ 本仓不设子接口（不是漏写） | 官方代开发目录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Basic.md](./Basic.md)（关注模式 / 二维码）· [User.md](./User.md)（家长创建与读取）· [Department.md](./Department.md)（部门管理员 = 「负责范围」的来源）
- 邻域：客户联系域 `externalcontact/get`（`external_userid` 的详情）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[老师可查看班级模式 92652](https://developer.work.weixin.qq.com/document/path/92652) · [手机号转外部联系人 92506](https://developer.work.weixin.qq.com/document/path/92506)
