# 微信客服 · 机器人管理（问答库）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 机器人管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfKnowledgeService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfKnowledgeService.cs)（父，**零端点**）· [IWechatWorkInternalKfKnowledgeService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfKnowledgeService_Internal.cs)（承载全部 8 支） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/)（`KfKnowledgeModels.cs` 的 `KfKnowledgeText` / `KfKnowledgeQuestion` / `KfKnowledgeSimilarQuestions` / `KfKnowledgeAnswer`；`KfKnowledgeAttachments.cs` 的 `KfKnowledgeAttachment` 与四种附件体；请求在 `RequestModel/`；命名空间 `Mud.Wechat.Work.DataModels.Kf`） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1f** 八支路由（含 `mod_group` / `mod_intent` 拼写）/ **KF2b** 零端点父接口 + 继承链恰 1 个自建子接口 / KF3 令牌绑定 / KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 分组管理 [95971](https://developer.work.weixin.qq.com/document/path/95971) · 问答管理 [95972](https://developer.work.weixin.qq.com/document/path/95972)（**仅企业自建应用有对应页面**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1f / KF2b / KF3 / KF4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§2 单应用面族、§12 路由命名与覆盖写）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

微信客服「智能助手（机器人）」所用的**问答知识库**：分组 + 问答两层，8 支端点。

| # | 方法 | 路由 | 用途 |
|---|---|---|---|
| E1 | `AddKnowledgeGroupAsync` | `POST kf/knowledge/add_group` | 建分组，返回 `group_id` |
| E2 | `DeleteKnowledgeGroupAsync` | `POST kf/knowledge/del_group` | 删分组（默认分组不可删） |
| E3 | `UpdateKnowledgeGroupAsync` | `POST kf/knowledge/**mod_group**` | 改分组名 |
| E4 | `GetKnowledgeGroupListAsync` | `POST kf/knowledge/list_group` | 分页取分组 |
| E5 | `AddKnowledgeIntentAsync` | `POST kf/knowledge/add_intent` | 建问答，返回 `intent_id` |
| E6 | `DeleteKnowledgeIntentAsync` | `POST kf/knowledge/del_intent` | 删问答 |
| E7 | `UpdateKnowledgeIntentAsync` | `POST kf/knowledge/**mod_intent**` | 改问答（**整体覆盖写**） |
| E8 | `GetKnowledgeIntentListAsync` | `POST kf/knowledge/list_intent` | 分页取问答 |

形态：官方**仅向企业自建应用开放**本族 ⇒ **零端点父接口 + 唯一自建子接口承载全部 8 支**（KF2b，落位对齐巡查上报族、身份验证二次验证族）。**没有第三方 / 代开发子接口**——注入 `IWechatWorkThirdPartyKfKnowledgeService` / `IWechatWorkProviderKfKnowledgeService` 会失败，这是官方无文档页，不是漏挂。

**路由命名是本族最大的拼写坑**：修改端点是 **`mod_group` / `mod_intent`**（不是 `update_`），而本仓**方法名**统一叫 `Update*Async`（KF1f 逐字锁定路由）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（KF3） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- 应用须配置到「**微信客服-可调用接口的应用**」中（自建子接口注释原文）。
- 量纲：分组名 `name` **≤12 个字**且**不可重复**、**分组总数 ≤100**；问题文本与每条相似问题 **≤200 个字**、相似问题 **≤100 个**；答案文本 **≤500 个字**、**答案目前仅支持 1 个**、答案附件 **≤4 个**；**单分组问答 ≤200 条**；**不同分组的问题不可重复**。
- **默认分组（`is_default = 1`）不可修改、不可删除**。
- **`mod_intent` 是整体覆盖写**：即便只改一个字段，也必须把 `question` / `similar_questions` / `answers` 传完整。
- 附件里的 `media_id` / `thumb_media_id` 由「上传临时素材」（media 域）产生；**读取问答列表时这些素材字段不返回，只回传 `name`**（图片/视频）——即**写读字段面不对称**。
- 小程序附件的 `appid` **必须是关联到企业的小程序 appid**。
- 问答内容是企业自有的对外话术，一般不含个人数据；仍建议不要把整库问答打进日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E8 | ✅ | ❌（无文档页 ⇒ 无子接口） | ❌（同） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfKnowledgeService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalKfKnowledgeService` | `Mud.Wechat.Work` | 自建子接口（**承载全部 8 支**） | **8** | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfKnowledgeService)`。

