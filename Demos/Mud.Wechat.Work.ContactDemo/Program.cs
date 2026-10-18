// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.Interfaces;

// 企业微信通讯录（联系人）功能 Demo：演示「多应用底座注册 → 通讯录模块注册 → 自建子接口注入 → 端点调用」的最小闭环。
// 说明：
//   1) 父接口（IWechatWorkUsersService 等）IsAbstract=true 不注册，须注入自建子接口
//      （IWechatWorkInternalUsersService / IWechatWorkInternalDepartmentsService / IWechatWorkInternalTagsService）。
//   2) 成员 DTO 命名空间为 Mud.Wechat.Work.DataModels.Contracts.Users（唯一例外，与目录名 Contacts 不一致）。
//   3) 凭据一律来自环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md），仓库内不保留任何真实密钥。
//   4) 通讯录写入需要「通讯录同步」权限的 secret，且创建成员默认关闭（写操作闸）。

// ① 装配前先校验凭据：缺项一次性列出全部缺失变量，避免带着占位符启动后在首次调用才失败。
var env = DemoEnv.RequireAll("WORK_CORP_ID", "WORK_AGENT_SECRET");

var builder = WebApplication.CreateBuilder(args);

// ② 注册企业微信应用（自建应用：corpid + corpsecret 换取 access_token）。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "default";               // AppKey == "default" 自动推断为默认应用
    o.AppType = WechatAppType.Internal;
    o.CorpId = env["WORK_CORP_ID"];
    o.AgentSecret = env["WORK_AGENT_SECRET"];
});

// ③ 注册通讯录模块（成员/部门/标签/查看权限/异步导入/异步导出六域）。
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

var app = builder.Build();

// 令牌链等业务错误以异常承载（区别于生成 JSON 端点的「HTTP 200 + errcode」）⇒ 统一映射 502 + 错误码。
app.UseWechatApiErrorGate();

// ④ 健康检查：只回显配置摘要与写闸状态，不含任何凭据。
app.MapDemoHealth("work-contact", new
{
    appKey = "default",
    appType = WechatAppType.Internal.ToString(),
    modules = new[] { "Contact" },
});

// ⑤ 部门：获取部门列表（默认全量组织架构）。
app.MapGet("/api/departments",
    (IWechatWorkInternalDepartmentsService departments, CancellationToken ct) =>
        departments.GetDepartmentListAsync(id: null, cancellationToken: ct));

// ⑥ 成员：按 userid 读取成员详情。
app.MapGet("/api/members/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));

// ⑦ 成员：获取某部门的成员摘要（fetch_child=1 递归子部门）。
app.MapGet("/api/members",
    (IWechatWorkInternalUsersService users, int departmentId, CancellationToken ct) =>
        users.GetUserSimpleListAsync(departmentId, fetchChild: 1, cancellationToken: ct));

// ⑧ 标签：获取标签列表。
app.MapGet("/api/tags",
    (IWechatWorkInternalTagsService tags, CancellationToken ct) =>
        tags.GetTagListAsync(ct));

// ⑨ 成员：创建成员（写操作，默认不映射 —— 请求得到 404；设置 MUDWECHAT_DEMO_ALLOW_WRITE=1 后启用）。
if (DemoWriteGate.Enabled)
{
    app.MapPost("/api/members",
        (IWechatWorkInternalUsersService users, CreateUserRequest request, CancellationToken ct) =>
            users.CreateUserAsync(request, ct));
}

DemoWriteGate.Announce(app.Logger);

app.Run();
