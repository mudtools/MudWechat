// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「邮件」模块管理公共邮箱族企业自建应用 SDK
/// （创建公共邮箱 + 更新公共邮箱 + 删除公共邮箱 + 获取公共邮箱详情 + 模糊搜索公共邮箱 +
/// 获取客户端专用密码列表 + 删除客户端专用密码）。
/// <para>
/// 官方仅向企业自建应用开放本域 7 个端点，全部收敛声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalSecurityVipService"/> 仅自建子接口承载）。
/// 官方未向第三方应用与服务商代开发应用开放本域，故本家族不声明对应应用类型子接口
/// （能力漂移守卫：继承链上恰好只有自建子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 应用调用接口只能访问自身创建的公共邮箱。
/// </para>
/// <para>
/// 客户端专用密码业务限制：一个公共邮箱通过 API 接口创建的客户端专用密码不能超过 10 个
/// （不包括已删除的）；密码仅会在创建时返回一次，请妥善存储。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Mail",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMailPublicMailService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMailPublicMailService : IWechatWorkMailPublicMailService
{
    /// <summary>
    /// 创建公共邮箱
    /// <para>创建一个公共邮箱（业务邮箱），使用成员由成员、部门与标签共同组成；
    /// 可选创建客户端专用密码。</para>
    /// <para>官方业务限制：公共邮箱名称不多于 64 个字符或 32 个汉字，不得与其他公共邮箱重名；
    /// userid_list、department_list、tag_list 不能同时为空。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="CreatePublicMailRequest"/>：email / name 官方必填，成员列表与密码选项官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>公共邮箱 ID（id）与可选的客户端专用密码信息（auth_code_id / auth_code，仅当设置创建密码时返回，密码仅返回一次）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95511"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/publicmail/create")]
    Task<CreatePublicMailResponse> CreatePublicMailAsync(
        [Body] CreatePublicMailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新公共邮箱
    /// <para>更新指定的公共邮箱；列表类型字段不传则保持不变，传空结构或空数组则清空。</para>
    /// <para>官方业务限制：别名长度 6~64 个字节且为有效的企业邮箱格式，企业内必须唯一，
    /// 最多可设置 5 个别名，更新时为覆盖式更新；
    /// userid_list、department_list、tag_list 不能同时为空（使用成员不允许全部清空）。</para>
    /// </summary>
    /// <param name="request">更新请求体（<see cref="UpdatePublicMailRequest"/>：id 官方必填，其余字段官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>可选的客户端专用密码信息（auth_code_id / auth_code，仅当设置创建密码时返回，密码仅返回一次）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98000"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/publicmail/update")]
    Task<UpdatePublicMailResponse> UpdatePublicMailAsync(
        [Body] UpdatePublicMailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除公共邮箱
    /// <para>删除已有的公共邮箱。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeletePublicMailRequest"/>：id 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98001"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/publicmail/delete")]
    Task<WechatWorkResponse> DeletePublicMailAsync(
        [Body] DeletePublicMailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取公共邮箱详情
    /// <para>批量获取公共邮箱详细信息，包含公共邮箱名称、权限信息。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPublicMailRequest"/>：id_list 公共邮箱 ID 列表，官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>公共邮箱详情列表（list：id / email / name / userid_list / department_list / tag_list / alias_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98002"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/publicmail/get")]
    Task<GetPublicMailResponse> GetPublicMailAsync(
        [Body] GetPublicMailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 模糊搜索公共邮箱（官方即 GET，fuzzy / email 走 Query）
    /// <para>模糊搜索公共邮箱，可匹配邮箱名也可匹配邮箱地址；fuzzy 传 0 时获取全部公共邮箱。</para>
    /// </summary>
    /// <param name="fuzzy">是否模糊搜索（官方必填）：1 - 开启模糊搜索，0 - 获取全部公共邮箱。</param>
    /// <param name="email">公共邮箱名称或邮箱地址（官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>公共邮箱简要信息列表（list：id / email / name）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98003"/></para>
    /// </remarks>
    [Get("/cgi-bin/exmail/publicmail/search")]
    Task<SearchPublicMailResponse> SearchPublicMailAsync(
        [Query("fuzzy")] long fuzzy,
        [Query("email")] string? email = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户端专用密码列表
    /// <para>获取公共邮箱创建的客户端专用密码列表（含 ID、创建时间、最后使用时间、备注）。</para>
    /// <para>官方注明：客户端专用密码仅会在创建时候返回，本接口不返回密码本身。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPublicMailAuthCodeListRequest"/>：id 公共邮箱 ID，官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户端专用密码列表（auth_code_list：auth_code_id / create_time / last_use_time / remark）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100183"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/publicmail/get_auth_code_list")]
    Task<GetPublicMailAuthCodeListResponse> GetPublicMailAuthCodeListAsync(
        [Body] GetPublicMailAuthCodeListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除客户端专用密码
    /// <para>删除公共邮箱的客户端专用密码。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeletePublicMailAuthCodeRequest"/>：id / auth_code_id 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100184"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/publicmail/delete_auth_code")]
    Task<WechatWorkResponse> DeletePublicMailAuthCodeAsync(
        [Body] DeletePublicMailAuthCodeRequest request,
        CancellationToken cancellationToken = default);
}