## 4. 端点

### E1. `AddKnowledgeGroupAsync` — 添加分组

```csharp
[Post("/cgi-bin/kf/knowledge/add_group")]
Task<AddKfKnowledgeGroupResponse> AddKnowledgeGroupAsync(
    [Body] AddKfKnowledgeGroupRequest request, CancellationToken cancellationToken = default);
```

- 请求：`name`（`string?`，官方必填，**≤12 个字**，不可与已有分组重复）。
- 响应：`group_id`（`string?`）。

### E2. `DeleteKnowledgeGroupAsync` — 删除分组

```csharp
[Post("/cgi-bin/kf/knowledge/del_group")]
Task<WechatWorkResponse> DeleteKnowledgeGroupAsync(
    [Body] DeleteKfKnowledgeGroupRequest request, CancellationToken cancellationToken = default);
```

- 请求：`group_id`（官方必填）。**默认分组（`is_default = 1`）不可删除**。

### E3. `UpdateKnowledgeGroupAsync` — 修改分组

```csharp
[Post("/cgi-bin/kf/knowledge/mod_group")]
Task<WechatWorkResponse> UpdateKnowledgeGroupAsync(
    [Body] UpdateKfKnowledgeGroupRequest request, CancellationToken cancellationToken = default);
```

- 路由是 **`mod_group`**（不是 `update_group`），方法名却是 `Update*` —— 本仓「方法名统一 `Update*`、路由逐字照官方」。
- 请求：`group_id`（必填）+ `name`（**官方必填**，≤12 个字）。默认分组不可修改。

### E4. `GetKnowledgeGroupListAsync` — 获取分组列表

