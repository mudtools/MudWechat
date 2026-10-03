// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.DataModels.Contracts.Users;

// 企业微信通讯录（联系人）功能 Demo：
// 演示「多应用底座注册 → 通讯录模块注册 → 自建子接口注入 → 端点调用」的最小闭环。
// 说明：
//   1) 父接口（IWechatWorkUsersService 等）IsAbstract=true 不注册，须注入自建子接口
//      （IWechatWorkInternalUsersService / IWechatWorkInternalDepartmentsService / IWechatWorkInternalTagsService）。
//   2) 成员 DTO 命名空间为 Mud.Wechat.Work.DataModels.Contracts.Users（唯一例外，与目录名 Contacts 不一致）。
//   3) 下方 <企业 CorpId> / <自建应用 secret> 为占位符，运行前替换；
//      正式环境建议从环境变量 / 用户机密（User Secrets）注入，勿提交密钥。

var builder = WebApplication.CreateBuilder(args);

// ① 注册企业微信应用（自建应用：corpid + corpsecret 换取 access_token）。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "default";               // AppKey == "default" 自动推断为默认应用
    o.AppType = WechatAppType.Internal;
    o.CorpId = "<企业 CorpId>";
    o.AgentSecret = "<自建应用 secret>";
});

// ② 注册通讯录模块（成员/部门/标签/查看权限/异步导入/异步导出六域）。
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

var app = builder.Build();

// ③ 部门：获取部门列表（默认全量组织架构）。
app.MapGet("/api/departments",
    (IWechatWorkInternalDepartmentsService departments, CancellationToken ct) =>
        departments.GetDepartmentListAsync(id: null, cancellationToken: ct));

// ④ 成员：按 userid 读取成员详情。
app.MapGet("/api/members/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));

// ⑤ 成员：获取某部门的成员摘要（fetch_child=1 递归子部门）。
app.MapGet("/api/members",
    (IWechatWorkInternalUsersService users, int departmentId, CancellationToken ct) =>
        users.GetUserSimpleListAsync(departmentId, fetchChild: 1, cancellationToken: ct));

// ⑥ 成员：创建成员（最小字段：userid + name）。
app.MapPost("/api/members",
    (IWechatWorkInternalUsersService users, CreateUserRequest request, CancellationToken ct) =>
        users.CreateUserAsync(request, ct));

// ⑦ 标签：获取标签列表。
app.MapGet("/api/tags",
    (IWechatWorkInternalTagsService tags, CancellationToken ct) =>
        tags.GetTagListAsync(ct));

app.Run();