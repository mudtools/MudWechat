# Favorite 域 · 收藏管理

微信小店 / 视频号（channels 生态）「收藏管理」域：**1 支端点**，落在 `/channels/ec/favorites/*`
（店铺收藏人数统计）。接口 [IChannelsFavoriteService.cs](./IChannelsFavoriteService.cs)，注册组 `Favorite`
（`[HttpClientApi(RegistryGroupName = "Favorite", TokenManage = nameof(IChannelsAppManager))]`），
注册入口 `AddFavoriteApi()`（[../../Extensions/ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs) L194），
模块枚举 `ChannelsModule.Favorite`（[../../Extensions/ChannelsModule.cs](../../Extensions/ChannelsModule.cs)）。逐页核验 2026-10-18。

**计数只以守卫为准**：
[ChannelsFavoriteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsFavoriteContractGuards.cs)
的 CFA1（`FavoriteEndpoints_ShouldMatchOfficialRoutes`）钉「**恰 1**、`/channels/ec/favorites/` 前缀计数 1」，
CH-R2 `ExpectedRoutesByDomain["Favorite"]`
（[ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)）
逐条列出同 1 条。行 README 的 `Favorite 1`、`ChannelsModule.Favorite` 注释的「1 端点：收藏数统计」与守卫三处口径一致。

