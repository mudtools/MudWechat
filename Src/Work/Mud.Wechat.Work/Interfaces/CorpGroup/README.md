# CorpGroup 域 · 上下游（企业互联 / 上下游）

企业微信「上下游 / 企业互联」域：本仓共落地 **20 支端点**，分属三个接口族——
上下游基础接口族 **6 支**（守卫 [CG1](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCorpGroupContractGuards.cs)：公共父接口
[IWechatWorkCorpGroupService.cs](./IWechatWorkCorpGroupService.cs) 1 支 + 自建/代开发中间父接口
[IWechatWorkCorpGroupInternalProviderService.cs](./IWechatWorkCorpGroupInternalProviderService.cs) 5 支）、
上下游通讯录管理族 **9 支**（守卫 CG5：[IWechatWorkCorpGroupContactsService.cs](./IWechatWorkCorpGroupContactsService.cs) 4 支读取面 +
[IWechatWorkCorpGroupContactsService_Internal.cs](./IWechatWorkCorpGroupContactsService_Internal.cs) 5 支写入/任务/查询）、
上下游对接规则族 **5 支**（守卫 CG9：[IWechatWorkCorpGroupRulesService.cs](./IWechatWorkCorpGroupRulesService.cs) 零端点父接口 +
[IWechatWorkCorpGroupRulesService_Internal.cs](./IWechatWorkCorpGroupRulesService_Internal.cs) 5 支）。
应用类型子接口：[IWechatWorkCorpGroupService_Internal.cs](./IWechatWorkCorpGroupService_Internal.cs) /
[IWechatWorkCorpGroupService_Provider.cs](./IWechatWorkCorpGroupService_Provider.cs) /
[IWechatWorkCorpGroupService_ThirdParty.cs](./IWechatWorkCorpGroupService_ThirdParty.cs) /
[IWechatWorkCorpGroupContactsService_Provider.cs](./IWechatWorkCorpGroupContactsService_Provider.cs)（空标记）。
注册组 `CorpGroup`，模块枚举 `WechatModule.CorpGroup`（[`AddCorpGroupApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录接口文件的 XML 注释、
> [WechatCorpGroupContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCorpGroupContractGuards.cs)（CG1~CG11）、
> [Src/Work/Mud.Wechat.Work.DataModels/CorpGroup/](../../../Mud.Wechat.Work.DataModels/CorpGroup/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面列写本仓真实契约：`access_token` 恒为 Query 注入（守卫 CG3 / CG7 / CG10，MUD005 已接受风险，
无法改 Header）；「开放面」指官方对该端点开放的应用类型，由 CG2 / CG6 的层级断言锁死。

### 上下游基础接口族（CG1，6 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `ListAppShareInfoAsync` | POST | `/cgi-bin/corpgroup/corp/list_app_share_info` | `access_token`(Query)；自建 + 第三方 + 代开发 | 唯一三类应用公共面；95813/95324/96872，旧树 93403/93405/96816 |
| 2 | `GetCorpGroupTokenAsync` | POST | `/cgi-bin/corpgroup/corp/gettoken` | `access_token`(Query)；自建 + 代开发 | 换取**下级/下游企业**应用凭证；95816/96873，旧树 93359/96814 |
| 3 | `TransferMiniProgramSessionAsync` | POST | `/cgi-bin/miniprogram/transfer_session` | `access_token`(Query)；自建 + 代开发 | 消费的令牌是 #2 返回的下级企业凭证；95817/96874，旧树 93355/96813 |
| 4 | `UnionidToExternalUserIdAsync` | POST | `/cgi-bin/corpgroup/unionid_to_external_userid` | `access_token`(Query)；自建 + 代开发 | 95818/96875；可带 `mass_call_ticket` |
| 5 | `UnionidToPendingIdAsync` | POST | `/cgi-bin/corpgroup/unionid_to_pending_id` | `access_token`(Query)；自建 + 代开发 | 97357/98040；pending_id 有效期 90 天 |
| 6 | `ExternalUserIdToPendingIdAsync` | POST | `/cgi-bin/corpgroup/batch/external_userid_to_pending_id` | `access_token`(Query)；自建 + 代开发 | 97357/98040；单批 ≤100 |

### 上下游通讯录管理族（CG5，9 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 7 | `GetChainListAsync` | GET | `/cgi-bin/corpgroup/corp/get_chain_list` | `access_token`(Query)；自建 + 代开发 | 95820/96876 同页四读取端点；无请求体 |
| 8 | `GetChainGroupAsync` | POST | `/cgi-bin/corpgroup/corp/get_chain_group` | `access_token`(Query)；自建 + 代开发 | 95820/96876 |
| 9 | `GetChainCorpInfoListAsync` | POST | `/cgi-bin/corpgroup/corp/get_chain_corpinfo_list` | `access_token`(Query)；自建 + 代开发 | 95820/96876；cursor + limit 翻页 |
| 10 | `GetChainCorpInfoAsync` | POST | `/cgi-bin/corpgroup/corp/get_chain_corpinfo` | `access_token`(Query)；自建 + 代开发 | 95820/96876 |
| 11 | `ImportChainContactsAsync` | POST | `/cgi-bin/corpgroup/import_chain_contact` | `access_token`(Query)；**仅自建** | 95821；异步任务，返回 `jobid` |
| 12 | `GetChainImportResultAsync` | GET | `/cgi-bin/corpgroup/getresult` | `access_token`(Query)；**仅自建** | 95823；`jobid` 走 Query |
| 13 | `RemoveChainCorpAsync` | POST | `/cgi-bin/corpgroup/corp/remove_corp` | `access_token`(Query)；**仅自建** | 95822；仅已验证企业 |
| 14 | `GetChainUserCustomIdAsync` | POST | `/cgi-bin/corpgroup/corp/get_chain_user_custom_id` | `access_token`(Query)；**仅自建** | 97441 |
| 15 | `GetCorpSharedChainListAsync` | POST | `/cgi-bin/corpgroup/get_corp_shared_chain_list` | `access_token`(Query)；**仅自建** | 97442 |

### 上下游对接规则族（CG9，5 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 16 | `ListChainRuleIdsAsync` | POST | `/cgi-bin/corpgroup/rule/list_ids` | `access_token`(Query)；**仅自建** | 95631；仅「创建空间的主企业」 |
| 17 | `DeleteChainRuleAsync` | POST | `/cgi-bin/corpgroup/rule/delete_rule` | `access_token`(Query)；**仅自建** | 95632 |
| 18 | `GetChainRuleInfoAsync` | POST | `/cgi-bin/corpgroup/rule/get_rule_info` | `access_token`(Query)；**仅自建** | 95633 |
| 19 | `AddChainRuleAsync` | POST | `/cgi-bin/corpgroup/rule/add_rule` | `access_token`(Query)；**仅自建** | 95634；与 #20 共用「每天 ≤1000 次」额度 |
| 20 | `ModifyChainRuleAsync` | POST | `/cgi-bin/corpgroup/rule/modify_rule` | `access_token`(Query)；**仅自建** | 95635 |

## 1. 应用类型面不等宽：第三方类型化面只有 1 支

CG2 把三件事钉成断言：公共父接口 `IWechatWorkCorpGroupService` 声明面**恰 1** 条（#1）；
中间父接口 `IWechatWorkCorpGroupInternalProviderService` 声明面**恰 5** 条（#2–#6）；
`IWechatWorkThirdPartyCorpGroupService` 必须**直接**继承公共父接口、**不得**继承中间父接口，
其类型化面 `HaveCount(1)`，自建与代开发子接口类型化面各 `HaveCount(6)`。
⇒ 注入 `IWechatWorkThirdPartyCorpGroupService` 时拿不到 #2–#6，这不是本仓漏挂，而是官方无第三方文档页；
**不得**靠「让第三方也继承中间父接口」对齐，那会同时改坏 CG1/CG2 与 G5 白名单。
自建与代开发子接口都是**空标记**（零声明端点），全部端点来自继承链。

## 2. `gettoken` 返回的凭证不进令牌基座，#3 需宿主自己接

`GetCorpGroupTokenAsync` 应答 [GetCorpGroupTokenResponse.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/GetCorpGroupTokenResponse.cs)
的 `AccessToken`（`string?`，官方上限 512 字节）与 `ExpiresIn`（`int`）是**下级/下游企业**应用的凭证：
本仓令牌链路只托管调用方自身应用的 `access_token`，**不自动缓存**该凭证（接口 XML 注释明示）。
`TransferMiniProgramSessionAsync` 官方要求携带**下级企业**的 `access_token` 调用 ⇒ 由宿主写入令牌存储后切换上下文、
或自行注入。该值是凭据：不得进日志、遥测或异常消息，配置与示例一律用占位符。

## 3. 三类令牌归属键与「企业级令牌」例外

| 子接口 | `TokenManagerKey` | 语义 |
|---|---|---|
| `IWechatWorkInternalCorpGroupService` / `IWechatWorkInternalCorpGroupContactsService` / `IWechatWorkInternalCorpGroupRulesService` | `WechatTokenManagerKeys.InternalAccessToken` | 自建应用自身凭证 |
| `IWechatWorkProviderCorpGroupService` / `IWechatWorkProviderCorpGroupContactsService` / `IWechatWorkThirdPartyCorpGroupService` | `WechatTokenManagerKeys.CorpAccessToken` | 授权企业级令牌：一个 (AppKey, authCorpId) 一份，scope = authCorpId |

令牌路由键恒为 `Wechat.AccessToken`（CG3/CG7/CG10 断言 `TokenType` + `InjectionMode.Query` + `Name = "access_token"`）。
第三方/代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到目标授权企业作用域
（`permanent_code` 只写参数名，不得进日志）。路由键与归属键的分离口径见
[AGENTS.md §5.1](../../../../../AGENTS.md) 与 [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)。

## 4. unionid 转换族：三档频率、`mass_call_ticket` 豁免、pending_id 90 天

接口 XML 注释留档的逐端点业务限制（[IWechatWorkCorpGroupInternalProviderService.cs](./IWechatWorkCorpGroupInternalProviderService.cs)）：

- #4/#5/#6 按**上游企业**维度限流：**10 万次/小时、48 万次/天、750 万次/月**。
- #4 传入有效 `mass_call_ticket` 可**不受该限制**（仍受基础频率限制），官方定位为数据初始化场景；
  请求体 [UnionidToExternalUserIdRequest.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/RequestModel/UnionidToExternalUserIdRequest.cs)
  的 `MassCallTicket`（`string?`）即该票据——属凭据，不得进日志。
- `pending_id` 是临时外部联系人 ID：**有效期 90 天**、共享应用内唯一 ⇒ 预转换后须在 90 天内完成真实外部联系人换取，
  过期需重新生成。
- #6 单批最多同时查询 **100** 个（请求体 `ExternalUserIds`，JSON 名 `external_userid`）。
- 本仓**不做**本地限流与批次裁剪，越界由应答 `errcode` 表达。

## 5. `corpgroup/getresult`：三条同义路由的拼写互不相同

#12 的路由是 `/cgi-bin/corpgroup/getresult`（**无下划线**）。CG5 的守卫注释专门标注：它与导出域
`/cgi-bin/export/get_result`、异步导入域 `/cgi-bin/batch/getresult` 三种拼写各异，
不得按「统一命名」顺手改写；任务号参数名也固定为 Query 上的 `jobid`（`[Query("jobid")] string jobId`），
官方上限 64 字节。

## 6. 通讯录导入的四道容量闸 + 串行单任务 + 仅已验证企业

`ImportChainContactsAsync`（95821）接口注释留档：单次 ≤ **1000** 个企业、单个企业 ≤ **200** 人、
单次 ≤ **2000** 人、每天 ≤ **20000** 人；**只允许串行调用且同时只能存在一个导入任务**（含管理后台提交的任务）；
**仅已验证的企业可调用**；自建应用须配置到「上下游 - 可调用接口的应用」中；
**自 2023-12-01 起不再支持通过系统应用 secret 调用**。
异步语义：#11 只返回 `JobId`（[ImportChainContactsResponse.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/ChainContacts/ImportChainContactsResponse.cs)），
必须轮询 #12 拿 `Status`（1 开始 / 2 进行中 / 3 完成）与 `Result`；
#12 **只能查询已提交过的历史任务**、并发限制 **5**（接口注释）。
移除端点 #13 同样「仅已验证企业可调用」，注释标注并发限制 **1**。

条目形状见 [ChainImportCorpItem.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/ChainContacts/RequestModel/ChainImportCorpItem.cs) 与
[ChainImportContactItem.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/ChainContacts/RequestModel/ChainImportContactItem.cs)：
`IdentityType`（`int`，JSON `identity_type`）取值 **1 成员 / 2 负责人**，单企业负责人最多 **5** 人、可不填；
`UserCustomId`（`string?`）官方约束是**字符串承载的 64 位无符号整型**——取值 1 到 2^64-2、必须全数字、
不得有前置 0、且不能为 11 位或 13 位数字 ⇒ 不要用 `long` 建模（该键本仓就是 `string?`）。

## 7. 对接规则：只有主企业，且「至少填一个」的条件必填

规则族 5 支（95631~95635）**仅适用于上下游中创建空间的主企业**，第三方与代开发无文档页 ⇒
本族只有自建子接口（CG10 断言声明面恰 5、父接口零端点）。
`AddChainRuleAsync` / `ModifyChainRuleAsync` **每天最多 1000 次且两者共用额度**；其余三支注释无频率数值。

规则体的两组范围都是「同组两键至少填一个」的条件必填，本仓不本地拦截：

| 类型 | 属性 → JSON 名 | 口径 |
|---|---|---|
| [ChainRuleOwnerRange.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/Rules/ChainRuleOwnerRange.cs) | `DepartmentIds` → `departmentids`、`UserIds` → `userids` | 两键均为 `List<string>?`，**官方示例把 id 以字符串传输** ⇒ 不得改成 `List<long>` |
| [ChainRuleMemberRange.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/Rules/ChainRuleMemberRange.cs) | `GroupIds` → `groupids`、`CorpIds` → `corpids` | 同为 `List<string>?`，至少填一个 |

应答侧注意：`AddChainRuleResponse.RuleId` 是 `int?`，`DeleteChainRuleRequest.RuleId` /
`GetChainRuleInfoRequest.RuleId` / `ModifyChainRuleRequest.RuleId` 是非可空 `int`（官方必填）。

## 8. 企业互联旧文档号是同一契约的别名，`93404` 不是端点

#1/#2/#3 的注释同时给出「上下游」新树与「企业互联」旧树编号（95813↔93403、95816↔93359、95817↔93355
及代开发侧 96872↔96816、96873↔96814、96874↔96813）——同路由同契约，不是六个端点。
CG 守卫注释另记：企业互联树的 `93404`「获取下级企业付费版本信息」**不是独立 HTTP 端点**，
本仓把它建模为 [Agent.cs](../../../Mud.Wechat.Work.DataModels/ProviderAuthentication/Agent.cs) 的
`IsSharedFromOtherCorp`（`bool`）字段，勿据此加端点。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddCorpGroupApi());

// 自建应用注入自建子接口（公共父接口 IsAbstract，不可注入）
public sealed class UpstreamSync(
    IWechatWorkInternalCorpGroupService corpGroup,
    IWechatWorkInternalCorpGroupContactsService chainContacts,
    IWechatWorkInternalCorpGroupRulesService chainRules)
{
    // 1) 拉取应用共享信息：limit ≤ 100，0/缺省 = 全量；用 next_cursor 续拉，ending == 1 表示拉完
    public async Task PullSharedAppsAsync(int upstreamAgentId, CancellationToken ct)
    {
        string? cursor = null;
        while (true)
        {
            var resp = await corpGroup.ListAppShareInfoAsync(new ListAppShareInfoRequest
            {
                AgentId = upstreamAgentId,      // int，官方必填
                BusinessType = 1,               // int?：0 企业互联/局校互联，1 上下游
                Limit = 100,                    // int?，最大值 100
                Cursor = cursor,                // string?，首次可不填
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/corpgroup/corp/list_app_share_info");

            foreach (AppShareCorpInfo corp in resp.CorpList ?? new List<AppShareCorpInfo>())
            {
                // corp.CorpId / corp.CorpName / corp.AgentId（int?）
            }

            if (resp.Ending == 1 || string.IsNullOrEmpty(resp.NextCursor)) break;
            cursor = resp.NextCursor;
        }
    }

    // 2) 批量导入上下游联系人：串行提交、同一时刻只允许一个任务；jobid 再轮询
    public async Task<string> ImportChainAsync(string chainId, CancellationToken ct)
    {
        var resp = await chainContacts.ImportChainContactsAsync(new ImportChainContactsRequest
        {
            ChainId = chainId,
            ContactList =
            [
                new ChainImportCorpItem
                {
                    CorpName = "示例下游企业",
                    GroupPath = "华东/一组",
                    CustomId = "1000000000000000001",   // string：64 位无符号整型口径，勿用 long
                    ContactInfoList =
                    [
                        new ChainImportContactItem
                        {
                            Name = "张三",
                            IdentityType = 2,            // 1 成员 / 2 负责人（单企业最多 5 人）
                            Mobile = "+8613800000000",
                        },
                    ],
                },
            ],
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/corpgroup/import_chain_contact");
        return resp.JobId!;                              // 之后用 GetChainImportResultAsync(jobId) 轮询
    }

    // 3) 新增对接规则：owner / member 两组范围各自「至少填一个」，id 以字符串传输
    public async Task<int> AddRuleAsync(string chainId, CancellationToken ct)
    {
        var resp = await chainRules.AddChainRuleAsync(new AddChainRuleRequest
        {
            ChainId = chainId,
            RuleInfo = new ChainRuleInfo
            {
                OwnerCorpRange = new ChainRuleOwnerRange { UserIds = ["manager0001"] },
                MemberCorpRange = new ChainRuleMemberRange { CorpIds = ["ww1234567890abcdef"] },
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/corpgroup/rule/add_rule");
        return resp.RuleId!.Value;
    }
}

// 第三方 / 代开发：先切到授权企业作用域，再调用（令牌解析为该 authCorpId 的企业级 access_token）
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    var shared = await thirdPartyCorpGroup.ListAppShareInfoAsync(
        new ListAppShareInfoRequest { AgentId = agentIdOfUpstreamApp }, ct);
    WechatWorkException.ThrowIfFailed(shared, "/cgi-bin/corpgroup/corp/list_app_share_info");
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域各端点 QPS 之外的全局频控配额 | 接口注释只留档 #4/#5/#6 的三档频率、#19/#20 的「每天 1000 次」与 #11 的四道容量闸；#1/#2/#3/#7~#10/#12~#18 的频率数值**未逐项核验** | 逐页核验后补接口 remarks，再更新本文速览备注 |
| `GetChainCorpInfoListAsync` 的 `limit` 上限 | 请求体只有 `Limit`（`int?`）与 `Cursor`（`string?`），本仓注释无最大值数字 ⇒ **未逐项核验** | 需要批次裁剪时先核验官方页，不得从邻域借数值 |
| `business_type` 取值全集 | 注释留档 `0 企业互联/局校互联、1 上下游企业`；其它值是否存在**未逐项核验** | 出现新取值先核验，再决定是否建枚举 |
| `ChainGroupInfo` / `GetChainGroupResponse` 的分组层级上限 | 本仓无注释数值 ⇒ **未逐项核验** | 需要建群深度校验时先核验官方页 |
| `qualification_status` = 3「已认证」的口径来源 | [GetChainCorpInfoResponse.cs](../../../Mud.Wechat.Work.DataModels/CorpGroup/ChainContacts/GetChainCorpInfoResponse.cs) 注释给出 1 未验证 / 2 已验证 / 3 已认证；「已认证」是否等价于「可调用导入/移除」的门槛**未逐项核验** | 不得把 `IsJoined` 与 `QualificationStatus` 合并推断为「可调用」判据 |
| #12 的状态机终态与超时 | 注释只有 1/2/3 三值与「只能查历史任务、并发 5」；任务留存时长与轮询建议间隔**未逐项核验** | 轮询节奏由宿主自定，勿在 SDK 侧加隐式重试 |
| `mass_call_ticket` 的获取端点 | 本域只消费该参数，票据本身由邻域接口签发 ⇒ 获取面归属**未在本域核验** | 需要签发链时查 `ExternalContact` 邻域，勿在本域加端点 |
| 代开发侧 `97441` / `97442` | CG5 注释记录：官方权限说明提及代开发，但代开发文档树无对应页 ⇒ 本仓按需求清单落自建子接口，代开发是否实际可调**未逐项核验** | 若官方补页，须同批改 CG1/CG5/CG6 与本文 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（含 ≤100 / ≤1000 / 200 / 2000 / 20000、三档频率、90 天、2023-12-01 口径） | 本目录 10 份接口文件的 XML 注释 |
| 基础族路由 / 计数（恰 6）与第三方不继承中间父接口 | `WechatCorpGroupContractGuards.cs` CG1（L53–L87，含 `Routes` 表）、CG2（L89–L196） |
| 通讯录族路由 / 计数（恰 9）、`getresult` 拼写告警 | CG5（L277–L316，含 `ContactsRoutes` 表）、CG6（L318–L355） |
| 规则族路由 / 计数（恰 5）、主企业口径 | CG9（L415–L445，含 `RuleRoutes` 表）、CG10（L447–L474） |
| `AccessToken` + Query + `access_token` 绑定 | CG3（L214–L241）、CG7（L357–L380）、CG11（L476–L513） |
| JSON 上下文登记（CorpGroup 15 型 / ChainContacts 23 型 / Rules 11 型）与同名 DTO 分裂 | CG4（L243–L274）、CG8（L382–L412）、CG11（L498–L513） |
| 类型化面计数用的继承链收集辅助 | `CollectInterfaceEndpoints`（L198–L212） |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work.DataModels/CorpGroup/](../../../Mud.Wechat.Work.DataModels/CorpGroup/)（`RequestModel/`、`ChainContacts/`、`Rules/`） |
| 三个上下文源生成文件 | `../../../Mud.Wechat.Work.DataModels/Generated/` 下 `CorpGroupJsonContext.g.cs` / `ChainContactsJsonContext.g.cs` / `RulesJsonContext.g.cs` |
| Query 令牌注入白名单（G5）与令牌归属键（TO 系列） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L135–L137） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号与限制，**同批**改
   [WechatCorpGroupContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCorpGroupContractGuards.cs)
   的 `Routes`（CG1，6）/ `ContactsRoutes`（CG5，9）/ `RuleRoutes`（CG9，5）三张表与各自的 `HaveCount` +
   `Distinct` 双断言；改开放面须同时改 CG2 / CG6 / CG10 的声明面计数与类型化面计数。
2. 官方若给第三方补 #2–#6 的文档页 → 先核验，再让第三方子接口改继承中间父接口，并同批改 CG2（含
   「第三方不得继承中间父接口」的反向断言、类型化面 1 → 6）与
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
   的 G5 白名单接口名清单（守卫注释已写明「新增应用类型须先核对官方文档并同批调整 CG2 与 G5」）。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`，`SerializerClassName` 取所在子族对应的
   `CorpGroup` / `ChainContacts` / `Rules`（三者是**三个**上下文）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 CG4 / CG8 / CG11 的类型清单与 [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L135–L137）。
   同名 DTO 不得落进同一上下文（CG4 注释：与客户联系域同名型分属 `CorpGroupJsonContext` 与 `CustomerJsonContext`）。
4. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员与 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddCorpGroupApi()` XML（其端点叙述须与本域计数一致）、
   可注入子接口的 `RegistryGroupName = "CorpGroup"`（父接口保持空组）。
5. 凭据面改动一律不得引入真值：`access_token`、`gettoken` 应答凭证、`mass_call_ticket`、`permanent_code`
   只写参数名；新增敏感参数名须同批登记脱敏词表（`SensitiveUrlKeys.Register` / G7）。
6. 本文与 [../../README.md](../../README.md) 的域内端点数、官方文档号表述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
