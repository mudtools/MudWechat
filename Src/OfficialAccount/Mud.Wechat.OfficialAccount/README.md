# Mud.Wechat.OfficialAccount

微信公众号 / 服务号 SDK **主包**：业务声明式客户端、模块注册器、AOT JsonContext 合并、素材下载双通道、JS-SDK 签名服务。对应企微线的 `Mud.Wechat.Work`。

## 内容

- **声明式业务客户端**（`Interfaces/`，命名空间平铺为 `Mud.Wechat.OfficialAccount`）：27 个域目录 / 29 个公开接口 / **188 条去重路由**（守卫 RC1 锁定）。公众号无「自建 / 套件 / 代开发」三类形态 ⇒ **不设 `IsAbstract` 公共父接口、不设应用类型子接口**，每个域接口直接作注册与注入面。

  | 目录（= 模块） | 端点 | 关键官方约束（详见各接口 XML 注释与 `MpModule` 注释） |
  |---|---|---|
  | `Basic/` | 3 | API 服务器 IP / 推送服务器 IP / 网络通信检测（`action` 仅 `dns`/`ping`/`telnet`） |
  | `Tag/` | 8 | 全部仅认证；标签上限 100、单用户标签上限 20、标签名 ≤ 30 字符 |
  | `User/` | 8 | 仅认证；批量 100 条 / 关注者单批 10000 / 黑名单单批 1000 / 拉黑单次 20 |
  | `Menu/` | 7 | 一级 3 个 / 每级 5 个二级硬上限；个性化菜单每日新增·删除 2000、测试 20000；删默认菜单级联删全部个性化菜单 |
  | `CustomerMessage/` | 3 | 下发额度 5 条/48 小时（用户发消息）或 3 条/1 分钟（菜单·关注·扫码）；聊天记录区间 ≤ 24 小时、每次 ≤ 10000 条 |
  | `KfAccount/` | 7 | 每账号最多 100 个客服账号；账号形状「前缀(≤10)@公众号微信号(≤30)」 |
  | `KfSession/` | 5 | 创建会话要求客服已绑定微信号**且在线**；未接入列表最多 100 条且无分页游标 |
  | `Template/` | 8 | 服务号专属；日调用上限 10 万次；行业每月可改 1 次；每账号可同时使用 25 个模板；结果经 `TEMPLATESENDJOBFINISH` 回执 |
  | `SubscriptionNotice/` | 7 | 服务号专属；一次性消耗用户订阅次数；模板管理前缀 `/wxaapi/newtmpl/`（无 `/cgi-bin/`） |
  | `OpenApi/` | 5 | `clear_quota` 与 `clear_quota/v2` 合计每月 10 次；`openapi/quota/clear` 每月 50 次；rid 查询有效期 7 天 |
  | `Sns/` | 4 | 服务号专属；**全部免令牌**（不消费应用级 `access_token`）；频率 5 万/分钟；`refresh_token`（30 天）生命周期归宿主 |
  | `Mass/` | 7 | `mass/send` 服务号专属；`clientmsgid` 24 小时防重；提交成功 ≠ 群发完成，结果经 `MASSSENDJOBFINISH` 推送 |
  | `Qrcode/` | 1 | 服务号专属；`showqrcode` 换图走 `mp.weixin.qq.com`、无须登录态，SDK 不建模 |
  | `AutoReply/` | 1 | 只读查询；认证/未认证的服务号/订阅号与测试号均有权限（官方原文） |
  | `Draft/` | 6 | `draft/switch` 官方已废弃不实现；`batchget` 的 `count` 1~20 |
  | `FreePublish/` | 5 | 仅认证；提交成功不等于发布完成；`batchget` 的 `count` 1~20；条目键为 `article_id` |
  | `ProductCard/` | 1 | 路径前缀 `/channels/ec/`（视频号小店域），非 `/cgi-bin/` |
  | `Comment/` | 8 | 仅认证 + 留言功能权限（88000）；以 `msg_data_id` + `index` 定位文章；评论列表 `count` ≤ 50（88010） |
  | `DataCube/` | 21 | 仅认证；全部 `POST /datacube/*`；跨度上限逐端点 1/7/15/30 天，越界由官方 61501 表达 |
  | `Media/` | 6（上传侧） | 临时素材 3 天有效；类型大小逐类核验（image 10M / voice 2M·60s / video 10M / thumb 64KB） |
  | `SmartApi/` | 12 | AI 3 + OCR 7 + 图像处理 2；OCR 七端点 100 次/天（菜单识别页无上限）；图片 < 2M；9 端点双调用形态 ⇒ 每端点双方法 |
  | `QrcodeJump/` | 4 | 服务号专属；官方 5 次/秒（44990）；发布配额每月 100 次（886000）；须先关联小程序（61007） |
  | `ShortLink/` | 2 | `long_data` ≤ 4KB、`expire_seconds` ≤ 2592000 秒（30 天），越界由官方 9410010/9410011 表达 |
  | `Store/` | 12 | **仅开放电商类目**（43104）；主体级配额上限（管理员手机/微信号/身份证/主体各 5 次） |
  | `OneCode/` | 6 | 服务号需**申请开通**（非「仅认证」）；`code_count` 须为 10000 的整数倍且 ∈ [10000, 20000000] |
  | `Invoice/` | 17 | 全部消费应用级 `access_token`、不引入 `api_ticket`；`scantitle` 独家不支持第三方代调用 |
  | `Card/` | 14（双接口） | 主体生命周期 / 投放 11 + 券码核销 3；全 POST；建卡（`card` 包装 + `card_type` 判别 11 分支）与修改（`card_id` + 分支平级、无 `advanced_info`）**不同构**；`api_ticket` 前端取卡由 `IMpTicketService` 承载；未建模 10 族共 39 端点由守卫 CD8 零路由留档 |

