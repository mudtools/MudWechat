# Demo 体系总览

本目录是 Mud.Wechat SDK 的**可照做示例集**。设计约束与逐 Demo 规格见
`.docs/MudWechat-Demo体系规划方案-v1.md`（本文只承载索引、凭据矩阵与运行顺序，不复述设计理由）。

## 1 工程清单

| 层 | 工程 | 用途 | 独立进程 | 状态 |
|---|---|---|---|---|
| L0 | `Mud.Wechat.Demo.Shared` | 环境变量校验、写操作闸、`/healthz` 装配、业务异常 → 502 映射闸 | 否（类库） | 已实施 |
| L0 | `Mud.Wechat.Demo.SelfTest` | 回调链路离线自测（验签/时效/解密/receiveid/抗重放/分发），`--channel work｜mp｜component｜channels｜bot` | 是（控制台） | 已实施 |
| L1 | `Mud.Wechat.Work.ContactDemo` | 企微通讯录读写 + 模块注册（DM-01） | 是 | 已实施 |
| L1 | `Mud.Wechat.Work.ContactCallbackDemo` | 企微通讯录变更回调 + 类型化处理器（DM-02） | 是 | 已实施 |
| L1 | `Mud.Wechat.Work.WebDemo` | 网页授权 + JS-SDK + 发消息（DM-03） | 是 | 已实施 |
| L1 | `Mud.Wechat.OfficialAccount.Demo` | 公众号授权/JS-SDK/模板消息/被动回复（DM-04） | 是 | 已实施 |
| L1 | `Mud.Wechat.MiniProgram.Demo` | 登录/小程序码/订阅消息（DM-05） | 是 | 已实施 |
| L1 | `Mud.Wechat.Pay.Demo` | Native/JSAPI 下单、通知、查询、退款（DM-06） | 是 | 已实施 |
| L2 | `Mud.Wechat.OpenPlatform.Demo` | 开放平台票据与授权编排（DM-09） | 是 | 已实施 |
| L2 | `Mud.Wechat.Channels.Demo` | 小店商品读取 + JSON 回调（DM-10） | 是 | 已实施 |
| L2 | `Mud.Wechat.AI.AgentDemo` | 智能机器人 + Agent + 工具面（DM-11，回调 / 长连接两形态） | 是 | 已实施 |
| L2 | `Mud.Wechat.Ads.Demo` | 腾讯广告 OAuth2 授权链 + 客户/日报只读（DM-13，本线**无回调面**，唯一非微信凭据模型的一条线） | 是 | 已实施 |
| L3 | `Mud.Wechat.AllInOne.Demo` | 五线同宿主共存：企微（XML + 机器人 JSON）/ 公众号 / 支付 / 小店 / 开放平台（DM-12，**零业务端点**） | 是 | 已实施 |

**共 13 个工程**（L0 两 + L1 六 + L2 四 + L3 一），逐工程都有 `README.md`；
其中基座自身的契约（四部件职责、使用纪律、为何不并进 SDK）在 `Mud.Wechat.Demo.Shared/README.md`。

**门禁定位**：`Demos/*` 全部 `IsPackable=false`，不进 AOT strict（`verify-build.ps1` 步骤 2 只扫 `Src/`）、
不进测试（步骤 3 只扫 `Tests/`）、不进 `pack.bat`/`publish.bat` 产物；
但已在 `Mud.Wechat.slnx` 的 `/Demos/` 组内，故**步骤 1 的全量 Release 编译覆盖它们** ——
「Demo 编译即验证 SDK 公开面可用」是体系的第一价值，破坏它等于让示例烂掉。

## 2 凭据矩阵

变量名统一 `MUDWECHAT_DEMO_` 前缀（与真实部署的环境变量天然隔离），由 `DemoEnv` 读取。
**占位符（含 `<` `>` 或 `your-`）等同于缺失** —— 目的是防止把模板里的占位值当成真凭据发出去。

