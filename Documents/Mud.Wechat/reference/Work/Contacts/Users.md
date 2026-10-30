# 通讯录 · 成员管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Contacts` 域 · 成员管理子族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkUsersService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkUsersService.cs)（父，8 支读取面）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkUsersService_Internal.cs)（7）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkUsersService_ThirdParty.cs)（8）· [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkUsersService_Provider.cs)（空标记） |
| DTO 源文件 | [DataModels/Contacts/Users/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/Users/)（40 型，其中 U4 断言登记 28 型；命名空间为 `Mud.Wechat.Work.DataModels.Contracts.Users`，全仓唯一独插 `Contracts` 段） |
| 契约守卫 | [WechatUsersContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatUsersContractGuards.cs)（U1 路由表 / U2 层级与代开发空标记 / U3 令牌绑定 / U4 JSON 上下文）＋ [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5）＋ [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3） |
| 官方文档 | 读取 90196/90332/96255 · 摘要 90200/90336/96259 · 详情 90201/90337/96260 · ID 列表 96067/96021/96269 · 手机 95402/91693/96267 · 邮箱 95895/95892/96268 · openid 互转 90202/90338/96261 · 写入 90195–90199/90331–90335 · 二次验证 90203 · 加入二维码 91714 · 邀请 90975/91127 · 成员授权 94513/94514/94894 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 2 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：U1~U4 + G5 + TO1~TO3 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

企业微信通讯录的**成员侧**：读成员、把外部标识换成 `userid`、写成员，以及第三方「成员授权」模式下的授权关系读取。

| 能力块 | 端点 | 说明 |
|---|---|---|
| 读取面（三类应用一致） | E1–E8 | 按 `userid` 读、按部门读摘要/详情、游标分页取 ID、手机号/邮箱换 `userid`、`userid`↔`openid` 互转 |
| 写入面（自建 + 第三方各声明一次） | E9–E13 | 创建 / 更新 / 删除 / 批量删除成员、邀请成员 |
| 自建专属 | E14–E15 | 登录二次验证放行、获取加入企业二维码 |
| 第三方专属（成员授权模式） | E16–E18 | 授权成员列表分页、查成员是否已授权、选人 ticket 换 open_userid |

三件事要先立住：

1. **本族的「能读到谁」由令牌对应应用的可见范围决定**，不是由参数决定；越界成员读不到，SDK 不做本地预判。
2. **写入端点的咽喉是 secret 而不是接口**：官方权限口径为「仅通讯录同步助手（自建）或第三方通讯录应用可调用」——同一个 `access_token` 路由键，但换令牌用的 `corpsecret` 必须是通讯录同步那一个。
3. **代开发子接口是空标记**（U2 断言其 `DeclaredOnly` 端点集为空）：代开发只继承 E1–E8 读取面，官方未向其开放写入。

同域兄弟：部门见 [Departments.md](./Departments.md)，标签见 [Tags.md](./Tags.md)，异步导入见 [Batch.md](./Batch.md)，异步导出见 [Export.md](./Export.md)，隐藏规则见 [ContactRules.md](./ContactRules.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（`Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入（U3） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager`（错配抛 `WechatTokenOwnerMismatchException`） |

写入用的 secret 是通讯录同步助手那一个（`AgentSecret` 的配置值本身是凭据，不得进日志）：

```csharp
// 自建：把 AgentSecret 配成「通讯录同步助手」的 secret，再解析自建子接口
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "contact-sync";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:ContactSyncSecret"]; // 凭据：只从配置读，勿硬编码、勿落日志
});
```

第三方/代开发先切授权企业作用域（`permanent_code` 只写参数名，**不得进日志、遥测或异常消息**）：

```csharp
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var profile = await thirdPartyUsers.GetUserAsync(userId, ct); // 第三方在 userid 字段位返回 open_userid
}
```

### 2.2 权限与业务前置

- 官方权限：读取面要求应用对目标成员/部门有**查看权限**；写入面要求「通讯录同步助手或第三方通讯录应用」。
- **敏感字段停返时间线**（接口注释留档）：自 **2022-06-20** 起新建的自建与代开发应用，不再返回头像、性别、手机、邮箱、企业邮箱、个人二维码、地址（需 oauth2 手工授权）；自 **2022-08-15** 起「通讯录同步」新增 IP 不能再调用 E1 `user/get` 与部门族 `department/list`，官方建议改用 E4 `user/list_id` / 部门 `simplelist`。
- 第三方读 `name` 自 **2019-12-30** 起以 `userid` 代替。
- **E5 / E6 的熔断**：手机号或邮箱出错次数超过企业人数上限的 20% ⇒ 该接口 1 天不可调用。
- 批量闸：`useridlist` ≤200（E12）；邀请 user ≤1000、party ≤100、tag ≤100（E13）；成员 `department` ≤100 个、每部门成员 ≤3 万（E9）；分页 `limit` E4 为 1~10000、E16 默认与最大 1000。
- `userid` 企业内唯一、1~64 字节、唯一性检查忽略大小写；`mobile`/`email` 均企业内唯一且**不能同时为空**（E9）。
- **个人数据**：`mobile`、`email`、`biz_mail`、`address`、`avatar`、`qr_code`、`extattr` 都是个人数据，示例一律用假值，且不得写入日志、遥测或异常消息。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E8 读取面 | ✅ | ✅ | ✅（继承父接口） |
| E9–E13 写入 / 邀请 | ✅ 自建子接口 | ✅ 第三方子接口 | ❌ |
| E14 二次验证、E15 加入二维码 | ✅ | ❌ | ❌ |
| E16–E18 成员授权三支 | ❌ | ✅ | ❌ |

E9–E13 的路由与 DTO 完全相同，但**在自建与第三方两支子接口各自声明**（官方分文档形态、令牌作用域不同），故 U1 路由表为 23 处声明 / 18 条去重路由。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkUsersService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 8 | 否 |
| `IWechatWorkInternalUsersService` | `Mud.Wechat.Work` | 自建子接口 | 7 | 是 |
| `IWechatWorkThirdPartyUsersService` | `Mud.Wechat.Work` | 第三方子接口 | 8 | 是 |
| `IWechatWorkProviderUsersService` | `Mud.Wechat.Work` | 代开发子接口（**空标记**） | 0 | 是 |

注册组 `Contact`（六子族共用），模块枚举 `WechatModule.Contact`，装配入口 **`AddContactApi()`（单数，勿改成 `AddContactsApi`）**。

```csharp
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddContactApi());
```

**注入子接口，不注入父接口。** 自建应用要用写入面就注入 `IWechatWorkInternalUsersService`；只需读取面时子接口同样能调（继承来的 E1–E8）。

## 4. 端点

### E1. `GetUserAsync` — 读取成员

```csharp
Task<UserInfo> GetUserAsync([Query("userid")] string userid, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/user/get` |
| 声明接口 | 父接口（三类应用均可调） |
| 令牌 | Query `access_token` |

**请求参数**

| 位置 | 名称 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| Query | `userid` | `string` | 是 | 成员 UserID，对应管理端账号，企业内唯一、不区分大小写、1~64 字节 |

**响应参数**（`UserInfo` **直接继承 `WechatWorkResponse`**，`errcode`/`errmsg` 与业务字段同层）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `userid` | `UserId` | `string` | 成员账号；**第三方调用时该字段位返回 open_userid** |
| `open_userid` | `OpenUserId` | `string?` | 全局唯一标识（同一服务商跨应用同值，最长 64 字节），仅第三方 |
| `name` | `Name` | `string?` | 名称；第三方以 userid 代替，代开发需管理员授权 |
| `department` | `Department` | `List<int>?` | 所属部门（仅返回应用有查看权限的部门；成员授权模式固定返回根部门 1） |
| `order` | `Order` | `List<long>?` | 部门内排序值，**数值越大越靠前**，与 `department` 一一对应；成员授权模式不返回 |
| `position` / `external_position` | 同名 | `string?` | 职务 / 对外职务（后者设置后优先对外展示） |
| `mobile` / `email` / `biz_mail` / `telephone` / `address` / `alias` | `Mobile` / `Email` / `BizMail` / `Telephone` / `Address` / `Alias` | `string?` | 均需授权，且受 §2.2 停返时间线约束 |
| `gender` | `Gender` | `string?` | **字符串**形态：`"0"` 未定义 / `"1"` 男 / `"2"` 女（官方参数表标整数、示例按字符串传输） |
| `avatar` / `thumb_avatar` / `qr_code` | `Avatar` / `ThumbAvatar` / `QrCode` | `string?` | 头像 / 缩略图 / 个人二维码 URL |
| `is_leader_in_dept` | `IsLeaderInDept` | `List<int>?` | 是否部门负责人（1/0，数量须与 `department` 一致） |
| `direct_leader` | `DirectLeader` | `List<string>?` | 直属上级（最多 1 个，仅返回可见范围内） |
| `main_department` | `MainDepartment` | `int?` | 主部门（应用对主部门有查看权限时才返回） |
| `extattr.attrs[]` | `ExtAttr` | `UserExtAttr?` | 扩展属性：`type` 0 文本（`text.value` ≤64 字符）/ 1 网页（`web.url` 须带 http(s)、`web.title` ≤12 字符） |
| `external_profile` | `ExternalProfile` | `UserExternalProfile?` | 对外属性：`external_corp_name` / `wechat_channels` / `external_attr[]`（比 `extattr` 多 `type` 2 小程序） |
| `status` | `Status` | `int?` | 激活状态：1 已激活 / 2 已禁用 / 4 未激活 / 5 退出企业 |
| `enable` / `wxplugin_status` / `english_name` | 同名 | `int?` / `int?` / `string?` | **官方现行参数表未单列**，为兼容既有部署与 Senparc 契约保留 ⇒ 判空使用（见 §7） |

**本端点注意事项**

- 返回值本身就是响应体：`WechatWorkException.ThrowIfFailed(user, "/cgi-bin/user/get")` 之后才能读字段——业务失败时 HTTP 仍是 200。
- 2022-06-20 之后新建的应用拿到的是**阉割版** `UserInfo`，字段缺失不等于接口故障；要完整资料须走 oauth2 手工授权。

### E2. `GetUserSimpleListAsync` — 获取部门成员（摘要）

```csharp
Task<GetUserSimpleListResponse> GetUserSimpleListAsync(
    [Query("department_id")] int departmentId,
    [Query("fetch_child")] int? fetchChild = null,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/user/simplelist`（`simplelist` 全连写、**无下划线**） |
| 声明接口 | 父接口 |

**请求参数**

| 位置 | 名称 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| Query | `department_id` | `int` | 是 | 部门 ID；应用须有该部门查看权限 |
| Query | `fetch_child` | `int?` | 否 | `1` 递归子部门成员；`0` 或不填仅直属 |

**响应参数**：`userlist[]` → [UserSimpleInfo](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/Users/UserSimpleInfo.cs)：`userid`（`string`）、`name`（`string?`，第三方以 userid 代替）、`department`（`List<int>?`）、`open_userid`（`string?`）。

**本端点注意事项**：摘要只有四个字段，需要手机/邮箱/职务等请用 E1 或 E3。

### E3. `GetUserDetailListAsync` — 获取部门成员详情

```csharp
Task<GetUserDetailListResponse> GetUserDetailListAsync(
    [Query("department_id")] int departmentId,
    [Query("fetch_child")] int? fetchChild = null,
    CancellationToken cancellationToken = default);
```

- 路由 `GET /cgi-bin/user/list`；`userlist[]` 是 `List<UserInfo>`（字段集同 E1 的批量版）。
- **官方提示本端点性能较低**，大范围同步请改用 E4 + E1，或导出族 [Export.md](./Export.md) 的异步任务。

### E4. `ListUserIdsAsync` — 获取成员 ID 列表

```csharp
Task<ListUserIdsResponse> ListUserIdsAsync([Body] ListUserIdsRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/user/list_id`（查询语义却走 POST，守卫 U1 锁 `PostAttribute`） |
| 声明接口 | 父接口 |

**请求参数**（`ListUserIdsRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `cursor` | `Cursor` | `string?` | 否 | 分页游标，由上一次响应返回；**首次不填** |
| `limit` | `Limit` | `int?` | 否 | 单次请求量，取值 **1~10000** |

**响应参数**（`ListUserIdsResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `dept_user[]` | `DeptUser` | `List<UserDepartmentInfo>?` | 用户-部门关系；同一成员在多个部门下会有多条记录 |
| `dept_user[].userid` | `UserId` | `string?` | **自建应用返回** |
| `dept_user[].open_userid` | `OpenUserId` | `string?` | **第三方 / 代开发返回** |
| `dept_user[].department` | `Department` | `int?` | 部门 ID |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标；**返回空表示已无更多** |

**调用示例**（本族主示例：游标分页 + 双形态取值）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Contracts.Users;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "contact-sync";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:ContactSyncSecret"]; // 凭据：通讯录同步助手 secret
});
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

var users = provider.GetRequiredService<IWechatWorkInternalUsersService>();

string? cursor = null;
var accounts = new List<string>();
while (true)
{
    var resp = await users.ListUserIdsAsync(
        new ListUserIdsRequest { Cursor = cursor, Limit = 10000 }, cancellationToken);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/user/list_id");

    foreach (UserDepartmentInfo rel in resp.DeptUser ?? new List<UserDepartmentInfo>())
    {
        // 自建返回 userid、第三方/代开发返回 open_userid：同一份代码两个字段位都可能为空。
        string account = rel.UserId ?? rel.OpenUserId ?? string.Empty;
        if (account.Length > 0) accounts.Add(account);
    }

    if (string.IsNullOrEmpty(resp.NextCursor)) break; // 空游标 = 已无更多
    cursor = resp.NextCursor;
}
```

### E5. `GetUserIdByMobileAsync` — 手机号获取 userid

```csharp
Task<GetUserIdByMobileResponse> GetUserIdByMobileAsync([Body] GetUserIdByMobileRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/getuserid`（**全连写**）。
- 请求：`mobile`（`string`，必填，5~32 字节）。响应：`userid`（`string`；第三方获取的是密文 userid）。
- **熔断**：出错次数超过企业人数上限的 20% ⇒ 1 天不可调用；不要用它做「批量校验手机号」。

### E6. `GetUserIdByEmailAsync` — 邮箱获取 userid

```csharp
Task<GetUserIdByEmailResponse> GetUserIdByEmailAsync([Body] GetUserIdByEmailRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/get_userid_by_email`（**带下划线**，与 E5 拼写风格相反）。
- 请求：`email`（`string`，必填）、`email_type`（`int?`，`1` 企业邮箱（默认）/ `2` 个人邮箱）。
- 响应：`userid`（已升级 openid 的代开发/第三方为密文 userid）。同样有 20% 错误率熔断。

### E7. `ConvertUserIdToOpenIdAsync` — userid 转 openid

```csharp
Task<ConvertUserIdToOpenIdResponse> ConvertUserIdToOpenIdAsync([Body] ConvertUserIdToOpenIdRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `UserId` | `string` | 是 | 企业内成员 ID；成员须在应用可见范围内 |
| `agentid` | `AgentId` | `int?` | 否 | 发红包的应用 ID；仅企业转账/红包时可不填，不填时响应不返回 `appid` |

响应：`openid`（`string`）、`appid`（`string?`，发红包时会用到）。
**前置**：成员须用微信登录企业微信，或关注微信插件（原企业号）；外部联系人请走外部联系人的转换接口（不属本族）。

### E8. `ConvertOpenIdToUserIdAsync` — openid 转 userid

```csharp
Task<ConvertOpenIdToUserIdResponse> ConvertOpenIdToUserIdAsync([Body] ConvertOpenIdToUserIdRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/convert_to_userid`；请求 `openid`（`string`，必填，企业支付后返回的 openid）；响应 `userid`（`string`）。
- 典型用于企业支付结果回查；管理组须对该 openid 对应成员有查看权限。

### E9. `CreateUserAsync` — 创建成员

```csharp
Task<CreateUserResponse> CreateUserAsync([Body] CreateUserRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/user/create` |
| 声明接口 | 自建子接口 **与** 第三方子接口（同路由、各声明一次） |

**请求参数**（`CreateUserRequest`，节选必填与有约束项）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `UserId` | `string` | 是 | 1~64 字节，仅数字/字母及 `_-@.`，**首字符须数字或字母** |
| `name` | `Name` | `string` | 是 | 1~64 个 UTF-8 字符 |
| `mobile` | `Mobile` | `string?` | 条件 | 与 `email` **不能同时为空**；大陆可省 `+86`，其他地区须带国际码 |
| `email` | `Email` | `string?` | 条件 | 6~64 字节，企业内唯一 |
| `department` | `Department` | `List<int>?` | 否 | ≤100 个；不填或为 0 落「其他（待设置部门）」；**填了不存在的部门会被自动新建** |
| `order` | `Order` | `List<long>?` | 否 | 须与 `department` 个数一致；**值越大越靠前**，范围 `[0, 2^32)` |
| `is_leader_in_dept` | `IsLeaderInDept` | `List<int>?` | 否 | 个数须与 `department` 一致（1 是 / 0 否） |
| `direct_leader` | `DirectLeader` | `List<string>?` | 否 | 直属上级，最多 1 个 |
| `biz_mail` | `BizMail` | `string?` | 否 | 6~64 字节，企业内唯一；未填则系统生成默认企业邮箱（可改一次） |
| `avatar_mediaid` | `AvatarMediaId` | `string?` | 否 | 素材接口上传得到的 media_id |
| `to_invite` | `ToInvite` | `bool?` | 否 | 默认 `true`；官方每天下发一次邀请、最多 3 个工作日 |
| `extattr` / `external_profile` | 同名 | 对象 | 否 | 扩展属性 / 对外属性；`extattr` 须先在管理端建字段，否则被忽略 |
| `enable` / `status` 类 | `Enable` | `int?` | 否 | 1 启用 / 0 禁用 |
| 其余 | `english_name`、`alias`、`position`、`external_position`、`nickname`、`telephone`、`address`、`main_department`、`gender` | — | 否 | 长度与形态约束见 DTO 注释 |

**响应参数**：`created_department_list.department_info[]`（`name` / `id`）——**因填写不存在的部门而被官方自动新建**的部门回显。

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException
using Mud.Wechat.Work.DataModels.Contracts.Users;      // 请求/响应 DTO
// 片段自足前置：与 E4 使用同一个自建子接口实例。
var users = provider.GetRequiredService<IWechatWorkInternalUsersService>();

var created = await users.CreateUserAsync(
    new CreateUserRequest
    {
        UserId = "zhangsan",                 // 假值
        Name = "张三",                        // 假值
        Mobile = "13800000000",              // 假值；与 email 不能同时为空
        Department = new List<int> { 2 },
        Order = new List<long> { 1 },        // 成员 order：值大靠前（与部门相反）
        ToInvite = true,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/user/create");

foreach (CreatedDepartmentInfo auto in created.CreatedDepartmentList?.DepartmentInfo ?? new List<CreatedDepartmentInfo>())
{
    // 传了不存在的部门 id 时官方自动新建并在此回显——同步程序必须收下这些 id。
    logger.LogInformation("官方自动新建部门 id={DepartmentId}", auto.Id);
}
```

### E10. `UpdateUserAsync` — 更新成员

```csharp
Task<WechatWorkResponse> UpdateUserAsync([Body] UpdateUserRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/update`（自建 + 第三方各声明一次）；应答仅 `errcode`/`errmsg`。
- **除 `userid` 外全部可选，未传入的字段不更新**（不是「传空即清空」）。
- `new_userid`：仅**系统自动生成**的 `userid` 允许改一次，新值由该字段给。
- `biz_mail_alias`：`UserBizMailAlias.item`（≤5 个）为**覆盖式**更新，传空结构或空数组即清空。
- `mobile`：已激活成员的手机号需本人改，此时该参数**被忽略但不报错**。
- 官方提示 `BizMail` / `BizMailAlias` 与其它字段的更新**不具原子性**。

### E11. `DeleteUserAsync` — 删除成员

```csharp
Task<WechatWorkResponse> DeleteUserAsync([Query("userid")] string userid, CancellationToken cancellationToken = default);
```

- 路由 **`GET`** `/cgi-bin/user/delete`（写入却走 GET，U1 锁 `GetAttribute`）。
- 若成员绑定了腾讯企业邮，删除成员会**同时删除邮箱账号**。

### E12. `BatchDeleteUsersAsync` — 批量删除成员

```csharp
Task<WechatWorkResponse> BatchDeleteUsersAsync([Body] BatchDeleteUsersRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/batchdelete`；请求 `useridlist`（`List<string>`，**≤200**）。
- **存在无效 `userid` 即整批失败**（无部分删除）——与标签族「部分非法仍 `errcode=0`」的语义**相反**，见 §5。

### E13. `InviteMembersAsync` — 邀请成员

```csharp
Task<InviteMembersResponse> InviteMembersAsync([Body] InviteMembersRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/batch/invite`（**路由在 `batch/` 下，归属成员域**，不属异步导入族） |
| 声明接口 | 自建子接口 + 第三方子接口 |

- 请求：`user`（`List<string>?` ≤1000）、`party`（`List<int>?` ≤100）、`tag`（`List<int>?` ≤100），**三者不能同时为空**。
- 响应：`invaliduser`（`List<string>?`）/ `invalidparty`（`List<int>?`）/ `invalidtag`（`List<int>?`）。
- 同一用户只需邀请一次；**邀请频率是异步检查**，本接口返回成功不代表接收者一定收到邀请消息。

### E14. `CompleteSecondaryAuthAsync` — 登录二次验证（仅自建）

```csharp
Task<WechatWorkResponse> CompleteSecondaryAuthAsync([Query("userid")] string userid, CancellationToken cancellationToken = default);
```

- 路由 `GET /cgi-bin/user/authsucc`（`authsucc` 全连写）。
- 开启二次验证后，成员登录跳转企业验证页并带 `code`；企业侧校验完须调本接口放行，否则成员进不了企业。

### E15. `GetJoinQrcodeAsync` — 获取加入企业二维码（仅自建）

```csharp
Task<GetJoinQrcodeResponse> GetJoinQrcodeAsync([Query("size_type")] int? sizeType = null, CancellationToken cancellationToken = default);
```

- 路由 `GET /cgi-bin/corp/get_join_qrcode`（**路由在 `corp/` 下，归属成员域**）。
- `size_type`：`1` 171×171 / `2` 399×399 / `3` 741×741 / `4` 2052×2052；不传由官方默认。
- 响应 `join_qrcode` **有效期 7 天**，过期须重新获取；须拥有通讯录管理权限并用通讯录同步 secret。

### E16. `ListMemberAuthAsync` — 获取成员授权列表（仅第三方）

```csharp
Task<ListMemberAuthResponse> ListMemberAuthAsync([Body] ListMemberAuthRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/list_member_auth`；仅当企业授权模式为「**成员授权**」时有意义。
- 请求：`cursor`（首次不填）、`limit`（默认与最大 **1000**）。
- 响应：`member_auth_list[].open_userid`、`next_cursor`（空串即最后一页）。

### E17. `CheckMemberAuthAsync` — 查询成员是否已授权（仅第三方）

```csharp
Task<CheckMemberAuthResponse> CheckMemberAuthAsync([Body] CheckMemberAuthRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/check_member_auth`；请求 `open_userid`（必填）。
- 响应 `is_member_auth` 为 **`bool?`**——用可空承载是为了不把「字段缺失」误读成 `false`。

### E18. `ListSelectedTicketUsersAsync` — 选人 ticket 换用户（仅第三方）

```csharp
Task<ListSelectedTicketUserResponse> ListSelectedTicketUsersAsync([Body] ListSelectedTicketUserRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/user/list_selected_ticket_user`；请求 `selected_ticket`（选人 JS-SDK 返回的票据，必填）。
- 响应：`operator_open_userid`、`open_userid_list`（**可见范围内**）、`unauth_open_userid_list`（**可见范围外**）、`total`。
- 两个列表都要读：只取 `open_userid_list` 会静默丢掉用户实际选中但不可见的成员。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 成员 `order` vs 部门 `order` | 成员**值大靠前**；部门**值小靠前**（语义相反） | 共用一个比较器 → 组织树静默错序 |
| `userid` vs `open_userid` | E1 第三方在 **`userid` 字段位**返回 open_userid；E4 按应用类型分别落两个字段 | 只读 `userid` → 第三方场景拿到的是全局标识却当账号用 |
| E12 批量删除 vs 标签增删成员 | 前者**无效即整批失败**；后者部分非法仍 `errcode=0` 附非法清单 | 用同一套判空逻辑 → 一半漏处理 |
| E5 `getuserid` vs E6 `get_userid_by_email` | 一个全连写、一个带下划线 | 「顺手统一」拼写 → 路由不匹配官方 |
| E13 邀请（`/batch/invite`）、E15 二维码（`/corp/...`） | 路由前缀在别处，**归属仍是成员域** | 按前缀归位到导入域/ corp 域 → 守卫与文档错位 |
| `gender` | `string?`（`"0"`/`"1"`/`"2"`），官方参数表标整数 | 改回 `int` → 反序列化失败 |
| E1 的返回值 | `UserInfo` **继承 `WechatWorkResponse`**，`errcode` 与业务字段同层 | 忘了判错 → 把失败响应当「字段为空的成员」 |
| `enable` / `wxplugin_status` / `english_name` | 官方现行参数表未单列，本仓为兼容保留 | 当必然存在来用 → 空引用 |
| 查询走 POST 的四支 | E4、E5、E6、E7/E8 及 E16–E18 均为 POST | 改成 GET → 打坏 U1 与 G5 |
| 代开发子接口 | 空标记、只继承读取面（U2 断言端点集为空） | 找 `IWechatWorkProviderUsersService.CreateUserAsync` → 不存在 |
| E14/E15 | 仅自建；E16–E18 | 仅第三方——注入错子接口即编译不过 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌失效自愈集合 `{40014, 42001, 42007}`（企业级另含 `42009/42011`）由基座处理；令牌走 Query 是官方契约（`MUD005`）；**本域接口不承载通讯录变更事件**（回调侧由 `Mud.Wechat.Work.Callback` 以通配键 `"*"` 路由）。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 40014 | `InvalidAccessToken` | 令牌不合法 | 自愈集合；写入面多为 secret 用错（非通讯录同步） |
| 42001 | `ExpiredAccessToken` | 令牌过期 | 自愈集合 |
| 42007 | `RelatedAccessTokenInvalid` | 关联令牌失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

本域接口注释**未逐端点留档业务错误码**（仅给出 E5/E6 的 20% 错误率熔断、E12 的整批失败等行为口径）⇒ 具体码值属**未逐项核验**，见 §7。判错统一走 `WechatWorkException.ThrowIfFailed(response, requestUri)`，不要按码值写死业务分支。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 逐端点业务错误码 | 接口注释与守卫均未登记成员域码值（如 userid 不存在、mobile 已存在一类） ⇒ **未逐项核验**，不得凭常见 SDK 记忆写进代码判据 | 官方 90196/90195 各页错误码表 |
| 逐端点数值 QPS | 接口注释只在通讯录查看权限族留了频率；成员域各端点频率上限**未逐项核验** | 官方各端点页 |
| `enable` / `wxplugin_status` / `english_name` | DTO 注明「官方现行参数表未单列，为兼容既有部署与 Senparc 契约保留」⇒ 是否仍在实际响应中出现**未逐项核验** | 官方 90196 |
| 代开发能否调 E4 及其返回形态 | 代开发子接口空标记、继承父接口读取面；但注释只描述自建（`userid`）与第三方（`open_userid`）两种 secret 口径 | 官方 96269 |
| `hand_join` / 部门 `parentname` | 本域接口、DTO、六支守卫三类证据中**均未出现**，本仓不建模 | 见域 README 缺口表 |
| 成员授权模式的授权判定细则 | E16–E18 只留分页与字段口径，「哪些情况下 `is_member_auth` 为真」未留档 | 官方 94513/94514 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§1 层级不等宽、§3 `order` 语义反转、§4 信息面差异与停返时间线、§5 原子性差异）
- 同域：[Departments.md](./Departments.md) · [Tags.md](./Tags.md) · [Batch.md](./Batch.md) · [Export.md](./Export.md) · [ContactRules.md](./ContactRules.md)
- 跨域：通讯录变更事件走回调（`Mud.Wechat.Work.Callback`，通配键 `"*"`），见 [AGENTS.md §5.5](../../../../../AGENTS.md)
- 官方页：[读取成员（自建 90196）](https://developer.work.weixin.qq.com/document/path/90196) · [获取成员 ID 列表（96067）](https://developer.work.weixin.qq.com/document/path/96067) · [创建成员（90195）](https://developer.work.weixin.qq.com/document/path/90195) · [邀请成员（90975）](https://developer.work.weixin.qq.com/document/path/90975)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