> **事实来源限定**：本文每一句断言的出处只有三处 —— 本仓接口文件的 XML 注释、本仓契约守卫、本仓 DTO。
> 本仓未摘录的官方页面细节一律写进文末「已知留档缺口」，不推断、不从邻域（HomePage / Product / Shop）借值。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetFavoritesCountAsync` | POST | `/channels/ec/favorites/count/get` | `access_token`（Query） | 官方契约 POST + **空请求体**（CFA2 禁 `[Body]`）；离线数据，存在延迟 |

- **本域令牌是本线独立令牌**，键 `ChannelsTokenTypes.AccessToken` = `"Wechat.Channels.AccessToken"`
  （[../../../Mud.Wechat.Channels/ChannelsTokenTypes.cs](../../../Mud.Wechat.Channels/ChannelsTokenTypes.cs)，
  守卫 CH-T1），接口级 `[Token(TokenType = ChannelsTokenTypes.AccessToken, InjectionMode = Query, Name = "access_token")]`
  （守卫 CH-T2 / CFA3）。与企微 / 公众号令牌在共享注册表里天然按类型隔离，守卫 CH-T3 同时锁程序集引用与源码文本。
- **本域没有免令牌端点**（对比 [../Basic/README.md](../Basic/README.md) 的第 5 支）。
- **命名空间平铺**：接口在 `Mud.Wechat.Channels`，DTO 在 `Mud.Wechat.Channels.DataModels.Favorite`；
  本线**没有** `IsAbstract` 公共父接口（守卫 CH-X2）。
- **本域无分页、无批量**：1 支端点即全量，应答是单对象 5 个数值字段。

## 1. 「官方未开放 ⇒ 零端点」是本域的口径基线

接口 remarks 的域范围原文：「本域仅店铺收藏统计 1 端点（**商品收藏如官方后续开放再按域内增量补端点**）」。
⇒ 除 `/channels/ec/favorites/count/get` 之外，本域对「商品收藏列表 / 用户收藏明细 / 收藏写入」
**全部零声明**。判据是官方文档面：只有本仓接口与 CH-R2 路由表里存在的路由才是契约，
缺页即非契约，不得按第三方 SDK 清单或邻域经验回填端点（同款先例见本页第 4 节与守卫 CHP7 / CLG7）。

## 2. 令牌注入与 MUD005 例外

唯一一支端点由接口级 `[Token(...)]` 统一注入：调用方**不写**令牌参数，生成客户端在请求期取令牌并挂到
Query 的 `access_token`。官方契约强制 Query 注入、无法改 Header，这是本仓已登记的 **MUD005 接受风险**
（接口 remarks 原文标注）。

红线：`access_token` 只作为参数名出现在文档与代码里，**其值不得进日志、遥测、异常消息或响应体**。
`WechatChannelsException` 构造期对请求地址脱敏（`SensitiveUrlRedactor`）。

## 3. 数据时效：这是离线统计数，不是实时数

接口 remarks 与守卫类 remarks 都锁同一条官方原文：**「此接口使用离线数据进行统计，结果可能存在延迟」**，
且守卫明确要求「禁止弱化为『实时统计』」。配套事实：

- 接口 remarks 同时写明「本接口无特殊注意事项、无特殊错误码（走通用错误码）」⇒ 本域**没有**专属错误码常量。
- 应答字段是**累计口径**（`favor_uv_acc_*` 的 `acc` 即 accumulate），本仓不做差分、不做缓存、不做调度；
  若宿主需要「今日新增收藏」，差值算法与基准时刻由宿主自己定义。

## 4. 应答字段与「官方无枚举值 ⇒ 不得私造常量类」

五个字段名由 CFA5 逐个锁定，全部照抄官方原文
（载体 [ChannelsFavoritesCountResponse.cs](../../../Mud.Wechat.Channels/Favorite/ChannelsFavoritesCountResponse.cs)，属性均为 `int?`）：

| JSON 字段 | 属性 | 官方语义 |
|---|---|---|
| `favor_uv_acc_shop_homepage` | `FavorUvAccShopHomepage` | 店铺首页收藏用户数 |
| `favor_uv_acc_order_detail` | `FavorUvAccOrderDetail` | 订单详情页收藏用户数 |
| `favor_uv_acc_product_detail` | `FavorUvAccProductDetail` | 商品详情页收藏用户数 |
| `favor_uv_acc_other_scene` | `FavorUvAccOtherScene` | 其他场景收藏用户数 |
| `favor_uv_acc_all` | `FavorUvAccAll` | 所有场景收藏用户数 |

CFA4 钉 DTO **恰 1**（`SerializerClassName = Favorite`）且**常量类必须为空** ——
守卫原文「Favorite 域官方无枚举值——不得私造常量类」。这也解释了守卫编号为何止于 CFA5、**没有 CFA6**
（类 remarks：「官方无枚举值 ⇒ 无 CFA6」），邻域里的 `*EnumConstants_ShouldLockOfficialLiteralValues`
在本域无对应物，不要「补齐形态」而造一个空常量类。

`favor_uv_acc_all` 与四个分渠道字段的关系（是否严格等于四者之和）本仓未做断言，见文末缺口。

## 5. 权限与账号类型：比多数小店端点更窄

接口 remarks：第三方平台代调用所属**权限集 id 为 `129`**（守卫类 remarks 特别指出：这比多数小店端点的
`129/131/192` 三集组合**更窄**，接入侧不得按其他域的权限集口径放行服务商代调用）；
官方适用范围**仅「微信小店」账号类型**可调用（小店供货商等其他账号类型官方未放行）。

## 6. 与既有六线的路由交叠红线

- 本域 1 条 `/channels/ec/favorites/*` 不与公众号 / 小程序 / 企微 / 支付 / 开放平台 / 广告任何一线交叠；
  CH-R1 的重叠白名单只有「`/cgi-bin/token` + `/cgi-bin/stable_token` + [../Basic/README.md](../Basic/README.md) 的 8 条 `/cgi-bin/*`」。
- CH-V1 禁止本线出现任何 `/wxa/` 路由。
- 守卫 CH-R2 的逐域路由表按**接口类型的命名空间**归域，给别的域「顺手」落一条 favorites 路由、
  或在本域落一条别域路由，都会同时打红 CFA1 与 CH-R2。

## 快速开始

```csharp
using Mud.Wechat.Channels;                              // 接口平铺命名空间（CH-X2）
using Mud.Wechat.Channels.Abstractions;                 // AddChannelsApp
using Mud.Wechat.Channels.Abstractions.Exceptions;      // WechatChannelsException
using Mud.Wechat.Channels.DataModels.Favorite;
using Mud.Wechat.Channels.Extensions;                   // AddWechatChannelsApi / ChannelsModule

// ① 令牌底座（配置节 ChannelsApps）；业务面只装本域。
builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddFavoriteApi());

// ② 解析接口（宿主更常见的是构造注入：MyHandler(IChannelsFavoriteService favorite)）。
var favorite = app.Services.GetRequiredService<IChannelsFavoriteService>();

// ③ 取店铺收藏人数 —— 官方空请求体，方法签名只有 CancellationToken（CFA2 禁 [Body]）。
var resp = await favorite.GetFavoritesCountAsync(ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(resp, "/channels/ec/favorites/count/get");

long total = resp.FavorUvAccAll ?? 0L;                  // 累计口径，可能延迟
long homepage = resp.FavorUvAccShopHomepage ?? 0L;
long orderDetail = resp.FavorUvAccOrderDetail ?? 0L;
long productDetail = resp.FavorUvAccProductDetail ?? 0L;
long otherScene = resp.FavorUvAccOtherScene ?? 0L;
Console.WriteLine($"收藏总人数 {total}（首页 {homepage} / 订单 {orderDetail} / 商品 {productDetail} / 其他 {otherScene}）");
```

**判错只有一层**：应答是 `ChannelsResponse` 信封（`errcode` / `errmsg`，`errcode == 0` 即 `IsSuccess`）；
生成客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 离线数据的延迟量级 | remarks 只照抄官方「可能存在延迟」，**具体延迟多久未逐项核验** | 对账或对时延敏感前逐页核验官方收藏页，勿按邻域经验设刷新节奏 |
| `favor_uv_acc_all` 与四个分渠道字段的关系 | 本仓不校验加和关系，官方是否保证严格相等**未核验** | 需要一致性时先核验，再决定是否在宿主侧做校验（本仓不加） |
| 商品收藏 / 收藏写入面 | 官方未开放 ⇒ 本域**零端点**（接口 remarks 原文「如官方后续开放再按域内增量补端点」） | 官方若开放，按「改动本域需要同批完成」全批推进，勿先建 DTO |
| 本域频率限制 | 官方频率段**未在本仓摘录** ⇒ 无本地计数依据 | 需要限流时逐页核验，勿按邻域经验值建闸 |
| 通用错误码在本域的表现 | remarks 写「无特殊错误码」，但令牌失效 / 配额耗尽一类通用码的具体取值属全线共用面 | 按码分支前先核验 [../Basic/README.md](../Basic/README.md) 的通用错误码留档 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、离线数据警示、权限集 129、账号类型限制 | [IChannelsFavoriteService.cs](./IChannelsFavoriteService.cs) 的 XML 注释 |
| 路由 / 计数（恰 1）、`/channels/ec/favorites/` 前缀计数 | [ChannelsFavoriteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsFavoriteContractGuards.cs) CFA1（`FavoriteEndpoints_ShouldMatchOfficialRoutes`） |
| POST + 空请求体（禁 `[Body]`） | 同上 CFA2（`FavoriteEndpoints_ShouldBePost_WithEmptyBody`） |
| 令牌绑定与注册组 `Favorite` + `ChannelsModule.Favorite` + `AddFavoriteApi` | 同上 CFA3 |
| DTO 计数（恰 1）、`SerializerClassName = Favorite`、**常量类必须为空** | 同上 CFA4（`FavoriteDataModels_ShouldBeRegisteredInJsonContext`） |
| 5 个 `favor_uv_acc_*` 字段名 | 同上 CFA5（`FavoriteDataModels_ShouldLockOfficialFields`） |
| 响应 DTO 与属性形态 | [../../../Mud.Wechat.Channels/Favorite/ChannelsFavoritesCountResponse.cs](../../../Mud.Wechat.Channels/Favorite/ChannelsFavoritesCountResponse.cs) |
| 全线逐域路由表与重叠白名单 | [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)（CH-R1 / CH-R2） |
| 令牌字面量 / Query 注入 / 禁复用公众号令牌 / 禁 `/wxa/` | [ChannelsTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTokenOwnerContractGuards.cs)（CH-T1~T3 / CH-V1） |
| 四包骨架 / 平铺无 `IsAbstract` / JsonContext 合并登记 | [ChannelsScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsScaffoldContractGuards.cs)（CH-X1~X5） |
| 本线整体口径与各域计数 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**改 CFA1 的 `FavoriteRoutes` 与计数（**恰 1**）、
   CH-R2 `ExpectedRoutesByDomain["Favorite"]`。新增端点必须先确认官方文档页仍在（判据是文档面，不是网关可达性）。
2. 官方若给唯一端点补参数表 → 同时改接口签名（加 `[Body]`）与 CFA2 的空请求体断言，本文速览同批改。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Favorite")]` → 跑仓库根
   `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` → 同批改 CFA4 计数（1）。
   **漏登记会在 AOT 首次序列化才失败**（CH-X5 的合并列表就是防这一手）；`FavoriteJsonContext` 多为 `internal`，
   不得在宿主示例里引用。
4. **不要**为本域新增静态常量类（CFA4 断言常量集合为空；官方无枚举值 ⇒ 也不会有 CFA6）。
5. 改注册面 → `ChannelsModule.Favorite`、`ChannelsServiceBuilder.AddFavoriteApi()`（L194）与本域同批；
   只加接口不加注册入口会在 `AddChannelsApp` 之后 fail-fast。
6. 收藏相关**回调**不在本域：本线回调面统一落 [../../../Mud.Wechat.Channels.Callback/README.md](../../../Mud.Wechat.Channels.Callback/README.md)
   （官方强制 JSON 报文 + 仅安全模式），事件键集由该包的守卫闭环持有，本域不声明任何回调。
7. **不要**给本域加免令牌端点或第二套令牌（CH-T1）；**不要**新增 `/wxa/` 路由（CH-V1）；
   **不要**新增 `IsAbstract` 父接口（CH-X2）；**不要**跨线声明别的线的端点（本线不得声明 `/cgi-bin/*` 之外的同路由）；
   **不要**在本文件或示例里写出令牌真值。
8. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
