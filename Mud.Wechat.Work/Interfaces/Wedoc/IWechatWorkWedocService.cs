// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「文档」模块管理文档域公共 SDK
/// （新建文档 + 重命名文档 + 删除文档 + 获取文档基础信息 + 分享文档）。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocService"/>。
/// </para>
/// <para>管理文档内容族见 <see cref="IWechatWorkWedocDocumentService"/>；
/// 管理表格内容族见 <see cref="IWechatWorkWedocSpreadsheetService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与代开发自建应用需具有「文档」权限。文档相关操作仅可作用于该应用自己创建的文档/收集表。</para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocService
{
    /// <summary>
    /// 新建文档
    /// <para>该接口用于新建文档、表格、智能表格及智能文档（doc_type 3/4/10/11）；
    /// 新建收集表需使用收集表管理相关接口。</para>
    /// <para>官方业务限制：doc_name 最多 255 个字符（超出会被截断）；
    /// 若指定 spaceid 则 fatherid 需同时指定；新建文档的 docid 仅创建时返回，需自行妥善保存。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateWedocDocumentRequest"/>：spaceid / fatherid / doc_type / doc_name / admin_users）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建文档的访问链接与 docid（url / docid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97460"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97464"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97470"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/create_doc")]
    Task<CreateWedocDocumentResponse> CreateDocumentAsync(
        [Body] CreateWedocDocumentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重命名文档
    /// <para>该接口用于重命名指定文档或收集表；仅可修改应用自己创建的文档/收集表。</para>
    /// <para>官方业务限制：docid 与 formid 只能填其中一个；
    /// new_name 最多 255 个字符（英文算 1 个，汉字算 2 个，超过会被截断）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="RenameWedocDocumentRequest"/>：docid / formid / new_name）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97736"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97745"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97740"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/rename_doc")]
    Task<WechatWorkResponse> RenameDocumentAsync(
        [Body] RenameWedocDocumentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除文档
    /// <para>该接口用于删除指定文档、表格、智能表格及收集表；仅可删除应用自己创建的文档/收集表。</para>
    /// <para>官方业务限制：docid 与 formid 只能填其中一个。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteWedocDocumentRequest"/>：docid / formid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97735"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97746"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97742"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/del_doc")]
    Task<WechatWorkResponse> DeleteDocumentAsync(
        [Body] DeleteWedocDocumentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文档基础信息
    /// <para>该接口用于获取指定文档的基础信息（docid / 文档名 / 创建时间 / 最后修改时间 / 文档类型）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocDocumentBaseInfoRequest"/>：docid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文档基础信息（doc_base_info）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97734"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97747"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97743"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/get_doc_base_info")]
    Task<GetWedocDocumentBaseInfoResponse> GetDocumentBaseInfoAsync(
        [Body] GetWedocDocumentBaseInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分享文档
    /// <para>该接口用于获取文档、表格、智能表格及收集表的分享链接。</para>
    /// <para>官方业务限制：docid 与 formid 只能填其中一个；只能访问该应用创建的文档。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ShareWedocDocumentRequest"/>：docid / formid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文档分享链接（share_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97733"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97748"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97744"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/doc_share")]
    Task<ShareWedocDocumentResponse> ShareDocumentAsync(
        [Body] ShareWedocDocumentRequest request,
        CancellationToken cancellationToken = default);
}
