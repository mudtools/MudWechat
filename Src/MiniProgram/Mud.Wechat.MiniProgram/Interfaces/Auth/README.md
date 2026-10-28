# Auth 域 · 登录与用户

微信小程序「登录与用户」域：**8 支端点**（`/sns/jscode2session` + `/wxa/` 七支）。
接口 [IWxaAuthService.cs](./IWxaAuthService.cs)（8 支全部承载于此），注册组 `Auth`
（`RegistryGroupName = "Auth"`、`TokenManage = nameof(IMpAppManager)`），
DI 组方法 [`AddAuthApi()`](../../Extensions/MiniProgramServiceBuilder.cs)（`MiniProgramModule.Auth`）。
本域另有过渡弃用接口 [IWxaCode2SessionService.cs](./IWxaCode2SessionService.cs)（`[Obsolete]`，
仅重复声明 `/sns/jscode2session`，Mud.Wechat 4.0 移除），**不计入 8 支**。
逐页核验 2026-10-18（本 README 的取证日期；接口 XML 内另记有各自核验日）。

> **事实来源限定**：本文所有断言只来自本仓的接口文件、契约守卫与 DTO 声明，
> 不引用任何外部文档原文。凡本仓核不到的（官方频率数字、错误码全集、权限集归属等），
> 一律集中列在文末「已知留档缺口」并标 **未逐项核验**，不做推测。

## 端点速览

本线与公众号同属微信公众平台，令牌域**复用** `MpTokenTypes.AccessToken`（值 `"Wechat.Mp.AccessToken"`），
注入方式为 **Query 的 `access_token`**（守卫 MP-X2 / MP-X3，见文末）。因此除 `Code2Session` 外，
下表「令牌」列一律为 `access_token`(Query)。

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `Code2SessionAsync` | GET | `/sns/jscode2session` | **无**（方法级 `[Token(Disabled = true)]`） | 见下节；Query 带 `appid`/`secret`/`js_code`/`grant_type` |
| 2 | `CheckSessionAsync` | GET | `/wxa/checksession` | `access_token`(Query) | Query `signature` + `sig_method`；应答仅 `errcode`/`errmsg` |
| 3 | `ResetUserSessionKeyAsync` | GET | `/wxa/resetusersessionkey` | `access_token`(Query) | 同上两支 Query；旧 `session_key` 立即失效 |
| 4 | `GetPhoneNumberAsync` | POST | `/wxa/business/getuserphonenumber` | `access_token`(Query) | 体 `WxaGetPhoneNumberRequest { code }`；按次计费 |
| 5 | `GetPaidUnionIdAsync` | GET | `/wxa/getpaidunionid` | `access_token`(Query) | `openid` + `transaction_id`/`out_trade_no` 二选一 |
| 6 | `GetPluginOpenPidAsync` | GET | `/wxa/getpluginopenpid` | `access_token`(Query) | Query `code` + `plugin_appid` |
| 7 | `CheckEncryptedMsgAsync` | POST | `/wxa/business/checkencryptedmsg` | `access_token`(Query) | 应答键名为官方拼写 `vaild` |
| 8 | `GetUserEncryptKeyAsync` | POST | `/wxa/business/getuserencryptkey` | `access_token`(Query) | 体 `WxaGetUserEncryptKeyRequest { openid, signature, sig_method }` |

### 免令牌端点：`Code2SessionAsync`（本域唯一无令牌支）

- 接口级已声明 `[Token(TokenType = MpTokenTypes.AccessToken, ...)]`，本方法用**方法级
  `[Token(Disabled = true)]` 覆盖接口级**（上游 3.0.7 / F-1 起支持）。
  换会话走 `appid` + `secret`，若错误地注入应用级令牌属**语义错误**——守卫 MP-X2b 锁定该裁决。
- 因此本域**不套用**「Query 恒带 `access_token`」的默认理解；写跨端点封装时须把第 1 支单列。
- `grant_type` **无默认值**，须显式传 `WxaGrantTypes.AuthorizationCode`（值 `"authorization_code"`，
  常量定义在 [IWxaCode2SessionService.cs](./IWxaCode2SessionService.cs)）。
- 参数名 `jsCode` → Query 键 `js_code`（MP-X5 锁定，不得驼峰化）。
- 应答 `WxaCode2SessionResponse`：`OpenId`→`openid`、`SessionKey`→`session_key`（MP-X5）、
  `UnionId`→`unionid`。`unionid` 为**条件返回**，本仓未逐项核验其触发条件全集（见缺口）。
- **`session_key` 属敏感凭据**：不得进日志、遥测或异常消息（MP-X7 同时禁止
  `WxaCode2SessionResponse` 覆写 `ToString`，防止默认打印带出）。`appsecret` 只写参数名，
  示例一律用占位符。
- 本通道不涉及流式响应，无 `Dispose` 义务（本线需要释放的是 `WxaCodeResult`，属 `QrCodeLink` 域）。

## 路由前缀族：`/sns/` 与 `/wxa/` 混排

