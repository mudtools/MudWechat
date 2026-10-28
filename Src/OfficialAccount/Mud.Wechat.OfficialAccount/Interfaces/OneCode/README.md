# OneCode 域 · 微信「一物一码」

微信公众号 / 服务号「一物一码」（一码通）域：**6 端点**（申请 / 查询申请单 / 下载二维码包 / 激活 /
查询激活 / ticket 换码），全 POST、同前缀 `/intp/marketcode/`。接口
[IMpOneCodeService.cs](./IMpOneCodeService.cs)，注册组 `OneCode`，模块枚举 `MpModule.OneCode`
（`AddOneCodeApi()` ⇒ 生成入口 `AddOneCodeWebApiHttpClient()`）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpOneCodeContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpOneCodeContractGuards.cs)
> （`OT1`~`OT6`）、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/OneCode/](../../../Mud.Wechat.OfficialAccount/OneCode/)）。
> **计数以守卫为准**：`OT1` 钉「6 条 POST 路由、同前缀、每端点均带请求体」。`QT1` 注释记 6 端点。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `ApplyCodeAsync` | POST | `/intp/marketcode/applycode` | 应用级 `access_token`（Query） | `code_count` 10000 倍数 ∈ [10000, 20000000]；`isv_application_id` 幂等键 |
| 2 | `QueryCodeApplicationAsync` | POST | `/intp/marketcode/applycodequery` | 应用级 `access_token`（Query） | `application_id` 或 `isv_application_id` 二选一；轮询至 `status=FINISH` |
| 3 | `DownloadCodePackageAsync` | POST | `/intp/marketcode/applycodedownload` | 应用级 `access_token`（Query） | 下载前须 `status=FINISH`；返回 JSON 字符串 `buffer`（base64） |
| 4 | `ActivateCodeAsync` | POST | `/intp/marketcode/codeactive` | 应用级 `access_token`（Query） | 返回 `MpResponse`；`code_start`/`code_end` 闭区间 |
| 5 | `QueryCodeActivationAsync` | POST | `/intp/marketcode/codeactivequery` | 应用级 `access_token`（Query） | `application_id`+`code_index` 或 `code`/`code_url` 两路径互斥 |
| 6 | `ExchangeCodeTicketAsync` | POST | `/intp/marketcode/tickettocode` | 应用级 `access_token`（Query） | `openid`+`code_ticket`；与 5 共用 `MpCodeInfoResponse` |

**账号门槛（关键，勿写成「仅认证」）**：6 页适用范围**均为「服务号（需申请）」**——须先向官方申请开通该能力，
未开通即不可调用（SDK 不做本地闸，由官方错误码表达）；本域**无**主体配额类错误码。

## 契约要点（代码名读不出的部分）

1. **账号资格是「需申请」不是「仅认证」**：与模板消息 / OCR 域的「仅认证」措辞不同，本域是「服务号（需申请）」。

2. **无新票据体系 / 不耦合支付**：6 端点全部消费应用级 `access_token`（Query 注入）；下载所得 `buffer`
   需 base64 decode + 解密（官方未给算法与密钥来源），与令牌体系无关；本域不耦合支付。

3. **`code_count` 硬约束**：官方原文「必须是 **10000 的整数倍**，范围 **10000-20000000**」⇒ 无法小额申请；
   SDK **不做本地范围/倍数拦截**（越界交官方表达）。

4. **幂等键 `isv_application_id`**：官方原文「相同 `isv_application_id` 视为同一申请单」⇒ 重试不重复生成。

5. **响应为平级字段（无 `data` 包裹）**：根即 `errcode`/`errmsg` + 业务字段，**与门店域 `data` 包裹形态不同**；
   `OT2` 钉 `codeactive` 返回 `MpResponse`、`codeactivequery` 与 `tickettocode` 共用 `MpCodeInfoResponse`，
   且响应类型集**不含 `data` 键**。

