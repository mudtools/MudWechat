# 客户联系 · 企业服务人员

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 企业服务人员族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactFollowUserService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/FollowUser/IWechatWorkExternalContactFollowUserService.cs)（父，1 支公共面）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/FollowUser/IWechatWorkExternalContactFollowUserService_Internal.cs)（空标记）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/FollowUser/IWechatWorkExternalContactFollowUserService_ThirdParty.cs)（1）· [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/FollowUser/IWechatWorkExternalContactFollowUserService_Provider.cs)（1） |
| DTO 源文件 | [DataModels/ExternalContact/FollowUser/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/FollowUser/)（`SerializerClassName = FollowUser`） |
| 契约守卫 | [WechatExternalContactFollowUserContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactFollowUserContractGuards.cs)（FU1 路由表 3 条 / FU2 层级：自建空标记 + 第三方 1 + 代开发 1 / FU3 令牌绑定 / FU4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 成员列表 92571/92576/96310 · 可建联成员 101146（仅第三方）· 权限检查 96312（仅代开发） |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：FU1~FU4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.13、§4 陷阱 13）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

回答「**哪些成员能做客户联系**」这三件事：

| 端点 | 用途 | 开放面 |
|---|---|---|
| E1 `get_follow_user_list` | 取配置了客户联系功能的成员 userid 列表 | 三类应用 |
| E2 `customer_acquisition_app/get_permit` | 取营销获客应用配置的**客户可建联成员**范围 | **仅第三方**（且仅营销获客类应用） |
| E3 `check_follow_user` | 按成员/部门逐项检查是否具备客户联系使用权限，分 authorized / unauthorized 两组 | **仅代开发** |

本族是 ExternalContact 域里**差异端点分布最特别**的一族：父接口 1 条 + 第三方 1 条 + 代开发 1 条，**自建子接口是空标记**（FU2 三处计数分别锁定：自建 `BeEmpty()`、第三方 `HaveCount(1)`、代开发 `HaveCount(1)`）。

含义：同样叫「企业服务人员」，第三方要的是**获客建联范围**、代开发要的是**权限自检**，两者互不可见——写跨应用类型通用代码时不能假设三个注入面方法集相同。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（FU3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

```csharp
// 第三方 / 代开发：企业级令牌一企一份，先切作用域
using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var check = await providerFollowUser.CheckFollowUserAsync(request, ct);
}
```

### 2.2 权限与业务前置

- **E1**：自建应用须配置到「客户联系 - 可调用接口的应用」中；第三方 / 代开发须具有「客户基础信息」权限；**只能取到可见范围内的成员**。
- **E2**：**仅营销获客类应用可调用**，返回的是该应用在管理端配置的「客户可建联成员」范围（成员 / 部门 / 标签三种并列表达）。
- **E3**：`userid` 与 `partyid` **均可空**（两者都不传时的官方行为注释未留档，见 §7）；结果按 authorized / unauthorized 两组分别给出用户与部门 id。
- 返回的部门 id（E2 的 `department_list`）注释提示「可再经『获取部门成员』接口展开为成员 ID」——本族不做展开。
- `permanent_code` / `corpsecret` 等只写参数名，**不得进日志、遥测或异常消息**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 成员列表 | ✅ | ✅ | ✅ |
| E2 客户可建联成员 | ❌ | ✅ | ❌ |
| E3 权限检查 | ❌ | ❌ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactFollowUserService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 1 | 否 |
| `IWechatWorkInternalExternalContactFollowUserService` | `Mud.Wechat.Work` | 自建子接口（**空标记**） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactFollowUserService` | `Mud.Wechat.Work` | 第三方子接口 | **1** | 是 |
| `IWechatWorkProviderExternalContactFollowUserService` | `Mud.Wechat.Work` | 代开发子接口 | **1** | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactFollowUserService)`。

## 4. 端点

### E1. `GetFollowUserListAsync` — 获取配置了客户联系功能的成员列表

```csharp
Task<GetFollowUserListResponse> GetFollowUserListAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/externalcontact/get_follow_user_list` |
| 请求 | **无请求体、无业务 Query 参数**，签名只剩取消令牌 |
| 令牌 | Query `access_token` |

**响应参数**：`follow_user`（`List<string>?`）——配置了客户联系功能的成员 userid 列表。

**本端点注意事项**

- 这是**其它端点的前置**：E1 得到的成员集，就是「谁的客户能被读、能给谁配「联系我」」的定义域；先取这份名单再调其它列表类接口，比逐成员试错便宜得多。
- 无分页 ⇒ 大企业的成员集一次返回，但受可见范围裁剪。

### E2. `GetCustomerAcquisitionPermitAsync` — 获取客户可建联成员（仅第三方）

```csharp
Task<GetCustomerAcquisitionPermitResponse> GetCustomerAcquisitionPermitAsync(
    CancellationToken cancellationToken = default);
```

- 路由 **`GET`** `/cgi-bin/externalcontact/customer_acquisition_app/get_permit`；无请求参数。
- 仅营销获客类应用可调用。

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `user_list[]` | `UserList` | `List<string>?` | 可建联成员 userid |
| `department_list[]` | `DepartmentList` | `List<long>?` | 可建联部门 id（可再经「获取部门成员」展开） |
| `tag_list[]` | `TagList` | `List<long>?` | 可建联标签 id |