| 前缀族 | 本域端点 | 口径 |
|---|---|---|
| `/sns/jscode2session` | 第 1 支 | 免令牌换会话族 |
| `/wxa/checksession`、`/wxa/resetusersessionkey`、`/wxa/getpaidunionid`、`/wxa/getpluginopenpid` | 第 2/3/5/6 支 | `/wxa/` 直属，**连写无下划线** |
| `/wxa/business/getuserphonenumber`、`/wxa/business/checkencryptedmsg`、`/wxa/business/getuserencryptkey` | 第 4/7/8 支 | `/wxa/business/` 子族 |

- **连写陷阱**：`getuserphonenumber` / `getpluginopenpid` / `getpaidunionid` /
  `checkencryptedmsg` / `getuserencryptkey` / `resetusersessionkey` 全部是**单词连写**，
  官方无下划线、无驼峰；写成 `get_user_phone_number` 即 404。守卫 MP-X5 以逐字路由数组钉死。
- **官方另有代商家变体** `/sns/component/jscode2session`，属**开放平台线**，本线**零声明**
  （守卫 NC2 把它列入「非本线契约路由」参照集，防止误加）。

## 签名字符串的算法口径

本域三支用到签名，本仓**只透传、不在服务端计算**：

| 端点 | 官方口径（接口 XML 原文） | SDK 侧 |
|---|---|---|
| `CheckSessionAsync` | `signature = hmac_sha256(session_key, rawData)`，`rawData` 为 `wx.checkSession` 场景参与签名的原始串 | 由小程序端与宿主产出，SDK 原样进 Query |
| `GetUserEncryptKeyAsync` | `signature = hmac_sha256(session_key, openid)` | 同上，SDK 不计算 |
| `CheckEncryptedMsgAsync` | `encrypted_msg_hash = to_hexstr(sha1(encrypted_msg))` | 由宿主计算后放进请求体 |

`sig_method` 常量：`WxaSignatureMethods.HmacSha256`（值 `"hmac_sha256"`，是**算法名而非凭据**）。
本仓未核验官方是否还接受其他 `sig_method` 取值（见缺口）。

## 逐端点非代码可推契约

1. **`GetPhoneNumberAsync` 按次计费**：接口 XML 明写「按次计费」，且 `code` **一次性**消费、
   应答 `phone_info.watermark.appid` 须由调用方**自行比对**当前 appid（SDK 不做该比对）。
   计费单价与本域逐端点频率数字本仓**未逐项核验**（见缺口）。
2. **`ResetUserSessionKeyAsync` 非日常调用**：调用后旧 `session_key` 立即失效，
   接口 XML 标注不用于常规流程。
3. **`GetPaidUnionIdAsync` 二选一**：`transaction_id` 与 `out_trade_no` 只能给一支；
   两者同时给时接口 XML 记为 **`transaction_id` 优先**（本仓按 XML 原文承载，未核验官方行为）。
4. **`CheckEncryptedMsgAsync` 适用范围与窗口**：接口 XML 记为仅适用**手机号**加密数据、
   仅覆盖**近 3 天**的报文；应答键 `vaild`（官方拼写缺陷，**照原文承载不得修正**，
   `WxaCheckEncryptedMsgResponse.Vaild`）。
5. **`GetUserEncryptKeyAsync` 应答为列表**：`KeyInfoList`→`key_info_list`，
   每项 `WxaUserEncryptKeyInfo { encrypt_key, version, expire_in, iv }`（`Version`/`ExpireIn` 为 `long?`）。
   `encrypt_key` / `iv` 属解密材料，同 `session_key` 级别处置：**禁入日志与异常消息**。
6. **POST / GET 的直觉不可用**：本域查询类第 2/3/5/6 支确为 GET；第 4/7/8 支为 POST（走请求体）。
   与 `Charge`/`DataAnalysis`/`CloudBase` 域「查询也全是 POST」的形态不同，跨域封装**不得**共用动词假设。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram.DataModels.Auth;
using Mud.Wechat.OfficialAccount.Abstractions;   // AddMpApp：令牌底座仍由公众号包提供

// 两段式装配：先建 MP 令牌 / 多应用底座，再装本线模块
builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddAuthApi());

// 注入 IWxaAuthService（示例用字段名 auth）
// 1) 免令牌换会话：grant_type 无默认值，须显式传常量
var session = await auth.Code2SessionAsync(
    appId: "<APPID>",
    secret: "<APP_SECRET>",        // 占位符；真实值不得入日志/遥测/异常消息
    jsCode: "<JS_CODE>",           // 一次性
    grantType: WxaGrantTypes.AuthorizationCode,
    cancellationToken: ct);
WxaException.ThrowIfFailed(session, "/sns/jscode2session");
// session.OpenId / session.UnionId 可用；session.SessionKey 只留在内存，不得打印

// 2) 手机号（按次计费，code 一次性）
var phone = await auth.GetPhoneNumberAsync(
    new WxaGetPhoneNumberRequest { Code = "<PHONE_CODE>" }, ct);
