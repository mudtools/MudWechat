# PromotionQrCode 域 · 推广二维码（企业注册 / 通讯录迁移收尾）

企业微信「推广二维码」域（服务商推广注册链路）：本仓共落地 **4 支端点**，分属**两族两凭据**、由本目录 **3 份接口文件**承载——
企业注册族 **2 支**（守卫 PQ1 + PQ2：零端点父接口 [IWechatWorkServicePromotionQrCodeService.cs](./IWechatWorkServicePromotionQrCodeService.cs)（`IsAbstract`）+
唯一第三方子接口 [IWechatWorkThirdPartyServicePromotionQrCodeService.cs](./IWechatWorkThirdPartyServicePromotionQrCodeService.cs) 承载全部 2 支）、
通讯录迁移族 **2 支**（守卫 PQ3：独立接口 [IWechatWorkPromotionQrCodeContactSyncService.cs](./IWechatWorkPromotionQrCodeContactSyncService.cs)，
**无父接口、无应用类型子接口、不带 `[Token]`**）。
自建与代开发子接口**刻意不存在**（官方仅第三方应用开发文档树有本页，PQ2 用继承链断言锁死）。
注册组 `PromotionQrCode`（两族共用同一注册项，族内实现类名 `WechatWorkServicePromotionQrCodeService` 与
`WechatWorkPromotionQrCodeContactSyncService` 由 PQ2 / PQ3 以字面量锁定），
模块枚举 `WechatModule.PromotionQrCode`（[WechatModule.cs](../../Extensions/WechatModule.cs)）与
[`AddPromotionQrCodeApi()`](../../Extensions/WechatWorkServiceBuilder.cs)。
契约面 DTO 全量 **8 型**（请求 3 + 响应 3 + 嵌套 2）登记在 `PromotionQrCodeJsonContext`（PQ5）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 3 份接口文件的 XML 注释、
> [WechatPromotionQrCodeContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPromotionQrCodeContractGuards.cs)（PQ1~PQ6）、
> [../../../Mud.Wechat.Work/DataModels/PromotionQrCode/](../../../Mud.Wechat.Work/DataModels/PromotionQrCode/) 下的 8 个 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号（90581~90584）仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。
> 尤其 `state` 的长度规则、注册链接拼接方式、凭据可用 API 面这类**邻域口径**，本文只写本域注释留档的部分，
> 不把邻域数值借过来。

## 端点速览

令牌/开放面列写本仓真实契约（本域是**两种令牌**，不是同一种）：

- 企业注册族：PQ4 断言父接口与第三方子接口的 `[Token]` 均为
  `TokenType = WechatTokenTypes.ProviderAccessToken` + `InjectionMode.Query` + `Name = "provider_access_token"`，
  且 `TokenManagerKey` **既不等于** `InternalAccessToken` **也不等于** `CorpAccessToken`（服务商级令牌无歧义 ⇒ 不声明归属域键，TO1）。
- 通讯录迁移族：PQ3 断言接口**不得声明 `[Token]`**（断言为 `Null`），令牌以
  `[Query("access_token")] string accessToken` 显式逐调用传入，因此**不进入** G5 Query 令牌注入白名单。
- 两族的 Query 参数都属 MUD005 已知接受风险面（官方契约强制 Query，无法改 Header）。

### 企业注册族（PQ1 / PQ2 / PQ4，2 支，令牌 = `provider_access_token`）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetRegisterCodeAsync` | POST | `/cgi-bin/service/get_register_code` | `provider_access_token`(Query)；**仅第三方** | 90581；`template_id` 必填（本仓非可空 `string`）；`register_code` ≤512 字节、**只能消费一次**；`expires_in` 示例 600 |
| 2 | `GetRegisterInfoAsync` | POST | `/cgi-bin/service/get_register_info` | 同上；**仅第三方** | 90582；`register_code` 生成后查询有效期 **24 小时**；错误码 84024 非全新创建企业；`contact_sync` 条件返回 |

