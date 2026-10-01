// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Department;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「部门管理」域公共 SDK（自建应用 / 第三方应用 / 服务商代开发三类应用均可调用的读取面端点）。
/// <para>
/// 通讯录写入端点（创建/更新/删除部门）声明于派生接口：自建应用见 <see cref="IWechatWorkInternalDepartmentsService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyDepartmentsService"/>，服务商代开发见 <see cref="IWechatWorkProviderDepartmentsService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkUsersService"/>：三类应用消费的令牌路由键均为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkDepartmentsService
{
    /// <summary>
    /// 获取部门列表
    /// <para>获取指定部门及其下所有子孙部门（递归）或全量组织架构；只能拉取 token 对应应用的权限范围内的部门。</para>
    /// <para>官方提示该接口性能较低，建议改用获取子部门 ID 列表与获取单个部门详情；自 2022-08-15 起「通讯录同步」新增 IP 不能再调用此接口。</para>
    /// </summary>
    /// <param name="id">部门 ID，获取该部门及其下所有子孙部门（递归）；不填则默认获取全量组织架构。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>部门列表（department；第三方不可获取 name/name_en）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90208"/></remarks>
    [Get("/cgi-bin/department/list")]
    Task<GetDepartmentListResponse> GetDepartmentListAsync(
        [Query("id")] int? id = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取子部门 ID 列表
    /// <para>获取指定部门及其下的子部门（递归）的 ID 与父部门 ID 列表；2022-08 通讯录安全加固后的官方推荐替代接口。</para>
    /// <para>普通自建应用只能拉取 token 对应应用的权限范围内的部门；第三方通讯录应用可获取企业所有部门 ID。</para>
    /// </summary>
    /// <param name="id">部门 ID，获取该部门及其下的子部门（递归）；不填则默认获取全量组织架构。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>部门 ID 列表（department_id）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95350"/></remarks>
    [Get("/cgi-bin/department/simplelist")]
    Task<GetChildDepartmentIdListResponse> GetChildDepartmentIdListAsync(
        [Query("id")] int? id = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取单个部门详情
    /// <para>获取指定部门详情（含父部门 ID 与部门负责人）。普通自建应用只能拉取权限范围内的部门详情；
    /// 第三方通讯录应用可获取企业所有部门详情（部门名字除外）。</para>
    /// </summary>
    /// <param name="id">部门 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>部门详情（department）。</returns>
    /// <remarks>请参照原SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95351"/></remarks>
    [Get("/cgi-bin/department/get")]
    Task<GetDepartmentResponse> GetDepartmentAsync(
        [Query("id")] int id,
        CancellationToken cancellationToken = default);
}
