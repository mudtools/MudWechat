# 会议室管理 · OA 会议室资源增删改查（仅企业自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `MeetingRoom` 域 · 会议室管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMeetingRoomService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/Room/IWechatWorkMeetingRoomService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMeetingRoomService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/Room/IWechatWorkMeetingRoomService_Internal.cs)（**唯一**子接口，承载全部 4 支；文件名按父接口 + `_Internal` 后缀） |
| DTO 源文件 | [DataModels/MeetingRoom/Room/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/MeetingRoom/Room/)（与预定族同属 `SerializerClassName = MeetingRoom` → `MeetingRoomJsonContext`，全域 22 型；`Room/` 子目录仅作组织、不入命名空间） |
| 契约守卫 | [WechatMeetingRoomContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMeetingRoomContractGuards.cs)（`MeetingRoomRoomEndpoints_ShouldMatchOfficialRoutes` 4 支路由 / `MeetingRoomSingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent` 继承链只有自建子接口 / `MeetingRoomTokenBinding_ShouldBeAccessTokenInjectedViaQuery` / `MeetingRoomDataModels_ShouldBeRegisteredInJsonContext` / `MeetingRoomOfficialFieldNames_ShouldBePreservedVerbatim`（含 `need_approval`、`department_list` 与「无业务字段的端点必须返回 `WechatWorkResponse`」）/ `MeetingRoomNumericalBooleanFields_ShouldBeCarriedAsNumbers` / `MeetingRoomOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable`）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 会议室管理 [93619](https://developer.work.weixin.qq.com/document/path/93619)（**4 支端点同页的四个小节**；「会议室」分类树 93617；访问频率附录 90312）——第三方应用与服务商代开发两棵文档树**均无对应页面** |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（E1 建 → E2 查 → E3 改（`range` 覆盖写）→ E4 删））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MR1/MR3/MR4/MR5/MR6/MR7/MR8 断言 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/README.md)（含 93619 参数表复核结论） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**实体会议室资源本身**的生命周期管理（官方 OA 会议室），4 支端点全部官方即 POST、全部无分页：

| 层 | 端点 | 用途 |
|---|---|---|
| 建 | E1 `oa/meetingroom/add` | 创建会议室（名称、容纳人数、位置、使用范围、支持设备），回 `meetingroom_id` |
| 读 | E2 `oa/meetingroom/list` | 按城市 / 楼宇 / 楼层 / 设备 ID 过滤列出会议室（**官方无分页参数**） |
| 改 | E3 `oa/meetingroom/edit` | 按 ID 修改各属性（`range` 为**覆盖写**） |
| 删 | E4 `oa/meetingroom/del` | 按 ID 删除会议室 |

形态：**零端点父接口 + 唯一自建子接口承载**（MR3）——官方「会议室」文档树只在企业自建应用根下存在 ⇒ 不设第三方 / 代开发子接口。

本族产出的 `meetingroom_id` 是 [Booking.md](./Booking.md) 全部预定端点的定位键：**先有会议室，才有预定**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`，MR4 锁定） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中**；代开发与第三方官方标注「暂不支持」（93619「权限说明」原文）。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 量纲闸门（接口注释原文）：`name` **最多 30 个字符**；`capacity` 为可容纳人数；`range.user_list` 与 `range.department_list` **各最多 1000 项**；**不填 `range` 则默认全公司可用**。
- **位置三件套必须成组**：要设置位置信息，必须**同时填写 `city`、`building`、`floor`**（E1/E3 同口径；E2 用低级过滤项时须填写其上一级）。
- **E3 已生成的建筑不支持修改经纬度**（`coordinate` 只在新建时有效）。
- `coordinate` 为所在建筑经纬度，官方建议用**腾讯地图坐标拾取器**获取。
- 设备 ID 附录（`equipment`）：**1 电视 / 2 电话 / 3 投影 / 4 白板 / 5 视频**。
- `range.user_list` 填成员 userid、`department_list` 填部门 ID ⇒ 属**组织架构与人员数据**：示例用假值，会议室授权名单**不要整体落日志**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E4 | ✅ | ❌ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册会议室模块（管理族与预定族同组同模块）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMeetingRoomApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.MeetingRoom` |
| 注册入口 | `WechatWorkServiceBuilder.AddMeetingRoomApi()`（→ 源生成 `AddMeetingRoomWebApiHttpClient()`） |
| 注册组名 | `"MeetingRoom"`（与预定族同组，MR3 锁定） |
| 可注入接口 | `IWechatWorkInternalMeetingRoomService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMeetingRoomService`） |
| 不可注入 | `IWechatWorkMeetingRoomService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`，`RegistryGroupName` 空） |

## 4. 端点契约明细

> **必填性由类型系统表达**（MR8）：官方「必须=是」的字段声明为**非可空**，「必须=否」保持**可空**以便省略；`list` 的四个过滤项全部为可空（官方全为选填）。

### E1. `AddMeetingRoomAsync` — 添加会议室

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/add")]
Task<MeetingRoomAddResponse> AddMeetingRoomAsync(
    [Body] MeetingRoomAddRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/meetingroom/add` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/oa/meetingroom/add?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `MeetingRoomAddRequest` |

**请求体** `MeetingRoomAddRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `name` | `Name` | `string` | **是** | 会议室名称，**≤30 个字符** |
| `capacity` | `Capacity` | `int` | **是** | 可容纳人数 |
| `city` / `building` / `floor` | `City` / `Building` / `Floor` | `string?` | 否 | 位置；**要设位置就三个一起填** |
| `coordinate` | `Coordinate` | `MeetingRoomCoordinate?` | 否 | `longitude` / `latitude`（`decimal`），建议取腾讯地图坐标拾取器 |
| `range` | `Range` | `MeetingRoomRange?` | 否 | 使用范围；**不填即全公司可用** |
| `equipment` | `EquipmentIdList` | `List<int>?` | 否 | 支持的设备 ID：1 电视 / 2 电话 / 3 投影 / 4 白板 / 5 视频 |

`MeetingRoomRange`：`user_list`（`List<string>?`，成员 userid，**≤1000**）+ `department_list`（**`List<long>?`**，部门 ID，**≤1000**；JSON 名由 MR6 锁定）。

**返回** `MeetingRoomAddResponse.MeetingRoomId`（`int?`，JSON `meetingroom_id`）——后续编辑、删除与预定全部以该值定位。

**调用示例**（E1 建 → E2 查 → E3 改 → E4 删，一条完整链路）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMeetingRoomApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.MeetingRoom;  // 请求/响应 DTO

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册会议室模块（管理族 + 预定族同模块）
builder.Services.AddWechatWorkServices(b => b.AddMeetingRoomApi());

// ③ 注入唯一的自建子接口（本域无第三方 / 代开发子接口）
var room = provider.GetRequiredService<IWechatWorkInternalMeetingRoomService>();

// ④ E1 新建会议室：位置三件套必须同时填写；不填 range 即全公司可用
var added = await room.AddMeetingRoomAsync(new MeetingRoomAddRequest
{
    Name = "3F-会议室A",                     // 非可空且 ≤30 字符
    Capacity = 12,                           // 非可空
    City = "深圳",
    Building = "总部",
    Floor = "3",
    Coordinate = new MeetingRoomCoordinate { Longitude = 113.93m, Latitude = 22.54m },
    EquipmentIdList = new List<int> { 1, 5 }, // 1 电视 / 5 视频
    Range = new MeetingRoomRange
    {
        UserIdList = new List<string> { "emp-0001" },        // 假值，勿用真人 userid
        DepartmentIdList = new List<long> { 1001 },          // 官方为 long 形态
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/oa/meetingroom/add");

// ⑤ E2 按位置与设备过滤查询：官方无分页，四个过滤项全部选填（填下级须带上级）
var listed = await room.GetMeetingRoomListAsync(new MeetingRoomListRequest
{
    City = "深圳",
    Building = "总部",
    Floor = "3",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(listed, "/cgi-bin/oa/meetingroom/list");

foreach (var item in listed.MeetingRoomList ?? new List<MeetingRoomInfo>())
{
    // need_approval 官方用数字 0/1 承载（0 无需审批 / 1 需要审批），本仓无数值-布尔转换器
    logger.LogInformation("会议室 {MeetingRoomId} need_approval={NeedApproval} capacity={Capacity}",
        item.MeetingRoomId, item.NeedApproval, item.Capacity);
}

// ⑥ E3 编辑：range 是覆盖写（传什么就是什么，不是增量添加）；已生成的建筑不支持改经纬度
var edited = await room.EditMeetingRoomAsync(new MeetingRoomEditRequest
{
    MeetingRoomId = added.MeetingRoomId!.Value,   // 非可空：官方必填
    Capacity = 20,
    Range = new MeetingRoomRange
    {
        DepartmentIdList = new List<long> { 1001, 1002 }, // 整范围被替换为这两个部门
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(edited, "/cgi-bin/oa/meetingroom/edit");

// ⑦ E4 删除：请求侧只有 meetingroom_id 一个业务参数；官方无业务响应字段
var deleted = await room.DeleteMeetingRoomAsync(new MeetingRoomDeleteRequest
{
    MeetingRoomId = added.MeetingRoomId!.Value,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/oa/meetingroom/del");
```

### E2. `GetMeetingRoomListAsync` — 查询会议室列表

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/list")]
Task<MeetingRoomListResponse> GetMeetingRoomListAsync(
    [Body] MeetingRoomListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `MeetingRoomListRequest`（**四项全部官方选填、全部可空；无分页参数**）：`city` / `building` / `floor`（`string?`）+ `equipment`（`EquipmentIdList`，`List<int>?`）。用低级过滤项时**须同时填写其上一级**（如用楼宇必须填城市）。

**返回** `MeetingRoomListResponse.MeetingRoomList`（JSON `meetingroom_list`，元素 `MeetingRoomInfo`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `meetingroom_id` | `MeetingRoomId` | `int?` | 会议室 ID |
| `name` / `capacity` | `Name` / `Capacity` | `string?` / `int?` | 名称与容纳人数 |
| `city` / `building` / `floor` | `City` / `Building` / `Floor` | `string?` | 位置 |
| `coordinate` | `Coordinate` | `MeetingRoomCoordinate?` | 经纬度 |
| `range` | `Range` | `MeetingRoomRange?` | 使用范围——**官方注明仅「会议室系统应用」查询时返回** |
| `equipment` | `EquipmentIdList` | `List<int>?` | 支持设备 ID |
| `need_approval` | `NeedApproval` | `int?` | **仅查询侧回传**（添加侧无此字段）；**0 无需审批 / 1 需要审批**，数字承载布尔语义（MR6 锁定字段名、MR7 锁定类型） |

### E3. `EditMeetingRoomAsync` — 编辑会议室

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/edit")]
Task<WechatWorkResponse> EditMeetingRoomAsync(
    [Body] MeetingRoomEditRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `MeetingRoomEditRequest`：`meetingroom_id`（`int`，**唯一必填**）+ `name` / `capacity` / `city` / `building` / `floor` / `coordinate` / `range` / `equipment`（**全部可空，不传即不改**）。

**返回**：`WechatWorkResponse`——官方成功响应仅 `errcode` / `errmsg`，**不另建响应 DTO**（MR6 断言返回类型）。

三条业务口径：① **`range` 为覆盖操作而非增量添加**（`user_list` / `department_list` 各 ≤1000，填写即覆盖整个使用范围）；② 修改位置须**同时输入城市、楼宇、楼层**；③ **已生成的建筑不支持修改经纬度**。

### E4. `DeleteMeetingRoomAsync` — 删除会议室

**签名**

```csharp
[Post("/cgi-bin/oa/meetingroom/del")]
Task<WechatWorkResponse> DeleteMeetingRoomAsync(
    [Body] MeetingRoomDeleteRequest request,
    CancellationToken cancellationToken = default);
```

- **请求体**：`MeetingRoomDeleteRequest.MeetingRoomId`（`int`，JSON `meetingroom_id`）——**唯一业务参数**，官方标必填。
- **返回**：`WechatWorkResponse`（同 E3，官方无业务字段）。
- 注释未给「删除后既有预定如何处理」的口径 ⇒ 见 §7。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`range` 是覆盖写** | E3 的 `user_list` / `department_list` 填写后**覆盖整个使用范围**，不是追加 | 只传新增成员 → 原范围被清空 |
| **位置三件套成组** | 设位置必须同时填 `city`+`building`+`floor`；`list` 侧用下级过滤须带上级 | 只填 `floor` → 位置不生效 / 过滤失效 |
| **经纬度只在新建时有效** | E3 注释明确「已生成的建筑暂不支持修改经纬度」 | 反复调 E3 改 `coordinate` 期望生效 |
| `need_approval` 数字布尔 + 仅查询侧 | 官方线格式 0/1 ⇒ 以 `int?` 承载（MR7）；**添加侧 DTO 没有该字段** | 声明 `bool?` → 读侧抛 `JsonException`；在添加请求里设它 → 无此参数 |
| `range` 仅会议室系统应用回传 | 官方注明 E2 的 `range.user_list` / `department_list` **仅会议室系统应用查询时返回** ⇒ 普通自建应用拿到的是缺省 | 把「range 为空」当成「范围为空=全公司」的结论 |
| `department_list` 是 `List<long>` | 部门 ID 用 `long` 承载（与通讯录侧 `int` 形态不同），`user_list` 是 `List<string>` | 类型「对齐」成 int 引发序列化偏差 |
| **无分页** | E2 请求体就是四个过滤项，官方无 `cursor` / `offset` ⇒ 结果集大小不可控 | 按邻域惯例补分页参数（MR1 路由与 MR8 可空侧会打红） |
| E3/E4 无业务响应体 | 两支一律返回 `WechatWorkResponse`，**不得另建响应 DTO**（MR6 断言） | 为「对称」建 `MeetingRoomEditResponse` |
| 必填侧非可空、选填侧可空 | MR8 逐字段：`add` 的 `name`/`capacity`、`edit`/`del` 的 `meetingroom_id` 非可空；`list` 四项、`edit` 的 `name` 等可空 | 收窄可空性 → 每个请求被迫带选填参数 |
| 查询官方即 POST | E2 是 POST + 请求体（本域四支全 POST、零 GET） | 改 GET → 打坏 MR1 与 G5 |
| 与预定域是两个「会议室」 | 本族管**资源**（`oa/meetingroom/` 下的 `add` / `list` / `edit` / `del`），[Booking.md](./Booking.md) 管**排期与占用**；视频会议硬件会议室又在 `/cgi-bin/meeting/rooms/` 前缀下 | 三条线混用 ID 或路由 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验名称长度 / 人数 / 1000 项上限 / 位置成组性**、**不做 `range` 的读-改-写合并**（覆盖写语义由宿主自行保证）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功才回 `meetingroom_id` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

会议室管理侧业务码（名称超长、`capacity` 非法、位置参数不完整、`range` 项超 1000、目标会议室不存在或无权限等）的**具体值接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口 XML 注释**未逐条摘录** ⇒ **未逐项核验** | 官方 93619 + 全局错误码页 |
| 删除会议室对既有预定的影响 | 注释只说「按会议室 ID 删除会议室」，未记既有 `booking_id` 的处置（失效 / 级联取消 / 报错）⇒ 未核验 | 官方 93619 与 [Booking.md](./Booking.md) E5 联读 |
| `equipment` 设备 ID 是否可扩展 | 注释只列 1~5 五档（电视/电话/投影/白板/视频），官方是否新增未核验 | 官方 93619 附录 |
| `coordinate` 的坐标系 | 注释只说「官方建议经腾讯地图坐标拾取器获取」，未记坐标系（GCJ-02 / WGS-84）与精度要求 ⇒ 未核验 | 官方 93619 + 腾讯地图文档 |
| `list` 结果集上限 | 官方无分页参数，也未记一次返回的最大条数 ⇒ 未核验 | 官方 93619 |
| `need_approval` 的写入面 | 该字段仅查询侧回传，**添加/编辑请求体中没有对应参数** ⇒ 审批开关的设置面不在本域（可能在管理后台） | 官方会议室管理后台说明 |
| 「会议室系统应用」的判别口径 | 注释说 `range` 仅会议室系统应用查询时返回，但如何判定当前应用是否该系统应用未记录 | 官方 93619 权限说明 |
| 端点频率限制 | 本族页面**无独立数值阈值**，走官方附录默认口径（每企业单 cgi/api 1 万次/分、15 万次/小时；每 IP 2 万次/分、60 万次/小时，90312） | 官方 90312 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MeetingRoom/README.md)（10 支端点速览、四接口令牌表、逐端点契约明细与 DTO 表）
- 同域另一族：[Booking.md](./Booking.md)（排期查询、三种预定制、取消与详情——本族产出的 `meetingroom_id` 是其定位键）
- 跨域：`range.user_list` 的成员账号来自通讯录 [Users.md](../Contacts/Users.md)、`department_list` 的部门 ID 来自 [Departments.md](../Contacts/Departments.md)（注意本族用 `long` 承载部门 ID）
- 易混的近邻：`Meeting` 域 MeetingRooms 子接口是**视频会议硬件会议室**（`/cgi-bin/meeting/rooms/*`），与本域 OA 会议室分属官方两棵章节树
- 官方页：[会议室管理（93619）](https://developer.work.weixin.qq.com/document/path/93619) · [会议室预定管理（93620）](https://developer.work.weixin.qq.com/document/path/93620) · [访问频率限制（90312）](https://developer.work.weixin.qq.com/document/path/90312)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
