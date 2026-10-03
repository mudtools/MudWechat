// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.ContactCallbackDemo.Handlers;

// 企业微信通讯录变更回调 Demo：
// 演示「验签 → AES 解密 → 事件解析 → 类型化处理器分发」的完整接收链路。
// 说明：
//   1) AddWechatApp 提供令牌/仓储底座（AddWechatCallback 的 TryAdd 仓储依赖此项），必须先注册。
//   2) 通讯录变更事件走「应用数据通道」（Channel = App），自建应用 ReceiveId = 企业 CorpId。
//   3) 回调 URL = https://<host>/wechat/default（AppKey 与路由 /{前缀}/{AppKey} 一致）。
//   4) 下方 <...> 为占位符，运行前替换；密钥禁止提交仓库，建议走环境变量 / 用户机密。

var builder = WebApplication.CreateBuilder(args);

// ① 令牌与多应用底座（自建应用；回调仓储 TryAdd 依赖此项）。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "default";
    o.AppType = WechatAppType.Internal;
    o.CorpId = "<企业 CorpId>";
    o.AgentSecret = "<自建应用 secret>";
});

// ② 回调接收 + 回调凭据 + 类型化处理器（处理器在请求 scope 内解析，Transient）。
builder.Services.AddWechatCallback(o =>
{
    o.GlobalRoutePrefix = "wechat";                     // 路由 /wechat/{AppKey}
    o.Apps["default"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 URL 验证 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "<企业 CorpId>",                    // 自建应用回调：静态 ReceiveId = CorpId
        AppType = WechatAppType.Internal,
        Channel = WechatCallbackChannel.App,            // 通讯录变更走应用数据回调通道
    };
})
.AddHandler<CreateUserHandler>()
.AddHandler<UpdateUserHandler>()
.AddHandler<DeleteUserHandler>()
.AddHandler<CreatePartyHandler>()
.AddHandler<UpdatePartyHandler>()
.AddHandler<DeletePartyHandler>()
.AddHandler<UpdateTagHandler>();

var app = builder.Build();

// ③ 回调中间件：GET 走 URL 验证（echostr 回显），POST 走验签 + 解密 + 分发。
app.UseWechatWebhook();

app.Run();