WxaException.ThrowIfFailed(phone, "/wxa/business/getuserphonenumber");
// 自行比对水印 appid，SDK 不校验
var pure = phone.PhoneInfo?.PurePhoneNumber;        // JSON 键 purePhoneNumber

// 3) 加密报文核验：应答键是官方拼写 vaild
var check = await auth.CheckEncryptedMsgAsync(
    new WxaCheckEncryptedMsgRequest
    {
        EncryptedMsgHash = "<SHA1_HEX_OF_ENCRYPTED_MSG>",
        OpenId = "<OPENID>",
    }, ct);
WxaException.ThrowIfFailed(check, "/wxa/business/checkencryptedmsg");
var ok = check.Vaild;                                 // bool?

// 4) 登录态校验：signature 由端上/宿主算好，SDK 不重算
var alive = await auth.CheckSessionAsync(signature: "<SIGNATURE>",
    sigMethod: WxaSignatureMethods.HmacSha256, cancellationToken: ct);
```

本域**没有**返回流或 `IDisposable` 应答的端点，无需 `using`；
本线需要释放的是 `QrCodeLink` 域的 `WxaCodeResult`，不在本域。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **本域无独立守卫文件** | 断言分散在 [MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)（MP-X5 路由/计数、MP-X2b 免令牌裁决、MP-X5 字段名、MP-X7 `session_key` 日志面）+ [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)（MP-X2 令牌复用）+ [MiniProgramNonContractRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramNonContractRouteContractGuards.cs)（NC2 代商家变体归属） | 若要收拢，需新建 `IWxaAuthService` 专属守卫文件并同批更新本文「事实来源」 |
| `unionid` 返回的触发条件全集 | 接口 XML 只给「条件返回」措辞，**未逐项核验** | 核验后补 remarks，勿据常识补全 |
| `js_code` 单次使用 / 有效期、`40029`/`45011`/`40226` 等错误码全集 | 本文不重复接口 XML 之外的解释；错误码表未逐页核验 | 需要判错表时先核验再补 |
| 官方逐端点调用频率与计费单价 | **未逐项核验** | 不得写入本文；核验后同批更新 |
| 本域权限集 ID | 接口与守卫均未登记，**未核验** | 与 `DataAnalysis`/`CloudBase` 不同，勿假设有权限集数字 |
| 其他 `sig_method` 取值 | 本仓只有 `hmac_sha256` 一个常量 | 未核验官方是否另有取值 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、官方 URL、错误码、签名口径 | [IWxaAuthService.cs](./IWxaAuthService.cs) 的 XML 注释；常量 `WxaSignatureMethods` |
| 免令牌裁决 / `grant_type` 常量 / 弃用说明 | [IWxaCode2SessionService.cs](./IWxaCode2SessionService.cs)（`WxaGrantTypes.AuthorizationCode`、`[Obsolete]` 文案） |
| 路由集合（8 条逐字）与合计计数 398 | `MiniProgramContractGuards` 的 MP-X5（`Endpoints_ShouldMatchOfficialRoutesAndCount`）；计数基线在 `Tests/ContractBaseline.cs` |
| `[Token].Name == access_token`、令牌类型复用 | MP-X3 / `MiniProgramScaffoldContractGuards` 的 MP-X2 |
| `session_key` 不得进日志、应答不得覆写 `ToString` | MP-X7 |
| 应答 / 请求 DTO 字段名与类型 | [../../../Mud.Wechat.MiniProgram/Auth/](../../../Mud.Wechat.MiniProgram/Auth/)（12 个文件，`SerializerClassName = "Auth"`） |
| AOT 序列化上下文（`internal`，示例中不得引用） | `../../../Mud.Wechat.MiniProgram/Generated/AuthJsonContext.g.cs` |
| 本线端点总表与装配入口 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方 URL 与逐参数约束，**同批**更新 MP-X5 的本域路由数组
   与合计计数（当前 8；全线 398 = 392 特性路由 + 6 手工通道，基线在 `Tests/ContractBaseline.cs`）。
2. **加 / 改 DTO** → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Auth`）→
   跑 `scripts/AddHttpJsonSerializable.ps1` 与 `scripts/GenerateJsonContext.ps1` →
   确认并入 `AuthJsonContext`；字段名**照官方原文**（MP-X5 会因驼峰化而红）。
3. **改注册面** → `MiniProgramModule.Auth` + `AddAuthApi()`（[MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）
   与本域注册委托须同步；新增接口时确认其仍落 `RegistryGroupName = "Auth"`。
4. **更新本文**：速览表、计数、缺口与「事实来源」四处。
5. **凭据红线**：新增参数若承载 `session_key` / `appsecret` / `encrypt_key` / `iv` /
   `openid` / `unionid` 等，须同批改 MP-X7 的日志扫描面，并确认应答 DTO 不覆写 `ToString`。
6. **本线无回调工程**：消息与事件接收属公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`），
   形态由 [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)
   （MP-X4：`Mud.Wechat.MiniProgram.Callback.csproj` 不得存在）锁定，
   **不要**为「登录事件」顺手补回调工程。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
