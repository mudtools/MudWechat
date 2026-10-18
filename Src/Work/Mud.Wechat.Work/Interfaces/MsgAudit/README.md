# MsgAudit 域 · 会话内容存档（管理面）

企业微信「会话内容存档」域的 **HTTP 管理面**：本仓共落地 **5 支端点**，按官方章节切成四个接口族——
开启成员列表域 **1 支**（守卫 MA1a）、机器人信息域 **1 支**（MA1b）、会话同意情况域 **2 支**（MA1c）、
内部群信息域 **1 支**（MA1d）。四族都是**零端点抽象父接口 + 唯一自建子接口承载**形态：
父接口 [IWechatWorkMsgAuditPermitUserService.cs](./IWechatWorkMsgAuditPermitUserService.cs) /
[IWechatWorkMsgAuditRobotService.cs](./IWechatWorkMsgAuditRobotService.cs) /
[IWechatWorkMsgAuditAgreeService.cs](./IWechatWorkMsgAuditAgreeService.cs) /
[IWechatWorkMsgAuditGroupChatService.cs](./IWechatWorkMsgAuditGroupChatService.cs) 不注册；
端点声明于 [IWechatWorkInternalMsgAuditPermitUserService.cs](./IWechatWorkInternalMsgAuditPermitUserService.cs) /
[IWechatWorkInternalMsgAuditRobotService.cs](./IWechatWorkInternalMsgAuditRobotService.cs) /
[IWechatWorkInternalMsgAuditAgreeService.cs](./IWechatWorkInternalMsgAuditAgreeService.cs) /
[IWechatWorkInternalMsgAuditGroupChatService.cs](./IWechatWorkInternalMsgAuditGroupChatService.cs)。
注册组 `MsgAudit`，模块枚举 `WechatModule.MsgAudit`（[`AddMsgAuditApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 8 份接口文件的 XML 注释、
> [WechatMsgAuditContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMsgAuditContractGuards.cs)（MA1a~MA4）、
> [Src/Work/Mud.Wechat.Work.DataModels/MsgAudit/](../../../Mud.Wechat.Work.DataModels/MsgAudit/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

本域 5 支端点官方**只向企业自建应用开放**（代开发与第三方均标注「暂不支持」，MA2 反向断言继承链上
不得出现 `*Provider*` / `*ThirdParty*` 子接口）。令牌/开放面列写本仓真实契约：
`access_token` 恒为 Query 注入（MA3 对**全部 8 个接口**断言 `TokenType` + `Query` + `Name = "access_token"`，
MUD005 已接受风险），且该 `access_token` 必须由「会话内容存档」应用的 secret 换发（见下节）。

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetPermitUserListAsync` | POST | `/cgi-bin/msgaudit/get_permit_user_list` | `access_token`(Query，存档应用 secret 换发)；**仅自建** | 91614；`type` 1/2/3，不填回全量；≤1000 次/分 |
| 2 | `GetRobotInfoAsync` | GET | `/cgi-bin/msgaudit/get_robot_info` | 同上；**仅自建** | 91774；`robot_id` 走 Query（MA1b 断言）；≤600 次/分 |
| 3 | `CheckSingleAgreeAsync` | POST | `/cgi-bin/msgaudit/check_single_agree` | 同上；**仅自建** | 91782；单批 ≤100 条目；≤2500 次/分 |
| 4 | `CheckRoomAgreeAsync` | POST | `/cgi-bin/msgaudit/check_room_agree` | 同上；**仅自建** | 91782；≤1500 次/分；应答不回 `userid` |
| 5 | `GetGroupChatInfoAsync` | POST | `/cgi-bin/msgaudit/groupchat/get` | 同上；**仅自建** | 92951；仅内部群；≤2000 次/分；`errcode 301052` = 存档过期 |

## 1. 令牌口径：本域所有 `access_token` 必须是「会话内容存档」应用换发的那一支

接口 XML 注释与 MA3 守卫注释同口径写明：**本模块全部接口的 `access_token` 必须由「会话内容存档」应用
secret 获取，宿主将该 secret 配置为对应应用的 `AgentSecret`**。
⇒ 这不是「任意一个自建应用令牌都能调」的域：用 CRM、审批等其它应用的 secret 换发的令牌调用本域 5 支端点，
在官方侧即属能力越界，本仓**不做**本地 secret 预判，越界由应答 `errcode` 表达。
`AgentSecret` 与换发出的 `access_token` 都是凭据：**不得进日志、遥测或异常消息**，配置与示例一律用占位符。

## 2. `exteranalopenid`：官方原文笔误，两处都被守卫钉住

会话同意情况域的「外部成员标识」官方原文拼写是 `exteranalopenid`（不是 `externalopenid`）。
MA1c 在**请求条目**与**应答**两处分别锁定 JSON 名：
[CheckMsgAuditSingleAgreeRequest.cs](../../../Mud.Wechat.Work.DataModels/MsgAudit/RequestModel/CheckMsgAuditSingleAgreeRequest.cs)
里的 `MsgAuditAgreeQueryItem.ExteranalOpenid` 与
[CheckMsgAuditAgreeResponse.cs](../../../Mud.Wechat.Work.DataModels/MsgAudit/CheckMsgAuditAgreeResponse.cs)
里的 `MsgAuditAgreeInfo.ExteranalOpenid`，两者 `[JsonPropertyName]` 均必须等于 `"exteranalopenid"`。
C# 属性名也跟着官方拼写（`ExteranalOpenid`）⇒ **不得**「顺手改成正确拼写」，改了序列化即与官方不符。

## 3. 单聊与群聊两支共用一个响应类型，但字段集合不同构

| 端点 | 请求体 | 应答 `agreeinfo[]` 字段 |
|---|---|---|
| #3 `check_single_agree` | `CheckMsgAuditSingleAgreeRequest.Info`（`List<MsgAuditAgreeQueryItem>?`，JSON `info`，每项 `userid` + `exteranalopenid`） | `userid` / `exteranalopenid` / `agree_status` / `status_change_time` 四键 |
| #4 `check_room_agree` | `CheckMsgAuditRoomAgreeRequest.RoomId`（`string?`，JSON `roomid`） | **不返回 `userid`**（接口注释原文：「群聊响应的 agreeinfo 不返回内部成员 userid 字段」） |

两支都返回同一个 `CheckMsgAuditAgreeResponse`（`AgreeInfo`，JSON `agreeinfo`）⇒ 群聊场景下
`MsgAuditAgreeInfo.UserId` 恒为 `null`，**不得**据此推断「该成员未同意」；判定只认 `AgreeStatus`
（`string?`，注释留档取值 `Agree` / `Disagree`）与 `StatusChangeTime`（`long?`，Unix 秒、UTC）。

## 4. 五档频率与单批上限：全部由官方侧执行，本仓不拦截

接口注释留档的逐端点数值（本仓无本地限流器、无批次裁剪）：

| 端点 | 频率上限 | 批次约束 |
|---|---|---|
| #1 | ≤ 1000 次/分钟 | 无请求条目上限注释（仅 `type` 一个参数） |
| #2 | ≤ 600 次/分钟 | `robot_id` 单个，必填 |
| #3 | ≤ 2500 次/分钟 | **一次最多 100 个查询条目，超出会被拦截**（注释原文） |
| #4 | ≤ 1500 次/分钟 | `roomid` 单个 |
| #5 | ≤ 2000 次/分钟 | `roomid` 单个 |

## 5. #1 只回「实际生效」成员，且部门/标签被打散

[GetMsgAuditPermitUserListRequest.cs](../../../Mud.Wechat.Work.DataModels/MsgAudit/RequestModel/GetMsgAuditPermitUserListRequest.cs)
的唯一参数 `Type`（`int?`，JSON `type`）注释口径：**1 办公版 / 2 服务版 / 3 企业版，不填返回全量成员列表**。
接口注释另记两条：返回的是「设置在开启范围内的成员 userid」，**部门/标签会被打散为全部成员 userid**；
且**仅返回实际生效成员**——开启范围超出购买人数时，**不包含**超容后不生效的 userid。
⇒ 该列表不能用来判断「谁被配置进了开启范围」，只能用来判断「谁当前真的在存档」；
应答是 `GetMsgAuditPermitUserListResponse.Ids`（`List<string>?`，JSON `ids`），**无分页面**。

## 6. 机器人信息应答嵌在 `data` 对象里

#2 是本域唯一非「顶层平铺」的应答：[GetMsgAuditRobotInfoResponse.cs](../../../Mud.Wechat.Work.DataModels/MsgAudit/GetMsgAuditRobotInfoResponse.cs)
把三个字段包在 `Data`（`MsgAuditRobotInfo?`，JSON `data`）里 ⇒ 取值是 `resp.Data?.RobotId` /
`.Name` / `.CreatorUserId`，不要按其余四支的顶层读法写。
#2 也是本域唯一 GET 端点，且 `robot_id` 必须以 `[Query("robot_id")] string robotId` 传
（MA1b 用反射断言该 Query 标注存在，改成 Body 即红）。

## 7. 本域不含「取正文 / 解密」；存档私钥与 `sync_msg` 属另一能力面

会话内容存档的**取消息正文与解密**（`sync_msg`、encrypt_key / secret_key 解密、媒体拉取）不在本域：
本域 5 支端点只做「成员开启状态 / 机器人信息 / 同意情况 / 内部群信息」四类查询。
原生 C SDK 的进程内封装落在 [`ExtendedSDK/Finance/`](../../README.md)（`IWechatWorkFinanceClient` +
`AddWechatFinanceSdk()` + 配置节 `WechatFinance`），**无 HTTP 端点、不进 `WechatModule`、不参与令牌链路**
（守卫 FIN-B6 断言两面零交叉，包级 README 已声明二者是两件事）。
⇒ 存档私钥、机器人 secret 一类凭据同样**不得进日志、遥测或异常消息**；本域接口与 DTO 不承载私钥参数，
凡在本文之外看到「本域应当有解密/拉正文端点」的推断，均与仓内契约不符。
91774 的原生 C SDK 面亦不由 HTTP SDK 承载（`WechatModule.MsgAudit` remarks 明示）。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMsgAuditApi());

