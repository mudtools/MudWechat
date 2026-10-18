# Advertiser 域 · 客户账号

腾讯广告 Marketing API v3.0「客户账号」域：广告主账号的**查询 / 创建 / 更新 / 批量改日预算**，共 **4 支端点**。
接口 [IWechatAdsAdvertiserService.cs](./IWechatAdsAdvertiserService.cs)，注册组 `Advertiser`，模块枚举 `AdsModule.Advertiser`。

> 本 README 只汇编**已在接口 XML 注释与契约守卫中留档**的事实。逐参数类型、数组长度、字节上限、条件必填的原文以
> [IWechatAdsAdvertiserService.cs](./IWechatAdsAdvertiserService.cs) 为准；路由 / 动词 / 端点计数的权威以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。
> 三者冲突时以守卫 + 接口注释为准并**就地修正本文**，不得反向弱化前两者。

## 快速开始

```csharp
// 凭据底座必须先于业务接口注册（缺失时 Build() 直接点名报错，不留到首调）
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddAdvertiserApi());   // 或 AddWechatAdsApi(AdsModule.Advertiser)

// 注入即调用：access_token / timestamp / nonce 由传输层成组注入，签名里没有它们
public sealed class AccountSync(IWechatAdsAdvertiserService advertisers)
{
    public async Task<IReadOnlyList<AdsAdvertiserInfo>> PullAsync(CancellationToken ct)
    {
        var resp = await advertisers.GetAsync(
            accountId: 1234567890,
            fields: AdsQueryJson.Fields("account_id", "corporation_name", "daily_budget"),
            paginationMode: "PAGINATION_MODE_NORMAL",
            pageSize: 100,
            page: 1,
            cancellationToken: ct);

        WechatAdsException.ThrowIfFailed(resp, "/v3.0/advertiser/get");   // 声明式面不自动抛，判错归调用方
        return resp.Data?.List ?? [];
    }
}
```

## 端点速览

| 方法 | 动词 | 路由 | 官方「所属权限」 | `user_token` | 幂等头 |
|---|---|---|---|---|---|
| `GetAsync` | GET | `/v3.0/advertiser/get` | `account_management,ads_management,ads_insights,audience_management,user_actions`（**五者任一**） | 不带 | 无 |
| `AddAsync` | POST | `/v3.0/advertiser/add` | 本仓**未逐页摘录**（见下方「已知留档缺口」） | **不带**（本页无「特定请求参数」表） | `X-Request-Id` |
| `UpdateAsync` | POST | `/v3.0/advertiser/update` | `account_management` | 显式可选 Query | 无 |
| `UpdateDailyBudgetAsync` | POST | `/v3.0/advertiser/update_daily_budget` | `ads_management` | 显式可选 Query | 无 |

动词形态是本域第一个易错点：**只有 `get` 是 GET**（官方 curl 用 `curl -G -d` ⇒ 参数进 Query、无请求体），
另三支均为 POST + `Content-Type: application/json`。写错动词不会有任何编译期或组件运行期检查抓到——
官方网关回的是「参数缺失 / 资源不存在」而不是「方法不允许」，现象与「账号没权限」难以区分，
所以路由表被硬编码钉进守卫 `ADS-B2`。

## 本域六条契约要点

### 1. 凭据一个都不出现在方法签名里

v3.0「全局参数」要求每个业务请求**同时**携带 `access_token` + `timestamp`（秒级，最大误差 300 秒，时区 GMT+8）
+ `nonce`（≤32 字符、全局唯一），三者必须**同源同次现取**。声明式 `[Token]` 只能注入其中一个、组不出合法请求
⇒ 本线一律不走 `[Token]`，组装在传输层 `AdsAuthorizationHandler`（守卫 `ADS-B1`）。

### 2. `fields` / `filtering` 只能是 `string`

官方 curl 把它们写成**单个**键值对、值为 JSON 字面量（`-d 'fields=[...]'`、`-d 'filtering=[{"field":…}]'`），
而组件对数组走「重复同名参数」、对复杂类型走「逐属性展平」，两种都不是官方线格式
⇒ 参数类型为 `string`，构造入口唯一：`AdsQueryJson.Fields(...)` / `AdsQueryJson.Filtering(...)`。

`filtering` 本页限制（官方原文）：数组长度**为 1**；`field` 只有 `corporation_name` 一支；
`operator` 取 `EQUALS` / `CONTAINS`；`values` 数组长度为 1、字段 1–120 字节。不传或传空 = 无限制条件。
SDK 不做本地长度拦截——官方示例自己就以 `fields=[]` 传空，拦截会挡掉合法请求。

### 3. `user_token` 只在这两页，且它是凭据

`update` 与 `update_daily_budget` 在「全局参数」之外另列 `user_token`（实名认证完成获取的令牌），
它是**逐请求**传入的操作者令牌、与账号级 `access_token` 不是一个东西 ⇒ 做成显式可选 Query 参数而非传输层注入。
该参数名不在组件脱敏词表内，由模块注册期登记为强制掩码键（守卫 `ADS-B5`）。**不得写进日志 / 遥测 / 异常消息。**

### 4. `X-Request-Id` 只存在于 `add`

官方原文：「资源请求的唯一 id，该请求头参数用于保证接口重试的幂等性……如果在重复传入相同请求参数和
`X-Request-Id`，那么会返回该 `X-Request-Id` 对应的唯一的投放资源」⇒ **本域唯一有官方幂等面的写端点**。
`update` / `update_daily_budget` 两页均无该请求头表，**不要**给它们补一个。网络重试必须复用同一值。