### 通讯录迁移族（PQ1 / PQ3，2 支，令牌 = 通讯录迁移 `access_token`，显式传参）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 3 | `SetAuthorizedAppScopeAsync` | POST | `/cgi-bin/agent/set_scope` | `access_token`(Query，**显式参数**，无 `[Token]`)；**仅第三方** | 90583；`agentid` 必填（`int`）；`allow_user`/`allow_party`/`allow_tag` **未填即清空**；应答 `invaliduser`/`invalidparty`/`invalidtag` |
| 4 | `SetContactSyncSuccessAsync` | **GET** | `/cgi-bin/sync/contact_sync_success` | 同上；**仅第三方** | 90584；**GET 却执行状态变更且不可逆**（同步完成 + 解锁通讯录 + 作废迁移 `access_token`）；无业务负载 ⇒ 返回 `WechatWorkResponse`，不建空响应 DTO |

## 1. 两族两套凭据，绝不可互换

| 族 | 凭据 | 来源 | 生命周期 | SDK 是否托管 |
|---|---|---|---|---|
| #1/#2 | `provider_access_token`（服务商级） | 服务商凭证面（邻域签发） | 由令牌基座按路由键 `WechatTokenTypes.ProviderAccessToken` 获取与刷新 | **托管**（声明式 `[Token]` + Query 注入） |
| #3/#4 | 通讯录迁移 `access_token` | #2 应答的 `contact_sync.access_token`，或「注册完成回调事件」的 `AccessToken` | **30 分钟**有效，且被 #4 **立即作废** | **刻意不托管**（接口 remarks 原文：SDK 令牌基座不缓存该凭证） |

官方在 90583 / 90584 两页均明文「**请注意与 `provider_access_token` 的区别**」，且明确**不可用后者代替**前者（接口 remarks 逐字留档）。
该凭证具备**全部通讯录读写权限**，因此：

- 本族不带 `[Token]`，一旦声明就会把别的令牌错注入（PQ3 的 `BeNull` 断言正是为堵这一点）；
- 调用方须自行从 #2 应答或回调事件取出并显式传入，**不得**落日志、遥测、异常消息或 URL 记录；
  同理 `permanent_code`、`auth_code`、`suite_ticket`、`corpsecret` 在本域只允许作为参数名出现。
- 本域没有父接口/子接口分层（无 SDK 管理令牌 ⇒ 无按应用类型分裂的必要），
  生成实现类名 `WechatWorkPromotionQrCodeContactSyncService` 就是接口名去 `I` 前缀（PQ3 字面量锁定）。

## 2. 三条路由前缀分属三段，族内不同前缀不可混

本域 4 支端点分处**三个**路由前缀，且族划分就是按前缀 + 令牌划的：
`/cgi-bin/service/`（#1/#2，注册码）、`/cgi-bin/agent/`（#3，可见范围）、`/cgi-bin/sync/`（#4，同步收尾）。
⇒ 不要按「同域同前缀」的直觉去改写路由；PQ1 的路由表逐条锁死完整字符串（含 `contact_sync_success` 的下划线拼写）。

## 3. 本域**没有**二维码图片：`register_code` 是注册链接里的码，不是图

四支端点全部是 **JSON 请求 / JSON 响应**，没有二进制媒体面：

- #1 应答只有 `register_code`（`string`，≤512 字节）与 `expires_in`（`int`），它是**注册码字符串**，
  由服务商拼进注册链接后交给企业点击（链接模板本仓未留档 ⇒ 见缺口表）；**不返回图片字节**。
- 「官方默认为 1 时返回 `image/png` 二进制流而非 JSON」那条陷阱属于
  `../Authentication/IWechatWorkProviderAuthenticationService.cs` 的 `get_app_qrcode`
  （`result_type` 1 = 图片 buffer、2 = 返回 `qrcode` url；走本 SDK 的 JSON 契约面须传 2），
  **与本域无关**，不要把 `result_type` 之类的形状借到本域 DTO 上。
- 本域四个响应类型都继承 `WechatWorkResponse`（单层普通信封，`errcode == 0` 即成功），
  #4 干脆不建 DTO（PQ5 注释原文）。

## 4. `state` 的长度规则**本仓有两套**，按域取用不得互借