| 变量 | 消费者 | 缺失后果 |
|---|---|---|
| `MUDWECHAT_DEMO_WORK_CORP_ID` | DM-01 / DM-02 / DM-03 / DM-11 / SelfTest | 启动 fail-fast |
| `MUDWECHAT_DEMO_WORK_AGENT_ID` | DM-03 | 启动 fail-fast |
| `MUDWECHAT_DEMO_WORK_AGENT_SECRET` | DM-01 / DM-02 / DM-03 / DM-11 | 启动 fail-fast |
| `MUDWECHAT_DEMO_WORK_CALLBACK_TOKEN` | DM-02 / SelfTest | 启动 fail-fast |
| `MUDWECHAT_DEMO_WORK_CALLBACK_AES_KEY` | DM-02 / SelfTest | 启动 fail-fast（长度须 43） |
| `MUDWECHAT_DEMO_MP_APP_ID` / `_MP_APP_SECRET` | DM-04 | 启动 fail-fast |
| `MUDWECHAT_DEMO_MP_CALLBACK_TOKEN` / `_MP_CALLBACK_AES_KEY` | DM-04 / SelfTest（`--channel mp`） | 启动 fail-fast（AESKey 长度须 43） |
| `MUDWECHAT_DEMO_MP_TEMPLATE_ID` | DM-04 | **可选**，未设置时模板消息端点返回配置提示 |
| `MUDWECHAT_DEMO_WXA_APP_ID` / `_WXA_APP_SECRET` | DM-05 | 启动 fail-fast |
| `MUDWECHAT_DEMO_WXA_TEMPLATE_ID` | DM-05 | **可选**，未设置时订阅消息端点返回配置提示 |
| `MUDWECHAT_DEMO_PAY_MCH_ID` / `_PAY_SERIAL_NUMBER` / `_PAY_APP_ID` / `_PAY_API_V3_KEY` | DM-06 | 启动 fail-fast |
| `MUDWECHAT_DEMO_PAY_PRIVATE_KEY` 或 `_PAY_PRIVATE_KEY_PATH` | DM-06 | 启动 fail-fast（二选一；裸 Base64 会被补成 PKCS#8 PEM） |
| `MUDWECHAT_DEMO_PAY_NOTIFY_BASE_URL` | DM-06 | **可选**，留空则按请求 Host 推导 `notify_url` 并在响应里提示「本机地址收不到官方通知」 |
| `MUDWECHAT_DEMO_CHANNELS_APP_ID` / `_CHANNELS_APP_SECRET` | DM-10 | 启动 fail-fast（`AppId` 同时是回调解密明文尾部 `appid` 的校验基准） |
| `MUDWECHAT_DEMO_CHANNELS_CALLBACK_TOKEN` / `_CHANNELS_CALLBACK_AES_KEY` | DM-10 / SelfTest（`--channel channels`） | 启动 fail-fast（AESKey 长度须 43；小店**强制安全模式**，无明文可选） |
| `MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_ID` / `_COMPONENT_APP_SECRET` | DM-09 | 启动 fail-fast |
| `MUDWECHAT_DEMO_OPENPLATFORM_TOKEN` / `_OPENPLATFORM_AES_KEY` | DM-09 / SelfTest（`--channel component`） | 启动 fail-fast（AESKey 长度须 43；「授权事件接收 URL」的验签与解密凭据） |
| `MUDWECHAT_DEMO_WORK_BOT_PUSH_TOKEN` / `_WORK_BOT_PUSH_AES_KEY` | DM-11（`AI_CHANNEL=callback`）/ SelfTest（`--channel bot`） | 启动 fail-fast（AESKey 长度须 43）；机器人**回调形态**的凭据，落 `WechatCallbackOptions.Apps`（`Channel = Bot`，`ReceiveId` 必须留空） |
| `MUDWECHAT_DEMO_WORK_BOT_ID` / `_WORK_BOT_SECRET` | DM-11（`AI_CHANNEL=long`） | 启动 fail-fast；机器人**长连接形态**的凭据，落 `WechatBots:Bots:{key}`（官方 101463：与回调地址二选一，同键配两套 ⇒ 启动期 fail-fast） |
| `MUDWECHAT_DEMO_AI_CHANNEL` | DM-11 | **可选**，`callback`（缺省）/ `long`；其它值启动即抛（防「拼错开关 ⇒ 演示错形态」） |
| `MUDWECHAT_DEMO_AI_CONNECT` | DM-11 | **可选**，long 形态下 `=1` 才真正建连（缺省只装配 + 走启动期三闸，不产生重连日志洪水） |
| `MUDWECHAT_DEMO_ADS_CLIENT_ID` / `_ADS_CLIENT_SECRET` | DM-13 | 启动 fail-fast；腾讯广告**应用级**凭据（`client_id` 官方标 integer，配置面按字符串承载：全数字 + ≤19 位，不截 long） |
| `MUDWECHAT_DEMO_ADS_REDIRECT_URI` | DM-13 | **可选**（官方列选填）；官方**不支持指定端口号** ⇒ 本机地址收不到回跳，缺省时换码须显式传参 |
| `MUDWECHAT_DEMO_ADS_APP_KEY` / `_ADS_SCOPE` / `_ADS_ACCOUNT_TYPE` | DM-13 | **可选**，缺省 `default` / `ADS_MANAGEMENT` / 不带该参数（官方枚举值集本线不复制，纯透传） |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 全部 | **可选**，见 §3 |
| `MUDWECHAT_DEMO_USE_REDIS` / `_REDIS_SERVER_ADDRESS` / `_REDIS_PASSWORD` | DM-12 | **可选**基建。`=1` 时 `AddWechatRedis` 必须早于任何应用注册（顺序错 ⇒ `TryAdd` 静默失效）；本机没起 Redis 时在连接期 fail-fast，属预期 |
| `MUDWECHAT_DEMO_OTLP_ENDPOINT` | DM-12 | **可选**，存在才装 `AddWechatOpenTelemetry`（不静默用默认 `localhost:4317` 去连一个不存在的收集器） |
| `MUDWECHAT_DEMO_ALLINONE_WRONG_ORDER` | DM-12 | **可选**，`=1` 且 `USE_REDIS=1` 时**故意**颠倒注册顺序，让 SDK 的注册期 `InvalidOperationException` 当场可见 |

