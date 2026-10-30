# 会议室连接器（MRA）· 设备状态查询与远端操作

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Meeting` 域 · 会议室连接器（MRA）管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingMraService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingMraService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingMraService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/IWechatWorkMeetingMraService_Internal.cs)（**唯一**子接口，承载全部 4 支） |
| DTO 源文件 | [DataModels/Meeting/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Meeting/)（全域 261 型平铺、`SerializerClassName = Meeting` → `MeetingJsonContext`；本族相关：`QueryMraStatusRequest/Response`、`SetMraDefaultLayoutRequest`、`SetMraRaiseHandRequest`、`HangupMraRequest`、`MraDeviceRef`；本族在 MT4 的逐族分解中计 6 型） |
| 契约守卫 | [WechatMeetingContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingContractGuards.cs)（MT1 `MeetingEndpoints_ShouldMatchOfficialRoutes`：本族路由表 `HaveCount(4)`、全部挂 `mra/` 段且官方即 POST / MT2 `MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`：`AssertFamily(父 0, 自建 4)` / MT3 令牌绑定 / MT4 上下文登记 / MT5「MRA 族的设备引用是 `MraDeviceRef.TmpOpenid`（JSON `tmp_openid`）」）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取 MRA 状态 [98786](https://developer.work.weixin.qq.com/document/path/98786) · 设置默认布局 98787 · 举手/放下 98788 · 挂断 MRA 呼叫 98789（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（查状态 → 设默认分屏 → 举手 → 挂断））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MT1 / MT2 / MT3 / MT4 / MT5 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 8/10/11 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

对**会议室连接器（MRA，即 voip/sip 设备）**做「查状态 + 三个远端动作」，4 支端点全部挂 `/cgi-bin/meeting/mra/`：

| # | 方法 | 路由 | 用途 |
|---|---|---|---|
| E1 | `QueryMraStatusAsync` | `mra/query_status` | 取指定 MRA 设备的当前状态（角色、音/视频、共享、举手、默认布局、IP） |
| E2 | `SetMraDefaultLayoutAsync` | `mra/set_default_layout` | 设置该设备的默认分屏方式 |
| E3 | `SetMraRaiseHandAsync` | `mra/set_raise_hand` | 替该设备举手 / 放下手 |
| E4 | `HangupMraAsync` | `mra/hangup` | 挂断该 MRA 呼叫 |

形态：**零端点父接口 + 唯一自建子接口承载**（MT2）；三支写端点均返回 `WechatWorkResponse`（官方无业务负载）。

**作用对象是「API 创建的会议」**：E1/E2/E3 的注释都限定「对 API 创建的会议……」，且四支都要求**该应用创建的会议** ⇒ 不是给整个企业做设备台账管理（设备清单在 Rooms 族 [MeetingRooms.md](./MeetingRooms.md)）。

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

- 自建应用须配置在「**可调用接口的应用**」列表中；**仅允许获取 / 修改 / 操作该应用创建的会议的数据**。
- **设备引用只有一个键**：`MraDeviceRef.TmpOpenid`（JSON `tmp_openid`，官方必填）⇒ 先取得该 MRA 在本场会议的临时身份 ID（E1 之外，可由实时会中成员列表、PSTN 侧 `get_tmp_openid` 一类端点或回调事件获得）。
- **E2 的前置状态闸门**：注释明示「**如果当前 MRA 已显示会议自定义布局或个性布局或焦点视频，则不支持进行默认布局设置**」。
- `default_layout` 与 `default_novideo_user` 在 E2 中**官方必填**：分屏 **1 等分 / 2 全屏 / 3 1+N**；非视频与会者显示方式 **1 显示 / 2 隐藏**。
- E1 的 `instance_id` 本端点固定为 **9（voip、sip 设备，即 MRA 设备）**。
- `ip` 字段注释：**当成员在会中时才能返回** ⇒ 属可识别网络位置的数据，不得落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E4 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMeetingMraService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingMraService`） |
| 不可注入 | `IWechatWorkMeetingMraService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `QueryMraStatusAsync` — 获取 MRA 状态信息

**签名**

```csharp
[Post("/cgi-bin/meeting/mra/query_status")]
Task<QueryMraStatusResponse> QueryMraStatusAsync(
    [Body] QueryMraStatusRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/meeting/mra/query_status` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/meeting/mra/query_status?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `QueryMraStatusRequest`：`meetingid` + `tmp_openid`（均官方必填） |

**返回** `QueryMraStatusResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `tmp_openid` | `TmpOpenid` | `string?` | 被查询成员当场会议的临时 ID |
| `instance_id` | `InstanceId` | `int?` | 终端设备类型——**本端点为 9（voip/sip，即 MRA）** |
| `user_role` | `UserRole` | `int?` | **0 普通 / 1 创建者 / 2 主持人 / 3 创建者+主持人 / 4 小程序 / 5 小程序+主持人 / 6 联席主持人 / 7 创建者+联席主持人** |
| `webinar_member_role` | `WebinarMemberRole` | `int?` | **0 普通参会 / 1 内部嘉宾 / 2 外部嘉宾 / 3 邀请链接入会嘉宾 / 4 观众** |
| `ip` | `Ip` | `string?` | 成员 IP——**仅会中时返回** |
| `name` | `Name` | `string?` | 当前显示名称 |
| `audio_state` / `video_state` / `screen_shared_state` | `AudioState` / `VideoState` / `ScreenSharedState` | `bool?` | 麦克风 / 摄像头 / 屏幕共享开关态 |
| `default_layout` | `DefaultLayout` | `int?` | **1 等分 / 2 全屏 / 3 1+N** |
| `raise_hands_state` | `RaiseHandsState` | `bool?` | true 举手中 / false 手放下 |

**调用示例**（查状态 → 设默认分屏 → 举手 → 挂断；`tmp_openid` 是四支共用的设备键）

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
var mra = provider.GetRequiredService<IWechatWorkInternalMeetingMraService>();

// ④ E1 查状态：meetingid + tmp_openid 均必填；须是本应用创建的会议
var status = await mra.QueryMraStatusAsync(new QueryMraStatusRequest
{
    Meetingid = "demo-meeting-id",
    TmpOpenid = "demo-tmp-openid",            // 假值；实际取自会中成员列表 / PSTN 侧接口 / 回调事件
}, cancellationToken);
WechatWorkException.ThrowIfFailed(status, "/cgi-bin/meeting/mra/query_status");

// 只打印布尔与枚举计数，不打印 name / ip（可识别身份与网络位置）
logger.LogInformation("MRA 终端类型 {InstanceId}、麦克风状态 {AudioState}、默认分屏 {Layout}",
    status.InstanceId, status.AudioState, status.DefaultLayout);

// ⑤ E2 设默认分屏：若该设备已显示自定义布局 / 个性布局 / 焦点视频，官方不支持本设置
var layoutSet = await mra.SetMraDefaultLayoutAsync(new SetMraDefaultLayoutRequest
{
    Meetingid = "demo-meeting-id",
    DefaultLayout = 1,                        // 1 等分 / 2 全屏 / 3 1+N
    DefaultNovideoUser = 1,                   // 1 显示 / 2 隐藏
    Mra = new MraDeviceRef { TmpOpenid = "demo-tmp-openid" },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(layoutSet, "/cgi-bin/meeting/mra/set_default_layout");

// ⑥ E3 举手 / 放下：raise_hand 是布尔
var raised = await mra.SetMraRaiseHandAsync(new SetMraRaiseHandRequest
{
    Meetingid = "demo-meeting-id",
    RaiseHand = true,
    Mra = new MraDeviceRef { TmpOpenid = "demo-tmp-openid" },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(raised, "/cgi-bin/meeting/mra/set_raise_hand");

// ⑦ E4 挂断：mra 对象承载设备键；应答无业务字段
var hungUp = await mra.HangupMraAsync(new HangupMraRequest
{
    Meetingid = "demo-meeting-id",
    Mra = new MraDeviceRef { TmpOpenid = "demo-tmp-openid" },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(hungUp, "/cgi-bin/meeting/mra/hangup");
```

### E2. `SetMraDefaultLayoutAsync` — 设置 MRA 默认布局

**签名**

```csharp
[Post("/cgi-bin/meeting/mra/set_default_layout")]
Task<WechatWorkResponse> SetMraDefaultLayoutAsync(
    [Body] SetMraDefaultLayoutRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**（四字段全官方必填）：`meetingid` + `default_layout`（`int?`：**1 等分 / 2 全屏 / 3 1+N**）+ `default_novideo_user`（`int?`：**1 显示 / 2 隐藏**）+ `mra`（`MraDeviceRef`，内含 `tmp_openid`）。

**返回**：`WechatWorkResponse`（无业务负载）。

### E3. `SetMraRaiseHandAsync` — MRA 举手 / 手放下

**签名**

```csharp
[Post("/cgi-bin/meeting/mra/set_raise_hand")]
Task<WechatWorkResponse> SetMraRaiseHandAsync(
    [Body] SetMraRaiseHandRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `raise_hand`（`bool?`，**true 举手 / false 放下**）+ `mra`。
**返回**：`WechatWorkResponse`。

### E4. `HangupMraAsync` — 挂断 MRA 呼叫

**签名**

```csharp
[Post("/cgi-bin/meeting/mra/hangup")]
Task<WechatWorkResponse> HangupMraAsync(
    [Body] HangupMraRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`meetingid` + `mra`（`MraDeviceRef`）。**返回**：`WechatWorkResponse`。

> 挂断是**不可逆动作**（本族无「重呼 MRA」端点），重新入会须由设备侧或 Rooms 族的呼叫链路（`rooms/call`）再走一次。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **设备键是 `tmp_openid`，不是设备序列号** | 三支写端点都通过 `MraDeviceRef.TmpOpenid`（MT5 锁定 JSON 名 `tmp_openid`）定位当场会议里的这台设备 | 拿 Rooms 族的 `meeting_room_id` / `rooms_id` 顶替 |
| `tmp_openid` **当场有效** | 同域通则：临时 ID 跨会议取值不同 ⇒ 每次会议须重新取得 | 缓存后复用 → 目标不存在 |
| **E2 的状态闸门** | 「当前 MRA 已显示会议自定义布局或个性布局或焦点视频」时**不支持**默认布局设置 | 反复重试设布局，误判为接口故障 |
| 三处必填 vs 一处选填 | E2 的 `default_layout` 与 `default_novideo_user` **均官方必填**；E1 只需 `meetingid` + `tmp_openid` | 只传布局不传非视频成员显示方式 |
| 布尔与数值混用 | `raise_hand` / `audio_state` / `video_state` / `screen_shared_state` 是 `bool?`；`default_layout` / `default_novideo_user` / `user_role` / `instance_id` 是 `int?` | 「统一成数字」→ 线格式不符 |
| `instance_id` 取值口径 | 本族应答固定为 **9（voip/sip）**；全量设备枚举（0 PSTN / 1 PC / 2 Mac / 3 Android / 4 iOS / 5 Web / 6 iPad / 7 Android Pad / 8 小程序 / 9 voip·sip / 10 linux / 20·21 Rooms for Touch …）在会控与布局族的请求里使用 | 以为 9 是「Rooms 设备」 |
| 两个角色字段并存 | `user_role`（会议角色）与 `webinar_member_role`（网络研讨会角色）**语义不同、互不等价** | 只读一个就判定身份 |
| 挂断无回执 | E2~E4 都只回 `errcode`/`errmsg`；**「操作成功」不代表设备侧呈现已变更** | 凭应答断定状态，须再调 E1 复核 |
| 仅「API 创建的会议」 | 注释限定作用面 ⇒ 由客户端创建的会议不保证可操作 | 对全部会议下发动作 |
| `ip` 仅会中返回 | 不在会中时该字段缺省 ⇒ 空值不代表无 IP | 拿缺省当断线证据 |
| 全部官方即 POST | 含查询语义的 E1 也是 POST（全域仅 `layout/list_template` 是 GET） | 改 GET → 打坏 MT1 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验必填组合与状态闸门**、**不做操作结果复核**、**不脱敏 `name` / `ip`**（属宿主日志层）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 接口调用成功 | 写操作后建议再调 E1 复核设备状态 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`tmp_openid` 不存在、设备未在会中、会议非本应用创建、当前布局形态不支持默认布局设置等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 98786~98789 + 全局错误码页 |
| 端点频率与日配额 | 本域注释**无 QPS / 日配额数值** ⇒ 未逐项核验，勿借邻域数值 | 官方频率附录 90312 |
| `tmp_openid` 的取得路径全集 | 注释只说「被操作 MRA 设备的会中临时 ID」，未列官方推荐的获取端点清单 | [MeetingAdvanced.md](./MeetingAdvanced.md)（实时会中成员列表）与回调事件 |
| MRA 设备的注册与台账 | 本族只做「会中操作」，设备的接入、绑定与在线状态不在本族 ⇒ 属 Rooms 族 | [MeetingRooms.md](./MeetingRooms.md)（`mra_address`、`dial_string`） |
| 「焦点视频」的判定与设置面 | E2 闸门提到「已显示焦点视频时不支持」，但本族**无**读取/设置焦点视频的端点 | 官方布局族文档（[MeetingLayout.md](./MeetingLayout.md)） |
| 挂断后的重呼链路 | 本族无重呼端点，是否须经 `rooms/call` 未在本域留档 | 官方 98804（Rooms 呼叫） |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Meeting/README.md)（第 10 节「MRA 族的设备引用是 `MraDeviceRef.TmpOpenid`」、第 11 节 Rooms/MRA 与 OA 会议室的分树关系）
- 同域近邻：[MeetingRooms.md](./MeetingRooms.md)（Rooms 设备与 `mra_address` / `dial_string`，含呼叫与取消呼叫）· [MeetingLayout.md](./MeetingLayout.md)（自定义布局 / 个性布局——E2 闸门所指的两种形态）· [MeetingControl.md](./MeetingControl.md)（企业成员侧的会中控件，同样以 `tmp_openid` + `instance_id` 定位）
- 跨域边界：`tmp_openid` → 报名 ID 的反查在高级族；`tmp_external_userid` 的转换在 AccountId 域
- 官方页：[获取 MRA 状态（98786）](https://developer.work.weixin.qq.com/document/path/98786) · [设置默认布局（98787）](https://developer.work.weixin.qq.com/document/path/98787) · [举手/放下（98788）](https://developer.work.weixin.qq.com/document/path/98788) · [挂断 MRA 呼叫（98789）](https://developer.work.weixin.qq.com/document/path/98789)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
