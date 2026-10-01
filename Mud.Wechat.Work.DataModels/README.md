# Mud.Wechat.Work.DataModels

企业微信 SDK **数据模型包**（纯 DTO，无外部依赖）——官方接口出入参模型，独立交付以避免宿主引入不必要的运行时依赖。

## 内容

- 企业微信官方 API 的请求/响应 DTO。
- `WechatWorkJsonContext`：AOT 源生成 JSON 序列化上下文（`[JsonSerializable]` 登记，禁止运行时反射序列化）。

## 说明

- 本包不引用 `Mud.HttpUtils` 运行时，仅引用 `Mud.HttpUtils.Generator`（分析器，`PrivateAssets=all`）。
- 新增 `[HttpJsonSerializable]` 领域模型必须同步登记到 `Abstractions/Authentication/Models/AuthenticationJsonContext.cs`（若属 Abstractions 域），否则 `AotStrictMode=true` 门禁下 `AOT006` 会失败。
- DTO 均走 `JsonTypeInfo` 源生成序列化，勿以反射方式调用 `JsonSerializer.Serialize<T>/Deserialize<T>`。

## 依赖

无运行时依赖（仅源生成器）。
