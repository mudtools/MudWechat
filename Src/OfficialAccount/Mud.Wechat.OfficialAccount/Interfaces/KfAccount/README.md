# KfAccount 域 · 客服账号管理

微信公众号 / 服务号「客服消息 → 客服管理」子分组：**7 支端点**（客服列表 / 在线客服列表 / 添加 / 修改 / 删除 / 设头像 / 邀请绑定）。
接口 [IMpKfAccountService.cs](./IMpKfAccountService.cs)，注册组 `KfAccount`，
模块枚举 `MpModule.KfAccount`（[MpModule.cs](../../Extensions/MpModule.cs)；`AddKfAccountApi()` ⇒ 生成入口
`AddKfAccountWebApiHttpClient()`，见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpKfContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpKfContractGuards.cs)
> （`KF1`~`KF9`，客服管理与会话控制共用这一支守卫）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`/`QT2`）、
> [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`/`RC3`）
> 与本仓 DTO（[DataModels/KfAccount/](../../../Mud.Wechat.OfficialAccount/KfAccount/)）。
> 未在以上四处留档的官方约束一律进文末「已知留档缺口」，正文不引用外部官方文档表述。

## 端点速览

7 条路由逐条钉在 `KF1`（守卫号 `KF1`），并含一条前缀分布断言：
`KfAccountRoutes.Count(StartsWith("/cgi-bin/")) == 2` —— **只有两支列表接口带 `/cgi-bin/`，其余 5 支没有**。
本域在 `QT1` 白名单登记为「`IMpKfAccountService`（7 端点）」，`QT2` 断言接口 `[Token]` 的 `Name = access_token`。

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetKfListAsync` | GET | `/cgi-bin/customservice/getkflist` | Query 注入 `access_token`（MUD005） | `[Query("business_id")] string?`，普通账号不填 |
| 2 | `GetOnlineKfListAsync` | GET | `/cgi-bin/customservice/getonlinekflist` | 同上 | 同上；返回 `status`/`accepted_case`/`kf_openid` |
| 3 | `AddKfAccountAsync` | POST | `/customservice/kfaccount/add` | 同上 | **无 `/cgi-bin` 前缀**；官方无 `password` 字段 |
| 4 | `UpdateKfAccountAsync` | POST | `/customservice/kfaccount/update` | 同上 | 官方修改页**无 `business_id`** ⇒ 不与添加共用 DTO |
| 5 | `DelKfAccountAsync` | POST | `/customservice/kfaccount/del` | 同上 | 官方未声明与既有会话/标签的联动 |
| 6 | `UploadKfHeadImgAsync` | POST | `/customservice/kfaccount/uploadheadimg` | 同上 | **multipart**，文件字段名必须 `media`（≤5M）；`kf_account` 在 **Query** |
| 7 | `InviteKfWorkerAsync` | POST | `/customservice/kfaccount/inviteworker` | 同上 | `kf_account` + `invite_wx` 均必填 |

**适用范围**：官方各页为「小程序 ✔ / 公众号 仅认证 / 服务号 仅认证 / 小游戏 ✔」。
本 SDK **只覆盖公众号与服务号形态**、**非服务号专属** ⇒ **不引入 `AccountType` 本地闸**，
账号面不符由官方错误码表达（与 `CM8` 同一条裁决口径）。

**跨线边界（`KF1` remarks + `MP-X1` 零交叠裁决）**：客服**管理员**（`kfaccount/setadmin`、`kfaccount/canceladmin`）
与**微信客服绑定**（`work/bind`、`work/get`、`work/unbind`）共 5 条端点归**小程序线**，
**不属于本域、也不在本线的路由计数里**。看到 `customservice` 前缀不要顺手补齐。

## 数量上限：每个账号最多 100 个客服账号

`MpAddKfAccountRequest.MaxKfAccountCount = 100`，超限返回 `65405`
（`MpErrorCodes.KfAccountCountExceeded`）。`KF8` 把该值连同三个长度常量钉住，并在守卫 remarks 里
明确**这些常量是「调用方前置校验依据」**——即 SDK 提供常量供宿主自检，但**接口本身不做本地拦截**
（100 是账号级配置态，本地计数易与官方态漂移，命中仍以 `65405` 为准）。

## 账号形状：`前缀(≤10) @ 公众号微信号(≤30)`，昵称 ≤16 字

`kf_account` = 「账号前缀@公众号微信号」：

| 约束 | 值 | 留档位置 |
|---|---|---|
| 前缀最大长度 | **10**，且只允许英文 / 数字 / 下划线 | `MpAddKfAccountRequest.AccountPrefixMaxLength`（`KF8`） |
| 后缀（公众号微信号）最大长度 | **30** | `AccountSuffixMaxLength` |
| 昵称最大长度 | **16 字** | `NicknameMaxLength`（超限 `65403`） |
| 账号形状不合法 | `65404`（`IllegalKfAccount`） | 官方错误码页 |

**前置条件**：公众号**须先在公众平台官网设置微信号**，否则连可用后缀都取不到
（接口 remarks 只留这条前置，未把它与具体错误码挂钩 ⇒ 命中仍以官方返回为准）。
形状不合法本身对应 `65404`（`IllegalKfAccount`）。

## 「未绑定微信号的客服账号不能投入使用」

添加成功 ≠ 可用。`65402`（`KfAccountNotBound`）表示账号尚未绑定微信号 ⇒
添加后必须经 `InviteKfWorkerAsync`（`invite_wx` = 待邀请的微信号）完成绑定，
或由该微信号在公众平台侧确认邀请。邀请面的 6 枚专有码（`65407`~`65412`）全部留档在接口 remarks：

`65407` 已是本账号客服 / `65408` 已发送过邀请 / `65409` 无效微信号 /
`65410` 对方绑定客服账号数达上限 / `65411` 存在待确认邀请 / `65412` 该客服账号已绑定微信号。

> 与 `KfSession` 域的分工：**绑定**（能不能上岗）在本域，**在线与接待**（谁在接）在
> [KfSession 域](../KfSession/README.md)。

## 头像上传：multipart 直传，且 `kf_account` 在 Query 不在表单

`KF3` 是本域最容易被改坏的一支断言：

- `UploadKfHeadImgAsync` 必须**恰有一个 `IFormContent` 参数**且标 `[MultipartForm]`——
  写成 `[FormContent]` 时生成器发射的调用在运行时装不上（表现为 `CS1061`）；
- 文件字段名**必须为 `media`**，文件 **≤5M**（格式不对 ⇒ `40005` invalid file type）；
- `kf_account` 是 **Query** 参数（官方形态
  `…/uploadheadimg?access_token=…&kf_account=KF_ACCOUNT`），不是表单字段——
  `KF3` 直接断言该参数的 Query 名为 `kf_account`；
- **头像是直传文件，不经素材域**：本端点**不引用** `media_id`，也不要求先调 `IMpMediaService`
  （若看到「先上传素材再回填」的说法，属未经本仓留档的推断）。
- `IFormContent` 由调用方构建，本仓不提供实现类，流生命周期由调用方掌握。

## 两套客服 DTO：`kf_id` 一为字符串一为数值

`KF5` 用精确 JSON 名集合钉住两页返回形态**不同**，因此建两套类型：

| 类型 | 官方 JSON 键 | 关键点 |
|---|---|---|
| `MpKfAccount`（`getkflist`） | `kf_account` / `kf_nick` / `kf_headimgurl` / `kf_id` | `KfId` 是 **`string?`** |
| `MpKfOnlineAccount`（`getonlinekflist`） | `kf_account` / `status` / `kf_id` / `accepted_case` / `kf_openid` | `KfId` 是 **`long`**；`Status` 是 `int` |

- `status` 取值见 `MpKfOnlineStatus`（`0` 不在线 / `1` web 在线）——该常量类**必须**放在 Abstractions
  （[MpKfConstants.cs](../../../Mud.Wechat.OfficialAccount/MpKfConstants.cs)），
  **不得**落进 `DataModels.KfAccount` 命名空间（`KF9` 断言：纯常量类不进 DTO 命名空间，否则会被扫进 `*JsonContext`）；
- `accepted_case` 是该客服**正在接待的会话数** ⇒ 可直接用于「按负载分配客服」。

## 请求侧 DTO 不共用（官方页面形状不一致，照录）

`KF5` 逐类锁定：`MpAddKfAccountRequest{kf_account, nickname, business_id}`、
`MpUpdateKfAccountRequest{kf_account, nickname}`（修改页无 `business_id`）、
`MpDelKfAccountRequest{kf_account}`、`MpInviteKfWorkerRequest{kf_account, invite_wx}`。
「加个可选字段然后合并成一个 DTO」会破坏官方形态，`KF5` 会红。

## `password` 型字段的处置（凭据约定）

官方添加客服账号页**没有** `password` 字段 ⇒ `KF5` 有一条反向断言：
`MpAddKfAccountRequest` 的属性集 **`NotContain("password")`**。
按本仓凭据约定：即使日后官方引入口令类字段，也只能以**参数名**出现在正文与示例里，
真值不得进日志、遥测或异常消息；本文与示例一律不出现任何凭据真值。
`invite_wx`（微信号）与 `kf_openid` 是用户级敏感标识，同样不得落日志。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddKfAccountApi());

// 注入 IMpKfAccountService（注册组 KfAccount）
// 1) 添加客服账号：形状必须是「前缀(≤10)@公众号微信号(≤30)」，昵称 ≤16 字
MpException.ThrowIfFailed(
    await kfAccount.AddKfAccountAsync(new MpAddKfAccountRequest
    {
        KfAccount = kfFullAccount,          // 形如 "support01@my_mp_id"
        Nickname = "小助手一号",
        BusinessId = null,                  // 仅客服子商户场景才填
    }, ct),
    "/customservice/kfaccount/add");        // 65404 形状不合法 / 65405 超 100 个 / 65406 已存在

// 2) 邀请微信号完成绑定：未绑定不能投入使用（65402）
await kfAccount.InviteKfWorkerAsync(new MpInviteKfWorkerRequest
{
    KfAccount = kfFullAccount,
    InviteWx = wechatIdToInvite,            // 微信号：不要写进日志
}, ct);

// 3) 全量客服列表（GET；business_id 可选）
var list = await kfAccount.GetKfListAsync(businessId: null, ct);
List<MpKfAccount> accounts = list.KfList ?? new List<MpKfAccount>();

// 4) 在线客服列表：按 accepted_case 做负载分配
var online = await kfAccount.GetOnlineKfListAsync(ct);
List<MpKfOnlineAccount> workers = online.KfOnlineList ?? new List<MpKfOnlineAccount>();
MpKfOnlineAccount? pick = workers
    .Where(w => w.Status == MpKfOnlineStatus.WebOnline)
    .OrderBy(w => w.AcceptedCase)
    .FirstOrDefault();

// 5) 改昵称（官方修改页无 business_id）
await kfAccount.UpdateKfAccountAsync(new MpUpdateKfAccountRequest
{
    KfAccount = kfFullAccount,
    Nickname = "小助手一号（改）",
}, ct);

// 6) 设头像：form 由调用方构建（文件字段名必须 media、≤5M），kf_account 走 Query
public async Task SetAvatarAsync(IFormContent form, string kfFullAccount, CancellationToken ct)
{
    MpException.ThrowIfFailed(
        await kfAccount.UploadKfHeadImgAsync(form, kfFullAccount, ct),
        "/customservice/kfaccount/uploadheadimg");   // 40005 文件格式不对
}
```