6. **业务闭环（SDK 只给端点、不编排）**：申请 → 轮询 `applycodequery` 至 `status=FINISH`（取码段
   `code_generate_list`）→ 按码段下载 → 激活（绑定小程序跳转）→ 查询激活；另有 ticket 换正式营销码。
   `MpCodeApplyStatuses.Finished = "FINISH"` 只登记示例值——官方**未给状态枚举表**，宿主轮询应以
   「**非 Finished**」为继续条件，**不得**据此推断 `PROCESSING` 之类中间态（`OT6`）。

7. **零新增错误码常量**：6 页错误码表**仅列通用 `40001`**（`applycode` 页另误挂与本接口无关的 `40002 invalid grant_type`）
   ⇒ 本域**不建任何错误码常量**，`OT6` 负向断言「无 `OneCode` 前缀的错误码常量名」，复用 `InvalidCredential=40001` /
   `InvalidGrantType=40002`。

8. **两处官方文档缺陷的建模裁决（`OT4` 负向锁定，防「顺手修正」）**：
   - 下载页类型列写「formdata 文件 buffer」（非法 JSON 类型），而同页示例为 JSON 包裹的 `buffer` ⇒ SDK 按**示例**
     建为**字符串** `MpCodeDownloadResponse.Buffer`，走既有 JSON 管线、**不新建下载通道**；官方「解密参见 3.1」指向不存在的章节。
   - `MpCodeActiveQueryRequest.Code` 为**字符串**（九位原始码，前导零不可丢）；`ApplicationId` 请求侧取字段表数值型 `long?`
     （官方请求示例写字符串系文档矛盾）。`tickettocode` 页未列 Query 参数表（疑漏）⇒ SDK 按同域契约仍以 Query 注入 `access_token`。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddOneCodeApi());

// 注入 IMpOneCodeService（注册组 OneCode）

// 1) 申请二维码：code_count 必须 10000 的整数倍 ∈ [10000, 20000000]；isv_application_id 为幂等键
MpApplyCodeResponse applied = await oneCode.ApplyCodeAsync(new MpApplyCodeRequest
{
    CodeCount = 10000,
    IsvApplicationId = "ISV-ORDER-20261018-0001",
}, ct);
MpException.ThrowIfFailed(applied, "/intp/marketcode/applycode");
long applicationId = applied.ApplicationId ?? 0;

// 2) 轮询申请单至 FINISH（以「非 Finished」为继续条件，勿猜中间态）
MpCodeApplyQueryResponse query = await oneCode.QueryCodeApplicationAsync(
    new MpApplyCodeQueryRequest { ApplicationId = applicationId }, ct);
MpException.ThrowIfFailed(query, "/intp/marketcode/applycodequery");
bool finished = string.Equals(query.Status, MpCodeApplyStatuses.Finished, StringComparison.Ordinal);
List<MpCodeRange> ranges = query.CodeGenerateList ?? new List<MpCodeRange>();

// 3) 下载码包（须 status=FINISH）：buffer 为 base64 JSON 字符串，decode + 解密归宿主
MpCodeDownloadResponse pkg = await oneCode.DownloadCodePackageAsync(new MpCodeDownloadRequest
{
    ApplicationId = applicationId,
    CodeStart = 0,
    CodeEnd = 9999,
}, ct);
MpException.ThrowIfFailed(pkg, "/intp/marketcode/applycodedownload");
string buffer = pkg.Buffer ?? string.Empty;

// 4) 激活码段（闭区间），绑定小程序跳转；wxa_type 默认 0 正式版
MpException.ThrowIfFailed(
    await oneCode.ActivateCodeAsync(new MpCodeActiveRequest
    {
        ApplicationId = applicationId,
        ActivityName = "SUMMER",           // 分析维度，官方要求规范命名
        ProductBrand = "BRAND-A",
        ProductTitle = "SKU-A",
        ProductCode = "0000000000000",     // EAN 条码（示例假值）
        WxaAppId = "wxfakefakefakefake",
        WxaPath = "pages/index/index",
        WxaType = MpCodeWxaTypes.Release,
        CodeStart = 0,
        CodeEnd = 9999,
    }, ct),
    "/intp/marketcode/codeactive");

// 5) ticket 换正式营销码（openid 为敏感值，勿入日志；此处用占位变量）
MpCodeInfoResponse exchanged = await oneCode.ExchangeCodeTicketAsync(
    new MpTicketToCodeRequest { OpenId = openIdVariable, CodeTicket = codeTicketVariable }, ct);