**广告线刻意没有 `MUDWECHAT_DEMO_ADS_ACCOUNT_ID`**：`account_id` 来自 `oauth/token` 应答的 `authorizer_info`、随令牌落库，
做成配置项就允许「配置说 A 账号、令牌其实属于 B 账号」这种非法状态（与 `WechatAppConfig` 不提供独立 `TemplateId` 同一思路）。

`DemoEnv.RequireAll(...)` 会**一次性列出全部缺失项**再抛，而不是碰到第一个就退出 ——
配置一次就能对齐，不必反复启动试错。错误消息末尾固定指向本文件。

**唯一例外是 DM-12**：它按「凭据束」读取（`CredentialSet.Read`），**缺束的线整线不装、进程照样起来**。
理由是共存这件事必须在只有部分线凭据的开发机上可验证 —— 一个引用六线 SDK 的宿主若要求全部凭据齐备，
等于没人会真去跑它。逐线缺失只在 `/routes` 台账里留一行原因。

## 3 写操作闸

`MUDWECHAT_DEMO_ALLOW_WRITE=1` 是唯一放行写操作的开关：

- 未设置时，创建成员 / 发消息 / 群发 / 建菜单 / 退款这类端点**根本不映射路由**（不是返回 403），
  启动日志打印「写操作已关闭」；
- 设置后启动日志打 **Warning**「写操作已启用」，提醒当前进程会向真实企业写数据；
- 闸的实现只有一句 `DemoEnv.Flag("ALLOW_WRITE")`（只认 `"1"`），刻意保持可被 grep 到 ——
  任何 Demo 里出现绕过该闸的写端点都属体系缺陷。

**为什么默认关闭**：Demo 常被直接 `dotnet run` 到真实测试企业上，
通讯录写操作（尤其覆盖式更新）与退款不可逆，默认关是成本最低的防线。

## 4 运行顺序