### E3. `CheckFollowUserAsync` — 检查用户是否配置了客户联系使用权限（仅代开发）

```csharp
Task<CheckFollowUserResponse> CheckFollowUserAsync(
    [Body] CheckFollowUserRequest request, CancellationToken cancellationToken = default);
```

- 路由 **`POST`** `/cgi-bin/externalcontact/check_follow_user`（检查语义走 POST）。

**请求参数**（`CheckFollowUserRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `List<string>?` | 否 | 待检查的成员 userid 列表 |
| `partyid` | `Partyid` | `List<long>?` | 否 | 待检查的部门 id 列表 |

**响应参数**：`authorized` / `unauthorized`（各为 `CheckFollowUserResult?`，内含 `userid`（`List<string>?`）与 `partyid`（`List<long>?`））。

**调用示例**（本族主示例：自建取名单 + 代开发权限自检）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;   // IWechatAppContextSwitcher
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.Provider;
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];     // 凭据：只从配置读
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var followUsers = provider.GetRequiredService<IWechatWorkProviderExternalContactFollowUserService>();
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
string authCorpId = config["Wechat:AuthCorpId"]!;
string permanentCode = config["Wechat:PermanentCode"]!;

using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    // ① E1：该企业里配置了客户联系功能的成员（仅可见范围内）
    var list = await followUsers.GetFollowUserListAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(list, "/cgi-bin/externalcontact/get_follow_user_list");
    logger.LogInformation("客户联系成员数：{Count}", list.FollowUser?.Count ?? 0);

    // ② E3：按成员与部门逐项自检权限归属，结果分 authorized / unauthorized
    var check = await followUsers.CheckFollowUserAsync(
        new CheckFollowUserRequest
        {
            Userid = new List<string> { "zhangsan", "lisi" },        // 假值
            Partyid = new List<long> { 2 },
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(check, "/cgi-bin/externalcontact/check_follow_user");

    logger.LogInformation("有权限成员 {OkCount} / 无权限成员 {NoCount}",
        check.Authorized?.Userid?.Count ?? 0, check.Unauthorized?.Userid?.Count ?? 0);
    // 只记数量与判定结果，不记成员与部门明细。
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `customer_acquisition_app/`（本族 E2）vs `customer_acquisition/`（获客助手族） | 路由前缀差一个 `_app`，属两个子族 | 「顺手统一」前缀 → 路由不匹配、打坏 FU1/CA1 |
| `tag_list`（本族 `List<long>?`）vs 客户标签 `tag_id`（`string`） | 同为「标签 id」但元素类型不同（跨族不通用） | 共用一个标签模型 → 反序列化失败 |
| E1 的成员集 vs E3 的 authorized 集 | E1 是「已配置客户联系功能的成员」；E3 是「按传入身份检查权限」 | 以为两者恒等 → 权限巡检结论失真 |
| E1/E2 的 GET vs E3 的 POST | 两支无参读取走 GET、检查走 POST（FU1 逐条锁定） | 统一动词 → 打坏 FU1 与 G5 |
| E3 请求两键 | `userid` / `partyid` **均可空** | 传空对象期待「检查全部」→ 行为未留档 |
| 差异端点归属 | 第三方只有 E2、代开发只有 E3、自建两者都无 | 在三类注入面间互换调用 → 编译不过 |
| 应用形态限定 | E2 仅**营销获客类**应用可调用 | 普通第三方应用调用 → 权限类失败 |
| 父接口 | 只承载 E1，不注册 DI | 注入父接口 → 找不到 E2/E3 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不展开部门为成员**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 成功仍要读 unauthorized 分组 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

本族接口注释**未留档业务错误码**（如非营销获客应用调用 E2、未配置客户联系权限）⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E3 两键均空时的官方行为 | 注释只说「均可空」，未给空请求的语义（全量检查 / 参数错误）⇒ 未逐项核验 | 官方 96312 |
| E1 成员数量上限与频率 | 注释未给数值 ⇒ 未逐项核验 | 官方 92571 |
| E2 的 `tag_list` 标签体系归属 | 注释未说明是哪一面标签（企业客户标签 / 规则组标签）⇒ 未逐项核验 | 官方 101146 |
| 业务错误码 | 三支端点均未留档码值 | 官方各页 |
| 「营销获客类应用」判定方式 | 本域无对应查询端点 | 官方第三方接入说明 |
| 官方文档正文 | 本文只摘录本仓 XML 注释 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.13 端点表、§4 陷阱 13 敏感标识红线）
- 同域：[Customer.md](./Customer.md)（按成员读客户）· [CustomerAcquisition.md](./CustomerAcquisition.md)（获客助手链接，前缀 `customer_acquisition/`）· [Tag.md](./Tag.md)（标签 id 的另一套类型）· [ContactWay.md](./ContactWay.md)（「联系我」要求成员已配置客户联系功能）
- 官方页：[获取配置了客户联系功能的成员列表（自建 92571）](https://developer.work.weixin.qq.com/document/path/92571) · [检查用户权限（代开发 96312）](https://developer.work.weixin.qq.com/document/path/96312) · [获取客户可建联成员（第三方 101146）](https://developer.work.weixin.qq.com/document/path/101146)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