MpException.ThrowIfFailed(exchanged, "/intp/marketcode/tickettocode");
```

凭据红线：`access_token`、`openid`、`code_ticket` 只作参数名，不得进日志、遥测或异常消息；`openid`/`code_ticket`
以变量占位而非字面量。小程序 appid / 商品条码等示例均为假值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 申请单状态枚举 | 官方未给枚举表，示例仅 `FINISH` | SDK 只登记示例值；宿主以「非 Finished」续轮询，勿建中间态 |
| `download` 响应解密 | 官方「解密参见 3.1」指向不存在章节、算法与密钥来源未核验 | `buffer` 按 JSON 字符串建模；解密归宿主，核验前不写算法结论 |
| `tickettocode` 的 Query 参数表 | 官方页未列（疑漏），SDK 仍按同域注入 `access_token` | 若官方补表则复核注入面 |
| 第三方代调用面 | 6 页均记权限集 id **46**（M0-R3 裁决只记 XML、不扩实现面） | 按裁决执行，勿为此扩接口 |
| 码段合法范围 / 跨度 | `code_start`/`code_end` 官方未给合法范围、是否允许倒序；说明上限 9999 与示例 200 不一致 | 照录 `MpCodeActiveRequest` remarks，不本地拦截 |
| 频率限制 | 6 页均无频率章节、无频次数值 | SDK 不编造频次数 |
| OneCode DTO 数 | `OT5` 钉 11 个 DTO 登记进 `OneCodeJsonContext`（`SerializerClassName = OneCode`） | 漂移先核对官方再同批改 `OT5` |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文、「需申请」门槛、code_count 约束、幂等键、平级响应 | [IMpOneCodeService.cs](./IMpOneCodeService.cs) 的 XML 注释 |
| 6 条 POST 路由同前缀、每端点带请求体 | `OT1` = `OneCodeEndpoints_ShouldMatchOfficialRoutes` |
| 响应形态（平级 / 复用 / 共用）、无 `data` 键 | `OT2` = `OneCodeResponseShapes_ShouldKeepAdjudicatedForms` |
| 逐类型官方字段名 | `OT3` = `OneCodeShapes_ShouldLockOfficialFieldNames` |
| 文档冲突处置（Code 为 string / Buffer 为 string / 请求侧 long?）负向锁定 | `OT4` = `OneCodeDocumentConflicts_ShouldKeepAdjudicatedShapes` |
| DTO 登记 11 + 模块 + 白名单 + 令牌 | `OT5` = `OneCodeModuleAndDataModels_ShouldBeRegistered` |
| `MpCodeWxaTypes` 0/1/2、`Finished=FINISH`、零新增错误码 | `OT6` = `OneCodeConstants_ShouldMatchVerifiedOfficialValues` |
| 字段名与类型（申请 / 查询 / 下载 / 激活 / 换码） | [DataModels/OneCode/](../../../Mud.Wechat.OfficialAccount/OneCode/)（11 个文件） |
| 生成上下文清单 | [Generated/OneCodeJsonContext.g.cs](../../../Mud.Wechat.OfficialAccount/Generated/OneCodeJsonContext.g.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与逐页限制，**同批**更新 `OT1` 的 6 条路由表与 `OT2` 的响应形态断言，
   以及 [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`）
   与 [../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName = OneCode`）→ 跑
   [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)
   → 同批更新 `OT5` 的 11 个 DTO 计数。
3. 确认 [Generated/OneCodeJsonContext.g.cs](../../../Mud.Wechat.OfficialAccount/Generated/OneCodeJsonContext.g.cs) 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 调整注册面 / 模块 → 同批改 [MpModule.cs](../../Extensions/MpModule.cs)、[MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)
   的 `AddOneCodeApi()`，并同步 `QT1` 白名单。
5. **不要**为本域新增任何错误码常量（`OT6` 负向锁定）；**不要**把 `Code`/`Buffer` 从 string 改成数值或流；
   **不要**据 `FINISH` 推断完整状态机。
6. 最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
