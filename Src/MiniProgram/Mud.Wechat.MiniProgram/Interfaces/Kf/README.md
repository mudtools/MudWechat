# Kf 域 · 客服

微信小程序「客服」域 SDK（11 端点：客服角色 2 + 客服子商户 4 + 微信客服 3 + 客服输入状态 1 + 临时素材 1）。 接口：[IWxaKfMediaService](./IWxaKfMediaService.cs)、[IWxaKfService](./IWxaKfService.cs)；注册组 `Kf`；模块枚举 `MiniProgramModule.Kf`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/DataModels/Kf`](../../../Mud.Wechat.MiniProgram/DataModels/Kf/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaKfService = 11；按 HTTP 特性实计 **10** 支，另有手工通道 **1** 支，本域合计 **11** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaKfService` | `SetAdminAsync` | POST | `/customservice/kfaccount/setadmin` | `AccessToken`（Query），Query `access_token` | 设置客服账号角色（管理员）。官方路由：`POST /customservice/kfaccount/setadmin`。<br>官方文档：https://developers.weixi… |
| 2 | `IWxaKfService` | `CancelAdminAsync` | POST | `/customservice/kfaccount/canceladmin` | `AccessToken`（Query），Query `access_token` | 取消客服账号角色（管理员）。官方路由：`POST /customservice/kfaccount/canceladmin`。<br>官方文档：https://developers.we… |
| 3 | `IWxaKfService` | `RegisterBusinessAsync` | POST | `/cgi-bin/business/register` | `AccessToken`（Query），Query `access_token` | 注册客服子商户。官方路由：`POST /cgi-bin/business/register`。<br>官方文档：https://developers.weixin.qq.com/mini… |
| 4 | `IWxaKfService` | `UpdateBusinessAsync` | POST | `/cgi-bin/business/update` | `AccessToken`（Query），Query `access_token` | 更新客服子商户信息。官方路由：`POST /cgi-bin/business/update`。<br>官方文档：https://developers.weixin.qq.com/mini… |
| 5 | `IWxaKfService` | `GetBusinessInfoAsync` | POST | `/cgi-bin/business/get` | `AccessToken`（Query），Query `access_token` | 获取客服子商户信息。官方路由：`POST /cgi-bin/business/get`。<br>官方文档：https://developers.weixin.qq.com/minipro… |
| 6 | `IWxaKfService` | `ListBusinessAsync` | POST | `/cgi-bin/business/list` | `AccessToken`（Query），Query `access_token` | 获取客服子商户列表。官方路由：`POST /cgi-bin/business/list`。<br>官方文档：https://developers.weixin.qq.com/minipr… |
| 7 | `IWxaKfService` | `GetWorkAccountAsync` | POST | `/customservice/work/get` | `AccessToken`（Query），Query `access_token` | 获取微信客服账号信息。官方路由：`POST /customservice/work/get`。<br>官方文档：https://developers.weixin.qq.com/mini… |
| 8 | `IWxaKfService` | `BindWorkAccountAsync` | POST | `/customservice/work/bind` | `AccessToken`（Query），Query `access_token` | 绑定微信客服。官方路由：`POST /customservice/work/bind`。<br>官方文档：https://developers.weixin.qq.com/minipro… |
| 9 | `IWxaKfService` | `UnbindWorkAccountAsync` | POST | `/customservice/work/unbind` | `AccessToken`（Query），Query `access_token` | 解绑微信客服。官方路由：`POST /customservice/work/unbind`。<br>官方文档：https://developers.weixin.qq.com/minip… |
| 10 | `IWxaKfService` | `SendTypingStatusAsync` | POST | `/cgi-bin/message/custom/business/typing` | `AccessToken`（Query），Query `access_token` | 下发客服输入状态（`POST /cgi-bin/message/custom/business/typing`）。<br>官方文档：https://developers.weixin.q… |
| M1 | `IWxaKfMediaService` | `GetMediaAsync` | GET | `/cgi-bin/media/get` | 手工注入 `access_token`（非 `[Token]`） | 获取临时素材（`GET /cgi-bin/media/get?access_token=…&media_id=…`）。<br>官方文档：https://developers.weixin… |

