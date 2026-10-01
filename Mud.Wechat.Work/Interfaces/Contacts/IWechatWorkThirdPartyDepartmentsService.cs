// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.DataModels.Contacts.Department.RequestModel;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「部门管理」域第三方应用（Suite）SDK：除继承自 <see cref="IWechatWorkDepartmentsService"/> 的公共读取端点外，
/// 本接口提供第三方应用的通讯录写入端点（创建/更新/删除部门，须第三方通讯录应用）。
/// <para>自建应用见 <see cref="IWechatWorkInternalDepartmentsService"/>；服务商代开发见 <see cref="IWechatWorkProviderDepartmentsService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（经 <c>get_corp_token</c> 以 <c>permanent_code</c> 换取，路由键
/// <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId）——调用前须经
/// <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Contact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkDepartmentsService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyDepartmentsService : IWechatWorkDepartmentsService
{
    /// <summary>
    /// 创建部门
    /// <para>向授权企业通讯录写入新部门。name 与 parentid 必填；未指定 id 时由官方自动生成。</para>
    /// <para>官方权限口径：「第三方仅通讯录应用可以调用」。与自建应用版本（<see cref="IWechatWorkInternalDepartmentsService.CreateDepartmentAsync"/>）
    /// 路由相同、契约一致，但令牌作用域不同（授权企业级），故按应用类型分别声明。</para>
    /// </summary>
    /// <param name="request">部门请求体（<see cref="CreateDepartmentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（含新建部门的 ID）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90341"/></para>
    /// </remarks>
    [Post("/cgi-bin/department/create")]
    Task<CreateDepartmentResponse> CreateDepartmentAsync(
        [Body] CreateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新部门
    /// <para>更新授权企业通讯录中的既有部门。须拥有指定部门的管理权限；移动部门还需新父部门的管理权限。</para>
    /// <para>官方权限口径：「第三方仅通讯录应用可以调用」。</para>
    /// </summary>
    /// <param name="request">部门请求体（<see cref="UpdateDepartmentRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90342"/></para>
    /// </remarks>
    [Post("/cgi-bin/department/update")]
    Task<WechatWorkResponse> UpdateDepartmentAsync(
        [Body] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除部门
    /// <para>从授权企业通讯录中删除部门。须拥有指定部门的管理权限。</para>
    /// <para>不能删除根部门；不能删除含有子部门、成员的部门。</para>
    /// </summary>
    /// <param name="id">部门 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90343"/></para>
    /// </remarks>
    [Get("/cgi-bin/department/delete")]
    Task<WechatWorkResponse> DeleteDepartmentAsync(
        [Query("id")] int id,
        CancellationToken cancellationToken = default);
}