**判错只有一层**：`errcode` / `errmsg`（5 支写操作都返回 `MpResponse`）。
「新版客服功能」未开通 / 未升级时，本子分组多数端点返回 **`65400`**——那是**配置态问题**，不是调用错误。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域**无独立守卫文件** | 断言与 `KfSession` 共用 [MpKfContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpKfContractGuards.cs)（`KF1`/`KF3`/`KF4`/`KF5`/`KF6`/`KF7`/`KF8`/`KF9`） | 拆域需同批改守卫 remarks 的「7 + 5」构成 |
| `65410`「对方绑定客服账号数达上限」的具体上限值 | 仓内只有常量名、**无数值留档** | 未逐项核验，不得本地预校验 |
| `business_id` 的适用面（客服子商户 / 微信客服链路） | 只知「普通账号不需要填」 | 与小程序线 `work/*` 的对应关系**未核验**，勿跨线借语义 |
| 删除客服账号是否解除既有会话 / 标签 | 官方**未声明** ⇒ `DelKfAccountAsync` remarks 明确「SDK 不推断」 | 需要一致性时自行先用会话控制端点收尾 |
| 头像图片的格式白名单 | 只钉「≤5M」与 `40005` 语义，**支持格式集未逐项核验** | 不得从素材域借 type 枚举 |
| 第三方平台代调用面 | 本域接口 remarks **未留档** ⇒ 按未核验处理 | 重新逐页核验 → 补 remarks → 同批更新本文 |
| `MpErrorCodes` 未覆盖的客服码 | `KF7` 只钉 `40005` + `65401`~`65415` 全 15 值 | 需更全时先逐页核验再补 `KF7` |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、100 上限、账号形状、绑定前置、multipart 易错点、账号适用性澄清 | [IMpKfAccountService.cs](./IMpKfAccountService.cs) 的 XML 注释 |
| 7 条路由 + 「仅 2 条带 `/cgi-bin/`」+ 跨线 5 端点归小程序线（`MP-X1`） | `KF1` = `KfAccountEndpoints_ShouldMatchOfficialRoutes` |
| `uploadheadimg` 的 `[MultipartForm] IFormContent` + `kf_account` 的 Query 名 | `KF3` = `UploadKfHeadImg_ShouldUseMultipartWithQueryAccount` |
| DTO 登记进 `KfAccountJsonContext`（`expectedCount = 8`） | `KF4` = `KfDataModels_ShouldBeRegisteredInJsonContext` |
| 两套客服 DTO 的精确 JSON 名集合、`KfId` string/long 分工、`NotContain("password")` | `KF5` = `KfDataModels_ShouldLockOfficialFieldNamesAndTypeDifference` |
| `MpModule.KfAccount` + `AddKfAccountApi()` + 白名单「本域在内」 | `KF6` = `KfModules_ShouldBeRegisteredAndWhitelisted` |
| 已核验错误码常量（`40005` + `65401`~`65415`） | `KF7` = `KfErrorCodes_ShouldMatchVerifiedOfficialValues` |
| 四个长度/数量常量与 `MpKfOnlineStatus` 两值 | `KF8` = `KfLimitConstants_ShouldMatchOfficialValues` |
| 纯常量类不得落 `DataModels.<域>` 命名空间 | `KF9` = `ConstantClasses_ShouldNotLiveInDataModelNamespaces` |
| Query 令牌白名单与令牌绑定 | `QT1`（全集持票人）、`QT2`（本域接口 `access_token` 断言） |
| 8 个 DTO（`SerializerClassName = "KfAccount"`） | [DataModels/KfAccount/](../../../Mud.Wechat.OfficialAccount/KfAccount/) |
| 在线状态常量 | [MpKfConstants.cs](../../../Mud.Wechat.OfficialAccount/MpKfConstants.cs) |
| 生成入口 / 模块枚举 / 上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |
| 本线总览（**统计已过期**，不可当权威） | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与前置条件，**同批**更新 `KF1` 的路由表与前缀分布断言（现为 7 条、2 条带 `/cgi-bin/`）；
   新端点若无 `/cgi-bin` 前缀必须**照官方**，勿「补齐」。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `KfAccount`，见
   [AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1)）→ 跑
   [GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) → 同批更新 `KF4` 的 `expectedCount`（8）。
3. 确认 `Generated/KfAccountJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 新增**纯常量** → 落 Abstractions（`KF9`），不要落 `DataModels.KfAccount`；改常量值同批更新 `KF8`。
5. 动注册形态 → `MpModule.KfAccount` + `AddKfAccountApi()` + `KF6` + `QT1` + `RC1`/`RC2` +
   [ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线一起改；**计数以守卫为准**。
6. **不要**引入 `AccountType` 本地闸；**不要**把 `kf_account` 从 Query 挪进表单；
   **不要**把 `[MultipartForm]` 改成 `[FormContent]`；**不要**为 `MpAddKfAccountRequest` 加 `password` 字段；
   **不要**把 `KfId` 统一成一个类型（两页形态不同是有意的）。
7. 客服**发消息**（会话消息）属 [CustomerMessage 域](../CustomerMessage/README.md)；
   会话创建/关闭属 [KfSession 域](../KfSession/README.md)；本域只管账号本身。
8. 最后跑 [verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