> **手工通道**：`IWxaKfMediaService`（1 支） 不是 `[HttpClientApi]` 接口 —— 成功响应为二进制流 / 文本等原始内容，须走原始响应直读 + Content-Type 分支判错，故没有 HTTP 特性。其方法仍是官方端点，已计入本域合计，明细见下方「手工通道端点」。

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaKfMediaService`

**为何不是 `[HttpClientApi]` 接口**：本端点成功时返回**素材二进制流**（图片 / 语音等），
失败时才返回 JSON —— 与小程序码、用户反馈图片同形，必须走
`IBaseHttpClient.SendRawAsync` 原始响应直读 + **Content-Type 分支判错**
（实现见 `WxaKfMediaService`，同 `IWxaCodeService` / `IWxaFeedbackMediaService` 口径）。

**为何本线要建这条端点（MP-X1 核对结论）**：公众号线**只有** `/cgi-bin/media/upload`，
**没有** `/cgi-bin/media/get` —— 本端点在公众号线**无重叠**，属小程序线的真实缺口
（2026-10-12 依官方清单核对）。

**令牌**：手工取当前应用令牌拼 Query（`access_token`），**不进** Query 令牌白名单
（白名单只收 `[Token]` 声明式接口）；令牌失效（`40001` / `40014` / `42001`）时
失效本应用令牌并重试一次。

**视频素材是特例（官方原文）**：`media/get` 对**视频**素材返回 JSON
`{"video_url":"DOWN_URL"}` 而非二进制流；其余素材类型返回二进制。
故调用方须按 `ContentType` 自行分支：命中 `json` 时解析 `video_url`。

### `IWxaKfService`

**官方文档**（小程序服务端 API → 客服消息 / 客服子商户 / 微信客服，2026-10-12 依据官方清单核验）：
客服消息 https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/、客服子商户 https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-management/ 族、微信客服 https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-work/ 族。

**为何长期只有 9 端点（MP-X1）**：客服账号的「增删改/头像/邀请绑定」（`/customservice/kfaccount/{add,update,del,uploadheadimg,inviteworker}`）、
客服列表（`/cgi-bin/customservice/getkflist`、`getonlinekflist`）、消息发送（`/cgi-bin/message/custom/send`）、
素材**上传**（`/cgi-bin/media/upload`）均已在公众号线建模；本域只补公众号线**未覆盖**的端点。

**2026-10-12 核对补漏（订正既有说明）**：原注释称「素材 `/cgi-bin/media/{upload,get}` 均已被公众号线占据」
与实测**不符**——公众号线只有 `media/upload`，**没有** `media/get`。故本轮补入：
- `/cgi-bin/message/custom/business/typing`（客服输入状态）：官方按账号类型二分路由，
公众号/服务号走 `message/custom/typing`、**小程序/小游戏走 `message/custom/business/typing`**，
公众号线承载的是前者，与本端点**不同路由、不构成重叠**。
- `/cgi-bin/media/get`（获取临时素材）：成功返回**素材二进制流**、失败才返回 JSON，
与小程序码同形 ⇒ 走手工通道 `IWxaKfMediaService`（不在本接口）。

**令牌与鉴权**：全部走应用级 `access_token`（Query）；客服账号格式为
`账号前缀@公众号微信号`（官方原文），子商户与微信客服族以 `mch_id` / `kf_openid` 定位。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SetAdminAsync` — 设置客服账号角色（管理员）。官方路由：`POST /customservice/kfaccount/setadmin`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/api_setkfadmin。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /customservice/kfaccount/setadmin`

- **请求体**：`WxaKfAccountRequest`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/customservice/kfaccount/setadmin` ＋ 请求体 `{"kfaccount":…}`；Query 携带 `access_token`。
- **角色语义**：设置后该客服账号具备管理员权限（可配置其他客服/角色），与 / `CancelAdminAsync` 互为逆操作。公众号线未覆盖该端点（MP-X1 下仅本线提供）。

### 2. `CancelAdminAsync` — 取消客服账号角色（管理员）。官方路由：`POST /customservice/kfaccount/canceladmin`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/api_cancelkfadmin。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /customservice/kfaccount/canceladmin`