| 出现位置 | 官方口径（本仓留档原文） | 证据 |
|---|---|---|
| 本域 #1 `GetRegisterCodeRequest.State` | 「**只支持英文字母和数字，最长为 128 字节**」；指定后 #2 应答与「注册完成回调事件」会回传同值 | [GetRegisterCodeRequest.cs](../../DataModels/PromotionQrCode/RequestModel/GetRegisterCodeRequest.cs) + 接口 remarks（90581） |
| `Authentication` 邻域 `get_app_qrcode` / 定制授权链接 | 「`state` 仅可填写 a-zA-Z0-9、**长度不可超过 32 个字节**」 | `../Authentication/IWechatWorkProviderAuthenticationService.cs` remarks |

⇒ 两者都是「字母数字」但**长度上限不同**（128 vs 32），跨域复用同一个渠道标识常量必然在其中一侧越界。
本仓**不做**本地长度与字符集校验，越界由应答 `errcode` 表达；需要预检时按**调用所在域**取上限。
另注意 #2 应答的 `State`（`string?`）是**可空**：官方未指定该参数时响应中**没有**该字段（PQ6 断言其类型为 `string`）。

## 5. `register_code` 的一次性消费与两个不同的时间窗

| 窗口 | 数值 | 语义 |
|---|---|---|
| 消费窗口 `expires_in` | 官方**示例值 600**（秒） | 生成的注册链接必须在该有效期内被点击；点击即**消费** |
| 查询窗口 | **24 小时**（自 `register_code` 生成起） | #2 `GetRegisterInfoAsync` 的可查询期，与上面是两件事 |

硬约束（均出自接口 remarks 与 DTO 注释，非推断）：

- `register_code` **只能消费一次**，消费发生在「访问注册链接」时；重新获取要重新调 #1。
- #2 **只接受**「注册完成回调事件」或「获取注册码」返回的 `register_code`，自造或改写无效。
- #2 仅在**全新创建企业**场景可用：错误码 **84024**「非全新创建企业，不支持使用该接口查询」。
- 请求侧的 `TemplateId`（`template_id`，最长 128 字节，官方必填）与 `RegisterCode`（`register_code`，官方必填）
  是本域唯二以**非可空 `string`** 表达的字段（默认值 `string.Empty`）；其余请求字段全部可空，
  配合 `JsonIgnoreCondition.WhenWritingNull`  ⇒ 不赋值即不出现在报文里。
- `corp_name` / `admin_name` / `admin_mobile` / `follow_user` 都是**选填的预填项**：
  前三者传入后在注册填写页自动带入表格；`follow_user` 必须是**服务商所在企业的成员**，
  配置后该注册码创建的企业在服务商管理后台的报备记录会自动标注跟进人。

## 6. 「未填即清空」：#3 的三个范围参数不是「保持不变」

`SetAuthorizedAppScopeRequest`（[RequestModel/SetAuthorizedAppScopeRequest.cs](../../DataModels/PromotionQrCode/RequestModel/SetAuthorizedAppScopeRequest.cs)）：

| 属性 | JSON 名 | 类型 | 缺省语义 |
|---|---|---|---|
| `AgentId` | `agentid` | `int`（非可空，官方必填） | 授权方应用 id |
| `AllowUser` | `allow_user` | `List<string>?` | **不填 ⇒ 清空成员列表** |
| `AllowParty` | `allow_party` | `List<int>?` | **不填 ⇒ 清空部门列表** |
| `AllowTag` | `allow_tag` | `List<int>?` | **不填 ⇒ 清空标签列表** |

PQ6 把「三者必须可空、缺省即官方清空语义」钉成断言（原因写在守卫 remarks：官方明文
「若未填该字段，则清空可见范围中成员/部门/标签列表」，**非**「保持不变」）。
⇒ 想「只改部门、不动成员」在本端点上**做不到**，必须先读回现状再整组提交。
类型上还有两处不对称，照抄官方、勿统一：请求侧成员是 `List<string>`（userid）、部门与标签是 `List<int>`；
应答侧非法项字段名是 `invaliduser` / `invalidparty` / `invalidtag`（**无下划线**，且与请求侧的 `allow_user` 等**不同名**），
类型分别为 `List<string>?` / `List<int>?` / `List<int>?`（[SetAuthorizedAppScopeResponse.cs](../../DataModels/PromotionQrCode/SetAuthorizedAppScopeResponse.cs)）。

