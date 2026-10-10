# Mud.Wechat.Redis

企业微信 SDK 的 **Redis 分布式存储扩展包**：为多实例分布式部署提供四个存储端口的 Redis 实现，
适用于多实例共享令牌、跨实例持久化企业授权（永久授权码）、多实例共享 suite_ticket 与回调抗重放。

对应 `Mud.Feishu.Redis` 的同构模块（连接基座 / 键构造护栏 / DI 编排模式一致，存储层按企业微信契约重设计）。

## 覆盖的存储端口

| 端口 | Redis 实现 | 多实例语义 |
|---|---|---|
| `IWechatTokenStore` | `RedisWechatTokenStore` | 令牌跨实例共享；实现 `IWechatTokenStoreBatchRemove`（W1/M10 批量删除，N 次往返收敛为 ⌈N/500⌉ 次） |
| `IWechatCorpAuthStore` | `RedisWechatCorpAuthStore` | 企业授权（含**不可重取**的永久授权码）跨实例持久化，重启不再丢授权 |
| `IWechatSuiteTicketStore` | `RedisWechatSuiteTicketStore` | suite_ticket 跨实例共享——不再只有收到推送的实例能签发套件令牌 |
| `IWechatCallbackReplayGuard` | `RedisWechatCallbackReplayGuard` | 回调抗重放窗口跨实例生效（进程内实现多实例下重放窗口失效） |

四个端口的默认实现均为进程内（`TryAddSingleton` 注册）。本包**先注册即胜出**：

```csharp
// Program.cs —— 完整装配：顺序即契约，Redis 必须最先
builder.Services
    .AddWechatRedis(builder.Configuration)                 // ① 连接基座 + 四个存储端口（默认注册健康检查）
    .AddWechatApp(builder.Configuration, "WechatApps")     // ② 令牌/授权/票据基座（Redis 实现已就位）
    .AddWechatCallback(o => { /* 回调凭据与处理器 */ });    // ③ 回调（抗重放窗口跨实例生效）

var app = builder.Build();
app.MapHealthChecks("/health");                            // 健康检查端点（唯一判据 PING，端点连通数仅作 data 呈现）

app.Run();
```

**调用顺序 = 契约**：颠倒顺序时 TryAdd 语义会让 Redis 实现**静默失效**（默认进程内实现仍然生效且无任何错误）。
`AddWechatRedis` 在注册期检测颠倒顺序并 fail-fast。宿主**自行预注册**的自定义实现按契约优先于本包
（TryAdd 语义），不受顺序守卫影响。

## 配置（配置节 `WechatRedis`）

```json
{
  "WechatRedis": {
    "Connection": {
      "ServerAddress": "localhost:6379",   // host:port / redis:// / rediss://（后者显式启用 TLS）
      "Password": "",
      "Ssl": false,
      "DefaultDatabase": null,
      "ConnectTimeout": 5000,
      "SyncTimeout": 5000,
      "ConnectRetry": 3,
      "AbortOnConnectFail": true
    },
    "Advanced": {
      "AllowAdmin": false,
      "ClientName": null                    // 兜底 Wechat-Redis-{MachineName}
    },
    "KeyPrefix": "wechat",                  // 四类键共享的唯一前缀旋钮；多环境隔离改此值
    "SuiteTicketTtl": "00:00:00"            // 默认不过期（覆盖写保新鲜）；正值建议 ≥ 20 分钟
  }
}
```

代码配置形态：`services.AddWechatRedis(o => o.Connection.ServerAddress = "localhost:6379");`

校验：`IValidateOptions<WechatRedisOptions>` + net6+ `ValidateOnStart()`（ns2.0 在首次解析 options 时触发）。

## 键空间

| 存储 | Redis 键 | TTL |
|---|---|---|
| 令牌 | `{KeyPrefix}:token:{storeKey}` | 跟随调用方 `expiresInSeconds`；`<= 0` 时**删键** |
| 企业授权 | `{KeyPrefix}:corpa:{appKey}:{authCorpId}` | 不过期（轮换/撤销显式删） |
| 套件票据 | `{KeyPrefix}:ticket:{suiteId}` | `SuiteTicketTtl`（默认不过期） |
| 回调重放 | `{KeyPrefix}:replay:{fingerprint}` | 调用方传入的窗口（±300s） |

键名仅含 appKey / authCorpId / suiteId / SHA1 指纹——**均非凭据**，可安全入日志。
令牌 `storeKey` 为三段式 `{tokenType}:{appKey}:{scopeKey}`（与 `InMemoryWechatTokenStore` 键空间逐字节一致，
`PurgeAppTokensAsync` 三段解析依赖此不变量）。

## 回调抗重放（fail-closed）

Redis 实现的 `TryMarkAsync` 在存储故障时**原样包装上抛**（`WechatRedisException`）——接收器无捕获面，
异常冒泡为回调 5xx，官方 96238 重试可重新进入管线。显式禁止吞异常返回 `true`（放行重放）或
`false`（静默丢事件且官方不再重试）。

## 健康 / 预热 / 生命周期

- **健康检查**：默认注册（`registerHealthCheck: false` 可关闭，仅注册类型由宿主自行 `AddCheck`）；
  唯一判据为 PING，端点连通数仅作 `data` 呈现。
- **启动预热**：`IHostedService` 启动期 PING；`AbortOnConnectFail=true`（默认）时 PING 失败终止启动
  （fail-fast），否则告警放行交由自动重连。
- **连接事件**：`ConnectionFailed` / `ConnectionRestored` 记录日志；连接初始化失败的异常消息只携带
  **脱敏后**的配置描述（Password 掩码）。

## AOT / Trim

- net8.0+ 支持 Native AOT 发布（`AotStrictMode=true` 门禁全绿；`StackExchange.Redis` 3.3.0 已实测）。
- 企业授权聚合序列化：net8+ 走 `AuthenticationJsonContext`（`JsonTypeInfo`，零反射）；
  ns2.0/net6 为反射 STJ（AOT 门禁仅对 net8.0 构建，低 TFM 做 AOT 非受支持场景）。
- 配置绑定走源生成器形状（`Configure<T>(o => section.Bind(o))`）；`IConfiguration` 入口标注
  `RequiresUnreferencedCode` / `RequiresDynamicCode`（与 `AddWechatApp` 同款）。

## 集成测试

真实 Redis 的端到端用例由环境变量门控（CI 默认不跑）：

```bash
WECHAT_REDIS_TESTS_CONNECTION=localhost:6379 dotnet test Tests/Mud.Wechat.Redis.Tests
```