- **模块注册器**（`Extensions/`）：`MpModule` 枚举 28 个成员（含 `Authentication`，令牌签发随 `AddMpApp` 自动注册、不经本枚举注册路径）；`MpServiceBuilder` 27 个 `Add{域}Api()` + `AddAllApis()` + `AddModules(params MpModule[])`；每域注册委托指向源生成器产出的 `Add{域}WebApiHttpClient()`（无签入源文件）。`Build()` 期校验 `IMpAppManager` 已注册（未先 `AddMpApp` 即注册期 fail-fast）。
- **素材下载双通道**（`Media/`）：`IMpMediaDownloadService` 承载 3 条**响应为二进制流**的端点（`/cgi-bin/media/get`、`/cgi-bin/media/get/jssdk`、`/cgi-bin/material/get_material`），走 `SendRawAsync` + Content-Type 分支判错，**不进 JSON 生成管线**；命中令牌失效码时失效并重试一次。
- **JS-SDK 签名服务**（`Web/` + `Extensions/MpJsApiSignatureExtensions.cs`）：`AddMpJsApiSignature()` 注册 `IMpJsApiSignatureService`，取 `type=jsapi` 票据按官方算法产出 `wx.config` 五字段；**刻意不返回 `jsapi_ticket` 与原始签名串**。
- **动态回调来源 IP 白名单**（`Callback/`，命名空间复用 `Mud.Wechat.OfficialAccount.Callback`）：`AddMpCallbackSourceIpWhitelist()` 注册刷新服务 + 内存快照提供者，实现 Abstractions 的 `IMpCallbackSourceIpProvider`。**默认不注册即完全关闭**（该能力必然带来周期性 API 调用，故用注册式开关而非配置项）；与静态 `MpCallbackOptions.AllowedSourceIPs` 取并集。
- **AOT JsonContext 合并**（`Extensions/MpJsonResolverExtensions.cs`）：合并 OA DataModels 的 29 个域 `JsonContext` 进组件序列化管线（仅 `NET8_0_OR_GREATER`）。**该清单必须与 `Generated/` 目录逐项对齐**——组件的 AOT 分支只组合此处登记的上下文、绝不回退反射，漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。

## 用法

### 注册模块

```csharp
// Program.cs：令牌与多公众号底座先行（AddMpApp 落 Abstractions），再按需链式注册模块
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(builder => builder
    .AddBasicApi()              // 基础接口
    .AddUserApi()               // 用户信息
    .AddMenuApi()               // 自定义菜单
    .AddTemplateApi());         // 模板消息（服务号）
```

### 注入客户端、调用端点

客户端基于 `Mud.HttpUtils` 声明式生成，**令牌的获取 / 缓存 / 提前刷新 / errcode 失效恢复全自动**（官方契约强制注入走 Query）：