## 7. GET 却改状态：#4 三连不可逆，禁止盲目重试

`/cgi-bin/sync/contact_sync_success` 官方就是 **GET**（PQ1 的路由表以 `typeof(GetAttribute)` 锁定，
守卫 remarks 明确「勿顺手统一为 POST」），但它一次执行三件事：
**设置通讯录同步完成 + 解除通讯录锁定状态 + 使通讯录迁移 `access_token` 失效**。

后果（接口 remarks 原文「危险操作约束（不可逆）」）：

- 调用成功后**不能再调** #3；
- 迁移 `access_token` 超 30 分钟自然失效（同样解除锁定）后，#3/#4 **都不可再调**；
- 因此**重试 / 幂等回放机制必须对该端点关闭或加去重**：一个「看起来失败」（如响应超时）但实际已生效的
  GET 重放会直接废掉后续设置可见范围的机会。本仓不做任何自动重试判定，责任在宿主侧。
- 无业务负载 ⇒ 返回 `WechatWorkResponse`，只能按 `errcode` / `errmsg` 判错。

## 8. `contact_sync` 是条件返回，且凭证权限极大

`GetRegisterInfoResponse.ContactSync`（`ContactSyncCredential?`，JSON `contact_sync`）**仅当注册推广包开启
通讯录迁移接口时才返回**（接口 remarks 原文），故必须按可空处理：

```csharp
if (info.ContactSync is null)
{
    // 该推广包未开启通讯录迁移：没有迁移凭证，#3/#4 无从调用
}
```

[ContactSyncCredential.cs](../../DataModels/PromotionQrCode/ContactSyncCredential.cs) 只有两字段：
`AccessToken`（JSON `access_token`，非可空 `string`，官方口径 30 分钟有效期）与
`ExpiresIn`（JSON `expires_in`，`int`，官方示例 1800）。
同应答另有 `Corpid`（`corpid`）、`AuthUserInfo`（`auth_user_info` →
[RegisterAuthUserInfo.cs](../../DataModels/PromotionQrCode/RegisterAuthUserInfo.cs) 只承载 `userid`）、
`State`、`TemplateId` 四个可空字段。
调用 #3/#4 的**前提**是「已开启通讯录迁移」且「已收到授权成功通知」；企业注册初始化安装应用后
应用默认可见范围为**根部门**，只有需要修改时才调 #3。
本域只**消费**该凭证，不签发也不扩权：它能调哪些通讯录 API 属通讯录邻域，本域不作断言（见缺口表）。

## 9. 令牌声明形态与三层不变量的落位

- 本域是企业微信线里**少数使用服务商级令牌**的域：路由键 `Wechat.ProviderAccessToken`，
  与「企业级令牌」（`WechatTokenManagerKeys.CorpAccessToken`，scope = authCorpId）不是一回事 ⇒
  调 #1/#2 **不需要** `IWechatAppContextSwitcher.UseCorpScope(...)`（服务商自身凭证即够）。
- PQ4 用「`TokenManagerKey` 不等于两个归属域键」反向断言「服务商级令牌不叠加归属域键」，
  并注意**反射陷阱**：属性未显式书写时读到的是构造函数默认值哨兵，所以不能断言「等于 null」。
  口径见 [AGENTS.md](../../../../../AGENTS.md) 与
  [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1）。
- 由于本族令牌由服务商自身持有、不与授权企业混用，本域也**没有**任何第三方 `permanent_code` 参与：
  #1/#2 的链路只依赖 `provider_access_token` + `register_code`。
- #3/#4 的形态对齐 [../Authentication/IWechatWorkProviderAuthenticationUrl.cs](../Authentication/IWechatWorkProviderAuthenticationUrl.cs)
  （不带 `[Token]` + 显式 Query），接口 remarks 已互相指认；这两支也**不出现**在 G5 白名单里，
  这不是漏挂，而是「无 SDK 管理令牌」的必然结果。

## 10. 字段名一律照抄官方原文（PQ6 逐条锁定）

本域所有 JSON 名都由 PQ6 的 `JsonNameShouldBe` 逐属性钉死，改名即红：

| DTO | 属性 → JSON 名 |
|---|---|
| `GetRegisterCodeRequest` | `TemplateId` → `template_id`、`CorpName` → `corp_name`、`AdminName` → `admin_name`、`AdminMobile` → `admin_mobile`、`State` → `state`、`FollowUser` → `follow_user` |
| `GetRegisterCodeResponse` | `RegisterCode` → `register_code`、`ExpiresIn` → `expires_in` |
| `GetRegisterInfoRequest` | `RegisterCode` → `register_code` |
| `GetRegisterInfoResponse` | `Corpid` → `corpid`、`ContactSync` → `contact_sync`、`AuthUserInfo` → `auth_user_info`、`State` → `state`、`TemplateId` → `template_id` |
| `ContactSyncCredential` | `AccessToken` → `access_token`、`ExpiresIn` → `expires_in` |
| `SetAuthorizedAppScopeRequest` | `AllowUser` → `allow_user`、`AllowParty` → `allow_party`、`AllowTag` → `allow_tag` |
| `SetAuthorizedAppScopeResponse` | `InvalidUser` → `invaliduser`、`InvalidParty` → `invalidparty`、`InvalidTag` → `invalidtag` |

`follow_user` 官方口径是**单个 userid 字符串**（不是数组）；`corpid` 全小写、不加下划线。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块（父接口 IsAbstract，不可注入）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPromotionQrCodeApi());