```bash
# ① 全量编译（无需任何凭据）
dotnet build Mud.Wechat.slnx -c Release

# ② 无凭据 fail-fast 自检：故意不带环境变量启动，应列出全部缺失变量后退出
dotnet run --project Demos/Mud.Wechat.Work.ContactCallbackDemo --no-build -c Release

# ③ 配好 §2 的 work 变量，起只读 Demo 并探测
ASPNETCORE_URLS=http://127.0.0.1:56550 \
  dotnet run --project Demos/Mud.Wechat.Work.ContactDemo --no-build -c Release
curl http://127.0.0.1:56550/healthz

# ④ 起回调 Demo（另一个端口），跑离线自测验证整条链路
ASPNETCORE_URLS=http://127.0.0.1:56534 \
  dotnet run --project Demos/Mud.Wechat.Work.ContactCallbackDemo --no-build -c Release
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -c Release -- \
  --channel work --base-url http://127.0.0.1:56534

# ⑤ 公众号线同形：起 DM-04 后跑 mp 通道（变量名见 §2）
ASPNETCORE_URLS=http://127.0.0.1:56554 \
  dotnet run --project Demos/Mud.Wechat.OfficialAccount.Demo --no-build -c Release
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -c Release -- --channel mp

# ⑥ 小程序线：本线无回调包 ⇒ 不接 SelfTest，用假凭据探测「装配 + 错误码回显」即可
ASPNETCORE_URLS=http://127.0.0.1:56556 \
  dotnet run --project Demos/Mud.Wechat.MiniProgram.Demo --no-build -c Release
curl http://127.0.0.1:56556/healthz
curl -X POST http://127.0.0.1:56556/api/session \
     -H "Content-Type: application/json" -d '{"jsCode":"REPLACE_WITH_WX_LOGIN_CODE"}'
# 期望 502 + {"errorCode":40013,...}：说明请求确实走到了 api.weixin.qq.com

# ⑦ 支付线：无 access_token，凭据是商户私钥逐请求签名 ⇒ 需要一份自签/测试私钥才能起进程。
#   SelfTest 未覆盖本线：回调验签依赖**平台证书**签名，本机无法离线伪造（见规划方案 R-3）。
ASPNETCORE_URLS=http://127.0.0.1:56558 \
  dotnet run --project Demos/Mud.Wechat.Pay.Demo --no-build -c Release
curl http://127.0.0.1:56558/healthz
# 假商户序列号下的期望：502 + {"payErrorCode":"SIGN_ERROR",...}
# —— 权威码是**字符串**（APIv3 无整数 errcode），故支付闸必须在通用闸之内注册（越靠内越先捕获）。
curl -X POST http://127.0.0.1:56558/pay/9999999999 -d '{}'
# 期望 403 {"code":"FAIL",...}：未登记的商户键 fail-closed，绝不回落默认商户

# ⑧ 开放平台线：凭证链起点（component_verify_ticket）只能由微信后台推送，本机用 SelfTest 构造即可，
#   故本线**不需要真实平台凭据也能验整条接收链**。
ASPNETCORE_URLS=http://127.0.0.1:56560 \
  dotnet run --project Demos/Mud.Wechat.OpenPlatform.Demo --no-build -c Release
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel component
# 15 项断言全过 = 合法票据 success / 篡改签名·时间戳越界·跨平台 receiveid 一律 fail /
#   authorized 事件后台账有码、消费后重放不再装码、unauthorized 清码 / 未知 InfoType 不静默 success。
# 注：本线失败**不回 4xx** —— 官方只认响应体是否逐字 "success"，故断言写在响应体上（见该 Demo README §6）。

# ⑨ 小店线：回调是官方强制 JSON + 安全模式 ⇒ 本机用 SelfTest 构造信封即可验整条接收链，不需要真实小店。
ASPNETCORE_URLS=http://localhost:56562 \
  dotnet run --project Demos/Mud.Wechat.Channels.Demo --no-build -c Release
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel channels
# 17 项断言全过 = GET 三参回显 echostr 密文原文 / POST 类型化载荷落台账 / 重放·篡改·超窗·跨属主 AppId
#   一律 403 + 空体 / 未接线事件 200 但台账标「兜底记录」/ 非 JSON 415 / PUT 405 / 未配置 AppKey 403。
# 注：本线三态又是第三种口径 —— 拒绝 = 403 + **空体**（不给「验签是否通过」的探测面），
#   与企微（403 带拒绝语义）、开放平台（响应体 success/fail）都不同，别照搬断言。
# 假凭据下 /api/token、/api/shop、/api/products 均回 502 + {"errorCode":40013}；
#   写闸关闭时 POST /api/products/{id}/listing 为 404。

# ⑩ AI 线（DM-11）：机器人两种 API 形态官方强制二选一，故同一工程用 AI_CHANNEL 切档。
#   callback 档可完全离线（SelfTest 用 SDK 自身密码学内核构造 JSON 安全模式报文）：
ASPNETCORE_URLS=http://localhost:56564 \
  MUDWECHAT_DEMO_AI_CHANNEL=callback \
  dotnet run --project Demos/Mud.Wechat.AI.AgentDemo --no-build -c Release
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel bot
# 21 项断言全过 = GET 回显**解密后的明文**（Bot 侧 echo 不是 echostr 原文）/ text 与 feedback_event
#   回 200 + 加密空包、enter_chat 回 text / 重放·篡改·超窗·缺 msgid·明文 receiveid 非空一律 403 /
#   非 JSON 415 / PUT 405 / 空体 400 / JSON 打到通配路由（Channel=App）403。
# 注：本线成功路径**恒 200**（软超时与「无人接线」都是 200 + `{}`，不是 XML 侧的 503），
#   差别只在 /api/bot-events 台账里可见 —— 这是 Bot 通道的观测盲区，别用 XML 侧的三态直觉去断言。
curl http://localhost:56564/demo/tools   # enabledTools / registryTools / modelVisible 三段应各 3 项
curl "http://localhost:56564/demo/turn?question=查一下部门%201%20的名称"   # 离线回合，替身模型真发起工具调用

#   long 档：默认只装配不建连（Development 下 ValidateOnBuild 会校验 hosted service 的构造依赖）
MUDWECHAT_DEMO_AI_CHANNEL=long MUDWECHAT_DEMO_WORK_BOT_ID=aibot…… MUDWECHAT_DEMO_WORK_BOT_SECRET=…… \
  ASPNETCORE_URLS=http://localhost:56564 dotnet run --project Demos/Mud.Wechat.AI.AgentDemo --no-build -c Release
#   → 日志「智能机器人 default 未启用长连接…跳过建连」；再加 MUDWECHAT_DEMO_AI_CONNECT=1 才看到
#     夺租约 → aibot_subscribe → 10s 无应答 → 1s/2s/4s 指数退避的完整循环（假凭据下属预期）。

# ⑪ 多线同宿主（DM-12）：五线回调链共存一个进程，**不挂业务端点**。
#   与 ①~⑩ 最大的差别是凭据语义：缺束的线整线跳过、进程照样起来（见 §2 末尾的「唯一例外」）。
#   零凭据起进程本身就是这条 Demo 的第一项验证：
dotnet run --project Demos/Mud.Wechat.AllInOne.Demo --no-build -c Release   # /healthz 里五线全 inactive
#   再按 Demos/Mud.Wechat.AllInOne.Demo/README.md §3 的假值 export 段配齐五束凭据后重启，跑五条探针：
B=http://localhost:56566
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel work      --base-url $B --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel mp        --base-url $B --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel channels  --base-url $B --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel bot       --base-url $B --route /wechat/bot --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel component --base-url $B --health-fragment all-in-one
#   实测（假凭据、不出网）：work 4/4、mp 5/5、channels 17/17、bot 20/21、component 10/15。
#   两处未过都是**设计边界**不是缺陷：bot 那条「JSON 打到通配路由期望 403」在本宿主实际 404
#     （DM-12 刻意不注册通配键 "*" —— 注册了它就会劫走同前缀的 /wechat/component）；
#     component 那五条落在 DM-09 专属的授权码编排端点上，DM-12 不挂业务端点。
#   Git Bash 必须加 MSYS_NO_PATHCONV=1：否则 --route /wechat/bot 被改写成 C:/Program Files/Git/... ，
#   探针会以 file:// scheme 报错，看起来像 SDK 故障、实为 shell 路径转换。
#   支付线无探针通道（验签依赖平台证书，离线伪造不了），可验证面只有路由归属：
curl -s -o /dev/null -w '%{http_code}\n' -X POST -H 'Content-Type: application/json' -d '{"id":"x"}' $B/pay/1900000101   # 403

# ⑫ 广告线（DM-13）：唯一非微信凭据模型的一条线（OAuth2 双令牌 + 授权/业务两台主机 + {code,message,message_cn,data} 信封）。
#   本线**无回调面** ⇒ 不接 SelfTest；离线可验证面是「装配 + 复合参数编码 + 错误语义分流」：
export MUDWECHAT_DEMO_ADS_CLIENT_ID=1234567890 MUDWECHAT_DEMO_ADS_CLIENT_SECRET=fake-secret-for-boot
ASPNETCORE_URLS=http://localhost:56568 \
  dotnet run --project Demos/Mud.Wechat.Ads.Demo --no-build -c Release
curl -s http://localhost:56568/api/advertisers?pageSize=10      # 期望 401 + errorCode=-1（库里无授权状态 ⇒ 需重新授权，非官方报错）
curl -s -X POST http://localhost:56568/api/auth/exchange -H 'Content-Type: application/json' -d '{}'   # 期望 400 缺 code
# 假 code 真出网时经专属闸回 502 + 官方 code（实测 11011 Authorization code is malformed），证明令牌链与信封解析整条走通；
# 写闸关闭时 POST /api/auth/refresh 为 404 —— 刷新会**作废旧的两支令牌**，按不可逆动作管，不当读操作放行。
```

