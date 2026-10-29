# Mud.Wechat · SDK 开发文档

七线共仓的微信生态 .NET SDK。本目录是**面向调用方**的开发文档；仓库内的域 README（`Src/**/Interfaces/**/README.md`）与 `.docs/` 设计方案面向维护者，两者视角不同、互不取代。

规范见 [Documents/_spec/文档编写规范.md](../_spec/文档编写规范.md)。

## 从哪读起

| 你是 | 先读 |
|---|---|
| 第一次接入 | `guide/快速开始.md`（未成文）→ 你所在产品线的 `reference/<线>/` |
| 要调某个具体接口 | 按源码路径直接映射到 `reference/`（见下）→ 该族的端点小节 |
| 排查凭据/令牌问题 | `guide/配置与凭据.md`（未成文）+ 该族文档的 §2 前置条件、§6 失败处理 |
| 想知道某域有哪些端点 | 该域的源码 `Interfaces/<域>/README.md`（域总览，端点速览表在此，不重抄） |

## 路径映射

```
Src/<线>/<主包>/Interfaces/<域>/<族>/   ⇄   reference/<线>/<域>/<族>.md
```

例：`Src/Work/Mud.Wechat.Work/Interfaces/AccountId/Bot/` ⇄ [reference/Work/AccountId/Bot.md](./reference/Work/AccountId/Bot.md)

段名照抄源码目录，不翻译不改名。因此**从源码目录能直接推出文档路径，反之亦然**，覆盖度可脚本核对。

## 覆盖进度

端点数与业务域数取仓库 [README.md](../../README.md) 的「七条产品线，一个底座」表口径（该表未统一计量单位：`Work` 记契约接口数、`Channels` 记已落地端点数），此处只作规模参考；**逐端点的权威计数一律以各域契约守卫为准**。

| 产品线 | 主包 | 业务域 | 端点/契约接口 | 已成文 | 已核验 |
|---|---|---|---|---|---|
| 企业微信（WeCom） | `Mud.Wechat.Work` | 35 | 446 | **7 族（`AccountId` 全域）** | 6 |
| 微信公众号 | `Mud.Wechat.OfficialAccount` | 27 | 194 | 0 | 0 |
| 微信小程序 | `Mud.Wechat.MiniProgram` | 21 | 141 | 0 | 0 |
| 微信支付 APIv3/v2 | `Mud.Wechat.Pay` | 10 | 57 | 0 | 0 |
| 微信开放平台 | `Mud.Wechat.OpenPlatform` | — | 4 | 0 | 0 |
| 微信小店 / 视频号 | `Mud.Wechat.Channels` | 27 | 309 | 0 | 0 |
| 腾讯广告（在建） | `Mud.Wechat.Ads` | 8 | 33 | 0 | 0 |
| 跨线基建（叶层 / Redis / OpenTelemetry / AI） | `Mud.Wechat.Abstractions` 等 | — | — | 0 | 0 |

已成文 7 篇（`AccountId` 域 7 个接口族全覆盖，20 支端点）/ 待写按族计约 **200+ 篇**（族数量级，非端点数量级；确切缺口由 `reference/` 与 `Interfaces/` 目录比对得出，不靠人工记清单）。

### 已成文清单

| 文档 | 端点 | 令牌族 | 状态 |
|---|---|---|---|
| [IdConvert.md](./reference/Work/AccountId/IdConvert.md) | 11 | `access_token` | `已核验` |
| [Interop.md](./reference/Work/AccountId/Interop.md) | 3 | `access_token`（自建） | `已核验` |
| [Bot.md](./reference/Work/AccountId/Bot.md) | 1 | `provider_access_token` | `已核验` |
| [Corpid.md](./reference/Work/AccountId/Corpid.md) | 1 | `provider_access_token` | `已核验` |
| [Migration.md](./reference/Work/AccountId/Migration.md) | 2 | `provider_access_token` | `待补充`（`agentid` 必填性内部证据矛盾） |
| [TmpExternalUserId.md](./reference/Work/AccountId/TmpExternalUserId.md) | 1 | `access_token`（三类应用） | `已核验` |
| [ChatIdUpgrade.md](./reference/Work/AccountId/ChatIdUpgrade.md) | 1 | `suite_access_token` | `已核验` |

`已核验` = 正文每个断言可指到内部证据或带日期官方取文 + 示例逐字编译通过（见规范 §7）；各篇 §7「已知留档缺口」登记的是官方未提供或本文不主张的项，不因缺口表存在而降级。

推进顺序建议：**先按线补 `guide/`（凭据模型各线不同，重复解释成本最高）→ 再按域补 L3（一次一个域，与该域守卫同批评审）→ 最后统一 `appendix/`**。

## 本项目的特殊约束（写文档前必须知道）

- **域 README 已是权威端点速览**：L3 文档不重抄速览表，只链接过去（规范 §3 的 L4 由源码目录 README 承担）。
- **契约守卫是路由与计量的事实来源**：`Tests/**/ContractGuards/`，改契约面必须同批改守卫与文档。
- **接口 XML 注释里的官方文档号只作定位标识**，不作为事实来源；需要官方正文时必须渲染后取文并记日期（本项目官方站正文由 JS 渲染，直接抓 HTML 只会拿到导航）。
- **锚点不一致要留档不要静默改写**：见样版文档 §7「已知留档缺口」的处理方式。
- 文档改动不进 CHANGELOG；但**契约面变更未同批改文档视为半成品**（对齐 `AGENTS.md` §2 同批纪律）。