public sealed class PromotionRegisterFlow(
    IWechatWorkThirdPartyServicePromotionQrCodeService register,
    IWechatWorkPromotionQrCodeContactSyncService contactSync)
{
    // 1) 生成注册码：走服务商 provider_access_token（基座自动注入 Query，调用侧不碰凭据）
    public async Task<string> CreateRegisterCodeAsync(CancellationToken ct)
    {
        var resp = await register.GetRegisterCodeAsync(new GetRegisterCodeRequest
        {
            TemplateId = "TPL_FAKE_0001",              // string，官方必填，最长 128 字节（推广包 ID）
            CorpName = "示例新企业",                     // 选填：注册页自动预填
            AdminName = "李四",
            AdminMobile = "+8613800000000",            // 假手机号
            State = "channel0001",                      // 仅英文字母与数字，本域上限 128 字节（勿套邻域的 32）
            FollowUser = "sp_staff_fake0001",           // 必须是服务商所在企业成员（假 userid）
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/service/get_register_code");

        return resp.RegisterCode;                       // 只能消费一次；expires_in 为链接有效期（示例 600）
    }

    // 2) 查询注册状态：register_code 生成后 24 小时内可查；contact_sync 是条件返回
    public async Task<string?> QueryCorpidAsync(string registerCode, CancellationToken ct)
    {
        var info = await register.GetRegisterInfoAsync(
            new GetRegisterInfoRequest { RegisterCode = registerCode }, ct);
        WechatWorkException.ThrowIfFailed(info, "/cgi-bin/service/get_register_info");
        // 84024 = 非全新创建企业，不支持用本接口查询

        if (info.ContactSync is null)
        {
            return info.Corpid;                         // 该推广包未开启通讯录迁移：#3/#4 不可用
        }

        string contactToken = info.ContactSync.AccessToken;   // 凭据：不落日志、不进异常消息
        // 3) 修改授权应用可见范围：三个 allow_* 未填即清空 ⇒ 想保留的维度必须显式重填
        var scope = await contactSync.SetAuthorizedAppScopeAsync(contactToken, new SetAuthorizedAppScopeRequest
        {
            AgentId = 1000001,
            AllowUser = ["zhangsan0001"],               // 假 userid
            AllowParty = [2, 3],
        }, ct);
        WechatWorkException.ThrowIfFailed(scope, "/cgi-bin/agent/set_scope");

        List<string> invalidUsers = scope.InvalidUser ?? new List<string>();
        List<int> invalidParties = scope.InvalidParty ?? new List<int>();
        if (invalidUsers.Count > 0 || invalidParties.Count > 0)
        {
            // 官方以非法项列表回传，errcode 仍可能是 0 ⇒ 必须自行检查这三个列表
        }

        // 4) 收尾：GET 却不可逆（同步完成 + 解锁 + 作废迁移凭证）⇒ 只调用一次，失败不得盲目重放
        await contactSync.SetContactSyncSuccessAsync(contactToken, ct);
        // 此后 contactToken 已失效，且不能再调 SetAuthorizedAppScopeAsync

        return info.Corpid;
    }
}
```

判错为单层普通信封（`errcode == 0` 即成功）。
本域 #1/#2 的 `provider_access_token` 由基座按令牌路由键注入并自动恢复；
#3/#4 的迁移凭证由宿主显式传参，**过期与作废都要自己判**，基座不会重试。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 四支端点的频率数字 | 三处 remarks 原文都是「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」⇒ 具体阈值未在本域留档，**未逐项核验** | 需要限流时先核验官方附录页，勿从邻域借数值 |
| 注册链接的拼接模板 | 本域只产出 `register_code` 字符串；「把码拼成可点击注册链接」的 URL 形状与参数名本仓未留档 ⇒ **未逐项核验** | 前端跳转页与官方链接模板一并核验后再写示例 |
| `expires_in` 的实际语义上限 | 只有官方**示例值** 600（#1）与 1800（`contact_sync`），是否随推广包/企业配置变化**未逐项核验** | 不得把示例值当成长期常量硬编码 |
| `template_id` 的签发面 | 注释只说明来源是服务商管理端的注册推广包，本域无对应端点 ⇒ 获取面归属**未在本域核验** | 需要程序化建包时先核验官方，勿在本域加端点 |
| `auth_user_info` 的字段全集 | 本仓 [RegisterAuthUserInfo.cs](../../DataModels/PromotionQrCode/RegisterAuthUserInfo.cs) 只承载 `userid`；官方是否另有授权管理员姓名/手机号等字段**未逐项核验** | 补字段须先核验原文再同批改 PQ5 的 8 型计数 |
| 迁移 `access_token` 可调用的通讯录 API 面 | 本域只消费该凭证于 #3/#4；「它是否有全部通讯录读写权限之外的边界」只有 remarks 一句「有全部通讯录读写权限」⇒ 可用面清单**未逐项核验** | 需要扩到其它通讯录端点时，先核验邻域文档，勿在本域加端点 |
| 错误码全集 | 本域仅留档 **84024**；`register_code` 过期/已消费、`template_id` 无效、可见范围越界等场景的码值**未逐项核验** | 出现新码先核验，再统一登记码表与判错文案 |
| 「注册完成回调事件」的载荷形状 | 该事件返回的 `AccessToken` 与 `state` 是 #3/#4 的另一凭证来源，但**事件载荷在 Callback 包，不属本域**；本域不建模事件 | 需要回调侧字段时查 Callback 包，勿在本域加 DTO |
| 自建 / 代开发是否补页 | PQ2 断言继承链上**只能**有第三方子接口；官方若补自建/代开发文档页，本域层级即需重开 ⇒ 现状以外的情形**未核验** | 官方补页后同批改 PQ2、G5（若引入 `[Token]`）与本文速览 |
| 包级 README 的整体计数陈旧 | [../../README.md](../../README.md) 对本域「2 支 provider 令牌 + 2 支通讯录迁移令牌」的描述与本文一致，但其「35 个业务域 / ContractGuards 61 个文件」两处统计实测为 **38 / 70** | 该文件是导航性描述、不作事实来源；本域事实以 PQ1~PQ6 与本目录 remarks 为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（512 字节 / 24 小时 / 30 分钟 / 128 字节 / 600 与 1800 示例 / 未填即清空 / 84024） | 本目录 3 份接口文件的 XML 注释与 `[Token]` / `[Query]` 声明 |
| 路由与动词表（恰 4，含 `GetAttribute` 那支） | [WechatPromotionQrCodeContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPromotionQrCodeContractGuards.cs) PQ1（表 L54–L72 / 断言 L78，含 `HaveCount(4)` + `Distinct` 双断言） |
| 企业注册族层级（零端点父接口、唯一第三方子接口、不得出现自建/代开发子接口、`InheritedFrom` 与注册组） | PQ2（L102–L131） |
| 通讯录迁移族层级（无 `[Token]`、无父接口、恰 2 端点、两支都有 `[Query("access_token")]`） | PQ3（L138–L175） |
| 令牌绑定（`ProviderAccessToken` + Query + `provider_access_token`，且不等于两个归属域键） | PQ4（L182–L207） |
| JSON 上下文登记（恰 8 型：请求 3 + 响应 3 + 嵌套 2；#4 无 DTO） | PQ5（L214–L238）与生成物 [PromotionQrCodeJsonContext.g.cs](../../DataModels/Generated/PromotionQrCodeJsonContext.g.cs)（`internal`，示例不得引用） |
| 官方字段名与「未填即清空 / state 字符集」陷阱 | PQ6（L246–L283，`JsonNameShouldBe` 辅助 L286–L293） |
| DTO 属性名 / 类型 / JSON 名 / 可空性 | [../../../Mud.Wechat.Work/DataModels/PromotionQrCode/](../../../Mud.Wechat.Work/DataModels/PromotionQrCode/)（5 个根文件 + [RequestModel/](../../../Mud.Wechat.Work/DataModels/PromotionQrCode/RequestModel/) 3 个） |
| 邻域对照：`state` 的 32 字节口径与二维码图片二进制返回形态 | `../Authentication/IWechatWorkProviderAuthenticationService.cs`（`get_app_qrcode`）、`../Authentication/IWechatWorkProviderAuthenticationUrl.cs`（无 `[Token]` + 显式 Query 的同形态先例） |
| 令牌归属键与 TO1 口径 | [AGENTS.md](../../../../../AGENTS.md)、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| G5 Query 令牌白名单（本域**不**在内，理由见第 9 节） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)（`PromotionQrCode` 成员）、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)（L126–L128 的 `AddPromotionQrCodeWebApiHttpClient()`、L251 的 `AddPromotionQrCodeApi()`）、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（`PromotionQrCodeJsonContext.Default`） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号（90581~90584 段）与限制原文，**同批**改
   [WechatPromotionQrCodeContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPromotionQrCodeContractGuards.cs)
   的 `Routes` 表（PQ1，恰 4，元素含 `Type HttpAttribute` 即**动词一起锁**）与 `HaveCount` + `Distinct` 双断言；
   `contact_sync_success` 的 **GET** 不得改成 POST（PQ1 表注释已写明「勿顺手统一」）。
2. 官方若为自建 / 代开发补本族文档页 → 先核验，再同批改 PQ2 的 `derived` 集合断言（当前只能是唯一第三方子接口）、
   PQ4 的接口清单、本文速览的「开放面」列；若新子接口引入 `[Token]`，还须同批把它登记进
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
   的 G5 白名单（当前 #3/#4 因无 SDK 管理令牌而**不入**白名单，改形态前先想清楚 PQ3 的 `BeNull` 断言）。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "PromotionQrCode")]`，跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1`，
   同批更新 PQ5 的 **8** 型全量计数与 [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)
   的 `PromotionQrCodeJsonContext.Default` 合并项。字段名照抄官方原文（PQ6 逐条锁定），
   「参数表 vs 示例」冲突不得自行改写；`allow_*` 三者必须保持可空（清空语义依赖它）。
   多目标框架（`netstandard2.0` 等）下不得使用 `init` / `record`。
4. 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 的 `PromotionQrCode` 成员与 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddPromotionQrCodeApi()` XML
   （其端点叙述须与本域「4 = 2 + 2、两族两凭据」一致）与注册项、两族可注入接口的 `RegistryGroupName = "PromotionQrCode"`
   （父接口保持空组）。
5. 凭据面改动一律不得引入真值：`provider_access_token`、通讯录迁移 `access_token`、`permanent_code`、`auth_code`、
   `suite_ticket`、`corpsecret` 只写参数名；示例中的 `userid` / `external_userid` / 手机号一律假值；
   新增进 Query 的敏感参数名须同批登记脱敏词表（`SensitiveUrlKeys.Register` / G7）。
   任何把 #3/#4 令牌改为基座托管的改动，等同于撤回「SDK 不缓存该凭证」的决策，必须先立风险记录再动 PQ3。
6. 本域**不含回调事件**：「注册完成回调事件」与推广注册相关的**事件载荷在 Callback 包，不属本域**；
   本域只有同步 HTTP 面，勿在本目录加事件 DTO。
7. 本文与 [../../README.md](../../README.md) 的本域端点数与令牌口径表述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
