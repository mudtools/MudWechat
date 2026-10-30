# 微信客服 · 客服账号管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 客服账号管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfAccountService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfAccountService.cs)（父，承载全部 5 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfAccountService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfAccountService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfAccountService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/)（命名空间恒为 `Mud.Wechat.Work.DataModels.Kf`，`RequestModel/` 子目录仅作组织） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1a** 五支路由 / KF2 三类子接口零端点空标记 / KF3 令牌绑定 / **KF1g2** 组件族不得重复声明 `account/list` 与 `add_contact_way` / KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [94662](https://developer.work.weixin.qq.com/document/path/94662)（添加）/ [94661](https://developer.work.weixin.qq.com/document/path/94661)（列表）/ 94663（删除）/ 94664（修改）/ [94665](https://developer.work.weixin.qq.com/document/path/94665)（链接）· 代开发 96404 / 96415 / 96405 / 96406 / 96416 · 组件版另页 99368 / 99400（**同路由，不另立契约面**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1a / KF2 / KF3 / KF1g2 / KF4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§1 落位不对称、§4 本族反直觉路由、§14 POST 汇总）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

客服账号（`open_kfid`）的生命周期与**可投放的客服链接**，5 支端点：

| # | 方法 | 路由 | 用途 |
|---|---|---|---|
| E1 | `AddAccountAsync` | `POST /cgi-bin/kf/account/add` | 建账号（名称 + 头像素材），返回 `open_kfid` |
| E2 | `GetAccountListAsync` | `POST /cgi-bin/kf/account/list` | `offset` + `limit` 分页列账号 |
| E3 | `DeleteAccountAsync` | `POST /cgi-bin/kf/account/del` | 删账号 |
| E4 | `UpdateAccountAsync` | `POST /cgi-bin/kf/account/update` | 改名称 / 头像（均可选填） |
| E5 | `GetAccountContactWayAsync` | `POST /cgi-bin/kf/add_contact_way` | 取客服账号链接（带 `scene` 场景值） |

形态：官方对**三类应用开放完全一致的 5 支** ⇒ 全收敛父接口 + 三个零差异空标记子接口（KF2）。注册组与模块名是**单数 `Kf`**（`WechatModule.Kf` / `AddKfApi()`），接口目录是大写 `Interfaces/KF/`、DTO 命名空间是小写 `...DataModels.Kf`——这条不对称是 [AGENTS.md §4](../../../../../AGENTS.md) 明列的既存点，**勿「顺手修正」**。

下游消费：`open_kfid` 是本域一切端点的定位主键（接待人员、会话分配、消息收发、统计、升级服务全部以它定位「哪个客服账号」），`url`（E5）投放给用户后即成为会话入口与 `scene` / `scene_param` 的来源。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（KF3 对全部 28 个接口逐一断言） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），须先 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`；`permanent_code` 只写参数名，不得进日志）。

### 2.2 权限与业务前置

- 自建应用须配置到「**微信客服-可调用接口的应用**」中；且**账号管理族端点还须**在管理后台「通过API管理会话消息」-「企业内部开发」对应应用的「**可管理的客服账号**」处**配置至少一个客服账号**。
- 第三方 / 代开发须具有「**微信客服 → 管理账号、分配会话和收发消息**」权限（E1/E3/E4 管理族）或「**微信客服 → 获取基础信息**」权限（E2/E5 查询类）。
- **只能通过 API 管理企业明确授权 API 管理的账号**：企业须在管理后台「微信客服-通过API管理微信客服账号」处把对应账号设为允许 API 管理。
- **一家企业最多 5000 个客服账号**；被操作账号对应的**接待人员应在应用可见范围内**。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- E1 的 `media_id`（头像）**必须先经「上传临时素材」接口获取**（media 域，本域不含上传端点）。
- E5 返回的 `url` **不能修改、也不能把参数复制到其他链接**，否则进入会话事件的参数校验不通过、无法回调；`scene_param=SCENE_PARAM` 需 urlencode，编码前 ≤128 字节。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E5 | ✅ | ✅ | ✅ |

> **组件版共用路由**：官方另有组件文档 99368（列表）/ 99400（链接）两页，但**与本域同路由、同 DTO** ⇒ 本仓只在客服账号域声明一次，**KF1g2 反向锁定组件子接口不得重复声明**。组件应用调 E2 时，响应 `manage_privilege` **不返回**（且只能拿到企业已授权的账号）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfAccountService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalKfAccountService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyKfAccountService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderKfAccountService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfAccountService)`。