### 5. 日预算：六条硬约束，SDK 一条都不拦

`daily_budget` 单位为**分**（与 `advertiser` / `adgroups` / 报表域的金额字段同一口径，SDK 不做单位换算）：

1. 取值须介于 **5,000 分 – 4,000,000,000 分**（50 元 – 40,000,000 元）；设为 `0` 表示不设预算。
2. **每次修改幅度不能低于 5,000 分**；微信公众号平台小程序账户每次**提高**幅度不能低于 **50,000 分**。
3. 修改后不得**低于今日已消耗金额的 1.2 倍加上冻结金**。
4. 且不得**低于今日已消耗金额加上 5,000 分**。
5. 仅对竞价投放生效，合约投放不受影响。
6. `update_daily_budget` 页原文另明确：**允许**从不限预算（`0`）改为指定预算，也允许反向改为 `0`。

②③④ 的判定依赖当日消耗与冻结金（SDK 不可见）⇒ **不做本地拦截**，越界由应答 `code` 表达。

### 6. `update_daily_budget` 是双层失败语义（本域最易误判之处）

外层信封 `code == 0` **只表示请求被受理**。每一条的成败看 `data.list[i].code`，部分失败时外层仍为 `0`
且另有 `data.fail_id_list` ⇒ **判错必须两层都做**，只判外层会把「半数失败」当成全成功。
这也是本端点不能只靠单层 `ThrowIfFailed` 就返回的原因（接口 remarks 里逐字写明）。

请求体根字段只有 `update_daily_budget_spec`（必填，数组最大长度 100）。
另注意 `use_min_daily_budget`：官方原文承认可能出现「期望下调、实际上调」，实际生效值以应答元素的
`daily_budget` 为准，不要拿请求值当结论。

## 分页：两种模式同时存在，别只认一支

`advertiser/get` 的 `pagination_mode` **必填**，可选 `PAGINATION_MODE_NORMAL` / `PAGINATION_MODE_CURSOR`：

| 模式 | 请求侧 | 应答侧 | 注意 |
|---|---|---|---|
| NORMAL | `page`（1–1000，官方原文「普通翻页模式必填」）+ `page_size`（1–100） | `data.page_info` | `cursor` 不填 |
| CURSOR | `cursor` + `page_size` | `data.cursor_page_info` | 首次不填 `cursor`；游标有效期 **24 小时**；游标模式下除分页参数外的请求参数须与上一次调用**完全一致** |

两种分页元信息在官方 `data` 下**同时列出**、各自「按请求的模式返回」⇒ DTO 两支均建为可空，按自己请求的模式取对应一支。
游标模式的 `has_more` 官方原文「返回 false 表示已无下一页，此时**务必停止拉取**」。

`account_id`：直客客户必须填写，代理商可不填；代理商不填则返回代理商下**全部子客户**信息 ⇒ 参数可空。
`agency_id`：仅服务商主体请求时必填，其他 token 忽略该参数（条件必填，SDK 不预判）。

## `add`：企业 / 个人两套必填条件

表内**硬必填**只有 `system_industry_id` 与 `introduction_url`，其余按 `registration_type` 分企业 / 个人两套
条件必填口径（企业必填企业名称 / 执照号 / 资质图 / 所在地；个人仅用身份证明结构）。
官方以「【企业账号】必填」标注而非独立必填表 ⇒ SDK 不拦截，错配由应答 `code` 表达。
`data` 只回新建的 `account_id`。

## 官方留档缺口（改动前先补，别凭印象）

- **`advertiser/add` 的「所属权限」本仓未逐页摘录**（其余三页已摘录）。需要写进本表时须重新逐页核验官方页面，
  并把事实补进接口 remarks，**同批**更新本 README——不要在本文里先写一个未经核定的值。
- 官方 `advertiser/get` 页存在已照录的自相矛盾：`area_code` 字段表标 `integer` 而示例写字符串 `"110100"`；
  `account_id` 示例值为占位符 `<ACCOUNT_ID>`；`individual_qualification` 字段表两字段、示例四字段
  （DTO 按示例建为四字段超集，缺字段会让官方已返回的数据无处安放）。

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数类型 / 数组长度 / 字节上限 / 条件必填原文 | [IWechatAdsAdvertiserService.cs](./IWechatAdsAdvertiserService.cs) 的 XML 注释 |
| 路由 / 动词 / 端点计数（恰 4） | `AdsContractGuards` 的 `AdvertiserEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `AdvertiserRoutes` |
| 凭据不落签名 | `ADS-B1` |
| `user_token` 掩码登记 | `ADS-B5`、`AdsSensitiveQueryKeys` |
| 域名与 SSRF 白名单 | `Mud.Wechat.Ads/README.md`「域名与白名单」（后缀域 `e.qq.com`，守卫 `ADS-B4` / `AB-G9`） |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/advertiser/{add,get,update,update_daily_budget}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 注释补齐官方文档 URL + 频率上限 / 串行 / 覆盖删除 / 权限可见范围，**同批**更新
   `AdvertiserRoutes` 与端点计数断言（新增端点未入表即红，防「实现了但没锁」）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 恒取命名空间域段 `Advertiser`）→
   跑 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 更新守卫的上下文登记断言。
3. 新增 Query 凭据参数名 → 「补进脱敏词表」或「登记带追踪号的自过期豁免」二选一（`ADS-B5`）。
4. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
5. **不要**把本域签名「对齐」成仓内另一条经验：报表域就是三支 GET + 一支 POST，`hourly_reports/get` 就是没有
   `organization_id`；反向套用同样违规。