- **请求体**：`WxaKfAccountRequest`
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/customservice/kfaccount/canceladmin` ＋ 请求体 `{"kfaccount":…}`；Query 携带 `access_token`。

### 3. `RegisterBusinessAsync` — 注册客服子商户。官方路由：`POST /cgi-bin/business/register`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-management/api_registerbusiness。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /cgi-bin/business/register`

- **请求体**：`WxaBusinessRegisterRequest` — 商户号与品牌信息，见 `WxaBusinessRegisterRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/cgi-bin/business/register` ＋ 请求体 JSON；Query 携带 `access_token`。
- **子商户用途（官方原文）**：客服消息的「子商户」形态（多品牌号共享客服能力）， / 注册后以 `mch_id` 作为唯一标识参与 `GetBusinessInfoAsync` / `ListBusinessAsync` 等操作。

### 4. `UpdateBusinessAsync` — 更新客服子商户信息。官方路由：`POST /cgi-bin/business/update`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-management/api_updatebusiness。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /cgi-bin/business/update`

- **请求体**：`WxaBusinessUpdateRequest` — 商户号与待更新品牌信息，见 `WxaBusinessUpdateRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/cgi-bin/business/update` ＋ 请求体 JSON；Query 携带 `access_token`。

### 5. `GetBusinessInfoAsync` — 获取客服子商户信息。官方路由：`POST /cgi-bin/business/get`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-management/api_getbusiness。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /cgi-bin/business/get`

- **请求体**：`WxaBusinessGetRequest` — 商户号（`mch_id`），见 `WxaBusinessGetRequest`。
- **返回**：`WxaBusinessGetResponse` — 子商户品牌信息，见 `WxaBusinessGetResponse`。

**备注（原文）**

- 官方契约：**POST** `/cgi-bin/business/get` ＋ 请求体 JSON；Query 携带 `access_token`。

### 6. `ListBusinessAsync` — 获取客服子商户列表。官方路由：`POST /cgi-bin/business/list`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-management/api_listbusiness。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /cgi-bin/business/list`
- **返回**：`WxaBusinessListResponse` — 子商户品牌列表，见 `WxaBusinessListResponse`。

**备注（原文）**

- 官方契约：**POST** `/cgi-bin/business/list`（请求体可为空对象）；Query 携带 `access_token`。

### 7. `GetWorkAccountAsync` — 获取微信客服账号信息。官方路由：`POST /customservice/work/get`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-work/api_getkfworkbound。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /customservice/work/get`

- **请求体**：`WxaKfWorkRequest` — 微信客服账号标识（`kf_openid`），见 `WxaKfWorkRequest`。
- **返回**：`WxaKfWorkGetResponse` — 微信客服账号信息，见 `WxaKfWorkGetResponse`。

**备注（原文）**

- 官方契约：**POST** `/customservice/work/get` ＋ 请求体 JSON；Query 携带 `access_token`。

### 8. `BindWorkAccountAsync` — 绑定微信客服。官方路由：`POST /customservice/work/bind`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-work/api_bindkfwork。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /customservice/work/bind`

- **请求体**：`WxaKfWorkRequest` — 微信客服账号标识，见 `WxaKfWorkRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/customservice/work/bind` ＋ 请求体 JSON；Query 携带 `access_token`。

### 9. `UnbindWorkAccountAsync` — 解绑微信客服。官方路由：`POST /customservice/work/unbind`。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-work/api_unbindkfwork。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /customservice/work/unbind`

- **请求体**：`WxaKfWorkRequest` — 微信客服账号标识，见 `WxaKfWorkRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/customservice/work/unbind` ＋ 请求体 JSON；Query 携带 `access_token`。

### 10. `SendTypingStatusAsync` — 下发客服输入状态（`POST /cgi-bin/message/custom/business/typing`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/api_typing。

- **声明接口**：[IWxaKfService](./IWxaKfService.cs)
- **官方契约**：`POST /cgi-bin/message/custom/business/typing`