## 4. 端点

### E1. `AddAccountAsync` — 添加客服账号

```csharp
[Post("/cgi-bin/kf/account/add")]
Task<AddKfAccountResponse> AddAccountAsync(
    [Body] AddKfAccountRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `name` | `Name` | `string?` | **是** | 客服名称，**不多于 16 个字符** |
| `media_id` | `MediaId` | `string?` | **是** | 头像临时素材 id，**≤128 字节**，经「上传临时素材」获取 |

响应：`open_kfid`（新账号 ID）。**创建后调用方应用自动获得该账号的管理权限**。

### E2. `GetAccountListAsync` — 获取客服账号列表

```csharp
[Post("/cgi-bin/kf/account/list")]
Task<GetKfAccountListResponse> GetAccountListAsync(
    [Body] GetAccountListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `offset` | `Offset` | `int?` | 否 | 分页偏移量，默认 `0`、以 0 为起点 |
| `limit` | `Limit` | `int?` | 否 | 默认与最大均 **100**，取值 1~100 |

响应：`account_list[]`（`KfAccount`）＝ `open_kfid` / `name` / `avatar` / **`manage_privilege`（`bool?`，当前应用是否有该账号管理权限：编辑信息、分配会话、收发消息；组件应用调用时不返回）**。
**分页终止判据：返回条数 `< limit` 即无更多数据**（本域无 cursor）。

### E3. `DeleteAccountAsync` — 删除客服账号

```csharp
[Post("/cgi-bin/kf/account/del")]
Task<WechatWorkResponse> DeleteAccountAsync(
    [Body] DeleteKfAccountRequest request, CancellationToken cancellationToken = default);
```

- 请求：`open_kfid`（官方必填，**≤64 字节**）。响应仅 `errcode` / `errmsg`。

### E4. `UpdateAccountAsync` — 修改客服账号

```csharp
[Post("/cgi-bin/kf/account/update")]
Task<WechatWorkResponse> UpdateAccountAsync(
    [Body] UpdateKfAccountRequest request, CancellationToken cancellationToken = default);
```

- 请求：`open_kfid`（必填，≤64 字节）+ `name`（≤16 字符，**不需修改可不填**）+ `media_id`（≤128 字节，**不需修改可不填**）。
- 两支可选字段是**本域少见的「补丁式」更新**——与 E1 的「两者皆必填」不对称，见 §5。

### E5. `GetAccountContactWayAsync` — 获取客服账号链接

```csharp
[Post("/cgi-bin/kf/add_contact_way")]
Task<GetKfAccountContactWayResponse> GetAccountContactWayAsync(
    [Body] GetKfAccountContactWayRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `open_kfid` | `OpenKfId` | `string?` | **是** | 客服账号 ID |
| `scene` | `Scene` | `string?` | 否 | 自定义场景值，**≤32 字节**、须匹配 `[0-9a-zA-Z_-]*` |

响应：`url`（客服链接，可嵌 H5 或据此生成二维码）。`scene` 非空时链接可再拼 `scene_param=SCENE_PARAM`（需 urlencode、编码前 ≤128 字节），用户进入会话事件会**原样回传** `scene` / `scene_param`。

**调用示例**（建账号（头像素材先上传）→ 分页取列表并按管理权限过滤 → 取带场景值的客服链接 → 改名）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Kf;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "kf-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddKfApi());

var accounts = provider.GetRequiredService<IWechatWorkInternalKfAccountService>();

// ① 建账号：name ≤16 字符、media_id ≤128 字节（头像须先经「上传临时素材」取得，本域无上传端点）
//   前提：该企业的账号已在管理后台设为「允许 API 管理」，否则后续操作无权
var created = await accounts.AddAccountAsync(
    new AddKfAccountRequest { Name = "售后咨询", MediaId = "MEDIA假值" }, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/kf/account/add");
string openKfId = created.OpenKfId ?? throw new InvalidOperationException("响应未返回 open_kfid");

// ② 列表：官方即 POST；offset + limit（默认与上限 100）；返回条数 < limit 即到底
var managed = new List<string>();
for (int offset = 0; ; offset += 100)
{
    var page = await accounts.GetAccountListAsync(
        new GetKfAccountListRequest { Offset = offset, Limit = 100 }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/kf/account/list");

    List<KfAccount> batch = page.AccountList ?? new List<KfAccount>();
    foreach (KfAccount account in batch)
    {
        // manage_privilege：false 表示本应用无法编辑该账号 / 分配会话 / 收发消息
        // 组件应用调用时该字段不返回（null），只能拿到企业已授权的账号
        if (account.ManagePrivilege == true && account.OpenKfId is { Length: > 0 } id) managed.Add(id);
    }
    if (batch.Count < 100) break;      // 官方口径：条数小于 limit 即无更多数据
}
logger.LogInformation("企业客服账号总数分页完成，本应用可管理的 {Count} 个", managed.Count);

// ③ 客服链接：scene ≤32 字节且只允许 [0-9a-zA-Z_-]；返回的 url 不可改、不可把参数复制到别的链接
var way = await accounts.GetAccountContactWayAsync(
    new GetKfAccountContactWayRequest { OpenKfId = openKfId, Scene = "site-footer" }, cancellationToken);
WechatWorkException.ThrowIfFailed(way, "/cgi-bin/kf/add_contact_way");
logger.LogInformation("客服链接已取得（scene 已编入，进入会话事件将原样回传 scene / scene_param）");

// ④ 改名：name / media_id 均可选填，不需要改的就不传（补丁式更新）
var updated = await accounts.UpdateAccountAsync(
    new UpdateKfAccountRequest { OpenKfId = openKfId, Name = "售后咨询组" }, cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/kf/account/update");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `add_contact_way`（E5）vs 客户联系域的 `add_contact_way` | 客服侧这支挂在 **`/cgi-bin/kf/` 根下**（不在 `account/` 子路径），与客户联系域的同名端点（「联系我」配置）**不同前缀、不同 DTO** | 按 `kf/account/` 猜路由，或误用客户联系域的 DTO |
| E1 vs E4 的必填面 | E1 的 `name` 与 `media_id` **均必填**；E4 的 `name` / `media_id` **均可选填**（不改就不传） | 用 E1 的必填假设去写 E4 → 把不改的头像重传 |
| E2 的分页形态 | **`offset` + `limit`**，判完依据是「**返回条数 < limit**」；本域**没有** cursor | 按游标写循环（无 `next_cursor` 字段可读） |
| `limit` 的默认与上限 | 默认 **100**、范围 1~100 | 传 1000 期望大页 |
| `manage_privilege` | `bool?`：**组件应用调用时不返回**（值为 `null`）；`false` 表示本应用无权编辑该账号 / 分配会话 / 收发消息 | `if (account.ManagePrivilege)` 把 `null` 当假 → 组件侧过滤掉全部账号 |
| 「可管理的客服账号」配置 | E1/E3/E4 除「配置到可调用应用」外，**还须在管理后台给该应用配置至少一个可管理账号** | 报权限类错误却以为令牌问题 |
| API 管理白名单 | **只能管理企业显式设为「允许 API 管理」的账号** | 对未授权账号增删改全部失败 |
| `media_id` 的来源 | 头像是**临时素材** id（media 域上传接口获取，三天有效），本域**无上传端点** | 在本域找上传；或用长期素材 id |
| `url` 的不可变性 | 返回的客服链接**不能修改、不能把参数复制到其他链接**，否则进入会话事件参数校验不过、无法回调 | 自行拼 URL 或换域名 |
| `scene` 与 `scene_param` | `scene` 是本端点写入的自定义值（≤32 字节、正则受限）；`scene_param` 是**在返回链接后自行拼接**的参数（需 urlencode、编码前 ≤128 字节），事件里原样回传 | 混为一谈或忘了 encode |
| 组件版 99368 / 99400 | 官方**另立文档页**但**同路由同 DTO** ⇒ 本仓单一所有者声明，**KF1g2 禁止组件子接口重复声明** | 在组件族再造两条端点 → 守卫打红 |
| 5000 账号上限 | 一家企业最多 5000 个客服账号 | 批量建号撞上限 |
| 目录 / 命名空间大小写 | 接口目录 `Interfaces/KF/`（大写 F），DTO 命名空间 `...DataModels.Kf`（小写 f），注册名**单数** `Kf` | 按目录名写 `using ...DataModels.KF;` → 编译失败 |
| 动词 | 5 支全 POST（E2 列表也 POST） | 改 GET → 打坏 KF1a 与 G5 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做本地必填校验**、**不做分页批次裁剪**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功才可用返回的 `open_kfid` 配接待人员与会话 |
| 95014 | 官方业务码（本域接口注释留档，见 [KfSession.md](./KfSession.md)） | 目标接待人员未在企业微信激活 | 属会话族口径，账号侧未留档 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

账号数超 5000、账号未设为允许 API 管理、`media_id` 非法或过期、`scene` 越界 / 不匹配正则等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未逐条摘录 ⇒ **未逐项核验**（含「账号未授权 API 管理」「超出 5000」的码） | 官方 94661~94665 + 全局错误码页 |
| 端点频率限制 | 注释未给数值 ⇒ **未逐项核验**，勿从其它域借数 | 官方访问频率附录 90312 |
| 头像素材的规格（尺寸 / 格式 / 大小） | 注释只给 `media_id` 长度上限，未给图片规格 ⇒ 未核验 | 官方 94662 + 上传临时素材页 |
| 删除账号的连带效应 | 是否要求先清接待人员、进行中的会话如何处理，注释未说明 ⇒ 未核验 | 官方 94663 |
| `open_kfid` 的稳定性 | 是否随账号重建复用、是否跨应用一致，注释未说明 ⇒ 未核验 | 官方 94661 |
| 组件应用与 `manage_privilege` 的完整差异 | 注释只写「组件应用调用时不返回该字段」，未说明其它字段是否受限 ⇒ 未逐项核验 | 官方 99368 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§1 落位不对称、§2 八族开放面不等宽、§4 本族两条反直觉路由、§8 场景值口径、§14「查询类官方即 POST」汇总）
- 同域：[KfServicer.md](./KfServicer.md)（给账号配接待人员）· [KfSession.md](./KfSession.md)（会话状态与收发消息）· [KfStatistics.md](./KfStatistics.md)（按 `open_kfid` 取统计）· [KfUpgrade.md](./KfUpgrade.md)（升级服务）· [KfComponent.md](./KfComponent.md)（组件族与共用路由的所有权裁决）
- 跨域：`scene` / `scene_param` 的**回传面**在回调侧「用户进入会话事件」（Callback 包）；`ca_link` 获客链接见 [KfSession.md](./KfSession.md) §5，与获客助手域的统计口径互不计入
- 素材邻域：`media_id` 由「上传临时素材」（media 域）产生、三天有效，本域不承载
- 官方页：[添加客服账号（94662）](https://developer.work.weixin.qq.com/document/path/94662) · [获取客服账号列表（94661）](https://developer.work.weixin.qq.com/document/path/94661) · [获取客服账号链接（94665）](https://developer.work.weixin.qq.com/document/path/94665) · [组件版列表 99368](https://developer.work.weixin.qq.com/document/path/99368)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
