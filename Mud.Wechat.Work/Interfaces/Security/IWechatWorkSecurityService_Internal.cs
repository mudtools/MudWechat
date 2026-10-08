// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Security;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「安全管理」域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （文件防泄漏操作记录 / 可信设备导入与查询确认 / 截屏录屏操作记录 / 企业微信域名 IP 信息），全部声明于本接口。
/// <para>第三方应用与服务商代开发官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 文件防泄漏需配置到「文件防泄漏 - 可调用接口的应用」；设备管理需配置到
/// 「安全与管理 - 设备管理 - 可调用接口的应用」；截屏/录屏管理需配置到「截屏/录屏管理 - 可调用接口的应用」；
/// 域名 IP 信息需在「我的企业 - 设置 - 域名IP - 可调用API的应用」中配置。
/// 自 2023-12-01 0 点起不再支持通过系统应用 secret 调用；应用可见范围外用户的数据会被过滤。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Security",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSecurityService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalSecurityService : IWechatWorkSecurityService
{
    /// <summary>
    /// 查询文件操作记录
    /// <para>查询启用「文件防泄漏」企业的文件上传、下载、转发等操作记录；已产生的操作记录永久保存。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetFileOperRecordRequest"/>；时间跨度不超过 14 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件操作记录分页结果（has_more / next_cursor / record_list）。</returns>
    /// <remarks>
    /// <para>操作者单次最多 100 个 userid，limit 最多 1000；操作类型/来源见 <see cref="FileOperRecordOperation"/>。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98079"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/get_file_oper_record")]
    Task<GetFileOperRecordResponse> GetFileOperRecordAsync(
        [Body] GetFileOperRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入可信企业设备
    /// <para>向「设备管理」导入可信企业设备（Windows / Mac）；每次调用最多导入 100 条记录。</para>
    /// </summary>
    /// <param name="request">导入请求体（<see cref="ImportTrustDevicesRequest"/>；Windows 必填 mac_addr，Mac 必填 seq_no）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐设备导入结果（result：device_index / device_code / duplicated_device_code / status）。</returns>
    /// <remarks>
    /// <para>status：1 成功、2 重复导入、3 不支持的设备、4 数据格式错误。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98920"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/trustdevice/import")]
    Task<ImportTrustDevicesResponse> ImportTrustDevicesAsync(
        [Body] ImportTrustDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备信息
    /// <para>按设备类型分页查询设备列表（可信企业设备 / 未知设备 / 可信个人设备）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetTrustDevicesRequest"/>；type 必填，limit 最高 100 默认 100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备列表分页结果（device_list / next_cursor；未确认设备的 MAC 等信息返回脱敏数据）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98920"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/trustdevice/list")]
    Task<GetTrustDevicesResponse> GetTrustDevicesAsync(
        [Body] GetTrustDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员使用设备
    /// <para>按最后登录成员查询其设备列表（可信企业设备 / 未知设备 / 可信个人设备）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetTrustDevicesByUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备列表（device_list，与「获取设备信息」的设备结构一致）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98920"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/trustdevice/get_by_user")]
    Task<GetTrustDevicesByUserResponse> GetTrustDevicesByUserAsync(
        [Body] GetTrustDevicesByUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除设备信息
    /// <para>删除指定类型的设备记录；每次调用最多删除 100 个设备。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteTrustDevicesRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98920"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/trustdevice/delete")]
    Task<WechatWorkResponse> DeleteTrustDevicesAsync(
        [Body] DeleteTrustDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 确认为可信设备
    /// <para>管理员确认归属申请；仅可确认状态为待管理员确认（status 3/4）的设备，每次最多 100 个。</para>
    /// </summary>
    /// <param name="request">确认请求体（<see cref="ApproveTrustDevicesRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐设备确认结果（success_list / fail_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98920"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/trustdevice/approve")]
    Task<ApproveTrustDevicesResponse> ApproveTrustDevicesAsync(
        [Body] ApproveTrustDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 驳回可信设备申请
    /// <para>管理员驳回归属申请；仅可驳回状态为待管理员确认（status 3/4）的设备，每次最多 100 个。</para>
    /// </summary>
    /// <param name="request">驳回请求体（<see cref="RejectTrustDevicesRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐设备驳回结果（success_list / fail_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98920"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/trustdevice/reject")]
    Task<RejectTrustDevicesResponse> RejectTrustDevicesAsync(
        [Body] RejectTrustDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取截屏操作记录
    /// <para>查询启用「截屏/录屏管理」企业的成员截屏/录屏操作记录。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetScreenOperRecordRequest"/>；时间跨度不超过 14 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>截屏/录屏操作记录分页结果（has_more / next_cursor / record_list）。</returns>
    /// <remarks>
    /// <para>操作者单次最多 100 个 userid、100 个部门，limit 最多 1000；可见范围外用户数据被过滤。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100128"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/get_screen_oper_record")]
    Task<GetScreenOperRecordResponse> GetScreenOperRecordAsync(
        [Body] GetScreenOperRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业微信域名IP信息
    /// <para>企业微信为 SaaS 服务，域名和 IP 可能变动；本接口返回客户端需要访问的最新域名与 IP 清单。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>域名与 IP 清单（domain_list / ip_list；is_necessary=1 的域名或 IP 被拦截会导致功能异常）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100079"/></para>
    /// </remarks>
    [Get("/cgi-bin/security/get_server_domain_ip")]
    Task<GetServerDomainIpResponse> GetServerDomainIpAsync(
        CancellationToken cancellationToken = default);
}