- **请求体**：`WxaKfTypingRequest` — 输入状态请求体（`touser` / `command` 必填），见 `WxaKfTypingRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示下发成功）。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`。
- **小程序专用路由（官方原文）**：公众号 / 服务号用 `/cgi-bin/message/custom/typing`， / **小程序 / 小游戏用 `/cgi-bin/message/custom/business/typing`**； / 公众号线承载的是前者，与本端点**不同路由**（MP-X1 无重叠）。
- **时限约束（官方错误码）**：`45080` 下发前须与该用户 30 秒内有消息交互； / `45081` 已在输入状态不可重复下发；`45072` `command` 取值非法。

### 手工通道端点（非 `[HttpClientApi]`）

以下接口不带 HTTP 特性，路由与参数取自方法 XML 注释，调用经手工原始响应通道（非生成管线）。

#### M1. `GetMediaAsync` — 获取临时素材（`GET /cgi-bin/media/get?access_token=…&media_id=…`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/api_getmedia。

- **声明接口**：[IWxaKfMediaService](./IWxaKfMediaService.cs)
- **官方契约**：`GET /cgi-bin/media/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `mediaId` | 方法体 | `string` | 媒体文件 ID（官方 `media_id`，必填；由 `/cgi-bin/media/upload` 上传后获得）。 |
- **返回**：`WxaCodeResult` — 素材内容，见 `WxaCodeResult`：**非视频**为二进制流；**视频**为含 `video_url` 的 JSON
（调用方按 `ContentType` 分支解析）。失败时抛 `WxaException`。

**备注（原文）**

- 官方契约：**GET**，**无请求体**；Query 携带 `access_token` 与 `media_id`。
- 官方错误码要点：`40007`（invalid media_id，无效的媒体 ID）；其余见通用错误码。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/DataModels/Kf`](../../../Mud.Wechat.MiniProgram/DataModels/Kf/) 内共 **10** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **10** 个，`SerializerClassName` = `Kf`。

AOT 源生成上下文：[`KfJsonContext.g.cs`](./../../DataModels/Generated/KfJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../DataModels/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaKfAccountRequest` | 请求体 | [Kf/WxaKfAccountRequest.cs](../../DataModels/Kf/WxaKfAccountRequest.cs) | `kfaccount` |
| `WxaBusinessRegisterRequest` | 请求体 | [Kf/WxaBusinessRegisterRequest.cs](../../DataModels/Kf/WxaBusinessRegisterRequest.cs) | `mch_id`, `brand_name`, `brand_icon` |
| `WxaBusinessUpdateRequest` | 请求体 | [Kf/WxaBusinessUpdateRequest.cs](../../DataModels/Kf/WxaBusinessUpdateRequest.cs) | `mch_id`, `brand_name`, `brand_icon` |
| `WxaBusinessGetResponse` | 应答 | [Kf/WxaBusinessGetResponse.cs](../../DataModels/Kf/WxaBusinessGetResponse.cs) | `business_info` |
| `WxaBusinessGetRequest` | 请求体 | [Kf/WxaBusinessGetRequest.cs](../../DataModels/Kf/WxaBusinessGetRequest.cs) | `mch_id` |
| `WxaBusinessListResponse` | 应答 | [Kf/WxaBusinessListResponse.cs](../../DataModels/Kf/WxaBusinessListResponse.cs) | `business_list` |
| `WxaKfWorkGetResponse` | 应答 | [Kf/WxaKfWorkGetResponse.cs](../../DataModels/Kf/WxaKfWorkGetResponse.cs) | `kf_openid` |
| `WxaKfWorkRequest` | 请求体 | [Kf/WxaKfWorkRequest.cs](../../DataModels/Kf/WxaKfWorkRequest.cs) | `kf_openid` |
| `WxaKfTypingRequest` | 请求体 | [Kf/WxaKfTypingRequest.cs](../../DataModels/Kf/WxaKfTypingRequest.cs) | `touser`, `command` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Kf;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddKfApi());

var service = app.Services.GetRequiredService<IWxaKfService>();
var ct = CancellationToken.None;
var response = await service.ListBusinessAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/cgi-bin/business/list");
// 占位值仅用于编译示意；业务值须按 IWxaKfService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ListBusinessAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaKfMediaService](./IWxaKfMediaService.cs)、[IWxaKfService](./IWxaKfService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/DataModels/Kf`](../../../Mud.Wechat.MiniProgram/DataModels/Kf/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
