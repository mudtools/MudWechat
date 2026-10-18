# Work.ContactDemo — 企业微信通讯录读写

## 目的

演示 Mud.Wechat 企业微信线最小闭环：多应用底座注册 → 通讯录模块注册 → 自建子接口注入 → 端点调用。
覆盖成员读取/创建、部门列表、标签列表四类端点，并示范 Demo 的凭据与写操作纪律。

## 覆盖的 SDK 能力

| 能力 | 类型 | 说明 |
|---|---|---|
| `AddWechatApp(Action<WechatAppConfig>)` | DI 装配 | 自建应用（`corpid` + `corpsecret`）令牌底座，`access_token` 全生命周期托管 |
| `AddWechatWorkServices(b => b.AddContactApi())` | 模块注册 | 通讯录六域（成员/部门/标签/查看权限/异步导入/异步导出） |
| `IWechatWorkInternalDepartmentsService` | 业务接口 | `GetDepartmentListAsync` |
| `IWechatWorkInternalUsersService` | 业务接口 | `GetUserAsync` / `GetUserSimpleListAsync` / `CreateUserAsync` |
| `IWechatWorkInternalTagsService` | 业务接口 | `GetTagListAsync` |
| `DemoEnv` / `DemoWriteGate` / `MapDemoHealth` | Demo 基座 | 环境变量校验、写操作闸、健康检查 |

> 父接口（`IWechatWorkUsersService` 等）`IsAbstract = true` 不注册 DI，注入必须用上面的自建子接口。
> 成员 DTO 命名空间为 `Mud.Wechat.Work.DataModels.Contracts.Users`（全仓唯一与目录名 `Contacts` 不一致的域）。

## 前置条件

- .NET 8 SDK；
- 一个**测试企业**（勿用生产企业），其中已创建自建应用或「通讯录同步」应用；
- 通讯录**写入**（`POST /api/members`）需要「通讯录同步」权限的 secret，普通自建应用 secret 会被官方拒绝。

## 环境变量

| 变量 | 必填 | 说明 |
|---|---|---|
| `MUDWECHAT_DEMO_WORK_CORP_ID` | 是 | 企业 CorpId（`ww...`） |
| `MUDWECHAT_DEMO_WORK_AGENT_SECRET` | 是 | 自建应用 / 通讯录同步应用 secret |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | 置 `1` 才映射创建成员端点 |

PowerShell 示例（密钥建议用本机用户机密或会话变量，不要写入配置文件）：

```powershell
$env:MUDWECHAT_DEMO_WORK_CORP_ID = "wwxxxxxxxxxxxxxx"
$env:MUDWECHAT_DEMO_WORK_AGENT_SECRET = "应用 secret"
```

## 运行

```bash
dotnet run --project Demos/Mud.Wechat.Work.ContactDemo
```

缺环境变量时在 `builder.Build()` 之前一次性抛出全部缺失项，不会带着占位符启动。

## 验证

```bash
curl http://localhost:56550/healthz                 # 配置摘要 + 写闸状态，不含凭据
curl http://localhost:56550/api/departments         # 部门列表
curl http://localhost:56550/api/members?departmentId=1
curl http://localhost:56550/api/members/<userid>
curl http://localhost:56550/api/tags
```

写操作（默认关闭，未开启时上面最后一条返回 404）：

```bash
$env:MUDWECHAT_DEMO_ALLOW_WRITE = "1"
curl -X POST http://localhost:56550/api/members \
     -H "Content-Type: application/json" \
     -d '{"userid":"demo_user","name":"Demo 成员","department":[1]}'
```

预期链路行为：首次调用自动经 `gettoken` 换取并缓存 `access_token`；命中失效码 `40014/42001/42007/42009`
时 SDK 自动失效重取，无需宿主干预。

## 安全注意

- 只使用测试企业；写操作会在企业内真实创建成员。
- `AgentSecret` 不进日志、不进异常消息（SDK 侧已有脱敏），Demo 也不打印它。
- `healthz` 只回显应用键、应用类型与模块名，不回显任何凭据。

## 官方文档

- 获取部门列表：<https://developer.work.weixin.qq.com/document/path/90208>
- 读取成员：<https://developer.work.weixin.qq.com/document/path/90196>
- 获取部门成员详情：<https://developer.work.weixin.qq.com/document/path/90201>
- 创建成员：<https://developer.work.weixin.qq.com/document/path/90195>
- 获取标签列表：<https://developer.work.weixin.qq.com/document/path/90216>