`--channel` 已决定默认端口与回调路由，⑤ 不带 `--base-url` 也能跑；
带 `--base-url` 的写法只在需要换端口或跨机器时才有必要。

端口按各 Demo 的 `launchSettings.json` / README 为准；同一台机器并行跑多个 Demo 时靠端口区分。

## 5 安全须知

1. **不得使用生产账号**。凭据指向测试企业/测试商户；`ALLOW_WRITE` 只对自己拥有的测试环境开启。
2. **密钥不入库**：真实值只走环境变量或本机 User Secrets。仓库里只允许出现变量名与占位符。
3. **不得进日志**：`AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` /
   `suite_ticket` / 回调 `PushToken` / `EncodingAESKey` / 私钥 / APIv3 密钥，以及解密明文 `DecryptedXml`
   与支付回调 `resource` 明文（含 `openid`，红线 PAY-B7），
   一律不得打印、不得进遥测、不得写进异常消息。`/healthz` 只回显**非敏感配置摘要**（AppKey、模块名、路由、闸状态）。
4. **不得为跑通 Demo 而削弱 SDK 契约**：回调两道 fail-closed 闸（±300s 时效 + 一次性指纹）、
   官方 32 字节块 PKCS7、`receiveid` 校验、族级合法性闸都是安全边界；
   Demo 里遇到「校验不过」应改配置，而不是改 `Src/`。
