# Mud.Wechat.Work.Callback

企业微信 SDK **回调接收包**：`suite_ticket` / 授权事件推送的验签、AES 解密、事件分发与 `suite_ticket` 仓储。

## 内容

- `WechatCallbackReceiver` / `WechatCallbackHandler`：回调入口、验签与解密。
- `WechatCallbackCrypto`：企业微信回调 AES 解密。
- `WechatCallbackEvent`：事件模型与分发。
- `IWechatCallbackReplayGuard` / `InMemoryWechatCallbackReplayGuard`：抗重放一次性指纹去重。
- `WechatCallbackOptions`：回调配置（`CorpId` 语义为「接收方 ID」——企业自建回调为企业 `CorpId`，套件回调为 `SuiteId`）。
- `WechatCallbackServiceCollectionExtensions`：DI 注册入口。

## 抗重放不变量

验签通过后必须过两道 fail-closed 闸：

1. 时间戳时效窗口 ±300s（缺失/非数字即拒）；
2. 一次性指纹去重（SHA1 指纹，不落盘密文本身）。

多实例部署时须由宿主提供 `IWechatCallbackReplayGuard` 的分布式实现（`TryAdd` 前置注册覆盖），否则重放窗口失效。

## 授权自动化解耦

本包不引用主包 `Work`。`change_auth` / `cancel_auth` 事件经 `IServiceProvider.GetService<IWechatAuthorizationCoordinator>()` 惰性可选解析：未安装主包授权模块时首次 `Warning` 后降级不抛。`cancel_auth` 清理范围恒为 `SuiteId` 命中集，未命中只告警不删库。

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`