// 抽象父接口不注册，须注入自建子接口
public sealed class ArchiveComplianceProbe(
    IWechatWorkInternalMsgAuditPermitUserService permitUsers,
    IWechatWorkInternalMsgAuditAgreeService agree,
    IWechatWorkInternalMsgAuditGroupChatService groupChat,
    IWechatWorkInternalMsgAuditRobotService robots)
{
    // 1) 当前实际生效的企业版存档成员（type = 3；不填即全量）
    public async Task<IReadOnlyList<string>> ActiveUserIdsAsync(CancellationToken ct)
    {
        var resp = await permitUsers.GetPermitUserListAsync(
            new GetMsgAuditPermitUserListRequest { Type = 3 }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/get_permit_user_list");
        return resp.Ids ?? [];                        // 无分页面，一次返回
    }

    // 2) 单聊同意情况：一批最多 100 条，超出官方拦截
    public async Task CheckSingleAgreeAsync(CancellationToken ct)
    {
        var resp = await agree.CheckSingleAgreeAsync(new CheckMsgAuditSingleAgreeRequest
        {
            Info =
            [
                new MsgAuditAgreeQueryItem
                {
                    UserId = "zhangsan",
                    ExteranalOpenid = "wmXXXXXXXXXXXX",   // 字段名照官方笔误拼写，勿改
                },
            ],
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/check_single_agree");

        foreach (MsgAuditAgreeInfo item in resp.AgreeInfo ?? new List<MsgAuditAgreeInfo>())
        {
            // item.ExteranalOpenid / item.AgreeStatus（Agree / Disagree）/ item.StatusChangeTime（Unix 秒）
            // item.UserId 仅单聊应答有值
        }
    }

    // 3) 群聊同意情况：同一响应类型，但应答不含 userid
    public async Task CheckRoomAgreeAsync(string roomId, CancellationToken ct)
    {
        var resp = await agree.CheckRoomAgreeAsync(
            new CheckMsgAuditRoomAgreeRequest { RoomId = roomId }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/check_room_agree");
    }

    // 4) 内部群信息：仅支持内部群；errcode 301052 表示存档已过期
    public async Task<(string? Name, string? Creator)> GetInnerGroupAsync(
        string roomId, CancellationToken ct)
    {
        var resp = await groupChat.GetGroupChatInfoAsync(
            new GetMsgAuditGroupChatRequest { RoomId = roomId }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/groupchat/get");
        return (resp.RoomName, resp.Creator);          // Members: MemberId + JoinTime
    }

    // 5) 机器人信息：GET + robot_id 走 Query；应答嵌在 data 对象内
    public async Task<string?> GetRobotNameAsync(string robotId, CancellationToken ct)
    {
        var resp = await robots.GetRobotInfoAsync(robotId, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/get_robot_info");
        return resp.Data?.Name;
    }
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，
`301052` 一类业务错误码**不会**触发令牌恢复，须由宿主当作「存档过期」自行处置。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 五档频率是否按「企业 / 应用 / 接口」维度独立计数 | 接口注释只写「不可超过 X 次/分钟」，计数维度**未逐项核验** | 需要自建限流前先核验官方页 |
| `errcode` 全集 | 本域注释只留档 `301052`（存档过期）；其余码段与「secret 非存档应用」对应的具体错误码**未逐项核验** | 判错只按 `errcode != 0` 通用处理，不得枚举假设 |
| `agree_status` 取值全集 | DTO 注释留档 `Agree` / `Disagree` 两值；是否存在第三态**未逐项核验** | 需要判定「未知」时先核验，勿加隐式默认 |
| `type` 与购买版本的映射关系 | 注释只有 1 办公版 / 2 服务版 / 3 企业版；版本口径由邻域 license 决定，映射**未在本域核验** | 跨域聚合前先核验，勿在 SDK 侧建映射表 |
| 外部群（含跨企业成员）的信息查询面 | #5 注释明示「仅支持查询内部群」，外部群对应端点**未在本域核验**、本域零声明 | 不得用 #5 承载外部群；需要时先核验官方页再加面 |
| 群聊 `roomid` 的获取来源 | 注释只说「填入会话内容存档中获取到的 roomid」，其产出面属存档数据面（非本域）⇒ **未在本域核验** | 取 roomid 走 `ExtendedSDK/Finance/` 侧，勿在本域加端点 |
| `robot_id` 与 `WechatFinance` 机器人键的对应关系 | 本域只提供查询，二者是否同一标识**未逐项核验** | 装配原生客户端前先核验，勿假定同值 |
| #1 的返回规模上限 | 无分页面，成员量极大时是否截断**未逐项核验** | 大企业接入前须实测，勿在 SDK 侧加截断假设 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（五档频率、≤100 条目、type 取值、301052、群聊不回 userid） | 本目录 8 份接口文件的 XML 注释 |
| 四族路由与计数（MA1a 1 / MA1b 1 / MA1c 2 / MA1d 1） | [WechatMsgAuditContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMsgAuditContractGuards.cs) MA1a（L95–L112）、MA1b（L115–L142）、MA1c（L145–L177）、MA1d（L180–L198） |
| `robot_id` 必须以 `[Query("robot_id")]` 传递 | MA1b（L134–L141） |
| `exteranalopenid` 两处拼写锁定 | MA1c（L168–L176） |
| 四族「零端点父接口 + 唯一自建子接口」、不得出现第三方/代开发子接口 | MA2（L201–L250） |
| 全部 8 接口 `AccessToken` + Query + `access_token`、存档 secret 约束 | MA3（L253–L281） |
| JSON 上下文登记（全量 12 个契约面类型） | MA4（L284–L312） |
| DTO 属性名 / 类型 / JSON 名 | [../../../Mud.Wechat.Work.DataModels/MsgAudit/](../../../Mud.Wechat.Work.DataModels/MsgAudit/)（含 `RequestModel/`） |
| 上下文源生成文件 | [../../../Mud.Wechat.Work.DataModels/Generated/MsgAuditJsonContext.g.cs](../../../Mud.Wechat.Work.DataModels/Generated/MsgAuditJsonContext.g.cs) |
| Query 令牌注入白名单（G5）与令牌归属键（TO 系列） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| 原生 C SDK 面（与本文无关的另一能力面） | [../../README.md](../../README.md) 的「会话内容存档原生封装」段、`ExtendedSDK/Finance/` |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L147） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号与频率/批次限制，**同批**改
   [WechatMsgAuditContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMsgAuditContractGuards.cs)
   的 `PermitUserRoutes` / `RobotRoutes` / `AgreeRoutes` / `GroupChatRoutes` 四张表与各自 `HaveCount`
   （1 / 1 / 2 / 1）；改动词须同步改 MA1b 的 `[Query("robot_id")]` 反射断言。
2. 加 / 改族接口 → MA2 的四族清单（父接口 + 期望子接口集合）须同批改；
   **不要**为「对齐邻域」给本域补 `*Provider*` / `*ThirdParty*` 子接口——官方四页均标「暂不支持」，
   MA2 会以反证失败。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `MsgAudit`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 MA4 的类型清单（当前全量 12）与 [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L147）。
   外部成员标识键一律照官方笔误 `exteranalopenid`，属性名保持 `ExteranalOpenid`。
4. 改子接口令牌与注册组 → 自建子接口必须保留 `TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken`
   与 `RegistryGroupName = "MsgAudit"`（父接口保持空组、不声明归属键）；新增可注入子接口须同批登记
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
   的 G5 白名单，并过
   [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）。
5. 要落地取正文 / 解密面 → **不落在本域**：那是 `ExtendedSDK/Finance/` 的 C SDK 封装（无端点、不进 `WechatModule`），
   改动须同批过 FIN 系列守卫，并同步 [../../README.md](../../README.md) 的「两件事」表述。
6. 本文与 [../../README.md](../../README.md) 的域内端点数、族划分表述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