```csharp
[Post("/cgi-bin/kf/knowledge/list_group")]
Task<GetKfKnowledgeGroupListResponse> GetKnowledgeGroupListAsync(
    [Body] GetKfKnowledgeGroupListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `cursor` | `Cursor` | `string?` | 否 | 上次返回的 `next_cursor`，首次可不填 |
| `limit` | `Limit` | `int?` | 否 | **默认 500、最大 1000** |
| `group_id` | `GroupId` | `string?` | 否 | 不填则返回**全部分组** |

响应：`group_list[]`（`KfKnowledgeGroup`：`group_id` / `name` / **`is_default`（`int?` 0 否 / 1 是）**）+ `next_cursor` + **`has_more`（`int?`，0/1，官方以数字承载布尔）**。

### E5. `AddKnowledgeIntentAsync` — 添加问答

```csharp
[Post("/cgi-bin/kf/knowledge/add_intent")]
Task<AddKfKnowledgeIntentResponse> AddKnowledgeIntentAsync(
    [Body] AddKfKnowledgeIntentRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`AddKfKnowledgeIntentRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `group_id` | `GroupId` | `string?` | **是** | 所属分组 |
| `question` | `Question` | `KfKnowledgeQuestion?` | **是** | `text.content`（必填，**≤200 字**） |
| `similar_questions` | `SimilarQuestions` | `KfKnowledgeSimilarQuestions?` | 否 | `items[]`，**最多 100 个**、每项 ≤200 字 |
| `answers` | `Answers` | `List<KfKnowledgeAnswer>?` | **是** | **目前仅支持 1 个** |

`KfKnowledgeAnswer`＝`text`（必填，`content` **≤500 字**）+ `attachments`（**≤4 个**）。附件 `KfKnowledgeAttachment` 按 `msgtype` 四选一：

| `msgtype` | 子体 | 字段（官方必填加粗） |
|---|---|---|
| `image` | `image` | **`media_id`**（上传临时素材）；`name` **仅列表回传** |
| `video` | `video` | **`media_id`**；`name` 同上 |
| `link` | `link` | **`title`**、**`url`**、`desc`、`pic_url`（链接封面 URL） |
| `miniprogram` | `miniprogram` | **`appid`**（须为关联到企业的小程序）、`title` ≤64 字节、**`thumb_media_id`**（封面建议 520×416）、**`pagepath`** |

响应：`intent_id`（`string?`）。

### E6. `DeleteKnowledgeIntentAsync` — 删除问答

```csharp
[Post("/cgi-bin/kf/knowledge/del_intent")]
Task<WechatWorkResponse> DeleteKnowledgeIntentAsync(
    [Body] DeleteKfKnowledgeIntentRequest request, CancellationToken cancellationToken = default);
```

- 请求：`intent_id`（官方必填）。

### E7. `UpdateKnowledgeIntentAsync` — 修改问答（整体覆盖写）

```csharp
[Post("/cgi-bin/kf/knowledge/mod_intent")]
Task<WechatWorkResponse> UpdateKnowledgeIntentAsync(
    [Body] UpdateKfKnowledgeIntentRequest request, CancellationToken cancellationToken = default);
```

- 路由 **`mod_intent`**；请求：`intent_id`（必填）+ `question` / `similar_questions` / `answers`——**整体覆盖写，须传完整字段内容**（不是补丁式更新）。
- 注意 `answers[].text.content` 与 `attachments` 的覆盖语义：**未传即视为清空该块**（详见 §5）。

### E8. `GetKnowledgeIntentListAsync` — 获取问答列表

```csharp
[Post("/cgi-bin/kf/knowledge/list_intent")]
Task<GetKfKnowledgeIntentListResponse> GetKnowledgeIntentListAsync(
    [Body] GetKfKnowledgeIntentListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `cursor` / `limit` | 同名 | `string?` / `int?` | 同 E4（默认 500、最大 1000） |
| `group_id` | `GroupId` | `string?` | 不填则返回**全部分组**的问答 |
| `intent_id` | `IntentId` | `string?` | 不填则返回全部问答 |

响应：`intent_list[]`（`KfKnowledgeIntent`：`group_id` / `intent_id` / `question` / `similar_questions` / `answers`）+ `next_cursor` + `has_more`。
**读取面的素材字段不回传**：图片/视频只回 `name`，`media_id` 与小程序 `thumb_media_id` 均不返回（**写读不对称**）。

**调用示例**（建分组 → 建一条带链接附件的问答 → 分页读回 → 改问答（整体覆盖））

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Kf;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "kf-a";
    o.AppType = WechatAppType.Internal;   // 本族官方仅自建开放：无第三方 / 代开发子接口
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddKfApi());

var kb = provider.GetRequiredService<IWechatWorkInternalKfKnowledgeService>();

// ① 分组：name ≤12 个字、不可重复、分组总数 ≤100
var group = await kb.AddKnowledgeGroupAsync(new AddKfKnowledgeGroupRequest { Name = "售后问答" }, cancellationToken);
WechatWorkException.ThrowIfFailed(group, "/cgi-bin/kf/knowledge/add_group");
string groupId = group.GroupId ?? throw new InvalidOperationException("响应未返回 group_id");

// ② 问答：问题 ≤200 字、答案 ≤500 字且目前仅 1 个、附件 ≤4 个、单分组 ≤200 条
var intent = await kb.AddKnowledgeIntentAsync(
    new AddKfKnowledgeIntentRequest
    {
        GroupId = groupId,
        Question = new KfKnowledgeQuestion { Text = new KfKnowledgeText { Content = "如何查询订单状态？" } },
        SimilarQuestions = new KfKnowledgeSimilarQuestions
        {
            Items = new List<KfKnowledgeText> { new KfKnowledgeText { Content = "订单进度在哪看" } },
        },
        Answers = new List<KfKnowledgeAnswer>
        {
            new KfKnowledgeAnswer
            {
                Text = new KfKnowledgeText { Content = "可在小程序「我的订单」页查看物流与售后进度。" },
                Attachments = new List<KfKnowledgeAttachment>
                {
                    new KfKnowledgeAttachment
                    {
                        MsgType = "link",
                        Link = new KfKnowledgeLinkAttachment { Title = "订单查询", Url = "https://example.com/orders" }, // 假值
                    },
                },
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(intent, "/cgi-bin/kf/knowledge/add_intent");
string intentId = intent.IntentId ?? throw new InvalidOperationException("响应未返回 intent_id");

// ③ 分页读回：limit 默认 500、最大 1000；has_more 是数字 0/1，不是布尔
string? cursor = null;
bool hasMore = true;
while (hasMore)
{
    var page = await kb.GetKnowledgeIntentListAsync(
        new GetKfKnowledgeIntentListRequest { GroupId = groupId, Cursor = cursor, Limit = 500 }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/kf/knowledge/list_intent");

    hasMore = page.HasMore != 0;
    cursor = page.NextCursor ?? cursor;
    // 读取面只回 name，不回传 media_id / thumb_media_id（写读不对称）
    logger.LogInformation("分组 {GroupId} 本页问答 {Count} 条", groupId, page.IntentList?.Count ?? 0);
}

// ④ 改问答：路由是 mod_intent，且整体覆盖写 —— 只传 intent_id + 一个字段会把其余内容清掉
var updated = await kb.UpdateKnowledgeIntentAsync(
    new UpdateKfKnowledgeIntentRequest
    {
        IntentId = intentId,
        Question = new KfKnowledgeQuestion { Text = new KfKnowledgeText { Content = "如何查询订单进度？" } },
        Answers = new List<KfKnowledgeAnswer>
        {
            new KfKnowledgeAnswer { Text = new KfKnowledgeText { Content = "在小程序「我的订单」页查看，含物流与售后进度。" } },
            // 未传 attachments ⇒ 覆盖后该问答不再带附件（覆盖写语义）
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/kf/knowledge/mod_intent");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `mod_group` / `mod_intent` vs `update_*` | 官方路由用 **`mod_` 前缀**，本仓**方法名**仍叫 `UpdateKnowledgeGroupAsync` / `UpdateKnowledgeIntentAsync`（KF1f 锁定路由字符串） | 手拼 URL 写成 `update_group` → 路由不识别 |
| 三个「上限」各管什么 | 分组总数 ≤100、**单分组问答 ≤200**、相似问题 ≤100、答案附件 ≤4、**答案数量目前 = 1** | 一次塞 3 条答案 → 被拒 |
| 「不同分组问题不可重复」 | 问题文本在**全库唯一**（跨分组也不行），与「分组名不可重复」是两个约束 | 换分组复用同一问题 |
| **`mod_intent` 整体覆盖写** | 未传的块视为清空：只改答案不传 `similar_questions` ⇒ 相似问题丢失 | 按补丁式更新写代码 |
| 写读不对称 | 写入要 `media_id` / `thumb_media_id`；**列表只回 `name`（图片/视频）不回素材 id** | 用列表结果原样回填 update → 附件丢素材 |
| `has_more` 是数字 | 官方 0/1 ⇒ `int?`（与 `sync_msg` 同口径），**翻页判据是 `has_more`，不是列表长度** | 改 `bool?` → 反序列化失败；按长度判停 → 提前结束 |
| `is_default` | 同样是 `int?` 0/1；**默认分组不可删、不可改名** | 试图清空默认分组 |
| `limit` 默认与上限 | 默认 **500**、最大 **1000**（本域其它端点是 100 / 1000 不等） | 传 5000 期望一次拉全库 |
| `group_id` / `intent_id` 的过滤语义 | E4 不填 `group_id` = 全部分组；E8 不填 `group_id` = 全部分组问答、不填 `intent_id` = 全部问答（两者是**独立过滤器**） | 以为 E8 必须带分组 |
| 应用类型 | **仅自建**：注入第三方 / 代开发子接口不存在（KF2b 锁继承链） | 按三类等宽形态写代码 |
| 小程序附件 `appid` | **必须是关联到企业的小程序 appid**（不是任意 appid） | 保存失败或回复不生效 |
| 与获客 / 智能机器人两域的关系 | 本族是**微信客服的智能回复知识库**（`kf/knowledge/*`），与「智能机器人」（Aibot / 回调长连接）和「获客助手」都不是同一套配置面 | 三域话术互相搬运 |
| 动词 | 8 支全 POST（含两支列表读取） | 改 GET → 打坏 KF1f 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验分组数 / 问答数 / 重复问题**（由官方拒绝）、**不做覆盖写保护**（漏传字段即清空）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1/E5 成功才能拿到 `group_id` / `intent_id` 供后续端点使用 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

分组名重复 / 超 100、问题重复 / 超 200、答案数超限、素材 id 非法、默认分组被改删等业务码**接口注释未逐条留档**（注释只给约束，未给码）⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 未逐条摘录 ⇒ **未逐项核验**（含「问题重复」「超上限」的具体码） | 官方 95971 / 95972 + 全局错误码页 |
| 问答生效条件与生效时机 | 注释只描述 CRUD，未说明知识库条目何时被智能助手命中（是否需管理后台开启智能回复） ⇒ 未核验 | 官方 95971 + 微信客服智能回复文档 |
| `ai_knowledge_hit_rate` 与本库的关系 | 统计族注释说「须开启智能回复原生功能并配置知识库才产生」，但**未给两者的一一映射口径** ⇒ 未核验 | [KfStatistics.md](./KfStatistics.md) + 官方 95489 |
| 附件素材的规格与有效期 | `media_id` 由临时素材接口产生（三天有效），**知识库条目对过期素材的处理未说明** ⇒ 未核验 | 官方 95972 + 上传临时素材页 |
| `pagepath` 是否须 `.html` 结尾 | 本域 `miniprogram` 附件注释未写此后缀要求（**发消息侧的 `KfMiniProgramMsgBody` 有写**），两处口径是否一致未核验 | 官方 95972 与 94677 对照 |
| 分组 / 问答的排序语义 | 未见 `order` 类字段，列表顺序规则未说明 ⇒ 未核验 | 官方 95971 / 95972 |
| 端点频率限制 | 注释未给数值 ⇒ 未逐项核验 | 官方访问频率附录 90312 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§2 八族开放面不等宽：机器人族仅自建、§12 路由命名与覆盖写）
- 同域：[KfSession.md](./KfSession.md)（`sync_msg` 里 `ai_session_reply_cnt` 所对应的智能助手接待、`origin = 4` 事件）· [KfStatistics.md](./KfStatistics.md)（`ai_knowledge_hit_rate` / `ai_transfer_rate`）· [KfAccount.md](./KfAccount.md)（问答库归属的客服账号）
- 同形态参照（零端点父接口 + 唯一自建子接口承载）：[Patrol.md](../Gov/Patrol.md)、[Resident.md](../Gov/Resident.md)、[IdentityTfa.md](../Identity/IdentityTfa.md)、[Hr.md](../Hr/Hr.md)
- 素材邻域：`media_id` / `thumb_media_id` 全部来自「上传临时素材」（media 域）
- 官方页：[分组管理（95971）](https://developer.work.weixin.qq.com/document/path/95971) · [问答管理（95972）](https://developer.work.weixin.qq.com/document/path/95972)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