5. **Demo 不改 `Src/` 与 `Tests/` 契约面**：需要新的公开面时先按 AGENTS.md §2 同批补守卫，再写示例。

## 6 新增 Demo 的落地清单

照既有工程形态复制，逐项对齐（缺一项即半成品）：

1. 工程名 `Mud.Wechat.{线}.{场景}Demo`，落 `Demos/<工程名>/`，**csproj 里不重复写工程属性**——
   `Demos/Directory.Build.props` 统一给单 TFM `net8.0`、Exe、关 AOT/trim、`IsPackable=false`
   （仅 L0 类库需覆盖 `OutputType=Library`）；
2. 引用 `Mud.Wechat.Demo.Shared` + 所需产品线包；回调 Demo 额外内嵌
   `Mud.Wechat.Callback.Analyzers`（让 MUDCB002~005 在示例里就生效，等于给分析器喂狗粮）。
   **例外有二：小店线与 AI 机器人通道刻意不内嵌** —— 分析器档位表 `WechatCallbackHandlerAnalyzer.Profiles` 只有企业微信与公众号两档
   （守卫 CB-L1f 锁定），小店档位缺失时内嵌只会**静默空跑**（0 诊断、无报错），给读者「接线已受编译期校验」的假象
   （见 `Mud.Wechat.Channels.Demo/README.md` §7）；智能机器人走独立 JSON 键集（`WechatBotEventTypes` 不进
   `OfficialPayloadContracts`），同样只制造假信号（见 `Mud.Wechat.AI.AgentDemo/README.md` §6.16）。
3. `Program.cs` 在任何 DI 装配**之前**调用 `DemoEnv.RequireAll(...)`（**唯一例外 DM-12**：它用 `CredentialSet.Read`
   按束读取，缺束整线跳过 ⇒ 共存命题才能在只有部分线凭据的开发机上被验证）；写端点包在 `if (DemoWriteGate.Enabled)` 内；
4. `app.MapDemoHealth("<demo 名>", new { 非敏感摘要 })` + `DemoWriteGate.Announce(app.Logger)`；
   HTTP 型 Demo 另在 `builder.Build()` 之后挂 `app.UseWechatApiErrorGate()` ——
   令牌链与手工二进制通道以 `WechatApiException` 承载业务错误，无此闸时一次 `invalid appid` 会返回带堆栈的 500；
   该闸还会拆开**组件负缓存的包装异常**（`Mud.HttpUtils` 的 `TokenManagerBase` 在负缓存窗口内把
   `WechatApiException` 放进 `InvalidOperationException.InnerException`）—— 内层是 `WechatApiException` 才映射 502，
   普通 `InvalidOperationException` 仍原样上抛（DM-10 实测：不拆开时，窗口内的第二次调用会绕过闸、
   由开发者异常页把 SDK 堆栈写进响应体）；
5. 工程加入 `Mud.Wechat.slnx` 的 `/Demos/` 组（否则步骤 1 覆盖不到，等于不设防）；
6. 写 `Demos/<工程名>/README.md`（目的/覆盖能力/前置条件/环境变量/运行/验证/官方文档/安全）；
7. 涉回调则把该线接入 `Mud.Wechat.Demo.SelfTest` 的 `--channel`，并验证
   「合法报文 200 / 重放 403 / 篡改签名 403」三态；
8. 跑 `dotnet build Mud.Wechat.slnx -c Release` 与 `scripts/audit-config-keys.ps1`。
