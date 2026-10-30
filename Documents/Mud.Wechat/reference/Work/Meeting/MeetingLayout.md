# 会议布局与背景 · 基础布局、高级布局与背景图

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 会议布局和背景管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingLayoutService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingLayoutService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingLayoutService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingLayoutService_Internal.cs)（**唯一**子接口，承载全部 15 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`LayoutTemplate`、`LayoutBasicRequest/Page/Seat`、`LayoutAdvancedRequest/Page/Seat/GridUser/PollingSetting`、`LayoutBasicInfo`、`LayoutAdvancedInfo`、`LayoutApplyUser`、`LayoutBackground`、`LayoutBackgroundImage` 与各请求 / 响应；本族在 MT4 的逐族分解中计 31 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(15)`、**`layout/list_template` 是本域唯一 GET**（专项断言 `typeof(GetAttribute)`，理由「勿顺手统一为 POST」）且无请求体、`layout/` 与 `advanced_layout/` 两个前缀并存 / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 15)` / MT3 令牌绑定 / MT4 上下文登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 模板列表 [98844](https://developer.work.weixin.qq.com/document/path/98844) · 添加基础布局 98845 · 添加高级布局 98861 · 修改基础布局 98846 · 修改高级布局 98868 · 设默认布局 98847 · 应用高级布局 98869 · 布局列表 98862 · 用户布局 98865 · 批量删除布局 98866 · 添加背景 98851 · 设默认背景 98852 · 背景列表 98856 · 删除背景 98853 · 批量删除背景 98854（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（模板 → 基础布局增改 → 设默认 → 高级布局增/应用/列表/删除 → 背景五支））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 5/8/9 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

围绕单场会议的**画面布局与虚拟背景**，15 支端点分三组、跨两个路由前缀：

| 组 | # | 方法 | 路由 |
|---|---|---|---|
| 模板 | E1 | `ListLayoutTemplatesAsync` | **GET** `layout/list_template`（**本域唯一 GET，无请求体**） |
| 基础布局 | E2 | `AddMeetingLayoutAsync` | `layout/add` |
| | E4 | `UpdateMeetingLayoutAsync` | `layout/update` |
| | E6 | `SetDefaultMeetingLayoutAsync` | `layout/set_default` |
| 高级布局 | E3 | `AddAdvancedLayoutAsync` | **`advanced_layout/add`** |
| | E5 | `UpdateAdvancedLayoutAsync` | `advanced_layout/update` |
| | E7 | `ApplyAdvancedLayoutAsync` | `advanced_layout/apply` |
| | E8 | `ListMeetingLayoutsAsync` | `advanced_layout/list` |
| | E9 | `GetUserLayoutAsync` | `advanced_layout/get_user_layout` |
| | E10 | `BatchDeleteLayoutsAsync` | `advanced_layout/batch_delete` |
| 背景 | E11 | `AddMeetingBackgroundAsync` | `layout/add_background` |
| | E12 | `SetDefaultMeetingBackgroundAsync` | `layout/set_default_background` |
| | E13 | `ListMeetingBackgroundsAsync` | `layout/list_background` |
| | E14 | `DeleteMeetingBackgroundAsync` | `layout/delete_background` |
| | E15 | `BatchDeleteMeetingBackgroundsAsync` | `layout/batch_delete_background` |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）。

三档布局的优先级（E9 注释原文）：**用户个性布局 > 会议自定义布局（高级布局、基础布局）> 会议默认布局**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（TO1~TO3） | 运行期归属校验 |

### 2.2 权限与业务前置

- 自建应用须配置在「**可调用接口的应用**」列表中；本族注释只写权限口径、**未写「仅本应用创建的会议」**（与 Rooms / 会控族不同）。
- **容量闸**：一场会议**最多 10 个基础布局**（E2）；单会议**最多 20 个高级布局**（E3）；批量删除**最多 20 个 ID**（E10）；应用对象 `user_list` **单次 ≤20 个用户**（E7）；一场会议**最多 7 个背景**（E11）。
- **终端限制**：**高级布局目前仅支持 H.323/SIP 会议室终端**（E3/E7/E8/E9 反复出现），`LayoutApplyUser.TmpOpenid` 注释亦标「仅对 H.323/SIP 会议室终端生效」。
- **座次身份会前/会中不同**（E2/E4 注释）：**会前只允许设置邀请者成员，会中只允许设置参会成员** ⇒ 座次里的 `userid` / `tmp_openid` 有效性取决于调用时机。
- **宫格与模板**：`grid_id`（宫格 ID）来自 E1 模板的 `picture_url` 布局图，「该 ID 用于确定布局宫格和成员座次的映射关系，设置和修改时需要当做入参传递」⇒ **宫格 ID 不能凭空构造**；同一 `grid_id` 重复传入时**仅第一次出现的对象生效**。
- 宫格类型 `grid_type`：**1 视频画面 / 2 共享画面 / 3 拓展应用**（基础布局；**目前一页仅可添加一个应用**，且应用须与会议绑定、开启网页服务、同企业可见）；高级布局仅 **1 视频 / 2 共享**。
- 视频画面来源 `video_type`（高级布局）：**1 演讲者 / 2 自动填充 / 3 指定人员**（`=3` 时需传 `tmp_openid`，且 `nick_name` **必填且为 base64 编码**）。
- **背景图片要求**（E11）：**PNG 格式、≤10MB、分辨率最小 1920×1080**，且背景图片上传方式为**异步上传**——可订阅「**素材上传结果**」事件获得结果通知；`image_md5` 是图片内容 MD5 的**十六进制表示**。
- `default_image_order` / `default_layout_order` **从 1 开始计数**（序号，不是 ID）。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E15 | ✅ | ❌ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册会议模块（十一族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMeetingApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Meeting` |
| 注册入口 | `WechatWorkServiceBuilder.AddMeetingApi()`（→ 源生成 `AddMeetingWebApiHttpClient()`） |
| 注册组名 | `"Meeting"`（十一族共用；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalMeetingLayoutService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingLayoutService`） |
| 不可注入 | `IWechatWorkMeetingLayoutService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `ListLayoutTemplatesAsync` — 获取布局模板（本域唯一 GET、无请求体）

**签名**

```csharp
[Get("/cgi-bin/meeting/layout/list_template")]
Task<ListLayoutTemplatesResponse> ListLayoutTemplatesAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | **`GET`** `/cgi-bin/meeting/layout/list_template` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/layout/list_template?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | **无**（MT1 断言方法参数只余 `CancellationToken`） |

**返回** `ListLayoutTemplatesResponse.LayoutTemplateList`（`List<LayoutTemplate>?`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `layout_template_id` | `LayoutTemplateId` | `string?` | 模板 ID（写入布局时的必填键） |
| `thumbnail_url` | `ThumbnailUrl` | `string?` | 缩略图 URL |
| `picture_url` | `PictureUrl` | `string?` | 布局图 URL——**图中含宫格 ID，是 `grid_id` 的来源** |
| `render_rule` | `RenderRule` | `string?` | 成员个人渲染布局的规则说明（**JSON 字符串**） |

**调用示例**（E1 取模板 → E2 加基础布局 → E6 设默认 → E3/E7 高级布局 → E9/E8 读回 → E11~E13 背景 → E10/E14/E15 清理）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMeetingApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Meeting;      // 请求 / 响应 DTO

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册会议模块（十一族同组；本族仅自建开放）
builder.Services.AddWechatWorkServices(b => b.AddMeetingApi());

// ③ 注入唯一的自建子接口
var layout = provider.GetRequiredService<IWechatWorkInternalMeetingLayoutService>();

// ④ E1 取模板：GET 且无请求体（域内唯一 GET 端点）
var templates = await layout.ListLayoutTemplatesAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(templates, "/cgi-bin/meeting/layout/list_template");
string? templateId = templates.LayoutTemplateList?.FirstOrDefault()?.LayoutTemplateId;

// ⑤ E2 添加基础布局：一场会议最多 10 个；grid_id 取模板布局图中的宫格 ID
const string meetingid = "demo-meeting-id";   // 须是「API 成功预定」的会议
if (templateId is not null)
{
    var added = await layout.AddMeetingLayoutAsync(new AddMeetingLayoutRequest
    {
        Meetingid = meetingid,
        LayoutList = new List<LayoutBasicRequest>
        {
            new LayoutBasicRequest
            {
                PageList = new List<LayoutBasicPage>
                {
                    new LayoutBasicPage
                    {
                        LayoutTemplateId = templateId,
                        UserSeatList = new List<LayoutBasicSeat>
                        {
                            // userid / tmp_openid / tool_sdkid 三者填其一；grid_type=1 时 nick_name 必填
                            new LayoutBasicSeat { GridId = "1", GridType = 1, Userid = "demo_user_0001", NickName = "示例成员" },
                            new LayoutBasicSeat { GridId = "2", GridType = 2 },
                        },
                    },
                },
            },
        },
        DefaultLayoutOrder = 1,               // 从 1 开始计数；首次不送则默认选中第一个布局
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(added, "/cgi-bin/meeting/layout/add");
    logger.LogInformation("基础布局已添加，当前应用布局是否已选定：{Selected}", added.SelectedLayoutId is not null);

    // ⑥ E4 修改基础布局：layout_id + page_list 必填，可顺带设为会议应用布局
    var updated = await layout.UpdateMeetingLayoutAsync(new UpdateMeetingLayoutRequest
    {
        Meetingid = meetingid,
        LayoutId = added.LayoutList?.FirstOrDefault()?.LayoutId ?? string.Empty,
        EnableSetDefault = true,
        PageList = added.LayoutList?.FirstOrDefault()?.PageList,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/meeting/layout/update");
}

// ⑦ E6 设置默认布局：selected_layout_id 送空字符串 "" 表示恢复会议自带的默认原始布局
var setDefault = await layout.SetDefaultMeetingLayoutAsync(new SetDefaultMeetingLayoutRequest
{
    Meetingid = meetingid,
    SelectedLayoutId = string.Empty,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(setDefault, "/cgi-bin/meeting/layout/set_default");

// ⑧ E3 添加高级布局：单会议最多 20 个；仅 H.323/SIP 会议室终端可用
var advAdded = await layout.AddAdvancedLayoutAsync(new AddAdvancedLayoutRequest
{
    Meetingid = meetingid,
    LayoutList = new List<LayoutAdvancedRequest>
    {
        new LayoutAdvancedRequest
        {
            LayoutName = "峰会主画面",
            PageList = new List<LayoutAdvancedPage>
            {
                new LayoutAdvancedPage
                {
                    LayoutTemplateId = templateId ?? string.Empty,
                    EnablePolling = true,
                    PollingSetting = new LayoutAdvancedPollingSetting
                    {
                        PollingIntervalUnit = 2,     // 1 秒 / 2 分钟（官方必填）
                        PollingInterval = 30,        // 1~999999
                        IgnoreUserNovideo = true,    // 官方必填
                        IgnoreUserAbsence = true,    // 官方必填
                    },
                    UserSeatList = new List<LayoutAdvancedSeat>
                    {
                        // video_type：1 演讲者 / 2 自动填充 / 3 指定人员（此时 nick_name 必填且 base64）
                        new LayoutAdvancedSeat { GridId = "1", GridType = 1, VideoType = 1 },
                    },
                },
            },
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(advAdded, "/cgi-bin/meeting/advanced_layout/add");
string? advLayoutId = advAdded.LayoutList?.FirstOrDefault()?.LayoutId;

// ⑨ E7 应用高级布局：user_list 为空 = 给整场会议设置；带用户 = 只给这些用户设个性布局（≤20，仅 H.323/SIP 终端）
if (advLayoutId is not null)
{
    var applied = await layout.ApplyAdvancedLayoutAsync(new ApplyAdvancedLayoutRequest
    {
        Meetingid = meetingid,
        LayoutId = advLayoutId,
        // UserList：如需恢复，把 layout_id 传空字符串 ""（恢复成当前会议的默认布局）
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(applied, "/cgi-bin/meeting/advanced_layout/apply");

    // ⑩ E8 布局列表（基础 + 高级都在这一支）
    var all = await layout.ListMeetingLayoutsAsync(new ListMeetingLayoutsRequest { Meetingid = meetingid }, cancellationToken);
    WechatWorkException.ThrowIfFailed(all, "/cgi-bin/meeting/advanced_layout/list");
    logger.LogInformation("会议内布局共 {Count} 个", all.LayoutList?.Count ?? 0);

    // ⑪ E10 批量删除布局：最多 20 个 ID；不校验存在性；正在被应用的布局无法删除
    var deletedLayouts = await layout.BatchDeleteLayoutsAsync(new BatchDeleteLayoutsRequest
    {
        Meetingid = meetingid,
        LayoutIdList = new List<string> { advLayoutId },
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(deletedLayouts, "/cgi-bin/meeting/advanced_layout/batch_delete");
}

// ⑫ E11 添加背景：PNG、≤10MB、分辨率最小 1920x1080；上传是异步的（结果走「素材上传结果」事件）
var bgAdded = await layout.AddMeetingBackgroundAsync(new AddMeetingBackgroundRequest
{
    Meetingid = meetingid,
    ImageList = new List<LayoutBackgroundImage>
    {
        new LayoutBackgroundImage
        {
            ImageMd5 = config["Demo:ImageMd5"],      // 图片内容 MD5 的十六进制表示
            ImageUrl = config["Demo:ImageUrl"],
        },
    },
    DefaultImageOrder = 1,                            // 从 1 开始；不填默认 1
}, cancellationToken);
WechatWorkException.ThrowIfFailed(bgAdded, "/cgi-bin/meeting/layout/add_background");

// ⑬ E13 背景列表 → E12 切换默认背景（"" 恢复黑色默认背景）→ E9 查用户当前布局
var backgrounds = await layout.ListMeetingBackgroundsAsync(new ListMeetingBackgroundsRequest { Meetingid = meetingid }, cancellationToken);
WechatWorkException.ThrowIfFailed(backgrounds, "/cgi-bin/meeting/layout/list_background");

var setBg = await layout.SetDefaultMeetingBackgroundAsync(new SetDefaultMeetingBackgroundRequest
{
    Meetingid = meetingid,
    SelectedBackgroundId = backgrounds.SelectedBackgroundId ?? string.Empty,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(setBg, "/cgi-bin/meeting/layout/set_default_background");

// E9 需要 tmp_openid + instance_id（被操作用户终端类型；H.323/SIP 设备为 9）
var userLayout = await layout.GetUserLayoutAsync(new GetUserLayoutRequest
{
    Meetingid = meetingid,
    TmpOpenid = "demo-tmp-openid",
    InstanceId = 9,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(userLayout, "/cgi-bin/meeting/advanced_layout/get_user_layout");
// layout_type：0 默认布局 / 2 自定义会议布局 / 3 个性布局
logger.LogInformation("该用户当前布局类型 {LayoutType}", userLayout.LayoutType);

// ⑭ E14 / E15 删除与批量删除背景
if (backgrounds.BackgroundList?.Count > 0)
{
    var bgDeleted = await layout.DeleteMeetingBackgroundAsync(new DeleteMeetingBackgroundRequest
    {
        Meetingid = meetingid,
        BackgroundId = backgrounds.BackgroundList[0].BackgroundId ?? string.Empty,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(bgDeleted, "/cgi-bin/meeting/layout/delete_background");
}

var bgBatchDeleted = await layout.BatchDeleteMeetingBackgroundsAsync(new BatchDeleteMeetingBackgroundsRequest
{
    Meetingid = meetingid,
    BackgroundIdList = (backgrounds.BackgroundList ?? new List<LayoutBackground>())
        .Select(b => b.BackgroundId ?? string.Empty).ToList(),
}, cancellationToken);
WechatWorkException.ThrowIfFailed(bgBatchDeleted, "/cgi-bin/meeting/layout/batch_delete_background");
```

### E2 / E4. 基础布局的添加与修改

```csharp
[Post("/cgi-bin/meeting/layout/add")]
Task<AddMeetingLayoutResponse> AddMeetingLayoutAsync(
    [Body] AddMeetingLayoutRequest request, CancellationToken cancellationToken = default);

[Post("/cgi-bin/meeting/layout/update")]
Task<WechatWorkResponse> UpdateMeetingLayoutAsync(
    [Body] UpdateMeetingLayoutRequest request, CancellationToken cancellationToken = default);
```

| 端点 | 请求 | 返回 |
|---|---|---|
| E2 `layout/add` | `meetingid` + `layout_list[]`（每项 `page_list[]`，页含 `layout_template_id` + `user_seat_list[]`）+ `default_layout_order`（从 1 起；首次不送默认选中第一个） | `selected_layout_id` + `layout_list[]`（`LayoutBasicInfo`：`layout_id` + `page_list[]`） |
| E4 `layout/update` | `meetingid` + `layout_id` + `page_list[]`（均必填）+ `enable_set_default`（默认不设置） | `WechatWorkResponse` |

`LayoutBasicSeat`：`grid_id`（必填，**重复 grid_id 仅首次生效**）+ `grid_type`（**1 视频 / 2 共享 / 3 拓展应用**）+ `userid` / `tmp_openid` / `tool_sdkid`（**三者填其一**）+ `nick_name`（`grid_type=1` 时必填）。

### E3 / E5. 高级布局的添加与修改

```csharp
[Post("/cgi-bin/meeting/advanced_layout/add")]
Task<AddAdvancedLayoutResponse> AddAdvancedLayoutAsync(
    [Body] AddAdvancedLayoutRequest request, CancellationToken cancellationToken = default);

[Post("/cgi-bin/meeting/advanced_layout/update")]
Task<WechatWorkResponse> UpdateAdvancedLayoutAsync(
    [Body] UpdateAdvancedLayoutRequest request, CancellationToken cancellationToken = default);
```

`LayoutAdvancedRequest`：`layout_name` + `page_list[]`（必填）；`LayoutAdvancedPage`：`layout_template_id`（必填）+ `enable_polling`（默认关闭）+ `polling_setting` + `user_seat_list[]`；`LayoutAdvancedPollingSetting` 四字段**均官方必填**（`polling_interval_unit` 1 秒 / 2 分钟；`polling_interval` **1~999999**；`ignore_user_novideo`；`ignore_user_absence`）；`LayoutAdvancedSeat`：`grid_id` + `grid_type`（**1 视频 / 2 共享**）+ `video_type`（**1 演讲者 / 2 自动填充 / 3 指定人员**）+ `user_list[]`（`LayoutAdvancedGridUser`：`userid` 与 `tmp_openid` **必填其一**；`nick_name` 在 `video_type=3` 时必填且 **base64**；**轮询关闭时一个用户、开启后可多个**）+ 扁平字段 `userid`（JSON 仍是 `userid`，用于读取基础布局座次的扁平形态）。

E5 `advanced_layout/update`：`meetingid` + `layout_id` + `page_list`（均必填）+ `layout_name`；注释强调「**修改的是布局定义**」——若该布局正被会议使用则**自动应用新布局**，若正被用户使用则有相应联动（见 §5）。

### E6 / E7. 默认布局与应用

```csharp
[Post("/cgi-bin/meeting/layout/set_default")]
Task<WechatWorkResponse> SetDefaultMeetingLayoutAsync(
    [Body] SetDefaultMeetingLayoutRequest request, CancellationToken cancellationToken = default);

[Post("/cgi-bin/meeting/advanced_layout/apply")]
Task<WechatWorkResponse> ApplyAdvancedLayoutAsync(
    [Body] ApplyAdvancedLayoutRequest request, CancellationToken cancellationToken = default);
```

- **E6**：`meetingid` + `selected_layout_id`（均必填）；**`selected_layout_id` 传空 `""` = 恢复会议自带的默认原始布局**。
- **E7**：`meetingid` + `layout_id`（必填，**传空 `""` = 恢复当前会议的默认布局**）+ `user_list[]`（`LayoutApplyUser.TmpOpenid`，**官方必填、单场会议唯一、仅对 H.323/SIP 会议室终端生效**）；**`user_list` 为空 ⇒ 给整场会议设高级自定义布局；带用户 ⇒ 只给这些用户设个性布局；单次 ≤20 个用户**。

### E8 / E9 / E10. 布局列表 / 用户布局 / 批量删除

| 端点 | 请求 | 返回要点 |
|---|---|---|
| E8 `advanced_layout/list` | `meetingid` | `selected_layout_id` + `layout_list[]`（`LayoutAdvancedInfo`：**基础与高级都在这一支**；基础座次的扁平形态由 `LayoutAdvancedSeat` 承载） |
| E9 `advanced_layout/get_user_layout` | `meetingid` + `tmp_openid` + `instance_id`（**三者均官方必填**；`instance_id` 取值 0 PSTN / 1 PC / 2 Mac / 3 Android / 4 iOS / 5 Web / 6 iPad / 7 Android Pad / 8 小程序 / **9 voip·sip** / 10 linux / 20·21 Rooms for Touch …） | `selected_layout_id` / `layout_name` / **`layout_type`（0 默认 / 2 自定义会议布局 / 3 个性布局）** / `page_list[]` |
| E10 `advanced_layout/batch_delete` | `meetingid` + `layout_id_list`（**最多 20 个 ID**） | `WechatWorkResponse`；**不校验 ID 是否存在**，但**正在被应用的布局无法删除**（须先切换布局或恢复默认） |

### E11~E15. 背景五支

| # | 方法 | 路由 | 请求 / 返回要点 |
|---|---|---|---|
| E11 | `AddMeetingBackgroundAsync` | `layout/add_background` | `meetingid` + `image_list[]`（每项 `image_md5` + `image_url` 均必填）+ `default_image_order`（从 1 起，默认 1）→ 回 `selected_background_id` + `background_list[]` |
| E12 | `SetDefaultMeetingBackgroundAsync` | `layout/set_default_background` | `meetingid` + `selected_background_id`；**送空 `""` = 恢复会议默认的黑色背景** |
| E13 | `ListMeetingBackgroundsAsync` | `layout/list_background` | `meetingid` → `selected_background_id` + `background_list[]`（`LayoutBackground`：`background_id` + `image_md5`） |
| E14 | `DeleteMeetingBackgroundAsync` | `layout/delete_background` | `meetingid` + `background_id`（均必填）→ `WechatWorkResponse` |
| E15 | `BatchDeleteMeetingBackgroundsAsync` | `layout/batch_delete_background` | `meetingid` + `background_id_list`（均必填）→ `WechatWorkResponse` |

**背景硬性要求**：一场会议**最多 7 个背景**；图片 **PNG 格式、≤10MB、分辨率最小 1920×1080**；上传方式为**异步上传**，结果通知走「素材上传结果」事件（回调面，不在本域）。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **本域唯一 GET 在 E1** | `layout/list_template` 是 GET 且无请求体（MT1 专项断言，理由「勿顺手统一为 POST」）；其余 14 支全 POST（含 4 支 list/get 语义） | 改 POST / 给 GET 塞请求体 → 打坏 MT1 与 G5 |
| **`layout/` 与 `advanced_layout/` 是两个前缀** | 基础布局走 `layout/*`，高级布局走 `advanced_layout/*`，背景又回到 `layout/*_background` | 按前缀推断族归属、路由写错 |
| **`grid_id` 来自模板布局图** | E1 的 `picture_url` 内含宫格 ID，是座次映射的唯一来源；**同一 `grid_id` 重复传入仅第一次生效** | 自行编造宫格 ID → 布局错乱 |
| **三档布局的优先级** | 用户个性布局 > 会议自定义布局（高级/基础）> 会议默认布局（E9 注释） | 以为设了会议布局就覆盖个性布局 |
| **`""` 是恢复默认的特殊值** | E6 `selected_layout_id` 传 `""` 恢复原始布局；E7 `layout_id` 传 `""` 恢复会议默认；E12 `selected_background_id` 传 `""` 恢复黑色背景 | 把空串当非法值挡在本地 |
| `user_list` 空与非空语义相反 | E7：**空 = 给整场会议设高级布局；非空 = 只给指定用户设个性布局**（≤20 人） | 传空以为是「不给任何人设」 |
| **高级布局仅 H.323/SIP 终端** | E3/E7/E8/E9 四处标注；`LayoutApplyUser.tmp_openid` 亦注明仅对该类终端生效 | 给普通客户端用户应用个性布局后不生效 |
| `grid_type` 三档 vs 两档 | 基础布局 **1/2/3**（3 拓展应用，一页仅一个应用，且应用须绑定会议 + 开网页服务 + 可见性合规）；高级布局只有 **1/2** | 在高级布局里设拓展应用 |
| `video_type=3` 时的 base64 昵称 | 高级布局指定人员时 `nick_name` **必填且 base64 编码**（读取侧文档页也标 base64） | 传明文昵称或漏传 |
| **轮询四字段全必填** | `polling_interval_unit`（1 秒 / 2 分钟）、`polling_interval`（1~999999）、`ignore_user_novideo`、`ignore_user_absence` | 只设间隔被拒 |
| 轮询开合改变用户数 | 轮询关闭时一个用户、开启后一个宫格可含多个用户 | 开轮询仍只塞一人 |
| **修改布局即改定义** | E5 注释：被会议使用中的布局修改后**自动应用到会议**；被用户使用中另有联动 | 以为修改不影响进行中的会议 |
| **正在应用的布局不可删** | E10：须先切换或恢复默认；同时**批量删除不校验 ID 存在性** | 删当前布局失败，或误信「删了 N 个」 |
| 序号不是 ID | `default_layout_order` / `default_image_order` **从 1 开始计数**的列表序号 | 传列表里的 layout_id / background_id |
| **背景是异步上传** | `image_md5`（内容 MD5 的十六进制）+ `image_url` 只是提交，结果须靠「素材上传结果」事件 | 提交即断言背景可用 |
| 会前会中可设成员不同 | E2/E4：会前只允许设置邀请者成员，会中只允许设置参会成员 | 会前设参会者、会中设未邀请者 |
| 容量数值各不同 | 基础布局 ≤**10**、高级布局 ≤**20**、批量删除 ≤**20 个 ID**、应用对象 ≤**20 个用户**、背景 ≤**7** | 互相套用 |
| `nick_name` 三处含义不同 | `LayoutBasicSeat.nick_name`（grid_type=1 必填）/ `LayoutAdvancedGridUser.nick_name`（video_type=3 必填且 base64）/ `MeetingEnrollInfo.nick_name`（报名昵称）⇒ 编码要求不同 | 混用同一构造 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验宫格 ID 合法性与数量上限**、**不校验会前/会中身份**、**不做 base64 编解码**、**不做「先切默认再删除」的编排**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 成功回 `selected_layout_id`；E3/E11 成功回列表但不代表设备侧已生效 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

布局数量超限、宫格 ID 非法、拓展应用条件不满足、正在应用的布局被删、终端类型不支持高级布局、背景格式/尺寸/数量越界等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 98844~98869 / 98851~98856 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| 模板清单的可变性 | E1 未记模板数量、版本差异与新增节奏；`render_rule` 的 JSON 结构无字段级说明 | 官方 98844 |
| 「素材上传结果」事件的载荷 | E11 与 Webinar 封面都依赖该事件，但事件键与载荷不在本域 ⇒ 未在本域核验 | `Mud.Wechat.Work.Callback` 事件族 |
| 背景图与 `image_md5` 的校验强度 | 注释只给 MD5 十六进制要求，未记校验失败的行为与重试语义 | 官方 98851 |
| 拓展应用（`grid_type=3`）的应用来源 | 注释给了三项前置条件（绑定会议 / 开网页服务 / 可见性），但 `tool_sdkid` 的获取端点不在本域 | 官方拓展应用文档 |
| 修改高级布局时对「正在使用」的具体行为 | 注释有截断的联动说明，未记完整规则与用户侧生效时机 | 官方 98868 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 8 节「本域只有 1 个 GET」清单；第 9 节 #78/#79/#83/#86/#87 五档容量）
- 同域近邻：[MeetingMra.md](./MeetingMra.md)（**MRA 已显示自定义/个性布局或焦点视频时不支持设默认布局**——正是本族产出的两种形态）· [MeetingRooms.md](./MeetingRooms.md)（高级布局的目标终端 H.323/SIP 与 `tmp_openid` 来源）· [MeetingControl.md](./MeetingControl.md)（会中成员与 `tmp_openid` 的读取入口）
- 跨域对照：背景与封面的异步上传依赖 [Media.md](../Media/Media.md) 的素材体系；`instance_id` 设备类型枚举与会控族共用同一口径
- 官方页：[获取布局模板（98844）](https://developer.work.weixin.qq.com/document/path/98844) · [添加基础布局（98845）](https://developer.work.weixin.qq.com/document/path/98845) · [添加高级布局（98861）](https://developer.work.weixin.qq.com/document/path/98861) · [应用高级布局（98869）](https://developer.work.weixin.qq.com/document/path/98869) · [用户布局（98865）](https://developer.work.weixin.qq.com/document/path/98865) · [添加会议背景（98851）](https://developer.work.weixin.qq.com/document/path/98851)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