```csharp
using Mud.Wechat.OfficialAccount;                                  // 业务接口 + 下载通道
using Mud.Wechat.OfficialAccount.Extensions;                        // AddMpServices / MpModule
using Mud.Wechat.OfficialAccount.Abstractions;                      // AddMpApp
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;       // IMpAppContextSwitcher
using Mud.Wechat.OfficialAccount.DataModels.User;                   // 域 DTO

public sealed class FollowerService(
    IMpUserService users,                       // 用户信息域
    IMpAppContextSwitcher switcher,             // 多公众号切换器（Abstractions）
    IMpMediaDownloadService media)              // 素材下载双通道
{
    // lang 为可选 Query 参数 ⇒ 取消令牌须具名传（位置实参会绑到 lang）
    public Task<MpUserInfoResponse> GetAsync(string openId, CancellationToken ct)
        => users.GetUserInfoAsync(openId, cancellationToken: ct);

    // 多公众号：切换当前应用上下文（一次性 using，释放自动归还），令牌解析到该公众号
    public async Task<MpGetFansResponse> GetFansOfAsync(string appKey, CancellationToken ct)
    {
        using var scope = switcher.UseAppScope(appKey);
        return await users.GetFansAsync(nextOpenId: null, ct);
    }

    // 下载通道：成功是文件流、失败才是 JSON errcode —— 流由调用方释放，SDK 不落盘
    public async Task SaveVoiceAsync(string mediaId, string path, CancellationToken ct)
    {
        using var result = await media.DownloadTemporaryMediaAsync(mediaId, ct);
        if (result.VideoUrl != null) return;      // 视频素材为 JSON 形态（只有下载地址）
        if (result.Content == null) return;       // 其余形态必为文件流
        await using var file = File.Create(path);
        await result.Content.CopyToAsync(file, ct);
    }
}
```

### 错误处理

非零 `errcode`（且非令牌失效码 `{40001, 40014, 42001}`）抛 `MpException`（`ErrorCode` + 构造期脱敏的 `RequestUri`）；令牌失效码由 `MpTokenInvalidationDetector` 识别后走恢复链路自动「失效缓存 → 刷新 → 重试」：

```csharp
try
{
    await users.UpdateRemarkAsync(new MpUpdateRemarkRequest { OpenId = openId, Remark = "vip" }, ct);
}
catch (MpException ex) when (ex.ErrorCode == 48001)
{
    // 接口无权限：该能力要求已认证账号，按官方 errcode 语义处理而非本地预判
}
```

### JS-SDK 前端签名

```csharp
builder.Services.AddMpApp(configuration, "MpApps")
               .AddMpJsApiSignature();           // 须在 AddMpApp 之后（依赖票据管理器）

// 页面控制器注入 IMpJsApiSignatureService，产出 wx.config 所需的 appId/nonceStr/timestamp/signature
var sign = await jsApi.SignAsync("https://example.com/page");   // URL 中的 # 片段自动去除
```

## 依赖

- `Mud.Wechat.Abstractions`（公用层）、`Mud.Wechat.OfficialAccount.Abstractions`、`Mud.Wechat.OfficialAccount.DataModels`
- `Mud.HttpUtils` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（分析器）
- **不引用 `Mud.Wechat.OfficialAccount.Callback`**（回调包只依赖 Abstractions，本包只实现其配置侧端口）

## 说明

- 目标框架继承根 `Directory.Build.props`（`netstandard2.0;net6.0;net8.0;net10.0`、Version `1.0.3`），可打包。
- `MUD005`（Query 传令牌的 URL 泄露面）项目级抑制：微信公众平台官方契约强制 `access_token` 走 Query、不支持 Header 注入，属已知接受风险；库内遥测由 `SensitiveUrlRedactor` 脱敏。
- **免令牌端点必须独立成接口**（`[Token]` 是接口级特性）：`IMpSnsService`（sns 四端点）、`IMpOpenApiTokenFreeService`（`clear_quota/v2` 应急逃生端点）与带令牌接口同注册组，由同一条生成注册入口装载。
- 下载通道不计入接口特性路由，但计入守卫 RC2 的全量 194 条（188 接口 + 3 令牌/票据 + 3 下载路径常量）；官方面索引页唯一路由 196 条（RC4，`.docs/` 缺失时自动跳过）。
- `netstandard2.0` 下禁 `init`/`record`/`with`、无 `ArgumentNullException.ThrowIfNull`；新增代码沿用既有 TFM 条件编译形态。
- 契约守卫位于 `Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/`（24 个文件，按域前缀 BS/TG/US/MG/CM/KF/TP/SN/OA/MS/CT/DC/MD/SM/QJ/SL/ST/OT/IT/TK/CD + QT Query 令牌注入白名单 + RC 路由计数纪律）。**端点 / 路由 / DTO / 注册面变更须同批更新守卫**。
